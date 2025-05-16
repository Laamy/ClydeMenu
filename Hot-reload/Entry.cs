namespace Hot_reload;

using System.IO;
using System;
using System.Reflection;

using UnityEngine;
using Object = UnityEngine.Object;

using Hot_reload.Components;

public class Entry
{
    static object modInstance;

    static MethodInfo modLoad;
    static MethodInfo modUnload;

    public static bool isLoaded = false;
    public static MethodInfo modUpdate;
    public static MethodInfo modFixedUpdate;
    public static MethodInfo modOnGUI;

    static Assembly modAssembly;

    public static void Load()
    {
        Kernel32.AllocConsole();
        {
            var stdHandle = Kernel32.GetStdHandle(-11);
            var safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
            var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            var encoding = System.Text.Encoding.UTF8;
            var standardOutput = new StreamWriter(fileStream, encoding) { AutoFlush = true };
            Console.SetOut(standardOutput);
            Console.Clear();
        }

        var obj = new GameObject("HotReloadListener");
        obj.AddComponent<HotReloadBehaviour>();
        Object.DontDestroyOnLoad(obj);

        Console.WriteLine("Hot-reload injected successfully");

        InitDependencies();
        Console.WriteLine("Mod dependences loaded");
        Reload();
        Console.WriteLine("Mod successfully loaded");
    }

    public static void InitDependencies()
    {
        // executable patching live ig
        var dllPath = @"C:\build\r.e.p.o\0Harmony.dll";
        var bytes = File.ReadAllBytes(dllPath);
        Assembly.Load(bytes);
    }

    public static void Reload()
    {
        try
        {
            isLoaded = false;
            modUnload?.Invoke(null, null);
            modUnload = null;
            modLoad = null;
            modInstance = null;
            modAssembly = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            var dllPath = @"C:\build\r.e.p.o\ClydeMenu.dll";
            var bytes = File.ReadAllBytes(dllPath);
            modAssembly = Assembly.Load(bytes);

            var type = modAssembly.GetType("ClydeMenu.Entry");
            modLoad = type?.GetMethod("Load", BindingFlags.Public | BindingFlags.Static);
            modUnload = type?.GetMethod("Unload", BindingFlags.Public | BindingFlags.Static);
            modUpdate = type?.GetMethod("Update", BindingFlags.Public | BindingFlags.Static);
            modFixedUpdate = type?.GetMethod("FixedUpdate", BindingFlags.Public | BindingFlags.Static);
            modOnGUI = type?.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Static);

            modLoad?.Invoke(null, null);
            isLoaded = true;

            Console.WriteLine("ClydeMenu hot-reloaded.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Reload error: {ex}");
        }
    }

    public static void Unload()
    {
        try
        {
            modUnload?.Invoke(modInstance, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unload error: {ex}");
        }

        modUnload = null;
        modLoad = null;
        modInstance = null;
        modAssembly = null;

        GC.Collect();

        Console.Clear();
        Kernel32.FreeConsole();
    }
}
