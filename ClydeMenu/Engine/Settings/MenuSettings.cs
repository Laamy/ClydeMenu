namespace ClydeMenu.Engine.Settings;

using System.Collections.Generic;
using System.Timers;

[ES3Serializable]
public class Setting<T>
{
    private readonly string _key;

    public T Value
    {
        get
        {
            try
            {
                if (!MenuSettings.SaveData.ContainsKey(_key))// bro..?
                    MenuSettings.SaveData[_key] = default(T);

                return (T)MenuSettings.SaveData[_key];
            }
            catch
            {
                return default(T);
            }
        }
        set => MenuSettings.SaveData[_key] = value;
    }

    public Setting(string key, T defaultValue)
    {
        _key = key;

        if (!MenuSettings.SaveData.ContainsKey(_key))
            MenuSettings.SaveData[_key] = defaultValue;
    }
}

[ES3Serializable]
internal class MenuSettings
{
    public static Dictionary<string, object> SaveData = new();

    public static Setting<string> GameTheme { get; set; } = new("GameTheme", "Dark");

    public static Setting<bool> VISUAL_MAPINFO { get; set; } = new("VISUAL_MAPINFO", true);
    public static Setting<bool> VISUAL_NETNFO { get; set; } = new("VISUAL_NETNFO", true);
    public static Setting<bool> VISUAL_NOISELOGGER { get; set; } = new("VISUAL_NOISELOGGER", true);
    public static Setting<bool> VISUAL_FreeLook { get; set; } = new("VISUAL_FreeLook", false);
    public static Setting<bool> VISUAL_RAINBOW { get; set; } = new("VISUAL_RAINBOW", false);

    public static Setting<bool> AccountSpoofer { get; set; } = new("AccountSpoofer", false);
    public static Setting<bool> PingSpoofer { get; set; } = new("PingSpoofer", false);

    public static Setting<bool> FastIntro { get; set; } = new("FastIntro", true);
    public static Setting<float> FastIntroSpeed { get; set; } = new("FastIntroSpeed", 0.99f);

    // debugging. do not use in release
    public static Setting<bool> ESP_Player { get; set; } = new("ESP_Player", false);
    public static Setting<bool> ESP_Enemy { get; set; } = new("ESP_Enemy", false);
    public static Setting<bool> ESP_Valuable { get; set; } = new("ESP_Valuable", false);
    public static Setting<bool> ESP_Extractions { get; set; } = new("ESP_Extractions", false);

    // cache stuff
    public static Setting<uint> ChangeLogVersion { get; set; } = new("ChangeLogVersion", 0);
    public static Setting<bool> OpenedShop { get; set; } = new("OpenedShop", false);

    internal static Setting<uint> Currency { get; set; } = new("FreeCurrency", 0);

    public static Timer _saveTimer;

    public static void Save()
    {
        ES3Settings eS3Settings = new ES3Settings([ES3.Location.File]) { path = "SettingsData_Clyde.es3", };
        ES3.Save("Settings", SaveData, eS3Settings);
    }

    public static void Load()
    {
        ES3Settings eS3Settings = new ES3Settings([ES3.Location.File]) { path = "SettingsData_Clyde.es3", };

        var defaultSaveData = SaveData;
        if (ES3.FileExists(eS3Settings))
        {
            SaveData = ES3.Load<Dictionary<string, object>>("Settings", eS3Settings);
            foreach (var key in defaultSaveData.Keys)
            {
                if (!SaveData.ContainsKey(key))
                    SaveData[key] = defaultSaveData[key];
            }
        }
        else Save();

        if (_saveTimer != null)
            return;

        _saveTimer = new Timer(1000);
        _saveTimer.Elapsed += (_, _) =>
        {
            Save();
        };
        _saveTimer.Start();
    }
}
