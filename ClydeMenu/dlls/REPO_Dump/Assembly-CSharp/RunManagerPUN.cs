using System;
using System.Collections;
using Photon.Pun;
using Steamworks;
using UnityEngine;

// Token: 0x0200012D RID: 301
public class RunManagerPUN : MonoBehaviour
{
	// Token: 0x060009A8 RID: 2472 RVA: 0x00059CD0 File Offset: 0x00057ED0
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.runManager = RunManager.instance;
		this.runManager.runManagerPUN = this;
		if (this.runManager.levelCurrent == this.runManager.levelShop)
		{
			this.runManager.levelIsShop = true;
		}
		else
		{
			this.runManager.levelIsShop = false;
		}
		this.runManager.restarting = false;
		this.runManager.restartingDone = false;
		if (PhotonNetwork.IsMasterClient && GameManager.instance.connectRandom)
		{
			if (!SteamManager.instance.currentLobby.Id.IsValid)
			{
				base.StartCoroutine(this.HostSteamLobby());
				return;
			}
			if (SemiFunc.RunIsLobbyMenu())
			{
				this.SendJoinSteamLobby();
			}
		}
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x00059D98 File Offset: 0x00057F98
	private void SendJoinSteamLobby()
	{
		SteamManager.instance.UnlockLobby(true);
		this.photonView.RPC("JoinSteamLobbyRPC", RpcTarget.OthersBuffered, new object[]
		{
			SteamManager.instance.currentLobby.Id.ToString()
		});
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x00059DE7 File Offset: 0x00057FE7
	private IEnumerator HostSteamLobby()
	{
		SteamManager.instance.HostLobby(true);
		while (!SteamManager.instance.currentLobby.Id.IsValid)
		{
			yield return null;
		}
		Debug.Log("Created open lobby with ID: " + SteamManager.instance.currentLobby.Id.ToString());
		SteamManager.instance.SetLobbyData(PhotonNetwork.CurrentRoom.Name);
		this.SendJoinSteamLobby();
		yield break;
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x00059DF8 File Offset: 0x00057FF8
	[PunRPC]
	private void JoinSteamLobbyRPC(string _steamID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		Debug.Log("I got the lobby id: " + _steamID);
		SteamId lobbyID = default(SteamId);
		lobbyID.Value = ulong.Parse(_steamID);
		SteamManager.instance.JoinLobby(lobbyID);
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00059E3E File Offset: 0x0005803E
	[PunRPC]
	private void UpdateLevelRPC(string _levelName, int _levelsCompleted, bool _gameOver, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.runManager.UpdateLevel(_levelName, _levelsCompleted, _gameOver);
	}

	// Token: 0x04001137 RID: 4407
	internal PhotonView photonView;

	// Token: 0x04001138 RID: 4408
	private RunManager runManager;
}
