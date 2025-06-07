using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000064 RID: 100
public class EnemyHeadHairLean : MonoBehaviour
{
	// Token: 0x06000324 RID: 804 RVA: 0x0001EFDC File Offset: 0x0001D1DC
	private void Update()
	{
		if (this.RandomTimer <= 0f && this.Agent.velocity.magnitude > 0.1f)
		{
			this.RandomTimer = Random.Range(this.RandomTimeMin, this.RandomTimeMax);
			this.RandomCurrent = Random.Range(this.RandomMin, this.RandomMax);
		}
		else
		{
			this.RandomTimer -= Time.deltaTime;
		}
		float equilibriumPos = 0f;
		if (this.Agent.velocity.magnitude > 0.1f)
		{
			equilibriumPos = Mathf.Clamp(this.Agent.velocity.magnitude * this.Amount, -this.MaxAmount, this.MaxAmount) + this.RandomCurrent;
		}
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParams, Time.deltaTime, this.SpringFreq, this.SpringDamping);
		SpringUtils.UpdateDampedSpringMotion(ref this.SpringCurrent, ref this.SpringVelocity, equilibriumPos, this.SpringParams);
		base.transform.localRotation = Quaternion.Euler(this.SpringCurrent, 0f, 0f);
	}

	// Token: 0x0400057E RID: 1406
	public NavMeshAgent Agent;

	// Token: 0x0400057F RID: 1407
	[Space]
	public float Amount = -500f;

	// Token: 0x04000580 RID: 1408
	public float MaxAmount = 20f;

	// Token: 0x04000581 RID: 1409
	[Space]
	public float RandomMin;

	// Token: 0x04000582 RID: 1410
	public float RandomMax;

	// Token: 0x04000583 RID: 1411
	private float RandomCurrent;

	// Token: 0x04000584 RID: 1412
	private float RandomTimer;

	// Token: 0x04000585 RID: 1413
	public float RandomTimeMin;

	// Token: 0x04000586 RID: 1414
	public float RandomTimeMax;

	// Token: 0x04000587 RID: 1415
	[Space]
	public float SpringFreq = 15f;

	// Token: 0x04000588 RID: 1416
	public float SpringDamping = 0.5f;

	// Token: 0x04000589 RID: 1417
	private float SpringTarget;

	// Token: 0x0400058A RID: 1418
	private float SpringCurrent;

	// Token: 0x0400058B RID: 1419
	private float SpringVelocity;

	// Token: 0x0400058C RID: 1420
	private SpringUtils.tDampedSpringMotionParams SpringParams = new SpringUtils.tDampedSpringMotionParams();
}
