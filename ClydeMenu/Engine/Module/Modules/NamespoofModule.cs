namespace ClydeMenu.Engine.Commands;

using System;
using ClydeMenu.Engine.Settings;
using ExitGames.Client.Photon;


using Photon.Pun;
using Photon.Realtime;

using UnityEngine;
using Random = System.Random;

public class NamespoofModule : BaseModule
{
    public NamespoofModule() : base("Namespoof", "Spoof your username to random crap", "Misc") { }

    private readonly Random rng = new();

    string[] names = [
        "Aiden", "Brielle", "Caleb", "Delilah", "Ethan", "Fiona", "Gavin", "Hazel",
        "Isaac", "Jasmine", "Kai", "Luna", "Miles", "Nora", "Owen", "Penelope",
        "Quentin", "Ruby", "Silas", "Tessa", "Uriel", "Violet", "Wyatt", "Ximena",
        "Yusuf", "Zara", "Alina", "Brayden", "Cora", "Dante", "Elise", "Felix",
        "Gianna", "Holden", "Isla", "Jaxon", "Keira", "Leo", "Mila", "Nolan",
        "Olive", "Parker", "Quinn", "Riley", "Sage", "Theo", "Una", "Vivian",
        "Wesley", "Xander", "Yara", "Zion", "Adrian", "Brooke", "Carter", "Dylan",
        "Ella", "Finn", "Grace", "Hunter", "Ivy", "Jack", "Kylie", "Liam",
        "Maya", "Noah", "Olivia", "Peyton", "Quincy", "Ryder", "Sofia", "Troy",
        "Uriah", "Vera", "Willa", "Xena", "Yasmine", "Zane", "Aaliyah", "Beckett",
        "Anderson", "Bennett", "Dawson", "Ellis", "Fisher", "Garcia", "Hughes",
        "Iverson", "Jacobs", "Keller", "Lawson", "Martinez", "Nelson", "Owens", "Patel",
        "Quincy", "Reed", "Santos", "Turner", "Upton", "Vargas", "Walker", "Xu",
        "Young", "Zimmerman", "Archer", "Baker", "Chavez", "Diaz", "Evans", "Ford",
        "Gibson", "Hall", "Ingram", "Jennings", "Knight", "Lee", "Morris", "Nguyen",
        "Ortiz", "Price", "Quintero", "Robinson", "Smith", "Taylor", "Underwood", "Valdez",
        "White", "Xiong", "Yates", "Zuniga", "Adams", "Bishop", "Caldwell", "Dixon",
        "Edwards", "Floyd", "Gordon", "Harrison", "Ibarra", "Jordan", "Klein", "Lamb",
        "Mason", "Nash", "O'Connor", "Peters", "Quinlan", "Ramos", "Sullivan", "Tate",
        "Ullman", "Vaughn", "Wagner", "Alvarez", "Adolf", "Hitler", "Shock", "Lilly", "Alex",
        "Confusion", "Seth", "Rambo", "Random", "Olvera"
    ];

    private void OnLobbyJoinStart()
    {
        if (MenuSettings.AccountSpoofer.Value)
        {
            // randomize name from a small array of names n last names
            var name = names[rng.Next(names.Length)] + names[rng.Next(names.Length)];
            Console.WriteLine($"Name Spoofed to {name}");
            PhotonNetwork.NickName = name;

            if (new Random().Next(2) == 0)
                PhotonNetwork.NickName += new Random().Next(11, 70); // cuz y not
        }
    }

    public override void Initialize()
    {
        GameEvents.OnLobbyJoinStart += OnLobbyJoinStart;
        GameEvents.OnLobbyJoin += OnLobbyJoin;
        GameEvents.OnKicked += CrashServer;
    }

    private void CrashServer()
    {
        var options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(199, null, options, SendOptions.SendReliable);
    }

    //[HarmonyPatch(typeof(PhotonNetwork), nameof(PhotonNetwork.GetPing))]
    //public static class Patch_GetPing
    //{
    //    static bool Prefix(ref int __result)
    //    {
    //        __result = 10;
    //        return false;
    //    }
    //}
    //
    //[HarmonyPatch(typeof(DataDirector), nameof(DataDirector.PhotonSetRegion))]
    //public static class Patch_PhotonSetRegion
    //{
    //    static bool Prefix()
    //    {
    //        Debug.Log("Photon - Set Region (SPOOF)");
    //        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us";
    //        return false;
    //    }
    //}

    // NOTE: I have no way to set volume back to 100% for all clients yet
    // so if you set the volume of a account spoofer you can still identify them
    private void OnLobbyJoin()
    {
        if (MenuSettings.AccountSpoofer.Value)
        {
            // randomize colour 2!!
            ClientInstance.GetLocalAvatar().PlayerAvatarSetColor(new Random().Next(0, 35));
        }
    }
}
