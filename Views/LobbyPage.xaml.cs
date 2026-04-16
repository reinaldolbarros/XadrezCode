using ChessMAUI.Services;

namespace ChessMAUI.Views;

public partial class LobbyPage : ContentPage
{
    private static readonly string[] Avatars =
        ["♟","♛","♚","♜","♝","♞","🎯","🔥","💎","👑","🦁","🐉","⚡","🌟","🎭","🛡️"];

    public LobbyPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var profile = AppState.Current.Profile;

        if (profile.IsNew)
        {
            string? name = await DisplayPromptAsync(
                "Bem-vindo!", "Qual é o seu nome?",
                maxLength: 20, keyboard: Keyboard.Text);

            profile.Name   = string.IsNullOrWhiteSpace(name) ? "Jogador" : name.Trim();
            profile.Avatar = "♟";
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        var p     = AppState.Current.Profile;
        var daily = AppState.Current.Daily;

        // Perfil
        AvatarLabel.Text    = p.Avatar;
        NameLabel.Text      = p.Name;
        TierLabel.Text      = p.TierIcon;
        TierName.Text       = p.TierName;
        BalanceLabel.Text   = $"$ {p.Balance:N0}";
        WinsLabel.Text      = p.Wins.ToString();
        LossesLabel.Text    = p.Losses.ToString();
        TournWinsLabel.Text = p.TournamentsWon.ToString();

        // Tickets de satélite (exibe abaixo do nome se houver algum)
        var tickets = p.GetAllTickets();
        if (tickets.Count > 0)
        {
            string ticketStr = string.Join("  ", tickets.Select(kv => $"🎟 ${kv.Key:N0}×{kv.Value}"));
            RatingLabel.Text = $"{p.Points:N0} pts  ·  {ticketStr}";
        }
        else
        {
            RatingLabel.Text = $"{p.Points:N0} pts";
        }

        // Bônus diário
        bool claimed = daily.BonusClaimedToday;
        BonusBtn.IsVisible        = !claimed;
        BonusStreakLabel.Text     = $"Sequência: {daily.LoginStreak} dia{(daily.LoginStreak != 1 ? "s" : "")}  🔥";
        BonusFrame.BorderColor    = claimed ? Color.FromArgb("#333355") : Color.FromArgb("#FFD700");
        BonusTitle.Text           = claimed ? "🎁  Bônus Diário  ✓ Resgatado" : "🎁  Bônus Diário";
        BonusTitle.TextColor      = claimed ? Color.FromArgb("#666688") : Color.FromArgb("#FFD700");

        // Missões
        BuildMissions(daily);

        // Mini ranking
        BuildMiniRanking();
    }

    // -----------------------------------------------------------------------
    // Bônus diário
    // -----------------------------------------------------------------------
    private async void OnBonusClicked(object? sender, EventArgs e)
    {
        var state = AppState.Current;
        int fichas = state.Daily.ClaimDailyBonus();
        state.Profile.Credit(fichas);

        int streak = state.Daily.LoginStreak;
        string next = streak switch { >= 7 => "Máximo!", >= 5 => "7 dias = 500 fichas", >= 3 => "5 dias = 300 fichas", >= 2 => "3 dias = 200 fichas", _ => "2 dias = 150 fichas" };
        await DisplayAlert("🎁 Bônus Diário!",
            $"+{fichas} fichas\n\n🔥 Sequência: {streak} dia{(streak != 1 ? "s" : "")}\n\n➡ Próximo prêmio: {next}", "Ótimo!");

        RefreshUI();
    }

    // -----------------------------------------------------------------------
    // Missões diárias
    // -----------------------------------------------------------------------
    private void BuildMissions(DailyService daily)
    {
        MissionContainer.Children.Clear();
        var missions = daily.GetMissions();
        for (int i = 0; i < missions.Count; i++)
        {
            var m = missions[i];
            var row = new Grid { ColumnDefinitions = { new(GridLength.Auto), new(GridLength.Star), new(GridLength.Auto) }, Margin = new Thickness(0,2) };

            var icon = new Label { Text = m.Icon, FontSize = 18, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(0,0,8,0) };
            row.Add(icon);

            var info = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };
            info.Add(new Label { Text = m.Description, TextColor = m.Completed ? Color.FromArgb("#4CAF50") : Colors.White, FontSize = 12 });
            // Progress bar
            var barTrack = new Grid { HeightRequest = 5 };
            barTrack.Add(new BoxView { Color = Color.FromArgb("#0F3460"), CornerRadius = 2, HorizontalOptions = LayoutOptions.Fill });
            double pct = m.Target > 0 ? Math.Min(1.0, (double)m.Progress / m.Target) : 0;
            double missionBarWidth = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density - 130) * pct;
            barTrack.Add(new BoxView { Color = m.Completed ? Color.FromArgb("#4CAF50") : Color.FromArgb("#7B68EE"),
                CornerRadius = 2, HorizontalOptions = LayoutOptions.Start, WidthRequest = Math.Max(0, missionBarWidth) });
            info.Add(barTrack);
            info.Add(new Label { Text = $"{m.Progress}/{m.Target}", TextColor = Color.FromArgb("#AAAACC"), FontSize = 10 });
            Grid.SetColumn(info, 1);
            row.Add(info);

