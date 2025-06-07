using System;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class ItemUpgradePlayerTumbleWings : MonoBehaviour
{
	// Token: 0x06000D43 RID: 3395 RVA: 0x00074039 File Offset: 0x00072239
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x00074047 File Offset: 0x00072247
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerTumbleWings(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x0400151B RID: 5403
	private ItemToggle itemToggle;
}
