namespace ClydeMenu;

using System;
using System.Collections.Generic;

using ClydeMenu.Engine;
using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Commands;
using ClydeMenu.Engine.Components;
using ClydeMenu.Engine.Settings;

using HarmonyLib;
using UnityEngine;

public class Entry
{
    private static readonly List<BaseComponent> loadedComps = [];

    public static void Log(string msg)
    {
        if (!Storage.IsBepinExLoaded)
            Console.WriteLine(msg);
        else Debug.Log($"[ClydeMenu] {msg}", Storage.HotReloadListener);
    }

    public static void Load()
    {
        Storage.HotReloadListener = GameObject.Find("HotReloadListener");
        Storage.IsBepinExLoaded = false;
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetName().Name == "BepInEx")
            {
                Storage.IsBepinExLoaded = true;
                break;
            }
        }

        MenuSettings.Load();

        GameEvents.Start();
        ModuleHandler.Start();

        Storage.harmony = new Harmony("com.clyde_menu");
        Storage.harmony.PatchAll(typeof(Engine.Patches).Assembly);

        try
        {
            InitModule<MenuSceneComponent>("MenuScene"); // menu stack stuff for other components to use

            InitModule<ClientComponent>("ClydeMenu");
        }
        catch (Exception e)
        {
            Entry.Log($"Error loading comps: {e}");
            return;
        }

        Entry.Log("ClydeMenu injected successfully");
    }

    public static void InitModule<T>(string modName) where T : BaseComponent
    {
        Entry.Log($"Creating {modName} component");

        var type = typeof(T);
        var instance = (T)Activator.CreateInstance(type);
        loadedComps.Add(instance);
    }

    public static void Unload()
    {
        Storage.harmony.UnpatchAll("com.clyde_menu");

        loadedComps.Clear();

        MenuSettings._saveTimer?.Stop();
        MenuSettings._saveTimer?.Dispose();

        GameEvents.Shutdown();
        ModuleHandler.Shutdown();

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public static void Update()
    {
        foreach (var comp in loadedComps)
            comp.Update();
    }

    public static void LateUpdate()
    {
        foreach (var comp in loadedComps)
            comp.LateUpdate();
    }

    public static void FixedUpdate()
    {
        foreach (var comp in loadedComps)
            comp.FixedUpdate();
    }

    public static void OnGUI()
    {
        foreach (var comp in loadedComps)
            comp.OnGUI();
    }
}
