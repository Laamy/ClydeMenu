using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200013F RID: 319
public class ItemDroneIndestructible : MonoBehaviour
{
	// Token: 0x06000AD2 RID: 2770 RVA: 0x0005F650 File Offset: 0x0005D850
	private void Start()
	{
		this.itemDrone = base.GetComponent<ItemDrone>();
		this.myPhysGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x0005F678 File Offset: 0x0005D878
	private void Update()
	{
		if (this.itemEquippable.isEquipped)
		{
			return;
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
				this.itemDrone.magnetTargetPhysGrabObject.OverrideIndestructible(0.1f);
			}
		}
	}

	// Token: 0x04001173 RID: 4467
	private ItemDrone itemDrone;

	// Token: 0x04001174 RID: 4468
	private PhysGrabObject myPhysGrabObject;

	// Token: 0x04001175 RID: 4469
	private ItemEquippable itemEquippable;
}
