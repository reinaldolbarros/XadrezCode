using ChessMAUI.Models;

namespace ChessMAUI.Services;

/// <summary>
/// Matchmaking 1v1 aleatório. Tempo definido por quem busca (1–20 min).
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

    public event Action? MatchReady;
    public event Action? SearchCancelled;

    private CancellationTokenSource? _cts;

    public async Task StartSearchingAsync(int minutes)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        State.Phase         = OnlineMatchPhase.Searching;
        State.AgreedMinutes = minutes;
        State.MatchId       = Guid.NewGuid().ToString()[..8];

        try
        {
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
