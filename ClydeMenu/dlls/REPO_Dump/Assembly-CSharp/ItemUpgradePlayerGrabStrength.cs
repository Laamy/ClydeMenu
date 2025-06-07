using System;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class ItemUpgradePlayerGrabStrength : MonoBehaviour
{
	// Token: 0x06000D26 RID: 3366 RVA: 0x000736EF File Offset: 0x000718EF
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x000736FD File Offset: 0x000718FD
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerGrabStrength(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040014FF RID: 5375
	private ItemToggle itemToggle;
}
