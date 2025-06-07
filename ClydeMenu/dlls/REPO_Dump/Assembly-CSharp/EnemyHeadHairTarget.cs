using System;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class EnemyHeadHairTarget : MonoBehaviour
{
	// Token: 0x06000326 RID: 806 RVA: 0x0001F139 File Offset: 0x0001D339
	private void Start()
	{
		base.transform.parent = this.Parent;
	}

	// Token: 0x0400058D RID: 1421
	public Transform Parent;
}
