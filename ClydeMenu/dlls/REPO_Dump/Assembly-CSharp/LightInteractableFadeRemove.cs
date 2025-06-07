using System;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public class LightInteractableFadeRemove : MonoBehaviour
{
	// Token: 0x060008C9 RID: 2249 RVA: 0x00054EE7 File Offset: 0x000530E7
	private void Start()
	{
		this.lightComponent = base.GetComponent<Light>();
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x00054EF8 File Offset: 0x000530F8
	private void Update()
	{
		if (this.isFading)
		{
			this.currentTime += Time.deltaTime;
			float time = this.currentTime / this.fadeDuration;
			this.lightComponent.intensity = this.fadeCurve.Evaluate(time) * this.lightComponent.intensity;
			if (this.currentTime >= this.fadeDuration)
			{
				Object.Destroy(this.lightComponent);
				Object.Destroy(this);
			}
		}
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x00054F6F File Offset: 0x0005316F
	public void StartFading()
	{
		this.isFading = true;
	}

	// Token: 0x0400100A RID: 4106
	public AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x0400100B RID: 4107
	public float fadeDuration = 2f;

	// Token: 0x0400100C RID: 4108
	private Light lightComponent;

	// Token: 0x0400100D RID: 4109
	private float currentTime;

	// Token: 0x0400100E RID: 4110
	private bool isFading;
}
