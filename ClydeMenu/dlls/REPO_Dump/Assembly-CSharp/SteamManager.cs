using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

// Token: 0x0200024C RID: 588
public class SteamManager : MonoBehaviour
{
	// Token: 0x0600130B RID: 4875 RVA: 0x000AA608 File Offset: 0x000A8808
	private void Awake()
	{
		if (!SteamManager.instance)
		{
			SteamManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			try
			{
				SteamClient.Init(3241660U, true);
			}
			catch (Exception ex)
			{
				Debug.LogError("Steamworks failed to initialize. Error: " + ex.Message);
			}
			Debug.Log("STEAM ID: " + SteamClient.SteamId.ToString());
			if (Debug.isDebugBuild)
			{
				foreach (SteamManager.Developer developer in this.developerList)
				{
					if (SteamClient.SteamId.ToString() == developer.steamID)
					{
						Debug.Log("DEVELOPER MODE: " + developer.name.ToUpper());
						this.developerMode = true;
					}
				}
			}
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x000AA71C File Offset: 0x000A891C
	private void OnEnable()
	{
		SteamMatchmaking.OnLobbyCreated += new Action<Result, Lobby>(this.OnLobbyCreated);
		SteamMatchmaking.OnLobbyEntered += new Action<Lobby>(this.OnLobbyEntered);
		SteamFriends.OnGameLobbyJoinRequested += new Action<Lobby, SteamId>(this.OnGameLobbyJoinRequested);
		SteamMatchmaking.OnLobbyMemberJoined += new Action<Lobby, Friend>(this.OnLobbyMemberJoined);
		SteamMatchmaking.OnLobbyMemberLeave += new Action<Lobby, Friend>(this.OnLobbyMemberLeft);
		SteamMatchmaking.OnLobbyMemberDataChanged += new Action<Lobby, Friend>(this.OnLobbyMemberDataChanged);
		SteamFriends.OnGameOverlayActivated += new Action<bool>(this.OnGameOverlayActivated);
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x000AA7A0 File Offset: 0x000A89A0
	private void Start()
	{
		this.GetSteamAuthTicket(out this.steamAuthTicket);
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		if (commandLineArgs.Length >= 2)
		{
			int i = 0;
			while (i < commandLineArgs.Length - 1)
			{
				if (commandLineArgs[i].ToLower() == "+connect_lobby")
				{
					ulong num;
					if (ulong.TryParse(commandLineArgs[i + 1], ref num) && num > 0UL)
					{
						Debug.Log("Auto-Connecting to lobby: " + num.ToString());
						this.OnGameLobbyJoinRequested(new Lobby(num), SteamClient.SteamId);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x000AA82A File Offset: 0x000A8A2A
	private void OnLobbyMemberJoined(Lobby _lobby, Friend _friend)
	{
		Debug.Log("Steam: Lobby member joined: " + _friend.Name);
		if (this.privateLobby && MenuPageLobby.instance)
		{
			MenuPageLobby.instance.JoiningPlayer(_friend.Name);
		}
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x000AA867 File Offset: 0x000A8A67
	private void OnLobbyMemberLeft(Lobby _lobby, Friend _friend)
	{
		Debug.Log("Steam: Lobby member left: " + _friend.Name);
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x000AA880 File Offset: 0x000A8A80
	private void OnLobbyMemberDataChanged(Lobby _lobby, Friend _friend)
	{
		Debug.Log(" ");
		Debug.Log("Steam: Lobby member data changed for: " + _friend.Name);
		Debug.Log("I am " + SteamClient.Name);
		Debug.Log("Current Owner: " + _lobby.Owner.Name);
		if (PhotonNetwork.IsMasterClient && RunManager.instance.masterSwitched && SteamClient.SteamId == _lobby.Owner.Id)
		{
			Debug.Log("I am the new owner and i am locking the lobby.");
			this.LockLobby();
		}
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x000AA91F File Offset: 0x000A8B1F
	private void OnDestroy()
	{
		if (SteamManager.instance == this)
		{
			this.CancelSteamAuthTicket();
			SteamClient.Shutdown();
		}
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x000AA93C File Offset: 0x000A8B3C
	private void OnGameLobbyJoinRequested(Lobby _lobby, SteamId _steamID)
	{
		SteamManager.<OnGameLobbyJoinRequested>d__17 <OnGameLobbyJoinRequested>d__;
		<OnGameLobbyJoinRequested>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnGameLobbyJoinRequested>d__.<>4__this = this;
		<OnGameLobbyJoinRequested>d__._lobby = _lobby;
		<OnGameLobbyJoinRequested>d__.<>1__state = -1;
		<OnGameLobbyJoinRequested>d__.<>t__builder.Start<SteamManager.<OnGameLobbyJoinRequested>d__17>(ref <OnGameLobbyJoinRequested>d__);
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x000AA97C File Offset: 0x000A8B7C
	private void OnLobbyEntered(Lobby _lobby)
	{
		this.currentLobby.Leave();
		this.currentLobby = _lobby;
		Debug.Log("Steam: Lobby entered with ID: " + _lobby.Id.ToString());
		Debug.Log("Steam: Region: " + _lobby.GetData("Region"));
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x000AA9DC File Offset: 0x000A8BDC
	private void OnLobbyCreated(Result _result, Lobby _lobby)
	{
		if (_result == Result.OK)
		{
			Debug.Log("Steam: Lobby created with ID: " + _lobby.Id.ToString());
			return;
		}
		Debug.LogError("Steam: Failed to create lobby. Error: " + _result.ToString());
		NetworkManager.instance.LeavePhotonRoom();
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x000AAA38 File Offset: 0x000A8C38
	public void HostLobby(bool _open)
	{
		SteamManager.<HostLobby>d__20 <HostLobby>d__;
		<HostLobby>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<HostLobby>d__.<>4__this = this;
		<HostLobby>d__._open = _open;
		<HostLobby>d__.<>1__state = -1;
		<HostLobby>d__.<>t__builder.Start<SteamManager.<HostLobby>d__20>(ref <HostLobby>d__);
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x000AAA78 File Offset: 0x000A8C78
	public void LeaveLobby()
	{
		if (this.currentLobby.IsOwnedBy(SteamClient.SteamId))
		{
			Debug.Log("Steam: Leaving lobby... and ruining it for others.");
			this.currentLobby.SetData("BuildName", "");
		}
		else
		{
			Debug.Log("Steam: Leaving lobby...");
		}
		this.CancelSteamAuthTicket();
		this.currentLobby.Leave();
		this.currentLobby = this.noLobby;
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x000AAAE0 File Offset: 0x000A8CE0
	public void UnlockLobby(bool _open)
	{
		Debug.Log("Steam: Unlocking lobby...");
		if (_open)
		{
			this.currentLobby.SetPublic();
			this.privateLobby = false;
		}
		else
		{
			this.currentLobby.SetPrivate();
			this.currentLobby.SetFriendsOnly();
			this.privateLobby = true;
		}
		this.currentLobby.SetJoinable(true);
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x000AAB3B File Offset: 0x000A8D3B
	public void LockLobby()
	{
		Debug.Log("Steam: Locking lobby...");
		this.currentLobby.SetPrivate();
		this.currentLobby.SetFriendsOnly();
		this.currentLobby.SetJoinable(false);
		this.privateLobby = true;
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x000AAB73 File Offset: 0x000A8D73
	public void JoinLobby(SteamId _lobbyID)
	{
		Debug.Log("Steam: Joining lobby...");
		SteamMatchmaking.JoinLobbyAsync(_lobbyID);
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x000AAB88 File Offset: 0x000A8D88
	public void SetLobbyData(string _roomName)
	{
		Debug.Log("Steam: Setting lobby data...");
		this.currentLobby.SetData("Region", PhotonNetwork.CloudRegion);
		this.currentLobby.SetData("BuildName", BuildManager.instance.version.title);
		this.currentLobby.SetData("RoomName", _roomName);
		if (this.privateLobby && !string.IsNullOrEmpty(DataDirector.instance.networkPassword))
		{
			this.currentLobby.SetData("HasPassword", "1");
			return;
		}
		this.currentLobby.SetData("HasPassword", "0");
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x000AAC30 File Offset: 0x000A8E30
	public void SendSteamAuthTicket()
	{
		Debug.Log("Sending Steam Auth Ticket...");
		string value = this.GetSteamAuthTicket(out this.steamAuthTicket);
		PhotonNetwork.AuthValues = new AuthenticationValues();
		PhotonNetwork.AuthValues.UserId = SteamClient.SteamId.ToString();
		PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Steam;
		PhotonNetwork.AuthValues.AddAuthParameter("ticket", value);
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x000AAC98 File Offset: 0x000A8E98
	private string GetSteamAuthTicket(out AuthTicket ticket)
	{
		Debug.Log("Getting Steam Auth Ticket...");
		ticket = SteamUser.GetAuthSessionTicket();
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < ticket.Data.Length; i++)
		{
			stringBuilder.AppendFormat("{0:x2}", ticket.Data[i]);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x000AACF0 File Offset: 0x000A8EF0
	public void CancelSteamAuthTicket()
	{
		Debug.Log("Cancelling Steam Auth Ticket...");
		if (this.steamAuthTicket == null)
		{
			return;
		}
		this.steamAuthTicket.Cancel();
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x000AAD10 File Offset: 0x000A8F10
	public void OpenSteamOverlayToLobby()
	{
		SteamFriends.OpenOverlay("friends");
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x000AAD1C File Offset: 0x000A8F1C
	private void OnGameOverlayActivated(bool obj)
	{
		InputManager.instance.ResetInput();
	}

	// Token: 0x04002076 RID: 8310
	public static SteamManager instance;

	// Token: 0x04002077 RID: 8311
	internal Lobby currentLobby;

	// Token: 0x04002078 RID: 8312
	internal Lobby noLobby;

	// Token: 0x04002079 RID: 8313
	internal bool joinLobby;

	// Token: 0x0400207A RID: 8314
	private bool privateLobby;

	// Token: 0x0400207B RID: 8315
	public GameObject networkConnectPrefab;

	// Token: 0x0400207C RID: 8316
	internal AuthTicket steamAuthTicket;

	// Token: 0x0400207D RID: 8317
	[Space]
	public List<SteamManager.Developer> developerList;

	// Token: 0x0400207E RID: 8318
	internal bool developerMode;

	// Token: 0x020003FB RID: 1019
	[Serializable]
	public class Developer
	{
		// Token: 0x04002D52 RID: 11602
		public string name;

		// Token: 0x04002D53 RID: 11603
		public string steamID;
	}
}
