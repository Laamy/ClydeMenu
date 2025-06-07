using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class SlowWalkerSparkEffect : MonoBehaviour
{
	// Token: 0x060004C3 RID: 1219 RVA: 0x0002F811 File Offset: 0x0002DA11
	private void Start()
	{
		this.sparkEffects = new List<ParticleSystem>();
		this.sparkEffects.AddRange(base.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x0002F830 File Offset: 0x0002DA30
	private void Update()
	{
		if (this.playSparkEffectTimer <= 0f && this.isPlayingSparkEffect)
		{
			this.ToggleSparkEffect(false);
			this.isPlayingSparkEffect = false;
		}
		if (this.playSparkEffectTimer > 0f)
		{
			if (!this.isPlayingSparkEffect)
			{
				this.ToggleSparkEffect(true);
				this.isPlayingSparkEffect = true;
			}
			this.playSparkEffectTimer -= Time.deltaTime;
		}
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x0002F898 File Offset: 0x0002DA98
	private void ToggleSparkEffect(bool toggle)
	{
		foreach (ParticleSystem particleSystem in this.sparkEffects)
		{
			if (toggle)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x0002F8F8 File Offset: 0x0002DAF8
	public void PlaySparkEffect()
	{
		this.playSparkEffectTimer = 0.2f;
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x0002F905 File Offset: 0x0002DB05
	public void StopSparkEffect()
	{
		this.playSparkEffectTimer = 0f;
	}

	// Token: 0x040007BC RID: 1980
	private float playSparkEffectTimer;

	// Token: 0x040007BD RID: 1981
	private bool isPlayingSparkEffect;

	// Token: 0x040007BE RID: 1982
	private List<ParticleSystem> sparkEffects;
}
