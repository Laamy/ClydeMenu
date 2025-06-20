﻿using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Settings;
using Photon.Pun;
using System.Collections.Generic;

using UnityEngine;

namespace ClydeMenu.Engine.Commands;

[ClydeChange("New togglable RichChat that logs & displays chat messages", ClydeVersion.Release_v1_6_1)]
public class RichChatModule : BaseModule
{
    public RichChatModule() : base("RichChat", "", "Visual") { }

    public static RichChatModule instance;
    [ClydeChange("Fixed RichChat not being aligned to the bottom of the screen", ClydeVersion.Release_v1_7_1)]
    public override void Initialize()
    {
        IsEnabled = true;
        instance = this;

        chatBounds.position = new Vector2(0, (Screen.height/2) - (chatBounds.height/2));
        PostSystemMessage("Welcome back!");
    }

    public List<(string msg, string user)> richChatMessages = new() { };
    public static void PostSystemMessage(string msg) => instance.richChatMessages.Add((msg, "ClydeMenu"));

    public Rect chatBounds = new(100, 100, 300, 300);
    public int padding = 5;

    [ClydeChange("Fixed RichChat bug where long messages break the layout", ClydeVersion.Release_v1_6_2)]
    public override void OnRender()
    {
        if (!MenuSettings.IsChatOpen.Value)
            return;

        var lineCount = 0;
        foreach (var (msg, user) in richChatMessages)
        {
            var fullMsg = $"<{user}> {msg}";
            var lines = WrapText(fullMsg, chatBounds.width - (padding * 2));
            lineCount += lines.Length;
        }

        while (lineCount >= 16)
        {
            var firstMsg = richChatMessages[0];
            var firstLines = WrapText($"<{firstMsg.user}> {firstMsg.msg}", chatBounds.width - (padding * 2));
            lineCount -= firstLines.Length;
            richChatMessages.RemoveAt(0);
        }

        using (RenderUtils.Window.Begin(MenuSceneComponent.IsMenuOpen(), ref chatBounds, "RichChat"))
        {
            if (richChatMessages.Count == 0)
                return;

            if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
                return;

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
