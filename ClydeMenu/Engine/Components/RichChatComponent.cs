namespace ClydeMenu.Engine;

using UnityEngine;

using ClydeMenu.Engine.Components;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.Video;
using ClydeMenu.Engine.Menu;
using Unity.VisualScripting;

public class RichChatComponent : BaseComponent
{
    public static RichChatComponent instance;

    public RichChatComponent()
    {
        instance = this;

        chatBounds.position = new Vector2(Screen.width - chatBounds.width - 10, Screen.height - chatBounds.height - 10);
    }

    public List<(string msg, string user)> richChatMessages = new()
    {
        // Example messages, replace with actual message data
        ("Hello, world!", "User1"),
        ("This is a rich chat message.", "User2")
    };

    public Rect chatBounds = new Rect(100, 100, 300, 300);
    public int padding = 5;

    public override void OnGUI()
    {
        if (richChatMessages.Count == 0)
            return;

        if (richChatMessages.Count >= 18)
            richChatMessages.RemoveAt(0);

        RenderUtils.Window.Start((MenuSceneComponent.Instance?.IsFocused()).Value, ref chatBounds);
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
