namespace ClydeMenu.Engine;

using UnityEngine;

using ClydeMenu.Engine.Settings;

using System;
using static UnityEngine.UI.Image;
using UnityEngine.UI;

internal class MainMenuController
{
    static MenuButton shop;
    internal static void Prepare(MenuPageMain __instance)
    {
        shop = GameObject.Instantiate(__instance.tutorialButton, __instance.tutorialButton.transform.parent);
        shop.buttonTextString = "Clyde Shop";
        shop.gameObject.SetActive(true);
        var rect = shop.GetComponent<RectTransform>();
        var originalRect = __instance.tutorialButton.GetComponent<RectTransform>();

        rect.anchorMin = originalRect.anchorMin;
        rect.anchorMax = originalRect.anchorMax;
        rect.pivot = originalRect.pivot;
        rect.anchoredPosition = originalRect.anchoredPosition + new Vector2(500, 0);

        typeof(MenuButton).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.Invoke(shop, null);

        shop.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        shop.GetComponent<Button>().onClick.AddListener(() => {
            // some kind of menu
        });

        Console.WriteLine("Clyde Menu button created in main menu");
    }

    internal static void Render()
    {
        var currency = $"{MenuSettings.Currency.Value}K";
        var pos = new Vector2(Screen.width - 110, 10);
        var size = new Vector2(100, 30);

        RenderUtils.DrawRect(pos, size, new Color(0.12f, 0.05f, 0.18f, 1));
        RenderUtils.DrawRectBorder(pos, size, new Color(1, 0.84f, 0, 1), 2);

        var strMs = RenderUtils.StringSize(currency);
        RenderUtils.DrawString(pos + new Vector2((size.x - strMs.x) / 2 + 4, (size.y - strMs.y) / 2), currency, Color.white, 16);

        var center = pos + new Vector2(14, size.y / 2);
        var p1 = center + new Vector2(0, -6);
        var p2 = center + new Vector2(6, 0);
        var p3 = center + new Vector2(0, 6);
        var p4 = center + new Vector2(-6, 0);

        RenderUtils.DrawLine(p1, p2, Color.cyan, 1.5f);
        RenderUtils.DrawLine(p2, p3, Color.cyan, 1.5f);
        RenderUtils.DrawLine(p3, p4, Color.cyan, 1.5f);
        RenderUtils.DrawLine(p4, p1, Color.cyan, 1.5f);

        var mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        if (mouse.x >= pos.x && mouse.x <= pos.x + size.x && mouse.y >= pos.y && mouse.y <= pos.y + size.y)
        {
            var note = "You can get these by extracting items ingame";
            var noteSize = RenderUtils.StringSize(note, 12);
            var noteBoxSize = noteSize + new Vector2(10, 6);
            var notePos = mouse + new Vector2(10, 10);

            if (notePos.x + noteBoxSize.x > Screen.width)
                notePos.x = Screen.width - noteBoxSize.x;
            if (notePos.y + noteBoxSize.y > Screen.height)
                notePos.y = Screen.height - noteBoxSize.y;

            RenderUtils.DrawRect(notePos, noteBoxSize, new Color(0, 0, 0, 0.9f));
            RenderUtils.DrawRectBorder(notePos, noteBoxSize, Color.white, 1);
            RenderUtils.DrawString(notePos + new Vector2(5, 3), note, Color.white, 12);
        }
    }
}
