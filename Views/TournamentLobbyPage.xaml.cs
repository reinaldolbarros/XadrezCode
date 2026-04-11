using ChessMAUI.Models;

namespace ChessMAUI.Views;

public partial class TournamentLobbyPage : ContentPage
{
    private static readonly decimal[]        BuyInFilters = [0, 10, 25, 50, 100, 250, 500];
    private static readonly TournamentType[] TypeFilters  =
    [
        TournamentType.Standard, TournamentType.HeadsUp,   TournamentType.Bounty,
        TournamentType.Satellite, TournamentType.Turbo,    TournamentType.HyperTurbo,
        TournamentType.Ranked
    ];

    private TournamentType? _activeType   = null; // null = todos
    private decimal         _activeFilter = 0;    // 0 = todos

    public TournamentLobbyPage()
    {
        InitializeComponent();
        BuildTypeTabs();
        BuildFilterBar();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var svc = AppState.Current.RoomLobby;
        svc.RoomsUpdated -= OnRoomsUpdated;
        svc.RoomsUpdated += OnRoomsUpdated;
        RefreshList();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        AppState.Current.RoomLobby.RoomsUpdated -= OnRoomsUpdated;
    }

    private void OnRoomsUpdated() =>
        MainThread.BeginInvokeOnMainThread(RefreshList);

    // -----------------------------------------------------------------------
    // Abas de tipo
    // -----------------------------------------------------------------------
    private void BuildTypeTabs()
    {
        TypeTabBar.Children.Clear();

        AddTypeTab(null, "Todos");
        foreach (var t in TypeFilters)
        {
            string label = t switch
            {
                TournamentType.Standard   => "Standard",
                TournamentType.HeadsUp    => "⚔ 1v1",
                TournamentType.Bounty     => "🎯 Bounty",
                TournamentType.Satellite  => "🚀 Satélite",
                TournamentType.Turbo      => "⚡ Turbo",
                TournamentType.HyperTurbo => "🔥 Hyper",
                TournamentType.Ranked     => "🏅 Ranked",
                _ => t.ToString()
            };
            AddTypeTab(t, label);
        }
    }

    private void AddTypeTab(TournamentType? type, string label)
    {
        bool active = _activeType == type;
        var btn = new Button
        {
            Text            = label,
            FontSize        = 12,
            CornerRadius    = 16,
            Padding         = new Thickness(14, 6),
            BackgroundColor = active ? Color.FromArgb("#FFD700") : Color.FromArgb("#1C2A4A"),
            TextColor       = active ? Colors.Black : Colors.White
        };
        btn.Clicked += (_, _) => { _activeType = type; BuildTypeTabs(); BuildFilterBar(); RefreshList(); };
        TypeTabBar.Children.Add(btn);
    }

    // -----------------------------------------------------------------------
    // Filtros de buy-in
    // -----------------------------------------------------------------------
    private void BuildFilterBar()
    {
        FilterBar.Children.Clear();
        foreach (var f in BuyInFilters)
        {
            string label = f == 0 ? "Todos" : $"$ {f:N0}";
            var btn = new Button
            {
                Text            = label,
                FontSize        = 11,
                CornerRadius    = 14,
                Padding         = new Thickness(12, 4),
                BackgroundColor = f == _activeFilter ? Color.FromArgb("#0F3460") : Color.FromArgb("#252B45"),
                TextColor       = Colors.White
            };
            decimal captured = f;
            btn.Clicked += (_, _) => { _activeFilter = captured; BuildFilterBar(); RefreshList(); };
            FilterBar.Children.Add(btn);
        }
    }

    // -----------------------------------------------------------------------
    // Lista de salas
    // -----------------------------------------------------------------------
    private void RefreshList()
    {
        var rooms = AppState.Current.RoomLobby.Rooms
            .Where(r => _activeType == null  || r.Type  == _activeType)
            .Where(r => _activeFilter == 0   || r.BuyIn == _activeFilter)
            .OrderBy(r => r.Type).ThenBy(r => r.BuyIn).ThenBy(r => r.Size)
            .ToList();

        RoomList.Children.Clear();
        bool alt = false;
        foreach (var room in rooms)
        {
            RoomList.Children.Add(BuildRow(room, alt));
            alt = !alt;
        }
    }

    private Grid BuildRow(TournamentRoom room, bool alt)
    {
        var bg  = alt ? Color.FromArgb("#16213E") : Color.FromArgb("#12192E");
        var row = new Grid
        {
            ColumnDefinitions = { new(GridLength.Star), new(72), new(72), new(72), new(60) },
            BackgroundColor   = bg,
            Padding           = new Thickness(14, 9)
        };

        // Coluna 0: badge tipo + nome + status
        var nameStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Spacing = 2 };

        // Badge colorido do tipo
        var badgeRow = new HorizontalStackLayout { Spacing = 6 };
        if (!string.IsNullOrEmpty(room.TypeBadge))
            badgeRow.Add(new Label { Text = room.TypeBadge, FontSize = 11 });
        badgeRow.Add(new Label
        {
            Text      = room.TypeLabel,
            TextColor = TypeColor(room.Type),
            FontSize  = 10, FontAttributes = FontAttributes.Bold
        });
        nameStack.Add(badgeRow);

