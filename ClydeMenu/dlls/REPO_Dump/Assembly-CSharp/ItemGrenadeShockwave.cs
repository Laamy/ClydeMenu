using System;
using UnityEngine;

// Token: 0x02000155 RID: 341
public class ItemGrenadeShockwave : MonoBehaviour
{
	// Token: 0x06000BAA RID: 2986 RVA: 0x000679B1 File Offset: 0x00065BB1
	public void Explosion()
	{
		Object.Instantiate<GameObject>(this.shockwavePrefab, base.transform.position, Quaternion.identity);
	}

	// Token: 0x040012F5 RID: 4853
	public GameObject shockwavePrefab;
}
