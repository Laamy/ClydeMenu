using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200019F RID: 415
[RequireComponent(typeof(LineRenderer))]
public class PhysGrabBeam : MonoBehaviour
{
	// Token: 0x06000DCB RID: 3531 RVA: 0x0007859C File Offset: 0x0007679C
	private void Start()
	{
		if (!this.playerAvatar.isLocal)
		{
			this.PhysGrabPointOrigin = this.PhysGrabPointOriginClient;
		}
		this.originalScrollSpeed = this.scrollSpeed;
		this.originalMaterial = this.lineRenderer.material;
		this.lineMaterial = this.lineRenderer.material;
		this.lineMaterialOverCharge = this.lineRendererOverCharge.material;
		this.overchargeParticles.AddRange(this.overchargeImpact.GetComponentsInChildren<ParticleSystem>());
		this.overchargeImpact.parent = this.playerAvatar.transform.parent;
		this.lineRenderer.enabled = false;
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x0007863E File Offset: 0x0007683E
	private void LateUpdate()
	{
		if (!this.lineRenderer.enabled)
		{
			if (this.lineRendererOverCharge.enabled)
			{
				this.lineRendererOverCharge.enabled = false;
			}
			return;
		}
		this.DrawCurve();
		this.ScrollTexture();
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x00078674 File Offset: 0x00076874
	private void PlayAllOverchargeParticles()
	{
		foreach (ParticleSystem particleSystem in this.overchargeParticles)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x000786C4 File Offset: 0x000768C4
	private void OnEnable()
	{
		this.physGrabPointPullerSmoothPosition = this.PhysGrabPointPuller.position;
		if (VideoGreenScreen.instance)
		{
			this.lineMaterial = this.greenScreenMaterial;
			base.GetComponent<LineRenderer>().material = this.greenScreenMaterial;
		}
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x00078700 File Offset: 0x00076900
	private void OnDisable()
	{
		this.lineMaterial = this.originalMaterial;
		if (this.lineRenderer)
		{
			this.lineRenderer.material = this.originalMaterial;
		}
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x0007872C File Offset: 0x0007692C
	public void OverChargeLaunchPlayer()
	{
		if (this.playerAvatar.isLocal)
		{
			CameraGlitch.Instance.PlayLong();
			PhysGrabber.instance.physGrabBeamOverChargeFloat = 0.5f;
		}
		Vector3 vector = -this.playerAvatar.localCameraTransform.forward * 0.14f;
		vector += Vector3.up * 0.7f;
		this.overchargeImpact.rotation = Quaternion.LookRotation(vector);
		this.overchargeImpact.position = this.PhysGrabPointOrigin.position;
		this.playerAvatar.physGrabber.ReleaseObject(0.1f);
		this.playerAvatar.tumble.TumbleRequest(true, false);
		this.playerAvatar.tumble.TumbleForce(vector * 25f);
		this.playerAvatar.tumble.TumbleTorque(-this.playerAvatar.transform.forward * 45f);
		this.playerAvatar.tumble.TumbleOverrideTime(2.2f);
		this.playerAvatar.tumble.ImpactHurtSet(3f, 20);
		this.soundOverchargeImpact.Play(this.overchargeImpact.position, 1f, 1f, 1f, 1f);
		this.PlayAllOverchargeParticles();
		this.curvePointsOverChargeOffsets = new Vector3[this.CurveResolution];
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x0007889C File Offset: 0x00076A9C
	private void DrawCurve()
	{
		if (!this.PhysGrabPointPuller)
		{
			return;
		}
		bool flag = false;
		float num = (float)this.playerAvatar.physGrabber.physGrabBeamOverCharge / 2f;
		if (num <= 0.05f || !this.playerAvatar.physGrabber.grabbedPhysGrabObject || !this.playerAvatar.physGrabber.grabbedPhysGrabObject.isEnemy)
		{
			if (this.lineRendererOverCharge.enabled)
			{
				this.lineRendererOverCharge.enabled = false;
			}
		}
		else
		{
			if (!this.lineRendererOverCharge.enabled)
			{
				this.lineRendererOverCharge.enabled = true;
			}
			flag = true;
			this.lineRendererOverCharge.widthMultiplier = 0.5f * (num / 100f);
		}
		Vector3[] array = new Vector3[this.CurveResolution];
		Vector3[] array2 = new Vector3[this.CurveResolution];
		Vector3 position = this.PhysGrabPointPuller.position;
		this.physGrabPointPullerSmoothPosition = Vector3.Lerp(this.physGrabPointPullerSmoothPosition, position, Time.deltaTime * 10f);
		Vector3 p = this.physGrabPointPullerSmoothPosition * this.CurveStrength;
		if (flag && SemiFunc.FPSImpulse15())
		{
			float num2 = (float)this.playerAvatar.physGrabber.physGrabBeamOverCharge / 2f;
			num2 /= 100f;
			this.curvePointsOverChargeOffsets = new Vector3[this.CurveResolution];
			float num3 = 0.1f + 0.2f * num2;
			for (int i = 0; i < this.CurveResolution; i++)
			{
				this.curvePointsOverChargeOffsets[i] = new Vector3(Random.Range(-num3, num3), Random.Range(-num3, num3), Random.Range(-num3, num3));
			}
		}
		for (int j = 0; j < this.CurveResolution; j++)
		{
			float t = (float)j / ((float)this.CurveResolution - 1f);
			array[j] = this.CalculateBezierPoint(t, this.PhysGrabPointOrigin.position, p, this.PhysGrabPoint.position);
			if (flag && this.curvePointsOverChargeOffsets != null)
			{
				array2[j] = array[j];
				this.curvePointsOverChargeOffsets[j] = Vector3.Lerp(this.curvePointsOverChargeOffsets[j], Vector3.zero, Time.deltaTime * 30f);
				array2[j] += this.curvePointsOverChargeOffsets[j];
			}
		}
		this.lineRenderer.positionCount = this.CurveResolution;
		this.lineRenderer.SetPositions(array);
		if (flag)
		{
			this.lineRendererOverCharge.positionCount = this.CurveResolution;
			this.lineRendererOverCharge.SetPositions(array2);
		}
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x00078B4C File Offset: 0x00076D4C
	private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
	{
		return Mathf.Pow(1f - t, 2f) * p0 + 2f * (1f - t) * t * p1 + Mathf.Pow(t, 2f) * p2;
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x00078BA4 File Offset: 0x00076DA4
	private void ScrollTexture()
	{
		if (this.lineMaterial)
		{
			if (this.playerAvatar.physGrabber.colorState == 1)
			{
				this.lineMaterial.mainTextureScale = new Vector2(-1f, 1f);
			}
			else
			{
				this.lineMaterial.mainTextureScale = new Vector2(1f, 1f);
			}
			Vector2 mainTextureOffset = Time.time * this.scrollSpeed;
			this.lineMaterial.mainTextureOffset = mainTextureOffset;
			if (this.lineRendererOverCharge.enabled)
			{
				this.lineMaterialOverCharge.mainTextureOffset = mainTextureOffset;
			}
		}
	}

	// Token: 0x04001647 RID: 5703
	public PlayerAvatar playerAvatar;

	// Token: 0x04001648 RID: 5704
	public Transform PhysGrabPointOrigin;

	// Token: 0x04001649 RID: 5705
	public Transform PhysGrabPointOriginClient;

	// Token: 0x0400164A RID: 5706
	public Transform PhysGrabPoint;

	// Token: 0x0400164B RID: 5707
	public Transform PhysGrabPointPuller;

	// Token: 0x0400164C RID: 5708
	public Material greenScreenMaterial;

	// Token: 0x0400164D RID: 5709
	private Material originalMaterial;

	// Token: 0x0400164E RID: 5710
	[HideInInspector]
	public Vector3 physGrabPointPullerSmoothPosition;

	// Token: 0x0400164F RID: 5711
	public float CurveStrength = 1f;

	// Token: 0x04001650 RID: 5712
	public int CurveResolution = 20;

	// Token: 0x04001651 RID: 5713
	public LineRenderer lineRendererOverCharge;

	// Token: 0x04001652 RID: 5714
	[Header("Texture Scrolling")]
	public Vector2 scrollSpeed = new Vector2(5f, 0f);

	// Token: 0x04001653 RID: 5715
	[HideInInspector]
	public Vector2 originalScrollSpeed;

	// Token: 0x04001654 RID: 5716
	public LineRenderer lineRenderer;

	// Token: 0x04001655 RID: 5717
	[HideInInspector]
	public Material lineMaterial;

	// Token: 0x04001656 RID: 5718
	private Material lineMaterialOverCharge;

	// Token: 0x04001657 RID: 5719
	private Vector3[] curvePointsOverChargeOffsets;

	// Token: 0x04001658 RID: 5720
	public Transform overchargeImpact;

	// Token: 0x04001659 RID: 5721
	private List<ParticleSystem> overchargeParticles = new List<ParticleSystem>();

	// Token: 0x0400165A RID: 5722
	public Sound soundOverchargeImpact;
}
