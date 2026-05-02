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

        AppState.Current.Stars.RecordOpen();

        var profile = AppState.Current.Profile;

        if (profile.IsNew && !AppState.Current.Auth.IsAnonymous)
        {
            await Shell.Current.GoToAsync("ProfilePage");
            return;
        }

        await RefreshUI();
    }

    private async Task RefreshUI()
    {
        var p     = AppState.Current.Profile;
        var daily = AppState.Current.Daily;

        // File.Exists em background para não bloquear a thread de UI
        bool hasPhoto = !string.IsNullOrEmpty(p.AvatarPath)
            && await Task.Run(() => File.Exists(p.AvatarPath));

        AvatarImage.IsVisible = hasPhoto;
        AvatarLabel.IsVisible = !hasPhoto;
        if (hasPhoto) AvatarImage.Source = ImageSource.FromFile(p.AvatarPath);
        else          AvatarLabel.Text   = p.Avatar;

        NameLabel.Text    = p.Name;

        // Localização inline com o nome
        string loc = p.Country.Length > 0 && p.State.Length > 0 ? $"{p.Country} · {p.State}"
                   : p.Country.Length > 0 ? p.Country
                   : p.State.Length > 0   ? p.State : "";
        ProfileLocationLabel.Text = loc;
        WinsLabel.Text      = p.Wins.ToString();
        LossesLabel.Text    = p.Losses.ToString();
        TournWinsLabel.Text = p.TournamentsWon.ToString();

        // Bônus diário
        bool claimed = daily.BonusClaimedToday;
        BonusBtn.IsVisible        = !claimed;
        BonusArrow.IsVisible      = claimed;
        BonusStreakLabel.Text     = $"Sequência: {daily.LoginStreak} dia{(daily.LoginStreak != 1 ? "s" : "")}";
        BonusFrame.Stroke         = new SolidColorBrush(claimed ? Color.FromArgb("#3A2E68") : Color.FromArgb("#FFD700"));
        BonusTitle.Text           = claimed ? "Bônus Diário  ·  Resgatado ✓" : "Bônus Diário  ·  Disponível!";
        BonusTitle.TextColor      = claimed ? Color.FromArgb("#A56CFF") : Color.FromArgb("#FFD700");

        AdminBtn.IsVisible = AppState.Current.IsAdminMode && !AppState.Current.Auth.IsAnonymous;

        RatingLabel.Text = $"Rating: {p.Points:N0}";

        var starsSvc = AppState.Current.Stars;
        StarsLabel.Text = $"⭐ {starsSvc.Balance} · {starsSvc.DedicationTitle}";

        // Casual / Liga — COMENTADO: aguardando base de jogadores
        // var casual = AppState.Current.CasualRanking;
        // string priorityStr = casual.HasLigaPriority ? "  ·  ⚡ Prioridade Liga" : "";
        // RatingLabel.Text = $"{p.Points:N0} pts{ticketStr}{priorityStr}";
        // int    wpts      = casual.WeeklyPoints;
        // int    threshold = CasualRankingService.PriorityThreshold;
        // double fillPct   = Math.Min(1.0, (double)wpts / threshold);
        // bool   hasPrio   = casual.HasLigaPriority;
        // double barMax = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density - 120;
        // CasualBarFill.WidthRequest = Math.Max(0, barMax * fillPct);
        // CasualBarFill.Color        = hasPrio ? Color.FromArgb("#4CAF50") : Color.FromArgb("#3A6AB0");
        // CasualPtsLabel.Text        = hasPrio ? "✓ Prioridade" : $"{wpts}/{threshold} pts";
        // CasualPtsLabel.TextColor   = hasPrio ? Color.FromArgb("#4CAF50") : Color.FromArgb("#5A7898");
        // CasualBorder.Stroke        = new SolidColorBrush(hasPrio ? Color.FromArgb("#2A6040") : Color.FromArgb("#2A5090"));
        // CasualStatusLabel.Text     = hasPrio ? "⚡ Vaga prioritária garantida na Liga esta semana!"
        //     : wpts > 0 ? $"Continue jogando — faltam {threshold - wpts} pts para prioridade"
        //     : "Jogue para garantir vaga prioritária na Liga";
        // CasualStatusLabel.TextColor = hasPrio ? Color.FromArgb("#4CAF50") : Color.FromArgb("#4A6888");

        // Puzzle do Dia
        var  puzzleSvc  = AppState.Current.PuzzleSvc;
        bool isSub      = AppState.Current.Subscription.IsActive;
        int  doneToday  = puzzleSvc.GetDailyCount();
        bool canPlay    = puzzleSvc.CanPlayMore(isSub);
        var  nextPuzzle = puzzleSvc.GetCurrentPuzzle();
        string stars    = new string('★', nextPuzzle.Difficulty) + new string('☆', 3 - nextPuzzle.Difficulty);
        string countStr = isSub ? $"{doneToday} hoje" : $"{doneToday}/{PuzzleService.FreeLimit} hoje";
        PuzzleSolvedBadge.IsVisible = !canPlay && !isSub;
        int poolSize = puzzleSvc.PoolSize;
        string poolTag = poolSize > 50 ? $"  ·  {poolSize} disponíveis" : "";
        PuzzleSubLabel.Text = canPlay
            ? $"{nextPuzzle.Category}  ·  {stars}  ·  {countStr}{poolTag}"
            : isSub ? $"{nextPuzzle.Category}  ·  {stars}  ·  {countStr}{poolTag}"
                    : $"Limite diário atingido ({PuzzleService.FreeLimit} puzzles)  ·  🔒 Premium";
        PuzzleCard.BackgroundColor = canPlay
            ? Color.FromArgb("#0D1A2A")
            : Color.FromArgb("#0D1010");
        PuzzleCard.Stroke = new SolidColorBrush(canPlay
            ? Color.FromArgb("#1A3A5A")
            : Color.FromArgb("#3A1A1A"));

        // Career.Progress faz JSON deserialization — executa em background
        var career = await Task.Run(() => AppState.Current.Career.Progress);
        string cycleTag = career.TitlesWon > 0 ? $"  ·  {career.TitlesWon}× 🏆" : "";
        CareerSubLabel.Text = career.IsCareerCompleted
            ? $"🏆 Campeão do Ciclo {career.EffectiveCycleYear}{cycleTag}"
            : career.ActiveTournament != null
                ? $"Ciclo {career.EffectiveCycleYear}  ·  {career.ActiveTournament.LevelName}{cycleTag}"
                : "Do Torneio Local ao Campeonato Mundial";

        // Liga — COMENTADO: aguardando base de jogadores
        // SeasonSubLabel.Text = AppState.Current.Season.CurrentSeasonLabel;
        // BuildChampions(AppState.Current);


        // Liga — COMENTADO: aguardando base de jogadores
        // BuildMiniRanking();
    }

    // -----------------------------------------------------------------------
    // Destaques da Liga — COMENTADO: aguardando base de jogadores
    // -----------------------------------------------------------------------
    // private void BuildChampions(AppState state)
    // {
    //     var weekly = state.League.GetWeeklyChampion(state.Profile, state.Titles);
    //     WeekChampAvatar.Text     = weekly.Avatar;
    //     WeekChampName.Text       = weekly.Name;
    //     WeekChampName.TextColor  = weekly.IsHuman ? Color.FromArgb("#4CAF50") : Colors.White;
    //     var monthly = state.Season.GetMonthlyLeader(state.Titles, state.Profile);
    //     MonthChampAvatar.Text    = monthly.Avatar;
    //     MonthChampName.Text      = monthly.Name;
    //     MonthChampName.TextColor = monthly.IsHuman ? Color.FromArgb("#4CAF50") : Colors.White;
    // }

    // -----------------------------------------------------------------------
    // Bônus diário
    // -----------------------------------------------------------------------
    private async void OnBonusClicked(object? sender, EventArgs e)
    {
        var state = AppState.Current;
        int starsBonus = state.Daily.ClaimDailyBonus();
        int starsStreak = state.Daily.TryClaimStreakMission();
        int totalStars = starsBonus + starsStreak;
        if (totalStars > 0) state.Stars.Add(totalStars);

        int streak = state.Daily.LoginStreak;
        string streakLine = starsStreak > 0
            ? $"\nMissão de sequência completa! +{starsStreak} ⭐"
            : streak >= 7 ? "\nSequência máxima mantida!" : "";
        string next = streak switch { >= 7 => "Máximo!", >= 5 => "7 dias = +5 ⭐", >= 3 => "5 dias", >= 2 => "3 dias", _ => "2 dias" };
        await DisplayAlert("Bônus Diário",
            $"+{starsBonus} ⭐  Sequência: {streak} dia{(streak != 1 ? "s" : "")}{streakLine}\nPróximo marco: {next}", "OK");

        await RefreshUI();
    }

    // -----------------------------------------------------------------------
    // Missões diárias
    // -----------------------------------------------------------------------
    // -----------------------------------------------------------------------
    // Mini ranking da Liga — COMENTADO: aguardando base de jogadores
    // -----------------------------------------------------------------------
    // private void BuildMiniRanking()
    // {
    //     MiniRankContainer.Children.Clear();
    //     var state   = AppState.Current;
    //     var board   = state.Season.GetLeaderboard(state.Titles, state.Profile);
    //     var sub     = state.Subscription;
    //     var human   = board.FirstOrDefault(e => e.IsHuman);
    //     var toShow  = board.Take(5).ToList();
    //     bool humanOutside = human != null && human.Position > 5;
    //     if (humanOutside) toShow.Add(human!);
    //     bool separatorAdded = false;
    //     foreach (var e in toShow)
    //     {
    //         if (humanOutside && e.IsHuman && !separatorAdded)
    //         {
    //             separatorAdded = true;
    //             MiniRankContainer.Children.Add(new Label { Text = "·  ·  ·", TextColor = Color.FromArgb("#3A5070"),
    //                 HorizontalTextAlignment = TextAlignment.Center, FontSize = 10, Margin = new Thickness(0, 1) });
    //         }
    //         string bgHex    = e.IsHuman ? "#1C2A0A" : "transparent";
    //         string posColor = e.Position switch { 1 => "#FFD700", 2 => "#C0C0D0", 3 => "#CD7F32", _ => "#7090B0" };
    //         string medal    = e.PositionLabel;
    //         var row = new Grid
    //         {
    //             ColumnDefinitions = new ColumnDefinitionCollection(new(26), new(GridLength.Star), new(GridLength.Auto)),
    //             Padding = new Thickness(2, 2),
    //             BackgroundColor = bgHex == "transparent" ? Colors.Transparent : Color.FromArgb(bgHex)
    //         };
    //         row.Add(new Label { Text = medal, FontSize = 12, TextColor = Color.FromArgb(posColor),
    //             HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center });
    //         string subBadge = e.IsHuman && sub.IsActive ? $" {sub.BadgeIcon}" : "";
    //         string loc      = e.LocationLabel;
    //         var nameLbl = new Label { VerticalOptions = LayoutOptions.Center };
    //         var fmt     = new FormattedString();
    //         fmt.Spans.Add(new Span { Text = e.Name + subBadge,
    //             TextColor = e.IsHuman ? Color.FromArgb("#4CAF50") : e.NameColor, FontSize = 13,
    //             FontAttributes = e.IsHuman ? FontAttributes.Bold : FontAttributes.None });
    //         if (!string.IsNullOrEmpty(loc))
    //             fmt.Spans.Add(new Span { Text = $"  {loc}", TextColor = Color.FromArgb("#506070"), FontSize = 11 });
    //         nameLbl.FormattedText = fmt;
    //         Grid.SetColumn(nameLbl, 1);
    //         row.Add(nameLbl);
    //         var pts = new Label { Text = $"{e.Points:N0} pts",
    //             TextColor = Color.FromArgb(e.Position == 1 ? "#FFD700" : e.Position == 2 ? "#C0C0D0" : e.Position == 3 ? "#CD7F32" : "#607890"),
    //             FontSize = 12, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End };
    //         Grid.SetColumn(pts, 2);
    //         row.Add(pts);
    //         MiniRankContainer.Children.Add(row);
    //     }
    // }

    // -----------------------------------------------------------------------
    // Admin: 5 toques rápidos no saldo ativa/desativa
    // -----------------------------------------------------------------------
    private int      _adminTapCount = 0;
    private DateTime _lastAdminTap  = DateTime.MinValue;

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
            ? "⚙ MODO ADMIN ATIVADO\nBotão '⚙ Admin' disponível no topo da tela."
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
    private async void OnRandomMatchClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("RandomMatchPage");

    private async void OnCareerClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("CareerPage");

    private async void OnFriendGameTapped(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("FriendInvitePage");

    private async void OnQuickPlayTapped(object? sender, TappedEventArgs e)
    {
        AppState.Current.PendingTournamentGame = false;
        AppState.Current.PendingFriendGame     = false;
        await Shell.Current.GoToAsync("GamePage");
    }

    // Liga/Casual — COMENTADO: aguardando base de jogadores
    // private async void OnLeagueClicked(object? sender, TappedEventArgs e)
    //     => await Shell.Current.GoToAsync("LeaguePage");
    // private async void OnSeasonRankingClicked(object? sender, TappedEventArgs e)
    //     => await Shell.Current.GoToAsync("SeasonRankingPage");
    // private async void OnTournamentsClicked(object? sender, EventArgs e)
    //     => await Shell.Current.GoToAsync("TournamentLobbyPage");
    // private async void OnHistoryClicked(object? sender, EventArgs e)
    //     => await Shell.Current.GoToAsync("TournamentHistoryPage");

    private async void OnPuzzleTapped(object? sender, TappedEventArgs e)
    {
        var  svc   = AppState.Current.PuzzleSvc;
        bool isSub = AppState.Current.Subscription.IsActive;

        if (!svc.CanPlayMore(isSub))
        {
            await DisplayAlert(
                "🔒 Limite Diário",
                $"Você já jogou os {PuzzleService.FreeLimit} puzzles gratuitos de hoje.\n\n" +
                "Assine o plano Premium para puzzles ilimitados e volte amanhã para mais desafios gratuitos!",
                "OK");
            return;
        }

        AppState.Current.PendingPuzzle = true;
        await Shell.Current.GoToAsync("GamePage");
    }

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

    private async Task OnChangePasswordAsync()
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

    private void OnMenuClicked(object? sender, EventArgs e)
    {
        MenuChangePasswordItem.IsVisible = !AppState.Current.Auth.IsAnonymous;
        MenuDropdown.IsVisible    = true;
        MenuDismissOverlay.IsVisible = true;
    }

    private void OnMenuDismiss(object? sender, TappedEventArgs e)
    {
        MenuDropdown.IsVisible    = false;
        MenuDismissOverlay.IsVisible = false;
    }

    private async void OnMenuChangePasswordTapped(object? sender, TappedEventArgs e)
    {
        MenuDropdown.IsVisible    = false;
        MenuDismissOverlay.IsVisible = false;
        await OnChangePasswordAsync();
    }

    private async void OnMenuLogoutTapped(object? sender, TappedEventArgs e)
    {
        MenuDropdown.IsVisible    = false;
        MenuDismissOverlay.IsVisible = false;
        await OnLogoutAsync();
    }

    private async Task OnLogoutAsync()
    {
        bool confirm = await DisplayAlert("Sair", "Deseja sair da sua conta?", "Sair", "Cancelar");
        if (!confirm) return;

        AppState.Current.Auth.Logout();
        var window = Application.Current?.Windows.FirstOrDefault();
        if (window != null) window.Page = new LoginPage();
    }
}
