using System.Text.Json;

namespace FanSlider;

public class AppSettings
{
    public int LastLevel { get; set; }              // 上次应用的档位 (0=自动)
    public bool BoostOn { get; set; }               // 上次是否开启 FanBoost (真·满速)
    public bool ApplyOnStartup { get; set; }        // 本程序启动时自动恢复上次档位
    public bool RestoreAutoOnExit { get; set; }     // 退出时恢复 EC 自动策略
    public bool AutoStartWithWindows { get; set; }  // 登录时随 Windows 启动 (HKCU Run)

    // v2: 每风扇接管状态
    public bool CpuTakeover { get; set; }
    public bool GpuTakeover { get; set; }
    public int CpuDuty { get; set; } = 50;
    public int GpuDuty { get; set; } = 50;

    private static string Dir =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FanSlider");
    private static string FilePath => Path.Combine(Dir, "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (System.IO.File.Exists(FilePath))
                return JsonSerializer.Deserialize<AppSettings>(System.IO.File.ReadAllText(FilePath)) ?? new AppSettings();
        }
        catch { }
        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            System.IO.Directory.CreateDirectory(Dir);
            System.IO.File.WriteAllText(FilePath, JsonSerializer.Serialize(this,
                new JsonSerializerOptions { WriteIndented = true }));
        }
        catch { }
    }
}
