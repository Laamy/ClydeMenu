using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000147 RID: 327
public class ItemLight : MonoBehaviour
{
	// Token: 0x06000B27 RID: 2855 RVA: 0x000633D8 File Offset: 0x000615D8
	private void Start()
	{
		this.physGrabObject = base.GetComponentInParent<PhysGrabObject>();
		this.lightIntensityOriginal = this.itemLight.intensity;
		this.lightRangeOriginal = this.itemLight.range;
		this.itemLight.intensity = 0f;
		this.itemLight.range = 0f;
		this.itemLight.enabled = false;
		if (this.meshRenderers.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.meshRenderers)
			{
				if (meshRenderer && meshRenderer.gameObject.activeSelf && meshRenderer && meshRenderer.gameObject.activeSelf)
				{
					Material material = meshRenderer.material;
					this.fresnelScaleOriginal = material.GetFloat("_FresnelScale");
					break;
				}
			}
		}
		if (this.alwaysActive)
		{
			this.itemLight.enabled = true;
			this.showLight = true;
			this.itemLight.intensity = this.lightIntensityOriginal;
			this.itemLight.range = this.lightRangeOriginal;
		}
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x00063510 File Offset: 0x00061710
	private void SetAllFresnel(float _value)
	{
		if (this.meshRenderers.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.meshRenderers)
			{
				if (meshRenderer && meshRenderer.gameObject.activeSelf)
				{
					meshRenderer.material.SetFloat("_FresnelScale", _value);
				}
			}
		}
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x00063590 File Offset: 0x00061790
	private void Update()
	{
		if (this.showLight)
		{
			if (!this.itemLight.enabled)
			{
				this.itemLight.intensity = 0f;
				this.itemLight.range = 0f;
				this.animationCurveEval = 0f;
				this.itemLight.enabled = true;
			}
			if (this.itemLight.intensity < this.lightIntensityOriginal - 0.01f)
			{
				this.animationCurveEval += Time.deltaTime * 0.05f;
				float t = this.lightIntensityCurve.Evaluate(this.animationCurveEval);
				if (this.meshRenderers.Count > 0)
				{
					foreach (MeshRenderer meshRenderer in this.meshRenderers)
					{
						if (meshRenderer && meshRenderer.gameObject.activeSelf)
						{
							Material material = meshRenderer.material;
							float @float = material.GetFloat("_FresnelScale");
							material.SetFloat("_FresnelScale", Mathf.Lerp(@float, this.fresnelScaleOriginal, t));
						}
					}
				}
				this.itemLight.intensity = Mathf.Lerp(this.itemLight.intensity, this.lightIntensityOriginal, t);
				this.itemLight.range = Mathf.Lerp(this.itemLight.range, this.lightRangeOriginal, t);
			}
		}
		else if (this.itemLight.enabled)
		{
			this.animationCurveEval += Time.deltaTime * 1f;
			float t2 = this.lightIntensityCurve.Evaluate(this.animationCurveEval);
			this.itemLight.intensity = Mathf.Lerp(this.itemLight.intensity, 0f, t2);
			this.itemLight.range = Mathf.Lerp(this.itemLight.range, 0f, t2);
			if (this.meshRenderers.Count > 0)
			{
				foreach (MeshRenderer meshRenderer2 in this.meshRenderers)
				{
					if (meshRenderer2 && meshRenderer2.gameObject.activeSelf)
					{
						Material material2 = meshRenderer2.material;
						float float2 = material2.GetFloat("_FresnelScale");
						material2.SetFloat("_FresnelScale", Mathf.Lerp(float2, 0f, t2));
					}
				}
			}
			if (this.itemLight.intensity < 0.01f)
			{
				this.animationCurveEval = 0f;
				this.itemLight.intensity = 0f;
				this.itemLight.range = 0f;
				if (this.meshRenderers.Count > 0)
				{
					foreach (MeshRenderer meshRenderer3 in this.meshRenderers)
					{
						if (meshRenderer3 && meshRenderer3.gameObject.activeSelf)
						{
							meshRenderer3.material.SetFloat("_FresnelScale", 0f);
						}
					}
				}
				this.itemLight.enabled = false;
			}
		}
		if (SemiFunc.FPSImpulse1())
		{
			if (SemiFunc.PlayerGetNearestTransformWithinRange(16f, base.transform.position, false, default(LayerMask)))
			{
				this.culledLight = false;
			}
			else
			{
				this.culledLight = true;
			}
			if (!this.alwaysActive)
			{
				if (this.culledLight)
				{
					this.showLight = false;
					return;
				}
				if (this.physGrabObject.grabbed)
				{
					this.showLight = false;
					return;
				}
				if (!this.showLight)
				{
					this.itemLight.enabled = true;
					this.showLight = true;
					return;
				}
			}
			else
			{
				if (this.culledLight)
				{
					this.showLight = false;
					return;
				}
				this.showLight = true;
			}
		}
	}

	// Token: 0x04001201 RID: 4609
	public bool alwaysActive;

	// Token: 0x04001202 RID: 4610
	public Light itemLight;

	// Token: 0x04001203 RID: 4611
	private float lightIntensityOriginal;

	// Token: 0x04001204 RID: 4612
	private float lightRangeOriginal;

	// Token: 0x04001205 RID: 4613
	private bool showLight = true;

	// Token: 0x04001206 RID: 4614
	private PhysGrabObject physGrabObject;

	// Token: 0x04001207 RID: 4615
	private bool culledLight;

	// Token: 0x04001208 RID: 4616
	public AnimationCurve lightIntensityCurve;

	// Token: 0x04001209 RID: 4617
	private float animationCurveEval;

	// Token: 0x0400120A RID: 4618
	public List<MeshRenderer> meshRenderers;

	// Token: 0x0400120B RID: 4619
	private float fresnelScaleOriginal;
}
