using System;
using UnityEngine;

// Token: 0x02000066 RID: 102
public class EnemyHeadHairTilt : MonoBehaviour
{
	// Token: 0x06000328 RID: 808 RVA: 0x0001F154 File Offset: 0x0001D354
	private void Update()
	{
		float num = Mathf.Clamp(Vector3.Cross(this.ForwardPrev, this.EnemyTransform.forward).y * this.Amount, -this.MaxAmount, this.MaxAmount);
		if (this.RandomTimer <= 0f && num > 0.1f)
		{
			this.RandomTimer = Random.Range(this.RandomTimeMin, this.RandomTimeMax);
			this.RandomCurrent = Random.Range(this.RandomMin, this.RandomMax);
		}
		else
		{
			this.RandomTimer -= Time.deltaTime;
		}
		num += this.RandomCurrent;
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParams, Time.deltaTime, this.SpringFreq, this.SpringDamping);
		SpringUtils.UpdateDampedSpringMotion(ref this.SpringCurrent, ref this.SpringVelocity, num, this.SpringParams);
		base.transform.localRotation = Quaternion.Euler(0f, this.SpringCurrent, 0f);
		this.ForwardPrev = this.EnemyTransform.forward;
	}

	// Token: 0x0400058E RID: 1422
	public Transform EnemyTransform;

	// Token: 0x0400058F RID: 1423
	private Vector3 ForwardPrev;

	// Token: 0x04000590 RID: 1424
	[Space]
	public float Amount = -500f;

	// Token: 0x04000591 RID: 1425
	public float MaxAmount = 20f;

	// Token: 0x04000592 RID: 1426
	[Space]
	public float RandomMin;

	// Token: 0x04000593 RID: 1427
	public float RandomMax;

	// Token: 0x04000594 RID: 1428
	private float RandomCurrent;

	// Token: 0x04000595 RID: 1429
	private float RandomTimer;

	// Token: 0x04000596 RID: 1430
	public float RandomTimeMin;

	// Token: 0x04000597 RID: 1431
	public float RandomTimeMax;

	// Token: 0x04000598 RID: 1432
	[Space]
	public float SpringFreq = 15f;

	// Token: 0x04000599 RID: 1433
	public float SpringDamping = 0.5f;

	// Token: 0x0400059A RID: 1434
	private float SpringTarget;

	// Token: 0x0400059B RID: 1435
	private float SpringCurrent;

	// Token: 0x0400059C RID: 1436
	private float SpringVelocity;

	// Token: 0x0400059D RID: 1437
	private SpringUtils.tDampedSpringMotionParams SpringParams = new SpringUtils.tDampedSpringMotionParams();
}
