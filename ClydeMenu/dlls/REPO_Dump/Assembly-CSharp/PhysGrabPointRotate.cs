using System;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class PhysGrabPointRotate : MonoBehaviour
{
	// Token: 0x06000E4C RID: 3660 RVA: 0x0007FB7C File Offset: 0x0007DD7C
	private void Start()
	{
		this.popIn = AssetManager.instance.animationCurveWooshIn;
		this.popOut = AssetManager.instance.animationCurveWooshAway;
		base.transform.localScale = Vector3.zero;
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshRenderer.material = this.originalMaterial;
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x0007FBD8 File Offset: 0x0007DDD8
	private void OnEnable()
	{
		if (!this.meshRenderer)
		{
			this.meshRenderer = base.GetComponent<MeshRenderer>();
		}
		if (this.meshRenderer)
		{
			if (!VideoGreenScreen.instance)
			{
				this.meshRenderer.material = this.originalMaterial;
				return;
			}
			this.meshRenderer.material = this.greenScreenMaterial;
		}
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x0007FC3C File Offset: 0x0007DE3C
	private void Update()
	{
		if (!this.physGrabber)
		{
			return;
		}
		if (this.physGrabber)
		{
			Vector3 mouseTurningVelocity = this.physGrabber.mouseTurningVelocity;
			if (this.physGrabber.isRotating)
			{
				this.rotationActiveTimer = 0.1f;
			}
			if (this.rotationActiveTimer > 0f)
			{
				this.physGrabber.OverrideColorToPurple(0.1f);
				base.transform.LookAt(this.physGrabber.playerAvatar.PlayerVisionTarget.VisionTransform.position);
				this.animationEval += Time.deltaTime * 2f;
				this.animationEval = Mathf.Clamp(this.animationEval, 0f, 1f);
				float d = this.popIn.Evaluate(this.animationEval);
				base.transform.localScale = Vector3.one * 0.5f * d;
				base.transform.Rotate(0f, 0f, -Mathf.Atan2(mouseTurningVelocity.y, mouseTurningVelocity.x) * 57.29578f);
				this.smoothRotation = Quaternion.Slerp(this.smoothRotation, base.transform.rotation, Time.deltaTime * 10f);
				base.transform.rotation = this.smoothRotation;
				this.rotationActiveTimer -= Time.deltaTime;
			}
			else
			{
				this.animationEval -= Time.deltaTime * 6f;
				this.animationEval = Mathf.Clamp(this.animationEval, 0f, 1f);
				float d2 = this.popOut.Evaluate(1f - this.animationEval);
				Vector3 a = Vector3.one * 0.5f;
				base.transform.localScale = a - a * d2;
			}
		}
		if (base.transform.localScale.magnitude < 0.01f)
		{
			this.meshRenderer.enabled = false;
		}
		else
		{
			if (!this.meshRenderer.enabled)
			{
				this.soundRotationStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.meshRenderer.enabled = true;
		}
		float num = this.physGrabber.mouseTurningVelocity.magnitude * 0.2f;
		bool enabled = this.meshRenderer.enabled;
		float pitchMultiplier = Mathf.Min(Mathf.Max(num / 5f, 1f), 2f);
		this.soundRotationLoop.PlayLoop(enabled, 0.5f, 1f, pitchMultiplier);
		this.offsetX -= num * 0.08f * Time.deltaTime;
		base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(this.offsetX, 0f);
		float num2 = Mathf.Sin(Time.time * 10f) * 0.2f;
		base.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f, 1f + num2);
		base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(this.offsetX, -num2 * 0.5f);
	}

	// Token: 0x0400177F RID: 6015
	internal PhysGrabber physGrabber;

	// Token: 0x04001780 RID: 6016
	private Quaternion smoothRotation;

	// Token: 0x04001781 RID: 6017
	internal float rotationActiveTimer;

	// Token: 0x04001782 RID: 6018
	private float rotationSpeed;

	// Token: 0x04001783 RID: 6019
	private float offsetX;

	// Token: 0x04001784 RID: 6020
	private AnimationCurve popIn;

	// Token: 0x04001785 RID: 6021
	private AnimationCurve popOut;

	// Token: 0x04001786 RID: 6022
	private MeshRenderer meshRenderer;

	// Token: 0x04001787 RID: 6023
	public Material originalMaterial;

	// Token: 0x04001788 RID: 6024
	public Material greenScreenMaterial;

	// Token: 0x04001789 RID: 6025
	internal float animationEval;

	// Token: 0x0400178A RID: 6026
	public Sound soundRotationStart;

	// Token: 0x0400178B RID: 6027
	public Sound soundRotationEnd;

	// Token: 0x0400178C RID: 6028
	public Sound soundRotationLoop;
}
