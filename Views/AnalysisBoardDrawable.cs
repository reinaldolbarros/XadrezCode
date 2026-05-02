using ChessMAUI.Models;
using ChessMAUI.Services;

namespace ChessMAUI.Views;

public class AnalysisBoardDrawable : IDrawable
{
    public ChessBoard? Board { get; set; }

    private int   _h1r = -1, _h1c = -1, _h2r = -1, _h2c = -1;
    private Color _highlightColor = Colors.Transparent;

    public void SetHighlight(int fromRow, int fromCol, int toRow, int toCol, Color color)
    {
        _h1r = fromRow; _h1c = fromCol;
        _h2r = toRow;   _h2c = toCol;
        _highlightColor = color;
    }

    public void ClearHighlight()
    {
        _h1r = _h1c = _h2r = _h2c = -1;
        _highlightColor = Colors.Transparent;
    }

    public void Draw(ICanvas canvas, RectF bounds)
    {
        if (Board == null) return;

        var (lightColor, darkColor) = BoardThemeService.BoardColors;
        float cw       = bounds.Width  / 8f;
        float ch       = bounds.Height / 8f;
        float fontSize = MathF.Min(cw, ch) * 0.76f;
        float off      = MathF.Max(1.5f, fontSize * 0.055f);

        canvas.Antialias = true;

        for (int r = 0; r < 8; r++)
        for (int c = 0; c < 8; c++)
        {
            bool  isLight = (r + c) % 2 == 0;
            float x = c * cw;
            float y = r * ch;

            canvas.FillColor = isLight ? lightColor : darkColor;
            canvas.FillRectangle(x, y, cw, ch);

            bool isH1 = r == _h1r && c == _h1c;
            bool isH2 = r == _h2r && c == _h2c;
            if ((isH1 || isH2) && _h1r >= 0)
            {
                canvas.FillColor = _highlightColor;
                canvas.FillRectangle(x, y, cw, ch);
            }

            var piece = Board.GetPiece(r, c);
            if (piece == null) continue;

            bool  isWhite = piece.Color == PieceColor.White;
            canvas.FontSize = piece.Type == PieceType.King ? fontSize * 1.20f : fontSize;

            if (isWhite)
            {
                canvas.FontColor = Colors.Black.WithAlpha(0.30f);
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    canvas.DrawString(piece.Symbol,
                        x + dx * off * 1.8f, y + dy * off * 1.8f, cw, ch,
                        HorizontalAlignment.Center, VerticalAlignment.Center);
                }
                canvas.FontColor = Color.FromArgb("#F5F0DC");
                canvas.DrawString(piece.Symbol, x, y, cw, ch,
                    HorizontalAlignment.Center, VerticalAlignment.Center);
            }
            else
            {
                canvas.FontColor = Colors.White.WithAlpha(0.78f);
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    canvas.DrawString(piece.Symbol,
                        x + dx * off * 1.2f, y + dy * off * 1.2f, cw, ch,
                        HorizontalAlignment.Center, VerticalAlignment.Center);
                }
                canvas.FontColor = Color.FromArgb("#1C1C2C");
                canvas.DrawString(piece.Symbol, x, y, cw, ch,
                    HorizontalAlignment.Center, VerticalAlignment.Center);
            }
        }
    }
}
