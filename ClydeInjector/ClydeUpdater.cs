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

        var client = new WebClient();

        var harmony = Path.Combine(injFolder.FullName, "0Harmony.dll");
        if (!File.Exists(harmony))
        {
            client.DownloadFile($"{TargetUri}/dependencies/0Harmony.dll", harmony);
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

        var i = 1;
        foreach (var lib in coreLibs)
        {
            var targetUri = $"{TargetUri}/{version}/{lib}";
            var target = $"{injFolder.FullName}/{lib}";
            if (File.Exists(target))
                File.Delete(target);

            client.DownloadFile(targetUri , target);
            Program.Log($"Successfully downloaded {lib} ({i}/{coreLibs.Length})");
            i++;
        }

        if (File.Exists(versionPath))
            File.Delete(versionPath);
        File.WriteAllText(versionPath, version);
    }

    private static string LatestFolder()
    {
        var client = new WebClient();
        var notes = client.DownloadString($"{TargetUri}/NOTES.cs");

        foreach (var note in notes.Split('\r'))
        {
            if (note.Contains("//latest"))
            {
                var pattern = Regex.Match(note, @"0x[0-9A-Fa-f]+");
                if (!pattern.Success)
                    throw new Exception("Fatal error trying to get notes for clyde");

                uint versionId = Convert.ToUInt32(pattern.Value, 16);
                var version = ToVersionString(versionId);
                return version;
            }
        }
        return null;
    }
}
