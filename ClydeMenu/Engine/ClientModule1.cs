namespace ClydeMenu.Engine;

using System;
using System.Diagnostics;

using UnityEngine;

public class ClientModule1 : MonoBehaviour
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
            Log($"Menu is now {(isShown ? "shown" : "hidden")}", Color.white, Color.green);
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

    public static void Log(string message, Color colorMain, Color colorFlash, float size = 16)
    {
        Console.WriteLine(message);
        //if (GameNotification.instance)
        //    GameNotification.instance.BigMessage(message, "thief", size, colorMain, colorFlash);
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
            /*
            GUI.Box(new Rect(10, 10, 200, 100), "ClydeMenu", RenderContext.CurrentTheme);
            GUI.Label(new Rect(20, 40, 180, 20), "Press F1 to hide", RenderContext.CurrentTheme);
            GUI.Label(new Rect(20, 60, 180, 20), "Press F3 to spawn orb", RenderContext.CurrentTheme);
            */

            //GUI.Toolbar(Vector2.zero, 1, [new GUIContent().])

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
                        GameObject plyr = ClientInstance.GetLocalPlayer();
                        if (plyr == null)
                        {
                            Console.WriteLine("Player not found");
                            return;
                        }
                        Vector3 targetPos = plyr.transform.position + plyr.transform.forward * 2f;
                        for (var i = 0; i < 0x1FF; i++)
                            ItemUtils.SpawnSurplus(targetPos, false);
                        Console.WriteLine($"crash attempted !!!");
                    });
                }]
            );
        }
    }
}
