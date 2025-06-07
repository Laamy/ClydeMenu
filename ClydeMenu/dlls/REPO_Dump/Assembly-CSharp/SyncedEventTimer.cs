using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001AE RID: 430
public class SyncedEventTimer : MonoBehaviour
{
	// Token: 0x06000EC5 RID: 3781 RVA: 0x00085B56 File Offset: 0x00083D56
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0)
		{
			this.singlePlayer = true;
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.isMaster = true;
		}
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x00085B88 File Offset: 0x00083D88
	public void StartTimer()
	{
		if (this.singlePlayer || this.isMaster)
		{
			this.timer = Random.Range(this.timerMin, this.timerMax);
			this.onTimerStart.Invoke();
			base.StartCoroutine(this.Timer());
			this.timerActive = true;
			if (this.isMaster)
			{
				this.photonView.RPC("StartTimerRPC", RpcTarget.Others, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x00085BF9 File Offset: 0x00083DF9
	[PunRPC]
	private void StartTimerRPC()
	{
		this.timerActive = true;
		this.onTimerStart.Invoke();
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x00085C0D File Offset: 0x00083E0D
	private IEnumerator Timer()
	{
		while (this.timer > 0f)
		{
			this.timer -= Time.deltaTime;
			yield return null;
		}
		this.EndTimer();
		if (this.isMaster)
		{
			this.photonView.RPC("EndTimerRPC", RpcTarget.Others, Array.Empty<object>());
		}
		yield break;
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x00085C1C File Offset: 0x00083E1C
	private void Update()
	{
		if (this.timerActive)
		{
			this.onTimerTick.Invoke();
		}
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x00085C31 File Offset: 0x00083E31
	[PunRPC]
	private void EndTimerRPC()
	{
		this.timerActive = false;
		this.onTimerEnd.Invoke();
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x00085C45 File Offset: 0x00083E45
	public void EndTimer()
	{
		this.timerActive = false;
		this.onTimerEnd.Invoke();
	}

	// Token: 0x04001854 RID: 6228
	private PhotonView photonView;

	// Token: 0x04001855 RID: 6229
	private float timer;

	// Token: 0x04001856 RID: 6230
	public float timerMin = 4f;

	// Token: 0x04001857 RID: 6231
	public float timerMax = 5f;

	// Token: 0x04001858 RID: 6232
	public UnityEvent onTimerStart;

	// Token: 0x04001859 RID: 6233
	public UnityEvent onTimerEnd;

	// Token: 0x0400185A RID: 6234
	public UnityEvent onTimerTick;

	// Token: 0x0400185B RID: 6235
	private bool singlePlayer;

	// Token: 0x0400185C RID: 6236
	private bool isMaster;

	// Token: 0x0400185D RID: 6237
	private bool timerActive;
}
