using System;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class ItemDroneBattery : MonoBehaviour, ITargetingCondition
{
	// Token: 0x06000AC6 RID: 2758 RVA: 0x0005F1A3 File Offset: 0x0005D3A3
	private void Start()
	{
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemDrone = base.GetComponent<ItemDrone>();
		this.myPhysGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x0005F1D5 File Offset: 0x0005D3D5
	public bool CustomTargetingCondition(GameObject target)
	{
		return SemiFunc.BatteryChargeCondition(target.GetComponent<ItemBattery>());
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x0005F1E4 File Offset: 0x0005D3E4
	private void Update()
	{
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemDrone.itemActivated)
		{
			this.myPhysGrabObject.OverrideZeroGravity(0.1f);
			this.myPhysGrabObject.OverrideDrag(1f, 0.1f);
			this.myPhysGrabObject.OverrideAngularDrag(10f, 0.1f);
			if (this.itemDrone.magnetActive && this.itemDrone.magnetTargetPhysGrabObject)
			{
				ItemBattery component = this.itemDrone.magnetTargetPhysGrabObject.GetComponent<ItemBattery>();
				if (component)
				{
					component.ChargeBattery(base.gameObject, 5f);
					this.itemBattery.Drain(5f);
				}
				if (component.batteryLife >= 99f && !component.batteryActive && component.autoDrain)
				{
					this.itemDrone.MagnetActiveToggle(false);
				}
			}
		}
	}

	// Token: 0x04001165 RID: 4453
	private ItemDrone itemDrone;

	// Token: 0x04001166 RID: 4454
	private PhysGrabObject myPhysGrabObject;

	// Token: 0x04001167 RID: 4455
	private ItemEquippable itemEquippable;

	// Token: 0x04001168 RID: 4456
	private ItemBattery itemBattery;
}
