using System;
using UnityEngine;

// Token: 0x0200017D RID: 381
public class ItemUpgradePlayerGrabRange : MonoBehaviour
{
	// Token: 0x06000D23 RID: 3363 RVA: 0x000736B7 File Offset: 0x000718B7
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x000736C5 File Offset: 0x000718C5
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerGrabRange(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040014FE RID: 5374
	private ItemToggle itemToggle;
}
