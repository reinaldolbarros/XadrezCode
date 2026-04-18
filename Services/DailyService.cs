namespace ChessMAUI.Services;

public class DailyMission
{
    public string Id             { get; set; } = "";
    public string Icon           { get; set; } = "";
    public string Description    { get; set; } = "";
    public int    Target         { get; set; }
    public int    Progress       { get; set; }
    public bool   Completed      => Progress >= Target;
    public int    BalanceReward  { get; set; }
}

/// <summary>Gerencia bônus diário e missões diárias.</summary>
public class DailyService
{
    private const string KeyLastLogin    = "daily_last_login";
    private const string KeyStreak       = "daily_streak";
    private const string KeyBonusClaimed = "daily_bonus_claimed";
    private const string KeyMissionDate  = "daily_mission_date";
    private const string KeyM1Progress   = "daily_m1_progress";
    private const string KeyM2Progress   = "daily_m2_progress";
    private const string KeyM3Progress   = "daily_m3_progress";

    // ── Streak ───────────────────────────────────────────────────────────────
    public int LoginStreak
    {
        get => Preferences.Default.Get(KeyStreak, 0);
        private set => Preferences.Default.Set(KeyStreak, value);
    }

    // ── Bônus diário ─────────────────────────────────────────────────────────
    public bool BonusClaimedToday
        => Preferences.Default.Get(KeyBonusClaimed, "") == TodayKey;

    /// <summary>Reivindica o bônus diário e retorna fichas ganhas.</summary>
    public int ClaimDailyBonus()
    {
        UpdateStreak();
        Preferences.Default.Set(KeyBonusClaimed, TodayKey);

        int streak = LoginStreak;
        return streak switch { >= 7 => 150, >= 5 => 100, >= 3 => 75, >= 2 => 50, _ => 30 };
    }

    private void UpdateStreak()
    {
        var lastStr = Preferences.Default.Get(KeyLastLogin, "");
        var today   = DateTime.Today;

        if (!DateTime.TryParse(lastStr, out var last))
        {
            LoginStreak = 1;
        }
        else
        {
            int diff = (today - last.Date).Days;
            if (diff == 0) return;
            LoginStreak = diff == 1 ? LoginStreak + 1 : 1;
        }
        Preferences.Default.Set(KeyLastLogin, today.ToString("yyyy-MM-dd"));
    }

    // ── Missões diárias ───────────────────────────────────────────────────────
    public List<DailyMission> GetMissions()
    {
        // Reset se mudou o dia
        if (Preferences.Default.Get(KeyMissionDate, "") != TodayKey)
        {
            Preferences.Default.Set(KeyMissionDate, TodayKey);
            Preferences.Default.Set(KeyM1Progress,  0);
            Preferences.Default.Set(KeyM2Progress,  0);
            Preferences.Default.Set(KeyM3Progress,  0);
        }

        return
        [
            new DailyMission { Id="m1", Icon="🎮", Description="Jogar 3 partidas",
                Target=3, Progress=Preferences.Default.Get(KeyM1Progress,0), BalanceReward=30 },
            new DailyMission { Id="m2", Icon="⚔️", Description="Vencer 2 partidas",
                Target=2, Progress=Preferences.Default.Get(KeyM2Progress,0), BalanceReward=50 },
            new DailyMission { Id="m3", Icon="🏆", Description="Chegar ao Top 3 em torneio",
                Target=1, Progress=Preferences.Default.Get(KeyM3Progress,0), BalanceReward=50 },
        ];
    }

    /// <summary>Registra uma partida jogada. Chame ao fim de qualquer jogo.</summary>
    public bool RecordGamePlayed()
    {
        var m = GetMissions()[0];
        if (m.Completed) return false;
        Preferences.Default.Set(KeyM1Progress, m.Progress + 1);
        return m.Progress + 1 >= m.Target; // retorna true se recém completou
    }

    /// <summary>Registra uma vitória. Chame quando o humano vence.</summary>
    public bool RecordWin()
    {
        var m = GetMissions()[1];
        if (m.Completed) return false;
        Preferences.Default.Set(KeyM2Progress, m.Progress + 1);
        return m.Progress + 1 >= m.Target;
    }

    /// <summary>Registra eliminação em torneio.</summary>
    public bool RecordTournamentElimination()
    {
        var m = GetMissions()[2];
        if (m.Completed) return false;
        Preferences.Default.Set(KeyM3Progress, m.Progress + 1);
        return m.Progress + 1 >= m.Target;
    }

    private static string TodayKey => DateTime.Today.ToString("yyyy-MM-dd");
}
