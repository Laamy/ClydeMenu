using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002AA RID: 682
public class FrogTrap : Trap
{
	// Token: 0x06001552 RID: 5458 RVA: 0x000BBF7C File Offset: 0x000BA17C
	protected override void Start()
	{
		base.Start();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.initialFrogRotation = this.Frog.transform.localRotation;
		this.rb = base.GetComponent<Rigidbody>();
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x000BBFCC File Offset: 0x000BA1CC
	protected override void Update()
	{
		base.Update();
		this.CrankLoop.PlayLoop(this.LoopPlaying, 0.8f, 0.8f, 1f);
		if (this.physGrabObject.grabbed)
		{
			if (!this.grabbedPrev)
			{
				this.Jump.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					Quaternion turnX = Quaternion.Euler(45f, 180f, 0f);
					Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
					Quaternion identity = Quaternion.identity;
					this.physGrabObject.TurnXYZ(turnX, turnY, identity);
				}
				this.grabbedPrev = true;
				if (this.physGrabObject.grabbedLocal)
				{
					PhysGrabber.instance.OverrideGrabDistance(0.8f);
				}
			}
			this.everPickedUp = true;
			this.LoopPlaying = false;
			if (this.trapActive)
			{
				this.TrapStop();
			}
		}
		else
		{
			this.grabbedPrev = false;
			if (this.everPickedUp)
			{
				this.trapStart = true;
			}
		}
		if (this.trapStart && !this.impactDetector.inCart)
		{
			this.TrapActivate();
		}
		if (this.trapActive && !this.physGrabObject.grabbed)
		{
			this.enemyInvestigate = true;
			this.LoopPlaying = true;
			if (this.FrogJumpActive)
			{
				this.FrogJumpLerp += this.FrogJumpSpeed * Time.deltaTime;
				if (this.FrogJumpLerp >= 1f)
				{
					this.FrogJumpLerp = 0f;
					this.FrogJumpActive = false;
				}
			}
			this.FrogFeet.transform.localEulerAngles = new Vector3(0f, 0f, this.FrogJumpCurve.Evaluate(this.FrogJumpLerp) * this.FrogJumpIntensity);
			this.FrogCrank.transform.Rotate(0f, 0f, 80f * Time.deltaTime);
			float num = 1f;
			float num2 = 40f;
			float num3 = num * Mathf.Sin(Time.time * num2);
			float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
			this.Frog.transform.localRotation = this.initialFrogRotation * Quaternion.Euler(num3, 0f, z);
			this.Frog.transform.localPosition = new Vector3(this.Frog.transform.localPosition.x, this.Frog.transform.localPosition.y - num3 * 0.005f * Time.deltaTime, this.Frog.transform.localPosition.z);
			if (this.frogJumpTimer > 0f)
			{
				this.frogJumpTimer -= Time.deltaTime;
				return;
			}
			if (SemiFunc.IsMultiplayer())
			{
				if (SemiFunc.IsMasterClient())
				{
					this.photonView.RPC("FrogJumpRPC", RpcTarget.All, Array.Empty<object>());
					return;
				}
			}
			else
			{
				this.FrogJump();
			}
		}
	}

	// Token: 0x06001554 RID: 5460 RVA: 0x000BC2C8 File Offset: 0x000BA4C8
	public void FrogJump()
	{
		if (this.impactDetector.inCart)
		{
			this.TrapStop();
			return;
		}
		this.frogJumpTimer = Random.Range(0.5f, 0.8f);
		this.enemyInvestigate = true;
		this.Jump.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
		this.FrogJumpActive = true;
		this.FrogJumpLerp = 0f;
		this.enemyInvestigateRange = 15f;
		if (this.isLocal)
		{
			if (Vector3.Dot(this.Frog.transform.up, Vector3.up) > 0.5f)
			{
				this.rb.AddForce(Vector3.up * 1f, ForceMode.Impulse);
				this.rb.AddForce(base.transform.forward * 1.5f, ForceMode.Impulse);
				Vector3 a = Random.insideUnitSphere.normalized * Random.Range(0.05f, 0.1f);
				a.z = 0f;
				a.x = 0f;
				this.rb.AddTorque(a * 0.25f, ForceMode.Impulse);
				return;
			}
			this.rb.AddForce(Vector3.up * 1f, ForceMode.Impulse);
			Vector3 normalized = Random.insideUnitSphere.normalized;
			this.rb.AddTorque(normalized * 0.03f, ForceMode.Impulse);
		}
	}

	// Token: 0x06001555 RID: 5461 RVA: 0x000BC44A File Offset: 0x000BA64A
	[PunRPC]
	public void FrogJumpRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.FrogJump();
	}

	// Token: 0x06001556 RID: 5462 RVA: 0x000BC45C File Offset: 0x000BA65C
	public void TrapStop()
	{
		this.trapActive = false;
		this.trapStart = false;
		this.LoopPlaying = false;
		this.trapTriggered = false;
		this.CrankEnd.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001557 RID: 5463 RVA: 0x000BC4B0 File Offset: 0x000BA6B0
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.CrankStart.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.trapActive = true;
			this.trapTriggered = true;
			this.frogJumpTimer = 0f;
		}
	}

	// Token: 0x040024D8 RID: 9432
	private PhysGrabObject physgrabobject;

	// Token: 0x040024D9 RID: 9433
	[Space]
	[Header("Frog Components")]
	public GameObject Frog;

	// Token: 0x040024DA RID: 9434
	public GameObject FrogFeet;

	// Token: 0x040024DB RID: 9435
	public GameObject FrogCrank;

	// Token: 0x040024DC RID: 9436
	[Space]
	[Header("Sounds")]
	public Sound CrankStart;

	// Token: 0x040024DD RID: 9437
	public Sound CrankEnd;

	// Token: 0x040024DE RID: 9438
	public Sound CrankLoop;

	// Token: 0x040024DF RID: 9439
	public Sound Jump;

	// Token: 0x040024E0 RID: 9440
	[Space]
	public AnimationCurve FrogJumpCurve;

	// Token: 0x040024E1 RID: 9441
	private float FrogJumpLerp;

	// Token: 0x040024E2 RID: 9442
	public float FrogJumpSpeed;

	// Token: 0x040024E3 RID: 9443
	private bool FrogJumpActive;

	// Token: 0x040024E4 RID: 9444
	public float FrogJumpIntensity;

	// Token: 0x040024E5 RID: 9445
	private Quaternion initialFrogRotation;

	// Token: 0x040024E6 RID: 9446
	private Rigidbody rb;

	// Token: 0x040024E7 RID: 9447
	private bool LoopPlaying;

	// Token: 0x040024E8 RID: 9448
	private bool everPickedUp;

	// Token: 0x040024E9 RID: 9449
	private float frogJumpTimer;

	// Token: 0x040024EA RID: 9450
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x040024EB RID: 9451
	private bool grabbedPrev;
}
