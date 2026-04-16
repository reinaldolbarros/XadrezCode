namespace ChessMAUI.Services;

/// <summary>Perfil do jogador persistido via Preferences.</summary>
public class ProfileService
{
    private const string KeyName      = "profile_name";
    private const string KeyBalance   = "profile_balance";
    private const string KeyWins      = "profile_wins";
    private const string KeyLosses    = "profile_losses";
    private const string KeyTourneys  = "profile_tourneys";
    private const string KeyAvatar    = "profile_avatar";
    private const string KeyPoints    = "profile_points";
    private const string KeyWeekPts   = "profile_week_points";
    private const string KeyWeekReset = "profile_week_reset";
    private const string KeyTickets   = "profile_tickets";

    public string Name
    {
        get => Preferences.Default.Get(KeyName, "");
        set => Preferences.Default.Set(KeyName, value);
    }

    public decimal Balance
    {
        get => (decimal)Preferences.Default.Get(KeyBalance, 1000.0);
        set => Preferences.Default.Set(KeyBalance, (double)value);
    }

    public int Wins
    {
        get => Preferences.Default.Get(KeyWins, 0);
        set => Preferences.Default.Set(KeyWins, value);
    }

    public int Losses
    {
        get => Preferences.Default.Get(KeyLosses, 0);
        set => Preferences.Default.Set(KeyLosses, value);
    }

    public int TournamentsWon
    {
        get => Preferences.Default.Get(KeyTourneys, 0);
        set => Preferences.Default.Set(KeyTourneys, value);
    }

    public string Avatar
    {
        get => Preferences.Default.Get(KeyAvatar, "♟");
        set => Preferences.Default.Set(KeyAvatar, value);
    }

    public int Points
    {
        get => Preferences.Default.Get(KeyPoints, 0);
        set => Preferences.Default.Set(KeyPoints, value);
    }

    public int WeekPoints
    {
        get
        {
            CheckWeekReset();
            return Preferences.Default.Get(KeyWeekPts, 0);
        }
        private set => Preferences.Default.Set(KeyWeekPts, value);
    }

    private void CheckWeekReset()
    {
        var lastReset = DateTime.Parse(Preferences.Default.Get(KeyWeekReset, DateTime.MinValue.ToString()));
        var monday    = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek == 0 ? 6 : (int)DateTime.Today.DayOfWeek - 1);
        if (lastReset < monday)
        {
            Preferences.Default.Set(KeyWeekPts,   0);
            Preferences.Default.Set(KeyWeekReset, DateTime.Today.ToString());
        }
    }

    public void AddPoints(int pts)
    {
        Points     += pts;
        WeekPoints += pts;
    }

    // ── Tickets de satélite ──────────────────────────────────────────────────
    private Dictionary<string, int> LoadTickets()
    {
        var json = Preferences.Default.Get(KeyTickets, "{}");
        try { return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? []; }
        catch { return []; }
    }

    private void SaveTickets(Dictionary<string, int> t)
        => Preferences.Default.Set(KeyTickets, System.Text.Json.JsonSerializer.Serialize(t));

    public bool HasTicket(decimal buyIn)
    {
        var t = LoadTickets();
        return t.TryGetValue(buyIn.ToString("0.##"), out int c) && c > 0;
    }

    public void AddTicket(decimal buyIn)
    {
        var t   = LoadTickets();
        var key = buyIn.ToString("0.##");
        t[key]  = (t.TryGetValue(key, out int c) ? c : 0) + 1;
        SaveTickets(t);
    }

    public bool UseTicket(decimal buyIn)
    {
        var t   = LoadTickets();
        var key = buyIn.ToString("0.##");
        if (!t.TryGetValue(key, out int c) || c <= 0) return false;
        if (c == 1) t.Remove(key); else t[key] = c - 1;
        SaveTickets(t);
        return true;
    }

    /// <summary>Retorna todos os tickets disponíveis: buy-in → quantidade.</summary>
    public Dictionary<decimal, int> GetAllTickets()
    {
        var raw = LoadTickets();
        var result = new Dictionary<decimal, int>();
        foreach (var kv in raw.Where(kv => kv.Value > 0))
            if (decimal.TryParse(kv.Key, out decimal d)) result[d] = kv.Value;
        return result;
    }

    // Faixa baseada em pontos totais
    public static (string Icon, string Name, int Min, int Max) GetTier(int points) => points switch
    {
        >= 1500 => ("♚", "Rei",        1500, int.MaxValue),
        >= 500  => ("♞", "Cavaleiro",   500,  1499),
        _       => ("♟", "Pião",           0,   499),
    };

    public string TierIcon => GetTier(Points).Icon;
    public string TierName => GetTier(Points).Name;

    public bool IsNew => string.IsNullOrWhiteSpace(Name);

    public bool TryDebit(decimal amount)
    {
        if (Balance < amount) return false;
        Balance -= amount;
        return true;
    }

    public void Credit(decimal amount) => Balance += amount;
    public void RecordWin()  => Wins++;
    public void RecordLoss() => Losses++;

}
