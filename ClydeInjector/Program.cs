namespace ClydeInjector;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

using SharpMonoInjector;

internal class Program
{
    internal static void Log(string v)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("ClydeInjector");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("] ");
        Console.ResetColor();
        Console.WriteLine(v);
    }

    static bool progressEnded = true;
    internal static void ProgressBar(float progress)
    {
        var barWidth = 30;
        var pos = (int)(barWidth * progress);
        if (progressEnded)
            Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("ClydeInjector");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("] ");

        Console.ResetColor();
        Console.Write("[");
        for (var i = 0; i < barWidth; i++)
        {
            if (i < pos)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Write(" ");
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.Write(" ");
            }
        }
        Console.ResetColor();
        Console.Write($"] ({Math.Floor(progress * 100)}%)");
        Console.WriteLine();
        progressEnded = false;
    }

    internal static void ProgressBarEnd()
    {
        Console.WriteLine();
        progressEnded = true;
    }

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

        try
        {
            Injector injector = new Injector("REPO");
            injector.Inject(rawAssembly, @namespace, entry, entryFunc);
            Log("Successfully injected ClydeMenu");
        }
        catch
        {
            Log("Fatal error while injecting ClydeMenu (Did you disable your antivirus? Or double injected)");
        }
        Thread.Sleep(3000);
    }
}
