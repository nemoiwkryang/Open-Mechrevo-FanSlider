using System.Diagnostics;

namespace FanSlider;

/// <summary>
/// v8 完全独立接管 (2026-09-06 定案): 脱离 GCUBridge/GCUService 的直接 EC 曲线接管。
/// 相对 v7 的增量 (固件/源码双重实证):
///   - 快照扩为门控 0x751/0x726/0x7C5/0x7C6/0x741 + 全表区 0xF00-0xF5F (96B) + 0x787/0x782
///   - Arm 补 0x7C6|=0x04 (ENABLE_UNIVERSAL_FAN_CTRL, 固件 img 0x9770: bit2=0 走回退路径)
///    与 0x741|=0x01 (AP 存在位, 镜像 GCUService App.cs 行为)。
///    注: v7 不写 0x7C6 也成功, 是因为实验时 OEM 栈存活已预置该位; 脱离栈后必须自置。
///   - 写表为三区交错全写 (UpT/DownT/Duty, OEM SetEcFanTable 47 次/表), T0 强制 0, ≥10ms/字节
///   - 还原为按位读改写 (只还原我们改动的位, 不破坏并发修改的其它位)
///   - 新增 Reassert(): 控制权被抢 (0x7C5 bit7 被清) 后重新武装 + 重推当前曲线
///   - 新增 RestoreDefault(): 0xF5F 握手让固件重装内置默认表 (脱离服务也能还原 OEM 默认)
///   - 接管租约 (TakeoverLease) 持久化: 崩溃/被杀后下次启动可还原
///
/// 接管序列:
/// 1. 快照门控 + 全表区
/// 2. 0x751←0x00 → 0x726|=0x80 → 0x7C5|=0x80(回读验证重试3次) → 0x7C6|=0x04 → 0x741|=0x01
///    → 0x727|=0x40 (AP Customer Mode 灯, RAM 表模式隐藏解锁位, 2026-09-06 重启排查实证)
/// 3. 三区交错写 CPU/GPU 表 (Up[i]/Down[i+1]/Duty[i], Duty=Ti×2, T0=0, ≥10ms/字节)
/// 4. 回读验证 duty 列 → Active
/// 5. 完成后 EC 固件自主按表闭环, 无需周期回写
/// </summary>
public sealed class EcCurveSession : IDisposable
{
    const int WriteDelayMs = 10; // EC 逐句柄消费, ≥10ms/字节 (TAKEOVER_SPEC §5: 无间隔连写会脱离表跟随)

    readonly EcDriver _ec;

    // 门控快照 (接管前原值, 供 TakeoverLease 持久化)
    internal byte Saved751 { get; private set; }
    internal byte Saved726 { get; private set; }
    internal byte Saved7C5 { get; private set; }
    internal byte Saved7C6 { get; private set; }
    internal byte Saved741 { get; private set; }
    internal byte Saved727 { get; private set; }
    internal byte Saved787 { get; private set; }
    internal byte Saved782 { get; private set; }

    // 全表区快照 (0xF00-0xF5F 6 块, 还原用)
    internal byte[] CpuUp   { get; } = new byte[FanTableModel.Points];
    internal byte[] CpuDown { get; } = new byte[FanTableModel.Points];
    internal byte[] CpuDuty { get; } = new byte[FanTableModel.Points];
    internal byte[] GpuUp   { get; } = new byte[FanTableModel.Points];
    internal byte[] GpuDown { get; } = new byte[FanTableModel.Points];
    internal byte[] GpuDuty { get; } = new byte[FanTableModel.Points];

    public FanTableModel Cpu { get; private set; } = null!;
    public FanTableModel Gpu { get; private set; } = null!;

    public bool Active { get; private set; }
    public static string? LastFailReason { get; private set; }

    private EcCurveSession(EcDriver ec) => _ec = ec;