            var reward = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End, Spacing = 1 };
            reward.Add(new Label { Text = $"+{m.BalanceReward}$", TextColor = Color.FromArgb("#4CAF50"), FontSize = 10, HorizontalTextAlignment = TextAlignment.End });
            Grid.SetColumn(reward, 2);
            row.Add(reward);

            MissionContainer.Children.Add(row);

            // Separator (except last)
            if (i < missions.Count - 1)
                MissionContainer.Children.Add(new BoxView { Color = Color.FromArgb("#0F3460"), HeightRequest = 1, Margin = new Thickness(0,2) });
        }
    }

    // -----------------------------------------------------------------------
    // Mini ranking (top 5)
    // -----------------------------------------------------------------------
    private void BuildMiniRanking()
    {
        MiniRankContainer.Children.Clear();
        var profile = AppState.Current.Profile;
        var entries = AppState.Current.Ranking.GetGlobal(profile).Take(5);

        foreach (var e in entries)
        {
            var row = new Grid { ColumnDefinitions = { new(32), new(GridLength.Auto), new(GridLength.Star), new(GridLength.Auto) }, Margin = new Thickness(0,1) };

            row.Add(new Label { Text = e.PositionLabel, FontSize = e.Position <= 3 ? 15 : 12,
                TextColor = e.Position <= 3 ? Colors.White : Color.FromArgb("#AAAACC"),
                HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center });

            var tierLbl = new Label { Text = e.TierIcon, FontSize = 14, Margin = new Thickness(4,0),
                HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center };
            Grid.SetColumn(tierLbl, 1);
            row.Add(tierLbl);

            var nameStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center };
            nameStack.Add(new Label { Text = e.Name, TextColor = e.NameColor, FontSize = 12,
                FontAttributes = e.IsHuman ? FontAttributes.Bold : FontAttributes.None });
            nameStack.Add(new Label { Text = e.TierName, TextColor = Color.FromArgb("#888899"), FontSize = 9 });
            Grid.SetColumn(nameStack, 2);
            row.Add(nameStack);

            var pts = new Label { Text = $"{e.Points:N0} pts", TextColor = Color.FromArgb("#4CAF50"),
                FontSize = 11, FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End };
            Grid.SetColumn(pts, 3);
            row.Add(pts);

            if (e.IsHuman)
            {
                var highlight = new BoxView { Color = Color.FromArgb("#1C2A0A"), CornerRadius = 4 };
                row.BackgroundColor = Color.FromArgb("#1C2A0A");
            }

            MiniRankContainer.Children.Add(row);
        }
    }

    // -----------------------------------------------------------------------
    // Admin: 5 toques rápidos no saldo ativa/desativa
    // -----------------------------------------------------------------------
    private int      _adminTapCount = 0;
    private DateTime _lastAdminTap  = DateTime.MinValue;

    private async void OnAdminActivate(object? sender, TappedEventArgs e)
    {
        var now = DateTime.UtcNow;
        // Reseta contador se passou mais de 1,5s desde o último toque
        if ((now - _lastAdminTap).TotalSeconds > 1.5)
            _adminTapCount = 0;

        _lastAdminTap = now;
        _adminTapCount++;

        if (_adminTapCount < 5) return;
        _adminTapCount = 0;

        AppState.Current.IsAdminMode = !AppState.Current.IsAdminMode;
        string msg = AppState.Current.IsAdminMode
            ? "⚙ MODO ADMIN ATIVADO\nBotões de teste disponíveis no jogo."
            : "Modo admin desativado.";
        await DisplayAlert("Admin", msg, "OK");
    }

    // -----------------------------------------------------------------------
    // Avatar: toque para escolher
    // -----------------------------------------------------------------------
    private async void OnAvatarTapped(object? sender, EventArgs e)
    {
        string? choice = await DisplayActionSheet("Escolha seu avatar", "Cancelar", null, Avatars);
        if (choice == null || choice == "Cancelar") return;
        AppState.Current.Profile.Avatar = choice;
        AvatarLabel.Text = choice;
    }

    // -----------------------------------------------------------------------
    // Navegação
    // -----------------------------------------------------------------------
    private async void OnTournamentsClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("TournamentLobbyPage");

    private async void OnHistoryClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("TournamentHistoryPage");

    private async void OnRankingClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("RankingPage");

    private async void OnQuickPlayClicked(object? sender, EventArgs e)
    {
        AppState.Current.PendingTournamentGame = false;
        await Shell.Current.GoToAsync("GamePage");
    }
}
