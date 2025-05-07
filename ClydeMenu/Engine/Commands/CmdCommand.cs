namespace ClydeMenu.Engine.Commands;

using System;

using UnityEngine;

public class CmdCommand : BaseCommand
{
    public CmdCommand() : base("cmds", "Get a list of commands", "<>") {}

    public override void Execute(string[] args)
    {
        Console.WriteLine("Commands:");
        foreach (var cmd in CmdHandler.Commands)
            Console.WriteLine($"- {cmd.Name} {cmd.Arguments} -- {cmd.Description}");
    }
}
