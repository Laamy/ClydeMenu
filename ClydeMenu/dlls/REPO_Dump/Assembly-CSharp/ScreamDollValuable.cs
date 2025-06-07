using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002BA RID: 698
public class ScreamDollValuable : MonoBehaviour
{
	// Token: 0x060015FC RID: 5628 RVA: 0x000C1608 File Offset: 0x000BF808
	private void StateActive()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.loopPlaying = true;
		this.animator.SetBool("active", true);
		this.animator.enabled = true;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (Random.Range(0, 100) < 7)
			{
				this.rb.AddForce(Random.insideUnitSphere * 3f, ForceMode.Impulse);
				this.rb.AddTorque(Random.insideUnitSphere * 7f, ForceMode.Impulse);
			}
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
			if (!this.physGrabObject.grabbed)
			{
				this.SetState(ScreamDollValuable.States.Idle);
			}
		}
		if (this.physGrabObject.grabbedLocal)
		{
			PhysGrabber.instance.OverridePullDistanceIncrement(-1f * Time.fixedDeltaTime);
		}
	}

	// Token: 0x060015FD RID: 5629 RVA: 0x000C175C File Offset: 0x000BF95C
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.loopPlaying = false;
		this.animator.SetBool("active", false);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.physGrabObject.grabbed)
		{
			this.SetState(ScreamDollValuable.States.Active);
		}
	}

	// Token: 0x060015FE RID: 5630 RVA: 0x000C17AB File Offset: 0x000BF9AB
	[PunRPC]
	public void SetStateRPC(ScreamDollValuable.States state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x060015FF RID: 5631 RVA: 0x000C17C4 File Offset: 0x000BF9C4
	private void SetState(ScreamDollValuable.States state)
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

	// Token: 0x06001600 RID: 5632 RVA: 0x000C1811 File Offset: 0x000BFA11
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
		this.rb = base.GetComponent<Rigidbody>();
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x000C1843 File Offset: 0x000BFA43
	private void Update()
	{
		this.soundScreamLoop.PlayLoop(this.loopPlaying, 5f, 5f, 1f);
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x000C1868 File Offset: 0x000BFA68
	private void FixedUpdate()
	{
		ScreamDollValuable.States states = this.currentState;
		if (states != ScreamDollValuable.States.Idle)
		{
			if (states == ScreamDollValuable.States.Active)
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

	// Token: 0x06001603 RID: 5635 RVA: 0x000C1890 File Offset: 0x000BFA90
	public void EnemyInvestigate()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			EnemyDirector.instance.SetInvestigate(base.transform.position, 20f, false);
		}
	}

	// Token: 0x06001604 RID: 5636 RVA: 0x000C18BB File Offset: 0x000BFABB
	public void StopAnimator()
	{
		this.animator.enabled = false;
	}

	// Token: 0x06001605 RID: 5637 RVA: 0x000C18C9 File Offset: 0x000BFAC9
	public void OnHurtColliderHitEnemy()
	{
		this.physGrabObject.heavyBreakImpulse = true;
	}

	// Token: 0x0400262A RID: 9770
	private Animator animator;

	// Token: 0x0400262B RID: 9771
	private PhysGrabObject physGrabObject;

	// Token: 0x0400262C RID: 9772
	private Rigidbody rb;

	// Token: 0x0400262D RID: 9773
	public Sound soundScreamLoop;

	// Token: 0x0400262E RID: 9774
	private PhotonView photonView;

	// Token: 0x0400262F RID: 9775
	internal ScreamDollValuable.States currentState;

	// Token: 0x04002630 RID: 9776
	private bool stateStart;

	// Token: 0x04002631 RID: 9777
	private bool loopPlaying;

	// Token: 0x02000419 RID: 1049
	public enum States
	{
		// Token: 0x04002DD4 RID: 11732
		Idle,
		// Token: 0x04002DD5 RID: 11733
		Active
	}
}