    /// <summary>备份原状态 → 武装门控 → 全表区写入接管。失败原因见 LastFailReason。</summary>
    public static EcCurveSession? Start(EcDriver ec, int[] cpuDutyPct, int[] gpuDutyPct)
    {
        LastFailReason = null;
        try
        {
            var s751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
            var s726 = ec.Read(FanProtocol.ADDR_CUSTOM_FLAG);
            var s7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            var s7c6 = ec.Read(FanProtocol.ADDR_FAN_CONTROL_TRIGGER);
            var s741 = ec.Read(FanProtocol.ADDR_AP_EXIST);
            var s727 = ec.Read(FanProtocol.ADDR_CUSTOMER_MODE_LIGHT);
            var s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
            var s782 = ec.Read(FanProtocol.ADDR_QKEY_MODE);
            if (s751 is null || s726 is null || s7c5 is null || s7c6 is null || s741 is null || s727 is null || s787 is null || s782 is null)
            { LastFailReason = "read"; return null; }

            var sess = new EcCurveSession(ec)
            {
                Saved751 = s751.Value, Saved726 = s726.Value, Saved7C5 = s7c5.Value,
                Saved7C6 = s7c6.Value, Saved741 = s741.Value, Saved727 = s727.Value,
                Saved787 = s787.Value, Saved782 = s782.Value,
            };
            if (!sess.SnapshotTables()) return null;
            sess.BuildModels(cpuDutyPct, gpuDutyPct);
            if (!sess.ArmCustom()) { LastFailReason = "gate"; return null; }
            if (!sess.WriteAll()) { LastFailReason = "write"; return null; }
            if (!sess.VerifyDuty()) { LastFailReason = "verify"; return null; }
            sess.Active = true;
            TakeoverLease.FromSession(sess).Save();
            return sess;
        }
        catch (Exception ex) { LastFailReason = ex.Message; return null; }
    }

    /// <summary>平直曲线便捷入口 (CLI/回归用): 全 16 点 = pct, T0=0。</summary>
    public static EcCurveSession? StartFlat(EcDriver ec, int cpuPct, int gpuPct) =>
        Start(ec, FlatDuty(cpuPct), FlatDuty(gpuPct));

    static int[] FlatDuty(int pct)
    {
        pct = Math.Clamp(pct, 0, 100);
        var a = new int[FanTableModel.Points];
        Array.Fill(a, pct);
        return a;
    }

    bool SnapshotTables() =>
        ReadBlock(FanProtocol.ADDR_CPU_TBL_UP0, CpuUp)   && ReadBlock(FanProtocol.ADDR_CPU_TBL_DOWN0, CpuDown)
     && ReadBlock(FanProtocol.ADDR_CPU_TBL_DUTY0, CpuDuty) && ReadBlock(FanProtocol.ADDR_GPU_TBL_UP0, GpuUp)
     && ReadBlock(FanProtocol.ADDR_GPU_TBL_DOWN0, GpuDown) && ReadBlock(FanProtocol.ADDR_GPU_TBL_DUTY0, GpuDuty);

    bool ReadBlock(ushort start, byte[] into)
    {
        for (int i = 0; i < into.Length; i++)
        {
            var v = _ec.Read((ushort)(start + i));
            if (v is null) { LastFailReason = "tables"; return false; }
            into[i] = v.Value;
        }
        return true;
    }

    /// <summary>从快照重建模型: 温度轴取快照 (无效则默认阶梯), duty 取调用方曲线, T0 强制 0。</summary>
    void BuildModels(int[] cpuDutyPct, int[] gpuDutyPct)
    {
        Cpu = FanTableModel.FromBlocks(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0,
            CpuUp, CpuDown, CpuDuty);
        Gpu = FanTableModel.FromBlocks(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0,
            GpuUp, GpuDown, GpuDuty);
        if (!Cpu.HasUsableAxis) Cpu.SetDefaultAxis();
        if (!Gpu.HasUsableAxis) Gpu.SetDefaultAxis();
        Cpu.ActivePoints = FanTableModel.Points;
        Gpu.ActivePoints = FanTableModel.Points;
        SetDuty(Cpu, cpuDutyPct);
        SetDuty(Gpu, gpuDutyPct);
    }

