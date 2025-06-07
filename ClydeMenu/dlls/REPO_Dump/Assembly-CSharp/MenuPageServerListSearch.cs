using System;
using UnityEngine;

// Token: 0x0200022E RID: 558
public class MenuPageServerListSearch : MonoBehaviour
{
	// Token: 0x06001277 RID: 4727 RVA: 0x000A642D File Offset: 0x000A462D
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

	// Token: 0x06001278 RID: 4728 RVA: 0x000A644D File Offset: 0x000A464D
	public void ExitPage()
	{
		MenuManager.instance.PageCloseAllExcept(MenuPageIndex.ServerList);
		MenuManager.instance.PageSetCurrent(MenuPageIndex.ServerList, this.menuPageParent);
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x000A646D File Offset: 0x000A466D
	public void ButtonConfirm()
	{
		this.menuPageServerList.SetSearch(this.menuTextInput.textCurrent);
		this.ExitPage();
	}

	// Token: 0x04001F06 RID: 7942
	public MenuButton confirmButton;

	// Token: 0x04001F07 RID: 7943
	public MenuTextInput menuTextInput;

	// Token: 0x04001F08 RID: 7944
	internal MenuPage menuPageParent;

	// Token: 0x04001F09 RID: 7945
	internal MenuPageServerList menuPageServerList;
}
