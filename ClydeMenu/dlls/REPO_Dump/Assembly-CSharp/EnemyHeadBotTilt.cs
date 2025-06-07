using System;
using UnityEngine;

// Token: 0x02000067 RID: 103
public class EnemyHeadBotTilt : MonoBehaviour
{
	// Token: 0x0600032A RID: 810 RVA: 0x0001F29C File Offset: 0x0001D49C
	private void Update()
	{
		float equilibriumPos = Mathf.Clamp(Vector3.Cross(this.ForwardPrev, base.transform.forward).y * this.Amount, -this.MaxAmount, this.MaxAmount);
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParams, Time.deltaTime, this.SpringFreq, this.SpringDamping);
		SpringUtils.UpdateDampedSpringMotion(ref this.SpringCurrent, ref this.SpringVelocity, equilibriumPos, this.SpringParams);
		base.transform.localRotation = Quaternion.Euler(0f, -this.SpringCurrent * 0.5f, this.SpringCurrent);
		this.ForwardPrev = base.transform.forward;
	}

	// Token: 0x0400059E RID: 1438
	private Vector3 ForwardPrev;

	// Token: 0x0400059F RID: 1439
	[Space]
	public float Amount = -500f;

	// Token: 0x040005A0 RID: 1440
	public float MaxAmount = 20f;

	// Token: 0x040005A1 RID: 1441
	[Space]
	public float SpringFreq = 15f;

	// Token: 0x040005A2 RID: 1442
	public float SpringDamping = 0.5f;

	// Token: 0x040005A3 RID: 1443
	private float SpringTarget;

	// Token: 0x040005A4 RID: 1444
	private float SpringCurrent;

	// Token: 0x040005A5 RID: 1445
	private float SpringVelocity;

	// Token: 0x040005A6 RID: 1446
	private SpringUtils.tDampedSpringMotionParams SpringParams = new SpringUtils.tDampedSpringMotionParams();
}
