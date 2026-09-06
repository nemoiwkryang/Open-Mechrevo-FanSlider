namespace FanSlider;

/// <summary>
/// EC register map reverse-engineered from the OEM stack (GCUService.exe,
/// Define/ECSpec.cs + runtime JIT disassembly of MyFanManager_RamFan1p5).
/// </summary>
public static class FanProtocol
{
    /// <summary>主风扇模式控制字节 (ADDR_MAFAN_CONTROL_BYTE = 1873).</summary>
    public const ushort ADDR_FAN_MODE = 0x751;

    /// <summary>手动风扇档位寄存器, 100ms 步进爬升 (ADDR_TimAP_FanSwitchSpeedT100mSec = 1927).</summary>
    public const ushort ADDR_FAN_USER_SPEED = 0x787;

    /// <summary>左/CPU 风扇当前占空比读数 (ADDR_EC_MAIN_FAN_L_DUTY_BYTE = 1883).</summary>
    public const ushort ADDR_FAN_L_DUTY = 0x75B;

    /// <summary>右/GPU 风扇当前占空比读数 (ADDR_EC_MAIN_FAN_R_DUTY_BYTE = 1884).</summary>
    public const ushort ADDR_FAN_R_DUTY = 0x75C;

    /// <summary>主风扇 RPM 高字节 (ADDR_EC_MAIN_FAN_RPM_BYTE1 = 1124) — 换算系数未知, 仅显示原始值.</summary>
    public const ushort ADDR_FAN_RPM_HI = 0x464;
    public const ushort ADDR_FAN_RPM_LO = 0x465;
    public const ushort ADDR_FAN2_RPM_HI = 0x46C;  // 1132
    public const ushort ADDR_FAN2_RPM_LO = 0x46B;  // 1131

    public static int? ReadRpm(EcDriver ec, bool secondary)
    {
        var hi = ec.Read(secondary ? ADDR_FAN2_RPM_HI : ADDR_FAN_RPM_HI);
        var lo = ec.Read(secondary ? ADDR_FAN2_RPM_LO : ADDR_FAN_RPM_LO);
        if (hi is null || lo is null) return null;
        return (hi.Value << 8) | lo.Value;
    }

    public static int NormalizeDuty(byte raw0to200) => Math.Clamp(raw0to200 / DUTY_SCALE, 0, 100);

    // ---- RamFan1p5 每风扇曲线表区 (EC RAM, 16 点 × 字节) ----
    public const ushort ADDR_CPU_TBL_UP0    = 0xF00;  // 3840 温度上限
    public const ushort ADDR_CPU_TBL_DOWN0  = 0xF10;  // 3856 温度下限(滞回)
    public const ushort ADDR_CPU_TBL_DUTY0  = 0xF20;  // 3872 占空比
    public const ushort ADDR_GPU_TBL_UP0    = 0xF30;  // 3888
    public const ushort ADDR_GPU_TBL_DOWN0  = 0xF40;  // 3904
    public const ushort ADDR_GPU_TBL_DUTY0  = 0xF50;  // 3920
    public const ushort ADDR_TBL_STATUS1    = 0xF5D;  // 3933
    public const ushort ADDR_TBL_STATUS2    = 0xF5E;  // 3934
    public const ushort ADDR_TBL_CTRL       = 0xF5F;  // 3935 提交控制
    public const int    TBL_POINTS          = 16;

    // ---- 身份/支持字节 (只读, 用于判定机型模板) ----
    public const ushort ADDR_PROJECT_ID   = 0x740;  // 1856
    public const ushort ADDR_SUPPORT_B1   = 0x765;  // 1893
    public const ushort ADDR_SUPPORT_B2   = 0x766;  // 1894
    public const ushort ADDR_SYSTEM_ID    = 0x456;  // 1110
    public const ushort ADDR_ROMID        = 0x771;  // 1905
    public const ushort ADDR_ROMID2       = 0x772;  // 1906
    public const ushort ADDR_MODULEID     = 0x7D3;  // 2003
    public const ushort ADDR_MODULEID_GPU = 0x7D2;  // 2002

