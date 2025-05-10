namespace ClydeMenu.Engine.Commands;

using System;
using System.Drawing.Imaging;
using System.Reflection;
using ExitGames.Client.Photon;

using Photon.Pun;
using Photon.Realtime;
using static UnityEngine.InputSystem.InputRemoting;

public class MessageSpoofCommand : BaseCommand
{
    public MessageSpoofCommand() : base("spoof", "Spoof a message as someone else", "<player, \"message\">") { }

    public override void Execute(string[] args)
    {
        foreach (var plyr in SemiFunc.PlayerGetList())
        {
            var plyrName = SemiFunc.PlayerGetName(plyr);
            if (!plyrName.ToLower().Contains(args[0].ToLower()))
                continue;

            var view = ClientInstance.GetPhotonView(plyr);

            view.RPC("ChatMessageSendRPC", RpcTarget.All,
            [
                SemiFunc.PlayerGetSteamID(plyr),
                args[1]//lol
            ]);
            return;
        }

        Console.WriteLine($"Player '{args[0]}' not found.");
    }
}
