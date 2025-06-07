using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001D3 RID: 467
public class PlayerPhysPusher : MonoBehaviour
{
	// Token: 0x06001002 RID: 4098 RVA: 0x00093447 File Offset: 0x00091647
	private void Awake()
	{
		this.PhotonView = base.GetComponent<PhotonView>();
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06001003 RID: 4099 RVA: 0x00093461 File Offset: 0x00091661
	private void Start()
	{
		if (GameManager.instance.gameMode == 0 || !PhotonNetwork.IsMasterClient || this.PhotonView.IsMine)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001004 RID: 4100 RVA: 0x00093490 File Offset: 0x00091690
	private void FixedUpdate()
	{
		if (this.Player.isDisabled || this.Player.isTumbling || this.Player.rbVelocity.magnitude < 0.1f)
		{
			this.Collider.gameObject.SetActive(false);
		}
		else
		{
			this.Collider.gameObject.SetActive(true);
		}
		float num = Vector3.Distance(base.transform.position, this.ColliderTarget.position);
		if ((this.Reset && num > 0.5f) || num > 1f || this.Player.rbVelocity.magnitude < 0.1f || Vector3.Dot(this.Player.rbVelocity, this.PreviousVelocity) < 0.25f)
		{
			this.Rigidbody.MovePosition(this.ColliderTarget.position);
			this.Reset = false;
		}
		this.Rigidbody.MoveRotation(this.ColliderTarget.rotation);
		Vector3 b = base.transform.InverseTransformDirection(this.Rigidbody.velocity);
		this.Rigidbody.AddRelativeForce(this.Player.rbVelocity - b, ForceMode.Impulse);
		this.PreviousVelocity = this.Player.rbVelocity;
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x000935D2 File Offset: 0x000917D2
	private void Update()
	{
		this.Collider.localScale = this.ColliderTarget.localScale;
	}

	// Token: 0x04001B37 RID: 6967
	private PhotonView PhotonView;

	// Token: 0x04001B38 RID: 6968
	private Rigidbody Rigidbody;

	// Token: 0x04001B39 RID: 6969
	public PlayerAvatar Player;

	// Token: 0x04001B3A RID: 6970
	[Space]
	public Transform ColliderTarget;

	// Token: 0x04001B3B RID: 6971
	public Transform Collider;

	// Token: 0x04001B3C RID: 6972
	internal bool Reset;

	// Token: 0x04001B3D RID: 6973
	private Vector3 PreviousVelocity;
}
