using ChessMAUI.Models;
using ChessMAUI.Services;
using ChessMAUI.ViewModels;

namespace ChessMAUI.Views;

public partial class GamePage : ContentPage
{
    private readonly GameViewModel _vm;

    private CancellationTokenSource? _chatCts;
    private double _squareSize;

    private int _selectedDiff = 0;
    private static readonly int[]    DiffDepths  = [1, 3, 5];
    private static readonly string[] DiffLabels  = ["Fácil", "Médio", "Difícil"];

    public GamePage()
    {
        InitializeComponent();
        _vm = new GameViewModel();
        BindingContext = _vm;

        _vm.PromotionRequested  += OnPromotionRequested;
        _vm.ChatMessageReceived += OnChatMessageReceived;
        _vm.TournamentGameEnded += OnTournamentGameEnded;
        _vm.ResignRequested     += OnResignRequested;
        _vm.DrawOfferRequested  += OnDrawOfferRequested;
        _vm.PropertyChanged     += OnVmPropertyChanged;
        _vm.RequestHandoff      += ShowHandoffOverlay;

        _selectedDiff = Preferences.Default.Get("AiDifficulty", 0);
        BuildBoard();
        BoardThemeService.ThemeChanged += OnThemeChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AdminBar.IsVisible = AppState.Current.IsAdminMode;

        SelectDiff(_selectedDiff);


        var state = AppState.Current;

        // Consome a flag UMA ÚNICA VEZ — evita reiniciar o jogo em cada OnAppearing
        if (state.PendingCareerGame)
        {
            state.PendingCareerGame  = false;
            SetupPanel.IsVisible     = false;
            ResultPanel.IsVisible    = false;
            Title = "Modo Carreira";
            _vm.StartTournamentGame(
                state.CareerOpponentName,
                state.CareerTimeMinutes,
                state.CareerAIDepth);
        }
        else if (state.PendingTournamentGame)
        {
            state.PendingTournamentGame = false;
            SetupPanel.IsVisible        = false;
            ResultPanel.IsVisible       = false;
            Title = $"vs {state.TournamentOpponentName}";
            _vm.StartTournamentGame(
                state.TournamentOpponentName,
                state.TournamentTimeMinutes,
                state.TournamentAIDepth);
        }
        else if (state.PendingOnlineGame)
        {
            state.PendingOnlineGame = false;
            SetupPanel.IsVisible    = false;
            ResultPanel.IsVisible   = false;
            Title = $"vs {state.OnlineOpponentName}";
            string myName  = AppState.Current.Profile.Name;
            string oppName = state.OnlineOpponentName;
            WhitePlayerLabel.Text = state.OnlinePlayerIsWhite
                ? $"♙ {myName} (Brancas)"
                : $"♙ {oppName} (Brancas)";
            BlackPlayerLabel.Text = state.OnlinePlayerIsWhite
                ? $"♟ {oppName} (Pretas)"
                : $"♟ {myName} (Pretas)";
            _vm.StartTournamentGame(oppName, state.OnlineTimeMinutes, 3);
        }
        else if (state.PendingFriendGame)
        {
            state.PendingFriendGame = false;
            string p1 = string.IsNullOrWhiteSpace(state.FriendPlayer1Name) ? "Jogador 1" : state.FriendPlayer1Name;
            string p2 = string.IsNullOrWhiteSpace(state.FriendOpponentName) ? "Jogador 2" : state.FriendOpponentName;
            Title = $"{p1} vs {p2}";
            _vm.StartFriendGame(p1, p2, state.FriendTimeMinutes);
            WhitePlayerLabel.Text = $"♙ {p1} (Brancas)";
            BlackPlayerLabel.Text = $"♟ {p2} (Pretas)";
            SetupPanel.IsVisible   = false;
            HandoffPanel.IsVisible = false;
        }
        else if (!_vm.IsTournamentMode && !_vm.IsFriendMode && _vm.GameOver)
        {
            ResultPanel.IsVisible = false;
            SetupPanel.IsVisible  = true;
            SelectDiff(_selectedDiff);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        var state = AppState.Current;

        // Fallback para carreira: captura resultado real mesmo que TournamentGameEnded não tenha disparado
        if (state.IsCareerGame && _vm.GameOver && !state.MatchResultReady)
        {
            bool isDraw = _vm.StatusMessage.Contains("Empate") || _vm.StatusMessage.Contains("Afogamento");
            state.LastMatchHumanWon = _vm.HumanWon == true;
            state.LastMatchWasDraw  = isDraw;
            state.MatchResultReady  = true;
        }
        // Fallback original para torneios de bracket
        else if (_vm.IsTournamentMode && _vm.GameOver && !state.MatchResultReady)
        {
            state.MatchResultReady = true;
        }
    }

    // -----------------------------------------------------------------------
    // Torneio — navega automaticamente ao fim da partida
    // -----------------------------------------------------------------------
    private void OnTournamentGameEnded(bool humanWon)
    {
        var state = AppState.Current;
        state.Daily.RecordGamePlayed();

        bool isDraw = _vm.StatusMessage.Contains("Empate") || _vm.StatusMessage.Contains("Afogamento");
        int  eloDelta = 0;

        if (state.IsCareerGame)
        {
            state.LastMatchHumanWon = humanWon;
            state.LastMatchWasDraw  = isDraw;
            state.MatchResultReady  = true;
            int oppElo = ProfileService.EloRatingForAI(state.CareerAIDepth);
            eloDelta   = state.Profile.UpdateElo(oppElo, humanWon, isDraw);
            if (humanWon)      state.Profile.RecordWin();
            else if (!isDraw)  state.Profile.RecordLoss();
        }
        else if (state.IsOnlineGame)
        {
            int oppElo = state.OnlineMatch.State.OpponentRating;
            eloDelta   = state.Profile.UpdateElo(oppElo, humanWon, isDraw);
            if (humanWon)      state.Profile.RecordWin();
            else if (!isDraw)  state.Profile.RecordLoss();
        }

        bool   hasNextRound = false;
        int    nextRoundNum = 1;
        string nextBtnText  = "Próxima";
        if (state.IsCareerGame)
        {
            var prog = state.Career.Progress;
            var t    = prog.ActiveTournament;
            if (t != null)
            {
                nextRoundNum = t.CurrentRound + 1;
                hasNextRound = t.Format switch
                {
                    CareerFormat.Swiss => t.CurrentRound < t.TotalRounds,
                    // Elimination: only continue if human won (loss = eliminated)
                    CareerFormat.Elimination => humanWon && !isDraw && t.CurrentRound < t.TotalRounds,
                    // BestOfN: continue if neither player reached win threshold
                    CareerFormat.BestOfN =>
                        (t.HumanWins   + (humanWon && !isDraw ? 1 : 0)) < t.WinsNeeded &&
                        (t.HumanLosses + (!humanWon && !isDraw ? 1 : 0)) < t.WinsNeeded &&
                        t.CurrentRound < t.TotalRounds,
                    _ => false
                };
                nextBtnText = t.Format switch
                {
                    CareerFormat.Elimination => nextRoundNum == 3 ? "Jogar a Final" : $"Fase {nextRoundNum}",
                    CareerFormat.BestOfN     => $"Partida {nextRoundNum}",
                    _                        => $"Rodada {nextRoundNum}"
                };
            }
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            ResultTitle.Text      = humanWon ? "Vitória!" : isDraw ? "Empate" : "Derrota";
            ResultTitle.TextColor = humanWon ? Color.FromArgb("#4CAF50")
                                 : isDraw    ? Color.FromArgb("#FFD700")
                                 : Color.FromArgb("#FF5252");
            string eloSign2 = eloDelta >= 0 ? "+" : "";
            ResultDetail.Text = $"{_vm.StatusMessage}\n\nRating: {eloSign2}{eloDelta}  →  {state.Profile.Points}";
            if (state.IsCareerGame)
            {
                ResultActionBtn.IsVisible      = hasNextRound;
                ResultActionBtn.Text           = nextBtnText;
                ResultSecondaryLabel.IsVisible = true;
                ResultSecondaryLabel.Text      = "Ver resultado da fase";
            }
            else if (state.IsOnlineGame)
            {
                ResultActionBtn.IsVisible      = true;
                ResultActionBtn.Text           = "← Voltar ao lobby";
                ResultSecondaryLabel.IsVisible = false;
            }
            else
            {
                ResultActionBtn.IsVisible  = true;
                ResultActionBtn.Text       = "Voltar ao Torneio";
                ResultSecondaryLabel.Text  = "← Voltar ao lobby";
            }
            ResultPanel.IsVisible = true;
        });
    }

