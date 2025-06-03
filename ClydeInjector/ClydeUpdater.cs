namespace ClydeInjector;

using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

internal class ClydeUpdater
{
    private static string ToVersionString(uint version)
    {
        var major = (byte)((version >> 24) & 0xFF);
        var minor = (byte)((version >> 16) & 0xFF);
        var debug = (byte)((version >> 8) & 0xFF);

        var result = $"v{major}.{minor}";
        if (debug != 0)
            result += $".{debug}";

        return result;
    }

    public static readonly string TargetUri = "https://raw.githubusercontent.com/Laamy/ClydeMenu/refs/heads/master/ClydeMenu/Content";
    
    public static DirectoryInfo injFolder = new($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\.Clyde");

    public static string[] dependencies = ["0Harmony.dll"];
    public static string[] coreLibs = ["ClydeMenu.dll", "Hot-reload.dll"];

    public static void ValidateCache()
    {
        if (!injFolder.Exists)
            injFolder.Create();

        var harmony = Path.Combine(injFolder.FullName, "0Harmony.dll");
        if (!File.Exists(harmony))
        {
            DownloadFile($"{TargetUri}/dependencies/0Harmony.dll", harmony);
            Program.Log("Downloaded client dependencies");
        }

        var version = LatestFolder();
        var versionPath = $"{injFolder.FullName}/version.txt";
        if (File.Exists(versionPath))
        {
            if (File.ReadAllText(versionPath) == version)
            {
                var exit = true;
                foreach (var lib in coreLibs)
                    if (!File.Exists($"{injFolder.FullName}/{lib}"))
                        exit = false;
                if (exit)
                {
                    Program.Log("Clyde is up to date");
                    return;
                }
            }
        }

        Program.Log("Attempting to update/repair clydemenu");

        for (var i = 0; i < coreLibs.Length; i++)
        {
            var lib = coreLibs[i];
            var targetUri = $"{TargetUri}/{version}/{lib}";
            var target = $"{injFolder.FullName}/{lib}";
            if (File.Exists(target))
                File.Delete(target);

            DownloadFile(targetUri, target);
            var progress = (i + 1f) / coreLibs.Length;
            Program.ProgressBar(progress);
        }
        Program.ProgressBarEnd();
        Program.Log("Successfully updated ClydeMenu (Disable your antivirus if this didnt fix clyde)");

        if (File.Exists(versionPath))
            File.Delete(versionPath);
        File.WriteAllText(versionPath, version);
    }

    private static string LatestFolder()
    {
        var notes = DownloadString($"{TargetUri}/NOTES.cs");

        foreach (var note in notes.Split('\n'))
        {
            if (note.Contains("//latest"))
            {
                var pattern = Regex.Match(note, @"0x[0-9A-Fa-f]+");
                if (!pattern.Success)
                    throw new Exception("Fatal error trying to get notes for clyde");

                uint versionId = Convert.ToUInt32(pattern.Value, 16);
                var version = ToVersionString(versionId);
                Program.Log($"Checking for ClydeMenu {version}");
                return version;
            }
        }
        return null;
    }

    static void DownloadFile(string url, string path)
    {
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

        using var response = (HttpWebResponse)request.GetResponse();
        using var stream = response.GetResponseStream();
        using var file = File.Create(path);
        stream.CopyTo(file);
    }

    static string DownloadString(string url)
    {
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

        using var response = (HttpWebResponse)request.GetResponse();
        using var stream = response.GetResponseStream();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
