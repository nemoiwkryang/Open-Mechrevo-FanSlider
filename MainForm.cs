using Timer = System.Windows.Forms.Timer;

namespace FanSlider;

/// <summary>
/// v8: 完全独立直写 EC 接管 (2026-09-06 定案)。不依赖 GCUBridge/GCUService:
/// 快照全表区+门控 → 武装 (0x751/0x726/0x7C5/0x7C6/0x741) → 三区交错写表 (T0=0) → 固件自主闭环。
/// 接管租约持久化 (崩溃可恢复), 0x7C5 bit7 被抢自动重接管, 退出/关机按快照交还。
/// 滑条 = 整体定速 (全 16 点), 曲线编辑器 = 逐点微调 (T0 锁定 0)。UI 一律 0-100%。
/// </summary>
public class MainForm : Form
{
    enum UiState { Idle, TakingOver, TakenOver, Applying, Restoring, Error, NoProfile, NoDriver }

    readonly EcDriver _ec = new();
    readonly AppSettings _settings = AppSettings.Load();

    EcCurveSession? _curve;
    bool _cpuCustom, _gpuCustom;
    bool _suppressUiEvents, _exitingHard, _rearming;
    DateTime _stickyUntil = DateTime.MinValue;
    DateTime _lastRearm = DateTime.MinValue;
    UiState _state = UiState.Idle;

    TogglePill _masterPill = null!;
    DutySlider _cpuSlider = null!, _gpuSlider = null!;
    CurveEditor _cpuCurve = null!, _gpuCurve = null!;
    TogglePill _cpuCustomPill = null!, _gpuCustomPill = null!;
    Label _cpuCustomHint = null!, _gpuCustomHint = null!;
    Label _cpuPct = null!, _gpuPct = null!;
    Label _cpuLive = null!, _gpuLive = null!;
    Label _cpuRpm = null!, _gpuRpm = null!;
    Label _cpuTemp = null!, _gpuTemp = null!;
    Label _cpuState = null!, _gpuState = null!;
    StatusChip _chipGcu = null!, _chipBit = null!, _chipSrc = null!, _chipDuty = null!;
    Label _statusLabel = null!;
    Button _applyBtn = null!, _restoreBtn = null!;
    CheckBox _boostCheck = null!;
    Label[] _levelLabels = Array.Empty<Label>();
    CheckBox _restoreAutoOnExitCheck = null!, _autoStartCheck = null!, _trayCheck = null!;
    Timer _pollTimer = null!;
    NotifyIcon _tray = null!;
    bool _closeToTray = true;
    bool _dirty;

