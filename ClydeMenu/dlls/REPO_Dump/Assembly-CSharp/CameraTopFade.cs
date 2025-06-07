using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class CameraTopFade : MonoBehaviour
{
	// Token: 0x060000B3 RID: 179 RVA: 0x000071D7 File Offset: 0x000053D7
	private void Awake()
	{
		CameraTopFade.Instance = this;
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x000071E0 File Offset: 0x000053E0
	public void Set(float amount, float time)
	{
		this.ActiveTimer = time;
		if (!this.Active)
		{
			this.Active = true;
			this.AmountStart = this.AmountCurrent;
			this.AmountEnd = amount;
			this.LerpAmount = 0f;
		}
		if (!this.Fading)
		{
			Color color = this.Mesh.material.color;
			color.a = 0f;
			this.Mesh.material.color = color;
			this.MeshTransform.gameObject.SetActive(true);
			this.Fading = true;
			base.StartCoroutine(this.Fade());
		}
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x0000727C File Offset: 0x0000547C
	private IEnumerator Fade()
	{
		while (this.Fading)
		{
			if (this.Active)
			{
				this.AmountCurrent = Mathf.Lerp(this.AmountStart, this.AmountEnd, this.Curve.Evaluate(this.LerpAmount));
				this.LerpAmount += this.Speed * Time.deltaTime;
				this.LerpAmount = Mathf.Clamp01(this.LerpAmount);
				if (this.ActiveTimer > 0f)
				{
					this.ActiveTimer -= Time.deltaTime;
				}
				else
				{
					this.AmountStart = this.AmountCurrent;
					this.AmountEnd = 0f;
					this.Active = false;
					this.LerpAmount = 0f;
				}
			}
			else
			{
				this.AmountCurrent = Mathf.Lerp(this.AmountStart, this.AmountEnd, this.Curve.Evaluate(this.LerpAmount));
				this.LerpAmount += this.Speed * Time.deltaTime;
				this.LerpAmount = Mathf.Clamp01(this.LerpAmount);
				if (this.LerpAmount >= 1f)
				{
					this.Fading = false;
					this.MeshTransform.gameObject.SetActive(false);
				}
			}
			Color color = this.Mesh.material.color;
			color.a = this.AmountCurrent;
			this.Mesh.material.color = color;
			yield return null;
		}
		yield break;
	}

	// Token: 0x040001D1 RID: 465
	public static CameraTopFade Instance;

	// Token: 0x040001D2 RID: 466
	public Transform MeshTransform;

	// Token: 0x040001D3 RID: 467
	public MeshRenderer Mesh;

	// Token: 0x040001D4 RID: 468
	[Space]
	public AnimationCurve Curve;

	// Token: 0x040001D5 RID: 469
	public float Speed = 1f;

	// Token: 0x040001D6 RID: 470
	private bool Fading;

	// Token: 0x040001D7 RID: 471
	private bool Active;

	// Token: 0x040001D8 RID: 472
	private float ActiveTimer;

	// Token: 0x040001D9 RID: 473
	private float Amount;

	// Token: 0x040001DA RID: 474
	private float AmountCurrent;

	// Token: 0x040001DB RID: 475
	private float AmountStart;

	// Token: 0x040001DC RID: 476
	private float AmountEnd;

	// Token: 0x040001DD RID: 477
	private float LerpAmount;
}
