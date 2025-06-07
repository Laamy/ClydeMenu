using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class MenuPageServerList : MonoBehaviourPunCallbacks
{
	// Token: 0x06001262 RID: 4706 RVA: 0x000A5D45 File Offset: 0x000A3F45
	private void Start()
	{
		this.menuPage = base.GetComponent<MenuPage>();
		this.buttonNext.HideSetInstant();
		this.buttonPrevious.HideSetInstant();
		base.StartCoroutine(this.GetServerList());
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x000A5D76 File Offset: 0x000A3F76
	private IEnumerator GetServerList()
	{
		PhotonNetwork.Disconnect();
		while (PhotonNetwork.NetworkingClient.State != ClientState.Disconnected && PhotonNetwork.NetworkingClient.State != ClientState.PeerCreated)
		{
			yield return null;
		}
		SteamManager.instance.SendSteamAuthTicket();
		DataDirector.instance.PhotonSetRegion();
		DataDirector.instance.PhotonSetVersion();
		DataDirector.instance.PhotonSetAppId();
		PhotonNetwork.ConnectUsingSettings();
		while (!this.receivedList)
		{
			yield return null;
		}
		this.loadingGraphics.SetDone();
		float _pitch = 1f;
		float _positionY = 0f;
		int _rooms = 0;
		foreach (MenuPageServerList.ServerListRoom room in this.roomList)
		{
			this.CreateServerElement(ref _positionY, room, _rooms, MenuElementServer.IntroType.Vertical);
			float pitch = MenuManager.instance.soundPageIntro.Pitch;
			MenuManager.instance.soundPageIntro.Pitch = 1f + _pitch;
			MenuManager.instance.soundPageIntro.Play(Vector3.zero, 0.75f, 1f, 1f, 1f);
			MenuManager.instance.soundPageIntro.Pitch = pitch;
			_pitch += 0.1f;
			int num = _rooms;
			_rooms = num + 1;
			if (_rooms >= this.pageRooms)
			{
				break;
			}
			yield return new WaitForSecondsRealtime(0.1f);
		}
		List<MenuPageServerList.ServerListRoom>.Enumerator enumerator = default(List<MenuPageServerList.ServerListRoom>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x06001264 RID: 4708 RVA: 0x000A5D85 File Offset: 0x000A3F85
	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby(DataDirector.instance.customLobby);
	}

	// Token: 0x06001265 RID: 4709 RVA: 0x000A5D98 File Offset: 0x000A3F98
	public override void OnRoomListUpdate(List<RoomInfo> _roomList)
	{
		this.roomList.Clear();
		foreach (RoomInfo roomInfo in _roomList)
		{
			if (!roomInfo.RemovedFromList && roomInfo.IsOpen && roomInfo.PlayerCount < roomInfo.MaxPlayers)
			{
				MenuPageServerList.ServerListRoom serverListRoom = new MenuPageServerList.ServerListRoom();
				serverListRoom.displayName = (string)roomInfo.CustomProperties["server_name"];
				serverListRoom.roomName = roomInfo.Name;
				serverListRoom.playerCount = roomInfo.PlayerCount;
				serverListRoom.maxPlayers = roomInfo.MaxPlayers;
				this.roomList.Add(serverListRoom);
			}
		}
		this.roomList.Shuffle<MenuPageServerList.ServerListRoom>();
		this.SetPageLogic();
		this.receivedList = true;
		PhotonNetwork.Disconnect();
	}

	// Token: 0x06001266 RID: 4710 RVA: 0x000A5E78 File Offset: 0x000A4078
	private void Update()
	{
		if (this.pageMax == 0 || this.searchInProgress)
		{
			this.buttonNext.Hide();
			this.buttonPrevious.Hide();
		}
		else
		{
			if (this.pageCurrent >= this.pageMax)
			{
				this.buttonNext.Hide();
			}
			if (this.pageCurrent <= 0)
			{
				this.buttonPrevious.Hide();
			}
		}
		if (this.searchOffsetLerp < 1f)
		{
			this.searchOffsetLerp += Time.deltaTime * 5f;
			if (this.searchActive)
			{
				this.searchOffset.anchoredPosition = new Vector3(0f, Mathf.LerpUnclamped(0f, 8f, this.searchOffsetCurve.Evaluate(this.searchOffsetLerp)), 0f);
				this.searchTextCanvas.alpha = Mathf.Lerp(this.searchTextCanvas.alpha, 1f, this.searchOffsetLerp);
			}
			else
			{
				this.searchOffset.anchoredPosition = new Vector3(0f, Mathf.LerpUnclamped(8f, 0f, this.searchOffsetCurve.Evaluate(this.searchOffsetLerp)), 0f);
				this.searchTextCanvas.alpha = Mathf.Lerp(this.searchTextCanvas.alpha, 0f, this.searchOffsetLerp);
			}
		}
		if (SemiFunc.InputDown(InputKey.Back) && MenuManager.instance.currentMenuPageIndex == MenuPageIndex.ServerList)
		{
			this.ExitPage();
		}
	}

	// Token: 0x06001267 RID: 4711 RVA: 0x000A5FF3 File Offset: 0x000A41F3
	public void ExitPage()
	{
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.PublicGameChoice, false);
	}

	// Token: 0x06001268 RID: 4712 RVA: 0x000A6010 File Offset: 0x000A4210
	private void CreateServerElement(ref float _positionY, MenuPageServerList.ServerListRoom _room, int _index, MenuElementServer.IntroType _introType)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.serverElementPrefab, this.serverElementParent);
		gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, _positionY, 0f);
		MenuElementServer component = gameObject.GetComponent<MenuElementServer>();
		component.textName.text = _room.displayName;
		component.textPlayers.text = _room.playerCount.ToString() + "/" + _room.maxPlayers.ToString();
		component.roomName = _room.roomName;
		component.introType = _introType;
		component.menuButtonPopUp.bodyText = "Are you sure you want to join\n''" + _room.displayName + "''";
		_positionY -= 28f;
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x000A60CD File Offset: 0x000A42CD
	private IEnumerator UpdatePage()
	{
		foreach (object obj in this.serverElementParent)
		{
			Object.Destroy(((Transform)obj).gameObject);
		}
		MenuElementServer.IntroType _introType = MenuElementServer.IntroType.Right;
		if (this.pagePrevious > this.pageCurrent)
		{
			_introType = MenuElementServer.IntroType.Left;
		}
		List<MenuPageServerList.ServerListRoom> _list = this.roomList;
		if (this.searchActive)
		{
			_list = this.roomListSearched;
		}
		float _pitch = 1f;
		float _positionY = 0f;
		int _page = this.pageCurrent * this.pageRooms;
		int i = 0;
		while (i < this.pageRooms && _page < _list.Count)
		{
			float pitch = MenuManager.instance.soundPageIntro.Pitch;
			MenuManager.instance.soundPageIntro.Pitch = 1f + _pitch;
			MenuManager.instance.soundPageIntro.Play(Vector3.zero, 0.5f, 1f, 1f, 1f);
			MenuManager.instance.soundPageIntro.Pitch = pitch;
			this.CreateServerElement(ref _positionY, _list[_page], _page, _introType);
			yield return new WaitForSecondsRealtime(0.05f);
			_pitch += 0.1f;
			int num = _page;
			_page = num + 1;
			num = i;
			i = num + 1;
		}
		this.pagePrevious = this.pageCurrent;
		yield break;
	}

	// Token: 0x0600126A RID: 4714 RVA: 0x000A60DC File Offset: 0x000A42DC
	private IEnumerator SearchLogic()
	{
		this.searchInProgress = true;
		foreach (object obj in this.serverElementParent)
		{
			Object.Destroy(((Transform)obj).gameObject);
		}
		this.loadingGraphics.Reset();
		this.roomListSearched.Clear();
		List<MenuPageServerList.ServerListRoom>.Enumerator enumerator3;
		if (this.searchActive)
		{
			List<string> _searchTerms = new List<string>();
			foreach (string text in this.searchString.Split(' ', 0))
			{
				_searchTerms.Add(text);
			}
			this.searchText.text = "''";
			foreach (string text2 in _searchTerms)
			{
				if (_searchTerms.IndexOf(text2) == 0)
				{
					TextMeshProUGUI textMeshProUGUI = this.searchText;
					textMeshProUGUI.text += text2;
				}
				else
				{
					TextMeshProUGUI textMeshProUGUI2 = this.searchText;
					textMeshProUGUI2.text = textMeshProUGUI2.text + " + " + text2;
				}
			}
			TextMeshProUGUI textMeshProUGUI3 = this.searchText;
			textMeshProUGUI3.text += "''";
			int _logicCurrent = 0;
			int _logicMax = 5;
			foreach (MenuPageServerList.ServerListRoom _room in this.roomList)
			{
				bool _add = true;
				foreach (string _term in _searchTerms)
				{
					int i = _logicCurrent;
					_logicCurrent = i + 1;
					if (_logicCurrent >= _logicMax)
					{
						_logicCurrent = 0;
						yield return null;
					}
					if (!_room.displayName.ToLower().Contains(_term.ToLower()))
					{
						_add = false;
						break;
					}
					_term = null;
				}
				List<string>.Enumerator enumerator4 = default(List<string>.Enumerator);
				if (_add)
				{
					this.roomListSearched.Add(_room);
				}
				_room = null;
			}
			enumerator3 = default(List<MenuPageServerList.ServerListRoom>.Enumerator);
			this.roomListSearched.Shuffle<MenuPageServerList.ServerListRoom>();
			_searchTerms = null;
		}
		else
		{
			yield return new WaitForSeconds(1f);
		}
		this.SetPageLogic();
		this.loadingGraphics.SetDone();
		float _pitch = 1f;
		float _positionY = 0f;
		List<MenuPageServerList.ServerListRoom> list = this.roomList;
		if (this.searchActive)
		{
			list = this.roomListSearched;
		}
		int _rooms = 0;
		foreach (MenuPageServerList.ServerListRoom room in list)
		{
			this.CreateServerElement(ref _positionY, room, _rooms, MenuElementServer.IntroType.Vertical);
			float pitch = MenuManager.instance.soundPageIntro.Pitch;
			MenuManager.instance.soundPageIntro.Pitch = 1f + _pitch;
			MenuManager.instance.soundPageIntro.Play(Vector3.zero, 0.75f, 1f, 1f, 1f);
			MenuManager.instance.soundPageIntro.Pitch = pitch;
			_pitch += 0.1f;
			int i = _rooms;
			_rooms = i + 1;
			if (_rooms >= this.pageRooms)
			{
				break;
			}
			yield return new WaitForSecondsRealtime(0.1f);
		}
		enumerator3 = default(List<MenuPageServerList.ServerListRoom>.Enumerator);
		this.searchInProgress = false;
		yield break;
		yield break;
	}

	// Token: 0x0600126B RID: 4715 RVA: 0x000A60EC File Offset: 0x000A42EC
	public void SetSearch(string _searchString)
	{
		this.searchString = _searchString;
		bool flag = this.searchActive;
		if (!string.IsNullOrEmpty(this.searchString))
		{
			this.searchActive = true;
		}
		else
		{
			this.searchActive = false;
		}
		if (this.searchActive != flag)
		{
			this.searchOffsetLerp = 0f;
		}
		base.StartCoroutine(this.SearchLogic());
	}

	// Token: 0x0600126C RID: 4716 RVA: 0x000A6148 File Offset: 0x000A4348
	private void SetPageLogic()
	{
		this.pageCurrent = 0;
		this.pageMax = 0;
		List<MenuPageServerList.ServerListRoom> list = this.roomList;
		if (this.searchActive)
		{
			list = this.roomListSearched;
		}
		if (list.Count > this.pageRooms)
		{
			this.pageMax = Mathf.CeilToInt((float)(list.Count / this.pageRooms));
		}
		if (this.pageMax > 0 && this.pageMax * this.pageRooms == list.Count)
		{
			this.pageMax--;
		}
	}

	// Token: 0x0600126D RID: 4717 RVA: 0x000A61CC File Offset: 0x000A43CC
	private void OnDestroy()
	{
		PhotonNetwork.Disconnect();
	}

	// Token: 0x0600126E RID: 4718 RVA: 0x000A61D3 File Offset: 0x000A43D3
	public void ButtonCreateNew()
	{
		if (this.searchInProgress)
		{
			return;
		}
		MenuManager.instance.PageOpenOnTop(MenuPageIndex.ServerListCreateNew).GetComponent<MenuPageServerListCreateNew>().menuPageParent = this.menuPage;
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x000A61FA File Offset: 0x000A43FA
	public void ButtonSearch()
	{
		if (this.searchInProgress)
		{
			return;
		}
		MenuPageServerListSearch component = MenuManager.instance.PageOpenOnTop(MenuPageIndex.ServerListSearch).GetComponent<MenuPageServerListSearch>();
		component.menuPageParent = this.menuPage;
		component.menuPageServerList = this;
	}

	// Token: 0x06001270 RID: 4720 RVA: 0x000A6228 File Offset: 0x000A4428
	public void ButtonNextPage()
	{
		if (this.searchInProgress)
		{
			return;
		}
		if (this.pageCurrent < this.pageMax)
		{
			MenuManager.instance.soundPageIntro.Play(Vector3.zero, 0.75f, 1f, 1f, 1f);
			this.pageCurrent++;
			base.StopAllCoroutines();
			base.StartCoroutine(this.UpdatePage());
		}
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x000A6298 File Offset: 0x000A4498
	public void ButtonPreviousPage()
	{
		if (this.searchInProgress)
		{
			return;
		}
		if (this.pageCurrent > 0)
		{
			MenuManager.instance.soundPageOutro.Play(Vector3.zero, 0.75f, 1f, 1f, 1f);
			this.pageCurrent--;
			base.StopAllCoroutines();
			base.StartCoroutine(this.UpdatePage());
		}
	}

	// Token: 0x04001EEE RID: 7918
	public GameObject serverElementPrefab;

	// Token: 0x04001EEF RID: 7919
	public Transform serverElementParent;

	// Token: 0x04001EF0 RID: 7920
	public MenuLoadingGraphics loadingGraphics;

	// Token: 0x04001EF1 RID: 7921
	private bool receivedList;

	// Token: 0x04001EF2 RID: 7922
	private List<MenuPageServerList.ServerListRoom> roomList = new List<MenuPageServerList.ServerListRoom>();

	// Token: 0x04001EF3 RID: 7923
	private List<MenuPageServerList.ServerListRoom> roomListSearched = new List<MenuPageServerList.ServerListRoom>();

	// Token: 0x04001EF4 RID: 7924
	private int pageRooms = 8;

	// Token: 0x04001EF5 RID: 7925
	private int pageCurrent;

	// Token: 0x04001EF6 RID: 7926
	private int pagePrevious = -1;

	// Token: 0x04001EF7 RID: 7927
	private int pageMax;

	// Token: 0x04001EF8 RID: 7928
	private MenuPage menuPage;

	// Token: 0x04001EF9 RID: 7929
	[Space]
	public MenuButtonArrow buttonNext;

	// Token: 0x04001EFA RID: 7930
	public MenuButtonArrow buttonPrevious;

	// Token: 0x04001EFB RID: 7931
	[Space]
	public RectTransform searchOffset;

	// Token: 0x04001EFC RID: 7932
	public CanvasGroup searchTextCanvas;

	// Token: 0x04001EFD RID: 7933
	public TextMeshProUGUI searchText;

	// Token: 0x04001EFE RID: 7934
	public AnimationCurve searchOffsetCurve;

	// Token: 0x04001EFF RID: 7935
	private float searchOffsetLerp;

	// Token: 0x04001F00 RID: 7936
	internal string searchString;

	// Token: 0x04001F01 RID: 7937
	private bool searchActive;

	// Token: 0x04001F02 RID: 7938
	private bool searchInProgress;

	// Token: 0x020003EE RID: 1006
	private class ServerListRoom
	{
		// Token: 0x04002CD8 RID: 11480
		public string displayName;

		// Token: 0x04002CD9 RID: 11481
		public string roomName;

		// Token: 0x04002CDA RID: 11482
		public int playerCount;

		// Token: 0x04002CDB RID: 11483
		public int maxPlayers;
	}
}