    public MainForm()
    {
        Text = "FanSlider — 机械革命 EC 风扇控制台 v8";
        BackColor = UiTheme.BgWindow;
        Font = UiTheme.FBody;
        ForeColor = UiTheme.InkPrimary;
        ClientSize = new Size(780, 660);
        MinimumSize = new Size(740, 620);
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleMode = AutoScaleMode.Dpi;
        DoubleBuffered = true;
        try { Icon = SystemIcons.Shield; } catch { }

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5,
            Padding = new Padding(12, 8, 12, 8), BackColor = UiTheme.BgWindow,
        };
        UiTheme.DoubleBuffer(root);
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));  // header
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // rail
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // channels
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));  // global
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // footer

        root.Controls.Add(MakeHeader(), 0, 0);
        root.Controls.Add(MakeRail(), 0, 1);
        root.Controls.Add(MakeChannels(), 0, 2);
        root.Controls.Add(MakeGlobal(), 0, 3);
        root.Controls.Add(MakeFooter(), 0, 4);
        Controls.Add(root);

        WireEvents();

        _tray = new NotifyIcon { Text = "FanSlider", Visible = true, Icon = SystemIcons.Application };
        _tray.DoubleClick += (_, _) => ShowFromTray();
        var trayMenu = new ContextMenuStrip { Font = UiTheme.FBody };
        trayMenu.Items.Add("显示窗口", null, (_, _) => ShowFromTray());
        trayMenu.Items.Add("恢复自动并退出", null, (_, _) => { _settings.RestoreAutoOnExit = true; _exitingHard = true; Application.Exit(); });
        trayMenu.Items.Add("直接退出", null, (_, _) => { _exitingHard = true; Application.Exit(); });
        _tray.ContextMenuStrip = trayMenu;

        _pollTimer = new Timer { Interval = 2000 };
        _pollTimer.Tick += (_, _) => Poll();

        Load += (_, _) => InitEc();
    }

    // ---------------- UI builders ----------------

    Control MakeHeader()
    {
        var band = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.BgHeader, Margin = new Padding(0, 0, 0, 8) };
        var titleHost = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.BgHeader };
        titleHost.Controls.Add(new Label
        {
            Text = "机械革命 · EC 双风扇独立控制 · v8 直写 EC (无 OEM 服务依赖)", AutoSize = true,
            ForeColor = UiTheme.InkMuted, Font = UiTheme.FSmall, Location = new Point(18, 38),
        });
        titleHost.Controls.Add(new Label
        {
            Text = "FanSlider", AutoSize = true,
            ForeColor = UiTheme.InkPrimary, Font = UiTheme.FBrand, Location = new Point(18, 12),
        });

        var takeover = new Panel { Dock = DockStyle.Right, Width = 240, BackColor = UiTheme.BgHeader };
        _masterPill = new TogglePill { Location = new Point(8, 22), OnColor = UiTheme.Ok, AccessibleName = "接管控制权开关" };
        var pillCaption = new Label
        {
            Text = "接管控制权", AutoSize = true, Font = UiTheme.FCardTitle,
            ForeColor = UiTheme.InkPrimary, Location = new Point(72, 24),
        };
        takeover.Controls.Add(_masterPill);
        takeover.Controls.Add(pillCaption);
        band.Controls.Add(titleHost);
        band.Controls.Add(takeover);
        return band;
    }

    Control MakeRail()
    {
        var rail = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, AutoSize = true, WrapContents = false,
            BackColor = UiTheme.BgWindow, Margin = new Padding(0, 0, 0, 8), Height = 32,
        };
        _chipGcu = MakeChip("GCUBridge: —");
        _chipBit = MakeChip("独立输出 0x7C5.bit7: —");
        _chipSrc = MakeChip("控制源: —");
        _chipDuty = MakeChip("读数 L/R: — / —");
        rail.Controls.AddRange(new Control[] { _chipGcu, _chipBit, _chipSrc, _chipDuty });
        return rail;
    }

    static StatusChip MakeChip(string text)
    {
        var c = new StatusChip { Caption = text, Margin = new Padding(0, 0, 8, 0) };
        c.Fit();
        return c;
    }

    Control MakeChannels()
    {
        var grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0, 0, 0, 8) };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        var cpu = MakeFanCard("CPU 风扇 · 左", "0x75B / 0x464", UiTheme.CpuAccent, UiTheme.CpuDim,
            out _cpuCustomPill, out _cpuCustomHint, out _cpuPct, out _cpuLive, out _cpuRpm, out _cpuTemp,
            out _cpuSlider, out _cpuCurve, out _cpuState);
        var gpu = MakeFanCard("GPU 风扇 · 右", "0x75C / 0x46C", UiTheme.GpuAccent, UiTheme.GpuDim,
            out _gpuCustomPill, out _gpuCustomHint, out _gpuPct, out _gpuLive, out _gpuRpm, out _gpuTemp,
            out _gpuSlider, out _gpuCurve, out _gpuState);
        cpu.Margin = new Padding(0, 0, 6, 0);
        gpu.Margin = new Padding(6, 0, 0, 0);
        grid.Controls.Add(cpu, 0, 0);
        grid.Controls.Add(gpu, 1, 0);
        return grid;
    }

    CardPanel MakeFanCard(string title, string note, Color accent, Color dim,
        out TogglePill custom, out Label customHint, out Label pct, out Label live, out Label rpm, out Label temp,
        out DutySlider slider, out CurveEditor curve, out Label state)
    {
        var card = new CardPanel { Dock = DockStyle.Fill, Accent = accent };
        var grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5 };
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));            // head
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));        // big pct + readout
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));            // 整体定速滑条 (含刻度)
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 134));       // 逐点曲线编辑器
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));            // state line

        var head = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true };
        head.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        head.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var titles = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        titles.Controls.Add(new Label { Text = title, AutoSize = true, Font = UiTheme.FCardTitle, ForeColor = UiTheme.InkPrimary, Margin = new Padding(8, 2, 0, 0) });
        titles.Controls.Add(new Label { Text = note, AutoSize = true, Font = UiTheme.FSmall, ForeColor = UiTheme.InkMuted, Margin = new Padding(8, 0, 0, 0) });
        var customBox = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        custom = new TogglePill { OnColor = accent, Enabled = false, AccessibleName = title + "自定义开关" };
        customHint = new Label { Text = "跟随 OEM 曲线", AutoSize = true, Font = UiTheme.FSmall, ForeColor = UiTheme.InkMuted, Margin = new Padding(0, 2, 0, 0) };
        customBox.Controls.Add(custom);
        customBox.Controls.Add(customHint);
        head.Controls.Add(titles, 0, 0);
        head.Controls.Add(customBox, 1, 0);
        grid.Controls.Add(head, 0, 0);

        var values = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true };
        values.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        values.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        pct = new Label
        {
            Text = "50%", AutoSize = false, Width = 120, Height = 56, Font = UiTheme.FBig,
            ForeColor = UiTheme.InkMuted, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(4, 0, 0, 0),
        };
        var readout = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        live = new Label { Text = "实测  —%", AutoSize = true, Font = UiTheme.FMono, ForeColor = accent, Margin = new Padding(0) };
        rpm = new Label { Text = "转速  ———— (raw)", AutoSize = true, Font = UiTheme.FMono, ForeColor = UiTheme.InkSecondary, Margin = new Padding(0, 2, 0, 0) };
        temp = new Label { Text = "温度  —°C", AutoSize = true, Font = UiTheme.FMono, ForeColor = UiTheme.InkMuted, Margin = new Padding(0, 2, 0, 0) };
        readout.Controls.Add(live);
        readout.Controls.Add(rpm);
        readout.Controls.Add(temp);
        values.Controls.Add(pct, 0, 0);
        values.Controls.Add(readout, 1, 0);
        grid.Controls.Add(values, 0, 1);

        slider = new DutySlider { Dock = DockStyle.Fill, Height = 64, Accent = accent, Dim = dim, Enabled = false, Margin = new Padding(4, 0, 4, 0), AccessibleName = title + "整体定速滑条" };
        var sliderRow = new Panel { Dock = DockStyle.Fill, Margin = new Padding(4, 4, 4, 0) };
        sliderRow.Controls.Add(slider); // Fill 先加 (z 序底), Top 标题后加占顶条
        sliderRow.Controls.Add(new Label { Text = "整体定速 (全部温度点)", AutoSize = true, Dock = DockStyle.Top, Height = 16, Font = UiTheme.FSmall, ForeColor = UiTheme.InkMuted });
        grid.Controls.Add(sliderRow, 0, 2);

        curve = new CurveEditor { Dock = DockStyle.Fill, Accent = accent, Dim = dim, Enabled = false, Margin = new Padding(4, 0, 4, 0), AccessibleName = title + "逐点曲线编辑器" };
        var curveRow = new Panel { Dock = DockStyle.Fill, Margin = new Padding(4, 4, 4, 0) };
        curveRow.Controls.Add(curve);
        curveRow.Controls.Add(new Label { Text = "逐点曲线 (T0 锁定 0, 拖动圆点)", AutoSize = true, Dock = DockStyle.Top, Height = 16, Font = UiTheme.FSmall, ForeColor = UiTheme.InkMuted });
        grid.Controls.Add(curveRow, 0, 3);

        state = new Label
        {
            Text = "未接管", AutoSize = false, Height = 18, Dock = DockStyle.Fill,
            Font = UiTheme.FMono, ForeColor = UiTheme.InkMuted, TextAlign = ContentAlignment.MiddleRight,
        };
        grid.Controls.Add(state, 0, 4);
        card.Controls.Add(grid);
        return card;
    }

    Control MakeGlobal()
    {
        var card = new CardPanel { Dock = DockStyle.Fill, ShowAccentBar = false, Margin = new Padding(0, 0, 0, 8) };
        var row = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4 };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row.Controls.Add(new Label
        {
            Text = "全局档位  (0x787)", AutoSize = true, Font = UiTheme.FSmall,
            ForeColor = UiTheme.InkMuted, Margin = new Padding(8, 16, 12, 0),
        }, 0, 0);

        var segs = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 10, 0, 0) };
        _levelLabels = new Label[6];
        string[] names = ["自动", "L1", "L2", "L3", "L4", "L5"];
        for (int i = 0; i <= 5; i++)
        {
            int t = i;
            _levelLabels[i] = new Label
            {
                Text = names[i], AutoSize = false, Width = 48, Height = 28,
                TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand,
                Font = UiTheme.FBody, ForeColor = UiTheme.InkMuted, BackColor = UiTheme.BgSurfaceHi,
                Margin = new Padding(0, 0, 4, 0),
            };
            _levelLabels[i].Click += (_, _) => ApplyLevel(t);
            segs.Controls.Add(_levelLabels[i]);
        }
        row.Controls.Add(segs, 1, 0);

        _boostCheck = new CheckBox
        {
            Text = "真·满速 FanBoost", AutoSize = true, Font = UiTheme.FBody,
            ForeColor = UiTheme.Boost, BackColor = Color.Transparent, Margin = new Padding(8, 14, 8, 0),
        };
        var tip = new ToolTip();
        tip.SetToolTip(_boostCheck, "OEM 一键强冷组合: 0x727=0x40, 0x751=0x40, 0x726=0x80。噪音极大。");
        row.Controls.Add(_boostCheck, 2, 0);

        _applyBtn = new Button
        {
            Text = "应用滑条值", AutoSize = false, Width = 120, Height = 32,
            FlatStyle = FlatStyle.Flat, BackColor = UiTheme.CpuDim, ForeColor = Color.White,
            Font = UiTheme.FBody, Margin = new Padding(8, 12, 8, 0),
        };
        _applyBtn.FlatAppearance.BorderSize = 0;
        var applyTip = new ToolTip();
        applyTip.SetToolTip(_applyBtn, "直写 EC 表区 (T0=0, 三区交错) 并等待实时 duty 收敛。不依赖任何 OEM 服务。");
        row.Controls.Add(_applyBtn, 3, 0);
        card.Controls.Add(row);
        return card;
    }

    Control MakeFooter()
    {
        var box = new Panel { Dock = DockStyle.Fill, AutoSize = true, BackColor = UiTheme.BgWindow };
        _statusLabel = new Label
        {
            AutoSize = false, Height = 36, Dock = DockStyle.Top,
            Font = UiTheme.FSmall, ForeColor = UiTheme.InkSecondary,
            Text = "● 初始化…",
        };
        var bottom = new TableLayoutPanel { Dock = DockStyle.Top, Height = 36, ColumnCount = 2 };
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var opts = new FlowLayoutPanel { AutoSize = true, WrapContents = true };
        _restoreAutoOnExitCheck = Opt("退出时自动交还", _settings.RestoreAutoOnExit);
        _autoStartCheck = Opt("随 Windows 启动", _settings.AutoStartWithWindows);
        _trayCheck = Opt("关闭窗口 = 隐藏到托盘", true);
        opts.Controls.AddRange(new Control[] { _restoreAutoOnExitCheck, _autoStartCheck, _trayCheck });
        _restoreBtn = new Button
        {
            Text = "恢复自动并交还", AutoSize = false, Width = 160, Height = 32,
            FlatStyle = FlatStyle.Flat, BackColor = UiTheme.BgSurfaceHi, ForeColor = Color.White,
            Font = UiTheme.FBody, Margin = new Padding(8, 2, 0, 0),
        };
        _restoreBtn.FlatAppearance.BorderColor = UiTheme.Warn;
        _restoreBtn.FlatAppearance.BorderSize = 1;
        var restoreTip = new ToolTip();
        restoreTip.SetToolTip(_restoreBtn, "按接管前快照还原全表区与门控, 交还 EC (脱离接管)。");
        bottom.Controls.Add(opts, 0, 0);
        bottom.Controls.Add(_restoreBtn, 1, 0);
        box.Controls.Add(bottom);
        box.Controls.Add(_statusLabel);
        return box;
    }

    static CheckBox Opt(string text, bool on) => new()
    {
        Text = text, AutoSize = true, Font = UiTheme.FSmall, ForeColor = UiTheme.InkMuted,
        Checked = on, Margin = new Padding(0, 6, 12, 0),
    };

    void WireEvents()
    {
        _masterPill.CheckedChanged += (_, _) =>
        {
            if (_suppressUiEvents) return;
            if (_masterPill.Checked) BeginTakeover();
            else BeginRestore();
        };
        _cpuCustomPill.CheckedChanged += (_, _) =>
        {
            if (_suppressUiEvents || _state != UiState.TakenOver) return;
            _cpuCustom = _cpuCustomPill.Checked;
            _dirty = true;
            UpdateCustomHints(); EnableSliders();
        };
        _gpuCustomPill.CheckedChanged += (_, _) =>
        {
            if (_suppressUiEvents || _state != UiState.TakenOver) return;
            _gpuCustom = _gpuCustomPill.Checked;
            _dirty = true;
            UpdateCustomHints(); EnableSliders();
        };
        _cpuSlider.ValueChanged += (_, _) =>
        {
            _cpuPct.Text = $"{_cpuSlider.Value}%";
            if (_suppressUiEvents) return;
            if (_cpuCustom) { _dirty = true; _cpuCurve.SetAll(_cpuSlider.Value); }
            UpdateApplyBtn();
        };
        _gpuSlider.ValueChanged += (_, _) =>
        {
            _gpuPct.Text = $"{_gpuSlider.Value}%";
            if (_suppressUiEvents) return;
            if (_gpuCustom) { _dirty = true; _gpuCurve.SetAll(_gpuSlider.Value); }
            UpdateApplyBtn();
        };
        _cpuCurve.CurveChanged += (_, _) => OnCurveEdited(_cpuCurve, _cpuSlider, _cpuPct, ref _cpuCustom);
        _gpuCurve.CurveChanged += (_, _) => OnCurveEdited(_gpuCurve, _gpuSlider, _gpuPct, ref _gpuCustom);
        _applyBtn.Click += (_, _) => BeginApply();
        _boostCheck.CheckedChanged += (_, _) => { if (!_suppressUiEvents) ApplyBoost(_boostCheck.Checked); };
        _restoreBtn.Click += (_, _) => BeginRestore();
        _restoreAutoOnExitCheck.CheckedChanged += (_, _) => { _settings.RestoreAutoOnExit = _restoreAutoOnExitCheck.Checked; _settings.Save(); };
        _autoStartCheck.CheckedChanged += (_, _) => { SetAutoStart(_autoStartCheck.Checked); _settings.AutoStartWithWindows = _autoStartCheck.Checked; _settings.Save(); };
        _trayCheck.CheckedChanged += (_, _) => _closeToTray = _trayCheck.Checked;
    }

    /// <summary>曲线编辑器被拖动: 标记脏, 均匀曲线时回同步整体滑条, 非均匀时大数字显示"自定"。</summary>
    void OnCurveEdited(CurveEditor editor, DutySlider slider, Label pct, ref bool custom)
    {
        if (_suppressUiEvents || _state != UiState.TakenOver || !custom) return;
        _dirty = true;
        int uni = editor.UniformValue;
        if (uni >= 0)
        {
            _suppressUiEvents = true;
            slider.Value = uni;
            _suppressUiEvents = false;
            pct.Text = $"{uni}%";
        }
        else pct.Text = "自定";
        UpdateApplyBtn();
    }

    // ---------------- lifecycle ----------------

    void InitEc()
    {
        if (!_ec.Open())
        {
            SetState(UiState.NoDriver, $"✕ 驱动不可用 — 无法打开 \\\\.\\ACPIDriver（{_ec.LastError}）。需要管理员权限且 UWACPIDriver 已加载。");
            _pollTimer.Start();
            return;
        }
        var probe = _ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        if (probe is null)
        {
            SetState(UiState.NoDriver, $"✕ 驱动可打开但 ECREAD 失败 — {_ec.LastError}");
            _pollTimer.Start();
            return;
        }

        // 曲线编辑器温度轴: 取 EC 当前 UpT (快照), 读不到则默认阶梯
        _cpuCurve.SetAxis(ReadBlock(FanProtocol.ADDR_CPU_TBL_UP0) ?? DefaultAxis());
        _gpuCurve.SetAxis(ReadBlock(FanProtocol.ADDR_GPU_TBL_UP0) ?? DefaultAxis());

        _suppressUiEvents = true;
        ApplyLevelUi(FanProtocol.DecodeLevel(probe.Value));
        var mode = _ec.Read(FanProtocol.ADDR_FAN_MODE);
        if (mode is not null) _boostCheck.Checked = FanProtocol.IsBoosted(mode.Value);
        _cpuSlider.Value = Math.Clamp(_settings.CpuDuty, 0, 100);
        _gpuSlider.Value = Math.Clamp(_settings.GpuDuty, 0, 100);
        _cpuPct.Text = $"{_cpuSlider.Value}%";
        _gpuPct.Text = $"{_gpuSlider.Value}%";
        _cpuCurve.SetAll(_cpuSlider.Value);
        _gpuCurve.SetAll(_gpuSlider.Value);
        _suppressUiEvents = false;

        // 崩溃恢复: 上次接管未交还 (租约存在) → 按快照还原 EC
        var lease = TakeoverLease.Load();
        if (lease is not null)
        {
            bool restored = lease.RestoreToEc(_ec);
            TakeoverLease.Clear();
            SetState(UiState.Idle, restored
                ? "⚠ 检测到上次未交还的接管, 已按租约还原 EC 与门控。"
                : "⚠ 检测到上次未交还的接管, 但还原失败 — 表区可能残留接管曲线, 建议立即「恢复自动并交还」或重启。");
        }
        else
        {
            SetState(UiState.Idle, "● 待机中 — EC 由 OEM 曲线托管。打开右上「接管控制权」直写曲线接管 (无需任何 OEM 服务)。");
        }
        Poll();
        _pollTimer.Start();
    }

    byte[]? ReadBlock(ushort start)
    {
        var a = new byte[FanTableModel.Points];
        for (int i = 0; i < a.Length; i++)
        {
            var v = _ec.Read((ushort)(start + i));
            if (v is null) return null;
            a[i] = v.Value;
        }
        return a;
    }

    static byte[] DefaultAxis()
    {
        var a = new byte[FanTableModel.Points];
        for (int i = 0; i < a.Length; i++)
        {
            a[i] = (byte)Math.Min(255, 45 + i * 4);
            if (i == a.Length - 1) a[i] = FanTableModel.PadTemp;
        }
        return a;
    }

    void BeginTakeover()
    {
        if (_state is UiState.TakingOver or UiState.Applying or UiState.Restoring) return;
        SetState(UiState.TakingOver, "● 正在接管 — 快照+武装门控 (0x751/0x726/0x7C5/0x7C6/0x741) → 三区写表 (约 2s)…");
        _masterPill.Busy = true;
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                var curve = EcCurveSession.Start(_ec,
                    (int[])_cpuCurve.Duty.Clone(), (int[])_gpuCurve.Duty.Clone());
                BeginInvoke(() =>
                {
                    if (curve is null)
                    {
                        _masterPill.Checked = false;
                        SetState(UiState.Error, $"✕ 接管失败 — {EcCurveSession.LastFailReason}");
                        _stickyUntil = DateTime.UtcNow.AddSeconds(30);
                        return;
                    }
                    _curve = curve;
                    _cpuCustom = _gpuCustom = true;
                    SetTakenOverUi();
                    SetState(UiState.TakenOver, "● 已接管 — 曲线已写入 EC (T0=0, 全表区), 固件自主闭环。拖动滑条/曲线后点「应用」。");
                    _cpuState.Text = _gpuState.Text = "";
                    StampApplied();
                });
            }
            finally
            {
                BeginInvoke(() => { _masterPill.Busy = false; Poll(); });
            }
        });
    }

    void BeginApply()
    {
        if (_curve is not { Active: true }) return;
        if (!_cpuCustom && !_gpuCustom)
        {
            SetState(UiState.TakenOver, "⚠ 两路都是「跟随 OEM 曲线」, 直接点「恢复自动并交还」即可。");
            return;
        }
        SetState(UiState.Applying, "● 正在应用 — 写表区到 EC (约 1-2s), 随后等待实时 duty 收敛 (≤30s)…");
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                // null = 该风扇交还快照 duty 列 (跟随 OEM 曲线), 非 null = 写编辑器曲线
                int[]? cpu = _cpuCustom ? (int[])_cpuCurve.Duty.Clone() : null;
                int[]? gpu = _gpuCustom ? (int[])_gpuCurve.Duty.Clone() : null;
                bool ok = _curve.Apply(cpu, gpu);

                // 收敛验证仅对"均匀曲线"有意义 (单目标值); 非均匀曲线走短静置
                int? wantC = _cpuCurve.IsUniform ? _cpuCurve.UniformValue : null;
                int? wantG = _gpuCurve.IsUniform ? _gpuCurve.UniformValue : null;
                bool converged;
                if (ok && (wantC is not null || wantG is not null))
                    converged = _curve.WaitForEc(wantC, wantG, 30000);
                else { Thread.Sleep(8000); converged = true; }

                BeginInvoke(() =>
                {
                    _dirty = false;
                    UpdateApplyBtn();
                    StampApplied();
                    if (!ok) SetState(UiState.Error, "✕ 应用失败 — 写表区不完整。");
                    else if (converged) SetState(UiState.TakenOver, "● 已应用且已收敛 — 实时 duty 达到目标 (T0=0 表驱动)。");
                    else SetState(UiState.TakenOver, "⚠ 已应用, 但 30s 内 duty 未完全收敛 (GPU 25% 地板或温度处于过渡段)。");
                });
            }
            catch (Exception ex)
            {
                BeginInvoke(() =>
                {
                    SetState(UiState.Error, $"✕ 应用失败 — {ex.Message}");
                    _stickyUntil = DateTime.UtcNow.AddSeconds(30);
                });
            }
            finally
            {
                BeginInvoke(Poll);
            }
        });
    }

    void BeginRestore()
    {
        _suppressUiEvents = true;
        _masterPill.Checked = false;
        _suppressUiEvents = false;
        if (_curve is null)
        {
            SetIdleUi();
            SetState(UiState.Idle, "● 待机中 — EC 由 OEM 曲线托管。");
            return;
        }
        SetState(UiState.Restoring, "● 正在交还 — 还原全表区与门控 (按接管前快照, 约 2s)…");
        _masterPill.Busy = true;
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                _curve.Dispose();
                _curve = null;
                BeginInvoke(() =>
                {
                    SetIdleUi();
                    SetState(UiState.Idle, "● 已交还 — 全表区与门控已按快照还原, EC 回到接管前状态。");
                });
            }
            catch (Exception ex)
            {
                BeginInvoke(() => SetState(UiState.Idle, $"⚠ 交还不完整 — {ex.Message}"));
            }
            finally
            {
                BeginInvoke(() => { _masterPill.Busy = false; Poll(); });
            }
        });
    }

    // ---------------- poll ----------------

    void Poll()
    {
        if (!_ec.IsReady) return;
        byte? user = _ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        byte? mode = _ec.Read(FanProtocol.ADDR_FAN_MODE);
        byte? res = _ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        byte? dutyL = _ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
        byte? dutyR = _ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
        int? rpmL = FanProtocol.ReadRpm(_ec, false);
        int? rpmR = FanProtocol.ReadRpm(_ec, true);
        bool gcu = OemService.IsGcuBridgeRunning();
        bool bit = res is not null && (res.Value & FanProtocol.RESPECTIVE_BIT) != 0;

        if (mode is not null)
        {
            _suppressUiEvents = true;
            _boostCheck.Checked = FanProtocol.IsBoosted(mode.Value);
            _suppressUiEvents = false;
        }

        if (dutyL is not null)
        {
            int p = FanProtocol.NormalizeDuty(dutyL.Value);
            _cpuLive.Text = $"实测 {p}%";
            _cpuSlider.LiveValue = p;
        }
        if (dutyR is not null)
        {
            int p = FanProtocol.NormalizeDuty(dutyR.Value);
            _gpuLive.Text = $"实测 {p}%";
            _gpuSlider.LiveValue = p;
        }
        _cpuRpm.Text = rpmL is int a ? $"转速 {a:N0} (raw)" : "转速  ———— (raw)";
        _gpuRpm.Text = rpmR is int b ? $"转速 {b:N0} (raw)" : "转速  ———— (raw)";
        // 温度占位: 阶段2 Test D 标定 host 可见温度寄存器后实现 (候选 0x1097/0x1103)
        _cpuTemp.Text = "温度  —°C";
        _gpuTemp.Text = "温度  —°C";

        string src = _curve is { Active: true } ? "曲线接管"
                   : (mode is not null && FanProtocol.IsBoosted(mode.Value)) ? "FanBoost 已叠加"
                   : (user is not null && user.Value != 0x00) ? $"OEM 手动档 L{FanProtocol.DecodeLevel(user.Value)}"
                   : "EC 自动";
        SetChip(_chipGcu, gcu ? "GCUBridge: 运行中" : "GCUBridge: 已暂停", gcu ? UiTheme.Ok : UiTheme.InkMuted);
        SetChip(_chipBit, bit ? "独立输出 0x7C5.bit7: 开" : "独立输出 0x7C5.bit7: 关",
            _curve is { Active: true } ? (bit ? UiTheme.Ok : UiTheme.Err) : (bit ? UiTheme.Ok : UiTheme.InkMuted));
        SetChip(_chipSrc, "控制源: " + src, _curve is { Active: true } ? UiTheme.CpuAccent : UiTheme.InkMuted);
        SetChip(_chipDuty,
            $"读数 L/R: {(dutyL is null ? "—" : FanProtocol.NormalizeDuty(dutyL.Value) + "%")} / {(dutyR is null ? "—" : FanProtocol.NormalizeDuty(dutyR.Value) + "%")}",
            UiTheme.InkSecondary);

        _tray.Text = _curve is { Active: true }
            ? $"FanSlider — 已接管 CPU {_cpuSlider.Value}% / GPU {_gpuSlider.Value}%"
            : "FanSlider — 待机";

        // 门控被外部清除检测 (OEM 栈抢权/换挡): 自动重新武装 + 重推当前曲线
        if (_curve is { Active: true } && _state == UiState.TakenOver && !bit)
            TryAutoRearm();
    }

    /// <summary>控制权被抢后自动重接管 (节流 15s, 防重入)。</summary>
    void TryAutoRearm()
    {
        if (_rearming || DateTime.UtcNow - _lastRearm < TimeSpan.FromSeconds(15)) return;
        _rearming = true;
        SetState(UiState.TakenOver, "⚠ 检测到控制权被抢 (0x7C5.bit7 被清), 正在自动重新接管…");
        ThreadPool.QueueUserWorkItem(_ =>
        {
            bool ok;
            try { ok = _curve?.Reassert() == true; }
            catch { ok = false; }
            _lastRearm = DateTime.UtcNow;
            _rearming = false;
            BeginInvoke(() =>
            {
                Poll();
                if (ok) SetState(UiState.TakenOver, "● 已自动重新接管 — 门控与曲线已重推。");
                else SetState(UiState.Error, "✕ 自动重接管失败 — 请手动点「应用滑条值」。");
            });
        });
    }

    static void SetChip(StatusChip chip, string text, Color led)
    {
        chip.Caption = text;
        chip.Led = led;
        chip.Fit();
    }

    void StampApplied()
    {
        var t = DateTime.Now.ToString("HH:mm:ss");
        _cpuState.Text = _cpuCustom ? $"已应用 {t}" : "跟随 OEM";
        _gpuState.Text = _gpuCustom ? $"已应用 {t}" : "跟随 OEM";
    }

    void SetTakenOverUi()
    {
        _suppressUiEvents = true;
        _masterPill.Checked = true;
        _cpuCustomPill.Enabled = true;
        _gpuCustomPill.Enabled = true;
        _cpuCustomPill.Checked = _cpuCustom;
        _gpuCustomPill.Checked = _gpuCustom;
        _suppressUiEvents = false;
        _cpuPct.ForeColor = UiTheme.InkPrimary;
        _gpuPct.ForeColor = UiTheme.InkPrimary;
        UpdateCustomHints();
        EnableSliders();
        foreach (var l in _levelLabels) l.Enabled = false;
    }

    void SetIdleUi()
    {
        _cpuCustom = _gpuCustom = false;
        _suppressUiEvents = true;
        _masterPill.Checked = false;
        _cpuCustomPill.Checked = false;
        _gpuCustomPill.Checked = false;
        _cpuCustomPill.Enabled = false;
        _gpuCustomPill.Enabled = false;
        _suppressUiEvents = false;
        _cpuPct.ForeColor = UiTheme.InkMuted;
        _gpuPct.ForeColor = UiTheme.InkMuted;
        _cpuState.Text = _gpuState.Text = "未接管";
        UpdateCustomHints();
        EnableSliders();
        foreach (var l in _levelLabels) l.Enabled = true;
    }

    void UpdateCustomHints()
    {
        _cpuCustomHint.Text = _cpuCustom ? "自定义" : "跟随 OEM 曲线";
        _gpuCustomHint.Text = _gpuCustom ? "自定义" : "跟随 OEM 曲线";
    }

    void EnableSliders()
    {
        bool live = _curve is { Active: true } && _state is UiState.TakenOver or UiState.Applying;
        _cpuSlider.Enabled = live && _cpuCustom;
        _gpuSlider.Enabled = live && _gpuCustom;
        _cpuCurve.Enabled = live && _cpuCustom;
        _gpuCurve.Enabled = live && _gpuCustom;
    }

    void UpdateApplyBtn() => _applyBtn.Text = _dirty ? "应用滑条值 ●" : "应用滑条值";

    void SetState(UiState state, string text)
    {
        _state = state;
        Color c = state switch
        {
            UiState.TakenOver => UiTheme.Ok,
            UiState.TakingOver or UiState.Applying or UiState.Restoring => UiTheme.Warn,
            UiState.Error or UiState.NoDriver or UiState.NoProfile => UiTheme.Err,
            _ => UiTheme.InkSecondary,
        };
        if (DateTime.UtcNow < _stickyUntil && state != UiState.Error) return;
        _statusLabel.Text = text;
        _statusLabel.ForeColor = c;
        EnableSliders();
    }

    // ---------------- v1 globals ----------------

    void ApplyLevel(int level)
    {
        if (_curve is { Active: true }) return;
        if (!_ec.Open()) return;
        byte value = FanProtocol.EncodeLevel(level);
        if (_ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, value))
        {
            _settings.LastLevel = level; _settings.Save();
            ApplyLevelUi(level);
            SetState(UiState.Idle, level == 0 ? "已交还 EC 自动策略 (0x787←0x00)" : $"0x787←0x{value:X2} (L{level})");
        }
        Poll();
    }

    void ApplyLevelUi(int level)
    {
        for (int i = 0; i <= 5; i++)
        {
            bool on = i == level;
            _levelLabels[i].ForeColor = on ? UiTheme.InkPrimary : UiTheme.InkMuted;
            _levelLabels[i].BackColor = on ? UiTheme.CpuDim : UiTheme.BgSurfaceHi;
        }
    }

    void ApplyBoost(bool on)
    {
        if (!_ec.Open()) return;
        var cur = _ec.Read(FanProtocol.ADDR_FAN_MODE);
        if (cur is null) return;
        byte v = FanProtocol.SetBoost(cur.Value, on);
        if (_ec.Write(FanProtocol.ADDR_FAN_MODE, v))
        {
            _settings.BoostOn = on; _settings.Save();
            SetState(on ? UiState.Idle : UiState.Idle, on ? "FanBoost 真·满速 ON (0x751 bit6)" : "FanBoost OFF");
        }
        Poll();
    }

    // ---------------- misc ----------------

    void ShowFromTray() { Show(); WindowState = FormWindowState.Normal; BringToFront(); Activate(); }

    static void SetAutoStart(bool enable)
    {
        using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Run", writable: true);
        if (key == null) return;
        var path = Environment.ProcessPath ?? "";
        if (enable && !string.IsNullOrEmpty(path)) key.SetValue("FanSlider", $"\"{path}\"");
        else key.DeleteValue("FanSlider", throwOnMissingValue: false);
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_QUERYENDSESSION = 0x0011; // 系统关机/注销前询问
        if (m.Msg == WM_QUERYENDSESSION)
        {
            try
            {
                // 关机前按快照交还, 避免 EC 停留在接管曲线 (崩溃恢复租约也会被清)
                if (_settings.RestoreAutoOnExit)
                {
                    _curve?.Dispose();
                    _curve = null;
                }
            }
            catch { }
        }
        base.WndProc(ref m);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!_exitingHard && _closeToTray && e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true; Hide();
            _tray.ShowBalloonTip(1500, "FanSlider", "已隐藏到托盘，控制权保持中。双击图标恢复窗口。", ToolTipIcon.Info);
            return;
        }
        if (_exitingHard && _settings.RestoreAutoOnExit)
        {
            try { _curve?.Dispose(); _curve = null; }
            catch { }
        }
        _tray.Visible = false; _tray.Dispose(); _pollTimer.Stop(); _ec.Dispose();
        base.OnFormClosing(e);
    }
}
