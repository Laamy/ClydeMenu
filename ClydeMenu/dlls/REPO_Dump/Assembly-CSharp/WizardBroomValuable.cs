using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002C1 RID: 705
public class WizardBroomValuable : Trap
{
	// Token: 0x06001621 RID: 5665 RVA: 0x000C20EC File Offset: 0x000C02EC
	private bool CheckForwardRaycast(out RaycastHit _hit)
	{
		_hit = default(RaycastHit);
		if (this.raycastTimer < this.raycastCooldown)
		{
			return false;
		}
		this.raycastTimer = 0f;
		Vector3 vector = this.broom.transform.TransformPoint(this.rayOffset);
		Vector3 vector2 = -this.broom.transform.right;
		Debug.DrawRay(vector, vector2, Color.red);
		return Physics.Raycast(vector, vector2, out _hit, this.rayDistance, this.visionObstruct);
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x000C216B File Offset: 0x000C036B
	protected override void Start()
	{
		base.Start();
		this.rb = base.GetComponent<Rigidbody>();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.visionObstruct = SemiFunc.LayerMaskGetVisionObstruct();
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x000C2196 File Offset: 0x000C0396
	protected override void Update()
	{
		base.Update();
	}

	// Token: 0x06001624 RID: 5668 RVA: 0x000C21A0 File Offset: 0x000C03A0
	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			switch (this.currentState)
			{
			case WizardBroomValuable.States.Idle:
				this.StateIdle();
				break;
			case WizardBroomValuable.States.MoveForward:
				this.StateMoveForward();
				break;
			case WizardBroomValuable.States.Turn:
				this.StateTurn();
				break;
			case WizardBroomValuable.States.Unstick:
				this.StateUnstick();
				break;
			case WizardBroomValuable.States.grabbed:
				this.StateGrabbed();
				break;
			case WizardBroomValuable.States.Sleep:
				this.StateSleep();
				break;
			}
			if (this.stuckTimer > 0f)
			{
				this.stuckTimer -= Time.fixedDeltaTime;
			}
			if (this.physGrabObject.grabbed && this.trapActive)
			{
				this.SetState(WizardBroomValuable.States.grabbed);
			}
			if (this.impactDetector.inCart && this.trapTriggered)
			{
				this.TrapStop();
			}
		}
	}

