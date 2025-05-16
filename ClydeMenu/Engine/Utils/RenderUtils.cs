namespace ClydeMenu.Engine;

using System;

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
    public void HandleBorder(bool events = true)
    {
        GUI.Box(Location, "", MainStyle);
        GUI.Box(new Rect(Location.x, Location.y, Location.width, TitlebarHeight), "", MainStyle);// unity does some dogshit clipping

        if (!events)
            return;

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

    public static void DrawRect(Vector2 pos, Vector2 size, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(new Rect(pos.x, pos.y, size.x, size.y), Texture2D.whiteTexture);
    }

    public static Vector2 StringSize(string text, float size = 16)
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = (int)size;

        return style.CalcSize(new GUIContent(text));
    }

    public static void DrawString(Vector2 pos, string text, Color color, float size = 16)
    {
        GUI.color = color;
        GUIStyle style = new GUIStyle();
        style.fontSize = (int)size;
        style.normal.textColor = color;

        GUI.Label(new Rect(pos.x, pos.y, 1000, 1000), text, style);
    }

    public static void DrawLine(Vector2 start, Vector2 end, Color color, float width = 1)
    {
        var distance = Vector2.Distance(start, end);
        var angle = Math.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;

        GUI.color = color;
        var orig = GUI.matrix;
        GUIUtility.RotateAroundPivot((float)angle, start);
        GUI.DrawTexture(new Rect(start.x, start.y, distance, width), Texture2D.whiteTexture);
        GUI.matrix = orig;
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float width = 1)
    {
        Vector2 pos1 = Camera.main.WorldToScreenPoint(start);
        Vector2 pos2 = Camera.main.WorldToScreenPoint(end);
        DrawLine(pos1, pos2, color, width);
    }

    private static int[,] cubeEdges = {
        {0,1},{1,2},{2,3},{3,0},
        {4,5},{5,6},{6,7},{7,4},
        {0,4},{1,5},{2,6},{3,7}
    };

    public static void DrawAABB(Bounds bounds, Color colour, int maxDis = 1000)
    {
        Vector3 min = bounds.min, max = bounds.max;
        Vector3[] vertices = {
            new(min.x, min.y, min.z), new(max.x, min.y, min.z),
            new(max.x, min.y, max.z), new(min.x, min.y, max.z),
            new(min.x, max.y, min.z), new(max.x, max.y, min.z),
            new(max.x, max.y, max.z), new(min.x, max.y, max.z)
        };

        float scaleX = (float)Screen.width / Camera.main.pixelWidth;
        float scaleY = (float)Screen.height / Camera.main.pixelHeight;

        bool visible = false;
        Vector2[] sVertices = new Vector2[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3 p = Camera.main.WorldToScreenPoint(vertices[i]);
            if (p.z > 0 && p.z < maxDis) visible = true;
            sVertices[i] = new(p.x * scaleX, Screen.height - (p.y * scaleY));
        }

        if (!visible)
            return;

        for (int i = 0; i < 12; i++)
        {
            Vector2 start = sVertices[cubeEdges[i, 0]];
            Vector2 end = sVertices[cubeEdges[i, 1]];

            if ((start.x >= 0 && start.x <= Screen.width && start.y >= 0 && start.y <= Screen.height) ||
                (end.x >= 0 && end.x <= Screen.width && end.y >= 0 && end.y <= Screen.height))
                DrawLine(start, end, colour, 3);//grrr....
        }
    }

    public static void SetCursorState(bool visible)
    {
        //Console.WriteLine($"Setting cursor state to {visible}");
        // gotta fix the cursor bruh
        var info = ClientInstance.FetchField<InputManager>("disableAimingTimer");
        var info2 = ClientInstance.FetchField<MenuCursor>("showTimer");
        if (info == null || info2 == null)
            return;

        if (visible)
        {
            info.SetValue(InputManager.instance, 0.1f);
            info2.SetValue(MenuCursor.instance, 0.1f);
        }

        if (Cursor.visible == visible)
            return;

        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public static RenderWindow Window(string v, Rect rect) => new(rect, v, CurrentTheme);
}
