using System;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class ItemDroneTargetingCondition : MonoBehaviour, ITargetingCondition
{
	// Token: 0x06000AC0 RID: 2752 RVA: 0x0005EC83 File Offset: 0x0005CE83
	public bool CustomTargetingCondition(GameObject target)
	{
		target.GetComponent<PhysGrabObjectImpactDetector>();
		return target.CompareTag("Enemy");
	}
}
