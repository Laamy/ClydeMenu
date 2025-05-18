namespace ClydeMenu.Engine.Commands;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Object = UnityEngine.Object;

public class ModuleHandler
{
    public static List<BaseModule> Modules = [];
    public static Dictionary<string, List<BaseModule>> Categories = new();

    public static void Shutdown()
    {
        foreach (var module in Modules)
            module.OnDisable();

        Modules.Clear();
        Modules = null;

        Categories.Clear();
        Categories = null;
        Entry.Log("ModuleHandler::Shutdown(void) -> Shutdown successful, resources cleaned up.");
    }

    public static void Start()
    {
        try
        {
            var moduleType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(BaseModule)) && !type.IsAbstract);

            foreach (var moduleT in moduleType)
            {
                var module = (BaseModule)Activator.CreateInstance(moduleT);
                Modules.Add(module);
                Entry.Log($"Module '{module.Name}' initialized.");
            }

            foreach (var module in Modules)
            {
                if (!Categories.ContainsKey(module.Category))
                    Categories[module.Category] = [];

                Categories[module.Category].Add(module);
            }

            foreach (var module in Modules)
                module.Initialize();
        }
        catch (Exception e)
        {
            Entry.Log($"Error initializing modules: {e}");
        }
    }
}
