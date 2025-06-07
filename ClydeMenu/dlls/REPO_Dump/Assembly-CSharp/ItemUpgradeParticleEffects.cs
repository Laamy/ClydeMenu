using System;
using UnityEngine;

// Token: 0x02000179 RID: 377
public class ItemUpgradeParticleEffects : MonoBehaviour
{
	// Token: 0x06000D18 RID: 3352 RVA: 0x000735DB File Offset: 0x000717DB
	private void Update()
	{
		this.destroyTimer += Time.deltaTime;
		if (this.destroyTimer > 5f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040014FA RID: 5370
	private float destroyTimer;
}
