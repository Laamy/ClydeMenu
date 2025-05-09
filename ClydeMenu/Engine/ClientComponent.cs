namespace ClydeMenu.Engine;

using System;
using System.Reflection;

using ExitGames.Client.Photon;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine;

public class ClientComponent : MonoBehaviour
{
    public bool isShown = false;

    public void Start()
    {
        NameSpoofer.InitSpoofer();

        Render.Init();
        Render.SetCursorState(isShown);

        ClientInstance.Init(this);
        Console.WriteLine("ClydeMenu initialized");
    }

    public void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            isShown = !isShown;
            Render.SetCursorState(isShown);
            Console.WriteLine($"Menu is now {(isShown ? "shown" : "hidden")}");
        }

        // im gonna get so mad if i press this 1 more fucking time
        //if (Input.GetKeyDown(KeyCode.Delete))
        //    Process.GetCurrentProcess().Kill();

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
            Main.Tabs(["Player", "Spawn", "Debug"], [
                () => {
                    //godmode
                    Main.Toggle(new Vector2(10, 10), "Godmode", Storage.Godmode, (bool enabled) => {
                        Storage.Godmode = enabled;

                        if (enabled)
                            ClientInstance.SetHealth(99999, 99999);
                        else ClientInstance.SetHealth(100, 100);//TODO: make this actually be maxhealth and not a fake
                    });

                    //antikick
                    Main.Button(new Vector2(10, 50), "AntiKick", () => {
                        var client = PhotonNetwork.NetworkingClient;
                        var target = NetworkManager.instance;
                        var method = target.GetType().GetMethod("OnEventReceivedCustom", BindingFlags.Instance | BindingFlags.NonPublic);

                        if (method != null)
                        {
                            var handler = (Action<EventData>)Delegate.CreateDelegate(typeof(Action<EventData>), target, method);
                            client.EventReceived -= handler;
                            Console.WriteLine("AntiKick disabled (restart to clear)");
                        }
                        else {
                            Console.WriteLine("Failed to disable AntiKick (not in a game or already countered?)");
                        }
                    });
                    string[] firstNames = new string[] {
                        "Aiden", "Brielle", "Caleb", "Delilah", "Ethan", "Fiona", "Gavin", "Hazel",
                        "Isaac", "Jasmine", "Kai", "Luna", "Miles", "Nora", "Owen", "Penelope",
                        "Quentin", "Ruby", "Silas", "Tessa", "Uriel", "Violet", "Wyatt", "Ximena",
                        "Yusuf", "Zara", "Alina", "Brayden", "Cora", "Dante", "Elise", "Felix",
                        "Gianna", "Holden", "Isla", "Jaxon", "Keira", "Leo", "Mila", "Nolan",
                        "Olive", "Parker", "Quinn", "Riley", "Sage", "Theo", "Una", "Vivian",
                        "Wesley", "Xander", "Yara", "Zion"
                    };

                    string[] lastNames = new string[] {
                        "Anderson", "Bennett", "Carter", "Dawson", "Ellis", "Fisher", "Garcia", "Hughes",
                        "Iverson", "Jacobs", "Keller", "Lawson", "Martinez", "Nelson", "Owens", "Patel",
                        "Quincy", "Reed", "Santos", "Turner", "Upton", "Vargas", "Walker", "Xu",
                        "Young", "Zimmerman", "Archer", "Baker", "Chavez", "Diaz", "Evans", "Ford",
                        "Gibson", "Hall", "Ingram", "Jennings", "Knight", "Lee", "Morris", "Nguyen",
                        "Ortiz", "Price", "Quintero", "Robinson", "Smith", "Taylor", "Underwood", "Valdez",
                        "White", "Xiong", "Yates", "Zuniga"
                    };

                    //name spoof (random numbers)
                    Main.Button(new Vector2(10, 100), "Name Spoof", () => {
                        var rng = new System.Random();
                        string randomName = firstNames[rng.Next(firstNames.Length)] + lastNames[rng.Next(lastNames.Length)];

                        Console.WriteLine($"Name Spoofed to {randomName}");
                        PhotonNetwork.NickName = randomName;
                    });
                },
                () => {
                    Main.Button(new Vector2(200, 10), "Crash", () => {
                        for (var i = 0; i < 0xFFF; i++)
                            ItemUtils.SpawnSurplus(Vector2.zero, false);
                        Console.WriteLine($"crash attempted !!!");
                    });

                    // spawn in small enemy orb
                    Main.Button(new Vector2(10, 50), "Spawn Enemy Orb", () => {
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
                },
                () => {
                    Main.Button(new Vector2(10, 10), "my experimental config", () => {
                        var type = typeof(PunManager);
                        var instance = PunManager.instance;
                        var field = type.GetField("photonView", BindingFlags.NonPublic | BindingFlags.Instance);
                        var view = (PhotonView)field.GetValue(instance);

                        view.RPC("UpgradePlayerGrabStrengthRPC", RpcTarget.AllBuffered, new object[] { "76561198930262816", 14 });
                        view.RPC("UpgradePlayerGrabRangeRPC", RpcTarget.AllBuffered, new object[] { "76561198930262816", 2 });
                        view.RPC("UpgradeMapPlayerCountRPC", RpcTarget.AllBuffered, new object[] { "76561198930262816", 1 });
                        view.RPC("UpgradePlayerEnergyRPC", RpcTarget.AllBuffered, new object[] { "76561198930262816", 500/10 });//500 stamina extra
                        view.RPC("UpgradePlayerHealthRPC", RpcTarget.AllBuffered, new object[] { "76561198930262816", 500/20 });//500 health extra
                        // i think this tricks every client into triggering UpgradePlayerGrabStrengthRPC with those args to an extent?
                    });

                    Main.Button(new Vector2(10, 50), "crown hack", () => {
                        var type = typeof(PunManager);
                        var instance = PunManager.instance;
                        var field = type.GetField("photonView", BindingFlags.NonPublic | BindingFlags.Instance);
                        var view = (PhotonView)field.GetValue(instance);

                        view.RPC("CrownPlayerRPC", RpcTarget.AllBuffered, new object[] { "76561198930262816" });
                    });

                    Main.Button(new Vector2(10, 100), "experimennt 3!", () => {
                        var type = typeof(PunManager);
                        var instance = PunManager.instance;
                        var field = type.GetField("photonView", BindingFlags.NonPublic | BindingFlags.Instance);
                        var view = (PhotonView)field.GetValue(instance);

                        view.RPC("SetRunStatRPC", RpcTarget.Others, new object[] { "level", 69420-1 });
                        //[PunRPC]
                        //public void UpdateStatRPC(string dictionaryName, string key, int value)
                        //{
                        //    StatsManager.instance.DictionaryUpdateValue(dictionaryName, key, value);
                        //}
                    });

                    Main.Button(new Vector2(10, 150), "make everyone a cheater", () => {
                        var type = typeof(PunManager);
                        var instance = PunManager.instance;
                        var field = type.GetField("photonView", BindingFlags.NonPublic | BindingFlags.Instance);
                        var view = (PhotonView)field.GetValue(instance);

                        foreach (var plyr in SemiFunc.PlayerGetList())
                        {
                            var steamId = SemiFunc.PlayerGetSteamID(plyr);
                            view.RPC("UpgradeItemBatteryRPC", RpcTarget.AllBuffered, new object[] { steamId, 255 });
                            view.RPC("UpgradePlayerExtraJumpRPC", RpcTarget.AllBuffered, new object[] { steamId, 255 });
                            view.RPC("UpgradePlayerTumbleLaunchRPC", RpcTarget.AllBuffered, new object[] { steamId, 255 });
                            view.RPC("UpgradePlayerSprintSpeedRPC", RpcTarget.AllBuffered, new object[] { steamId, 255 });
                            view.RPC("UpgradePlayerGrabStrengthRPC", RpcTarget.AllBuffered, new object[] { steamId, 255 });
                            view.RPC("UpgradePlayerGrabRangeRPC", RpcTarget.AllBuffered, new object[] { steamId, 255 });
                            view.RPC("UpgradeMapPlayerCountRPC", RpcTarget.AllBuffered, new object[] { steamId, 255 });
                            view.RPC("UpgradePlayerThrowStrengthRPC", RpcTarget.AllBuffered, new object[] { steamId, 255 });
                            view.RPC("UpgradePlayerEnergyRPC", RpcTarget.AllBuffered, new object[] { steamId, 255 });
                            view.RPC("UpgradePlayerHealthRPC", RpcTarget.AllBuffered, new object[] { steamId, 255 });
                        }
                    });

                    Main.Button(new Vector2(10, 200), "Kick MasterClient", () => {

                        object[] eventContent = [];
                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

                        PhotonNetwork.RaiseEvent(199, eventContent, raiseEventOptions, SendOptions.SendReliable);

                        //foreach (var plyr in SemiFunc.PlayerGetList())
                        //{
                        //    if (plyr.photonView.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
                        //        continue;
                        //
                        //    object[] eventContent = [];
                        //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                        //
                        //    PhotonNetwork.RaiseEvent(199, eventContent, raiseEventOptions, SendOptions.SendReliable);
                        //}
                    });
                }]
            );
        }
    }
}
