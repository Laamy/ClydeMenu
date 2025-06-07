using System;
using UnityEngine;

// Token: 0x020002CB RID: 715
public class PhysGrabObjectSphereCollider : MonoBehaviour
{
	// Token: 0x06001664 RID: 5732 RVA: 0x000C5C00 File Offset: 0x000C3E00
	private void OnDrawGizmos()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f * this.gizmoTransparency);
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.localScale);
		Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
		Gizmos.color = new Color(0f, 1f, 0f, 0.2f * this.gizmoTransparency);
		Gizmos.DrawSphere(Vector3.zero, 0.5f);
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x040026A6 RID: 9894
	public bool drawGizmos = true;

	// Token: 0x040026A7 RID: 9895
	[Range(0.2f, 1f)]
	public float gizmoTransparency = 1f;
}
