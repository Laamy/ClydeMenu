namespace ClydeMenu.Engine;

using ClydeMenu.Engine.Settings;
using UnityEngine;

internal class MainMenuController
{
    internal static void Render()
    {
        var currency = $"${MenuSettings.Currency.Value}K";
        
        RenderUtils.DrawRect(new Vector2(Screen.width - 120, 10), new Vector2(110, 30), new Color(0.1f, 0.1f, 0.1f, 1));
        RenderUtils.DrawRectBorder(new Vector2(Screen.width - 120, 10), new Vector2(110, 30), Color.white, 4);
        var strMs = RenderUtils.StringSize(currency);
        RenderUtils.DrawString(new Vector2(Screen.width - 120 + (110 - strMs.x) / 2 + 10, 10 + (30 - strMs.y) / 2), currency, Color.white, 16);
        
        // diamond
        RenderUtils.DrawLine(new Vector2(Screen.width - 120 + 10, 10 + 15), new Vector2(Screen.width - 120 + 20, 10 + 5), Color.yellow, 2);
        RenderUtils.DrawLine(new Vector2(Screen.width - 120 + 20, 10 + 5), new Vector2(Screen.width - 120 + 30, 10 + 15), Color.yellow, 2);
        RenderUtils.DrawLine(new Vector2(Screen.width - 120 + 30, 10 + 15), new Vector2(Screen.width - 120 + 20, 10 + 25), Color.yellow, 2);
        RenderUtils.DrawLine(new Vector2(Screen.width - 120 + 20, 10 + 25), new Vector2(Screen.width - 120 + 10, 10 + 15), Color.yellow, 2);
    }
}
