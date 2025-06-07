using System;
using UnityEngine;

// Token: 0x020002C5 RID: 709
public class PowerCrystalValuable : MonoBehaviour
{
	// Token: 0x0600164C RID: 5708 RVA: 0x000C515B File Offset: 0x000C335B
	private void Start()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
	}

	// Token: 0x0600164D RID: 5709 RVA: 0x000C516C File Offset: 0x000C336C
	private void Update()
	{
		if (this.GlowActive)
		{
			this.GlowLerp += 8f * Time.deltaTime;
		}
		if (this.GlowLerp >= 1f)
		{
			this.GlowLerp = 0f;
			this.GlowActive = false;
		}
		this.CrystalLight.lightComponent.intensity = 3f + this.GlowCurve.Evaluate(this.GlowLerp) * this.GlowIntensity;
	}

	// Token: 0x0600164E RID: 5710 RVA: 0x000C51E8 File Offset: 0x000C33E8
	public void Explode()
	{
		SemiFunc.LightRemove(this.CrystalLight);
		this.particleScriptExplosion.Spawn(this.Center.position, 1f, 50, 50, 1f, false, false, 1f);
	}

	// Token: 0x0600164F RID: 5711 RVA: 0x000C522C File Offset: 0x000C342C
	public void GlowDim()
	{
		this.GlowIntensity = 3f;
		this.GlowActive = true;
	}

	// Token: 0x06001650 RID: 5712 RVA: 0x000C5240 File Offset: 0x000C3440
	public void GlowMed()
	{
		this.GlowIntensity = 5f;
		this.GlowActive = true;
	}

	// Token: 0x06001651 RID: 5713 RVA: 0x000C5254 File Offset: 0x000C3454
	public void GlowStrong()
	{
		this.GlowIntensity = 10f;
		this.GlowActive = true;
	}

	// Token: 0x0400268B RID: 9867
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x0400268C RID: 9868
	public Transform Center;

	// Token: 0x0400268D RID: 9869
	public GameObject Crystal;

	// Token: 0x0400268E RID: 9870
	public AnimationCurve GlowCurve;

	// Token: 0x0400268F RID: 9871
	public PropLight CrystalLight;

	// Token: 0x04002690 RID: 9872
	private bool GlowActive;

	// Token: 0x04002691 RID: 9873
	private float GlowLerp;

	// Token: 0x04002692 RID: 9874
	private float GlowIntensity;
}
