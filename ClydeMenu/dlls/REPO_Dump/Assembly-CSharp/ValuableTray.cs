using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002B8 RID: 696
public class ValuableTray : Trap
{
	// Token: 0x060015F3 RID: 5619 RVA: 0x000C1319 File Offset: 0x000BF519
	protected override void Start()
	{
		base.Start();
		this.dropTimer = this.dropThreshold;
		this.previousDropped = this.dropped;
	}

	// Token: 0x060015F4 RID: 5620 RVA: 0x000C133C File Offset: 0x000BF53C
	protected override void Update()
	{
		if (this.dropped >= this.trayThings.Count)
		{
			return;
		}
		if (this.dropped != this.previousDropped && this.dropped < this.trayThings.Count)
		{
			this.trayThings[this.dropped].gameObject.SetActive(false);
			this.trayParticles[this.dropped].Play();
			this.physGrabObject.impactDetector.BreakHeavy(this.physGrabObject.centerPoint, 0f);
			this.previousDropped = this.dropped;
			if (this.dropped == 0)
			{
				this.trayColliders[0].gameObject.SetActive(false);
			}
			else if (this.dropped == 1)
			{
				this.trayColliders[1].gameObject.SetActive(false);
			}
			else if (this.dropped == 2)
			{
				this.trayColliders[1].gameObject.SetActive(false);
			}
			else if (this.dropped == 3)
			{
				this.trayColliders[2].gameObject.SetActive(false);
			}
			else if (this.dropped == 5)
			{
				this.trayColliders[3].gameObject.SetActive(false);
			}
			else if (this.dropped == 7)
			{
				this.trayColliders[4].gameObject.SetActive(false);
			}
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		float num = Vector3.Dot(base.transform.up, Vector3.up);
		this.dropTimer += Time.deltaTime;
		if (num < 0.97f && this.dropTimer >= this.dropThreshold)
		{
			this.DropThing();
		}
	}

	// Token: 0x060015F5 RID: 5621 RVA: 0x000C14FB File Offset: 0x000BF6FB
	private void DropThing()
	{
		if (this.dropped >= this.trayThings.Count)
		{
			return;
		}
		this.dropTimer = 0f;
		this.UpdateDropAmount(this.dropped + 1);
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x000C152A File Offset: 0x000BF72A
	[PunRPC]
	public void UpdateDropAmountRPC(int _index, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.dropped = _index;
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x000C153C File Offset: 0x000BF73C
	private void UpdateDropAmount(int _index)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer() || this.dropped >= this.trayThings.Count)
		{
			return;
		}
		if (this.dropped == _index)
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.UpdateDropAmountRPC(_index, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("UpdateDropAmountRPC", RpcTarget.All, new object[]
		{
			_index
		});
	}

	// Token: 0x0400260D RID: 9741
	public List<Transform> trayThings = new List<Transform>();

	// Token: 0x0400260E RID: 9742
	public List<ParticleSystem> trayParticles = new List<ParticleSystem>();

	// Token: 0x0400260F RID: 9743
	public List<Collider> trayColliders = new List<Collider>();

	// Token: 0x04002610 RID: 9744
	private float dropThreshold = 0.4f;

	// Token: 0x04002611 RID: 9745
	private float dropTimer;

	// Token: 0x04002612 RID: 9746
	private int dropped = -1;

	// Token: 0x04002613 RID: 9747
	private int previousDropped;
}
