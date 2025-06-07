using System;
using UnityEngine;

// Token: 0x020002B0 RID: 688
public class ValuableCocktail : Trap
{
	// Token: 0x06001586 RID: 5510 RVA: 0x000BE50D File Offset: 0x000BC70D
	protected override void Start()
	{
		base.Start();
		this.semiPuke = base.GetComponentInChildren<SemiPuke>();
	}

	// Token: 0x06001587 RID: 5511 RVA: 0x000BE524 File Offset: 0x000BC724
	protected override void Update()
	{
		if ((Vector3.Dot(base.transform.up, Vector3.up) < 0.4f || this.physGrabObject.rbVelocity.magnitude > 7f) && !this.isSpilled)
		{
			Quaternion direction = Quaternion.LookRotation(Vector3.down, Vector3.up);
			this.semiPuke.PukeActive(this.SemiPukeTransform.position, direction);
			this.SpillCocktail();
			this.isSpilled = true;
		}
	}

	// Token: 0x06001588 RID: 5512 RVA: 0x000BE5A0 File Offset: 0x000BC7A0
	private void SpillCocktail()
	{
		this.liquid.gameObject.SetActive(false);
		this.eye.gameObject.SetActive(false);
		this.eyeParticle.Play();
		this.soundSpill.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
		this.physGrabObject.impactDetector.BreakMedium(this.physGrabObject.centerPoint);
	}

	// Token: 0x04002569 RID: 9577
	public Transform liquid;

	// Token: 0x0400256A RID: 9578
	public Transform SemiPukeTransform;

	// Token: 0x0400256B RID: 9579
	public Transform eye;

	// Token: 0x0400256C RID: 9580
	public ParticleSystem eyeParticle;

	// Token: 0x0400256D RID: 9581
	public Sound soundSpill;

	// Token: 0x0400256E RID: 9582
	private bool isSpilled;

	// Token: 0x0400256F RID: 9583
	private SemiPuke semiPuke;
}
