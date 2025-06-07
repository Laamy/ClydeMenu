using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class ItemDroneZeroGravity : MonoBehaviour
{
	// Token: 0x06000ADA RID: 2778 RVA: 0x0005FE06 File Offset: 0x0005E006
	private void Start()
	{
		this.itemDrone = base.GetComponent<ItemDrone>();
		this.myPhysGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemBattery = base.GetComponent<ItemBattery>();
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0005FE38 File Offset: 0x0005E038
	private void FixedUpdate()
	{
		if (this.itemDrone.magnetActive && this.itemDrone.magnetTargetPhysGrabObject)
		{
			if (this.itemDrone.playerTumbleTarget)
			{
				this.itemBattery.batteryLife -= 2f * Time.fixedDeltaTime;
				this.itemDrone.magnetTargetPhysGrabObject.OverrideMaterial(SemiFunc.PhysicMaterialSticky(), 0.1f);
			}
			EnemyParent componentInParent = this.itemDrone.magnetTargetPhysGrabObject.GetComponentInParent<EnemyParent>();
			if (componentInParent)
			{
				SemiFunc.ItemAffectEnemyBatteryDrain(componentInParent, this.itemBattery, this.tumbleEnemyTimer, Time.fixedDeltaTime, 1f);
				this.tumbleEnemyTimer += Time.fixedDeltaTime;
			}
		}
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x0005FEFC File Offset: 0x0005E0FC
	private void Update()
	{
		if (!this.itemDrone.itemActivated)
		{
			this.tumbleEnemyTimer = 0f;
		}
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		if (this.itemDrone.itemActivated && this.itemDrone.magnetActive && this.itemDrone.playerAvatarTarget && this.itemDrone.targetIsLocalPlayer)
		{
			this.itemBattery.batteryLife -= 2f * Time.deltaTime;
			PlayerController.instance.AntiGravity(0.1f);
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
				this.itemDrone.magnetTargetPhysGrabObject.OverrideDrag(0.1f, 0.1f);
				this.itemDrone.magnetTargetPhysGrabObject.OverrideAngularDrag(0.1f, 0.1f);
				this.itemDrone.magnetTargetPhysGrabObject.OverrideZeroGravity(0.1f);
			}
		}
	}

	// Token: 0x0400117E RID: 4478
	private ItemDrone itemDrone;

	// Token: 0x0400117F RID: 4479
	private PhysGrabObject myPhysGrabObject;

	// Token: 0x04001180 RID: 4480
	private ItemEquippable itemEquippable;

	// Token: 0x04001181 RID: 4481
	private float tumbleEnemyTimer;

	// Token: 0x04001182 RID: 4482
	private ItemBattery itemBattery;
}
