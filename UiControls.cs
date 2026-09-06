using System.Drawing.Drawing2D;

namespace FanSlider;

internal sealed class CardPanel : Panel
{
    public Color Accent { get; set; } = UiTheme.CpuAccent;
    public bool ShowAccentBar { get; set; } = true;

    public CardPanel()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = UiTheme.BgSurface;   // 子控件继承此色
        Padding = new Padding(14, 10, 14, 12);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        UiTheme.EnableClearType(e.Graphics);
        e.Graphics.Clear(UiTheme.BgWindow);   // 圆角外露出窗体色
        var r = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
        using var path = UiTheme.RoundRect(r, 10);
        using (var b = new SolidBrush(UiTheme.BgSurface)) e.Graphics.FillPath(b, path);
        using (var p = new Pen(UiTheme.Hairline)) e.Graphics.DrawPath(p, path);
        if (ShowAccentBar)
        {
            using var bar = new SolidBrush(Accent);
            e.Graphics.FillRectangle(bar, 0, 12, 4, Height - 24);
        }
        base.OnPaint(e);
    }
}

internal sealed class TogglePill : Control
{
    bool _checked;
    public bool Checked
    {
        get => _checked;
        set { if (_checked == value) return; _checked = value; CheckedChanged?.Invoke(this, EventArgs.Empty); Invalidate(); }
    }
    public event EventHandler? CheckedChanged;
    public Color OnColor { get; set; } = UiTheme.CpuAccent;
    public bool Busy { get; set; }

    public TogglePill()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = UiTheme.BgSurface;
        Size = new Size(56, 28);
        Cursor = Cursors.Hand;
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        if (Enabled && e.Button == MouseButtons.Left && !Busy) Checked = !Checked;
        base.OnMouseClick(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        UiTheme.EnableClearType(e.Graphics);
        e.Graphics.Clear(BackColor);
        var r = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
        using var path = UiTheme.RoundRect(r, Height / 2f);
        Color fill = !Enabled ? UiTheme.TrackOff
                   : Busy ? UiTheme.Warn
                   : _checked ? OnColor
                   : UiTheme.TrackOff;
        using (var b = new SolidBrush(fill)) e.Graphics.FillPath(b, path);
        float knob = Height - 8;
        float x = _checked ? Width - knob - 4 : 4;
        using (var b = new SolidBrush(UiTheme.InkPrimary))
            e.Graphics.FillEllipse(b, x, 4, knob, knob);
    }
}

internal sealed class DutySlider : Control
{
    int _value = 50;
    int? _live;
    bool _dragging;

    public int Minimum { get; set; }
    public int Maximum { get; set; } = 100;
    public Color Accent { get; set; } = UiTheme.CpuAccent;
    public Color Dim { get; set; } = UiTheme.CpuDim;
    public int Value
    {
        get => _value;
        set
        {
            int v = Math.Clamp(value, Minimum, Maximum);
            if (v == _value) return;
            _value = v;
            ValueChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
    }
    public int? LiveValue { get => _live; set { _live = value; Invalidate(); } }
    public event EventHandler? ValueChanged;

    public DutySlider()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
        BackColor = UiTheme.BgSurface;
        Height = 64;
        TabStop = true;
        Cursor = Cursors.Hand;
    }

    float TrackY => Height * 0.42f;
    RectangleF TrackRect
    {
        get
        {
            float pad = 12;
            return new RectangleF(pad, TrackY - 5, Width - pad * 2, 10);
        }
    }

