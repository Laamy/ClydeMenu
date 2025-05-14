namespace ClydeMenu.Engine.Menu;

using Photon.Pun;
using UnityEngine;

public abstract class BaseMenu
{
    public abstract void Render();

    public abstract void OnPush();
    public abstract void OnPop();

    public abstract void OnUpdate();

    public virtual bool ShouldBlockInput => true;
    public virtual bool ShouldDarkenBackground => true;
    
    public bool InFocus
    {
        get
        {
            if (MenuSceneComponent.Instance.FocusedMenu == null)
                return false;
            return MenuSceneComponent.Instance.FocusedMenu == this;
        }
    }

    public void Pop() => MenuSceneComponent.Instance.PopMenu(this);

    public BaseMenu() { }
}
