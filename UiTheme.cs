using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace FanSlider;

internal static class UiTheme
{
    public static readonly Color BgWindow    = Color.FromArgb(13, 16, 22);
    public static readonly Color BgSurface   = Color.FromArgb(24, 29, 38);
    public static readonly Color BgSurfaceHi = Color.FromArgb(32, 39, 51);
    public static readonly Color BgHeader    = Color.FromArgb(9, 11, 16);
    public static readonly Color Hairline    = Color.FromArgb(44, 52, 66);
    public static readonly Color InkPrimary  = Color.FromArgb(232, 237, 244);
    public static readonly Color InkSecondary= Color.FromArgb(168, 178, 192);
    public static readonly Color InkMuted    = Color.FromArgb(112, 122, 138);
    public static readonly Color CpuAccent   = Color.FromArgb(56, 189, 248);
    public static readonly Color CpuDim      = Color.FromArgb(30, 84, 110);
    public static readonly Color GpuAccent   = Color.FromArgb(251, 146, 60);
    public static readonly Color GpuDim      = Color.FromArgb(120, 72, 32);
    public static readonly Color Ok          = Color.FromArgb(46, 204, 113);
    public static readonly Color Warn        = Color.FromArgb(245, 166, 35);
    public static readonly Color Err         = Color.FromArgb(255, 92, 92);
    public static readonly Color Boost       = Color.FromArgb(255, 59, 92);
    public static readonly Color TrackOff    = Color.FromArgb(38, 44, 55);

    public static readonly Font FBrand     = FontOf("Microsoft YaHei UI", 13.5f, FontStyle.Bold);
    public static readonly Font FCardTitle = FontOf("Microsoft YaHei UI", 10.5f, FontStyle.Bold);
    public static readonly Font FBig       = FontOf("Segoe UI", 28f, FontStyle.Bold);
    public static readonly Font FBody      = FontOf("Microsoft YaHei UI", 9.5f, FontStyle.Regular);
    public static readonly Font FMono      = FontOf("Consolas", 9f, FontStyle.Regular);
    public static readonly Font FSmall     = FontOf("Microsoft YaHei UI", 8.25f, FontStyle.Regular);

    static Font FontOf(string name, float size, FontStyle style)
    {
        try { return new Font(name, size, style, GraphicsUnit.Point); }
        catch { return new Font("Segoe UI", size, style, GraphicsUnit.Point); }
    }

    public static GraphicsPath RoundRect(RectangleF r, float rad)
    {
        rad = Math.Max(0.5f, Math.Min(rad, Math.Min(r.Width, r.Height) / 2f));
        var p = new GraphicsPath();
        float d = rad * 2;
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    public static void EnableClearType(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
    }

    public static void DoubleBuffer(Control c)
    {
        typeof(Control).GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(c, true);
    }
}
