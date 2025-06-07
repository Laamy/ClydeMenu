using System;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class ItemUpgradePlayerGrabThrow : MonoBehaviour
{
	// Token: 0x06000D29 RID: 3369 RVA: 0x00073727 File Offset: 0x00071927
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x00073735 File Offset: 0x00071935
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerThrowStrength(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x04001500 RID: 5376
	private ItemToggle itemToggle;
}
