namespace ClydeMenu.Engine;

using UnityEngine;

using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Commands;
using ClydeMenu.Engine.Components;
using ClydeMenu.Engine.Settings;
using ClydeMenu.Engine.Menu.Menus;
using System;
using Object = UnityEngine.Object;

public static class MonoBehaviourCache
{
    private static MonoBehaviour[] cache;
    private static float lastUpdateTime = -1f;
    private static readonly float updateInterval = 0.5f;

    public static MonoBehaviour[] All
    {
        get
        {
            if (cache == null || Time.realtimeSinceStartup - lastUpdateTime > updateInterval)
            {
                cache = Object.FindObjectsOfType<MonoBehaviour>();
                lastUpdateTime = Time.realtimeSinceStartup;
            }
            return cache;
        }
    }
}

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
        if (Input.GetKeyDown(KeyCode.F8))
        {
            if (SemiFunc.MenuLevel())
            {
                DebugWorld.Load();
            }
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            MenuSettings.Currency.Value += 1;
            Console.WriteLine($"Currency increased to {MenuSettings.Currency.Value}k");
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameObject go = new GameObject("TopLine");
            LineRenderer lr = go.AddComponent<LineRenderer>();

            lr.positionCount = 2;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.useWorldSpace = true;

            Material mat = new Material(Shader.Find("Hidden/Internal-Colored"));
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

            mat.SetColor("_Color", Color.red);

            lr.SetPosition(0, new Vector3(0, 0, 0));
            lr.SetPosition(1, new Vector3(100, 100, 100));

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

        Clocks.Tick();
    }

    public bool isInitialized = false;

    public override void OnGUI()
    {
        if (!isInitialized)
        {
            GUI.skin.font = Font.CreateDynamicFontFromOSFont("Consolas", 48); // might switch to Segoe UI
            isInitialized = true;
        }

        //foreach (MonoBehaviour mb in MonoBehaviourCache.All)
        //{
        //    var method = mb.GetType().GetMethod("OnDrawGizmos",
        //        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
        //    if (method != null)
        //    {
        //        method.Invoke(mb, null);
        //    }
        //}

        // esps (DEBUGGING!)
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
                try
                {
                    foreach (var valuable in ClientInstance.GetValuableList())
                        RenderUtils.DrawAABB(ClientInstance.GetActiveColliderBounds(valuable.gameObject), Color.yellow);
                }
                catch { }

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

        if (Clocks.IsAlive("MainMenuUpdate"))
            MainMenuController.Render();
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
