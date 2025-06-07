using System;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public class PlayerDeathEffects : MonoBehaviour
{
	// Token: 0x06000FB2 RID: 4018 RVA: 0x0008E195 File Offset: 0x0008C395
	private void Start()
	{
		this.deathLightIntensityDefault = this.deathLight.intensity;
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x0008E1A8 File Offset: 0x0008C3A8
	private void Update()
	{
		base.transform.position = this.followTransform.position;
		base.transform.rotation = this.followTransform.rotation;
		if (this.triggered)
		{
			this.deathLight.intensity = Mathf.Lerp(this.deathLight.intensity, 0f, Time.deltaTime * 1f);
			if (this.smokeParticles.isStopped && this.bitWeakParticles.isStopped && this.bitStrongParticles.isStopped && this.deathLight.intensity < 0.01f)
			{
				base.gameObject.SetActive(false);
			}
			if (this.hurtColliderTimer > 0f)
			{
				this.hurtColliderTimer -= Time.deltaTime;
				if (this.hurtColliderTimer <= 0f)
				{
					this.hurtCollider.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x0008E29C File Offset: 0x0008C49C
	public void Trigger()
	{
		this.bitWeakParticles.main.startColor = this.playerAvatarVisuals.color;
		this.bitStrongParticles.main.startColor = this.playerAvatarVisuals.color;
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.triggered = true;
		this.enableTransform.gameObject.SetActive(true);
		this.hurtCollider.gameObject.SetActive(true);
		this.hurtColliderTimer = this.hurtColliderTime;
		this.deathSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.smokeParticles.gameObject.SetActive(true);
		this.smokeParticles.Play();
		this.fireParticles.gameObject.SetActive(true);
		this.fireParticles.Play();
		this.bitWeakParticles.gameObject.SetActive(true);
		this.bitWeakParticles.Play();
		this.bitStrongParticles.gameObject.SetActive(true);
		this.bitStrongParticles.Play();
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x0008E41B File Offset: 0x0008C61B
	public void Reset()
	{
		base.gameObject.SetActive(true);
		this.triggered = false;
		this.deathLight.intensity = this.deathLightIntensityDefault;
		this.enableTransform.gameObject.SetActive(false);
	}

	// Token: 0x04001A55 RID: 6741
	private bool triggered;

	// Token: 0x04001A56 RID: 6742
	public PlayerAvatarVisuals playerAvatarVisuals;

	// Token: 0x04001A57 RID: 6743
	[Space]
	public Transform followTransform;

	// Token: 0x04001A58 RID: 6744
	public Transform enableTransform;

	// Token: 0x04001A59 RID: 6745
	[Space]
	public Light deathLight;

	// Token: 0x04001A5A RID: 6746
	private float deathLightIntensityDefault;

	// Token: 0x04001A5B RID: 6747
	public ParticleSystem smokeParticles;

	// Token: 0x04001A5C RID: 6748
	public ParticleSystem fireParticles;

	// Token: 0x04001A5D RID: 6749
	public ParticleSystem bitWeakParticles;

	// Token: 0x04001A5E RID: 6750
	public ParticleSystem bitStrongParticles;

	// Token: 0x04001A5F RID: 6751
	public HurtCollider hurtCollider;

	// Token: 0x04001A60 RID: 6752
	private float hurtColliderTime = 0.5f;

	// Token: 0x04001A61 RID: 6753
	private float hurtColliderTimer;

	// Token: 0x04001A62 RID: 6754
	[Space]
	public Sound deathSound;
}
