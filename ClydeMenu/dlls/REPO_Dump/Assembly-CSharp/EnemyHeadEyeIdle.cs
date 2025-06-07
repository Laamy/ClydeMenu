using System;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class EnemyHeadEyeIdle : MonoBehaviour
{
	// Token: 0x06000304 RID: 772 RVA: 0x0001E328 File Offset: 0x0001C528
	private void Update()
	{
		if (this.EyeTarget.Idle)
		{
			if (this.Timer <= 0f)
			{
				this.Timer = Random.Range(this.TimeMin, this.TimeMax);
				this.CurrentX = Random.Range(this.MinX, this.MaxX);
				this.CurrentY = Random.Range(this.MinY, this.MaxY);
			}
			else
			{
				this.Timer -= Time.deltaTime;
			}
		}
		else
		{
			this.CurrentX = 0f;
			this.CurrentY = 0f;
		}
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(this.CurrentX, this.CurrentY, base.transform.localPosition.z), this.Speed * Time.deltaTime);
	}

	// Token: 0x0400052D RID: 1325
	public EnemyHeadEyeTarget EyeTarget;

	// Token: 0x0400052E RID: 1326
	public float Speed;

	// Token: 0x0400052F RID: 1327
	[Space]
	public float TimeMin;

	// Token: 0x04000530 RID: 1328
	public float TimeMax;

	// Token: 0x04000531 RID: 1329
	private float Timer;

	// Token: 0x04000532 RID: 1330
	[Space]
	public float MinX;

	// Token: 0x04000533 RID: 1331
	public float MaxX;

	// Token: 0x04000534 RID: 1332
	private float CurrentX;

	// Token: 0x04000535 RID: 1333
	[Space]
	public float MinY;

	// Token: 0x04000536 RID: 1334
	public float MaxY;

	// Token: 0x04000537 RID: 1335
	private float CurrentY;
}
