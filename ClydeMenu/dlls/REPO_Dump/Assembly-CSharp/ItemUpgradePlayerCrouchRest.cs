using System;
using UnityEngine;

// Token: 0x0200017A RID: 378
public class ItemUpgradePlayerCrouchRest : MonoBehaviour
{
	// Token: 0x06000D1A RID: 3354 RVA: 0x0007360F File Offset: 0x0007180F
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x0007361D File Offset: 0x0007181D
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerCrouchRest(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040014FB RID: 5371
	private ItemToggle itemToggle;
}
