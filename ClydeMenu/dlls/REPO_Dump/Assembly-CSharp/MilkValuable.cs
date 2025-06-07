using System;
using UnityEngine;

// Token: 0x020002B6 RID: 694
public class MilkValuable : Trap
{
	// Token: 0x060015D1 RID: 5585 RVA: 0x000C05D0 File Offset: 0x000BE7D0
	protected override void Start()
	{
		base.Start();
		this.rb = base.GetComponent<Rigidbody>();
		this.milkYPos = this.bottomY;
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x000C05F0 File Offset: 0x000BE7F0
	protected override void Update()
	{
		if (this.physGrabObject.rbVelocity.magnitude > this.sloshVelocity && Time.time >= this.nextSloshTime)
		{
			this.sloshSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			this.nextSloshTime = Time.time + this.sloshCooldown;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.physGrabObject.OverrideTorqueStrength(0.5f, 0.1f);
	}

	// Token: 0x040025D1 RID: 9681
	[Header("Physics Settings")]
	public float milkMass = 1f;

	// Token: 0x040025D2 RID: 9682
	public float gravity = 9.81f;

	// Token: 0x040025D3 RID: 9683
	public float forceMultiplier = 10f;

	// Token: 0x040025D4 RID: 9684
	public float springStrength = 50f;

	// Token: 0x040025D5 RID: 9685
	public float friction = 5f;

	// Token: 0x040025D6 RID: 9686
	public float angularInfluence = 2f;

	// Token: 0x040025D7 RID: 9687
	public float impactForceMultiplier = 4f;

	// Token: 0x040025D8 RID: 9688
	public float gravityForceMultiplier = 0.8f;

	// Token: 0x040025D9 RID: 9689
	[Header("Center of mass")]
	public float centerOfMassOffset = 1f;

	// Token: 0x040025DA RID: 9690
	public float centerOfMassMargin = 0.2f;

	// Token: 0x040025DB RID: 9691
	[Header("Milk Movement Range")]
	public float bottomY;

	// Token: 0x040025DC RID: 9692
	public float topY = 3f;

	// Token: 0x040025DD RID: 9693
	[Header("Sounds")]
	public Sound sloshSound;

	// Token: 0x040025DE RID: 9694
	[Header("Sound frequency variables")]
	public float sloshVelocity = 2f;

	// Token: 0x040025DF RID: 9695
	public float sloshCooldown = 0.3f;

	// Token: 0x040025E0 RID: 9696
	private Rigidbody rb;

	// Token: 0x040025E1 RID: 9697
	private float milkYPos;

	// Token: 0x040025E2 RID: 9698
	private float milkYVel;

	// Token: 0x040025E3 RID: 9699
	private float targetMilkY;

	// Token: 0x040025E4 RID: 9700
	private float acceleration;

	// Token: 0x040025E5 RID: 9701
	private float nextSloshTime;

	// Token: 0x040025E6 RID: 9702
	private float netForce;

	// Token: 0x040025E7 RID: 9703
	private float springForce;

	// Token: 0x040025E8 RID: 9704
	private float frictionForce;
}
