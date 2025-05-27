namespace ClydeMenu.Engine.Menu.Menus;

using System;
using System.Linq;

using ClydeMenu.Engine.Settings;

public class MenuTemplate : BaseMenu
{
    public override void OnPop() {}
    public override void OnPush() {}
    public override void OnUpdate() {}

    private ThemeConfig StyleTheme = new();

    public MenuTemplate()
    {
        if (Storage.InternalThemeStyle != null)
            StyleTheme = Storage.InternalThemeStyle;
        else
        {
            if (MenuSettings.GameTheme != null)
                StyleTheme = Storage.StyleThemes[MenuSettings.GameTheme.Value];
            else
                StyleTheme = Storage.StyleThemes["Dark"];
        }
        Storage.SETTINGS_Theme = Array.IndexOf(Storage.StyleThemes.Keys.ToArray(), MenuSettings.GameTheme.Value);
    }

    public override void Render()
    {
    }
}
