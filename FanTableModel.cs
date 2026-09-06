namespace FanSlider;

/// <summary>
/// 单颗风扇的 16 点曲线 (EC RAM 表区)。协议为 GCUService 的
/// FanTable_Manager1p5.SetEcFanTable 运行时 JIT / CDB 实证:
///   for i in 0..15: Write(UP0+i, UpT[i]); Write(DOWN0+i+1, DownT[i+1]); Write(DUTY0+i, Duty*2)
///   不写 Down[0]；i >= 有效点数: 写 0xFF (温度) / 0xC8 (DUTY=100%)。
///   提交脉冲在 PerFanSession：0x7C6=0x04→0x00，再刷新 0x7C5 bit7。
/// </summary>
public class FanTableModel
{
    public const int Points = 16;
    public const int WritesPerTable = Points * 3 - 1; // OEM 不写 Down[0]
    public const byte PadTemp = 0xFF;
    public const byte PadDuty = 0xC8; // 100% —— 越界即满速 (OEM 安全设计)

    public ushort UpBase, DownBase, DutyBase;
    public byte[] UpT   = new byte[Points];
    public byte[] DownT = new byte[Points];
    public byte[] DutyPct = new byte[Points]; // 0..100 (百分比, 写 EC 前 ×2)
    public int ActivePoints = 11;              // 实际使用的表项数, 其余填充

    public FanTableModel(ushort up, ushort down, ushort duty) { UpBase = up; DownBase = down; DutyBase = duty; }

    /// <summary>
    /// 生成平直 Duty 曲线。保留 OEM 温度轴；16 点 Duty 全部写成同一百分比。
    /// 尾部 0xC8 安全填充只用于 OEM 原表未覆盖的温度点；定速模式必须覆盖全部 16 点，
    /// 否则 EC 会落到尾部 100% 填充（hwtest2: GPU 20% 时后 6 点仍是 0xC8）。
    /// </summary>
    public void SetFlatDuty(int pct, byte startTemp = 45, byte step = 4)
    {
        pct = Math.Clamp(pct, 0, 100);
        if (!HasTemperatureAxis())
        {
            for (int i = 0; i < Points; i++)
            {
                UpT[i] = (byte)Math.Min(255, startTemp + i * step);
                DownT[i] = (byte)Math.Max(0, UpT[i] - 2);
            }
        }
        ActivePoints = Points;
        for (int i = 0; i < Points; i++) DutyPct[i] = (byte)pct;
    }

    private bool HasTemperatureAxis() =>
        UpT.Skip(1).Any(v => v != 0 && v != PadTemp) ||
        DownT.Skip(1).Any(v => v != 0 && v != PadTemp);

    /// <summary>温度轴是否有可用点 (v8 接管判断: 快照轴全 0/全 FF 时回退默认阶梯)。</summary>
    public bool HasUsableAxis => UpT.Skip(1).Any(v => v != 0 && v != PadTemp);

    /// <summary>写默认温度轴 (45°C 起每 4°C 一点, Down=Up-2 滞回, 末点 0xFF 哨兵, Down[0] 不写)。</summary>
    public void SetDefaultAxis(byte startTemp = 45, byte step = 4)
    {
        for (int i = 0; i < Points; i++)
        {
            UpT[i] = (byte)Math.Min(255, startTemp + i * step);
            DownT[i] = (byte)Math.Max(0, UpT[i] - 2);
        }
        UpT[Points - 1] = PadTemp; // OEM 哨兵 (SetEcFanTable i==15 → 0xFF)
        DownT[0] = 0;              // Down[0] 永不被写 (OEM 47 次/表)
    }

    /// <summary>从已读回的 OEM 模型复制温度轴和 Duty。</summary>
    public void CopyFrom(FanTableModel source)
    {
        Array.Copy(source.UpT, UpT, Points);
        Array.Copy(source.DownT, DownT, Points);
        Array.Copy(source.DutyPct, DutyPct, Points);
        ActivePoints = source.ActivePoints;
    }

    /// <summary>从快照三块字节重建模型（Duty 原始 0–200 → 百分比）。</summary>
    public static FanTableModel FromBlocks(ushort up, ushort down, ushort duty, byte[] upB, byte[] downB, byte[] dutyB)
    {
        var m = new FanTableModel(up, down, duty);
        int active = 0;
        for (int i = 0; i < Points; i++)
        {
            m.UpT[i] = upB[i];
            m.DownT[i] = downB[i];
            m.DutyPct[i] = (byte)Math.Clamp((int)Math.Round(dutyB[i] / (double)FanProtocol.DUTY_SCALE), 0, 100);
            if (upB[i] != PadTemp) active = i + 1;
        }
        m.ActivePoints = active > 0 ? active : Points;
        return m;
    }

