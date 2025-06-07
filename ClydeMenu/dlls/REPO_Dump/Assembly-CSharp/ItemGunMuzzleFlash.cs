using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class ItemGunMuzzleFlash : MonoBehaviour
{
	// Token: 0x06000CAE RID: 3246 RVA: 0x00070100 File Offset: 0x0006E300
	public void ActivateAllEffects()
	{
		base.gameObject.SetActive(true);
		this.smoke = base.transform.Find("Particle Smoke").GetComponent<ParticleSystem>();
		this.impact = base.transform.Find("Particle Impact").GetComponent<ParticleSystem>();
		this.sparks = base.transform.Find("Particle Sparks").GetComponent<ParticleSystem>();
		this.shootLight = base.GetComponentInChildren<Light>();
		this.smoke.gameObject.SetActive(true);
		this.impact.gameObject.SetActive(true);
		this.sparks.gameObject.SetActive(true);
		this.shootLight.enabled = true;
		this.smoke.Play();
		this.impact.Play();
		this.sparks.Play();
		base.StartCoroutine(this.MuzzleFlashDestroy());
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x000701E3 File Offset: 0x0006E3E3
	private IEnumerator MuzzleFlashDestroy()
	{
		yield return new WaitForSeconds(0.1f);
		while (this.smoke.isPlaying || this.impact.isPlaying || this.sparks.isPlaying || this.shootLight.enabled)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x000701F4 File Offset: 0x0006E3F4
	private void Update()
	{
		if (this.shootLight)
		{
			this.shootLight.intensity = Mathf.Lerp(this.shootLight.intensity, 0f, Time.deltaTime * 10f);
			if (this.shootLight.intensity < 0.01f)
			{
				this.shootLight.enabled = false;
			}
		}
	}

	// Token: 0x0400146D RID: 5229
	private ParticleSystem smoke;

	// Token: 0x0400146E RID: 5230
	private ParticleSystem impact;

	// Token: 0x0400146F RID: 5231
	private ParticleSystem sparks;

	// Token: 0x04001470 RID: 5232
	private Light shootLight;
}