    static void SetDuty(FanTableModel m, int[] dutyPct)
    {
        for (int i = 0; i < FanTableModel.Points; i++)
        {
            int v = i < dutyPct.Length ? dutyPct[i] : 0;
            m.DutyPct[i] = (byte)Math.Clamp(v, 0, 100);
        }
        m.DutyPct[0] = 0; // T0=0 关键: 最低温度点占空比必须为 0 (固件 img 0x977d/0x9783 启用标记)
    }

    /// <summary>SPEC 前置: 0x751=0x00, 0x726|=0x80, 0x7C5|=0x80(回读验证重试3次), 0x7C6|=0x04, 0x741|=0x01。</summary>
    bool ArmCustom()
    {
        if (!_ec.Write(FanProtocol.ADDR_FAN_MODE, 0x00)) return false;
        var cf = _ec.Read(FanProtocol.ADDR_CUSTOM_FLAG);
        if (cf is null || !_ec.Write(FanProtocol.ADDR_CUSTOM_FLAG, (byte)(cf.Value | FanProtocol.CUSTOM_FLAG_BIT))) return false;

        for (int i = 0; i < 3; i++)
        {
            var r = _ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            if (r is null) return false;
            _ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, FanProtocol.SetRespective(r.Value, true));
            Thread.Sleep(300);
            var back = _ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            if (back is not null && (back.Value & FanProtocol.RESPECTIVE_BIT) != 0) break;
            if (i == 2) return false;
        }

