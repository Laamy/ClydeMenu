using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class TruckScreenOpen : MonoBehaviour
{
	// Token: 0x06000868 RID: 2152 RVA: 0x00051570 File Offset: 0x0004F770
	private void Start()
	{
		this.openScreenYPosOriginal = base.transform.localPosition.y;
		this.doorParticles = base.GetComponentInChildren<ParticleSystem>();
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.openScreenYPosOriginal + this.doorOpenPosition, base.transform.localPosition.z);
		base.StartCoroutine(this.DelayedClose());
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x000515F8 File Offset: 0x0004F7F8
	private void TruckScreenOpenStartLogic()
	{
		this.openScreenActive = true;
		GameDirector.instance.CameraImpact.ShakeDistance(6f, 3f, 8f, base.transform.position, 0.2f);
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.openScreenYPosOriginal, base.transform.localPosition.z);
		this.openScreenCurveTimer = 0f;
		this.doorLoopStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.doorLoopPlaying = true;
		this.doorDone = false;
		this.doorParticles.Play();
		this.doorClose = false;
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x000516C6 File Offset: 0x0004F8C6
	public void TruckScreenOpenStart()
	{
		if (GameManager.Multiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.photonView.RPC("TruckScreenOpenStartRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.TruckScreenOpenStartLogic();
		}
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x000516F3 File Offset: 0x0004F8F3
	[PunRPC]
	private void TruckScreenOpenStartRPC()
	{
		this.TruckScreenOpenStartLogic();
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x000516FC File Offset: 0x0004F8FC
	private void TruckScreenCloseStart()
	{
		this.openScreenActive = true;
		GameDirector.instance.CameraImpact.ShakeDistance(6f, 3f, 8f, base.transform.position, 0.2f);
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.openScreenYPosOriginal + this.doorOpenPosition, base.transform.localPosition.z);
		this.openScreenCurveTimer = 0f;
		this.doorLoopStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.doorLoopPlaying = true;
		this.doorDone = false;
		this.doorParticles.Play();
		this.doorClose = true;
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x000517D1 File Offset: 0x0004F9D1
	private IEnumerator DelayedClose()
	{
		yield return new WaitForSeconds(2f);
		this.TruckScreenCloseStart();
		yield break;
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x000517E0 File Offset: 0x0004F9E0
	private IEnumerator DelayedLevelSwitch()
	{
		yield return new WaitForSeconds(2f);
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Normal);
		yield break;
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x000517E8 File Offset: 0x0004F9E8
	private void Update()
	{
		this.doorLoop.PlayLoop(this.doorLoopPlaying, 2f, 2f, 1f);
		if (!this.openScreenActive)
		{
			return;
		}
		if (this.openScreenCurveTimer < 1f)
		{
			this.openScreenCurveTimer += Time.deltaTime;
			float time = this.openScreenCurveTimer;
			if (this.doorClose)
			{
				time = 1f - this.openScreenCurveTimer;
			}
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.openScreenYPosOriginal + this.openScreenCurve.Evaluate(time) * this.doorOpenPosition, base.transform.localPosition.z);
			if (this.openScreenCurveTimer > 0.8f && !this.doorDone)
			{
				GameDirector.instance.CameraImpact.ShakeDistance(6f, 3f, 8f, base.transform.position, 0.1f);
				this.doorDone = true;
				this.doorLoopEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.doorSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.doorLoopPlaying = false;
				this.doorParticles.Play();
				if (!this.doorClose)
				{
					if (!GameManager.Multiplayer())
					{
						base.StartCoroutine(this.DelayedLevelSwitch());
						return;
					}
					if (SemiFunc.IsMasterClientOrSingleplayer())
					{
						base.StartCoroutine(this.DelayedLevelSwitch());
						return;
					}
				}
			}
		}
		else
		{
			this.openScreenActive = false;
		}
	}

	// Token: 0x04000F81 RID: 3969
	public AnimationCurve openScreenCurve;

	// Token: 0x04000F82 RID: 3970
	private float openScreenCurveTimer;

	// Token: 0x04000F83 RID: 3971
	private float openScreenYPosOriginal;

	// Token: 0x04000F84 RID: 3972
	private bool openScreenActive;

	// Token: 0x04000F85 RID: 3973
	private bool doorDone;

	// Token: 0x04000F86 RID: 3974
	private bool doorLoopPlaying;

	// Token: 0x04000F87 RID: 3975
	public Sound doorLoop;

	// Token: 0x04000F88 RID: 3976
	public Sound doorLoopStart;

	// Token: 0x04000F89 RID: 3977
	public Sound doorLoopEnd;

	// Token: 0x04000F8A RID: 3978
	public Sound doorSound;

	// Token: 0x04000F8B RID: 3979
	private ParticleSystem doorParticles;

	// Token: 0x04000F8C RID: 3980
	private bool doorClose;

	// Token: 0x04000F8D RID: 3981
	private float doorOpenPosition = 4.13f;

	// Token: 0x04000F8E RID: 3982
	private PhotonView photonView;
}
