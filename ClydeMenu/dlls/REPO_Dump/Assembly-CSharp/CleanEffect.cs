using System;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class CleanEffect : MonoBehaviour
{
	// Token: 0x060006D0 RID: 1744 RVA: 0x00041A22 File Offset: 0x0003FC22
	public void Update()
	{
		if (!this.destroyNow)
		{
			return;
		}
		this.destroyTimer += Time.deltaTime;
		if (this.destroyTimer > 1f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00041A57 File Offset: 0x0003FC57
	public void Clean()
	{
		this.destroyNow = true;
	}

	// Token: 0x04000B94 RID: 2964
	[Space]
	[Header("Sounds")]
	public Sound CleanSound;

	// Token: 0x04000B95 RID: 2965
	public ParticleSystem GleamParticles;

	// Token: 0x04000B96 RID: 2966
	private float destroyTimer;

	// Token: 0x04000B97 RID: 2967
	public bool destroyNow;
}
