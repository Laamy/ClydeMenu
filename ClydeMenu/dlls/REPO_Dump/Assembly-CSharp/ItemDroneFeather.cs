using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class ItemDroneFeather : MonoBehaviour
{
	// Token: 0x06000ACA RID: 2762 RVA: 0x0005F2DB File Offset: 0x0005D4DB
	private void Start()
	{
		this.itemDrone = base.GetComponent<ItemDrone>();
		this.myPhysGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemBattery = base.GetComponent<ItemBattery>();
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x0005F30D File Offset: 0x0005D50D
	private void BatteryDrain(float amount)
	{
		this.itemBattery.batteryLife -= amount * Time.fixedDeltaTime;
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x0005F328 File Offset: 0x0005D528
	private void FixedUpdate()
	{
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		if (this.itemDrone.itemActivated && this.itemDrone.magnetActive && this.itemDrone.playerAvatarTarget && this.itemDrone.targetIsLocalPlayer)
		{
			PlayerController.instance.Feather(0.1f);
			this.BatteryDrain(2f);
		}
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
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
				PlayerTumble component = this.itemDrone.magnetTargetPhysGrabObject.GetComponent<PlayerTumble>();
				if (!component)
				{
					this.itemDrone.magnetTargetPhysGrabObject.OverrideMass(1f, 0.1f);
					this.itemDrone.magnetTargetPhysGrabObject.OverrideDrag(1f, 0.1f);
					this.itemDrone.magnetTargetPhysGrabObject.OverrideAngularDrag(5f, 0.1f);
					return;
				}
				component.DisableCustomGravity(0.1f);
				this.itemDrone.magnetTargetPhysGrabObject.OverrideMass(0.5f, 0.1f);
				this.BatteryDrain(2f);
				if (component.playerAvatar.isLocal)
				{
					PlayerController.instance.Feather(0.1f);
				}
			}
		}
	}

	// Token: 0x04001169 RID: 4457
	private ItemDrone itemDrone;

	// Token: 0x0400116A RID: 4458
	private PhysGrabObject myPhysGrabObject;

	// Token: 0x0400116B RID: 4459
	private ItemEquippable itemEquippable;

	// Token: 0x0400116C RID: 4460
	private ItemBattery itemBattery;
}
