using System;
using UnityEngine;

// Token: 0x020002C0 RID: 704
public class VaseExplosionTest : MonoBehaviour
{
	// Token: 0x0600161E RID: 5662 RVA: 0x000C2097 File Offset: 0x000C0297
	private void Start()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
	}

	// Token: 0x0600161F RID: 5663 RVA: 0x000C20A8 File Offset: 0x000C02A8
	public void Explosion()
	{
		this.particleScriptExplosion.Spawn(this.Center.position, 1f, 10, 10, 1f, false, false, 1f);
	}

	// Token: 0x04002652 RID: 9810
	public Transform Center;

	// Token: 0x04002653 RID: 9811
	private ParticleScriptExplosion particleScriptExplosion;
}
