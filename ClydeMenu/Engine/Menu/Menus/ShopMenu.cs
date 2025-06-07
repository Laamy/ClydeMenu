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

    // oh i could do smth like steam levels
    private List<(string name, int cost, Setting<bool>, Setting<bool>)> shopItems = new()
    {
        ("Debug world (F8)", 1, MenuSettings.Shop.DebugWorld, null),
        ("Rainbow", 500, MenuSettings.Shop.Rainbow, MenuSettings.Shop.FunTabUnlocked),
    };

    CameraGlitch glitchCam;

    public ShopMenu()
    {
        glitchCam = GameObject.FindObjectOfType<CameraGlitch>();

        Storage.StyleTheme = ClientInstance.GetClientTheme();
    }

    public override void Render()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 uiMargin = new Vector2(600, 200);
        Vector2 topLeft = uiMargin;
        Vector2 size = screenSize - (uiMargin * 2);
        Vector2 titleSize = new Vector2(size.x, 40);

        RenderUtils.DrawRect(topLeft, size, Storage.StyleTheme.ContentBoxBackground);

        var titleTopLeft = topLeft;
        RenderUtils.DrawRect(titleTopLeft, titleSize, Storage.StyleTheme.Titlebar);
        RenderUtils.DrawString(titleTopLeft + new Vector2(10, 10), "Shop Menu", Storage.StyleTheme.TitlebarText, 18);

        var closeButtonSize = new Vector2(30, 30);
        var closeButtonPos = new Vector2(topLeft.x + size.x - closeButtonSize.x - 5, topLeft.y + 5);
        if (DrawButton(closeButtonPos, closeButtonSize, Storage.StyleTheme.TitlebarCloseButton))
            MenuSceneComponent.Instance.PopMenu(this);

        var itemY = topLeft.y + titleSize.y + 10;
        foreach (var (name, cost, setting, categoryTag) in shopItems)
        {
            var itemTopLeft = new Vector2(topLeft.x + 10, itemY);
            var itemSize = new Vector2(size.x - 20, 40);
            RenderUtils.DrawRect(itemTopLeft, itemSize, setting.Value == true ? Storage.StyleTheme.Sidebar : Storage.StyleTheme.ContentBox);

            RenderUtils.DrawString(itemTopLeft + new Vector2(10, 10), name, Storage.StyleTheme.MenuText, 16);
            var costLabel = $"Cost: {cost} gem(s)";
            if (setting.Value)
                costLabel = "Owned";

            RenderUtils.DrawString(itemTopLeft + new Vector2(itemSize.x - 170, 10), costLabel, Storage.StyleTheme.MenuTextDark, 16);

            if (LabelPressed(setting.GetName(), new Rect(itemTopLeft, itemSize)))
            {
                if (!setting.Value && MenuSettings.Currency.Value >= cost)
                {
                    MenuSettings.Currency.Value -= (uint)cost;
                    if (categoryTag != null)
                        categoryTag.Value = true;
                    setting.Value = true;
                    glitchCam.PlayUpgrade();
                }
            }

            itemY += itemSize.y + 10;
        }
    }

    private Dictionary<string, bool> buttonStates = new();
    bool LabelPressed(string selectionKey, Rect buttonRect)
    {
        var curEvent = Event.current;
        Vector2 mousePos = curEvent.mousePosition;

        if (!buttonStates.ContainsKey(selectionKey))
            buttonStates[selectionKey] = false;

        if (curEvent.type == EventType.MouseDown && buttonRect.Contains(mousePos))
        {
            if (!buttonStates[selectionKey])
            {
                buttonStates[selectionKey] = true;
                return true;
            }
        }
        else if (curEvent.type == EventType.MouseUp && buttonStates[selectionKey])
        {
            buttonStates[selectionKey] = false;
        }

        return false;
    }

    private bool IsHeld(string label)
    {
        if (!buttonStates.ContainsKey(label))
            buttonStates[label] = false;

        return buttonStates[label];
    }

    private bool DrawButton(Vector2 pos, Vector2 size, Color color)
    {
        RenderUtils.DrawRect(pos, size, color);

        return LabelPressed("EscapeBtnShop", new Rect(pos, size));
    }
}
