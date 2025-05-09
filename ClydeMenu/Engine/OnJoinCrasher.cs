using System;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using UnityEngine;

namespace ClydeMenu.Engine;

internal class OnJoinCrasher : MonoBehaviourPunCallbacks
{
    public void Start()
    {
        StartCoroutine(TrueStart());
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(JoinNewRandom());
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server");
        var success = PhotonNetwork.JoinRandomRoom();
        if (!success)
        {
            Debug.Log("Failed to join random room. waitinng for a bit then repeating");
            StartCoroutine(TrueStart());
        }
    }

    IEnumerator TrueStart()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.NetworkingClient.State != ClientState.Disconnected && PhotonNetwork.NetworkingClient.State != ClientState.PeerCreated)
            yield return new WaitForSeconds(0.1f);

        PhotonNetwork.NickName = "debug bot";
        PhotonNetwork.AutomaticallySyncScene = false;
        SteamManager.instance.SendSteamAuthTicket();
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = BuildManager.instance.version.title;
        PhotonNetwork.ConnectUsingSettings();
    }

    IEnumerator JoinNewRandom()
    {
        object[] eventContent = [];
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

        PhotonNetwork.RaiseEvent(199, eventContent, raiseEventOptions, SendOptions.SendReliable);

        // noww we disconnect and rejoin another one randomly :)
        Debug.Log("Joined room, disconnecting...");
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.NetworkingClient.State != ClientState.Disconnected && PhotonNetwork.NetworkingClient.State != ClientState.PeerCreated)
            yield return new WaitForSeconds(0.1f);

        // now we wait cuz timeout stuff
        yield return new WaitForSeconds(5f);

        SteamManager.instance.SendSteamAuthTicket();
        PhotonNetwork.ConnectUsingSettings();
    }
}