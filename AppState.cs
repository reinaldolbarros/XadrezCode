using ChessMAUI.Models;
using ChessMAUI.Services;

namespace ChessMAUI;

/// <summary>Estado global compartilhado entre páginas (singleton).</summary>
public class AppState
{
    public static AppState Current { get; } = new();

    public AuthService              Auth        { get; } = new();
    public ProfileService           Profile     { get; } = new();
    public TournamentService        TournSvc    { get; } = new();
    public MatchmakingService       Matchmaking { get; } = new();
    public RoomLobbyService         RoomLobby   { get; } = new();
    public TournamentHistoryService History     { get; } = new();
    public RankingService           Ranking     { get; } = new();
    public DailyService             Daily        { get; } = new();
    public AdminService             Admin        { get; } = new();
    public SubscriptionService      Subscription { get; } = new();
    public AdService                Ads          { get; } = new();
    public TitleService             Titles        { get; } = new();
    public SeasonService            Season        { get; } = new();
    public LeagueService            League        { get; } = new();
    public CasualRankingService     CasualRanking { get; } = new();

    // Torneio ativo
    public Tournament? ActiveTournament { get; set; }

    // Resultado da última partida
    public bool LastMatchHumanWon { get; set; }

    // BracketPage → GamePage
    public bool PendingTournamentGame { get; set; }

    // GamePage → BracketPage
    public bool MatchResultReady { get; set; }

    // Contexto da partida
    public string TournamentOpponentName   { get; set; } = "";
    public int    TournamentAIDepth        { get; set; } = 2;
    public int    TournamentTimeMinutes    { get; set; } = 5;

    public bool IsInTournamentMatch => PendingTournamentGame;

    // Jogo com amigo (pass-and-play)
    public bool   PendingFriendGame  { get; set; }
    public string FriendOpponentName { get; set; } = "";
    public int    FriendTimeMinutes  { get; set; }

    // Modo administrador para testes
    public bool IsAdminMode { get; set; } = false;
}