	// Token: 0x06001625 RID: 5669 RVA: 0x000C2264 File Offset: 0x000C0464
	private void StateMoveForward()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.raycastTimer += Time.fixedDeltaTime;
		this.physGrabObject.OverrideZeroGravity(0.1f);
		if (this.stuckTimer <= 0f)
		{
			if (this.rb.velocity.magnitude > 0.1f)
			{
				this.stuckTimer = Random.Range(0.1f, 2f);
				return;
			}
			this.SetState(WizardBroomValuable.States.Unstick);
		}
		RaycastHit raycastHit;
		if (this.CheckForwardRaycast(out raycastHit) || Vector3.Dot(this.broom.transform.right, Vector3.up) > 0.1f)
		{
			this.SetState(WizardBroomValuable.States.Turn);
			return;
		}
		this.rb.AddForce(-this.broom.transform.right * 2000f * Time.fixedDeltaTime, ForceMode.Force);
	}

	// Token: 0x06001626 RID: 5670 RVA: 0x000C2350 File Offset: 0x000C0550
	private void StateTurn()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.rb.AddForce(Vector3.up * 50f * Time.fixedDeltaTime, ForceMode.Impulse);
		}
		this.raycastTimer += Time.fixedDeltaTime;
		this.physGrabObject.OverrideZeroGravity(0.1f);
		RaycastHit raycastHit;
		if (this.CheckForwardRaycast(out raycastHit))
		{
			Vector3 a = Vector3.Cross(raycastHit.normal, this.broom.transform.right);
			this.rb.AddTorque(a * 200f * Time.fixedDeltaTime, ForceMode.Force);
			this.rb.AddForce(this.broom.transform.right * 500f * Time.fixedDeltaTime, ForceMode.Force);
			return;
		}
		this.SetState(WizardBroomValuable.States.MoveForward);
	}

	// Token: 0x06001627 RID: 5671 RVA: 0x000C2434 File Offset: 0x000C0634
	private void StateUnstick()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.unStickTimer = Random.Range(1f, 2f);
			this.randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
			this.rb.AddForce(this.randomDirection * 500f * Time.fixedDeltaTime, ForceMode.Impulse);
			this.randomTorque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
		}
		if (this.unStickTimer > 0f)
		{
			this.rb.AddTorque(this.randomTorque * 5000f * Time.fixedDeltaTime, ForceMode.Force);
			this.unStickTimer -= Time.fixedDeltaTime;
			return;
		}
		this.stuckTimer = Random.Range(0.1f, 2f);
		this.SetState(WizardBroomValuable.States.MoveForward);
	}

	// Token: 0x06001628 RID: 5672 RVA: 0x000C2570 File Offset: 0x000C0770
	private void StateIdle()
	{
		this.stateStart = false;
	}

	// Token: 0x06001629 RID: 5673 RVA: 0x000C257C File Offset: 0x000C077C
	private void StateGrabbed()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.physGrabObject.OverrideZeroGravity(0.1f);
		this.physGrabObject.OverrideDrag(0.2f, 0.1f);
		this.physGrabObject.OverrideAngularDrag(0.2f, 0.1f);
		if (!this.physGrabObject.grabbed)
		{
			this.SetState(WizardBroomValuable.States.MoveForward);
		}
	}

	// Token: 0x0600162A RID: 5674 RVA: 0x000C25E6 File Offset: 0x000C07E6
	private void StateSleep()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		if (!this.impactDetector.inCart && this.trapTriggered)
		{
			this.trapActive = true;
			this.SetState(WizardBroomValuable.States.MoveForward);
		}
	}

	// Token: 0x0600162B RID: 5675 RVA: 0x000C261C File Offset: 0x000C081C
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.GrabRelease();
			this.broomBoxBreakSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			this.plankParticles.Play();
			this.bitParticles.Play();
			this.box.SetActive(false);
			this.broom.SetActive(true);
			this.trapActive = true;
			this.trapTriggered = true;
			this.SetState(WizardBroomValuable.States.MoveForward);
		}
	}

	// Token: 0x0600162C RID: 5676 RVA: 0x000C26A5 File Offset: 0x000C08A5
	public void TrapStop()
	{
		this.trapActive = false;
		this.SetState(WizardBroomValuable.States.Sleep);
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x000C26B8 File Offset: 0x000C08B8
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

	// Token: 0x0600162E RID: 5678 RVA: 0x000C2788 File Offset: 0x000C0988
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

	// Token: 0x0600162F RID: 5679 RVA: 0x000C280A File Offset: 0x000C0A0A
	private void SetState(WizardBroomValuable.States state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x04002654 RID: 9812
	public UnityEvent broomTimer;

	// Token: 0x04002655 RID: 9813
	private Rigidbody rb;

	// Token: 0x04002656 RID: 9814
	private LayerMask visionObstruct;

	// Token: 0x04002657 RID: 9815
	private Vector3 rayOffset = new Vector3(-1.92f, 0f, 0f);

	// Token: 0x04002658 RID: 9816
	private float rayDistance = 1f;

	// Token: 0x04002659 RID: 9817
	private float raycastCooldown = 0.2f;

	// Token: 0x0400265A RID: 9818
	private float raycastTimer;

	// Token: 0x0400265B RID: 9819
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x0400265C RID: 9820
	public GameObject box;

	// Token: 0x0400265D RID: 9821
	public GameObject broom;

	// Token: 0x0400265E RID: 9822
	public ParticleSystem plankParticles;

	// Token: 0x0400265F RID: 9823
	public ParticleSystem bitParticles;

	// Token: 0x04002660 RID: 9824
	public Sound broomBoxBreakSound;

	// Token: 0x04002661 RID: 9825
	private float stuckTimer = 2f;

	// Token: 0x04002662 RID: 9826
	private float unStickTimer;

	// Token: 0x04002663 RID: 9827
	private Vector3 randomDirection;

	// Token: 0x04002664 RID: 9828
	private Vector3 randomTorque;

	// Token: 0x04002665 RID: 9829
	public WizardBroomValuable.States currentState;

	// Token: 0x04002666 RID: 9830
	private bool stateStart;

	// Token: 0x0200041B RID: 1051
	public enum States
	{
		// Token: 0x04002DDB RID: 11739
		Idle,
		// Token: 0x04002DDC RID: 11740
		MoveForward,
		// Token: 0x04002DDD RID: 11741
		Turn,
		// Token: 0x04002DDE RID: 11742
		Unstick,
		// Token: 0x04002DDF RID: 11743
		grabbed,
		// Token: 0x04002DE0 RID: 11744
		Sleep
	}
}