        // Nome + tamanho
        nameStack.Add(new Label
        {
            Text      = $"{SizeEmoji(room.Size)}  #{room.Id}",
            TextColor = Colors.White, FontSize = 13, FontAttributes = FontAttributes.Bold
        });

        // Info extra por tipo
        string extra = room.Type switch
        {
            TournamentType.Bounty    => $"🎯 Bounty: $ {room.BountyPerPlayer:N0}/elim.",
            TournamentType.Satellite => $"🚀 Vaga: $ {room.SatelliteTarget:N0}",
            TournamentType.Ranked    => $"ELO {room.MinRating}–{(room.MaxRating == 9999 ? "∞" : room.MaxRating.ToString())}",
            TournamentType.HeadsUp   => "Melhor de 3",
            _ => room.StatusLabel
        };
        nameStack.Add(new Label { Text = extra, TextColor = room.StatusColor, FontSize = 10 });
        row.Add(nameStack);

        // Jogadores + barra
        var progStack = new VerticalStackLayout { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Spacing = 3 };
        progStack.Add(new Label
        {
            Text = $"{room.Joined}/{room.Size}", TextColor = Colors.White, FontSize = 12,
            HorizontalTextAlignment = TextAlignment.Center
        });
        progStack.Add(new ProgressBar
        {
            Progress        = room.Size > 0 ? (double)room.Joined / room.Size : 0,
            ProgressColor   = room.CanJoin ? Color.FromArgb("#4CAF50") : Color.FromArgb("#555555"),
            BackgroundColor = Color.FromArgb("#0F3460"),
            HeightRequest   = 4
        });
        Grid.SetColumn(progStack, 1);
        row.Add(progStack);

        // Buy-in
        var buyInLbl = new Label
        {
            Text = $"$ {room.BuyIn:N0}", TextColor = Color.FromArgb("#FFD700"),
            FontSize = 12, FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(buyInLbl, 2);
        row.Add(buyInLbl);

        // Prêmio
        string prizeText = room.Type == TournamentType.Satellite
            ? $"Vaga\n${room.SatelliteTarget:N0}"
            : $"$ {room.PrizePool:N0}";
        var prizeLbl = new Label
        {
            Text = prizeText, TextColor = Color.FromArgb("#4CAF50"),
            FontSize = 11, HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(prizeLbl, 3);
        row.Add(prizeLbl);

        // Tempo
        var timeLbl = new Label
        {
            Text = room.TimeLabel, TextColor = Color.FromArgb("#AAAACC"),
            FontSize = 11, HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(timeLbl, 4);
        row.Add(timeLbl);

        if (room.CanJoin)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (_, _) => await OnRoomTapped(room);
            row.GestureRecognizers.Add(tap);
        }
        else
        {
            row.Opacity = 0.45;
        }

        return row;
    }

    private static Color TypeColor(TournamentType t) => t switch
    {
        TournamentType.HeadsUp    => Color.FromArgb("#FF9800"),
        TournamentType.Bounty     => Color.FromArgb("#F44336"),
        TournamentType.Satellite  => Color.FromArgb("#2196F3"),
        TournamentType.Turbo      => Color.FromArgb("#FFEB3B"),
        TournamentType.HyperTurbo => Color.FromArgb("#FF5722"),
        TournamentType.Ranked     => Color.FromArgb("#9C27B0"),
        _                         => Color.FromArgb("#4CAF50")
    };

    private static string SizeEmoji(int size) => size switch
    {
        2  => "⚔ 2",
        8  => "⚡ 8",
        16 => "🔥 16",
        32 => "💎 32",
        64 => "👑 64",
        _  => size.ToString()
    };

    // -----------------------------------------------------------------------
    // Validação e entrada na sala
    // -----------------------------------------------------------------------
    private async Task OnRoomTapped(TournamentRoom room)
    {
        var profile = AppState.Current.Profile;

        // Validação Ranked
        if (room.Type == TournamentType.Ranked &&
            (profile.Rating < room.MinRating || profile.Rating > room.MaxRating))
        {
            await DisplayAlert("Faixa de Rating",
                $"Este torneio é para jogadores com rating entre {room.MinRating} e {(room.MaxRating == 9999 ? "∞" : room.MaxRating.ToString())}.\nSeu rating: {profile.Rating}", "OK");
            return;
        }

        if (!profile.TryDebit(room.BuyIn))
        {
            await DisplayAlert("Saldo insuficiente",
                $"Você precisa de $ {room.BuyIn:N0}.\nSaldo: $ {profile.Balance:N0}", "OK");
            return;
        }

        AppState.Current.RoomLobby.JoinRoom(room);
        AppState.Current.Matchmaking.CreateRoom(
            room.Size, room.BuyIn, room.TimeMinutes,
            profile.Name, profile.Rating, profile.Avatar);

        await Shell.Current.GoToAsync("WaitingRoomPage");
    }
}
