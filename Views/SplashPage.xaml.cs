using ChessMAUI.Services;

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

        // Inicializa Supabase e restaura sessão salva antes de verificar auth
        await SupabaseService.Instance.InitializeAsync();

        var auth    = AppState.Current.Auth;
        var profile = AppState.Current.Profile;

        // Se usuário autenticado (não anônimo), sincroniza perfil do servidor
        if (auth.IsAuthenticated && !auth.IsAnonymous)
            _ = profile.LoadFromSupabaseAsync();

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
