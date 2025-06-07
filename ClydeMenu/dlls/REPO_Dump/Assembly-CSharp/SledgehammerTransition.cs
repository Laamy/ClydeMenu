using System;
using UnityEngine;

// Token: 0x020000C8 RID: 200
public class SledgehammerTransition : MonoBehaviour
{
	// Token: 0x0600072E RID: 1838 RVA: 0x00044790 File Offset: 0x00042990
	public void IntroSet()
	{
		this.Intro = true;
		this.LerpAmount = 0f;
		this.PositionStart = this.SwingTarget.position;
		this.RotationStart = this.SwingTarget.rotation;
		this.ScaleStart = this.SwingTarget.localScale;
		base.transform.position = this.PositionStart;
		base.transform.rotation = this.RotationStart;
		base.transform.localScale = this.ScaleStart;
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00044818 File Offset: 0x00042A18
	public void OutroSet()
	{
		this.Intro = false;
		this.LerpAmount = 0f;
		this.PositionStart = this.HitTarget.position;
		this.RotationStart = this.HitTarget.rotation;
		this.ScaleStart = this.HitTarget.localScale;
		base.transform.position = this.PositionStart;
		base.transform.rotation = this.RotationStart;
		base.transform.localScale = this.ScaleStart;
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x000448A0 File Offset: 0x00042AA0
	public void Update()
	{
		if (this.LerpAmount < 1f)
		{
			if (this.Intro)
			{
				this.LerpAmount += this.IntroSpeed * Time.deltaTime;
				base.transform.position = Vector3.Lerp(this.PositionStart, this.HitTarget.position, this.IntroCurve.Evaluate(this.LerpAmount));
				base.transform.rotation = Quaternion.Lerp(this.RotationStart, this.HitTarget.rotation, this.IntroCurve.Evaluate(this.LerpAmount));
				base.transform.localScale = Vector3.Lerp(this.ScaleStart, this.HitTarget.localScale, this.IntroCurve.Evaluate(this.LerpAmount));
			}
			else
			{
				this.LerpAmount += this.OutroSpeed * Time.deltaTime;
				base.transform.position = Vector3.Lerp(this.PositionStart, this.SwingTarget.position, this.OutroCurve.Evaluate(this.LerpAmount));
				base.transform.rotation = Quaternion.Lerp(this.RotationStart, this.SwingTarget.rotation, this.OutroCurve.Evaluate(this.LerpAmount));
				base.transform.localScale = Vector3.Lerp(this.ScaleStart, this.SwingTarget.localScale, this.OutroCurve.Evaluate(this.LerpAmount));
			}
			if (this.LerpAmount >= 1f)
			{
				if (this.Intro)
				{
					this.Controller.IntroDone();
				}
				else
				{
					this.Controller.OutroDone();
				}
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x04000C74 RID: 3188
	public SledgehammerController Controller;

	// Token: 0x04000C75 RID: 3189
	private Vector3 PositionStart;

	// Token: 0x04000C76 RID: 3190
	private Quaternion RotationStart;

	// Token: 0x04000C77 RID: 3191
	private Vector3 ScaleStart;

	// Token: 0x04000C78 RID: 3192
	[Space]
	public Transform SwingTarget;

	// Token: 0x04000C79 RID: 3193
	public Transform HitTarget;

	// Token: 0x04000C7A RID: 3194
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x04000C7B RID: 3195
	public float IntroSpeed = 1f;

	// Token: 0x04000C7C RID: 3196
	[Space]
	public AnimationCurve OutroCurve;

	// Token: 0x04000C7D RID: 3197
	public float OutroSpeed = 1f;

	// Token: 0x04000C7E RID: 3198
	private float LerpAmount;

	// Token: 0x04000C7F RID: 3199
	private bool Intro;
}
