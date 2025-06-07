using System;
using UnityEngine;

// Token: 0x02000156 RID: 342
public class ItemShockwave : MonoBehaviour
{
	// Token: 0x06000BAC RID: 2988 RVA: 0x000679D8 File Offset: 0x00065BD8
	private void Start()
	{
		this.startScale = base.transform.localScale.x;
		this.lightShockwave = base.GetComponentInChildren<Light>();
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>();
		this.meshRenderer.material.color = Color.white;
		base.transform.localScale = Vector3.zero;
		this.soundExplosion.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.soundExplosionGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.particleSystemSparks.Play();
		this.particleSystemLightning.Play();
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(20f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x00067B00 File Offset: 0x00065D00
	private void Update()
	{
		base.transform.Rotate(Vector3.up, 100f * Time.deltaTime);
		if (base.transform.localScale.x < this.startScale)
		{
			base.transform.localScale += Vector3.one * Time.deltaTime * 20f;
			this.lightShockwave.intensity = Mathf.Lerp(4f, 35f, Mathf.InverseLerp(0f, this.startScale, base.transform.localScale.x));
			this.lightShockwave.range = base.transform.localScale.x * 3f;
			return;
		}
		if (!this.finalScale)
		{
			base.transform.localScale = Vector3.one * this.startScale;
			this.hurtCollider.gameObject.SetActive(false);
			this.finalScale = true;
			return;
		}
		float num = Mathf.Lerp(base.transform.localScale.x, this.startScale * 1.2f, Time.deltaTime * 2f);
		base.transform.localScale = Vector3.one * num;
		float num2 = Mathf.InverseLerp(this.startScale, this.startScale * 1.2f, num);
		Color color = this.meshRenderer.material.color;
		color.a = Mathf.Lerp(1f, 0f, num2);
		this.meshRenderer.material.color = color;
		this.lightShockwave.intensity = Mathf.Lerp(35f, 0f, num2);
		if (num2 > 0.998f)
		{
			if (this.particleSystemSparks)
			{
				this.particleSystemSparks.transform.parent = null;
			}
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040012F6 RID: 4854
	public MeshRenderer meshRenderer;

	// Token: 0x040012F7 RID: 4855
	private float startScale = 1f;

	// Token: 0x040012F8 RID: 4856
	private bool finalScale;

	// Token: 0x040012F9 RID: 4857
	private Light lightShockwave;

	// Token: 0x040012FA RID: 4858
	public ParticleSystem particleSystemWave;

	// Token: 0x040012FB RID: 4859
	public ParticleSystem particleSystemSparks;

	// Token: 0x040012FC RID: 4860
	public ParticleSystem particleSystemLightning;

	// Token: 0x040012FD RID: 4861
	private HurtCollider hurtCollider;

	// Token: 0x040012FE RID: 4862
	public Sound soundExplosion;

	// Token: 0x040012FF RID: 4863
	public Sound soundExplosionGlobal;
}
