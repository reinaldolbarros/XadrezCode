namespace ChessMAUI.Views;

public partial class FriendInvitePage : ContentPage
{
    private int _selectedMinutes = 0;

    public FriendInvitePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var profile = AppState.Current.Profile;
        if (!AppState.Current.Auth.IsAnonymous && !string.IsNullOrWhiteSpace(profile.Name))
            Player1Entry.Text = profile.Name;

        SelectTime(0);
    }

    private void OnTimeSelected(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string s && int.TryParse(s, out int m))
            SelectTime(m);
    }

    private void SelectTime(int minutes)
    {
        _selectedMinutes = minutes;

        int[]    vals = [0, 1, 3, 5, 10];
        Border[] btns = [TimeBtn0, TimeBtn1, TimeBtn3, TimeBtn5, TimeBtn10];

        for (int i = 0; i < btns.Length; i++)
        {
            bool sel = vals[i] == minutes;
            btns[i].BackgroundColor = Color.FromArgb(sel ? "#1A3A65" : "#111D32");
            btns[i].Stroke          = new SolidColorBrush(Color.FromArgb(sel ? "#2A5090" : "#1A2840"));
            btns[i].StrokeThickness = sel ? 1.5 : 1;

            if (btns[i].Content is Label lbl)
            {
                lbl.TextColor      = sel ? Colors.White : Color.FromArgb("#607890");
                lbl.FontAttributes = sel ? FontAttributes.Bold : FontAttributes.None;
            }
        }
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        string p1 = string.IsNullOrWhiteSpace(Player1Entry.Text) ? "Jogador 1" : Player1Entry.Text.Trim();
        string p2 = string.IsNullOrWhiteSpace(Player2Entry.Text) ? "Jogador 2" : Player2Entry.Text.Trim();

        var state = AppState.Current;
        state.PendingFriendGame     = true;
        state.PendingTournamentGame = false;
        state.FriendPlayer1Name     = p1;
        state.FriendOpponentName    = p2;
        state.FriendTimeMinutes     = _selectedMinutes;

        await Shell.Current.GoToAsync("GamePage");
    }
}
