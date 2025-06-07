using System;
using UnityEngine;

// Token: 0x0200029F RID: 671
public class BarrelValuable : Trap
{
	// Token: 0x060014F3 RID: 5363 RVA: 0x000B9907 File Offset: 0x000B7B07
	protected override void Start()
	{
		base.Start();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x000B991C File Offset: 0x000B7B1C
	public void Explode()
	{
		this.particleScriptExplosion.Spawn(this.Center.position, 1f, 50, 100, 1f, false, false, 1f);
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x000B9955 File Offset: 0x000B7B55
	public void PotentialExplode()
	{
		if (this.isLocal)
		{
			if (this.HitCount >= this.MaxHitCount - 1)
			{
				this.Explode();
				return;
			}
			this.HitCount++;
		}
	}

	// Token: 0x04002444 RID: 9284
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x04002445 RID: 9285
	private int HitCount;

	// Token: 0x04002446 RID: 9286
	private int MaxHitCount = 3;

	// Token: 0x04002447 RID: 9287
	public Transform Center;
}