        // ENABLE_UNIVERSAL_FAN_CTRL 门: 脱离 OEM 栈后必须自置 (固件 img 0x9770 JZ → 回退)
        var g = _ec.Read(FanProtocol.ADDR_FAN_CONTROL_TRIGGER);
        if (g is null || !_ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.SetUniversalFanCtrl(g.Value, true))) return false;

        // AP 存在位 (镜像 OEM: GCUService 启动置位, 让固件认为主机控制中心在线)
        var ap = _ec.Read(FanProtocol.ADDR_AP_EXIST);
        if (ap is null || !_ec.Write(FanProtocol.ADDR_AP_EXIST, (byte)(ap.Value | FanProtocol.AP_EXIST_BIT))) return false;

        // AP Customer Mode 灯位 —— RAM 表模式隐藏解锁 (2026-09-06 真机实证):
        // 冷启动后不置 0x727 bit6, 即使其余门控全武装表路径也不生效 (固件走内部曲线);
        // 官方链 OPERATING_CUSTOM_MODE 的 CustomerModeLightOn 正是置此位。
        var cm = _ec.Read(FanProtocol.ADDR_CUSTOMER_MODE_LIGHT);
        return cm is not null && _ec.Write(FanProtocol.ADDR_CUSTOMER_MODE_LIGHT, (byte)(cm.Value | FanProtocol.CUSTOMER_MODE_LIGHT_BIT));
    }

    /// <summary>写两张表 (三区交错序, OEM 47 次/表, ≥10ms/字节)。</summary>
    bool WriteAll() =>
        Cpu.WriteToEc(_ec, WriteDelayMs) >= FanTableModel.WritesPerTable
     && Gpu.WriteToEc(_ec, WriteDelayMs) >= FanTableModel.WritesPerTable;

    /// <summary>回读验证: 两张表 duty 列逐字节等于模型期望值。</summary>
    bool VerifyDuty()
    {
        for (int i = 0; i < FanTableModel.Points; i++)
        {
            if (_ec.Read((ushort)(FanProtocol.ADDR_CPU_TBL_DUTY0 + i)) != FanTableModel.Scale(Cpu.DutyPct[i])) return false;
            if (_ec.Read((ushort)(FanProtocol.ADDR_GPU_TBL_DUTY0 + i)) != FanTableModel.Scale(Gpu.DutyPct[i])) return false;
        }
        return true;
    }

    /// <summary>更新曲线。null = 该风扇交还快照 duty 列 (跟随 OEM 曲线), 非 null = 写入编辑器曲线。</summary>
    public bool Apply(int[]? cpuDutyPct, int[]? gpuDutyPct)
    {
        if (!Active) return false;
        bool ok = true;
        if (cpuDutyPct is not null) { SetDuty(Cpu, cpuDutyPct); ok &= Cpu.WriteToEc(_ec, WriteDelayMs) >= FanTableModel.WritesPerTable; }
        else ok &= RestoreDutyColumn(false);
        if (gpuDutyPct is not null) { SetDuty(Gpu, gpuDutyPct); ok &= Gpu.WriteToEc(_ec, WriteDelayMs) >= FanTableModel.WritesPerTable; }
        else ok &= RestoreDutyColumn(true);
        return ok;
    }

    /// <summary>单风扇交还快照 duty 列 (T0 强制 0 保持表驱动模式)。温度轴保持当前。</summary>
    bool RestoreDutyColumn(bool gpu)
    {
        var snap = gpu ? GpuDuty : CpuDuty;
        ushort baseAddr = gpu ? FanProtocol.ADDR_GPU_TBL_DUTY0 : FanProtocol.ADDR_CPU_TBL_DUTY0;
        for (int i = 0; i < FanTableModel.Points; i++)
        {
            byte v = i == 0 ? (byte)0 : snap[i];
            if (!_ec.Write((ushort)(baseAddr + i), v)) return false;
            Thread.Sleep(WriteDelayMs);
        }
        return true;
    }

    /// <summary>控制权被抢 (0x7C5 bit7 被清) 后重新武装并重推当前曲线。</summary>
    public bool Reassert()
    {
        if (!Active) return false;
        return ArmCustom() && WriteAll();
    }

    /// <summary>等待实时 duty 收敛到目标。null = 该风扇不参与判定 (非均匀曲线)。
    /// CPU 精确, GPU 允许 25% 地板 (raw 0x32)。</summary>
    public bool WaitForEc(int? cpuPct, int? gpuPct, int timeoutMs = 45000)
    {
        byte? wantC = cpuPct is int c0 ? FanTableModel.Scale(c0) : null;
        byte? wantG = gpuPct is int g0 ? (byte)Math.Max(FanTableModel.Scale(g0), FanProtocol.GPU_DUTY_FLOOR) : null;
        if (wantC is null && wantG is null) return true;
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            bool cOk = wantC is null, gOk = wantG is null;
            if (wantC is not null)
            {
                var cv = _ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
                cOk = cv is not null && Near(cv.Value, wantC.Value);
            }
            if (wantG is not null)
            {
                var gv = _ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
                gOk = gv is not null && Near(gv.Value, wantG.Value);
            }
            if (cOk && gOk) return true;
            Thread.Sleep(1500);
        }
        return false;
    }

    static bool Near(byte actual, byte want) => Math.Abs(actual - want) <= 3;

    /// <summary>交还: 还原全表区 + 按位还原门控 (只还原我们改的位)。</summary>
    public void Dispose()
    {
        if (!Active) return;
        Active = false;
        try
        {
            EcRestore.WriteTable(_ec, FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0, CpuUp, CpuDown, CpuDuty);
            EcRestore.WriteTable(_ec, FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0, GpuUp, GpuDown, GpuDuty);
            _ec.Write(FanProtocol.ADDR_FAN_MODE, Saved751); // 整字节还原 (我们整字节写过 0x00)
            EcRestore.RestoreBit(_ec, FanProtocol.ADDR_CUSTOM_FLAG, FanProtocol.CUSTOM_FLAG_BIT, Saved726);
            EcRestore.RestoreBit(_ec, FanProtocol.ADDR_FAN_RESPECTIVE, FanProtocol.RESPECTIVE_BIT, Saved7C5);
            EcRestore.RestoreBit(_ec, FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.ENABLE_UNIVERSAL_FAN_CTRL_BIT, Saved7C6);
            EcRestore.RestoreBit(_ec, FanProtocol.ADDR_AP_EXIST, FanProtocol.AP_EXIST_BIT, Saved741);
            EcRestore.RestoreBit(_ec, FanProtocol.ADDR_CUSTOMER_MODE_LIGHT, FanProtocol.CUSTOMER_MODE_LIGHT_BIT, Saved727);
            _ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, Saved787);
            _ec.Write(FanProtocol.ADDR_QKEY_MODE, Saved782);
        }
        catch { }
        finally { TakeoverLease.Clear(); }
    }

    /// <summary>
    /// 0xF5F 握手: 让固件把内置默认曲线重载入表区 (复刻 GCUService FanTable_Init:
    /// 0xF5F=mode → 0xF5D=0xFD → 0xF5E=0xC9, 轮询 0xF5D/0xF5E 被固件改写视为就绪)。
    /// mode: 1=Turbo, 2=Gaming, 3=Office (RamFan1p5_ECSpec)。用于脱离 OEM 栈后的"还原 OEM 默认"。
    /// 注意: 会覆盖当前表区 (含接管中的曲线), 调用前应确保无活动会话。
    /// </summary>
    public static bool RestoreDefault(EcDriver ec, int mode = 2)
    {
        if (!ec.Write(FanProtocol.ADDR_TBL_CTRL, (byte)mode)) return false;
        Thread.Sleep(50);
        if (!ec.Write(FanProtocol.ADDR_TBL_STATUS1, 0xFD)) return false;
        Thread.Sleep(50);
        if (!ec.Write(FanProtocol.ADDR_TBL_STATUS2, 0xC9)) return false;
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < 5000)
        {
            var s1 = ec.Read(FanProtocol.ADDR_TBL_STATUS1);
            var s2 = ec.Read(FanProtocol.ADDR_TBL_STATUS2);
            if (s1 is not null && s2 is not null && (s1.Value != 0xFD || s2.Value != 0xC9)) return true;
            Thread.Sleep(500);
        }
        return false;
    }
}

