using System;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class EnemyBangBomb : MonoBehaviour
{
	// Token: 0x0600013B RID: 315 RVA: 0x0000C3A5 File Offset: 0x0000A5A5
	private void Update()
	{
		this.source.rotation = SemiFunc.SpringQuaternionGet(this.spring, this.target.rotation, -1f);
	}

	// Token: 0x040002B0 RID: 688
	public SpringQuaternion spring;

	// Token: 0x040002B1 RID: 689
	public Transform source;

	// Token: 0x040002B2 RID: 690
	public Transform target;
}
