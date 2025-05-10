namespace ClydeMenu.Engine;

using System;

using UnityEngine;

using ClydeMenu.Engine.Menu;

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
    }
}