/// <summary>EC 快照还原的共享静态工具 (EcCurveSession.Dispose 与 TakeoverLease.RestoreToEc 共用)。</summary>
internal static class EcRestore
{
    /// <summary>按 OEM 交错序还原一张表 (Up[i]/Down[i+1]/Duty[i], ≥10ms/字节)。</summary>
    public static void WriteTable(EcDriver ec, ushort upBase, ushort downBase, ushort dutyBase,
        byte[] up, byte[] down, byte[] duty)
    {
        for (int i = 0; i < FanTableModel.Points; i++)
        {
            ec.Write((ushort)(upBase + i), up[i]);
            if (i < FanTableModel.Points - 1) ec.Write((ushort)(downBase + i + 1), down[i + 1]);
            ec.Write((ushort)(dutyBase + i), duty[i]);
            Thread.Sleep(10);
        }
    }

    /// <summary>按位还原: 只恢复 mask 位为 saved 中的值, 其余位保留当前 (读改写)。</summary>
    public static void RestoreBit(EcDriver ec, ushort addr, byte mask, byte saved)
    {
        var cur = ec.Read(addr);
        if (cur is null) return;
        ec.Write(addr, (byte)((cur.Value & ~mask) | (saved & mask)));
    }
}

/// <summary>
/// 接管租约: 预接管快照 + 生效曲线持久化到 %APPDATA%\FanSlider\lease.json。
/// 进程崩溃/被杀后, 下次启动可据此还原 EC (RestoreToEc) 或清除残留。
/// </summary>
public class TakeoverLease
{
    public bool Active { get; set; }
    public DateTime TakenAt { get; set; }
    public byte Saved751 { get; set; }
    public byte Saved726 { get; set; }
    public byte Saved7C5 { get; set; }
    public byte Saved7C6 { get; set; }
    public byte Saved741 { get; set; }
    public byte Saved727 { get; set; }
    public byte Saved787 { get; set; }
    public byte Saved782 { get; set; }
    public byte[] CpuUp { get; set; } = new byte[FanTableModel.Points];
    public byte[] CpuDown { get; set; } = new byte[FanTableModel.Points];
    public byte[] CpuDuty { get; set; } = new byte[FanTableModel.Points];
    public byte[] GpuUp { get; set; } = new byte[FanTableModel.Points];
    public byte[] GpuDown { get; set; } = new byte[FanTableModel.Points];
    public byte[] GpuDuty { get; set; } = new byte[FanTableModel.Points];
    public int CpuPct { get; set; }
    public int GpuPct { get; set; }

