using System;
using UnityEngine;

// Token: 0x020000E7 RID: 231
public class NavMeshBox : MonoBehaviour
{
	// Token: 0x06000825 RID: 2085 RVA: 0x0004FA04 File Offset: 0x0004DC04
	private void OnDrawGizmos()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		Gizmos.color = new Color(0.4f, 0.19f, 1f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(component.center, component.size);
		Gizmos.color = new Color(0.9f, 0.22f, 1f, 0.2f);
		Gizmos.DrawCube(component.center, component.size);
	}
}
