namespace ClydeInjector;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SharpMonoInjector;

internal class Program
{
    internal static void Log(string v) => Console.WriteLine($"[ClydeInjector] {v}");

    static void Main(string[] args)
    {
        if (Process.GetProcessesByName("REPO").Length == 0)
        {
            Log("Please open REPO before attempting to inject clyde");
            Thread.Sleep(3000);
            return;
        }

        Log("Attempting to validate cache..");
        ClydeUpdater.ValidateCache();

        // i'll do inject crap here i guess
        //smi.exe inject -p REPO -a Hot-reload.dll -n Hot_reload -c Entry -m Load
        var rawAssembly = File.ReadAllBytes($"{ClydeUpdater.injFolder.FullName}/Hot-reload.dll");
        var @namespace = "Hot_reload";
        var entry = "Entry";
        var entryFunc = "Load";

        Injector injector = new Injector("REPO");
        injector.Inject(rawAssembly, @namespace, entry, entryFunc);
    }
}
