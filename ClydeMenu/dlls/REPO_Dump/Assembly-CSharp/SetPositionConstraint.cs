using System;
using UnityEngine;
using UnityEngine.Animations;

// Token: 0x02000133 RID: 307
public class SetPositionConstraint : MonoBehaviour
{
	// Token: 0x06000AAE RID: 2734 RVA: 0x0005EAC7 File Offset: 0x0005CCC7
	private void Start()
	{
		base.GetComponent<PositionConstraint>().constraintActive = true;
	}
}
