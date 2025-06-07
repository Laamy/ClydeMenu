using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200026C RID: 620
public class HurtVignette : MonoBehaviour
{
	// Token: 0x060013B2 RID: 5042 RVA: 0x000AEEA0 File Offset: 0x000AD0A0
	private void Start()
	{
		HurtVignette.instance = this;
	}

	// Token: 0x060013B3 RID: 5043 RVA: 0x000AEEA8 File Offset: 0x000AD0A8
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated || SemiFunc.MenuLevel())
		{
			return;
		}
		if (!PlayerController.instance.playerAvatarScript.isDisabled && (float)PlayerController.instance.playerAvatarScript.playerHealth.health < 10f)
		{
			if (this.lerp < 1f)
			{
				this.lerp += 1f * Time.deltaTime;
				this.rectTransform.localScale = Vector3.one * Mathf.Lerp(this.inactiveScale, this.activeScale, this.introCurve.Evaluate(this.lerp));
				this.image.color = Color.Lerp(this.inactiveColor, this.activeColor, this.introCurve.Evaluate(this.lerp));
				return;
			}
		}
		else if (this.lerp > 0f)
		{
			this.lerp -= 1f * Time.deltaTime;
			this.rectTransform.localScale = Vector3.one * Mathf.Lerp(this.inactiveScale, this.activeScale, this.outroCurve.Evaluate(this.lerp));
			this.image.color = Color.Lerp(this.inactiveColor, this.activeColor, this.outroCurve.Evaluate(this.lerp));
		}
	}

	// Token: 0x040021CD RID: 8653
	public Image image;

	// Token: 0x040021CE RID: 8654
	public RectTransform rectTransform;

	// Token: 0x040021CF RID: 8655
	[Space]
	public Color activeColor;

	// Token: 0x040021D0 RID: 8656
	public Color inactiveColor;

	// Token: 0x040021D1 RID: 8657
	[Space]
	public float activeScale;

	// Token: 0x040021D2 RID: 8658
	public float inactiveScale;

	// Token: 0x040021D3 RID: 8659
	[Space]
	public AnimationCurve introCurve;

	// Token: 0x040021D4 RID: 8660
	public AnimationCurve outroCurve;

	// Token: 0x040021D5 RID: 8661
	private float lerp;

	// Token: 0x040021D6 RID: 8662
	public static HurtVignette instance;
}
