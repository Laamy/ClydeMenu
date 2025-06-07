using System;
using UnityEngine;

// Token: 0x0200017C RID: 380
public class ItemUpgradePlayerExtraJump : MonoBehaviour
{
	// Token: 0x06000D20 RID: 3360 RVA: 0x0007367F File Offset: 0x0007187F
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x0007368D File Offset: 0x0007188D
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerExtraJump(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040014FD RID: 5373
	private ItemToggle itemToggle;
}
