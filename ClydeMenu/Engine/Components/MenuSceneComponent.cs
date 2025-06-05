namespace ClydeMenu.Engine.Menu;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using ClydeMenu.Engine.Components;

internal class MenuSceneComponent : BaseComponent
{
    public static MenuSceneComponent Instance { get; private set; } //= new MenuSceneComponent(); // gets overwritten its just so it stops spammin errors

    public MenuSceneComponent()
    {
        if (Instance != null)
        {
            Debug.LogWarning("MenuScene instance already exists.");
            return;
        }
        
        Instance = this;
    }

    private readonly List<BaseMenu> baseMenus = [];

    public bool IsFocused() => baseMenus.Count > 0;

    public bool HasMenuByType(Type menu) => baseMenus.Any(m => m.GetType() == menu);
    public bool HasMenuByType<T>() where T : BaseMenu => baseMenus.Any(m => m.GetType() == typeof(T));
    public bool HasMenu(BaseMenu menu) => baseMenus.Contains(menu);

    public bool PushOrPopMenuByType<T>() where T : BaseMenu
    {
        if (HasMenuByType<T>())
        {
            var menu = baseMenus.First(m => m.GetType() == typeof(T));
            PopMenu(menu);
            return false;
        }
        else
        {
            var menu = Activator.CreateInstance<T>();
            PushMenu(menu);
            return true;
        }
    }

    public void PushMenu(BaseMenu menu)
    {
        baseMenus.Add(menu);
        Entry.Log($"Pushed menu: {menu?.GetType().Name}");

        RenderUtils.SetCursorState(true);
    }

    public void PopMenu(BaseMenu menu)
    {
        baseMenus.Remove(menu);

        if (!IsFocused())
            RenderUtils.SetCursorState(false);
    }

    public override void LateUpdate()
    {
        if (IsFocused())
            RenderUtils.SetCursorState(true);
    }

    public override void Update()
    {
        if (IsFocused())
            RenderUtils.SetCursorState(true);

        for (var i = baseMenus.Count - 1; i >= 0; i--)
        {
            BaseMenu menu = baseMenus[i];
            menu.OnUpdate();
            if (menu.ShouldBlockInput)
                break;
        }
    }

    public override void OnGUI()
    {
        var menus = new List<BaseMenu>(baseMenus);
        foreach (var menu in menus)
        {
            if (menu.ShouldDarkenBackground)
            {
                GUI.color = new Color(0, 0, 0, 0.1f);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                GUI.color = Color.white;
            }

            menu.Render();
        }

        if (IsFocused())
        {
            var curEvent = Event.current;
            Vector2 mousePos = curEvent.mousePosition;

            // basic debug cursor
            var size = 2;
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(mousePos.x - size - 2, mousePos.y - size - 2, size * 2 + 4, size * 2 + 4), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(mousePos.x - size, mousePos.y - size, size * 2, size * 2), Texture2D.whiteTexture);
        }
    }
}
