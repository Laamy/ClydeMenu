using System;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public class CleanEffectParticleSettings : MonoBehaviour
{
	// Token: 0x060006D3 RID: 1747 RVA: 0x00041A68 File Offset: 0x0003FC68
	private void Start()
	{
		this.UpdateParticleProperties();
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x00041A70 File Offset: 0x0003FC70
	private void UpdateParticleProperties()
	{
		float num = this.cleanEffectParticleSize.transform.localScale.x * 10f / 4f;
		float x = this.cleanEffectParticleRadius.transform.localScale.x;
		float num2 = this.cleanEffectParticleAmount.transform.localScale.x * 100f / 4f;
		ParticleSystem.MainModule main = this.gleamParticles.main;
		float startSizeMultiplier = main.startSizeMultiplier;
		main.startSizeMultiplier = startSizeMultiplier * num;
		this.gleamParticles.shape.radius = x / 2f;
		ParticleSystem.EmissionModule emission = this.gleamParticles.emission;
		float rateOverTimeMultiplier = emission.rateOverTimeMultiplier;
		emission.rateOverTimeMultiplier = rateOverTimeMultiplier * num2;
		Object.Destroy(this.cleanEffectParticleSize);
		Object.Destroy(this.cleanEffectParticleRadius);
		Object.Destroy(this.cleanEffectParticleAmount);
	}

	// Token: 0x04000B98 RID: 2968
	public ParticleSystem gleamParticles;

	// Token: 0x04000B99 RID: 2969
	public GameObject cleanEffectParticleRadius;

	// Token: 0x04000B9A RID: 2970
	public GameObject cleanEffectParticleSize;

	// Token: 0x04000B9B RID: 2971
	public GameObject cleanEffectParticleAmount;
}
