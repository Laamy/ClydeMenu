using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001B9 RID: 441
public class PlayerAvatarCollision : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x06000F45 RID: 3909 RVA: 0x000894B8 File Offset: 0x000876B8
	private void Start()
	{
		this.PlayerController = PlayerController.instance;
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x000894C8 File Offset: 0x000876C8
	private void Update()
	{
		if (this.PlayerAvatar.isLocal)
		{
			this.Scale = this.PlayerController.PlayerCollision.transform.localScale;
			this.Collider.enabled = false;
		}
		this.CollisionTransform.localScale = this.Scale;
		this.deathHeadPosition = this.CollisionTransform.position + Vector3.up * (this.Collider.height * this.CollisionTransform.localScale.y - 0.18f);
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x0008955C File Offset: 0x0008775C
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(info, this.PlayerAvatar.photonView))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.Scale);
			return;
		}
		this.Scale = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x000895A8 File Offset: 0x000877A8
	public void SetCrouch()
	{
		this.Scale = PlayerCollision.instance.CrouchCollision.localScale;
		this.CollisionTransform.localScale = this.Scale;
	}

	// Token: 0x0400190A RID: 6410
	public PlayerAvatar PlayerAvatar;

	// Token: 0x0400190B RID: 6411
	private PlayerController PlayerController;

	// Token: 0x0400190C RID: 6412
	public Transform CollisionTransform;

	// Token: 0x0400190D RID: 6413
	public CapsuleCollider Collider;

	// Token: 0x0400190E RID: 6414
	private Vector3 Scale;

	// Token: 0x0400190F RID: 6415
	internal Vector3 deathHeadPosition;
}
