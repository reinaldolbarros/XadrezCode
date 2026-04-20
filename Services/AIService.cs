using ChessMAUI.Models;

namespace ChessMAUI.Services;

public class AIService
{
    private readonly int   _depth;
    private readonly float _randomness; // 0 = determinístico, 1 = totalmente aleatório

    // Profundidades por nível — Difícil usa busca com limite de tempo em vez de profundidade fixa
    public AIService(int depth = 2)
    {
        _depth = depth switch
        {
            1 => 1,  // Fácil
            3 => 2,  // Médio
            _ => 4   // Difícil — 4 níveis, limitado por tempo
        };
        _randomness = depth switch
        {
            1 => 0.35f, // Fácil: 35% de jogada variada
            3 => 0.08f, // Médio: 8% de variação para abertura diferente
            _ => 0.00f  // Difícil: puro minimax, sem aleatoriedade
        };
    }

    private static readonly int[] Material = [100, 320, 330, 500, 900, 20_000];

    private static readonly int[,] PawnPst = {
        {  0,  0,  0,  0,  0,  0,  0,  0 },
        { 50, 50, 50, 50, 50, 50, 50, 50 },
        { 10, 10, 20, 30, 30, 20, 10, 10 },
        {  5,  5, 10, 25, 25, 10,  5,  5 },
        {  0,  0,  0, 20, 20,  0,  0,  0 },
        {  5, -5,-10,  0,  0,-10, -5,  5 },
        {  5, 10, 10,-20,-20, 10, 10,  5 },
        {  0,  0,  0,  0,  0,  0,  0,  0 }
    };
    private static readonly int[,] KnightPst = {
        {-50,-40,-30,-30,-30,-30,-40,-50},
        {-40,-20,  0,  0,  0,  0,-20,-40},
        {-30,  0, 10, 15, 15, 10,  0,-30},
        {-30,  5, 15, 20, 20, 15,  5,-30},
        {-30,  0, 15, 20, 20, 15,  0,-30},
        {-30,  5, 10, 15, 15, 10,  5,-30},
        {-40,-20,  0,  5,  5,  0,-20,-40},
        {-50,-40,-30,-30,-30,-30,-40,-50}
    };
    private static readonly int[,] BishopPst = {
        {-20,-10,-10,-10,-10,-10,-10,-20},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-10,  0,  5, 10, 10,  5,  0,-10},
        {-10,  5,  5, 10, 10,  5,  5,-10},
        {-10,  0, 10, 10, 10, 10,  0,-10},
        {-10, 10, 10, 10, 10, 10, 10,-10},
        {-10,  5,  0,  0,  0,  0,  5,-10},
        {-20,-10,-10,-10,-10,-10,-10,-20}
    };
    private static readonly int[,] RookPst = {
        {  0,  0,  0,  0,  0,  0,  0,  0},
        {  5, 10, 10, 10, 10, 10, 10,  5},
        { -5,  0,  0,  0,  0,  0,  0, -5},
        { -5,  0,  0,  0,  0,  0,  0, -5},
        { -5,  0,  0,  0,  0,  0,  0, -5},
        { -5,  0,  0,  0,  0,  0,  0, -5},
        { -5,  0,  0,  0,  0,  0,  0, -5},
        {  0,  0,  0,  5,  5,  0,  0,  0}
    };
    private static readonly int[,] QueenPst = {
        {-20,-10,-10, -5, -5,-10,-10,-20},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-10,  0,  5,  5,  5,  5,  0,-10},
        { -5,  0,  5,  5,  5,  5,  0, -5},
        {  0,  0,  5,  5,  5,  5,  0, -5},
        {-10,  5,  5,  5,  5,  5,  0,-10},
        {-10,  0,  5,  0,  0,  0,  0,-10},
        {-20,-10,-10, -5, -5,-10,-10,-20}
    };
    private static readonly int[,] KingMidPst = {
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-20,-30,-30,-40,-40,-30,-30,-20},
        {-10,-20,-20,-20,-20,-20,-20,-10},
        { 20, 20,  0,  0,  0,  0, 20, 20},
        { 20, 30, 10,  0,  0, 10, 30, 20}
    };

    // -------------------------------------------------------------------------
    public Task<ChessMove?> GetBestMoveAsync(ChessBoard board, CancellationToken ct = default)
        => Task.Run(() => FindBest(board, ct), ct);

    private ChessMove? FindBest(ChessBoard board, CancellationToken ct)
    {
        var moves = ChessEngine.GetAllLegalMoves(board, board.CurrentTurn);
        if (moves.Count == 0) return null;

        // Fácil / Médio: chance de jogada aleatória para variedade
        if (_randomness > 0 && Random.Shared.NextDouble() < _randomness)
        {
            // Escolhe aleatoriamente entre as 5 melhores jogadas (não qualquer jogada)
            OrderMoves(board, moves);
            int pool = Math.Min(moves.Count, _randomness > 0.2f ? 4 : 3);
            return moves[Random.Shared.Next(pool)];
        }

        // Difícil: limite de tempo de 2,5 segundos via CancellationToken
        CancellationTokenSource? timeCts = null;
        CancellationToken searchToken = ct;
        if (_depth >= 3)
        {
            timeCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeCts.CancelAfter(TimeSpan.FromSeconds(4.0));
            searchToken = timeCts.Token;
        }

        try
        {
            return SearchBest(board, searchToken);
        }
        finally
        {
            timeCts?.Dispose();
        }
    }

    private ChessMove? SearchBest(ChessBoard board, CancellationToken ct)
    {
        var moves = ChessEngine.GetAllLegalMoves(board, board.CurrentTurn);
        OrderMoves(board, moves);

        ChessMove? best    = null;
        int        bestVal = int.MinValue + 1;

        // Ruído só nos modos mais fáceis para variar abertura sem prejudicar qualidade
        int noise = _randomness > 0 ? 5 : 0;

        foreach (var move in moves)
        {
            if (ct.IsCancellationRequested) break;
            var clone = board.Clone();
            ChessEngine.ApplyMove(clone, move);
            int score = -Negamax(clone, _depth - 1, int.MinValue + 1, int.MaxValue - 1, ct)
                        + Random.Shared.Next(noise + 1);

            if (score > bestVal) { bestVal = score; best = move; }
        }

        // Se a busca foi interrompida e não encontrou nada, retorna o primeiro movimento legal
        return best ?? moves.FirstOrDefault();
    }

    // -------------------------------------------------------------------------
    private int Negamax(ChessBoard board, int depth, int alpha, int beta, CancellationToken ct)
    {
        if (ct.IsCancellationRequested) return 0;

        var state = ChessEngine.GetGameState(board);
        if (state == GameState.Checkmate)                   return -(20_000 + depth * 100);
        if (state is GameState.Stalemate or GameState.Draw) return 0;
        if (depth == 0)                                     return Evaluate(board);

        var moves = ChessEngine.GetAllLegalMoves(board, board.CurrentTurn);
        OrderMoves(board, moves);

        foreach (var move in moves)
        {
            if (ct.IsCancellationRequested) break;
            var clone = board.Clone();
            ChessEngine.ApplyMove(clone, move);
            int score = -Negamax(clone, depth - 1, -beta, -alpha, ct);
            alpha = Math.Max(alpha, score);
            if (alpha >= beta) break;
        }

        return alpha;
    }

    // -------------------------------------------------------------------------
    private int Evaluate(ChessBoard board)
    {
        int score    = 0;
        int whiteMob = 0;
        int blackMob = 0;

        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                var p = board.GetPiece(r, c);
                if (p == null) continue;
                int val = Material[(int)p.Type] + PstBonus(p, r, c);
                score += p.Color == board.CurrentTurn ? val : -val;

                // Mobilidade: cada peça vale +2 por movimento disponível
                int mob = ChessEngine.GetLegalMoves(board, r, c).Count;
                if (p.Color == PieceColor.White) whiteMob += mob;
                else                             blackMob += mob;
            }

        // Bônus de mobilidade
        int mobBonus = board.CurrentTurn == PieceColor.White
            ? (whiteMob - blackMob) * 2
            : (blackMob - whiteMob) * 2;

        return score + mobBonus;
    }

    private static int PstBonus(ChessPiece p, int row, int col)
    {
        int r = p.Color == PieceColor.White ? row : 7 - row;
        return p.Type switch
        {
            PieceType.Pawn   => PawnPst[r, col],
            PieceType.Knight => KnightPst[r, col],
            PieceType.Bishop => BishopPst[r, col],
            PieceType.Rook   => RookPst[r, col],
            PieceType.Queen  => QueenPst[r, col],
            PieceType.King   => KingMidPst[r, col],
            _ => 0
        };
    }

    private static void OrderMoves(ChessBoard board, List<ChessMove> moves)
        => moves.Sort((a, b) => MvvLva(board, b).CompareTo(MvvLva(board, a)));

    private static int MvvLva(ChessBoard board, ChessMove m)
    {
        int score    = 0;
        var victim   = board.GetPiece(m.ToRow, m.ToCol);
        var attacker = board.GetPiece(m.FromRow, m.FromCol);
        if (victim != null && attacker != null)
            score += Material[(int)victim.Type] * 10 - Material[(int)attacker.Type];
        if (m.PromotionPiece.HasValue)
            score += Material[(int)m.PromotionPiece.Value];
        return score;
    }
}
