namespace ClydeMenu.Engine.Menu.Menus;

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using ClydeMenu.Engine.Settings;

public class ChangeLogMenu : BaseMenu
{
    private ThemeConfig StyleTheme = new();
    private Vector2 scrollPos;
    private Rect windowRect;

    public ChangeLogMenu()
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

        int screenWidth = Screen.width, screenHeight = Screen.height;
        var windowWidth = 910;
        var windowHeight = 500;

        windowRect = new Rect(
            screenWidth / 2 - windowWidth / 2,
            screenHeight / 2 - windowHeight / 2,
            windowWidth,
            windowHeight
        );
    }

    public override void OnPop() {}
    public override void OnPush() {}
    public override void OnUpdate() {}

    public override void Render()
    {
        windowRect = GUI.Window(691, windowRect, DrawWindow, GUIContent.none);
    }

    private void DrawWindow(int windowID)
    {
        var textColor = StyleTheme.MenuText;
        var panelColor = StyleTheme.ContentBoxBackground;

        RenderUtils.DrawRect(new Vector2(0, 0), new Vector2(windowRect.width, 30), StyleTheme.Titlebar);
        RenderUtils.DrawString(new Vector2(10, 7), $"Changelog {ClydeVersion.ToVersionString(ClydeVersion.Current)}", StyleTheme.TitlebarText, 18);

        var closeBtnRect = new Rect(windowRect.width - 30, 5, 25, 25);
        if (GUI.Button(closeBtnRect, "X"))
        {
            MenuSceneComponent.Instance.PopMenu(this);

            MenuSettings.ChangeLogVersion.Value = ClydeVersion.Current;
        }

        RenderUtils.DrawRect(new Vector2(0, 30), new Vector2(windowRect.width, windowRect.height - 30), panelColor);

        scrollPos = GUI.BeginScrollView(
            new Rect(10, 35, windowRect.width - 20, windowRect.height - 45),
            scrollPos,
            new Rect(0, 0, windowRect.width - 40, 1000),
            false,
            true
        );

        IReadOnlyList<ClydeChangeEntry> changes = ClydeVersion.Get(ClydeVersion.Current);

        if (changes.Count == 0)
            RenderUtils.DrawString(new Vector2(10, 10), "No changelog entries for this update. (Report this in the discord)", textColor, 16);
        else
        {
            float yOffset = 10;
            foreach (var change in changes)
            {
                var prefix = change.IsDebug ? "[Debug] " : "[Change] ";
                RenderUtils.DrawString(new Vector2(10, yOffset), prefix + change.Description, textColor, 16);
                yOffset += 26;
            }
        }

        GUI.EndScrollView();

        GUI.DragWindow(new Rect(0, 0, windowRect.width, 30));
    }
}