    /// <summary>从 EC 读回表 (含填充项, 原样返回 16×3 数组)。</summary>
    public static FanTableModel? FromEc(EcDriver ec, ushort up, ushort down, ushort duty)
    {
        var m = new FanTableModel(up, down, duty);
        for (int i = 0; i < Points; i++)
        {
            var u = ec.Read((ushort)(up + i));
            var d = ec.Read((ushort)(down + i));
            var t = ec.Read((ushort)(duty + i));
            if (u is null || d is null || t is null) return null;
            m.UpT[i] = u.Value; m.DownT[i] = d.Value;
            m.DutyPct[i] = (byte)Math.Round(t.Value / (double)FanProtocol.DUTY_SCALE);
        }
        return m;
    }

    /// <summary>写入一张风扇表。严格复刻 OEM SetEcFanTable 的 47 次序列:
    /// 每个点依次写 Up[i]、Down[i+1]、Duty[i]；不写 Down[0]。</summary>
    public int WriteToEc(EcDriver ec) => WriteToEc(ec, 0);

    /// <summary>写入一张风扇表 (delayMs: 每字节间隔; v8 直写路径用 ≥10ms, 见 EcCurveSession)。</summary>
    public int WriteToEc(EcDriver ec, int delayMs)
    {
        int n = 0;
        for (int i = 0; i < Points; i++)
        {
            byte up = UpT[i];
            if (!ec.Write((ushort)(UpBase + i), up)) return n;
            n++;

            if (i < Points - 1)
            {
                byte down = DownT[i + 1];
                if (!ec.Write((ushort)(DownBase + i + 1), down)) return n;
                n++;
            }

            byte duty = Scale(DutyPct[i]);
            if (!ec.Write((ushort)(DutyBase + i), duty)) return n;
            n++;
            if (delayMs > 0) Thread.Sleep(delayMs);
        }
        return n;
    }

    public static byte Scale(int pct) => (byte)Math.Clamp(pct * FanProtocol.DUTY_SCALE, 0, FanProtocol.DUTY_MAX);

    /// <summary>读回校验: 表区内容是否等于本模型 (忽略读失败的项)。</summary>
    public bool VerifyOnEc(EcDriver ec)
    {
        for (int i = 0; i < Points; i++)
        {
            byte up   = i < ActivePoints ? UpT[i]   : PadTemp;
            byte down = i < ActivePoints ? DownT[i] : PadTemp;
            byte duty = i < ActivePoints ? Scale(DutyPct[i]) : PadDuty;
            if (ec.Read((ushort)(UpBase + i))   != up)   return false;
            if (ec.Read((ushort)(DownBase + i)) != down) return false;
            if (ec.Read((ushort)(DutyBase + i)) != duty) return false;
        }
        return true;
    }
}

/// <summary>
/// 分风扇控制会话。基于 CDB 捕获的 OEM 实际行为:
/// 服务暂停 → 0x7C6=04→00 → 0x7C5 独立位 → 两张表写入。
/// 不写 EF3/EF8、不强制改 0x751；退出时全量还原。
/// </summary>
public class PerFanSession : IDisposable
{
    private readonly EcDriver _ec;
    private readonly FanTableSnapshot _snap;
    private byte _saved751, _saved787, _saved7c5;

    public FanTableModel Cpu { get; }
    public FanTableModel Gpu { get; }
    public bool Active { get; private set; }
    public bool StoppedGcu { get; private set; }
    public int LastCpuWrites { get; private set; }
    public int LastGpuWrites { get; private set; }
    public static string? LastFailReason { get; private set; }

    private PerFanSession(EcDriver ec, FanTableSnapshot snap, FanTableModel cpu, FanTableModel gpu)
    {
        _ec = ec; _snap = snap;
        Cpu = cpu;
        Gpu = gpu;
    }

