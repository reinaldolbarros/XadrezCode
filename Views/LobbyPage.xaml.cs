using ChessMAUI.Services;

namespace ChessMAUI.Views;

public partial class LobbyPage : ContentPage
{
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
            await Shell.Current.GoToAsync("ProfilePage");
            return;
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        var p     = AppState.Current.Profile;
        var daily = AppState.Current.Daily;

        // Perfil — avatar (foto ou emoji)
        bool hasPhoto = !string.IsNullOrEmpty(p.AvatarPath) && File.Exists(p.AvatarPath);
        AvatarImage.IsVisible = hasPhoto;
        AvatarLabel.IsVisible = !hasPhoto;
        if (hasPhoto) AvatarImage.Source = ImageSource.FromFile(p.AvatarPath);
        else          AvatarLabel.Text   = p.Avatar;

        NameLabel.Text      = p.Name;
        TierLabel.Text      = p.TierIcon;
        TierName.Text       = p.TierName;
        BalanceLabel.Text   = $"$ {p.Balance:N0}";
        WinsLabel.Text      = p.Wins.ToString();
        LossesLabel.Text    = p.Losses.ToString();
        TournWinsLabel.Text = p.TournamentsWon.ToString();

        // Tickets de satélite
        var tickets = p.GetAllTickets();
        string ticketStr = tickets.Count > 0
            ? "  ·  " + string.Join("  ", tickets.Select(kv => $"${kv.Key:N0}×{kv.Value}"))
            : "";

        // Bônus diário
        bool claimed = daily.BonusClaimedToday;
        BonusBtn.IsVisible        = !claimed;
        BonusStreakLabel.Text     = $"Sequência: {daily.LoginStreak} dia{(daily.LoginStreak != 1 ? "s" : "")}";
        BonusFrame.Stroke         = new SolidColorBrush(claimed ? Color.FromArgb("#1A2840") : Color.FromArgb("#FFD700"));
        BonusTitle.Text           = claimed ? "Bônus Diário  ·  Resgatado" : "Bônus Diário";
        BonusTitle.TextColor      = claimed ? Color.FromArgb("#666688") : Color.FromArgb("#FFD700");

        // Botão admin (visível apenas em modo admin)
        AdminBtn.IsVisible = AppState.Current.IsAdminMode;

        // Banner de assinatura
        var sub = AppState.Current.Subscription;
        if (sub.IsActive)
        {
            SubTitleLabel.Text   = $"{sub.BadgeIcon} Plano {sub.BadgeLabel}";
            SubDetailLabel.Text  = $"Ativo até {sub.ExpiresAt:dd/MM/yyyy} · Sem anúncios";
            SubBanner.Stroke     = new SolidColorBrush(sub.ActiveTier == SubscriptionTier.Ouro
                ? Color.FromArgb("#B8860B") : Color.FromArgb("#2A5090"));
            SubBtn.Text          = "Gerenciar";
        }
        else
        {
            SubTitleLabel.Text   = "Plano Gratuito";
            SubDetailLabel.Text  = "Assine e jogue sem anúncios";
            SubBanner.Stroke     = new SolidColorBrush(Color.FromArgb("#1A2840"));
            SubBtn.Text          = "Ver planos";
        }

        // Prioridade da Liga via Arena Casual
        var casual = AppState.Current.CasualRanking;
        string priorityStr = casual.HasLigaPriority ? "  ·  ⚡ Prioridade Liga" : "";
        RatingLabel.Text = $"{p.Points:N0} pts{ticketStr}{priorityStr}";

        // Banner Arena Casual — barra de progresso + status
        int    wpts      = casual.WeeklyPoints;
        int    threshold = CasualRankingService.PriorityThreshold;
        double fillPct   = Math.Min(1.0, (double)wpts / threshold);
        bool   hasPrio   = casual.HasLigaPriority;

