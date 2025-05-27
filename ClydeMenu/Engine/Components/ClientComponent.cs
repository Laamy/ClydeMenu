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

        RenderVisualThing();

        try
        {
            SortNoiseCrap();
            RenderNoiseCrap();
        }
        catch { }
    }

    const int padding = 6;
    // TODO: move these to their own modules
    private void RenderNoiseCrap()
    {
        lock (Patches.audioStack)
        {
            var y = 20 + padding;
            foreach (var audio in Patches.audioStack)
            {
                if (audio.time < Time.time - 5f)
                    continue;

                var measure = RenderUtils.StringSize(audio.label, 16);
                RenderUtils.DrawString(new Vector2(
                    Screen.width - measure.x - padding,
                    Screen.height - y
                ), audio.label, Color.white, 16);
                y += 20;
            }
        }
    }

    private void SortNoiseCrap()
    {
        lock (Patches.audioStack)
        {
            foreach (var audio in Patches.audioStack)
            {
                if (audio.time < Time.time - 5f)
                    Patches.audioStack.Remove(audio);
            }
        }
    }

    private void RenderVisualThing()
    {
        if (!MenuSettings.VISUAL_MAPINFO.Value)
            return;

        if (RoundDirector.instance == null)
            return;

        var haulGoalMax = ClientInstance.FetchFieldValue<int, RoundDirector>("haulGoal", RoundDirector.instance);
        if (haulGoalMax == 0)
            return;
        float num = haulGoalMax / ClientInstance.FetchFieldValue<int, RoundDirector>("extractionPoints", RoundDirector.instance);

        var curLevel = RunManager.instance.levelsCompleted;
        num *= quotaMultiply[Mathf.Clamp(curLevel, 0, quotaMultiply.Length - 1)];

        RenderUtils.DrawString(new Vector2(10, 10), $"Min:{haulGoalMax} | Max:{(int)num}\r\nLvl:{curLevel + 1}", Color.white, 20);
    }

    /*level 1: multiply quota by 3.57142851821014
level 2: multiply quota by 4.76190457268368
level 3: multiply quota by 4.74639982908994
level 4: multiply quota by 7.07604922227163
level 5: multiply quota by 7.00606421609719
level 6: multiply quota by 9.20059248925003
level 7: multiply quota by 8.99406905420288
level 8: multiply quota by 8.70545410147724
level 9: multiply quota by 8.39388691548115
level 10: multiply quota by 8.21523722771264
level 11+: multiply quota by 8.1699877074361  */

    public float[] quotaMultiply = [
        3.57142851821014f,
        4.76190457268368f,
        4.74639982908994f,
        7.07604922227163f,
        7.00606421609719f,
        9.20059248925003f,
        8.99406905420288f,
        8.70545410147724f,
        8.39388691548115f,
        8.21523722771264f,
        8.1699877074361f
    ];

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
