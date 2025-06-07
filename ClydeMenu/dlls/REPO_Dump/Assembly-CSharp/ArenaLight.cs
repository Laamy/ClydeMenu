using System;
using UnityEngine;

// Token: 0x020000D6 RID: 214
public class ArenaLight : MonoBehaviour
{
	// Token: 0x0600078C RID: 1932 RVA: 0x000480DE File Offset: 0x000462DE
	private void Start()
	{
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.arenaLight = base.GetComponentInChildren<Light>();
		this.lightIntensity = this.arenaLight.intensity;
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x0004810C File Offset: 0x0004630C
	private void Update()
	{
		if (this.arenaLight.enabled)
		{
			if (this.arenaLight.intensity > this.lightIntensity)
			{
				this.arenaLight.intensity = Mathf.Lerp(this.arenaLight.intensity, this.lightIntensity, Time.deltaTime * 2f);
				Color b = new Color(0.3f, 0f, 0f);
				this.meshRenderer.material.SetColor("_EmissionColor", Color.Lerp(this.meshRenderer.material.GetColor("_EmissionColor"), b, Time.deltaTime * 2f));
				return;
			}
			this.arenaLight.intensity = this.lightIntensity;
		}
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x000481CB File Offset: 0x000463CB
	public void TurnOnArenaWarningLight()
	{
		this.meshRenderer.material.SetColor("_EmissionColor", Color.red);
		this.arenaLight.enabled = true;
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x000481F3 File Offset: 0x000463F3
	public void PulsateLight()
	{
		this.arenaLight.intensity = this.lightIntensity * 2f;
		this.meshRenderer.material.SetColor("_EmissionColor", Color.red);
	}

	// Token: 0x04000D5C RID: 3420
	internal MeshRenderer meshRenderer;

	// Token: 0x04000D5D RID: 3421
	internal Light arenaLight;

	// Token: 0x04000D5E RID: 3422
	private float lightIntensity = 0.5f;
}
