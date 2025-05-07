namespace ClydeMenu.Engine;

using System;
using System.Reflection;

using UnityEngine;
using Object = UnityEngine.Object;

using Photon.Pun;

internal static class AssemblyReader
{
    public static object Fetch(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null)
            return Object.FindObjectOfType(type);
        Console.WriteLine($"{typeName} instance not found");
        return null;
    }
}

internal class ClientInstance
{
    //public static object PlayerHealthInstance { get; private set; }
    //public static object PlayerMaxHealthInstance { get; private set; }

    public static void Init(ClientComponent mod)
    {
        //PlayerHealthInstance = AssemblyReader.Fetch("PlayerHealth, Assembly-CSharp");
        //PlayerMaxHealthInstance = AssemblyReader.Fetch("ItemUpgradePlayerHealth, Assembly-CSharp");
    }

    public static object PlayerController
    {
        get
        {
            var instance = AssemblyReader.Fetch("PlayerController, Assembly-CSharp");
            if (instance == null)
                Console.WriteLine("Unable to find PlayerController instance.");

            return instance;
        }
    }

    public static object PlayerAvatarScript
    {
        get
        {
            var instance = PlayerController;
            if (instance == null)
                return null;

            var avatarScript = instance.GetType().GetField("playerAvatarScript", BindingFlags.Public | BindingFlags.Instance);
            if (avatarScript == null)
            {
                Console.WriteLine("Unable to find playerAvatarScript field in PlayerController.");
                return null;
            }

            return avatarScript.GetValue(instance);
        }
    }

    public static object PlayerHealthInstance
    {
        get
        {
            var instance = PlayerAvatarScript;
            if (instance == null)
                return null;

            var healthField = instance.GetType().GetField("playerHealth", BindingFlags.Public | BindingFlags.Instance);
            if (healthField == null)
            {
                Console.WriteLine("Unable to find playerHealth field in PlayerAvatarScript.");
                return null;
            }

            return healthField.GetValue(instance);
        }
    }

    public static void SetHealth(int health, int maxHealth)
    {
        var healthInstance = PlayerHealthInstance;
        var healthRPC = healthInstance.GetType().GetMethod("UpdateHealthRPC");
        if (healthRPC == null)
        {
            Console.WriteLine("Unable to find UpdateHealthRPC method in PlayerHealth.");
            return;
        }

        healthRPC.Invoke(healthInstance, [health, maxHealth, true]);
    }

    public static GameObject GetLocalPlayer()
        => PhotonNetwork.IsConnected ? NetworkUtils.FindLocalPlayerInNetwork() : NetworkUtils.FindLocalPlayerOffline();
}
