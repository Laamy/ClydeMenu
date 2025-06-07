using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A8 RID: 168
public class EnemyValuable : MonoBehaviour
{
	// Token: 0x060006B3 RID: 1715 RVA: 0x000409DC File Offset: 0x0003EBDC
	private void Start()
	{
		this.impactDetector = base.GetComponentInChildren<PhysGrabObjectImpactDetector>();
		this.impactDetector.indestructibleSpawnTimer = 0.1f;
		this.outerMaterial = this.outerMeshRenderer.material;
		this.fresnelPowerIndex = Shader.PropertyToID("_FresnelPower");
		this.fresnelColorIndex = Shader.PropertyToID("_FresnelColor");
		this.fresnelPowerDefault = this.outerMaterial.GetFloat(this.fresnelPowerIndex);
		this.outerMaterial.SetFloat(this.fresnelPowerIndex, this.fresnelPowerIndestructible);
		this.fresnelColorDefault = this.outerMaterial.GetColor(this.fresnelColorIndex);
		this.outerMaterial.SetColor(this.fresnelColorIndex, this.fresnelColorIndestructible);
		EnemyDirector.instance.AddEnemyValuable(this);
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x00040AA0 File Offset: 0x0003ECA0
	private void Update()
	{
		if (this.indestructibleTimer > 0f)
		{
			this.indestructibleTimer -= Time.deltaTime;
			if (this.indestructibleTimer <= 0f)
			{
				this.impactDetector.destroyDisable = false;
			}
		}
		else if (this.indestructibleLerp < 1f)
		{
			float value = Mathf.Lerp(this.fresnelPowerIndestructible, this.fresnelPowerDefault, this.indestructibleCurve.Evaluate(this.indestructibleLerp));
			this.outerMaterial.SetFloat(this.fresnelPowerIndex, value);
			Color value2 = Color.Lerp(this.fresnelColorIndestructible, this.fresnelColorDefault, this.indestructibleCurve.Evaluate(this.indestructibleLerp));
			this.outerMaterial.SetColor(this.fresnelColorIndex, value2);
			this.indestructibleLerp += 2f * Time.deltaTime;
		}
		this.innerTransform.Rotate(base.transform.up * 60f * Time.deltaTime);
		this.speckSmallTransform.Rotate(base.transform.up * 100f * Time.deltaTime);
		this.speckBigTransform.Rotate(-base.transform.up * 20f * Time.deltaTime);
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x00040C02 File Offset: 0x0003EE02
	public void Destroy()
	{
		this.impactDetector.DestroyObject(true);
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x00040C10 File Offset: 0x0003EE10
	public void DestroyImpulse()
	{
		foreach (ParticleSystem particleSystem in this.particleSystems)
		{
			particleSystem.gameObject.SetActive(true);
			particleSystem.transform.parent = null;
			particleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
		}
	}

	// Token: 0x04000B45 RID: 2885
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04000B46 RID: 2886
	private float indestructibleTimer = 5f;

	// Token: 0x04000B47 RID: 2887
	public MeshRenderer outerMeshRenderer;

	// Token: 0x04000B48 RID: 2888
	private Material outerMaterial;

	// Token: 0x04000B49 RID: 2889
	private int fresnelPowerIndex;

	// Token: 0x04000B4A RID: 2890
	private int fresnelColorIndex;

	// Token: 0x04000B4B RID: 2891
	[Space]
	private float fresnelPowerDefault;

	// Token: 0x04000B4C RID: 2892
	public float fresnelPowerIndestructible;

	// Token: 0x04000B4D RID: 2893
	[Space]
	private Color fresnelColorDefault;

	// Token: 0x04000B4E RID: 2894
	public Color fresnelColorIndestructible;

	// Token: 0x04000B4F RID: 2895
	[Space]
	public AnimationCurve indestructibleCurve;

	// Token: 0x04000B50 RID: 2896
	private float indestructibleLerp;

	// Token: 0x04000B51 RID: 2897
	[Space]
	public Transform innerTransform;

	// Token: 0x04000B52 RID: 2898
	public Transform speckSmallTransform;

	// Token: 0x04000B53 RID: 2899
	public Transform speckBigTransform;

	// Token: 0x04000B54 RID: 2900
	[Space]
	public List<ParticleSystem> particleSystems;
}
