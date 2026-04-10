using ChessMAUI.Models;

namespace ChessMAUI.Services;

public class AIService
{
    private readonly int SearchDepth;

    public AIService(int depth = 3) => SearchDepth = Math.Clamp(depth, 1, 5);

    // Valores materiais: Peão, Cavalo, Bispo, Torre, Rainha, Rei
    private static readonly int[] Material = [100, 320, 330, 500, 900, 20_000];

    // Tabelas de bônus posicional (perspectiva das Brancas; Pretas usam linha espelhada)
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
    private static readonly int[,] KingPst = {
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
    // Ponto de entrada assíncrono
    // -------------------------------------------------------------------------
    public Task<ChessMove?> GetBestMoveAsync(ChessBoard board, CancellationToken ct = default)
        => Task.Run(() => FindBest(board, ct), ct);

    private ChessMove? FindBest(ChessBoard board, CancellationToken ct)
    {
        var moves = ChessEngine.GetAllLegalMoves(board, board.CurrentTurn);
        if (moves.Count == 0) return null;

        OrderMoves(board, moves);

        ChessMove? best = null;
        int bestScore = int.MinValue;

        foreach (var move in moves)
        {
            if (ct.IsCancellationRequested) break;
            var clone = board.Clone();
            ChessEngine.ApplyMove(clone, move);
            int score = -Negamax(clone, SearchDepth - 1, int.MinValue + 1, int.MaxValue - 1, ct);
            if (score > bestScore) { bestScore = score; best = move; }
        }

        return best;
    }

    // -------------------------------------------------------------------------
    // Negamax com poda alpha-beta
    // -------------------------------------------------------------------------
    private int Negamax(ChessBoard board, int depth, int alpha, int beta, CancellationToken ct)
    {
        if (ct.IsCancellationRequested) return 0;

        var state = ChessEngine.GetGameState(board);
        if (state == GameState.Checkmate)                    return -(20_000 + depth * 100);
        if (state is GameState.Stalemate or GameState.Draw)  return 0;
        if (depth == 0)                                      return Evaluate(board);

        var moves = ChessEngine.GetAllLegalMoves(board, board.CurrentTurn);
        OrderMoves(board, moves);

        foreach (var move in moves)
        {
            if (ct.IsCancellationRequested) break;
            var clone = board.Clone();
            ChessEngine.ApplyMove(clone, move);
            int score = -Negamax(clone, depth - 1, -beta, -alpha, ct);
            alpha = Math.Max(alpha, score);
            if (alpha >= beta) break; // corte beta
        }

        return alpha;
    }

    // -------------------------------------------------------------------------
    // Avalia o tabuleiro do ponto de vista do jogador atual
    // -------------------------------------------------------------------------
    private int Evaluate(ChessBoard board)
    {
        int score = 0;
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                var p = board.GetPiece(r, c);
                if (p == null) continue;
                int val = Material[(int)p.Type] + PstBonus(p, r, c);
                score += p.Color == board.CurrentTurn ? val : -val;
            }
        return score;
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
            PieceType.King   => KingPst[r, col],
            _ => 0
        };
    }

    // -------------------------------------------------------------------------
    // Ordena movimentos: capturas vantajosas e promoções primeiro (MVV-LVA)
    // -------------------------------------------------------------------------
    private static void OrderMoves(ChessBoard board, List<ChessMove> moves)
    {
        moves.Sort((a, b) => MvvLva(board, b).CompareTo(MvvLva(board, a)));
    }

    private static int MvvLva(ChessBoard board, ChessMove m)
    {
        int score = 0;
        var victim   = board.GetPiece(m.ToRow, m.ToCol);
        var attacker = board.GetPiece(m.FromRow, m.FromCol);
        if (victim != null && attacker != null)
            score += Material[(int)victim.Type] * 10 - Material[(int)attacker.Type];
        if (m.PromotionPiece.HasValue)
            score += Material[(int)m.PromotionPiece.Value];
        return score;
    }
}
