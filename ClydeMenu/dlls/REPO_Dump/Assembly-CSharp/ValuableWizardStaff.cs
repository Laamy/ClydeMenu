using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002C3 RID: 707
public class ValuableWizardStaff : MonoBehaviour
{
	// Token: 0x0600163F RID: 5695 RVA: 0x000C2E12 File Offset: 0x000C1012
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06001640 RID: 5696 RVA: 0x000C2E2C File Offset: 0x000C102C
	private void Update()
	{
		if (this.laserTimer > 0f)
		{
			this.laserTimer -= Time.deltaTime;
			Vector3 endPosition = this.laserTransform.position + this.laserTransform.forward * 15f;
			bool isHitting = false;
			RaycastHit raycastHit;
			if (Physics.Raycast(this.laserTransform.position, this.laserTransform.forward, out raycastHit, 15f, SemiFunc.LayerMaskGetVisionObstruct()))
			{
				endPosition = raycastHit.point;
				isHitting = true;
			}
			this.semiLaser.LaserActive(this.laserTransform.position, endPosition, isHitting);
		}
	}

	// Token: 0x06001641 RID: 5697 RVA: 0x000C2ED4 File Offset: 0x000C10D4
	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.laserTimer > 0f)
		{
			Vector3 force = -this.laserTransform.forward * 1000f * Time.fixedDeltaTime;
			this.rb.AddForce(force, ForceMode.Force);
		}
	}

	// Token: 0x06001642 RID: 5698 RVA: 0x000C2F28 File Offset: 0x000C1128
	public void StaffLaser()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			float num = Random.Range(1f, 4f);
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("StaffLaserRPC", RpcTarget.All, new object[]
				{
					num
				});
				return;
			}
			this.StaffLaserRPC(num);
		}
	}

	// Token: 0x06001643 RID: 5699 RVA: 0x000C2F7B File Offset: 0x000C117B
	[PunRPC]
	public void StaffLaserRPC(float _time)
	{
		this.laserTimer = _time;
	}

	// Token: 0x04002679 RID: 9849
	private PhotonView photonView;

	// Token: 0x0400267A RID: 9850
	private float laserTimer;

	// Token: 0x0400267B RID: 9851
	public SemiLaser semiLaser;

	// Token: 0x0400267C RID: 9852
	public Transform laserTransform;

	// Token: 0x0400267D RID: 9853
	private Rigidbody rb;
}
