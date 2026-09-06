using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FanSlider;

internal static class Program
{
    [DllImport("kernel32.dll")]
    private static extern bool AttachConsole(int dwProcessId);
    private const int ATTACH_PARENT_PROCESS = -1;

    [STAThread]
    private static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--selftest")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); SelfTest(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--writetest")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); WriteTest(args.Length > 1 ? int.Parse(args[1]) : 1, args.Length > 2 ? int.Parse(args[2]) : 1500); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--set")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); SetLevel(args.Length > 1 ? int.Parse(args[1]) : 5); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--boost")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); Boost(args.Length > 1 && args[1] != "0" && !args[1].Equals("off", StringComparison.OrdinalIgnoreCase)); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--probe")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); Probe(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--tables")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); DumpTables(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--read" && args.Length > 1)
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); ReadRegs(args.Skip(1).ToArray()); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--wr" && args.Length > 2)
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); WriteRegRaw(args[1], args[2]); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest(args.Length > 1 ? int.Parse(args[1]) : 30); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest2")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest2(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest3")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest3(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest4")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest4(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest5")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest5(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--watch")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); Watch(args.Length > 1 ? int.Parse(args[1]) : 120); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--smapc")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); SmapcProbe(args.Length > 1 ? int.Parse(args[1]) : 256); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest6")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest6(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest7")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest7(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest8")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest8(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest9")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest9(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest14")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest14(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest13")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest13(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest12")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest12(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest11")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest11(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--fantest10")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FanTest10(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--dutyrw")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); DutyRwTest(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--statemimic")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); StateMimicTest(
                args.Length > 1 ? int.Parse(args[1]) : 60,
                args.Length > 2 ? int.Parse(args[2]) : 20); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--profiletest")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); CurveTest(
                args.Length > 1 ? int.Parse(args[1]) : 60,
                args.Length > 2 ? int.Parse(args[2]) : 20); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--restoredefault")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); RestoreDefaultTest(args.Length > 1 ? int.Parse(args[1]) : 2); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--oemexact")
        {
            var flags = args.Skip(5).ToList();
            try { AttachConsole(ATTACH_PARENT_PROCESS); OemExactTest(
                args.Length > 1 ? int.Parse(args[1]) : 3,
                args.Length > 2 ? int.Parse(args[2]) : 60,
                args.Length > 3 ? int.Parse(args[3]) : 20,
                args.Length > 4 ? int.Parse(args[4]) : 60,
                flags.Contains("combo"), flags.Contains("fullprofile"), flags.Contains("bit7off")); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--cleanbit")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); CleanBitTest(
                args.Length > 1 ? int.Parse(args[1]) : 60,
                args.Length > 2 ? int.Parse(args[2]) : 20,
                args.Length > 3 && args[3] == "bit7"); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--tblsplit")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); TableSplitTest(
                args.Length > 1 ? int.Parse(args[1]) : 60,
                args.Length > 2 ? int.Parse(args[2]) : 60); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--tblmode")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); TableModeTest(
                args.Length > 1 ? int.Parse(args[1]) : 60,
                args.Length > 2 ? int.Parse(args[2]) : 20,
                args.Length > 3 ? Convert.ToInt32(args[3], 16) : 0x10,
                args.Length > 4 ? int.Parse(args[4]) : 60); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--heartbeat")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); HeartbeatTest(
                args.Length > 1 ? int.Parse(args[1]) : 60,
                args.Length > 2 ? int.Parse(args[2]) : 20); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--tblshadow")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); TableShadowTest(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--pwmtest2")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); PwmTestBit7(
                args.Length > 1 ? int.Parse(args[1]) : 200,
                args.Length > 2 ? int.Parse(args[2]) : 40); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--pwmtest")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); PwmTest(
                args.Length > 1 ? int.Parse(args[1]) : 200,
                args.Length > 2 ? int.Parse(args[2]) : 40); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--session")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); SessionTest(
                args.Length > 1 ? int.Parse(args[1]) : 40,
                args.Length > 2 ? int.Parse(args[2]) : 15,
                args.Length > 3 ? int.Parse(args[3]) : 20); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--hwtest")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); HardwareMatrix(); }
            catch (Exception ex)
            {
                try { File.AppendAllText(@"D:\tmp\cc_analysis\hwtest.log", "CRASH: " + ex + "\n"); } catch { }
            }
            return;
        }
        if (args.Length > 0 && args[0] == "--hwtest2")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); HardwareMatrix2(); }
            catch (Exception ex)
            {
                try { File.AppendAllText(@"D:\tmp\cc_analysis\hwtest2.log", "CRASH: " + ex + "\n"); } catch { }
            }
            return;
        }
        if (args.Length > 0 && args[0] == "--hwtest3")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); HardwareCoexist(); }
            catch (Exception ex)
            {
                try { File.AppendAllText(@"D:\tmp\cc_analysis\hwtest3.log", "CRASH: " + ex + "\n"); } catch { }
            }
            return;
        }
        if (args.Length > 0 && args[0] == "--hwtest4")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); HardwareBitOnly(); }
            catch (Exception ex)
            {
                try { File.AppendAllText(@"D:\tmp\cc_analysis\hwtest4.log", "CRASH: " + ex + "\n"); } catch { }
            }
            return;
        }
        if (args.Length > 0 && args[0] == "--fullmax")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); FullMaxTest(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--usability")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); UsabilitySweep(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        if (args.Length > 0 && args[0] == "--restore")
        {
            try { AttachConsole(ATTACH_PARENT_PROCESS); RestoreOem(); }
            catch (Exception ex) { try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "selftest.log"), "CRASH: " + ex + "\n"); } catch { } }
            return;
        }
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }

    /// <summary>
    /// 端到端验证 (与 GUI 同一条代码路径): PerFanSession.Start → CPU=x%/GPU=y% →
    /// 长窗口采样 → Dispose 全量还原。用法: FanSlider.exe --session [cpu] [gpu] [seconds]
    /// </summary>
    private static void SessionTest(int cpuPct, int gpuPct, int seconds)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }

        P($"SESSION TEST: CPU={cpuPct}% GPU={gpuPct}% {seconds}s → 还原");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        static string H(byte? v) => v is null ? "??" : $"0x{v.Value:X2}";
        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var m = ec.Read(FanProtocol.ADDR_FAN_MODE);
            var u = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
            var res = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            var rpmL = FanProtocol.ReadRpm(ec, false);
            var rpmR = FanProtocol.ReadRpm(ec, true);
            P($"{tag,-16} DUTY L={l} R={r}  0x751={H(m)} 0x787={H(u)} 0x7C5={H(res)}  RPM={rpmL?.ToString() ?? "??"}/{rpmR?.ToString() ?? "??"}");
        }
        Snap("baseline");

        var sess = PerFanSession.Start(ec);
        if (sess is null) { P($"SESSION START FAIL: {PerFanSession.LastFailReason} {OemService.LastError}"); return; }
        P($"session started: cpuOK={sess.SetCpuDuty(cpuPct)} gpuOK={sess.SetGpuDuty(gpuPct)}");
        Snap("applied");

        for (int t = 5; t <= seconds; t += 5) { Thread.Sleep(5000); Snap($"T+{t}s"); }

        sess.Dispose();
        P("session disposed (全量还原)");
        Thread.Sleep(4000);
        Snap("restored+4s");
        Thread.Sleep(10000);
        Snap("restored+14s");
        P("SESSION TEST DONE");
    }

    /// <summary>
    /// 直写占空比镜像 0x461/0x469: 停 GCUBridge, 写左/右目标占空比(原始 0-200),
    /// 每秒采样 0x75B/0x75C/RPM, 验证写入瞬间是否生效、多久被 EC 覆盖。
    /// 用法: FanSlider.exe --pwmtest [leftRaw] [rightRaw]
    /// </summary>
    private static void PwmTest(int leftRaw, int rightRaw)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }
        static string H(byte? v) => v is null ? "??" : $"0x{v.Value:X2}";
        P($"PWM TEST: 停服务 → 0x461={leftRaw} 0x469={rightRaw}, 1s ×12");

        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var m461 = ec.Read(0x461);
            var m469 = ec.Read(0x469);
            var res = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            var rpmL = FanProtocol.ReadRpm(ec, false);
            var rpmR = FanProtocol.ReadRpm(ec, true);
            P($"{tag,-14} DUTY L={l} R={r}  0x461={H(m461)} 0x469={H(m469)} 0x7C5={H(res)}  RPM={rpmL?.ToString() ?? "??"}/{rpmR?.ToString() ?? "??"}");
        }

        Snap("baseline");
        bool wasRunning = OemService.IsGcuBridgeRunning();
        if (wasRunning && !OemService.StopGcuBridge()) { P("STOP GCUBridge FAIL"); return; }
        P("GCUBridge stopped");
        Snap("stopped");

        bool w1 = ec.Write(0x461, (byte)Math.Clamp(leftRaw, 0, 200));
        bool w2 = ec.Write(0x469, (byte)Math.Clamp(rightRaw, 0, 200));
        P($"write 0x461={leftRaw} ok={w1}  0x469={rightRaw} ok={w2}");
        for (int i = 1; i <= 12; i++) { Thread.Sleep(1000); Snap($"T+{i}s"); }

        if (wasRunning) { bool ok = OemService.StartGcuBridge(); P($"GCUBridge restart ok={ok}"); }
        Thread.Sleep(5000);
        Snap("after-restart");
        P("PWM TEST DONE");
    }

    /// <summary>
    /// bit7 用户占空比模式下的镜像写入: 停服务 → 0x7C5|=0x80 → 写 0x461/0x469,
    /// 验证在"用户接管占空比"状态下写入是否还会被 EC 覆盖、占空比/RPM 是否跟随。
    /// 用法: FanSlider.exe --pwmtest2 [leftRaw] [rightRaw]
    /// </summary>
    private static void PwmTestBit7(int leftRaw, int rightRaw)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }
        static string H(byte? v) => v is null ? "??" : $"0x{v.Value:X2}";
        P($"PWM2 TEST: 停服务 → 0x7C5|=0x80 → 0x461={leftRaw} 0x469={rightRaw}, 1s ×15");

        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var m461 = ec.Read(0x461);
            var m469 = ec.Read(0x469);
            var res = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            var rpmL = FanProtocol.ReadRpm(ec, false);
            var rpmR = FanProtocol.ReadRpm(ec, true);
            P($"{tag,-14} DUTY L={l} R={r}  0x461={H(m461)} 0x469={H(m469)} 0x7C5={H(res)}  RPM={rpmL?.ToString() ?? "??"}/{rpmR?.ToString() ?? "??"}");
        }

        var saved7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        Snap("baseline");
        bool wasRunning = OemService.IsGcuBridgeRunning();
        if (wasRunning && !OemService.StopGcuBridge()) { P("STOP GCUBridge FAIL"); return; }
        P("GCUBridge stopped");

        if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(saved7c5.Value | 0x80));
        Snap("bit7-on");

        bool w1 = ec.Write(0x461, (byte)Math.Clamp(leftRaw, 0, 200));
        bool w2 = ec.Write(0x469, (byte)Math.Clamp(rightRaw, 0, 200));
        P($"write 0x461={leftRaw} ok={w1}  0x469={rightRaw} ok={w2}");
        for (int i = 1; i <= 15; i++) { Thread.Sleep(1000); Snap($"T+{i}s"); }

        if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, saved7c5.Value);
        P("0x7C5 restored");
        if (wasRunning) { bool ok = OemService.StartGcuBridge(); P($"GCUBridge restart ok={ok}"); }
        Thread.Sleep(5000);
        Snap("after-restart");
        P("PWM2 TEST DONE");
    }

    /// <summary>
    /// 哨兵表实验: 停服务 → 用平直哨兵 Duty (CPU=60%/GPU=20%) 覆盖两张表 → 立即/10s/30s
    /// 回读表区 + duty, 区分"表被 EC 覆盖"vs"表保留但不消费"。结束还原。
    /// 用法: FanSlider.exe --tblshadow
    /// </summary>
    private static void TableShadowTest()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }
        P($"TBL SHADOW: 停服务 → 哨兵表 CPU=60%/GPU=20%, 回读表区 0/10/30s");

        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        static byte[] Block(EcDriver e, ushort start)
        {
            var a = new byte[16];
            for (int i = 0; i < 16; i++) a[i] = e.Read((ushort)(start + i)) ?? 0xEE;
            return a;
        }
        void DumpTable(string tag)
        {
            var cd = Block(ec, FanProtocol.ADDR_CPU_TBL_DUTY0);
            var gd = Block(ec, FanProtocol.ADDR_GPU_TBL_DUTY0);
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var res = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            P($"{tag,-14} CPUDUTY[0..3]={cd[0]:X2} {cd[1]:X2} {cd[2]:X2} {cd[3]:X2}  " +
              $"GPUDUTY[0..3]={gd[0]:X2} {gd[1]:X2} {gd[2]:X2} {gd[3]:X2}  " +
              $"DUTY L={l} R={r} 0x7C5={(res is null ? "??" : $"0x{res.Value:X2}")}");
        }

        var snap = FanTableSnapshot.Take(ec);
        DumpTable("baseline");
        bool wasRunning = OemService.IsGcuBridgeRunning();
        if (wasRunning && !OemService.StopGcuBridge()) { P("STOP GCUBridge FAIL"); return; }
        P("GCUBridge stopped");

        var cpu = FanTableModel.FromBlocks(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0,
            FanProtocol.ADDR_CPU_TBL_DUTY0, snap.CpuUp!, snap.CpuDown!, snap.CpuDuty!);
        var gpu = FanTableModel.FromBlocks(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0,
            FanProtocol.ADDR_GPU_TBL_DUTY0, snap.GpuUp!, snap.GpuDown!, snap.GpuDuty!);
        cpu.SetFlatDuty(60);
        gpu.SetFlatDuty(20);
        int nc = cpu.WriteToEc(ec);
        int ng = gpu.WriteToEc(ec);
        P($"sentinel writes cpu={nc}/47 gpu={ng}/47");
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        if (snap.Respective7C5 is byte r7c5) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(r7c5 | 0x80));

        DumpTable("T+0s");
        Thread.Sleep(10000); DumpTable("T+10s");
        Thread.Sleep(20000); DumpTable("T+30s");

        snap.Restore(ec);
        if (snap.Respective7C5 is byte s7c5) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, s7c5);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        P("table+bit7 restored");
        if (wasRunning) P($"GCUBridge restart ok={OemService.StartGcuBridge()}");
        Thread.Sleep(3000);
        DumpTable("after-restart");
        P("TBL SHADOW DONE");
    }

    /// <summary>
    /// 心跳接管: 停服务 → 置独立位 → 写哨兵表(CPU=60/GPU=20) → 表后补 0x7C6=04→00,
    /// 之后每 2s 重发提交脉冲 + 每 10s 重写表, 共 N 秒, 观察 duty 是否开始跟随表。
    /// 用法: FanSlider.exe --heartbeat [seconds] [interval]
    /// </summary>
    private static void HeartbeatTest(int seconds, int interval)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }
        P($"HEARTBEAT: CPU=60/GPU=20 哨兵表 + 每{interval/1000}s提交脉冲, {seconds}s");

        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        static string H(byte? v) => v is null ? "??" : $"0x{v.Value:X2}";
        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var cd = ec.Read(FanProtocol.ADDR_CPU_TBL_DUTY0);
            var gd = ec.Read(FanProtocol.ADDR_GPU_TBL_DUTY0);
            var res = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            var u = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
            var m = ec.Read(FanProtocol.ADDR_FAN_MODE);
            P($"{tag,-14} DUTY L={l} R={r}  tbl0 C={H(cd)} G={H(gd)}  0x7C5={H(res)} 0x787={H(u)} 0x751={H(m)}");
        }

        var saved7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        var snap = FanTableSnapshot.Take(ec);
        if (snap.CpuUp is null || snap.GpuUp is null) { P("SNAPSHOT FAIL"); return; }
        Snap("baseline");
        bool wasRunning = OemService.IsGcuBridgeRunning();
        if (wasRunning && !OemService.StopGcuBridge()) { P("STOP GCUBridge FAIL"); return; }
        P("GCUBridge stopped");

        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(saved7c5.Value | 0x80));

        var cpu = FanTableModel.FromBlocks(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0,
            FanProtocol.ADDR_CPU_TBL_DUTY0, snap.CpuUp!, snap.CpuDown!, snap.CpuDuty!);
        var gpu = FanTableModel.FromBlocks(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0,
            FanProtocol.ADDR_GPU_TBL_DUTY0, snap.GpuUp!, snap.GpuDown!, snap.GpuDuty!);
        cpu.SetFlatDuty(60);
        gpu.SetFlatDuty(20);
        cpu.WriteToEc(ec);
        gpu.WriteToEc(ec);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        P("sentinel table + post-commit 04/00 applied");
        Snap("T+0s");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        int beats = 0;
        while (sw.Elapsed.TotalSeconds < seconds)
        {
            Thread.Sleep(interval);
            beats++;
            ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
            ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
            if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(saved7c5.Value | 0x80));
            if (beats % 5 == 0) { cpu.WriteToEc(ec); gpu.WriteToEc(ec); }
            Snap($"beat{beats}");
        }

        P("heartbeat loop done — restore");
        snap.Restore(ec);
        if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, saved7c5.Value);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        if (wasRunning) P($"GCUBridge restart ok={OemService.StartGcuBridge()}");
        Thread.Sleep(3000);
        Snap("after-restart");
        P("HEARTBEAT DONE");
    }

    /// <summary>
    /// 表模式实验: 停服务 → 0x751=mode → 0x787=0 → bit7 → 哨兵表(CPU=cpuPct/GPU=20)
    /// → 每 interval ms 提交脉冲 + 每 5 拍重写表。验证 EC 是否只在特定模式字节下消费表区。
    /// 用法: FanSlider.exe --tblmode [seconds] [interval] [mode751hex] [cpuPct]
    /// </summary>
    private static void TableModeTest(int seconds, int interval, int mode751, int cpuPct)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }
        P($"TBLMODE: 0x751=0x{mode751:X2} CPU={cpuPct}/GPU=20 心跳{interval}ms × {seconds}s");

        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        static string H(byte? v) => v is null ? "??" : $"0x{v.Value:X2}";
        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var cd = ec.Read(FanProtocol.ADDR_CPU_TBL_DUTY0);
            var gd = ec.Read(FanProtocol.ADDR_GPU_TBL_DUTY0);
            var res = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            var u = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
            var m = ec.Read(FanProtocol.ADDR_FAN_MODE);
            var rpmL = FanProtocol.ReadRpm(ec, false);
            var rpmR = FanProtocol.ReadRpm(ec, true);
            P($"{tag,-14} DUTY L={l} R={r}  tbl0 C={H(cd)} G={H(gd)}  0x7C5={H(res)} 0x787={H(u)} 0x751={H(m)} RPM={rpmL?.ToString() ?? "??"}/{rpmR?.ToString() ?? "??"}");
        }

        var saved7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        var saved751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
        var saved787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        var snap = FanTableSnapshot.Take(ec);
        if (snap.CpuUp is null || snap.GpuUp is null) { P("SNAPSHOT FAIL"); return; }
        Snap("baseline");
        bool wasRunning = OemService.IsGcuBridgeRunning();
        if (wasRunning && !OemService.StopGcuBridge()) { P("STOP GCUBridge FAIL"); return; }
        P("GCUBridge stopped");

        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        ec.Write(FanProtocol.ADDR_FAN_MODE, (byte)mode751);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(saved7c5.Value | 0x80));

        var cpu = FanTableModel.FromBlocks(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0,
            FanProtocol.ADDR_CPU_TBL_DUTY0, snap.CpuUp!, snap.CpuDown!, snap.CpuDuty!);
        var gpu = FanTableModel.FromBlocks(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0,
            FanProtocol.ADDR_GPU_TBL_DUTY0, snap.GpuUp!, snap.GpuDown!, snap.GpuDuty!);
        cpu.SetFlatDuty(cpuPct);
        gpu.SetFlatDuty(20);
        cpu.WriteToEc(ec);
        gpu.WriteToEc(ec);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        P($"applied: mode 0x{mode751:X2} + CPU={cpuPct}/GPU=20 sentinel tables + bit7");
        Snap("T+0s");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        int beats = 0;
        while (sw.Elapsed.TotalSeconds < seconds)
        {
            Thread.Sleep(interval);
            beats++;
            ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
            ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
            if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(saved7c5.Value | 0x80));
            if (beats % 5 == 0) { cpu.WriteToEc(ec); gpu.WriteToEc(ec); }
            Snap($"beat{beats}");
        }

        P("tblmode loop done — restore");
        snap.Restore(ec);
        if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, saved7c5.Value);
        if (saved751 is not null) ec.Write(FanProtocol.ADDR_FAN_MODE, saved751.Value);
        if (saved787 is not null) ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, saved787.Value);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        if (wasRunning) P($"GCUBridge restart ok={OemService.StartGcuBridge()}");
        Thread.Sleep(3000);
        Snap("after-restart");
        P("TBLMODE DONE");
    }

    /// <summary>
    /// 表分裂实验: 停服务, 写哨兵平直表 (CPU=60% GPU=20%, 保留 OEM 温度轴),
    /// 保持 0x7C5 原值 (bit7=0, 即 12:49 观测到 EC 按表跟随时的状态), 0x751 不动。
    /// 每拍: 表前脉冲 04→00, 写表, 表后脉冲 04→00 (两种时序都覆盖)。采样含 0x461/0x469 目标。
    /// 用法: FanSlider.exe --tblsplit [seconds]
    /// </summary>
    private static void TableSplitTest(int seconds, int cpuPct)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }
        P($"TBL SPLIT: 停服务 CPU={cpuPct}/GPU=20 哨兵表, 0x7C5 保持原值, {seconds}s");

        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        static string H(byte? v) => v is null ? "??" : $"0x{v.Value:X2}";
        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var cd = ec.Read(FanProtocol.ADDR_CPU_TBL_DUTY0);
            var gd = ec.Read(FanProtocol.ADDR_GPU_TBL_DUTY0);
            var res = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            var m461 = ec.Read(0x461);
            var m469 = ec.Read(0x469);
            var rpmL = FanProtocol.ReadRpm(ec, false);
            var rpmR = FanProtocol.ReadRpm(ec, true);
            P($"{tag,-14} DUTY L={l} R={r}  target 0x461={H(m461)} 0x469={H(m469)}  tbl0 C={H(cd)} G={H(gd)}  0x7C5={H(res)} RPM={rpmL?.ToString() ?? "??"}/{rpmR?.ToString() ?? "??"}");
        }

        var saved7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        var saved787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        var snap = FanTableSnapshot.Take(ec);
        if (snap.CpuUp is null || snap.CpuDown is null || snap.CpuDuty is null ||
            snap.GpuUp is null || snap.GpuDown is null || snap.GpuDuty is null)
        { P("SNAPSHOT FAIL"); return; }
        Snap("baseline");
        bool wasRunning = OemService.IsGcuBridgeRunning();
        if (wasRunning && !OemService.StopGcuBridge()) { P("STOP GCUBridge FAIL"); return; }
        P("GCUBridge stopped");

        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        var cpu = FanTableModel.FromBlocks(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0,
            FanProtocol.ADDR_CPU_TBL_DUTY0, snap.CpuUp!, snap.CpuDown!, snap.CpuDuty!);
        var gpu = FanTableModel.FromBlocks(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0,
            FanProtocol.ADDR_GPU_TBL_DUTY0, snap.GpuUp!, snap.GpuDown!, snap.GpuDuty!);
        cpu.SetFlatDuty(cpuPct);
        gpu.SetFlatDuty(20);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        int beats = 0;
        while (sw.Elapsed.TotalSeconds < seconds)
        {
            beats++;
            // 表前: 04→00 (OEM SetFanTable 前缀时序)
            ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
            ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
            cpu.WriteToEc(ec);
            gpu.WriteToEc(ec);
            // 表后: 再来一次 04→00 (覆盖 commit-after 语义)
            ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
            ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
            Snap($"beat{beats}");
            Thread.Sleep(10000);
        }

        P("tblsplit done — restore");
        snap.Restore(ec);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        if (saved787 is not null) ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, saved787.Value);
        if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, saved7c5.Value);
        if (wasRunning) P($"GCUBridge restart ok={OemService.StartGcuBridge()}");
        Thread.Sleep(4000);
        Snap("after-restart");
        P("TBL SPLIT DONE");
    }

    /// <summary>
    /// 净 bit7 实验: 停服务 → 写哨兵平直表 (CPU=cpuPct/GPU=20) → 0x7C6=00 开闸 / 04 提交。
    /// 0x7C5 完全不动 (除非参数 bit7)。采样 0x461/0x469 目标是否向哨兵值收敛。
    /// 用法: FanSlider.exe --cleanbit [cpuPct] [seconds] [bit7]
    /// </summary>
    private static void CleanBitTest(int cpuPct, int seconds, bool setBit7)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }
        P($"CLEANBIT: 停服务 CPU={cpuPct}/GPU=20 表, bit7={(setBit7 ? "强制开" : "不动")}, {seconds}s");

        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        static string H(byte? v) => v is null ? "??" : $"0x{v.Value:X2}";
        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var cd = ec.Read(FanProtocol.ADDR_CPU_TBL_DUTY0);
            var gd = ec.Read(FanProtocol.ADDR_GPU_TBL_DUTY0);
            var res = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            var m461 = ec.Read(0x461);
            var m469 = ec.Read(0x469);
            P($"{tag,-14} DUTY L={l} R={r}  target 0x461={H(m461)} 0x469={H(m469)}  tbl0 C={H(cd)} G={H(gd)}  0x7C5={H(res)}");
        }

        var saved7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        var saved787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        var snap = FanTableSnapshot.Take(ec);
        if (snap.CpuUp is null || snap.CpuDown is null || snap.CpuDuty is null ||
            snap.GpuUp is null || snap.GpuDown is null || snap.GpuDuty is null)
        { P("SNAPSHOT FAIL"); return; }
        Snap("baseline");
        bool wasRunning = OemService.IsGcuBridgeRunning();
        if (wasRunning && !OemService.StopGcuBridge()) { P("STOP GCUBridge FAIL"); return; }
        Thread.Sleep(600);
        P("GCUBridge stopped");

        var cpu = FanTableModel.FromBlocks(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0,
            FanProtocol.ADDR_CPU_TBL_DUTY0, snap.CpuUp!, snap.CpuDown!, snap.CpuDuty!);
        var gpu = FanTableModel.FromBlocks(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0,
            FanProtocol.ADDR_GPU_TBL_DUTY0, snap.GpuUp!, snap.GpuDown!, snap.GpuDuty!);
        cpu.SetFlatDuty(cpuPct);
        gpu.SetFlatDuty(20);

        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        cpu.WriteToEc(ec);
        gpu.WriteToEc(ec);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        if (setBit7 && saved7c5 is not null)
            ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(saved7c5.Value | 0x80));
        Snap("applied");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        int beats = 0;
        while (sw.Elapsed.TotalSeconds < seconds)
        {
            Thread.Sleep(10000);
            beats++;
            Snap($"beat{beats}");
        }

        P("cleanbit done — restore");
        snap.Restore(ec);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        if (saved787 is not null) ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, saved787.Value);
        if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, saved7c5.Value);
        if (wasRunning) P($"GCUBridge restart ok={OemService.StartGcuBridge()}");
        Thread.Sleep(4000);
        Snap("after-restart");
        P("CLEANBIT DONE");
    }

    /// <summary>
    /// OEM 逐字节重放: 停服务后, 每轮执行 0x787=00 → 0x7C6=00 → 94 次表写 (每字节间隔 10ms,
    /// 复刻 AcpiCtrl.Write 的 DelayTime 节流) → 0x7C6=04 提交停留。默认 3 轮 (服务对同内容连推 3 份)。
    /// 哨兵平直表 CPU=cpuPct/GPU=gpuPct, 独立位保持开。用法: FanSlider.exe --oemexact [rounds] [seconds] [gpuPct] [cpuPct]
    /// </summary>
    private static void OemExactTest(int rounds, int seconds, int gpuPct, int cpuPct, bool combo, bool fullProfile, bool bit7off)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }
        P($"OEMEXACT: {rounds}轮 10ms节流 CPU={cpuPct}% GPU={gpuPct}% combo={(combo ? "on" : "off")} fullprofile={(fullProfile ? "on" : "off")} bit7={(bit7off ? "强制关" : "保持开")} 采样{seconds}s");

        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        static string H(byte? v) => v is null ? "??" : $"0x{v.Value:X2}";
        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var cd = ec.Read(FanProtocol.ADDR_CPU_TBL_DUTY0);
            var gd = ec.Read(FanProtocol.ADDR_GPU_TBL_DUTY0);
            var res = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            var m461 = ec.Read(0x461);
            var m469 = ec.Read(0x469);
            var rpmL = FanProtocol.ReadRpm(ec, false);
            var rpmR = FanProtocol.ReadRpm(ec, true);
            P($"{tag,-14} DUTY L={l} R={r}  target 0x461={H(m461)} 0x469={H(m469)}  tbl0 C={H(cd)} G={H(gd)}  0x7C5={H(res)} RPM={rpmL?.ToString() ?? "??"}/{rpmR?.ToString() ?? "??"}");
        }

        var saved7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        var saved787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        var snap = FanTableSnapshot.Take(ec);
        if (snap.CpuUp is null || snap.CpuDown is null || snap.CpuDuty is null ||
            snap.GpuUp is null || snap.GpuDown is null || snap.GpuDuty is null)
        { P("SNAPSHOT FAIL"); return; }
        Snap("baseline");
        bool wasRunning = OemService.IsGcuBridgeRunning();
        if (wasRunning && !OemService.StopGcuBridge()) { P("STOP GCUBridge FAIL"); return; }
        Thread.Sleep(600);
        P("GCUBridge stopped");

        var cpu = FanTableModel.FromBlocks(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0,
            FanProtocol.ADDR_CPU_TBL_DUTY0, snap.CpuUp!, snap.CpuDown!, snap.CpuDuty!);
        var gpu = FanTableModel.FromBlocks(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0,
            FanProtocol.ADDR_GPU_TBL_DUTY0, snap.GpuUp!, snap.GpuDown!, snap.GpuDuty!);
        cpu.SetFlatDuty(cpuPct);
        gpu.SetFlatDuty(gpuPct);

        // 复刻 AcpiCtrl.Write 的 10ms 节流逐字节写表 (47 项)
        void ThrottledTable(FanTableModel m)
        {
            for (int i = 0; i < FanTableModel.Points; i++)
            {
                byte up = i < m.ActivePoints ? m.UpT[i] : FanTableModel.PadTemp;
                ec.Write((ushort)(m.UpBase + i), up); Thread.Sleep(10);
                if (i < FanTableModel.Points - 1)
                {
                    byte down = (i + 1) < m.ActivePoints ? m.DownT[i + 1] : FanTableModel.PadTemp;
                    ec.Write((ushort)(m.DownBase + i + 1), down); Thread.Sleep(10);
                }
                byte duty = i < m.ActivePoints ? FanTableModel.Scale(m.DutyPct[i]) : FanTableModel.PadDuty;
                ec.Write((ushort)(m.DutyBase + i), duty); Thread.Sleep(10);
            }
        }

        for (int round = 1; round <= rounds; round++)
        {
            if (combo)
            {
                // a9c0: UserRestore_Mode_Details → SetAPCustomerModeLightOn 0x727=0x40,
                // SetFanMode 0x751=0x00, 0x726=0x80 — 服务应用曲线前的客户模式 combo
                ec.Write(0x727, 0x40); Thread.Sleep(10);
                ec.Write(FanProtocol.ADDR_FAN_MODE, 0x00); Thread.Sleep(10);
                ec.Write(0x726, 0x80); Thread.Sleep(10);
            }
            if (fullProfile)
            {
                // a9c0 事件序 (206..435) 完整复刻: 开闸前 SPC/电源/GPU 全家桶。
                // EC 的 0x7C6=04 提交可能只在"同一 profile 应用窗口"内才落定。
                void W(ushort a, byte v) { ec.Write(a, v); Thread.Sleep(10); }
                W(0x743, 0x05); // CTGP funCtrl enable
                W(0x744, 0x32); // CTGP target 50W
                W(0x745, 0x00); // DB power target
                W(0x746, 0x05); // DB max TGP
                W(0x783, 0x4B); // PL1 75W
                W(0x784, 0x55); // PL2 85W
                W(0x785, 0x55); // PL4 85W
                W(0x786, 0x00); // Tcc offset
                W(0x753, 0x41); // CPU VRM 65
                W(0x754, 0x78); // CPU VRM 120
                W(0x787, 0x00); // 手动档退出 (SetFanSwitchSpeed)
                W(0x7A6, 0x09); // health protection
                W(0x728, 0x01); // super performance
                W(0x741, 0x80); // customer mode light flag
            }
            ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
            ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE); // 00 开闸
            Thread.Sleep(10);
            ThrottledTable(cpu);
            ThrottledTable(gpu);
            ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER); // 04 提交, 停留
            Thread.Sleep(10);
            if (saved7c5 is not null)
                ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE,
                    bit7off ? (byte)(saved7c5.Value & 0x7F) : (byte)(saved7c5.Value | 0x80));
            P($"round {round}/{rounds} pushed (~{(FanTableModel.WritesPerTable * 2 + 3) * 10}ms throttled)");
            Snap($"r{round}");
            Thread.Sleep(2000);
        }

        var sw2 = System.Diagnostics.Stopwatch.StartNew();
        int beats = 0;
        while (sw2.Elapsed.TotalSeconds < seconds)
        {
            Thread.Sleep(10000);
            beats++;
            Snap($"beat{beats}");
        }

        P("oemexact done — restore");
        snap.Restore(ec);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        if (saved787 is not null) ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, saved787.Value);
        if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, saved7c5.Value);
        if (wasRunning) P($"GCUBridge restart ok={OemService.StartGcuBridge()}");
        Thread.Sleep(4000);
        Snap("after-restart");
        P("OEMEXACT DONE");
    }

    /// <summary>
    /// EC 曲线直写端到端回归 (2026-09-06): EcCurveSession.Start(T0=0 平直曲线) → 采样 → Dispose 还原。
    /// 实证: CPU 40%/GPU 20% → duty 精确收敛 80/50。用法: FanSlider.exe --profiletest [cpuPct] [gpuPct]
    /// </summary>
    private static void CurveTest(int cpuPct, int gpuPct)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }
        P($"CURVETEST: 接管 CPU={cpuPct}% GPU={gpuPct}% (T0=0 平直曲线直写)");

        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        static string H(byte? v) => v is null ? "??" : $"0x{v.Value:X2}";
        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var c1 = ec.Read((ushort)(FanProtocol.ADDR_CPU_TBL_DUTY0 + 1));
            var g1 = ec.Read((ushort)(FanProtocol.ADDR_GPU_TBL_DUTY0 + 1));
            var res = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            var rpmL = FanProtocol.ReadRpm(ec, false);
            var rpmR = FanProtocol.ReadRpm(ec, true);
            P($"{tag,-14} DUTY L={l} R={r}  tbl1 C={H(c1)} G={H(g1)}  0x7C5={H(res)}  RPM={rpmL?.ToString() ?? "??"}/{rpmR?.ToString() ?? "??"}");
        }

        Snap("baseline");
        var sess = EcCurveSession.StartFlat(ec, cpuPct, gpuPct);
        if (sess is null) { P($"START FAIL: {EcCurveSession.LastFailReason}"); return; }
        P("curve written (T0=0, 三区全表) + custom/respective/universal gates armed");

        for (int i = 1; i <= 6; i++) { Snap($"T+{i * 5}s"); Thread.Sleep(5000); }
        bool ok = sess.WaitForEc(cpuPct, gpuPct, 45000);
        P($"WaitForEc(cpu={cpuPct},gpu={gpuPct}) = {ok}");

        sess.Dispose();
        P("curve disposed (全表区 + 门控按快照还原)");
        Thread.Sleep(5000);
        Snap("restored");
        P("CURVETEST DONE");
    }

    /// <summary>0xF5F 握手: 让固件重装内置默认曲线 (脱离服务也能还原 OEM 默认)。用法: --restoredefault [mode 1=Turbo 2=Gaming 3=Office]</summary>
    private static void RestoreDefaultTest(int mode)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }
        P($"RESTOREDEFAULT: mode={mode} (0xF5F 握手)");

        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }
        bool ok = EcCurveSession.RestoreDefault(ec, mode);
        P($"RestoreDefault(mode={mode}) = {ok}");
        Thread.Sleep(3000);
        var c1 = ec.Read((ushort)(FanProtocol.ADDR_CPU_TBL_DUTY0 + 1));
        var g1 = ec.Read((ushort)(FanProtocol.ADDR_GPU_TBL_DUTY0 + 1));
        P($"tbl1 C={(c1 is null ? "??" : $"0x{c1.Value:X2}")} G={(g1 is null ? "??" : $"0x{g1.Value:X2}")}  (固件默认表已载入)");
        P("RESTOREDEFAULT DONE");
    }

    /// <summary>
    /// 状态镜像实验: 服务存活时, 完整复刻"服务已推表"的 EC 状态组合再写哨兵表:
    /// 0x751=0xA0 (User HiMode, 服务常态) + 0x787=0 + 0x7C6=0x04 常驻 + 0x7C5=0xA0 (bit7|bit5)
    /// + 哨兵表 CPU/GPU。若 EC 控制环按状态组合采纳表区, duty 将收敛到哨兵值。
    /// 用法: FanSlider.exe --statemimic [cpuPct] [gpuPct]
    /// </summary>
    private static void StateMimicTest(int cpuPct, int gpuPct)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }
        P($"STATEMIMIC: 停服务 CPU={cpuPct}/GPU={gpuPct} 哨兵表 + HiMode/7C5=A0/7C6=04 状态镜像");

        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        static string H(byte? v) => v is null ? "??" : $"0x{v.Value:X2}";
        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var c1 = ec.Read((ushort)(FanProtocol.ADDR_CPU_TBL_DUTY0 + 1));
            var g1 = ec.Read((ushort)(FanProtocol.ADDR_GPU_TBL_DUTY0 + 1));
            var res = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            var m = ec.Read(FanProtocol.ADDR_FAN_MODE);
            var tr = ec.Read(FanProtocol.ADDR_FAN_CONTROL_TRIGGER);
            var rpmL = FanProtocol.ReadRpm(ec, false);
            var rpmR = FanProtocol.ReadRpm(ec, true);
            P($"{tag,-16} DUTY L={l} R={r}  tbl1 C={H(c1)} G={H(g1)}  0x7C5={H(res)} 0x751={H(m)} 0x7C6={H(tr)}  RPM={rpmL?.ToString() ?? "??"}/{rpmR?.ToString() ?? "??"}");
        }

        var saved751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
        var saved787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        var saved7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        var snap = FanTableSnapshot.Take(ec);
        if (snap.CpuUp is null || snap.GpuUp is null) { P("SNAPSHOT FAIL"); return; }
        Snap("baseline");
        bool wasRunning = OemService.IsGcuBridgeRunning();
        if (wasRunning && !OemService.StopGcuBridge()) { P("STOP FAIL"); return; }
        Thread.Sleep(600);
        P("GCUBridge stopped");

        var cpu = FanTableModel.FromBlocks(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0,
            FanProtocol.ADDR_CPU_TBL_DUTY0, snap.CpuUp!, snap.CpuDown!, snap.CpuDuty!);
        var gpu = FanTableModel.FromBlocks(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0,
            FanProtocol.ADDR_GPU_TBL_DUTY0, snap.GpuUp!, snap.GpuDown!, snap.GpuDuty!);
        cpu.SetFlatDuty(cpuPct);
        gpu.SetFlatDuty(gpuPct);

        // 状态镜像: 与服务推送完成后的 EC 状态完全一致
        ec.Write(FanProtocol.ADDR_FAN_MODE, 0xA0);                    // User HiMode (服务常态)
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);              // 手动档退出
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER); // 7C6=04 常驻
        cpu.WriteToEc(ec);
        gpu.WriteToEc(ec);
        ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, 0xA0);              // bit7|bit5
        Snap("applied");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        int beats = 0;
        while (sw.Elapsed.TotalSeconds < 60)
        {
            Thread.Sleep(10000);
            beats++;
            ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_TRIGGER); // 保持 04
            if (beats % 3 == 0) { cpu.WriteToEc(ec); gpu.WriteToEc(ec); }                    // 每30s重推表
            Snap($"beat{beats}");
        }

        P("statemimic done — restore");
        snap.Restore(ec);
        if (saved751 is not null) ec.Write(FanProtocol.ADDR_FAN_MODE, saved751.Value);
        if (saved787 is not null) ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, saved787.Value);
        if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, saved7c5.Value);
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, FanProtocol.FAN_CONTROL_IDLE);
        if (wasRunning) P($"GCUBridge restart ok={OemService.StartGcuBridge()}");
        Thread.Sleep(4000);
        Snap("after-restart");
        P("STATEMIMIC DONE");
    }

    /// <summary>
    /// 正式协议硬件矩阵。只走 PerFanSession，不写 EF3/EF8，不改 0x751。
    /// 用法: FanSlider.exe --hwtest
    /// </summary>
    private static void HardwareMatrix()
    {
        var paths = new[]
        {
            @"D:\tmp\cc_analysis\hwtest.log",
            Path.Combine(AppContext.BaseDirectory, "hwtest.log"),
        };
        void P(string s)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] {s}";
            Console.WriteLine(line);
            foreach (var p in paths)
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(p)!);
                    File.AppendAllText(p, line + Environment.NewLine);
                }
                catch { }
            }
        }

        P("===== HWTEST start =====");
        P($"exe={Environment.ProcessPath}");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        static int Pct(byte? raw) => raw is null ? -1 : FanProtocol.NormalizeDuty(raw.Value);
        string Hex(ushort addr) => ec.Read(addr) is byte v ? $"0x{v:X2}" : "??";
        string Axis(ushort baseAddr) =>
            string.Join(",", Enumerable.Range(0, 4).Select(i => Hex((ushort)(baseAddr + i))));

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var rpmL = FanProtocol.ReadRpm(ec, false);
            var rpmR = FanProtocol.ReadRpm(ec, true);
            var d20 = ec.Read(FanProtocol.ADDR_CPU_TBL_DUTY0);
            var d21 = ec.Read((ushort)(FanProtocol.ADDR_CPU_TBL_DUTY0 + 1));
            var d50 = ec.Read(FanProtocol.ADDR_GPU_TBL_DUTY0);
            var d51 = ec.Read((ushort)(FanProtocol.ADDR_GPU_TBL_DUTY0 + 1));
            P($"{tag,-14} L={Pct(l),3}%({l,3}) R={Pct(r),3}%({r,3}) RPM={rpmL?.ToString() ?? "?"}/{rpmR?.ToString() ?? "?"}  751={Hex(0x751)} 787={Hex(0x787)} 7C5={Hex(0x7C5)} 7C6={Hex(0x7C6)}  tblDuty CPU={d20}/{d21} GPU={d50}/{d51}  GCU={(OemService.IsGcuBridgeRunning() ? "run" : "stop")}");
        }

        P($"baseline axis CPU Up={Axis(FanProtocol.ADDR_CPU_TBL_UP0)} Down={Axis(FanProtocol.ADDR_CPU_TBL_DOWN0)}");
        P($"baseline axis GPU Up={Axis(FanProtocol.ADDR_GPU_TBL_UP0)} Down={Axis(FanProtocol.ADDR_GPU_TBL_DOWN0)}");
        Snap("baseline");

        var sess = PerFanSession.Start(ec);
        if (sess is null)
        {
            P($"SESSION START FAIL reason={PerFanSession.LastFailReason} svc={OemService.LastError}");
            Snap("after-fail");
            return;
        }
        P($"session ok stoppedGcu={sess.StoppedGcu} cpuAxisPts={sess.Cpu.ActivePoints} gpuAxisPts={sess.Gpu.ActivePoints}");
        P($"after-start axis CPU Up={Axis(FanProtocol.ADDR_CPU_TBL_UP0)}");
        Snap("session-on");

        (int cpu, int gpu, int holdSec)[] steps =
        {
            (10, 10, 8),
            (25, 25, 10),
            (50, 20, 12),
            (75, 35, 12),
            (100, 100, 10),
        };

        foreach (var (cpu, gpu, hold) in steps)
        {
            bool okC = sess.SetCpuDuty(cpu);
            bool okG = sess.SetGpuDuty(gpu);
            byte expectC = FanTableModel.Scale(cpu);
            byte expectG = FanTableModel.Scale(gpu);
            byte? tC0 = ec.Read(FanProtocol.ADDR_CPU_TBL_DUTY0);
            byte? tC1 = ec.Read((ushort)(FanProtocol.ADDR_CPU_TBL_DUTY0 + 1));
            byte? tG0 = ec.Read(FanProtocol.ADDR_GPU_TBL_DUTY0);
            byte? tG1 = ec.Read((ushort)(FanProtocol.ADDR_GPU_TBL_DUTY0 + 1));
            bool tableOk = tC0 == expectC && tC1 == expectC && tG0 == expectG && tG1 == expectG;
            byte? bit = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            bool bit7 = bit is not null && (bit.Value & FanProtocol.RESPECTIVE_BIT) != 0;
            byte? u787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
            P($"SET CPU={cpu}% GPU={gpu}% writeC={okC}/{sess.LastCpuWrites} writeG={okG}/{sess.LastGpuWrites} table={(tableOk ? "MATCH" : "MISMATCH")} expect={expectC}/{expectG} got CPU={tC0}/{tC1} GPU={tG0}/{tG1} bit7={(bit7 ? "1" : "0")} 787={Hex(0x787)} axisCPU={Axis(FanProtocol.ADDR_CPU_TBL_UP0)}");

            int[] marks = hold <= 8 ? [4, 8] : hold <= 10 ? [4, 8, 10] : [4, 8, 12];
            int prev = 0;
            int lastL = -1, lastR = -1;
            foreach (int sec in marks)
            {
                Thread.Sleep((sec - prev) * 1000);
                prev = sec;
                Snap($"  +{sec}s");
                lastL = Pct(ec.Read(FanProtocol.ADDR_FAN_L_DUTY));
                lastR = Pct(ec.Read(FanProtocol.ADDR_FAN_R_DUTY));
            }

            int dL = lastL < 0 ? 99 : Math.Abs(lastL - cpu);
            int dR = lastR < 0 ? 99 : Math.Abs(lastR - gpu);
            bool splitWanted = Math.Abs(cpu - gpu) >= 20;
            bool splitGot = lastL >= 0 && lastR >= 0 && Math.Abs(lastL - lastR) >= 8;
            string verdict;
            if (!okC || !okG || !tableOk || !bit7 || u787 != 0x00) verdict = "PROTO-FAIL";
            else if (dL <= 15 && dR <= 15 && (!splitWanted || splitGot)) verdict = "PASS";
            else if (splitWanted && splitGot) verdict = "SPLIT";
            else if (dL <= 25 || dR <= 25) verdict = "NEAR";
            else verdict = "FAIL";
            P($"VERDICT CPU={cpu}/{lastL} GPU={gpu}/{lastR} Δ={dL}/{dR} splitWant={splitWanted} splitGot={splitGot} {verdict}");
        }

        P("restoring session…");
        sess.Dispose();
        Thread.Sleep(4000);
        Snap("restored");
        P($"restored axis CPU Up={Axis(FanProtocol.ADDR_CPU_TBL_UP0)} Down={Axis(FanProtocol.ADDR_CPU_TBL_DOWN0)}");
        P($"GCUBridge after restore: {(OemService.IsGcuBridgeRunning() ? "RUNNING" : "NOT RUNNING")}");
        P("===== HWTEST done =====");
    }

    /// <summary>
    /// 对照: 停服务后写平直表，但把 0x7C6 保持为基线 0x04（不脉冲到 0x00）。
    /// 并读完整 16 点 Duty、F5D/F5E、0x461/0x469、TempRead。
    /// </summary>
    private static void HardwareMatrix2()
    {
        var log = @"D:\tmp\cc_analysis\hwtest2.log";
        void P(string s)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] {s}";
            Console.WriteLine(line);
            try { Directory.CreateDirectory(Path.GetDirectoryName(log)!); File.AppendAllText(log, line + Environment.NewLine); } catch { }
        }

        P("===== HWTEST2 start (keep 0x7C6=0x04, long hold) =====");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        static int Pct(byte? raw) => raw is null ? -1 : FanProtocol.NormalizeDuty(raw.Value);
        string Hex(ushort a) => ec.Read(a) is byte v ? $"0x{v:X2}" : "??";
        string DumpDuty(ushort baseAddr) =>
            string.Join(" ", Enumerable.Range(0, 16).Select(i => Hex((ushort)(baseAddr + i))));

        byte? t1 = null, t2 = null, t3 = null;
        try { t1 = (byte?)ec.ReadTemp(EcDriver.IOCTL_GPD_ACPI_TMPREAD1, 0); } catch { }
        try { t2 = (byte?)ec.ReadTemp(EcDriver.IOCTL_GPD_ACPI_TMPREAD2, 0); } catch { }
        try { t3 = (byte?)ec.ReadTemp(EcDriver.IOCTL_GPD_ACPI_TMPREAD3, 0); } catch { }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var rpmL = FanProtocol.ReadRpm(ec, false);
            var rpmR = FanProtocol.ReadRpm(ec, true);
            P($"{tag,-12} L={Pct(l),3}%({l,3}) R={Pct(r),3}%({r,3}) RPM={rpmL}/{rpmR}  751={Hex(0x751)} 787={Hex(0x787)} 7C5={Hex(0x7C5)} 7C6={Hex(0x7C6)} 461={Hex(0x461)} 469={Hex(0x469)} F5D={Hex(0xF5D)} F5E={Hex(0xF5E)} F5F={Hex(0xF5F)} TMP={t1}/{t2}/{t3} GCU={(OemService.IsGcuBridgeRunning() ? "run" : "stop")}");
        }

        Snap("baseline");
        P("CPU duty16 " + DumpDuty(FanProtocol.ADDR_CPU_TBL_DUTY0));
        P("GPU duty16 " + DumpDuty(FanProtocol.ADDR_GPU_TBL_DUTY0));
        P("CPU up16   " + DumpDuty(FanProtocol.ADDR_CPU_TBL_UP0));
        P("CPU dn16   " + DumpDuty(FanProtocol.ADDR_CPU_TBL_DOWN0));

        var sess = PerFanSession.Start(ec);
        if (sess is null) { P($"START FAIL {PerFanSession.LastFailReason}"); return; }
        P($"session stoppedGcu={sess.StoppedGcu} pts={sess.Cpu.ActivePoints}/{sess.Gpu.ActivePoints}");

        // 对照: 把触发器钉回基线 0x04，而不是正式路径的 idle 0x00
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, 0x04);
        P("forced 7C6=0x04 after Start");
        Snap("session-on");

        (int cpu, int gpu, int hold)[] steps = { (100, 20, 25), (20, 100, 25), (100, 100, 20) };
        foreach (var (cpu, gpu, hold) in steps)
        {
            bool okC = sess.SetCpuDuty(cpu);
            bool okG = sess.SetGpuDuty(gpu);
            ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, 0x04);
            P($"SET {cpu}/{gpu} write={okC}/{okG} n={sess.LastCpuWrites}/{sess.LastGpuWrites} CPU16={DumpDuty(FanProtocol.ADDR_CPU_TBL_DUTY0)} GPU16={DumpDuty(FanProtocol.ADDR_GPU_TBL_DUTY0)}");
            int prev = 0;
            foreach (int sec in new[] { 5, 12, 20, hold })
            {
                if (sec > hold) break;
                Thread.Sleep((sec - prev) * 1000);
                prev = sec;
                Snap($"  +{sec}s");
            }
        }

        sess.Dispose();
        Thread.Sleep(4000);
        Snap("restored");
        P("CPU duty16 " + DumpDuty(FanProtocol.ADDR_CPU_TBL_DUTY0));
        P("GCU after: " + (OemService.IsGcuBridgeRunning() ? "RUNNING" : "NOT RUNNING"));
        P("===== HWTEST2 done =====");
    }

    /// <summary>
    /// 共存活对照：不停 GCU、不脉冲 0x7C6，只写 16 点平直表并置 0x7C5 bit7。
    /// 验证表区是否被服务覆盖、风扇是否仍由 GCU 根据表插值。
    /// </summary>
    private static void HardwareCoexist()
    {
        var log = @"D:\tmp\cc_analysis\hwtest3.log";
        void P(string s)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] {s}";
            Console.WriteLine(line);
            try { Directory.CreateDirectory(Path.GetDirectoryName(log)!); File.AppendAllText(log, line + Environment.NewLine); } catch { }
        }

        P("===== HWTEST3 coexist (GCU stays up, no 7C6 pulse) =====");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL {ec.LastError}"); return; }

        static int Pct(byte? raw) => raw is null ? -1 : FanProtocol.NormalizeDuty(raw.Value);
        string Hex(ushort a) => ec.Read(a) is byte v ? $"0x{v:X2}" : "??";
        string Dump(ushort b) => string.Join(" ", Enumerable.Range(0, 16).Select(i => Hex((ushort)(b + i))));

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            P($"{tag,-12} L={Pct(l),3}% R={Pct(r),3}% RPM={FanProtocol.ReadRpm(ec, false)}/{FanProtocol.ReadRpm(ec, true)} 751={Hex(0x751)} 787={Hex(0x787)} 7C5={Hex(0x7C5)} 7C6={Hex(0x7C6)} 461={Hex(0x461)} 469={Hex(0x469)} GCU={(OemService.IsGcuBridgeRunning() ? "run" : "stop")}");
        }

        var snap = FanTableSnapshot.Take(ec);
        var saved7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        Snap("baseline");
        P("CPU duty " + Dump(FanProtocol.ADDR_CPU_TBL_DUTY0));
        P("GPU duty " + Dump(FanProtocol.ADDR_GPU_TBL_DUTY0));

        var cpu = FanTableModel.FromBlocks(
            FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0,
            snap.CpuUp!, snap.CpuDown!, snap.CpuDuty!);
        var gpu = FanTableModel.FromBlocks(
            FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0,
            snap.GpuUp!, snap.GpuDown!, snap.GpuDuty!);
        cpu.SetFlatDuty(100);
        gpu.SetFlatDuty(20);
        int nC = cpu.WriteToEc(ec);
        int nG = gpu.WriteToEc(ec);
        if (saved7c5 is not null)
            ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, FanProtocol.SetRespective(saved7c5.Value, true));
        P($"wrote CPU={nC}/47 GPU={nG}/47 7C5={Hex(0x7C5)} (GCU not stopped, 7C6 not pulsed)");
        P("CPU duty " + Dump(FanProtocol.ADDR_CPU_TBL_DUTY0));
        P("GPU duty " + Dump(FanProtocol.ADDR_GPU_TBL_DUTY0));

        foreach (int sec in new[] { 2, 5, 10, 16 })
        {
            Thread.Sleep(sec == 2 ? 2000 : (sec == 5 ? 3000 : 6000));
            Snap($"+{sec}s");
            P("  CPU " + Dump(FanProtocol.ADDR_CPU_TBL_DUTY0));
            P("  GPU " + Dump(FanProtocol.ADDR_GPU_TBL_DUTY0));
        }

        snap.Restore(ec);
        if (saved7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, saved7c5.Value);
        Thread.Sleep(2000);
        Snap("restored");
        P("CPU duty " + Dump(FanProtocol.ADDR_CPU_TBL_DUTY0));
        P($"GCU still {(OemService.IsGcuBridgeRunning() ? "RUNNING" : "DEAD")}");
        P("===== HWTEST3 done =====");
    }

    /// <summary>只翻 0x7C5 bit7，不改表、不停服务。看 L/R 会不会从同步变成独立。</summary>
    private static void HardwareBitOnly()
    {
        var log = @"D:\tmp\cc_analysis\hwtest4.log";
        void P(string s)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] {s}";
            Console.WriteLine(line);
            try { File.AppendAllText(log, line + Environment.NewLine); } catch { }
        }
        P("===== HWTEST4 bit-only =====");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL {ec.LastError}"); return; }
        static int Pct(byte? raw) => raw is null ? -1 : FanProtocol.NormalizeDuty(raw.Value);
        string Hex(ushort a) => ec.Read(a) is byte v ? $"0x{v:X2}" : "??";
        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            P($"{tag,-14} L={Pct(l),3}% R={Pct(r),3}% RPM={FanProtocol.ReadRpm(ec,false)}/{FanProtocol.ReadRpm(ec,true)} 7C5={Hex(0x7C5)} 7C6={Hex(0x7C6)} 751={Hex(0x751)} 787={Hex(0x787)}");
        }
        var orig = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        Snap("baseline");
        if (orig is null) return;
        ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, FanProtocol.SetRespective(orig.Value, true));
        Thread.Sleep(3000); Snap("bit7 ON 3s");
        Thread.Sleep(5000); Snap("bit7 ON 8s");
        ec.Write(FanProtocol.ADDR_FAN_CONTROL_TRIGGER, 0x04);
        Thread.Sleep(3000); Snap("bit7 ON + 7C6=04");
        ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, orig.Value);
        Thread.Sleep(3000); Snap("restored 7C5");
        P("===== HWTEST4 done =====");
    }

    /// <summary>
    /// 停 GCUBridge 后用完整协议两边拉满 20s，验证是不是控制中心服务在覆盖。
    /// 用法: FanSlider.exe --fullmax
    /// </summary>
    private static void FullMaxTest()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }

        P("FULLMAX: 停 GCUBridge → PerFanSession 100/100 → 20s → 还原 → 启服务");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var f20 = ec.Read(FanProtocol.ADDR_CPU_TBL_DUTY0);
            var f50 = ec.Read(FanProtocol.ADDR_GPU_TBL_DUTY0);
            P($"{tag,-16} DUTY L={l}({(l ?? 0) / 2}%) R={r}({(r ?? 0) / 2}%)  tblDuty0={f20}/{f50}  0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"} 0xEF3=0x{ec.Read(FanProtocol.ADDR_TBL_EN_CPU)?.ToString("X2") ?? "??"} 0xEF8=0x{ec.Read(FanProtocol.ADDR_TBL_EN_GPU)?.ToString("X2") ?? "??"} 0x7C5=0x{ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE)?.ToString("X2") ?? "??"}");
        }

        P(RunSc("stop", "GCUBridge"));
        Thread.Sleep(2500);
        try
        {
            Snap("baseline");
            var sess = PerFanSession.Start(ec);
            if (sess is null) { P("SESSION START FAIL"); return; }
            P($"applied: cpuOK={sess.SetCpuDuty(100)} gpuOK={sess.SetGpuDuty(100)}");
            for (int i = 1; i <= 4; i++) { Thread.Sleep(5000); Snap($"T+{i * 5}s"); }
            sess.Dispose();
            Thread.Sleep(3000);
            Snap("restored");
        }
        finally
        {
            P(RunSc("start", "GCUBridge"));
            Thread.Sleep(2000);
        }
        P("FULLMAX DONE");
    }

    /// <summary>
    /// 可用性扫描: 多档 CPU/GPU 目标, 等风扇爬升后读回 0x75B/0x75C。
    /// 用法: FanSlider.exe --usability
    /// </summary>
    private static void UsabilitySweep()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }

        P("USABILITY: 多档目标 → 实测读回");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        static int Pct(byte? raw) => raw is null ? -1 : Math.Clamp(raw.Value / 2, 0, 100);
        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            P($"{tag,-28} L={Pct(l),3}%({l,3})  R={Pct(r),3}%({r,3})  0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"} 0xEF3={ec.Read(FanProtocol.ADDR_TBL_EN_CPU)} 0xEF8={ec.Read(FanProtocol.ADDR_TBL_EN_GPU)} 0x7C5=0x{ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE)?.ToString("X2") ?? "??"}");
        }

        Snap("baseline");
        var sess = PerFanSession.Start(ec);
        if (sess is null) { P("SESSION START FAIL"); return; }

        (int cpu, int gpu)[] steps =
        {
            (100, 20),
            (80, 40),
            (100, 100),
        };

        foreach (var (cpu, gpu) in steps)
        {
            bool okC = sess.SetCpuDuty(cpu);
            bool okG = sess.SetGpuDuty(gpu);
            // 每档重新钉住生效条件, 防止被固件清掉
            ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
            ec.Write(FanProtocol.ADDR_FAN_MODE, FanProtocol.FAN_MODE_CUSTOMIZE_TBL);
            ec.Write(FanProtocol.ADDR_TBL_EN_CPU, 0x01);
            ec.Write(FanProtocol.ADDR_TBL_EN_GPU, 0x01);
            var c = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            if (c is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, FanProtocol.SetRespective(c.Value, true));

            byte? tCpu = ec.Read(FanProtocol.ADDR_CPU_TBL_DUTY0);
            byte? tGpu = ec.Read(FanProtocol.ADDR_GPU_TBL_DUTY0);
            P($"set CPU={cpu}% GPU={gpu}%  write={(okC && okG ? "OK" : "FAIL")}  tblDuty0={tCpu}/{tGpu}  0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"} 0x7C5=0x{ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE)?.ToString("X2") ?? "??"} EF={ec.Read(FanProtocol.ADDR_TBL_EN_CPU)}/{ec.Read(FanProtocol.ADDR_TBL_EN_GPU)}");
            foreach (int sec in new[] { 5, 12, 20 })
            {
                Thread.Sleep((sec == 5 ? 5 : sec - (sec == 12 ? 5 : 12)) * 1000);
                var l = Pct(ec.Read(FanProtocol.ADDR_FAN_L_DUTY));
                var r = Pct(ec.Read(FanProtocol.ADDR_FAN_R_DUTY));
                int dL = Math.Abs(l - cpu);
                int dR = Math.Abs(r - gpu);
                bool split = Math.Abs(cpu - gpu) >= 20 && Math.Abs(l - r) >= 8;
                string verdict = (dL <= 15 && dR <= 15) ? "PASS" : split ? "SPLIT" : (dL <= 25 || dR <= 25) ? "NEAR" : "FAIL";
                P($"  +{sec,2}s  L={l}% (Δ{dL})  R={r}% (Δ{dR})  split={(split ? "Y" : "N")}  {verdict}");
            }
        }

        sess.Dispose();
        Thread.Sleep(4000);
        Snap("restored");
        P("USABILITY DONE");
    }

    /// <summary>
    /// 把 EC 恢复到"OEM 接管"状态: EF3/EF8=0, 0x7C5 独立位清, 0x751=0x00。
    /// 表区本身不重写 (那是 OEM 快照/服务的职责)。用法: FanSlider.exe --restore
    /// </summary>
    private static void RestoreOem()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }
        P("RESTORE OEM: EF3/EF8=0, 0x7C5&=0x7F, 0x751=0x00");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }
        P($"before: 0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"} " +
          $"0x787=0x{ec.Read(FanProtocol.ADDR_FAN_USER_SPEED)?.ToString("X2") ?? "??"} " +
          $"0x7C5=0x{ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE)?.ToString("X2") ?? "??"} " +
          $"0xEF3=0x{ec.Read(FanProtocol.ADDR_TBL_EN_CPU)?.ToString("X2") ?? "??"} " +
          $"0xEF8=0x{ec.Read(FanProtocol.ADDR_TBL_EN_GPU)?.ToString("X2") ?? "??"}");
        ec.Write(FanProtocol.ADDR_TBL_EN_CPU, 0x00);
        ec.Write(FanProtocol.ADDR_TBL_EN_GPU, 0x00);
        var c = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        if (c is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(c.Value & 0x7F));
        ec.Write(FanProtocol.ADDR_FAN_MODE, 0x00);
        Thread.Sleep(1500);
        P($"after:  0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"} " +
          $"0x787=0x{ec.Read(FanProtocol.ADDR_FAN_USER_SPEED)?.ToString("X2") ?? "??"} " +
          $"0x7C5=0x{ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE)?.ToString("X2") ?? "??"} " +
          $"0xEF3=0x{ec.Read(FanProtocol.ADDR_TBL_EN_CPU)?.ToString("X2") ?? "??"} " +
          $"0xEF8=0x{ec.Read(FanProtocol.ADDR_TBL_EN_GPU)?.ToString("X2") ?? "??"} " +
          $"DUTY L={ec.Read(FanProtocol.ADDR_FAN_L_DUTY)} R={ec.Read(FanProtocol.ADDR_FAN_R_DUTY)}");
        P("RESTORE DONE");
    }

    private static void SelfTest()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);

        P("FanSlider selftest — EC register probe");
        using var ec = new EcDriver();
        if (!ec.Open())
        {
            P($"OPEN FAIL: {ec.LastError}");
            P("提示: 需要管理员权限，且 UWACPIDriver (ACPI\\INOU0000) 已加载并 Running。");
            File.WriteAllText(logPath, sb.ToString());
            Environment.ExitCode = 2;
            return;
        }
        P("OPEN OK: \\\\.\\ACPIDriver");

        void Show(string name, ushort addr)
        {
            var v = ec.Read(addr);
            P(v is null
                ? $"  {name,-26} 0x{addr:X3} = READ FAIL ({ec.LastError})"
                : $"  {name,-26} 0x{addr:X3} = 0x{v.Value:X2} ({v.Value})");
        }
        Show("USER_SPEED (档位)", FanProtocol.ADDR_FAN_USER_SPEED);
        Show("MAFAN_CONTROL", FanProtocol.ADDR_FAN_MODE);
        Show("FAN_L_DUTY (左/CPU)", FanProtocol.ADDR_FAN_L_DUTY);
        Show("FAN_R_DUTY (右/GPU)", FanProtocol.ADDR_FAN_R_DUTY);
        Show("FAN1_RPM_HI", FanProtocol.ADDR_FAN_RPM_HI);
        Show("FAN1_RPM_LO", FanProtocol.ADDR_FAN_RPM_LO);
        Show("FAN2_RPM_HI", FanProtocol.ADDR_FAN2_RPM_HI);
        Show("FAN2_RPM_LO", FanProtocol.ADDR_FAN2_RPM_LO);

        var u = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        if (u is not null)
            P($"解析: 当前档位 = L{FanProtocol.DecodeLevel(u.Value)}");
        var m = ec.Read(FanProtocol.ADDR_FAN_MODE);
        if (m is not null)
            P($"解析: 风扇模式字节 0x751 = {FanProtocol.DescribeModeByte(m.Value)}");
        P("SELFTEST DONE");
        try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { }
    }

    /// <summary>
    /// 设置并持续保持风扇档位 (0=自动, 5=满速)。用法: FanSlider.exe --set &lt;level&gt;
    /// </summary>
    /// <summary>
    /// 实验: 扫描 0x751 的候选"强制满速"字节, 观察 DUTY 是否冲到 0xFF/高值。
    /// 每个值保持 2.5s, 最后恢复 0xA0。
    /// </summary>
    private static void Probe()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        P("PROBE 0x751 — 找真正拉满 DUTY 的字节");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); return; }

        void Snap(string tag)
        {
            var d751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
            var d787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var r1 = ec.Read(FanProtocol.ADDR_FAN_RPM_HI); var r2 = ec.Read(FanProtocol.ADDR_FAN_RPM_LO);
            var s1 = ec.Read(FanProtocol.ADDR_FAN2_RPM_HI); var s2 = ec.Read(FanProtocol.ADDR_FAN2_RPM_LO);
            int? a = (r1 is not null && r2 is not null) ? (r1.Value << 8) | r2.Value : null;
            int? b = (s1 is not null && s2 is not null) ? (s1.Value << 8) | s2.Value : null;
            P($"{tag,-12} 0x751={ToHex(d751)} 0x787={ToHex(d787)} DUTY={l}/{r}% RPM={a}/{b}");
        }
        static string ToHex(byte? b) => b is null ? "??" : $"0x{b.Value:X2}";

        Snap("baseline");
        foreach (ushort addr in new ushort[] { 0x780, 0x781, 0x782 })
            P($"(read) 0x{addr:X3} = {ToHex(ec.Read(addr))}");

        byte[] try751 = { 0x10, 0x40, 0xFF, 0x01, 0x02 };
        foreach (var v in try751)
        {
            ec.Write(FanProtocol.ADDR_FAN_MODE, v);
            Thread.Sleep(2500);
            Snap($"0x751={ToHex(v)}");
        }
        ec.Write(FanProtocol.ADDR_FAN_MODE, 0xA0);
        Thread.Sleep(1500);
        Snap("restored");

        foreach (var v in new byte[] { 0xA1, 0xA2, 0xA3, 0xA4, 0xA5 })
        {
            ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, v);
            Thread.Sleep(2500);
            Snap($"0x787={ToHex(v)}");
        }
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x85);
        Thread.Sleep(1500);
        Snap("L5 recheck");
        P("PROBE DONE");
        Flush();
    }

    /// <summary>
    /// 开启/关闭 真·满速 FanBoost: 0x751 |= 0x40 / &amp;= ~0x40。用法: --boost [on|off]
    /// </summary>
    private static void Boost(bool on)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        P($"Boost({(on ? "ON (真·满速)" : "off")}): OR/CLEAR bit6 of 0x751");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 2; return; }

        var cur = ec.Read(FanProtocol.ADDR_FAN_MODE);
        if (cur is null) { P($"READ 0x751 FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 3; return; }
        byte v = FanProtocol.SetBoost(cur.Value, on);
        P($"0x751: 0x{cur.Value:X2} -> 0x{v:X2}");
        if (!ec.Write(FanProtocol.ADDR_FAN_MODE, v)) { P($"WRITE FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 4; return; }

        for (int i = 1; i <= 5; i++)
        {
            Thread.Sleep(1500);
            var back = ec.Read(FanProtocol.ADDR_FAN_MODE);
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var r1 = ec.Read(FanProtocol.ADDR_FAN_RPM_HI); var r2 = ec.Read(FanProtocol.ADDR_FAN_RPM_LO);
            var s1 = ec.Read(FanProtocol.ADDR_FAN2_RPM_HI); var s2 = ec.Read(FanProtocol.ADDR_FAN2_RPM_LO);
            int? a = (r1 is not null && r2 is not null) ? (r1.Value << 8) | r2.Value : null;
            int? b = (s1 is not null && s2 is not null) ? (s1.Value << 8) | s2.Value : null;
            P($"+{i}s: 0x751={(back is null ? "??" : $"0x{back.Value:X2}")}  DUTY={l}/{r} (raw, 200=max)  RPMraw={a}/{b}");
        }
        P("BOOST DONE");
        Flush();
    }

    private static void SetLevel(int level)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);

        P($"SetLevel: L{level} ({(level == 0 ? "自动" : level == 5 ? "满速" : "手动")}) — 持续生效，不恢复");
        using var ec = new EcDriver();
        if (!ec.Open())
        {
            P($"OPEN FAIL: {ec.LastError}");
            Flush();
            Environment.ExitCode = 2;
            return;
        }

        P($"写入前: 0x787=0x{ec.Read(FanProtocol.ADDR_FAN_USER_SPEED)?.ToString("X2") ?? "??"}  " +
          $"0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"}  " +
          $"DUTY L={ec.Read(FanProtocol.ADDR_FAN_L_DUTY)}% R={ec.Read(FanProtocol.ADDR_FAN_R_DUTY)}%  " +
          RpmLine(ec));

        byte v = FanProtocol.EncodeLevel(level);
        P(ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, v)
            ? $"写入 0x{v:X2} → EC 0x787 成功"
            : $"写入失败: {ec.LastError}");

        // EC 按 100ms 步进爬升，多采样几次观察转速上升
        for (int i = 1; i <= 5; i++)
        {
            Thread.Sleep(1000);
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            P($"+{i}s: 0x787=0x{ec.Read(FanProtocol.ADDR_FAN_USER_SPEED)?.ToString("X2") ?? "??"}  " +
              $"0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"}  " +
              $"DUTY L={l}% R={r}%  " + RpmLine(ec));
        }
        P("SET DONE");
        Flush();

        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }
    }

    private static string RpmLine(EcDriver ec)
    {
        var r1 = ec.Read(FanProtocol.ADDR_FAN_RPM_HI); var r2 = ec.Read(FanProtocol.ADDR_FAN_RPM_LO);
        var s1 = ec.Read(FanProtocol.ADDR_FAN2_RPM_HI); var s2 = ec.Read(FanProtocol.ADDR_FAN2_RPM_LO);
        int? a = (r1 is not null && r2 is not null) ? (r1.Value << 8) | r2.Value : null;
        int? b = (s1 is not null && s2 is not null) ? (s1.Value << 8) | s2.Value : null;
        return $"RPMraw1={a?.ToString() ?? "?"} RPMraw2={b?.ToString() ?? "?"}";
    }

    /// <summary>
    /// 一次性写测试: 先保存 0x787 当前值 → 写入指定档位 → 等待 → 恢复原值。
    /// 用法: FanSlider.exe --writetest &lt;level 1-5&gt; [hold_ms]
    /// </summary>
    private static void WriteTest(int level, int holdMs)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) { sb.AppendLine(s); }

        P($"Writetest: L{level} hold {holdMs}ms");
        using var ec = new EcDriver();
        if (!ec.Open())
        {
            P($"OPEN FAIL: {ec.LastError}");
            File.AppendAllText(logPath, sb + Environment.NewLine);
            Environment.ExitCode = 2;
            return;
        }
        var saved = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        P($"保存原值 0x787 = {(saved is null ? "READ FAIL" : $"0x{saved.Value:X2}")}");
        byte v = FanProtocol.EncodeLevel(level);
        P(ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, v)
            ? $"写入 0x{v:X2} → 0x787 OK" : $"写入失败: {ec.LastError}");
        Thread.Sleep(holdMs);
        var back = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        string backStr = back is null ? "FAIL" : $"0x{back.Value:X2}, L{FanProtocol.DecodeLevel(back.Value)}";
        P($"写后读回 0x787 = {backStr}");
        if (saved is not null)
        {
            P(ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, saved.Value)
                ? $"已恢复 0x787 = 0x{saved.Value:X2}" : $"恢复失败: {ec.LastError}");
        }
        var dutyL = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
        var dutyR = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
        var r1 = ec.Read(FanProtocol.ADDR_FAN_RPM_HI); var r2 = ec.Read(FanProtocol.ADDR_FAN_RPM_LO);
        var s1 = ec.Read(FanProtocol.ADDR_FAN2_RPM_HI); var s2 = ec.Read(FanProtocol.ADDR_FAN2_RPM_LO);
        int? rpm1 = (r1 is not null && r2 is not null) ? (r1.Value << 8) | r2.Value : null;
        int? rpm2 = (s1 is not null && s2 is not null) ? (s1.Value << 8) | s2.Value : null;
        P($"恢复后: DUTY L={dutyL}% R={dutyR}%  RPM1={rpm1?.ToString() ?? "?"}  RPM2={rpm2?.ToString() ?? "?"}");
        P("WRITETEST DONE");
        try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { }
    }

    /// <summary>
    /// 自动验证: 拍快照 → 开独立位 → CPU 表写平直 pct → 观察 0x75B 下降/0x75C 不变 → 写回快照。
    /// 用法: FanSlider.exe --fantest [pct=30]
    /// </summary>
    private static void FanTest(int pct)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        P($"FANTEST: CPU 风扇接管至 {pct}% (GPU 应保持不动), 10s 后恢复");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 2; return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var res = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            P($"{tag,-28} DUTY L={l}% R={r}%  0x7C5={(res is null ? "??" : $"0x{res.Value:X2}")}");
        }

        var snap = FanTableSnapshot.Take(ec);
        if (snap.CpuUp is null || snap.GpuUp is null) { P("快照失败, 中止"); Flush(); Environment.ExitCode = 3; return; }
        Snap("baseline");

        // 开独立位
        var cur7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        if (cur7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, FanProtocol.SetRespective(cur7c5.Value, true));

        // 读回现表, 平直化为 pct, 只写 CPU 表
        var m = FanTableModel.FromEc(ec, FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0);
        if (m is null) { P("读表失败"); Flush(); Environment.ExitCode = 4; return; }
        m.SetFlatDuty(pct);
        int n = m.WriteToEc(ec);
        P($"CPU 表写入 {n}/48 字节, 生效校验={(m.VerifyOnEc(ec) ? "OK" : "MISMATCH")}");

        // 关键: 机器当前处于用户手动档 (0x787=0x85), EC 按档位运行、不查表。
        // 验证表控制必须临时回到自动档 (0x787=0x00), 测完恢复原档位。
        var savedUser = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        if (savedUser is not null) ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        P($"临时 0x787: 0x{savedUser?.ToString("X2") ?? "??"} → 0x00 (自动档, 查表生效)");

        for (int i = 1; i <= 4; i++) { Thread.Sleep(2500); Snap($"+{i * 2.5:F0}s"); }

        // 恢复: 写回 CPU 表 → 还原档位 → 还原独立位
        P($"恢复 CPU 表: {(snap.RestoreCpu(ec) ? "OK" : "FAIL")}");
        if (savedUser is not null) ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, savedUser.Value);
        P($"0x787 还原为 0x{savedUser?.ToString("X2") ?? "??"}");
        P("独立位还原: " + (cur7c5 is not null && ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, cur7c5.Value) ? "OK" : "FAIL"));
        Thread.Sleep(4000);
        Snap("restored");
        P("FANTEST DONE");
        Flush();
    }

    /// <summary>
    /// 差分测试: CPU 表=80%, GPU 表=30% (差异 100 raw), 自动档窗口内观察
    /// 0x75B/0x75C 谁对应哪张表、表控是否真实生效。用法: FanSlider.exe --fantest2
    /// </summary>
    private static void FanTest2()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        P("FANTEST2 差分: CPU表=80%(160) GPU表=30%(60)");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 2; return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var u = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
            P($"{tag,-26} DUTY L={l} R={r} (raw0-200)  0x787={(u is null ? "??" : $"0x{u.Value:X2}")}");
        }

        var snap = FanTableSnapshot.Take(ec);
        if (snap.CpuUp is null || snap.GpuUp is null) { P("快照失败"); Flush(); Environment.ExitCode = 3; return; }
        Snap("baseline");

        var cur7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        if (cur7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, FanProtocol.SetRespective(cur7c5.Value, true));

        var mc = FanTableModel.FromEc(ec, FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0);
        var mg = FanTableModel.FromEc(ec, FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0);
        if (mc is null || mg is null) { P("读表失败"); Flush(); Environment.ExitCode = 4; return; }
        mc.SetFlatDuty(80); mg.SetFlatDuty(30);
        P($"CPU 表 {mc.WriteToEc(ec)}/48, GPU 表 {mg.WriteToEc(ec)}/48");

        var savedUser = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        if (savedUser is not null) ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        for (int i = 1; i <= 4; i++) { Thread.Sleep(2500); Snap($"+{i * 2.5:F0}s"); }

        P($"恢复: cpu={(snap.RestoreCpu(ec) ? "OK" : "FAIL")} gpu={(snap.RestoreGpu(ec) ? "OK" : "FAIL")}");
        if (savedUser is not null) ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, savedUser.Value);
        if (cur7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, cur7c5.Value);
        Thread.Sleep(4000);
        Snap("restored");
        P("FANTEST2 DONE");
        Flush();
    }

    /// <summary>
    /// 表生效条件差分: CPU=80%/GPU=30% + 0x751=0xA0(HiMode), 分阶段测独立位 A=OFF B=ON。
    /// 用法: FanSlider.exe --fantest3
    /// </summary>
    private static void FanTest3()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        P("FANTEST3: 表 CPU=80% GPU=30%, 0x751=0xA0(HiMode), A=独立OFF B=独立ON");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 2; return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var u = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
            var m = ec.Read(FanProtocol.ADDR_FAN_MODE);
            var c = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            P($"{tag,-26} L={l} R={r}  0x751={(m is null ? "??" : $"0x{m.Value:X2}")} 0x787={(u is null ? "??" : $"0x{u.Value:X2}")} 0x7C5={(c is null ? "??" : $"0x{c.Value:X2}")}");
        }

        var snap = FanTableSnapshot.Take(ec);
        if (snap.CpuUp is null) { P("快照失败"); Flush(); Environment.ExitCode = 3; return; }
        byte? s751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
        byte? s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        byte? s7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        Snap("baseline");

        var mc = FanTableModel.FromEc(ec, FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0);
        var mg = FanTableModel.FromEc(ec, FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0);
        if (mc is null || mg is null) { P("读表失败"); Flush(); Environment.ExitCode = 4; return; }
        mc.SetFlatDuty(80); mg.SetFlatDuty(30);
        P($"表写入: cpu={mc.WriteToEc(ec)}/48 gpu={mg.WriteToEc(ec)}/48");

        if (s787 is not null) ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        if (s7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(s7c5.Value & 0x7F));
        ec.Write(FanProtocol.ADDR_FAN_MODE, 0xA0);
        P("--- A: 0x751=0xA0, 独立OFF ---");
        for (int i = 1; i <= 5; i++) { Thread.Sleep(3000); Snap($"A+{i * 3}s"); }
        if (s7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(s7c5.Value | 0x80));
        P("--- B: 独立ON ---");
        for (int i = 1; i <= 5; i++) { Thread.Sleep(3000); Snap($"B+{i * 3}s"); }

        ec.Write(FanProtocol.ADDR_FAN_MODE, s751 ?? 0x00);
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, s787 ?? 0x00);
        ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, s7c5 ?? 0x20);
        bool ok = snap.Restore(ec);
        P($"恢复(两表+模式+档位+独立位): {(ok ? "OK" : "PARTIAL")}");
        Thread.Sleep(4000);
        Snap("restored");
        P("FANTEST3 DONE");
        Flush();
    }

    /// <summary>
    /// 差分测试4: 活跃表区(0xF00..0xF5F)写 CPU=100% GPU=10%, 然后切换 0x751 触发 EC 重载,
    /// 观察 L/R 分离。用法: FanSlider.exe --fantest4
    /// </summary>
    private static void FanTest4()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        P("FANTEST4: 0xF00区 CPU=100/GPU=10 + 0x751 模式脉冲重载");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 2; return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            P($"{tag,-24} L={l} R={r}  0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"} 0x787=0x{ec.Read(FanProtocol.ADDR_FAN_USER_SPEED)?.ToString("X2") ?? "??"} 0x7C5=0x{ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE)?.ToString("X2") ?? "??"}");
        }

        byte? s751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
        byte? s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        byte? s7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);

        // 快照 0xF00..0xF5F (6 段)
        var saved = new System.Collections.Generic.Dictionary<ushort, byte[]>();
        for (ushort a = 0xF00; a <= 0xF50; a += 0x10)
        {
            var blk = new byte[16];
            for (int i = 0; i < 16; i++) blk[i] = ec.Read((ushort)(a + i)) ?? 0xEE;
            saved[a] = blk;
        }
        Snap("baseline");

        var cpu = new FanTableModel(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0);
        cpu.SetFlatDuty(100);
        var gpu = new FanTableModel(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0);
        gpu.SetFlatDuty(10);
        P($"表写入: cpu={cpu.WriteToEc(ec)}/48 gpu={gpu.WriteToEc(ec)}/48");

        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        if (s7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(s7c5.Value | 0x80));
        for (int i = 1; i <= 4; i++) { Thread.Sleep(3000); Snap($"phase1+{i * 3}s"); }

        // 模式脉冲触发 EC 重载 (Turbo 1s → Normal)
        ec.Write(FanProtocol.ADDR_FAN_MODE, 0x10); Thread.Sleep(2000);
        ec.Write(FanProtocol.ADDR_FAN_MODE, 0x00);
        for (int i = 1; i <= 4; i++) { Thread.Sleep(3000); Snap($"reload+{i * 3}s"); }

        foreach (var (a, blk) in saved)
            for (int i = 0; i < 16; i++) ec.Write((ushort)(a + i), blk[i]);
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, s787 ?? 0x00);
        ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, s7c5 ?? 0x20);
        ec.Write(FanProtocol.ADDR_FAN_MODE, s751 ?? 0x00);
        Thread.Sleep(4000);
        Snap("restored");
        P("FANTEST4 DONE");
        Flush();
    }

    /// <summary>
    /// 一锤定音: 写表(CPU=100,GPU=10)+独立ON+0x751=0x80(User表模式), 观察 L→200/R→20。
    /// 用法: FanSlider.exe --fantest5
    /// </summary>
    private static void FanTest5()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        P("FANTEST5: CPU表=100 GPU表=10 + 独立ON + 0x751=0x80 (User曲线模式)");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 2; return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            P($"{tag,-24} L={l} R={r}  0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"} 0x787=0x{ec.Read(FanProtocol.ADDR_FAN_USER_SPEED)?.ToString("X2") ?? "??"} 0x7C5=0x{ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE)?.ToString("X2") ?? "??"}");
        }

        byte? s751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
        byte? s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        byte? s7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);

        var saved = new System.Collections.Generic.Dictionary<ushort, byte[]>();
        for (ushort a = 0xF00; a <= 0xF50; a += 0x10)
        {
            var blk = new byte[16];
            for (int i = 0; i < 16; i++) blk[i] = ec.Read((ushort)(a + i)) ?? 0xEE;
            saved[a] = blk;
        }
        Snap("baseline");

        var cpu = new FanTableModel(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0);
        cpu.SetFlatDuty(100);
        var gpu = new FanTableModel(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0);
        gpu.SetFlatDuty(10);
        P($"表写入: cpu={cpu.WriteToEc(ec)}/48 gpu={gpu.WriteToEc(ec)}/48");

        if (s7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(s7c5.Value | 0x80));
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        ec.Write(FanProtocol.ADDR_FAN_MODE, 0x80);   // User_Fan_Mode: EC 按 RAM 曲线表跑
        for (int i = 1; i <= 5; i++) { Thread.Sleep(3000); Snap($"+{i * 3}s"); }

        foreach (var (a, blk) in saved)
            for (int i = 0; i < 16; i++) ec.Write((ushort)(a + i), blk[i]);
        ec.Write(FanProtocol.ADDR_FAN_MODE, s751 ?? 0x00);
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, s787 ?? 0x00);
        ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, s7c5 ?? 0x20);
        Thread.Sleep(4000);
        Snap("restored");
        P("FANTEST5 DONE");
        Flush();
    }

    /// <summary>
    /// EC 写监视器 v2: 每秒扫描 0x400..0xFFF 快照 + 关键控制字节, 输出差异。
    /// 用法: FanSlider.exe --watch [秒=600]
    /// </summary>
    private static void Watch(int seconds)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "watch.log");
        void P(string s)
        {
            try { File.AppendAllText(logPath, $"[{DateTime.Now:HH:mm:ss}] {s}{Environment.NewLine}"); } catch { }
        }

        P($"WATCH2 START — 扫描 0x400..0xFFF + 控制字节, {seconds}s");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        const ushort lo = 0x400, hi = 0xFFF;
        byte[]? prev = null;
        byte[]? prevCtl = null;
        ushort[] ctl = { 0x751, 0x787, 0x786, 0x7C5, 0x767, 0x768, 0x76F, 0x770, 0x771, 0x772, 0x765, 0x766, 0x76D, 0x76E };
        var end = DateTime.Now.AddSeconds(seconds);
        int ticks = 0;
        while (DateTime.Now < end)
        {
            var snap = new byte[hi - lo + 1];
            var ctlSnap = new byte[ctl.Length];
            bool ok = true;
            for (int a = lo; a <= hi; a++)
            {
                var v = ec.Read((ushort)a);
                if (v is null) { ok = false; break; }
                snap[a - lo] = v.Value;
            }
            for (int i = 0; i < ctl.Length; i++)
            {
                var v = ec.Read(ctl[i]);
                if (v is null) { ok = false; break; }
                ctlSnap[i] = v.Value;
            }
            if (!ok) { P("read fail, retry"); Thread.Sleep(1000); continue; }

            if (prev is not null)
            {
                var diffs = new List<string>();
                for (int i = 0; i < snap.Length; i++)
                    if (snap[i] != prev[i])
                        diffs.Add($"0x{lo + i:X3}:{prev[i]:X2}→{snap[i]:X2}");
                if (diffs.Count > 0)
                    P("DIFF " + string.Join(" ", diffs));
            }
            if (prevCtl is not null)
                for (int i = 0; i < ctl.Length; i++)
                    if (ctlSnap[i] != prevCtl[i])
                        P($"CTL 0x{ctl[i]:X3}: {prevCtl[i]:X2}→{ctlSnap[i]:X2}");
            prev = snap; prevCtl = ctlSnap;
            ticks++;
            if (ticks % 40 == 0) P($"alive t={ticks}");
            Thread.Sleep(600);
        }
        P("WATCH END");
    }

    /// <summary>
    /// 慢收敛假设验证: CPU表=100% GPU表=10%, 自动档, 连续观察 90s。
    /// 若 L 向 200、R 向 20 缓慢逼近 → 0xF00 区就是活跃表, 只是 EC 有斜率保护。
    /// 用法: FanSlider.exe --fantest6
    /// </summary>
    private static void FanTest6()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        P("FANTEST6: CPU表=100 GPU表=10, 自动档, 观察90s (慢收敛/斜率保护假设)");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 2; return; }

        byte? s751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
        byte? s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        byte? s7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);

        var saved = new System.Collections.Generic.Dictionary<ushort, byte[]>();
        for (ushort a = 0xF00; a <= 0xF50; a += 0x10)
        {
            var blk = new byte[16];
            for (int i = 0; i < 16; i++) blk[i] = ec.Read((ushort)(a + i)) ?? 0xEE;
            saved[a] = blk;
        }
        Snap("baseline");

        var cpu = new FanTableModel(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0);
        cpu.SetFlatDuty(100);
        var gpu = new FanTableModel(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0);
        gpu.SetFlatDuty(10);
        P($"表写入: cpu={cpu.WriteToEc(ec)}/48 gpu={gpu.WriteToEc(ec)}/48");
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        if (s7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(s7c5.Value | 0x80));

        for (int i = 1; i <= 18; i++)
        {
            Thread.Sleep(5000);
            Snap($"+{i * 5}s");
        }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var tblL = ec.Read(FanProtocol.ADDR_CPU_TBL_DUTY0);
            var tblR = ec.Read(FanProtocol.ADDR_GPU_TBL_DUTY0);
            P($"{tag,-14} L={l} R={r} | 表首项 L={tblL} R={tblR} 0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"} 0x787=0x{ec.Read(FanProtocol.ADDR_FAN_USER_SPEED)?.ToString("X2") ?? "??"}");
        }

        foreach (var (a, blk) in saved)
            for (int i = 0; i < 16; i++) ec.Write((ushort)(a + i), blk[i]);
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, s787 ?? 0x00);
        ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, s7c5 ?? 0x20);
        Thread.Sleep(5000);
        Snap("restored");
        P("FANTEST6 DONE");
        Flush();
    }

    /// <summary>
    /// fantest7: GetFanMode 位解码显示 0x751 bit4=CUSTOMIZE 表模式 (此前"Turbo"判断错误)。
    /// 写表 CPU=100/GPU=10 → 0x751=0x10 (Customize)。若 L→200 R→20 则分风扇谜底解开。
    /// 用法: FanSlider.exe --fantest7
    /// </summary>
    private static void FanTest7()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        P("FANTEST7: 表 CPU=100/GPU=10 + 0x751=0x10 (Customize 表模式假设)");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 2; return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var tl = ec.Read(FanProtocol.ADDR_CPU_TBL_DUTY0);
            var tr = ec.Read(FanProtocol.ADDR_GPU_TBL_DUTY0);
            P($"{tag,-16} L={l} R={r} | tblDuty0 L={tl} R={tr} 0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"} 0x787=0x{ec.Read(FanProtocol.ADDR_FAN_USER_SPEED)?.ToString("X2") ?? "??"} 0x7C5=0x{ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE)?.ToString("X2") ?? "??"}");
        }

        byte? s751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
        byte? s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        byte? s7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        var saved = new System.Collections.Generic.Dictionary<ushort, byte[]>();
        for (ushort a = 0xF00; a <= 0xF50; a += 0x10)
        {
            var blk = new byte[16];
            for (int i = 0; i < 16; i++) blk[i] = ec.Read((ushort)(a + i)) ?? 0xEE;
            saved[a] = blk;
        }
        Snap("baseline");

        var cpu = new FanTableModel(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0);
        cpu.SetFlatDuty(100);
        var gpu = new FanTableModel(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0);
        gpu.SetFlatDuty(10);
        P($"表写入 cpu={cpu.WriteToEc(ec)}/48 gpu={gpu.WriteToEc(ec)}/48");
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);

        // P1: Customize 模式 (bit4), 独立位保持
        ec.Write(FanProtocol.ADDR_FAN_MODE, 0x10);
        P("--- P1: 0x751=0x10 ---");
        for (int i = 1; i <= 4; i++) { Thread.Sleep(4000); Snap($"P1+{i * 4}s"); }

        // P2: 反转两表 (CPU=10, GPU=100), 验证方向性
        cpu.SetFlatDuty(10); gpu.SetFlatDuty(100);
        cpu.WriteToEc(ec); gpu.WriteToEc(ec);
        P("--- P2: 表反转 ---");
        for (int i = 1; i <= 4; i++) { Thread.Sleep(4000); Snap($"P2+{i * 4}s"); }

        // 恢复
        foreach (var (a, blk) in saved)
            for (int i = 0; i < 16; i++) ec.Write((ushort)(a + i), blk[i]);
        ec.Write(FanProtocol.ADDR_FAN_MODE, s751 ?? 0x00);
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, s787 ?? 0x00);
        if (s7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, s7c5.Value);
        Thread.Sleep(4000);
        Snap("restored");
        P("FANTEST7 DONE");
        Flush();
    }

    /// <summary>
    /// 直写 duty 假设验证 (来自 watch 抓包): 0x461/0x469 是左右风扇 duty 镜像。
    /// CPU 侧写 200 (100%), GPU 侧写 50 (25%), 观察 0x75B/0x75C 和 RPM 是否分离。
    /// 用法: FanSlider.exe --dutyrw
    /// </summary>
    private static void DutyRwTest()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        const ushort CPU_DUTY_RW = 0x461, GPU_DUTY_RW = 0x469;
        P("DUTYRW: 直写 0x461=200(CPU 100%) / 0x469=50(GPU 25%) 12s, 再反转 12s, 恢复");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 2; return; }

        byte? s461 = ec.Read(CPU_DUTY_RW), s469 = ec.Read(GPU_DUTY_RW);
        byte? s751 = ec.Read(FanProtocol.ADDR_FAN_MODE), s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY); var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var l2 = ec.Read(CPU_DUTY_RW); var r2 = ec.Read(GPU_DUTY_RW);
            var rpm1 = (ec.Read(FanProtocol.ADDR_FAN_RPM_HI) << 8) | ec.Read(FanProtocol.ADDR_FAN_RPM_LO);
            var rpm2 = (ec.Read(FanProtocol.ADDR_FAN2_RPM_HI) << 8) | ec.Read(FanProtocol.ADDR_FAN2_RPM_LO);
            P($"{tag,-14} duty(75B/C)={l}/{r} 461/469={l2}/{r2}  RPM={rpm1}/{rpm2}  0x751=0x{s751:X2} 0x787=0x{s787:X2}");
        }
        Snap("baseline");

        // 阶段A: CPU=100%, GPU=25%
        ec.Write(CPU_DUTY_RW, 200); ec.Write(GPU_DUTY_RW, 50);
        for (int i = 1; i <= 4; i++) { Thread.Sleep(3000); Snap($"A+{i * 3}s"); }
        // 阶段B: 反转
        ec.Write(CPU_DUTY_RW, 50); ec.Write(GPU_DUTY_RW, 200);
        for (int i = 1; i <= 4; i++) { Thread.Sleep(3000); Snap($"B+{i * 3}s"); }

        if (s461 is not null) ec.Write(CPU_DUTY_RW, s461.Value);
        if (s469 is not null) ec.Write(GPU_DUTY_RW, s469.Value);
        Thread.Sleep(4000);
        Snap("restored");
        P("DUTYRW DONE");
        Flush();
    }

    /// <summary>
    /// SMAPC 表探测: 通过驱动私有通道 {0xBB, offset} 读 SMART APC 表, 每次返回 4 字节。
    /// OEM 明文副本 ReadSmartApcTableByDll 的等价实现。
    /// 用法: FanSlider.exe --smapc [偏移上限=256]
    /// </summary>
    private static void SmapcProbe(int maxOffset)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s)
        {
            sb.AppendLine(s);
            try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } // 逐行落盘防卡死丢数据
        }

        P($"SMAPC PROBE: 扫描 0..{maxOffset} (IOCTL 0x9C40A500, in=128B)");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }
        P("OPEN OK");

        // 先单次试探, 带 5s 看门狗思路: 若第一条都回不来就放弃通道
        var t0 = DateTime.Now;
        var first = ec.SmapcExchange(new byte[] { 0xBB, 0x00 });
        P($"first exchange {(first is null ? $"FAIL ({ec.LastError})" : $"OK {(DateTime.Now - t0).TotalMilliseconds:F0}ms: {first[0]:X2} {first[1]:X2} {first[2]:X2} {first[3]:X2}")}");
        if (first is null) { P("SMAPC PROBE ABORT"); return; }

        for (int off = 4; off < maxOffset; off += 4)
        {
            var r = ec.SmapcExchange(new byte[] { 0xBB, (byte)off });
            if (r is null) { P($"offset 0x{off:X2}: FAIL ({ec.LastError})"); break; }
            if (r[0] != 0 || r[1] != 0 || r[2] != 0 || r[3] != 0)
                P($"offset 0x{off:X2}: {r[0]:X2} {r[1]:X2} {r[2]:X2} {r[3]:X2}  ({r[0]},{r[1]},{r[2]},{r[3]})");
        }
        P("SMAPC PROBE DONE");
    }

    /// <summary>只读任意寄存器: FanSlider.exe --read 7C5 787 751 ...</summary>
    private static void ReadRegs(string[] hexAddrs)    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 2; return; }
        foreach (var h in hexAddrs)
        {
            if (!ushort.TryParse(h, System.Globalization.NumberStyles.HexNumber, null, out ushort a)) { P($"bad addr {h}"); continue; }
            var v = ec.Read(a);
            P($"0x{a:X3} = {(v is null ? $"FAIL ({ec.LastError})" : $"0x{v.Value:X2} ({v.Value})")}");
        }
        Flush();
    }

    /// <summary>写任意 EC 寄存器(十六进制): FanSlider.exe --wr 1804 64</summary>
    private static void WriteRegRaw(string hexAddr, string hexVal)
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        if (!ushort.TryParse(hexAddr, System.Globalization.NumberStyles.HexNumber, null, out ushort a) ||
            !byte.TryParse(hexVal, System.Globalization.NumberStyles.HexNumber, null, out byte v))
        {
            P($"bad args: {hexAddr} {hexVal}");
            Flush();
            Environment.ExitCode = 2;
            return;
        }
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 2; return; }
        bool ok = ec.Write(a, v);
        P($"0x{a:X3} <- 0x{v:X2} ({(ok ? "OK" : "FAIL " + ec.LastError)})");
        var back = ec.Read(a);
        P($"readback = {(back is null ? "FAIL" : $"0x{back.Value:X2} ({back.Value})")}");
        Flush();
    }

    /// <summary>
    /// 只读探针: dump EC 表区 0xF00-0xF5F + 身份字节, 用于验证表布局假设。
    /// 用法: FanSlider.exe --tables
    /// </summary>
    private static void DumpTables()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        var sb = new System.Text.StringBuilder();
        void P(string s) => sb.AppendLine(s);
        void Flush() { try { File.AppendAllText(logPath, sb + Environment.NewLine); } catch { } }

        P("DUMP TABLES — read-only 0xF00..0xF5F");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); Flush(); Environment.ExitCode = 2; return; }

        void Row(string name, ushort start, int count)
        {
            var vals = new List<byte>();
            for (int i = 0; i < count; i++)
            {
                var v = ec.Read((ushort)(start + i));
                vals.Add(v ?? 0xEE); // EE 标记读失败
            }
            P($"{name,-14} 0x{start:X3}: {string.Join(' ', vals.Select(b => b.ToString("X2")))}");
        }
        Row("CPU_UP", FanProtocol.ADDR_CPU_TBL_UP0, 16);
        Row("CPU_DOWN", FanProtocol.ADDR_CPU_TBL_DOWN0, 16);
        Row("CPU_DUTY", FanProtocol.ADDR_CPU_TBL_DUTY0, 16);
        Row("GPU_UP", FanProtocol.ADDR_GPU_TBL_UP0, 16);
        Row("GPU_DOWN", FanProtocol.ADDR_GPU_TBL_DOWN0, 16);
        Row("GPU_DUTY", FanProtocol.ADDR_GPU_TBL_DUTY0, 16);
        P($"STATUS1=0x{ec.Read(FanProtocol.ADDR_TBL_STATUS1)?.ToString("X2") ?? "??"} " +
          $"STATUS2=0x{ec.Read(FanProtocol.ADDR_TBL_STATUS2)?.ToString("X2") ?? "??"} " +
          $"CTRL=0x{ec.Read(FanProtocol.ADDR_TBL_CTRL)?.ToString("X2") ?? "??"}");

        // 上下文快照
        P($"ctx: 0x787=0x{ec.Read(FanProtocol.ADDR_FAN_USER_SPEED)?.ToString("X2") ?? "??"} " +
          $"0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"} " +
          $"DUTY L={ec.Read(FanProtocol.ADDR_FAN_L_DUTY)} R={ec.Read(FanProtocol.ADDR_FAN_R_DUTY)}");

        // 身份字节 (判定机型模板)
        P($"id:  PROJECT=0x{ec.Read(FanProtocol.ADDR_PROJECT_ID)?.ToString("X2") ?? "??"} " +
          $"SUP1=0x{ec.Read(FanProtocol.ADDR_SUPPORT_B1)?.ToString("X2") ?? "??"} " +
          $"SUP2=0x{ec.Read(FanProtocol.ADDR_SUPPORT_B2)?.ToString("X2") ?? "??"} " +
          $"SYS=0x{ec.Read(FanProtocol.ADDR_SYSTEM_ID)?.ToString("X2") ?? "??"} " +
          $"ROM=0x{ec.Read(FanProtocol.ADDR_ROMID)?.ToString("X2") ?? "??"}/0x{ec.Read(FanProtocol.ADDR_ROMID2)?.ToString("X2") ?? "??"} " +
          $"MOD=0x{ec.Read(FanProtocol.ADDR_MODULEID)?.ToString("X2") ?? "??"}/0x{ec.Read(FanProtocol.ADDR_MODULEID_GPU)?.ToString("X2") ?? "??"}");
        P("DUMP TABLES DONE");
        Flush();
    }

    /// <summary>
    /// fantest8: MYFAN3 PWM 默认值寄存器假设 (0x786=L1..0x78A=L5, ADDR_L*_PWM_DEFAULT_MYFAN3)。
    /// 写 L1=0x32(50) → 0x787=0x81 (切 L1), 观察 DUTY 是否变 ~100; 再还原。
    /// 用法: FanSlider.exe --fantest8
    /// </summary>
    private static void FanTest8()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }

        P("FANTEST8: 0x786(L1_PWM)=0x32 → 0x787=0x81(L1), 观察 DUTY");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        var s786 = ec.Read(0x786); var s787 = ec.Read(0x787);
        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            P($"{tag,-16} DUTY L={l} R={r}  0x786=0x{ec.Read(0x786)?.ToString("X2") ?? "??"} 0x787=0x{ec.Read(0x787)?.ToString("X2") ?? "??"} 0x78C=0x{ec.Read(0x78C)?.ToString("X2") ?? "??"}");
        }
        Snap("baseline");

        ec.Write(0x786, 0x32);           // L1 PWM 默认 = 50 (0-200 标度=25%)
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x81); // 切到 L1
        for (int i = 1; i <= 4; i++) { Thread.Sleep(3000); Snap($"L1+{i * 3}s"); }

        // 试改 0x78C (GPU 设置)
        var s78c = ec.Read(0x78C);
        ec.Write(0x78C, 0x20);
        P("--- 0x78C → 0x20 (GPU 32) ---");
        Thread.Sleep(4000); Snap("78C+4s");
        if (s78c is not null) ec.Write(0x78C, s78c.Value);

        // 还原
        if (s786 is not null) ec.Write(0x786, s786.Value);
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        Thread.Sleep(4000);
        Snap("restored");
        P("FANTEST8 DONE");
    }

    /// <summary>
    /// fantest9 (决定性): 无 AP 干预窗口。流程:
    /// 1) net stop GCUBridge (服务停掉, GCUService 退出)
    /// 2) 写表 CPU=100/GPU=10 + 0x751=0x00 自动档, 观察 60s: L→200/R→20?
    /// 3) 若表无效: 写 0x787=0x85 档位对比
    /// 4) net start GCUBridge 恢复
    /// 需管理员。用法: FanSlider.exe --fantest9
    /// </summary>
    private static void FanTest9()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }

        P("FANTEST9: 停 GCUBridge → 写表 CPU=100/GPU=10 → 自动档 60s → 恢复");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            P($"{tag,-18} DUTY L={l} R={r}  0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"} 0x787=0x{ec.Read(FanProtocol.ADDR_FAN_USER_SPEED)?.ToString("X2") ?? "??"} 0x7C5=0x{ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE)?.ToString("X2") ?? "??"}");
        }

        // 1) 停服务 (SCM)
        P(RunSc("stop", "GCUBridge"));
        Thread.Sleep(3000);

        try
        {
            byte? s751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
            byte? s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
            byte? s7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            var saved = new System.Collections.Generic.Dictionary<ushort, byte[]>();
            for (ushort a = 0xF00; a <= 0xF50; a += 0x10)
            {
                var blk = new byte[16];
                for (int i = 0; i < 16; i++) blk[i] = ec.Read((ushort)(a + i)) ?? 0xEE;
                saved[a] = blk;
            }
            Snap("baseline");

            var cpu = new FanTableModel(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0);
            cpu.SetFlatDuty(100);
            var gpu = new FanTableModel(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0);
            gpu.SetFlatDuty(10);
            P($"表写入 cpu={cpu.WriteToEc(ec)}/48 gpu={gpu.WriteToEc(ec)}/48");

            if (s7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(s7c5.Value | 0x80));
            ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
            ec.Write(FanProtocol.ADDR_FAN_MODE, 0x00);
            P("--- 无服务干扰, 自动档+表 ---");
            for (int i = 1; i <= 6; i++) { Thread.Sleep(5000); Snap($"T+{i * 5}s"); }

            P("--- 对照: 0x787=0x85 (L5) ---");
            ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x85);
            Thread.Sleep(5000); Snap("L5+5s");

            // 恢复
            foreach (var (a, blk) in saved)
                for (int i = 0; i < 16; i++) ec.Write((ushort)(a + i), blk[i]);
            ec.Write(FanProtocol.ADDR_FAN_MODE, s751 ?? 0x00);
            ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, s787 ?? 0x00);
            if (s7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, s7c5.Value);
            Thread.Sleep(3000);
            Snap("restored");
        }
        finally
        {
            P(RunSc("start", "GCUBridge"));
            Thread.Sleep(2000);
        }
        P("FANTEST9 DONE");
    }

    private static string RunSc(string action, string svc)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo("sc.exe", $"{action} {svc}")
            {
                CreateNoWindow = true, UseShellExecute = false,
                RedirectStandardOutput = true, RedirectStandardError = true,
            };
            using var p = System.Diagnostics.Process.Start(psi)!;
            string o = p.StandardOutput.ReadToEnd().Trim();
            p.WaitForExit(10000);
            return $"sc {action} {svc}: {o.Split('\n').FirstOrDefault()?.Trim()}";
        }
        catch (Exception ex) { return $"sc {action} 失败: {ex.Message}"; }
    }

    /// <summary>
    /// fantest10 (最终模型验证): 停服务 → 0x461(CPU上限)=0x50(80), 0x469(GPU上限)=0xC8(200)
    /// → 观察两 DUTY 分离 (L≤80, R 逼近温度需求) → 还原 → 启服务。
    /// 用法: FanSlider.exe --fantest10
    /// </summary>
    private static void FanTest10()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }

        P("FANT10: 停服务; 0x461=0x50(CPU上限80) 0x469=0xC8(GPU上限200)");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            P($"{tag,-16} DUTY L={l} R={r}  0x461=0x{ec.Read(0x461)?.ToString("X2") ?? "??"} 0x469=0x{ec.Read(0x469)?.ToString("X2") ?? "??"}");
        }

        P(RunSc("stop", "GCUBridge"));
        Thread.Sleep(3000);
        try
        {
            var s461 = ec.Read(0x461); var s469 = ec.Read(0x469);
            Snap("baseline");

            ec.Write(0x461, 0x50);   // CPU 上限 80 (40%)
            ec.Write(0x469, 0xC8);   // GPU 上限 200 (100%)
            P("--- CPU=80 / GPU=200 ---");
            for (int i = 1; i <= 4; i++) { Thread.Sleep(4000); Snap($"P1+{i * 4}s"); }

            ec.Write(0x461, 0xC8);   // 反转
            ec.Write(0x469, 0x32);   // GPU 上限 50
            P("--- 反转: CPU=200 / GPU=50 ---");
            for (int i = 1; i <= 4; i++) { Thread.Sleep(4000); Snap($"P2+{i * 4}s"); }

            if (s461 is not null) ec.Write(0x461, s461.Value);
            if (s469 is not null) ec.Write(0x469, s469.Value);
            Thread.Sleep(3000);
            Snap("restored");
        }
        finally
        {
            P(RunSc("start", "GCUBridge"));
            Thread.Sleep(2000);
        }
        P("FANT10 DONE");
    }

    /// <summary>
    /// fantest11 (SetFanMode 序列复刻): 表 CPU=100/GPU=10 → 0x751=0x10 →
    /// 复刻 OEM 顺序写 0xEF3(CPU表生效?) / 0xEF8(GPU表生效?) / 0xF08 / 0xF0D。
    /// 用法: FanSlider.exe --fantest11
    /// </summary>
    private static void FanTest11()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }

        P("FANT11: 表 CPU=100/GPU=10 → 0x751=0x10 → 0xEF3/0xEF8/0xF08/0xF0D 触发");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            P($"{tag,-18} DUTY L={l} R={r}  0x751=0x{ec.Read(FanProtocol.ADDR_FAN_MODE)?.ToString("X2") ?? "??"} 0x787=0x{ec.Read(FanProtocol.ADDR_FAN_USER_SPEED)?.ToString("X2") ?? "??"}");
        }

        byte? s751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
        byte? s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        var saved = new System.Collections.Generic.Dictionary<ushort, byte[]>();
        for (ushort a = 0xEF0; a <= 0xF50; a += 0x10)
        {
            var blk = new byte[16];
            for (int i = 0; i < 16; i++) blk[i] = ec.Read((ushort)(a + i)) ?? 0xEE;
            saved[a] = blk;
        }
        // 记录 0xEF0-0xF2F 原始值 (触发改前)
        P("before: " + string.Join(' ', Enumerable.Range(0xEF0, 0x30).Select(a => $"{a:X3}:{ec.Read((ushort)a)?.ToString("X2") ?? "??"}")));

        var cpu = new FanTableModel(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0);
        cpu.SetFlatDuty(100);
        var gpu = new FanTableModel(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0);
        gpu.SetFlatDuty(10);
        P($"表写入 cpu={cpu.WriteToEc(ec)}/48 gpu={gpu.WriteToEc(ec)}/48");

        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        ec.Write(FanProtocol.ADDR_FAN_MODE, 0x10);
        Snap("mode=0x10");

        // OEM SetFanMode 序列: 以 1/0 为参数写 0xEF3, 0xEF8 (来自 JIT: r9d=0xef3 / 0xef8 + LogCtrl)
        ec.Write(0xEF3, 0x01);
        ec.Write(0xEF8, 0x01);
        P("--- 0xEF3=1, 0xEF8=1 ---");
        for (int i = 1; i <= 4; i++) { Thread.Sleep(4000); Snap($"E+{i * 4}s"); }

        // 反转表, 再触发
        cpu.SetFlatDuty(10); gpu.SetFlatDuty(100);
        cpu.WriteToEc(ec); gpu.WriteToEc(ec);
        ec.Write(0xEF3, 0x00);
        ec.Write(0xEF8, 0x00);
        Thread.Sleep(500); 
        ec.Write(0xEF3, 0x01);
        ec.Write(0xEF8, 0x01);
        P("--- 表反转 + 0xEF3/EF8 脉冲 ---");
        for (int i = 1; i <= 4; i++) { Thread.Sleep(4000); Snap($"F+{i * 4}s"); }

        P("after:  " + string.Join(' ', Enumerable.Range(0xEF0, 0x30).Select(a => $"{a:X3}:{ec.Read((ushort)a)?.ToString("X2") ?? "??"}")));

        // 恢复
        foreach (var (a, blk) in saved)
            for (int i = 0; i < 16; i++) ec.Write((ushort)(a + i), blk[i]);
        ec.Write(FanProtocol.ADDR_FAN_MODE, s751 ?? 0x00);
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, s787 ?? 0x00);
        Thread.Sleep(3000);
        Snap("restored");
        P("FANT11 DONE");
    }

    /// <summary>
    /// fantest12 (映射定位): 只写 CPU 表=200 (GPU 表原样), 0xEF3=1,0xEF8=0
    /// → 谁动? 再只写 GPU 表=20, 0xEF3=0,0xEF8=1 → 谁动?
    /// 用法: FanSlider.exe --fantest12
    /// </summary>
    private static void FanTest12()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }

        P("FANT12: 单表激活映射 (只 CPU表200/EF3=1 → 只 GPU表20/EF8=1)");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            P($"{tag,-18} DUTY L={l} R={r}");
        }

        byte? s751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
        byte? s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        var saved = new System.Collections.Generic.Dictionary<ushort, byte[]>();
        for (ushort a = 0xEF0; a <= 0xF50; a += 0x10)
        {
            var blk = new byte[16];
            for (int i = 0; i < 16; i++) blk[i] = ec.Read((ushort)(a + i)) ?? 0xEE;
            saved[a] = blk;
        }
        Snap("baseline");

        var cpu = new FanTableModel(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0);
        cpu.SetFlatDuty(100);
        var gpu = new FanTableModel(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0);
        gpu.SetFlatDuty(10);

        // Phase1: 只写 CPU 表 (GPU 表写回原值), 0xEF3=1 0xEF8=0
        cpu.WriteToEc(ec);
        // 还原 GPU 表
        var g0 = saved[0xF30]; var g1 = saved[0xF40]; var g2 = saved[0xF50];
        for (int i = 0; i < 16; i++)
        { ec.Write((ushort)(0xF30 + i), g0[i]); ec.Write((ushort)(0xF40 + i), g1[i]); ec.Write((ushort)(0xF50 + i), g2[i]); }
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        ec.Write(FanProtocol.ADDR_FAN_MODE, 0x10);
        ec.Write(0xEF3, 0x01);
        ec.Write(0xEF8, 0x00);
        P("--- P1: 仅CPU表=200, EF3=1, EF8=0 ---");
        for (int i = 1; i <= 4; i++) { Thread.Sleep(4000); Snap($"P1+{i * 4}s"); }

        // Phase2: 只写 GPU 表=20, EF3=0 EF8=1
        gpu.SetFlatDuty(10); gpu.WriteToEc(ec);
        var c0 = saved[0xF00]; var c1 = saved[0xF10]; var c2 = saved[0xF20];
        for (int i = 0; i < 16; i++)
        { ec.Write((ushort)(0xF00 + i), c0[i]); ec.Write((ushort)(0xF10 + i), c1[i]); ec.Write((ushort)(0xF20 + i), c2[i]); }
        ec.Write(0xEF3, 0x00);
        ec.Write(0xEF8, 0x01);
        P("--- P2: 仅GPU表=20, EF3=0, EF8=1 ---");
        for (int i = 1; i <= 4; i++) { Thread.Sleep(4000); Snap($"P2+{i * 4}s"); }

        foreach (var (a, blk) in saved)
            for (int i = 0; i < 16; i++) ec.Write((ushort)(a + i), blk[i]);
        ec.Write(FanProtocol.ADDR_FAN_MODE, s751 ?? 0x00);
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, s787 ?? 0x00);
        Thread.Sleep(3000);
        Snap("restored");
        P("FANT12 DONE");
    }

    /// <summary>
    /// fantest13 (分风扇终局判定): 两表写差异曲线 (CPU=200 恒定, GPU=40 恒定),
    /// EF3=1+EF8=1, 然后人为制造 GPU 侧温度差 (GPU 压力) 观察 R 是否独立跟 GPU 表。
    /// 简化: 静置观察 L/R 是否随各自温度独立变化 (CPU 冷/GPU 热时 L 应低 R 应高)。
    /// 用法: FanSlider.exe --fantest13
    /// </summary>
    private static void FanTest13()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }

        P("FANT13: CPU表=200/10两段, GPU表=40恒定; EF3+EF8; 静置60s观察 L/R 独立性");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var t1 = ec.Read(0x449);  // 温度源A (watch 高频=某温度)
            var t2 = ec.Read(0x44C);  // 温度源B
            P($"{tag,-18} DUTY L={l} R={r}  T_A=0x{t1?.ToString("X2") ?? "??"}({t1}) T_B=0x{t2?.ToString("X2") ?? "??"}({t2})");
        }

        byte? s751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
        byte? s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        var saved = new System.Collections.Generic.Dictionary<ushort, byte[]>();
        for (ushort a = 0xEF0; a <= 0xF50; a += 0x10)
        {
            var blk = new byte[16];
            for (int i = 0; i < 16; i++) blk[i] = ec.Read((ushort)(a + i)) ?? 0xEE;
            saved[a] = blk;
        }
        Snap("baseline");

        // CPU 表: 低温段 10%, 65℃以上 100% (让 CPU 热时冲高)
        var cpu = new FanTableModel(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0);
        for (int i = 0; i < 16; i++)
        {
            cpu.UpT[i] = (byte)Math.Min(255, 40 + i * 4);
            cpu.DownT[i] = (byte)Math.Max(0, cpu.UpT[i] - 3);
            cpu.DutyPct[i] = cpu.UpT[i] < 65 ? (byte)10 : (byte)100;
        }
        cpu.ActivePoints = 16;
        cpu.WriteToEc(ec);

        // GPU 表: 恒定 40%
        var gpu = new FanTableModel(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0);
        gpu.SetFlatDuty(40);
        gpu.WriteToEc(ec);

        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        ec.Write(FanProtocol.ADDR_FAN_MODE, 0x10);
        ec.Write(0xEF3, 0x01);
        ec.Write(0xEF8, 0x01);
        P("--- CPU阶梯表 + GPU恒定40% ---");
        for (int i = 1; i <= 12; i++) { Thread.Sleep(5000); Snap($"T+{i * 5}s"); }

        foreach (var (a, blk) in saved)
            for (int i = 0; i < 16; i++) ec.Write((ushort)(a + i), blk[i]);
        ec.Write(FanProtocol.ADDR_FAN_MODE, s751 ?? 0x00);
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, s787 ?? 0x00);
        Thread.Sleep(3000);
        Snap("restored");
        P("FANT13 DONE");
    }

    /// <summary>
    /// fantest14 (最终组合): 0x7C5 bit7=1 (独立输出ON) + 0x751=0x10 + EF3=1/EF8=1
    /// + 差异表 (CPU恒定200, GPU恒定40)。若 L/R 仍同步, 则此 EC 双风扇无独立控制。
    /// 用法: FanSlider.exe --fantest14
    /// </summary>
    private static void FanTest14()
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "selftest.log");
        void P(string s) { try { File.AppendAllText(logPath, s + Environment.NewLine); } catch { } }

        P("FANT14: 独立位ON + 0x751=0x10 + EF3/EF8=1 + CPU表200/GPU表40");
        using var ec = new EcDriver();
        if (!ec.Open()) { P($"OPEN FAIL: {ec.LastError}"); return; }

        void Snap(string tag)
        {
            var l = ec.Read(FanProtocol.ADDR_FAN_L_DUTY);
            var r = ec.Read(FanProtocol.ADDR_FAN_R_DUTY);
            var c = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
            P($"{tag,-18} DUTY L={l} R={r}  0x7C5=0x{c?.ToString("X2") ?? "??"}");
        }

        byte? s751 = ec.Read(FanProtocol.ADDR_FAN_MODE);
        byte? s787 = ec.Read(FanProtocol.ADDR_FAN_USER_SPEED);
        byte? s7c5 = ec.Read(FanProtocol.ADDR_FAN_RESPECTIVE);
        var saved = new System.Collections.Generic.Dictionary<ushort, byte[]>();
        for (ushort a = 0xEF0; a <= 0xF50; a += 0x10)
        {
            var blk = new byte[16];
            for (int i = 0; i < 16; i++) blk[i] = ec.Read((ushort)(a + i)) ?? 0xEE;
            saved[a] = blk;
        }
        Snap("baseline");

        var cpu = new FanTableModel(FanProtocol.ADDR_CPU_TBL_UP0, FanProtocol.ADDR_CPU_TBL_DOWN0, FanProtocol.ADDR_CPU_TBL_DUTY0);
        cpu.SetFlatDuty(100);
        cpu.WriteToEc(ec);
        var gpu = new FanTableModel(FanProtocol.ADDR_GPU_TBL_UP0, FanProtocol.ADDR_GPU_TBL_DOWN0, FanProtocol.ADDR_GPU_TBL_DUTY0);
        gpu.SetFlatDuty(20);
        gpu.WriteToEc(ec);

        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, 0x00);
        if (s7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, (byte)(s7c5.Value | 0x80));
        ec.Write(FanProtocol.ADDR_FAN_MODE, 0x10);
        ec.Write(0xEF3, 0x01);
        ec.Write(0xEF8, 0x01);
        P("--- all ON ---");
        for (int i = 1; i <= 10; i++) { Thread.Sleep(5000); Snap($"T+{i * 5}s"); }

        foreach (var (a, blk) in saved)
            for (int i = 0; i < 16; i++) ec.Write((ushort)(a + i), blk[i]);
        ec.Write(FanProtocol.ADDR_FAN_MODE, s751 ?? 0x00);
        ec.Write(FanProtocol.ADDR_FAN_USER_SPEED, s787 ?? 0x00);
        if (s7c5 is not null) ec.Write(FanProtocol.ADDR_FAN_RESPECTIVE, s7c5.Value);
        Thread.Sleep(3000);
        Snap("restored");
        P("FANT14 DONE");
    }
}
