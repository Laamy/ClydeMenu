using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000C3 RID: 195
public class PickerController : MonoBehaviour
{
	// Token: 0x06000713 RID: 1811 RVA: 0x0004324C File Offset: 0x0004144C
	private void Start()
	{
		this.IntroSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.AnimatedPicker.SetActive(false);
		this.StabPoint.SetActive(false);
		this.MainCamera = Camera.main;
		this.Mask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x000432E0 File Offset: 0x000414E0
	private void Update()
	{
		if (this.OutroAudioPlay && !ToolController.instance.ToolHide.Active)
		{
			this.OutroSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.OutroAudioPlay = false;
			GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		}
		if (this.isAnimating)
		{
			this.AnimatePicker();
		}
		this.AlignStabObjects();
		if (this.ShowTimer > 0f)
		{
			this.ShowTimer -= Time.deltaTime;
		}
		if (ToolController.instance.Interact)
		{
			this.isStabbing = true;
		}
		Interaction activeInteraction = ToolController.instance.ActiveInteraction;
		if (activeInteraction && !this.isAnimating && this.ShowTimer <= 0f && ToolController.instance.ToolHide.Active && this.isStabbing)
		{
			this.StabPoint.SetActive(true);
			PaperPick component = activeInteraction.GetComponent<PaperPick>();
			this.stabObject = component.PaperInteraction.GameObject();
			this.StabPoint.transform.position = component.PaperInteraction.PaperTransform.position;
			Vector3 forward = this.StabPoint.transform.position - base.transform.position;
			this.StabPoint.transform.rotation = Quaternion.LookRotation(forward);
			GameDirector.instance.CameraShake.Shake(3f, 0.2f);
			this.StartAnimation();
			this.isStabbing = false;
		}
		base.transform.position = ToolController.instance.ToolFollow.transform.position;
		base.transform.rotation = ToolController.instance.ToolFollow.transform.rotation;
		base.transform.localScale = ToolController.instance.ToolHide.transform.localScale;
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x000434E8 File Offset: 0x000416E8
	private void AlignStabObjects()
	{
		for (int i = 0; i < this.stabObjects.Count; i++)
		{
			Vector3 position = this.PickerPoint.position;
			Vector3 position2 = this.PickerPointEnd.position;
			if (this.isAnimating)
			{
				position = this.PickerPointAnimate.position;
				position2 = this.PickerPointEndAnimate.position;
			}
			float num = this.StabObjectSpacing * (float)i;
			float num2 = Vector3.Distance(position, position2);
			float t = num / num2;
			this.stabObjects[i].transform.position = Vector3.Lerp(position, position2, t);
			this.stabObjects[i].transform.LookAt(base.transform.position);
			this.stabObjects[i].transform.Rotate(90f, 0f, 0f, Space.Self);
			this.stabObjects[i].transform.Rotate(0f, this.stabObjectsAngles[i], 0f, Space.Self);
		}
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x000435F4 File Offset: 0x000417F4
	private void AnimatePicker()
	{
		if (this.animationProgress < 2f)
		{
			ToolController.instance.ForceActiveTimer = 0.1f;
			int num = 0;
			if (!this.introAnimation)
			{
				num = 1;
			}
			if (!this.introAnimation)
			{
				this.animationProgress += Time.deltaTime * this.AnimationSpeedOutro;
				float t = this.PickerStabOutro.Evaluate(this.animationProgress - (float)num);
				this.AnimatedPicker.transform.position = Vector3.LerpUnclamped(this.PositionStart, this.meshObject.transform.position, t);
				this.AnimatedPicker.transform.rotation = Quaternion.LerpUnclamped(this.RotationStart, this.meshObject.transform.rotation, t);
				this.AnimatedPicker.transform.localScale = Vector3.LerpUnclamped(this.ScaleStart, this.meshObject.transform.localScale, t);
			}
			else
			{
				this.animationProgress += Time.deltaTime * this.AnimationSpeedIntro;
				float t2 = this.PickerStabIntro.Evaluate(this.animationProgress - (float)num);
				this.AnimatedPicker.transform.position = Vector3.LerpUnclamped(this.PositionStart, this.StabPointChild.transform.position, t2);
				this.AnimatedPicker.transform.rotation = Quaternion.LerpUnclamped(this.RotationStart, this.StabPointChild.transform.rotation, t2);
				this.AnimatedPicker.transform.localScale = Vector3.LerpUnclamped(this.ScaleStart, this.StabPointChild.transform.localScale, t2);
			}
			if (this.animationProgress > 1f && !this.stab)
			{
				GameDirector.instance.CameraImpact.Shake(3f, 0f);
				PaperInteraction component = this.stabObject.GetComponent<PaperInteraction>();
				component.Picked = true;
				component.CleanEffect.Clean();
				component.CleanEffect.transform.parent = null;
				GameObject paperVisual = component.paperVisual;
				paperVisual.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
				paperVisual.layer = LayerMask.NameToLayer("TopLayer");
				this.StabSound.Play(paperVisual.transform.position, 1f, 1f, 1f, 1f);
				paperVisual.transform.parent = this.ParentTransform;
				this.stabObjects.Add(paperVisual);
				this.stabObjectsAngles.Add((float)Random.Range(0, 360));
				this.introAnimation = false;
				this.stab = true;
				this.AnimationSet(this.introAnimation);
				GameDirector.instance.CameraShake.Shake(2f, 0.25f);
				return;
			}
		}
		else
		{
			this.EndAnimation();
		}
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x000438C0 File Offset: 0x00041AC0
	private void AnimationSet(bool intro)
	{
		if (intro)
		{
			this.RotationStart = this.meshObject.transform.rotation;
			this.PositionStart = this.meshObject.transform.position;
			this.ScaleStart = this.meshObject.transform.localScale;
		}
		else
		{
			this.RotationStart = this.StabPointChild.transform.rotation;
			this.PositionStart = this.StabPointChild.transform.position;
			this.ScaleStart = this.StabPointChild.transform.localScale;
		}
		this.AnimatedPicker.transform.rotation = this.RotationStart;
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x0004396C File Offset: 0x00041B6C
	public void StartAnimation()
	{
		this.isAnimating = true;
		this.animationProgress = 0f;
		this.StabPoint.SetActive(true);
		this.AnimatedPicker.SetActive(true);
		this.AnimatedPicker.transform.position = base.transform.position;
		this.AnimatedPicker.transform.rotation = base.transform.rotation;
		this.AnimatedPicker.transform.localScale = base.transform.localScale;
		this.meshRenderer.enabled = false;
		this.AnimationSet(this.introAnimation);
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x00043A0C File Offset: 0x00041C0C
	private void EndAnimation()
	{
		this.stab = false;
		this.introAnimation = true;
		this.isAnimating = false;
		this.StabPoint.SetActive(false);
		this.AnimatedPicker.SetActive(false);
		this.meshRenderer.enabled = true;
	}

	// Token: 0x04000C15 RID: 3093
	public Transform ParentTransform;

	// Token: 0x04000C16 RID: 3094
	public AnimationCurve PickerStabIntro;

	// Token: 0x04000C17 RID: 3095
	public AnimationCurve PickerStabOutro;

	// Token: 0x04000C18 RID: 3096
	public float AnimationSpeedIntro = 1f;

	// Token: 0x04000C19 RID: 3097
	public float AnimationSpeedOutro = 1f;

	// Token: 0x04000C1A RID: 3098
	public GameObject AnimatedPicker;

	// Token: 0x04000C1B RID: 3099
	[Space]
	public GameObject StabPoint;

	// Token: 0x04000C1C RID: 3100
	public GameObject StabPointChild;

	// Token: 0x04000C1D RID: 3101
	[Space]
	private LayerMask Mask;

	// Token: 0x04000C1E RID: 3102
	private Camera MainCamera;

	// Token: 0x04000C1F RID: 3103
	public GameObject meshObject;

	// Token: 0x04000C20 RID: 3104
	public MeshRenderer meshRenderer;

	// Token: 0x04000C21 RID: 3105
	private bool isAnimating;

	// Token: 0x04000C22 RID: 3106
	private float animationProgress;

	// Token: 0x04000C23 RID: 3107
	private float ShowTimer = 0.3f;

	// Token: 0x04000C24 RID: 3108
	private bool stab;

	// Token: 0x04000C25 RID: 3109
	private GameObject stabObject;

	// Token: 0x04000C26 RID: 3110
	private List<GameObject> stabObjects = new List<GameObject>();

	// Token: 0x04000C27 RID: 3111
	private List<float> stabObjectsAngles = new List<float>();

	// Token: 0x04000C28 RID: 3112
	[Space]
	public Transform PickerPoint;

	// Token: 0x04000C29 RID: 3113
	public Transform PickerPointEnd;

	// Token: 0x04000C2A RID: 3114
	public Transform PickerPointAnimate;

	// Token: 0x04000C2B RID: 3115
	public Transform PickerPointEndAnimate;

	// Token: 0x04000C2C RID: 3116
	public float StabObjectSpacing = 10f;

	// Token: 0x04000C2D RID: 3117
	private bool isStabbing;

	// Token: 0x04000C2E RID: 3118
	private bool introAnimation = true;

	// Token: 0x04000C2F RID: 3119
	private Quaternion RotationStart;

	// Token: 0x04000C30 RID: 3120
	private Quaternion RotationEnd;

	// Token: 0x04000C31 RID: 3121
	private Vector3 PositionStart;

	// Token: 0x04000C32 RID: 3122
	private Vector3 PositionEnd;

	// Token: 0x04000C33 RID: 3123
	private Vector3 ScaleStart;

	// Token: 0x04000C34 RID: 3124
	private Vector3 ScaleEnd;

	// Token: 0x04000C35 RID: 3125
	[Space]
	public Sound IntroSound;

	// Token: 0x04000C36 RID: 3126
	public Sound OutroSound;

	// Token: 0x04000C37 RID: 3127
	public Sound StabSound;

	// Token: 0x04000C38 RID: 3128
	private bool OutroAudioPlay = true;
}
