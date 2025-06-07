using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class AnimatedOffset : MonoBehaviour
{
	// Token: 0x06000926 RID: 2342 RVA: 0x000579ED File Offset: 0x00055BED
	public void Active(float time)
	{
		this.ActiveTimer = time;
		if (!this.Animating)
		{
			this.Animating = true;
			base.StartCoroutine(this.Animate());
		}
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x00057A12 File Offset: 0x00055C12
	private IEnumerator Animate()
	{
		while (this.Animating)
		{
			if (this.ActiveTimer > 0f)
			{
				if (this.IntroLerp == 0f)
				{
					this.IntroSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				this.IntroLerp = Mathf.Clamp01(this.IntroLerp + this.IntroSpeed * Time.deltaTime);
				base.transform.localPosition = Vector3.Lerp(Vector3.zero, this.PositionOffset, this.IntroCurve.Evaluate(this.IntroLerp));
				base.transform.localRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(this.RotationOffset), this.IntroCurve.Evaluate(this.IntroLerp));
				if (this.IntroLerp >= 1f)
				{
					this.ActiveTimer -= Time.deltaTime;
				}
				this.OutroLerp = 0f;
			}
			else
			{
				if (this.OutroLerp == 0f)
				{
					this.OutroSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				this.OutroLerp = Mathf.Clamp01(this.OutroLerp + this.OutroSpeed * Time.deltaTime);
				base.transform.localPosition = Vector3.Lerp(this.PositionOffset, Vector3.zero, this.OutroCurve.Evaluate(this.OutroLerp));
				base.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(this.RotationOffset), Quaternion.identity, this.OutroCurve.Evaluate(this.OutroLerp));
				if (this.OutroLerp >= 1f)
				{
					this.Animating = false;
				}
				this.IntroLerp = 0f;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x040010A9 RID: 4265
	internal bool Animating;

	// Token: 0x040010AA RID: 4266
	private float ActiveTimer;

	// Token: 0x040010AB RID: 4267
	public Vector3 PositionOffset;

	// Token: 0x040010AC RID: 4268
	public Vector3 RotationOffset;

	// Token: 0x040010AD RID: 4269
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x040010AE RID: 4270
	public float IntroSpeed;

	// Token: 0x040010AF RID: 4271
	public Sound IntroSound;

	// Token: 0x040010B0 RID: 4272
	private float IntroLerp;

	// Token: 0x040010B1 RID: 4273
	[Space]
	public AnimationCurve OutroCurve;

	// Token: 0x040010B2 RID: 4274
	public float OutroSpeed;

	// Token: 0x040010B3 RID: 4275
	public Sound OutroSound;

	// Token: 0x040010B4 RID: 4276
	private float OutroLerp;
}
