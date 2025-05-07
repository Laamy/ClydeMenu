namespace ClydeMenu.Engine.Commands;

public class ExitCommand : BaseCommand
{
    public ExitCommand() : base("exit", "Eject the cmdbar & clydemenu components", "<>") { }

    public override void Execute(string[] args) => Entry.Unload();
}
