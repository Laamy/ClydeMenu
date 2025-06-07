using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002A2 RID: 674
public class IceSawValuable : Trap
{
	// Token: 0x06001512 RID: 5394 RVA: 0x000BA0D8 File Offset: 0x000B82D8
	protected override void Start()
	{
		base.Start();
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
		this.rb = base.GetComponent<Rigidbody>();
		this.animator = base.GetComponent<Animator>();
		this.initialTankRotation = this.meshTransform.transform.localRotation;
	}

	// Token: 0x06001513 RID: 5395 RVA: 0x000BA134 File Offset: 0x000B8334
	private void FixedUpdate()
	{
		IceSawValuable.States states = this.currentState;
		if (states != IceSawValuable.States.Idle)
		{
			if (states == IceSawValuable.States.Active)
			{
				this.StateActive();
				return;
			}
		}
		else
		{
			this.StateIdle();
		}
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x000BA15C File Offset: 0x000B835C
	protected override void Update()
	{
		base.Update();
		if (this.trapStart)
		{
			this.TrapActivate();
		}
		this.soundBladeLoop.PlayLoop(this.loopPlaying, 5f, 5f, 1f);
		this.blade.Rotate(-Vector3.right * this.bladeSpeed * Time.deltaTime);
		this.bladeSpeed = Mathf.Lerp(0f, this.bladeMaxSpeed, this.bladeCurve.Evaluate(this.bladeLerp));
		float num = 0.3f * this.bladeCurve.Evaluate(this.bladeLerp);
		float num2 = 60f * this.bladeCurve.Evaluate(this.bladeLerp);
		float num3 = num * Mathf.Sin(Time.time * num2);
		float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
		this.meshTransform.transform.localRotation = this.initialTankRotation * Quaternion.Euler(num3, 0f, z);
		this.meshTransform.transform.localPosition = new Vector3(this.meshTransform.transform.localPosition.x, this.meshTransform.transform.localPosition.y - num3 * 0.005f * Time.deltaTime, this.meshTransform.transform.localPosition.z);
		if (this.currentState == IceSawValuable.States.Active)
		{
			this.enemyInvestigate = true;
			this.enemyInvestigateRange = 15f;
			if (this.bladeLerp < 1f)
			{
				this.bladeLerp += Time.deltaTime / this.secondsToStart;
				return;
			}
			this.bladeLerp = 1f;
			return;
		}
		else
		{
			if (this.bladeLerp > 0f)
			{
				this.bladeLerp -= Time.deltaTime / this.secondsToStop;
				return;
			}
			this.bladeLerp = 0f;
			return;
		}
	}

	// Token: 0x06001515 RID: 5397 RVA: 0x000BA34C File Offset: 0x000B854C
	private void StateActive()
	{
		if (this.stateStart)
		{
			this.hurtCollider.gameObject.SetActive(true);
			this.soundBladeStart.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.loopPlaying = true;
			this.stateStart = false;
		}
		this.overLapBoxCheckTimer += Time.deltaTime;
		if (this.overLapBoxCheckTimer >= 0.1f)
		{
			Vector3 a = this.triggerCollider.bounds.size * 0.5f;
			a.x *= Mathf.Abs(base.transform.lossyScale.x);
			a.y *= Mathf.Abs(base.transform.lossyScale.y);
			a.z *= Mathf.Abs(base.transform.lossyScale.z);
			if (Physics.OverlapBox(this.triggerCollider.bounds.center, a / 2f, this.triggerCollider.transform.rotation, LayerMask.GetMask(new string[]
			{
				"Default"
			}), QueryTriggerInteraction.Collide).Length != 0)
			{
				this.Sparks();
			}
			this.overLapBoxCheckTimer = 0f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.rb.AddTorque(base.transform.up * 1f * Time.fixedDeltaTime * 30f, ForceMode.Force);
			if (Random.Range(0, 100) < 7)
			{
				this.rb.AddForce(Random.insideUnitSphere * 0.5f, ForceMode.Impulse);
				this.rb.AddTorque(Random.insideUnitSphere * 0.1f, ForceMode.Impulse);
			}
			if (!this.physgrabobject.grabbed && !this.trapActive)
			{
				this.SetState(IceSawValuable.States.Idle);
			}
		}
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x000BA544 File Offset: 0x000B8744
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.hurtCollider.gameObject.SetActive(false);
			this.loopPlaying = false;
			this.soundBladeEnd.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.stateStart = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && (this.physgrabobject.grabbed || this.trapActive))
		{
			this.SetState(IceSawValuable.States.Active);
		}
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x000BA5C6 File Offset: 0x000B87C6
	[PunRPC]
	public void SetStateRPC(IceSawValuable.States state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x06001518 RID: 5400 RVA: 0x000BA5E0 File Offset: 0x000B87E0
	private void SetState(IceSawValuable.States state)
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

	// Token: 0x06001519 RID: 5401 RVA: 0x000BA62D File Offset: 0x000B882D
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.GrabRelease();
			this.sawTimer.Invoke();
			this.trapActive = true;
			this.trapTriggered = true;
		}
	}

