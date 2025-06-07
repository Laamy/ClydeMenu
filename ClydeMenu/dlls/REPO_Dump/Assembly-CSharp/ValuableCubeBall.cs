using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002B1 RID: 689
public class ValuableCubeBall : MonoBehaviour
{
	// Token: 0x0600158A RID: 5514 RVA: 0x000BE628 File Offset: 0x000BC828
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.bounceAmount = Random.Range(2, 6);
	}

	// Token: 0x0600158B RID: 5515 RVA: 0x000BE65C File Offset: 0x000BC85C
	public void BigBounce()
	{
		this.bounceSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.torqueMultiplier = Random.Range(this.bigTorqueRange.x, this.bigTorqueRange.y);
		if (this.bounces < this.bounceAmount && !this.physGrabObject.grabbed)
		{
			this.bounces++;
			this.rb.AddForce(Vector3.up * Random.Range(this.bigForceRange.x, this.bigForceRange.y), ForceMode.Impulse);
			this.rb.AddTorque(Random.insideUnitSphere * this.torqueMultiplier, ForceMode.Impulse);
			return;
		}
		if (this.rb.velocity.magnitude < 0.1f || this.physGrabObject.grabbed)
		{
			this.bounces = 0;
			this.bounceAmount = Random.Range(2, 6);
		}
	}

	// Token: 0x0600158C RID: 5516 RVA: 0x000BE770 File Offset: 0x000BC970
	public void SmallBounce()
	{
		this.bounceSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.torqueMultiplier = Random.Range(this.smallTorqueRange.x, this.smallTorqueRange.y);
		if (this.bounces < this.bounceAmount && !this.physGrabObject.grabbed && this.rb.velocity.magnitude > 0.2f)
		{
			this.bounces++;
			this.rb.AddForce(Vector3.up * Random.Range(this.smallForceRange.x, this.smallForceRange.y), ForceMode.Impulse);
			this.rb.AddTorque(Random.insideUnitSphere * this.torqueMultiplier, ForceMode.Impulse);
			this.bounces = this.bounceAmount;
			return;
		}
		if (this.rb.velocity.magnitude <= 0.2f || this.physGrabObject.grabbed)
		{
			this.bounces = 0;
			this.bounceAmount = Random.Range(2, 6);
		}
	}

	// Token: 0x0600158D RID: 5517 RVA: 0x000BE8B0 File Offset: 0x000BCAB0
	public void MediumBounce()
	{
		this.bounceSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.torqueMultiplier = Random.Range(this.mediumTorqueRange.x, this.mediumTorqueRange.y);
		if (this.bounces < this.bounceAmount && !this.physGrabObject.grabbed)
		{
			this.bounces++;
			this.rb.AddForce(Vector3.up * Random.Range(this.mediumForceRange.x, this.mediumForceRange.y), ForceMode.Impulse);
			this.rb.AddTorque(Random.insideUnitSphere * this.torqueMultiplier, ForceMode.Impulse);
			return;
		}
		if (this.rb.velocity.magnitude < 0.1f || this.physGrabObject.grabbed)
		{
			this.bounces = 0;
			this.bounceAmount = Random.Range(2, 6);
		}
	}

	// Token: 0x04002570 RID: 9584
	public Sound bounceSound;

	// Token: 0x04002571 RID: 9585
	private Vector2 bigForceRange = new Vector2(2f, 4f);

	// Token: 0x04002572 RID: 9586
	private Vector2 smallForceRange = new Vector2(1f, 1.5f);

	// Token: 0x04002573 RID: 9587
	private Vector2 mediumForceRange = new Vector2(1f, 2f);

	// Token: 0x04002574 RID: 9588
	private Vector2 bigTorqueRange = new Vector2(0.1f, 0.5f);

	// Token: 0x04002575 RID: 9589
	private Vector2 smallTorqueRange = new Vector2(0f, 0.1f);

	// Token: 0x04002576 RID: 9590
	private Vector2 mediumTorqueRange = new Vector2(0.05f, 0.2f);

	// Token: 0x04002577 RID: 9591
	private float torqueMultiplier = 0.1f;

	// Token: 0x04002578 RID: 9592
	private int bounceAmount = 3;

	// Token: 0x04002579 RID: 9593
	private int bounces;

	// Token: 0x0400257A RID: 9594
	private Rigidbody rb;

	// Token: 0x0400257B RID: 9595
	private PhotonView photonView;

	// Token: 0x0400257C RID: 9596
	private PhysGrabObject physGrabObject;
}
