namespace ClydeMenu.Engine.Commands;

using System;
using System.Linq;

using UnityEngine;

//public class SpawnCommand : BaseCommand
//{
//    public SpawnCommand() : base("spawn", "Spawn in an item", "<\"name\">") { }
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
//        Vector3 infront = localPlyr.transform.position + localPlyr.transform.forward * 2f;
//
//        var item = ItemUtils.getItemsContains(args[0]);
//        if (item.Count == 0)
//        {
//            Console.WriteLine($"Item '{args[0]}' not found.");
//            return;
//        }
//
//        ItemUtils.SpawnItem(infront, item.FirstOrDefault().prefab);
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
