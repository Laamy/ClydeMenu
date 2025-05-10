namespace ClydeMenu.Engine;

using System;
using System.Reflection;

using UnityEngine;

using ClydeMenu.Engine.Utils;

public class RenderWindow
{
    public const float TitlebarHeight = 16;
    public const float TabHeight = 45;

    public RenderWindow(Rect location, string title, GUIStyle style)
    {
        Location = location;
        Title = title;

        MainStyle = style;

        var invert = style.normal.background.GetPixel(0, 0);
        Color color = new Color(1 - invert.r, 1 - invert.g, 1 - invert.b, invert.a);

        InvertStyle = new GUIStyle(style)
        {
            normal = {
                background = TextureUtils.CreateSolid(color),
                textColor = style.normal.textColor
            }
        };
    }

    public Rect Location { get; set; }
    public string Title { get; set; }

    public GUIStyle MainStyle { get; set; }
    public GUIStyle InvertStyle { get; set; }

    #region Drawing utils

    public void Box(Rect rect, string v)
    {
        rect = MakeRelative(rect);
        GUI.Box(rect, v, MainStyle);
    }

    private void ErrorHandle(Action action)
    {
        try
        {
            action?.Invoke();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in GUI: {e.Message}");
        }
    }

    public Vector2 MeasureString(string v)
        => MainStyle.CalcSize(new GUIContent(v));

    public void Button(Vector2 pos, string v, Action action)
    {
        int padding = 5;

        Vector2 size = MeasureString(v);
        Rect rect = new Rect(pos.x, pos.y, size.x + (padding * 2), size.y + (padding * 2));
        rect = MakeRelative(rect);

        if (GUI.Button(rect, v, MainStyle))
            ErrorHandle(() => action?.Invoke());
    }

    public void Toggle(Vector2 pos, string v, bool state, Action<bool> action)
    {
        int padding = 5;
        Vector2 size = MeasureString(v);
        Rect rect = new Rect(pos.x, pos.y, size.x + (padding * 2), size.y + (padding * 2));
        rect = MakeRelative(rect);
        if (GUI.Button(rect, v, state ? InvertStyle : MainStyle))
            ErrorHandle(() => action?.Invoke(!state));
    }

    private int currentTab = 0;
    public void Tabs(string[] names, Action[] drawActions)
    {
        // im gonna assume that if this is called that the window wont have anything in it
        if (names.Length != drawActions.Length)
            throw new ArgumentException("Tabs names and actions must be the same length");

        var oldLoc = Location;
        Location = new Rect(Location.x, Location.y + TitlebarHeight, Location.width, Location.height - TitlebarHeight);

        float tabWidth = Location.width / names.Length;
        for (int i = 0; i < names.Length; i++)
        {
            Rect tabRect = new Rect(Location.x + (i * tabWidth), Location.y, tabWidth, TabHeight);
            if (GUI.Button(tabRect, names[i], MainStyle))
                currentTab = i;
        }

        Location = new Rect(Location.x, Location.y + TabHeight, Location.width, Location.height - TabHeight);

        ErrorHandle(() => drawActions[currentTab].Invoke());

        Location = oldLoc;
    }

    #endregion

    #region Functional

    public Rect MakeRelative(Rect rect)
    {
        rect.x += Location.x;
        rect.y += Location.y + TitlebarHeight;
        return rect;
    }

    private void SetWinRect(Rect rect)
    {
        rect.x = Mathf.Clamp(rect.x, 0, Screen.width - rect.width);
        rect.y = Mathf.Clamp(rect.y, 0, Screen.height - rect.height);
        Location = rect;
    }

    private Vector2 oldMouse;
    private bool isDragging = false;
    public void HandleBorder()
    {
        GUI.Box(Location, "", MainStyle);
        GUI.Box(new Rect(Location.x, Location.y, Location.width, TitlebarHeight), "", MainStyle);// unity does some dogshit clipping

        // handle mouse dragging
        var curEvent = Event.current;
        Vector2 mousePos = curEvent.mousePosition;

        Rect titleBarRect = new Rect(Location.x, Location.y, Location.width, TitlebarHeight);

        switch (curEvent.type)
        {
            case EventType.MouseDown:
                if (titleBarRect.Contains(mousePos))
                {
                    isDragging = true;
                    oldMouse = mousePos;
                    curEvent.Use();
                }
                break;
            case EventType.MouseDrag:
                if (isDragging)
                {
                    Vector2 delta = mousePos - oldMouse;
                    Rect newRect = new Rect(Location.x + delta.x, Location.y + delta.y, Location.width, Location.height);
                    SetWinRect(newRect);
                    oldMouse = mousePos;
                    curEvent.Use();
                }
                break;
            case EventType.MouseUp:
                if (isDragging)
                {
                    isDragging = false;
                    curEvent.Use();
                }
                break;
        }
    }

    internal void Toggle(Vector2 vector2, string v, object antiKick, Action<bool> value) => throw new NotImplementedException();

    #endregion
}

public class RenderUtils
{
    public static GUIStyle CurrentTheme;

    public static void Init()
    {
        CurrentTheme = new GUIStyle(GUI.skin.box)
        {
            normal = {
                background = TextureUtils.CreateSolid(48, 48, 48, 170),
                textColor = new Color(1, 1, 1, 0.8f)
            },
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter,
            border = new RectOffset(5, 5, 5, 5)
        };
    }

    public static void SetCursorState(bool visible)
    {
        Console.WriteLine($"Setting cursor state to {visible}");

        // gotta fix the cursor bruh
        Type type = typeof(InputManager);
        FieldInfo info = type.GetField("disableAimingTimer", BindingFlags.NonPublic | BindingFlags.Instance);
        if (info == null)
        {
            Console.WriteLine("Failed to find disableAimingTimer field");
            return;
        }

        float curValue = (float)info.GetValue(InputManager.instance);
        
        if (visible)
        {
            if (curValue < 2f || curValue > 10f)
            {
                float clampValue = Mathf.Clamp(curValue, 2f, 10f);
                info.SetValue(InputManager.instance, clampValue);
            }
        }
        if (Cursor.visible == visible)
            return;

        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;

        if (!visible)
            info.SetValue(InputManager.instance, 0f);
    }

    public static RenderWindow Window(string v, Rect rect) => new(rect, v, CurrentTheme);
}
