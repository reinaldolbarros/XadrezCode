using System.Security.Cryptography;
using System.Text;

namespace ChessMAUI.Services;

public class AuthService
{
    private const string KeyProvider = "auth_provider";
    private const string KeyEmail    = "auth_email";
    private const string KeyUsername = "auth_username";
    private const string KeyPassHash = "auth_pass_hash";

    public bool   IsAuthenticated => !string.IsNullOrEmpty(Provider);
    public string Provider        => Preferences.Default.Get(KeyProvider, "");
    public string Email           => Preferences.Default.Get(KeyEmail,    "");
    public string Username        => Preferences.Default.Get(KeyUsername, "");
    public bool   IsAnonymous     => Provider == "anonymous";

    // ── Anônimo ──────────────────────────────────────────────────────────────
    public void LoginAnonymous()
        => Preferences.Default.Set(KeyProvider, "anonymous");

    // ── Verifica se conta existe por login OU e-mail ──────────────────────────
    public bool AccountExists(string credential)
    {
        var c    = credential.Trim().ToLower();
        var mail = Preferences.Default.Get(KeyEmail,    "");
        var user = Preferences.Default.Get(KeyUsername, "");
        bool match = (!string.IsNullOrEmpty(mail) && mail.Equals(c, StringComparison.Ordinal))
                  || (!string.IsNullOrEmpty(user) && user.Equals(c, StringComparison.Ordinal));
        return match && !string.IsNullOrEmpty(Preferences.Default.Get(KeyPassHash, ""));
    }

    // ── Login por login OU e-mail + senha ─────────────────────────────────────
    public bool TryLogin(string credential, string password)
    {
        if (!AccountExists(credential)) return false;
        if (Preferences.Default.Get(KeyPassHash, "") != Hash(password)) return false;
        Preferences.Default.Set(KeyProvider, "email"); // persiste a sessão
        return true;
    }

    // ── Cadastro com login + e-mail + senha ────────────────────────────────────
    public bool TryRegister(string username, string email, string password)
    {
        Preferences.Default.Set(KeyUsername, username.Trim().ToLower());
        Preferences.Default.Set(KeyEmail,    email.Trim().ToLower());
        Preferences.Default.Set(KeyPassHash, Hash(password));
        Preferences.Default.Set(KeyProvider, "email");
        return true;
    }

    // ── Redefinição de senha por login OU e-mail ───────────────────────────────
    public bool ResetPassword(string credential, string newPassword)
    {
        if (!AccountExists(credential)) return false;
        Preferences.Default.Set(KeyPassHash, Hash(newPassword));
        return true;
    }

    // ── Logout: encerra sessão, mantém credenciais no dispositivo ─────────────
    public void Logout()
        => Preferences.Default.Remove(KeyProvider);

    private static string Hash(string s)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(s));
        return Convert.ToHexString(bytes);
    }
}
