using System;
using UnityEngine;

// Token: 0x02000188 RID: 392
public class DirtFinderBob : MonoBehaviour
{
	// Token: 0x06000D70 RID: 3440 RVA: 0x00075D9E File Offset: 0x00073F9E
	private void Start()
	{
		this.CameraBob = CameraBob.Instance;
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x00075DAC File Offset: 0x00073FAC
	private void Update()
	{
		if (GameManager.Multiplayer() && !this.PlayerAvatar.isLocal)
		{
			return;
		}
		this.TargetPosY = this.CameraBob.transform.localRotation.y * this.PosYMultiplier;
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParamsPosY, Time.deltaTime, this.SpringFreqPosY, this.SpringDampingPosY);
		SpringUtils.UpdateDampedSpringMotion(ref this.CurrentPosY, ref this.VelocityPosY, this.TargetPosY, this.SpringParamsPosY);
		this.TargetPosZ = this.CameraBob.transform.localRotation.z * this.PosZMultiplier;
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParamsPosZ, Time.deltaTime, this.SpringFreqPosZ, this.SpringDampingPosZ);
		SpringUtils.UpdateDampedSpringMotion(ref this.CurrentPosZ, ref this.VelocityPosZ, this.TargetPosZ, this.SpringParamsPosZ);
		base.transform.localPosition = new Vector3(0f, this.CurrentPosY * 0.0025f + CameraJump.instance.transform.localPosition.y, 0f);
		base.transform.localRotation = Quaternion.Euler(CameraJump.instance.transform.localRotation.eulerAngles.x * 2f, 0f, 0f);
	}

	// Token: 0x04001569 RID: 5481
	public CameraBob CameraBob;

	// Token: 0x0400156A RID: 5482
	public PlayerAvatar PlayerAvatar;

	// Token: 0x0400156B RID: 5483
	[Space]
	public float PosZMultiplier = 1f;

	// Token: 0x0400156C RID: 5484
	public float SpringFreqPosZ = 15f;

	// Token: 0x0400156D RID: 5485
	public float SpringDampingPosZ = 0.5f;

	// Token: 0x0400156E RID: 5486
	private float TargetPosZ;

	// Token: 0x0400156F RID: 5487
	private float CurrentPosZ;

	// Token: 0x04001570 RID: 5488
	private float VelocityPosZ;

	// Token: 0x04001571 RID: 5489
	private SpringUtils.tDampedSpringMotionParams SpringParamsPosZ = new SpringUtils.tDampedSpringMotionParams();

	// Token: 0x04001572 RID: 5490
	[Space]
	public float PosYMultiplier = 1f;

	// Token: 0x04001573 RID: 5491
	public float SpringFreqPosY = 15f;

	// Token: 0x04001574 RID: 5492
	public float SpringDampingPosY = 0.5f;

	// Token: 0x04001575 RID: 5493
	private float TargetPosY;

	// Token: 0x04001576 RID: 5494
	private float CurrentPosY;

	// Token: 0x04001577 RID: 5495
	private float VelocityPosY;

	// Token: 0x04001578 RID: 5496
	private SpringUtils.tDampedSpringMotionParams SpringParamsPosY = new SpringUtils.tDampedSpringMotionParams();
}
