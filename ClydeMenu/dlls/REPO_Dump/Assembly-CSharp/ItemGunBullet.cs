using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class ItemGunBullet : MonoBehaviour
{
	// Token: 0x06000C9D RID: 3229 RVA: 0x0006F6B4 File Offset: 0x0006D8B4
	public void ActivateAll()
	{
		base.gameObject.SetActive(true);
		this.hitEffectTransform = base.transform.Find("Hit Effect");
		this.particleSparks = this.hitEffectTransform.Find("Particle Sparks").GetComponent<ParticleSystem>();
		this.particleSmoke = this.hitEffectTransform.Find("Particle Smoke").GetComponent<ParticleSystem>();
		this.particleImpact = this.hitEffectTransform.Find("Particle Impact").GetComponent<ParticleSystem>();
		this.hitLight = this.hitEffectTransform.Find("Hit Light").GetComponent<Light>();
		this.shootLine = base.GetComponentInChildren<LineRenderer>();
		Vector3 position = base.transform.position;
		Vector3 forward = this.hitPosition - position;
		this.shootLine.enabled = true;
		this.shootLine.SetPosition(0, base.transform.position);
		this.shootLine.SetPosition(1, base.transform.position + forward.normalized * 0.5f);
		this.shootLine.SetPosition(2, this.hitPosition - forward.normalized * 0.5f);
		this.shootLine.SetPosition(3, this.hitPosition);
		this.shootLineActive = true;
		this.shootLineLerp = 0f;
		if (this.bulletHit)
		{
			this.hitEffectTransform.gameObject.SetActive(true);
			this.particleSparks.gameObject.SetActive(true);
			this.particleSmoke.gameObject.SetActive(true);
			this.particleImpact.gameObject.SetActive(true);
			this.hitLight.enabled = true;
			GameObject gameObject = this.hitGameObject;
			if (this.hasHurtCollider)
			{
				gameObject = this.hurtCollider.gameObject;
			}
			gameObject.gameObject.SetActive(true);
			Quaternion rotation = Quaternion.LookRotation(forward);
			gameObject.transform.rotation = rotation;
			gameObject.transform.position = this.hitPosition;
			this.hitEffectTransform.position = this.hitPosition;
			this.hitEffectTransform.rotation = rotation;
			if (this.hasExtraParticles)
			{
				this.extraParticles.SetActive(true);
				this.extraParticles.transform.position = this.hitPosition;
				this.extraParticles.transform.rotation = rotation;
			}
		}
		base.StartCoroutine(this.BulletDestroy());
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x0006F91B File Offset: 0x0006DB1B
	private IEnumerator BulletDestroy()
	{
		yield return new WaitForSeconds(0.2f);
		while (this.particleSparks.isPlaying || this.particleSmoke.isPlaying || this.particleImpact.isPlaying || this.hitLight.enabled || this.shootLine.enabled || (this.hasHurtCollider && this.hurtCollider && this.hurtCollider.gameObject.activeSelf) || (!this.hasHurtCollider && this.hitGameObject && this.hitGameObject.activeSelf))
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x0006F92C File Offset: 0x0006DB2C
	private void LineRendererLogic()
	{
		if (this.shootLineActive)
		{
			this.shootLine.widthMultiplier = this.shootLineWidthCurve.Evaluate(this.shootLineLerp);
			this.shootLineLerp += Time.deltaTime * 5f;
			if (this.shootLineLerp >= 1f)
			{
				this.shootLine.enabled = false;
				this.shootLine.gameObject.SetActive(false);
				this.shootLineActive = false;
			}
		}
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x0006F9A8 File Offset: 0x0006DBA8
	private void Update()
	{
		this.LineRendererLogic();
		if (this.bulletHit)
		{
			if (this.hasHurtCollider)
			{
				if (this.hurtColliderTimer > 0f)
				{
					this.hurtColliderTimer -= Time.deltaTime;
					if (this.hurtCollider)
					{
						this.hurtCollider.gameObject.SetActive(true);
					}
				}
				else if (this.hurtCollider)
				{
					this.hurtCollider.gameObject.SetActive(false);
				}
			}
			else
			{
				this.hitGameObjectDestroyTime -= Time.deltaTime;
				if (this.hitGameObjectDestroyTime <= 0f && this.hitGameObject)
				{
					this.hitGameObject.SetActive(false);
				}
			}
			if (this.hitLight)
			{
				this.hitLight.intensity = Mathf.Lerp(this.hitLight.intensity, 0f, Time.deltaTime * 10f);
				if (this.hitLight.intensity < 0.01f)
				{
					this.hitLight.enabled = false;
				}
			}
		}
	}

	// Token: 0x04001444 RID: 5188
	private Transform hitEffectTransform;

	// Token: 0x04001445 RID: 5189
	private ParticleSystem particleSparks;

	// Token: 0x04001446 RID: 5190
	private ParticleSystem particleSmoke;

	// Token: 0x04001447 RID: 5191
	private ParticleSystem particleImpact;

	// Token: 0x04001448 RID: 5192
	private Light hitLight;

	// Token: 0x04001449 RID: 5193
	private LineRenderer shootLine;

	// Token: 0x0400144A RID: 5194
	public bool hasHurtCollider = true;

	// Token: 0x0400144B RID: 5195
	public HurtCollider hurtCollider;

	// Token: 0x0400144C RID: 5196
	internal bool bulletHit;

	// Token: 0x0400144D RID: 5197
	internal Vector3 hitPosition;

	// Token: 0x0400144E RID: 5198
	public float hurtColliderTimer = 0.25f;

	// Token: 0x0400144F RID: 5199
	private bool shootLineActive;

	// Token: 0x04001450 RID: 5200
	private float shootLineLerp;

	// Token: 0x04001451 RID: 5201
	internal AnimationCurve shootLineWidthCurve;

	// Token: 0x04001452 RID: 5202
	public GameObject hitGameObject;

	// Token: 0x04001453 RID: 5203
	public float hitGameObjectDestroyTime = 2f;

	// Token: 0x04001454 RID: 5204
	public bool hasExtraParticles;

	// Token: 0x04001455 RID: 5205
	public GameObject extraParticles;
}