        // Largura da barra (estimada; recalculada no layout)
        double barMax = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density - 120;
        CasualBarFill.WidthRequest = Math.Max(0, barMax * fillPct);
        CasualBarFill.Color        = hasPrio ? Color.FromArgb("#4CAF50") : Color.FromArgb("#3A6AB0");
        CasualPtsLabel.Text        = hasPrio ? "✓ Prioridade" : $"{wpts}/{threshold} pts";
        CasualPtsLabel.TextColor   = hasPrio ? Color.FromArgb("#4CAF50") : Color.FromArgb("#5A7898");

        CasualBorder.Stroke        = new SolidColorBrush(hasPrio
            ? Color.FromArgb("#2A6040") : Color.FromArgb("#2A5090"));
        CasualStatusLabel.Text     = hasPrio
            ? "⚡ Vaga prioritária garantida na Liga esta semana!"
            : wpts > 0
                ? $"Continue jogando — faltam {threshold - wpts} pts para prioridade"
                : "Jogue para garantir vaga prioritária na Liga";
        CasualStatusLabel.TextColor = hasPrio
            ? Color.FromArgb("#4CAF50") : Color.FromArgb("#4A6888");

        // Destaques da Liga
        SeasonSubLabel.Text = AppState.Current.Season.CurrentSeasonLabel;
        BuildChampions(AppState.Current);

        // Missões
        BuildMissions(daily);

