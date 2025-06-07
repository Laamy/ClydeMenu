using System;
using UnityEngine;

// Token: 0x02000225 RID: 549
public class MenuPageSettings : MonoBehaviour
{
	// Token: 0x06001239 RID: 4665 RVA: 0x000A4C7F File Offset: 0x000A2E7F
	private void Start()
	{
		MenuPageSettings.instance = this;
		this.menuPage = base.GetComponent<MenuPage>();
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x000A4C93 File Offset: 0x000A2E93
	private void Update()
	{
		if (SemiFunc.InputDown(InputKey.Back))
		{
			this.ButtonEventBack();
		}
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x000A4CA4 File Offset: 0x000A2EA4
	public void ButtonEventGameplay()
	{
		MenuManager.instance.PageCloseAllAddedOnTop();
		MenuManager.instance.PageAddOnTop(MenuPageIndex.SettingsGameplay);
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x000A4CBC File Offset: 0x000A2EBC
	public void ButtonEventAudio()
	{
		MenuManager.instance.PageCloseAllAddedOnTop();
		MenuManager.instance.PageAddOnTop(MenuPageIndex.SettingsAudio);
	}

	// Token: 0x0600123D RID: 4669 RVA: 0x000A4CD4 File Offset: 0x000A2ED4
	public void ButtonEventBack()
	{
		if (RunManager.instance.levelCurrent == RunManager.instance.levelMainMenu)
		{
			MenuManager.instance.PageCloseAllExcept(MenuPageIndex.Main);
			MenuManager.instance.PageSetCurrent(MenuPageIndex.Main, MenuPageMain.instance.menuPage);
			return;
		}
		if (RunManager.instance.levelCurrent == RunManager.instance.levelLobbyMenu)
		{
			MenuManager.instance.PageCloseAllExcept(MenuPageIndex.Lobby);
			MenuManager.instance.PageSetCurrent(MenuPageIndex.Lobby, MenuPageLobby.instance.menuPage);
			return;
		}
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.Escape, false);
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x000A4D70 File Offset: 0x000A2F70
	public void ButtonEventControls()
	{
		MenuManager.instance.PageCloseAllAddedOnTop();
		MenuManager.instance.PageAddOnTop(MenuPageIndex.SettingsControls);
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x000A4D88 File Offset: 0x000A2F88
	public void ButtonEventGraphics()
	{
		MenuManager.instance.PageCloseAllAddedOnTop();
		MenuManager.instance.PageAddOnTop(MenuPageIndex.SettingsGraphics);
	}

	// Token: 0x04001EB1 RID: 7857
	public static MenuPageSettings instance;

	// Token: 0x04001EB2 RID: 7858
	internal MenuPage menuPage;
}
