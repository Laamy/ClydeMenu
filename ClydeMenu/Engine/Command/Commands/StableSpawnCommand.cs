namespace ClydeMenu.Engine.Commands;

using System;

using UnityEngine;

//public class StableSpawnCommand : BaseCommand
//{
//    public StableSpawnCommand() : base("sspawn", "Spawn in an item (Stable but limited)", "<(orb,bag,misc), ((all: small,medium,large), (misc: debug,white))>") { }
//
//    public override void Execute(string[] args)
//    {
//        var localPlyr = ClientInstance.GetLocalPlayer();
//        if (localPlyr == null)
//        {
//            Console.WriteLine("Local player not found.");
//            return;
//        }
//
//        // NOTE: i might start spawning infront of the camera and not localplayer..
//        Vector3 infront = localPlyr.transform.position + localPlyr.transform.forward * 2f;
//
//        if (args.Length < 2)
//        {
//            Console.WriteLine("Invalid arguments. Usage: sspawn <(orb,bag), (small,medium,large)>");
//            return;
//        }
//        string itemType = args[0].ToLower();
//        string itemSize = args[1].ToLower();
//
//        GameObject itemPrefab = null;
//        switch (itemType)
//        {
//            case "orb":
//                itemPrefab = itemSize switch
//                {
//                    "small" => AssetManager.instance.enemyValuableSmall,
//                    "medium" => AssetManager.instance.enemyValuableMedium,
//                    "large" => AssetManager.instance.enemyValuableBig,
//                    _ => null
//                };
//                break;
//            case "bag":
//                itemPrefab = itemSize switch
//                {
//                    "small" => AssetManager.instance.surplusValuableSmall,
//                    "medium" => AssetManager.instance.surplusValuableMedium,
//                    "large" => AssetManager.instance.surplusValuableBig,
//                    _ => null
//                };
//                break;
//            case "misc":
//                itemPrefab = itemSize switch
//                {
//                    "debug" => AssetManager.instance.debugEnemyInvestigate,
//                    "white" => new GameObject("LargeBox"),
//                    _ => null
//                };
//                break;
//            default:
//                Console.WriteLine($"Item type '{itemType}' not recognized.");
//                return;
//        }
//
//        if (itemPrefab == null)
//        {
//            Console.WriteLine($"Item '{itemType}' with size '{itemSize}' not found.");
//            return;
//        }
//
//        // Spawn the item
//        ItemUtils.SpawnItem(infront, itemPrefab);
//    }
//}

/*
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
*/
