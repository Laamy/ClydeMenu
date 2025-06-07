using System;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class FlashlightController : MonoBehaviour
{
	// Token: 0x060006BA RID: 1722 RVA: 0x00040D14 File Offset: 0x0003EF14
	private void Start()
	{
		if (this.PlayerAvatar.isLocal)
		{
			FlashlightController.Instance = this;
			base.transform.parent = this.FollowTransformLocal;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
		}
		else
		{
			Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer("Triggers");
			}
			this.toolBackAway.Active = false;
			this.lightOnAudio.SpatialBlend = 1f;
			this.lightOnAudio.Volume *= 0.5f;
			this.lightOffAudio.SpatialBlend = 1f;
			this.lightOffAudio.Volume *= 0.5f;
		}
		this.mesh.enabled = false;
		this.spotlight.enabled = false;
		this.halo.enabled = false;
		this.LightActive = false;
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x00040E1C File Offset: 0x0003F01C
	private void Update()
	{
		if (GameDirector.instance.currentState >= GameDirector.gameState.Main && RunManager.instance.levelCurrent != RunManager.instance.levelLobby && RunManager.instance.levelCurrent != RunManager.instance.levelShop && !SemiFunc.MenuLevel() && !this.hideFlashlight)
		{
			if (this.PlayerAvatar.isDisabled || this.PlayerAvatar.isCrouching || this.PlayerAvatar.isTumbling)
			{
				this.active = false;
				if ((this.PlayerAvatar.isTumbling || this.PlayerAvatar.isSliding) && this.currentState < FlashlightController.State.Idle && this.currentState != FlashlightController.State.Hidden)
				{
					this.currentState = FlashlightController.State.Idle;
				}
			}
			else
			{
				this.active = true;
			}
		}
		else
		{
			this.active = false;
		}
		if (this.PlayerAvatar.isDisabled && this.currentState != FlashlightController.State.Hidden)
		{
			this.currentState = FlashlightController.State.Hidden;
			this.mesh.enabled = false;
			this.spotlight.enabled = false;
			this.halo.enabled = false;
			this.LightActive = false;
			this.hiddenScale = 0f;
		}
		if (this.currentState == FlashlightController.State.Hidden)
		{
			this.Hidden();
		}
		else if (this.currentState == FlashlightController.State.Intro)
		{
			this.Intro();
		}
		else if (this.currentState == FlashlightController.State.LightOn)
		{
			this.LightOn();
		}
		else if (this.currentState == FlashlightController.State.Idle)
		{
			this.Idle();
		}
		else if (this.currentState == FlashlightController.State.LightOff)
		{
			this.LightOff();
		}
		else if (this.currentState == FlashlightController.State.Outro)
		{
			this.Outro();
		}
		if (!this.PlayerAvatar.isLocal)
		{
			base.transform.position = this.FollowTransformClient.position;
			base.transform.rotation = this.FollowTransformClient.rotation;
			base.transform.localScale = this.FollowTransformClient.localScale * this.hiddenScale;
		}
		else
		{
			base.transform.localScale = Vector3.one * this.hiddenScale;
		}
		float intensity = this.baseIntensity;
		if (RoundDirector.instance.allExtractionPointsCompleted)
		{
			this.flickerMultiplier = Mathf.Lerp(this.flickerMultiplier, this.flickerMultiplierTarget, 10f * Time.deltaTime);
			intensity = (this.baseIntensity + this.flickerIntensity) * this.flickerMultiplier;
			if (this.flickerLerp < 1f)
			{
				this.flickerLerp += 1.5f * Time.deltaTime;
				this.flickerIntensity = this.flickerCurve.Evaluate(this.flickerLerp) * 0.15f;
			}
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				if (this.flickerTimer <= 0f)
				{
					this.flickerTimer = Random.Range(2f, 10f);
					this.PlayerAvatar.FlashlightFlicker(Random.Range(0.25f, 0.35f));
				}
				else
				{
					this.flickerTimer -= Time.deltaTime;
				}
			}
		}
		this.spotlight.intensity = intensity;
		this.ClickAnim();
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x00041117 File Offset: 0x0003F317
	private void Hidden()
	{
		if (this.active)
		{
			this.currentState = FlashlightController.State.Intro;
			this.stateTimer = 1f;
		}
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x00041134 File Offset: 0x0003F334
	private void Intro()
	{
		this.mesh.enabled = true;
		float num = Mathf.LerpUnclamped(this.hiddenRot, 0f, this.introCurveRot.Evaluate(this.introRotLerp));
		this.hideTransform.localRotation = Quaternion.Euler(0f, -num, -num);
		float num2 = this.introRotSpeed;
		if (!this.PlayerAvatar.isLocal)
		{
			num2 *= 2f;
		}
		this.introRotLerp += num2 * Time.deltaTime;
		this.introRotLerp = Mathf.Clamp01(this.introRotLerp);
		this.hiddenScale = Mathf.LerpUnclamped(0f, 1f, this.introCurveScale.Evaluate(this.introRotLerp));
		if (this.PlayerAvatar.isLocal)
		{
			float y = Mathf.LerpUnclamped(this.hiddenY, 0f, this.introCurveY.Evaluate(this.introYLerp));
			this.hideTransform.localPosition = new Vector3(0f, y, 0f);
			this.introYLerp += this.introYSpeed * Time.deltaTime;
			this.introYLerp = Mathf.Clamp01(this.introYLerp);
		}
		else
		{
			this.hideTransform.localPosition = Vector3.zero;
		}
		if (this.stateTimer <= 0f)
		{
			this.currentState = FlashlightController.State.LightOn;
			this.stateTimer = 0.5f;
			this.introRotLerp = 0f;
			this.introYLerp = 0f;
			this.click = true;
			this.lightOnAudio.Play(base.transform.position, 1f, 1f, 1f, 1f);
			return;
		}
		this.stateTimer -= Time.deltaTime;
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x000412F0 File Offset: 0x0003F4F0
	private void LightOn()
	{
		this.spotlight.enabled = true;
		this.halo.enabled = true;
		this.LightActive = true;
		this.baseIntensity = Mathf.LerpUnclamped(0f, 1f, this.lightOnCurve.Evaluate(this.lightOnLerp));
		this.lightOnLerp += this.lightOnSpeed * Time.deltaTime;
		this.lightOnLerp = Mathf.Clamp01(this.lightOnLerp);
		if (this.stateTimer <= 0f)
		{
			this.currentState = FlashlightController.State.Idle;
			this.lightOnLerp = 0f;
			return;
		}
		this.stateTimer -= Time.deltaTime;
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x000413A0 File Offset: 0x0003F5A0
	private void Idle()
	{
		if (!this.active)
		{
			this.currentState = FlashlightController.State.LightOff;
			this.stateTimer = 0.25f;
			if (this.PlayerAvatar.isTumbling || this.PlayerAvatar.isSliding)
			{
				this.stateTimer = 0f;
			}
			this.click = true;
			this.lightOffAudio.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00041420 File Offset: 0x0003F620
	private void LightOff()
	{
		this.spotlight.enabled = false;
		this.halo.enabled = false;
		this.LightActive = false;
		if (this.stateTimer <= 0f)
		{
			this.currentState = FlashlightController.State.Outro;
			this.stateTimer = 1f;
			if (this.PlayerAvatar.isTumbling || this.PlayerAvatar.isSliding)
			{
				this.stateTimer = 0.25f;
				return;
			}
		}
		else
		{
			this.stateTimer -= Time.deltaTime;
		}
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x000414A4 File Offset: 0x0003F6A4
	private void Outro()
	{
		float num = Mathf.LerpUnclamped(0f, this.hiddenRot, this.outroCurveRot.Evaluate(this.outroRotLerp));
		this.hideTransform.localRotation = Quaternion.Euler(0f, num, -num);
		float num2 = this.outroRotSpeed;
		if (this.PlayerAvatar.isTumbling || this.PlayerAvatar.isSliding)
		{
			num2 *= 5f;
		}
		else if (!this.PlayerAvatar.isLocal)
		{
			num2 *= 2f;
		}
		this.outroRotLerp += num2 * Time.deltaTime;
		this.outroRotLerp = Mathf.Clamp01(this.outroRotLerp);
		this.hiddenScale = Mathf.LerpUnclamped(1f, 0f, this.outroCurveScale.Evaluate(this.outroRotLerp));
		if (this.PlayerAvatar.isLocal)
		{
			float y = Mathf.LerpUnclamped(0f, this.hiddenY, this.outroCurveY.Evaluate(this.outroYLerp));
			this.hideTransform.localPosition = new Vector3(0f, y, 0f);
			float num3 = this.outroYSpeed;
			if (this.PlayerAvatar.isTumbling || this.PlayerAvatar.isSliding)
			{
				num3 *= 5f;
			}
			this.outroYLerp += num3 * Time.deltaTime;
			this.outroYLerp = Mathf.Clamp01(this.outroYLerp);
		}
		else
		{
			this.hideTransform.localPosition = Vector3.zero;
		}
		if (this.stateTimer <= 0f)
		{
			this.currentState = FlashlightController.State.Hidden;
			this.mesh.enabled = false;
			this.outroRotLerp = 0f;
			this.outroYLerp = 0f;
			return;
		}
		this.stateTimer -= Time.deltaTime;
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x0004166C File Offset: 0x0003F86C
	private void ClickAnim()
	{
		if (this.click)
		{
			float num = Mathf.LerpUnclamped(0f, this.clickStrength, this.clickCurve.Evaluate(this.clickLerp));
			this.clickTransform.localRotation = Quaternion.Euler(0f, -num, 0f);
			this.clickLerp += this.clickSpeed * Time.deltaTime;
			this.clickLerp = Mathf.Clamp01(this.clickLerp);
			if (this.clickLerp == 1f)
			{
				this.clickLerp = 0f;
				this.click = false;
			}
		}
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x0004170B File Offset: 0x0003F90B
	public void FlickerSet(float _multiplier)
	{
		this.flickerLerp = 0f;
		this.flickerMultiplierTarget = _multiplier;
	}

	// Token: 0x04000B56 RID: 2902
	public static FlashlightController Instance;

	// Token: 0x04000B57 RID: 2903
	public Transform FollowTransformLocal;

	// Token: 0x04000B58 RID: 2904
	public Transform FollowTransformClient;

	// Token: 0x04000B59 RID: 2905
	internal bool hideFlashlight;

	// Token: 0x04000B5A RID: 2906
	[Space]
	public PlayerAvatar PlayerAvatar;

	// Token: 0x04000B5B RID: 2907
	public MeshRenderer mesh;

	// Token: 0x04000B5C RID: 2908
	public Light spotlight;

	// Token: 0x04000B5D RID: 2909
	public Behaviour halo;

	// Token: 0x04000B5E RID: 2910
	public Transform hideTransform;

	// Token: 0x04000B5F RID: 2911
	public Transform clickTransform;

	// Token: 0x04000B60 RID: 2912
	public ToolBackAway toolBackAway;

	// Token: 0x04000B61 RID: 2913
	internal bool active;

	// Token: 0x04000B62 RID: 2914
	internal FlashlightController.State currentState;

	// Token: 0x04000B63 RID: 2915
	private float stateTimer;

	// Token: 0x04000B64 RID: 2916
	[HideInInspector]
	public bool LightActive;

	// Token: 0x04000B65 RID: 2917
	[Header("Hidden")]
	public float hiddenRot;

	// Token: 0x04000B66 RID: 2918
	public float hiddenY;

	// Token: 0x04000B67 RID: 2919
	private float hiddenScale;

	// Token: 0x04000B68 RID: 2920
	[Header("Intro")]
	public AnimationCurve introCurveScale;

	// Token: 0x04000B69 RID: 2921
	public AnimationCurve introCurveRot;

	// Token: 0x04000B6A RID: 2922
	public AnimationCurve introCurveY;

	// Token: 0x04000B6B RID: 2923
	public float introRotSpeed;

	// Token: 0x04000B6C RID: 2924
	private float introRotLerp;

	// Token: 0x04000B6D RID: 2925
	public float introYSpeed;

	// Token: 0x04000B6E RID: 2926
	private float introYLerp;

	// Token: 0x04000B6F RID: 2927
	[Header("Light")]
	public AnimationCurve lightOnCurve;

	// Token: 0x04000B70 RID: 2928
	public float lightOnSpeed;

	// Token: 0x04000B71 RID: 2929
	private float lightOnLerp;

	// Token: 0x04000B72 RID: 2930
	public AnimationCurve clickCurve;

	// Token: 0x04000B73 RID: 2931
	public float clickSpeed;

	// Token: 0x04000B74 RID: 2932
	public float clickStrength;

	// Token: 0x04000B75 RID: 2933
	private float clickLerp;

	// Token: 0x04000B76 RID: 2934
	private bool click;

	// Token: 0x04000B77 RID: 2935
	private float baseIntensity;

	// Token: 0x04000B78 RID: 2936
	public Sound lightOnAudio;

	// Token: 0x04000B79 RID: 2937
	public Sound lightOffAudio;

	// Token: 0x04000B7A RID: 2938
	[Header("Outro")]
	public AnimationCurve outroCurveScale;

	// Token: 0x04000B7B RID: 2939
	public AnimationCurve outroCurveRot;

	// Token: 0x04000B7C RID: 2940
	public AnimationCurve outroCurveY;

	// Token: 0x04000B7D RID: 2941
	public float outroRotSpeed;

	// Token: 0x04000B7E RID: 2942
	private float outroRotLerp;

	// Token: 0x04000B7F RID: 2943
	public float outroYSpeed;

	// Token: 0x04000B80 RID: 2944
	private float outroYLerp;

	// Token: 0x04000B81 RID: 2945
	[Header("Flicker")]
	public AnimationCurve flickerCurve;

	// Token: 0x04000B82 RID: 2946
	private float flickerIntensity;

	// Token: 0x04000B83 RID: 2947
	private float flickerMultiplier = 0.5f;

	// Token: 0x04000B84 RID: 2948
	private float flickerMultiplierTarget = 0.5f;

	// Token: 0x04000B85 RID: 2949
	private float flickerLerp;

	// Token: 0x04000B86 RID: 2950
	private float flickerTimer;

	// Token: 0x02000330 RID: 816
	internal enum State
	{
		// Token: 0x0400298C RID: 10636
		Hidden,
		// Token: 0x0400298D RID: 10637
		Intro,
		// Token: 0x0400298E RID: 10638
		LightOn,
		// Token: 0x0400298F RID: 10639
		Idle,
		// Token: 0x04002990 RID: 10640
		LightOff,
		// Token: 0x04002991 RID: 10641
		Outro
	}
}
