using System;
using UnityEngine;

// Token: 0x0200022D RID: 557
public class MenuPageServerListCreateNew : MonoBehaviour
{
	// Token: 0x06001273 RID: 4723 RVA: 0x000A632D File Offset: 0x000A452D
	private void Update()
	{
		if (SemiFunc.InputDown(InputKey.Confirm))
		{
			this.ButtonConfirm();
		}
		if (SemiFunc.InputDown(InputKey.Back))
		{
			this.ExitPage();
		}
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x000A634D File Offset: 0x000A454D
	public void ExitPage()
	{
		MenuManager.instance.PageCloseAllExcept(MenuPageIndex.ServerList);
		MenuManager.instance.PageSetCurrent(MenuPageIndex.ServerList, this.menuPageParent);
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x000A6370 File Offset: 0x000A4570
	public void ButtonConfirm()
	{
		if (string.IsNullOrEmpty(this.menuTextInput.textCurrent))
		{
			this.confirmButton.OnHovering();
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, 1f, 1f, false);
			return;
		}
		DataDirector.instance.networkServerName = this.menuTextInput.textCurrent;
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, 1f, 1f, false);
		RunManager.instance.ResetProgress();
		StatsManager.instance.saveFileCurrent = "";
		GameManager.instance.SetConnectRandom(true);
		GameManager.instance.localTest = false;
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.LobbyMenu);
		RunManager.instance.lobbyJoin = true;
	}

	// Token: 0x04001F03 RID: 7939
	public MenuButton confirmButton;

	// Token: 0x04001F04 RID: 7940
	public MenuTextInput menuTextInput;

	// Token: 0x04001F05 RID: 7941
	internal MenuPage menuPageParent;
}
