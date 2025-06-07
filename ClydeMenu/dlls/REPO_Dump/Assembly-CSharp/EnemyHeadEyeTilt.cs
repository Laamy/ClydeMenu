using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class EnemyHeadEyeTilt : MonoBehaviour
{
	// Token: 0x0600030A RID: 778 RVA: 0x0001E57C File Offset: 0x0001C77C
	private void Update()
	{
		base.transform.localRotation = this.Follow.localRotation;
	}

	// Token: 0x04000545 RID: 1349
	public Transform Follow;
}
