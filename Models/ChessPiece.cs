namespace ChessMAUI.Models;

public class ChessPiece
{
    public PieceType Type { get; set; }
    public PieceColor Color { get; set; }
    public bool HasMoved { get; set; }

    public ChessPiece(PieceType type, PieceColor color)
    {
        Type = type;
        Color = color;
        HasMoved = false;
    }

    public ChessPiece Clone() => new ChessPiece(Type, Color) { HasMoved = HasMoved };

    public string Symbol => (Type, Color) switch
    {
        (PieceType.King,   PieceColor.White) => "♔",
        (PieceType.Queen,  PieceColor.White) => "♕",
        (PieceType.Rook,   PieceColor.White) => "♖",
        (PieceType.Bishop, PieceColor.White) => "♗",
        (PieceType.Knight, PieceColor.White) => "♘",
        (PieceType.Pawn,   PieceColor.White) => "♙",
        (PieceType.King,   PieceColor.Black) => "♚",
        (PieceType.Queen,  PieceColor.Black) => "♛",
        (PieceType.Rook,   PieceColor.Black) => "♜",
        (PieceType.Bishop, PieceColor.Black) => "♝",
        (PieceType.Knight, PieceColor.Black) => "♞",
        (PieceType.Pawn,   PieceColor.Black) => "♟",
        _ => "?"
    };
}
