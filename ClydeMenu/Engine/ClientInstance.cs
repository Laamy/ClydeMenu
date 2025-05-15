    namespace ClydeMenu.Engine;

using System;
using System.Reflection;

using UnityEngine;
using Object = UnityEngine.Object;

using Photon.Pun;
using System.Media;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using static PlayerHealth;
using Unity.VisualScripting;

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

    // ???????? why is this so fucking buggy bruh
    public static List<EnemyParent> GetEnemyList()
    {
        if (EnemyDirector.instance == null)
            return [];

        var enemies = EnemyDirector.instance.enemiesSpawned;
        if (enemies == null || enemies.Count == 0)
            return [];

        return enemies;
    }

    public static List<GameObject> GetExtractionPoints()
    {
        List<GameObject> ePoints = [];
        try
        {
            if (RoundDirector.instance == null)
                return [];

            ePoints = (List<GameObject>)FetchField<List<GameObject>>("extractionPointList").GetValue(RoundDirector.instance);
            if (ePoints == null || ePoints.Count == 0)
                return [];
        }
        catch (Exception)
        {
            //Console.WriteLine($"Error fetching extraction points: {ex}");
            return [];
        }

        return ePoints;
    }

    private static List<ValuableObject> valuableObjects = [];
    private static Stopwatch timer;
    public static List<ValuableObject> GetValuableList() // this is gonna piss me off lowkey..
    {
        if (timer == null)
            timer = Stopwatch.StartNew();
        
        if (timer.ElapsedMilliseconds >= 1000)
        {
            valuableObjects = Object.FindObjectsOfType<ValuableObject>(true).ToList();
            timer = Stopwatch.StartNew();
        }
        
        return valuableObjects;
    }

    public static GameObject GetLocalPlayer()
    {
        var players = SemiFunc.PlayerGetList();
        if (players == null || players.Count == 0)
            return null;

        return players[0].gameObject;
    }

    public static PlayerAvatar GetLocalAvatar()
    {
        var players = SemiFunc.PlayerGetList();
        if (players == null || players.Count == 0)
            return null;

        return players[0];
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

    internal static T FetchFieldValue<T, B>(string v, B cls)
    {
        var field = FetchField<T>(v);
        return (T)field.GetValue(cls);
    }

    internal static void SetFieldValue<T, B>(string v, B cls, T newValue)
    {
        var field = FetchField<T>(v);
        field.SetValue(cls, newValue);
    }

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

    internal static void SpoofMsg(PlayerAvatar avatar, string msg) => avatar.ChatMessageSend(msg, false);
    internal static void RevivePlayer(PlayerAvatar avatar)
    {
        if (FetchFieldValue<int, PlayerHealth>("health", avatar.playerHealth) > 0)
            avatar.Revive(true);
    }
    internal static void HealPlayer(PlayerAvatar avatar, int amount)
    {
        var _this = avatar.playerHealth;

        var health = FetchFieldValue<int, PlayerHealth>("health", _this);
        var maxHealth = FetchFieldValue<int, PlayerHealth>("maxHealth", _this);

        SetFieldValue("health", _this, Mathf.Clamp(health + amount, 0, maxHealth));
        health = FetchFieldValue<int, PlayerHealth>("health", _this);

        StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(avatar), health, false);
        if (GameManager.Multiplayer())
        {
            GetPhotonView(_this).RPC("UpdateHealthRPC", RpcTarget.Others, new object[]
            {
                health,
                maxHealth,
                false
            });
        }
    }
    internal static void KillPlayer(PlayerAvatar avatar)
    {
        avatar.playerHealth.Death();
        avatar.PlayerDeath(-1);
    }
}
