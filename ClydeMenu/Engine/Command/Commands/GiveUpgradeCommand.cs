namespace ClydeMenu.Engine.Commands;

using System;
using System.Collections.Generic;
using System.Reflection;

using Photon.Pun;
using UnityEngine;
using WebSocketSharp;

public class GiveUpgradeCommand : BaseCommand
{
    public GiveUpgradeCommand() : base("give", "Spoof a message as someone else", "<player, upgrade, amount>") { }

    public List<string> upgrades = new List<string>
    {
        "UpgradeItemBatteryRPC",
        "UpgradePlayerExtraJumpRPC",
        "UpgradePlayerTumbleLaunchRPC",
        "UpgradePlayerSprintSpeedRPC",
        "UpgradePlayerGrabStrengthRPC",
        "UpgradePlayerGrabRangeRPC",
        "UpgradeMapPlayerCountRPC",
        "UpgradePlayerThrowStrengthRPC",
        "UpgradePlayerEnergyRPC",
        "UpgradePlayerHealthRPC"
    };

    public override void Execute(string[] args)
    {
        var view = ClientInstance.GetPhotonView(PunManager.instance);

        var upgrade = args[1].ToLower();
        var upgradeFound = upgrades.Find(x => x.ToLower().Contains(upgrade.ToLower()));

        if (upgradeFound.IsNullOrEmpty())
        {
            Console.WriteLine($"Upgrade '{args[1]}' not found.");
            return;
        }

        int amount = 0;
        if (int.TryParse(args[2], out amount))
        {
            amount = Mathf.Clamp(amount, 0, 255);
        }
        else
        {
            Console.WriteLine($"Invalid amount '{args[2]}'.");
            return;
        }

        foreach (var plyr in SemiFunc.PlayerGetList())
        {
            var steamId = SemiFunc.PlayerGetSteamID(plyr);
            var plyrName = SemiFunc.PlayerGetName(plyr);
            if (!plyrName.ToLower().Contains(args[0].ToLower()))
                continue;

            view.RPC(upgradeFound, RpcTarget.AllBuffered, [steamId, amount]);
            Console.WriteLine($"Gave {amount} {upgradeFound} to {plyrName}");
            return;
        }

        Console.WriteLine($"Player '{args[0]}' not found.");
    }
}
