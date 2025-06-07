using System;
using UnityEngine;

// Token: 0x020002CA RID: 714
public class PhysGrabObjectMeshCollider : MonoBehaviour
{
	// Token: 0x06001662 RID: 5730 RVA: 0x000C5B20 File Offset: 0x000C3D20
	private void OnDrawGizmos()
	{
		if (!this.showGizmo)
		{
			return;
		}
		Mesh sharedMesh = base.GetComponent<MeshCollider>().sharedMesh;
		if (sharedMesh != null)
		{
			Gizmos.color = new Color(0f, 1f, 0f, 0.2f * this.gizmoAlpha);
			Gizmos.DrawMesh(sharedMesh, base.transform.position, base.transform.rotation, base.transform.localScale);
			Gizmos.color = new Color(0f, 1f, 0f, 0.4f * this.gizmoAlpha);
			Gizmos.DrawWireMesh(sharedMesh, base.transform.position, base.transform.rotation, base.transform.localScale);
		}
	}

	// Token: 0x040026A4 RID: 9892
	public bool showGizmo = true;

	// Token: 0x040026A5 RID: 9893
	[Range(0.2f, 1f)]
	public float gizmoAlpha = 1f;
}