    // -----------------------------------------------------------------------
    // Missões: registra partidas casuais
    // -----------------------------------------------------------------------
    private void OnVmPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(_vm.GameOver) || !_vm.GameOver) return;
        if (_vm.IsTournamentMode) return;

        if (_vm.IsFriendMode)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HandoffPanel.IsVisible = false;
                bool isDraw   = _vm.StatusMessage.Contains("Empate") || _vm.StatusMessage.Contains("Afogamento");
                bool whiteWon = _vm.StatusMessage.Contains(_vm.WhitePlayerName) && _vm.StatusMessage.Contains("vence");
                ResultTitle.Text       = isDraw ? "Empate" : whiteWon ? $"{_vm.WhitePlayerName} vence!" : $"{_vm.BlackPlayerName} vence!";
                ResultTitle.TextColor  = isDraw ? Color.FromArgb("#FFD700") : Color.FromArgb("#4CAF50");
                ResultDetail.Text      = _vm.StatusMessage;
                ResultActionBtn.Text   = "← Voltar ao lobby";
                ResultSecondaryLabel.IsVisible = false;
                ResultPanel.IsVisible  = true;
            });
            return;
        }

        // HumanWon é setado pelo admin (ForceWin/ForceLoss); senão, lê o StatusMessage
        bool humanWon = _vm.HumanWon.HasValue
            ? _vm.HumanWon == true
            : _vm.StatusMessage.Contains("Brancas vencem");

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var state = AppState.Current;

            // Registra W/L e atualiza Elo
            bool isDraw2 = _vm.StatusMessage.Contains("Empate") || _vm.StatusMessage.Contains("Afogamento");
            if (humanWon)     state.Profile.RecordWin();
            else if (!isDraw2) state.Profile.RecordLoss();

            int aiDepth   = DiffDepths[_selectedDiff];
            int oppElo    = ProfileService.EloRatingForAI(aiDepth);
            int eloChange = state.Profile.UpdateElo(oppElo, humanWon, isDraw2);

            // Missão 1 — jogar 3 partidas (dá fichas por engajamento, não por vencer)
            bool m1Done = state.Daily.RecordGamePlayed();
            if (m1Done)
            {
                var m = state.Daily.GetMissions()[0];
                state.Profile.Credit(m.BalanceReward, "Missão – 3 partidas jogadas", "♟");
                await DisplayAlert("Missão Completa", $"+{m.BalanceReward} fichas por jogar 3 partidas!", "OK");
            }

            // Anúncio intersticial (apenas usuários gratuitos, máx 2/dia)
            if (state.Ads.ShouldShowAd(state.Subscription))
                await state.Ads.SimulateInterstitialAsync(this);

            // Painel de resultado
            bool isDraw = isDraw2;
            ResultTitle.Text      = humanWon ? "Vitória!" : isDraw ? "Empate" : "Derrota";
            ResultTitle.TextColor = humanWon
                ? Color.FromArgb("#4CAF50")
                : isDraw ? Color.FromArgb("#FFD700") : Color.FromArgb("#FF5252");
            string eloSign = eloChange >= 0 ? "+" : "";
            ResultDetail.Text              = $"{_vm.StatusMessage}\n\nRating: {eloSign}{eloChange}  →  {state.Profile.Points}";
            ResultActionBtn.Text           = "Novo Jogo";
            ResultSecondaryLabel.IsVisible = true;
            ResultSecondaryLabel.Text      = "← Voltar ao lobby";
            ResultPanel.IsVisible          = true;
        });
    }

    // -----------------------------------------------------------------------
    // Handoff pass-and-play — exibe overlay entre turnos
    // -----------------------------------------------------------------------
    private void ShowHandoffOverlay(string nextPlayerName)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            HandoffLabel.Text      = $"Vez de {nextPlayerName}";
            HandoffPanel.IsVisible = true;
        });
    }

    private void OnHandoffDismissed(object? sender, EventArgs e)
    {
        HandoffPanel.IsVisible = false;
    }

    // -----------------------------------------------------------------------
    // Chat do bot — exibe balão e some após 3 s
    // -----------------------------------------------------------------------
    private void OnChatMessageReceived(string message)
    {
        _chatCts?.Cancel();
        _chatCts = new CancellationTokenSource();
        var token = _chatCts.Token;

        ChatLabel.Text       = $"🤖  {message}";
        ChatBubble.IsVisible = true;

        Task.Run(async () =>
        {
            await Task.Delay(3000, token);
            if (!token.IsCancellationRequested)
                MainThread.BeginInvokeOnMainThread(() => ChatBubble.IsVisible = false);
        }, token);
    }

    // -----------------------------------------------------------------------
    // Admin: forçar resultado
    // -----------------------------------------------------------------------
    private void OnAdminWin(object? sender, EventArgs e)  => _vm.ForceWin();
    private void OnAdminLose(object? sender, EventArgs e) => _vm.ForceLoss();

    // -----------------------------------------------------------------------
    // Confirmação: Desistir
    // -----------------------------------------------------------------------
    private Task<bool> OnResignRequested()
        => DisplayAlert("Desistir", "Tem certeza que quer desistir?", "Sim, desistir", "Cancelar");

    // -----------------------------------------------------------------------
    // Confirmação: Propor empate — simula resposta da IA (~30% aceita)
    // -----------------------------------------------------------------------
    private async Task<bool> OnDrawOfferRequested()
    {
        bool aiAccepts = Random.Shared.NextDouble() < 0.30;
        string msg = aiAccepts
            ? "A IA aceita o empate."
            : "A IA recusa o empate.";
        await DisplayAlert("Proposta de Empate", msg, "OK");
        return aiAccepts;
    }

    private async void OnResultBackTapped(object? sender, TappedEventArgs e)
    {
        ResultPanel.IsVisible = false;
        await Shell.Current.GoToAsync("..");
    }

    private void SelectDiff(int idx)
    {
        _selectedDiff = idx;
        Preferences.Default.Set("AiDifficulty", idx);
        DiffLabel.Text = $"🤖  IA: {DiffLabels[idx]}";
    }

    private async void OnDiffSettingsClicked(object? sender, EventArgs e)
    {
        string? choice = await DisplayActionSheet(
            "Dificuldade da IA", "Cancelar", null, "Fácil", "Médio", "Difícil");
        if (choice == null || choice == "Cancelar") return;
        int idx = Array.IndexOf(DiffLabels, choice);
        if (idx >= 0) SelectDiff(idx);
    }

    private void OnSetupNewGameClicked(object? sender = null, EventArgs? e = null)
    {
        ResultPanel.IsVisible         = false;
        SetupPanel.IsVisible          = false;
        WhitePlayerLabel.Text         = "♙ Você (Brancas)";
        BlackPlayerLabel.Text         = "♟ IA (Pretas)";
        Title                         = "ChessArena";
        AppState.Current.IsCareerGame = false;
        _vm.StartNewGame(0, DiffDepths[_selectedDiff]);
    }

    private async void OnResultActionClicked(object? sender, EventArgs e)
    {
        var state = AppState.Current;

        if (state.IsCareerGame)
        {
            if (!state.MatchResultReady)
            {
                ResultPanel.IsVisible = false;
                await Shell.Current.GoToAsync("..");
                return;
            }

            state.MatchResultReady = false;
            var (hasNext, opp) = state.Career.ProcessRoundResult(
                state.LastMatchHumanWon, state.LastMatchWasDraw, state.CareerOpponentName);

            if (!hasNext || opp == null)
            {
                ResultPanel.IsVisible = false;
                await Shell.Current.GoToAsync("..");
                return;
            }

            state.CareerOpponentName = opp.Name;
            state.CareerAIDepth      = CareerService.GetAIDepth(opp.Difficulty);
            state.CareerTimeMinutes  = CareerService.GetTimeMinutes(opp.Difficulty);

            ResultPanel.IsVisible = false;
            _vm.StartTournamentGame(opp.Name, state.CareerTimeMinutes, state.CareerAIDepth);
            return;
        }

        if (state.IsOnlineGame)
        {
            state.IsOnlineGame    = false;
            ResultPanel.IsVisible = false;
            await Shell.Current.GoToAsync("..");
            return;
        }

        ResultPanel.IsVisible = false;
        if (_vm.IsTournamentMode || _vm.IsFriendMode)
            await Shell.Current.GoToAsync("..");
        else
            OnSetupNewGameClicked();
    }

    // -----------------------------------------------------------------------
    // Botão: Voltar ao Torneio
    // -----------------------------------------------------------------------
    private async void OnReturnToTournamentClicked(object? sender, EventArgs e)
    {
        AppState.Current.MatchResultReady = true;
        Title = "Xadrez";
        await Shell.Current.GoToAsync("..");
    }

    // -----------------------------------------------------------------------
    // Botão: Som — alterna mudo/ativo
    // -----------------------------------------------------------------------
    private void OnSoundToggled(object? sender, EventArgs e)
    {
        _vm.SoundEnabled = !_vm.SoundEnabled;
        SoundBtn.Text    = _vm.SoundEnabled ? "🔊" : "🔇";
    }

    // -----------------------------------------------------------------------
    // Promoção de peão — exibe popup de escolha
    // -----------------------------------------------------------------------
    private async void OnPromotionRequested(string color)
    {
        string title  = "Promover Peão";
        string? choice = await DisplayActionSheet(title, null, null,
            "♛ Rainha", "♜ Torre", "♝ Bispo", "♞ Cavalo");

        string key = choice?.Split(' ')[1].ToLower() switch
        {
            "rainha" => "queen",
            "torre"  => "rook",
            "bispo"  => "bishop",
            "cavalo" => "knight",
            _        => "queen"
        };

        _vm.PromoteCommand.Execute(key);
    }

    // -----------------------------------------------------------------------
    // Configura o GraphicsView do tabuleiro
    // -----------------------------------------------------------------------
    private readonly BoardDrawable _drawable = new();

    private void BuildBoard()
    {
        _drawable.Squares = _vm.Squares;
        BoardView.Drawable = _drawable;

        _vm.BoardChanged += () =>
            MainThread.BeginInvokeOnMainThread(() => BoardView.Invalidate());

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnBoardTapped;
        BoardView.GestureRecognizers.Add(tap);
    }

    private void OnThemeChanged()
    {
        MainThread.BeginInvokeOnMainThread(() => BoardView.Invalidate());
    }

    private async void OnThemePaletteClicked(object? sender, EventArgs e)
    {
        string? choice = await DisplayActionSheet(
            "Tema do tabuleiro", "Cancelar", null,
            BoardThemeService.ThemeLabels);

        if (choice == null || choice == "Cancelar") return;

        int idx = Array.IndexOf(BoardThemeService.ThemeLabels, choice);
        if (idx >= 0)
            BoardThemeService.SetTheme((BoardThemeService.Theme)idx);
    }

    private void OnBoardTapped(object? sender, TappedEventArgs e)
    {
        if (_squareSize <= 0) return;
        var pos = e.GetPosition(BoardView);
        if (pos is null) return;
        int col = Math.Clamp((int)(pos.Value.X / _squareSize), 0, 7);
        int row = Math.Clamp((int)(pos.Value.Y / _squareSize), 0, 7);
        _vm.SquareTappedCommand.Execute(_vm.Squares[row, col]);
    }

    // -----------------------------------------------------------------------
    // Adapta o tamanho do tabuleiro à tela
    // -----------------------------------------------------------------------
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // Espaço fixo consumido pelas linhas ao redor do tabuleiro (timers, botões, etc.)
        // Timers on: relógio IA (~52) + status (~38) + capturas (~26) + relógio jogador (~52) + botões (~52) + lances (~28) + padding (~16) = 264 → 280 com margem
        // Timers off: status (~38) + capturas (~26) + botões (~52) + lances (~28) + padding (~16) = 160 → 180 com margem
        double used      = _vm.TimerVisible ? 280 : 180;
        double available = Math.Min(width - 16, height - used);
        if (available <= 0) return;

        _squareSize = available / 8.0;

        BoardView.WidthRequest  = available;
        BoardView.HeightRequest = available;
        BoardView.Invalidate();
    }
}
