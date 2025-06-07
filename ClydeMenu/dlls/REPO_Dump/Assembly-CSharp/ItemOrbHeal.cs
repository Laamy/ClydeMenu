using System;
using UnityEngine;

// Token: 0x02000169 RID: 361
public class ItemOrbHeal : MonoBehaviour
{
	// Token: 0x06000C4B RID: 3147 RVA: 0x0006D108 File Offset: 0x0006B308
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemBattery = base.GetComponent<ItemBattery>();
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x0006D130 File Offset: 0x0006B330
	private void Update()
	{
		if (!this.itemOrb.itemActive || this.itemBattery.batteryLife <= 0f)
		{
			return;
		}
		if (this.itemOrb.localPlayerAffected)
		{
			if (this.healTimer > this.healRate)
			{
				PlayerController.instance.playerAvatarScript.playerHealth.Heal(this.healAmount, true);
				this.healTimer = 0f;
			}
			this.healTimer += Time.deltaTime;
		}
	}

	// Token: 0x04001403 RID: 5123
	private ItemOrb itemOrb;

	// Token: 0x04001404 RID: 5124
	private ItemBattery itemBattery;

	// Token: 0x04001405 RID: 5125
	private PhysGrabObject physGrabObject;

	// Token: 0x04001406 RID: 5126
	private float healRate = 2f;

	// Token: 0x04001407 RID: 5127
	private float healTimer;

	// Token: 0x04001408 RID: 5128
	private int healAmount = 10;
}
