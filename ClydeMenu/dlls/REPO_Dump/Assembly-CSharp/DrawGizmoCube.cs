using System;
using UnityEngine;

// Token: 0x02000238 RID: 568
public class DrawGizmoCube : MonoBehaviour
{
	// Token: 0x06001299 RID: 4761 RVA: 0x000A7363 File Offset: 0x000A5563
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0.95f, 0f, 0.2f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}

	// Token: 0x0600129A RID: 4762 RVA: 0x000A73A2 File Offset: 0x000A55A2
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 0.95f, 0f, 0.5f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}
}
