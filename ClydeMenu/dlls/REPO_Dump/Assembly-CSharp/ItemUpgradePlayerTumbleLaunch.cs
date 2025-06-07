using System;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class ItemUpgradePlayerTumbleLaunch : MonoBehaviour
{
	// Token: 0x06000D32 RID: 3378 RVA: 0x000737CF File Offset: 0x000719CF
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x000737DD File Offset: 0x000719DD
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerTumbleLaunch(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x04001503 RID: 5379
	private ItemToggle itemToggle;
}
