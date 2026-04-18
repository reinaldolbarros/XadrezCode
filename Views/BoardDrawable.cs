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
    private static readonly Color LastMoveColor = Color.FromArgb("#8090EE90");

    private const string KingSymbol      = "♚";
    private const float  KingScale       = 1.20f;
    private const string WhitePawnSymbol = "♙";
    private const string BlackPawnSymbol = "♟";

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

            bool  isWhite       = sq.PieceIsWhite == true;
            bool  isKing        = sq.PieceSymbol == KingSymbol;
            float pieceFontSize = isKing ? fontSize * KingScale : fontSize;

            canvas.FontSize = pieceFontSize;

            if (isWhite)
            {
                // Sombra ao redor (reduzida)
                canvas.FontColor = ShadowColor.WithAlpha(0.16f);
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    canvas.DrawString(sq.PieceSymbol,
                        x + dx * off * 1.4f, y + dy * off * 1.4f, cw, ch,
                        HorizontalAlignment.Center, VerticalAlignment.Center);
                }
                canvas.FontColor = ShadowColor.WithAlpha(0.07f);
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
                // Glow ao redor (reduzido)
                canvas.FontColor = GlowColor.WithAlpha(0.60f);
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

            // ── Detalhe interno nos peões: curva no corpo + linha na base ─
            bool isPawn = sq.PieceSymbol == WhitePawnSymbol || sq.PieceSymbol == BlackPawnSymbol;
            if (isPawn)
            {
                float cx  = x + cw * 0.5f;
                float sw  = MathF.Max(0.6f, cw * 0.020f);
                var   ink = isWhite ? ShadowColor.WithAlpha(0.26f) : GlowColor.WithAlpha(0.24f);

                canvas.StrokeSize  = sw;
                canvas.StrokeColor = ink;

                // Curva suave no corpo (bezier cúbico vertical com leve inflexão)
                float bodyTop = y + ch * 0.43f;
                float bodyBot = y + ch * 0.68f;
                float bodyMid = y + ch * 0.555f;
                float bulge   = cw * 0.055f;   // quanto a curva desvia para o lado

                var path = new PathF();
                path.MoveTo(cx, bodyTop);
                path.CurveTo(
                    cx - bulge, bodyTop + (bodyMid - bodyTop) * 0.4f,
                    cx + bulge, bodyTop + (bodyMid - bodyTop) * 1.6f,
                    cx, bodyBot);
                canvas.DrawPath(path);

                // Linha horizontal na base
                float baseY  = y + ch * 0.78f;
                float halfW  = cw * 0.18f;
                canvas.DrawLine(cx - halfW, baseY, cx + halfW, baseY);
            }
        }
    }
}
