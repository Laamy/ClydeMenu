using System;
using UnityEngine;

// Token: 0x02000052 RID: 82
public class EnemyGnomeStunFly : MonoBehaviour
{
	// Token: 0x060002CE RID: 718 RVA: 0x0001C974 File Offset: 0x0001AB74
	private void Update()
	{
		if (this.enemyGnome.currentState == EnemyGnome.State.Stun && this.enemy.IsStunned() && (float)this.enemy.Rigidbody.physGrabObject.playerGrabbing.Count <= 0f && this.enemy.Rigidbody.physGrabObject.rbVelocity.magnitude > 2f)
		{
			this.soundTimer = 0.5f;
		}
		if (!this.enemy.isActiveAndEnabled)
		{
			this.spawnTimer = 2f;
			this.sound.PlayLoop(false, 5f, 50f, 1f);
		}
		else if (this.soundTimer > 0f && this.spawnTimer <= 0f)
		{
			this.sound.PlayLoop(true, 5f, 5f, 1f);
		}
		else
		{
			this.sound.PlayLoop(false, 5f, 5f, 1f);
		}
		if (this.spawnTimer > 0f)
		{
			this.spawnTimer -= Time.deltaTime;
		}
		if (this.soundTimer > 0f)
		{
			this.soundTimer -= Time.deltaTime;
		}
	}

	// Token: 0x040004DE RID: 1246
	public Enemy enemy;

	// Token: 0x040004DF RID: 1247
	public EnemyGnome enemyGnome;

	// Token: 0x040004E0 RID: 1248
	private float soundTimer;

	// Token: 0x040004E1 RID: 1249
	private float spawnTimer;

	// Token: 0x040004E2 RID: 1250
	[Space]
	public Sound sound;
}
