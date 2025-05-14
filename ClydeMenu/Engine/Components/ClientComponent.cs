namespace ClydeMenu.Engine;

using System;

using UnityEngine;

using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Commands;
using ClydeMenu.Engine.Components;

public class ClientComponent : BaseComponent
{
    public bool isShown = false;

    public ClientComponent()
    {
        //RenderUtils.Init();
        Console.WriteLine("ClydeMenu initialized");
    }

    public void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            MenuSceneComponent.Instance.PushOrPopMenuByType<DebugMenu>();
            Console.WriteLine($"Menu is now {(MenuSceneComponent.Instance.HasMenuByType<DebugMenu>() ? "shown" : "hidden")}");
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            // HotReloadListener gameobject
            // get objecvct by namne in scene
            var hotreload = GameObject.Find("HotReloadListener");
            if (hotreload != null)
            {
                Console.WriteLine("Located hot-reload listener, loaded in debug mode(?).");
            }
            else
            {
                Console.WriteLine("Unable to locate hot-reload listener, reboot in debug mode to access this tool.");
            }
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            MenuSceneComponent.Instance.PushOrPopMenuByType<MainMenu>();
            Console.WriteLine($"Menu is now {(MenuSceneComponent.Instance.HasMenuByType<MainMenu>() ? "shown" : "hidden")}");
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
            if (Storage.CHEAT_ESP_Player)
                RenderPlayerESP();

            if (Storage.CHEAT_ESP_Enemy)
            {
                foreach (var enemy in ClientInstance.GetEnemyList())
                    RenderUtils.DrawAABB(ClientInstance.GetActiveColliderBounds(enemy.gameObject), Color.red);
            }

            if (Storage.CHEAT_ESP_Valuable)
            {
                foreach (var valuable in ClientInstance.GetValuableList())
                    RenderUtils.DrawAABB(ClientInstance.GetActiveColliderBounds(valuable.gameObject), Color.yellow);
            }

            if (Storage.CHEAT_NETWORK_MassCrasher)
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
