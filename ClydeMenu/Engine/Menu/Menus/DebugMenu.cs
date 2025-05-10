namespace ClydeMenu.Engine.Menu;

using System.Reflection;
using System;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using ExitGames.Client.Photon;

class DebugStorage
{
    internal static RenderWindow Main;
}

public class DebugMenu : BaseMenu
{
    public override void OnPop() {}
    public override void OnPush() {}

    public override void OnUpdate() {}

    public override void Render()
    {
        if (DebugStorage.Main == null)
        {
            DebugStorage.Main = RenderUtils.Window("ClydeMenu", new Rect(10, 10, 600, 600));
        }

        DebugStorage.Main.HandleBorder();
        DebugStorage.Main.Tabs(["Player", "Spawn", "Debug"], [
            () => {
                //godmode
                DebugStorage.Main.Toggle(new Vector2(10, 10), "Godmode", Storage.MOD_Godmode, (bool enabled) => {
                    Storage.MOD_Godmode = enabled;

                    if (enabled)
                        ClientInstance.SetHealth(99999, 99999);
                    else ClientInstance.SetHealth(100, 100);//TODO: make this actually be maxhealth and not a fake
                });

                //antikick
                DebugStorage.Main.Button(new Vector2(10, 50), "AntiKick", () => {
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
            },
            () => {
                DebugStorage.Main.Button(new Vector2(200, 10), "Crash", () => {
                    for (var i = 0; i < 0xFFF; i++)
                        ItemUtils.SpawnSurplus(Vector2.zero, false);
                    Console.WriteLine($"crash attempted !!!");
                });

                // spawn in small enemy orb
                DebugStorage.Main.Button(new Vector2(10, 50), "Spawn Enemy Orb", () => {
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
                DebugStorage.Main.Button(new Vector2(10, 10), "my experimental config", () => {
                    var view = ClientInstance.GetPhotonView(PunManager.instance);

                    view.RPC("UpgradePlayerGrabStrengthRPC", RpcTarget.AllBuffered, new object[] { "76561198930262816", 14 });
                    view.RPC("UpgradePlayerGrabRangeRPC", RpcTarget.AllBuffered, new object[] { "76561198930262816", 2 });
                    view.RPC("UpgradeMapPlayerCountRPC", RpcTarget.AllBuffered, new object[] { "76561198930262816", 1 });
                    view.RPC("UpgradePlayerEnergyRPC", RpcTarget.AllBuffered, new object[] { "76561198930262816", 500/10 });//500 stamina extra
                    view.RPC("UpgradePlayerHealthRPC", RpcTarget.AllBuffered, new object[] { "76561198930262816", 500/20 });//500 health extra
                    // i think this tricks every client into triggering UpgradePlayerGrabStrengthRPC with those args to an extent?
                });

                DebugStorage.Main.Button(new Vector2(10, 50), "crown hack", () => {
                    var view = ClientInstance.GetPhotonView(PunManager.instance);

                    view.RPC("CrownPlayerRPC", RpcTarget.AllBuffered, ["76561198930262816"]);
                });

                DebugStorage.Main.Button(new Vector2(10, 150), "make everyone a cheater", () => {
                    var view = ClientInstance.GetPhotonView(PunManager.instance);

                    foreach (var plyr in SemiFunc.PlayerGetList())
                    {
                        var steamId = SemiFunc.PlayerGetSteamID(plyr);
                        view.RPC("UpgradeItemBatteryRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerExtraJumpRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerTumbleLaunchRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerSprintSpeedRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerGrabStrengthRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerGrabRangeRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradeMapPlayerCountRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerThrowStrengthRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerEnergyRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerHealthRPC", RpcTarget.AllBuffered, [steamId, 255]);
                    }
                });

                DebugStorage.Main.Button(new Vector2(10, 200), "Kick MasterClient", () => {
                    object[] eventContent = [];
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

                    PhotonNetwork.RaiseEvent(199, eventContent, raiseEventOptions, SendOptions.SendReliable);
                });
            }]
        );
    }
}
