using ChessMAUI.Models;
using ChessMAUI.Services;
using ChessMAUI.ViewModels;

namespace ChessMAUI.Views;

public partial class GamePage : ContentPage
{
    private readonly GameViewModel _vm;

    private CancellationTokenSource? _chatCts;
    private CancellationTokenSource? _analysisCts;
    private double _squareSize;
    private bool   _resultShownForGame;
    private bool   _nextTurnIsBlack;

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
        _vm.PuzzleFinished += OnPuzzleFinished;

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

            // Desafiante (p1) começa com brancas no 1º jogo; cores invertem a cada partida
            bool p1IsWhite = state.FriendGameCount % 2 == 0;
            state.FriendGameCount++;

            string white = p1IsWhite ? p1 : p2;
            string black = p1IsWhite ? p2 : p1;

            Title = $"{p1} vs {p2}";
            _vm.StartFriendGame(white, black, state.FriendTimeMinutes);
            WhitePlayerLabel.Text  = $"♙ {white} (Brancas)";
            BlackPlayerLabel.Text  = $"♟ {black} (Pretas)";
            SetupPanel.IsVisible   = false;
            HandoffPanel.IsVisible = false;
            _drawable.IsFlipped    = false;
        }
        else if (state.PendingPuzzle)
        {
            state.PendingPuzzle  = false;
            SetupPanel.IsVisible = false;
            ResultPanel.IsVisible = false;
            var puzzle = state.PuzzleSvc.GetCurrentPuzzle();
            Title = "Puzzle do Dia";
            PuzzleBanner.IsVisible = true;
            GiveUpBtn.IsVisible    = true;
            PuzzleCategoryLabel.Text  = $"🧩 {puzzle.Category}";
            PuzzleDescLabel.Text      = puzzle.Description;
            PuzzleProgressLabel.Text  = $"0 / {puzzle.Solution.Length}";
            _vm.StartPuzzle(puzzle);
        }
        else if (!_vm.IsTournamentMode && !_vm.IsFriendMode && !_vm.IsPuzzleMode && _vm.GameOver)
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
        if (_resultShownForGame) return;
        _resultShownForGame = true;

        var state = AppState.Current;
        int starsEarned = 0;
        starsEarned += state.Daily.RecordGamePlayed();

        bool isDraw = _vm.StatusMessage.Contains("Empate") || _vm.StatusMessage.Contains("Afogamento");
        int  eloDelta = 0;

        if (state.IsCareerGame)
        {
            state.LastMatchHumanWon = humanWon;
            state.LastMatchWasDraw  = isDraw;
            state.MatchResultReady  = true;
            int oppElo = ProfileService.EloRatingForAI(state.CareerAIDepth);
            eloDelta   = state.Profile.UpdateElo(oppElo, humanWon, isDraw);
            if (humanWon)     { state.Profile.RecordWin(); starsEarned += state.Daily.RecordWin(); }
            else if (!isDraw)   state.Profile.RecordLoss();
        }
        else if (state.IsOnlineGame)
        {
            int oppElo = state.OnlineMatch.State.OpponentRating;
            eloDelta   = state.Profile.UpdateElo(oppElo, humanWon, isDraw);
            if (humanWon)     { state.Profile.RecordWin(); starsEarned += state.Daily.RecordWin(); }
            else if (!isDraw)   state.Profile.RecordLoss();
        }

        if (starsEarned > 0) state.Stars.Add(starsEarned);

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
            AnalysisBtn.IsVisible = false;
            AnalysisPanel.IsVisible = false;

            if (state.IsCareerGame)
            {
                ResultActionBtn.IsVisible      = hasNextRound;
                ResultActionBtn.Text           = nextBtnText;
                ResultSecondaryLabel.IsVisible = true;
                ResultSecondaryLabel.Text      = "Ver resultado da fase";
                AnalysisBtn.IsVisible          = true;
            }
            else if (state.IsOnlineGame)
            {
                ResultActionBtn.IsVisible       = true;
                ResultActionBtn.Text            = "🔄 Revanche";
                ResultActionBtn.BackgroundColor = Color.FromArgb("#1A4A7A");
                NewOnlineBtn.IsVisible          = true;
                NewOnlineBtn.Text              = $"🔍 Nova Partida · {state.OnlineTimeMinutes}min";
                ResultSecondaryLabel.IsVisible  = true;
                ResultSecondaryLabel.Text       = "← Voltar ao lobby";
                AnalysisBtn.IsVisible           = true;
            }
            else
            {
                ResultActionBtn.IsVisible  = true;
                ResultActionBtn.Text       = "Voltar ao Torneio";
                ResultSecondaryLabel.Text  = "← Voltar ao lobby";
                AnalysisBtn.IsVisible      = true;
            }
            ResultPanel.IsVisible = true;
        });
    }

    // -----------------------------------------------------------------------
    // Missões: registra partidas casuais
    // -----------------------------------------------------------------------
    private void OnVmPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_vm.PuzzleMovesDone) && _vm.IsPuzzleMode)
        {
            MainThread.BeginInvokeOnMainThread(() =>
                PuzzleProgressLabel.Text = $"{_vm.PuzzleMovesDone} / {_vm.PuzzleMovesTotal}");
            return;
        }

        if (e.PropertyName != nameof(_vm.GameOver) || !_vm.GameOver) return;
        if (_vm.IsTournamentMode) return;
        if (_vm.IsPuzzleMode) return; // puzzle handled by PuzzleFinished event

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
                AnalysisBtn.IsVisible   = false;
                AnalysisPanel.IsVisible = false;
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
            int  starsNow = 0;
            starsNow += state.Daily.RecordGamePlayed();
            if (humanWon)      { state.Profile.RecordWin();  starsNow += state.Daily.RecordWin(); }
            else if (!isDraw2)   state.Profile.RecordLoss();

            if (starsNow > 0) state.Stars.Add(starsNow);

            int aiDepth   = DiffDepths[_selectedDiff];
            int oppElo    = ProfileService.EloRatingForAI(aiDepth);
            int eloChange = state.Profile.UpdateElo(oppElo, humanWon, isDraw2);

            // Painel de resultado
            ResultTitle.Text      = humanWon ? "Vitória!" : isDraw2 ? "Empate" : "Derrota";
            ResultTitle.TextColor = humanWon
                ? Color.FromArgb("#4CAF50")
                : isDraw2 ? Color.FromArgb("#FFD700") : Color.FromArgb("#FF5252");
            string eloSign = eloChange >= 0 ? "+" : "";
            ResultDetail.Text              = $"{_vm.StatusMessage}\n\nRating: {eloSign}{eloChange}  →  {state.Profile.Points}";
            ResultActionBtn.Text           = "Novo Jogo";
            ResultSecondaryLabel.IsVisible = true;
            ResultSecondaryLabel.Text      = "← Voltar ao lobby";
            AnalysisBtn.IsVisible          = true;
            AnalysisPanel.IsVisible        = false;
            ResultPanel.IsVisible          = true;
        });
    }

    // -----------------------------------------------------------------------
    // Handoff pass-and-play — exibe overlay entre turnos
    // -----------------------------------------------------------------------
    private void ShowHandoffOverlay(string nextPlayerName)
    {
        // Sem painel — o relógio controla a vez. Só inverte a perspectiva com fade.
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            _nextTurnIsBlack = nextPlayerName == _vm.BlackPlayerName;
            await BoardView.FadeTo(0, 110);
            _drawable.IsFlipped = _nextTurnIsBlack;
            BoardView.Invalidate();
            await BoardView.FadeTo(1, 110);
        });
    }

    private void OnHandoffDismissed(object? sender, EventArgs e)
    {
        HandoffPanel.IsVisible = false;
    }

    // -----------------------------------------------------------------------
    // Puzzle do Dia — resultado
    // -----------------------------------------------------------------------
    private void OnPuzzleFinished(bool solved)
    {
        var state = AppState.Current;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (solved)
            {
                state.PuzzleSvc.IncrementDailyCount();
                int stars = 2 + (_vm.PuzzleDifficulty - 1); // 2/3/4 estrelas por dificuldade
                state.Stars.Add(stars);
                int starsBonus = state.Daily.RecordWin();
                if (starsBonus > 0) state.Stars.Add(starsBonus);

                string diff = _vm.PuzzleDifficulty switch { 3 => "Difícil ★★★", 2 => "Médio ★★", _ => "Fácil ★" };
                bool   isSub = state.Subscription.IsActive;
                int    done  = state.PuzzleSvc.GetDailyCount();
                int    left  = PuzzleService.FreeLimit - done;
                string next  = isSub
                    ? "Próximo desafio disponível!"
                    : left > 0
                        ? $"+{left} puzzle{(left > 1 ? "s" : "")} grátis restante{(left > 1 ? "s" : "")} hoje"
                        : "Limite diário atingido — volte amanhã!";

                ResultTitle.Text      = "✓ Resolvido!";
                ResultTitle.TextColor = Color.FromArgb("#4CAF50");
                ResultDetail.Text     = $"{_vm.PuzzleCategory}  ·  {diff}\n\n+{stars} ⭐\n{next}";

                bool canPlayMore = state.PuzzleSvc.CanPlayMore(isSub);
                ResultActionBtn.Text            = canPlayMore ? "▶ Próximo Puzzle" : "← Voltar ao lobby";
                ResultActionBtn.BackgroundColor = Color.FromArgb("#769656");
                ResultSolutionBtn.IsVisible     = false;
                ResultSecondaryLabel.IsVisible  = canPlayMore;
                ResultSecondaryLabel.Text       = "← Voltar ao lobby";
            }
            else
            {
                // Falhou — não incrementa contagem ainda, não dá estrelas
                string diff = _vm.PuzzleDifficulty switch { 3 => "Difícil ★★★", 2 => "Médio ★★", _ => "Fácil ★" };

                ResultTitle.Text      = "✗ Falhou!";
                ResultTitle.TextColor = Color.FromArgb("#FF5252");
                ResultDetail.Text     = $"{_vm.PuzzleCategory}  ·  {diff}\n\nEsse foi o movimento correto — estude a posição!";

                ResultActionBtn.Text            = "→ Próximo desafio";
                ResultActionBtn.BackgroundColor = Color.FromArgb("#1A4A1A");
                ResultSolutionBtn.IsVisible     = true;
                ResultSecondaryLabel.IsVisible  = false;
            }

            AnalysisBtn.IsVisible   = false;
            AnalysisPanel.IsVisible = false;
            PuzzleBanner.IsVisible  = false;
            ResultPanel.IsVisible   = true;
        });
    }

    // Chamado pelo botão "Ver solução" no painel de resultado (após erro)
    private void OnViewSolutionClicked(object? sender, EventArgs e)
    {
        ResultPanel.IsVisible  = false;
        PuzzleBanner.IsVisible = true;
        GiveUpBtn.IsVisible    = false;
        _vm.ApplySolutionNow();

        SolutionCategoryLabel.Text     = $"🧩 {_vm.PuzzleCategory}";
        SolutionHintLabel.Text         = _vm.PuzzleDescription;
        SolutionExplanationLabel.Text  = GetSolutionExplanation(_vm.PuzzleCategory);

        SolutionBar.IsVisible = true;
    }

    private static string GetSolutionExplanation(string category) => category switch
    {
        "Xeque-mate" =>
            "A posição final é xeque-mate: o rei adversário está em xeque e não tem nenhuma casa válida para fugir. " +
            "Este é o objetivo final do xadrez — o rei não pode ser capturado nem escapar!",

        "Garfo" =>
            "O lance cria um garfo — uma única peça ataca dois alvos ao mesmo tempo. " +
            "O adversário só consegue salvar um deles e perde o outro sem compensação.",

        "Forca" =>
            "O lance prende uma peça adversária que não pode se mover sem sofrer perdas ainda maiores. " +
            "A peça está imobilizada e será capturada ou forçará outras perdas.",

        "Promoção" =>
            "O peão avançou até a última fileira e foi promovido a uma peça muito mais poderosa. " +
            "Criar um peão passado e garantir sua promoção é frequentemente decisivo no final de jogo.",

        "Defesa" =>
            "Este lance neutraliza a ameaça imediata do adversário, protegendo o rei ou salvando uma peça em perigo. " +
            "Reconhecer ataques e defender corretamente é tão importante quanto atacar.",

        "Rei Ativo" =>
            "No final do jogo, o rei deixa de ser vulnerável e se torna uma peça ativa e decisiva. " +
            "Centralizar o rei e aproximá-lo do adversário é uma técnica fundamental de finais.",

        "Espeto" =>
            "O espeto ataca uma peça valiosa que, ao se mover para escapar, expõe uma peça ainda mais valiosa atrás dela. " +
            "É o oposto da forca: a peça atacada precisa se mover e revela o alvo real.",

        "Ataque Descoberto" =>
            "Ao mover uma peça, você descobre o ataque de outra peça que estava bloqueada atrás dela. " +
            "O adversário precisa lidar com dois ataques ao mesmo tempo — o da peça que se moveu e o ataque descoberto.",

        "Duplo Xeque" =>
            "Duas peças dão xeque ao rei ao mesmo tempo. O único recurso é mover o rei — não é possível bloquear ou capturar ambas as peças que atacam.",

        "Sacrifício" =>
            "Você entregou material deliberadamente para obter uma compensação ainda maior — posição dominante, ataque decisivo ou mate forçado. " +
            "Sacrifícios são a forma mais espetacular de tática no xadrez.",

        "Captura Simples" =>
            "Uma peça adversária estava desprotegida (en prise). Capturá-la não tem custo algum — é um ganho de material gratuito.",

        "Desvio" =>
            "O lance força a peça adversária a abandonar sua função defensiva crucial. " +
            "Ao desviá-la, você abre caminho para um ataque que ela estava impedindo.",

        "Atração" =>
            "A manobra atrai o rei ou uma peça adversária para uma casa desfavorável onde ela ficará vulnerável ou presa. " +
            "Geralmente é executada com um sacrifício que o adversário é obrigado a aceitar.",

        "Final" =>
            "Os finais exigem técnica precisa: cada lance conta e erros custam muito caro. " +
            "Com poucas peças no tabuleiro, a atividade dos reis, o avanço de peões passados e a coordenação das peças restantes são decisivos.",

        "Tática" =>
            "Este puzzle envolve uma combinação tática que cria vantagem material ou posicional decisiva. " +
            "Observe os ataques, ameaças e fraquezas da posição adversária.",

        _ =>
            "Observe a posição com atenção: onde as peças estão, quais estão sob ataque e quais casas estão disponíveis. " +
            "A jogada correta tira vantagem de um desequilíbrio tático que o adversário não consegue neutralizar."
    };

    // Chamado pelo botão "Ver solução" no banner do puzzle (desistir sem tentar)
    private void OnGiveUpPuzzleClicked(object? sender, EventArgs e)
    {
        GiveUpBtn.IsVisible = false;
        _vm.GiveUpPuzzle();   // dispara PuzzleFinished(false) → OnPuzzleFinished
    }

    private void OnSolutionContinueClicked(object? sender, EventArgs e)
    {
        SolutionBar.IsVisible = false;

        var state = AppState.Current;
        state.PuzzleSvc.IncrementDailyCount();
        bool isSub       = state.Subscription.IsActive;
        bool canPlayMore = state.PuzzleSvc.CanPlayMore(isSub);

        ResultTitle.Text                = "Solução Exibida";
        ResultTitle.TextColor           = Color.FromArgb("#5A8AB0");
        ResultDetail.Text               = "Estude os lances e tente o próximo!";
        ResultActionBtn.Text            = canPlayMore ? "→ Próximo desafio" : "← Voltar ao lobby";
        ResultActionBtn.BackgroundColor = Color.FromArgb("#1A3A5A");
        ResultSolutionBtn.IsVisible     = false;
        ResultSecondaryLabel.IsVisible  = false;
        AnalysisBtn.IsVisible           = false;
        AnalysisPanel.IsVisible         = false;
        PuzzleBanner.IsVisible          = false;
        ResultPanel.IsVisible           = true;
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

    private async void OnNewOnlineClicked(object? sender, EventArgs e)
    {
        var state = AppState.Current;

        NewOnlineBtn.IsEnabled    = false;
        ResultActionBtn.IsEnabled = false;
        ResultDetail.Text         = "🔍 Buscando novo oponente...";

        await state.OnlineMatch.StartSearchingAsync(state.OnlineTimeMinutes, state.Profile.Points);

        if (state.OnlineMatch.State.Phase != OnlineMatchPhase.Confirmed)
        {
            NewOnlineBtn.IsEnabled    = true;
            ResultActionBtn.IsEnabled = true;
            return;
        }

        var s      = state.OnlineMatch.State;
        string myName  = state.Profile.Name;
        string oppName = s.OpponentName;

        state.OnlineOpponentName  = oppName;
        state.OnlinePlayerIsWhite = s.PlayerIsWhite;

        WhitePlayerLabel.Text = s.PlayerIsWhite
            ? $"♙ {myName} (Brancas)"
            : $"♙ {oppName} (Brancas)";
        BlackPlayerLabel.Text = s.PlayerIsWhite
            ? $"♟ {oppName} (Pretas)"
            : $"♟ {myName} (Pretas)";

        NewOnlineBtn.IsEnabled    = true;
        ResultActionBtn.IsEnabled = true;
        NewOnlineBtn.IsVisible    = false;
        ResultPanel.IsVisible     = false;
        _resultShownForGame       = false;
        Title = $"vs {oppName}";
        _vm.StartTournamentGame(oppName, state.OnlineTimeMinutes, 3);
    }

    // -----------------------------------------------------------------------
    // Análise pós-jogo — tabuleiro animado
    // -----------------------------------------------------------------------
    private readonly AnalysisBoardDrawable               _analysisBoardDrawable = new();
    private          IDispatcherTimer?                   _analysisAnimTimer;
    private          int                                 _analysisPhase;
    private          List<GameViewModel.MoveAnalysisItem> _analysisItems        = [];
    private          int                                 _analysisCurrentIdx;

    private async void OnAnalysisBtnClicked(object? sender, EventArgs e)
    {
        AnalysisPanel.IsVisible       = true;
        AnalysisSpinner.IsRunning     = true;
        AnalysisSpinner.IsVisible     = true;
        AnalysisContent.IsVisible     = false;

        AnalysisBoardView.Drawable = _analysisBoardDrawable;

        _analysisCts?.Cancel();
        _analysisCts = new CancellationTokenSource();

        try
        {
            _analysisItems = await _vm.AnalyzeGameAsync(_analysisCts.Token);
        }
        catch (OperationCanceledException) { return; }
        finally
        {
            AnalysisSpinner.IsRunning = false;
            AnalysisSpinner.IsVisible = false;
        }

        if (_analysisItems.Count == 0)
        {
            AnalysisQualityLabel.Text    = "✓ Partida limpa!";
            AnalysisQualityLabel.TextColor = Color.FromArgb("#4CAF50");
            AnalysisDescLabel.Text       = "Nenhum erro significativo encontrado. Parabéns pela partida!";
            AnalysisIndexLabel.Text      = "";
            AnalysisPrevBtn.IsVisible    = false;
            AnalysisNextBtn.IsVisible    = false;
            AnalysisBoardView.IsVisible  = false;
            AnalysisContent.IsVisible    = true;
            return;
        }

        AnalysisBoardView.IsVisible  = true;
        AnalysisPrevBtn.IsVisible    = _analysisItems.Count > 1;
        AnalysisNextBtn.IsVisible    = _analysisItems.Count > 1;
        _analysisCurrentIdx = 0;
        ShowAnalysisItem(0);
        AnalysisContent.IsVisible = true;
    }

    private void ShowAnalysisItem(int idx)
    {
        StopAnalysisAnim();
        _analysisPhase = 0;

        var item = _analysisItems[idx];

        AnalysisIndexLabel.Text = $"{idx + 1} / {_analysisItems.Count}";

        (string badge, Color badgeColor) = item.Quality switch
        {
            GameViewModel.MoveQuality.Blunder  => ("✗ Erro Grave", Color.FromArgb("#FF4444")),
            GameViewModel.MoveQuality.Mistake  => ("✗ Erro",       Color.FromArgb("#FF7744")),
            _                                  => ("⚠ Imprecisão", Color.FromArgb("#FFD700"))
        };
        AnalysisQualityLabel.Text      = $"{badge} · Lance {item.MoveNumber}";
        AnalysisQualityLabel.TextColor = badgeColor;
        AnalysisDescLabel.Text         = item.Description;

        _analysisBoardDrawable.Board = item.BoardBefore;
        ApplyAnimPhase(item, 0);
        AnalysisBoardView.Invalidate();

        _analysisAnimTimer          = Application.Current!.Dispatcher.CreateTimer();
        _analysisAnimTimer.Interval = TimeSpan.FromMilliseconds(1800);
        _analysisAnimTimer.Tick    += (_, _) =>
        {
            _analysisPhase = (_analysisPhase + 1) % 3;
            ApplyAnimPhase(item, _analysisPhase);
            MainThread.BeginInvokeOnMainThread(() => AnalysisBoardView.Invalidate());
        };
        _analysisAnimTimer.Start();
    }

    private void ApplyAnimPhase(GameViewModel.MoveAnalysisItem item, int phase)
    {
        switch (phase)
        {
            case 0:
                _analysisBoardDrawable.ClearHighlight();
                break;
            case 1: // Lance do jogador — vermelho
                _analysisBoardDrawable.SetHighlight(
                    item.PlayerMove.FromRow, item.PlayerMove.FromCol,
                    item.PlayerMove.ToRow,   item.PlayerMove.ToCol,
                    Color.FromArgb("#BBFF3333"));
                break;
            case 2: // Melhor lance — verde
                _analysisBoardDrawable.SetHighlight(
                    item.BestMove.FromRow, item.BestMove.FromCol,
                    item.BestMove.ToRow,   item.BestMove.ToCol,
                    Color.FromArgb("#BB33FF55"));
                break;
        }
    }

    private void OnAnalysisPrev(object? sender, EventArgs e)
    {
        if (_analysisCurrentIdx > 0)
        {
            _analysisCurrentIdx--;
            ShowAnalysisItem(_analysisCurrentIdx);
        }
    }

    private void OnAnalysisNext(object? sender, EventArgs e)
    {
        if (_analysisCurrentIdx < _analysisItems.Count - 1)
        {
            _analysisCurrentIdx++;
            ShowAnalysisItem(_analysisCurrentIdx);
        }
    }

    private void StopAnalysisAnim()
    {
        _analysisAnimTimer?.Stop();
        _analysisAnimTimer = null;
    }

    private void OnAnalysisCloseClicked(object? sender, EventArgs e)
    {
        StopAnalysisAnim();
        AnalysisPanel.IsVisible = false;
    }

    private async void OnResultBackTapped(object? sender, TappedEventArgs e)
    {
        _analysisCts?.Cancel();
        StopAnalysisAnim();
        AppState.Current.IsOnlineGame   = false;
        ResultPanel.IsVisible           = false;
        AnalysisPanel.IsVisible         = false;
        PuzzleBanner.IsVisible          = false;
        SolutionBar.IsVisible           = false;
        ResultSolutionBtn.IsVisible     = false;
        NewOnlineBtn.IsVisible          = false;
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
        _analysisCts?.Cancel();
        StopAnalysisAnim();
        ResultPanel.IsVisible          = false;
        AnalysisPanel.IsVisible        = false;
        SetupPanel.IsVisible           = false;
        PuzzleBanner.IsVisible         = false;
        SolutionBar.IsVisible          = false;
        ResultSolutionBtn.IsVisible    = false;
        WhitePlayerLabel.Text         = "♙ Você (Brancas)";
        BlackPlayerLabel.Text         = "♟ IA (Pretas)";
        Title                         = "ChessArena";
        AppState.Current.IsCareerGame = false;
        _vm.StartNewGame(0, DiffDepths[_selectedDiff]);
    }

    private async void OnResultActionClicked(object? sender, EventArgs e)
    {
        var state = AppState.Current;

        // Próximo desafio (modo puzzle — solved, failed ou solução exibida)
        if (_vm.IsPuzzleMode)
        {
            // Se o puzzle falhou e o usuário não viu a solução, incrementa agora
            if (!_vm.PuzzleSolvedResult && ResultSolutionBtn.IsVisible == false &&
                ResultActionBtn.Text.Contains("Próximo"))
            {
                state.PuzzleSvc.IncrementDailyCount();
            }

            bool isSub = state.Subscription.IsActive;
            if (state.PuzzleSvc.CanPlayMore(isSub))
            {
                ResultPanel.IsVisible = false;
                SolutionBar.IsVisible = false;
                var nextPuzzle = state.PuzzleSvc.GetCurrentPuzzle();
                Title = "Puzzle do Dia";
                PuzzleBanner.IsVisible    = true;
                GiveUpBtn.IsVisible       = true;
                PuzzleCategoryLabel.Text  = $"🧩 {nextPuzzle.Category}";
                PuzzleDescLabel.Text      = nextPuzzle.Description;
                PuzzleProgressLabel.Text  = $"0 / {nextPuzzle.Solution.Length}";
                _vm.StartPuzzle(nextPuzzle);
            }
            else
            {
                PuzzleBanner.IsVisible = false;
                ResultPanel.IsVisible  = false;
                await Shell.Current.GoToAsync("..");
            }
            return;
        }

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
            if (ResultActionBtn.Text.Contains("Revanche"))
            {
                ResultActionBtn.IsEnabled = false;
                NewOnlineBtn.IsEnabled    = false;
                ResultActionBtn.Text      = "Aguardando...";
                ResultDetail.Text         = "Oponente aceitou! Preparando nova partida...";
                await Task.Delay(1500);

                // Inverte as cores para a revanche
                state.OnlinePlayerIsWhite = !state.OnlinePlayerIsWhite;

                string myName  = state.Profile.Name;
                string oppName = state.OnlineOpponentName;
                WhitePlayerLabel.Text = state.OnlinePlayerIsWhite
                    ? $"♙ {myName} (Brancas)"
                    : $"♙ {oppName} (Brancas)";
                BlackPlayerLabel.Text = state.OnlinePlayerIsWhite
                    ? $"♟ {oppName} (Pretas)"
                    : $"♟ {myName} (Pretas)";

                ResultActionBtn.IsEnabled = true;
                NewOnlineBtn.IsEnabled    = true;
                NewOnlineBtn.IsVisible    = false;
                ResultPanel.IsVisible     = false;
                _resultShownForGame       = false;
                Title = $"Revanche vs {oppName}";
                _vm.StartTournamentGame(oppName, state.OnlineTimeMinutes, 3);
            }
            else
            {
                NewOnlineBtn.IsVisible = false;
                state.IsOnlineGame     = false;
                ResultPanel.IsVisible  = false;
                await Shell.Current.GoToAsync("..");
            }
            return;
        }

        ResultPanel.IsVisible = false;
        if (_vm.IsTournamentMode || _vm.IsFriendMode || _vm.IsPuzzleMode)
        {
            PuzzleBanner.IsVisible = false;
            await Shell.Current.GoToAsync("..");
        }
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
        if (_drawable.IsFlipped) { col = 7 - col; row = 7 - row; }
        _vm.SquareTappedCommand.Execute(_vm.Squares[row, col]);
    }

    // -----------------------------------------------------------------------
    // Adapta o tamanho do tabuleiro à tela
    // -----------------------------------------------------------------------
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        double puzzleExtra = _vm.IsPuzzleMode ? 44 : 0;
        double used = (_vm.TimerVisible ? 280 : 180) + puzzleExtra;
        double available = Math.Min(width - 16, height - used);
        if (available <= 0) return;

        _squareSize = available / 8.0;

        BoardView.WidthRequest  = available;
        BoardView.HeightRequest = available;
        BoardView.Invalidate();
    }
}
