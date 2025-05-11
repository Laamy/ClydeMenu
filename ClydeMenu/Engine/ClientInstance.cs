namespace ClydeMenu.Engine;

using System;
using System.Reflection;

using UnityEngine;
using Object = UnityEngine.Object;

using Photon.Pun;
using System.Media;
using System.Collections.Generic;

internal static class AssemblyReader
{
    public static object Fetch(string typeName)
    {
        var playerlist = SemiFunc.PlayerGetList();
        var steamid = SemiFunc.PlayerGetSteamID(playerlist[0]);

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

    private static GameObject _localPlayer;
    public static GameObject GetLocalPlayer()
    {
        //if (_localPlayer != null && _localPlayer.activeInHierarchy)
        //    return _localPlayer;

        var players = SemiFunc.PlayerGetList();
        if (players == null || players.Count == 0)
        {
            //Console.WriteLine("No players found");
            return null;
        }

        _localPlayer = players[0].gameObject;
        if (_localPlayer != null)
        {
            //Console.WriteLine($"Local player found: {_localPlayer.name}");
            return _localPlayer;
        }

        //Console.WriteLine("Local player not found");
        return null;
    }

    internal static PhotonView GetPhotonView<T>(T instance)
    {
        var type = typeof(T);
        var field = FetchField<T>("photonView");
        if (field == null)
            throw new MissingFieldException(type.FullName, "photonView");

        var view = (PhotonView)field.GetValue(instance);
        if (view == null)
            throw new NullReferenceException("photonView is null");

        return view;
    }

    internal static FieldInfo FetchField<T>(string v)
        => typeof(T).GetField(v, BindingFlags.NonPublic | BindingFlags.Instance);

    internal static MethodInfo FetchMethod<T>(string v)
        => typeof(T).GetMethod(v, BindingFlags.NonPublic | BindingFlags.Instance);

    internal static Bounds GetActiveColliderBounds(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>(true);
        List<Collider> activeColliders = new List<Collider>();

        foreach (Collider col in colliders)
        {
            if (col.enabled && col.gameObject.activeInHierarchy)
                activeColliders.Add(col);
        }

        if (activeColliders.Count == 0)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    if (renderers[i].enabled && renderers[i].gameObject.activeInHierarchy)
                        bounds.Encapsulate(renderers[i].bounds);
                }
                return bounds;
            }
            return new Bounds(obj.transform.position, Vector3.one * 0.5f);
        }

        Bounds resultBounds = activeColliders[0].bounds;
        for (int i = 1; i < activeColliders.Count; i++)
        {
            resultBounds.Encapsulate(activeColliders[i].bounds);
        }

        resultBounds.Expand(0.1f);
        return resultBounds;
    }
}
