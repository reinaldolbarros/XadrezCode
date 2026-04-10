namespace ChessMAUI.Models;

public enum TournamentStatus { Active, HumanEliminated, HumanWon }

public class Tournament
{
    public int              Size         { get; set; }
    public decimal          BuyIn        { get; set; }
    public decimal          PrizePool    { get; set; }
    public int              TotalRounds  { get; set; }
    public int              CurrentRound { get; set; } = 1;
    public TournamentStatus Status       { get; set; } = TournamentStatus.Active;

    public List<TournamentPlayer> Players { get; set; } = [];
    public List<TournamentMatch>  Matches { get; set; } = [];

    // Posição → prêmio em dinheiro virtual
    public Dictionary<int, decimal> PrizeTable { get; set; } = [];

    public TournamentPlayer? HumanPlayer =>
        Players.FirstOrDefault(p => p.IsHuman);

    public TournamentMatch? CurrentHumanMatch =>
        Matches.FirstOrDefault(m => m.Round == CurrentRound
                                 && m.IsHumanMatch
                                 && m.Status == MatchStatus.Pending);

    public List<TournamentMatch> CurrentRoundMatches =>
        Matches.Where(m => m.Round == CurrentRound).ToList();

    public string RoundName =>
        (TotalRounds - CurrentRound) switch
        {
            0 => "Final",
            1 => "Semifinal",
            2 => "Quartas de Final",
            3 => "Oitavas de Final",
            _ => $"Rodada de {(int)Math.Pow(2, TotalRounds - CurrentRound + 1)}"
        };

    public int PlayersRemaining =>
        Players.Count(p => !p.IsEliminated);
}
