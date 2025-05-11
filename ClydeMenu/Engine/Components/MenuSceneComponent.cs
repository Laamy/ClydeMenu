namespace ClydeMenu.Engine.Menu;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

internal class MenuSceneComponent : MonoBehaviour
{
    public static MenuSceneComponent Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            Debug.LogWarning("MenuScene instance already exists. Destroying duplicate.");
            return;
        }

        DontDestroyOnLoad(this);
        Instance = this;
    }

    private readonly List<BaseMenu> baseMenus = [];

    public BaseMenu FocusedMenu { get; private set; }

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
        Console.WriteLine($"Pushed menu: {menu?.GetType().Name}");

        FocusedMenu = menu;

        RenderUtils.SetCursorState(true);
    }

    public void PopMenu(BaseMenu menu)
    {
        if (menu == FocusedMenu)
            FocusedMenu = baseMenus.Count > 1 ? baseMenus[baseMenus.Count - 2] : null;

        baseMenus.Remove(menu);

        if (baseMenus.Count == 0)
            RenderUtils.SetCursorState(false);
    }

    public void Update()
    {
        if (baseMenus.Count != 0)
            RenderUtils.SetCursorState(true);

        for (int i = baseMenus.Count - 1; i >= 0; i--)
        {
            BaseMenu menu = baseMenus[i];
            menu.OnUpdate();
            if (menu.ShouldBlockInput)
                break;
        }
    }

    public void FixedUpdate()
    {
    }

    public void OnGUI()
    {
        foreach (var menu in baseMenus)
        {
            if (menu.ShouldDarkenBackground)
            {
                GUI.color = new Color(0, 0, 0, 0.1f);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                GUI.color = Color.white;
            }

            menu.Render();
        }

        if (baseMenus.Count != 0)
        {
            var curEvent = Event.current;
            Vector2 mousePos = curEvent.mousePosition;

            // basic debug cursor
            int size = 2;
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(mousePos.x - size - 2, mousePos.y - size - 2, size * 2 + 4, size * 2 + 4), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(mousePos.x - size, mousePos.y - size, size * 2, size * 2), Texture2D.whiteTexture);
        }
    }
}
