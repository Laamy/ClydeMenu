namespace ClydeMenu.Engine;

using System;

using UnityEngine;

using Photon.Pun;

using System.Collections.Generic;
using System.Linq;

internal class ItemUtils : MonoBehaviourPunCallbacks
{
    public static List<Item> getItemsContains(string contains)
    {
        List<Item> items = new List<Item>();
        foreach (var item in StatsManager.instance.itemDictionary.Values)
        {
            if (item.prefab.name.Contains(contains))
                items.Add(item);
        }
        return items;
    }

    // bugged so this is local only
    public static void SpawnPrefab(Vector3 targetPos, string prefab)
    {
        if (!PhotonNetwork.IsConnected)
        {
            Console.WriteLine("Not connected to Photon, offlinemode");
            var localItem = Instantiate(getItemsContains(prefab).FirstOrDefault().prefab, targetPos, Quaternion.identity);
            if (!localItem)
            {
                Console.WriteLine("Failed to spawn enemy orb (offlinemode)");
                return;
            }

            SetActive(localItem);
            return;
        }
    }

    public static void SpawnItem(Vector3 targetPos, GameObject prefab, bool active = true)
    {
        if (!PhotonNetwork.IsConnected)
        {
            Console.WriteLine("Not connected to Photon, offlinemode");
            var localItem = Instantiate(prefab, targetPos, Quaternion.identity);
            if (!localItem)
            {
                Console.WriteLine("Failed to spawn enemy orb (offlinemode)");
                return;
            }

            SetActive(localItem, active);
            return;
        }

        var item = PhotonNetwork.Instantiate("Valuables/" + prefab.name, targetPos, Quaternion.identity, 0, null);

        if (!item)
        {
            Console.WriteLine("Failed to spawn enemy orb (PhotonNetwork)");
            return;
        }

        SyncComps(item, active);
    }

    private static PhotonView GetView(GameObject obj)
    {
        PhotonView pv = obj.GetComponent<PhotonView>();
        if (pv == null)
        {
            var view = obj.AddComponent<PhotonView>();
            view.ViewID = PhotonNetwork.AllocateViewID(0);
            return view;
        }
        return pv;
    }

    private static PhotonTransformView GetTransform(GameObject obj) =>
        obj.GetComponent<PhotonTransformView>() ?? obj.AddComponent<PhotonTransformView>();

    private static PhotonRigidbodyView GetBodyView(GameObject obj)
    {
        if (!obj.GetComponent<Rigidbody>())
            return null;

        PhotonRigidbodyView body = obj.GetComponent<PhotonRigidbodyView>();
        if (body == null)
        {
            body = obj.AddComponent<PhotonRigidbodyView>();
            body.m_SynchronizeVelocity = true;
            body.m_SynchronizeAngularVelocity = true;
        }
        return body;
    }

    // TODO: figure out a way to sync it when ur not the host
    private static void SyncComps(GameObject item, bool active = true)
    {
        var view = GetView(item);
        var transform = GetTransform(item);
        var body = GetBodyView(item);

        view.ObservedComponents = [transform, body];
        view.Synchronization = ViewSynchronization.ReliableDeltaCompressed;

        if (active) SetActive(item);
        else DestroyImmediate(item);
    }

    private static void SetActive(GameObject item, bool active = true)
    {
        item.SetActive(active);
        foreach (var renderer in item.GetComponentsInChildren<Renderer>(true))
            renderer.enabled = active;
        if (active)
            item.layer = LayerMask.NameToLayer("Default");
    }

    // debug utils
    public static void SpawnEnemyOrb(Vector3 targetPos)
        => SpawnItem(targetPos, AssetManager.instance.enemyValuableBig);

    internal static void SpawnSurplus(Vector3 targetPos, bool active = true)
        => SpawnItem(targetPos, AssetManager.instance.surplusValuableSmall, active);
}