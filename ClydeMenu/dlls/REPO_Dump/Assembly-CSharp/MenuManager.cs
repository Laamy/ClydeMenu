using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001FE RID: 510
public class MenuManager : MonoBehaviour
{
	// Token: 0x06001129 RID: 4393 RVA: 0x0009D0A1 File Offset: 0x0009B2A1
	private void Awake()
	{
		if (!MenuManager.instance)
		{
			MenuManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x0009D0CC File Offset: 0x0009B2CC
	private void Start()
	{
		this.StateSet(MenuManager.MenuState.Closed);
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x0009D0D8 File Offset: 0x0009B2D8
	private void Update()
	{
		if (PlayerController.instance)
		{
			this.soundPosition = PlayerController.instance.transform.position;
		}
		else
		{
			this.soundPosition = base.transform.position;
		}
		int num = this.currentMenuState;
		if (num != 0)
		{
			if (num == 1)
			{
				this.StateClosed();
				this.stateStart = false;
			}
		}
		else
		{
			this.StateOpen();
			this.stateStart = false;
		}
		if (Input.GetMouseButton(0))
		{
			if (this.mouseHoldPosition == Vector2.zero)
			{
				this.mouseHoldPosition = SemiFunc.UIMousePosToUIPos();
			}
		}
		else
		{
			this.mouseHoldPosition = Vector2.zero;
		}
		if (this.textInputActive)
		{
			if (this.textInputActiveTimer <= 0f)
			{
				this.textInputActive = false;
			}
			this.textInputActiveTimer -= Time.deltaTime;
		}
		if (this.cutsceneSoundOverride)
		{
			if (this.cutsceneSoundOverrideTimer <= 0f)
			{
				this.cutsceneSoundOverride = false;
				this.soundAction.Type = this.defaultSoundType;
				this.soundConfirm.Type = this.defaultSoundType;
				this.soundDeny.Type = this.defaultSoundType;
				this.soundDud.Type = this.defaultSoundType;
				this.soundTick.Type = this.defaultSoundType;
				this.soundHover.Type = this.defaultSoundType;
				this.soundPageIntro.Type = this.defaultSoundType;
				this.soundPageOutro.Type = this.defaultSoundType;
				this.soundWindowPopUp.Type = this.defaultSoundType;
				this.soundWindowPopUpClose.Type = this.defaultSoundType;
				this.soundMove.Type = this.defaultSoundType;
			}
			this.cutsceneSoundOverrideTimer -= Time.deltaTime;
		}
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x0009D294 File Offset: 0x0009B494
	private void FixedUpdate()
	{
		if (this.menuHover > 0f)
		{
			this.menuHover -= Time.fixedDeltaTime;
			return;
		}
		this.currentMenuID = "";
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x0009D2C1 File Offset: 0x0009B4C1
	public void SetState(int state)
	{
		this.currentMenuState = state;
		this.stateStart = true;
	}

	// Token: 0x0600112E RID: 4398 RVA: 0x0009D2D4 File Offset: 0x0009B4D4
	public void MenuEffectHover(float pitch = -1f, float volume = -1f)
	{
		if (pitch != -1f)
		{
			this.soundHover.Pitch = pitch;
		}
		if (volume != -1f)
		{
			this.soundHover.Volume = volume;
		}
		this.soundHover.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x0009D334 File Offset: 0x0009B534
	public void MenuEffectClick(MenuManager.MenuClickEffectType effectType, MenuPage parentPage = null, float pitch = -1f, float volume = -1f, bool soundOnly = false)
	{
		switch (effectType)
		{
		case MenuManager.MenuClickEffectType.Action:
			if (!soundOnly && this.activeSelectionBox)
			{
				this.activeSelectionBox.SetClick(AssetManager.instance.colorYellow);
			}
			if (pitch != -1f)
			{
				this.soundAction.Pitch = pitch;
			}
			if (volume != -1f)
			{
				this.soundAction.Volume = volume;
			}
			this.soundAction.Play(this.soundPosition, 1f, 1f, 1f, 1f);
			return;
		case MenuManager.MenuClickEffectType.Confirm:
			if (!soundOnly && this.activeSelectionBox)
			{
				this.activeSelectionBox.SetClick(Color.green);
			}
			if (pitch != -1f)
			{
				this.soundConfirm.Pitch = pitch;
			}
			if (volume != -1f)
			{
				this.soundConfirm.Volume = volume;
			}
			this.soundConfirm.Play(this.soundPosition, 1f, 1f, 1f, 1f);
			return;
		case MenuManager.MenuClickEffectType.Deny:
			if (!soundOnly && this.activeSelectionBox)
			{
				this.activeSelectionBox.SetClick(Color.red);
			}
			if (pitch != -1f)
			{
				this.soundDeny.Pitch = pitch;
			}
			if (volume != -1f)
			{
				this.soundDeny.Volume = volume;
			}
			this.soundDeny.Play(this.soundPosition, 1f, 1f, 1f, 1f);
			return;
		case MenuManager.MenuClickEffectType.Dud:
			if (!soundOnly)
			{
				this.activeSelectionBox.SetClick(Color.gray);
			}
			if (pitch != -1f)
			{
				this.soundDud.Pitch = pitch;
			}
			if (volume != -1f)
			{
				this.soundDud.Volume = volume;
			}
			this.soundDud.Play(this.soundPosition, 1f, 1f, 1f, 1f);
			return;
		case MenuManager.MenuClickEffectType.Tick:
			if (!soundOnly)
			{
				Color click = new Color(0f, 0.5f, 1f, 1f);
				if (!parentPage)
				{
					if (MenuSelectionBox.instance)
					{
						MenuSelectionBox.instance.SetClick(click);
					}
				}
				else if (parentPage.selectionBox)
				{
					parentPage.selectionBox.SetClick(click);
				}
			}
			if (pitch != -1f)
			{
				this.soundTick.Pitch = pitch;
			}
			if (volume != -1f)
			{
				this.soundTick.Volume = volume;
			}
			this.soundTick.Play(this.soundPosition, 1f, 1f, 1f, 1f);
			return;
		default:
			return;
		}
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x0009D5C5 File Offset: 0x0009B7C5
	public void MenuEffectPopUpOpen()
	{
		this.soundWindowPopUp.Play(this.soundPosition, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001131 RID: 4401 RVA: 0x0009D5ED File Offset: 0x0009B7ED
	public void MenuEffectPopUpClose()
	{
		this.soundWindowPopUpClose.Play(this.soundPosition, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x0009D615 File Offset: 0x0009B815
	public void MenuEffectPageIntro()
	{
		this.soundPageIntro.Play(this.soundPosition, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x0009D63D File Offset: 0x0009B83D
	public void MenuEffectPageOutro()
	{
		this.soundPageOutro.Play(this.soundPosition, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x0009D665 File Offset: 0x0009B865
	public void MenuEffectMove()
	{
		this.soundMove.Play(this.soundPosition, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x0009D68D File Offset: 0x0009B88D
	private void StateOpen()
	{
		bool flag = this.stateStart;
		SemiFunc.CursorUnlock(0.1f);
		PlayerController.instance.InputDisableTimer = 0.1f;
		if (!this.currentMenuPage)
		{
			this.StateSet(MenuManager.MenuState.Closed);
		}
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x0009D6C3 File Offset: 0x0009B8C3
	private void StateClosed()
	{
		bool flag = this.stateStart;
		if (this.currentMenuPage)
		{
			this.StateSet(MenuManager.MenuState.Open);
		}
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x0009D6E0 File Offset: 0x0009B8E0
	public void PageAdd(MenuPage menuPage)
	{
		if (this.allPages.Contains(menuPage))
		{
			return;
		}
		this.allPages.Add(menuPage);
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x0009D6FD File Offset: 0x0009B8FD
	public void PageRemove(MenuPage menuPage)
	{
		if (!this.allPages.Contains(menuPage))
		{
			return;
		}
		this.allPages.Remove(menuPage);
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x0009D71C File Offset: 0x0009B91C
	public MenuPage PageOpen(MenuPageIndex menuPageIndex, bool addedPageOnTop = false)
	{
		MenuManager.MenuPages menuPages = this.menuPages.Find((MenuManager.MenuPages x) => x.menuPageIndex == menuPageIndex);
		if (menuPages == null)
		{
			Debug.LogError("Page not found");
			return null;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(menuPages.menuPage, MenuHolder.instance.transform, true);
		gameObject.GetComponent<RectTransform>().localPosition = new Vector2(0f, 0f);
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		MenuPage component = gameObject.GetComponent<MenuPage>();
		component.addedPageOnTop = addedPageOnTop;
		if (!addedPageOnTop)
		{
			this.PageSetCurrent(menuPageIndex, component);
		}
		else
		{
			component.parentPage = this.currentMenuPage;
		}
		return component;
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x0009D7E0 File Offset: 0x0009B9E0
	public void PageClose(MenuPageIndex menuPageIndex)
	{
		if (this.menuPages.Find((MenuManager.MenuPages x) => x.menuPageIndex == menuPageIndex) == null)
		{
			return;
		}
		MenuPage menuPage = this.allPages.Find((MenuPage x) => x.menuPageIndex == menuPageIndex);
		if (menuPage == null)
		{
			return;
		}
		menuPage.PageStateSet(MenuPage.PageState.Closing);
		this.allPages.Remove(menuPage);
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x0009D84C File Offset: 0x0009BA4C
	public bool PageCheck(MenuPageIndex menuPageIndex)
	{
		return this.allPages.Find((MenuPage x) => x.menuPageIndex == menuPageIndex) != null;
	}

	// Token: 0x0600113C RID: 4412 RVA: 0x0009D883 File Offset: 0x0009BA83
	public void PageSwap(MenuPageIndex menuPageIndex)
	{
		this.currentMenuPage.PageStateSet(MenuPage.PageState.Closing);
		this.PageOpen(menuPageIndex, false);
	}

	// Token: 0x0600113D RID: 4413 RVA: 0x0009D89C File Offset: 0x0009BA9C
	public MenuPage PageOpenOnTop(MenuPageIndex menuPageIndex)
	{
		MenuPage menuPage = this.currentMenuPage;
		this.PageInactiveAdd(menuPage);
		this.currentMenuPage.PageStateSet(MenuPage.PageState.Inactive);
		MenuPage menuPage2 = this.PageOpen(menuPageIndex, false);
		menuPage2.pageIsOnTopOfOtherPage = true;
		menuPage2.pageUnderThisPage = menuPage;
		return menuPage2;
	}

	// Token: 0x0600113E RID: 4414 RVA: 0x0009D8DC File Offset: 0x0009BADC
	public MenuPage PageAddOnTop(MenuPageIndex menuPageIndex)
	{
		if (this.addedPagesOnTop.Contains(this.currentMenuPage))
		{
			return null;
		}
		MenuPage menuPage = this.PageOpen(menuPageIndex, true);
		if (!this.addedPagesOnTop.Contains(this.currentMenuPage))
		{
			this.addedPagesOnTop.Add(menuPage);
		}
		return menuPage;
	}

	// Token: 0x0600113F RID: 4415 RVA: 0x0009D928 File Offset: 0x0009BB28
	public void PagePopUpTwoOptions(MenuButtonPopUp menuButtonPopUp, string popUpHeader, Color popUpHeaderColor, string popUpText, string option1Text, string option2Text, bool richText)
	{
		MenuPageIndex menuPageIndex = MenuPageIndex.PopUpTwoOptions;
		MenuPage pageUnderThisPage = this.currentMenuPage;
		this.currentMenuPage.PageStateSet(MenuPage.PageState.Inactive);
		MenuPage menuPage = this.PageOpen(menuPageIndex, false);
		menuPage.pageIsOnTopOfOtherPage = true;
		menuPage.pageUnderThisPage = pageUnderThisPage;
		MenuPageTwoOptions component = menuPage.GetComponent<MenuPageTwoOptions>();
		menuPage.menuHeaderName = popUpHeader;
		menuPage.menuHeader.text = popUpHeader;
		menuPage.menuHeader.color = popUpHeaderColor;
		component.option1Event = menuButtonPopUp.option1Event;
		component.option2Event = menuButtonPopUp.option2Event;
		component.bodyTextMesh.text = popUpText;
		component.bodyTextMesh.text = popUpText;
		component.option1Button.buttonTextString = option1Text;
		component.option2Button.buttonTextString = option2Text;
		component.richText = richText;
	}

	// Token: 0x06001140 RID: 4416 RVA: 0x0009D9D9 File Offset: 0x0009BBD9
	public void MenuHover()
	{
		this.menuHover = 0.1f;
	}

	// Token: 0x06001141 RID: 4417 RVA: 0x0009D9E6 File Offset: 0x0009BBE6
	public void PageSetCurrent(MenuPageIndex menuPageIndex, MenuPage menuPage)
	{
		this.currentMenuPageIndex = menuPageIndex;
		this.currentMenuPage = menuPage;
	}

	// Token: 0x06001142 RID: 4418 RVA: 0x0009D9F6 File Offset: 0x0009BBF6
	public void StateSet(MenuManager.MenuState state)
	{
		this.currentMenuState = (int)state;
	}

	// Token: 0x06001143 RID: 4419 RVA: 0x0009D9FF File Offset: 0x0009BBFF
	private void PageInactiveAdd(MenuPage menuPage)
	{
		if (this.inactivePages.Contains(menuPage))
		{
			return;
		}
		this.inactivePages.Add(menuPage);
	}

	// Token: 0x06001144 RID: 4420 RVA: 0x0009DA1C File Offset: 0x0009BC1C
	private void PageInactiveRemove(MenuPage menuPage)
	{
		if (!this.inactivePages.Contains(menuPage))
		{
			return;
		}
		this.inactivePages.Remove(menuPage);
	}

	// Token: 0x06001145 RID: 4421 RVA: 0x0009DA3C File Offset: 0x0009BC3C
	public void PageReactivatePageUnderThisPage(MenuPage _menuPage)
	{
		if (this.currentMenuPage != _menuPage)
		{
			return;
		}
		if (this.currentMenuPage.pageUnderThisPage)
		{
			if (this.currentMenuPage.pageUnderThisPage.currentPageState == MenuPage.PageState.Inactive)
			{
				this.currentMenuPage.pageUnderThisPage.PageStateSet(MenuPage.PageState.Activating);
			}
			this.PageSetCurrent(this.currentMenuPage.pageUnderThisPage.menuPageIndex, this.currentMenuPage.pageUnderThisPage);
		}
	}

	// Token: 0x06001146 RID: 4422 RVA: 0x0009DAB0 File Offset: 0x0009BCB0
	public void PageCloseAllExcept(MenuPageIndex menuPageIndex)
	{
		foreach (MenuPage menuPage in this.allPages)
		{
			if (menuPage.menuPageIndex != menuPageIndex)
			{
				menuPage.PageStateSet(MenuPage.PageState.Closing);
			}
		}
	}

	// Token: 0x06001147 RID: 4423 RVA: 0x0009DB0C File Offset: 0x0009BD0C
	public void PageCloseAll()
	{
		foreach (MenuPage menuPage in this.allPages)
		{
			menuPage.PageStateSet(MenuPage.PageState.Closing);
		}
	}

	// Token: 0x06001148 RID: 4424 RVA: 0x0009DB60 File Offset: 0x0009BD60
	public void PageCloseAllAddedOnTop()
	{
		foreach (MenuPage menuPage in this.addedPagesOnTop)
		{
			menuPage.PageStateSet(MenuPage.PageState.Closing);
		}
	}

	// Token: 0x06001149 RID: 4425 RVA: 0x0009DBB4 File Offset: 0x0009BDB4
	public void PlayerHeadAdd(MenuPlayerHead head)
	{
		if (!this.playerHeads.Contains(head))
		{
			this.playerHeads.Add(head);
		}
		for (int i = 0; i < this.playerHeads.Count; i++)
		{
			if (this.playerHeads[i] == null)
			{
				this.playerHeads.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600114A RID: 4426 RVA: 0x0009DC11 File Offset: 0x0009BE11
	public void SetActiveSelectionBox(MenuSelectionBox selectBox)
	{
		this.activeSelectionBox = selectBox;
	}

	// Token: 0x0600114B RID: 4427 RVA: 0x0009DC1C File Offset: 0x0009BE1C
	public void PlayerHeadRemove(MenuPlayerHead head)
	{
		if (this.playerHeads.Contains(head))
		{
			this.playerHeads.Remove(head);
		}
		for (int i = 0; i < this.playerHeads.Count; i++)
		{
			if (this.playerHeads[i] == null)
			{
				this.playerHeads.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600114C RID: 4428 RVA: 0x0009DC7A File Offset: 0x0009BE7A
	public void PagePopUpScheduled(string headerText, Color headerColor, string bodyText, string buttonText, bool richText)
	{
		this.pagePopUpScheduled = true;
		this.pagePopUpScheduledHeaderText = headerText;
		this.pagePopUpScheduledHeaderColor = headerColor;
		this.pagePopUpScheduledBodyText = bodyText;
		this.pagePopUpScheduledButtonText = buttonText;
		this.pagePopUpScheduledRichText = richText;
	}

	// Token: 0x0600114D RID: 4429 RVA: 0x0009DCA8 File Offset: 0x0009BEA8
	public void PagePopUpScheduledShow()
	{
		if (!this.pagePopUpScheduled)
		{
			return;
		}
		this.PagePopUp(this.pagePopUpScheduledHeaderText, this.pagePopUpScheduledHeaderColor, this.pagePopUpScheduledBodyText, this.pagePopUpScheduledButtonText, this.pagePopUpScheduledRichText);
		this.PagePopUpScheduledReset();
	}

	// Token: 0x0600114E RID: 4430 RVA: 0x0009DCDD File Offset: 0x0009BEDD
	public void PagePopUpScheduledReset()
	{
		this.pagePopUpScheduled = false;
	}

	// Token: 0x0600114F RID: 4431 RVA: 0x0009DCE8 File Offset: 0x0009BEE8
	public void SelectionBoxAdd(MenuSelectionBox selectBox)
	{
		if (!this.selectionBoxes.Contains(selectBox))
		{
			this.selectionBoxes.Add(selectBox);
		}
		for (int i = 0; i < this.selectionBoxes.Count; i++)
		{
			if (this.selectionBoxes[i] == null)
			{
				this.selectionBoxes.RemoveAt(i);
			}
		}
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x0009DD48 File Offset: 0x0009BF48
	public void SelectionBoxRemove(MenuSelectionBox selectBox)
	{
		if (this.selectionBoxes.Contains(selectBox))
		{
			this.selectionBoxes.Remove(selectBox);
		}
		for (int i = 0; i < this.selectionBoxes.Count; i++)
		{
			if (this.selectionBoxes[i] == null)
			{
				this.selectionBoxes.RemoveAt(i);
			}
		}
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x0009DDA8 File Offset: 0x0009BFA8
	public MenuSelectionBox SelectionBoxGetCorrect(MenuPage parentPage, MenuScrollBox menuScrollBox)
	{
		return this.selectionBoxes.Find((MenuSelectionBox x) => x.menuPage == parentPage && x.menuScrollBox == menuScrollBox);
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x0009DDE0 File Offset: 0x0009BFE0
	public void PagePopUp(string headerText, Color headerColor, string bodyText, string buttonText, bool richText)
	{
		MenuPageIndex menuPageIndex = MenuPageIndex.PopUp;
		MenuPage pageUnderThisPage = this.currentMenuPage;
		if (this.currentMenuPage.menuPageIndex == MenuPageIndex.PopUpTwoOptions)
		{
			pageUnderThisPage = this.currentMenuPage.pageUnderThisPage;
			this.currentMenuPage = pageUnderThisPage;
		}
		pageUnderThisPage.PageStateSet(MenuPage.PageState.Inactive);
		MenuPage menuPage = this.PageOpen(menuPageIndex, false);
		menuPage.pageIsOnTopOfOtherPage = true;
		menuPage.pageUnderThisPage = pageUnderThisPage;
		MenuPagePopUp component = menuPage.GetComponent<MenuPagePopUp>();
		menuPage.menuHeaderName = headerText;
		menuPage.menuHeader.text = headerText;
		menuPage.menuHeader.color = headerColor;
		component.bodyTextMesh.text = bodyText;
		component.okButton.buttonTextString = buttonText;
		component.richText = richText;
		this.currentMenuPage = menuPage;
		this.MenuEffectPopUpOpen();
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x0009DE88 File Offset: 0x0009C088
	public void TextInputActive()
	{
		this.textInputActive = true;
		this.textInputActiveTimer = 0.1f;
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x0009DE9C File Offset: 0x0009C09C
	public void CutsceneSoundOverride()
	{
		if (!this.cutsceneSoundOverride)
		{
			this.soundAction.Type = this.cutsceneSoundType;
			this.soundConfirm.Type = this.cutsceneSoundType;
			this.soundDeny.Type = this.cutsceneSoundType;
			this.soundDud.Type = this.cutsceneSoundType;
			this.soundTick.Type = this.cutsceneSoundType;
			this.soundHover.Type = this.cutsceneSoundType;
			this.soundPageIntro.Type = this.cutsceneSoundType;
			this.soundPageOutro.Type = this.cutsceneSoundType;
			this.soundWindowPopUp.Type = this.cutsceneSoundType;
			this.soundWindowPopUpClose.Type = this.cutsceneSoundType;
			this.soundMove.Type = this.cutsceneSoundType;
			this.cutsceneSoundOverride = true;
		}
		this.cutsceneSoundOverrideTimer = 0.1f;
	}

	// Token: 0x04001D20 RID: 7456
	public static MenuManager instance;

	// Token: 0x04001D21 RID: 7457
	internal MenuSelectionBox selectionBox;

	// Token: 0x04001D22 RID: 7458
	internal MenuSelectionBox activeSelectionBox;

	// Token: 0x04001D23 RID: 7459
	internal List<MenuPlayerHead> playerHeads = new List<MenuPlayerHead>();

	// Token: 0x04001D24 RID: 7460
	private List<MenuSelectionBox> selectionBoxes = new List<MenuSelectionBox>();

	// Token: 0x04001D25 RID: 7461
	internal bool textInputActive;

	// Token: 0x04001D26 RID: 7462
	private float textInputActiveTimer;

	// Token: 0x04001D27 RID: 7463
	internal string currentMenuID = "";

	// Token: 0x04001D28 RID: 7464
	public List<MenuManager.MenuPages> menuPages;

	// Token: 0x04001D29 RID: 7465
	internal MenuPageIndex currentMenuPageIndex;

	// Token: 0x04001D2A RID: 7466
	internal MenuPage currentMenuPage;

	// Token: 0x04001D2B RID: 7467
	internal int currentMenuState;

	// Token: 0x04001D2C RID: 7468
	private bool stateStart;

	// Token: 0x04001D2D RID: 7469
	internal MenuButton currentButton;

	// Token: 0x04001D2E RID: 7470
	internal int fetchSetting;

	// Token: 0x04001D2F RID: 7471
	private Vector3 soundPosition;

	// Token: 0x04001D30 RID: 7472
	private float menuHover;

	// Token: 0x04001D31 RID: 7473
	[Space]
	public AudioManager.AudioType defaultSoundType;

	// Token: 0x04001D32 RID: 7474
	public AudioManager.AudioType cutsceneSoundType;

	// Token: 0x04001D33 RID: 7475
	private bool cutsceneSoundOverride;

	// Token: 0x04001D34 RID: 7476
	private float cutsceneSoundOverrideTimer;

	// Token: 0x04001D35 RID: 7477
	[Space]
	public Sound soundAction;

	// Token: 0x04001D36 RID: 7478
	public Sound soundConfirm;

	// Token: 0x04001D37 RID: 7479
	public Sound soundDeny;

	// Token: 0x04001D38 RID: 7480
	public Sound soundDud;

	// Token: 0x04001D39 RID: 7481
	public Sound soundTick;

	// Token: 0x04001D3A RID: 7482
	public Sound soundHover;

	// Token: 0x04001D3B RID: 7483
	public Sound soundPageIntro;

	// Token: 0x04001D3C RID: 7484
	public Sound soundPageOutro;

	// Token: 0x04001D3D RID: 7485
	public Sound soundWindowPopUp;

	// Token: 0x04001D3E RID: 7486
	public Sound soundWindowPopUpClose;

	// Token: 0x04001D3F RID: 7487
	public Sound soundMove;

	// Token: 0x04001D40 RID: 7488
	internal Vector2 mouseHoldPosition;

	// Token: 0x04001D41 RID: 7489
	internal int screenUIWidth = 720;

	// Token: 0x04001D42 RID: 7490
	internal int screenUIHeight = 405;

	// Token: 0x04001D43 RID: 7491
	internal List<MenuPage> allPages = new List<MenuPage>();

	// Token: 0x04001D44 RID: 7492
	internal List<MenuPage> inactivePages = new List<MenuPage>();

	// Token: 0x04001D45 RID: 7493
	internal List<MenuPage> addedPagesOnTop = new List<MenuPage>();

	// Token: 0x04001D46 RID: 7494
	private bool pagePopUpScheduled;

	// Token: 0x04001D47 RID: 7495
	private string pagePopUpScheduledHeaderText;

	// Token: 0x04001D48 RID: 7496
	private Color pagePopUpScheduledHeaderColor;

	// Token: 0x04001D49 RID: 7497
	private string pagePopUpScheduledBodyText;

	// Token: 0x04001D4A RID: 7498
	private string pagePopUpScheduledButtonText;

	// Token: 0x04001D4B RID: 7499
	private bool pagePopUpScheduledRichText;

	// Token: 0x020003DA RID: 986
	[Serializable]
	public class MenuPages
	{
		// Token: 0x04002C99 RID: 11417
		public MenuPageIndex menuPageIndex;

		// Token: 0x04002C9A RID: 11418
		public GameObject menuPage;
	}

	// Token: 0x020003DB RID: 987
	public enum MenuState
	{
		// Token: 0x04002C9C RID: 11420
		Open,
		// Token: 0x04002C9D RID: 11421
		Closed
	}

	// Token: 0x020003DC RID: 988
	public enum MenuClickEffectType
	{
		// Token: 0x04002C9F RID: 11423
		Action,
		// Token: 0x04002CA0 RID: 11424
		Confirm,
		// Token: 0x04002CA1 RID: 11425
		Deny,
		// Token: 0x04002CA2 RID: 11426
		Dud,
		// Token: 0x04002CA3 RID: 11427
		Tick
	}
}
