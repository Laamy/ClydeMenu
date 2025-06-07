using System;
using UnityEngine;

// Token: 0x020001C9 RID: 457
[Serializable]
public class EyeSettings
{
	// Token: 0x04001A93 RID: 6803
	[Header("Upper Eyelid Settings")]
	public float upperLidAngle;

	// Token: 0x04001A94 RID: 6804
	public float upperLidClosedPercent;

	// Token: 0x04001A95 RID: 6805
	public float upperLidClosedPercentJitterAmount;

	// Token: 0x04001A96 RID: 6806
	public float upperLidClosedPercentJitterSpeed;

	// Token: 0x04001A97 RID: 6807
	[Header("Lower Eyelid Settings")]
	public float lowerLidAngle;

	// Token: 0x04001A98 RID: 6808
	public float lowerLidClosedPercent;

	// Token: 0x04001A99 RID: 6809
	public float lowerLidClosedPercentJitterAmount;

	// Token: 0x04001A9A RID: 6810
	public float lowerLidClosedPercentJitterSpeed;

	// Token: 0x04001A9B RID: 6811
	[Header("Pupil Settings")]
	public float pupilSize;

	// Token: 0x04001A9C RID: 6812
	public float pupilSizeJitterAmount;

	// Token: 0x04001A9D RID: 6813
	public float pupilSizeJitterSpeed;

	// Token: 0x04001A9E RID: 6814
	public float pupilPositionJitter;

	// Token: 0x04001A9F RID: 6815
	public float pupilPositionJitterAmount;

	// Token: 0x04001AA0 RID: 6816
	public float pupilOffsetRotationX;

	// Token: 0x04001AA1 RID: 6817
	public float pupilOffsetRotationY;
}
