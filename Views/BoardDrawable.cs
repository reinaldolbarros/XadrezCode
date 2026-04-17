using ChessMAUI.Services;
using ChessMAUI.ViewModels;

namespace ChessMAUI.Views;

public class BoardDrawable : IDrawable
{
    public SquareViewModel[,]? Squares { get; set; }

    private static readonly Color WhiteFill    = Color.FromArgb("#F5F0DC");
    private static readonly Color WhiteOutline  = Color.FromArgb("#4A2E10");
    private static readonly Color BlackFill     = Color.FromArgb("#1A1A2A");
    private static readonly Color BlackOutline  = Color.FromArgb("#C8C8DC");

    public void Draw(ICanvas canvas, RectF bounds)
    {
        if (Squares == null) return;

        float cw       = bounds.Width  / 8f;
        float ch       = bounds.Height / 8f;
        float fontSize = MathF.Min(cw, ch) * 0.70f;
        float off      = MathF.Max(1.0f, fontSize * 0.038f); // deslocamento do contorno

        var (coordLight, coordDark) = BoardThemeService.CoordColors;

        canvas.Antialias = true;

        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                var   sq = Squares[r, c];
                float x  = c * cw;
                float y  = r * ch;

                // ── Fundo da casa ─────────────────────────────────────────
                canvas.FillColor = sq.BackgroundColor;
                canvas.FillRectangle(x, y, cw, ch);

                // ── Coordenadas ───────────────────────────────────────────
                float cs = MathF.Max(7f, cw * 0.17f);
                canvas.FontSize  = cs;
                canvas.FontColor = sq.IsLight ? coordLight : coordDark;
                if (c == 0)
                    canvas.DrawString(((char)('8' - r)).ToString(),
                        x + 2, y + 1, cw * 0.3f, ch * 0.3f,
                        HorizontalAlignment.Left, VerticalAlignment.Top);
                if (r == 7)
                    canvas.DrawString(((char)('a' + c)).ToString(),
                        x, y + ch - cs * 1.4f, cw - 2, cs * 1.4f,
                        HorizontalAlignment.Right, VerticalAlignment.Bottom);

                // ── Peça ──────────────────────────────────────────────────
                if (string.IsNullOrEmpty(sq.PieceSymbol)) continue;

                bool  isWhite = sq.PieceIsWhite == true;
                Color fill    = isWhite ? WhiteFill   : BlackFill;
                Color outline = isWhite ? WhiteOutline : BlackOutline;

                canvas.FontSize = fontSize;

                // Contorno: 8 posições ao redor
                canvas.FontColor = outline;
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    canvas.DrawString(sq.PieceSymbol,
                        x + dx * off, y + dy * off, cw, ch,
                        HorizontalAlignment.Center, VerticalAlignment.Center);
                }

                // Peça principal
                canvas.FontColor = fill;
                canvas.DrawString(sq.PieceSymbol,
                    x, y, cw, ch,
                    HorizontalAlignment.Center, VerticalAlignment.Center);
            }
        }
    }
}
