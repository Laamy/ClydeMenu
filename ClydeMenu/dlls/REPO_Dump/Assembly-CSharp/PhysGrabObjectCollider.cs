using System;
using UnityEngine;

// Token: 0x020002C9 RID: 713
public class PhysGrabObjectCollider : MonoBehaviour
{
	// Token: 0x0600165F RID: 5727 RVA: 0x000C5AE3 File Offset: 0x000C3CE3
	private void Start()
	{
		this.physGrabObject = base.GetComponentInParent<PhysGrabObject>();
	}

	// Token: 0x06001660 RID: 5728 RVA: 0x000C5AF1 File Offset: 0x000C3CF1
	private void OnDestroy()
	{
		if (this.physGrabObject)
		{
			this.physGrabObject.colliders.Remove(base.transform);
		}
	}

	// Token: 0x040026A2 RID: 9890
	[HideInInspector]
	public int colliderID;

	// Token: 0x040026A3 RID: 9891
	private PhysGrabObject physGrabObject;
}
