using System;
using UnityEngine;

// Token: 0x02000070 RID: 112
public class EnemyRobePersistent : MonoBehaviour
{
	// Token: 0x060003DE RID: 990 RVA: 0x0002664C File Offset: 0x0002484C
	private void Update()
	{
		if (this.enemyRobe.isActiveAndEnabled && this.enemyRobe.currentState != EnemyRobe.State.Spawn)
		{
			if (!this.particleConstant.isPlaying)
			{
				this.particleConstant.Play();
				return;
			}
		}
		else if (this.particleConstant.isPlaying)
		{
			this.particleConstant.Stop();
		}
	}

	// Token: 0x04000690 RID: 1680
	public EnemyRobe enemyRobe;

	// Token: 0x04000691 RID: 1681
	public ParticleSystem particleConstant;
}
