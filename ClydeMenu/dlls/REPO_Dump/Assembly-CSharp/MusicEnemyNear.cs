using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class MusicEnemyNear : MonoBehaviour
{
	// Token: 0x06000055 RID: 85 RVA: 0x00003AF1 File Offset: 0x00001CF1
	private void Awake()
	{
		MusicEnemyNear.instance = this;
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00003AF9 File Offset: 0x00001CF9
	private void Start()
	{
		this.CurrentTrack = this.Tracks[Random.Range(0, this.Tracks.Length)];
		this.Camera = Camera.main;
		this.NewTrack();
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00003B34 File Offset: 0x00001D34
	public void LowerVolume(float multiplier, float time)
	{
		this.LowerMultiplierTarget = multiplier;
		this.LowerTimer = time;
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00003B44 File Offset: 0x00001D44
	private IEnumerator Logic()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(1f);
		for (;;)
		{
			if (this.LowerTimer > 0f)
			{
				this.LowerTimer -= Time.deltaTime;
				if (this.LowerTimer <= 0f)
				{
					this.LowerMultiplierTarget = 1f;
				}
				this.LowerMultiplier = Mathf.Lerp(this.LowerMultiplier, this.LowerMultiplierTarget, Time.deltaTime * 5f);
			}
			else
			{
				this.LowerMultiplier = Mathf.Lerp(this.LowerMultiplier, this.LowerMultiplierTarget, Time.deltaTime * 1f);
			}
			float num = 0f;
			if (RoundDirector.instance.allExtractionPointsCompleted)
			{
				num = 0.5f;
			}
			foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
			{
				float b = 0f;
				if (enemyParent.Spawned && enemyParent.Enemy.EnemyNearMusic)
				{
					if (!enemyParent.Enemy.HasPlayerDistance)
					{
						Debug.LogError(enemyParent.name + " needs 'player distance' component for near music.");
						continue;
					}
					if (!enemyParent.Enemy.HasOnScreen)
					{
						Debug.LogError(enemyParent.name + " needs 'on screen' component for near music.");
						continue;
					}
					if (!enemyParent.Enemy.HasPlayerRoom)
					{
						Debug.LogError(enemyParent.name + " needs 'player room' component for near music.");
						continue;
					}
					float num2 = enemyParent.Enemy.PlayerDistance.PlayerDistanceLocal;
					if (enemyParent.Enemy.CurrentState != EnemyState.Spawn)
					{
						if (enemyParent.Enemy.OnScreen.OnScreenLocal && num2 <= this.OnScreenDistance)
						{
							b = this.CurrentTrack.Volume;
						}
						else if (num2 <= this.MaxDistance)
						{
							if (this.RayTimer <= 0f)
							{
								this.RayTimer = 0.5f;
								Vector3 direction = this.Camera.transform.position - enemyParent.Enemy.CenterTransform.position;
								RaycastHit raycastHit;
								this.RayResult = Physics.Raycast(enemyParent.Enemy.CenterTransform.position, direction, out raycastHit, direction.magnitude, this.Mask);
							}
							else
							{
								this.RayTimer -= Time.deltaTime;
							}
							if (!this.RayResult || enemyParent.Enemy.PlayerRoom.SameLocal)
							{
								num2 = Mathf.Clamp(num2, this.MinDistance, this.MaxDistance);
								b = Mathf.Lerp(0f, this.CurrentTrack.Volume, 1f - (num2 - this.MinDistance) / (this.MaxDistance - this.MinDistance));
							}
						}
					}
				}
				num = Mathf.Max(num, b);
			}
			this.Volume = Mathf.Lerp(this.Volume, num * this.LowerMultiplier, Time.deltaTime * this.FadeSpeed);
			if (this.Volume > 0f)
			{
				if (!this.Source.isPlaying)
				{
					this.NewTrack();
					this.Source.time = Random.Range(0f, this.Source.clip.length);
					this.Source.Play();
				}
				this.Source.volume = this.Volume;
				this.LevelMusic.Interrupt(0.5f);
			}
			else if (this.Source.isPlaying)
			{
				this.Source.Stop();
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00003B53 File Offset: 0x00001D53
	private void NewTrack()
	{
		this.CurrentTrack = this.Tracks[Random.Range(0, this.Tracks.Length)];
		this.Source.clip = this.CurrentTrack.Clip;
	}

	// Token: 0x04000094 RID: 148
	public static MusicEnemyNear instance;

	// Token: 0x04000095 RID: 149
	public LevelMusic LevelMusic;

	// Token: 0x04000096 RID: 150
	public AudioSource Source;

	// Token: 0x04000097 RID: 151
	internal float Volume;

	// Token: 0x04000098 RID: 152
	[Space]
	public float MaxDistance = 15f;

	// Token: 0x04000099 RID: 153
	public float MinDistance = 4f;

	// Token: 0x0400009A RID: 154
	[Space]
	public float OnScreenDistance = 10f;

	// Token: 0x0400009B RID: 155
	[Space]
	public float FadeSpeed = 1f;

	// Token: 0x0400009C RID: 156
	private float LowerMultiplier = 1f;

	// Token: 0x0400009D RID: 157
	private float LowerMultiplierTarget = 1f;

	// Token: 0x0400009E RID: 158
	private float LowerTimer;

	// Token: 0x0400009F RID: 159
	private Camera Camera;

	// Token: 0x040000A0 RID: 160
	public LayerMask Mask;

	// Token: 0x040000A1 RID: 161
	private bool RayResult;

	// Token: 0x040000A2 RID: 162
	private float RayTimer;

	// Token: 0x040000A3 RID: 163
	private MusicEnemyNear.Track CurrentTrack;

	// Token: 0x040000A4 RID: 164
	[Space]
	public MusicEnemyNear.Track[] Tracks;

	// Token: 0x020002FD RID: 765
	[Serializable]
	public class Track
	{
		// Token: 0x040027FE RID: 10238
		public AudioClip Clip;

		// Token: 0x040027FF RID: 10239
		[Range(0f, 1f)]
		public float Volume = 0.5f;
	}
}
