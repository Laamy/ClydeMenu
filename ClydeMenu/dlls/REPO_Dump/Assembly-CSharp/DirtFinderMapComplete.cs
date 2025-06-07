using System;
using TMPro;
using UnityEngine;

// Token: 0x0200018D RID: 397
public class DirtFinderMapComplete : MonoBehaviour
{
	// Token: 0x06000D82 RID: 3458 RVA: 0x0007659A File Offset: 0x0007479A
	private void Start()
	{
		GameDirector.instance.CameraImpact.Shake(1f, 0.25f);
		GameDirector.instance.CameraShake.Shake(1f, 0.25f);
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x000765D0 File Offset: 0x000747D0
	private void Update()
	{
		if (this.FlashLerp < 1f)
		{
			this.FlashLerp += this.FlashSpeed * Time.deltaTime;
			this.FlashRenderer.color = Color.Lerp(this.FlashRenderer.color, new Color(255f, 255f, 255f, 0f), this.FlashCurve.Evaluate(this.FlashLerp));
			if (this.FlashLerp >= 1f)
			{
				this.FlashLerp = 1f;
				this.FlashRenderer.transform.gameObject.SetActive(false);
			}
		}
		this.TextDilate += 5f * this.TextDilateIncrease * Time.deltaTime;
		if (this.TextDilate >= 1f)
		{
			this.TextDilate = 1f;
			if (this.TextDilateWait > 0f)
			{
				this.TextDilateWait -= Time.deltaTime;
			}
			else
			{
				this.TextDilateWait = 0.5f;
				this.TextDilateIncrease = -1f;
			}
		}
		else if (this.TextDilate <= -1f)
		{
			this.TextDilate = -1f;
			if (this.TextDilateWait > 0f)
			{
				this.TextDilateWait -= Time.deltaTime;
			}
			else
			{
				this.TextDilateWait = 0.5f;
				this.TextDilateIncrease = 1f;
			}
		}
		this.TextTop.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, this.TextDilate);
		this.TextBot.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, this.TextDilate);
		this.CompleteTime -= Time.deltaTime;
		if (this.CompleteTime <= 0f)
		{
			this.CompleteTime = 0f;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x040015AD RID: 5549
	public SpriteRenderer FlashRenderer;

	// Token: 0x040015AE RID: 5550
	public AnimationCurve FlashCurve;

	// Token: 0x040015AF RID: 5551
	public float FlashSpeed;

	// Token: 0x040015B0 RID: 5552
	private float FlashLerp;

	// Token: 0x040015B1 RID: 5553
	[Space]
	public TextMeshPro TextTop;

	// Token: 0x040015B2 RID: 5554
	public TextMeshPro TextBot;

	// Token: 0x040015B3 RID: 5555
	private float TextDilate;

	// Token: 0x040015B4 RID: 5556
	private float TextDilateWait;

	// Token: 0x040015B5 RID: 5557
	private float TextDilateIncrease = 1f;

	// Token: 0x040015B6 RID: 5558
	[Space]
	public float CompleteTime;
}
