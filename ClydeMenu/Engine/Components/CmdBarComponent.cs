namespace ClydeMenu.Engine;

using System;

using UnityEngine;

using ClydeMenu.Engine.Commands;
using ClydeMenu.Engine.Components;

public class CmdBarComponent : BaseComponent
{
    public bool isCmdBar = false;

    public CmdBarComponent()
    {
        Console.WriteLine("CmdBar initialized");
    }

    public void HandleInputs()
    {
        // cmdbar
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            //isCmdBar = !isCmdBar;
            //Console.WriteLine($"CmdBar is now {(isCmdBar ? "shown" : "hidden")}");

            // SemiFunc.InputDisableMovement();
            //_ = isCmdBar ? SemiFunc.InputDisableMovement() : SemiFunc.input();
        }
    }

    public override void Update()
    {
        HandleInputs();
    }

    public bool isInitialized = false;

    public override void OnGUI()
    {
        if (!isCmdBar)
            return;

        GUI.SetNextControlName("CmdBarField");

        Rect textbox = new Rect(
            (Screen.width / 2) - (Screen.width / 2),
            Screen.height - 60,
            Screen.width,
            60
        );
        Storage.CmdBarInput = GUI.TextField(textbox, Storage.CmdBarInput, RenderUtils.CurrentTheme);

        if (Storage.CmdBarJustShown)
        {
            Storage.CmdBarJustShown = false;
            GUI.FocusControl("CmdBarField");
        }

        void CloseCmdbar()
        {
            Storage.CmdBarHistory.Insert(0, Storage.CmdBarInput);
            Storage.CmdBarHistoryIndex = -1;
            Storage.CmdBarInput = string.Empty;
            isCmdBar = false;
        }

        if (Event.current.rawType == EventType.Used)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.Return:
                    try {
                        CmdHandler.Handle(Storage.CmdBarInput);
                        CloseCmdbar();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error in CmdBar event polling: {e.Message}");
                        CloseCmdbar();
                    }
                    break;
                case KeyCode.Escape:
                    CloseCmdbar();
                    break;

                case KeyCode.UpArrow:
                    Storage.CmdBarHistoryIndex = Mathf.Clamp(Storage.CmdBarHistoryIndex + 1, -1, Storage.CmdBarHistory.Count - 1);
                    if (Storage.CmdBarHistoryIndex != -1)
                    {
                        Storage.CmdBarInput = Storage.CmdBarHistory[Storage.CmdBarHistoryIndex];
                        Storage.CmdBarJustShown = true;
                    }
                    break;
                case KeyCode.DownArrow:
                    Storage.CmdBarHistoryIndex = Mathf.Clamp(Storage.CmdBarHistoryIndex - 1, -1, Storage.CmdBarHistory.Count - 1);
                    if (Storage.CmdBarHistoryIndex != -1)
                    {
                        Storage.CmdBarInput = Storage.CmdBarHistory[Storage.CmdBarHistoryIndex];
                        Storage.CmdBarJustShown = true;
                    }
                    else Storage.CmdBarInput = string.Empty;
                    break;
            }
        }
        else Storage.CmdBarJustShown = true;
    }
}
