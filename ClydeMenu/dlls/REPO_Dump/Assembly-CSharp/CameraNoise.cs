using System;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class CameraNoise : MonoBehaviour
{
	// Token: 0x0600009D RID: 157 RVA: 0x000063F7 File Offset: 0x000045F7
	private void Awake()
	{
		CameraNoise.Instance = this;
		this.Strength = this.StrengthDefault;
	}

	// Token: 0x0600009E RID: 158 RVA: 0x0000640C File Offset: 0x0000460C
	private void Update()
	{
		if (this.OverrideTimer > 0f)
		{
			this.OverrideTimer -= Time.deltaTime;
			if (this.OverrideTimer <= 0f)
			{
				this.OverrideTimer = 0f;
			}
			else
			{
				this.Strength = Mathf.Lerp(this.Strength, this.OverrideStrength * GameplayManager.instance.cameraNoise, 5f * Time.deltaTime);
			}
			this.AnimNoise.noiseStrengthDefault = this.Strength;
		}
		else if (Mathf.Abs(this.Strength - this.StrengthDefault * GameplayManager.instance.cameraNoise) > 0.001f)
		{
			this.Strength = Mathf.Lerp(this.Strength, this.StrengthDefault * GameplayManager.instance.cameraNoise, 5f * Time.deltaTime);
		}
		this.AnimNoise.noiseStrengthDefault = this.Strength;
	}

	// Token: 0x0600009F RID: 159 RVA: 0x000064F4 File Offset: 0x000046F4
	public void Override(float strength, float time)
	{
		this.OverrideStrength = strength;
		this.OverrideTimer = time;
	}

	// Token: 0x0400018D RID: 397
	public static CameraNoise Instance;

	// Token: 0x0400018E RID: 398
	public float StrengthDefault = 0.2f;

	// Token: 0x0400018F RID: 399
	public AnimNoise AnimNoise;

	// Token: 0x04000190 RID: 400
	private float Strength = 1f;

	// Token: 0x04000191 RID: 401
	private float OverrideStrength = 1f;

	// Token: 0x04000192 RID: 402
	private float OverrideTimer;
}
