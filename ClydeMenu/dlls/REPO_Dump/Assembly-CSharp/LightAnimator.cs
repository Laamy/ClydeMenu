using System;
using UnityEngine;

// Token: 0x02000245 RID: 581
public class LightAnimator : MonoBehaviour
{
	// Token: 0x060012F2 RID: 4850 RVA: 0x000A9A1C File Offset: 0x000A7C1C
	private void Start()
	{
		this.lightComponent = base.GetComponent<Light>();
		this.lightIntensityInit = this.lightComponent.intensity;
		if (this.lightActive)
		{
			this.lightIntensity = this.lightIntensityInit;
			this.lightComponent.intensity = this.lightIntensity;
			this.lightComponent.enabled = true;
			return;
		}
		this.lightIntensity = 0f;
		this.lightComponent.intensity = this.lightIntensity;
		this.lightComponent.enabled = false;
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x000A9AA0 File Offset: 0x000A7CA0
	private void Update()
	{
		if (this.lightActive)
		{
			if (!this.lightComponent.enabled)
			{
				this.lightComponent.enabled = true;
			}
			this.lightIntensity = Mathf.Clamp(this.lightIntensity + this.introSpeed * Time.deltaTime, 0f, this.lightIntensityInit);
			this.lightComponent.intensity = this.lightIntensity;
			return;
		}
		if (this.lightComponent.enabled)
		{
			this.lightIntensity = Mathf.Clamp(this.lightIntensity - this.outroSpeed * Time.deltaTime, 0f, this.lightIntensityInit);
			this.lightComponent.intensity = this.lightIntensity;
			if (this.lightIntensity <= 0f)
			{
				this.lightComponent.enabled = false;
			}
		}
	}

	// Token: 0x04002033 RID: 8243
	private Light lightComponent;

	// Token: 0x04002034 RID: 8244
	private float lightIntensityInit;

	// Token: 0x04002035 RID: 8245
	private float lightIntensity;

	// Token: 0x04002036 RID: 8246
	public bool lightActive;

	// Token: 0x04002037 RID: 8247
	public float introSpeed;

	// Token: 0x04002038 RID: 8248
	public float outroSpeed;
}
