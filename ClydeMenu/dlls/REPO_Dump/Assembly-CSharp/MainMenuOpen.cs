using System;
using UnityEngine;

// Token: 0x02000215 RID: 533
public class MainMenuOpen : MonoBehaviour
{
	// Token: 0x060011D7 RID: 4567 RVA: 0x000A27C4 File Offset: 0x000A09C4
	private void Awake()
	{
		MainMenuOpen.instance = this;
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x000A27CC File Offset: 0x000A09CC
	public void NetworkConnect()
	{
		Object.Instantiate<GameObject>(this.networkConnectPrefab);
	}

	// Token: 0x060011D9 RID: 4569 RVA: 0x000A27DA File Offset: 0x000A09DA
	private void Start()
	{
		MenuManager.instance.PageOpen(MenuPageIndex.Main, false);
	}

	// Token: 0x060011DA RID: 4570 RVA: 0x000A27E9 File Offset: 0x000A09E9
	public void MainMenuSetState(int state)
	{
		this.mainMenuGameModeState = (MainMenuOpen.MainMenuGameModeState)state;
	}

	// Token: 0x060011DB RID: 4571 RVA: 0x000A27F2 File Offset: 0x000A09F2
	public MainMenuOpen.MainMenuGameModeState MainMenuGetState()
	{
		return this.mainMenuGameModeState;
	}

	// Token: 0x04001E36 RID: 7734
	public static MainMenuOpen instance;

	// Token: 0x04001E37 RID: 7735
	public GameObject networkConnectPrefab;

	// Token: 0x04001E38 RID: 7736
	internal bool firstOpen = true;

	// Token: 0x04001E39 RID: 7737
	public MainMenuOpen.MainMenuGameModeState mainMenuGameModeState;

	// Token: 0x020003E6 RID: 998
	public enum MainMenuGameModeState
	{
		// Token: 0x04002CBD RID: 11453
		SinglePlayer,
		// Token: 0x04002CBE RID: 11454
		MultiPlayer
	}
}
