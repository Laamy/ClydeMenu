using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization.Json;
using UnityEngine;

namespace ClydeMenu.Engine.Commands;

[ClydeChange("New togglable RichChat that logs & displays chat messages", ClydeVersion.Release_v1_6_1)]
public class RichChatModule : BaseModule
{
    public RichChatModule() : base("RichChat", "", "Visual") { }

    public static RichChatModule instance;
    public override void Initialize()
    {
        IsEnabled = true;
        instance = this;

        chatBounds.position = new Vector2(0, Screen.height - chatBounds.height - padding);
        PostSystemMessage("Welcome back!");
    }

    public List<(string msg, string user)> richChatMessages = new() { };
    public static void PostSystemMessage(string msg) => instance.richChatMessages.Add((msg, "ClydeMenu"));

    public Rect chatBounds = new(100, 100, 300, 300);
    public int padding = 5;

    public override void OnRender()
    {
        if (!MenuSettings.IsChatOpen.Value)
            return;

        if (richChatMessages.Count == 0)
            return;

        if (richChatMessages.Count >= 18)
            richChatMessages.RemoveAt(0);

        using (RenderUtils.Window.Begin(MenuSceneComponent.IsMenuOpen(), ref chatBounds, "RichChat"))
        {
            var y = 0;
            foreach (var (msg, user) in richChatMessages)
            {
                var fullMsg = $"<{user}> {msg}";
                var lines = WrapText(fullMsg, chatBounds.width - (padding * 2));

                var msr = RenderUtils.StringSize(fullMsg);
                foreach (var line in lines)
                {
                    RenderUtils.DrawString(new Vector2(padding, padding + y), line, new Color(0.8f, 0.8f, 0.8f));
                    y += (int)(msr.y + padding);
                }
            }
        }
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
