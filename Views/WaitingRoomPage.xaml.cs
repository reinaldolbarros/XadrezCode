using ChessMAUI.Models;
using ChessMAUI.Services;

namespace ChessMAUI.Views;

public partial class WaitingRoomPage : ContentPage
{
    private CancellationTokenSource? _cts;
    private bool _navigating;

    public WaitingRoomPage()
    {
        InitializeComponent();
    }

    // -----------------------------------------------------------------------
    // Ao entrar: inicializa UI e começa a preencher a sala
    // -----------------------------------------------------------------------
    protected override void OnAppearing()
    {
        base.OnAppearing();

        _navigating = false;
        var mm = AppState.Current.Matchmaking;

        // Desregistra antes de registrar (seguro para re-entrada)
        mm.PlayerJoined -= OnPlayerJoined;
        mm.RoomFull     -= OnRoomFull;
        mm.PlayerJoined += OnPlayerJoined;
        mm.RoomFull     += OnRoomFull;

        // Cabeçalho
        TournTitle.Text    = $"Torneio de {mm.TotalSlots} Jogadores";
        TournSubtitle.Text = $"Mata-mata  •  Prêmio total: $ {mm.BuyIn * mm.TotalSlots:N0}";
        BuyInLabel.Text    = $"$ {mm.BuyIn:N0}";
        TimeLabel.Text     = mm.TimeMinutes > 0 ? $"{mm.TimeMinutes} min" : "Livre";

        // Reconstrói slots com jogadores já na sala (pode ser re-entrada)
        RebuildSlots();
        UpdateProgress();

        // Inicia preenchimento de bots (se ainda não está cheio)
        if (!mm.IsReady)
        {
            _cts = new CancellationTokenSource();
            _ = mm.FillBotsAsync(_cts.Token);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        var mm = AppState.Current.Matchmaking;
        mm.PlayerJoined -= OnPlayerJoined;
        mm.RoomFull     -= OnRoomFull;
    }

    // -----------------------------------------------------------------------
    // Evento: novo jogador entrou
    // -----------------------------------------------------------------------
    private void OnPlayerJoined(RoomPlayer player)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AddSlotCard(player);
            UpdateProgress();
        });
    }

    // -----------------------------------------------------------------------
    // Evento: sala cheia → contagem regressiva
    // -----------------------------------------------------------------------
    private void OnRoomFull()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (_navigating) return;
            _navigating = true;

            CancelBtn.IsVisible      = false;
            CountdownFrame.IsVisible = true;

            for (int i = 3; i >= 1; i--)
            {
                CountdownLabel.Text = $"🎮  SALA COMPLETA!  Iniciando em {i}...";
                await Task.Delay(1000);
            }

            CountdownLabel.Text = "🎮  Iniciando torneio!";
            await Task.Delay(500);

            // Cria o torneio com os jogadores da sala
            var state = AppState.Current;
            var t     = state.TournSvc.CreateFromRoom(state.Matchmaking.Players, state.Matchmaking.BuyIn,
                              state.Matchmaking.RoomType, state.Matchmaking.SatelliteTarget);
            state.ActiveTournament      = t;
            state.TournamentTimeMinutes = state.Matchmaking.TimeMinutes;

            // Zera estado de partida anterior para não processar resultado fantasma
            state.MatchResultReady      = false;
            state.LastMatchHumanWon     = false;
            state.PendingTournamentGame = false;

            await Shell.Current.GoToAsync("BracketPage");
        });
    }

    // -----------------------------------------------------------------------
    // Botão: cancelar → devolve buy-in e volta ao lobby
    // -----------------------------------------------------------------------
    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        _cts?.Cancel();
        AppState.Current.Profile.Credit(AppState.Current.Matchmaking.BuyIn);
        await Shell.Current.GoToAsync("..");
    }

    // -----------------------------------------------------------------------
    // Constrói a lista de slots do zero (para re-entradas)
    // -----------------------------------------------------------------------
    private void RebuildSlots()
    {
        SlotsContainer.Children.Clear();

        // Adiciona os jogadores já na sala
        foreach (var p in AppState.Current.Matchmaking.Players)
            AddSlotCard(p);

        // Adiciona slots vazios
        int empty = AppState.Current.Matchmaking.TotalSlots - AppState.Current.Matchmaking.Players.Count;
        for (int i = 0; i < empty; i++)
            SlotsContainer.Children.Add(BuildEmptySlot());
    }

    // -----------------------------------------------------------------------
    // Adiciona um card de jogador e remove o primeiro slot vazio
    // -----------------------------------------------------------------------
    private void AddSlotCard(RoomPlayer player)
    {
        // Remove o primeiro slot vazio (tag == "empty")
        var emptySlot = SlotsContainer.Children
            .OfType<Frame>()
            .FirstOrDefault(f => f.StyleId == "empty");

        if (emptySlot != null)
            SlotsContainer.Children.Remove(emptySlot);

        // Insere na posição correta (humano primeiro)
        int insertAt = player.IsHuman ? 0 : Math.Max(0, SlotsContainer.Children.Count - CountEmptySlots());
        SlotsContainer.Children.Insert(insertAt, BuildPlayerCard(player));
    }

    private int CountEmptySlots() =>
        SlotsContainer.Children.OfType<Frame>().Count(f => f.StyleId == "empty");

    // -----------------------------------------------------------------------
    // Card de jogador ocupado
    // -----------------------------------------------------------------------
    private static Frame BuildPlayerCard(RoomPlayer player)
    {
        var frame = new Frame
        {
            BackgroundColor = player.IsHuman ? Color.FromArgb("#1C2A1C") : Color.FromArgb("#16213E"),
            BorderColor     = player.IsHuman ? Color.FromArgb("#4CAF50") : Color.FromArgb("#0F3460"),
            CornerRadius    = 8,
            Padding         = new Thickness(12, 8),
            HasShadow       = false
        };

        var grid = new Grid
        {
            ColumnDefinitions = { new(GridLength.Auto), new(GridLength.Star), new(GridLength.Auto) }
        };

        // Avatar
        grid.Add(new Label
        {
            Text            = player.Avatar,
            FontSize        = 20,
            VerticalOptions = LayoutOptions.Center,
            Margin          = new Thickness(0, 0, 10, 0)
        });

        // Nome + rating
        var nameStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Spacing = 1 };
        nameStack.Add(new Label
        {
            Text           = player.Name,
            TextColor      = Colors.White,
            FontSize       = 14,
            FontAttributes = player.IsHuman ? FontAttributes.Bold : FontAttributes.None
        });
        var (tierIcon, tierName, _, _) = ProfileService.GetTier(
            player.IsHuman ? AppState.Current.Profile.Points : player.Rating * 3);
        string sub = player.IsHuman
            ? $"Você  {tierIcon} {tierName}"
            : $"{tierIcon} {tierName}  ·  {player.Rating} ELO";
        nameStack.Add(new Label
        {
            Text      = sub,
            TextColor = player.IsHuman ? Color.FromArgb("#4CAF50") : Color.FromArgb("#AAAACC"),
            FontSize  = 11
        });
        Grid.SetColumn(nameStack, 1);
        grid.Add(nameStack);

        // Status
        var statusLbl = new Label
        {
            Text              = "✓ Pronto",
            TextColor         = Color.FromArgb("#4CAF50"),
            FontSize          = 12,
            FontAttributes    = FontAttributes.Bold,
            VerticalOptions   = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };
        Grid.SetColumn(statusLbl, 2);
        grid.Add(statusLbl);

        frame.Content = grid;
        return frame;
    }

    // -----------------------------------------------------------------------
    // Slot vazio
    // -----------------------------------------------------------------------
    private static Frame BuildEmptySlot()
    {
        var frame = new Frame
        {
            StyleId         = "empty",
            BackgroundColor = Color.FromArgb("#12172A"),
            BorderColor     = Color.FromArgb("#252B45"),
            CornerRadius    = 8,
            Padding         = new Thickness(14, 10),
            HasShadow       = false
        };

        var grid = new Grid { ColumnDefinitions = { new(GridLength.Auto), new(GridLength.Star), new(GridLength.Auto) } };

        grid.Add(new Label { Text = "⏳", FontSize = 20, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(0, 0, 10, 0) });

        Grid.SetColumn(new Label
        {
            Text          = "Aguardando...",
            TextColor     = Color.FromArgb("#444466"),
            FontSize      = 14,
            VerticalOptions = LayoutOptions.Center
        }, 1);

        // Re-add since SetColumn on detached labels doesn't work inline
        var nameLbl = new Label
        {
            Text            = "Aguardando...",
            TextColor       = Color.FromArgb("#444466"),
            FontSize        = 14,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(nameLbl, 1);
        grid.Add(nameLbl);

        frame.Content = grid;
        return frame;
    }

    // -----------------------------------------------------------------------
    // Atualiza barra de progresso e contador
    // -----------------------------------------------------------------------
    private void UpdateProgress()
    {
        var mm      = AppState.Current.Matchmaking;
        int current = mm.Players.Count;
        int total   = mm.TotalSlots;

        CounterLabel.Text  = $"{current}/{total}";
        JoinProgress.Progress = (double)current / total;

        StatusLabel.Text = current == total
            ? "Sala completa!"
            : $"Aguardando jogadores...";
    }
}
