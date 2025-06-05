namespace ClydeMenu.Engine.Menu.Menus;

using System;
using System.Collections.Generic;
using System.Linq;

using ClydeMenu.Engine.Settings;
using UnityEngine;

public class ShopMenu : BaseMenu
{
    public override void OnPop() {}
    public override void OnPush() {}
    public override void OnUpdate() {}

    private List<(string name, int cost)> shopItems = new()
    {
        ("Basic Hat", 10),
        ("Golden Hat", 500)
    };

    private ThemeConfig StyleTheme = new();

    public ShopMenu()
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
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 uiMargin = new Vector2(600, 200);
        Vector2 topLeft = uiMargin;
        Vector2 size = screenSize - (uiMargin * 2);
        Vector2 titleSize = new Vector2(size.x, 40);

        RenderUtils.DrawRect(topLeft, size, StyleTheme.ContentBoxBackground);

        var titleTopLeft = topLeft;
        RenderUtils.DrawRect(titleTopLeft, titleSize, StyleTheme.Titlebar);
        RenderUtils.DrawString(titleTopLeft + new Vector2(10, 10), "Shop Menu", StyleTheme.TitlebarText, 18);

        var closeButtonSize = new Vector2(30, 30);
        var closeButtonPos = new Vector2(topLeft.x + size.x - closeButtonSize.x - 5, topLeft.y + 5);
        if (DrawButton(closeButtonPos, closeButtonSize, StyleTheme.TitlebarCloseButton))
            MenuSceneComponent.Instance.PopMenu(this);

        var itemY = topLeft.y + titleSize.y + 10;
        foreach (var (name, cost) in shopItems)
        {
            var itemTopLeft = new Vector2(topLeft.x + 10, itemY);
            var itemSize = new Vector2(size.x - 20, 40);
            RenderUtils.DrawRect(itemTopLeft, itemSize, StyleTheme.ContentBox);

            RenderUtils.DrawString(itemTopLeft + new Vector2(10, 10), name, StyleTheme.MenuText, 16);
            RenderUtils.DrawString(itemTopLeft + new Vector2(itemSize.x - 70, 10), $"${cost}K", StyleTheme.MenuTextDark, 16);

            itemY += itemSize.y + 10;
        }
    }

    private bool DrawButton(Vector2 pos, Vector2 size, Color color)
    {
        RenderUtils.DrawRect(pos, size, color);

        var e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Vector2 mouse = e.mousePosition;
            if (mouse.x >= pos.x && mouse.x <= pos.x + size.x &&
                mouse.y >= pos.y && mouse.y <= pos.y + size.y)
            {
                e.Use();
                return true;
            }
        }

        return false;
    }
}