    static string Dir =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FanSlider");
    static string FilePath => Path.Combine(Dir, "lease.json");

    public static TakeoverLease FromSession(EcCurveSession s) => new()
    {
        Active = true,
        TakenAt = DateTime.Now,
        Saved751 = s.Saved751,
        Saved726 = s.Saved726,
        Saved7C5 = s.Saved7C5,
        Saved7C6 = s.Saved7C6,
        Saved741 = s.Saved741,
        Saved727 = s.Saved727,
        Saved787 = s.Saved787,
        Saved782 = s.Saved782,
        CpuUp = s.CpuUp, CpuDown = s.CpuDown, CpuDuty = s.CpuDuty,
        GpuUp = s.GpuUp, GpuDown = s.GpuDown, GpuDuty = s.GpuDuty,
        CpuPct = s.Cpu.DutyPct.Skip(1).Max(),
        GpuPct = s.Gpu.DutyPct.Skip(1).Max(),
    };

    public static TakeoverLease? Load()
    {
        try
        {
            if (!System.IO.File.Exists(FilePath)) return null;
            var l = System.Text.Json.JsonSerializer.Deserialize<TakeoverLease>(System.IO.File.ReadAllText(FilePath));
            return l is { Active: true } ? l : null;
        }
        catch { return null; }
    }

    public void Save()
    {
        try
        {
            System.IO.Directory.CreateDirectory(Dir);
            System.IO.File.WriteAllText(FilePath, System.Text.Json.JsonSerializer.Serialize(this,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
        }
        catch { }
    }

    public static void Clear()
    {
        try { if (System.IO.File.Exists(FilePath)) System.IO.File.Delete(FilePath); }
        catch { }
    }

    /// <summary>把租约快照写回 EC (崩溃恢复): 全表区 + 按位门控还原。</summary>
    public bool RestoreToEc(EcDriver ec)
    {
        try
        {
            EcRestore.WriteTable(ec, FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0, CpuUp, CpuDown, CpuDuty);
            EcRestore.WriteTable(ec, FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0, GpuUp, GpuDown, GpuDuty);
            ec.Write(FanProtocol.ADDR_FAN_MODE, Saved751);
            EcRestore.RestoreBit(ec, FanProtocol.ADDR_CUSTOM_FLAG, FanProtocol.CUSTOM_FLAG_BIT, Saved726);
            EcRestore.RestoreBit(ec, FanProtocol.ADDR_FAN_RESPECTIVE, FanProtocol.RESPECTIVE_BIT, Saved7C5);
            EcRestore.RestoreBit(ec, FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.ENABLE_UNIVERSAL_FAN_CTRL_BIT, Saved7C6);
            EcRestore.RestoreBit(ec, FanProtocol.ADDR_AP_EXIST, FanProtocol.AP_EXIST_BIT, Saved741);
            EcRestore.RestoreBit(ec, FanProtocol.ADDR_CUSTOMER_MODE_LIGHT, FanProtocol.CUSTOMER_MODE_LIGHT_BIT, Saved727);
            ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, Saved787);
            ec.Write(FanProtocol.ADDR_QKEY_MODE, Saved782);
            return true;
        }
        catch { return false; }
    }
}
