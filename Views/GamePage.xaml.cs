using ChessMAUI.Services;
using ChessMAUI.ViewModels;

namespace ChessMAUI.Views;

public partial class GamePage : ContentPage
{
    private readonly GameViewModel _vm;

    // Opções de tempo (minutos); 0 = sem limite
    private static readonly (string Label, int Minutes)[] TimeOptions =
    [
        ("Sem limite",  0),
        ("1 minuto",    1),
        ("2 minutos",   2),
        ("3 minutos",   3),
        ("4 minutos",   4),
        ("5 minutos",   5),
        ("10 minutos", 10),
        ("15 minutos", 15),
        ("30 minutos", 30),
    ];

    private CancellationTokenSource? _chatCts;
    private double _squareSize;

    public GamePage()
    {
        InitializeComponent();
        _vm = new GameViewModel();
        BindingContext = _vm;

        _vm.PromotionRequested  += OnPromotionRequested;
        _vm.ChatMessageReceived += OnChatMessageReceived;
        _vm.TournamentGameEnded += OnTournamentGameEnded;
        _vm.ResignRequested    += OnResignRequested;
        _vm.DrawOfferRequested += OnDrawOfferRequested;
        _vm.PropertyChanged    += OnVmPropertyChanged;

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
        // Registra missão diária — navegação fica a cargo do jogador (botão Voltar)
        AppState.Current.Daily.RecordGamePlayed();
    }

    // -----------------------------------------------------------------------
    // Missões: registra partidas casuais
    // -----------------------------------------------------------------------
    private void OnVmPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(_vm.GameOver) || !_vm.GameOver) return;
        if (_vm.IsTournamentMode) return; // tournament flow handled in OnTournamentGameEnded

        var state = AppState.Current;
        // Human plays white; "Brancas vencem" = human won in casual mode
        bool humanWon = _vm.StatusMessage.Contains("Brancas vencem");

        // Pontos e W/L por partida casual
        if (humanWon)
        {
            state.Profile.RecordWin();
            state.Profile.AddPoints(10, "Vitória – partida rápida", "♟");
        }
        else
        {
            state.Profile.RecordLoss();
        }

        // Missões diárias (retorna true se recém completou)
        bool m1Done = state.Daily.RecordGamePlayed();
        bool m2Done = humanWon && state.Daily.RecordWin();

        if (m1Done || m2Done)
        {
            var missions = state.Daily.GetMissions();
            if (m1Done) { var m = missions[0]; state.Profile.Credit(m.BalanceReward, "Missão – Partida jogada", "✅"); }
            if (m2Done) { var m = missions[1]; state.Profile.Credit(m.BalanceReward, "Missão – Vitória em partida", "✅"); }
            MainThread.BeginInvokeOnMainThread(async () =>
                await DisplayAlert("✅ Missão Completa!", "Recompensa creditada!", "OK"));
        }
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
    // Botão: Novo Jogo — escolhe dificuldade e depois tempo
    // -----------------------------------------------------------------------
    private static readonly (string Label, int Depth)[] DifficultyOptions =
    [
        ("Fácil",   1),
        ("Médio",   3),
        ("Difícil", 5),
    ];

    private async void OnNewGameClicked(object? sender, EventArgs e)
    {
        string[] diffLabels = DifficultyOptions.Select(o => o.Label).ToArray();
        string? diff = await DisplayActionSheet("Dificuldade da IA", "Cancelar", null, diffLabels);
        if (diff == null || diff == "Cancelar") return;
        int depth = DifficultyOptions.FirstOrDefault(o => o.Label == diff).Depth;

        string[] timeLabels = TimeOptions.Select(o => o.Label).ToArray();
        string? choice = await DisplayActionSheet("Tempo por jogador", "Cancelar", null, timeLabels);
        if (choice == null || choice == "Cancelar") return;

        int minutes = TimeOptions.FirstOrDefault(o => o.Label == choice).Minutes;
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
