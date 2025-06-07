using System;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class EnemyHeadChaseOffset : MonoBehaviour
{
	// Token: 0x060002F7 RID: 759 RVA: 0x0001D9B4 File Offset: 0x0001BBB4
	private void Update()
	{
		if (this.Enemy.CurrentState == EnemyState.Chase || this.Enemy.CurrentState == EnemyState.ChaseSlow)
		{
			if (this.Lerp <= 0f || this.Lerp >= 1f)
			{
				this.Active = true;
			}
		}
		else if (this.Lerp <= 0f || this.Lerp >= 1f)
		{
			this.Active = false;
		}
		if (this.Active)
		{
			this.Lerp += Time.deltaTime * this.IntroSpeed;
		}
		else
		{
			this.Lerp -= Time.deltaTime * this.OutroSpeed;
		}
		this.Lerp = Mathf.Clamp01(this.Lerp);
		if (this.Active)
		{
			base.transform.localRotation = Quaternion.SlerpUnclamped(Quaternion.identity, Quaternion.Euler(this.Offset), this.IntroCurve.Evaluate(this.Lerp));
			return;
		}
		base.transform.localRotation = Quaternion.SlerpUnclamped(Quaternion.identity, Quaternion.Euler(this.Offset), this.OutroCurve.Evaluate(this.Lerp));
	}

	// Token: 0x04000516 RID: 1302
	public Enemy Enemy;

	// Token: 0x04000517 RID: 1303
	[Space]
	public Vector3 Offset;

	// Token: 0x04000518 RID: 1304
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x04000519 RID: 1305
	public float IntroSpeed;

	// Token: 0x0400051A RID: 1306
	[Space]
	public AnimationCurve OutroCurve;

	// Token: 0x0400051B RID: 1307
	public float OutroSpeed;

	// Token: 0x0400051C RID: 1308
	private float Lerp;

	// Token: 0x0400051D RID: 1309
	private bool Active;
}
