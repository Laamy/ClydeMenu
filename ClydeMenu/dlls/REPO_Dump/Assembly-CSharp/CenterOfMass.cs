using System;
using UnityEngine;

// Token: 0x020002C7 RID: 711
public class CenterOfMass : MonoBehaviour
{
	// Token: 0x0600165A RID: 5722 RVA: 0x000C57A9 File Offset: 0x000C39A9
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, 0.1f);
	}
}
