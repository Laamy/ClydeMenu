using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002AF RID: 687
public class ValuableCar : Trap
{
	// Token: 0x06001578 RID: 5496 RVA: 0x000BDAB4 File Offset: 0x000BBCB4
	protected override void Start()
	{
		base.Start();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.currentState = ValuableCar.State.Inactive;
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x000BDAD0 File Offset: 0x000BBCD0
	protected override void Update()
	{
		base.Update();
		this.loopPlaying = (this.currentState == ValuableCar.State.MoveForward);
		this.loopPitch = Mathf.Lerp(this.loopPitch, 1f + this.physGrabObject.rb.velocity.magnitude * 1f, Time.deltaTime * 5f);
		this.sfxCarLoop.PlayLoop(this.loopPlaying, 0.8f, 0.8f, this.loopPitch);
		if (this.loopPlaying != this.loopPlayingPrevious)
		{
			if (this.loopPlaying)
			{
				this.sfxCarStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			else
			{
				this.sfxCarStop.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.loopPlayingPrevious = this.loopPlaying;
		}
		if (this.physGrabObject.grabbed)
		{
			if (!this.grabbedPrev)
			{
				this.grabbedPrev = true;
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					this.PlayHonk();
					Quaternion turnX = Quaternion.Euler(-45f, 0f, 0f);
					Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
					Quaternion identity = Quaternion.identity;
					this.physGrabObject.TurnXYZ(turnX, turnY, identity);
				}
				if (this.physGrabObject.grabbedLocal)
				{
				}
			}
		}
		else
		{
			this.grabbedPrev = false;
		}
		float num = 0.102f;
		if (this.currentState == ValuableCar.State.MoveForward)
		{
			if (this.carBodyLerp < 1f)
			{
				this.carBodyLerp += Time.deltaTime * 4f;
			}
			if (this.carBodyLerp >= 1f)
			{
				this.carBodyLerp = 0f;
			}
			float num2 = this.carBodyCurve.Evaluate(this.carBodyLerp) * 0.015f;
			this.carBody.localPosition = new Vector3(this.carBody.localPosition.x, num2 + num, this.carBody.localPosition.z);
			if (this.wheelsLerp < 1f)
			{
				this.wheelsLerp += Time.deltaTime * 4f;
			}
			if (this.wheelsLerp >= 1f)
			{
				this.wheelsLerp = 0f;
			}
			float x = 720f;
			this.wheelsFront.Rotate(new Vector3(x, 0f, 0f) * Time.deltaTime, Space.Self);
			this.wheelsBack.Rotate(new Vector3(x, 0f, 0f) * Time.deltaTime, Space.Self);
			if (this.driverBodyLerp < 1f)
			{
				this.driverBodyLerp += Time.deltaTime * 1f;
			}
			if (this.driverBodyLerp >= 1f)
			{
				this.driverBodyLerp = 0f;
			}
			float z = this.carBodyCurve.Evaluate(this.carBodyLerp) * 6f;
			this.driverBody.localRotation = Quaternion.Euler(this.driverBody.localRotation.eulerAngles.x, this.driverBody.localRotation.eulerAngles.y, z);
			if (this.driverArmsLerp < 1f)
			{
				this.driverArmsLerp += Time.deltaTime * 1f;
			}
			if (this.driverArmsLerp >= 1f)
			{
				this.driverArmsLerp = 0f;
			}
			this.driverArms.localRotation = Quaternion.Euler(this.driverArms.localRotation.eulerAngles.x, this.driverArms.localRotation.eulerAngles.y, this.driverArmsCurve.Evaluate(this.driverArmsLerp) * 20f);
		}
		else if (this.stateImpulse)
		{
			this.carBody.localPosition = new Vector3(this.carBody.localPosition.x, num, this.carBody.localPosition.z);
			this.wheelsFront.localRotation = Quaternion.Euler(this.wheelsFront.localRotation.eulerAngles.x, this.wheelsFront.localRotation.eulerAngles.y, this.wheelsFront.localRotation.eulerAngles.z);
			this.wheelsBack.localRotation = Quaternion.Euler(this.wheelsBack.localRotation.eulerAngles.x, this.wheelsBack.localRotation.eulerAngles.y, this.wheelsBack.localRotation.eulerAngles.y);
			this.driverArms.localRotation = Quaternion.Euler(this.driverArms.localRotation.eulerAngles.x, this.driverArms.localRotation.eulerAngles.y, this.driverArms.localRotation.eulerAngles.z);
		}
		this.StateMachine(false);
		if (SemiFunc.FPSImpulse5() && SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.currentState == ValuableCar.State.Inactive)
			{
				return;
			}
			if (this.impactDetector.inCart || this.physGrabObject.playerGrabbing.Count > 0)
			{
				this.UpdateState(ValuableCar.State.Idle);
				return;
			}
			bool flag = false;
			Collider[] array = Physics.OverlapBox(this.centerTransform.position, this.boxSize / 2f, base.transform.rotation, SemiFunc.LayerMaskGetVisionObstruct());
			int i = 0;
			while (i < array.Length)
			{
				if (!array[i].GetComponentInParent<ValuableCar>())
				{
					flag = true;
					if (this.currentState != ValuableCar.State.MoveForward && this.currentState != ValuableCar.State.Inactive)
					{
						this.UpdateState(ValuableCar.State.MoveForward);
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			if (!flag)
			{
				this.UpdateState(ValuableCar.State.Idle);
			}
		}
		this.CheckForNearbyPlayers();
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x000BE0D4 File Offset: 0x000BC2D4
	public void FixedUpdate()
	{
		this.StateMachine(true);
	}

	// Token: 0x0600157B RID: 5499 RVA: 0x000BE0DD File Offset: 0x000BC2DD
	private void StateInactive(bool _fixedUpdate)
	{
		if (_fixedUpdate)
		{
			return;
		}
		if (this.physGrabObject.grabbed)
		{
			this.UpdateState(ValuableCar.State.Idle);
		}
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x000BE0F8 File Offset: 0x000BC2F8
	private void StateIdle(bool _fixedUpdate)
	{
		if (_fixedUpdate)
		{
			return;
		}
		if (this.stateImpulse)
		{
			foreach (Collider collider in this.carColliders)
			{
				if (!(collider == null))
				{
					collider.material = this.defaultPhysicMaterial;
				}
			}
			this.stateImpulse = false;
		}
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x000BE16C File Offset: 0x000BC36C
	private void StateMoveForward(bool _fixedUpdate)
	{
		if (!_fixedUpdate || !this.playersNearby)
		{
			return;
		}
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			foreach (Collider collider in this.carColliders)
			{
				if (!(collider == null))
				{
					collider.material = this.movingPhysicMaterial;
				}
			}
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.fixedDeltaTime;
		if (this.stateTimer <= 0f)
		{
			this.stateTimer = 1f;
			if (Random.Range(0f, 1f) > 0.5f)
			{
				this.TurnCar();
			}
		}
		float d = 4f - this.physGrabObject.rb.velocity.magnitude;
		this.physGrabObject.rb.AddForce(base.transform.forward * d * Time.fixedDeltaTime, ForceMode.Impulse);
		Vector3 torque = new Vector3(0f, Mathf.Sin(Time.time * 10f) * 0.006f, 0f);
		this.physGrabObject.rb.AddTorque(torque, ForceMode.Impulse);
	}

	// Token: 0x0600157E RID: 5502 RVA: 0x000BE2C0 File Offset: 0x000BC4C0
	private void UpdateState(ValuableCar.State _state)
	{
		if (_state == this.currentState)
		{
			return;
		}
		this.currentState = _state;
		this.stateImpulse = true;
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("UpdateStateRPC", RpcTarget.All, new object[]
			{
				this.currentState
			});
			return;
		}
		this.UpdateStateRPC(this.currentState, default(PhotonMessageInfo));
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x000BE327 File Offset: 0x000BC527
	[PunRPC]
	private void UpdateStateRPC(ValuableCar.State _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x000BE33C File Offset: 0x000BC53C
	private void PlayHonk()
	{
		EnemyDirector.instance.SetInvestigate(base.transform.position, 15f, false);
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("PlayHonkRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.PlayHonkRPC(default(PhotonMessageInfo));
	}

	// Token: 0x06001581 RID: 5505 RVA: 0x000BE391 File Offset: 0x000BC591
	[PunRPC]
	private void PlayHonkRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.sfxCarHorn.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001582 RID: 5506 RVA: 0x000BE3C8 File Offset: 0x000BC5C8
	private void StateMachine(bool _fixedUpdate)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			switch (this.currentState)
			{
			case ValuableCar.State.Inactive:
				this.StateInactive(_fixedUpdate);
				return;
			case ValuableCar.State.Idle:
				this.StateIdle(_fixedUpdate);
				return;
			case ValuableCar.State.MoveForward:
				this.StateMoveForward(_fixedUpdate);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06001583 RID: 5507 RVA: 0x000BE410 File Offset: 0x000BC610
	private void TurnCar()
	{
		Vector3 torque = new Vector3(0f, Random.Range(-1f, 1f) * 0.1f, 0f);
		this.physGrabObject.rb.AddTorque(torque, ForceMode.Impulse);
		this.PlayHonk();
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x000BE45C File Offset: 0x000BC65C
	private void CheckForNearbyPlayers()
	{
		if (!SemiFunc.FPSImpulse5())
		{
			return;
		}
		using (List<PlayerAvatar>.Enumerator enumerator = SemiFunc.PlayerGetList().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.transform.position, base.transform.position) < 20f)
				{
					this.playersNearby = true;
					break;
				}
				this.playersNearby = false;
			}
		}
	}

	// Token: 0x0400254B RID: 9547
	[Header("Animation")]
	public Transform carBody;

	// Token: 0x0400254C RID: 9548
	public Transform wheelsFront;

	// Token: 0x0400254D RID: 9549
	public Transform wheelsBack;

	// Token: 0x0400254E RID: 9550
	public Transform driverBody;

	// Token: 0x0400254F RID: 9551
	public Transform driverArms;

	// Token: 0x04002550 RID: 9552
	public AnimationCurve carBodyCurve;

	// Token: 0x04002551 RID: 9553
	private float carBodyLerp;

	// Token: 0x04002552 RID: 9554
	private float wheelsLerp;

	// Token: 0x04002553 RID: 9555
	private float driverBodyLerp = 0.9f;

	// Token: 0x04002554 RID: 9556
	public AnimationCurve driverArmsCurve;

	// Token: 0x04002555 RID: 9557
	private float driverArmsLerp;

	// Token: 0x04002556 RID: 9558
	[Header("Sounds")]
	public Sound sfxCarHorn;

	// Token: 0x04002557 RID: 9559
	public Sound sfxCarStart;

	// Token: 0x04002558 RID: 9560
	public Sound sfxCarStop;

	// Token: 0x04002559 RID: 9561
	public Sound sfxCarLoop;

	// Token: 0x0400255A RID: 9562
	private bool grabbedPrev;

	// Token: 0x0400255B RID: 9563
	private bool loopPlaying;

	// Token: 0x0400255C RID: 9564
	private bool loopPlayingPrevious;

	// Token: 0x0400255D RID: 9565
	private float loopPitch;

	// Token: 0x0400255E RID: 9566
	[Header("Colliders")]
	public List<Collider> carColliders;

	// Token: 0x0400255F RID: 9567
	private bool playersNearby;

	// Token: 0x04002560 RID: 9568
	public PhysicMaterial defaultPhysicMaterial;

	// Token: 0x04002561 RID: 9569
	public PhysicMaterial movingPhysicMaterial;

	// Token: 0x04002562 RID: 9570
	public Transform centerTransform;

	// Token: 0x04002563 RID: 9571
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04002564 RID: 9572
	private Vector3 boxSize = new Vector3(0.16f, 0.1f, 0.42f);

	// Token: 0x04002565 RID: 9573
	private bool activated;

	// Token: 0x04002566 RID: 9574
	private ValuableCar.State currentState;

	// Token: 0x04002567 RID: 9575
	private bool stateImpulse;

	// Token: 0x04002568 RID: 9576
	private float stateTimer;

	// Token: 0x02000415 RID: 1045
	public enum State
	{
		// Token: 0x04002DBB RID: 11707
		Inactive,
		// Token: 0x04002DBC RID: 11708
		Idle,
		// Token: 0x04002DBD RID: 11709
		MoveForward
	}
}
