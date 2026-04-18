using ChessMAUI.Services;

namespace ChessMAUI.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _auth = AppState.Current.Auth;

    public LoginPage()
    {
        InitializeComponent();
        Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page
            .SetUseSafeArea(this, true);
    }

    // ── Alternar entre login e cadastro ──────────────────────────────────────
    private void OnShowRegister(object? sender, TappedEventArgs e)
    {
        EnterFields.IsVisible    = false;
        RegisterFields.IsVisible = true;
        ClearErrors();
        RegLoginEntry.Focus();
    }

    private void OnShowLogin(object? sender, TappedEventArgs e)
    {
        RegisterFields.IsVisible = false;
        EnterFields.IsVisible    = true;
        ClearErrors();
    }

    // ── Exibir / ocultar senha ───────────────────────────────────────────────
    private void OnToggleLoginPwd(object? sender, EventArgs e)
        => TogglePassword(LoginPasswordEntry, ToggleLoginPwdBtn);

    private void OnToggleRegPwd(object? sender, EventArgs e)
        => TogglePassword(RegPasswordEntry, ToggleRegPwdBtn);

    private void OnToggleRegConfirmPwd(object? sender, EventArgs e)
        => TogglePassword(RegConfirmPasswordEntry, ToggleRegConfirmPwdBtn);

    private void OnToggleNewPwd(object? sender, EventArgs e)
        => TogglePassword(NewPasswordEntry, ToggleNewPwdBtn);

    private void OnToggleConfirmPwd(object? sender, EventArgs e)
        => TogglePassword(ConfirmPasswordEntry, ToggleConfirmPwdBtn);

    private static void TogglePassword(Entry entry, Button btn)
    {
        entry.IsPassword = !entry.IsPassword;
        btn.Text         = entry.IsPassword ? "👁" : "🔒";
    }

    // ── Navegação entre campos (Return key) ──────────────────────────────────
    private void OnLoginEntryCompleted(object? sender, EventArgs e)         => LoginPasswordEntry.Focus();
    private void OnLoginPasswordCompleted(object? sender, EventArgs e)      => OnEntrarClicked(sender, e);
    private void OnRegLoginCompleted(object? sender, EventArgs e)           => RegEmailEntry.Focus();
    private void OnRegEmailCompleted(object? sender, EventArgs e)           => RegPasswordEntry.Focus();
    private void OnRegPasswordCompleted(object? sender, EventArgs e)        => RegConfirmPasswordEntry.Focus();
    private void OnRegConfirmPasswordCompleted(object? sender, EventArgs e) => OnCadastrarClicked(sender, e);
    private void OnResetCredentialCompleted(object? sender, EventArgs e)    => NewPasswordEntry.Focus();
    private void OnNewPasswordCompleted(object? sender, EventArgs e)        => ConfirmPasswordEntry.Focus();
    private void OnConfirmPasswordCompleted(object? sender, EventArgs e)    => OnResetPasswordClicked(sender, e);

    // ── Entrar ───────────────────────────────────────────────────────────────
    private async void OnEntrarClicked(object? sender, EventArgs e)
    {
        var credential = LoginEntry.Text?.Trim() ?? "";
        var password   = LoginPasswordEntry.Text ?? "";

        if (string.IsNullOrEmpty(credential))
            { ShowLoginError("Informe seu login ou e-mail."); return; }
        if (password.Length < 6)
            { ShowLoginError("Senha deve ter pelo menos 6 caracteres."); return; }
        if (!_auth.AccountExists(credential))
            { ShowLoginError("Conta não encontrada. Verifique o login ou e-mail."); return; }

        bool ok = _auth.TryLogin(credential, password);
        if (!ok)
            { ShowLoginError("Senha incorreta."); return; }

        await GoToShell();
    }

    // ── Cadastrar (com verificação de e-mail) ────────────────────────────────
    private async void OnCadastrarClicked(object? sender, EventArgs e)
    {
        var login    = RegLoginEntry.Text?.Trim() ?? "";
        var email    = RegEmailEntry.Text?.Trim().ToLower() ?? "";
        var password = RegPasswordEntry.Text ?? "";
        var confirm  = RegConfirmPasswordEntry.Text ?? "";

        if (string.IsNullOrEmpty(login))
            { ShowRegisterError("Informe um login (nome de usuário)."); return; }
        if (login.Length < 3)
            { ShowRegisterError("Login deve ter pelo menos 3 caracteres."); return; }
        if (string.IsNullOrEmpty(email) || !email.Contains('@') || !email.Contains('.'))
            { ShowRegisterError("Informe um e-mail válido."); return; }
        if (password.Length < 6)
            { ShowRegisterError("Senha deve ter pelo menos 6 caracteres."); return; }
        if (password != confirm)
            { ShowRegisterError("As senhas não conferem."); return; }
        if (_auth.AccountExists(email) || _auth.AccountExists(login))
            { ShowRegisterError("Login ou e-mail já cadastrado neste dispositivo."); return; }

        bool verified = await RunEmailVerification(email);
        if (!verified) return;

        _auth.TryRegister(login, email, password);
        AppState.Current.Profile.Name = login;
        await GoToShell();
    }

    private async Task<bool> RunEmailVerification(string email)
    {
        string code        = EmailService.GenerateCode();
        bool   mailOpened  = await EmailService.SendVerificationCode(email, code);

        string hint = mailOpened
            ? $"Um e-mail foi composto para {email}.\nConclua o envio e insira o código recebido abaixo."
            : $"Não foi possível abrir o app de e-mail.\nSeu código de verificação é:\n\n  {code}";

        string? entered = await DisplayPromptAsync(
            "Verificar e-mail", hint,
            placeholder: "6 dígitos", maxLength: 6,
            keyboard: Keyboard.Numeric);

        if (entered == null) return false;

        if (entered.Trim() != code)
        {
            ShowRegisterError("Código inválido. Tente novamente.");
            return false;
        }
        return true;
    }

    // ── Visitante ────────────────────────────────────────────────────────────
    private async void OnAnonymousClicked(object? sender, EventArgs e)
    {
        _auth.LoginAnonymous();
        await GoToShell();
    }

    // ── Redefinir senha ───────────────────────────────────────────────────────
    private void OnForgotPassword(object? sender, TappedEventArgs e)
    {
        ResetCredentialEntry.Text = LoginEntry.Text ?? "";
        NewPasswordEntry.Text     = "";
        ConfirmPasswordEntry.Text = "";
        ResetErrorLabel.IsVisible = false;
        FormPanel.IsVisible  = false;
        ResetPanel.IsVisible = true;
        ResetCredentialEntry.Focus();
    }

    private async void OnResetPasswordClicked(object? sender, EventArgs e)
    {
        var credential = ResetCredentialEntry.Text?.Trim() ?? "";
        var newPass    = NewPasswordEntry.Text ?? "";
        var confirm    = ConfirmPasswordEntry.Text ?? "";

        if (string.IsNullOrEmpty(credential))
            { ShowResetError("Informe seu login ou e-mail."); return; }
        if (newPass.Length < 6)
            { ShowResetError("Senha deve ter pelo menos 6 caracteres."); return; }
        if (newPass != confirm)
            { ShowResetError("As senhas não conferem."); return; }

        bool ok = _auth.ResetPassword(credential, newPass);
        if (!ok)
            { ShowResetError("Login ou e-mail não encontrado."); return; }

        string accountEmail = _auth.Email;
        if (!string.IsNullOrEmpty(accountEmail))
            await EmailService.SendTempPassword(accountEmail, newPass);

        await DisplayAlert("✓ Senha redefinida",
            "Sua senha foi atualizada. Entre com a nova senha.", "OK");

        LoginPasswordEntry.Text = "";
        OnBackFromReset(sender, new TappedEventArgs(null));
    }

    private void OnBackFromReset(object? sender, TappedEventArgs e)
    {
        ResetPanel.IsVisible     = false;
        FormPanel.IsVisible      = true;
        EnterFields.IsVisible    = true;
        RegisterFields.IsVisible = false;
        ClearErrors();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    private static async Task GoToShell()
    {
        var window = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault();
        if (window != null) window.Page = new AppShell();
        await Task.CompletedTask;
    }

    private void ClearErrors()
    {
        LoginErrorLabel.IsVisible    = false;
        RegisterErrorLabel.IsVisible = false;
    }

    private void ShowLoginError(string msg)
    {
        LoginErrorLabel.Text      = msg;
        LoginErrorLabel.IsVisible = true;
    }

    private void ShowRegisterError(string msg)
    {
        RegisterErrorLabel.Text      = msg;
        RegisterErrorLabel.IsVisible = true;
    }

    private void ShowResetError(string msg)
    {
        ResetErrorLabel.Text      = msg;
        ResetErrorLabel.IsVisible = true;
    }
}
