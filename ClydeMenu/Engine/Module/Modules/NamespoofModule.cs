using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace ClydeMenu.Engine.Commands;

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
        "Ullman", "Vaughn", "Wagner", "Alvarez",
    ];

    public override void Initialize()
    {
        GameEvents.OnLobbyJoinStart += OnLobbyJoinStart;
        GameEvents.OnKicked += CrashServer;
    }

    private void CrashServer()
    {
        var options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(199, null, options, SendOptions.SendReliable);
    }

    private void OnLobbyJoinStart()
    {
        if (Storage.CHEAT_PLAYER_Namespoof)
        {
            RandomizeName();
        }
    }

    private void RandomizeName()
    {
        var name = names[rng.Next(names.Length)] + names[rng.Next(names.Length)];
        Console.WriteLine($"Name Spoofed to {name}");
        PhotonNetwork.NickName = name;
    }
}
