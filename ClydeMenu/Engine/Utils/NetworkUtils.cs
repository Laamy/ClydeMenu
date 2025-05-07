namespace ClydeMenu.Engine;

using System;
using Photon.Pun;
using System.Reflection;
using UnityEngine;

internal class NetworkUtils
{
    public static GameObject FindLocalPlayerInNetwork()
    {
        // Check player list for photonView.IsMine
        var players = SemiFunc.PlayerGetList();
        if (players != null)
        {
            foreach (var player in players)
            {
                var photonView = GetPhotonView(player);
                if (photonView != null && photonView.IsMine)
                    return GetPlayerGameObject(player) ?? photonView.gameObject;
            }
        }

        // Find photon views owned by local player
        if (PhotonNetwork.LocalPlayer != null)
        {
            foreach (var photonView in UnityEngine.Object.FindObjectsOfType<PhotonView>())
            {
                if (photonView.Owner == PhotonNetwork.LocalPlayer && photonView.IsMine)
                    return photonView.gameObject;
            }
        }

        return null;
    }

    public static GameObject FindLocalPlayerOffline()
    {
        // First player in list
        var players = SemiFunc.PlayerGetList();
        if (players != null && players.Count > 0)
        {
            var gameObject = GetPlayerGameObject(players[0]);
            if (gameObject != null)
                return gameObject;
        }

        // PlayerAvatar component
        var playerAvatarType = Type.GetType("PlayerAvatar, Assembly-CSharp");
        if (playerAvatarType != null)
        {
            var playerAvatar = UnityEngine.Object.FindObjectOfType(playerAvatarType) as MonoBehaviour;
            if (playerAvatar != null)
                return playerAvatar.gameObject;
        }

        // Find by tag
        var playerByTag = GameObject.FindWithTag("Player");
        if (playerByTag != null)
            return playerByTag;

        // name containing "Player"
        foreach (var obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            if (obj.name.Contains("Player") && obj.activeInHierarchy)
                return obj;
        }

        return null;
    }

    private static PhotonView GetPhotonView(object player)
    {
        var photonViewField = player.GetType().GetField("photonView",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return photonViewField?.GetValue(player) as PhotonView;
    }

    private static GameObject GetPlayerGameObject(object player)
    {
        var gameObjectProperty = player.GetType().GetProperty("gameObject",
            BindingFlags.Public | BindingFlags.Instance);
        return gameObjectProperty?.GetValue(player) as GameObject;
    }
}
