using System;
using UnityEngine;

// Token: 0x020001BD RID: 445
public class PlayerAvatarRightArm : MonoBehaviour
{
	// Token: 0x06000F56 RID: 3926 RVA: 0x00089BA8 File Offset: 0x00087DA8
	private void Start()
	{
		this.playerAvatarVisuals = base.GetComponent<PlayerAvatarVisuals>();
		this.grabberLightIntensity = this.grabberLight.intensity;
		if (!GameManager.Multiplayer() || (this.playerAvatar && this.playerAvatar.photonView.IsMine))
		{
			this.grabberTransform.gameObject.SetActive(false);
			base.enabled = false;
		}
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x00089C10 File Offset: 0x00087E10
	private void Update()
	{
		if (this.playerAvatarVisuals.isMenuAvatar)
		{
			return;
		}
		this.deltaTime = this.playerAvatarVisuals.deltaTime;
		if (this.playerAvatar.playerHealth.hurtFreeze)
		{
			return;
		}
		if (this.mapToolController.Active && !this.playerAvatar.playerAvatarVisuals.animInCrawl)
		{
			this.SetPose(this.mapPose);
			this.HeadAnimate(true);
			this.AnimatePose();
		}
		else if (this.physGrabBeam && this.physGrabBeam.lineRenderer && this.physGrabBeam.lineRenderer.enabled && (!this.mapToolController.Active || !this.playerAvatar.playerAvatarVisuals.animInCrawl))
		{
			this.SetPose(this.grabberPose);
			this.HeadAnimate(false);
			this.AnimatePose();
		}
		else
		{
			this.SetPose(this.basePose);
			this.HeadAnimate(false);
			this.AnimatePose();
		}
		this.GrabberLogic();
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x00089D18 File Offset: 0x00087F18
	private void HeadAnimate(bool _active)
	{
		if (_active)
		{
			float num = this.playerAvatar.localCameraRotation.eulerAngles.x;
			if (num > 90f)
			{
				num -= 360f;
			}
			this.headRotation = Mathf.Lerp(this.headRotation, num * 0.5f, 20f * this.deltaTime);
			return;
		}
		this.headRotation = Mathf.Lerp(this.headRotation, 0f, 20f * this.deltaTime);
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x00089D98 File Offset: 0x00087F98
	private void AnimatePose()
	{
		if (this.poseLerp < 1f)
		{
			this.poseLerp += this.poseSpeed * this.deltaTime;
			this.poseCurrent = Vector3.LerpUnclamped(this.poseOld, this.poseNew, this.poseCurve.Evaluate(this.poseLerp));
		}
		this.rightArmTransform.localEulerAngles = new Vector3(this.poseCurrent.x, this.poseCurrent.y + this.headRotation, this.poseCurrent.z);
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x00089E2C File Offset: 0x0008802C
	private void SetPose(Vector3 _poseNew)
	{
		if (this.poseNew != _poseNew)
		{
			this.poseOld = this.poseCurrent;
			this.poseNew = _poseNew;
			this.poseLerp = 0f;
		}
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x00089E5C File Offset: 0x0008805C
	private void GrabberClawLogic()
	{
		if (this.physGrabBeam.lineRenderer.enabled)
		{
			if (this.grabberClawHidden)
			{
				this.grabberClawHidden = false;
				this.grabberClawParent.gameObject.SetActive(true);
			}
			this.grabberClawLerp = Mathf.Clamp01(this.grabberClawLerp + 3f * this.deltaTime);
		}
		else if (!this.grabberClawHidden)
		{
			if (this.grabberClawLerp <= 0f)
			{
				this.grabberClawHidden = true;
				this.grabberClawParent.gameObject.SetActive(false);
				this.grabberClawRotation = 0f;
			}
			this.grabberClawLerp = Mathf.Clamp01(this.grabberClawLerp - 3f * this.deltaTime);
		}
		if (!this.grabberClawHidden)
		{
			this.grabberClawChildLerp = Mathf.Clamp01(this.grabberClawChildLerp + 1f * this.deltaTime);
			if (this.grabberClawChildLerp >= 1f)
			{
				this.grabberClawChildLerp = 0f;
			}
			Vector3 euler = Vector3.LerpUnclamped(new Vector3(60f, 0f, 0f), new Vector3(80f, 0f, 0f), this.grabberClawChildCurve.Evaluate(this.grabberClawChildLerp));
			for (int i = 0; i < this.grabberClawChildren.Length; i++)
			{
				this.grabberClawChildren[i].localRotation = Quaternion.Euler(euler);
			}
			float num = Mathf.LerpUnclamped(500f, 200f, this.grabberClawChildCurve.Evaluate(this.grabberClawChildLerp));
			this.grabberClawRotation += num * this.deltaTime;
			if (this.grabberClawRotation > 360f)
			{
				this.grabberClawRotation -= 360f;
			}
			this.grabberClawParent.localScale = Vector3.one * this.grabberClawHideCurve.Evaluate(this.grabberClawLerp);
			this.grabberClawParent.localRotation = Quaternion.Euler(0f, 0f, this.grabberClawRotation);
			this.grabberOrb.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, this.grabberClawChildCurve.Evaluate(this.grabberClawChildLerp));
			this.grabberLight.intensity = Mathf.Lerp(0f, this.grabberLightIntensity, this.grabberClawHideCurve.Evaluate(this.grabberClawLerp));
		}
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x0008A0B4 File Offset: 0x000882B4
	private void GrabberLogic()
	{
		this.GrabberClawLogic();
		this.grabberTransform.position = this.grabberTransformTarget.position;
		this.grabberTransform.localScale = this.grabberTransformTarget.localScale;
		this.grabberTransform.rotation = SemiFunc.SpringQuaternionGet(this.grabberClawSpring, this.grabberTransformTarget.rotation, this.deltaTime);
		Quaternion targetRotation = Quaternion.identity;
		Quaternion localRotation = this.rightArmParentTransform.localRotation;
		if (this.physGrabBeam.lineRenderer.enabled && !this.mapToolController.Active)
		{
			Vector3 position = this.grabberAimTarget.position;
			this.grabberAimTarget.position = this.physGrabBeam.PhysGrabPointPuller.position;
			float b = 0f;
			if (this.grabberAimTarget.localPosition.x < -1f)
			{
				b = 1f;
			}
			this.grabberAimTarget.localPosition = new Vector3(Mathf.Max(this.grabberAimTarget.localPosition.x, 0.2f), this.grabberAimTarget.localPosition.y, Mathf.Max(this.grabberAimTarget.localPosition.z, b));
			this.grabberAimTarget.position = Vector3.Lerp(position, this.grabberAimTarget.position, 30f * this.deltaTime);
			this.rightArmParentTransform.LookAt(this.grabberAimTarget);
			targetRotation = this.rightArmParentTransform.localRotation;
		}
		this.rightArmParentTransform.localRotation = localRotation;
		this.rightArmParentTransform.localRotation = SemiFunc.SpringQuaternionGet(this.grabberSteerSpring, targetRotation, this.deltaTime);
		this.grabberReachDifferenceTimer += this.deltaTime;
		if (this.grabberReachDifferenceTimer > 1f)
		{
			this.grabberReachDifference = 0f;
			this.grabberReachDifferenceTimer = 0f;
		}
		this.grabberReachDifference += this.grabberReachPrevious - this.playerAvatar.physGrabber.pullerDistance;
		this.grabberReachPrevious = this.playerAvatar.physGrabber.pullerDistance;
		if (Mathf.Abs(this.grabberReachDifference) > 1f)
		{
			if (this.grabberReachDifference < 0f)
			{
				this.grabberReachTarget = 0.2f;
			}
			else
			{
				this.grabberReachTarget = -0.2f;
			}
			this.grabberReachTimer = 0.25f;
			this.grabberReachDifference = 0f;
		}
		else
		{
			this.grabberReachTimer -= this.deltaTime;
			if (this.grabberReachTimer <= 0f)
			{
				this.grabberReachTarget = 0f;
			}
		}
		float num = SemiFunc.SpringFloatGet(this.grabberReachSpring, this.grabberReachTarget, this.deltaTime);
		this.rightArmParentTransform.localScale = new Vector3(1f, 1f, 1f + num);
		if (this.playerAvatar.physGrabber.healing)
		{
			if (!this.grabberHealing)
			{
				this.grabberLight.color = this.grabberLightColorHeal;
				GameObject[] array = this.grabberOrbSpheres;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].GetComponent<Renderer>().material = this.grabberMaterialHeal;
				}
				this.grabberHealing = true;
				return;
			}
		}
		else if (this.grabberHealing)
		{
			this.grabberLight.color = this.grabberLightColor;
			GameObject[] array = this.grabberOrbSpheres;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<Renderer>().material = this.grabberMaterial;
			}
			this.grabberHealing = false;
		}
	}

	// Token: 0x04001936 RID: 6454
	public PlayerAvatar playerAvatar;

	// Token: 0x04001937 RID: 6455
	public Transform rightArmTransform;

	// Token: 0x04001938 RID: 6456
	public Transform rightArmParentTransform;

	// Token: 0x04001939 RID: 6457
	public MapToolController mapToolController;

	// Token: 0x0400193A RID: 6458
	public PhysGrabBeam physGrabBeam;

	// Token: 0x0400193B RID: 6459
	public AnimationCurve poseCurve;

	// Token: 0x0400193C RID: 6460
	public float poseSpeed;

	// Token: 0x0400193D RID: 6461
	private float poseLerp;

	// Token: 0x0400193E RID: 6462
	private Vector3 poseNew;

	// Token: 0x0400193F RID: 6463
	private Vector3 poseOld;

	// Token: 0x04001940 RID: 6464
	private Vector3 poseCurrent;

	// Token: 0x04001941 RID: 6465
	[Space]
	public Vector3 basePose;

	// Token: 0x04001942 RID: 6466
	public Vector3 mapPose;

	// Token: 0x04001943 RID: 6467
	public Vector3 grabberPose;

	// Token: 0x04001944 RID: 6468
	private float headRotation;

	// Token: 0x04001945 RID: 6469
	public Transform grabberTransform;

	// Token: 0x04001946 RID: 6470
	public Transform grabberTransformTarget;

	// Token: 0x04001947 RID: 6471
	[Space]
	public Material grabberMaterial;

	// Token: 0x04001948 RID: 6472
	public Material grabberMaterialHeal;

	// Token: 0x04001949 RID: 6473
	[Space]
	public Transform grabberOrb;

	// Token: 0x0400194A RID: 6474
	public GameObject[] grabberOrbSpheres;

	// Token: 0x0400194B RID: 6475
	[Space]
	public Light grabberLight;

	// Token: 0x0400194C RID: 6476
	public Color grabberLightColor;

	// Token: 0x0400194D RID: 6477
	public Color grabberLightColorHeal;

	// Token: 0x0400194E RID: 6478
	private float grabberLightIntensity;

	// Token: 0x0400194F RID: 6479
	private bool grabberHealing;

	// Token: 0x04001950 RID: 6480
	[Space]
	public Transform grabberAimTarget;

	// Token: 0x04001951 RID: 6481
	[Space]
	public SpringQuaternion grabberSteerSpring;

	// Token: 0x04001952 RID: 6482
	public SpringQuaternion grabberClawSpring;

	// Token: 0x04001953 RID: 6483
	public SpringFloat grabberReachSpring;

	// Token: 0x04001954 RID: 6484
	private float grabberReachTimer;

	// Token: 0x04001955 RID: 6485
	private float grabberReachTarget;

	// Token: 0x04001956 RID: 6486
	private float grabberReachPrevious;

	// Token: 0x04001957 RID: 6487
	private float grabberReachDifference;

	// Token: 0x04001958 RID: 6488
	private float grabberReachDifferenceTimer;

	// Token: 0x04001959 RID: 6489
	[Space]
	public Transform grabberClawParent;

	// Token: 0x0400195A RID: 6490
	public Transform[] grabberClawChildren;

	// Token: 0x0400195B RID: 6491
	public AnimationCurve grabberClawHideCurve;

	// Token: 0x0400195C RID: 6492
	public AnimationCurve grabberClawChildCurve;

	// Token: 0x0400195D RID: 6493
	private float grabberClawLerp;

	// Token: 0x0400195E RID: 6494
	private float grabberClawChildLerp;

	// Token: 0x0400195F RID: 6495
	private bool grabberClawHidden;

	// Token: 0x04001960 RID: 6496
	private float grabberClawRotation;

	// Token: 0x04001961 RID: 6497
	private float deltaTime;

	// Token: 0x04001962 RID: 6498
	private PlayerAvatarVisuals playerAvatarVisuals;
}
