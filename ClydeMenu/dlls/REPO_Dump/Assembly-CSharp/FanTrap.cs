using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002A8 RID: 680
public class FanTrap : Trap
{
	// Token: 0x06001548 RID: 5448 RVA: 0x000BBB50 File Offset: 0x000B9D50
	protected override void Start()
	{
		base.Start();
		this.hurtCollider.gameObject.SetActive(false);
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
		this.initialPlayerHitForce = this.hurtCollider.playerHitForce;
		this.initialPhysHitForce = this.hurtCollider.physHitForce;
		this.initialEnemyHitForce = this.hurtCollider.enemyHitForce;
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x000BBBBF File Offset: 0x000B9DBF
	private void FixedUpdate()
	{
	}

	// Token: 0x0600154A RID: 5450 RVA: 0x000BBBC4 File Offset: 0x000B9DC4
	protected override void Update()
	{
		base.Update();
		FanTrap.States states = this.currentState;
		if (states != FanTrap.States.Idle)
		{
			if (states == FanTrap.States.Active)
			{
				this.StateActive();
			}
		}
		else
		{
			this.StateIdle();
		}
		this.hurtCollider.gameObject.SetActive(this.currentState == FanTrap.States.Active);
		this.sfxFanLoop.PlayLoop(this.currentState == FanTrap.States.Active, 0.1f, 0.025f, 1f);
		this.sfxFanLoop.LoopPitch = Mathf.Lerp(0.1f, 1f, this.fanBladeSpeedCurve.Evaluate(this.fanBladeLerp));
		this.hurtCollider.playerHitForce = Mathf.Lerp(0f, this.initialPlayerHitForce, this.fanBladeSpeedCurve.Evaluate(this.fanBladeLerp));
		this.hurtCollider.physHitForce = Mathf.Lerp(0f, this.initialPhysHitForce, this.fanBladeSpeedCurve.Evaluate(this.fanBladeLerp));
		this.hurtCollider.enemyHitForce = Mathf.Lerp(0f, this.initialEnemyHitForce, this.fanBladeSpeedCurve.Evaluate(this.fanBladeLerp));
		this.fanBlades.Rotate(-Vector3.forward * this.fanBladeSpeed * Time.deltaTime);
		this.fanBladeSpeed = Mathf.Lerp(0f, this.fanBladeMaxSpeed, this.fanBladeSpeedCurve.Evaluate(this.fanBladeLerp));
	}

	// Token: 0x0600154B RID: 5451 RVA: 0x000BBD30 File Offset: 0x000B9F30
	private void StateActive()
	{
		if (this.stateStart)
		{
			this.sfxButtonOn.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.windParticles.Play();
			this.windSmallParticles.Play();
			this.buttonMesh.material.EnableKeyword("_EMISSION");
			this.fanTimer.Invoke();
			this.stateStart = false;
		}
		this.enemyInvestigate = true;
		if (SemiFunc.IsMasterClientOrSingleplayer() && !this.physgrabobject.grabbed)
		{
			this.SetState(FanTrap.States.Idle);
		}
		this.fanButton.localEulerAngles = new Vector3(21f, 0f, 0f);
		if (this.fanBladeLerp < 1f)
		{
			this.fanBladeLerp += Time.deltaTime / this.secondsToStart;
		}
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x000BBE14 File Offset: 0x000BA014
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.sfxButtonOff.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.windParticles.Stop();
			this.windSmallParticles.Stop();
			this.buttonMesh.material.DisableKeyword("_EMISSION");
			this.stateStart = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.physgrabobject.grabbed)
		{
			this.SetState(FanTrap.States.Active);
		}
		this.fanButton.localEulerAngles = new Vector3(0f, 0f, 0f);
		if (this.fanBladeLerp > 0f)
		{
			this.fanBladeLerp -= Time.deltaTime / this.secondsToStop;
		}
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x000BBEE6 File Offset: 0x000BA0E6
	[PunRPC]
	public void SetStateRPC(FanTrap.States state)
	{
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x000BBEF6 File Offset: 0x000BA0F6
	private void SetState(FanTrap.States state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.SetStateRPC(state);
			return;
		}
		this.photonView.RPC("SetStateRPC", RpcTarget.All, new object[]
		{
			state
		});
	}

	// Token: 0x040024C0 RID: 9408
	public UnityEvent fanTimer;

	// Token: 0x040024C1 RID: 9409
	public Transform fanButton;

	// Token: 0x040024C2 RID: 9410
	public HurtCollider hurtCollider;

	// Token: 0x040024C3 RID: 9411
	public MeshRenderer buttonMesh;

	// Token: 0x040024C4 RID: 9412
	private float initialPlayerHitForce;

	// Token: 0x040024C5 RID: 9413
	private float initialPhysHitForce;

	// Token: 0x040024C6 RID: 9414
	private float initialEnemyHitForce;

	// Token: 0x040024C7 RID: 9415
	[Header("Fan Blade")]
	public Transform fanBlades;

	// Token: 0x040024C8 RID: 9416
	public AnimationCurve fanBladeSpeedCurve;

	// Token: 0x040024C9 RID: 9417
	private PhysGrabObject physgrabobject;

	// Token: 0x040024CA RID: 9418
	private float fanBladeSpeed;

	// Token: 0x040024CB RID: 9419
	private float fanBladeMaxSpeed = 1500f;

	// Token: 0x040024CC RID: 9420
	private float fanBladeLerp;

	// Token: 0x040024CD RID: 9421
	private float secondsToStart = 2f;

	// Token: 0x040024CE RID: 9422
	private float secondsToStop = 4f;

	// Token: 0x040024CF RID: 9423
	internal FanTrap.States currentState;

	// Token: 0x040024D0 RID: 9424
	private bool stateStart;

	// Token: 0x040024D1 RID: 9425
	[Header("Sounds")]
	public Sound sfxButtonOn;

	// Token: 0x040024D2 RID: 9426
	public Sound sfxButtonOff;

	// Token: 0x040024D3 RID: 9427
	public Sound sfxFanLoop;

	// Token: 0x040024D4 RID: 9428
	[Header("Particles")]
	public ParticleSystem windParticles;

	// Token: 0x040024D5 RID: 9429
	public ParticleSystem windSmallParticles;

	// Token: 0x02000413 RID: 1043
	public enum States
	{
		// Token: 0x04002DB2 RID: 11698
		Idle,
		// Token: 0x04002DB3 RID: 11699
		Active
	}
}
