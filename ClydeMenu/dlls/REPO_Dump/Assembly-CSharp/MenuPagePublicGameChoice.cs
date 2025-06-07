using System;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class MenuPagePublicGameChoice : MonoBehaviour
{
	// Token: 0x06001255 RID: 4693 RVA: 0x000A5BC3 File Offset: 0x000A3DC3
	private void Update()
	{
		if (SemiFunc.InputDown(InputKey.Back) && MenuManager.instance.currentMenuPageIndex == MenuPageIndex.PublicGameChoice)
		{
			this.ExitPage();
		}
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x000A5BE4 File Offset: 0x000A3DE4
	public void ButtonRandomMatchmaking()
	{
		RunManager.instance.ResetProgress();
		StatsManager.instance.saveFileCurrent = "";
		GameManager.instance.SetConnectRandom(true);
		GameManager.instance.localTest = false;
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.LobbyMenu);
		RunManager.instance.lobbyJoin = true;
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x000A5C38 File Offset: 0x000A3E38
	public void ButtonServerList()
	{
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.ServerList, false);
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x000A5C52 File Offset: 0x000A3E52
	public void ExitPage()
	{
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.Regions, false).GetComponent<MenuPageRegions>().type = MenuPageRegions.Type.PlayRandom;
	}
}
