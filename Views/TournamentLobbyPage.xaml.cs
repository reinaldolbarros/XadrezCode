using ChessMAUI.Models;

namespace ChessMAUI.Views;

public partial class TournamentLobbyPage : ContentPage
{
    private static readonly decimal[]        BuyInFilters = [0, 10, 25, 50, 100, 250, 500, 1000, 2500];
    private static readonly TournamentType[] TypeFilters  =
    [
        TournamentType.Standard, TournamentType.HeadsUp,   TournamentType.Bounty,
        TournamentType.Satellite, TournamentType.Turbo,    TournamentType.HyperTurbo,
        TournamentType.Ranked
    ];

    private TournamentType? _activeType   = null;
    private decimal         _activeFilter = 0;
    private bool            _premiumOnly  = false;
    private bool            _isCustom     = false;

    // Seleções do painel personalizado
    private TournamentType _customType   = TournamentType.Standard;
    private int            _customSize   = 8;
    private int            _customTime   = 5;
    private decimal        _customBuyIn  = 25;

    public TournamentLobbyPage()
    {
        InitializeComponent();
        BuildTypeTabs();
        BuildFilterBar();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BalanceLabel.Text = $"$ {AppState.Current.Profile.Balance:N0}";
        var svc = AppState.Current.RoomLobby;
        svc.RoomsUpdated -= OnRoomsUpdated;
        svc.RoomsUpdated += OnRoomsUpdated;
        if (_isCustom) RebuildCustomSummary();
        else RefreshList();
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

        AddTypeTab(null,  "Todos",          false);
        AddTypeTab(null,  "💎 Premium",    true);
        AddTypeTab(null,  "✏ Personalizado", false, isCustomTab: true);
        foreach (var t in TypeFilters)
        {
            string label = t switch
            {
                TournamentType.Standard   => "Standard",
                TournamentType.HeadsUp    => "⚔ Duelo",
                TournamentType.Bounty     => "🎯 Bounty",
                TournamentType.Satellite  => "🎟 Bilhete Dourado",
                TournamentType.Turbo      => "⚡ Turbo",
                TournamentType.HyperTurbo => "🔥 Hyper",
                TournamentType.Ranked     => "🏅 Ranked",
                _ => t.ToString()
            };
            AddTypeTab(t, label, false);
        }
    }

    private void AddTypeTab(TournamentType? type, string label, bool isPremiumTab,
                            bool isCustomTab = false)
    {
        bool active = isCustomTab  ? _isCustom
                    : isPremiumTab ? (_premiumOnly && !_isCustom)
                    : (!_premiumOnly && !_isCustom && _activeType == type);

        Color bg = isCustomTab  ? (active ? Color.FromArgb("#7B68EE") : Color.FromArgb("#1E1A3A"))
                 : isPremiumTab ? (active ? Color.FromArgb("#FFD700")  : Color.FromArgb("#3A2800"))
                 : (active      ? Color.FromArgb("#FFD700")            : Color.FromArgb("#1C2A4A"));

        var btn = new Button
        {
            Text            = label,
            FontSize        = 12,
            CornerRadius    = 16,
            Padding         = new Thickness(14, 6),
            BackgroundColor = bg,
            TextColor       = active ? Colors.Black : Colors.White
        };
        btn.Clicked += (_, _) =>
        {
            _isCustom    = isCustomTab;
            _premiumOnly = !isCustomTab && isPremiumTab;
            _activeType  = (isCustomTab || isPremiumTab) ? null : type;
            BuildTypeTabs();
            if (_isCustom) { FilterScrollView.IsVisible = false; TableHeader.IsVisible = false;
                             RoomScrollView.IsVisible = false;   CustomPanel.IsVisible = true;
                             BuildCustomPanel(); }
            else           { FilterScrollView.IsVisible = true;  TableHeader.IsVisible = true;
                             RoomScrollView.IsVisible = true;    CustomPanel.IsVisible = false;
                             BuildFilterBar(); RefreshList(); }
        };
        TypeTabBar.Children.Add(btn);
    }

