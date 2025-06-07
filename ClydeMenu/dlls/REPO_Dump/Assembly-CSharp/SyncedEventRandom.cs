using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class SyncedEventRandom : MonoBehaviour
{
	// Token: 0x06000EBF RID: 3775 RVA: 0x00085A3F File Offset: 0x00083C3F
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000EC0 RID: 3776 RVA: 0x00085A50 File Offset: 0x00083C50
	public void RandomRangeFloat(float min, float max)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.resultRandomRangeFloat = Random.Range(min, max);
			this.resultReceivedRandomRangeFloat = true;
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.resultRandomRangeFloat = Random.Range(min, max);
			this.resultReceivedRandomRangeFloat = true;
			this.photonView.RPC("RandomRangeFloatRPC", RpcTarget.Others, new object[]
			{
				this.resultRandomRangeFloat
			});
		}
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x00085ABE File Offset: 0x00083CBE
	[PunRPC]
	private void RandomRangeFloatRPC(float result)
	{
		this.resultRandomRangeFloat = result;
		this.resultReceivedRandomRangeFloat = true;
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x00085AD0 File Offset: 0x00083CD0
	public void RandomRangeInt(int min, int max)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.resultRandomRangeInt = Random.Range(min, max);
			this.resultReceivedRandomRangeInt = true;
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.resultRandomRangeInt = Random.Range(min, max);
			this.resultReceivedRandomRangeInt = true;
			this.photonView.RPC("RandomRangeIntRPC", RpcTarget.Others, new object[]
			{
				this.resultRandomRangeInt
			});
		}
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x00085B3E File Offset: 0x00083D3E
	[PunRPC]
	private void RandomRangeIntRPC(int result)
	{
		this.resultRandomRangeInt = result;
		this.resultReceivedRandomRangeInt = true;
	}

	// Token: 0x0400184F RID: 6223
	[HideInInspector]
	public float resultRandomRangeFloat;

	// Token: 0x04001850 RID: 6224
	[HideInInspector]
	public int resultRandomRangeInt;

	// Token: 0x04001851 RID: 6225
	[HideInInspector]
	public bool resultReceivedRandomRangeFloat;

	// Token: 0x04001852 RID: 6226
	[HideInInspector]
	public bool resultReceivedRandomRangeInt;

	// Token: 0x04001853 RID: 6227
	private PhotonView photonView;
}
