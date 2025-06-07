using System;
using UnityEngine;

// Token: 0x0200023D RID: 573
public class DebugCube : MonoBehaviour
{
	// Token: 0x060012B9 RID: 4793 RVA: 0x000A8258 File Offset: 0x000A6458
	private void OnDrawGizmos()
	{
		Gizmos.color = this.color;
		Gizmos.matrix = this.gizmoTransform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		Gizmos.color = new Color(this.color.r, this.color.g, this.color.b, 0.2f);
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}

	// Token: 0x04001FD6 RID: 8150
	public Transform gizmoTransform;

	// Token: 0x04001FD7 RID: 8151
	internal Color color;
}
