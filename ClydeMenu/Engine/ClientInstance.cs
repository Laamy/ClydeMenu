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
    public static PlayerController PlayerController
    {
        get
        {
            var instance = AssemblyReader.Fetch("PlayerController, Assembly-CSharp");
            if (instance == null)
                Console.WriteLine("Unable to find PlayerController instance.");

            return (PlayerController)instance;
        }
    }

    public static void SetHealth(int health, int maxHealth)
        => PlayerController.playerAvatarScript.playerHealth.UpdateHealthRPC(health, maxHealth, true);

    public static GameObject GetLocalPlayer()
    {
        if (PhotonNetwork.LocalPlayer != null)
        {
            foreach (var photonView in Object.FindObjectsOfType<PhotonView>())
            {
                if (photonView.Owner == PhotonNetwork.LocalPlayer && photonView.IsMine)
                    return photonView.gameObject;
            }
        }

        var playerByTag = GameObject.FindWithTag("Player");
        if (playerByTag != null)
            return playerByTag;

        return null;
    }

    internal static PhotonView GetPhotonView<T>(T instance)
    {
        var type = typeof(T);
        var field = type.GetField("photonView", BindingFlags.NonPublic | BindingFlags.Instance);
        if (field == null)
            throw new MissingFieldException(type.FullName, "photonView");

        var view = (PhotonView)field.GetValue(instance);
        if (view == null)
            throw new NullReferenceException("photonView is null");

        return view;
    }
}
