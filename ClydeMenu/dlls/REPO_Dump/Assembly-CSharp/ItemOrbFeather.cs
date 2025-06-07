using System;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class ItemOrbFeather : MonoBehaviour
{
	// Token: 0x06000C48 RID: 3144 RVA: 0x0006CFC8 File Offset: 0x0006B1C8
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x0006CFE4 File Offset: 0x0006B1E4
	private void Update()
	{
		if (!this.itemOrb.itemActive)
		{
			return;
		}
		if (this.itemOrb.localPlayerAffected)
		{
			PlayerController.instance.Feather(0.1f);
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject physGrabObject in this.itemOrb.objectAffected)
		{
			if (physGrabObject && this.physGrabObject != physGrabObject)
			{
				PlayerTumble component = physGrabObject.GetComponent<PlayerTumble>();
				if (!component)
				{
					physGrabObject.OverrideMass(1f, 0.1f);
					physGrabObject.OverrideDrag(1f, 0.1f);
					physGrabObject.OverrideAngularDrag(5f, 0.1f);
				}
				else
				{
					component.DisableCustomGravity(0.1f);
					physGrabObject.OverrideMass(0.05f, 0.1f);
					if (component.playerAvatar.isLocal)
					{
						PlayerController.instance.Feather(0.1f);
					}
				}
			}
		}
	}

	// Token: 0x04001401 RID: 5121
	private ItemOrb itemOrb;

	// Token: 0x04001402 RID: 5122
	private PhysGrabObject physGrabObject;
}
