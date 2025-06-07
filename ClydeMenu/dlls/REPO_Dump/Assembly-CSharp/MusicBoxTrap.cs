using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000252 RID: 594
public class MusicBoxTrap : Trap
{
	// Token: 0x06001331 RID: 4913 RVA: 0x000AB4B0 File Offset: 0x000A96B0
	protected override void Start()
	{
		base.Start();
		this.rb = base.GetComponent<Rigidbody>();
		this.physgrabObject = base.GetComponent<PhysGrabObject>();
		this.MusicBoxDancer.gameObject.SetActive(false);
		this.PedestalTransform.gameObject.SetActive(false);
		this.colliderLidCollision = this.colliderLid.GetComponent<CollisionFree>();
		this.colliderDancersCollision = this.colliderDancers.GetComponent<CollisionFree>();
	}

	// Token: 0x06001332 RID: 4914 RVA: 0x000AB520 File Offset: 0x000A9720
	protected override void Update()
	{
		base.Update();
		if (!this.trapTriggered && this.physgrabObject.grabbed)
		{
			this.trapStart = true;
		}
		if (this.trapStart && !this.MusicBoxOpenAnimationActive && !this.MusicBoxCloseAnimationActive)
		{
			this.MusicBoxStart();
		}
		this.MusicBoxMusic.PlayLoop(this.MusicBoxPlaying, 2f, 2f, 1f);
		if (this.openTheBox && !this.colliderDancersCollision.colliding && !this.colliderLidCollision.colliding)
		{
			if (GameManager.instance.gameMode == 0)
			{
				float musicTime = Random.Range(0f, this.MusicBoxMusic.Source.clip.length);
				this.OpenTheBox(musicTime);
			}
			else if (PhotonNetwork.IsMasterClient)
			{
				float num = Random.Range(0f, this.MusicBoxMusic.Source.clip.length);
				this.photonView.RPC("OpenTheBox", RpcTarget.All, new object[]
				{
					num
				});
			}
			this.openTheBox = false;
		}
		if (this.MusicBoxOpenAnimationActive)
		{
			this.MusicBoxPlaying = true;
			this.MusicBoxLidProgress += Time.deltaTime;
			float num2 = this.MusicBoxLidCurve.Evaluate(this.MusicBoxLidProgress / this.MusicBoxLidDuration);
			this.MusicBoxLid.localRotation = Quaternion.Euler(-90f + -num2 * 100f, 0f, 0f);
			float num3 = this.MusicBoxLidRattlerCurve.Evaluate(this.MusicBoxLidProgress / this.MusicBoxLidDuration);
			this.MusicBoxRattler.localRotation = Quaternion.Euler(0f, 0f, -num3 * 300f);
			this.PedestalTransform.localScale = new Vector3(1f, Mathf.Lerp(0.15f, 1f, num2), 1f);
			float num4 = Mathf.Lerp(0.5f, 3f, num2);
			this.MusicBoxDancer.localScale = new Vector3(num4, num4, num4);
			if (this.MusicBoxLidProgress >= this.MusicBoxLidDuration)
			{
				this.MusicBoxOpenAnimationActive = false;
				this.MusicBoxLidProgress = 0f;
			}
		}
		if (this.MusicBoxCloseAnimationActive)
		{
			this.MusicBoxLidProgress += Time.deltaTime;
			float num5 = 1f - this.MusicBoxLidCurve.Evaluate(this.MusicBoxLidProgress / this.MusicBoxLidDuration);
			this.MusicBoxLid.localRotation = Quaternion.Euler(-90f + -num5 * 100f, 0f, 0f);
			float num6 = this.MusicBoxLidRattlerCurve.Evaluate(this.MusicBoxLidProgress / this.MusicBoxLidDuration);
			this.MusicBoxRattler.localRotation = Quaternion.Euler(0f, 0f, -num6 * 300f);
			this.PedestalTransform.localScale = new Vector3(1f, Mathf.Lerp(0.15f, 1f, num5), 1f);
			float num7 = Mathf.Lerp(0.5f, 3f, num5);
			this.MusicBoxDancer.localScale = new Vector3(num7, num7, num7);
			if (this.MusicBoxLidProgress >= this.MusicBoxLidDuration)
			{
				this.MusicBoxCloseAnimationActive = false;
				this.MusicBoxLidProgress = 0f;
				this.MusicBoxPlaying = false;
				this.MusicBoxDancer.gameObject.SetActive(false);
				this.PedestalTransform.gameObject.SetActive(false);
			}
		}
		if (this.MusicBoxPlaying)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				Quaternion turnX = Quaternion.Euler(0f, 180f, 0f);
				Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
				Quaternion identity = Quaternion.identity;
				bool flag = false;
				using (List<PhysGrabber>.Enumerator enumerator = this.physGrabObject.playerGrabbing.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.isRotating)
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					this.physGrabObject.TurnXYZ(turnX, turnY, identity);
				}
			}
			if (PhysGrabber.instance && PhysGrabber.instance.grabbedObject == this.rb)
			{
				CameraAim.Instance.AimTargetSoftSet(this.physGrabObject.transform.position + CameraAim.Instance.transform.right * 100f, 0.01f, 1f, 1f, base.gameObject, 100);
				PhysGrabber.instance.OverrideGrabDistance(1f);
			}
			this.enemyInvestigate = true;
			this.MusicBoxDancer.Rotate(0f, 0f, 40f * Time.deltaTime);
			this.MusicBoxDancerSpin.Rotate(0f, 20f * Time.deltaTime, 0f);
			if (!this.MusicBoxOpenAnimationActive)
			{
				float num8 = Mathf.Sin(Time.time * 50f) * 0.1f + Mathf.Sin(Time.time * 20f) * 0.1f - Mathf.Sin(Time.time * 70f) * 0.1f;
				float num9 = Mathf.Sin(Time.time * 70f) * 0.1f + Mathf.Sin(Time.time * 10f) * 0.1f - Mathf.Sin(Time.time * 50f) * 0.1f;
				this.MusicBoxRattler.localRotation = Quaternion.Euler(-num9 * 5f, 0f, num8 * 5f);
			}
			if (!this.physgrabObject.grabbed && !this.MusicBoxOpenAnimationActive)
			{
				this.MusicBoxStop();
			}
		}
	}

	// Token: 0x06001333 RID: 4915 RVA: 0x000ABADC File Offset: 0x000A9CDC
	public void MusicBoxStop()
	{
		this.MusicBoxPlaying = false;
		this.MusicBoxCloseAnimationActive = true;
		this.trapTriggered = false;
		this.trapStart = false;
		this.MusicBoxCloseSound.Play(this.physgrabObject.centerPoint, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
		this.colliderDancers.GetComponent<BoxCollider>().isTrigger = true;
		this.colliderDancers.tag = "Untagged";
		this.colliderDancers.gameObject.layer = 13;
		this.colliderLid.GetComponent<BoxCollider>().isTrigger = true;
		this.colliderLid.tag = "Untagged";
		this.colliderLid.gameObject.layer = 13;
	}

	// Token: 0x06001334 RID: 4916 RVA: 0x000ABBC4 File Offset: 0x000A9DC4
	public void MusicBoxStart()
	{
		if (!this.trapTriggered)
		{
			this.trapTriggered = true;
			this.openTheBox = true;
		}
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x000ABBDC File Offset: 0x000A9DDC
	[PunRPC]
	private void OpenTheBox(float musicTime)
	{
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
		this.MusicBoxOpenAnimationActive = true;
		this.MusicBoxOpenSound.Play(this.physgrabObject.centerPoint, 1f, 1f, 1f, 1f);
		this.MusicBoxDancer.gameObject.SetActive(true);
		this.PedestalTransform.gameObject.SetActive(true);
		this.openTheBox = false;
		this.colliderDancers.GetComponent<BoxCollider>().isTrigger = false;
		this.colliderDancers.tag = "Phys Grab Object";
		this.colliderDancers.gameObject.layer = 16;
		this.colliderLid.GetComponent<BoxCollider>().isTrigger = false;
		this.colliderLid.tag = "Phys Grab Object";
		this.colliderLid.gameObject.layer = 16;
		this.MusicBoxMusic.StartTimeOverride = musicTime;
	}

	// Token: 0x040020AD RID: 8365
	public Transform colliderLid;

	// Token: 0x040020AE RID: 8366
	public Transform colliderDancers;

	// Token: 0x040020AF RID: 8367
	private PhysGrabObject physgrabObject;

	// Token: 0x040020B0 RID: 8368
	private CollisionFree colliderLidCollision;

	// Token: 0x040020B1 RID: 8369
	private CollisionFree colliderDancersCollision;

	// Token: 0x040020B2 RID: 8370
	public Transform MusicBoxRattler;

	// Token: 0x040020B3 RID: 8371
	public Transform MusicBoxDancerSpin;

	// Token: 0x040020B4 RID: 8372
	public Transform MusicBoxDancer;

	// Token: 0x040020B5 RID: 8373
	public Transform MusicBoxLid;

	// Token: 0x040020B6 RID: 8374
	public Transform PedestalTransform;

	// Token: 0x040020B7 RID: 8375
	[Space]
	public AnimationCurve MusicBoxLidCurve;

	// Token: 0x040020B8 RID: 8376
	public AnimationCurve MusicBoxLidRattlerCurve;

	// Token: 0x040020B9 RID: 8377
	[Space]
	[Header("Sounds")]
	public Sound MusicBoxOpenSound;

	// Token: 0x040020BA RID: 8378
	public Sound MusicBoxCloseSound;

	// Token: 0x040020BB RID: 8379
	public Sound MusicBoxMusic;

	// Token: 0x040020BC RID: 8380
	private float MusicBoxLidDuration = 0.5f;

	// Token: 0x040020BD RID: 8381
	private float MusicBoxLidProgress;

	// Token: 0x040020BE RID: 8382
	private bool MusicBoxOpenAnimationActive;

	// Token: 0x040020BF RID: 8383
	private bool MusicBoxCloseAnimationActive;

	// Token: 0x040020C0 RID: 8384
	private bool MusicBoxPlaying;

	// Token: 0x040020C1 RID: 8385
	private bool openTheBox;

	// Token: 0x040020C2 RID: 8386
	private Rigidbody rb;
}
