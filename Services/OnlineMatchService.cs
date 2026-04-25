using ChessMAUI.Models;

namespace ChessMAUI.Services;

/// <summary>
/// Matchmaking 1v1 aleatório.
/// O tempo é definido por quem inicia a busca; o oponente aceita e entra.
/// Phase 1: mock local. Phase 2: substituir por Firebase/SignalR.
/// </summary>
public class OnlineMatchService
{
    private static readonly string[] MockNames =
    [
        "Carlos Silva", "Ana Lima",       "Pedro Santos",  "Julia Rocha",
        "Rafael Costa", "Fernanda Alves", "Bruno Martins", "Camila Nunes",
        "Diego Souza",  "Larissa Melo"
    ];

    public OnlineMatchState State { get; } = new();

    /// <summary>Oponente encontrado; partida pronta para iniciar.</summary>
    public event Action? MatchReady;
    /// <summary>Busca cancelada.</summary>
    public event Action? SearchCancelled;

    private CancellationTokenSource? _cts;

    // ─────────────────────────────────────────────────────────────────────────
    // Inicia busca — tempo proposto é o tempo da partida
    // ─────────────────────────────────────────────────────────────────────────
    public async Task StartSearchingAsync(TimeControlOption myTime)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        State.Phase       = OnlineMatchPhase.Searching;
        State.MyProposal  = myTime;
        State.AgreedTime  = myTime;
        State.MatchId     = Guid.NewGuid().ToString()[..8];

        try
        {
            // Simula latência de busca (2–6 s)
            await Task.Delay(Random.Shared.Next(2000, 6000), _cts.Token);

            State.OpponentName   = MockNames[Random.Shared.Next(MockNames.Length)];
            State.OpponentRating = Random.Shared.Next(700, 1900);
            State.PlayerIsWhite  = Random.Shared.Next(2) == 0;
            State.Phase          = OnlineMatchPhase.Confirmed;
            MatchReady?.Invoke();
        }
        catch (OperationCanceledException) { }
    }

    public void Cancel()
    {
        _cts?.Cancel();
        State.Phase = OnlineMatchPhase.Cancelled;
        SearchCancelled?.Invoke();
        Reset();
    }

    public void Reset()
    {
        _cts?.Cancel();
        _cts                 = null;
        State.Phase          = OnlineMatchPhase.Idle;
        State.OpponentName   = "";
        State.OpponentRating = 0;
        State.MatchId        = "";
    }
}
