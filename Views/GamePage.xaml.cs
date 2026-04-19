using ChessMAUI.Services;
using ChessMAUI.ViewModels;

namespace ChessMAUI.Views;

public partial class GamePage : ContentPage
{
    private readonly GameViewModel _vm;

    private CancellationTokenSource? _chatCts;
    private double _squareSize;

    private static readonly (string Label, int Depth)[] DifficultyOptions =
    [
        ("Fácil",   1),
        ("Médio",   3),
        ("Difícil", 5),
    ];

    private static readonly (string Label, int Minutes)[] TimeOptions =
    [
        ("Sem limite",  0),
        ("1 minuto",    1),
        ("3 minutos",   3),
        ("5 minutos",   5),
        ("10 minutos", 10),
        ("15 minutos", 15),
        ("30 minutos", 30),
    ];

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

        BuildBoard();
        BoardThemeService.ThemeChanged += OnThemeChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AdminBar.IsVisible = AppState.Current.IsAdminMode;

        var state = AppState.Current;

        // Consome a flag UMA ÚNICA VEZ — evita reiniciar o jogo em cada OnAppearing
        if (state.PendingTournamentGame)
        {
            state.PendingTournamentGame = false;
            Title = $"vs {state.TournamentOpponentName}";
            _vm.StartTournamentGame(
                state.TournamentOpponentName,
                state.TournamentTimeMinutes,
                state.TournamentAIDepth);
        }
        else if (state.PendingFriendGame)
        {
            state.PendingFriendGame = false;
            Title = $"{state.FriendOpponentName} vs Você";
            _vm.StartFriendGame("Você", state.FriendOpponentName, state.FriendTimeMinutes);
            WhitePlayerLabel.Text = "♙ Você (Brancas)";
            BlackPlayerLabel.Text = $"♟ {state.FriendOpponentName} (Pretas)";
            SetupPanel.IsVisible  = false;
            HandoffPanel.IsVisible = false;
        }
        else if (!_vm.IsTournamentMode && !_vm.IsFriendMode && _vm.GameOver)
        {
            // Abre os menus de configuração automaticamente
            OnSetupNewGameClicked();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Fallback: se o jogo terminou sem navegar automaticamente (ex: back do sistema)
        if (_vm.IsTournamentMode && _vm.GameOver && !AppState.Current.MatchResultReady)
            AppState.Current.MatchResultReady = true;
    }

    // -----------------------------------------------------------------------
    // Torneio — navega automaticamente ao fim da partida
    // -----------------------------------------------------------------------
    private void OnTournamentGameEnded(bool humanWon)
    {
        AppState.Current.Daily.RecordGamePlayed();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            ResultTitle.Text      = humanWon ? "Vitória!" : "Derrota";
            ResultTitle.TextColor = humanWon ? Color.FromArgb("#4CAF50") : Color.FromArgb("#FF5252");
            ResultDetail.Text     = _vm.StatusMessage;
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
                ResultTitle.Text      = isDraw ? "Empate" : whiteWon ? $"{_vm.WhitePlayerName} vence!" : $"{_vm.BlackPlayerName} vence!";
                ResultTitle.TextColor = isDraw ? Color.FromArgb("#FFD700") : Color.FromArgb("#4CAF50");
                ResultDetail.Text     = _vm.StatusMessage;
                ResultPanel.IsVisible = true;
            });
            return;
        }

        bool humanWon = _vm.StatusMessage.Contains("Brancas vencem");

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var state = AppState.Current;

            // Registra W/L para estatísticas do perfil (sem recompensa em fichas)
            if (humanWon) state.Profile.RecordWin();
            else          state.Profile.RecordLoss();

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
            bool isDraw = _vm.StatusMessage.Contains("Empate") || _vm.StatusMessage.Contains("Afogamento");
            ResultTitle.Text      = humanWon ? "Vitória!" : isDraw ? "Empate" : "Derrota";
            ResultTitle.TextColor = humanWon
                ? Color.FromArgb("#4CAF50")
                : isDraw ? Color.FromArgb("#FFD700") : Color.FromArgb("#FF5252");
            ResultDetail.Text     = _vm.StatusMessage;
            ResultPanel.IsVisible = true;
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

    // -----------------------------------------------------------------------
    // Botão: NOVO JOGO dentro do SetupPanel — abre os menus e inicia o jogo
    // -----------------------------------------------------------------------
    private async void OnResultBackTapped(object? sender, TappedEventArgs e)
    {
        ResultPanel.IsVisible = false;
        await Shell.Current.GoToAsync("..");
    }

    private async void OnSetupNewGameClicked(object? sender = null, EventArgs? e = null)
    {
        ResultPanel.IsVisible = false;
        bool fromSetupPanel = SetupPanel.IsVisible;

        string[] diffLabels = DifficultyOptions.Select(o => o.Label).ToArray();
        string? diff = await DisplayActionSheet("Dificuldade da IA", "Cancelar", null, diffLabels);
        if (diff == null || diff == "Cancelar")
        {
            if (fromSetupPanel) await Shell.Current.GoToAsync("..");
            return;
        }
        int depth = DifficultyOptions.First(o => o.Label == diff).Depth;

        string[] timeLabels = TimeOptions.Select(o => o.Label).ToArray();
        string? timeChoice = await DisplayActionSheet("Tempo por jogador", "Cancelar", null, timeLabels);
        if (timeChoice == null || timeChoice == "Cancelar")
        {
            if (fromSetupPanel) await Shell.Current.GoToAsync("..");
            return;
        }
        int minutes = TimeOptions.First(o => o.Label == timeChoice).Minutes;

        SetupPanel.IsVisible   = false;
        WhitePlayerLabel.Text  = "♙ Você (Brancas)";
        BlackPlayerLabel.Text  = "♟ IA (Pretas)";
        Title                  = "Xadrez";
        _vm.StartNewGame(minutes, depth);
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
