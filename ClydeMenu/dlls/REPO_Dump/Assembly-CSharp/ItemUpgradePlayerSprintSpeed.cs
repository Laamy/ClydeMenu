using System;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class ItemUpgradePlayerSprintSpeed : MonoBehaviour
{
	// Token: 0x06000D2F RID: 3375 RVA: 0x00073797 File Offset: 0x00071997
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x000737A5 File Offset: 0x000719A5
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerSprintSpeed(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x04001502 RID: 5378
	private ItemToggle itemToggle;
}
