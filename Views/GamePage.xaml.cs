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
    private CancellationTokenSource? _resizeCts;

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

        // Desabilita input do tabuleiro durante resize para evitar crash no WinUI input handler
        SizeChanged += OnPageSizeChanged;
    }

    private void OnPageSizeChanged(object? sender, EventArgs e)
    {
        BoardGrid.InputTransparent = true;

        _resizeCts?.Cancel();
        _resizeCts = new CancellationTokenSource();
        var token  = _resizeCts.Token;

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(300, token); // aguarda resize estabilizar
                MainThread.BeginInvokeOnMainThread(
                    () => BoardGrid.InputTransparent = false);
            }
            catch (OperationCanceledException) { }
        }, token);
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

        // XP por partida casual
        state.Profile.AddXp(humanWon ? 20 : 8);

        // Missões diárias (retorna true se recém completou)
        bool m1Done = state.Daily.RecordGamePlayed();
        bool m2Done = humanWon && state.Daily.RecordWin();

        if (m1Done || m2Done)
        {
            var missions = state.Daily.GetMissions();
            if (m1Done) { var m = missions[0]; state.Profile.Credit(m.BalanceReward); state.Profile.AddXp(m.XpReward); }
            if (m2Done) { var m = missions[1]; state.Profile.Credit(m.BalanceReward); state.Profile.AddXp(m.XpReward); }
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
    // Botão: Novo Jogo — exibe seletor de tempo e inicia partida
    // -----------------------------------------------------------------------
    private async void OnNewGameClicked(object? sender, EventArgs e)
    {
        string[] labels  = TimeOptions.Select(o => o.Label).ToArray();
        string? choice   = await DisplayActionSheet("Tempo por jogador", "Cancelar", null, labels);

        if (choice == null || choice == "Cancelar") return;

        int minutes = TimeOptions.FirstOrDefault(o => o.Label == choice).Minutes;
        _vm.StartNewGame(minutes);
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
    // Constrói o tabuleiro 8×8 programaticamente
    // -----------------------------------------------------------------------
    private void BuildBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            BoardGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            BoardGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                var sq = _vm.Squares[r, c];

                var bg = new BoxView { BindingContext = sq };
                bg.SetBinding(BackgroundColorProperty, nameof(SquareViewModel.BackgroundColor));

                var piece = new Label
                {
                    Style         = (Style)Resources["PieceLabel"],
                    BindingContext = sq,
                };
                piece.SetBinding(Label.TextProperty, nameof(SquareViewModel.PieceSymbol));

                // Coordenadas de notação (canto da casa)
                string coord = "";
                if (c == 0) coord += (char)('8' - r);
                if (r == 7) coord += (char)('a' + c);
                var coordLbl = new Label
                {
                    Text             = coord,
                    FontSize         = 9,
                    TextColor        = sq.IsLight
                                       ? Color.FromArgb("#B58863")
                                       : Color.FromArgb("#F0D9B5"),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions   = LayoutOptions.End,
                    Margin            = new Thickness(2, 0, 0, 1),
                    InputTransparent  = true
                };

                var cell = new Grid();
                cell.Add(bg);
                cell.Add(piece);
                cell.Add(coordLbl);

                var tap = new TapGestureRecognizer
                {
                    Command          = _vm.SquareTappedCommand,
                    CommandParameter = sq
                };
                cell.GestureRecognizers.Add(tap);

                Grid.SetRow(cell, r);
                Grid.SetColumn(cell, c);
                BoardGrid.Add(cell);
            }
        }
    }

    // -----------------------------------------------------------------------
    // Adapta o tamanho do tabuleiro à tela
    // -----------------------------------------------------------------------
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // Desconta: relógios (~56×2) + status (~36) + botões (~52) + margens
        double used      = _vm.TimerVisible ? 240 : 130;
        double available = Math.Min(width - 16, height - used);
        if (available <= 0) return;

        double sq = available / 8.0;

        BoardGrid.WidthRequest  = available;
        BoardGrid.HeightRequest = available;

        foreach (var row in BoardGrid.RowDefinitions)    row.Height = sq;
        foreach (var col in BoardGrid.ColumnDefinitions) col.Width  = sq;

        // Ajusta fonte das peças proporcionalmente
        double fontSize = sq * 0.62;
        foreach (var child in BoardGrid.Children)
        {
            if (child is not Grid cell) continue;
            foreach (var inner in cell.Children)
                if (inner is Label lbl && lbl.Style == (Style)Resources["PieceLabel"])
                    lbl.FontSize = fontSize;
        }
    }
}
