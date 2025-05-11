namespace ClydeMenu.Engine;

using System;

using UnityEngine;

using ClydeMenu.Engine.Menu;
using Unity.VisualScripting;
using ClydeMenu.Engine.Components.Visuals;
using Photon.Realtime;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using UnityEngine.UIElements;

public class ClientComponent : MonoBehaviour
{
    public bool isShown = false;

    public void Start()
    {
        RenderUtils.Init();
        RenderUtils.SetCursorState(isShown);

        Console.WriteLine("ClydeMenu initialized");
    }

    public void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            MenuSceneComponent.Instance.PushOrPopMenuByType<DebugMenu>();
            Console.WriteLine($"Menu is now {(MenuSceneComponent.Instance.HasMenuByType<DebugMenu>() ? "shown" : "hidden")}");
        }

        // im gonna get so mad if i press this 1 more fucking time
        //if (Input.GetKeyDown(KeyCode.Delete))
        //    Process.GetCurrentProcess().Kill();

        if (Input.GetKeyDown(KeyCode.F12))
            Entry.Unload();//bro
    }

    public void Update()
    {
        HandleInputs();
    }

    public bool isInitialized = false;

    public void OnGUI()
    {
        if (!isInitialized)
        {
            Start();
            isInitialized = true;
        }

        var localPlayer = ClientInstance.GetLocalPlayer();
        if (localPlayer == null)
        {
            Console.WriteLine("Local player not found");
            return;
        }

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
