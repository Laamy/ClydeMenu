using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002AD RID: 685
public class BlenderValuable : Trap
{
	// Token: 0x06001568 RID: 5480 RVA: 0x000BCB9C File Offset: 0x000BAD9C
	protected override void Start()
	{
		base.Start();
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
		this.rb = base.GetComponent<Rigidbody>();
		this.animator = base.GetComponent<Animator>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.initialTankRotation = this.meshTransform.transform.localRotation;
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x000BCC04 File Offset: 0x000BAE04
	private void FixedUpdate()
	{
		switch (this.currentState)
		{
		case BlenderValuable.States.Idle:
			this.StateIdle();
			return;
		case BlenderValuable.States.Active:
			this.StateActive();
			this.InvokeImpactDamage();
			return;
		case BlenderValuable.States.Pop:
			this.StatePop();
			return;
		case BlenderValuable.States.IdleEmpty:
			this.StateIdle();
			return;
		case BlenderValuable.States.ActiveEmpty:
			this.StateActive();
			return;
		default:
			return;
		}
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x000BCC5C File Offset: 0x000BAE5C
	protected override void Update()
	{
		base.Update();
		this.soundBladeLoop.PlayLoop(this.fullLoopPlaying, 5f, 5f, 1f);
		this.soundBladeStuckLoop.PlayLoop(this.stuckLoopPlaying, 5f, 5f, 1f);
		this.soundBladeEmptyLoop.PlayLoop(this.emptyLoopPlaying, 5f, 5f, 1f);
		this.blade.Rotate(Vector3.up * this.bladeSpeed * Time.deltaTime);
		this.bladeSpeed = Mathf.Lerp(0f, this.bladeMaxSpeed, this.bladeCurve.Evaluate(this.bladeLerp));
		this.money.Rotate(Vector3.up * (this.bladeSpeed / 1.5f) * Time.deltaTime);
		this.moneyBot.Rotate(Vector3.up * (this.bladeSpeed / 1.5f) * Time.deltaTime);
		this.billSoup.Rotate(Vector3.up * (this.bladeSpeed / 5f) * Time.deltaTime);
		float num = 0.3f * this.bladeCurve.Evaluate(this.bladeLerp);
		float num2 = 60f * this.bladeCurve.Evaluate(this.bladeLerp);
		float num3 = num * Mathf.Sin(Time.time * num2);
		float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
		this.meshTransform.transform.localRotation = this.initialTankRotation * Quaternion.Euler(num3, 0f, z);
		this.meshTransform.transform.localPosition = new Vector3(this.meshTransform.transform.localPosition.x, this.meshTransform.transform.localPosition.y - num3 * 0.005f * Time.deltaTime, this.meshTransform.transform.localPosition.z);
		this.money.localPosition = new Vector3(this.money.localPosition.x, this.money.localPosition.y - num3 * 0.005f * Time.deltaTime, this.money.localPosition.z);
		this.billSoup.localPosition = new Vector3(this.billSoup.localPosition.x, this.billSoup.localPosition.y - num3 * 0.005f * Time.deltaTime, this.billSoup.localPosition.z);
		if (this.currentState == BlenderValuable.States.Active || this.currentState == BlenderValuable.States.ActiveEmpty || this.currentState == BlenderValuable.States.Pop)
		{
			if (this.bladeLerp < 1f)
			{
				this.bladeLerp += Time.deltaTime / this.secondsToStart;
				if (this.bladeLerp > 0.5f && !this.hurtCollider.gameObject.activeSelf)
				{
					this.continueBlendAfterRelease = true;
					this.hurtCollider.gameObject.SetActive(true);
				}
			}
			else
			{
				this.bladeLerp = 1f;
			}
			if (this.currentState == BlenderValuable.States.ActiveEmpty)
			{
				return;
			}
			if (this.currentState == BlenderValuable.States.Pop)
			{
				return;
			}
			this.billSoup.gameObject.SetActive(true);
			if (this.billSoupLerpTime < this.startLerpDuration && this.bladeLerp >= 1f)
			{
				this.billSoupLerpTime += Time.deltaTime;
				float t = this.billSoupLerpTime / this.startLerpDuration;
				this.billSoup.localScale = Vector3.Lerp(this.billSoupZeroScale, this.initialBillSoupScale, t);
			}
			else if (this.billSoupLerpTime < this.startLerpDuration + this.billSoupLerpDuration && this.bladeLerp >= 1f)
			{
				this.billSoupLerpTime += Time.deltaTime;
				float t2 = (this.billSoupLerpTime - this.startLerpDuration) / this.billSoupLerpDuration;
				this.billSoup.localScale = Vector3.Lerp(this.initialBillSoupScale, this.targetBillSoupScale, t2);
			}
			if (this.moneyLerpTime < this.moneyLerpDuration && this.bladeLerp >= 1f)
			{
				this.moneyLerpTime += Time.deltaTime;
				float t3 = this.moneyLerpTime / this.moneyLerpDuration;
				Vector3 localScale = Vector3.Lerp(this.initialMoneyScale, this.targetMoneyScale, t3);
				this.money.localScale = localScale;
				this.moneyBot.localScale = localScale;
			}
			if (this.moneyLerpTime > this.moneyLerpDuration * 0.75f)
			{
				if (!this.dustParticlesPlaying)
				{
					this.dustParticles.Play();
					this.dustParticlesPlaying = true;
					this.fullLoopPlaying = false;
					this.emptyLoopPlaying = false;
					this.stuckLoopPlaying = true;
				}
			}
			else if (this.bladeLerp >= 1f && !this.billParticlesRandom.isPlaying)
			{
				this.billParticlesRandom.Play();
			}
			if (this.moneyLerpTime > this.moneyLerpDuration)
			{
				this.money.gameObject.SetActive(false);
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					this.SetState(BlenderValuable.States.Pop);
					return;
				}
			}
		}
		else
		{
			if (this.bladeLerp > 0f)
			{
				this.bladeLerp -= Time.deltaTime / this.secondsToStop;
				return;
			}
			this.bladeLerp = 0f;
		}
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x000BD1B8 File Offset: 0x000BB3B8
	private void StateActive()
	{
		if (this.stateStart)
		{
			this.soundBladeStart.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.stateStart = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!this.physgrabobject.grabbed && this.currentState == BlenderValuable.States.Active)
			{
				if (this.continueBlendAfterRelease)
				{
					this.continueBlendAfterRelease = false;
				}
				this.SetState(BlenderValuable.States.Idle);
			}
			if (!this.physgrabobject.grabbed && this.currentState == BlenderValuable.States.ActiveEmpty)
			{
				if (!this.continueBlendAfterRelease)
				{
					this.SetState(BlenderValuable.States.IdleEmpty);
					return;
				}
				this.blenderTimer.Invoke();
			}
		}
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x000BD264 File Offset: 0x000BB464
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.hurtCollider.gameObject.SetActive(false);
			this.fullLoopPlaying = false;
			this.stuckLoopPlaying = false;
			this.emptyLoopPlaying = false;
			this.soundBladeEnd.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.stateStart = false;
			if (this.dustParticlesPlaying)
			{
				this.dustParticles.Stop();
				this.dustParticlesPlaying = false;
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.physgrabobject.grabbed && this.currentState == BlenderValuable.States.Idle)
			{
				this.fullLoopPlaying = true;
				this.SetState(BlenderValuable.States.Active);
			}
			if (this.physgrabobject.grabbed && this.currentState == BlenderValuable.States.IdleEmpty)
			{
				this.emptyLoopPlaying = true;
				this.SetState(BlenderValuable.States.ActiveEmpty);
			}
		}
	}

