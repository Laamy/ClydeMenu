using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200004F RID: 79
public class EnemyGnome : MonoBehaviour
{
	// Token: 0x0600027F RID: 639 RVA: 0x00019D3B File Offset: 0x00017F3B
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000280 RID: 640 RVA: 0x00019D4C File Offset: 0x00017F4C
	private void Start()
	{
		this.enemy.NavMeshAgent.DefaultSpeed = Random.Range(this.speedMin, this.speedMax);
		this.enemy.NavMeshAgent.Agent.speed = this.enemy.NavMeshAgent.DefaultSpeed;
	}

	// Token: 0x06000281 RID: 641 RVA: 0x00019DA0 File Offset: 0x00017FA0
	private void Update()
	{
		if (!EnemyGnomeDirector.instance || !EnemyGnomeDirector.instance.setup)
		{
			return;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.AvoidLogic();
			this.RotationLogic();
			this.BackAwayOffsetLogic();
			this.MoveOffsetLogic();
			this.TimerLogic();
			if (this.enemy.CurrentState == EnemyState.Despawn)
			{
				this.UpdateState(EnemyGnome.State.Despawn);
			}
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyGnome.State.Stun);
			}
			switch (this.currentState)
			{
			case EnemyGnome.State.Spawn:
				this.StateSpawn();
				return;
			case EnemyGnome.State.Idle:
				this.StateIdle();
				return;
			case EnemyGnome.State.NoticeDelay:
				this.StateNoticeDelay();
				return;
			case EnemyGnome.State.Notice:
				this.StateNotice();
				return;
			case EnemyGnome.State.Move:
				this.StateMove();
				return;
			case EnemyGnome.State.MoveUnder:
				this.StateMoveUnder();
				return;
			case EnemyGnome.State.MoveOver:
				this.StateMoveOver();
				return;
			case EnemyGnome.State.MoveBack:
				this.StateMoveBack();
				return;
			case EnemyGnome.State.AttackMove:
				this.StateAttackMove();
				return;
			case EnemyGnome.State.Attack:
				this.StateAttack();
				return;
			case EnemyGnome.State.AttackDone:
				this.StateAttackDone();
				return;
			case EnemyGnome.State.Stun:
				this.StateStun();
				return;
			case EnemyGnome.State.Despawn:
				this.StateDespawn();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06000282 RID: 642 RVA: 0x00019EB8 File Offset: 0x000180B8
	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.avoidForce != Vector3.zero)
		{
			this.enemy.Rigidbody.rb.AddForce(this.avoidForce * 2f, ForceMode.Force);
		}
	}

	// Token: 0x06000283 RID: 643 RVA: 0x00019F04 File Offset: 0x00018104
	private void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyGnome.State.Idle);
		}
	}

	// Token: 0x06000284 RID: 644 RVA: 0x00019F54 File Offset: 0x00018154
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
		}
		this.enemy.Rigidbody.DisableFollowPosition(0.1f, 0.5f);
		this.IdleBreakerLogic();
		if (EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackPlayer || EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackValuable)
		{
			this.UpdateState(EnemyGnome.State.NoticeDelay);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, EnemyGnomeDirector.instance.destinations[this.directorIndex]) > 2f)
		{
			this.UpdateState(EnemyGnome.State.Move);
		}
	}

	// Token: 0x06000285 RID: 645 RVA: 0x0001A024 File Offset: 0x00018224
	private void StateMove()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.enemy.NavMeshAgent.SetDestination(EnemyGnomeDirector.instance.destinations[this.directorIndex]);
		this.MoveBackPosition();
		this.MoveOffsetSet();
		SemiFunc.EnemyCartJump(this.enemy);
		if (EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackPlayer || EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackValuable)
		{
			this.UpdateState(EnemyGnome.State.NoticeDelay);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, EnemyGnomeDirector.instance.destinations[this.directorIndex]) <= 0.2f)
		{
			this.UpdateState(EnemyGnome.State.Idle);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, EnemyGnomeDirector.instance.destinations[this.directorIndex]) <= 2f)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyGnome.State.Idle);
			}
		}
	}

	// Token: 0x06000286 RID: 646 RVA: 0x0001A144 File Offset: 0x00018344
	private void StateNoticeDelay()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = Random.Range(0f, 1f);
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyGnome.State.Notice);
		}
	}

	// Token: 0x06000287 RID: 647 RVA: 0x0001A19C File Offset: 0x0001839C
	private void StateNotice()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
			this.stateTimer = 0.5f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyGnome.State.AttackMove);
		}
	}

	// Token: 0x06000288 RID: 648 RVA: 0x0001A220 File Offset: 0x00018420
	private void StateAttackMove()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		Vector3 destination = this.AttackPositionLogic();
		this.enemy.NavMeshAgent.SetDestination(destination);
		bool flag = EnemyGnomeDirector.instance.CanAttack(this);
		this.MoveBackPosition();
		this.MoveOffsetSet();
		SemiFunc.EnemyCartJump(this.enemy);
		if (EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackPlayer && EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackValuable)
		{
			this.UpdateState(EnemyGnome.State.Move);
			return;
		}
		if (flag)
		{
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyGnome.State.Attack);
			return;
		}
		if (!this.enemy.NavMeshAgent.CanReach(this.AttackVisionDynamic(), 1f) && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f)
		{
			if (this.AttackPositionLogic().y > this.enemy.Rigidbody.transform.position.y + 0.2f)
			{
				this.enemy.Jump.StuckTrigger(this.AttackVisionPosition() - this.enemy.Vision.VisionTransform.position);
			}
			NavMeshHit navMeshHit;
			if (!this.VisionBlocked() && !NavMesh.SamplePosition(this.AttackVisionDynamic(), out navMeshHit, 0.5f, -1))
			{
				if (Mathf.Abs(this.AttackVisionDynamic().y - this.enemy.Rigidbody.transform.position.y) < 0.2f)
				{
					this.UpdateState(EnemyGnome.State.MoveUnder);
					return;
				}
				if (this.AttackPositionLogic().y > this.enemy.Rigidbody.transform.position.y)
				{
					this.UpdateState(EnemyGnome.State.MoveOver);
				}
			}
		}
	}

	// Token: 0x06000289 RID: 649 RVA: 0x0001A3F8 File Offset: 0x000185F8
	private void StateMoveUnder()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		bool flag = EnemyGnomeDirector.instance.CanAttack(this);
		Vector3 vector = this.AttackPositionLogic();
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, this.enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		this.MoveOffsetSet();
		SemiFunc.EnemyCartJump(this.enemy);
		if (EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackPlayer && EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackValuable)
		{
			this.UpdateState(EnemyGnome.State.MoveBack);
			return;
		}
		if (flag)
		{
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyGnome.State.Attack);
			return;
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(vector, out navMeshHit, 0.5f, -1))
		{
			this.UpdateState(EnemyGnome.State.MoveBack);
			return;
		}
		if (this.VisionBlocked())
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyGnome.State.MoveBack);
				return;
			}
		}
		else
		{
			EnemyGnomeDirector.instance.SeeTarget();
			this.stateTimer = 2f;
		}
	}

	// Token: 0x0600028A RID: 650 RVA: 0x0001A520 File Offset: 0x00018720
	private void StateMoveOver()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		bool flag = EnemyGnomeDirector.instance.CanAttack(this);
		Vector3 vector = this.AttackPositionLogic();
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, this.enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		this.MoveOffsetSet();
		SemiFunc.EnemyCartJump(this.enemy);
		if (this.AttackVisionDynamic().y > this.enemy.Rigidbody.transform.position.y + 0.2f && !flag)
		{
			this.enemy.Jump.StuckTrigger(this.AttackVisionDynamic() - this.enemy.Rigidbody.transform.position);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.AttackVisionDynamic(), 2f);
		}
		if (EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackPlayer && EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackValuable)
		{
			this.UpdateState(EnemyGnome.State.MoveBack);
			return;
		}
		if (flag)
		{
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyGnome.State.Attack);
			return;
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(vector, out navMeshHit, 0.5f, -1))
		{
			this.UpdateState(EnemyGnome.State.MoveBack);
			return;
		}
		if (this.VisionBlocked())
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyGnome.State.MoveBack);
				return;
			}
		}
		else
		{
			EnemyGnomeDirector.instance.SeeTarget();
			this.stateTimer = 2f;
		}
	}

	// Token: 0x0600028B RID: 651 RVA: 0x0001A6D0 File Offset: 0x000188D0
	private void StateMoveBack()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		if (!this.enemy.Jump.jumping)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.moveBackPosition, this.enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		}
		this.MoveOffsetSet();
		SemiFunc.EnemyCartJump(this.enemy);
		this.stateTimer -= Time.deltaTime;
		bool flag = EnemyGnomeDirector.instance.CanAttack(this);
		if (this.stateTimer <= 0f && (Vector3.Distance(base.transform.position, this.enemy.Rigidbody.transform.position) > 2f || this.enemy.Rigidbody.notMovingTimer > 2f) && !this.enemy.Jump.jumping)
		{
			Vector3 normalized = (base.transform.position - this.moveBackPosition).normalized;
			this.enemy.Jump.StuckTrigger(base.transform.position - this.moveBackPosition);
			base.transform.position = this.enemy.Rigidbody.transform.position;
			base.transform.position += normalized * 2f;
		}
		if (flag)
		{
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyGnome.State.Attack);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.moveBackPosition) <= 0.2f)
		{
			this.UpdateState(EnemyGnome.State.AttackMove);
			return;
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit, 0.5f, -1))
		{
			this.UpdateState(EnemyGnome.State.AttackMove);
		}
	}

	// Token: 0x0600028C RID: 652 RVA: 0x0001A8E4 File Offset: 0x00018AE4
	private void StateAttack()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateTimer = 3f;
			this.stateImpulse = false;
		}
		if (this.stateTimer > 0.5f && !this.enemyGnomeAnim.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
		{
			this.UpdateState(EnemyGnome.State.AttackMove);
			return;
		}
		this.enemy.StuckCount = 0;
		this.enemy.Rigidbody.DisableFollowPosition(0.1f, 1f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyGnome.State.AttackDone);
		}
	}

	// Token: 0x0600028D RID: 653 RVA: 0x0001A9C4 File Offset: 0x00018BC4
	private void StateAttackDone()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		this.enemy.StuckCount = 0;
		this.enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.moveBackTimer = 2f;
			this.attackCooldown = 2f;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit, 0.5f, -1))
			{
				this.UpdateState(EnemyGnome.State.AttackMove);
				return;
			}
			this.UpdateState(this.attackMoveState);
		}
	}

	// Token: 0x0600028E RID: 654 RVA: 0x0001AA7E File Offset: 0x00018C7E
	private void StateStun()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyGnome.State.Idle);
		}
	}

	// Token: 0x0600028F RID: 655 RVA: 0x0001AAA3 File Offset: 0x00018CA3
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
	}

	// Token: 0x06000290 RID: 656 RVA: 0x0001AAB4 File Offset: 0x00018CB4
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			EnemyGnomeDirector.instance.OnSpawn();
			this.UpdateState(EnemyGnome.State.Spawn);
		}
	}

	// Token: 0x06000291 RID: 657 RVA: 0x0001AADB File Offset: 0x00018CDB
	public void OnHurt()
	{
		this.soundHurt.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000292 RID: 658 RVA: 0x0001AB10 File Offset: 0x00018D10
	public void OnDeath()
	{
		this.soundDeath.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.05f);
		ParticleSystem[] array = this.deathEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x06000293 RID: 659 RVA: 0x0001ABE7 File Offset: 0x00018DE7
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			EnemyGnomeDirector.instance.Investigate(this.enemy.StateInvestigate.onInvestigateTriggeredPosition);
		}
	}

	// Token: 0x06000294 RID: 660 RVA: 0x0001AC0A File Offset: 0x00018E0A
	public void OnVision()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			EnemyGnomeDirector.instance.SetTarget(this.enemy.Vision.onVisionTriggeredPlayer);
		}
	}

	// Token: 0x06000295 RID: 661 RVA: 0x0001AC2D File Offset: 0x00018E2D
	public void OnImpactLight()
	{
		if (!this.enemy.IsStunned())
		{
			return;
		}
		this.soundImpactLight.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000296 RID: 662 RVA: 0x0001AC6D File Offset: 0x00018E6D
	public void OnImpactMedium()
	{
		if (!this.enemy.IsStunned())
		{
			return;
		}
		this.soundImpactMedium.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000297 RID: 663 RVA: 0x0001ACAD File Offset: 0x00018EAD
	public void OnImpactHeavy()
	{
		if (!this.enemy.IsStunned())
		{
			return;
		}
		this.soundImpactHeavy.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000298 RID: 664 RVA: 0x0001ACF0 File Offset: 0x00018EF0
	public void UpdateState(EnemyGnome.State _state)
	{
		if (this.currentState == _state)
		{
			return;
		}
		if (_state == EnemyGnome.State.Attack)
		{
			this.attackMoveState = this.currentState;
		}
		this.currentState = _state;
		this.stateImpulse = true;
		this.stateTimer = 0f;
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateStateRPC", RpcTarget.All, new object[]
			{
				this.currentState
			});
			return;
		}
		this.UpdateStateRPC(this.currentState, default(PhotonMessageInfo));
	}

	// Token: 0x06000299 RID: 665 RVA: 0x0001AD74 File Offset: 0x00018F74
	private void RotationLogic()
	{
		if (this.currentState == EnemyGnome.State.Move || this.currentState == EnemyGnome.State.Notice || this.currentState == EnemyGnome.State.AttackMove || this.currentState == EnemyGnome.State.MoveUnder || this.currentState == EnemyGnome.State.MoveOver || this.currentState == EnemyGnome.State.MoveBack || this.currentState == EnemyGnome.State.Attack)
		{
			if (this.currentState == EnemyGnome.State.Notice || ((this.currentState == EnemyGnome.State.AttackMove || this.currentState == EnemyGnome.State.MoveUnder || this.currentState == EnemyGnome.State.MoveOver || this.currentState == EnemyGnome.State.Attack) && Vector3.Distance(this.enemy.Rigidbody.transform.position, EnemyGnomeDirector.instance.attackPosition) < 5f))
			{
				Quaternion rotation = this.rotationTransform.rotation;
				this.rotationTransform.rotation = Quaternion.LookRotation(this.AttackVisionPosition() - this.enemy.Rigidbody.transform.position);
				this.rotationTransform.eulerAngles = new Vector3(0f, this.rotationTransform.eulerAngles.y, 0f);
				Quaternion rotation2 = this.rotationTransform.rotation;
				this.rotationTransform.rotation = rotation;
				this.rotationTarget = rotation2;
			}
			else if (this.enemy.Rigidbody.rb.velocity.magnitude > 0.1f)
			{
				Vector3 position = this.rotationTransform.position;
				Quaternion rotation3 = this.rotationTransform.rotation;
				this.rotationTransform.position = this.enemy.Rigidbody.transform.position;
				this.rotationTransform.rotation = Quaternion.LookRotation(this.enemy.Rigidbody.rb.velocity.normalized);
				this.rotationTransform.eulerAngles = new Vector3(0f, this.rotationTransform.eulerAngles.y, 0f);
				Quaternion rotation4 = this.rotationTransform.rotation;
				this.rotationTransform.position = position;
				this.rotationTransform.rotation = rotation3;
				this.rotationTarget = rotation4;
			}
		}
		else if (this.currentState == EnemyGnome.State.Idle && Vector3.Distance(EnemyGnomeDirector.instance.transform.position, this.enemy.Rigidbody.transform.position) > 0.1f)
		{
			Quaternion rotation5 = this.rotationTransform.rotation;
			this.rotationTransform.rotation = Quaternion.LookRotation(EnemyGnomeDirector.instance.transform.position - this.enemy.Rigidbody.transform.position);
			this.rotationTransform.eulerAngles = new Vector3(0f, this.rotationTransform.eulerAngles.y, 0f);
			Quaternion rotation6 = this.rotationTransform.rotation;
			this.rotationTransform.rotation = rotation5;
			this.rotationTarget = rotation6;
		}
		this.rotationTransform.rotation = SemiFunc.SpringQuaternionGet(this.rotationSpring, this.rotationTarget, -1f);
	}

	// Token: 0x0600029A RID: 666 RVA: 0x0001B08C File Offset: 0x0001928C
	private void AvoidLogic()
	{
		if (this.currentState == EnemyGnome.State.Move || this.currentState == EnemyGnome.State.AttackMove || this.currentState == EnemyGnome.State.MoveUnder || this.currentState == EnemyGnome.State.MoveOver)
		{
			if (this.avoidTimer > 0f)
			{
				this.avoidTimer -= Time.deltaTime;
				return;
			}
			this.avoidForce = Vector3.zero;
			this.avoidTimer = 0.25f;
			if (!this.enemy.Jump.jumping)
			{
				Collider[] array = Physics.OverlapBox(this.avoidCollider.transform.position, this.avoidCollider.size / 2f, this.avoidCollider.transform.rotation, LayerMask.GetMask(new string[]
				{
					"PhysGrabObject"
				}));
				for (int i = 0; i < array.Length; i++)
				{
					EnemyRigidbody componentInParent = array[i].GetComponentInParent<EnemyRigidbody>();
					if (componentInParent)
					{
						EnemyGnome component = componentInParent.enemy.GetComponent<EnemyGnome>();
						if (component)
						{
							Vector3 normalized = (base.transform.position - component.transform.position).normalized;
							this.avoidForce += normalized.normalized;
						}
					}
				}
				return;
			}
		}
		else
		{
			this.avoidForce = Vector3.zero;
		}
	}

	// Token: 0x0600029B RID: 667 RVA: 0x0001B1DC File Offset: 0x000193DC
	private Vector3 AttackPositionLogic()
	{
		Vector3 result = EnemyGnomeDirector.instance.attackPosition + new Vector3(Mathf.Cos(this.attackAngle), 0f, Mathf.Sin(this.attackAngle)) * 0.7f;
		this.attackAngle += Time.deltaTime * 1f;
		return result;
	}

	// Token: 0x0600029C RID: 668 RVA: 0x0001B23A File Offset: 0x0001943A
	private Vector3 AttackVisionPosition()
	{
		return EnemyGnomeDirector.instance.attackVisionPosition;
	}

	// Token: 0x0600029D RID: 669 RVA: 0x0001B246 File Offset: 0x00019446
	private Vector3 AttackVisionDynamic()
	{
		if (EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackPlayer)
		{
			return this.AttackPositionLogic();
		}
		return this.AttackVisionPosition();
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0001B262 File Offset: 0x00019462
	private void MoveBackPosition()
	{
		if (Vector3.Distance(base.transform.position, this.enemy.Rigidbody.transform.position) < 1f)
		{
			this.moveBackPosition = base.transform.position;
		}
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0001B2A4 File Offset: 0x000194A4
	private void BackAwayOffsetLogic()
	{
		if (this.moveBackTimer > 0f)
		{
			this.moveBackTimer -= Time.deltaTime;
			this.backAwayOffset.localPosition = Vector3.Lerp(this.backAwayOffset.localPosition, new Vector3(0f, 0f, -1f), Time.deltaTime * 10f);
			return;
		}
		this.backAwayOffset.localPosition = Vector3.Lerp(this.backAwayOffset.localPosition, Vector3.zero, Time.deltaTime * 10f);
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0001B338 File Offset: 0x00019538
	private void MoveOffsetLogic()
	{
		if (this.moveOffsetTimer > 0f)
		{
			this.moveOffsetTimer -= Time.deltaTime;
			if (this.enemy.Jump.jumping)
			{
				this.moveOffsetTimer = 0f;
			}
			if (this.moveOffsetTimer <= 0f)
			{
				this.moveOffsetPosition = Vector3.zero;
			}
			else
			{
				this.moveOffsetSetTimer -= Time.deltaTime;
				if (this.moveOffsetSetTimer <= 0f)
				{
					Vector3 vector = Random.insideUnitSphere * 0.5f;
					vector.y = 0f;
					this.moveOffsetPosition = vector;
					this.moveOffsetSetTimer = Random.Range(0.2f, 1f);
				}
			}
		}
		this.moveOffsetTransform.localPosition = Vector3.Lerp(this.moveOffsetTransform.localPosition, this.moveOffsetPosition, Time.deltaTime * 20f);
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x0001B421 File Offset: 0x00019621
	private void MoveOffsetSet()
	{
		this.moveOffsetTimer = 0.2f;
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x0001B430 File Offset: 0x00019630
	private bool VisionBlocked()
	{
		if (this.visionTimer <= 0f)
		{
			this.visionTimer = 0.1f;
			Vector3 direction = this.AttackVisionPosition() - this.enemy.Vision.VisionTransform.position;
			this.visionPrevious = Physics.Raycast(this.enemy.Vision.VisionTransform.position, direction, direction.magnitude, LayerMask.GetMask(new string[]
			{
				"Default"
			}));
		}
		return this.visionPrevious;
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x0001B4B8 File Offset: 0x000196B8
	private void TimerLogic()
	{
		this.visionTimer -= Time.deltaTime;
		this.attackCooldown -= Time.deltaTime;
		this.overlapCheckTimer -= Time.deltaTime;
		if (this.overlapCheckCooldown > 0f)
		{
			this.overlapCheckCooldown -= Time.deltaTime;
			if (this.overlapCheckCooldown <= 0f)
			{
				this.overlapCheckPrevious = false;
			}
		}
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x0001B530 File Offset: 0x00019730
	public void IdleBreakerLogic()
	{
		bool flag = false;
		foreach (EnemyGnome enemyGnome in EnemyGnomeDirector.instance.gnomes)
		{
			if (enemyGnome != this && Vector3.Distance(base.transform.position, enemyGnome.transform.position) < 2f)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (this.idleBreakerTimer <= 0f)
			{
				this.idleBreakerTimer = Random.Range(2f, 15f);
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("IdleBreakerRPC", RpcTarget.All, Array.Empty<object>());
					return;
				}
				this.IdleBreakerRPC(default(PhotonMessageInfo));
				return;
			}
			else
			{
				this.idleBreakerTimer -= Time.deltaTime;
			}
		}
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x0001B618 File Offset: 0x00019818
	[PunRPC]
	private void UpdateStateRPC(EnemyGnome.State _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
		if (this.currentState == EnemyGnome.State.Spawn)
		{
			this.enemyGnomeAnim.OnSpawn();
		}
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x0001B63D File Offset: 0x0001983D
	[PunRPC]
	private void IdleBreakerRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.enemyGnomeAnim.idleBreakerImpulse = true;
	}

	// Token: 0x04000492 RID: 1170
	[Space]
	public EnemyGnome.State currentState;

	// Token: 0x04000493 RID: 1171
	private bool stateImpulse;

	// Token: 0x04000494 RID: 1172
	private float stateTimer;

	// Token: 0x04000495 RID: 1173
	private EnemyGnome.State attackMoveState;

	// Token: 0x04000496 RID: 1174
	[Space]
	public Enemy enemy;

	// Token: 0x04000497 RID: 1175
	public EnemyGnomeAnim enemyGnomeAnim;

	// Token: 0x04000498 RID: 1176
	private PhotonView photonView;

	// Token: 0x04000499 RID: 1177
	internal int directorIndex;

	// Token: 0x0400049A RID: 1178
	[Space]
	public SpringQuaternion rotationSpring;

	// Token: 0x0400049B RID: 1179
	private Quaternion rotationTarget;

	// Token: 0x0400049C RID: 1180
	[Space]
	public BoxCollider avoidCollider;

	// Token: 0x0400049D RID: 1181
	private float avoidTimer;

	// Token: 0x0400049E RID: 1182
	private Vector3 avoidForce;

	// Token: 0x0400049F RID: 1183
	[Space]
	public float speedMin = 1f;

	// Token: 0x040004A0 RID: 1184
	public float speedMax = 2f;

	// Token: 0x040004A1 RID: 1185
	[Space]
	public Transform backAwayOffset;

	// Token: 0x040004A2 RID: 1186
	public Transform moveOffsetTransform;

	// Token: 0x040004A3 RID: 1187
	public Transform rotationTransform;

	// Token: 0x040004A4 RID: 1188
	private float moveOffsetTimer;

	// Token: 0x040004A5 RID: 1189
	private float moveOffsetSetTimer;

	// Token: 0x040004A6 RID: 1190
	private Vector3 moveOffsetPosition;

	// Token: 0x040004A7 RID: 1191
	private float attackAngle;

	// Token: 0x040004A8 RID: 1192
	private Vector3 moveBackPosition;

	// Token: 0x040004A9 RID: 1193
	private float moveBackTimer;

	// Token: 0x040004AA RID: 1194
	private bool visionPrevious;

	// Token: 0x040004AB RID: 1195
	private float visionTimer;

	// Token: 0x040004AC RID: 1196
	internal float attackCooldown;

	// Token: 0x040004AD RID: 1197
	private float idleBreakerTimer;

	// Token: 0x040004AE RID: 1198
	internal float overlapCheckTimer;

	// Token: 0x040004AF RID: 1199
	internal float overlapCheckCooldown;

	// Token: 0x040004B0 RID: 1200
	internal bool overlapCheckPrevious;

	// Token: 0x040004B1 RID: 1201
	[Space]
	public ParticleSystem[] deathEffects;

	// Token: 0x040004B2 RID: 1202
	[Space]
	public Sound soundHurt;

	// Token: 0x040004B3 RID: 1203
	public Sound soundDeath;

	// Token: 0x040004B4 RID: 1204
	[Space]
	public Sound soundImpactLight;

	// Token: 0x040004B5 RID: 1205
	public Sound soundImpactMedium;

	// Token: 0x040004B6 RID: 1206
	public Sound soundImpactHeavy;

	// Token: 0x02000310 RID: 784
	public enum State
	{
		// Token: 0x04002886 RID: 10374
		Spawn,
		// Token: 0x04002887 RID: 10375
		Idle,
		// Token: 0x04002888 RID: 10376
		NoticeDelay,
		// Token: 0x04002889 RID: 10377
		Notice,
		// Token: 0x0400288A RID: 10378
		Move,
		// Token: 0x0400288B RID: 10379
		MoveUnder,
		// Token: 0x0400288C RID: 10380
		MoveOver,
		// Token: 0x0400288D RID: 10381
		MoveBack,
		// Token: 0x0400288E RID: 10382
		AttackMove,
		// Token: 0x0400288F RID: 10383
		Attack,
		// Token: 0x04002890 RID: 10384
		AttackDone,
		// Token: 0x04002891 RID: 10385
		Stun,
		// Token: 0x04002892 RID: 10386
		Despawn
	}
}
