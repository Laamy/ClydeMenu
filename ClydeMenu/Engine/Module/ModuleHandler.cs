namespace ClydeMenu.Engine.Commands;

using System;
using System.Collections.Generic;
using System.Linq;

public class ModuleHandler
{
    public static List<BaseModule> Commands = [];
    public static Dictionary<string, List<BaseModule>> Categories = new();

    public static void Init()
    {
        var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(BaseModule)) && !type.IsAbstract);

        foreach (var commandType in commandTypes)
        {
            var command = (BaseModule)Activator.CreateInstance(commandType);
            Commands.Add(command);
            Console.WriteLine($"Module '{command.Name}' initialized.");
        }

        foreach (var command in Commands)
        {
            if (!Categories.ContainsKey(command.Category))
                Categories[command.Category] = [];

            Categories[command.Category].Add(command);
        }

        foreach (var module in Commands)
            module.Initialize();
    }
}
