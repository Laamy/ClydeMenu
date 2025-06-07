using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class StaticGrabObject : MonoBehaviour
{
	// Token: 0x06000EB1 RID: 3761 RVA: 0x0008560B File Offset: 0x0008380B
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 1 && PhotonNetwork.IsMasterClient)
		{
			this.isMaster = true;
			this.photonView.TransferOwnership(PhotonNetwork.MasterClient);
		}
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x00085644 File Offset: 0x00083844
	private void Update()
	{
		if (this.grabbed)
		{
			for (int i = 0; i < this.playerGrabbing.Count; i++)
			{
				if (!this.playerGrabbing[i])
				{
					this.playerGrabbing.RemoveAt(i);
				}
			}
		}
		if (GameManager.instance.gameMode == 0 || this.isMaster)
		{
			this.velocity = Vector3.zero;
			foreach (PhysGrabber physGrabber in this.playerGrabbing)
			{
				Vector3 a = (physGrabber.physGrabPointPullerPosition - physGrabber.physGrabPoint.position) * 5f;
				this.velocity += a * Time.deltaTime;
			}
			if (this.dead && this.playerGrabbing.Count == 0)
			{
				this.DestroyPhysGrabObject();
			}
		}
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x00085748 File Offset: 0x00083948
	private void OnDisable()
	{
		this.playerGrabbing.Clear();
		this.grabbed = false;
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x0008575C File Offset: 0x0008395C
	public void GrabLink(int playerPhotonID, Vector3 point)
	{
		this.photonView.RPC("GrabLinkRPC", RpcTarget.All, new object[]
		{
			playerPhotonID,
			point
		});
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x00085788 File Offset: 0x00083988
	[PunRPC]
	private void GrabLinkRPC(int playerPhotonID, Vector3 point)
	{
		PhysGrabber component = PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>();
		component.physGrabPoint.position = point;
		component.localGrabPosition = this.colliderTransform.InverseTransformPoint(point);
		component.grabbedObjectTransform = this.colliderTransform;
		component.grabbed = true;
		if (component.photonView.IsMine)
		{
			Vector3 localPosition = component.physGrabPoint.localPosition;
			this.photonView.RPC("GrabPointSyncRPC", RpcTarget.MasterClient, new object[]
			{
				playerPhotonID,
				localPosition
			});
		}
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x00085814 File Offset: 0x00083A14
	[PunRPC]
	private void GrabPointSyncRPC(int playerPhotonID, Vector3 localPointInBox)
	{
		PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>().physGrabPoint.localPosition = localPointInBox;
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x0008582C File Offset: 0x00083A2C
	public void GrabStarted(PhysGrabber player)
	{
		if (!this.grabbed)
		{
			this.grabbed = true;
			if (GameManager.instance.gameMode == 0)
			{
				if (!this.playerGrabbing.Contains(player))
				{
					this.playerGrabbing.Add(player);
					return;
				}
			}
			else
			{
				this.photonView.RPC("GrabStartedRPC", RpcTarget.MasterClient, new object[]
				{
					player.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x0008589C File Offset: 0x00083A9C
	[PunRPC]
	private void GrabStartedRPC(int playerPhotonID)
	{
		PhysGrabber component = PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>();
		if (!this.playerGrabbing.Contains(component))
		{
			this.playerGrabbing.Add(component);
		}
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x000858D0 File Offset: 0x00083AD0
	public void GrabEnded(PhysGrabber player)
	{
		if (this.grabbed)
		{
			this.grabbed = false;
			if (GameManager.instance.gameMode == 0)
			{
				if (this.playerGrabbing.Contains(player))
				{
					this.playerGrabbing.Remove(player);
					return;
				}
			}
			else
			{
				this.photonView.RPC("GrabEndedRPC", RpcTarget.MasterClient, new object[]
				{
					player.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x00085940 File Offset: 0x00083B40
	[PunRPC]
	private void GrabEndedRPC(int playerPhotonID)
	{
		PhysGrabber component = PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>();
		component.grabbed = false;
		if (this.playerGrabbing.Contains(component))
		{
			this.playerGrabbing.Remove(component);
		}
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x0008597B File Offset: 0x00083B7B
	private void DestroyPhysGrabObject()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.DestroyPhysObjectFailsafe();
			Object.Destroy(base.gameObject);
			return;
		}
		this.photonView.RPC("DestroyPhysGrabObjectRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x000859B1 File Offset: 0x00083BB1
	[PunRPC]
	private void DestroyPhysGrabObjectRPC()
	{
		this.DestroyPhysObjectFailsafe();
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x000859C4 File Offset: 0x00083BC4
	private void DestroyPhysObjectFailsafe()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.CompareTag("Phys Grab Controller"))
			{
				transform.SetParent(null);
			}
		}
	}

	// Token: 0x04001848 RID: 6216
	private PhotonView photonView;

	// Token: 0x04001849 RID: 6217
	private bool isMaster;

	// Token: 0x0400184A RID: 6218
	public Transform colliderTransform;

	// Token: 0x0400184B RID: 6219
	[HideInInspector]
	public Vector3 velocity;

	// Token: 0x0400184C RID: 6220
	[HideInInspector]
	public bool grabbed;

	// Token: 0x0400184D RID: 6221
	public List<PhysGrabber> playerGrabbing = new List<PhysGrabber>();

	// Token: 0x0400184E RID: 6222
	[HideInInspector]
	public bool dead;
}
