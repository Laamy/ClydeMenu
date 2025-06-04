namespace ClydeMenu.Engine;

using System;

using UnityEngine;

using ClydeMenu.Engine.Utils;
using System.Collections.Generic;

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

    public static void DrawRectBorder(Vector2 pos, Vector2 size, Color color, int width = 1)
    {
        GUI.color = color;
        GUI.DrawTexture(new Rect(pos.x, pos.y, size.x, width), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(pos.x, pos.y + size.y - width, size.x, width), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(pos.x, pos.y, width, size.y), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(pos.x + size.x - width, pos.y, width, size.y), Texture2D.whiteTexture);
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

        bool visible = true;
        Vector2[] sVertices = new Vector2[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3 p = Camera.main.WorldToScreenPoint(vertices[i]);
            if (p.z <= 0 || p.z > 1000)
                return;
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

    private static Dictionary<int, float> fadeTimer = new();

    [ClydeChange("Waypoints now fade in/out when cursor is near or over it", ClydeVersion.Release_v1_4)]
    public static void DrawWaypoint(Storage.WaypointInfo waypoint)
    {
        var point = waypoint.Position;
        var label = waypoint.Label;
        var color = waypoint.Color;
        var id = waypoint.GetId();

        if (!fadeTimer.ContainsKey(id))
            fadeTimer[id] = 0;

        var localPlayer = ClientInstance.GetLocalPlayer();
        if (localPlayer == null || localPlayer.gameObject == null)
            return;
        var plyrPos = localPlayer.gameObject.transform.position;

        var distance = Vector3.Distance(plyrPos, point);

        if (distance == 0)
            distance = 1;

        var realDis = distance;

        Vector3 p = Camera.main.WorldToScreenPoint(point);
        if (p.z <= 0 || p.z > 1000)
            return;

        Vector2 pos = new Vector2(p.x,  p.y);

        var scaleX = (float)Screen.width / Camera.main.pixelWidth;
        var scaleY = (float)Screen.height / Camera.main.pixelHeight;
        Vector2 scaledScreen = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

        //Console.WriteLine($"{Vector2.Distance(center, scaledScreen)}");
        //var nearCursor = Vector2.Distance(center, scaledScreen) < 50;
        //var baseAlpha = color.a;
        //
        //if (!nearCursor)
        //    baseAlpha -= 0.3f;
        var scaleFactor = Mathf.Clamp(distance / 250, 0.02f, 0.05f);
        var nearCursor = Vector2.Distance(pos, scaledScreen / 2) < 4 / scaleFactor;

        var speed = 3;
        fadeTimer[id] = Mathf.Clamp01(fadeTimer[id] + (nearCursor ? -(Time.deltaTime * speed) : Time.deltaTime * speed));

        var baseAlpha = Mathf.Lerp(0.8f, 0.3f, fadeTimer[id]);

        var distanceScale = Mathf.Lerp(0.8f, 1.2f, fadeTimer[id]);
        distance *= distanceScale;
        scaleFactor = Mathf.Clamp(distance / 250, 0.02f, 0.05f);

        //DrawString(new Vector2(p.x, 20 + Screen.height - p.y), $"Center: {_center}\r\nPos: {pos}\r\nDis: {Vector2.Distance(pos, _center)}", new Color(0.9f, 0.9f, 0.9f), 16);

        var size = 2 / scaleFactor;
        var border = size / 3;
        Vector2 center = new Vector2(p.x * scaleX, Screen.height - p.y * scaleY);

        var orig = GUI.matrix;
        GUIUtility.RotateAroundPivot(45, center);
        var modAlpha = baseAlpha - 0.3f;
        if (modAlpha <= 0)
            modAlpha = 0.1f;
        DrawRect(center - new Vector2(border/2, border/2), new Vector2(size + border, size + border), new Color(color.r, color.g, color.b, modAlpha));
        DrawRect(center, new Vector2(size, size), new Color(color.r, color.g, color.b, baseAlpha));
        GUI.matrix = orig;

        // draw string at center of diamond tghank u game !
        var disLabel = $"{Math.Floor(realDis)}m";
        var measure = StringSize(disLabel, 1 / scaleFactor);
        DrawString(center - (measure/2) + new Vector2(0, size / 1.5f), disLabel, new Color(0.9f, 0.9f, 0.9f, baseAlpha), 1 / scaleFactor);

        measure = StringSize(label, 0.7f / scaleFactor);
        DrawString(center - (measure / 2) + new Vector2(0, size / 1.5f - size*1.2f), label, new Color(0.9f, 0.9f, 0.9f, baseAlpha), 0.7f / scaleFactor);
    }

    public static void SetCursorState(bool visible)
    {
        //Entry.Log($"Setting cursor state to {visible}");
        // gotta fix the cursor bruh
        var info = ClientInstance.FetchField<InputManager>("disableAimingTimer");
        if (info == null)
            return;

        if (visible)
            info.SetValue(InputManager.instance, 0.1f);

        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    public static class Window
    {
        /// <summary>
        /// Creates a clipping bounds with a visual draggable component for HUD editors
        /// </summary>
        /// <returns>Position the window ends up at (if draggable is specified)</returns>
        public static Vector2 Start(bool draggable, Rect window)
        {
            GUI.BeginClip(window);

            if (draggable)
            {
                // draw simple bounds
                DrawRect(window.position, window.size, new Color(0.2f, 0.2f, 0.2f, 0.5f));
                DrawRectBorder(window.position, window.size, new Color(0.5f, 0.5f, 0.5f, 0.5f), 2);

                return window.position; // placeholder for dragging code
            }

            return window.position;
        }

        public static void End()
        {
            GUI.EndClip();
        }
    }
}
