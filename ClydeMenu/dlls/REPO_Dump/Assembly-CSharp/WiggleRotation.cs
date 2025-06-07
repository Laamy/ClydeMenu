using System;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public class WiggleRotation : MonoBehaviour
{
	// Token: 0x0600070C RID: 1804 RVA: 0x00042C34 File Offset: 0x00040E34
	private void LateUpdate()
	{
		float num = this.wiggleOffsetPercentage / 100f * 2f * 3.1415927f;
		Quaternion localRotation = Quaternion.AngleAxis(Mathf.Sin(Time.time * this.wiggleFrequency + num) * this.maxRotation * this.wiggleMultiplier, this.wiggleAxis);
		base.transform.localRotation = localRotation;
	}

	// Token: 0x04000BF8 RID: 3064
	public Vector3 wiggleAxis = Vector3.up;

	// Token: 0x04000BF9 RID: 3065
	public float maxRotation = 10f;

	// Token: 0x04000BFA RID: 3066
	public float wiggleFrequency = 1f;

	// Token: 0x04000BFB RID: 3067
	[Range(0f, 100f)]
	public float wiggleOffsetPercentage;

	// Token: 0x04000BFC RID: 3068
	public float wiggleMultiplier = 1f;
}