    /// <summary>拍快照、停止竞争服务，再进入 OEM 的表写入前状态。失败原因见 LastFailReason。</summary>
    public static PerFanSession? Start(EcDriver ec)
    {
        LastFailReason = null;
        var s751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
        var s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        var s7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        if (s751 is null || s787 is null || s7c5 is null)
        {
            LastFailReason = "read";
            return null;
        }

        var snap = FanTableSnapshot.Take(ec);
        if (snap.CpuUp is null || snap.CpuDown is null || snap.CpuDuty is null ||
            snap.GpuUp is null || snap.GpuDown is null || snap.GpuDuty is null)
        {
            LastFailReason = "snapshot";
            return null;
        }

        bool wasRunning = OemService.IsGcuBridgeRunning();
        if (wasRunning && !OemService.StopGcuBridge())
        {
            LastFailReason = "service";
            return null;
        }
        if (wasRunning) Thread.Sleep(400);

        var cpu = FanTableModel.FromBlocks(
            FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0,
            snap.CpuUp, snap.CpuDown, snap.CpuDuty);
        var gpu = FanTableModel.FromBlocks(
            FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0,
            snap.GpuUp, snap.GpuDown, snap.GpuDuty);

        var sess = new PerFanSession(ec, snap, cpu, gpu)
        {
            _saved751 = s751.Value,
            _saved787 = s787.Value,
            _saved7c5 = s7c5.Value,
            StoppedGcu = wasRunning,
        };
        // OEM SetFanTable 前缀: 0x787=0, 0x7C6=04→00, 0x7C5|=0x80。不改 0x751，不碰 EF3/EF8。
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        sess.PrepareTableWrite();
        sess.Active = true;
        return sess;
    }

    private void PrepareTableWrite()
    {
        // OEM CDB (cdb_method_1h_trace_9104): lambda+0x13 → 0x7C6=0x00 (开写闸),
        // 之后才写 47 项表, 表完成 lambda+0x36 → 0x7C6=0x04 (提交, 停留 04)。
        // 之前"边 04 边写表"与 OEM 门控相反, 是表写进 RAM 却不被采用的最后差异。
        _ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        _ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, FanProtocol.SetRespective(_saved7c5, true));
    }

    /// <summary>更新 CPU 风扇平直占空比 (0-100%)。保留 OEM 温度轴。</summary>
    public bool SetCpuDuty(int pct)
    {
        OpenGate();
        Cpu.SetFlatDuty(pct);
        LastCpuWrites = Cpu.WriteToEc(_ec);
        CommitGate();
        return LastCpuWrites >= FanTableModel.WritesPerTable;
    }

    /// <summary>更新 GPU 风扇平直占空比 (0-100%)。保留 OEM 温度轴。</summary>
    public bool SetGpuDuty(int pct)
    {
        OpenGate();
        Gpu.SetFlatDuty(pct);
        LastGpuWrites = Gpu.WriteToEc(_ec);
        CommitGate();
        return LastGpuWrites >= FanTableModel.WritesPerTable;
    }

    /// <summary>单路交还：把该风扇的 OEM 原表写回，独立位保持开。</summary>
    public bool RestoreCpu()
    {
        OpenGate();
        bool ok = _snap.RestoreCpu(_ec);
        CommitGate();
        return ok;
    }

    public bool RestoreGpu()
    {
        OpenGate();
        bool ok = _snap.RestoreGpu(_ec);
        CommitGate();
        return ok;
    }

    /// <summary>写表前开闸: 0x7C6=0x00 + 独立位 (OEM lambda+0x13 时序)。</summary>
    private void OpenGate()
    {
        _ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
    }

    /// <summary>写表后提交: 0x7C6=0x04 停留, 刷新独立位 (OEM lambda+0x36 时序)。</summary>
    private void CommitGate()
    {
        _ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        var c = _ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        if (c is not null) _ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, FanProtocol.SetRespective(c.Value, true));
    }

    public void Dispose()
    {
        if (!Active) return;
        Active = false;
        try
        {
            _snap.Restore(_ec);
            _ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
            _ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
            _ec.Write(FanProtocol.ADDR_FAN_MODE, _saved751);
            _ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, _saved787);
            _ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, _saved7c5);
        }
        catch { }
        finally
        {
            if (StoppedGcu) OemService.StartGcuBridge();
        }
    }
}

/// <summary>暂停/恢复机械革命控制中心服务, 避免它每秒把风扇表写回去。</summary>
internal static class OemService
{
    public static string? LastError { get; private set; }

    /// <summary>读取当前 OEM 服务状态并尝试停止。返回 true 仅代表本次确实停止了一个运行中的服务。</summary>
    public static bool StopGcuBridge()
    {
        LastError = null;
        if (!IsGcuBridgeRunning()) return false;
        if (!Sc("stop"))
        {
            LastError = "sc stop GCUBridge 失败";
            return false;
        }
        if (!WaitUntil(running: false))
        {
            LastError = "GCUBridge/GCUService 仍在运行";
            return false;
        }
        return true;
    }

    public static bool StartGcuBridge() => Sc("start") && WaitUntil(running: true);

