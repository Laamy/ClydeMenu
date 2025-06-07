namespace ClydeMenu.Engine;

using UnityEngine;

using System.Collections.Generic;

using ClydeMenu.Engine.Components;
using ClydeMenu.Engine.Menu;

public class RichChatComponent : BaseComponent
{
    public static RichChatComponent instance;

    public RichChatComponent()
    {
        instance = this;

        chatBounds.position = new Vector2(Screen.width - chatBounds.width - 10, Screen.height - chatBounds.height - 10);
        PostSystemMessage("Welcome back!");
    }

    public List<(string msg, string user)> richChatMessages = new()
    {
        // Example messages, replace with actual message data
        
    };

    public static void PostSystemMessage(string msg)
    {
        instance.richChatMessages.Add((msg, "ClydeMenu"));
    }

    public Rect chatBounds = new(100, 100, 300, 300);
    public int padding = 5;

    public override void OnGUI()
    {
        if (richChatMessages.Count == 0)
            return;

        if (richChatMessages.Count >= 18)
            richChatMessages.RemoveAt(0);

        RenderUtils.Window.Start(MenuSceneComponent.IsMenuOpen(), ref chatBounds);
        {
            int y = 0;
            foreach (var (msg, user) in richChatMessages)
            {
                var fullMsg = $"<{user}> {msg}";
                var lines = WrapText(fullMsg, chatBounds.width - (padding*2));

                var msr = RenderUtils.StringSize(fullMsg);
                foreach (var line in lines)
                {
                    RenderUtils.DrawString(new Vector2(padding, padding + y), line, new Color(0.8f, 0.8f, 0.8f));
                    y += (int)(msr.y + padding);
                }
            }
        }
        RenderUtils.Window.End();
    }

    string[] WrapText(string text, float maxWidth, float textSize = 16)
    {
        List<string> lines = new();
        var words = text.Split(' ');

        var currentLine = "";
        foreach (var word in words)
        {
            var testLine = currentLine.Length > 0 ? (currentLine + " " + word) : word;
            Vector2 size = RenderUtils.StringSize(testLine, textSize);
            if (size.x > maxWidth)
            {
                if (!string.IsNullOrEmpty(currentLine))
                    lines.Add(currentLine);
                currentLine = word;
            }
            else currentLine = testLine;
        }

        if (!string.IsNullOrEmpty(currentLine))
            lines.Add(currentLine);

        return lines.ToArray();
    }
}
