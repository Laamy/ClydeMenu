using System;
using TMPro;
using UnityEngine;

// Token: 0x0200029D RID: 669
public class WorldSpaceUIValueLost : WorldSpaceUIChild
{
	// Token: 0x060014E6 RID: 5350 RVA: 0x000B8F04 File Offset: 0x000B7104
	protected override void Start()
	{
		base.Start();
		this.shakeXAmount = 0.005f;
		this.shakeYAmount = 0.005f;
		this.timer = 3f;
		this.text = base.GetComponent<TextMeshProUGUI>();
		this.textColor = this.text.color;
		this.text.color = Color.white;
		this.text.text = "-$" + SemiFunc.DollarGetString(this.value);
		this.scale = base.transform.localScale;
		if (this.value < 1000)
		{
			this.scale *= 0.75f;
			base.transform.localScale = this.scale;
		}
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x000B8FCC File Offset: 0x000B71CC
	protected override void Update()
	{
		base.Update();
		if (this.text.color != this.textColor)
		{
			this.flashTimer -= Time.deltaTime;
			if (this.flashTimer <= 0f && this.text.color != this.textColor)
			{
				this.text.color = Color.Lerp(this.text.color, this.textColor, 20f * Time.deltaTime);
				this.shakeX = Mathf.Lerp(this.shakeX, 0f, 20f * Time.deltaTime);
				this.shakeY = Mathf.Lerp(this.shakeY, 0f, 20f * Time.deltaTime);
			}
			else
			{
				if (this.shakeTimerX <= 0f)
				{
					this.shakeXTarget = Random.Range(-this.shakeXAmount, this.shakeXAmount);
					this.shakeTimerX = Random.Range(0.008f, 0.015f);
				}
				else
				{
					this.shakeTimerX -= Time.deltaTime;
					this.shakeX = Mathf.Lerp(this.shakeX, this.shakeXTarget, 50f * Time.deltaTime);
				}
				if (this.shakeTimerX <= 0f)
				{
					this.shakeYTarget = Random.Range(-this.shakeYAmount, this.shakeYAmount);
					this.shakeTimerX = Random.Range(0.008f, 0.015f);
				}
				else
				{
					this.shakeTimerX -= Time.deltaTime;
					this.shakeY = Mathf.Lerp(this.shakeY, this.shakeYTarget, 50f * Time.deltaTime);
				}
			}
		}
		this.floatY += 0.02f * Time.deltaTime;
		this.positionOffset = new Vector3(this.shakeX, this.shakeY + this.floatY, 0f);
		this.timer -= Time.deltaTime;
		if (this.timer > 0f)
		{
			this.curveLerp += 10f * Time.deltaTime;
			this.curveLerp = Mathf.Clamp01(this.curveLerp);
			base.transform.localScale = this.scale * this.curveIntro.Evaluate(this.curveLerp);
			return;
		}
		this.curveLerp -= 5f * Time.deltaTime;
		this.curveLerp = Mathf.Clamp01(this.curveLerp);
		base.transform.localScale = this.scale * this.curveOutro.Evaluate(this.curveLerp);
		if (this.curveLerp <= 0f)
		{
			WorldSpaceUIParent.instance.valueLostList.Remove(this);
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04002418 RID: 9240
	internal float timer;

	// Token: 0x04002419 RID: 9241
	private float flashTimer = 0.2f;

	// Token: 0x0400241A RID: 9242
	private Vector3 scale;

	// Token: 0x0400241B RID: 9243
	private TextMeshProUGUI text;

	// Token: 0x0400241C RID: 9244
	private Color textColor;

	// Token: 0x0400241D RID: 9245
	private float shakeXAmount;

	// Token: 0x0400241E RID: 9246
	private float shakeYAmount;

	// Token: 0x0400241F RID: 9247
	private float floatY;

	// Token: 0x04002420 RID: 9248
	private float shakeTimerX;

	// Token: 0x04002421 RID: 9249
	private float shakeXTarget;

	// Token: 0x04002422 RID: 9250
	private float shakeX;

	// Token: 0x04002423 RID: 9251
	private float shakeTimerY;

	// Token: 0x04002424 RID: 9252
	private float shakeYTarget;

	// Token: 0x04002425 RID: 9253
	private float shakeY;

	// Token: 0x04002426 RID: 9254
	public AnimationCurve curveIntro;

	// Token: 0x04002427 RID: 9255
	public AnimationCurve curveOutro;

	// Token: 0x04002428 RID: 9256
	private float curveLerp;

	// Token: 0x04002429 RID: 9257
	internal int value;
}
