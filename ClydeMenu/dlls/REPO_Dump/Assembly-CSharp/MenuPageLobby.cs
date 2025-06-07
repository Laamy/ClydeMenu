using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x0200021E RID: 542
public class MenuPageLobby : MonoBehaviour
{
	// Token: 0x06001204 RID: 4612 RVA: 0x000A33FD File Offset: 0x000A15FD
	private void Awake()
	{
		MenuPageLobby.instance = this;
		this.menuPage = base.GetComponent<MenuPage>();
		this.roomNameText.text = PhotonNetwork.CloudRegion + " " + PhotonNetwork.CurrentRoom.Name;
		this.UpdateChatPrompt();
	}

	// Token: 0x06001205 RID: 4613 RVA: 0x000A343C File Offset: 0x000A163C
	private void Start()
	{
		if (!SemiFunc.IsMasterClient())
		{
			this.inviteButton.transform.localPosition = new Vector3(this.startButton.transform.localPosition.x + 40f, this.startButton.transform.localPosition.y, this.startButton.transform.localPosition.z);
			this.inviteButton.buttonText.alignment = TextAlignmentOptions.Right;
			this.startButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x000A34D4 File Offset: 0x000A16D4
	private void Update()
	{
		if (this.joiningPlayersTimer > 0f)
		{
			this.joiningPlayersTimer -= Time.deltaTime;
		}
		else if (this.joiningPlayers.Count > 0)
		{
			this.joiningPlayers.Clear();
		}
		if (this.joiningPlayers.Count > 0 || this.joiningPlayersEndTimer > 0f)
		{
			this.joiningPlayer = true;
			this.joiningPlayersCanvasGroup.alpha = Mathf.Lerp(this.joiningPlayersCanvasGroup.alpha, 1f, Time.deltaTime * 10f);
			this.startButton.disabled = true;
		}
		else
		{
			this.joiningPlayersCanvasGroup.alpha = Mathf.Lerp(this.joiningPlayersCanvasGroup.alpha, 0f, Time.deltaTime * 10f);
			this.joiningPlayer = false;
			this.startButton.disabled = false;
		}
		if (this.joiningPlayersEndTimer > 0f)
		{
			this.joiningPlayersEndTimer -= Time.deltaTime;
		}
		this.listCheckTimer -= Time.deltaTime;
		if (this.listCheckTimer <= 0f)
		{
			this.listCheckTimer = 1f;
			List<PlayerAvatar> list = SemiFunc.PlayerGetList();
			bool flag = false;
			foreach (PlayerAvatar playerAvatar in list)
			{
				if (!this.lobbyPlayers.Contains(playerAvatar) && playerAvatar.playerAvatarVisuals.colorSet)
				{
					this.PlayerAdd(playerAvatar);
					flag = true;
				}
			}
			foreach (PlayerAvatar playerAvatar2 in Enumerable.ToList<PlayerAvatar>(this.lobbyPlayers))
			{
				if (!list.Contains(playerAvatar2))
				{
					this.PlayerRemove(playerAvatar2);
					flag = true;
				}
			}
			if (flag)
			{
				this.listObjects.Sort((GameObject a, GameObject b) => a.GetComponent<MenuPlayerListed>().playerAvatar.photonView.ViewID.CompareTo(b.GetComponent<MenuPlayerListed>().playerAvatar.photonView.ViewID));
				for (int i = 0; i < this.listObjects.Count; i++)
				{
					this.listObjects[i].GetComponent<MenuPlayerListed>().listSpot = i;
					this.listObjects[i].transform.SetSiblingIndex(i);
				}
			}
			foreach (GameObject gameObject in this.listObjects)
			{
				PlayerAvatar playerAvatar3 = gameObject.GetComponent<MenuPlayerListed>().playerAvatar;
				if (playerAvatar3)
				{
					if (playerAvatar3.photonView.Owner == PhotonNetwork.MasterClient)
					{
						gameObject.GetComponent<MenuPlayerListed>().playerName.text = playerAvatar3.playerName;
					}
					else
					{
						gameObject.GetComponent<MenuPlayerListed>().playerName.text = playerAvatar3.playerName;
					}
					this.SetPingText(gameObject.GetComponent<MenuPlayerListed>().pingText, playerAvatar3.playerPing);
				}
			}
		}
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x000A37EC File Offset: 0x000A19EC
	private void PlayerAdd(PlayerAvatar player)
	{
		this.lobbyPlayers.Add(player);
		GameObject gameObject = Object.Instantiate<GameObject>(this.menuPlayerListedPrefab, base.transform);
		MenuPlayerListed component = gameObject.GetComponent<MenuPlayerListed>();
		component.playerAvatar = player;
		component.playerHead.SetPlayer(player);
		component.GetComponent<RectTransform>().SetParent(this.playerListTransform);
		MenuSliderPlayerMicGain componentInChildren = component.GetComponentInChildren<MenuSliderPlayerMicGain>();
		componentInChildren.playerAvatar = player;
		if (player.isLocal)
		{
			Object.Destroy(componentInChildren.gameObject);
		}
		component.transform.localPosition = Vector3.zero;
		this.listObjects.Add(gameObject);
		this.menuPlayerListedList.Add(component);
		component.listSpot = Mathf.Max(this.listObjects.Count - 1, 0);
		foreach (string text in this.joiningPlayers)
		{
			if (player.playerName == text)
			{
				this.joiningPlayers.Remove(text);
				this.joiningPlayersEndTimer = 1f;
				break;
			}
		}
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x000A3910 File Offset: 0x000A1B10
	private void PlayerRemove(PlayerAvatar player)
	{
		this.lobbyPlayers.Remove(player);
		foreach (GameObject gameObject in this.listObjects)
		{
			if (gameObject.GetComponent<MenuPlayerListed>().playerAvatar == player)
			{
				gameObject.GetComponent<MenuPlayerListed>().MenuPlayerListedOutro();
				this.listObjects.Remove(gameObject);
				this.menuPlayerListedList.Remove(gameObject.GetComponent<MenuPlayerListed>());
				break;
			}
		}
		for (int i = 0; i < this.listObjects.Count; i++)
		{
			this.listObjects[i].GetComponent<MenuPlayerListed>().listSpot = i;
		}
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x000A39D8 File Offset: 0x000A1BD8
	private void SetPingText(TextMeshProUGUI text, int ping)
	{
		if (ping < 50)
		{
			text.color = new Color(0.2f, 0.8f, 0.2f);
		}
		else if (ping < 100)
		{
			text.color = new Color(0.8f, 0.8f, 0.2f);
		}
		else if (ping < 200)
		{
			text.color = new Color(0.8f, 0.4f, 0.2f);
		}
		else
		{
			text.color = new Color(0.8f, 0.2f, 0.2f);
		}
		text.text = ping.ToString() + " ms";
	}

	// Token: 0x0600120A RID: 4618 RVA: 0x000A3A7C File Offset: 0x000A1C7C
	public void JoiningPlayer(string playerName)
	{
		if (!this.joiningPlayers.Contains(playerName))
		{
			this.joiningPlayers.Add(playerName);
			this.joiningPlayersTimer = 10f;
		}
	}

	// Token: 0x0600120B RID: 4619 RVA: 0x000A3AA3 File Offset: 0x000A1CA3
	public void ChangeColorButton()
	{
		MenuManager.instance.PageOpenOnTop(MenuPageIndex.Color);
	}

	// Token: 0x0600120C RID: 4620 RVA: 0x000A3AB2 File Offset: 0x000A1CB2
	public void UpdateChatPrompt()
	{
		this.chatPromptText.text = InputManager.instance.InputDisplayReplaceTags("Press [chat] to chat");
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x000A3ACE File Offset: 0x000A1CCE
	public void ButtonLeave()
	{
		GameDirector.instance.OutroStart();
		NetworkManager.instance.leavePhotonRoom = true;
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x000A3AE5 File Offset: 0x000A1CE5
	public void ButtonSettings()
	{
		MenuManager.instance.PageOpenOnTop(MenuPageIndex.Settings);
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x000A3AF4 File Offset: 0x000A1CF4
	public void ButtonStart()
	{
		if (this.joiningPlayer)
		{
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, -1f, -1f, false);
			return;
		}
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;
		SteamManager.instance.LockLobby();
		DataDirector.instance.RunsPlayedAdd();
		if (RunManager.instance.loadLevel == 0)
		{
			RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.RunLevel);
			return;
		}
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Shop);
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x000A3B72 File Offset: 0x000A1D72
	public void ButtonInvite()
	{
		SteamManager.instance.OpenSteamOverlayToLobby();
	}

	// Token: 0x04001E6F RID: 7791
	public static MenuPageLobby instance;

	// Token: 0x04001E70 RID: 7792
	internal MenuPage menuPage;

	// Token: 0x04001E71 RID: 7793
	private float listCheckTimer;

	// Token: 0x04001E72 RID: 7794
	internal List<PlayerAvatar> lobbyPlayers = new List<PlayerAvatar>();

	// Token: 0x04001E73 RID: 7795
	internal List<GameObject> listObjects = new List<GameObject>();

	// Token: 0x04001E74 RID: 7796
	internal List<MenuPlayerListed> menuPlayerListedList = new List<MenuPlayerListed>();

	// Token: 0x04001E75 RID: 7797
	public GameObject menuPlayerListedPrefab;

	// Token: 0x04001E76 RID: 7798
	public RectTransform playerListTransform;

	// Token: 0x04001E77 RID: 7799
	public TextMeshProUGUI roomNameText;

	// Token: 0x04001E78 RID: 7800
	public TextMeshProUGUI chatPromptText;

	// Token: 0x04001E79 RID: 7801
	public MenuButton startButton;

	// Token: 0x04001E7A RID: 7802
	public MenuButton inviteButton;

	// Token: 0x04001E7B RID: 7803
	public CanvasGroup joiningPlayersCanvasGroup;

	// Token: 0x04001E7C RID: 7804
	private List<string> joiningPlayers = new List<string>();

	// Token: 0x04001E7D RID: 7805
	private float joiningPlayersTimer;

	// Token: 0x04001E7E RID: 7806
	private float joiningPlayersEndTimer;

	// Token: 0x04001E7F RID: 7807
	private bool joiningPlayer;
}
