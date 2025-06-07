using System;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class LightFlickerer : MonoBehaviour
{
	// Token: 0x06000847 RID: 2119 RVA: 0x00050B81 File Offset: 0x0004ED81
	private void Start()
	{
		this.lightComponent = base.GetComponent<Light>();
		this.intensity = this.lightComponent.intensity;
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x00050BA0 File Offset: 0x0004EDA0
	private void Update()
	{
		if (this.timer <= 0f)
		{
			this.timer = Random.Range(0.05f, 0.2f);
			this.intensityTarget = Random.Range(0.75f, 1f);
		}
		else
		{
			this.timer -= Time.deltaTime;
		}
		this.lightComponent.intensity = Mathf.Lerp(this.lightComponent.intensity, this.intensity * this.intensityTarget, Time.deltaTime * 30f);
	}

	// Token: 0x04000F4C RID: 3916
	private Light lightComponent;

	// Token: 0x04000F4D RID: 3917
	private float intensityTarget;

	// Token: 0x04000F4E RID: 3918
	private float timer;

	// Token: 0x04000F4F RID: 3919
	private float intensity;
}
