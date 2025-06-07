using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000101 RID: 257
public class FlatScreenTV : MonoBehaviour
{
	// Token: 0x060008EF RID: 2287 RVA: 0x00056159 File Offset: 0x00054359
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.staticGrabObject = base.GetComponent<StaticGrabObject>();
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x00056180 File Offset: 0x00054380
	private IEnumerator LateStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (Random.Range(0, 8) == 0)
			{
				this.broken = false;
			}
			this.BrokenOrNot();
		}
		yield break;
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x0005618F File Offset: 0x0005438F
	private void BrokenOrNot()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.BrokenOrNotRPC(this.broken);
			return;
		}
		this.photonView.RPC("BrokenOrNotRPC", RpcTarget.All, new object[]
		{
			this.broken
		});
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x000561CC File Offset: 0x000543CC
	[PunRPC]
	public void BrokenOrNotRPC(bool _broken)
	{
		this.broken = _broken;
		if (this.broken)
		{
			this.jumpScare.gameObject.SetActive(false);
			this.brokenPlane.gameObject.SetActive(true);
			return;
		}
		this.brokenPlane.gameObject.SetActive(false);
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x0005621C File Offset: 0x0005441C
	private void Update()
	{
		if (this.timer > 0f)
		{
			if (this.timer < 1.3f && !this.regularHurtCollider.gameObject.activeSelf)
			{
				this.regularHurtCollider.gameObject.SetActive(true);
			}
			this.timer -= Time.deltaTime;
			this.regularPlane.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f + Mathf.Sin(this.timer * 100f) * 0.1f, 1f + Mathf.Sin(this.timer * 100f) * 0.1f);
			this.regularPlane.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(-Mathf.Sin(this.timer * 100f) * 0.05f, -Mathf.Sin(this.timer * 100f) * 0.05f);
			this.isActive = true;
			return;
		}
		if (this.isActive)
		{
			this.regularPlane.gameObject.SetActive(false);
			this.regularHurtCollider.gameObject.SetActive(false);
			this.broken = true;
			this.BrokenOrNotRPC(this.broken);
		}
		this.isActive = false;
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00056365 File Offset: 0x00054565
	public void actionTime()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("actionTimeRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.actionTimeRPC();
		}
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00056394 File Offset: 0x00054594
	[PunRPC]
	public void actionTimeRPC()
	{
		if (this.timer > 0f)
		{
			return;
		}
		this.timer = 1.5f;
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 12f, base.transform.position, 0.1f);
		this.regularPlane.gameObject.SetActive(true);
		this.regularSoundGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.regularSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0400105D RID: 4189
	private float timer;

	// Token: 0x0400105E RID: 4190
	public Transform regularPlane;

	// Token: 0x0400105F RID: 4191
	public Sound regularSound;

	// Token: 0x04001060 RID: 4192
	public Sound regularSoundGlobal;

	// Token: 0x04001061 RID: 4193
	private bool isActive;

	// Token: 0x04001062 RID: 4194
	public Transform regularHurtCollider;

	// Token: 0x04001063 RID: 4195
	public Transform visionPoint;

	// Token: 0x04001064 RID: 4196
	private PhotonView photonView;

	// Token: 0x04001065 RID: 4197
	private StaticGrabObject staticGrabObject;

	// Token: 0x04001066 RID: 4198
	private bool broken = true;

	// Token: 0x04001067 RID: 4199
	public Transform jumpScare;

	// Token: 0x04001068 RID: 4200
	public Transform brokenPlane;
}
