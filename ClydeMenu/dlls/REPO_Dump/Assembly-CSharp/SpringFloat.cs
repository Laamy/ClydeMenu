using System;
using UnityEngine;

// Token: 0x0200012F RID: 303
[Serializable]
public class SpringFloat
{
	// Token: 0x0400113F RID: 4415
	public float damping = 0.5f;

	// Token: 0x04001140 RID: 4416
	public float speed = 10f;

	// Token: 0x04001141 RID: 4417
	[Space]
	public bool clamp;

	// Token: 0x04001142 RID: 4418
	public float min;

	// Token: 0x04001143 RID: 4419
	public float max = 1f;

	// Token: 0x04001144 RID: 4420
	internal float lastPosition;

	// Token: 0x04001145 RID: 4421
	internal float springVelocity;
}
