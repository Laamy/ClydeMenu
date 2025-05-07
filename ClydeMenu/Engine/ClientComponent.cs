namespace ClydeMenu.Engine;

using System;
using System.Diagnostics;

using UnityEngine;

public class ClientComponent : MonoBehaviour
{
    public bool isShown = false;

    public void Start()
    {
        Render.Init();
        Render.SetCursorState(isShown);

        ClientInstance.Init(this);
    }

    public void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            isShown = !isShown;
            Render.SetCursorState(isShown);
            Console.WriteLine($"Menu is now {(isShown ? "shown" : "hidden")}");
        }

        if (Input.GetKeyDown(KeyCode.Delete))
            Process.GetCurrentProcess().Kill();

        if (Input.GetKeyDown(KeyCode.F12))
            Entry.Unload();//bro
    }

    public void Update()
    {
        Render.SetCursorState(isShown);
        HandleInputs();
    }

    public bool isInitialized = false;
    internal RenderWindow Main;

    public void OnGUI()
    {
        if (!isInitialized)
        {
            Start();
            isInitialized = true;

            Main = Render.Window("ClydeMenu", new Rect(10, 10, 600, 600));
        }

        if (isShown)
        {
            Main.HandleBorder();
            Main.Tabs(["Player", "Spawn"], [
                () => {
                    //godmode
                    Main.Toggle(new Vector2(10, 10), "Godmode", Storage.Godmode, (bool enabled) => {
                        Storage.Godmode = enabled;

                        if (enabled)
                            ClientInstance.SetHealth(99999, 99999);
                        else ClientInstance.SetHealth(100, 100);//TODO: make this actually be maxhealth and not a fake
                    });
                },
                () => {
                    Main.Button(new Vector2(10, 10), "Big Orb", () => {
                        GameObject plyr = ClientInstance.GetLocalPlayer();
                        if (plyr == null)
                        {
                            Console.WriteLine("Player not found");
                            return;
                        }
                        Vector3 targetPos = plyr.transform.position + plyr.transform.forward * 2f;
                        ItemUtils.SpawnEnemyOrb(targetPos);
                        Console.WriteLine($"Spawned enemy orb at {targetPos}");
                    });

                    Main.Button(new Vector2(100, 10), "Money Bag", () => {
                        GameObject plyr = ClientInstance.GetLocalPlayer();
                        if (plyr == null)
                        {
                            Console.WriteLine("Player not found");
                            return;
                        }
                        Vector3 targetPos = plyr.transform.position + plyr.transform.forward * 2f;
                        ItemUtils.SpawnSurplus(targetPos);
                        Console.WriteLine($"Spawned money bag at {targetPos}");
                    });

                    Main.Button(new Vector2(200, 10), "Crash", () => {
                        for (var i = 0; i < 0xFFF; i++)
                            ItemUtils.SpawnSurplus(Vector2.zero, false);
                        Console.WriteLine($"crash attempted !!!");
                    });
                }]
            );
        }
    }
}
