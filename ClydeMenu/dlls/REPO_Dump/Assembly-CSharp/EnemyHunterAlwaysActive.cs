using System;
using UnityEngine;

// Token: 0x0200006C RID: 108
public class EnemyHunterAlwaysActive : MonoBehaviour
{
	// Token: 0x06000390 RID: 912 RVA: 0x00023DB1 File Offset: 0x00021FB1
	private void Start()
	{
		this.shootLightIntensity = this.shootLight.lightComponent.intensity;
		this.hitLightIntensity = this.hitLight.lightComponent.intensity;
	}

	// Token: 0x06000391 RID: 913 RVA: 0x00023DE0 File Offset: 0x00021FE0
	private void Update()
	{
		if (this.shootEffectActive)
		{
			this.shootLight.lightComponent.intensity -= Time.deltaTime * 20f;
			this.shootLight.originalIntensity = this.shootLightIntensity;
			if (this.shootLight.lightComponent.intensity <= 0f)
			{
				this.shootLight.lightComponent.enabled = false;
				this.shootEffectActive = false;
			}
		}
		if (this.hitEffectActive)
		{
			this.hitLight.lightComponent.intensity -= Time.deltaTime * 20f;
			this.hitLight.originalIntensity = this.hitLightIntensity;
			if (this.hitLight.lightComponent.intensity <= 0f)
			{
				this.hitLight.lightComponent.enabled = false;
				this.hitEffectActive = false;
			}
		}
	}

	// Token: 0x06000392 RID: 914 RVA: 0x00023EC4 File Offset: 0x000220C4
	public void Trigger()
	{
		this.shootEffectActive = true;
		this.hitEffectActive = true;
		this.shootLight.lightComponent.enabled = true;
		this.shootLight.lightComponent.intensity = this.shootLightIntensity;
		this.shootLight.originalIntensity = this.shootLightIntensity;
		this.hitLight.lightComponent.enabled = true;
		this.hitLight.lightComponent.intensity = this.hitLightIntensity;
		this.hitLight.originalIntensity = this.hitLightIntensity;
	}

	// Token: 0x04000639 RID: 1593
	private bool shootEffectActive;

	// Token: 0x0400063A RID: 1594
	public PropLight shootLight;

	// Token: 0x0400063B RID: 1595
	private float shootLightIntensity;

	// Token: 0x0400063C RID: 1596
	private bool hitEffectActive;

	// Token: 0x0400063D RID: 1597
	public PropLight hitLight;

	// Token: 0x0400063E RID: 1598
	private float hitLightIntensity;
}
