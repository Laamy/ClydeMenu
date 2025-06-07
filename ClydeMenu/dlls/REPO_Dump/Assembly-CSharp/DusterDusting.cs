using System;
using UnityEngine;

// Token: 0x020000C2 RID: 194
public class DusterDusting : MonoBehaviour
{
	// Token: 0x06000711 RID: 1809 RVA: 0x00042FA0 File Offset: 0x000411A0
	private void Update()
	{
		if (this.Active)
		{
			GameDirector.instance.CameraShake.Shake(1f, 0.25f);
			if (this.ActivePrev != this.Active)
			{
				this.Start.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.ActiveAmount += 2f * Time.deltaTime;
		}
		else
		{
			if (this.ActivePrev != this.Active)
			{
				this.Stop.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.ActiveAmount -= 1.5f * Time.deltaTime;
		}
		this.ActiveAmount = Mathf.Clamp01(this.ActiveAmount);
		this.ActivePrev = this.Active;
		this.Loop.PlayLoop(this.Active, 2f, 2f, 1f);
		if (this.ActiveAmount > 0f)
		{
			if (!this.ReverseZ)
			{
				this.LerpZ += this.SpeedZ * Time.deltaTime;
				if (this.LerpZ >= 1f)
				{
					this.ReverseZ = true;
				}
			}
			else
			{
				this.LerpZ -= this.SpeedZ * Time.deltaTime;
				if (this.LerpZ <= 0f)
				{
					this.ReverseZ = false;
				}
			}
		}
		if (this.ActiveAmount > 0f)
		{
			if (!this.ReverseX)
			{
				this.LerpX += this.SpeedX * Time.deltaTime;
				if (this.LerpX >= 1f)
				{
					this.ReverseX = true;
				}
			}
			else
			{
				this.LerpX -= this.SpeedX * Time.deltaTime;
				if (this.LerpX <= 0f)
				{
					this.ReverseX = false;
				}
			}
		}
		if (this.ActiveAmount > 0f)
		{
			base.transform.localRotation = Quaternion.Euler((this.Curve.Evaluate(this.LerpX) * this.AmountX - this.AmountX * 0.5f) * this.Curve.Evaluate(this.ActiveAmount), 0f, (this.Curve.Evaluate(this.LerpZ) * this.AmountZ - this.AmountX * 0.5f) * this.Curve.Evaluate(this.ActiveAmount));
			return;
		}
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	// Token: 0x04000C06 RID: 3078
	public bool Active;

	// Token: 0x04000C07 RID: 3079
	private bool ActivePrev;

	// Token: 0x04000C08 RID: 3080
	public float AmountZ;

	// Token: 0x04000C09 RID: 3081
	public float SpeedZ;

	// Token: 0x04000C0A RID: 3082
	private float LerpZ;

	// Token: 0x04000C0B RID: 3083
	private bool ReverseZ;

	// Token: 0x04000C0C RID: 3084
	[Space]
	public float AmountX;

	// Token: 0x04000C0D RID: 3085
	public float SpeedX;

	// Token: 0x04000C0E RID: 3086
	private float LerpX;

	// Token: 0x04000C0F RID: 3087
	private bool ReverseX;

	// Token: 0x04000C10 RID: 3088
	[Space]
	public AnimationCurve Curve;

	// Token: 0x04000C11 RID: 3089
	public float ActiveAmount;

	// Token: 0x04000C12 RID: 3090
	[Header("Sounds")]
	[Space]
	public Sound Start;

	// Token: 0x04000C13 RID: 3091
	public Sound Loop;

	// Token: 0x04000C14 RID: 3092
	public Sound Stop;
}
