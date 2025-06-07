using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000B9 RID: 185
public class PaperInteraction : MonoBehaviour
{
	// Token: 0x060006F2 RID: 1778 RVA: 0x00042190 File Offset: 0x00040390
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 1)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				int num = Random.Range(0, this.papers.Count);
				Vector3 vector = new Vector3(0f, (float)Random.Range(0, 360), 0f);
				this.photonView.RPC("SyncPaperVisual", RpcTarget.AllBuffered, new object[]
				{
					num,
					vector
				});
				return;
			}
		}
		else
		{
			this.paperVisual = Object.Instantiate<GameObject>(this.papers[Random.Range(0, this.papers.Count)], base.transform.position, Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f));
			this.paperVisual.transform.parent = this.PaperTransform;
		}
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x0004227C File Offset: 0x0004047C
	private void Update()
	{
		if (this.Picked)
		{
			if (GameManager.instance.gameMode == 1)
			{
				if (!this.destructionToMaster)
				{
					this.photonView.RPC("DestroyPaper", RpcTarget.MasterClient, Array.Empty<object>());
					this.destructionToMaster = true;
					return;
				}
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x000422CF File Offset: 0x000404CF
	[PunRPC]
	public void SyncPaperVisual(int randomPaper, Vector3 randomRotation)
	{
		this.paperVisual = Object.Instantiate<GameObject>(this.papers[randomPaper], base.transform.position, Quaternion.Euler(randomRotation));
		this.paperVisual.transform.parent = this.PaperTransform;
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x0004230F File Offset: 0x0004050F
	[PunRPC]
	public void DestroyPaper()
	{
		if (!this.destructionToOthers)
		{
			PhotonNetwork.Destroy(base.gameObject);
			this.destructionToOthers = true;
		}
	}

	// Token: 0x04000BC1 RID: 3009
	public List<GameObject> papers;

	// Token: 0x04000BC2 RID: 3010
	[HideInInspector]
	public bool Picked;

	// Token: 0x04000BC3 RID: 3011
	public Transform PaperTransform;

	// Token: 0x04000BC4 RID: 3012
	[HideInInspector]
	public GameObject paperVisual;

	// Token: 0x04000BC5 RID: 3013
	public CleanEffect CleanEffect;

	// Token: 0x04000BC6 RID: 3014
	private PhotonView photonView;

	// Token: 0x04000BC7 RID: 3015
	private bool destructionToMaster;

	// Token: 0x04000BC8 RID: 3016
	private bool destructionToOthers;
}