    int ValueFromX(int x)
    {
        var t = TrackRect;
        float f = t.Width <= 0 ? 0 : (x - t.X) / t.Width;
        return (int)Math.Round(Minimum + Math.Clamp(f, 0, 1) * (Maximum - Minimum));
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (!Enabled || e.Button != MouseButtons.Left) return;
        _dragging = true;
        Capture = true;
        Value = ValueFromX(e.X);
        Focus();
        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (_dragging) Value = ValueFromX(e.X);
        base.OnMouseMove(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _dragging = false;
        Capture = false;
        base.OnMouseUp(e);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (!Enabled) return;
        Value = _value + (e.Delta > 0 ? 1 : -1);
        base.OnMouseWheel(e);
    }

    protected override bool IsInputKey(Keys keyData) =>
        keyData is Keys.Left or Keys.Right or Keys.Home or Keys.End || base.IsInputKey(keyData);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (!Enabled) return;
        if (e.KeyCode == Keys.Left) Value = _value - 1;
        else if (e.KeyCode == Keys.Right) Value = _value + 1;
        else if (e.KeyCode == Keys.Home) Value = Minimum;
        else if (e.KeyCode == Keys.End) Value = Maximum;
        base.OnKeyDown(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        UiTheme.EnableClearType(e.Graphics);
        e.Graphics.Clear(BackColor);
        var t = TrackRect;
        using (var path = UiTheme.RoundRect(t, 5))
        using (var b = new SolidBrush(UiTheme.TrackOff))
            e.Graphics.FillPath(b, path);

        float frac = (Maximum == Minimum) ? 0 : (_value - Minimum) / (float)(Maximum - Minimum);
        float fillW = Math.Max(8, t.Width * frac);
        var fill = new RectangleF(t.X, t.Y, fillW, t.Height);
        Color accent = Enabled ? Accent : Dim;
        using (var path = UiTheme.RoundRect(fill, 5))
        {
            using (var glow = new SolidBrush(Color.FromArgb(50, accent)))
            {
                var gRect = new RectangleF(fill.X, fill.Y - 3, fill.Width, fill.Height + 6);
                using var gp = UiTheme.RoundRect(gRect, 6);
                e.Graphics.FillPath(glow, gp);
            }
            using var b = new SolidBrush(accent);
            e.Graphics.FillPath(b, path);
        }

        float thumbX = t.X + t.Width * frac;
        float thumbR = Enabled ? 8 : 6;
        using (var b = new SolidBrush(UiTheme.InkPrimary))
            e.Graphics.FillEllipse(b, thumbX - thumbR, TrackY - thumbR, thumbR * 2, thumbR * 2);
        using (var p = new Pen(accent, 2))
            e.Graphics.DrawEllipse(p, thumbX - thumbR, TrackY - thumbR, thumbR * 2, thumbR * 2);

        if (_live is int live)
        {
            float lf = (Maximum == Minimum) ? 0 : (live - Minimum) / (float)(Maximum - Minimum);
            float lx = t.X + t.Width * Math.Clamp(lf, 0, 1);
            using var p = new Pen(Color.FromArgb(180, UiTheme.InkPrimary), 1.5f) { DashStyle = DashStyle.Dash };
            e.Graphics.DrawLine(p, lx, t.Y - 8, lx, t.Bottom + 8);
        }

        // 刻度跟随轨道几何: 0/25/50/75/100% 画在轨道下方
        using (var f = new Font("Consolas", 8f))
        using (var b = new SolidBrush(UiTheme.InkMuted))
        {
            for (int i = 0; i <= 4; i++)
            {
                int v = Minimum + (Maximum - Minimum) * i / 4;
                string txt = i == 4 ? "100%" : v.ToString();
                var sz = TextRenderer.MeasureText(txt, f);
                float x = Math.Clamp(t.X + t.Width * i / 4f - sz.Width / 2f, 0, Width - sz.Width);
                TextRenderer.DrawText(e.Graphics, txt, f,
                    new Point((int)x, (int)t.Bottom + 10), UiTheme.InkMuted,
                    TextFormatFlags.NoPadding);
            }
        }
    }
}

internal sealed class StatusChip : Control
{
    public Color Led { get; set; } = UiTheme.InkMuted;
    string _caption = "";
    public string Caption { get => _caption; set { _caption = value; Invalidate(); } }

    public StatusChip()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        BackColor = UiTheme.BgWindow;
        Height = 26;
        AutoSize = false;
        Font = UiTheme.FSmall;
        ForeColor = UiTheme.InkSecondary;
        Padding = new Padding(8, 0, 10, 0);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        UiTheme.EnableClearType(e.Graphics);
        e.Graphics.Clear(BackColor);
        var r = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
        using var path = UiTheme.RoundRect(r, 8);
        using (var b = new SolidBrush(UiTheme.BgSurfaceHi)) e.Graphics.FillPath(b, path);
        using (var led = new SolidBrush(Led)) e.Graphics.FillEllipse(led, 8, Height / 2f - 4, 8, 8);
        TextRenderer.DrawText(e.Graphics, _caption, Font, new Rectangle(20, 0, Width - 24, Height),
            ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
    }

