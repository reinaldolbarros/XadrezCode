namespace ChessMAUI.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(2200);

        var auth = AppState.Current.Auth;
        Page next = (auth.IsAuthenticated && !auth.IsAnonymous)
            ? new AppShell()
            : new LoginPage();

        if (Application.Current is not null)
            Application.Current.Windows[0].Page = next;
    }
}
