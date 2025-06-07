using System;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class ItemUpgradeMapPlayerCount : MonoBehaviour
{
	// Token: 0x06000D15 RID: 3349 RVA: 0x000735A3 File Offset: 0x000717A3
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x000735B1 File Offset: 0x000717B1
	public void Upgrade()
	{
		PunManager.instance.UpgradeMapPlayerCount(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040014F9 RID: 5369
	private ItemToggle itemToggle;
}
