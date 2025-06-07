using System;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000213 RID: 531
public class NetworkConnect : MonoBehaviourPunCallbacks
{
	// Token: 0x060011C9 RID: 4553 RVA: 0x000A21AC File Offset: 0x000A03AC
	private void Start()
	{
		PhotonNetwork.NickName = SteamClient.Name;
		PhotonNetwork.AutomaticallySyncScene = false;
		DataDirector.instance.PhotonSetRegion();
		DataDirector.instance.PhotonSetVersion();
		DataDirector.instance.PhotonSetAppId();
		Object.Instantiate<GameObject>(this.punVoiceClient, Vector3.zero, Quaternion.identity);
		PhotonNetwork.Disconnect();
		base.StartCoroutine(this.CreateLobby());
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x000A220F File Offset: 0x000A040F
	private IEnumerator CreateLobby()
	{
		while (PhotonNetwork.NetworkingClient.State != ClientState.Disconnected && PhotonNetwork.NetworkingClient.State != ClientState.PeerCreated)
		{
			yield return null;
		}
		if (GameManager.instance.connectRandom)
		{
			Debug.Log("Connect random.");
			SteamManager.instance.SendSteamAuthTicket();
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = DataDirector.instance.networkRegion;
			PhotonNetwork.ConnectUsingSettings();
			yield break;
		}
		if (!GameManager.instance.localTest)
		{
			bool flag = true;
			if (SteamManager.instance.currentLobby.Id.IsValid)
			{
				flag = (SteamManager.instance.currentLobby.GetData("HasPassword") == "1");
			}
			if (flag)
			{
				while (this.passwordTimer > 0f)
				{
					this.passwordTimer -= Time.deltaTime;
					yield return null;
				}
				AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.CutsceneOnly, 0.1f);
				this.menuPagePassword = MenuManager.instance.PageOpen(MenuPageIndex.Password, false);
				this.menuPagePassword.transform.SetParent(this.menuPagePassword.transform.parent.parent.parent);
				Transform _prevCursorParent = MenuCursor.instance.transform.parent;
				MenuCursor.instance.transform.SetParent(this.menuPagePassword.transform.parent, false);
				while (this.menuPagePassword)
				{
					MenuManager.instance.CutsceneSoundOverride();
					yield return null;
				}
				MenuCursor.instance.transform.SetParent(_prevCursorParent, false);
				AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.Off, 0.1f);
				_prevCursorParent = null;
			}
		}
		if (!GameManager.instance.localTest)
		{
			if (SteamManager.instance.currentLobby.Id.IsValid)
			{
				this.RoomName = SteamManager.instance.currentLobby.GetData("RoomName");
				PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = SteamManager.instance.currentLobby.GetData("Region");
				string data = SteamManager.instance.currentLobby.GetData("BuildName");
				if (data != BuildManager.instance.version.title)
				{
					if (data != "")
					{
						Debug.Log("Build name mismatch. Leaving lobby. Build name is ''" + data + "''");
						string bodyText = "Game lobby is using version\n<color=#FDFF00><b>" + data + "</b>";
						MenuManager.instance.PagePopUpScheduled("Wrong Game Version", Color.red, bodyText, "Ok Dang", true);
					}
					else
					{
						Debug.Log("Lobby closed. Leaving lobby.");
						MenuManager.instance.PagePopUpScheduled("Lobby Closed", Color.red, "The lobby has closed.", "Ok Dang", true);
					}
					PhotonNetwork.Disconnect();
					SteamManager.instance.LeaveLobby();
					GameManager.instance.SetGameMode(0);
					RunManager.instance.levelCurrent = RunManager.instance.levelMainMenu;
					SceneManager.LoadSceneAsync("Reload");
					yield break;
				}
				Debug.Log("Already in lobby on Network Connect. Connecting to region: " + PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion);
			}
			else
			{
				Debug.Log("Created lobby on Network Connect.");
				PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = DataDirector.instance.networkRegion;
				SteamManager.instance.HostLobby(false);
				while (!SteamManager.instance.currentLobby.Id.IsValid)
				{
					yield return null;
				}
				this.RoomName = SteamManager.instance.currentLobby.Id.ToString();
			}
			SteamManager.instance.SendSteamAuthTicket();
		}
		else
		{
			Debug.Log("Local test mode.");
			RunManager.instance.ResetProgress();
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
			this.RoomName = SteamClient.Name;
		}
		PhotonNetwork.ConnectUsingSettings();
		yield break;
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x000A2220 File Offset: 0x000A0420
	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Master Server");
		if (GameManager.instance.connectRandom)
		{
			if (!string.IsNullOrEmpty(DataDirector.instance.networkServerName))
			{
				Debug.Log("I am creating a custom open lobby named: " + DataDirector.instance.networkServerName);
				RoomOptions roomOptions = new RoomOptions();
				roomOptions.CustomRoomPropertiesForLobby = new string[]
				{
					"server_name"
				};
				RoomOptions roomOptions2 = roomOptions;
				Hashtable hashtable = new Hashtable();
				hashtable.Add("server_name", DataDirector.instance.networkServerName);
				roomOptions2.CustomRoomProperties = hashtable;
				roomOptions.MaxPlayers = 6;
				roomOptions.IsVisible = true;
				PhotonNetwork.CreateRoom(null, roomOptions, DataDirector.instance.customLobby, null);
				return;
			}
			if (!string.IsNullOrEmpty(DataDirector.instance.networkJoinServerName))
			{
				Debug.Log("Joining specific open server: " + DataDirector.instance.networkJoinServerName);
				PhotonNetwork.JoinRoom(DataDirector.instance.networkJoinServerName, null);
				return;
			}
			Debug.Log("I am joining or creating an open lobby.");
			PhotonNetwork.JoinRandomOrCreateRoom(null, 6, MatchmakingMode.FillRoom, TypedLobby.Default, null, null, new RoomOptions
			{
				MaxPlayers = 6,
				IsVisible = true
			}, null);
			return;
		}
		else
		{
			if (!GameManager.instance.localTest && SteamManager.instance.currentLobby.Id.IsValid && SteamManager.instance.currentLobby.IsOwnedBy(SteamClient.SteamId))
			{
				Debug.Log("I am the owner.");
				SteamManager.instance.SetLobbyData(this.RoomName);
				this.TryJoiningRoom();
				return;
			}
			Debug.Log("I am not the owner.");
			this.TryJoiningRoom();
			return;
		}
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x000A23A4 File Offset: 0x000A05A4
	private void TryJoiningRoom()
	{
		Debug.Log("Trying to join room: " + this.RoomName);
		Hashtable hashtable = new Hashtable();
		hashtable.Add("PASSWORD", DataDirector.instance.networkPassword);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
		RoomOptions roomOptions = new RoomOptions
		{
			MaxPlayers = 6,
			IsVisible = false
		};
		Hashtable hashtable2 = new Hashtable();
		hashtable2.Add("PASSWORD", DataDirector.instance.networkPassword);
		roomOptions.CustomRoomProperties = hashtable2;
		PhotonNetwork.JoinOrCreateRoom(this.RoomName, roomOptions, DataDirector.instance.privateLobby, null);
	}

