namespace ChessMAUI.Models;

public class ChessBoard
{
    public ChessPiece?[,] Squares { get; private set; } = new ChessPiece?[8, 8];
    public PieceColor CurrentTurn { get; set; } = PieceColor.White;

    // En passant: coluna alvo onde o peão pode capturar (-1 = nenhum)
    public int EnPassantCol { get; set; } = -1;
    public int EnPassantRow { get; set; } = -1;

    // Meio-movimento (regra dos 50 movimentos)
    public int HalfMoveClock { get; set; } = 0;
    public int FullMoveNumber { get; set; } = 1;

    // Histórico de posições para detecção de repetição tripla
    public Dictionary<string, int> PositionHistory { get; set; } = [];

    public ChessBoard()
    {
        InitializeBoard();
    }

    private ChessBoard(ChessPiece?[,] squares, PieceColor turn, int epRow, int epCol, int half, int full, Dictionary<string, int> history)
    {
        Squares = squares;
        CurrentTurn = turn;
        EnPassantRow = epRow;
        EnPassantCol = epCol;
        HalfMoveClock = half;
        FullMoveNumber = full;
        PositionHistory = new Dictionary<string, int>(history);
    }

    public void InitializeBoard()
    {
        // Peças pretas (linha 0 e 1)
        Squares[0, 0] = new ChessPiece(PieceType.Rook,   PieceColor.Black);
        Squares[0, 1] = new ChessPiece(PieceType.Knight, PieceColor.Black);
        Squares[0, 2] = new ChessPiece(PieceType.Bishop, PieceColor.Black);
        Squares[0, 3] = new ChessPiece(PieceType.Queen,  PieceColor.Black);
        Squares[0, 4] = new ChessPiece(PieceType.King,   PieceColor.Black);
        Squares[0, 5] = new ChessPiece(PieceType.Bishop, PieceColor.Black);
        Squares[0, 6] = new ChessPiece(PieceType.Knight, PieceColor.Black);
        Squares[0, 7] = new ChessPiece(PieceType.Rook,   PieceColor.Black);
        for (int c = 0; c < 8; c++)
            Squares[1, c] = new ChessPiece(PieceType.Pawn, PieceColor.Black);

        // Peças brancas (linha 6 e 7)
        for (int c = 0; c < 8; c++)
            Squares[6, c] = new ChessPiece(PieceType.Pawn, PieceColor.White);
        Squares[7, 0] = new ChessPiece(PieceType.Rook,   PieceColor.White);
        Squares[7, 1] = new ChessPiece(PieceType.Knight, PieceColor.White);
        Squares[7, 2] = new ChessPiece(PieceType.Bishop, PieceColor.White);
        Squares[7, 3] = new ChessPiece(PieceType.Queen,  PieceColor.White);
        Squares[7, 4] = new ChessPiece(PieceType.King,   PieceColor.White);
        Squares[7, 5] = new ChessPiece(PieceType.Bishop, PieceColor.White);
        Squares[7, 6] = new ChessPiece(PieceType.Knight, PieceColor.White);
        Squares[7, 7] = new ChessPiece(PieceType.Rook,   PieceColor.White);
    }

    public ChessBoard Clone()
    {
        var newSquares = new ChessPiece?[8, 8];
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                newSquares[r, c] = Squares[r, c]?.Clone();

        return new ChessBoard(newSquares, CurrentTurn, EnPassantRow, EnPassantCol, HalfMoveClock, FullMoveNumber, PositionHistory);
    }

    public ChessPiece? GetPiece(int row, int col) =>
        IsInBounds(row, col) ? Squares[row, col] : null;

    public void SetPiece(int row, int col, ChessPiece? piece) =>
        Squares[row, col] = piece;

    public static bool IsInBounds(int row, int col) =>
        row >= 0 && row < 8 && col >= 0 && col < 8;

    public (int row, int col) FindKing(PieceColor color)
    {
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                if (Squares[r, c]?.Type == PieceType.King && Squares[r, c]?.Color == color)
                    return (r, c);
        return (-1, -1);
    }
}
