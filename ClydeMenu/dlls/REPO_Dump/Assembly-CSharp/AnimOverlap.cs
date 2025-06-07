using System;
using UnityEngine;

// Token: 0x02000233 RID: 563
public class AnimOverlap : MonoBehaviour
{
	// Token: 0x0600128E RID: 4750 RVA: 0x000A6DB2 File Offset: 0x000A4FB2
	private void Update()
	{
	}

	// Token: 0x04001F57 RID: 8023
	public Transform targetFollow;

	// Token: 0x04001F58 RID: 8024
	private float previousX;

	// Token: 0x04001F59 RID: 8025
	private float previousY;

	// Token: 0x04001F5A RID: 8026
	private Quaternion targetAngle;

	// Token: 0x04001F5B RID: 8027
	[Header("Rotation X")]
	public float springFreqRotX = 15f;

	// Token: 0x04001F5C RID: 8028
	public float springDampingRotX = 0.5f;

	// Token: 0x04001F5D RID: 8029
	private float targetRotX;

	// Token: 0x04001F5E RID: 8030
	private float currentRotX;

	// Token: 0x04001F5F RID: 8031
	private float velocityRotX;

	// Token: 0x04001F60 RID: 8032
	private SpringUtils.tDampedSpringMotionParams springParamsRotX = new SpringUtils.tDampedSpringMotionParams();

	// Token: 0x04001F61 RID: 8033
	[Header("Rotation Y")]
	public float springFreqRotY = 15f;

	// Token: 0x04001F62 RID: 8034
	public float springDampingRotY = 0.5f;

	// Token: 0x04001F63 RID: 8035
	private float targetRotY;

	// Token: 0x04001F64 RID: 8036
	private float currentRotY;

	// Token: 0x04001F65 RID: 8037
	private float velocityRotY;

	// Token: 0x04001F66 RID: 8038
	private SpringUtils.tDampedSpringMotionParams springParamsRotY = new SpringUtils.tDampedSpringMotionParams();

	// Token: 0x04001F67 RID: 8039
	[Header("Rotation Z")]
	public float springFreqRotZ = 15f;

	// Token: 0x04001F68 RID: 8040
	public float springDampingRotZ = 0.5f;

	// Token: 0x04001F69 RID: 8041
	private float targetRotZ;

	// Token: 0x04001F6A RID: 8042
	private float currentRotZ;

	// Token: 0x04001F6B RID: 8043
	private float velocityRotZ;

	// Token: 0x04001F6C RID: 8044
	private SpringUtils.tDampedSpringMotionParams springParamsRotZ = new SpringUtils.tDampedSpringMotionParams();

	// Token: 0x04001F6D RID: 8045
	private float velocity;
}