	// Token: 0x0600151A RID: 5402 RVA: 0x000BA656 File Offset: 0x000B8856
	public void TrapStop()
	{
		this.trapActive = false;
	}

	// Token: 0x0600151B RID: 5403 RVA: 0x000BA65F File Offset: 0x000B885F
	public void Sparks()
	{
		this.sparkParticles.Play();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.rb.AddForce(base.transform.right * 2f, ForceMode.Impulse);
		}
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x000BA694 File Offset: 0x000B8894
	public void ImpactDamage()
	{
		this.physGrabObject.lightBreakImpulse = true;
	}

	// Token: 0x0600151D RID: 5405 RVA: 0x000BA6A4 File Offset: 0x000B88A4
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

	// Token: 0x0600151E RID: 5406 RVA: 0x000BA774 File Offset: 0x000B8974
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

	// Token: 0x04002462 RID: 9314
	public Sound soundBladeLoop;

	// Token: 0x04002463 RID: 9315
	public Sound soundBladeStart;

	// Token: 0x04002464 RID: 9316
	public Sound soundBladeEnd;

	// Token: 0x04002465 RID: 9317
	[Space]
	public Transform meshTransform;

	// Token: 0x04002466 RID: 9318
	public UnityEvent sawTimer;

	// Token: 0x04002467 RID: 9319
	public Transform blade;

	// Token: 0x04002468 RID: 9320
	public AnimationCurve bladeCurve;

	// Token: 0x04002469 RID: 9321
	public float bladeSpeed;

	// Token: 0x0400246A RID: 9322
	private float bladeMaxSpeed = 1500f;

	// Token: 0x0400246B RID: 9323
	private float bladeLerp;

	// Token: 0x0400246C RID: 9324
	private float secondsToStart = 2f;

	// Token: 0x0400246D RID: 9325
	private float secondsToStop = 2f;

	// Token: 0x0400246E RID: 9326
	public HurtCollider hurtCollider;

	// Token: 0x0400246F RID: 9327
	public Collider triggerCollider;

	// Token: 0x04002470 RID: 9328
	private float overLapBoxCheckTimer;

	// Token: 0x04002471 RID: 9329
	public ParticleSystem sparkParticles;

	// Token: 0x04002472 RID: 9330
	internal IceSawValuable.States currentState;

	// Token: 0x04002473 RID: 9331
	private bool stateStart;

	// Token: 0x04002474 RID: 9332
	[Space]
	private Quaternion initialTankRotation;

	// Token: 0x04002475 RID: 9333
	private Animator animator;

	// Token: 0x04002476 RID: 9334
	private PhysGrabObject physgrabobject;

	// Token: 0x04002477 RID: 9335
	private Rigidbody rb;

	// Token: 0x04002478 RID: 9336
	private bool loopPlaying;

	// Token: 0x02000412 RID: 1042
	public enum States
	{
		// Token: 0x04002DAF RID: 11695
		Idle,
		// Token: 0x04002DB0 RID: 11696
		Active
	}
}
