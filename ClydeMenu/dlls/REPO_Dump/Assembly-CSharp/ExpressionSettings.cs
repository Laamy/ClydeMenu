using System;
using UnityEngine;

// Token: 0x020001CA RID: 458
[Serializable]
public class ExpressionSettings
{
	// Token: 0x04001AA2 RID: 6818
	public string expressionName;

	// Token: 0x04001AA3 RID: 6819
	[Range(0f, 100f)]
	public float weight;

	// Token: 0x04001AA4 RID: 6820
	internal float timer;

	// Token: 0x04001AA5 RID: 6821
	internal bool isExpressing;

	// Token: 0x04001AA6 RID: 6822
	internal bool stopExpressing;

	// Token: 0x04001AA7 RID: 6823
	[Space]
	public float headTiltAmount;

	// Token: 0x04001AA8 RID: 6824
	[Header("Left Eye Settings")]
	public EyeSettings leftEye;

	// Token: 0x04001AA9 RID: 6825
	[Header("Right Eye Settings")]
	public EyeSettings rightEye;
}
