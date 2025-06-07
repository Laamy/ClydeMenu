using System;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class EnemyThinManAnim : MonoBehaviour
{
	// Token: 0x06000534 RID: 1332 RVA: 0x00033070 File Offset: 0x00031270
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
		this.randomOffsets = new float[6];
		for (int i = 0; i < this.randomOffsets.Length; i++)
		{
			this.randomOffsets[i] = Random.Range(0f, 6.2831855f);
		}
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x000330CC File Offset: 0x000312CC
	private void Update()
	{
		this.growLoop.PlayLoop(this.tentaclesParent.activeSelf, 5f, 5f, 1f);
		if (this.controller.tentacleLerp > 0f)
		{
			this.backLight.intensity = this.tentacleCurve.Evaluate(this.controller.tentacleLerp) * 4f;
		}
		else
		{
			this.backLight.intensity = 0f;
		}
		if (this.controller.tentacleLerp > 0f)
		{
			if (!this.tentaclesParent.activeSelf)
			{
				this.tentaclesParent.SetActive(true);
			}
			this.tentaclesParent.transform.localScale = new Vector3(this.tentacleCurve.Evaluate(this.controller.tentacleLerp), this.tentacleCurve.Evaluate(this.controller.tentacleLerp), this.tentacleCurve.Evaluate(this.controller.tentacleLerp));
		}
		else if (this.tentaclesParent.activeSelf)
		{
			this.tentaclesParent.SetActive(false);
		}
		if (this.controller.tentacleLerp > 0f)
		{
			float num = this.controller.tentacleLerp * 20f;
			float num2 = Mathf.Lerp(10f, 1f, this.controller.tentacleLerp);
			this.tentacleR1.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + this.randomOffsets[0]) * num, 0f, 0f);
			this.tentacleR2.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + this.randomOffsets[1]) * num, 0f, 0f);
			this.tentacleR3.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + this.randomOffsets[2]) * num, 0f, 0f);
			this.tentacleL1.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + this.randomOffsets[3]) * num, 0f, 0f);
			this.tentacleL2.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + this.randomOffsets[4]) * num, 0f, 0f);
			this.tentacleL3.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + this.randomOffsets[5]) * num, 0f, 0f);
		}
		if (this.controller.currentState == EnemyThinMan.State.TentacleExtend || this.controller.currentState == EnemyThinMan.State.Damage)
		{
			if (!this.extendedTentacles.activeSelf)
			{
				this.tentaclesParent.SetActive(false);
				this.extendedPouch.SetActive(true);
				this.particleSmoke.Play();
				this.attack.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.extendedTentacles.SetActive(true);
			}
			if (this.controller.playerTarget)
			{
				GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, this.controller.playerTarget.transform.position, 0.3f);
				GameDirector.instance.CameraImpact.ShakeDistance(10f, 3f, 8f, this.controller.playerTarget.transform.position, 0.1f);
				float z = Vector3.Distance(this.controller.playerTarget.transform.position, this.extendedTentacles.transform.position);
				this.extendedTentacles.transform.localScale = new Vector3(1f, 1f, z);
				this.extendedTentacles.transform.LookAt(this.controller.playerTarget.PlayerVisionTarget.VisionTransform.position);
			}
		}
		else if (this.extendedTentacles.activeSelf)
		{
			Vector3 localScale = this.extendedTentacles.transform.localScale;
			localScale.z = Mathf.Lerp(localScale.z, 0f, 10f * Time.deltaTime);
			this.extendedTentacles.transform.localScale = localScale;
			if (this.extendedTentacles.transform.localScale.z <= 0.1f)
			{
				this.extendedTentacles.SetActive(false);
				this.extendedPouch.SetActive(false);
			}
		}
		if (this.rattleImpulse)
		{
			if (this.enemy.Health.healthCurrent > 0)
			{
				int num3 = Random.Range(1, 3);
				this.animator.SetTrigger("Rattle" + num3.ToString());
			}
			this.rattleImpulse = false;
		}
		if (this.enemy.CurrentState == EnemyState.Despawn && this.enemy.Health.healthCurrent > 0)
		{
			this.animator.SetBool("Despawn", true);
			if (this.despawnImpulse)
			{
				this.animator.SetTrigger("DespawnTrigger");
				this.despawnImpulse = false;
			}
		}
		else
		{
			this.animator.SetBool("Despawn", false);
			this.despawnImpulse = true;
		}
		if (this.enemy.IsStunned() && this.enemy.CurrentState != EnemyState.Despawn && this.enemy.Health.healthCurrent > 0)
		{
			this.animator.SetBool("Stun", true);
			if (this.stunImpulse)
			{
				this.animator.SetTrigger("StunTrigger");
				this.stunImpulse = false;
				return;
			}
		}
		else
		{
			this.animator.SetBool("Stun", false);
			this.stunImpulse = true;
		}
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x00033680 File Offset: 0x00031880
	public void NoticeSet()
	{
		if (this.enemy.Health.healthCurrent < 0)
		{
			return;
		}
		if (this.controller.playerTarget.isLocal)
		{
			float num = 30f;
			if (Vector3.Distance(this.controller.playerTarget.transform.position, this.enemy.transform.position) > 5f)
			{
				num = 20f;
			}
			CameraGlitch.Instance.PlayShort();
			CameraAim.Instance.AimTargetSet(this.controller.head.transform.position, 0.75f, 2f, this.controller.gameObject, 90);
			CameraZoom.Instance.OverrideZoomSet(num, 0.75f, 3f, 1f, this.controller.gameObject, 50);
			this.zoom.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		this.animator.SetTrigger("Scream");
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x00033795 File Offset: 0x00031995
	public void OnDeath()
	{
		this.animator.SetTrigger("Death");
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x000337A8 File Offset: 0x000319A8
	public void Scream()
	{
		this.screamLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.screamGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x0003380C File Offset: 0x00031A0C
	public void DeathEffect()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.particleImpact.Play();
		Quaternion rotation = Quaternion.LookRotation(-this.enemy.Health.hurtDirection.normalized);
		this.particleDirectionalBits.transform.rotation = rotation;
		this.particleDirectionalBits.Play();
		this.deathSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x000338F7 File Offset: 0x00031AF7
	public void SetDespawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x0003390C File Offset: 0x00031B0C
	public void DespawnSmoke()
	{
		this.controller.SmokeEffect(this.controller.rb.position);
		this.teleportOut.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0400084E RID: 2126
	internal Animator animator;

	// Token: 0x0400084F RID: 2127
	public EnemyThinMan controller;

	// Token: 0x04000850 RID: 2128
	public Enemy enemy;

	// Token: 0x04000851 RID: 2129
	public GameObject mesh;

	// Token: 0x04000852 RID: 2130
	public Light backLight;

	// Token: 0x04000853 RID: 2131
	public GameObject tentaclesParent;

	// Token: 0x04000854 RID: 2132
	public GameObject extendedTentacles;

	// Token: 0x04000855 RID: 2133
	public GameObject extendedPouch;

	// Token: 0x04000856 RID: 2134
	public float tentacleSpeed;

	// Token: 0x04000857 RID: 2135
	public AnimationCurve tentacleCurve;

	// Token: 0x04000858 RID: 2136
	private float[] randomOffsets;

	// Token: 0x04000859 RID: 2137
	private bool tentacleBackActive;

	// Token: 0x0400085A RID: 2138
	public GameObject tentacleR1;

	// Token: 0x0400085B RID: 2139
	public GameObject tentacleR2;

	// Token: 0x0400085C RID: 2140
	public GameObject tentacleR3;

	// Token: 0x0400085D RID: 2141
	public GameObject tentacleL1;

	// Token: 0x0400085E RID: 2142
	public GameObject tentacleL2;

	// Token: 0x0400085F RID: 2143
	public GameObject tentacleL3;

	// Token: 0x04000860 RID: 2144
	internal bool rattleImpulse;

	// Token: 0x04000861 RID: 2145
	public ParticleSystem particleSmoke;

	// Token: 0x04000862 RID: 2146
	public ParticleSystem particleSmokeCalmFill;

	// Token: 0x04000863 RID: 2147
	public ParticleSystem particleImpact;

	// Token: 0x04000864 RID: 2148
	public ParticleSystem particleDirectionalBits;

	// Token: 0x04000865 RID: 2149
	[Space]
	public Sound teleportIn;

	// Token: 0x04000866 RID: 2150
	public Sound teleportOut;

	// Token: 0x04000867 RID: 2151
	[Space]
	public Sound notice;

	// Token: 0x04000868 RID: 2152
	[Space]
	public Sound growLoop;

	// Token: 0x04000869 RID: 2153
	public Sound zoom;

	// Token: 0x0400086A RID: 2154
	public Sound attack;

	// Token: 0x0400086B RID: 2155
	public Sound screamLocal;

	// Token: 0x0400086C RID: 2156
	public Sound screamGlobal;

	// Token: 0x0400086D RID: 2157
	[Space]
	public Sound hurtSound;

	// Token: 0x0400086E RID: 2158
	public Sound deathSound;

	// Token: 0x0400086F RID: 2159
	private bool despawnImpulse;

	// Token: 0x04000870 RID: 2160
	private bool stunImpulse;
}
