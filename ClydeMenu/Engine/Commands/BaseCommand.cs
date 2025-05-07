namespace ClydeMenu.Engine.Commands;

using Photon.Pun;

public class BaseCommand : MonoBehaviourPunCallbacks
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Arguments { get; set; }

    public virtual void Execute(string[] args) { }

    public BaseCommand(string name, string description, string arguments)
    {
        Name = name;
        Description = description;
        Arguments = arguments;
    }
}