    /// <summary>重启 GCUBridge (profile 文件通道靠它把新表推入 EC)。服务未运行时直接启动。</summary>
    public static bool RestartGcuBridge()
    {
        LastError = null;
        if (IsGcuBridgeRunning())
        {
            if (!Sc("stop")) { LastError = "sc stop GCUBridge 失败"; return false; }
            if (!WaitUntil(running: false)) { LastError = "停止超时"; return false; }
        }
        if (!Sc("start")) { LastError = "sc start GCUBridge 失败"; return false; }
        if (!WaitUntil(running: true)) { LastError = "启动超时"; return false; }
        Thread.Sleep(2500); // 等服务完成 Receive 初始化并推表
        return true;
    }

    public static bool IsGcuBridgeRunning()
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo("sc.exe", "query GCUBridge")
            {
                CreateNoWindow = true, UseShellExecute = false,
                RedirectStandardOutput = true, RedirectStandardError = true,
            };
            using var p = System.Diagnostics.Process.Start(psi);
            if (p is null) return false;
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit(5000);
            return output.Contains("RUNNING", StringComparison.OrdinalIgnoreCase) ||
                   output.Contains("START_PENDING", StringComparison.OrdinalIgnoreCase);
        }
        catch { return false; }
    }

    private static bool Sc(string action)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo("sc.exe", $"{action} GCUBridge")
            {
                CreateNoWindow = true, UseShellExecute = false,
                RedirectStandardOutput = true, RedirectStandardError = true,
            };
            using var p = System.Diagnostics.Process.Start(psi);
            p?.WaitForExit(8000);
            return p is { ExitCode: 0 or 1062 or 1056 }; // 0=ok, 1062=already stopped, 1056=already running
        }
        catch { return false; }
    }

    private static bool WaitUntil(bool running)
    {
        for (int i = 0; i < 20; i++)
        {
            bool now = System.Diagnostics.Process.GetProcessesByName("GCUService").Length > 0
                       || System.Diagnostics.Process.GetProcessesByName("GCUBridge").Length > 0;
            if (now == running) return true;
            Thread.Sleep(150);
        }
        return !running; // 停服务时宁可继续, 启服务失败也不阻塞还原
    }
}

/// <summary>两颗风扇的快照/恢复管理 (接管前拍原表, 退出/取消接管时写回)。</summary>
public class FanTableSnapshot
{
    public byte[]? CpuUp, CpuDown, CpuDuty;
    public byte[]? GpuUp, GpuDown, GpuDuty;
    public byte? Respective7C5;

    public static FanTableSnapshot Take(EcDriver ec)
    {
        var s = new FanTableSnapshot();
        s.CpuUp   = Block(ec, FanProtocol.ADDR_CPU_TBL_UP0);
        s.CpuDown = Block(ec, FanProtocol.ADDR_CPU_TBL_DOWN0);
        s.CpuDuty = Block(ec, FanProtocol.ADDR_CPU_TBL_DUTY0);
        s.GpuUp   = Block(ec, FanProtocol.ADDR_GPU_TBL_UP0);
        s.GpuDown = Block(ec, FanProtocol.ADDR_GPU_TBL_DOWN0);
        s.GpuDuty = Block(ec, FanProtocol.ADDR_GPU_TBL_DUTY0);
        s.Respective7C5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        return s;
    }

    private static byte[]? Block(EcDriver ec, ushort start)
    {
        var a = new byte[FanTableModel.Points];
        for (int i = 0; i < a.Length; i++)
        {
            var v = ec.Read((ushort)(start + i));
            if (v is null) return null;
            a[i] = v.Value;
        }
        return a;
    }

    public bool Restore(EcDriver ec) => RestoreCpu(ec) && RestoreGpu(ec);
    public bool RestoreCpu(EcDriver ec) => WriteTable(ec,
        FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0,
        CpuUp, CpuDown, CpuDuty);
    public bool RestoreGpu(EcDriver ec) => WriteTable(ec,
        FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0,
        GpuUp, GpuDown, GpuDuty);

    private static bool WriteTable(EcDriver ec, ushort upBase, ushort downBase, ushort dutyBase,
        byte[]? up, byte[]? down, byte[]? duty)
    {
        if (up is null || down is null || duty is null) return true; // 没拍到就不恢复
        for (int i = 0; i < FanTableModel.Points; i++)
        {
            if (!ec.Write((ushort)(upBase + i), up[i])) return false;
            if (i < FanTableModel.Points - 1 && !ec.Write((ushort)(downBase + i + 1), down[i + 1])) return false;
            if (!ec.Write((ushort)(dutyBase + i), duty[i])) return false;
        }
        return true;
    }
}
