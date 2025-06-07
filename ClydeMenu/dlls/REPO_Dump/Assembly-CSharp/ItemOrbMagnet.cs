using System;
using UnityEngine;

// Token: 0x0200016B RID: 363
public class ItemOrbMagnet : MonoBehaviour
{
	// Token: 0x06000C51 RID: 3153 RVA: 0x0006D278 File Offset: 0x0006B478
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x0006D294 File Offset: 0x0006B494
	private void FixedUpdate()
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
				Vector3 normalized = (this.physGrabObject.transform.position - physGrabObject.transform.position).normalized;
				if ((this.physGrabObject.transform.position - physGrabObject.transform.position).magnitude > 0.45f)
				{
					physGrabObject.rb.AddForce(normalized * Mathf.Clamp(physGrabObject.rb.mass * 10f, 0.2f, 5f));
				}
				physGrabObject.rb.velocity = this.physGrabObject.rb.velocity;
				physGrabObject.OverrideZeroGravity(0.1f);
			}
		}
	}

	// Token: 0x0400140B RID: 5131
	private ItemOrb itemOrb;

	// Token: 0x0400140C RID: 5132
	private PhysGrabObject physGrabObject;
}