	// Token: 0x060011CD RID: 4557 RVA: 0x000A243D File Offset: 0x000A063D
	public override void OnCreatedRoom()
	{
		Debug.Log("Created room successfully " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CloudRegion);
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x000A2464 File Offset: 0x000A0664
	public override void OnJoinedRoom()
	{
		Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CloudRegion);
		this.joinedRoom = true;
		PhotonNetwork.AutomaticallySyncScene = true;
		RunManager.instance.waitToChangeScene = false;
		if (!PhotonNetwork.IsMasterClient)
		{
			StatsManager.instance.saveFileCurrent = "";
		}
		if (GameManager.instance.connectRandom && PhotonNetwork.IsMasterClient)
		{
			Debug.Log("Created Open Room.");
			SemiFunc.SaveFileCreate();
			SceneManager.LoadSceneAsync("Main");
		}
		if (GameManager.instance.localTest && PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.LoadLevel("Reload");
		}
	}

	// Token: 0x060011CF RID: 4559 RVA: 0x000A250C File Offset: 0x000A070C
	public override void OnCreateRoomFailed(short returnCode, string cause)
	{
		Debug.LogError("Failed to create room: " + cause);
		MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "<b>Cause:\n</b>" + cause, "Ok Dang", true);
		PhotonNetwork.Disconnect();
		SteamManager.instance.LeaveLobby();
		GameManager.instance.SetGameMode(0);
		base.StartCoroutine(RunManager.instance.LeaveToMainMenu());
	}

	// Token: 0x060011D0 RID: 4560 RVA: 0x000A257C File Offset: 0x000A077C
	public override void OnJoinRoomFailed(short returnCode, string cause)
	{
		Debug.LogError("Failed to join room: " + cause);
		if (cause == "UserId found in excluded list")
		{
			cause = "You are banned from this lobby.";
		}
		MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "<b>Cause:\n</b>" + cause, "Ok Dang", true);
		PhotonNetwork.Disconnect();
		SteamManager.instance.LeaveLobby();
		GameManager.instance.SetGameMode(0);
		base.StartCoroutine(RunManager.instance.LeaveToMainMenu());
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x000A2600 File Offset: 0x000A0800
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log(string.Format("Disconnected from server for reason {0}", cause));
		if (cause != DisconnectCause.DisconnectByClientLogic && cause != DisconnectCause.DisconnectByServerLogic)
		{
			MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "<b>Cause:\n</b>" + cause.ToString(), "Ok Dang", true);
			PhotonNetwork.Disconnect();
			SteamManager.instance.LeaveLobby();
			GameManager.instance.SetGameMode(0);
			base.StartCoroutine(RunManager.instance.LeaveToMainMenu());
		}
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x000A2688 File Offset: 0x000A0888
	private void OnDestroy()
	{
		if (this.joinedRoom)
		{
			Debug.Log("Game Mode: Multiplayer");
			GameManager.instance.SetGameMode(1);
		}
		Debug.Log("NetworkConnect destroyed.");
		RunManager.instance.waitToChangeScene = false;
		DataDirector.instance.networkServerName = "";
		DataDirector.instance.networkJoinServerName = "";
	}

	// Token: 0x04001E2C RID: 7724
	private bool joinedRoom;

	// Token: 0x04001E2D RID: 7725
	private string RoomName;

	// Token: 0x04001E2E RID: 7726
	private bool ConnectedToMasterServer;

	// Token: 0x04001E2F RID: 7727
	public GameObject punVoiceClient;

	// Token: 0x04001E30 RID: 7728
	private bool passwordCheck;

	// Token: 0x04001E31 RID: 7729
	private MenuPage menuPagePassword;

	// Token: 0x04001E32 RID: 7730
	private float passwordTimer = 2f;
}