    // ---- 每风扇独立输出开关 (OEM: SetEcFanControlRespective, JIT 机器码实证) ----
    // 读 0x7C5, 保留低 7 位; bit7=1 表示"风扇独立输出"启用
    public const ushort ADDR_FAN_RESPECTIVE = 0x7C5;   // 1989 (ADDR_AP_OEM_BYTE5)
    public const byte   RESPECTIVE_BIT      = 0x80;
    public static byte SetRespective(byte cur, bool on) => on
        ? (byte)(cur | RESPECTIVE_BIT)
        : (byte)(cur & 0x7F);

    /// <summary>自定义表模式标志 (0x726, 十进制 1830)。custom 模式必须置 bit7, 其他模式清。</summary>
    public const ushort ADDR_CUSTOM_FLAG = 0x726;
    public const byte   CUSTOM_FLAG_BIT  = 0x80;

    // ---- 分风扇控制触发序列 (由 CDB 捕获的 OEM 实际写入确认) ----
    // OEM 修改/应用曲线时: 0x7C6 = 0x04 -> 0x00, 随后写 0x7C5 的独立输出位,
    // 再按每点 Up[i], Down[i+1], Duty[i] 写入两张表。EF3/EF8 并未出现在 OEM 写序列中，
    // 不作为正式控制协议使用。
    // v8 独立接管把 0x7C6 bit2 作为 ENABLE_UNIVERSAL_FAN_CTRL 门 (固件 img 0x9770:
    // bit2=0 → 跳过表驱动路径回退内部默认曲线)。OEM 栈存活时该位由 GCUService 预置,
    // 脱离 OEM 栈后必须由我们读改写置位。
    public const ushort ADDR_FAN_CONTROL_TRIGGER = 0x7C6; // ADDR_AP_OEM_BYTE6
    public const byte FAN_CONTROL_TRIGGER = 0x04;
    public const byte FAN_CONTROL_IDLE = 0x00;
    public const byte ENABLE_UNIVERSAL_FAN_CTRL_BIT = 0x04;
    public static byte SetUniversalFanCtrl(byte cur, bool on) => on
        ? (byte)(cur | ENABLE_UNIVERSAL_FAN_CTRL_BIT)
        : (byte)(cur & ~ENABLE_UNIVERSAL_FAN_CTRL_BIT);

    // ---- AP 存在位 (GCUService Set_APExistToEC, App.cs:1041/1020): 启动置 bit0, 退出清 ----
    // 镜像 OEM 行为, 让固件认为"主机控制中心在线" (脱离 OEM 栈时我们自己当主机)。
    public const ushort ADDR_AP_EXIST = 0x741;   // ADDR_FAN_ALERT_BYTE
    public const byte   AP_EXIST_BIT = 0x01;

    // ---- Q-key/默认模式位域 (只快照还原, 不主动修改) ----
    public const ushort ADDR_QKEY_MODE = 0x782;

    // ---- AP Customer Mode 灯 (0x727 bit6, SetAPCustomerModeLightOn) ----
    // 2026-09-06 真机实证 (重启后排查): 这是 RAM 表模式的**隐藏解锁位**。
    // 冷启动后 0x727=0x00 时, 即使 0x7C6/0x7C5/0x726 全武装 + 表写入, 固件仍走内部曲线;
    // 置 0x727 bit6 后 v8 直写接管立即收敛 (官方链 OPERATING_CUSTOM_MODE 正是经它解锁)。
    public const ushort ADDR_CUSTOMER_MODE_LIGHT = 0x727;
    public const byte   CUSTOMER_MODE_LIGHT_BIT  = 0x40;

