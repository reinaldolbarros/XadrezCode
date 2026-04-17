using ChessMAUI.Services;

namespace ChessMAUI.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _auth = AppState.Current.Auth;

    public LoginPage()
    {
        InitializeComponent();
        // Respeita notch e home indicator no iOS quando fora do Shell
        Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page
            .SetUseSafeArea(this, true);
    }

    private void OnPasswordCompleted(object? sender, EventArgs e)
        => OnLoginClicked(sender, e);

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        var email    = EmailEntry.Text?.Trim().ToLower() ?? "";
        var password = PasswordEntry.Text ?? "";

        if (string.IsNullOrEmpty(email)) { ShowError("Informe o e-mail.");             return; }
        if (!email.Contains('@'))        { ShowError("E-mail inválido.");               return; }
        if (password.Length < 6)         { ShowError("Senha com mínimo 6 caracteres."); return; }

        bool ok;
        if (_auth.AccountExists(email))
        {
            ok = _auth.TryLogin(email, password);
            if (!ok) { ShowError("Senha incorreta."); return; }
        }
        else
        {
            ok = _auth.TryRegister(email, password);
        }

        if (ok) GoToShell();
    }

    private void OnAnonymousClicked(object? sender, EventArgs e)
    {
        _auth.LoginAnonymous();
        GoToShell();
    }

    private static void GoToShell()
        => Microsoft.Maui.Controls.Application.Current!.MainPage = new AppShell();

    private void ShowError(string msg)
    {
        ErrorLabel.Text      = msg;
        ErrorLabel.IsVisible = true;
    }
}
