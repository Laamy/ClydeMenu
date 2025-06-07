using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class BouncyNOt : MonoBehaviour
{
	// Token: 0x06000075 RID: 117 RVA: 0x00004F32 File Offset: 0x00003132
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000076 RID: 118 RVA: 0x00004F58 File Offset: 0x00003158
	public void Bounce()
	{
		this.bounceSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.bounces < this.bounceAmount && !this.physGrabObject.grabbed)
		{
			this.bounces++;
			this.rb.AddForce(Vector3.up * Random.Range(this.forceRange.x, this.forceRange.y), ForceMode.Impulse);
			this.rb.AddTorque(Random.insideUnitSphere * this.torqueMultiplier, ForceMode.Impulse);
			return;
		}
		if (this.rb.velocity.magnitude < 0.1f || this.physGrabObject.grabbed)
		{
			this.bounces = 0;
		}
	}

	// Token: 0x04000126 RID: 294
	public Sound bounceSound;

	// Token: 0x04000127 RID: 295
	private Vector2 forceRange = new Vector2(1f, 4f);

	// Token: 0x04000128 RID: 296
	private float torqueMultiplier = 0.1f;

	// Token: 0x04000129 RID: 297
	private int bounceAmount = 3;

	// Token: 0x0400012A RID: 298
	private int bounces;

	// Token: 0x0400012B RID: 299
	private Rigidbody rb;

	// Token: 0x0400012C RID: 300
	private PhotonView photonView;

	// Token: 0x0400012D RID: 301
	private PhysGrabObject physGrabObject;
}
