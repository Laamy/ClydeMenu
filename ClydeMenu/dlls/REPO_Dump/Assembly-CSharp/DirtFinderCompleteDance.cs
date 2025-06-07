using System;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class DirtFinderCompleteDance : MonoBehaviour
{
	// Token: 0x06000D73 RID: 3443 RVA: 0x00075F68 File Offset: 0x00074168
	private void Update()
	{
		if (this.Active)
		{
			this.IntroLerp += this.IntroSpeed * Time.deltaTime;
			this.IntroLerp = Mathf.Clamp01(this.IntroLerp);
			if (this.DanceLerpX >= 1f)
			{
				this.DanceLerpX = 0f;
			}
			this.DanceLerpX += this.DanceSpeedX * Time.deltaTime;
			float num = this.DanceCurveX.Evaluate(this.DanceLerpX) * this.IntroCurve.Evaluate(this.IntroLerp);
			if (this.DanceLerpY >= 1f)
			{
				this.DanceLerpY = 0f;
			}
			this.DanceLerpY += this.DanceSpeedY * Time.deltaTime;
			float num2 = this.DanceCurveY.Evaluate(this.DanceLerpY) * this.IntroCurve.Evaluate(this.IntroLerp);
			if (this.DanceLerpZ >= 1f)
			{
				this.DanceLerpZ = 0f;
			}
			this.DanceLerpZ += this.DanceSpeedZ * Time.deltaTime;
			float num3 = this.DanceCurveZ.Evaluate(this.DanceLerpZ) * this.IntroCurve.Evaluate(this.IntroLerp);
			base.transform.localRotation = Quaternion.Euler(this.DanceAmountX * num, this.DanceAmountY * num2, this.DanceAmountZ * num3);
		}
	}

	// Token: 0x04001579 RID: 5497
	public bool Active;

	// Token: 0x0400157A RID: 5498
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x0400157B RID: 5499
	public float IntroSpeed;

	// Token: 0x0400157C RID: 5500
	private float IntroLerp;

	// Token: 0x0400157D RID: 5501
	[Space]
	public AnimationCurve DanceCurveX;

	// Token: 0x0400157E RID: 5502
	public float DanceSpeedX;

	// Token: 0x0400157F RID: 5503
	public float DanceAmountX;

	// Token: 0x04001580 RID: 5504
	private float DanceLerpX;

	// Token: 0x04001581 RID: 5505
	[Space]
	public AnimationCurve DanceCurveY;

	// Token: 0x04001582 RID: 5506
	public float DanceSpeedY;

	// Token: 0x04001583 RID: 5507
	public float DanceAmountY;

	// Token: 0x04001584 RID: 5508
	private float DanceLerpY;

	// Token: 0x04001585 RID: 5509
	[Space]
	public AnimationCurve DanceCurveZ;

	// Token: 0x04001586 RID: 5510
	public float DanceSpeedZ;

	// Token: 0x04001587 RID: 5511
	public float DanceAmountZ;

	// Token: 0x04001588 RID: 5512
	private float DanceLerpZ;
}