	// Token: 0x0600156D RID: 5485 RVA: 0x000BD33C File Offset: 0x000BB53C
	private void StatePop()
	{
		if (this.stateStart)
		{
			this.particleScriptExplosion.Spawn(this.explosionCenter.position, 0.5f, 0, 0, 0f, true, false, 1f);
			this.billSoup.gameObject.SetActive(false);
			this.billParticles.Play();
			this.capParticle.Play();
			this.capCollider.SetActive(false);
			this.cap.gameObject.SetActive(false);
			this.capPop.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.blast.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			if (this.dustParticlesPlaying)
			{
				this.dustParticles.Stop();
				this.dustParticlesPlaying = false;
			}
			this.stateStart = false;
			this.stateTimer = 0.5f;
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f && SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.SetState(BlenderValuable.States.ActiveEmpty);
		}
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x000BD474 File Offset: 0x000BB674
	public void InvokeImpactDamage()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.moneyLossTimer -= Time.deltaTime;
			if (this.moneyLossTimer <= 0f && this.bladeLerp >= 1f)
			{
				this.physgrabobject.lightBreakImpulse = true;
				this.moneyLossTimer = 0.33f;
			}
		}
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x000BD4CB File Offset: 0x000BB6CB
	[PunRPC]
	public void SetStateRPC(BlenderValuable.States state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x06001570 RID: 5488 RVA: 0x000BD4E4 File Offset: 0x000BB6E4
	private void SetState(BlenderValuable.States state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.SetStateRPC(state, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("SetStateRPC", RpcTarget.All, new object[]
		{
			state
		});
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x000BD531 File Offset: 0x000BB731
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.trapActive = true;
			this.trapTriggered = true;
		}
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x000BD549 File Offset: 0x000BB749
	public void TimerEnd()
	{
		this.continueBlendAfterRelease = false;
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x000BD552 File Offset: 0x000BB752
	public void TrapStop()
	{
		this.trapActive = false;
	}

	// Token: 0x04002510 RID: 9488
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x04002511 RID: 9489
	private float startLerpDuration = 1f;

	// Token: 0x04002512 RID: 9490
	private float moneyLossTimer = 1f;

	// Token: 0x04002513 RID: 9491
	public Sound soundBladeLoop;

	// Token: 0x04002514 RID: 9492
	public Sound soundBladeStart;

	// Token: 0x04002515 RID: 9493
	public Sound soundBladeEnd;

	// Token: 0x04002516 RID: 9494
	public Sound soundBladeStuckLoop;

	// Token: 0x04002517 RID: 9495
	public Sound soundBladeEmptyLoop;

	// Token: 0x04002518 RID: 9496
	public Sound capPop;

	// Token: 0x04002519 RID: 9497
	public Sound blast;

	// Token: 0x0400251A RID: 9498
	public ParticleSystem billParticles;

	// Token: 0x0400251B RID: 9499
	public ParticleSystem billParticlesRandom;

	// Token: 0x0400251C RID: 9500
	public ParticleSystem dustParticles;

	// Token: 0x0400251D RID: 9501
	public ParticleSystem capParticle;

	// Token: 0x0400251E RID: 9502
	[Space]
	public Transform meshTransform;

	// Token: 0x0400251F RID: 9503
	public UnityEvent blenderTimer;

	// Token: 0x04002520 RID: 9504
	private bool continueBlendAfterRelease;

	// Token: 0x04002521 RID: 9505
	public Transform blade;

	// Token: 0x04002522 RID: 9506
	public Transform money;

	// Token: 0x04002523 RID: 9507
	public Transform moneyBot;

	// Token: 0x04002524 RID: 9508
	public Transform billSoup;

	// Token: 0x04002525 RID: 9509
	public Transform cap;

	// Token: 0x04002526 RID: 9510
	public Transform explosionCenter;

	// Token: 0x04002527 RID: 9511
	public GameObject capCollider;

	// Token: 0x04002528 RID: 9512
	public AnimationCurve bladeCurve;

	// Token: 0x04002529 RID: 9513
	public AnimationCurve billSoupCurve;

	// Token: 0x0400252A RID: 9514
	public float bladeSpeed;

	// Token: 0x0400252B RID: 9515
	private float bladeMaxSpeed = 1500f;

	// Token: 0x0400252C RID: 9516
	private float bladeLerp;

	// Token: 0x0400252D RID: 9517
	private float secondsToStart = 2f;

	// Token: 0x0400252E RID: 9518
	private float secondsToStop = 0.5f;

	// Token: 0x0400252F RID: 9519
	private bool dustParticlesPlaying;

	// Token: 0x04002530 RID: 9520
	private Vector3 billSoupZeroScale = Vector3.zero;

	// Token: 0x04002531 RID: 9521
	private Vector3 initialBillSoupScale = new Vector3(0.8f, 0.1f, 0.8f);

	// Token: 0x04002532 RID: 9522
	private Vector3 targetBillSoupScale = new Vector3(1f, 1f, 1f);

	// Token: 0x04002533 RID: 9523
	private float billSoupLerpDuration = 5f;

	// Token: 0x04002534 RID: 9524
	private float billSoupLerpTime;

	// Token: 0x04002535 RID: 9525
	private Vector3 initialMoneyScale = new Vector3(0.85f, 0.85f, 0.85f);

	// Token: 0x04002536 RID: 9526
	private Vector3 targetMoneyScale = new Vector3(0.7f, 0.2f, 0.7f);

	// Token: 0x04002537 RID: 9527
	private float moneyLerpDuration = 10f;

	// Token: 0x04002538 RID: 9528
	private float moneyLerpTime;

	// Token: 0x04002539 RID: 9529
	public HurtCollider hurtCollider;

	// Token: 0x0400253A RID: 9530
	internal BlenderValuable.States currentState;

	// Token: 0x0400253B RID: 9531
	private bool stateStart;

	// Token: 0x0400253C RID: 9532
	private float stateTimer;

	// Token: 0x0400253D RID: 9533
	[Space]
	private Quaternion initialTankRotation;

	// Token: 0x0400253E RID: 9534
	private Animator animator;

	// Token: 0x0400253F RID: 9535
	private PhysGrabObject physgrabobject;

	// Token: 0x04002540 RID: 9536
	private Rigidbody rb;

	// Token: 0x04002541 RID: 9537
	private bool fullLoopPlaying;

	// Token: 0x04002542 RID: 9538
	private bool stuckLoopPlaying;

	// Token: 0x04002543 RID: 9539
	private bool emptyLoopPlaying;

	// Token: 0x02000414 RID: 1044
	public enum States
	{
		// Token: 0x04002DB5 RID: 11701
		Idle,
		// Token: 0x04002DB6 RID: 11702
		Active,
		// Token: 0x04002DB7 RID: 11703
		Pop,
		// Token: 0x04002DB8 RID: 11704
		IdleEmpty,
		// Token: 0x04002DB9 RID: 11705
		ActiveEmpty
	}
}
