using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace FanSlider;

/// <summary>
/// Raw access to the Uniwill EC via the signed UWACPIDriver (device \\.\ACPIDriver,
/// bound to ACPI\INOU0000). Protocol copied from the OEM stack
/// (GCUService / OSDTpDetect's MyECIO.AcpiCtrl, verified byte-for-byte from
/// runtime-disassembled JIT code).
/// </summary>
public sealed class EcDriver : IDisposable
{
    public const uint GENERIC_READ = 0x80000000;
    public const uint GENERIC_WRITE = 0x40000000;
    public const uint FILE_SHARE_RW = 3;
    public const uint OPEN_EXISTING = 3;

    public const uint IOCTL_GPD_ACPI_ECREAD  = 2621482120; // 0x9C40A488
    public const uint IOCTL_GPD_ACPI_ECWRITE = 2621482124; // 0x9C40A48C
    public const uint IOCTL_GPD_ACPI_SMAPC   = 0x9C40A500; // OEM SMAPCTable 私有通道 (ACPIDriverDll 实测)
    public const uint IOCTL_GPD_ACPI_TMPREAD1 = 2621482192;
    public const uint IOCTL_GPD_ACPI_TMPREAD2 = 2621482196;
    public const uint IOCTL_GPD_ACPI_TMPREAD3 = 2621482200;

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern SafeFileHandle CreateFile(
        string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes,
        uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeviceIoControl(
        SafeFileHandle hDevice, uint dwIoControlCode,
        IntPtr lpInBuffer, int nInBufferSize,
        out int lpOutBuffer, int nOutBufferSize,
        out int lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeviceIoControl(
        SafeFileHandle hDevice, uint dwIoControlCode,
        IntPtr lpInBuffer, int nInBufferSize,
        IntPtr lpOutBuffer, int nOutBufferSize,
        out int lpBytesReturned, IntPtr lpOverlapped);

    private readonly object _lock = new();
    private SafeFileHandle? _handle;

    public string? LastError { get; private set; }
    public bool IsReady => _handle is { IsInvalid: false, IsClosed: false };

    public bool Open()
    {
        lock (_lock)
        {
            if (_handle is { IsInvalid: false, IsClosed: false }) return true;
            try
            {
                _handle = CreateFile(@"\\.\ACPIDriver", GENERIC_READ | GENERIC_WRITE,
                    FILE_SHARE_RW, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
                if (_handle.IsInvalid)
                {
                    LastError = $"CreateFile 失败: {Marshal.GetLastWin32Error()} (通常是权限不足或驱动未加载)";
                    _handle.Dispose();
                    _handle = null;
                    return false;
                }
                LastError = null;
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }
    }

    /// <summary>Read one EC RAM byte. Returns null on failure.</summary>
    public byte? Read(ushort addr)
    {
        lock (_lock)
        {
            if (!Open()) return null;
            IntPtr inBuf = Marshal.AllocHGlobal(4);
            try
            {
                Marshal.WriteInt32(inBuf, addr);
                if (DeviceIoControl(_handle!, IOCTL_GPD_ACPI_ECREAD, inBuf, 4,
                        out int outBuf, 4, out _, IntPtr.Zero))
                    return (byte)(outBuf & 0xFF);
                LastError = $"ECREAD 0x{addr:X3} 失败: {Marshal.GetLastWin32Error()}";
                return null;
            }
            catch (Exception ex) { LastError = ex.Message; return null; }
            finally { Marshal.FreeHGlobal(inBuf); }
        }
    }

    /// <summary>Temperature probe read via the driver's TMP ioctl classes (OEM: TempRead1/2/3).
    /// addr is passed as input like ECREAD; the driver routes it per ioctl.</summary>
    public int? ReadTemp(uint ioctl, ushort addr)
    {
        lock (_lock)
        {
            if (!Open()) return null;
            IntPtr inBuf = Marshal.AllocHGlobal(4);
            try
            {
                Marshal.WriteInt32(inBuf, addr);
                if (DeviceIoControl(_handle!, ioctl, inBuf, 4,
                        out int outBuf, 4, out _, IntPtr.Zero))
                    return outBuf & 0xFFFF;
                LastError = $"TEMPREAD 0x{addr:X3} 失败: {Marshal.GetLastWin32Error()}";
                return null;
            }
            catch (Exception ex) { LastError = ex.Message; return null; }
            finally { Marshal.FreeHGlobal(inBuf); }
        }
    }

    /// <summary>
    /// OEM SMAPC 通道 (ACPIDriverDll!SMAPCTable 的行为复刻):
    /// 复制 128 字节请求到缓冲头, DeviceIoControl(0x9C40A500, in=128, out=128)。
    /// 读表请求 = { 0xBB, offset, 0...(padding) }; 返回 4 字节结果在输出头部。
    /// </summary>
    public byte[]? SmapcExchange(byte[] request)
    {
        lock (_lock)
        {
            if (!Open()) return null;
            IntPtr inBuf = Marshal.AllocHGlobal(128);
            IntPtr outBuf = Marshal.AllocHGlobal(128);
            try
            {
                for (int i = 0; i < 128; i++) Marshal.WriteByte(inBuf, i, 0);
                for (int i = 0; i < request.Length && i < 128; i++) Marshal.WriteByte(inBuf, i, request[i]);
                if (!DeviceIoControl(_handle!, IOCTL_GPD_ACPI_SMAPC, inBuf, 128, outBuf, 128, out _, IntPtr.Zero))
                {
                    LastError = $"SMAPC 失败: {Marshal.GetLastWin32Error()}";
                    return null;
                }
                var result = new byte[128];
                for (int i = 0; i < 128; i++) result[i] = Marshal.ReadByte(outBuf, i);
                return result;
            }
            catch (Exception ex) { LastError = ex.Message; return null; }
            finally { Marshal.FreeHGlobal(inBuf); Marshal.FreeHGlobal(outBuf); }
        }
    }

    /// <summary>Write one EC RAM byte.</summary>
    public bool Write(ushort addr, byte value)
    {
        lock (_lock)
        {
            if (!Open()) return false;
            IntPtr inBuf = Marshal.AllocHGlobal(8);
            try
            {
                Marshal.WriteInt32(inBuf, addr);
                Marshal.WriteInt32(inBuf + 4, value);
                bool ok = DeviceIoControl(_handle!, IOCTL_GPD_ACPI_ECWRITE, inBuf, 8,
                    out _, 4, out _, IntPtr.Zero);
                if (!ok) LastError = $"ECWRITE 0x{addr:X3} 失败: {Marshal.GetLastWin32Error()}";
                return ok;
            }
            catch (Exception ex) { LastError = ex.Message; return false; }
            finally { Marshal.FreeHGlobal(inBuf); }
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _handle?.Dispose();
            _handle = null;
        }
    }
}
