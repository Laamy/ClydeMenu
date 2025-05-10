namespace ClydeMenu.Engine.Settings;

using System.Timers;

public class Setting<T>
{
    private T _value;
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            Settings.instance.ScheduleSave();
        }
    }

    public Setting() {}
    public Setting(T defaultValue) => _value = defaultValue;
}

internal class Settings
{
    public Setting<float> MusicVolume { get; set; } = new(1.0f);
    public Setting<int> MaxPlayers { get; set; } = new(4);
    public Setting<string> PlayerName { get; set; } = new("Player");
    public Setting<bool> Fullscreen { get; set; } = new(true);

    private Timer _saveTimer;

    public void Save()
    {
        Settings settings = new Settings();
        ES3Settings es3Settings = new ES3Settings([ES3.Location.Cache]) { path = "SettingsData_Clyde.es3" };

        ES3.Save<Settings>("Settings", settings, es3Settings);
    }

    public static void Load()
    {
        ES3Settings eS3Settings = new ES3Settings([ES3.Location.Cache]) { path = "SettingsData_Clyde.es3" };

        _instance = ES3.Load<Settings>("Settings", new Settings(), eS3Settings);
    }

    public void ScheduleSave()
    {
        if (_saveTimer != null)
            return;

        _saveTimer = new Timer(500);
        _saveTimer.Elapsed += (_, _) =>
        {
            _saveTimer = null;
            Save();
        };
        _saveTimer.Start();
    }

    private static Settings _instance;
    public static Settings instance
    {
        get
        {
            if (_instance == null)
                Load();

            return _instance;
        }
    }
}
