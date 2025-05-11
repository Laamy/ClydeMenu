namespace ClydeMenu.Engine.Commands;

using Photon.Pun;

public abstract class BaseModule : MonoBehaviourPunCallbacks
{
    public string Name { get; set; }
    public string Category { get; set; }
    public string Tooltip { get; set; }
    public bool IsEnabled { get; set; } = false;

    public abstract void Initialize();

    public virtual void OnEnable() {}
    public virtual void OnDisable() {}

    public virtual void OnRender() {}
    public virtual void OnUpdate() {}

    public BaseModule(string name, string tooltip, string category)
    {
        Name = name;
        Category = category;
        Tooltip = tooltip;
    }
}
