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

        // Visitantes pulam o cadastro de perfil
        if (profile.IsNew && !AppState.Current.Auth.IsAnonymous)
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

        // Localização inline com o nome
        string loc = p.Country.Length > 0 && p.State.Length > 0 ? $"{p.Country} · {p.State}"
                   : p.Country.Length > 0 ? p.Country
                   : p.State.Length > 0   ? p.State : "";
        ProfileLocationLabel.Text = loc;
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
        BonusTitle.Text           = claimed ? "Bônus Diário  ·  Resgatado ✓" : "Bônus Diário  ·  Disponível!";
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

        // Modo Carreira — subtítulo dinâmico
        var career = AppState.Current.Career.Progress;
        CareerSubLabel.Text = career.IsCareerCompleted
            ? "🏆 Campeão Mundial de Xadrez"
            : career.ActiveTournament != null
                ? $"Nível {(int)career.CurrentLevel + 1}/7  ·  {career.ActiveTournament.LevelName}"
                : "Do Local ao Campeonato Mundial";

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
        WeekChampAvatar.Text     = weekly.Avatar;
        WeekChampName.Text       = weekly.Name;
        WeekChampName.TextColor  = weekly.IsHuman ? Color.FromArgb("#4CAF50") : Colors.White;

        var monthly = state.Season.GetMonthlyLeader(state.Titles, state.Profile);
        MonthChampAvatar.Text    = monthly.Avatar;
        MonthChampName.Text      = monthly.Name;
        MonthChampName.TextColor = monthly.IsHuman ? Color.FromArgb("#4CAF50") : Colors.White;
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

        // Pega top 5; se o humano estiver fora do top 5, adiciona ao final
        var human   = board.FirstOrDefault(e => e.IsHuman);
        var toShow  = board.Take(5).ToList();
        bool humanOutside = human != null && human.Position > 5;
        if (humanOutside) toShow.Add(human!);

        bool separatorAdded = false;

        foreach (var e in toShow)
        {
            if (humanOutside && e.IsHuman && !separatorAdded)
            {
                separatorAdded = true;
                MiniRankContainer.Children.Add(new Label
                {
                    Text = "·  ·  ·", TextColor = Color.FromArgb("#3A5070"),
                    HorizontalTextAlignment = TextAlignment.Center, FontSize = 10, Margin = new Thickness(0, 1)
                });
            }

            bool isPodium = e.Position <= 3;

            // Cores por posição
            string bgHex  = e.IsHuman ? "#1C2A0A" : "transparent";
            string posColor = e.Position switch { 1 => "#FFD700", 2 => "#C0C0D0", 3 => "#CD7F32", _ => "#7090B0" };
            string medal    = e.PositionLabel;

            var row = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection(new(26), new(GridLength.Star), new(GridLength.Auto)),
                Padding         = new Thickness(2, 2),
                BackgroundColor = bgHex == "transparent" ? Colors.Transparent : Color.FromArgb(bgHex)
            };

            // Posição
            row.Add(new Label
            {
                Text = medal, FontSize = 12,
                TextColor = Color.FromArgb(posColor),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center
            });

            // Nome + localização inline (sem avatar)
            string subBadge = e.IsHuman && sub.IsActive ? $" {sub.BadgeIcon}" : "";
            string loc      = e.LocationLabel;
            var nameLbl     = new Label { VerticalOptions = LayoutOptions.Center };
            var fmt         = new FormattedString();
            fmt.Spans.Add(new Span
            {
                Text           = e.Name + subBadge,
                TextColor      = e.IsHuman ? Color.FromArgb("#4CAF50") : e.NameColor,
                FontSize       = 13,
                FontAttributes = e.IsHuman ? FontAttributes.Bold : FontAttributes.None
            });
            if (!string.IsNullOrEmpty(loc))
                fmt.Spans.Add(new Span
                {
                    Text      = $"  {loc}",
                    TextColor = Color.FromArgb("#506070"),
                    FontSize  = 11
                });
            nameLbl.FormattedText = fmt;
            Grid.SetColumn(nameLbl, 1);
            row.Add(nameLbl);

            // Pontos
            var pts = new Label
            {
                Text = $"{e.Points:N0} pts",
                TextColor = Color.FromArgb(e.Position == 1 ? "#FFD700" : e.Position == 2 ? "#C0C0D0" : e.Position == 3 ? "#CD7F32" : "#607890"),
                FontSize = 12,
                VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End
            };
            Grid.SetColumn(pts, 2);
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

    private async void OnCareerClicked(object? sender, TappedEventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("CareerPage");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro navegação", ex.Message + "\n\n" + ex.GetType().Name, "OK");
        }
    }

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
        await Shell.Current.GoToAsync("FriendInvitePage");
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
