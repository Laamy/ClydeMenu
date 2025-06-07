using System;
using UnityEngine;

// Token: 0x02000244 RID: 580
public class GizmoBall : MonoBehaviour
{
	// Token: 0x060012F0 RID: 4848 RVA: 0x000A9978 File Offset: 0x000A7B78
	private void OnDrawGizmos()
	{
		this.color.a = 0.5f;
		Gizmos.color = this.color;
		Gizmos.DrawSphere(base.transform.position + this.offset, this.radius);
		this.color.a = 1f;
		Gizmos.color = this.color;
		Gizmos.DrawWireSphere(base.transform.position + this.offset, this.radius);
	}

	// Token: 0x04002030 RID: 8240
	public Color color = Color.red;

	// Token: 0x04002031 RID: 8241
	public float radius = 0.5f;

	// Token: 0x04002032 RID: 8242
	public Vector3 offset;
}
