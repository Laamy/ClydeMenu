using System;
using UnityEngine;

// Token: 0x02000130 RID: 304
[Serializable]
public class SpringQuaternion
{
	// Token: 0x04001146 RID: 4422
	public float damping = 0.5f;

	// Token: 0x04001147 RID: 4423
	public float speed = 10f;

	// Token: 0x04001148 RID: 4424
	[Space]
	public bool clamp;

	// Token: 0x04001149 RID: 4425
	public float maxAngle = 20f;

	// Token: 0x0400114A RID: 4426
	internal Quaternion lastRotation;

	// Token: 0x0400114B RID: 4427
	internal Vector3 springVelocity = Vector3.zero;

	// Token: 0x0400114C RID: 4428
	internal bool setup;
}
