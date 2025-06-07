using System;
using UnityEngine;

// Token: 0x020000CF RID: 207
public class VacuumCleaner : MonoBehaviour
{
	// Token: 0x06000754 RID: 1876 RVA: 0x00045E20 File Offset: 0x00044020
	private void Start()
	{
		this.IntroSound.Play(this.FollowTransform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(3f, 0.25f);
		this.SuckingTimer = 1f;
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x00045E7C File Offset: 0x0004407C
	private void Update()
	{
		if (ToolController.instance.Interact)
		{
			Interaction activeInteraction = ToolController.instance.ActiveInteraction;
			if (activeInteraction)
			{
				VacuumSpotInteraction component = activeInteraction.GetComponent<VacuumSpotInteraction>();
				if (component && !this.DebugNoSuck)
				{
					component.VacuumSpot.cleanInput = true;
					this.LoopSuckSoundTimer = 0.1f;
					if (component.VacuumSpot.Amount > 0.2f)
					{
						this.SuckingTimer = Mathf.Max(this.SuckingTimer, this.SuckingTime);
					}
					else if (!component.VacuumSpot.CleanDone)
					{
						component.VacuumSpot.CleanDone = true;
					}
				}
			}
		}
		if (this.LoopSuckSoundTimer > 0f)
		{
			this.LoopSuckSoundTimer -= Time.deltaTime;
			this.LoopSuckSound.PlayLoop(true, 5f, 5f, 1f);
		}
		else
		{
			this.LoopSuckSound.PlayLoop(false, 5f, 5f, 1f);
		}
		if (this.SuckingTimer > 0f)
		{
			this.SuckingTimer -= Time.deltaTime;
			if (!this.Sucking)
			{
				GameDirector.instance.CameraShake.Shake(3f, 0.25f);
				this.Sucking = true;
				this.VacuumCleanerBag.Active = true;
				this.SuckingOffset.Active = true;
				if (this.OutroAudioPlay)
				{
					this.ParticleSystem.Play();
				}
				this.LoopStartSound.Play(this.FollowTransform.position, 1f, 1f, 1f, 1f);
			}
			GameDirector.instance.CameraShake.Shake(1f, 0.25f);
			this.SuckNoise.noiseStrengthDefault = Mathf.Lerp(this.SuckNoise.noiseStrengthDefault, this.SuckNoiseAmount, 5f * Time.deltaTime);
		}
		else
		{
			if (this.Sucking)
			{
				GameDirector.instance.CameraShake.Shake(3f, 0.25f);
				this.LoopStopSound.Play(this.FollowTransform.position, 1f, 1f, 1f, 1f);
				this.VacuumCleanerBag.Active = false;
				this.SuckingOffset.Active = false;
				if (this.OutroAudioPlay)
				{
					this.ParticleSystem.Stop();
				}
				this.Sucking = false;
			}
			this.SuckNoise.noiseStrengthDefault = Mathf.Lerp(this.SuckNoise.noiseStrengthDefault, 0f, 5f * Time.deltaTime);
		}
		this.LoopSound.PlayLoop(this.Sucking, 0.5f, 0.5f, 1f);
		if (this.OutroAudioPlay && !ToolController.instance.ToolHide.Active)
		{
			if (this.ParticleSystem != null && this.ParticleSystem.isPlaying)
			{
				this.ParticleSystem.gameObject.transform.parent = null;
				this.ParticleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
				this.ParticleSystem.Stop();
				this.ParticleSystem = null;
			}
			this.OutroSound.Play(this.FollowTransform.position, 1f, 1f, 1f, 1f);
			this.OutroAudioPlay = false;
			GameDirector.instance.CameraShake.Shake(3f, 0.25f);
		}
		this.FollowTransform.position = ToolController.instance.ToolFollow.transform.position;
		this.FollowTransform.rotation = ToolController.instance.ToolFollow.transform.rotation;
		this.FollowTransform.localScale = ToolController.instance.ToolHide.transform.localScale;
		this.ParentTransform.transform.position = ToolController.instance.ToolTargetParent.transform.position;
		this.ParentTransform.transform.rotation = ToolController.instance.ToolTargetParent.transform.rotation;
	}

	// Token: 0x04000CD6 RID: 3286
	public bool DebugNoSuck;

	// Token: 0x04000CD7 RID: 3287
	[Space]
	public float SuckingTime;

	// Token: 0x04000CD8 RID: 3288
	private float SuckingTimer;

	// Token: 0x04000CD9 RID: 3289
	private bool Sucking;

	// Token: 0x04000CDA RID: 3290
	[Space]
	public ToolActiveOffset SuckingOffset;

	// Token: 0x04000CDB RID: 3291
	public VacuumCleanerBag VacuumCleanerBag;

	// Token: 0x04000CDC RID: 3292
	public ParticleSystem ParticleSystem;

	// Token: 0x04000CDD RID: 3293
	public Transform FollowTransform;

	// Token: 0x04000CDE RID: 3294
	public Transform ParentTransform;

	// Token: 0x04000CDF RID: 3295
	[Space]
	public AnimNoise SuckNoise;

	// Token: 0x04000CE0 RID: 3296
	public float SuckNoiseAmount;

	// Token: 0x04000CE1 RID: 3297
	[Space]
	public Sound IntroSound;

	// Token: 0x04000CE2 RID: 3298
	public Sound OutroSound;

	// Token: 0x04000CE3 RID: 3299
	public Sound LoopSound;

	// Token: 0x04000CE4 RID: 3300
	public Sound LoopSuckSound;

	// Token: 0x04000CE5 RID: 3301
	private float LoopSuckSoundTimer;

	// Token: 0x04000CE6 RID: 3302
	public Sound LoopStartSound;

	// Token: 0x04000CE7 RID: 3303
	public Sound LoopStopSound;

	// Token: 0x04000CE8 RID: 3304
	private bool OutroAudioPlay = true;
}
