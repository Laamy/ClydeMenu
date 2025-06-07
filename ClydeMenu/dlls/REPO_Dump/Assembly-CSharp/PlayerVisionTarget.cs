using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001D8 RID: 472
public class PlayerVisionTarget : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x0600102E RID: 4142 RVA: 0x00094C4C File Offset: 0x00092E4C
	private void Awake()
	{
		this.PhotonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x00094C5A File Offset: 0x00092E5A
	private void Start()
	{
		this.PlayerAvatar = base.GetComponent<PlayerAvatar>();
		this.CurrentPosition = this.StandPosition;
		this.PlayerController = PlayerController.instance;
		this.MainCamera = Camera.main;
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x00094C8C File Offset: 0x00092E8C
	private void Update()
	{
		if (this.PlayerAvatar.isLocal)
		{
			if (this.PlayerController.Crouching)
			{
				if (this.PlayerController.Crawling)
				{
					this.TargetPosition = this.CrawlPosition;
				}
				else
				{
					this.TargetPosition = this.CrouchPosition;
				}
			}
			else
			{
				this.TargetPosition = this.StandPosition;
			}
			this.TargetRotation = this.MainCamera.transform.rotation;
		}
		this.CurrentPosition = Mathf.Lerp(this.CurrentPosition, this.TargetPosition, Time.deltaTime * this.LerpSpeed);
		this.CurrentRotation = Quaternion.Slerp(this.CurrentRotation, this.TargetRotation, Time.deltaTime * 20f);
		this.VisionTransform.localPosition = new Vector3(0f, this.CurrentPosition, 0f);
		this.VisionTransform.rotation = this.CurrentRotation;
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x00094D74 File Offset: 0x00092F74
	private void OnDrawGizmos()
	{
		if (this.DebugMeshActive)
		{
			Gizmos.color = new Color(0f, 1f, 0.13f, 0.75f);
			Gizmos.matrix = this.VisionTransform.localToWorldMatrix;
			Gizmos.DrawMesh(this.DebugMesh, 0, Vector3.zero, Quaternion.identity, Vector3.one * 0.15f);
		}
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x00094DDC File Offset: 0x00092FDC
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(info, base.photonView))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.TargetPosition);
			stream.SendNext(this.TargetRotation);
			return;
		}
		this.TargetPosition = (float)stream.ReceiveNext();
		this.TargetRotation = (Quaternion)stream.ReceiveNext();
	}

	// Token: 0x04001B6A RID: 7018
	private PhotonView PhotonView;

	// Token: 0x04001B6B RID: 7019
	private PlayerAvatar PlayerAvatar;

	// Token: 0x04001B6C RID: 7020
	private PlayerController PlayerController;

	// Token: 0x04001B6D RID: 7021
	public Transform VisionTransform;

	// Token: 0x04001B6E RID: 7022
	private Camera MainCamera;

	// Token: 0x04001B6F RID: 7023
	[Space]
	public float StandPosition;

	// Token: 0x04001B70 RID: 7024
	public float CrouchPosition;

	// Token: 0x04001B71 RID: 7025
	public float CrawlPosition;

	// Token: 0x04001B72 RID: 7026
	[Space]
	public float HeadStandPosition;

	// Token: 0x04001B73 RID: 7027
	public float HeadCrouchPosition;

	// Token: 0x04001B74 RID: 7028
	public float HeadCrawlPosition;

	// Token: 0x04001B75 RID: 7029
	private float TargetPosition;

	// Token: 0x04001B76 RID: 7030
	private Quaternion TargetRotation;

	// Token: 0x04001B77 RID: 7031
	internal float CurrentPosition;

	// Token: 0x04001B78 RID: 7032
	internal Quaternion CurrentRotation;

	// Token: 0x04001B79 RID: 7033
	public float LerpSpeed;

	// Token: 0x04001B7A RID: 7034
	[Space]
	public bool DebugMeshActive = true;

	// Token: 0x04001B7B RID: 7035
	public Mesh DebugMesh;
}
