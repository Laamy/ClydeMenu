using System;
using UnityEngine;

// Token: 0x0200016A RID: 362
public class ItemOrbIndestructible : MonoBehaviour
{
	// Token: 0x06000C4E RID: 3150 RVA: 0x0006D1CB File Offset: 0x0006B3CB
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x0006D1E8 File Offset: 0x0006B3E8
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
				physGrabObject.OverrideIndestructible(0.1f);
			}
		}
	}

	// Token: 0x04001409 RID: 5129
	private ItemOrb itemOrb;

	// Token: 0x0400140A RID: 5130
	private PhysGrabObject physGrabObject;
}
