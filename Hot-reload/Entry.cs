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
    public static MethodInfo modLateUpdate;
    public static MethodInfo modFixedUpdate;
    public static MethodInfo modOnGUI;

    static Assembly modAssembly;

    static bool isBepinExLoaded = false;
    static GameObject HotReloadListener = null;

    public static void Log(string msg)
    {
        if (!isBepinExLoaded)
            Console.WriteLine(msg);
        else Debug.Log($"[HotReloadListener] {msg}", HotReloadListener);
    }

    public static void Load()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetName().Name == "BepInEx")
            {
                isBepinExLoaded = true;
                break;
            }
        }

        HotReloadListener = new GameObject("HotReloadListener");
        HotReloadListener.AddComponent<HotReloadBehaviour>();
        Object.DontDestroyOnLoad(HotReloadListener);

        if (!isBepinExLoaded)
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

            Log("Booted into alone mode");
        }
        else
        {
            Log("Booted into bepinex mode");
        }

        Log("Hot-reload injected successfully");

        InitDependencies();
        Log("Mod dependences loaded");
        Reload();
        Log("Mod successfully loaded");
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
            modLateUpdate = type?.GetMethod("LateUpdate", BindingFlags.Public | BindingFlags.Static);
            modFixedUpdate = type?.GetMethod("FixedUpdate", BindingFlags.Public | BindingFlags.Static);
            modOnGUI = type?.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Static);

            modLoad?.Invoke(null, null);
            isLoaded = true;

            Log("ClydeMenu hot-reloaded.");
        }
        catch (Exception ex)
        {
            Log($"Reload error: {ex}");
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
            Log($"Unload error: {ex}");
        }

        modUnload = null;
        modLoad = null;
        modInstance = null;
        modAssembly = null;

        GC.Collect();

        if (!isBepinExLoaded)
        {
            Console.Clear();
            Kernel32.FreeConsole();
        }
    }
}
