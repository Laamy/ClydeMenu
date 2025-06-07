using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class MusicEnemySighting : MonoBehaviour
{
	// Token: 0x0600005B RID: 91 RVA: 0x00003BDD File Offset: 0x00001DDD
	private void Start()
	{
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00003BEC File Offset: 0x00001DEC
	private IEnumerator Logic()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(1f);
		for (;;)
		{
			bool flag = false;
			foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
			{
				if (enemyParent.Spawned && enemyParent.Enemy.SightingStinger)
				{
					if (!enemyParent.Enemy.HasPlayerDistance)
					{
						Debug.LogError(enemyParent.name + " needs 'player distance' component for sighting stinger.");
					}
					else if (!enemyParent.Enemy.HasOnScreen)
					{
						Debug.LogError(enemyParent.name + " needs 'on screen' component for sighting stinger.");
					}
					else if (enemyParent.Enemy.PlayerDistance.PlayerDistanceLocal <= this.DistanceMax && enemyParent.Enemy.OnScreen.OnScreenLocal && enemyParent.Enemy.TeleportedTimer <= 0f && enemyParent.Enemy.CurrentState != EnemyState.Spawn)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				if (!this.Active && this.CooldownTimer <= 0f)
				{
					this.LevelMusic.Interrupt(10f);
					GameDirector.instance.CameraImpact.Shake(2f, 0.1f);
					GameDirector.instance.CameraShake.Shake(2f, 1f);
					this.Active = true;
					this.Source.clip = this.Sounds[Random.Range(0, this.Sounds.Length)];
					this.Source.Play();
					this.CooldownTimer = this.Cooldown;
					this.ActiveTimer = this.ActiveTime;
				}
			}
			else if (this.CooldownTimer > 0f)
			{
				this.CooldownTimer -= Time.deltaTime;
			}
			if (this.ActiveTimer > 0f)
			{
				this.ActiveTimer -= Time.deltaTime;
				if (this.ActiveTimer <= 0f)
				{
					this.Active = false;
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x040000A5 RID: 165
	public AudioSource Source;

	// Token: 0x040000A6 RID: 166
	public LevelMusic LevelMusic;

	// Token: 0x040000A7 RID: 167
	public float Cooldown;

	// Token: 0x040000A8 RID: 168
	private float CooldownTimer;

	// Token: 0x040000A9 RID: 169
	public float DistanceMax;

	// Token: 0x040000AA RID: 170
	internal bool Active;

	// Token: 0x040000AB RID: 171
	public float ActiveTime;

	// Token: 0x040000AC RID: 172
	private float ActiveTimer;

	// Token: 0x040000AD RID: 173
	public AudioClip[] Sounds;
}
