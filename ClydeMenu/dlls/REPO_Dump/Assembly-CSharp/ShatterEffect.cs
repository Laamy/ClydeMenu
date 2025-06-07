using System;
using UnityEngine;

// Token: 0x020001E9 RID: 489
public class ShatterEffect : MonoBehaviour
{
	// Token: 0x0600108A RID: 4234 RVA: 0x00098E28 File Offset: 0x00097028
	private void Start()
	{
		this.mainModule = this.partSystem.main;
		this.emissionModule = this.partSystem.emission;
		this.shapeModule = this.partSystem.shape;
		this.mainModuleSmoke = this.particleSystemSmoke.main;
		this.shapeModuleSmoke = this.particleSystemSmoke.shape;
		this.SetupParticleSystem();
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x00098E90 File Offset: 0x00097090
	private void SetupParticleSystem()
	{
		this.emissionModule.rateOverTimeMultiplier = this.partSystem.emission.rateOverTimeMultiplier * (this.particleAmountMultiplier / 100f);
		this.shapeModule.scale = this.ParticleEmissionBox.localScale;
		this.shapeModuleSmoke.scale = this.ParticleEmissionBox.localScale;
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x00098EF4 File Offset: 0x000970F4
	public void SpawnParticles(Vector3 direction)
	{
		this.partSystem.transform.rotation = Quaternion.LookRotation(direction);
		this.particleSystemSmoke.transform.rotation = Quaternion.LookRotation(direction);
		this.partSystem.transform.position = this.ParticleEmissionBox.position;
		this.shapeModule.scale = this.ParticleEmissionBox.localScale;
		this.mainModule.startColor = this.particleColors;
		this.partSystem.Play();
		this.partSystem.transform.SetParent(null);
		this.particleSystemSmoke.Play();
		this.particleSystemSmoke.transform.SetParent(null);
		this.ShatterSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04001C64 RID: 7268
	public ParticleSystem partSystem;

	// Token: 0x04001C65 RID: 7269
	public ParticleSystem particleSystemSmoke;

	// Token: 0x04001C66 RID: 7270
	[Range(0f, 100f)]
	public float particleAmountMultiplier = 50f;

	// Token: 0x04001C67 RID: 7271
	public Gradient particleColors;

	// Token: 0x04001C68 RID: 7272
	public Transform ParticleEmissionBox;

	// Token: 0x04001C69 RID: 7273
	[Space]
	public Sound ShatterSound;

	// Token: 0x04001C6A RID: 7274
	private ParticleSystem.MainModule mainModule;

	// Token: 0x04001C6B RID: 7275
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x04001C6C RID: 7276
	private ParticleSystem.ShapeModule shapeModule;

	// Token: 0x04001C6D RID: 7277
	private ParticleSystem.ShapeModule shapeModuleSmoke;

	// Token: 0x04001C6E RID: 7278
	private ParticleSystem.MainModule mainModuleSmoke;
}
