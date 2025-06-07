using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002A5 RID: 677
public class BottleTrap : Trap
{
	// Token: 0x0600152E RID: 5422 RVA: 0x000BAE3E File Offset: 0x000B903E
	protected override void Start()
	{
		base.Start();
		this.initialBottleRotation = this.Bottle.transform.localRotation;
		this.rb = base.GetComponent<Rigidbody>();
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x000BAE74 File Offset: 0x000B9074
	protected override void Update()
	{
		base.Update();
		this.FlyLoop.PlayLoop(this.LoopPlaying, 0.8f, 0.8f, 1f);
		if (this.trapStart)
		{
			this.TrapActivate();
		}
		if (this.trapActive)
		{
			this.enemyInvestigate = true;
			this.enemyInvestigateRange = 15f;
			this.LoopPlaying = true;
			float num = 1f;
			float num2 = 40f;
			float num3 = num * Mathf.Sin(Time.time * num2);
			float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
			this.Bottle.transform.localRotation = this.initialBottleRotation * Quaternion.Euler(num3, 0f, z);
			this.Bottle.transform.localPosition = new Vector3(this.Bottle.transform.localPosition.x, this.Bottle.transform.localPosition.y - num3 * 0.005f * Time.deltaTime, this.Bottle.transform.localPosition.z);
		}
	}

	// Token: 0x06001530 RID: 5424 RVA: 0x000BAF90 File Offset: 0x000B9190
	private void FixedUpdate()
	{
		if (this.trapActive && this.isLocal)
		{
			this.rb.AddForce(-base.transform.up * 0.45f * 40f * Time.fixedDeltaTime * 50f, ForceMode.Force);
			this.rb.AddForce(Vector3.up * 0.15f * 10f * Time.fixedDeltaTime * 50f, ForceMode.Force);
			this.rb.AddTorque(this.randomTorque * 30f * Time.fixedDeltaTime * 50f, ForceMode.Force);
			if (this.timeToTwist > 200)
			{
				this.randomTorque = Random.insideUnitSphere.normalized * Random.Range(0f, 0.5f);
				this.timeToTwist = 0;
				if (this.rb.velocity.magnitude < 0.5f && !this.physgrabobject.grabbed)
				{
					this.rb.AddForce(base.transform.up * 5f, ForceMode.Impulse);
					this.rb.AddTorque(this.randomTorque * 20f, ForceMode.Impulse);
				}
			}
		}
	}

	// Token: 0x06001531 RID: 5425 RVA: 0x000BB102 File Offset: 0x000B9302
	public void TrapStop()
	{
		this.trapActive = false;
		this.LoopPlaying = false;
		this.DeparentAndDestroy(this.bottleParticleSystem);
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x000BB120 File Offset: 0x000B9320
	private void DeparentAndDestroy(ParticleSystem particleSystem)
	{
		if (particleSystem != null && particleSystem.isPlaying)
		{
			particleSystem.gameObject.transform.parent = null;
			particleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
			particleSystem.Stop(false);
			particleSystem = null;
		}
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x000BB168 File Offset: 0x000B9368
	public void IncrementTimeToTwist()
	{
		this.timeToTwist++;
	}

	// Token: 0x06001534 RID: 5428 RVA: 0x000BB178 File Offset: 0x000B9378
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.Pop.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.bottleTimer.Invoke();
			this.Cork.SetActive(false);
			this.corkParticleSystem.Play(false);
			this.bottleParticleSystem.Play(false);
			this.GrabRelease();
			this.trapActive = true;
			this.trapTriggered = true;
		}
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x000BB1FC File Offset: 0x000B93FC
	private void OnDestroy()
	{
		if (!this.bottleParticleSystem)
		{
			return;
		}
		this.bottleParticleSystem.transform.parent = null;
		this.bottleParticleSystem.Stop(true);
		this.bottleParticleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x000BB248 File Offset: 0x000B9448
	public void GrabRelease()
	{
		bool flag = false;
		foreach (PhysGrabber physGrabber in Enumerable.ToList<PhysGrabber>(this.physGrabObject.playerGrabbing))
		{
			if (!SemiFunc.IsMultiplayer())
			{
				physGrabber.ReleaseObject(0.1f);
			}
			else
			{
				physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
				{
					false,
					0.1f
				});
			}
			flag = true;
		}
		if (flag)
		{
			if (GameManager.instance.gameMode == 0)
			{
				this.GrabReleaseRPC(default(PhotonMessageInfo));
				return;
			}
			this.photonView.RPC("GrabReleaseRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06001537 RID: 5431 RVA: 0x000BB318 File Offset: 0x000B9518
	[PunRPC]
	private void GrabReleaseRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.physGrabObject.grabDisableTimer = 1f;
	}

	// Token: 0x0400248B RID: 9355
	public UnityEvent bottleTimer;

	// Token: 0x0400248C RID: 9356
	private PhysGrabObject physgrabobject;

	// Token: 0x0400248D RID: 9357
	[Space]
	[Header("Bottle Components")]
	public GameObject Bottle;

	// Token: 0x0400248E RID: 9358
	public GameObject Cork;

	// Token: 0x0400248F RID: 9359
	[Space]
	[Header("Sounds")]
	public Sound Pop;

	// Token: 0x04002490 RID: 9360
	public Sound FlyLoop;

	// Token: 0x04002491 RID: 9361
	[Space]
	private Quaternion initialBottleRotation;

	// Token: 0x04002492 RID: 9362
	private Rigidbody rb;

	// Token: 0x04002493 RID: 9363
	private bool LoopPlaying;

	// Token: 0x04002494 RID: 9364
	private Vector3 randomTorque;

	// Token: 0x04002495 RID: 9365
	private int timeToTwist;

	// Token: 0x04002496 RID: 9366
	public ParticleSystem bottleParticleSystem;

	// Token: 0x04002497 RID: 9367
	public ParticleSystem corkParticleSystem;
}
