using System;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class EnemyHeadEyeTremble : MonoBehaviour
{
	// Token: 0x0600030C RID: 780 RVA: 0x0001E59C File Offset: 0x0001C79C
	private void Update()
	{
		if (this.TimerX <= 0f)
		{
			this.TimerX = Random.Range(this.TimeMin, this.TimeMax);
			this.TargetX = Random.Range(this.Min, this.Max);
		}
		else
		{
			this.TimerX -= Time.deltaTime;
		}
		this.CurrentX = Mathf.Lerp(this.CurrentX, this.TargetX, this.Speed * Time.deltaTime);
		if (this.TimerY <= 0f)
		{
			this.TimerY = Random.Range(this.TimeMin, this.TimeMax);
			this.TargetY = Random.Range(this.Min, this.Max);
		}
		else
		{
			this.TimerY -= Time.deltaTime;
		}
		this.CurrentY = Mathf.Lerp(this.CurrentY, this.TargetY, this.Speed * Time.deltaTime);
		base.transform.localRotation = Quaternion.Euler(this.CurrentX, this.CurrentY, 0f);
	}

	// Token: 0x04000546 RID: 1350
	public float Speed;

	// Token: 0x04000547 RID: 1351
	[Space]
	public float TimeMin;

	// Token: 0x04000548 RID: 1352
	public float TimeMax;

	// Token: 0x04000549 RID: 1353
	private float TimerX;

	// Token: 0x0400054A RID: 1354
	private float TimerY;

	// Token: 0x0400054B RID: 1355
	[Space]
	public float Min;

	// Token: 0x0400054C RID: 1356
	public float Max;

	// Token: 0x0400054D RID: 1357
	private float TargetX;

	// Token: 0x0400054E RID: 1358
	private float CurrentX;

	// Token: 0x0400054F RID: 1359
	private float TargetY;

	// Token: 0x04000550 RID: 1360
	private float CurrentY;
}