    public void Fit()
    {
        var sz = TextRenderer.MeasureText(_caption, Font);
        Width = Math.Max(90, sz.Width + 36);
    }
}

/// <summary>
/// 16 点温度-占空比曲线编辑器。x = 温度轴 (UpT, 取自 EC 表/默认阶梯), y = duty 0-100%。
/// T0 (最低温度点) 锁定为 0 —— 固件把 0xF20/0xF50 的 T0 当"表已启用"标记 (img 0x977d/0x9783)。
/// 拖动圆点编辑; 悬停/拖动时显示 "T{i} {temp}°C · {duty}%"。
/// </summary>
internal sealed class CurveEditor : Control
{
    readonly int[] _duty = new int[FanTableModel.Points];   // 0-100, [0] 锁定 0
    readonly byte[] _upT = new byte[FanTableModel.Points];  // 温度轴
    int _dragIdx = -1;
    int _hoverIdx = -1;

    public Color Accent { get; set; } = UiTheme.CpuAccent;
    public Color Dim { get; set; } = UiTheme.CpuDim;
    public event EventHandler? CurveChanged;
    public int[] Duty => _duty;
    public bool IsUniform
    {
        get
        {
            int v = _duty[1];
            for (int i = 2; i < FanTableModel.Points; i++) if (_duty[i] != v) return false;
            return true;
        }
    }
    public int UniformValue => IsUniform ? _duty[1] : -1;

    public CurveEditor()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
        BackColor = UiTheme.BgSurface;
        Height = 118;
        Cursor = Cursors.Cross;
    }

    public void SetAxis(byte[] upT)
    {
        if (upT is not null) Array.Copy(upT, _upT, Math.Min(upT.Length, FanTableModel.Points));
        Invalidate();
    }

    /// <summary>整体定速: 全 16 点 = pct, T0 锁定 0。</summary>
    public void SetAll(int pct)
    {
        pct = Math.Clamp(pct, 0, 100);
        for (int i = 1; i < FanTableModel.Points; i++) _duty[i] = pct;
        _duty[0] = 0;
        CurveChanged?.Invoke(this, EventArgs.Empty);
        Invalidate();
    }

    /// <summary>设置逐点曲线 (T0 强制 0)。</summary>
    public void SetDuty(int[] duty)
    {
        for (int i = 0; i < FanTableModel.Points; i++)
            _duty[i] = i < duty.Length ? Math.Clamp(duty[i], 0, 100) : 0;
        _duty[0] = 0;
        CurveChanged?.Invoke(this, EventArgs.Empty);
        Invalidate();
    }

    // ---------------- geometry ----------------

    RectangleF Plot
    {
        get
        {
            float padL = 30, padR = 8, padT = 8, padB = 16;
            return new RectangleF(padL, padT, Math.Max(40, Width - padL - padR), Math.Max(40, Height - padT - padB));
        }
    }

    (int tMin, int tMax) TempRange
    {
        get
        {
            int mn = 255, mx = 0;
            for (int i = 1; i < FanTableModel.Points; i++)
            {
                byte t = _upT[i];
                if (t == 0 || t == FanTableModel.PadTemp) continue;
                if (t < mn) mn = t;
                if (t > mx) mx = t;
            }
            if (mx <= mn) { mn = 30; mx = 100; }
            return (mn, mx);
        }
    }

    float X(byte temp)
    {
        var p = Plot;
        var (mn, mx) = TempRange;
        float t = Math.Clamp(temp, (byte)mn, (byte)mx);
        float f = mx == mn ? 0 : (t - mn) / (float)(mx - mn);
        return p.X + f * p.Width;
    }

    float Y(int duty) => Plot.Bottom - Math.Clamp(duty, 0, 100) / 100f * Plot.Height;

    int DutyFromY(float y)
    {
        var p = Plot;
        float f = p.Height <= 0 ? 0 : (p.Bottom - y) / p.Height;
        return Math.Clamp((int)Math.Round(f * 100), 0, 100);
    }

    int HitTest(float x, float y)
    {
        int best = -1; float bestD = 10;
        for (int i = 0; i < FanTableModel.Points; i++)
        {
            float px = X(_upT[i]), py = Y(_duty[i]);
            float d = MathF.Sqrt((px - x) * (px - x) + (py - y) * (py - y));
            if (d < bestD) { bestD = d; best = i; }
        }
        return best;
    }

