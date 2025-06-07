using System;
using UnityEngine;

// Token: 0x020001B2 RID: 434
public class PlayerArmBackaway : MonoBehaviour
{
	// Token: 0x06000EDF RID: 3807 RVA: 0x0008618C File Offset: 0x0008438C
	private void Update()
	{
		if (this.ArmCollision.Blocked)
		{
			this.SpringTarget = this.BackAwayTarget;
		}
		else
		{
			this.SpringTarget = 0f;
		}
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParams, Time.deltaTime, this.SpringFreq, this.SpringDamping);
		SpringUtils.UpdateDampedSpringMotion(ref this.SpringCurrent, ref this.SpringVelocity, this.SpringTarget, this.SpringParams);
		base.transform.localPosition = new Vector3(0f, 0f, this.SpringCurrent);
	}

	// Token: 0x04001871 RID: 6257
	public PlayerArmCollision ArmCollision;

	// Token: 0x04001872 RID: 6258
	[Space]
	public float BackAwayTarget;

	// Token: 0x04001873 RID: 6259
	[Space]
	public float SpringFreq = 15f;

	// Token: 0x04001874 RID: 6260
	public float SpringDamping = 0.5f;

	// Token: 0x04001875 RID: 6261
	private float SpringTarget;

	// Token: 0x04001876 RID: 6262
	private float SpringCurrent;

	// Token: 0x04001877 RID: 6263
	private float SpringVelocity;

	// Token: 0x04001878 RID: 6264
	private SpringUtils.tDampedSpringMotionParams SpringParams = new SpringUtils.tDampedSpringMotionParams();
}
