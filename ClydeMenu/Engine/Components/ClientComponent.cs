namespace ClydeMenu.Engine;

using UnityEngine;

using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Commands;
using ClydeMenu.Engine.Components;
using ClydeMenu.Engine.Settings;
using System;
using UnityEngine.InputSystem.OnScreen;

public class ClientComponent : BaseComponent
{
    public bool isShown = false;

    public ClientComponent()
    {
        //RenderUtils.Init();
        Entry.Log("ClydeMenu initialized");
    }

    public void HandleInputs()
    {
        //if (Input.GetKeyDown(KeyCode.Delete))
        //{
        //    MenuSceneComponent.Instance.PushOrPopMenuByType<DebugMenu>();
        //    Entry.Log($"Menu is now {(MenuSceneComponent.Instance.HasMenuByType<DebugMenu>() ? "shown" : "hidden")}");
        //}
        
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            MenuSceneComponent.Instance.PushOrPopMenuByType<MainMenu>();
            Entry.Log($"Menu is now {(MenuSceneComponent.Instance.HasMenuByType<MainMenu>() ? "shown" : "hidden")}");
        }

        // im gonna get so mad if i press this 1 more fucking time
        //if (Input.GetKeyDown(KeyCode.Delete))
        //    Process.GetCurrentProcess().Kill();

        //if (Input.GetKeyDown(KeyCode.F12))
        //    Entry.Unload();//bro
    }

    public override void Update()
    {
        HandleInputs();
    }

    public bool isInitialized = false;

    public override void OnGUI()
    {
        if (!isInitialized)
        {
            GUI.skin.font = Font.CreateDynamicFontFromOSFont("Consolas", 48);
            isInitialized = true;
        }

        // esps
        {
            if (MenuSettings.ESP_Player.Value)
                RenderPlayerESP();

            if (MenuSettings.ESP_Enemy.Value)
            {
                foreach (var enemy in ClientInstance.GetEnemyList())
                    RenderUtils.DrawAABB(ClientInstance.GetActiveColliderBounds(enemy.gameObject), Color.red);
            }

            if (MenuSettings.ESP_Valuable.Value)
            {
                foreach (var valuable in ClientInstance.GetValuableList())
                    RenderUtils.DrawAABB(ClientInstance.GetActiveColliderBounds(valuable.gameObject), Color.yellow);
            }

            if (MenuSettings.ESP_Extractions.Value)
            {
                foreach (var extract in ClientInstance.GetExtractionPoints())
                    RenderUtils.DrawAABB(ClientInstance.GetActiveColliderBounds(extract.gameObject), Color.cyan);
            }
        }

        foreach (var mod in ModuleHandler.Modules)
        {
            if (mod.IsEnabled)
                mod.OnRender();
        }

        SortNoiseCrap();
    }

    private void SortNoiseCrap()
    {
        lock (Patches.audioStack)
        {
            foreach (var audio in Patches.audioStack)
            {
                if (audio.time < Time.time - 5f)
                {
                    Patches.audioStack.Remove(audio);
                    return;
                }
            }
        }
    }

    public void RenderPlayerESP()
    {
        var localPlayer = ClientInstance.GetLocalPlayer();
        if (localPlayer == null)
            return;

        var players = SemiFunc.PlayerGetList();
        if (players == null || players.Count == 0)
            return;

        foreach (var player in players)
        {
            if (player.gameObject == localPlayer)
                continue;

            RenderUtils.DrawAABB(ClientInstance.GetActiveColliderBounds(player.gameObject), Color.blue);
        }
    }
}
