using System;
using UnityEngine;

// Token: 0x02000167 RID: 359
public class ItemOrbBattery : MonoBehaviour, ITargetingCondition
{
	// Token: 0x06000C44 RID: 3140 RVA: 0x0006CF03 File Offset: 0x0006B103
	public bool CustomTargetingCondition(GameObject target)
	{
		return SemiFunc.BatteryChargeCondition(target.GetComponent<ItemBattery>());
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x0006CF10 File Offset: 0x0006B110
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x0006CF2C File Offset: 0x0006B12C
	private void Update()
	{
		if (!this.itemOrb.itemActive)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject physGrabObject in this.itemOrb.objectAffected)
		{
			if (physGrabObject && this.physGrabObject != physGrabObject)
			{
				physGrabObject.GetComponent<ItemBattery>().ChargeBattery(base.gameObject, SemiFunc.BatteryGetChargeRate(3));
			}
		}
	}

	// Token: 0x040013FF RID: 5119
	private ItemOrb itemOrb;

	// Token: 0x04001400 RID: 5120
	private PhysGrabObject physGrabObject;
}
