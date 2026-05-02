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

        var auth = AppState.Current.Auth;

        // Cria a próxima página antes do delay para que o MAUI pré-carregue
        // seus handlers nativos, evitando o flash branco na transição.
        Page next;
        if (!auth.IsAuthenticated || auth.IsAnonymous)
            next = new LoginPage();
        else if (DailyMissionsPage.ShouldShow())
            next = new DailyMissionsPage();
        else
            next = new AppShell();

        await Task.Delay(2200);

        if (Application.Current is not null)
            Application.Current.Windows[0].Page = next;
    }
}
