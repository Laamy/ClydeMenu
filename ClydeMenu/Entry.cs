namespace ClydeMenu;

using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using Object = UnityEngine.Object;

using ClydeMenu.Engine;
using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Commands;
using System.Reflection;
using System.Text;
//using HarmonyLib;

public class Entry
{
    private static readonly List<GameObject> loadedComps = [];

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

        //new Harmony("com.clyde.mods").PatchAll();

        GameEvents.Init();
        ModuleHandler.Init();
        
        try
        {
            InitModule<MenuSceneComponent>("MenuScene"); // menu stack stuff for other components to use

            InitModule<ClientComponent>("ClydeMenu");
            InitModule<CmdBarComponent>("CmdBar");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading comps: {e}");
            return;
        }

        Console.WriteLine("ClydeMenu injected successfully");
    }

    public static void InitModule<T>(string modName) where T : MonoBehaviour
    {
        var LoadObj = new GameObject(modName);
        var clientModule = LoadObj.AddComponent<T>();
        loadedComps.Add(LoadObj);
        Object.DontDestroyOnLoad(LoadObj);
    }

    public static void Unload()
    {
        Console.Clear();
        Kernel32.FreeConsole();

        foreach (var obj in loadedComps)
            Object.DestroyImmediate(obj);

        //CleanStaticReferences();
        GC.Collect();
    }
}
