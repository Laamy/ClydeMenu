using System;
using UnityEngine;

// Token: 0x0200005C RID: 92
public class EnemyHeadFloat : MonoBehaviour
{
	// Token: 0x0600030E RID: 782 RVA: 0x0001E6B6 File Offset: 0x0001C8B6
	public void Disable(float time)
	{
		this.DisableTimer = time;
	}

	// Token: 0x0600030F RID: 783 RVA: 0x0001E6C0 File Offset: 0x0001C8C0
	private void Update()
	{
		if (this.DisableTimer > 0f)
		{
			this.DisableTimer -= Time.deltaTime;
			return;
		}
		if (!this.ReversePos)
		{
			this.LerpPos += this.SpeedPos * Time.deltaTime;
			if (this.LerpPos >= 1f)
			{
				this.ReversePos = true;
				this.LerpPos = 1f;
			}
		}
		else
		{
			this.LerpPos -= this.SpeedPos * Time.deltaTime;
			if (this.LerpPos <= 0f)
			{
				this.ReversePos = false;
				this.LerpPos = 0f;
			}
		}
		base.transform.localPosition = new Vector3(0f, this.CurvePos.Evaluate(this.LerpPos) * this.AmountPos, 0f);
		this.LerpRot += this.SpeedRot * Time.deltaTime;
		if (this.LerpRot >= 1f)
		{
			this.LerpRot = 0f;
		}
		base.transform.localRotation = Quaternion.Euler(this.CurveRot.Evaluate(this.LerpRot) * this.AmountRot, 0f, 0f);
	}

	// Token: 0x04000551 RID: 1361
	public AnimationCurve CurvePos;

	// Token: 0x04000552 RID: 1362
	public float SpeedPos;

	// Token: 0x04000553 RID: 1363
	public float AmountPos;

	// Token: 0x04000554 RID: 1364
	private float LerpPos;

	// Token: 0x04000555 RID: 1365
	private bool ReversePos;

	// Token: 0x04000556 RID: 1366
	[Space]
	public AnimationCurve CurveRot;

	// Token: 0x04000557 RID: 1367
	public float SpeedRot;

	// Token: 0x04000558 RID: 1368
	public float AmountRot;

	// Token: 0x04000559 RID: 1369
	private float LerpRot;

	// Token: 0x0400055A RID: 1370
	private float DisableTimer;
}
