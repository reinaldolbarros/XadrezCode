using ChessMAUI.Models;

namespace ChessMAUI.Services;

/// <summary>
/// Gerencia múltiplas salas abertas simultaneamente (estilo PokerStars lobby).
/// Cada sala preenche bots independentemente para simular movimento real.
/// </summary>
public class RoomLobbyService
{
    private readonly List<TournamentRoom> _rooms = [];
    private readonly Random _rng = Random.Shared;

    // Configurações pré-definidas de salas que sempre existem no lobby
    private static readonly (int Size, decimal BuyIn, int Time)[] Templates =
    [
        (8,  10,  1),  (8,  25,  2),  (8,  50,  3),
        (16, 10,  2),  (16, 50,  3),  (16, 100, 5),
        (32, 25,  3),  (32, 100, 5),  (32, 250, 10),
        (64, 50,  5),  (64, 250, 10), (64, 500, 15),
    ];

    public IReadOnlyList<TournamentRoom> Rooms => _rooms;

    public event Action? RoomsUpdated;

    public RoomLobbyService()
    {
        GenerateRooms();
        _ = SimulateLobbyActivityAsync();
    }

    // Gera o lobby inicial com salas semi-preenchidas
    private void GenerateRooms()
    {
        _rooms.Clear();
        foreach (var (size, buyIn, time) in Templates)
        {
            var room = new TournamentRoom { Size = size, BuyIn = buyIn, TimeMinutes = time };
            // Pré-preenche entre 10% e 80% da sala para dar sensação de atividade
            room.Joined = _rng.Next((int)(size * 0.1), (int)(size * 0.8));
            _rooms.Add(room);
        }
    }

    // Simula bots entrando e salas enchendo/esvaziando ao longo do tempo
    private async Task SimulateLobbyActivityAsync()
    {
        while (true)
        {
            await Task.Delay(_rng.Next(1500, 4000));

            bool changed = false;
            foreach (var room in _rooms.ToList())
            {
                if (room.Status != RoomStatus.Open) continue;

                // Chance de alguém entrar (70%) ou sair (20%)
                int roll = _rng.Next(100);
                if (roll < 70 && room.Joined < room.Size)
                {
                    room.Joined++;
                    changed = true;

                    if (room.Joined >= room.Size)
                    {
                        room.Status = RoomStatus.Starting;
                        _ = ResetRoomAsync(room);
                    }
                }
                else if (roll < 90 && room.Joined > 1)
                {
                    room.Joined = Math.Max(1, room.Joined - 1);
                    changed = true;
                }
            }

            if (changed)
                MainThread.BeginInvokeOnMainThread(() => RoomsUpdated?.Invoke());
        }
    }

    // Após sala iniciar, recria ela vazia depois de alguns segundos
    private async Task ResetRoomAsync(TournamentRoom room)
    {
        await Task.Delay(5000);
        room.Status = RoomStatus.InProgress;
        RoomsUpdated?.Invoke();

        await Task.Delay(3000);
        // Recria a sala com os mesmos parâmetros
        var idx  = _rooms.IndexOf(room);
        var nova = new TournamentRoom
        {
            Size = room.Size, BuyIn = room.BuyIn, TimeMinutes = room.TimeMinutes,
            Joined = _rng.Next(1, Math.Max(2, room.Size / 4))
        };
        if (idx >= 0) _rooms[idx] = nova;
        MainThread.BeginInvokeOnMainThread(() => RoomsUpdated?.Invoke());
    }

    /// <summary>Registra o humano em uma sala. Retorna a sala atualizada.</summary>
    public TournamentRoom JoinRoom(TournamentRoom room)
    {
        room.Joined = Math.Min(room.Size, room.Joined + 1);
        if (room.Joined >= room.Size) room.Status = RoomStatus.Starting;
        return room;
    }

    public List<TournamentRoom> GetByBuyIn(decimal buyIn) =>
        _rooms.Where(r => r.BuyIn == buyIn && r.Status == RoomStatus.Open).ToList();

    public List<TournamentRoom> GetBySize(int size) =>
        _rooms.Where(r => r.Size == size && r.Status == RoomStatus.Open).ToList();
}
