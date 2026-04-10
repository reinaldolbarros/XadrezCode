namespace ChessMAUI.Services;

/// <summary>Perfil do jogador persistido via Preferences.</summary>
public class ProfileService
{
    private const string KeyName      = "profile_name";
    private const string KeyBalance   = "profile_balance";
    private const string KeyWins      = "profile_wins";
    private const string KeyLosses    = "profile_losses";
    private const string KeyTourneys  = "profile_tourneys";
    private const string KeyRating    = "profile_rating";
    private const string KeyAvatar    = "profile_avatar";

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

    public int Rating
    {
        get => Preferences.Default.Get(KeyRating, 1200);
        set => Preferences.Default.Set(KeyRating, value);
    }

    public string Avatar
    {
        get => Preferences.Default.Get(KeyAvatar, "♟");
        set => Preferences.Default.Set(KeyAvatar, value);
    }

    public bool IsNew => string.IsNullOrWhiteSpace(Name);

    public string RankTitle => Rating switch
    {
        >= 2400 => "Grande Mestre",
        >= 2200 => "Mestre Internacional",
        >= 2000 => "Mestre",
        >= 1800 => "Expert",
        >= 1600 => "Avançado",
        >= 1400 => "Intermediário",
        >= 1200 => "Iniciante",
        _       => "Novato"
    };

    public bool TryDebit(decimal amount)
    {
        if (Balance < amount) return false;
        Balance -= amount;
        return true;
    }

    public void Credit(decimal amount) => Balance += amount;
    public void RecordWin()  => Wins++;
    public void RecordLoss() => Losses++;

    /// <summary>Atualiza rating ELO após uma partida.</summary>
    public void UpdateElo(bool won, int opponentRating)
    {
        const int K = 32;
        double expected = 1.0 / (1.0 + Math.Pow(10, (opponentRating - Rating) / 400.0));
        double score    = won ? 1.0 : 0.0;
        Rating = Math.Max(100, Rating + (int)(K * (score - expected)));
    }
}
