using System;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class PlayerAvatarLeftArm : MonoBehaviour
{
	// Token: 0x06000F4D RID: 3917 RVA: 0x0008970D File Offset: 0x0008790D
	private void Start()
	{
		this.playerAvatarVisuals = base.GetComponent<PlayerAvatarVisuals>();
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x0008971C File Offset: 0x0008791C
	private void Update()
	{
		if (this.playerAvatarVisuals.isMenuAvatar)
		{
			return;
		}
		if (this.playerAvatar.playerHealth.hurtFreeze)
		{
			return;
		}
		if (this.flashlightController.currentState > FlashlightController.State.Hidden && this.flashlightController.currentState < FlashlightController.State.Outro && !this.playerAvatar.playerAvatarVisuals.animInCrawl)
		{
			this.SetPose(this.flashlightPose);
			this.HeadAnimate(true);
			this.AnimatePose();
			return;
		}
		this.SetPose(this.basePose);
		this.HeadAnimate(false);
		this.AnimatePose();
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x000897AC File Offset: 0x000879AC
	private void HeadAnimate(bool _active)
	{
		if (_active)
		{
			float num = this.playerAvatar.localCameraRotation.eulerAngles.x;
			if (num > 90f)
			{
				num -= 360f;
			}
			this.headRotation = Mathf.Lerp(this.headRotation, num * 0.5f, 20f * Time.deltaTime);
			return;
		}
		this.headRotation = Mathf.Lerp(this.headRotation, 0f, 20f * Time.deltaTime);
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x00089828 File Offset: 0x00087A28
	private void AnimatePose()
	{
		if (this.poseLerp < 1f)
		{
			this.poseLerp += this.poseSpeed * Time.deltaTime;
			this.poseCurrent = Vector3.LerpUnclamped(this.poseOld, this.poseNew, this.poseCurve.Evaluate(this.poseLerp));
		}
		Quaternion rotation = this.leftArmTransform.rotation;
		this.leftArmTransform.localEulerAngles = new Vector3(this.poseCurrent.x, this.poseCurrent.y - this.headRotation, this.poseCurrent.z);
		Quaternion rotation2 = this.leftArmTransform.rotation;
		this.leftArmTransform.rotation = rotation;
		this.leftArmTransform.rotation = SemiFunc.SpringQuaternionGet(this.poseSpring, rotation2, -1f);
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x000898FB File Offset: 0x00087AFB
	private void SetPose(Vector3 _poseNew)
	{
		if (this.poseNew != _poseNew)
		{
			this.poseOld = this.poseCurrent;
			this.poseNew = _poseNew;
			this.poseLerp = 0f;
		}
	}

	// Token: 0x04001921 RID: 6433
	public PlayerAvatar playerAvatar;

	// Token: 0x04001922 RID: 6434
	public Transform leftArmTransform;

	// Token: 0x04001923 RID: 6435
	public FlashlightController flashlightController;

	// Token: 0x04001924 RID: 6436
	[Space]
	public AnimationCurve poseCurve;

	// Token: 0x04001925 RID: 6437
	public float poseSpeed;

	// Token: 0x04001926 RID: 6438
	private float poseLerp;

	// Token: 0x04001927 RID: 6439
	private Vector3 poseNew;

	// Token: 0x04001928 RID: 6440
	private Vector3 poseOld;

	// Token: 0x04001929 RID: 6441
	private Vector3 poseCurrent;

	// Token: 0x0400192A RID: 6442
	[Space]
	public Vector3 basePose;

	// Token: 0x0400192B RID: 6443
	public Vector3 flashlightPose;

	// Token: 0x0400192C RID: 6444
	public SpringQuaternion poseSpring;

	// Token: 0x0400192D RID: 6445
	private PlayerAvatarVisuals playerAvatarVisuals;

	// Token: 0x0400192E RID: 6446
	private float headRotation;
}
