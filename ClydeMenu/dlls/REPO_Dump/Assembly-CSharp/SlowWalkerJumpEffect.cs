using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200007D RID: 125
public class SlowWalkerJumpEffect : MonoBehaviour
{
	// Token: 0x060004BE RID: 1214 RVA: 0x0002F67C File Offset: 0x0002D87C
	private void Start()
	{
		this.particles.AddRange(base.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0002F690 File Offset: 0x0002D890
	private void PlayParticles()
	{
		foreach (ParticleSystem particleSystem in this.particles)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0002F6E0 File Offset: 0x0002D8E0
	public void JumpEffect()
	{
		this.rotationTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
		this.PlayParticles();
		GameDirector.instance.CameraImpact.ShakeDistance(4f, 6f, 15f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(4f, 6f, 15f, base.transform.position, 0.1f);
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0002F770 File Offset: 0x0002D970
	public void LandEffect()
	{
		this.rotationTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
		GameDirector.instance.CameraImpact.ShakeDistance(6f, 6f, 15f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(6f, 6f, 15f, base.transform.position, 0.1f);
		this.PlayParticles();
	}

	// Token: 0x040007BA RID: 1978
	public Transform rotationTransform;

	// Token: 0x040007BB RID: 1979
	private List<ParticleSystem> particles = new List<ParticleSystem>();
}
