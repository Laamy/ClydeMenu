namespace ClydeMenu.Engine.Commands;

using System;

using ExitGames.Client.Photon;

using Photon.Pun;
using Photon.Realtime;

// cant figure this out yet
//public class KickCommand : BaseCommand
//{
//    public KickCommand() : base("kick", "Kick a player (not ban) from the current server without host perms", "<player>") { }
//
//    public override void Execute(string[] args)
//    {
//        if (args[0] == "all")
//        {
//            var options = new RaiseEventOptions();
//            options.Receivers = ReceiverGroup.All;
//            PhotonNetwork.RaiseEvent(199, null, options, SendOptions.SendReliable);
//            return;
//        }
//
//        foreach (var plyr in SemiFunc.PlayerGetList())
//        {
//            var plyrName = SemiFunc.PlayerGetName(plyr);
//            if (!plyrName.ToLower().Contains(args[0].ToLower()))
//                continue;
//
//            var plyrActorId = plyr.photonView.OwnerActorNr;
//
//            var options = new RaiseEventOptions();
//            options.TargetActors = new[] { plyrActorId };
//            PhotonNetwork.RaiseEvent(199, null, options, SendOptions.SendReliable);
//            Entry.Log($"Kicked player {plyr.name} from the server.");
//            return;
//        }
//
//        Entry.Log($"Player '{args[0]}' not found.");
//    }
//}
