using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002A3 RID: 675
public class PropaneTankTrap : Trap
{
	// Token: 0x06001520 RID: 5408 RVA: 0x000BA820 File Offset: 0x000B8A20
	protected override void Start()
	{
		base.Start();
		this.initialTankRotation = this.Tank.transform.localRotation;
		this.rb = base.GetComponent<Rigidbody>();
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.hurtCollider.gameObject.SetActive(false);
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x000BA880 File Offset: 0x000B8A80
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
			this.hurtCollider.gameObject.SetActive(true);
			float num = 1f;
			float num2 = 40f;
			float num3 = num * Mathf.Sin(Time.time * num2);
			float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
			this.Tank.transform.localRotation = this.initialTankRotation * Quaternion.Euler(num3, 0f, z);
			this.Tank.transform.localPosition = new Vector3(this.Tank.transform.localPosition.x, this.Tank.transform.localPosition.y - num3 * 0.005f * Time.deltaTime, this.Tank.transform.localPosition.z);
		}
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x000BA9B0 File Offset: 0x000B8BB0
	private void FixedUpdate()
	{
		if (this.trapActive && this.isLocal)
		{
			this.rb.AddForce(-base.transform.forward * 0.3f * 40f * Time.fixedDeltaTime * 50f, ForceMode.Force);
			this.rb.AddForce(Vector3.up * 0.1f * 10f * Time.fixedDeltaTime * 50f, ForceMode.Force);
			this.rb.AddTorque(-base.transform.right * 0.1f * 10f * Time.fixedDeltaTime * 50f, ForceMode.Force);
			if (this.timeToTwist > 200)
			{
				this.randomTorque = Random.insideUnitSphere.normalized * Random.Range(0f, 0.5f);
				this.timeToTwist = 0;
				if (this.rb.velocity.magnitude < 0.5f && !this.physgrabobject.grabbed)
				{
					this.rb.AddForce(base.transform.forward * 5f, ForceMode.Impulse);
					this.rb.AddTorque(this.randomTorque * 20f, ForceMode.Impulse);
				}
			}
		}
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x000BAB38 File Offset: 0x000B8D38
	public void TrapStop()
	{
		this.trapActive = false;
		this.flyEnd.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
		this.LoopPlaying = false;
		this.hurtCollider.gameObject.SetActive(false);
		this.DeparentAndDestroy(this.smokeParticleSystem);
		this.DeparentAndDestroy(this.fireParticleSystem);
		this.fireLight.SetActive(false);
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x000BABB4 File Offset: 0x000B8DB4
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

	// Token: 0x06001525 RID: 5413 RVA: 0x000BABFC File Offset: 0x000B8DFC
	public void IncrementTimeToTwist()
	{
		this.timeToTwist++;
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x000BAC0C File Offset: 0x000B8E0C
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.tankTimer.Invoke();
			this.fireParticleSystem.Play(false);
			this.fireLight.SetActive(true);
			this.flyStart.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.GrabRelease();
			this.trapActive = true;
			this.trapTriggered = true;
		}
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x000BAC84 File Offset: 0x000B8E84
	public void Explode()
	{
		this.particleScriptExplosion.Spawn(this.Center.position, 0.8f, 50, 100, 1f, false, false, 1f);
		this.DeparentAndDestroy(this.smokeParticleSystem);
		this.DeparentAndDestroy(this.fireParticleSystem);
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x000BACD8 File Offset: 0x000B8ED8
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

	// Token: 0x06001529 RID: 5417 RVA: 0x000BADA8 File Offset: 0x000B8FA8
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

	// Token: 0x04002479 RID: 9337
	public UnityEvent tankTimer;

	// Token: 0x0400247A RID: 9338
	private PhysGrabObject physgrabobject;

	// Token: 0x0400247B RID: 9339
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x0400247C RID: 9340
	[Space]
	public GameObject Tank;

	// Token: 0x0400247D RID: 9341
	public Transform Center;

	// Token: 0x0400247E RID: 9342
	[Space]
	[Header("Sounds")]
	public Sound Pop;

	// Token: 0x0400247F RID: 9343
	public Sound FlyLoop;

	// Token: 0x04002480 RID: 9344
	public Sound flyStart;

	// Token: 0x04002481 RID: 9345
	public Sound flyEnd;

	// Token: 0x04002482 RID: 9346
	[Space]
	private Quaternion initialTankRotation;

	// Token: 0x04002483 RID: 9347
	private Rigidbody rb;

	// Token: 0x04002484 RID: 9348
	private bool LoopPlaying;

	// Token: 0x04002485 RID: 9349
	private Vector3 randomTorque;

	// Token: 0x04002486 RID: 9350
	private int timeToTwist;

	// Token: 0x04002487 RID: 9351
	public HurtCollider hurtCollider;

	// Token: 0x04002488 RID: 9352
	public ParticleSystem smokeParticleSystem;

	// Token: 0x04002489 RID: 9353
	public ParticleSystem fireParticleSystem;

	// Token: 0x0400248A RID: 9354
	public GameObject fireLight;
}