    // -----------------------------------------------------------------------
    // Filtros de buy-in (adapta conforme tab ativo)
    // -----------------------------------------------------------------------
    private void BuildFilterBar()
    {
        FilterBar.Children.Clear();

        decimal[] filters = _premiumOnly
            ? [0, 500, 1000, 2500]
            : BuyInFilters;

        foreach (var f in filters)
        {
            string label = f == 0 ? "Todos" : $"$ {f:N0}";
            bool active  = f == _activeFilter;
            var btn = new Button
            {
                Text            = label,
                FontSize        = 11,
                CornerRadius    = 14,
                Padding         = new Thickness(12, 4),
                BackgroundColor = active
                    ? (_premiumOnly ? Color.FromArgb("#B8860B") : Color.FromArgb("#0F3460"))
                    : Color.FromArgb("#252B45"),
                TextColor = Colors.White
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
        var all = AppState.Current.RoomLobby.Rooms.AsEnumerable();

        if (_premiumOnly)
        {
            // Tab Premium: apenas salas de alto valor
            all = all.Where(r => r.IsHighStakes);
        }
        else if (_activeType != null)
        {
            // Tab de tipo específico: filtra por tipo (inclui premium dessa categoria)
            all = all.Where(r => r.Type == _activeType);
        }
        // else: "Todos" — mostra tudo

        if (_activeFilter > 0)
            all = all.Where(r => r.BuyIn == _activeFilter);

        var rooms = all
            .OrderBy(r => r.BuyIn).ThenBy(r => r.Type).ThenBy(r => r.Size)
            .ToList();

        RoomList.Children.Clear();

        if (_premiumOnly)
        {
            // Agrupa por evento (Grand Prix / Master Series / Elite Cup)
            foreach (var group in rooms.GroupBy(r => r.HighStakesName))
            {
                RoomList.Children.Add(BuildPremiumHeader(group.Key, group.First().BuyIn));
                bool a = false;
                foreach (var room in group) { RoomList.Children.Add(BuildRow(room, a)); a = !a; }
            }
        }
        else
        {
            bool alt = false;
            foreach (var room in rooms) { RoomList.Children.Add(BuildRow(room, alt)); alt = !alt; }
        }
    }

    private View BuildPremiumHeader(string name, decimal buyIn)
    {
        string subtitle = buyIn switch
        {
            >= 2500 => "Buy-in $ 2.500  ·  Pool até $ 40.000",
            >= 1000 => "Buy-in $ 1.000  ·  Pool até $ 16.000",
            _       => "Buy-in $ 500  ·  Pool até $ 8.000"
        };
        return new Frame
        {
            BackgroundColor = Color.FromArgb("#2A1D00"),
            BorderColor     = Color.FromArgb("#FFD700"),
            CornerRadius    = 0,
            Padding         = new Thickness(14, 8),
            HasShadow       = false,
            Content         = new VerticalStackLayout
            {
                Children =
                {
                    new Label { Text = $"💎  {name}", TextColor = Color.FromArgb("#FFD700"),
                                FontSize = 14, FontAttributes = FontAttributes.Bold },
                    new Label { Text = subtitle, TextColor = Color.FromArgb("#CCAA55"), FontSize = 10 }
                }
            }
        };
    }

    private Grid BuildRow(TournamentRoom room, bool alt)
    {
        var profile    = AppState.Current.Profile;
        bool hasTicket = profile.HasTicket(room.BuyIn);

        // Fundo especial para alto valor
        Color bg = room.IsHighStakes
            ? Color.FromArgb("#1C1400")
            : (alt ? Color.FromArgb("#16213E") : Color.FromArgb("#12192E"));

        var row = new Grid
        {
            ColumnDefinitions = { new(GridLength.Star), new(72), new(72), new(72), new(60) },
            BackgroundColor   = bg,
            Padding           = new Thickness(14, 9)
        };

        // Coluna 0: badge tipo + nome + status
        var nameStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Spacing = 2 };

        // Badge colorido do tipo + alto valor
        var badgeRow = new HorizontalStackLayout { Spacing = 6 };
        if (room.IsHighStakes)
            badgeRow.Add(new Label { Text = "💎", FontSize = 11 });
        if (!string.IsNullOrEmpty(room.TypeBadge))
            badgeRow.Add(new Label { Text = room.TypeBadge, FontSize = 11 });
        badgeRow.Add(new Label
        {
            Text      = room.IsHighStakes ? $"{room.HighStakesName} · {room.TypeLabel}" : room.TypeLabel,
            TextColor = room.IsHighStakes ? Color.FromArgb("#FFD700") : TypeColor(room.Type),
            FontSize  = 10, FontAttributes = FontAttributes.Bold
        });
        if (hasTicket)
            badgeRow.Add(new Label { Text = "🎟 Ticket", TextColor = Color.FromArgb("#4CAF50"), FontSize = 10, FontAttributes = FontAttributes.Bold });
        nameStack.Add(badgeRow);

        // Nome + tamanho
        string roomTitle = room.IsHighStakes
            ? $"{room.HighStakesName}  {SizeEmoji(room.Size)}"
            : $"{SizeEmoji(room.Size)}  #{room.Id}";
        nameStack.Add(new Label
        {
            Text      = roomTitle,
            TextColor = room.IsHighStakes ? Color.FromArgb("#FFD700") : Colors.White,
            FontSize  = 13, FontAttributes = FontAttributes.Bold
        });

        // Info extra por tipo
        string satelliteTargetName = room.SatelliteTarget switch
        {
            >= 2500 => "Elite Cup",
            >= 1000 => "Master Series",
            >= 500  => "Grand Prix",
            _       => $"$ {room.SatelliteTarget:N0}"
        };
        string extra = room.Type switch
        {
            TournamentType.Bounty    => $"🎯 Bounty: $ {room.BountyPerPlayer:N0}/elim.",
            TournamentType.Satellite => $"🎟 Vaga: {satelliteTargetName} ($ {room.SatelliteTarget:N0})",
            TournamentType.Ranked    => "🏅 Classificatório",
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
        string buyInText = hasTicket ? $"🎟\n$ {room.BuyIn:N0}" : $"$ {room.BuyIn:N0}";
        var buyInLbl = new Label
        {
            Text      = buyInText,
            TextColor = hasTicket ? Color.FromArgb("#4CAF50") : Color.FromArgb("#FFD700"),
            FontSize  = room.IsHighStakes ? 13 : 12, FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(buyInLbl, 2);
        row.Add(buyInLbl);

        // Prêmio
        string prizeText = room.Type == TournamentType.Satellite
            ? $"🎟 Vaga\n{satelliteTargetName}"
            : room.IsHighStakes
                ? $"💎 $ {room.PrizePool:N0}"
                : $"$ {room.PrizePool:N0}";
        var prizeLbl = new Label
        {
            Text = prizeText,
            TextColor = room.IsHighStakes ? Color.FromArgb("#FFD700") : Color.FromArgb("#4CAF50"),
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

    // -----------------------------------------------------------------------
    // Painel personalizado
    // -----------------------------------------------------------------------
    private static readonly (string Label, TournamentType Type)[] CustomTypes =
    [
        ("Standard",    TournamentType.Standard),
        ("⚔ Duelo",    TournamentType.HeadsUp),
        ("🎯 Bounty",   TournamentType.Bounty),
        ("⚡ Turbo",    TournamentType.Turbo),
        ("🔥 Hyper",    TournamentType.HyperTurbo),
        ("🏅 Ranked",   TournamentType.Ranked),
    ];
    private static readonly int[]     CustomSizes   = [2, 8, 16, 32, 64];
    private static readonly int[]     CustomTimes   = [0, 1, 2, 3, 5, 10];
    private static readonly decimal[] CustomBuyIns  = [10, 25, 50, 100, 250, 500];

    // Label de resumo — atualizada a cada seleção
    private Label? _customSummaryLabel;
    private Button? _createBtn;

    private void BuildCustomPanel()
    {
        CustomContainer.Children.Clear();

        // ── Tipo ────────────────────────────────────────────────────────────
        CustomContainer.Children.Add(BuildCustomSection("Tipo de Torneio",
            CustomTypes.Select(ct => ct.Label).ToArray(),
            CustomTypes.Select(ct => (object)ct.Type).ToArray(),
            _customType,
            v => { _customType = (TournamentType)v; RebuildCustomSummary(); }));

        // ── Nº de Jogadores ─────────────────────────────────────────────────
        // Duelo força 2 jogadores
        var sizesAvail = _customType == TournamentType.HeadsUp ? [2] : CustomSizes;
        if (_customType == TournamentType.HeadsUp) _customSize = 2;
        CustomContainer.Children.Add(BuildCustomSection("Nº de Jogadores",
            sizesAvail.Select(s => SizeEmoji(s)).ToArray(),
            sizesAvail.Select(s => (object)s).ToArray(),
            _customSize,
            v => { _customSize = (int)v; RebuildCustomSummary(); }));

        // ── Tempo por Partida ───────────────────────────────────────────────
        // HyperTurbo fixa 1 min; Turbo limita a ≤ 2 min
        int[]  timesAvail = _customType == TournamentType.HyperTurbo ? [1]
                          : _customType == TournamentType.Turbo       ? [0, 1, 2]
                          : CustomTimes;
        if (_customType == TournamentType.HyperTurbo) _customTime = 1;
        if (_customType == TournamentType.Turbo && _customTime > 2) _customTime = 2;
        CustomContainer.Children.Add(BuildCustomSection("Tempo por Partida",
            timesAvail.Select(t => t == 0 ? "Livre" : $"{t} min").ToArray(),
            timesAvail.Select(t => (object)t).ToArray(),
            _customTime,
            v => { _customTime = (int)v; RebuildCustomSummary(); }));

        // ── Buy-in ──────────────────────────────────────────────────────────
        CustomContainer.Children.Add(BuildCustomSection("Buy-in",
            CustomBuyIns.Select(b => $"$ {b:N0}").ToArray(),
            CustomBuyIns.Select(b => (object)b).ToArray(),
            _customBuyIn,
            v => { _customBuyIn = (decimal)v; RebuildCustomSummary(); }));

        // ── Resumo + botão ──────────────────────────────────────────────────
        _customSummaryLabel = new Label
        {
            TextColor = Color.FromArgb("#4CAF50"),
            FontSize  = 13,
            HorizontalTextAlignment = TextAlignment.Center
        };

        _createBtn = new Button
        {
            Text            = "CRIAR TORNEIO",
            BackgroundColor = Color.FromArgb("#7B68EE"),
            TextColor       = Colors.White,
            FontAttributes  = FontAttributes.Bold,
            FontSize        = 15,
            CornerRadius    = 12,
            HeightRequest   = 52
        };
        _createBtn.Clicked += async (_, _) => await OnCreateCustomTournament();

        var summaryFrame = new Frame
        {
            BackgroundColor = Color.FromArgb("#16213E"),
            BorderColor     = Color.FromArgb("#7B68EE"),
            CornerRadius    = 12,
            Padding         = new Thickness(16, 14),
            HasShadow       = false,
            Content         = new VerticalStackLayout
            {
                Spacing  = 12,
                Children = { _customSummaryLabel, _createBtn }
            }
        };
        CustomContainer.Children.Add(summaryFrame);
        RebuildCustomSummary();
    }

    private View BuildCustomSection(string title, string[] labels, object[] values,
                                    object selected, Action<object> onSelect)
    {
        var section = new VerticalStackLayout { Spacing = 8 };
        section.Add(new Label
        {
            Text = title, TextColor = Color.FromArgb("#AAAACC"),
            FontSize = 12, FontAttributes = FontAttributes.Bold
        });

        var chips = new FlexLayout
        {
            Wrap            = Microsoft.Maui.Layouts.FlexWrap.Wrap,
            JustifyContent  = Microsoft.Maui.Layouts.FlexJustify.Start,
            Direction       = Microsoft.Maui.Layouts.FlexDirection.Row
        };

        for (int i = 0; i < labels.Length; i++)
        {
            var capturedVal   = values[i];
            bool isActive     = capturedVal.Equals(selected);
            var chip = new Button
            {
                Text            = labels[i],
                FontSize        = 12,
                CornerRadius    = 20,
                Padding         = new Thickness(16, 8),
                Margin          = new Thickness(0, 0, 6, 6),
                BackgroundColor = isActive ? Color.FromArgb("#7B68EE") : Color.FromArgb("#252B45"),
                TextColor       = isActive ? Colors.White : Color.FromArgb("#AAAACC")
            };
            chip.Clicked += (_, _) => { onSelect(capturedVal); BuildCustomPanel(); };
            chips.Add(chip);
        }
        section.Add(chips);
        return section;
    }

    private void RebuildCustomSummary()
    {
        if (_customSummaryLabel == null || _createBtn == null) return;

        decimal pool  = _customBuyIn * _customSize;
        string  time  = _customTime == 0 ? "Livre" : $"{_customTime} min";
        string  type  = CustomTypes.FirstOrDefault(ct => ct.Type == _customType).Label;

        _customSummaryLabel.Text =
            $"{type}  ·  {_customSize} jogadores  ·  {time}\n" +
            $"Buy-in: $ {_customBuyIn:N0}  →  Prêmio total: $ {pool:N0}";

        bool canAfford = AppState.Current.Profile.Balance >= _customBuyIn ||
                         AppState.Current.Profile.HasTicket(_customBuyIn);
        _createBtn.BackgroundColor = canAfford
            ? Color.FromArgb("#7B68EE")
            : Color.FromArgb("#555555");
    }

    private async Task OnCreateCustomTournament()
    {
        var profile = AppState.Current.Profile;

        bool usedTicket = profile.UseTicket(_customBuyIn);
        if (!usedTicket)
        {
            if (!profile.TryDebit(_customBuyIn))
            {
                await DisplayAlert("Saldo insuficiente",
                    $"Você precisa de $ {_customBuyIn:N0}.\nSaldo: $ {profile.Balance:N0}", "OK");
                return;
            }
        }
        else
        {
            await DisplayAlert("🎟 Ticket Utilizado!",
                $"Vaga gratuita no torneio de $ {_customBuyIn:N0} ativada!", "Ótimo!");
        }

        decimal bounty = _customType == TournamentType.Bounty
            ? Math.Round(_customBuyIn * 0.3m, 0)
            : 0;

        AppState.Current.Matchmaking.CreateRoom(
            _customSize, _customBuyIn, _customTime,
            profile.Name, profile.Avatar,
            _customType, satelliteTarget: 0);

        await Shell.Current.GoToAsync("WaitingRoomPage");
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

        // Tenta usar ticket primeiro; senão debita fichas
        bool usedTicket = profile.UseTicket(room.BuyIn);
        if (!usedTicket)
        {
            if (!profile.TryDebit(room.BuyIn))
            {
                // Verifica se tem satélite para sugerir
                bool hasSatellite = AppState.Current.RoomLobby.Rooms
                    .Any(r => r.Type == TournamentType.Satellite && r.SatelliteTarget == room.BuyIn && r.CanJoin);

                string tip = hasSatellite
                    ? "\n\n💡 Jogue um Bilhete Dourado para ganhar uma vaga gratuita!"
                    : "";
                await DisplayAlert("Saldo insuficiente",
                    $"Você precisa de $ {room.BuyIn:N0}.\nSaldo: $ {profile.Balance:N0}{tip}", "OK");
                return;
            }
        }
        else
        {
            await DisplayAlert("🎟 Ticket Utilizado!",
                $"Vaga gratuita no torneio de $ {room.BuyIn:N0} ativada!", "Ótimo!");
        }

        AppState.Current.RoomLobby.JoinRoom(room);
        AppState.Current.Matchmaking.CreateRoom(
            room.Size, room.BuyIn, room.TimeMinutes,
            profile.Name, profile.Avatar,
            room.Type, room.SatelliteTarget);

        await Shell.Current.GoToAsync("WaitingRoomPage");
    }
}
