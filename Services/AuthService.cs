using System.Security.Cryptography;
using System.Text;

namespace ChessMAUI.Services;

public class AuthService
{
    private const string KeyProvider = "auth_provider"; // "", "anonymous", "email", "google"
    private const string KeyEmail    = "auth_email";
    private const string KeyPassHash = "auth_pass_hash";

    // ── Estado ───────────────────────────────────────────────────────────────
    public bool   IsAuthenticated => !string.IsNullOrEmpty(Provider);
    public string Provider        => Preferences.Default.Get(KeyProvider, "");
    public string Email           => Preferences.Default.Get(KeyEmail,    "");
    public bool   IsAnonymous     => Provider == "anonymous";
    public bool   IsGoogle        => Provider == "google";

    // ── Anônimo ──────────────────────────────────────────────────────────────
    public void LoginAnonymous()
        => Preferences.Default.Set(KeyProvider, "anonymous");

    // ── Email / senha ────────────────────────────────────────────────────────
    public bool AccountExists(string email)
    {
        var stored = Preferences.Default.Get(KeyEmail, "");
        return stored.Equals(email.Trim().ToLower(), StringComparison.Ordinal)
            && !string.IsNullOrEmpty(Preferences.Default.Get(KeyPassHash, ""));
    }

    public bool TryLogin(string email, string password)
    {
        if (!AccountExists(email)) return false;
        return Preferences.Default.Get(KeyPassHash, "") == Hash(password);
    }

    public bool TryRegister(string email, string password)
    {
        // Permite apenas uma conta por dispositivo; sobrescreve se nova
        Preferences.Default.Set(KeyEmail,    email.Trim().ToLower());
        Preferences.Default.Set(KeyPassHash, Hash(password));
        Preferences.Default.Set(KeyProvider, "email");
        return true;
    }

    // ── Comum ────────────────────────────────────────────────────────────────
    public void SetEmailProvider()
        => Preferences.Default.Set(KeyProvider, "email");

    public void Logout()
    {
        Preferences.Default.Remove(KeyProvider);
        Preferences.Default.Remove(KeyEmail);
        Preferences.Default.Remove(KeyPassHash);
    }

    private static string Hash(string s)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(s));
        return Convert.ToHexString(bytes);
    }
}
