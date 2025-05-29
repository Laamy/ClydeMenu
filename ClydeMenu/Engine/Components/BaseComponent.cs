namespace ClydeMenu.Engine.Components;

public class BaseComponent
{
    public virtual void Update() {}
    public virtual void LateUpdate() {}
    public virtual void FixedUpdate() {}
    public virtual void OnGUI() {}
}
