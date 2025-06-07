using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001C0 RID: 448
public class PlayerBattery : MonoBehaviour
{
	// Token: 0x06000F76 RID: 3958 RVA: 0x0008BF4D File Offset: 0x0008A14D
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.staticGrabObject = base.GetComponent<StaticGrabObject>();
	}

	// Token: 0x06000F77 RID: 3959 RVA: 0x0008BF68 File Offset: 0x0008A168
	private void Update()
	{
		if ((this.isLocal || this.playerAvatar.isLocal) && !this.isLocal)
		{
			base.GetComponent<Collider>().enabled = false;
			base.GetComponent<MeshRenderer>().enabled = false;
			this.isLocal = true;
		}
		base.transform.position = this.batteryPlacement.position;
		base.transform.rotation = this.batteryPlacement.rotation;
		if (PhotonNetwork.IsMasterClient)
		{
			if (this.staticGrabObject.playerGrabbing.Count > 0 && !this.masterCharging)
			{
				this.masterCharging = true;
				this.photonView.RPC("BatteryChargeStart", RpcTarget.All, Array.Empty<object>());
			}
			if (this.staticGrabObject.playerGrabbing.Count <= 0 && this.masterCharging)
			{
				this.masterCharging = false;
				this.photonView.RPC("BatteryChargeEnd", RpcTarget.All, Array.Empty<object>());
			}
		}
		if (this.chargeBattery)
		{
			if (this.chargeTimer < this.chargeRate)
			{
				this.chargeTimer += Time.deltaTime;
				return;
			}
			this.batteryChargeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			if (PhotonNetwork.IsMasterClient)
			{
				foreach (PhysGrabber physGrabber in this.staticGrabObject.playerGrabbing)
				{
				}
				this.amountPlayersGrabbing = this.staticGrabObject.playerGrabbing.Count;
				if (this.amountPlayersGrabbing != this.amountPlayersGrabbingPrevious)
				{
					this.photonView.RPC("UpdateAmountPlayersGrabbing", RpcTarget.Others, new object[]
					{
						this.amountPlayersGrabbing
					});
					this.amountPlayersGrabbingPrevious = this.amountPlayersGrabbing;
				}
			}
			if (this.playerAvatar.isLocal)
			{
				PlayerController.instance.EnergyCurrent += 1f * (float)this.amountPlayersGrabbing;
			}
			this.chargeTimer = 0f;
		}
	}

	// Token: 0x06000F78 RID: 3960 RVA: 0x0008C184 File Offset: 0x0008A384
	[PunRPC]
	private void UpdateAmountPlayersGrabbing(int amount)
	{
		this.amountPlayersGrabbing = amount;
	}

	// Token: 0x06000F79 RID: 3961 RVA: 0x0008C18D File Offset: 0x0008A38D
	[PunRPC]
	private void BatteryChargeStart()
	{
		this.chargeBattery = true;
	}

	// Token: 0x06000F7A RID: 3962 RVA: 0x0008C196 File Offset: 0x0008A396
	[PunRPC]
	private void BatteryChargeEnd()
	{
		this.chargeBattery = false;
	}

	// Token: 0x040019AF RID: 6575
	public PlayerAvatar playerAvatar;

	// Token: 0x040019B0 RID: 6576
	private PhotonView photonView;

	// Token: 0x040019B1 RID: 6577
	private StaticGrabObject staticGrabObject;

	// Token: 0x040019B2 RID: 6578
	private bool masterCharging;

	// Token: 0x040019B3 RID: 6579
	private bool isLocal;

	// Token: 0x040019B4 RID: 6580
	private bool chargeBattery;

	// Token: 0x040019B5 RID: 6581
	private float chargeRate = 0.5f;

	// Token: 0x040019B6 RID: 6582
	private float chargeTimer;

	// Token: 0x040019B7 RID: 6583
	private int amountPlayersGrabbing;

	// Token: 0x040019B8 RID: 6584
	private int amountPlayersGrabbingPrevious;

	// Token: 0x040019B9 RID: 6585
	public Transform batteryPlacement;

	// Token: 0x040019BA RID: 6586
	public Sound batteryChargeSound;
}
