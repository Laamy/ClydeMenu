using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D2 RID: 466
public class PlayerPhysObjectStander : MonoBehaviour
{
	// Token: 0x06000FFF RID: 4095 RVA: 0x00093360 File Offset: 0x00091560
	private void Awake()
	{
		this.Collider = base.GetComponent<SphereCollider>();
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x00093370 File Offset: 0x00091570
	private void Update()
	{
		if (this.checkTimer <= 0f)
		{
			this.physGrabObjects.Clear();
			Collider[] array = Physics.OverlapSphere(base.transform.position, this.Collider.radius, this.layerMask);
			if (array.Length != 0)
			{
				foreach (Collider collider in array)
				{
					PhysGrabObject physGrabObject = collider.gameObject.GetComponent<PhysGrabObject>();
					if (!physGrabObject)
					{
						physGrabObject = collider.gameObject.GetComponentInParent<PhysGrabObject>();
					}
					if (physGrabObject)
					{
						this.physGrabObjects.Add(physGrabObject);
					}
				}
			}
			this.checkTimer = 0.1f;
			return;
		}
		this.checkTimer -= 1f * Time.deltaTime;
	}

	// Token: 0x04001B33 RID: 6963
	public LayerMask layerMask;

	// Token: 0x04001B34 RID: 6964
	private SphereCollider Collider;

	// Token: 0x04001B35 RID: 6965
	internal List<PhysGrabObject> physGrabObjects = new List<PhysGrabObject>();

	// Token: 0x04001B36 RID: 6966
	private float checkTimer;
}
