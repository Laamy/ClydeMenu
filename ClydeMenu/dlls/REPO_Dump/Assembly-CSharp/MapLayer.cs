using System;
using UnityEngine;

// Token: 0x02000198 RID: 408
public class MapLayer : MonoBehaviour
{
	// Token: 0x06000DB4 RID: 3508 RVA: 0x00077A76 File Offset: 0x00075C76
	private void Start()
	{
		this.positionStart = base.transform.position;
	}

	// Token: 0x04001608 RID: 5640
	public int layer;

	// Token: 0x04001609 RID: 5641
	internal Vector3 positionStart;
}