        // Mini ranking
        BuildMiniRanking();
    }

    // -----------------------------------------------------------------------
    // Destaques da Liga
    // -----------------------------------------------------------------------
    private void BuildChampions(AppState state)
    {
        var weekly = state.League.GetWeeklyChampion(state.Profile, state.Titles);
        WeekChampAvatar.Text = weekly.Avatar;
        WeekChampName.Text   = weekly.Name;
        WeekChampTitle.Text  = $"{weekly.TitleIcon} {weekly.TitleLabel}";
        WeekChampPoints.Text = $"{weekly.Points:N0} pts";
        if (weekly.IsHuman)
        {
            WeekChampName.TextColor   = Color.FromArgb("#4CAF50");
            WeekChampPoints.TextColor = Color.FromArgb("#4CAF50");
        }
        else
        {
            WeekChampName.TextColor   = Colors.White;
            WeekChampPoints.TextColor = Color.FromArgb("#FFD700");
        }

        var monthly = state.Season.GetMonthlyLeader(state.Titles, state.Profile);
        MonthChampAvatar.Text = monthly.Avatar;
        MonthChampName.Text   = monthly.Name;
        MonthChampTitle.Text  = $"{monthly.TitleIcon} {monthly.TitleLabel}";
        MonthChampPoints.Text = $"{monthly.Points:N0} pts";
        if (monthly.IsHuman)
        {
            MonthChampName.TextColor   = Color.FromArgb("#4CAF50");
            MonthChampPoints.TextColor = Color.FromArgb("#4CAF50");
        }
        else
        {
            MonthChampName.TextColor   = Colors.White;
            MonthChampPoints.TextColor = Color.FromArgb("#FFD700");
        }
    }

    // -----------------------------------------------------------------------
    // Bônus diário
    // -----------------------------------------------------------------------
    private async void OnBonusClicked(object? sender, EventArgs e)
    {
        var state = AppState.Current;
        var sub   = state.Subscription;
        int fichas = state.Daily.ClaimDailyBonus(sub.BonusMultiplier, sub.FlatDailyBonus);
        state.Profile.Credit(fichas, "Bônus Diário", "♟");
        state.Profile.AddPoints(5, "Bônus de login diário", "♟");

        // Missão bônus Ouro: crédito automático diário
        if (sub.ActiveTier == SubscriptionTier.Ouro && sub.ClaimOuroBonusMission())
        {
            state.Profile.Credit(30, "Missão bônus Ouro", "◆");
            fichas += 30;
        }

        int streak = state.Daily.LoginStreak;
        string extra = sub.IsActive ? $"\n{sub.BadgeIcon} Bônus {sub.BadgeLabel} incluído" : "";
        string next = streak switch { >= 7 => "Máximo!", >= 5 => "7 dias = 150 fichas", >= 3 => "5 dias = 100 fichas", >= 2 => "3 dias = 75 fichas", _ => "2 dias = 50 fichas" };
        await DisplayAlert("Bônus Diário",
            $"+{fichas} fichas{extra}\n\nSequência: {streak} dia{(streak != 1 ? "s" : "")}\nPróximo prêmio: {next}", "OK");

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
            var row = new Grid { ColumnDefinitions = { new(GridLength.Star), new(GridLength.Auto) }, Margin = new Thickness(0,2) };

            var info = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };
            info.Add(new Label { Text = m.Description, TextColor = m.Completed ? Color.FromArgb("#4CAF50") : Colors.White, FontSize = 12 });
            var barTrack = new Grid { HeightRequest = 4 };
            barTrack.Add(new BoxView { Color = Color.FromArgb("#1A2840"), CornerRadius = 2, HorizontalOptions = LayoutOptions.Fill });
            double pct = m.Target > 0 ? Math.Min(1.0, (double)m.Progress / m.Target) : 0;
            double missionBarWidth = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density - 100) * pct;
            barTrack.Add(new BoxView { Color = m.Completed ? Color.FromArgb("#4CAF50") : Color.FromArgb("#3A6AB0"),
                CornerRadius = 2, HorizontalOptions = LayoutOptions.Start, WidthRequest = Math.Max(0, missionBarWidth) });
            info.Add(barTrack);
            info.Add(new Label { Text = $"{m.Progress}/{m.Target}", TextColor = Color.FromArgb("#607890"), FontSize = 10 });
            row.Add(info);

            var reward = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End, Spacing = 1 };
            reward.Add(new Label { Text = $"+{m.BalanceReward}", TextColor = Color.FromArgb("#4CAF50"), FontSize = 10, HorizontalTextAlignment = TextAlignment.End });
            Grid.SetColumn(reward, 1);
            row.Add(reward);

            MissionContainer.Children.Add(row);

            // Separator (except last)
            if (i < missions.Count - 1)
                MissionContainer.Children.Add(new BoxView { Color = Color.FromArgb("#1A2840"), HeightRequest = 1, Margin = new Thickness(0,2) });
        }
    }

    // -----------------------------------------------------------------------
    // Mini ranking da temporada da Liga — pódio destacado para top 3
    // -----------------------------------------------------------------------
    private void BuildMiniRanking()
    {
        MiniRankContainer.Children.Clear();
        var state   = AppState.Current;
        var board   = state.Season.GetLeaderboard(state.Titles, state.Profile);
        var sub     = state.Subscription;

        // Pega top 10; se o humano estiver fora do top 10, adiciona ao final
        var human   = board.FirstOrDefault(e => e.IsHuman);
        var toShow  = board.Take(10).ToList();
        bool humanOutside = human != null && human.Position > 10;
        if (humanOutside) toShow.Add(human!);

        bool separatorAdded = false;

        foreach (var e in toShow)
        {
            // Separador "· · ·" antes do jogador se ele está fora do top 5
            if (humanOutside && e.IsHuman && !separatorAdded)
            {
                separatorAdded = true;
                MiniRankContainer.Children.Add(new Label
                {
                    Text = "·  ·  ·", TextColor = Color.FromArgb("#3A5070"),
                    HorizontalTextAlignment = TextAlignment.Center, FontSize = 11, Margin = new Thickness(0, 2)
                });
            }

            bool isPodium = e.Position <= 3;

            // Cores por posição
            string bgHex  = e.IsHuman ? "#1C2A0A" : e.Position switch { 1 => "#1A1400", 2 => "#111118", 3 => "#140C00", _ => "transparent" };
            string posColor = e.Position switch { 1 => "#FFD700", 2 => "#C0C0D0", 3 => "#CD7F32", _ => "#7090B0" };
            string medal    = e.Position switch { 1 => "🥇", 2 => "🥈", 3 => "🥉", _ => "" };

            var row = new Grid
            {
                ColumnDefinitions = isPodium
                    ? new ColumnDefinitionCollection(new(30), new(GridLength.Auto), new(GridLength.Star), new(GridLength.Auto))
                    : new ColumnDefinitionCollection(new(28), new(GridLength.Auto), new(GridLength.Star), new(GridLength.Auto)),
                Margin          = new Thickness(0, isPodium ? 3 : 1),
                Padding         = new Thickness(isPodium ? 6 : 2, isPodium ? 5 : 2),
                BackgroundColor = bgHex == "transparent" ? Colors.Transparent : Color.FromArgb(bgHex)
            };

            // Posição / medalha
            row.Add(new Label
            {
                Text = isPodium ? medal : e.PositionLabel,
                FontSize = isPodium ? 18 : 11,
                TextColor = Color.FromArgb(posColor),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center
            });

            // Avatar
            var avatarLbl = new Label
            {
                Text = e.Avatar, FontSize = isPodium ? 18 : 13,
                Margin = new Thickness(isPodium ? 6 : 4, 0),
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(avatarLbl, 1);
            row.Add(avatarLbl);

            // Nome + título
            string subBadge = e.IsHuman && sub.IsActive ? $" {sub.BadgeIcon}" : "";
            var nameStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Spacing = 1 };
            nameStack.Add(new Label
            {
                Text = e.Name + subBadge,
                TextColor = e.IsHuman ? Color.FromArgb("#4CAF50") : e.NameColor,
                FontSize = isPodium ? 13 : 11,
                FontAttributes = (isPodium || e.IsHuman) ? FontAttributes.Bold : FontAttributes.None
            });
            string loc = e.LocationLabel;
            nameStack.Add(new Label
            {
                Text = string.IsNullOrEmpty(loc) ? "—" : loc,
                TextColor = Color.FromArgb(isPodium ? "#8090B0" : "#506070"),
                FontSize = isPodium ? 10 : 9
            });
            Grid.SetColumn(nameStack, 2);
            row.Add(nameStack);

            // Pontos
            var pts = new Label
            {
                Text = $"{e.Points:N0} pts",
                TextColor = Color.FromArgb(e.Position == 1 ? "#FFD700" : e.Position == 2 ? "#C0C0D0" : e.Position == 3 ? "#CD7F32" : "#607890"),
                FontSize = isPodium ? 12 : 10,
                FontAttributes = isPodium ? FontAttributes.Bold : FontAttributes.None,
                VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End
            };
            Grid.SetColumn(pts, 3);
            row.Add(pts);

            MiniRankContainer.Children.Add(row);
        }
    }

    // -----------------------------------------------------------------------
    // Admin: 5 toques rápidos no saldo ativa/desativa
    // -----------------------------------------------------------------------
    private int      _adminTapCount = 0;
    private DateTime _lastAdminTap  = DateTime.MinValue;

    private async void OnExtractClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("TournamentHistoryPage");

    private async void OnAdminActivate(object? sender, TappedEventArgs e)
    {
        var now = DateTime.UtcNow;
        if ((now - _lastAdminTap).TotalSeconds > 1.5)
            _adminTapCount = 0;

        _lastAdminTap = now;
        _adminTapCount++;

        if (_adminTapCount < 5) return;
        _adminTapCount = 0;

        AppState.Current.IsAdminMode = !AppState.Current.IsAdminMode;
        AdminBtn.IsVisible = AppState.Current.IsAdminMode;

        string msg = AppState.Current.IsAdminMode
            ? "⚙ MODO ADMIN ATIVADO\nBotão admin disponível. Toque em '⚙ Admin' para ver o extrato financeiro."
            : "Modo admin desativado.";
        await DisplayAlert("Admin", msg, "OK");
    }

    private async void OnAdminPageClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("AdminPage");

    // -----------------------------------------------------------------------
    // Avatar: toque → abre ProfilePage
    // -----------------------------------------------------------------------
    private async void OnAvatarTapped(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("ProfilePage");

    // -----------------------------------------------------------------------
    // Navegação
    // -----------------------------------------------------------------------
    private async void OnSubscriptionClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("SubscriptionPage");

    private async void OnSubscriptionBannerTapped(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("SubscriptionPage");

    private async void OnLeagueClicked(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("LeaguePage");

    private async void OnSeasonRankingClicked(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("SeasonRankingPage");

    private async void OnTournamentsClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("TournamentLobbyPage");

    private async void OnHistoryClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("TournamentHistoryPage");

    private async void OnRankingClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("RankingPage");

    private async void OnQuickPlayClicked(object? sender, EventArgs e)
    {
        AppState.Current.PendingTournamentGame = false;
        AppState.Current.PendingFriendGame     = false;
        await Shell.Current.GoToAsync("GamePage");
    }

    private async void OnFriendGameClicked(object? sender, EventArgs e)
    {
        string? opponentName = await DisplayPromptAsync(
            "Jogar com Amigo", "Nome do adversário (Pretas):",
            placeholder: "ex: João", maxLength: 20, keyboard: Keyboard.Default);
        if (string.IsNullOrWhiteSpace(opponentName)) return;

        string[] timeLabels = ["Sem limite", "1 minuto", "3 minutos", "5 minutos", "10 minutos"];
        int[]    timeValues = [0, 1, 3, 5, 10];
        string? timeChoice = await DisplayActionSheet("Tempo por jogador", "Cancelar", null, timeLabels);
        if (timeChoice == null || timeChoice == "Cancelar") return;

        int idx     = Array.IndexOf(timeLabels, timeChoice);
        int minutes = idx >= 0 ? timeValues[idx] : 0;

        var state = AppState.Current;
        state.PendingFriendGame     = true;
        state.PendingTournamentGame = false;
        state.FriendOpponentName    = opponentName.Trim();
        state.FriendTimeMinutes     = minutes;

        await Shell.Current.GoToAsync("GamePage");
    }

    private async void OnChangePasswordClicked(object? sender, EventArgs e)
    {
        var auth = AppState.Current.Auth;

        if (auth.IsAnonymous)
        {
            await DisplayAlert("Aviso", "Visitantes não possuem senha. Crie uma conta para usar esta funcionalidade.", "OK");
            return;
        }

        string? currentPass = await DisplayPromptAsync(
            "Alterar Senha", "Senha atual:", maxLength: 64, keyboard: Keyboard.Default);
        if (currentPass == null) return;

        if (!auth.TryLogin(auth.Email, currentPass) && !auth.TryLogin(auth.Username, currentPass))
        {
            await DisplayAlert("Erro", "Senha atual incorreta.", "OK");
            return;
        }

        string? newPass = await DisplayPromptAsync(
            "Alterar Senha", "Nova senha (mín. 6 caracteres):", maxLength: 64, keyboard: Keyboard.Default);
        if (newPass == null) return;
        if (newPass.Length < 6)
        {
            await DisplayAlert("Erro", "A nova senha deve ter pelo menos 6 caracteres.", "OK");
            return;
        }

        string? confirmPass = await DisplayPromptAsync(
            "Alterar Senha", "Confirmar nova senha:", maxLength: 64, keyboard: Keyboard.Default);
        if (confirmPass == null) return;
        if (newPass != confirmPass)
        {
            await DisplayAlert("Erro", "As senhas não conferem.", "OK");
            return;
        }

        auth.ResetPassword(auth.Email, newPass);
        await DisplayAlert("✓ Concluído", "Sua senha foi alterada com sucesso.", "OK");
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Sair", "Deseja sair da sua conta?", "Sair", "Cancelar");
        if (!confirm) return;

        AppState.Current.Auth.Logout();
        var window = Application.Current?.Windows.FirstOrDefault();
        if (window != null) window.Page = new LoginPage();
    }
}
