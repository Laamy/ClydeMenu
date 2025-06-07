namespace ClydeMenu.Engine.Commands;

using System.Collections.Generic;
using UnityEngine;
using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Settings;
using Unity.VisualScripting;

[ClydeChange("Clean layout for WASD + Space keystrokes", ClydeVersion.Release_v1_7_1)]
public class KeystrokesModule : BaseModule
{
    public KeystrokesModule() : base("Keystrokes", "Displays WASD + Space keys in layout", "Visual") { }

    private Rect keyBounds = new(300, Screen.height-200, 160, 200);
    private readonly int padding = 5;
    private readonly int fontSize = 22;
    private readonly float keySize = 45;
    private readonly float spacing = 3;

    private struct KeyBox
    {
        public string Label;
        public KeyCode Code;
        public Rect Rect;
    }

    [ClydeChange("New Keystrokes module that displays WASD + Space keys in a clean layout", ClydeVersion.Release_v1_7_1)]
    public override void OnRender()
    {
        if (!MenuSettings.Keystrokes.Value)
            return;

        using (RenderUtils.Window.Begin(MenuSceneComponent.IsMenuOpen(), ref keyBounds, "Keystrokes"))
        {
            var keys = new List<KeyBox>
            {
                new() { Label = "W", Code = KeyCode.W, Rect = Box(1, 0) },
                new() { Label = "A", Code = KeyCode.A, Rect = Box(0, 1) },
                new() { Label = "S", Code = KeyCode.S, Rect = Box(1, 1) },
                new() { Label = "D", Code = KeyCode.D, Rect = Box(2, 1) },
                new() { Label = "␣", Code = KeyCode.Space, Rect = new Rect(padding, Y(2), Width(3), keySize) },
                new() { Label = "↑", Code = KeyCode.LeftShift, Rect = Box(0, 3) },
                new() { Label = "^", Code = KeyCode.LeftControl, Rect = Box(1, 3) },
                new() { Label = "Q", Code = KeyCode.Q, Rect = Box(2, 3) },
            };

            Color background = Storage.StyleTheme.SidebarButton; background.a = 0.8f;
            Color heldBackground = Storage.StyleTheme.Sidebar; heldBackground.a = 0.8f;
            Color softText = Storage.StyleTheme.MenuText;
            Color hardText = Storage.StyleTheme.MenuTextSelected;

            foreach (var key in keys)
            {
                RenderUtils.DrawRect(key.Rect.position, key.Rect.size, Input.GetKey(key.Code) ? background : heldBackground);
                var color = Input.GetKey(key.Code) ? softText : hardText;
                var size = RenderUtils.StringSize(key.Label, fontSize);
                var pos = new Vector2(key.Rect.x + (key.Rect.width - size.x) / 2f, key.Rect.y + (key.Rect.height - size.y) / 2f);
                RenderUtils.DrawString(pos, key.Label, color, fontSize);
            }
        }
    }

    private Rect Box(int x, int y) => new(padding + x * (keySize + spacing), Y(y), keySize, keySize);
    private float Y(int row) => padding + row * (keySize + spacing);
    private float Width(int span) => (keySize + spacing) * span - spacing;
    public override void Initialize()
    {
        Storage.StyleTheme = ClientInstance.GetClientTheme();
        IsEnabled = true;
    }
}
