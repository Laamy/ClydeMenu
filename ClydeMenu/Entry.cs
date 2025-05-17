namespace ClydeMenu;

using System;
using System.Collections.Generic;

using ClydeMenu.Engine;
using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Commands;
using ClydeMenu.Engine.Components;

using HarmonyLib;

public class Entry
{
    private static readonly List<BaseComponent> loadedComps = [];

    public static void Load()
    {
        GameEvents.Start();
        ModuleHandler.Start();

        Storage.harmony = new Harmony("com.clyde_menu");
        Storage.harmony.PatchAll(typeof(Engine.Patches).Assembly);

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

    public static void InitModule<T>(string modName) where T : BaseComponent
    {
        Console.WriteLine($"Creating {modName} component");

        var type = typeof(T);
        var instance = (T)Activator.CreateInstance(type);
        loadedComps.Add(instance);
    }

    public static void Unload()
    {
        Storage.harmony.UnpatchAll("com.clyde_menu");

        loadedComps.Clear();

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
