using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200025F RID: 607
public class BatteryBarEffect : MonoBehaviour
{
	// Token: 0x06001370 RID: 4976 RVA: 0x000AD5D0 File Offset: 0x000AB7D0
	private void Start()
	{
		this.parentSize = base.transform.parent.transform.localScale.x;
		this.barImage = base.GetComponent<RawImage>();
		if (this.barLossEffect)
		{
			this.barColor = Color.red;
		}
		RectTransform component = base.GetComponent<RectTransform>();
		RectTransform component2 = base.transform.parent.GetComponent<RectTransform>();
		component.sizeDelta = component2.sizeDelta;
	}

	// Token: 0x06001371 RID: 4977 RVA: 0x000AD640 File Offset: 0x000AB840
	private void Update()
	{
		if (this.barLossEffect)
		{
			float num = this.barAnimationCurve.Evaluate(this.curveProgress);
			float a = this.barFadeOutCurve.Evaluate(this.curveProgress);
			float t = this.whiteFlashCurve.Evaluate(this.curveProgress);
			base.transform.localPosition = new Vector3(0f, 5f * this.parentSize * num, 0f);
			Color color = Color.Lerp(this.barColor, Color.white, t);
			this.barImage.color = new Color(color.r, color.g, color.b, a);
			this.curveProgress += Time.deltaTime * 5f;
			if (this.curveProgress >= 0.99f)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}
		else
		{
			float num2 = this.barAnimationCurve.Evaluate(this.curveProgress);
			float a2 = this.barFadeOutCurve.Evaluate(this.curveProgress);
			float t2 = this.whiteFlashCurve.Evaluate(this.curveProgress);
			base.transform.localPosition = new Vector3(0f, -(5f * this.parentSize) * num2, 0f);
			Color color2 = Color.Lerp(this.barColor, Color.white, t2);
			this.barImage.color = new Color(color2.r, color2.g, color2.b, a2);
			this.curveProgress += Time.deltaTime * 5f;
			if (this.curveProgress >= 0.99f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x04002179 RID: 8569
	private float parentSize;

	// Token: 0x0400217A RID: 8570
	public bool barLossEffect = true;

	// Token: 0x0400217B RID: 8571
	public AnimationCurve barAnimationCurve;

	// Token: 0x0400217C RID: 8572
	public AnimationCurve barFadeOutCurve;

	// Token: 0x0400217D RID: 8573
	public AnimationCurve whiteFlashCurve;

	// Token: 0x0400217E RID: 8574
	private float curveProgress;

	// Token: 0x0400217F RID: 8575
	private RawImage barImage;

	// Token: 0x04002180 RID: 8576
	internal Color barColor;
}
