using System;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class ItemOrbZeroGravity : MonoBehaviour
{
	// Token: 0x06000C57 RID: 3159 RVA: 0x0006D598 File Offset: 0x0006B798
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemBattery = base.GetComponent<ItemBattery>();
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x0006D5BE File Offset: 0x0006B7BE
	private void BatteryDrain(float amount)
	{
		this.itemBattery.batteryLife -= amount * Time.deltaTime;
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x0006D5DC File Offset: 0x0006B7DC
	private void Update()
	{
		if (!this.itemOrb.itemActive)
		{
			return;
		}
		if (this.itemOrb.localPlayerAffected)
		{
			PlayerController.instance.AntiGravity(0.1f);
			this.BatteryDrain(0.5f);
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject physGrabObject in this.itemOrb.objectAffected)
		{
			if (physGrabObject && this.physGrabObject != physGrabObject)
			{
				physGrabObject.OverrideDrag(0.5f, 0.1f);
				physGrabObject.OverrideAngularDrag(0.5f, 0.1f);
				physGrabObject.OverrideZeroGravity(0.1f);
				if (!physGrabObject.GetComponent<PlayerTumble>())
				{
					this.BatteryDrain(0.5f);
				}
			}
		}
	}

	// Token: 0x0400140F RID: 5135
	private ItemOrb itemOrb;

	// Token: 0x04001410 RID: 5136
	private PhysGrabObject physGrabObject;

	// Token: 0x04001411 RID: 5137
	private ItemBattery itemBattery;
}
