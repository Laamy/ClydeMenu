using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class NetworkManager : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x0600090F RID: 2319 RVA: 0x00057450 File Offset: 0x00055650
	private void Start()
	{
		NetworkManager.instance = this;
		if (PhotonNetwork.IsMasterClient)
		{
			this.lastSyncTime = 0f;
		}
		if (GameManager.instance.gameMode == 1)
		{
			PhotonNetwork.Instantiate(this.playerAvatarPrefab.name, Vector3.zero, Quaternion.identity, 0, null);
			PhotonNetwork.SerializationRate = 25;
			PhotonNetwork.SendRate = 25;
			bool flag = true;
			PhotonVoiceView[] array = Object.FindObjectsByType<PhotonVoiceView>(FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				PhotonNetwork.Instantiate("Voice", Vector3.zero, Quaternion.identity, 0, null);
			}
			base.photonView.RPC("PlayerSpawnedRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x0005750F File Offset: 0x0005570F
	public override void OnEnable()
	{
		base.OnEnable();
		PhotonNetwork.NetworkingClient.EventReceived += new Action<EventData>(this.OnEventReceivedCustom);
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x0005752D File Offset: 0x0005572D
	public override void OnDisable()
	{
		base.OnDisable();
		PhotonNetwork.NetworkingClient.EventReceived -= new Action<EventData>(this.OnEventReceivedCustom);
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x0005754B File Offset: 0x0005574B
	[PunRPC]
	public void PlayerSpawnedRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!this.instantiatedPlayerAvatarsList.Contains(_info.Sender))
		{
			this.instantiatedPlayerAvatarsList.Add(_info.Sender);
			this.instantiatedPlayerAvatars++;
		}
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x0005757F File Offset: 0x0005577F
	[PunRPC]
	public void AllPlayerSpawnedRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		LevelGenerator.Instance.AllPlayersReady = true;
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x00057598 File Offset: 0x00055798
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.lastSyncTime);
			stream.SendNext(this.instantiatedPlayerAvatars);
			return;
		}
		this.gameTime = (float)stream.ReceiveNext();
		this.instantiatedPlayerAvatars = (int)stream.ReceiveNext();
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x000575FC File Offset: 0x000557FC
	private void Update()
	{
		if (GameManager.instance.gameMode == 1)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				if (!this.LoadingDone && this.instantiatedPlayerAvatars == PhotonNetwork.CurrentRoom.PlayerCount)
				{
					base.photonView.RPC("AllPlayerSpawnedRPC", RpcTarget.AllBuffered, Array.Empty<object>());
					this.LoadingDone = true;
				}
				this.gameTime += Time.deltaTime;
				if (Time.time - this.lastSyncTime > this.syncInterval)
				{
					this.lastSyncTime = this.gameTime;
				}
			}
			else
			{
				this.gameTime += Time.deltaTime;
			}
			if (PhotonNetwork.CurrentRoom == null || (GameDirector.instance.currentState != GameDirector.gameState.Load && GameDirector.instance.currentState != GameDirector.gameState.EndWait))
			{
				this.loadingScreenTimer = 0f;
				return;
			}
			this.loadingScreenTimer += Time.deltaTime;
			if (this.loadingScreenTimer >= 25f)
			{
				this.TriggerLeavePhotonRoomForced();
				MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "Cause: Stuck in loading", "Ok Dang", true);
				return;
			}
		}
		else
		{
			this.loadingScreenTimer = 0f;
		}
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x0005771C File Offset: 0x0005591C
	public void LeavePhotonRoom()
	{
		Debug.Log("Leave Photon");
		PhotonNetwork.Disconnect();
		SteamManager.instance.LeaveLobby();
		GameManager.instance.SetGameMode(0);
		this.leavePhotonRoom = false;
		if (RunManager.instance.levelCurrent == RunManager.instance.levelTutorial)
		{
			TutorialDirector.instance.EndTutorial();
		}
		base.StartCoroutine(RunManager.instance.LeaveToMainMenu());
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x0005778A File Offset: 0x0005598A
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Debug.Log("Player entered room: " + newPlayer.NickName);
		if (MenuPageLobby.instance)
		{
			MenuPageLobby.instance.JoiningPlayer(newPlayer.NickName);
		}
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x000577BD File Offset: 0x000559BD
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		Debug.Log("Player left room: " + otherPlayer.NickName);
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x000577D4 File Offset: 0x000559D4
	public override void OnMasterClientSwitched(Player _newMasterClient)
	{
		Debug.Log("Master client left...");
		MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "Cause: Host disconnected", "Ok Dang", true);
		this.TriggerLeavePhotonRoomForced();
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00057805 File Offset: 0x00055A05
	public void TriggerLeavePhotonRoomForced()
	{
		GameDirector.instance.currentState = GameDirector.gameState.Main;
		GameDirector.instance.OutroStart();
		this.leavePhotonRoom = true;
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00057824 File Offset: 0x00055A24
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log(string.Format("Disconnected from server for reason {0}", cause));
		if (cause != DisconnectCause.DisconnectByClientLogic && cause != DisconnectCause.DisconnectByServerLogic)
		{
			if (cause != DisconnectCause.DisconnectByDisconnectMessage)
			{
				MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "<b>Cause:\n</b>" + cause.ToString(), "Ok Dang", true);
			}
			GameDirector.instance.OutroStart();
			this.leavePhotonRoom = true;
		}
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x00057898 File Offset: 0x00055A98
	private void OnEventReceivedCustom(EventData photonEvent)
	{
		if (photonEvent.Code == 199)
		{
			Debug.Log("You were kicked by the server.");
			MenuManager.instance.PagePopUpScheduled("Kicked", Color.red, "You were kicked by the host.", "Ok Dang", true);
			GameDirector.instance.OutroStart();
			this.leavePhotonRoom = true;
		}
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x000578EC File Offset: 0x00055AEC
	public void DestroyAll()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			return;
		}
		Debug.Log("Destroyed all network objects.");
		PhotonNetwork.DestroyAll();
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x00057908 File Offset: 0x00055B08
	public void KickPlayer(PlayerAvatar _playerAvatar)
	{
		if (_playerAvatar.photonView.OwnerActorNr == _playerAvatar.photonView.CreatorActorNr)
		{
			object[] eventContent = new object[]
			{
				_playerAvatar.photonView.OwnerActorNr
			};
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				Receivers = ReceiverGroup.All
			};
			PhotonNetwork.RaiseEvent(123, eventContent, raiseEventOptions, SendOptions.SendReliable);
		}
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x00057964 File Offset: 0x00055B64
	public void BanPlayer(PlayerAvatar _playerAvatar)
	{
		if (_playerAvatar.photonView.OwnerActorNr == _playerAvatar.photonView.CreatorActorNr)
		{
			object[] eventContent = new object[]
			{
				_playerAvatar.photonView.OwnerActorNr
			};
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				Receivers = ReceiverGroup.All
			};
			PhotonNetwork.RaiseEvent(124, eventContent, raiseEventOptions, SendOptions.SendReliable);
		}
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x000579BF File Offset: 0x00055BBF
	private void OnApplicationQuit()
	{
	}

	// Token: 0x04001096 RID: 4246
	public static NetworkManager instance;

	// Token: 0x04001097 RID: 4247
	public float gameTime;

	// Token: 0x04001098 RID: 4248
	private float syncInterval = 0.5f;

	// Token: 0x04001099 RID: 4249
	private float lastSyncTime;

	// Token: 0x0400109A RID: 4250
	public GameObject playerAvatarPrefab;

	// Token: 0x0400109B RID: 4251
	private List<Player> instantiatedPlayerAvatarsList = new List<Player>();

	// Token: 0x0400109C RID: 4252
	private int instantiatedPlayerAvatars;

	// Token: 0x0400109D RID: 4253
	private bool LoadingDone;

	// Token: 0x0400109E RID: 4254
	internal bool leavePhotonRoom;

	// Token: 0x0400109F RID: 4255
	private float loadingScreenTimer;
}
