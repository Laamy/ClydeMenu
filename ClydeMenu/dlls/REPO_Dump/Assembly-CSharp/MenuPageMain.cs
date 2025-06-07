using System;
using UnityEngine;

// Token: 0x0200021F RID: 543
public class MenuPageMain : MonoBehaviour
{
	// Token: 0x06001212 RID: 4626 RVA: 0x000A3BB2 File Offset: 0x000A1DB2
	private void Awake()
	{
		MenuPageMain.instance = this;
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x000A3BBC File Offset: 0x000A1DBC
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.menuPage = base.GetComponent<MenuPage>();
		if (MainMenuOpen.instance.firstOpen)
		{
			MainMenuOpen.instance.firstOpen = false;
			this.menuPage.disableOutroAnimation = false;
		}
		else
		{
			this.menuPage.disableIntroAnimation = false;
			this.menuPage.disableOutroAnimation = false;
			this.doIntroAnimation = false;
		}
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.TutorialPlayed) <= 0)
		{
			this.tutorialButtonBlinkActive = true;
			this.tutorialButton.customColors = true;
			this.tutorialButton.colorNormal = new Color(1f, 0.55f, 0f);
			this.tutorialButton.colorHover = Color.white;
			this.tutorialButton.colorClick = new Color(1f, 0.55f, 0f);
		}
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x000A3C98 File Offset: 0x000A1E98
	private void Update()
	{
		if (this.tutorialButtonBlinkActive)
		{
			if (this.tutorialButtonTimer <= 0f)
			{
				this.tutorialButtonTimer = 0.5f;
				this.tutorialButtonBlink = !this.tutorialButtonBlink;
				if (this.tutorialButtonBlink)
				{
					this.tutorialButton.colorNormal = Color.white;
				}
				else
				{
					this.tutorialButton.colorNormal = new Color(1f, 0.55f, 0f);
				}
			}
			else
			{
				this.tutorialButtonTimer -= Time.deltaTime;
			}
		}
		if (this.menuPage.currentPageState == MenuPage.PageState.Closing)
		{
			return;
		}
		if (RunManager.instance.localMultiplayerTest)
		{
			GameManager.instance.localTest = true;
			RunManager.instance.localMultiplayerTest = false;
			RunManager.instance.ResetProgress();
			RunManager.instance.waitToChangeScene = true;
			RunManager.instance.lobbyJoin = true;
			RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.LobbyMenu);
			SteamManager.instance.joinLobby = false;
		}
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.popUpTimer > 0f)
		{
			this.popUpTimer -= Time.deltaTime;
			if (this.popUpTimer <= 0f)
			{
				MenuManager.instance.PagePopUpScheduledShow();
			}
		}
		if (this.doIntroAnimation)
		{
			this.waitTimer += Time.deltaTime;
			if (this.waitTimer > 3f)
			{
				this.animateIn = true;
			}
			else
			{
				this.rectTransform.localPosition = new Vector3(-600f, 0f, 0f);
			}
			if (this.animateIn)
			{
				this.rectTransform.localPosition = new Vector3(Mathf.Lerp(this.rectTransform.localPosition.x, 0f, Time.deltaTime * 2f), 0f, 0f);
				if (Mathf.Abs(this.rectTransform.localPosition.x) < 50f && !this.introDone)
				{
					this.menuPage.PageStateSet(MenuPage.PageState.Active);
					this.introDone = true;
				}
			}
		}
		else if (!this.introDone)
		{
			this.menuPage.PageStateSet(MenuPage.PageState.Active);
			this.introDone = true;
		}
		if (SteamManager.instance.joinLobby)
		{
			if (this.joinLobbyTimer > 0f)
			{
				this.joinLobbyTimer -= Time.deltaTime;
				return;
			}
			GameManager.instance.localTest = false;
			RunManager.instance.ResetProgress();
			RunManager.instance.waitToChangeScene = true;
			RunManager.instance.lobbyJoin = true;
			RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.LobbyMenu);
			SteamManager.instance.joinLobby = false;
		}
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x000A3F2C File Offset: 0x000A212C
	public void ButtonEventSinglePlayer()
	{
		SemiFunc.MainMenuSetSingleplayer();
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.Saves, false);
	}

	// Token: 0x06001216 RID: 4630 RVA: 0x000A3F4B File Offset: 0x000A214B
	public void ButtonEventTutorial()
	{
		DataDirector.instance.TutorialPlayed();
		TutorialDirector.instance.Reset();
		RunManager.instance.ResetProgress();
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Tutorial);
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x000A3F78 File Offset: 0x000A2178
	public void ButtonEventHostGame()
	{
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.Regions, false).GetComponent<MenuPageRegions>().type = MenuPageRegions.Type.HostGame;
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x000A3F9C File Offset: 0x000A219C
	public void ButtonEventJoinGame()
	{
		SteamManager.instance.OpenSteamOverlayToLobby();
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x000A3FA8 File Offset: 0x000A21A8
	public void ButtonEventPlayRandom()
	{
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.Regions, false).GetComponent<MenuPageRegions>().type = MenuPageRegions.Type.PlayRandom;
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x000A3FCC File Offset: 0x000A21CC
	public void ButtonEventQuit()
	{
		RunManager.instance.skipLoadingUI = true;
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			playerAvatar.quitApplication = true;
		}
		GameDirector.instance.OutroStart();
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x000A4038 File Offset: 0x000A2238
	public void ButtonEventSettings()
	{
		MenuManager.instance.PageOpenOnTop(MenuPageIndex.Settings);
	}

	// Token: 0x04001E80 RID: 7808
	public static MenuPageMain instance;

	// Token: 0x04001E81 RID: 7809
	private RectTransform rectTransform;

	// Token: 0x04001E82 RID: 7810
	private float waitTimer;

	// Token: 0x04001E83 RID: 7811
	private bool animateIn;

	// Token: 0x04001E84 RID: 7812
	internal MenuPage menuPage;

	// Token: 0x04001E85 RID: 7813
	private bool introDone;

	// Token: 0x04001E86 RID: 7814
	public GameObject networkConnectPrefab;

	// Token: 0x04001E87 RID: 7815
	private float joinLobbyTimer = 0.1f;

	// Token: 0x04001E88 RID: 7816
	private float popUpTimer = 1.5f;

	// Token: 0x04001E89 RID: 7817
	private bool doIntroAnimation = true;

	// Token: 0x04001E8A RID: 7818
	public MenuButton tutorialButton;

	// Token: 0x04001E8B RID: 7819
	private bool tutorialButtonBlinkActive;

	// Token: 0x04001E8C RID: 7820
	private bool tutorialButtonBlink;

	// Token: 0x04001E8D RID: 7821
	private float tutorialButtonTimer;
}
