using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000162 RID: 354
public class ItemStunBaton : MonoBehaviour
{
	// Token: 0x06000C1B RID: 3099 RVA: 0x0006AF5C File Offset: 0x0006915C
	private void Start()
	{
		foreach (ParticleSystem particleSystem in this.stunBatonEffects.GetComponentsInChildren<ParticleSystem>())
		{
			this.particleSystems.Add(particleSystem);
		}
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x0006AF94 File Offset: 0x00069194
	public void PlayParticles()
	{
		this.stunBatonExtraHurtCollider.SetActive(true);
		this.stunBatonSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		foreach (ParticleSystem particleSystem in this.particleSystems)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x040013A5 RID: 5029
	public Transform stunBatonEffects;

	// Token: 0x040013A6 RID: 5030
	public Sound stunBatonSound;

	// Token: 0x040013A7 RID: 5031
	public GameObject stunBatonExtraHurtCollider;

	// Token: 0x040013A8 RID: 5032
	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
}
