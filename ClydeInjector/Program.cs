namespace ClydeInjector;

using System;
using System.IO;

using SharpMonoInjector;

internal class Program
{
    static DirectoryInfo injFolder = new($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\.Clyde");

    public static string[] injectables = ["ClydeMenu.dll", "0Harmony.dll"];
    
    static void Main(string[] args)
    {
        if (!injFolder.Exists)
            injFolder.Create();

        foreach (var injectable in injectables)
        {
            var path = Path.Combine(injFolder.FullName, injectable);
            if (!File.Exists(path))
                File.Copy(injectable, path);
        }

        Console.WriteLine($"Inject envrionment pushed to {injFolder.FullName}");

        // i'll do inject crap here i guess
        //smi.exe inject -p REPO -a Hot-reload.dll -n Hot_reload -c Entry -m Load
        var rawAssembly = File.ReadAllBytes("Hot-reload.dll");
        var @namespace = "Hot_reload";
        var entry = "Entry";
        var entryFunc = "Load";

        Injector injector = new Injector("REPO");
        injector.Inject(rawAssembly, @namespace, entry, entryFunc);
    }
}
