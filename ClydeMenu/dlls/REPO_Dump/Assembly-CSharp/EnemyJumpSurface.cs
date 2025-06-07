using System;
using UnityEngine;

// Token: 0x020000A0 RID: 160
public class EnemyJumpSurface : MonoBehaviour
{
	// Token: 0x06000662 RID: 1634 RVA: 0x0003E228 File Offset: 0x0003C428
	private void OnDrawGizmos()
	{
		Vector3 position = base.transform.position;
		Vector3 vector = position + base.transform.TransformDirection(this.jumpDirection.normalized * 0.3f);
		Gizmos.DrawLine(position, vector);
		Gizmos.DrawWireSphere(vector, 0.03f);
	}

	// Token: 0x04000A97 RID: 2711
	public Vector3 jumpDirection;
}
