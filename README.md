# FanSlider — 机械革命 (Uniwill EC) 双风扇独立控制

一个不依赖官方控制中心、直接读写 EC 的双风扇独立控制工具。
通过 `UWACPIDriver.sys`（设备 `ACPI\INOU0000`，接口 `\\.\ACPIDriver`）直写 EC RAM 曲线表，
由 EC 固件自主按温度插值闭环控制风扇。

- **双风扇独立控制**：CPU / GPU 各自 0–100% 定速，或 16 温度点逐点曲线（T0 锁定 0）
- **零 OEM 依赖**：不需要 GCUBridge / GCUService / 官方 UWP，可完全脱离运行
- **崩溃可恢复**：接管租约落盘，进程被杀后下次启动按快照还原 EC
- **自动重接管**：门控被外部清除时自动重新武装
- 官方模式切换（性能模式 / FanBoost / 手动档位）亦支持

## 兼容性

- 机械革命 / Uniwill 平台（本项目在 Mechrevo PH4 系列真机验证）
- EC 固件 ITE EC-V14.6（RamFan 1.5 温控协议）
- Windows 10/11，需管理员权限（`app.manifest` 已声明 `requireAdministrator`）
- 需 `UWACPIDriver.sys` 已安装并加载（机械革命控制中心安装包自带）

> ⚠️ **风险声明**：本项目通过逆向工程实现，直接操作 EC 寄存器。
> 仅供学习和个人使用，请自行承担风险。风扇控制不当可能导致过热损坏硬件。

## 构建

```text
dotnet build -c Release
```

产物：`bin\Release\net8.0-windows\FanSlider.exe`（需要 .NET 8 SDK / 运行时）。

## 使用

```text
FanSlider.exe   （双击，UAC 提权）
```

- 右上「接管控制权」：快照 + 武装门控 + 三区写表（约 2s），接管租约落盘 `%APPDATA%\FanSlider\lease.json`
- CPU/GPU「整体定速」滑条：全部 16 温度点同值
- 「逐点曲线」编辑器：拖动圆点逐点调整（T0 锁定 0），均匀时自动回同步滑条
- 「应用滑条值」：写表区 + 等待实时 duty 收敛（≤30s）
- 「恢复自动并交还」：按接管前快照还原全表区与门控
- 系统托盘常驻；关机/注销时按「退出时自动交还」还原

### CLI

```text
FanSlider.exe --profiletest 50 25   # 端到端回归（接管→收敛→还原），哨兵值需 ≥20%
FanSlider.exe --restoredefault 2    # 0xF5F 握手重装固件内置默认表 (1=Turbo 2=Gaming 3=Office)
FanSlider.exe --read 751 7C5 75B 75C
FanSlider.exe --wr 1804 46          # 写任意 EC 寄存器
FanSlider.exe --tables              # 导出风扇表区
FanSlider.exe --set 5 / --boost on  # 手动档位 / 真·满速
```

结果写 `bin\Release\net8.0-windows\selftest.log`。

## 工作原理（EC 风扇协议速览）

| 寄存器 | 含义 |
|---|---|
| `0x751` | 模式字节：gaming 0x00 / turbo 0x10 / office 0xA0；bit6 FanBoost |
| `0x726` | bit7 = custom 表模式标志 |
| `0x7C5` | bit7 = respective 门控（CPU/GPU 独立输出） |
| `0x7C6` | bit2 = ENABLE_UNIVERSAL_FAN_CTRL（表驱动总门） |
| `0x741` | bit0 = AP 存在（镜像 OEM 行为） |
| `0x727` | bit6 = AP Customer Mode —— **RAM 表模式隐藏解锁位**，冷启动后必须置位，否则固件走内部曲线 |
| `0xF00-F2F` / `0xF30-F5F` | CPU / GPU 曲线表（UpT/DownT/Duty ×16，Duty 存储 = 百分比 ×2） |
| `0x75B/0x75C` | 实时占空比（raw = %×2） |
| `0x43E/0x44F` | CPU / GPU 温度 |

接管序列（EC 固件自主闭环，无需周期回写）：

```
0x751←0x00 → 0x726|=0x80 → 0x7C5|=0x80(回读验证) → 0x7C6|=0x04 → 0x741|=0x01 → 0x727|=0x40
→ 三区交错写表 (Up[i]/Down[i+1]/Duty[i]=Ti×2, T0=0, ≥10ms/字节) → 回读验证
```

关键点（真机实测结论）：

- **T0（最低温度点 duty）必须为 0** —— 固件把 `0xF20/0xF50` 的 T0 当"表已启用"标记，非零则回退内部曲线
- **0x727 bit6 是隐藏解锁位** —— 冷启动后为 0，其余门控全武装表路径也不生效；官方链的 custom 模式进入（`CustomerModeLightOn`）正是置此位
- 固件无主机心跳看门狗：主机停写后按最后一次表内容持续驱动，直到被覆盖或断电
- EC RAM 表区**不跨重启保持**（重启清零），开机后需重新接管（可配合开机自启）

## 已知限制

- EC 按温度插值（含滞回+斜坡），实时 duty 通常略偏离档位值；平直曲线例外
- GPU 有约 25% 最低转速地板（RPM ~1621），设更低不生效
- `0x1804/0x1809`（内核 WMI-only 手动 duty）在此 EC 上直写无效（固件覆写）
- 官方栈若同时运行，可能周期性重申模式抢占控制权（应用会检测并自动重接管）
- 接管时不要同时在官方 UWP 里保存风扇曲线

## 项目结构

```text
FanSlider.csproj      # .NET 8 WinForms
EcDriver.cs           # \\.\ACPIDriver 封装 (ECREAD/ECWRITE/SMAPC/TMP ioctl)
FanProtocol.cs        # EC 寄存器表 + 位操作
FanTableModel.cs      # 16 点曲线模型 + OEM 47 次/表写序
EcCurveSession.cs     # 核心: 快照→武装→写表→验证→还原 + 接管租约
MainForm.cs           # 主界面 (滑条/曲线编辑器/状态)
UiControls.cs         # 自定义控件 (曲线编辑器/滑条/胶囊开关)
Program.cs            # CLI 诊断与回归测试入口
```

## 协议逆向来源

本项目基于对机械革命控制中心（GCUService / GCUBridge / UWP）与 EC 固件（ITE EC-V14.6）
的完整逆向：H 盘 GCUService_final5 可读源码、UWP 伪 C、MQTT 协议、bank-aware 8051 反汇编。
协议细节与实验记录见项目文档目录。
