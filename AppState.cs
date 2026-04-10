using ChessMAUI.Models;
using ChessMAUI.Services;

namespace ChessMAUI;

/// <summary>Estado global compartilhado entre páginas (singleton).</summary>
public class AppState
{
    public static AppState Current { get; } = new();

    public ProfileService          Profile     { get; } = new();
    public TournamentService       TournSvc    { get; } = new();
    public MatchmakingService      Matchmaking { get; } = new();
    public RoomLobbyService        RoomLobby   { get; } = new();
    public TournamentHistoryService History     { get; } = new();

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
    public int    TournamentOpponentRating { get; set; } = 1200;

    public bool IsInTournamentMatch => PendingTournamentGame;
}
