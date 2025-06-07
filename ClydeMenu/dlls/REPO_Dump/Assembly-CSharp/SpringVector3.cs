using System;
using UnityEngine;

// Token: 0x0200012E RID: 302
[Serializable]
public class SpringVector3
{
	// Token: 0x04001139 RID: 4409
	public float damping = 0.5f;

	// Token: 0x0400113A RID: 4410
	public float speed = 10f;

	// Token: 0x0400113B RID: 4411
	[Space]
	public bool clamp;

	// Token: 0x0400113C RID: 4412
	public float maxDistance = 1f;

	// Token: 0x0400113D RID: 4413
	internal Vector3 lastPosition;

	// Token: 0x0400113E RID: 4414
	internal Vector3 springVelocity = Vector3.zero;
}
