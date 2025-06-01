namespace ClydeMenu.Engine;

using UnityEngine;

using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Commands;
using ClydeMenu.Engine.Components;
using ClydeMenu.Engine.Settings;
using ClydeMenu.Engine.Menu.Menus;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

public class ClientComponent : BaseComponent
{
    public bool isShown = false;

    public ClientComponent()
    {
        if (MenuSettings.ChangeLogVersion.Value != ClydeVersion.Current)
            MenuSceneComponent.Instance.PushOrPopMenuByType<ChangeLogMenu>();

        var win = WindowManager.FindWindow(null, "R.E.P.O.");
        if (win != null)
            WindowManager.SetWindowText(WindowManager.FindWindow(null, "R.E.P.O."), $"R.E.P.O. {BuildManager.instance.version.title}    |    ClydeMenu {ClydeVersion.ToVersionString(ClydeVersion.Current)}");

        //RenderUtils.Init();
        Entry.Log("ClydeMenu initialized");
    }

    public void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            //private List<EnemySetup> enemyList
            //var enemyList = ClientInstance.GetEnemyList();
        }
        
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

                //foreach (var valuable in ClientInstance.GetValuableList())
                //{
                //    var bounds = ClientInstance.GetActiveColliderBounds(valuable.gameObject);
                //    RenderUtils.DrawWaypoint(valuable.gameObject.transform.position + (bounds.size/2), valuable.gameObject.name, Color.yellow);
                //}
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