    // ---------------- interaction ----------------

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (!Enabled || e.Button != MouseButtons.Left) return;
        int idx = HitTest(e.X, e.Y);
        if (idx <= 0) return;                 // T0 锁定, 不可拖
        _dragIdx = idx;
        _hoverIdx = idx;
        Capture = true;
        Focus();
        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (_dragIdx > 0)
        {
            int v = DutyFromY(e.Y);
            if (_duty[_dragIdx] != v)
            {
                _duty[_dragIdx] = v;
                CurveChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
            return;
        }
        int h = HitTest(e.X, e.Y);
        if (h != _hoverIdx) { _hoverIdx = h; Invalidate(); }
        base.OnMouseMove(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _dragIdx = -1;
        Capture = false;
        base.OnMouseUp(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _hoverIdx = -1;
        Invalidate();
        base.OnMouseLeave(e);
    }

    // ---------------- painting ----------------

    protected override void OnPaint(PaintEventArgs e)
    {
        UiTheme.EnableClearType(e.Graphics);
        var g = e.Graphics;
        g.Clear(BackColor);
        var p = Plot;
        var (tMin, tMax) = TempRange;

        // 网格 + duty 刻度 (左)
        using var gridPen = new Pen(Color.FromArgb(30, UiTheme.Hairline));
        using var txt = new Font("Consolas", 7.5f);
        for (int d = 0; d <= 100; d += 25)
        {
            float y = Y(d);
            g.DrawLine(gridPen, p.X, y, p.Right, y);
            TextRenderer.DrawText(g, $"{d}%", txt,
                new Point(0, (int)y - 6), UiTheme.InkMuted, TextFormatFlags.NoPadding);
        }

        // 温度刻度 (底)
        int step = Math.Max(1, (tMax - tMin) / 6);
        for (int t = tMin; t <= tMax; t += step)
        {
            float x = X((byte)t);
            g.DrawLine(gridPen, x, p.Y, x, p.Bottom);
            TextRenderer.DrawText(g, $"{t}", txt,
                new Point((int)x - 8, (int)p.Bottom + 2), UiTheme.InkMuted, TextFormatFlags.NoPadding);
        }

        // 填充区 + 折线
        var pts = new PointF[FanTableModel.Points];
        for (int i = 0; i < FanTableModel.Points; i++) pts[i] = new PointF(X(_upT[i]), Y(_duty[i]));
        using (var fill = new SolidBrush(Color.FromArgb(36, Enabled ? Accent : Dim)))
        {
            var poly = new PointF[FanTableModel.Points + 2];
            Array.Copy(pts, poly, FanTableModel.Points);
            poly[FanTableModel.Points] = new PointF(pts[FanTableModel.Points - 1].X, p.Bottom);
            poly[FanTableModel.Points + 1] = new PointF(pts[0].X, p.Bottom);
            g.FillPolygon(fill, poly);
        }
        using (var line = new Pen(Enabled ? Accent : Dim, 2f))
            g.DrawLines(line, pts);

        // 圆点 (T0 空心锁定, 其余实心; 悬停/拖动放大)
        for (int i = 0; i < FanTableModel.Points; i++)
        {
            float x = X(_upT[i]), y = Y(_duty[i]);
            bool active = i == _dragIdx || i == _hoverIdx;
            float r = i == 0 ? 3.5f : active ? 5.5f : 4f;
            using var b = new SolidBrush(i == 0 ? BackColor : (Enabled ? Accent : Dim));
            g.FillEllipse(b, x - r, y - r, r * 2, r * 2);
            using var pen = new Pen(Enabled ? Accent : Dim, i == 0 ? 1.5f : 1f);
            g.DrawEllipse(pen, x - r, y - r, r * 2, r * 2);
            if (i == 0)
                TextRenderer.DrawText(g, "T0=0", txt, new Point((int)x + 5, (int)y - 6), UiTheme.InkMuted, TextFormatFlags.NoPadding);
        }

        // 悬停/拖动读数
        if (_hoverIdx > 0)
        {
            int i = _hoverIdx;
            TextRenderer.DrawText(g, $"T{i}  {_upT[i]}°C · {_duty[i]}%",
                UiTheme.FMono, new Point((int)p.X + 2, 0), UiTheme.InkPrimary, TextFormatFlags.NoPadding);
        }
    }
}
