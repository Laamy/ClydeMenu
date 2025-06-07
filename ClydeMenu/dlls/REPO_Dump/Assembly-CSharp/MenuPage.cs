using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001FF RID: 511
public class MenuPage : MonoBehaviour
{
	// Token: 0x06001156 RID: 4438 RVA: 0x0009DFF0 File Offset: 0x0009C1F0
	private void Start()
	{
		this.selectionBox = base.GetComponentInChildren<MenuSelectionBox>();
		this.rectTransform = base.GetComponent<RectTransform>();
		this.originalPosition = this.rectTransform.localPosition;
		this.animateAwayPosition = new Vector2(this.originalPosition.x, this.originalPosition.y - this.rectTransform.rect.height);
		this.rectTransform.localPosition = new Vector2(this.originalPosition.x, this.originalPosition.y + this.rectTransform.rect.height);
		MenuManager.instance.PageAdd(this);
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x0009E0B7 File Offset: 0x0009C2B7
	private IEnumerator LateStart()
	{
		yield return null;
		if (!this.parentPage)
		{
			this.parentPage = this;
		}
		yield break;
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x0009E0C6 File Offset: 0x0009C2C6
	private void FixedUpdate()
	{
		if (this.pageActiveTimer <= 0f)
		{
			this.pageActive = false;
		}
		if (this.pageActiveTimer > 0f)
		{
			this.pageActive = true;
			this.pageActiveTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x0009E104 File Offset: 0x0009C304
	private void Update()
	{
		switch (this.currentPageState)
		{
		case MenuPage.PageState.Opening:
			this.StateOpening();
			this.stateStart = false;
			break;
		case MenuPage.PageState.Active:
			this.StateActive();
			this.stateStart = false;
			break;
		case MenuPage.PageState.Closing:
			this.StateClosing();
			this.stateStart = false;
			break;
		case MenuPage.PageState.Inactive:
			this.StateInactive();
			this.stateStart = false;
			break;
		case MenuPage.PageState.Activating:
			this.StateActivating();
			this.stateStart = false;
			break;
		}
		if (this.activeSettingElementTimer > 0f)
		{
			this.activeSettingElementTimer -= Time.deltaTime;
			if (this.activeSettingElementTimer <= 0f)
			{
				this.currentActiveSettingElement = -1;
			}
		}
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x0009E1B0 File Offset: 0x0009C3B0
	private void OnEnable()
	{
		this.ResetPage();
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x0009E1B8 File Offset: 0x0009C3B8
	public void ResetPage()
	{
		if (!this.rectTransform)
		{
			return;
		}
		this.rectTransform.localPosition = new Vector2(this.originalPosition.x, this.originalPosition.y + this.rectTransform.rect.height);
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x0009E212 File Offset: 0x0009C412
	public void PageStateSet(MenuPage.PageState pageState)
	{
		this.stateTimer = 0f;
		this.currentPageState = pageState;
		this.stateStart = true;
	}

	// Token: 0x0600115D RID: 4445 RVA: 0x0009E230 File Offset: 0x0009C430
	private void StateOpening()
	{
		if (this.stateStart)
		{
			if (!this.popUpAnimation)
			{
				MenuManager.instance.MenuEffectPageIntro();
			}
			else
			{
				MenuManager.instance.MenuEffectPopUpOpen();
			}
			this.LockAndHide();
		}
		if (!this.addedPageOnTop)
		{
			MenuSelectionBox.instance.firstSelection = true;
		}
		if (Vector2.Distance(this.rectTransform.localPosition, this.originalPosition) < 0.8f)
		{
			this.PageStateSet(MenuPage.PageState.Active);
		}
		if (!this.disableIntroAnimation)
		{
			float deltaTime = Time.deltaTime;
			this.rectTransform.localPosition = Vector2.Lerp(this.rectTransform.localPosition, this.originalPosition, 40f * deltaTime);
		}
		this.LockAndHide();
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x0009E2EC File Offset: 0x0009C4EC
	private void StateActive()
	{
		if (this.stateStart && !this.disableIntroAnimation)
		{
			this.rectTransform.localPosition = this.originalPosition;
		}
		if (!this.disableIntroAnimation)
		{
			this.rectTransform.localPosition = this.originalPosition;
		}
		this.PageAddedOnTopLogic();
		MenuSelectionBox instance = MenuSelectionBox.instance;
		if (!instance || instance != this.selectionBox)
		{
			this.selectionBox.Reinstate();
		}
		this.LockAndHide();
		if (MenuManager.instance.currentMenuPageIndex != this.menuPageIndex)
		{
			this.PageStateSet(MenuPage.PageState.Inactive);
		}
		this.pageActive = true;
		this.pageActiveTimer = 0.1f;
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x0009E39B File Offset: 0x0009C59B
	public bool SettingElementActiveCheckFree(int index)
	{
		return this.currentActiveSettingElement == -1 || this.currentActiveSettingElement == index;
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x0009E3B4 File Offset: 0x0009C5B4
	private void StateClosing()
	{
		this.LockAndHide();
		if (this.stateStart)
		{
			if (!this.popUpAnimation)
			{
				MenuManager.instance.MenuEffectPageOutro();
			}
			else
			{
				MenuManager.instance.MenuEffectPopUpClose();
			}
			if (MenuManager.instance.currentMenuPage == this)
			{
				MenuManager.instance.currentMenuPage = null;
				MenuManager.instance.PageRemove(this);
			}
		}
		if (Vector2.Distance(this.rectTransform.localPosition, this.animateAwayPosition) < 0.8f)
		{
			this.onPageEnd.Invoke();
			MenuManager.instance.PageRemove(this);
			Object.Destroy(base.gameObject);
		}
		float deltaTime = Time.deltaTime;
		this.rectTransform.localPosition = Vector2.Lerp(this.rectTransform.localPosition, this.animateAwayPosition, 40f * deltaTime);
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x0009E490 File Offset: 0x0009C690
	private void StateInactive()
	{
		bool flag = this.stateStart;
		if (MenuManager.instance.currentMenuPageIndex == this.menuPageIndex)
		{
			this.PageStateSet(MenuPage.PageState.Active);
		}
	}

	// Token: 0x06001162 RID: 4450 RVA: 0x0009E4B4 File Offset: 0x0009C6B4
	private void PageAddedOnTopLogic()
	{
		if (this.currentPageState == MenuPage.PageState.Opening || this.currentPageState == MenuPage.PageState.Closing)
		{
			return;
		}
		if (this.addedPageOnTop)
		{
			if (this.parentPage)
			{
				if (this.currentPageState != this.parentPage.currentPageState)
				{
					this.PageStateSet(this.parentPage.currentPageState);
					return;
				}
			}
			else if (this.currentPageState != MenuPage.PageState.Closing)
			{
				this.PageStateSet(MenuPage.PageState.Closing);
			}
		}
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x0009E51D File Offset: 0x0009C71D
	private void StateActivating()
	{
		bool flag = this.stateStart;
		if (this.stateTimer > 0.1f)
		{
			this.PageStateSet(MenuPage.PageState.Active);
		}
		this.stateTimer += Time.deltaTime;
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x0009E54C File Offset: 0x0009C74C
	public void SettingElementActiveSet(int index)
	{
		if (this.currentActiveSettingElement == -1)
		{
			this.currentActiveSettingElement = index;
		}
		this.activeSettingElementTimer = 0.1f;
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x0009E56C File Offset: 0x0009C76C
	private void LockAndHide()
	{
		SemiFunc.UIHideAim();
		SemiFunc.UIHideEnergy();
		SemiFunc.UIHideGoal();
		SemiFunc.UIHideHealth();
		SemiFunc.UIHideOvercharge();
		SemiFunc.UIHideInventory();
		SemiFunc.UIHideHaul();
		SemiFunc.UIHideCurrency();
		SemiFunc.UIHideShopCost();
		SemiFunc.UIHideTumble();
		SemiFunc.UIHideWorldSpace();
		SemiFunc.UIHideValuableDiscover();
		SemiFunc.CameraOverrideStopAim();
	}

	// Token: 0x04001D4C RID: 7500
	public string menuHeaderName;

	// Token: 0x04001D4D RID: 7501
	public MenuPageIndex menuPageIndex;

	// Token: 0x04001D4E RID: 7502
	private Vector2 originalPosition;

	// Token: 0x04001D4F RID: 7503
	internal RectTransform rectTransform;

	// Token: 0x04001D50 RID: 7504
	private Vector2 animateAwayPosition = new Vector2(0f, 0f);

	// Token: 0x04001D51 RID: 7505
	private Vector2 targetPosition;

	// Token: 0x04001D52 RID: 7506
	internal float bottomElementYPos;

	// Token: 0x04001D53 RID: 7507
	internal List<MenuSelectableElement> selectableElements = new List<MenuSelectableElement>();

	// Token: 0x04001D54 RID: 7508
	public TextMeshProUGUI menuHeader;

	// Token: 0x04001D55 RID: 7509
	internal bool pageIsOnTopOfOtherPage;

	// Token: 0x04001D56 RID: 7510
	internal MenuPage pageUnderThisPage;

	// Token: 0x04001D57 RID: 7511
	internal bool pageActive;

	// Token: 0x04001D58 RID: 7512
	private float pageActiveTimer;

	// Token: 0x04001D59 RID: 7513
	internal bool popUpAnimation;

	// Token: 0x04001D5A RID: 7514
	internal MenuSelectionBox selectionBox;

	// Token: 0x04001D5B RID: 7515
	private float stateTimer;

	// Token: 0x04001D5C RID: 7516
	internal bool addedPageOnTop;

	// Token: 0x04001D5D RID: 7517
	internal MenuPage parentPage;

	// Token: 0x04001D5E RID: 7518
	public bool disableIntroAnimation;

	// Token: 0x04001D5F RID: 7519
	public bool disableOutroAnimation;

	// Token: 0x04001D60 RID: 7520
	internal List<MenuSettingElement> settingElements = new List<MenuSettingElement>();

	// Token: 0x04001D61 RID: 7521
	internal int currentActiveSettingElement = -1;

	// Token: 0x04001D62 RID: 7522
	private float activeSettingElementTimer;

	// Token: 0x04001D63 RID: 7523
	public UnityEvent onPageEnd;

	// Token: 0x04001D64 RID: 7524
	internal int scrollBoxes;

	// Token: 0x04001D65 RID: 7525
	private bool stateStart = true;

	// Token: 0x04001D66 RID: 7526
	internal MenuPage.PageState currentPageState;

	// Token: 0x020003E1 RID: 993
	public enum PageState
	{
		// Token: 0x04002CAA RID: 11434
		Opening,
		// Token: 0x04002CAB RID: 11435
		Active,
		// Token: 0x04002CAC RID: 11436
		Closing,
		// Token: 0x04002CAD RID: 11437
		Inactive,
		// Token: 0x04002CAE RID: 11438
		Activating,
		// Token: 0x04002CAF RID: 11439
		Closed
	}
}
