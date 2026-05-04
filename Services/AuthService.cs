using Supabase.Gotrue;

namespace ChessMAUI.Services;

public class AuthService
{
    private Supabase.Client Db => SupabaseService.Instance.Client;

    // ── Propriedades síncronas (lidas da sessão em cache) ─────────────────────
    public bool IsAuthenticated => Db?.Auth.CurrentUser != null;

    public bool IsAnonymous
    {
        get
        {
            var meta = Db?.Auth.CurrentUser?.AppMetadata;
            return meta != null
                && meta.TryGetValue("provider", out var p)
                && p?.ToString() == "anonymous";
        }
    }

    public string Email  => Db?.Auth.CurrentUser?.Email ?? "";
    public string UserId => Db?.Auth.CurrentUser?.Id    ?? "";

    public string Username
    {
        get
        {
            var meta = Db?.Auth.CurrentUser?.UserMetadata;
            if (meta != null && meta.TryGetValue("username", out var u))
                return u?.ToString() ?? "";
            return "";
        }
    }

    // ── Anônimo ───────────────────────────────────────────────────────────────
    public Task LoginAnonymousAsync()
        => Db.Auth.SignIn(Supabase.Gotrue.Constants.SignInType.Anonymous);

    // ── Login por e-mail + senha ──────────────────────────────────────────────
    public async Task<bool> TryLoginAsync(string email, string password)
    {
        try
        {
            var session = await Db.Auth.SignIn(email.Trim().ToLower(), password);
            return session?.User != null;
        }
        catch { return false; }
    }

    // ── Cadastro ──────────────────────────────────────────────────────────────
    public async Task<(bool Ok, string Error)> TryRegisterAsync(
        string username, string email, string password)
    {
        try
        {
            var opts = new SignUpOptions
            {
                Data = new Dictionary<string, object>
                {
                    ["username"] = username.Trim(),
                }
            };
            var session = await Db.Auth.SignUp(email.Trim().ToLower(), password, opts);
            return (session?.User != null, "");
        }
        catch (Exception ex)
        {
            string msg = ex.Message.Contains("already registered")
                ? "E-mail já cadastrado."
                : "Erro ao cadastrar. Tente novamente.";
            return (false, msg);
        }
    }

    // ── Redefinição de senha via link por e-mail ──────────────────────────────
    public async Task<bool> SendPasswordResetAsync(string email)
    {
        try
        {
            await Db.Auth.ResetPasswordForEmail(email.Trim().ToLower());
            return true;
        }
        catch { return false; }
    }

    // ── Alterar senha (verifica a senha atual antes de atualizar) ─────────────
    public async Task<(bool Ok, string Error)> TryUpdatePasswordAsync(
        string currentPassword, string newPassword)
    {
        try
        {
            // Reautentica para confirmar a senha atual
            var email = Email;
            if (string.IsNullOrEmpty(email)) return (false, "Conta sem e-mail registrado.");

            var session = await Db.Auth.SignIn(email, currentPassword);
            if (session?.User == null) return (false, "Senha atual incorreta.");

            await Db.Auth.Update(new Supabase.Gotrue.UserAttributes { Password = newPassword });
            return (true, "");
        }
        catch { return (false, "Não foi possível alterar a senha."); }
    }

    // ── Logout ────────────────────────────────────────────────────────────────
    public Task LogoutAsync() => Db.Auth.SignOut();
}
