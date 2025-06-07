using System;
using UnityEngine;

// Token: 0x020002BB RID: 699
public class SurplusValuable : MonoBehaviour
{
	// Token: 0x06001607 RID: 5639 RVA: 0x000C18E0 File Offset: 0x000BFAE0
	private void Start()
	{
		this.impactDetector = base.GetComponentInChildren<PhysGrabObjectImpactDetector>();
		this.impactDetector.indestructibleSpawnTimer = 0.1f;
		this.coinParticles.Emit((int)(30f * this.coinMultiplier));
		this.spawnSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001608 RID: 5640 RVA: 0x000C194C File Offset: 0x000BFB4C
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
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x000C1986 File Offset: 0x000BFB86
	public void BreakLight()
	{
		this.coinParticles.Emit((int)(3f * this.coinMultiplier));
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x000C19A0 File Offset: 0x000BFBA0
	public void BreakMedium()
	{
		this.coinParticles.Emit((int)(5f * this.coinMultiplier));
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x000C19BA File Offset: 0x000BFBBA
	public void BreakHeavy()
	{
		this.coinParticles.Emit((int)(10f * this.coinMultiplier));
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x000C19D4 File Offset: 0x000BFBD4
	public void DestroyImpulse()
	{
		this.coinParticles.Emit((int)(20f * this.coinMultiplier));
		this.coinParticles.transform.parent = null;
		this.coinParticles.main.stopAction = ParticleSystemStopAction.Destroy;
	}

	// Token: 0x04002632 RID: 9778
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04002633 RID: 9779
	private float indestructibleTimer = 3f;

	// Token: 0x04002634 RID: 9780
	public float coinMultiplier = 1f;

	// Token: 0x04002635 RID: 9781
	public ParticleSystem coinParticles;

	// Token: 0x04002636 RID: 9782
	public Sound spawnSound;
}
