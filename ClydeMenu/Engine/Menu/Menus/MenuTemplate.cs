namespace ClydeMenu.Engine.Menu.Menus;

using System;
using System.Linq;

using ClydeMenu.Engine.Settings;

public class MenuTemplate : BaseMenu
{
    public override void OnPop() {}
    public override void OnPush() {}
    public override void OnUpdate() {}

    public MenuTemplate()
    {
        Storage.StyleTheme = ClientInstance.GetClientTheme();
    }

    public override void Render()
    {

    }
}
