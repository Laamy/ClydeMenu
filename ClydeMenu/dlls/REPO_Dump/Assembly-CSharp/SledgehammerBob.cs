using System;
using UnityEngine;

// Token: 0x020000C4 RID: 196
public class SledgehammerBob : MonoBehaviour
{
	// Token: 0x0600071B RID: 1819 RVA: 0x00043AAB File Offset: 0x00041CAB
	private void Start()
	{
		this.CameraBob = GameDirector.instance.CameraBob;
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x00043AC0 File Offset: 0x00041CC0
	private void Update()
	{
		this.TargetPosY = this.CameraBob.transform.localRotation.z * -500f;
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParamsPosY, Time.deltaTime, this.SpringFreqPosY, this.SpringDampingPosY);
		SpringUtils.UpdateDampedSpringMotion(ref this.CurrentPosY, ref this.VelocityPosY, this.TargetPosY, this.SpringParamsPosY);
		this.TargetPosZ = this.CameraBob.transform.localPosition.y * -0.5f;
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParamsPosZ, Time.deltaTime, this.SpringFreqPosZ, this.SpringDampingPosZ);
		SpringUtils.UpdateDampedSpringMotion(ref this.CurrentPosZ, ref this.VelocityPosZ, this.TargetPosZ, this.SpringParamsPosZ);
		base.transform.localRotation = Quaternion.Euler(-this.CurrentPosY, 0f, this.CurrentPosY);
		base.transform.localPosition = new Vector3(0f, 0f, this.CameraBob.transform.localPosition.y + this.CurrentPosZ);
	}

	// Token: 0x04000C39 RID: 3129
	public CameraBob CameraBob;

	// Token: 0x04000C3A RID: 3130
	[Space]
	public float SpringFreqPosZ = 15f;

	// Token: 0x04000C3B RID: 3131
	public float SpringDampingPosZ = 0.5f;

	// Token: 0x04000C3C RID: 3132
	private float TargetPosZ;

	// Token: 0x04000C3D RID: 3133
	private float CurrentPosZ;

	// Token: 0x04000C3E RID: 3134
	private float VelocityPosZ;

	// Token: 0x04000C3F RID: 3135
	private SpringUtils.tDampedSpringMotionParams SpringParamsPosZ = new SpringUtils.tDampedSpringMotionParams();

	// Token: 0x04000C40 RID: 3136
	[Space]
	public float SpringFreqPosY = 15f;

	// Token: 0x04000C41 RID: 3137
	public float SpringDampingPosY = 0.5f;

	// Token: 0x04000C42 RID: 3138
	private float TargetPosY;

	// Token: 0x04000C43 RID: 3139
	private float CurrentPosY;

	// Token: 0x04000C44 RID: 3140
	private float VelocityPosY;

	// Token: 0x04000C45 RID: 3141
	private SpringUtils.tDampedSpringMotionParams SpringParamsPosY = new SpringUtils.tDampedSpringMotionParams();
}
