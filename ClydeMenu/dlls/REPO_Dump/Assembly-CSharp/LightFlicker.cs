using System;
using UnityEngine;

// Token: 0x02000246 RID: 582
public class LightFlicker : MonoBehaviour
{
	// Token: 0x060012F5 RID: 4853 RVA: 0x000A9B71 File Offset: 0x000A7D71
	private void Start()
	{
		this.lightComp = base.GetComponent<Light>();
		this.intensityInit = this.lightComp.intensity;
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x000A9B90 File Offset: 0x000A7D90
	private void Update()
	{
		float num = Mathf.LerpUnclamped(this.intensityOld, this.intensityNew, this.intensityCurve.Evaluate(this.intensityLerp));
		this.intensityLerp += this.intensitySpeed * Time.deltaTime;
		if (this.intensityLerp >= 1f)
		{
			this.intensityOld = this.intensityNew;
			this.intensityNew = Random.Range(this.intensityAmountMin, this.intensityAmountMax);
			this.intensitySpeed = Random.Range(this.intensitySpeedMin, this.intensitySpeedMax);
			this.intensityLerp = 0f;
		}
		this.lightComp.intensity = this.intensityInit + num;
	}

	// Token: 0x04002039 RID: 8249
	private Light lightComp;

	// Token: 0x0400203A RID: 8250
	public AnimationCurve intensityCurve;

	// Token: 0x0400203B RID: 8251
	public float intensityAmountMin;

	// Token: 0x0400203C RID: 8252
	public float intensityAmountMax;

	// Token: 0x0400203D RID: 8253
	public float intensitySpeedMin;

	// Token: 0x0400203E RID: 8254
	public float intensitySpeedMax;

	// Token: 0x0400203F RID: 8255
	private float intensityLerp = 1f;

	// Token: 0x04002040 RID: 8256
	private float intensityNew;

	// Token: 0x04002041 RID: 8257
	private float intensityOld;

	// Token: 0x04002042 RID: 8258
	private float intensitySpeed;

	// Token: 0x04002043 RID: 8259
	private float intensityInit;
}
