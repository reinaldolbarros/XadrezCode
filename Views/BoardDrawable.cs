using ChessMAUI.Services;
using ChessMAUI.ViewModels;

namespace ChessMAUI.Views;

public class BoardDrawable : IDrawable
{
    public SquareViewModel[,]? Squares { get; set; }

    private static readonly Color WhitePiece    = Colors.White;
    private static readonly Color BlackPiece    = Color.FromArgb("#1A1209");
    private static readonly Color GlowColor     = Colors.White;
    private static readonly Color ShadowColor   = Colors.Black;
    // Verde claro semitransparente para última jogada
    private static readonly Color LastMoveColor = Color.FromArgb("#8090EE90");

    // O rei (♚) renderiza visualmente menor — compensação de tamanho
    private const string KingSymbol = "♚";
    private const float  KingScale  = 1.20f;

    public void Draw(ICanvas canvas, RectF bounds)
    {
        if (Squares == null) return;

        float cw       = bounds.Width  / 8f;
        float ch       = bounds.Height / 8f;
        float fontSize = MathF.Min(cw, ch) * 0.70f;
        float off      = MathF.Max(1.0f, fontSize * 0.042f);

        var (coordLight, coordDark) = BoardThemeService.CoordColors;
        canvas.Antialias = true;

        for (int r = 0; r < 8; r++)
        for (int c = 0; c < 8; c++)
        {
            var   sq = Squares[r, c];
            float x  = c * cw;
            float y  = r * ch;

            // ── Fundo base ────────────────────────────────────────────
            canvas.FillColor = sq.BackgroundColor;
            canvas.FillRectangle(x, y, cw, ch);

            // ── Overlay verde transparente — somente última jogada ────
            if (sq.IsLastMove && !sq.IsSelected && !sq.IsInCheck)
            {
                canvas.FillColor = LastMoveColor;
                canvas.FillRectangle(x, y, cw, ch);
            }

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

            bool  isWhite      = sq.PieceIsWhite == true;
            bool  isKing       = sq.PieceSymbol == KingSymbol;
            float pieceFontSize = isKing ? fontSize * KingScale : fontSize;

            canvas.FontSize = pieceFontSize;

            if (isWhite)
            {
                // Sombra escura ao redor de toda a peça (2 passes, offsets diferentes)
                canvas.FontColor = ShadowColor.WithAlpha(0.28f);
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    canvas.DrawString(sq.PieceSymbol,
                        x + dx * off * 1.4f, y + dy * off * 1.4f, cw, ch,
                        HorizontalAlignment.Center, VerticalAlignment.Center);
                }
                canvas.FontColor = ShadowColor.WithAlpha(0.14f);
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    canvas.DrawString(sq.PieceSymbol,
                        x + dx * off * 2.6f, y + dy * off * 2.6f, cw, ch,
                        HorizontalAlignment.Center, VerticalAlignment.Center);
                }
                // Peça principal: branca
                canvas.FontColor = WhitePiece;
                canvas.DrawString(sq.PieceSymbol,
                    x, y, cw, ch,
                    HorizontalAlignment.Center, VerticalAlignment.Center);
            }
            else
            {
                // Glow branco ao redor (8 direções) + peça preta na frente
                canvas.FontColor = GlowColor;
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    canvas.DrawString(sq.PieceSymbol,
                        x + dx * off, y + dy * off, cw, ch,
                        HorizontalAlignment.Center, VerticalAlignment.Center);
                }
                // Peça principal: quase preta
                canvas.FontColor = BlackPiece;
                canvas.DrawString(sq.PieceSymbol,
                    x, y, cw, ch,
                    HorizontalAlignment.Center, VerticalAlignment.Center);
            }
        }
    }
}
