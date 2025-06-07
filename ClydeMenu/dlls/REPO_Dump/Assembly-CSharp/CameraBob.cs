using System;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class CameraBob : MonoBehaviour
{
	// Token: 0x06000089 RID: 137 RVA: 0x0000599A File Offset: 0x00003B9A
	private void Awake()
	{
		CameraBob.Instance = this;
	}

	// Token: 0x0600008A RID: 138 RVA: 0x000059A2 File Offset: 0x00003BA2
	private void Start()
	{
		this.bobUpLerpStrengthCurrent = this.bobUpLerpStrength;
		this.bobSideLerpStrengthCurrent = this.bobSideLerpStrength;
	}

	// Token: 0x0600008B RID: 139 RVA: 0x000059BC File Offset: 0x00003BBC
	public void SetMultiplier(float multiplier, float time)
	{
		this.MultiplierTarget = multiplier;
		this.MultiplierTimer = time;
	}

	// Token: 0x0600008C RID: 140 RVA: 0x000059CC File Offset: 0x00003BCC
	private void Update()
	{
		float overrideSpeedMultiplier = PlayerController.instance.overrideSpeedMultiplier;
		float num = Time.deltaTime * overrideSpeedMultiplier;
		if (this.MultiplierTimer > 0f)
		{
			this.MultiplierTimer -= 1f * num;
		}
		else
		{
			this.MultiplierTarget = 1f;
		}
		this.Multiplier = Mathf.Lerp(this.Multiplier, this.MultiplierTarget, 5f * num);
		if (GameDirector.instance.currentState == GameDirector.gameState.Main && !PlayerController.instance.playerAvatarScript.isDisabled)
		{
			float num2 = 1f;
			float num3 = 1f;
			if (this.playerController.sprinting)
			{
				float b = this.SprintSpeedMultiplier + (float)StatsManager.instance.playerUpgradeSpeed[this.playerController.playerAvatarScript.steamID] * 0.1f;
				num2 = Mathf.Lerp(1f, b, this.playerController.SprintSpeedLerp);
			}
			else if (this.playerController.Crouching)
			{
				num2 = this.CrouchSpeedMultiplier;
				num3 = this.CrouchAmountMultiplier;
			}
			this.bobUpLerpStrengthCurrent = Mathf.Lerp(this.bobUpLerpStrengthCurrent, this.bobUpLerpStrength * num3, 5f * num);
			float b2 = Mathf.LerpUnclamped(0f, this.bobUpLerpStrengthCurrent, this.bobUpCurve.Evaluate(this.bobUpLerpAmount));
			this.bobUpLerpAmount += this.bobUpLerpSpeed * this.bobUpActiveLerp * num2 * num;
			if (this.bobUpLerpAmount > 1f)
			{
				if (this.playerController.CollisionController.Grounded && !CameraJump.instance.jumpActive)
				{
					if (this.playerController.sprinting)
					{
						this.playerController.playerAvatarScript.Footstep(Materials.SoundType.Heavy);
					}
					else if (this.bobUpActiveLerp > 0.75f && !this.playerController.Crouching)
					{
						this.playerController.playerAvatarScript.Footstep(Materials.SoundType.Medium);
					}
					else
					{
						this.playerController.playerAvatarScript.Footstep(Materials.SoundType.Light);
					}
				}
				this.bobUpLerpAmount = 0f;
			}
			if (this.playerController.moving && !this.camController.targetActive && this.playerController.CollisionController.Grounded)
			{
				this.bobUpActiveLerp = Mathf.Clamp01(this.bobUpActiveLerp + this.bobUpActiveLerpSpeedIn * num);
			}
			else
			{
				this.bobUpActiveLerp = Mathf.Clamp01(this.bobUpActiveLerp - this.bobUpActiveLerpSpeedOut * num);
			}
			this.bobSideLerpStrengthCurrent = Mathf.Lerp(this.bobSideLerpStrengthCurrent, this.bobSideLerpStrength * num3, 5f * num);
			float num4 = Mathf.LerpUnclamped(-this.bobSideLerpStrengthCurrent, this.bobSideLerpStrengthCurrent, this.bobSideCurve.Evaluate(this.bobSideLerpAmount));
			if (this.bobSideRev)
			{
				this.bobSideLerpAmount += this.bobSideLerpSpeed * this.bobSideActiveLerp * num2 * num;
				if (this.bobSideLerpAmount > 1f)
				{
					this.bobSideRev = false;
				}
			}
			else
			{
				this.bobSideLerpAmount -= this.bobSideLerpSpeed * this.bobSideActiveLerp * num2 * num;
				if (this.bobSideLerpAmount < 0f)
				{
					this.bobSideRev = true;
				}
			}
			if (this.playerController.moving && !this.camController.targetActive)
			{
				this.bobSideActiveLerp = Mathf.Clamp01(this.bobSideActiveLerp + this.bobSideActiveLerpSpeedIn * num);
			}
			else
			{
				this.bobSideActiveLerp = Mathf.Clamp01(this.bobSideActiveLerp - this.bobSideActiveLerpSpeedOut * num);
			}
			this.positionResult = new Vector3(0f, Mathf.LerpUnclamped(0f, b2, this.bobUpActiveCurve.Evaluate(this.bobUpActiveLerp)) * this.Multiplier, 0f);
			this.rotationResult = Quaternion.Euler(0f, Mathf.LerpUnclamped(0f, num4 * 10f, this.bobSideActiveCurve.Evaluate(this.bobSideActiveLerp)) * this.Multiplier, Mathf.LerpUnclamped(0f, num4 * 5f, this.bobSideActiveCurve.Evaluate(this.bobSideActiveLerp)) * this.Multiplier);
		}
		else
		{
			this.bobSideActiveLerp = 0f;
			this.bobUpActiveLerp = 0f;
			this.positionResult = Vector3.Lerp(this.positionResult, Vector3.zero, 5f * num);
			this.rotationResult = Quaternion.Slerp(this.rotationResult, Quaternion.identity, 5f * num);
		}
		base.transform.localPosition = Vector3.Lerp(Vector3.zero, this.positionResult, GameplayManager.instance.cameraAnimation);
		base.transform.localRotation = Quaternion.Slerp(Quaternion.identity, this.rotationResult, GameplayManager.instance.cameraAnimation);
	}

	// Token: 0x0400015C RID: 348
	public static CameraBob Instance;

	// Token: 0x0400015D RID: 349
	public CameraTarget camController;

	// Token: 0x0400015E RID: 350
	public PlayerController playerController;

	// Token: 0x0400015F RID: 351
	public AudioPlay footstepAudio;

	// Token: 0x04000160 RID: 352
	[Header("Bob Up")]
	public AnimationCurve bobUpCurve;

	// Token: 0x04000161 RID: 353
	public float bobUpLerpSpeed;

	// Token: 0x04000162 RID: 354
	public float bobUpLerpStrength;

	// Token: 0x04000163 RID: 355
	private float bobUpLerpStrengthCurrent;

	// Token: 0x04000164 RID: 356
	private float bobUpLerpAmount;

	// Token: 0x04000165 RID: 357
	public float bobUpActiveLerpSpeedIn = 1f;

	// Token: 0x04000166 RID: 358
	public float bobUpActiveLerpSpeedOut = 1f;

	// Token: 0x04000167 RID: 359
	private float bobUpActiveLerp;

	// Token: 0x04000168 RID: 360
	public AnimationCurve bobUpActiveCurve;

	// Token: 0x04000169 RID: 361
	[Header("Bob Side")]
	public AnimationCurve bobSideCurve;

	// Token: 0x0400016A RID: 362
	public float bobSideLerpSpeed;

	// Token: 0x0400016B RID: 363
	public float bobSideLerpStrength;

	// Token: 0x0400016C RID: 364
	private float bobSideLerpStrengthCurrent;

	// Token: 0x0400016D RID: 365
	private float bobSideLerpAmount;

	// Token: 0x0400016E RID: 366
	private bool bobSideRev;

	// Token: 0x0400016F RID: 367
	public float bobSideActiveLerpSpeedIn = 1f;

	// Token: 0x04000170 RID: 368
	public float bobSideActiveLerpSpeedOut = 1f;

	// Token: 0x04000171 RID: 369
	private float bobSideActiveLerp;

	// Token: 0x04000172 RID: 370
	public AnimationCurve bobSideActiveCurve;

	// Token: 0x04000173 RID: 371
	[Header("Other")]
	public float SprintSpeedMultiplier = 1f;

	// Token: 0x04000174 RID: 372
	public float CrouchSpeedMultiplier = 1f;

	// Token: 0x04000175 RID: 373
	public float CrouchAmountMultiplier = 1f;

	// Token: 0x04000176 RID: 374
	private float Multiplier;

	// Token: 0x04000177 RID: 375
	private float MultiplierTarget;

	// Token: 0x04000178 RID: 376
	private float MultiplierTimer;

	// Token: 0x04000179 RID: 377
	internal Vector3 positionResult;

	// Token: 0x0400017A RID: 378
	internal Quaternion rotationResult;
}
