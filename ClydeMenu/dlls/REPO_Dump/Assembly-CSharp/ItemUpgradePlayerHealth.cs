using System;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class ItemUpgradePlayerHealth : MonoBehaviour
{
	// Token: 0x06000D2C RID: 3372 RVA: 0x0007375F File Offset: 0x0007195F
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x0007376D File Offset: 0x0007196D
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerHealth(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x04001501 RID: 5377
	private ItemToggle itemToggle;
}
