using System;

using Photon.Pun;

using UnityEngine;

namespace ClydeMenu.Engine;

internal class NameSpoofer : MonoBehaviourPunCallbacks
{
    public static void RandomizeName()
    {
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

        var rng = new System.Random();
        string randomName = firstNames[rng.Next(firstNames.Length)] + lastNames[rng.Next(lastNames.Length)];

        Console.WriteLine($"Name Spoofed to {randomName}");
        PhotonNetwork.NickName = randomName;
    }
}