using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

// Token: 0x0200003B RID: 59
public class EnemyBang : MonoBehaviour
{
	// Token: 0x06000103 RID: 259 RVA: 0x000099BC File Offset: 0x00007BBC
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.talkClipSampleData = new float[this.talkClipSampleDataLength];
	}

	// Token: 0x06000104 RID: 260 RVA: 0x000099F4 File Offset: 0x00007BF4
	private void Update()
	{
		this.HeadLookAtLogic();
		this.FuseLogic();
		this.TalkLogic();
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (!LevelGenerator.Instance.Generated)
			{
				return;
			}
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyBang.State.Stun);
			}
			if (this.enemy.CurrentState == EnemyState.Despawn)
			{
				this.UpdateState(EnemyBang.State.Despawn);
			}
			switch (this.currentState)
			{
			case EnemyBang.State.Spawn:
				this.StateSpawn();
				break;
			case EnemyBang.State.Idle:
				this.StateIdle();
				break;
			case EnemyBang.State.Roam:
				this.StateRoam();
				break;
			case EnemyBang.State.FuseDelay:
				this.StateFuseDelay();
				break;
			case EnemyBang.State.Fuse:
				this.StateFuse();
				break;
			case EnemyBang.State.Move:
				this.StateMove();
				break;
			case EnemyBang.State.MoveUnder:
				this.StateMoveUnder();
				break;
			case EnemyBang.State.MoveOver:
				this.StateMoveOver();
				break;
			case EnemyBang.State.MoveBack:
				this.StateMoveBack();
				break;
			case EnemyBang.State.Stun:
				this.StateStun();
				break;
			case EnemyBang.State.StunEnd:
				this.StateStunEnd();
				break;
			case EnemyBang.State.Despawn:
				this.StateDespawn();
				break;
			}
			this.TimerLogic();
			this.RotationLogic();
			this.MoveOffsetLogic();
		}
	}

	// Token: 0x06000105 RID: 261 RVA: 0x00009B0C File Offset: 0x00007D0C
	private void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBang.State.Idle);
		}
	}

	// Token: 0x06000106 RID: 262 RVA: 0x00009B90 File Offset: 0x00007D90
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
		}
		Vector3 b = EnemyBangDirector.instance.destinations[this.directorIndex];
		if (EnemyBangDirector.instance.currentState == EnemyBangDirector.State.AttackPlayer || EnemyBangDirector.instance.currentState == EnemyBangDirector.State.AttackCart)
		{
			b = EnemyBangDirector.instance.attackPosition;
		}
		this.enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, b) > 2f)
		{
			this.UpdateState(EnemyBang.State.Roam);
		}
	}

	// Token: 0x06000107 RID: 263 RVA: 0x00009C60 File Offset: 0x00007E60
	private void StateRoam()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		Vector3 vector = EnemyBangDirector.instance.destinations[this.directorIndex];
		if (EnemyBangDirector.instance.currentState == EnemyBangDirector.State.AttackPlayer || EnemyBangDirector.instance.currentState == EnemyBangDirector.State.AttackCart)
		{
			vector = EnemyBangDirector.instance.attackPosition;
		}
		this.enemy.NavMeshAgent.SetDestination(vector);
		this.MoveBackPosition();
		SemiFunc.EnemyCartJump(this.enemy);
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, vector) <= 0.5f)
		{
			this.UpdateState(EnemyBang.State.Idle);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, vector) <= 2f)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyBang.State.Idle);
			}
		}
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00009D58 File Offset: 0x00007F58
	private void StateFuseDelay()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = Random.Range(0.1f, 1f);
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
		}
		this.enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBang.State.Fuse);
		}
	}

	// Token: 0x06000109 RID: 265 RVA: 0x00009E00 File Offset: 0x00008000
	private void StateFuse()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.FuseSet(true, 0f);
			this.stateImpulse = false;
			this.stateTimer = 1.5f;
		}
		this.enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBang.State.Move);
		}
	}

	// Token: 0x0600010A RID: 266 RVA: 0x00009EA8 File Offset: 0x000080A8
	private void StateMove()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		Vector3 vector = this.AttackPositionGet();
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, vector) > 1.5f)
		{
			this.enemy.NavMeshAgent.SetDestination(vector);
		}
		else
		{
			this.enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
			this.enemy.NavMeshAgent.Disable(0.1f);
		}
		this.MoveBackPosition();
		SemiFunc.EnemyCartJump(this.enemy);
		if (EnemyBangDirector.instance.currentState != EnemyBangDirector.State.AttackPlayer)
		{
			this.UpdateState(EnemyBang.State.Idle);
			return;
		}
		if (!this.enemy.NavMeshAgent.CanReach(this.AttackVisionDynamic(), 1f) && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f)
		{
			NavMeshHit navMeshHit;
			if (!this.enemy.Jump.jumping && !this.VisionBlocked() && !NavMesh.SamplePosition(this.AttackVisionDynamic(), out navMeshHit, 0.5f, -1))
			{
				if (EnemyBangDirector.instance.playerTargetCrawling && Mathf.Abs(this.AttackVisionDynamic().y - this.enemy.Rigidbody.transform.position.y) < 0.3f)
				{
					this.UpdateState(EnemyBang.State.MoveUnder);
					return;
				}
				if (vector.y > this.enemy.Rigidbody.transform.position.y)
				{
					this.UpdateState(EnemyBang.State.MoveOver);
					return;
				}
			}
			if (vector.y > this.enemy.Rigidbody.transform.position.y + 0.2f)
			{
				this.enemy.Jump.StuckTrigger(this.AttackVisionPositionGet() - this.enemy.Vision.VisionTransform.position);
			}
		}
	}

	// Token: 0x0600010B RID: 267 RVA: 0x0000A0B8 File Offset: 0x000082B8
	private void StateMoveUnder()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
			base.transform.position = this.enemy.Rigidbody.transform.position;
		}
		Vector3 vector = this.AttackPositionGet();
		this.enemy.NavMeshAgent.Disable(0.1f);
		this.enemy.Vision.StandOverride(0.25f);
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, vector) > 1f)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, vector, this.enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		}
		else
		{
			this.enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		}
		this.enemy.Jump.StuckDisable(0.5f);
		SemiFunc.EnemyCartJump(this.enemy);
		if (EnemyBangDirector.instance.currentState != EnemyBangDirector.State.AttackPlayer)
		{
			this.UpdateState(EnemyBang.State.MoveBack);
			return;
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(vector, out navMeshHit, 0.5f, -1))
		{
			this.UpdateState(EnemyBang.State.MoveBack);
			return;
		}
		if (this.VisionBlocked())
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyBang.State.MoveBack);
				return;
			}
		}
		else
		{
			EnemyBangDirector.instance.SeeTarget();
			this.stateTimer = 2f;
		}
	}

	// Token: 0x0600010C RID: 268 RVA: 0x0000A234 File Offset: 0x00008434
	private void StateMoveOver()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
			base.transform.position = this.enemy.Rigidbody.transform.position;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		this.enemy.Vision.StandOverride(0.25f);
		Vector3 vector = this.AttackPositionGet();
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, vector) > 1f)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, vector, this.enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		}
		else
		{
			base.transform.position = this.enemy.Rigidbody.transform.position;
			this.enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		}
		SemiFunc.EnemyCartJump(this.enemy);
		if (this.AttackVisionDynamic().y > this.enemy.Rigidbody.transform.position.y + 0.3f && !this.enemy.Jump.jumping)
		{
			Vector3 normalized = (this.AttackVisionDynamic() - this.enemy.Rigidbody.transform.position).normalized;
			this.enemy.Jump.StuckTrigger(normalized);
			this.enemy.Rigidbody.WarpDisable(0.25f);
			base.transform.position = this.enemy.Rigidbody.transform.position;
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.AttackVisionDynamic(), 2f);
		}
		if (!this.enemy.Jump.jumping)
		{
			if (EnemyBangDirector.instance.currentState != EnemyBangDirector.State.AttackPlayer)
			{
				this.UpdateState(EnemyBang.State.MoveBack);
				return;
			}
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(vector, out navMeshHit, 0.5f, -1))
			{
				this.UpdateState(EnemyBang.State.MoveBack);
				return;
			}
			if (this.VisionBlocked())
			{
				this.stateTimer -= Time.deltaTime;
				if (this.stateTimer <= 0f)
				{
					this.UpdateState(EnemyBang.State.MoveBack);
					return;
				}
			}
			else
			{
				EnemyBangDirector.instance.SeeTarget();
				this.stateTimer = 2f;
			}
		}
	}

	// Token: 0x0600010D RID: 269 RVA: 0x0000A4A4 File Offset: 0x000086A4
	private void StateMoveBack()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
			base.transform.position = this.enemy.Rigidbody.transform.position;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		if (!this.enemy.Jump.jumping)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.moveBackPosition, this.enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		}
		SemiFunc.EnemyCartJump(this.enemy);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f && (Vector3.Distance(base.transform.position, this.enemy.Rigidbody.transform.position) > 2f || this.enemy.Rigidbody.notMovingTimer > 2f) && !this.enemy.Jump.jumping)
		{
			Vector3 normalized = (base.transform.position - this.moveBackPosition).normalized;
			this.enemy.Jump.StuckTrigger(base.transform.position - this.moveBackPosition);
			base.transform.position = this.enemy.Rigidbody.transform.position;
			base.transform.position += normalized * 2f;
		}
		bool flag = false;
		NavMeshHit navMeshHit;
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.moveBackPosition) <= 0.2f)
		{
			flag = true;
		}
		else if (NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit, 0.5f, -1))
		{
			flag = true;
		}
		if (flag)
		{
			if (this.fuseActive)
			{
				this.UpdateState(EnemyBang.State.Move);
				return;
			}
			this.UpdateState(EnemyBang.State.Idle);
		}
	}

	// Token: 0x0600010E RID: 270 RVA: 0x0000A6C4 File Offset: 0x000088C4
	private void StateStun()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
		}
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyBang.State.StunEnd);
		}
	}

	// Token: 0x0600010F RID: 271 RVA: 0x0000A72C File Offset: 0x0000892C
	private void StateStunEnd()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBang.State.MoveBack);
		}
	}

	// Token: 0x06000110 RID: 272 RVA: 0x0000A7B0 File Offset: 0x000089B0
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			if (!this.fuseActive)
			{
				this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
				this.enemy.NavMeshAgent.ResetPath();
			}
			this.stateImpulse = false;
		}
	}

	// Token: 0x06000111 RID: 273 RVA: 0x0000A809 File Offset: 0x00008A09
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			EnemyBangDirector.instance.OnSpawn();
			this.UpdateState(EnemyBang.State.Spawn);
			this.FuseSet(false, 0f);
		}
	}

	// Token: 0x06000112 RID: 274 RVA: 0x0000A83C File Offset: 0x00008A3C
	public void OnHurt()
	{
		if (!this.talkSource.isActiveAndEnabled)
		{
			return;
		}
		this.anim.soundHurt.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		this.anim.StunLoopPause(0.5f);
	}

	// Token: 0x06000113 RID: 275 RVA: 0x0000A89C File Offset: 0x00008A9C
	public void OnDeath()
	{
		this.anim.soundDeathSFX.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		this.anim.soundDeathVO.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
		if (this.fuseActive && this.fuseLerp <= 1f)
		{
			this.explosionScript = this.particleScriptExplosion.Spawn(this.enemy.CenterTransform.position, 0.5f, 15, 10, 1f, false, false, 1f);
			this.explosionScript.HurtCollider.onImpactEnemy.AddListener(new UnityAction(this.OnExplodeHitEnemy));
		}
		ParticleSystem[] array = this.deathEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	// Token: 0x06000114 RID: 276 RVA: 0x0000A9B3 File Offset: 0x00008BB3
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			EnemyBangDirector.instance.Investigate(this.enemy.StateInvestigate.onInvestigateTriggeredPosition);
		}
	}

	// Token: 0x06000115 RID: 277 RVA: 0x0000A9D8 File Offset: 0x00008BD8
	public void OnVision()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			EnemyBangDirector.instance.SetTarget(this.enemy.Vision.onVisionTriggeredPlayer);
			if (this.currentState == EnemyBang.State.Idle || this.currentState == EnemyBang.State.Roam)
			{
				if (!this.fuseActive)
				{
					this.UpdateState(EnemyBang.State.FuseDelay);
				}
				else
				{
					this.UpdateState(EnemyBang.State.Move);
				}
				EnemyBangDirector.instance.TriggerNearby(base.transform.position);
			}
		}
	}

	// Token: 0x06000116 RID: 278 RVA: 0x0000AA48 File Offset: 0x00008C48
	public void OnExplodeHitEnemy()
	{
		if (this.explosionScript)
		{
			EnemyBang component = this.explosionScript.HurtCollider.onImpactEnemyEnemy.GetComponent<EnemyBang>();
			if (component)
			{
				component.enemy.Health.healthCurrent = 999;
				component.FuseSet(true, Random.Range(0.96f, 0.98f));
			}
		}
	}

	// Token: 0x06000117 RID: 279 RVA: 0x0000AAAC File Offset: 0x00008CAC
	public void OnImpactLight()
	{
		if (!this.enemy.IsStunned())
		{
			return;
		}
		this.anim.soundImpactLight.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000118 RID: 280 RVA: 0x0000AAFC File Offset: 0x00008CFC
	public void OnImpactMedium()
	{
		if (!this.enemy.IsStunned())
		{
			return;
		}
		this.anim.soundImpactMedium.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000119 RID: 281 RVA: 0x0000AB4C File Offset: 0x00008D4C
	public void OnImpactHeavy()
	{
		if (!this.enemy.IsStunned())
		{
			return;
		}
		this.anim.soundImpactHeavy.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600011A RID: 282 RVA: 0x0000AB9C File Offset: 0x00008D9C
	public void UpdateState(EnemyBang.State _state)
	{
		if (this.currentState == _state)
		{
			return;
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

	// Token: 0x0600011B RID: 283 RVA: 0x0000AC10 File Offset: 0x00008E10
	private void RotationLogic()
	{
		if (this.currentState != EnemyBang.State.StunEnd)
		{
			if (this.currentState == EnemyBang.State.Idle)
			{
				Vector3 position = EnemyBangDirector.instance.transform.position;
				if (Vector3.Distance(position, this.enemy.Rigidbody.transform.position) > 0.1f)
				{
					this.horizontalRotationTarget = Quaternion.LookRotation(position - this.enemy.Rigidbody.transform.position);
					this.horizontalRotationTarget.eulerAngles = new Vector3(0f, this.horizontalRotationTarget.eulerAngles.y, 0f);
				}
			}
			else if (this.currentState == EnemyBang.State.FuseDelay || this.currentState == EnemyBang.State.Fuse || this.currentState == EnemyBang.State.Move || this.currentState == EnemyBang.State.MoveUnder || this.currentState == EnemyBang.State.MoveOver || this.currentState == EnemyBang.State.MoveBack)
			{
				if (this.enemy.Rigidbody.velocity.magnitude < 0.1f)
				{
					Vector3 a = this.AttackVisionPositionGet();
					if (Vector3.Distance(a, this.enemy.Rigidbody.transform.position) > 0.1f)
					{
						this.horizontalRotationTarget = Quaternion.LookRotation(a - this.enemy.Rigidbody.transform.position);
						this.horizontalRotationTarget.eulerAngles = new Vector3(0f, this.horizontalRotationTarget.eulerAngles.y, 0f);
					}
				}
				else
				{
					this.horizontalRotationTarget = Quaternion.LookRotation(this.enemy.Rigidbody.velocity.normalized);
					this.horizontalRotationTarget.eulerAngles = new Vector3(0f, this.horizontalRotationTarget.eulerAngles.y, 0f);
				}
			}
			else if (this.enemy.Rigidbody.velocity.magnitude > 0.1f)
			{
				this.horizontalRotationTarget = Quaternion.LookRotation(this.enemy.Rigidbody.velocity.normalized);
				this.horizontalRotationTarget.eulerAngles = new Vector3(0f, this.horizontalRotationTarget.eulerAngles.y, 0f);
			}
		}
		this.rotationTransform.rotation = SemiFunc.SpringQuaternionGet(this.horizontalRotationSpring, this.horizontalRotationTarget, -1f);
	}

	// Token: 0x0600011C RID: 284 RVA: 0x0000AE6C File Offset: 0x0000906C
	private Vector3 AttackPositionGet()
	{
		return EnemyBangDirector.instance.attackPosition;
	}

	// Token: 0x0600011D RID: 285 RVA: 0x0000AE78 File Offset: 0x00009078
	private Vector3 AttackVisionPositionGet()
	{
		return EnemyBangDirector.instance.attackVisionPosition;
	}

	// Token: 0x0600011E RID: 286 RVA: 0x0000AE84 File Offset: 0x00009084
	private Vector3 AttackVisionDynamic()
	{
		if (EnemyBangDirector.instance.currentState == EnemyBangDirector.State.AttackPlayer)
		{
			return this.AttackPositionGet();
		}
		return this.AttackVisionPositionGet();
	}

	// Token: 0x0600011F RID: 287 RVA: 0x0000AEA0 File Offset: 0x000090A0
	private void MoveBackPosition()
	{
		if (Vector3.Distance(base.transform.position, this.enemy.Rigidbody.transform.position) < 1f)
		{
			this.moveBackPosition = base.transform.position;
		}
	}

	// Token: 0x06000120 RID: 288 RVA: 0x0000AEE0 File Offset: 0x000090E0
	private bool VisionBlocked()
	{
		if (this.visionTimer <= 0f)
		{
			this.visionTimer = 0.1f;
			Vector3 direction = this.AttackVisionPositionGet() - this.enemy.Vision.VisionTransform.position;
			this.visionPrevious = Physics.Raycast(this.enemy.Vision.VisionTransform.position, direction, direction.magnitude, LayerMask.GetMask(new string[]
			{
				"Default"
			}));
		}
		return this.visionPrevious;
	}

	// Token: 0x06000121 RID: 289 RVA: 0x0000AF68 File Offset: 0x00009168
	private void HeadLookAtLogic()
	{
		bool flag = false;
		if (this.currentState == EnemyBang.State.Move || this.currentState == EnemyBang.State.MoveUnder || this.currentState == EnemyBang.State.MoveOver || this.currentState == EnemyBang.State.MoveBack)
		{
			flag = true;
		}
		if (flag)
		{
			Vector3 vector = this.AttackVisionPositionGet() - this.headLookAtTarget.position;
			vector = SemiFunc.ClampDirection(vector, this.headLookAtTarget.forward, 90f);
			this.headLookAtSource.rotation = SemiFunc.SpringQuaternionGet(this.headLookAtSpring, Quaternion.LookRotation(vector), -1f);
			return;
		}
		this.headLookAtSource.rotation = SemiFunc.SpringQuaternionGet(this.headLookAtSpring, this.headLookAtTarget.rotation, -1f);
	}

	// Token: 0x06000122 RID: 290 RVA: 0x0000B016 File Offset: 0x00009216
	private void TimerLogic()
	{
		this.visionTimer -= Time.deltaTime;
	}

	// Token: 0x06000123 RID: 291 RVA: 0x0000B02C File Offset: 0x0000922C
	private void FuseLogic()
	{
		if (this.fuseActive)
		{
			if (!EnemyBangDirector.instance.debugNoFuseProgress)
			{
				this.fuseLerp += Time.deltaTime / 15f;
			}
			this.fuseLerp = Mathf.Clamp01(this.fuseLerp);
			if (SemiFunc.IsMasterClientOrSingleplayer() && this.fuseLerp >= 1f)
			{
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("ExplodeRPC", RpcTarget.All, Array.Empty<object>());
				}
				else
				{
					this.ExplodeRPC(default(PhotonMessageInfo));
				}
				this.UpdateState(EnemyBang.State.Despawn);
				this.stateImpulse = false;
				this.enemy.EnemyParent.Despawn();
			}
		}
	}

	// Token: 0x06000124 RID: 292 RVA: 0x0000B0DC File Offset: 0x000092DC
	private void MoveOffsetLogic()
	{
		if ((this.currentState == EnemyBang.State.Move || this.currentState == EnemyBang.State.MoveUnder || this.currentState == EnemyBang.State.MoveOver || this.currentState == EnemyBang.State.MoveBack) && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.AttackPositionGet()) > 2f)
		{
			this.moveOffsetTimer = 0.2f;
		}
		else if (this.currentState == EnemyBang.State.Roam && Vector3.Distance(this.enemy.Rigidbody.transform.position, EnemyBangDirector.instance.destinations[this.directorIndex]) <= 2f)
		{
			this.moveOffsetTimer = 0.2f;
		}
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
					Vector3 vector = Random.insideUnitSphere.normalized * Random.Range(0.5f, 1f);
					vector.y = 0f;
					this.moveOffsetPosition = vector;
					this.moveOffsetSetTimer = Random.Range(0.5f, 2f);
				}
			}
		}
		this.moveOffsetTransform.localPosition = Vector3.Lerp(this.moveOffsetTransform.localPosition, this.moveOffsetPosition, Time.deltaTime * 5f);
	}

	// Token: 0x06000125 RID: 293 RVA: 0x0000B27C File Offset: 0x0000947C
	private void TalkLogic()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.explosionTell)
		{
			bool flag = false;
			bool flag2 = false;
			if (!this.enemy.Jump.jumping)
			{
				if (this.currentState == EnemyBang.State.Idle || this.currentState == EnemyBang.State.Roam)
				{
					flag = true;
					flag2 = true;
				}
				else if (this.currentState == EnemyBang.State.Move || this.currentState == EnemyBang.State.MoveUnder || this.currentState == EnemyBang.State.MoveOver || this.currentState == EnemyBang.State.MoveBack)
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				if (!flag)
				{
					this.talkBreakerTimer = Mathf.Min(this.talkBreakerTimer, this.talkBreakerAttackTimeMax);
				}
				this.talkBreakerTimer -= Time.deltaTime;
				if (this.talkBreakerTimer <= 0f)
				{
					if (flag)
					{
						this.talkBreakerTimer = Random.Range(this.talkBreakerIdleTimeMin, this.talkBreakerIdleTimeMax);
					}
					else
					{
						this.talkBreakerTimer = Random.Range(this.talkBreakerAttackTimeMin, this.talkBreakerAttackTimeMax);
					}
					if (SemiFunc.IsMultiplayer())
					{
						this.photonView.RPC("TalkBreakerRPC", RpcTarget.All, new object[]
						{
							flag
						});
					}
					else
					{
						this.TalkBreakerRPC(flag, default(PhotonMessageInfo));
					}
				}
			}
			else
			{
				this.talkBreakerTimer = Mathf.Max(this.talkBreakerTimer, 2f);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.fuseActive)
			{
				if (this.explosionTell && this.fuseLerp >= this.explosionTellThreshold)
				{
					this.explosionTell = false;
					if (SemiFunc.IsMultiplayer())
					{
						this.photonView.RPC("ExplosionTellRPC", RpcTarget.All, Array.Empty<object>());
					}
					else
					{
						this.ExplosionTellRPC(default(PhotonMessageInfo));
					}
				}
			}
			else
			{
				this.explosionTell = true;
			}
		}
		if (this.talkClipTimer <= 0f)
		{
			this.talkClipTimer = 0.01f;
			this.talkClipLoudness = 0f;
			if (this.talkSource.clip && this.talkSource.isPlaying)
			{
				this.talkSource.clip.GetData(this.talkClipSampleData, this.talkSource.timeSamples);
				foreach (float f in this.talkClipSampleData)
				{
					this.talkClipLoudness += Mathf.Abs(f);
				}
				this.talkClipLoudness /= (float)this.talkClipSampleDataLength;
			}
			if (this.stunLoopSource.clip && this.stunLoopSource.isPlaying)
			{
				this.stunLoopSource.clip.GetData(this.talkClipSampleData, this.stunLoopSource.timeSamples);
				foreach (float f2 in this.talkClipSampleData)
				{
					this.talkClipLoudness += Mathf.Abs(f2);
				}
				this.talkClipLoudness /= (float)this.talkClipSampleDataLength;
			}
		}
		else
		{
			this.talkClipTimer -= Time.deltaTime;
		}
		this.talkTopTarget.localRotation = Quaternion.Euler(this.talkClipLoudness * -45f, 0f, 0f);
		this.talkBottomTarget.localRotation = Quaternion.Euler(this.talkClipLoudness * 90f, 0f, 0f);
		this.talkTopSource.localRotation = SemiFunc.SpringQuaternionGet(this.talkTopSpring, this.talkTopTarget.localRotation, -1f);
		this.talkBottomSource.localRotation = SemiFunc.SpringQuaternionGet(this.talkBottomSpring, this.talkBottomTarget.localRotation, -1f);
	}

	// Token: 0x06000126 RID: 294 RVA: 0x0000B600 File Offset: 0x00009800
	[PunRPC]
	private void UpdateStateRPC(EnemyBang.State _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
	}

	// Token: 0x06000127 RID: 295 RVA: 0x0000B614 File Offset: 0x00009814
	private void FuseSet(bool _active, float _lerp)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("FuseRPC", RpcTarget.All, new object[]
			{
				_active,
				_lerp
			});
			return;
		}
		this.FuseRPC(_active, _lerp, default(PhotonMessageInfo));
	}

	// Token: 0x06000128 RID: 296 RVA: 0x0000B663 File Offset: 0x00009863
	[PunRPC]
	private void FuseRPC(bool _active, float _lerp, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.fuseActive = _active;
		this.fuseLerp = _lerp;
	}

	// Token: 0x06000129 RID: 297 RVA: 0x0000B67C File Offset: 0x0000987C
	[PunRPC]
	private void ExplodeRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		ParticleSystem[] array = this.deathEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		this.explosionScript = this.particleScriptExplosion.Spawn(this.enemy.CenterTransform.position, 1f, 30, 25, 2f, false, false, 1f);
		this.explosionScript.HurtCollider.onImpactEnemy.AddListener(new UnityAction(this.OnExplodeHitEnemy));
	}

	// Token: 0x0600012A RID: 298 RVA: 0x0000B708 File Offset: 0x00009908
	[PunRPC]
	private void TalkBreakerRPC(bool _idle, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (_idle)
		{
			this.anim.soundIdleBreaker.Play(this.talkSource.transform.position, 1f, 1f, 1f, 1f);
			return;
		}
		this.anim.soundAttackBreaker.Play(this.talkSource.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600012B RID: 299 RVA: 0x0000B78C File Offset: 0x0000998C
	[PunRPC]
	private void ExplosionTellRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.anim.StunLoopPause(2f);
		this.anim.soundExplosionTell.Play(this.talkSource.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600012C RID: 300 RVA: 0x0000B7E8 File Offset: 0x000099E8
	[PunRPC]
	public void SetHeadRPC(int _index, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		int num = 0;
		foreach (GameObject gameObject in this.headObjects)
		{
			if (num == _index)
			{
				gameObject.SetActive(true);
			}
			else
			{
				Object.Destroy(gameObject);
			}
			num++;
		}
	}

	// Token: 0x0600012D RID: 301 RVA: 0x0000B830 File Offset: 0x00009A30
	[PunRPC]
	public void SetVoicePitchRPC(float _pitch, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.anim.soundAttackBreaker.Pitch = _pitch;
		this.anim.soundIdleBreaker.Pitch = _pitch;
		this.anim.soundHurt.Pitch = _pitch;
		this.anim.soundDeathVO.Pitch = _pitch;
		this.anim.soundExplosionTell.Pitch = _pitch;
		this.anim.soundFuseTell.Pitch = _pitch;
		this.anim.soundJumpVO.Pitch = _pitch;
		this.anim.soundLandVO.Pitch = _pitch;
		this.anim.soundStunIntro.Pitch = _pitch;
		this.anim.soundStunLoop.Pitch = _pitch;
		this.anim.soundStunOutro.Pitch = _pitch;
	}

	// Token: 0x04000253 RID: 595
	public EnemyBangAnim anim;

	// Token: 0x04000254 RID: 596
	[Space]
	public EnemyBang.State currentState;

	// Token: 0x04000255 RID: 597
	private bool stateImpulse;

	// Token: 0x04000256 RID: 598
	private float stateTimer;

	// Token: 0x04000257 RID: 599
	internal Enemy enemy;

	// Token: 0x04000258 RID: 600
	internal PhotonView photonView;

	// Token: 0x04000259 RID: 601
	internal int directorIndex;

	// Token: 0x0400025A RID: 602
	private Vector3 moveBackPosition;

	// Token: 0x0400025B RID: 603
	internal bool fuseActive;

	// Token: 0x0400025C RID: 604
	internal float fuseLerp;

	// Token: 0x0400025D RID: 605
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x0400025E RID: 606
	private ParticlePrefabExplosion explosionScript;

	// Token: 0x0400025F RID: 607
	private bool visionPrevious;

	// Token: 0x04000260 RID: 608
	private float visionTimer;

	// Token: 0x04000261 RID: 609
	public SpringQuaternion horizontalRotationSpring;

	// Token: 0x04000262 RID: 610
	private Quaternion horizontalRotationTarget = Quaternion.identity;

	// Token: 0x04000263 RID: 611
	[Space]
	public SpringQuaternion headLookAtSpring;

	// Token: 0x04000264 RID: 612
	public Transform headLookAtTarget;

	// Token: 0x04000265 RID: 613
	public Transform headLookAtSource;

	// Token: 0x04000266 RID: 614
	[Space]
	public ParticleSystem[] deathEffects;

	// Token: 0x04000267 RID: 615
	[Space]
	public GameObject[] headObjects;

	// Token: 0x04000268 RID: 616
	[Space]
	public Transform particleParent;

	// Token: 0x04000269 RID: 617
	public Transform moveOffsetTransform;

	// Token: 0x0400026A RID: 618
	public Transform rotationTransform;

	// Token: 0x0400026B RID: 619
	private Vector3 moveOffsetPosition;

	// Token: 0x0400026C RID: 620
	private float moveOffsetTimer;

	// Token: 0x0400026D RID: 621
	private float moveOffsetSetTimer;

	// Token: 0x0400026E RID: 622
	public AudioSource talkSource;

	// Token: 0x0400026F RID: 623
	public AudioSource stunLoopSource;

	// Token: 0x04000270 RID: 624
	[Space]
	public Sound[] talkSoundsTest;

	// Token: 0x04000271 RID: 625
	[Space]
	public SpringQuaternion talkTopSpring;

	// Token: 0x04000272 RID: 626
	public Transform talkTopSource;

	// Token: 0x04000273 RID: 627
	public Transform talkTopTarget;

	// Token: 0x04000274 RID: 628
	[Space]
	public SpringQuaternion talkBottomSpring;

	// Token: 0x04000275 RID: 629
	public Transform talkBottomSource;

	// Token: 0x04000276 RID: 630
	public Transform talkBottomTarget;

	// Token: 0x04000277 RID: 631
	[Space]
	public float talkBreakerIdleTimeMin = 5f;

	// Token: 0x04000278 RID: 632
	public float talkBreakerIdleTimeMax = 20f;

	// Token: 0x04000279 RID: 633
	[Space]
	public float talkBreakerAttackTimeMin = 2f;

	// Token: 0x0400027A RID: 634
	public float talkBreakerAttackTimeMax = 5f;

	// Token: 0x0400027B RID: 635
	private float talkBreakerTimer;

	// Token: 0x0400027C RID: 636
	private bool explosionTell;

	// Token: 0x0400027D RID: 637
	private float explosionTellThreshold = 0.95f;

	// Token: 0x0400027E RID: 638
	internal float explosionTellFuseThreshold = 0.9f;

	// Token: 0x0400027F RID: 639
	private float talkClipTimer;

	// Token: 0x04000280 RID: 640
	private float talkClipLoudness;

	// Token: 0x04000281 RID: 641
	private int talkClipSampleDataLength = 1024;

	// Token: 0x04000282 RID: 642
	private float[] talkClipSampleData;

	// Token: 0x02000304 RID: 772
	public enum State
	{
		// Token: 0x04002819 RID: 10265
		Spawn,
		// Token: 0x0400281A RID: 10266
		Idle,
		// Token: 0x0400281B RID: 10267
		Roam,
		// Token: 0x0400281C RID: 10268
		FuseDelay,
		// Token: 0x0400281D RID: 10269
		Fuse,
		// Token: 0x0400281E RID: 10270
		Move,
		// Token: 0x0400281F RID: 10271
		MoveUnder,
		// Token: 0x04002820 RID: 10272
		MoveOver,
		// Token: 0x04002821 RID: 10273
		MoveBack,
		// Token: 0x04002822 RID: 10274
		Stun,
		// Token: 0x04002823 RID: 10275
		StunEnd,
		// Token: 0x04002824 RID: 10276
		Despawn
	}
}
