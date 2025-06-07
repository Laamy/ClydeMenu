using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002AC RID: 684
public class BabyHeadValuable : Trap
{
	// Token: 0x0600155B RID: 5467 RVA: 0x000BC526 File Offset: 0x000BA726
	protected override void Start()
	{
		base.Start();
		this.rb = base.GetComponent<Rigidbody>();
		this.headTiltAngle = Vector3.Angle(this.head.up, Vector3.up);
	}

	// Token: 0x0600155C RID: 5468 RVA: 0x000BC558 File Offset: 0x000BA758
	protected override void Update()
	{
		base.Update();
		this.crySound.PlayLoop(this.isAwake, 1f, 1f, 1f);
		this.AdjustEyelidPivotBasedOnhHeadTilt();
		this.CheckForNearbyPlayers();
		if (this.isAwake)
		{
			this.enemyInvestigate = true;
			this.EyesAnimation();
		}
		this.AsleepAwakeHandle();
	}

	// Token: 0x0600155D RID: 5469 RVA: 0x000BC5B4 File Offset: 0x000BA7B4
	private void CheckForNearbyPlayers()
	{
		if (!SemiFunc.FPSImpulse5())
		{
			return;
		}
		this.players = SemiFunc.PlayerGetList();
		using (List<PlayerAvatar>.Enumerator enumerator = this.players.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.transform.position, base.transform.position) < this.cryDistance)
				{
					this.playersNearby = true;
					break;
				}
				this.playersNearby = false;
			}
		}
	}

	// Token: 0x0600155E RID: 5470 RVA: 0x000BC644 File Offset: 0x000BA844
	private void FixedUpdate()
	{
		if (this.isAwake && this.playersNearby)
		{
			this.BabyCry();
		}
	}

	// Token: 0x0600155F RID: 5471 RVA: 0x000BC65C File Offset: 0x000BA85C
	private void EyesAnimation()
	{
		this.eyeSnapTimer -= Time.deltaTime;
		if (this.eyeSnapTimer >= 0f)
		{
			return;
		}
		float x = Random.Range(-25f, 25f);
		float y = Random.Range(-50f, 50f);
		Quaternion b = Quaternion.Euler(x, y, 0f);
		this.leftEyePivot.localRotation = Quaternion.Lerp(this.leftEyePivot.localRotation, b, 1f);
		this.rightEyePivot.localRotation = Quaternion.Lerp(this.rightEyePivot.localRotation, b, 1f);
		this.eyeSnapTimer = Random.Range(this.joltIntervalMin, this.joltIntervalMax);
	}

	// Token: 0x06001560 RID: 5472 RVA: 0x000BC710 File Offset: 0x000BA910
	private void AsleepTransitionAnimation(Quaternion newPos)
	{
		this.eyelidPivot.localRotation = Quaternion.Lerp(this.eyelidPivot.localRotation, newPos, this.eyeLerpSpeed * Time.deltaTime);
		this.leftEyePivot.localRotation = Quaternion.Lerp(this.leftEye.localRotation, Quaternion.Euler(0f, 0f, 0f), this.eyeLerpSpeed * Time.deltaTime);
		this.rightEyePivot.localRotation = Quaternion.Lerp(this.rightEye.localRotation, Quaternion.Euler(0f, 0f, 0f), this.eyeLerpSpeed * Time.deltaTime);
		if (Quaternion.Angle(this.eyelidPivot.localRotation, newPos) < 1f)
		{
			this.animating = false;
			this.eyelidPivot.localRotation = newPos;
		}
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x000BC7E8 File Offset: 0x000BA9E8
	private void AdjustEyelidPivotBasedOnhHeadTilt()
	{
		if (this.isAwake)
		{
			return;
		}
		float t = Mathf.Clamp01(Mathf.InverseLerp(0f, this.maxTiltAngle, this.headTiltAngle));
		if (!this.animating)
		{
			this.eyelidPivot.localRotation = Quaternion.Lerp(this.awakeEyelidRotation, this.asleepEyelidRotation, t);
			return;
		}
		this.AsleepTransitionAnimation(Quaternion.Lerp(this.awakeEyelidRotation, this.asleepEyelidRotation, t));
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x000BC858 File Offset: 0x000BAA58
	private void AsleepAwakeHandle()
	{
		this.headTiltAngle = Vector3.Angle(this.head.up, Vector3.up);
		if (this.IsInAwakePosition())
		{
			if (this.asleepTimerOn)
			{
				this.asleepTimerOn = false;
				this.transitionTimer = 0f;
			}
			if (this.awakeTimerOn && !this.isAwake)
			{
				this.transitionTimer += Time.fixedDeltaTime;
				if (this.transitionTimer >= this.awakeTransitionTime)
				{
					this.BabyAwake();
					this.awakeTimerOn = false;
					this.transitionTimer = 0f;
					return;
				}
			}
			else if (!this.isAwake)
			{
				this.awakeTimerOn = true;
				return;
			}
		}
		else
		{
			if (this.awakeTimerOn)
			{
				this.awakeTimerOn = false;
				this.transitionTimer = 0f;
			}
			if (this.asleepTimerOn && this.isAwake)
			{
				this.transitionTimer += Time.fixedDeltaTime;
				if (this.transitionTimer >= this.asleepTransitionTime)
				{
					this.BabySleep();
					this.asleepTimerOn = false;
					this.transitionTimer = 0f;
					return;
				}
			}
			else if (this.isAwake)
			{
				this.asleepTimerOn = true;
			}
		}
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x000BC970 File Offset: 0x000BAB70
	private void BabyAwake()
	{
		this.awakeSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
		this.isAwake = true;
		this.cryJoltTimer = Random.Range(this.joltIntervalMin, this.joltIntervalMax);
		this.eyeSnapTimer = Random.Range(this.joltIntervalMin, this.joltIntervalMax);
		this.eyelidPivot.localRotation = Quaternion.Lerp(this.eyelidPivot.localRotation, this.awakeEyelidRotation, 1f);
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x000BCA03 File Offset: 0x000BAC03
	private void BabySleep()
	{
		this.asleepSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
		this.isAwake = false;
		this.animating = true;
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x000BCA3E File Offset: 0x000BAC3E
	private bool IsInAwakePosition()
	{
		return this.headTiltAngle < this.headAwakeAngle;
	}

	// Token: 0x06001566 RID: 5478 RVA: 0x000BCA50 File Offset: 0x000BAC50
	private void BabyCry()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.cryJoltTimer -= Time.deltaTime;
		if (this.cryJoltTimer <= 0f)
		{
			if (this.rb.velocity.magnitude < this.maxVelocity)
			{
				this.rb.AddForce(Vector3.up * Random.Range(this.joltForceMin, this.joltForceMax), ForceMode.Impulse);
				Vector3 torque = Random.insideUnitSphere.normalized * this.torqueMultiplier;
				this.rb.AddTorque(torque, ForceMode.Impulse);
			}
			this.cryJoltTimer = Random.Range(this.joltIntervalMin, this.joltIntervalMax);
		}
	}

	// Token: 0x040024ED RID: 9453
	[Header("Transforms")]
	public Transform head;

	// Token: 0x040024EE RID: 9454
	public Transform eyelidPivot;

	// Token: 0x040024EF RID: 9455
	public Transform leftEyePivot;

	// Token: 0x040024F0 RID: 9456
	public Transform rightEyePivot;

	// Token: 0x040024F1 RID: 9457
	public Transform leftEye;

	// Token: 0x040024F2 RID: 9458
	public Transform rightEye;

	// Token: 0x040024F3 RID: 9459
	[Header("Awake and asleep eyelid rotations")]
	public Quaternion awakeEyelidRotation;

	// Token: 0x040024F4 RID: 9460
	public Quaternion asleepEyelidRotation;

	// Token: 0x040024F5 RID: 9461
	[Header("Head tilt variables")]
	public float maxTiltAngle = 90f;

	// Token: 0x040024F6 RID: 9462
	public float headAwakeAngle = 45f;

	// Token: 0x040024F7 RID: 9463
	[Header("Eyelid animation variables")]
	public float eyeLerpSpeed = 2f;

	// Token: 0x040024F8 RID: 9464
	[Header("Sounds")]
	public Sound awakeSound;

	// Token: 0x040024F9 RID: 9465
	public Sound crySound;

	// Token: 0x040024FA RID: 9466
	public Sound asleepSound;

	// Token: 0x040024FB RID: 9467
	[Header("Transition times")]
	public float asleepTransitionTime = 0.5f;

	// Token: 0x040024FC RID: 9468
	public float awakeTransitionTime = 0.5f;

	// Token: 0x040024FD RID: 9469
	[Header("Crying shake variables")]
	public float joltIntervalMin = 0.2f;

	// Token: 0x040024FE RID: 9470
	public float joltIntervalMax = 1f;

	// Token: 0x040024FF RID: 9471
	[Space]
	public float joltForceMin = 1f;

	// Token: 0x04002500 RID: 9472
	public float joltForceMax = 5f;

	// Token: 0x04002501 RID: 9473
	[Space]
	public float torqueMultiplier = 0.5f;

	// Token: 0x04002502 RID: 9474
	[Space]
	public float maxVelocity = 2f;

	// Token: 0x04002503 RID: 9475
	[Header("Cry distance variables")]
	public float cryDistance = 20f;

	// Token: 0x04002504 RID: 9476
	protected Rigidbody rb;

	// Token: 0x04002505 RID: 9477
	private bool asleepTimerOn;

	// Token: 0x04002506 RID: 9478
	private bool awakeTimerOn;

	// Token: 0x04002507 RID: 9479
	private bool isAwake;

	// Token: 0x04002508 RID: 9480
	private bool animating;

	// Token: 0x04002509 RID: 9481
	private bool playersNearby;

	// Token: 0x0400250A RID: 9482
	private float transitionTimer;

	// Token: 0x0400250B RID: 9483
	private float cryJoltTimer;

	// Token: 0x0400250C RID: 9484
	private float eyeSnapTimer;

	// Token: 0x0400250D RID: 9485
	private float eyeSnapTime;

	// Token: 0x0400250E RID: 9486
	private float headTiltAngle;

	// Token: 0x0400250F RID: 9487
	private List<PlayerAvatar> players;
}
