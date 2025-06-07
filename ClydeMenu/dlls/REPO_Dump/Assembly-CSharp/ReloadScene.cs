using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000231 RID: 561
public class ReloadScene : MonoBehaviour, IPunObservable
{
	// Token: 0x06001285 RID: 4741 RVA: 0x000A66BF File Offset: 0x000A48BF
	[PunRPC]
	private void PlayerReady(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!this.PlayersReadyList.Contains(_info.Sender))
		{
			this.PlayersReadyList.Add(_info.Sender);
			this.PlayersReady++;
		}
	}

	// Token: 0x06001286 RID: 4742 RVA: 0x000A66F3 File Offset: 0x000A48F3
	private void Awake()
	{
		this.photonview = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x000A6701 File Offset: 0x000A4901
	private void Start()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonview.RPC("PlayerReady", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06001288 RID: 4744 RVA: 0x000A6720 File Offset: 0x000A4920
	private void Update()
	{
		if (this.minTime > 0f)
		{
			this.minTime -= Time.deltaTime;
			return;
		}
		if (!this.Restarting)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				SceneManager.LoadSceneAsync("Main");
				this.Restarting = true;
				return;
			}
			if (PhotonNetwork.IsMasterClient && this.PlayersReady >= PhotonNetwork.CurrentRoom.PlayerCount && (PhotonNetwork.LevelLoadingProgress == 0f || PhotonNetwork.LevelLoadingProgress == 1f))
			{
				PhotonNetwork.AutomaticallySyncScene = true;
				PhotonNetwork.LoadLevel("Main");
				this.Restarting = true;
			}
		}
	}

	// Token: 0x06001289 RID: 4745 RVA: 0x000A67B7 File Offset: 0x000A49B7
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.PlayersReady);
			return;
		}
		this.PlayersReady = (int)stream.ReceiveNext();
	}

	// Token: 0x04001F13 RID: 7955
	private PhotonView photonview;

	// Token: 0x04001F14 RID: 7956
	public int PlayersReady;

	// Token: 0x04001F15 RID: 7957
	private List<Player> PlayersReadyList = new List<Player>();

	// Token: 0x04001F16 RID: 7958
	private bool Restarting;

	// Token: 0x04001F17 RID: 7959
	private float minTime = 0.1f;
}
