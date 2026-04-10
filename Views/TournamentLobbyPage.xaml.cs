using ChessMAUI.Models;

namespace ChessMAUI.Views;

public partial class TournamentLobbyPage : ContentPage
{
    private static readonly decimal[] BuyInFilters = [0, 10, 25, 50, 100, 250, 500];
    private decimal _activeFilter = 0; // 0 = todos

    public TournamentLobbyPage()
    {
        InitializeComponent();
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

    private void OnRoomsUpdated() => RefreshList();

    // -----------------------------------------------------------------------
    // Filtros de buy-in
    // -----------------------------------------------------------------------
    private void BuildFilterBar()
    {
        FilterBar.Children.Clear();
        foreach (var f in BuyInFilters)
        {
            string label = f == 0 ? "Todas" : $"$ {f:N0}";
            var btn = new Button
            {
                Text            = label,
                FontSize        = 12,
                CornerRadius    = 16,
                Padding         = new Thickness(14, 6),
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
            .Where(r => _activeFilter == 0 || r.BuyIn == _activeFilter)
            .OrderBy(r => r.BuyIn)
            .ThenBy(r => r.Size)
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
        var bg    = alt ? Color.FromArgb("#16213E") : Color.FromArgb("#12192E");
        var row   = new Grid
        {
            ColumnDefinitions = { new(GridLength.Star), new(80), new(80), new(80), new(70) },
            BackgroundColor   = bg,
            Padding           = new Thickness(14, 10)
        };

        // Coluna 0: ícone tamanho + id
        var nameStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center };
        nameStack.Add(new Label
        {
            Text      = $"Mesa #{room.Id}  {SizeEmoji(room.Size)}",
            TextColor = Colors.White, FontSize = 13, FontAttributes = FontAttributes.Bold
        });
        nameStack.Add(new Label
        {
            Text      = room.StatusLabel,
            TextColor = room.StatusColor, FontSize = 11
        });
        row.Add(nameStack);

        // Barra de progresso de jogadores
        var progStack = new VerticalStackLayout { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
        progStack.Add(new Label
        {
            Text = $"{room.Joined}/{room.Size}", TextColor = Colors.White, FontSize = 13,
            HorizontalTextAlignment = TextAlignment.Center
        });
        var prog = new ProgressBar
        {
            Progress        = room.Size > 0 ? (double)room.Joined / room.Size : 0,
            ProgressColor   = room.CanJoin ? Color.FromArgb("#4CAF50") : Color.FromArgb("#555555"),
            BackgroundColor = Color.FromArgb("#0F3460"),
            HeightRequest   = 4
        };
        progStack.Add(prog);
        Grid.SetColumn(progStack, 1);
        row.Add(progStack);

        // Buy-in
        var buyInLbl = new Label
        {
            Text = $"$ {room.BuyIn:N0}", TextColor = Color.FromArgb("#FFD700"),
            FontSize = 13, FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(buyInLbl, 2);
        row.Add(buyInLbl);

        // Prêmio
        var prizeLbl = new Label
        {
            Text = $"$ {room.PrizePool:N0}", TextColor = Color.FromArgb("#4CAF50"),
            FontSize = 12, HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(prizeLbl, 3);
        row.Add(prizeLbl);

        // Tempo
        var timeLbl = new Label
        {
            Text = room.TimeLabel, TextColor = Color.FromArgb("#AAAACC"),
            FontSize = 12, HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(timeLbl, 4);
        row.Add(timeLbl);

        // Tap para entrar
        if (room.CanJoin)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (_, _) => await OnRoomTapped(room);
            row.GestureRecognizers.Add(tap);
        }
        else
        {
            row.Opacity = 0.5;
        }

        return row;
    }

    private static string SizeEmoji(int size) => size switch
    {
        8  => "⚡ 8",
        16 => "🔥 16",
        32 => "💎 32",
        64 => "👑 64",
        _  => size.ToString()
    };

    // -----------------------------------------------------------------------
    // Entrar na sala
    // -----------------------------------------------------------------------
    private async Task OnRoomTapped(TournamentRoom room)
    {
        var profile = AppState.Current.Profile;

        if (!profile.TryDebit(room.BuyIn))
        {
            await DisplayAlert("Saldo insuficiente",
                $"Você precisa de $ {room.BuyIn:N0}.\nSaldo: $ {profile.Balance:N0}", "OK");
            return;
        }

        AppState.Current.RoomLobby.JoinRoom(room);
        AppState.Current.Matchmaking.CreateRoom(
            room.Size, room.BuyIn, room.TimeMinutes, profile.Name,
            profile.Rating, profile.Avatar);

        await Shell.Current.GoToAsync("WaitingRoomPage");
    }
}