    // ---- 温度源 (待阶段2 Test D 标定; 候选: README 声称 0x1097/0x1103, 固件内部 0x043E/0x044F) ----
    public const ushort ADDR_TEMP_CPU = 0x1097;   // 待标定
    public const ushort ADDR_TEMP_GPU = 0x1103;   // 待标定
    public const uint   IOCTL_GPD_ACPI_TMPREAD_CPU  = 2621482192; // 复用 EcDriver 常量
    public const uint   IOCTL_GPD_ACPI_TMPREAD_GPU  = 2621482196;

    // 兼容历史诊断命令，正式控制路径不使用这两个地址。
    [Obsolete("未出现在 OEM 曲线写入序列，仅保留历史诊断命令兼容性")]
    public const ushort ADDR_TBL_EN_CPU = 0xEF3;
    [Obsolete("未出现在 OEM 曲线写入序列，仅保留历史诊断命令兼容性")]
    public const ushort ADDR_TBL_EN_GPU = 0xEF8;

    // OEM 自定义曲线期间 0x751/0x787 通常保持 0x00；不强制修改 0x751。
    public const byte FAN_MODE_CUSTOMIZE_TBL = 0x00;

    // ---- Duty 标度 (机器码 shl 1 实证): EC 字节 = 百分比 × 2, 0..200 (0xC8=满速) ----
    public const int DUTY_SCALE = 2;
    public const int DUTY_MAX   = 200;
    public const int DUTY_SAFETY_FLOOR = 0xC8; // 表尾部填充项 = 100% 安全设计, 不要动

    // GPU 25% 最低转速地板 (真机实测: 目标 20% → 落点 25%, raw 0x32, RPM 1621)
    public const byte GPU_DUTY_FLOOR = 0x32; // 25% × 2

    // ADDR_FAN_USER_SPEED 位定义 (MyFanCTLByteFlag):
    //   0x00      = 交还 EC 自动策略
    //   0x80      = User_Fan_Mode 位 (手动接管), 无档位
    //   0x81..85  = 手动档位 1..5 (5 = 该寄存器下的最高档, 约 DUTY 100/200)
    public const byte USER_MODE_BIT = 0x80;

    // ---- 真·满速 (实测 2026-09: DUTY 200/200, RPM ~5400, 是 L5 的两倍) ----
    // 0x751 的 bit6 = FanBoost 位. EC 会自动把它与当前模式字节做位或合并
    // (写 0x40 → 读回 0xA0|0x40 = 0xE0), 所以直接 OR 进现有值最安全。
    public const byte FAN_MODE_BOOST_BIT = 0x40;
    // 0x751=0x10 为 Turbo 位 (实测仅小幅提速 DUTY 91/200)
    public const byte FAN_MODE_TURBO_BIT = 0x10;

    public static bool IsBoosted(byte v751) => (v751 & FAN_MODE_BOOST_BIT) != 0;
    public static byte SetBoost(byte v751, bool on) => on
        ? (byte)(v751 | FAN_MODE_BOOST_BIT)
        : (byte)(v751 & ~FAN_MODE_BOOST_BIT);

    public static byte EncodeLevel(int level) => level switch
    {
        <= 0 => 0x00,
        >= 5 => (byte)(USER_MODE_BIT | 5),
        _    => (byte)(USER_MODE_BIT | (byte)level),
    };

    public static int DecodeLevel(byte v) =>
        (v & USER_MODE_BIT) != 0 ? Math.Clamp(v & 0x0F, 1, 5) : 0;

    public static string DescribeModeByte(byte v) => v switch
    {
        0x00 => "Normal (EC 自动)",
        0x10 => "Turbo",
        0x40 => "FanBoost",
        0x80 => "用户接管, 档位=0",
        0x81 => "用户档位 1",
        0x82 => "用户档位 2",
        0x83 => "用户档位 3",
        0x84 => "用户档位 4",
        0x85 => "用户档位 5 (满速)",
        0xA0 => "User HiMode",
        _    => $"0x{v:X2}",
    };
}
