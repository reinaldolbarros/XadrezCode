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
        var p = AppState.Current.Profile;
        AvatarLabel.Text    = p.Avatar;
        NameLabel.Text      = p.Name;
        RankLabel.Text      = p.RankTitle;
        RatingLabel.Text    = $"Rating: {p.Rating}";
        BalanceLabel.Text   = $"$ {p.Balance:N0}";
        WinsLabel.Text      = p.Wins.ToString();
        LossesLabel.Text    = p.Losses.ToString();
        TournWinsLabel.Text = p.TournamentsWon.ToString();
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

    private async void OnQuickPlayClicked(object? sender, EventArgs e)
    {
        AppState.Current.PendingTournamentGame = false;
        await Shell.Current.GoToAsync("GamePage");
    }
}
