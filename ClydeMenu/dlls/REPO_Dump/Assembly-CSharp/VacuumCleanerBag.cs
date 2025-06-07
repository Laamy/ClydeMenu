using System;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class VacuumCleanerBag : MonoBehaviour
{
	// Token: 0x06000757 RID: 1879 RVA: 0x0004629C File Offset: 0x0004449C
	private void Update()
	{
		if (this.ActiveX)
		{
			this.LerpX += this.SpeedX * Time.deltaTime;
			if (this.LerpX >= 1f)
			{
				if (!this.Active)
				{
					this.ActiveX = false;
				}
				this.LerpX = 0f;
			}
		}
		else if (this.Active)
		{
			this.ActiveX = true;
		}
		if (this.ActiveZ)
		{
			this.LerpZ += this.SpeedZ * Time.deltaTime;
			if (this.LerpZ >= 1f)
			{
				if (!this.Active)
				{
					this.ActiveZ = false;
				}
				this.LerpZ = 0f;
			}
		}
		else if (this.Active)
		{
			this.ActiveZ = true;
		}
		if (this.ActiveX || this.ActiveZ)
		{
			float num = Mathf.Lerp(0f, this.AmountX, this.AnimationCurve.Evaluate(this.LerpX));
			float num2 = Mathf.Lerp(0f, this.AmountZ, this.AnimationCurve.Evaluate(this.LerpZ));
			base.transform.localScale = new Vector3(1f + num, 1f, 1f + num2);
		}
	}

	// Token: 0x04000CE9 RID: 3305
	[HideInInspector]
	public bool Active;

	// Token: 0x04000CEA RID: 3306
	[Space]
	public float AmountX;

	// Token: 0x04000CEB RID: 3307
	public float SpeedX;

	// Token: 0x04000CEC RID: 3308
	private float LerpX;

	// Token: 0x04000CED RID: 3309
	private bool ActiveX;

	// Token: 0x04000CEE RID: 3310
	[Space]
	public float AmountZ;

	// Token: 0x04000CEF RID: 3311
	public float SpeedZ;

	// Token: 0x04000CF0 RID: 3312
	private float LerpZ;

	// Token: 0x04000CF1 RID: 3313
	private bool ActiveZ;

	// Token: 0x04000CF2 RID: 3314
	[Space]
	public AnimationCurve AnimationCurve;
}
