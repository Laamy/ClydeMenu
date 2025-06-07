using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000214 RID: 532
public class LobbyMenuOpen : MonoBehaviour
{
	// Token: 0x060011D4 RID: 4564 RVA: 0x000A26F8 File Offset: 0x000A08F8
	private void Awake()
	{
		LobbyMenuOpen.instance = this;
	}

	// Token: 0x060011D5 RID: 4565 RVA: 0x000A2700 File Offset: 0x000A0900
	private void Update()
	{
		if (this.opened)
		{
			return;
		}
		this.timer -= Time.deltaTime;
		if (this.timer <= 0f)
		{
			GameDirector.instance.CameraShake.Shake(0.25f, 0.05f);
			GameDirector.instance.CameraImpact.Shake(0.25f, 0.05f);
			MenuManager.instance.PageOpen(MenuPageIndex.Lobby, false);
			if (SemiFunc.IsMasterClient())
			{
				PhotonNetwork.CurrentRoom.IsOpen = true;
				if (!GameManager.instance.connectRandom)
				{
					SteamManager.instance.UnlockLobby(false);
				}
				else
				{
					PhotonNetwork.CurrentRoom.IsVisible = true;
				}
			}
			this.opened = true;
		}
	}

	// Token: 0x04001E33 RID: 7731
	public static LobbyMenuOpen instance;

	// Token: 0x04001E34 RID: 7732
	public float timer = 2f;

	// Token: 0x04001E35 RID: 7733
	private bool opened;
}
