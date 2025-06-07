using System;
using UnityEngine;

// Token: 0x0200017B RID: 379
public class ItemUpgradePlayerEnergy : MonoBehaviour
{
	// Token: 0x06000D1D RID: 3357 RVA: 0x00073647 File Offset: 0x00071847
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x00073655 File Offset: 0x00071855
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerEnergy(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040014FC RID: 5372
	private ItemToggle itemToggle;
}
