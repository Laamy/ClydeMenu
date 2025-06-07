using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021D RID: 541
public class MenuPageEsc : MonoBehaviour
{
	// Token: 0x060011FA RID: 4602 RVA: 0x000A30FB File Offset: 0x000A12FB
	private void Start()
	{
		MenuPageEsc.instance = this;
		this.menuPage = base.GetComponent<MenuPage>();
		this.PlayerGainSlidersUpdate();
	}

	// Token: 0x060011FB RID: 4603 RVA: 0x000A3115 File Offset: 0x000A1315
	private void Update()
	{
		if (SemiFunc.MenuLevel())
		{
			this.menuPage.PageStateSet(MenuPage.PageState.Closing);
		}
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x000A312A File Offset: 0x000A132A
	public void ButtonEventContinue()
	{
		this.menuPage.PageStateSet(MenuPage.PageState.Closing);
	}

	// Token: 0x060011FD RID: 4605 RVA: 0x000A3138 File Offset: 0x000A1338
	public void PlayerGainSlidersUpdate()
	{
		List<PlayerAvatar> list = new List<PlayerAvatar>();
		foreach (PlayerAvatar playerAvatar in this.playerMicGainSliders.Keys)
		{
			if (!playerAvatar || !this.playerMicGainSliders[playerAvatar])
			{
				list.Add(playerAvatar);
			}
		}
		foreach (PlayerAvatar playerAvatar2 in list)
		{
			this.playerMicGainSliders.Remove(playerAvatar2);
		}
		foreach (PlayerAvatar playerAvatar3 in GameDirector.instance.PlayerList)
		{
			if (!this.playerMicGainSliders.ContainsKey(playerAvatar3) && !playerAvatar3.isLocal)
			{
				float x = 375f;
				if (SemiFunc.IsMasterClient())
				{
					x = 400f;
				}
				GameObject gameObject = Object.Instantiate<GameObject>(this.playerMicrophoneVolumeSliderPrefab, base.transform);
				gameObject.transform.localPosition = new Vector3(x, 21f, 0f);
				gameObject.transform.localPosition += new Vector3(0f, 25f * (float)this.playerMicGainSliders.Count, 0f);
				MenuSliderPlayerMicGain component = gameObject.GetComponent<MenuSliderPlayerMicGain>();
				component.playerAvatar = playerAvatar3;
				component.SliderNameSet(playerAvatar3.playerName);
				this.playerMicGainSliders.Add(playerAvatar3, component);
			}
		}
	}

	// Token: 0x060011FE RID: 4606 RVA: 0x000A3300 File Offset: 0x000A1500
	public void ButtonEventSelfDestruct()
	{
		if (SemiFunc.IsMultiplayer())
		{
			ChatManager.instance.PossessSelfDestruction();
		}
		else
		{
			PlayerAvatar.instance.playerHealth.health = 0;
			PlayerAvatar.instance.playerHealth.Hurt(1, false, -1);
		}
		MenuManager.instance.PageCloseAll();
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x000A334C File Offset: 0x000A154C
	public void ButtonEventQuit()
	{
		RunManager.instance.skipLoadingUI = true;
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			playerAvatar.quitApplication = true;
		}
		GameDirector.instance.OutroStart();
	}

	// Token: 0x06001200 RID: 4608 RVA: 0x000A33B8 File Offset: 0x000A15B8
	public void ButtonEventQuitToMenu()
	{
		GameDirector.instance.OutroStart();
		NetworkManager.instance.leavePhotonRoom = true;
	}

	// Token: 0x06001201 RID: 4609 RVA: 0x000A33CF File Offset: 0x000A15CF
	public void ButtonEventChangeColor()
	{
		MenuManager.instance.PageSwap(MenuPageIndex.Color);
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x000A33DD File Offset: 0x000A15DD
	public void ButtonEventSettings()
	{
		MenuManager.instance.PageSwap(MenuPageIndex.Settings);
	}

	// Token: 0x04001E6B RID: 7787
	public static MenuPageEsc instance;

	// Token: 0x04001E6C RID: 7788
	internal MenuPage menuPage;

	// Token: 0x04001E6D RID: 7789
	public GameObject playerMicrophoneVolumeSliderPrefab;

	// Token: 0x04001E6E RID: 7790
	internal Dictionary<PlayerAvatar, MenuSliderPlayerMicGain> playerMicGainSliders = new Dictionary<PlayerAvatar, MenuSliderPlayerMicGain>();
}
