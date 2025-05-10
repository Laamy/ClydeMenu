namespace ClydeMenu.Engine.Commands;

using System;
using System.Collections.Generic;
using System.Linq;

public class CmdHandler
{
    public static List<BaseCommand> Commands = [];

    public static void Init()
    {
        var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(BaseCommand)) && !type.IsAbstract);

        foreach (var commandType in commandTypes)
        {
            var command = (BaseCommand)Activator.CreateInstance(commandType);
            Commands.Add(command);
            Console.WriteLine($"Command '{command.Name}' initialized.");
        }
    }

    public static void Handle(string rawCmd)
    {
        if (Commands.Count == 0)
            Init();

        var args = new List<string>();
        var currentArg = string.Empty;
        var inQuotes = false;
        foreach (byte chr in rawCmd)
        {
            if (chr == '"')
            {
                inQuotes = !inQuotes;
                if (string.IsNullOrEmpty(currentArg))
                    continue;

                if (currentArg.Length > 0)
                {
                    args.Add(currentArg);
                    currentArg = string.Empty;
                }
                continue;
            }

            if (chr == ' ' && !inQuotes)
            {
                if (!string.IsNullOrEmpty(currentArg))
                {
                    args.Add(currentArg);
                    currentArg = string.Empty;
                }
                continue;
            }

            currentArg += (char)chr;
        }

        if (!string.IsNullOrEmpty(currentArg))
            args.Add(currentArg);

        var cmd = args[0];
        args.RemoveAt(0);

        foreach (var c in Commands)
        {
            if (c.Name == cmd.ToLower())
            {
                c.Execute(args.ToArray());
                return;
            }
        }

        Console.WriteLine($"Command '{cmd}' not found.");
    }
}
