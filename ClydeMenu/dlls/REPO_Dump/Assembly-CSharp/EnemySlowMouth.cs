using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

// Token: 0x02000074 RID: 116
public class EnemySlowMouth : MonoBehaviour
{
	// Token: 0x0600041B RID: 1051 RVA: 0x00028EF4 File Offset: 0x000270F4
	private void Start()
	{
		this.spawnDespawnScaleSpring = new SpringFloat();
		this.spawnDespawnScaleSpring.damping = 0.5f;
		this.spawnDespawnScaleSpring.speed = 20f;
		this.photonView = base.GetComponent<PhotonView>();
		this.enemy = base.GetComponent<Enemy>();
		this.followTargetStartPosition = this.followTarget.localPosition;
		this.enemyVision = base.GetComponent<EnemyVision>();
		this.spawnParticles = new List<ParticleSystem>(this.particles.GetComponentsInChildren<ParticleSystem>());
		this.springTentacles = new List<SpringTentacle>(this.tentacles.GetComponentsInChildren<SpringTentacle>());
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x00028F8D File Offset: 0x0002718D
	private void AnimStateIdle()
	{
		this.enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Idle);
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x00028F9B File Offset: 0x0002719B
	private void AnimStatePuke()
	{
		this.enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Puke);
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x00028FA9 File Offset: 0x000271A9
	private void AnimStateStunned()
	{
		this.enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Stunned);
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x00028FB7 File Offset: 0x000271B7
	private void AnimStateTargetting()
	{
		this.enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Targetting);
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x00028FC5 File Offset: 0x000271C5
	private void AnimStateAttached()
	{
		this.enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Attached);
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x00028FD3 File Offset: 0x000271D3
	private void AnimStateAggro()
	{
		this.enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Aggro);
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x00028FE1 File Offset: 0x000271E1
	private void AnimStateSpawnDespawn()
	{
		this.enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.SpawnDespawn);
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x00028FEF File Offset: 0x000271EF
	private void AnimStateDeath()
	{
		this.enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Death);
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x00028FFD File Offset: 0x000271FD
	private void AnimStateLeave()
	{
		this.enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Leave);
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x0002900C File Offset: 0x0002720C
	private void PlaySpawnParticles()
	{
		foreach (ParticleSystem particleSystem in this.spawnParticles)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x0002905C File Offset: 0x0002725C
	private void StateSpawn(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.enemyVisuals.localScale = Vector3.zero;
				this.stateTimer = 3f;
				this.stateImpulse = false;
				this.PlaySpawnParticles();
			}
			float d = SemiFunc.SpringFloatGet(this.spawnDespawnScaleSpring, 1f, -1f);
			this.enemyVisuals.localScale = Vector3.one * d;
			this.AnimStateSpawnDespawn();
			if (this.stateTimer <= 0f)
			{
				this.enemyVisuals.localScale = Vector3.one;
				this.UpdateState(EnemySlowMouth.State.Idle);
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x00029108 File Offset: 0x00027308
	private void StateIdle(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateImpulse = false;
				this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
				this.enemy.NavMeshAgent.ResetPath();
				this.stateTimer = Random.Range(5f, 10f);
			}
			this.AnimStateIdle();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			if (SemiFunc.EnemySpawnIdlePause())
			{
				return;
			}
			if (this.IdlePukeLogic(0.1f))
			{
				return;
			}
			this.IdleBreakerVOLogic();
			this.LookAtVelocityDirection(false);
			this.FloatAround();
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemySlowMouth.State.Roam);
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x000291D0 File Offset: 0x000273D0
	private void StateDespawn(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateImpulse = false;
				this.PlaySpawnParticles();
				this.soundDetach.Play(this.centerTransform.position, 1f, 1f, 1f, 1f);
			}
			float d = SemiFunc.SpringFloatGet(this.spawnDespawnScaleSpring, 0f, -1f);
			this.enemyVisuals.localScale = Vector3.one * d;
			this.AnimStateSpawnDespawn();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			if (this.stateTimer <= 0f)
			{
				this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
				this.enemy.NavMeshAgent.ResetPath();
				this.enemy.EnemyParent.Despawn();
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x000292C0 File Offset: 0x000274C0
	private void StateRoam(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateImpulse = false;
				this.agentDestination = SemiFunc.EnemyRoamFindPoint(base.transform.position);
				this.stateTimer = Random.Range(5f, 10f);
				this.followTarget.localPosition = new Vector3(0f, 1f, 0f);
			}
			this.AnimStateIdle();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			if (this.IdlePukeLogic(0.1f))
			{
				return;
			}
			this.IdleBreakerVOLogic();
			this.LookAtVelocityDirection(false);
			this.StuckLogic();
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			if (Vector3.Distance(base.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 1f || this.stateTimer <= 0f)
			{
				this.UpdateState(EnemySlowMouth.State.Idle);
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x000293BC File Offset: 0x000275BC
	private void StateInvestigate(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateImpulse = false;
				this.followTarget.localPosition = new Vector3(0f, 1f, 0f);
				this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			}
			this.AnimStateIdle();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			this.LookAtVelocityDirection(false);
			if (this.IdlePukeLogic(0.2f))
			{
				return;
			}
			this.IdleBreakerVOLogic();
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			if (Vector3.Distance(base.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 1f)
			{
				this.UpdateState(EnemySlowMouth.State.Idle);
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x000294A0 File Offset: 0x000276A0
	private void StateStun(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
				this.enemy.NavMeshAgent.ResetPath();
				this.stateImpulse = false;
			}
			this.AnimStateStunned();
			foreach (SpringTentacle springTentacle in this.springTentacles)
			{
				if (SemiFunc.FPSImpulse5())
				{
					springTentacle.springStart.springVelocity = Random.insideUnitSphere * 25f;
					springTentacle.springMid.springVelocity = Random.insideUnitSphere * 25f;
					springTentacle.springEnd.springVelocity = Random.insideUnitSphere * 25f;
				}
			}
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			if (!this.enemy.IsStunned())
			{
				this.UpdateState(EnemySlowMouth.State.Idle);
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x000295C0 File Offset: 0x000277C0
	private void StateLeave(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateImpulse = false;
				Vector3 vector = SemiFunc.EnemyLeaveFindPoint(base.transform.position);
				this.agentDestination = vector;
				this.followTarget.localPosition = new Vector3(0f, 1f, 0f);
				this.stateTimer = Random.Range(10f, 15f);
				this.PlaySpawnParticles();
				SemiFunc.EnemyLeaveStart(this.enemy);
			}
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			this.AnimStateLeave();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			this.StuckLogic();
			this.IdleBreakerVOLogic();
			this.FastMoving(false);
			this.enemyRigidbody.OverrideFollowPosition(0.1f, 5f, 10f);
			this.enemy.NavMeshAgent.OverrideAgent(8f, 8f, 0.1f);
			if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 1f)
			{
				this.UpdateState(EnemySlowMouth.State.Idle);
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x000296FC File Offset: 0x000278FC
	private void StateNotice(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.TargettingPlayerStart();
				if (!this.audioSourceVO.isPlaying)
				{
					this.soundNoticeVO.Play(this.centerTransform.position, 1f, 1f, 1f, 1f);
				}
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					this.UpdatePlayerTarget(this.enemyVision.onVisionTriggeredPlayer);
				}
				this.stateTimer = 1f;
				this.stateImpulse = false;
			}
			this.AnimStateIdle();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			this.followTarget.localRotation = Quaternion.LookRotation(this.currentTarget.position - this.centerTransform.position, Vector3.up);
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemySlowMouth.State.GoToPlayer);
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x000297E4 File Offset: 0x000279E4
	private void StateAttack(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateImpulse = false;
				EnemySlowMouthAttaching component = Object.Instantiate<GameObject>(this.enemySlowMouthAttack, this.centerTransform.position, this.centerTransform.rotation).GetComponent<EnemySlowMouthAttaching>();
				component.targetPlayerAvatar = this.playerTarget;
				component.enemySlowMouth = this;
				this.attachedTimer = 0f;
			}
			this.AnimStateAttached();
			this.OverrideHideEnemy();
			this.LookAtVelocityDirection(false);
			return;
		}
		if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x00029869 File Offset: 0x00027A69
	private void DetatchLogic()
	{
		if (SemiFunc.FPSImpulse5())
		{
			if (!this.playerTarget)
			{
				this.UpdateState(EnemySlowMouth.State.Detach);
				return;
			}
			if (this.playerTarget.isDisabled)
			{
				this.UpdateState(EnemySlowMouth.State.Detach);
				return;
			}
		}
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x000298A0 File Offset: 0x00027AA0
	private void StateAttached(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateImpulse = false;
				if (this.attachedTimer <= 0f)
				{
					this.attachedTimer = Random.Range(20f, 60f);
				}
				this.stateTimer = Random.Range(1f, 15f);
			}
			this.OverrideHideEnemy();
			this.AnimStateAttached();
			this.PlayerEffects();
			this.enemy.EnemyParent.SpawnedTimerPause(1f);
			this.enemy.transform.position = base.transform.position;
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			this.DetatchLogic();
			if (SemiFunc.FPSImpulse5())
			{
				bool flag = this.playerTarget.playerAvatarVisuals.GetComponentInChildren<EnemySlowMouthPlayerAvatarAttached>();
				bool flag2 = this.playerTarget.localCameraTransform.GetComponentInChildren<EnemySlowMouthCameraVisuals>();
				if (!flag && !flag2)
				{
					if (this.currentTarget)
					{
						this.detachPosition = this.currentTarget.position;
						this.detachRotation = this.currentTarget.rotation;
					}
					this.UpdateState(EnemySlowMouth.State.Detach);
				}
			}
			this.IsPossessedBySeveral();
			this.attachedTimer -= Time.deltaTime;
			if (this.attachedTimer <= 0f || this.playerTarget.isDisabled)
			{
				if (this.currentTarget)
				{
					this.detachPosition = this.currentTarget.position;
					this.detachRotation = this.currentTarget.rotation;
					this.UpdateState(EnemySlowMouth.State.Detach);
					return;
				}
				this.enemy.EnemyParent.SpawnedTimerPause(0f);
				this.UpdateState(EnemySlowMouth.State.Despawn);
				return;
			}
			else if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemySlowMouth.State.Puke);
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x00029A64 File Offset: 0x00027C64
	private void StatePuke(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateImpulse = false;
				this.stateTimer = Random.Range(0.5f, 3f);
				if (Random.Range(0, 30) == 0)
				{
					this.stateTimer = 6f;
				}
			}
			this.PlayerEffects();
			this.OverrideHideEnemy();
			this.AnimStateAttached();
			this.DetatchLogic();
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemySlowMouth.State.Attached);
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00029AEC File Offset: 0x00027CEC
	private void StateDetach(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateImpulse = false;
				this.stateTimer = 0f;
				this.attachedTimer = 0f;
				this.possessCooldown = Random.Range(30f, 120f);
				this.enemy.transform.position = base.transform.position;
			}
			this.PlayerEffects();
			this.OverrideHideEnemy();
			this.AnimStateAttached();
			if (this.stateTimer <= 0f)
			{
				if (!this.playerTarget)
				{
					this.enemy.EnemyParent.SpawnedTimerPause(0f);
					this.UpdateState(EnemySlowMouth.State.Despawn);
					return;
				}
				Vector3 forward = this.playerTarget.localCameraTransform.forward;
				Vector3 origin = this.playerTarget.localCameraTransform.position + forward * 0.3f;
				float num = 0.2f;
				if (Physics.SphereCastAll(origin, 0.45f, forward, num, LayerMask.GetMask(new string[]
				{
					"Default"
				})).Length == 0)
				{
					Vector3 position = this.playerTarget.localCameraTransform.position + forward * num;
					if (this.playerTarget && this.playerTarget.tumble)
					{
						this.playerTarget.tumble.TumbleRequest(true, false);
					}
					this.physGrabObject.Teleport(position, this.playerTarget.localCameraTransform.rotation);
					this.enemy.NavMeshAgent.Warp(position);
					this.soundDetach.Play(position, 1f, 1f, 1f, 1f);
					this.soundDetachVO.Play(position, 1f, 1f, 1f, 1f);
					this.enemy.EnemyParent.SpawnedTimerPause(2f);
					this.UpdateState(EnemySlowMouth.State.Leave);
					return;
				}
				if (this.playerTarget.isDisabled)
				{
					this.enemy.EnemyParent.SpawnedTimerPause(0f);
					this.UpdateState(EnemySlowMouth.State.Despawn);
					return;
				}
				this.stateTimer = 0.25f;
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x00029D1C File Offset: 0x00027F1C
	private void StateIdlePuke(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateImpulse = false;
				this.stateTimer = Random.Range(0.5f, 2f);
				if (Random.Range(0, 30) == 0)
				{
					this.stateTimer = 4f;
				}
			}
			this.AnimStatePuke();
			this.semiPuke.PukeActive(this.mouthTransform.position, this.mouthTransform.rotation);
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(this.idlePukePreviousState);
				return;
			}
		}
		else
		{
			if (this.stateStartFixed)
			{
				this.stateStartFixed = false;
			}
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			Vector3 a = Random.insideUnitSphere * 80f;
			this.enemyRigidbody.rb.AddTorque(a * Time.fixedDeltaTime, ForceMode.Force);
			Vector3 a2 = -this.mouthTransform.forward * 400f;
			this.enemyRigidbody.rb.AddForce(a2 * Time.fixedDeltaTime, ForceMode.Force);
		}
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x00029E24 File Offset: 0x00028024
	private void StateGoToPlayerOver(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateTimer = 2f;
				this.stateImpulse = false;
				this.followTarget.localPosition = Vector3.zero;
			}
			this.followTarget.localPosition = this.currentTarget.localPosition;
			this.AnimStateTargetting();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			this.FastMoving(true);
			this.TargettingPlayer();
			if (this.IdlePukeLogic(1f))
			{
				return;
			}
			this.AttachToPlayer();
			this.enemy.NavMeshAgent.Disable(0.1f);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.enemy.NavMeshAgent.DefaultSpeed * 0.5f * Time.deltaTime);
			this.enemy.Vision.StandOverride(0.25f);
			if (this.playerTarget.PlayerVisionTarget.VisionTransform.position.y > this.enemy.Rigidbody.transform.position.y + 1.5f)
			{
				base.transform.position = this.enemy.Rigidbody.transform.position;
				base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, 2f);
			}
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(this.targetPosition, out navMeshHit, 0.5f, -1))
			{
				this.UpdateState(EnemySlowMouth.State.MoveBackToNavmesh);
			}
			else if (this.VisionBlocked() || !this.playerTarget || this.playerTarget.isDisabled)
			{
				if (this.stateTimer <= 0f || this.enemy.Rigidbody.notMovingTimer > 1f)
				{
					this.UpdateState(EnemySlowMouth.State.MoveBackToNavmesh);
				}
			}
			else
			{
				this.stateTimer = 2f;
			}
			if (SemiFunc.EnemyForceLeave(this.enemy))
			{
				this.UpdateState(EnemySlowMouth.State.MoveBackToNavmesh);
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x0002A030 File Offset: 0x00028230
	private void StateGoToPlayerUnder(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateTimer = 2f;
				this.stateImpulse = false;
				this.followTarget.localPosition = Vector3.zero;
			}
			this.AnimStateTargetting();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			this.FastMoving(true);
			this.TargettingPlayer();
			this.AttachToPlayer();
			if (this.IdlePukeLogic(1f))
			{
				return;
			}
			this.followTarget.localPosition = new Vector3(0f, 0.2f, 0f);
			this.enemy.NavMeshAgent.Disable(0.1f);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
			this.enemy.Vision.StandOverride(0.25f);
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(this.targetPosition, out navMeshHit, 0.5f, -1))
			{
				this.UpdateState(EnemySlowMouth.State.MoveBackToNavmesh);
			}
			else if (this.VisionBlocked() || !this.playerTarget || this.playerTarget.isDisabled)
			{
				if (this.stateTimer <= 0f)
				{
					this.UpdateState(EnemySlowMouth.State.MoveBackToNavmesh);
				}
			}
			else
			{
				this.stateTimer = 2f;
			}
			if (SemiFunc.EnemyForceLeave(this.enemy))
			{
				this.UpdateState(EnemySlowMouth.State.MoveBackToNavmesh);
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x0002A1A4 File Offset: 0x000283A4
	private void StateMoveBackToNavmesh(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateImpulse = false;
				this.stateTimer = 30f;
				this.followTarget.localPosition = Vector3.zero;
			}
			this.AnimStateTargetting();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			this.FastMoving(false);
			this.TargettingPlayer();
			this.enemy.NavMeshAgent.OverrideAgent(8f, 8f, 0.1f);
			if (this.IdlePukeLogic(1f))
			{
				return;
			}
			this.enemy.NavMeshAgent.Disable(0.1f);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.moveBackPosition, this.enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
			this.enemy.Vision.StandOverride(0.25f);
			if (Vector3.Distance(base.transform.position, this.enemyGroundPosition) > 2f || this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				Vector3 normalized = (this.moveBackPosition - this.enemyGroundPosition).normalized;
				base.transform.position = this.enemy.Rigidbody.transform.position;
				base.transform.position += normalized * 2f;
			}
			NavMeshHit navMeshHit;
			if (Vector3.Distance(this.enemyGroundPosition, this.moveBackPosition) <= 0f || NavMesh.SamplePosition(this.enemyGroundPosition, out navMeshHit, 0.5f, -1))
			{
				this.UpdateState(EnemySlowMouth.State.GoToPlayer);
				return;
			}
			if (this.stateTimer <= 0f)
			{
				this.enemy.EnemyParent.SpawnedTimerSet(0f);
				return;
			}
		}
		else if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x0002A384 File Offset: 0x00028584
	private void StateGoToPlayer(bool fixedUpdate)
	{
		if (fixedUpdate)
		{
			if (this.stateStartFixed)
			{
				this.stateStartFixed = false;
			}
			return;
		}
		if (this.stateImpulse)
		{
			this.followTarget.localPosition = Vector3.zero;
			this.stateImpulse = false;
			this.stateTimer = 5f;
			this.targetedPlayerTime = 0f;
			this.targetedPlayerTimeMax = Random.Range(8f, 22f);
		}
		this.AnimStateTargetting();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.FastMoving(true);
		this.TargettingPlayer();
		this.enemy.NavMeshAgent.OverrideAgent(8f, 8f, 0.1f);
		if (this.IdlePukeLogic(1f))
		{
			return;
		}
		this.AttachToPlayer();
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.MoveBackPosition();
		this.enemy.Vision.StandOverride(0.25f);
		NavMeshHit navMeshHit;
		if (!this.enemy.NavMeshAgent.CanReach(this.targetPosition, 1f) && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f && !this.VisionBlocked() && !NavMesh.SamplePosition(this.targetPosition, out navMeshHit, 0.5f, -1))
		{
			if (this.playerTarget.isCrawling && Mathf.Abs(this.targetPosition.y - this.enemy.Rigidbody.transform.position.y) < 0.3f)
			{
				this.UpdateState(EnemySlowMouth.State.GoToPlayerUnder);
				return;
			}
			if (this.targetPosition.y > this.enemy.Rigidbody.transform.position.y)
			{
				this.UpdateState(EnemySlowMouth.State.GoToPlayerOver);
				return;
			}
		}
		this.LeaveCheck();
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x0002A562 File Offset: 0x00028762
	private void StateDead(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (this.stateImpulse)
			{
				this.stateImpulse = false;
			}
			this.OverrideHideEnemy();
			this.AnimStateDeath();
			return;
		}
		if (this.stateStartFixed)
		{
			this.stateStartFixed = false;
		}
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x0002A594 File Offset: 0x00028794
	private void StateMachine(bool fixedUpdate)
	{
		if (!fixedUpdate && this.stateTimer > 0f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		if (fixedUpdate)
		{
			if (this.enemyHiddenTimer <= 0f && !this.enemyCollider.enabled)
			{
				this.PlaySpawnParticles();
				this.enemySlowMouthAnim.gameObject.SetActive(true);
				this.enemyCollider.enabled = true;
			}
			if (this.enemyHiddenTimer > 0f)
			{
				this.enemyHiddenTimer -= Time.fixedDeltaTime;
				if (this.enemyCollider.enabled)
				{
					this.PlaySpawnParticles();
					this.enemySlowMouthAnim.gameObject.SetActive(false);
					this.enemyCollider.enabled = false;
				}
			}
		}
		switch (this.currentState)
		{
		case EnemySlowMouth.State.Spawn:
			this.StateSpawn(fixedUpdate);
			return;
		case EnemySlowMouth.State.Idle:
			this.StateIdle(fixedUpdate);
			return;
		case EnemySlowMouth.State.Despawn:
			this.StateDespawn(fixedUpdate);
			return;
		case EnemySlowMouth.State.Roam:
			this.StateRoam(fixedUpdate);
			return;
		case EnemySlowMouth.State.Investigate:
			this.StateInvestigate(fixedUpdate);
			return;
		case EnemySlowMouth.State.Stun:
			this.StateStun(fixedUpdate);
			return;
		case EnemySlowMouth.State.Leave:
			this.StateLeave(fixedUpdate);
			return;
		case EnemySlowMouth.State.Notice:
			this.StateNotice(fixedUpdate);
			return;
		case EnemySlowMouth.State.Attack:
			this.StateAttack(fixedUpdate);
			return;
		case EnemySlowMouth.State.Attached:
			this.StateAttached(fixedUpdate);
			return;
		case EnemySlowMouth.State.Puke:
			this.StatePuke(fixedUpdate);
			return;
		case EnemySlowMouth.State.Detach:
			this.StateDetach(fixedUpdate);
			return;
		case EnemySlowMouth.State.IdlePuke:
			this.StateIdlePuke(fixedUpdate);
			return;
		case EnemySlowMouth.State.GoToPlayerOver:
			this.StateGoToPlayerOver(fixedUpdate);
			return;
		case EnemySlowMouth.State.GoToPlayerUnder:
			this.StateGoToPlayerUnder(fixedUpdate);
			return;
		case EnemySlowMouth.State.MoveBackToNavmesh:
			this.StateMoveBackToNavmesh(fixedUpdate);
			return;
		case EnemySlowMouth.State.GoToPlayer:
			this.StateGoToPlayer(fixedUpdate);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x0002A730 File Offset: 0x00028930
	public void UpdateState(EnemySlowMouth.State newState)
	{
		if (this.currentState != newState && SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("UpdateStateRPC", RpcTarget.All, new object[]
				{
					newState
				});
				return;
			}
			this.UpdateStateRPC(newState, default(PhotonMessageInfo));
		}
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x0002A788 File Offset: 0x00028988
	private void UpdatePlayerTarget(PlayerAvatar _player)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("UpdatePlayerTargetRPC", RpcTarget.All, new object[]
				{
					_player.photonView.ViewID
				});
				return;
			}
			this.UpdatePlayerTargetRPC(_player.photonView.ViewID, default(PhotonMessageInfo));
		}
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x0002A7E8 File Offset: 0x000289E8
	[PunRPC]
	private void UpdatePlayerTargetRPC(int _photonViewID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
		{
			if (playerAvatar.photonView.ViewID == _photonViewID)
			{
				this.playerTarget = playerAvatar;
				this.currentTarget = SemiFunc.PlayerGetFaceEyeTransform(playerAvatar);
				break;
			}
		}
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x0002A860 File Offset: 0x00028A60
	[PunRPC]
	public void UpdateStateRPC(EnemySlowMouth.State newState, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = newState;
		this.stateImpulse = true;
		this.stateStartFixed = true;
		this.stateTimer = 0f;
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x0002A88B File Offset: 0x00028A8B
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemySlowMouth.State.Spawn);
		}
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x0002A8A8 File Offset: 0x00028AA8
	public void OnHurt()
	{
		if (!this.audioSourceVO.isActiveAndEnabled)
		{
			return;
		}
		this.soundHurtVO.Play(this.centerTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x0002A8E3 File Offset: 0x00028AE3
	public void OnVision()
	{
		if (this.currentState == EnemySlowMouth.State.Idle || this.currentState == EnemySlowMouth.State.Investigate || this.currentState == EnemySlowMouth.State.Roam || this.currentState == EnemySlowMouth.State.Leave)
		{
			this.UpdateState(EnemySlowMouth.State.Notice);
		}
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x0002A910 File Offset: 0x00028B10
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && (this.currentState == EnemySlowMouth.State.Idle || this.currentState == EnemySlowMouth.State.Roam || this.currentState == EnemySlowMouth.State.Investigate))
		{
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			this.UpdateState(EnemySlowMouth.State.Investigate);
		}
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x0002A95C File Offset: 0x00028B5C
	public void OnDeath()
	{
		this.soundDieVO.Play(this.centerTransform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.05f);
		this.PlaySpawnParticles();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
			this.UpdateState(EnemySlowMouth.State.Dead);
		}
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x0002AA20 File Offset: 0x00028C20
	public void OnDespawn()
	{
		this.soundDieVO.Play(this.centerTransform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.05f);
		this.PlaySpawnParticles();
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x0002AAC4 File Offset: 0x00028CC4
	private void Update()
	{
		if (this.currentState != EnemySlowMouth.State.Stun)
		{
			this.physGrabObject.OverrideZeroGravity(0.1f);
		}
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			this.UpdateState(EnemySlowMouth.State.Despawn);
		}
		this.LoopSounds();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.possessCooldown > 0f)
			{
				this.possessCooldown -= Time.deltaTime;
			}
			if (this.enemy.IsStunned() && this.enemyHiddenTimer <= 0f)
			{
				this.UpdateState(EnemySlowMouth.State.Stun);
			}
			this.enemyGroundPosition = new Vector3(this.enemy.Rigidbody.transform.position.x, base.transform.position.y, this.enemy.Rigidbody.transform.position.z);
			this.TargetPositionLogic();
		}
		this.StateMachine(false);
		if (SemiFunc.FPSImpulse30())
		{
			this.followPointPositionPrev = this.followTarget.position;
		}
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x0002ABC4 File Offset: 0x00028DC4
	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			Vector3 forward = this.enemy.Rigidbody.transform.forward;
			RaycastHit raycastHit;
			if (Physics.Raycast(this.centerTransform.position, forward, out raycastHit, 1f, SemiFunc.LayerMaskGetVisionObstruct()) && !raycastHit.collider.GetComponentInParent<PhysGrabHinge>())
			{
				this.enemyRigidbody.rb.AddForce(-(forward * 600f) * Time.fixedDeltaTime, ForceMode.Force);
			}
		}
		this.StateMachine(true);
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x0002AC58 File Offset: 0x00028E58
	private void RandomNudge(float _force)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.soundIdleBreakerVO.Play(this.centerTransform.position, 1f, 1f, 1f, 1f);
		Vector3 a = this.centerTransform.up;
		int num = Random.Range(0, 4);
		if (num == 0)
		{
			a = this.centerTransform.up;
		}
		else if (num == 1)
		{
			a = this.centerTransform.right;
		}
		else if (num == 2)
		{
			a = -this.centerTransform.right;
		}
		else if (num == 3)
		{
			a = -this.centerTransform.up;
		}
		this.centerTransform.position + a * 0.5f;
		this.randomNudgeTimer = 0f;
		this.enemyRigidbody.rb.AddForce(a * _force, ForceMode.Impulse);
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x0002AD3C File Offset: 0x00028F3C
	private void LookAtVelocityDirection(bool _moving)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		Vector3 forward = this.enemyRigidbody.rb.velocity.normalized;
		if (_moving)
		{
			forward = this.enemy.moveDirection;
		}
		if (forward.magnitude > 0.001f)
		{
			Quaternion b = Quaternion.LookRotation(forward, Vector3.up);
			this.followTarget.localRotation = Quaternion.Slerp(this.followTarget.localRotation, b, Time.deltaTime * 2f);
		}
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x0002ADBC File Offset: 0x00028FBC
	private void FloatAround()
	{
		this.followTarget.localPosition = this.followTargetStartPosition + Vector3.up * Mathf.Sin(Time.time * 0.5f) * 0.5f;
		this.followTarget.localPosition += Vector3.left * Mathf.Sin(Time.time * 0.2f) * 0.3f;
		this.followTarget.localPosition += Vector3.forward * Mathf.Sin(Time.time * 0.2f) * 2f;
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x0002AE77 File Offset: 0x00029077
	private bool IdlePukeLogic(float _tickSpeed = 1f)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return false;
		}
		if (this.idlePukeCooldown > 0f)
		{
			this.idlePukeCooldown -= Time.deltaTime * _tickSpeed;
			return false;
		}
		this.IdlePukeExecute();
		return true;
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x0002AEB0 File Offset: 0x000290B0
	private void IdlePukeExecute()
	{
		if (this.audioSourceVO.isPlaying)
		{
			return;
		}
		this.idlePukePreviousState = this.currentState;
		this.UpdateState(EnemySlowMouth.State.IdlePuke);
		this.idlePukeCooldown = Random.Range(5f, 10f);
		float force = Random.Range(5f, 20f);
		this.RandomNudge(force);
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x0002AF0B File Offset: 0x0002910B
	private void IdleBreakerVOLogic()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.idleBreakerVOCooldown > 0f)
		{
			this.idleBreakerVOCooldown -= Time.deltaTime;
			return;
		}
		this.IdleBreakerVO();
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x0002AF3C File Offset: 0x0002913C
	public void IdleBreakerVO()
	{
		if (this.audioSourceVO.isPlaying)
		{
			return;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("IdleBreakerVORPC", RpcTarget.All, Array.Empty<object>());
			}
			else
			{
				this.IdleBreakerVORPC(default(PhotonMessageInfo));
			}
			this.idleBreakerVOCooldown = Random.Range(15f, 45f);
		}
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x0002AFA4 File Offset: 0x000291A4
	[PunRPC]
	public void IdleBreakerVORPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (!this.enemySlowMouthAnim.enabled)
		{
			return;
		}
		if (!this.audioSourceVO.enabled)
		{
			return;
		}
		this.soundIdleBreakerVO.Play(this.centerTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x0002B004 File Offset: 0x00029204
	private void AttachToTarget()
	{
		float num = Vector3.Distance(base.transform.position, this.targetDestination);
		if (!this.VisionBlocked() && num < 2f)
		{
			this.UpdateState(EnemySlowMouth.State.Attached);
		}
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x0002B040 File Offset: 0x00029240
	private void FastMoving(bool _lookAtTarget)
	{
		if (_lookAtTarget)
		{
			this.followTarget.localRotation = Quaternion.LookRotation(this.currentTarget.position - this.centerTransform.position, Vector3.up);
		}
		else
		{
			this.LookAtVelocityDirection(false);
		}
		this.enemyRigidbody.OverrideFollowPosition(0.1f, 4f, 4f);
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x0002B0A3 File Offset: 0x000292A3
	private void OverrideHideEnemy()
	{
		this.enemyHiddenTimer = 0.2f;
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x0002B0B0 File Offset: 0x000292B0
	private bool VisionBlocked()
	{
		if (SemiFunc.FPSImpulse5())
		{
			Vector3 direction = this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.CenterTransform.position;
			this.visionPrevious = Physics.Raycast(this.enemy.CenterTransform.position, direction, direction.magnitude, LayerMask.GetMask(new string[]
			{
				"Default"
			}));
		}
		return this.visionPrevious;
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x0002B12B File Offset: 0x0002932B
	private void LeaveCheck()
	{
		if (SemiFunc.EnemyForceLeave(this.enemy) || this.targetedPlayerTime >= this.targetedPlayerTimeMax)
		{
			this.UpdateState(EnemySlowMouth.State.Leave);
		}
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x0002B14F File Offset: 0x0002934F
	private void TargettingPlayerStart()
	{
		this.aggroTimer = Random.Range(8f, 15f);
		this.looseTargetTimer = 0f;
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x0002B174 File Offset: 0x00029374
	private void TargettingPlayer()
	{
		this.StuckLogic();
		float num = this.currentTarget.position.y - base.transform.position.y;
		if (num < 0.1f)
		{
			num = 0.1f;
		}
		if (num > 2f)
		{
			num = 2f;
		}
		this.followTarget.localPosition = new Vector3(0f, num, 0f);
		this.targetedPlayerTime += Time.deltaTime;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			float num2 = 1f;
			if (this.VisionBlocked())
			{
				num2 = 4f;
			}
			this.aggroTimer -= Time.deltaTime * num2;
			if ((this.playerTarget && this.playerTarget.isDisabled) || !this.playerTarget || this.aggroTimer <= 0f)
			{
				this.UpdateState(EnemySlowMouth.State.Leave);
			}
			if (this.VisionBlocked())
			{
				this.looseTargetTimer += Time.deltaTime;
				if (this.looseTargetTimer > 3f)
				{
					this.UpdateState(EnemySlowMouth.State.Leave);
					this.looseTargetTimer = 0f;
				}
			}
			else
			{
				this.looseTargetTimer = 0f;
			}
		}
		this.AudioSourceSmoothStop();
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x0002B2AC File Offset: 0x000294AC
	public void TargetPositionLogic()
	{
		if (this.currentState != EnemySlowMouth.State.GoToPlayer && this.currentState != EnemySlowMouth.State.GoToPlayerOver && this.currentState != EnemySlowMouth.State.GoToPlayerUnder)
		{
			return;
		}
		if (!this.playerTarget)
		{
			return;
		}
		Vector3 b;
		if (this.currentState == EnemySlowMouth.State.GoToPlayer || this.currentState == EnemySlowMouth.State.GoToPlayerUnder || this.currentState == EnemySlowMouth.State.GoToPlayerOver)
		{
			b = this.playerTarget.transform.position + this.playerTarget.transform.forward * 1.5f;
		}
		else
		{
			b = this.playerTarget.transform.position + this.playerTarget.transform.forward * this.targetForwardOffset;
		}
		this.targetPosition = Vector3.Lerp(this.targetPosition, b, 20f * Time.deltaTime);
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x0002B384 File Offset: 0x00029584
	private void AudioSourceSmoothStop()
	{
		if (this.audioSourceVO.isPlaying)
		{
			this.audioSourceVO.volume = Mathf.Lerp(this.audioSourceVO.volume, 0f, Time.deltaTime * 40f);
			if (this.audioSourceVO.volume <= 0.01f)
			{
				this.audioSourceVO.Stop();
				this.audioSourceVO.volume = 1f;
				return;
			}
		}
		else if (this.audioSourceVO.volume < 1f)
		{
			this.audioSourceVO.volume = 1f;
		}
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x0002B41C File Offset: 0x0002961C
	private void LoopSounds()
	{
		bool playing = this.currentState == EnemySlowMouth.State.GoToPlayer || this.currentState == EnemySlowMouth.State.GoToPlayerOver || this.currentState == EnemySlowMouth.State.GoToPlayerUnder || this.currentState == EnemySlowMouth.State.MoveBackToNavmesh;
		this.soundChaseLoopVO.PlayLoop(playing, 2f, 2f, 1f);
		bool playing2 = this.currentState == EnemySlowMouth.State.Stun;
		this.soundStunLoopVO.PlayLoop(playing2, 2f, 2f, 1f);
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x0002B498 File Offset: 0x00029698
	private void AttachToPlayer()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!this.playerTarget)
		{
			return;
		}
		if (!this.currentTarget)
		{
			return;
		}
		if (Vector3.Distance(this.centerTransform.position, this.currentTarget.position) < 1.8f)
		{
			if (SemiFunc.FPSImpulse30())
			{
				if (this.movingLeft)
				{
					this.enemyRigidbody.rb.AddForce(-this.centerTransform.right * 5f, ForceMode.Force);
					if (this.moveThisDirectionTimer <= 0f)
					{
						this.movingLeft = false;
						this.movingRight = true;
						this.moveThisDirectionTimer = Random.Range(0.2f, 3f);
					}
				}
				if (this.movingRight)
				{
					this.enemyRigidbody.rb.AddForce(this.centerTransform.right * 5f, ForceMode.Force);
					if (this.moveThisDirectionTimer <= 0f)
					{
						this.movingRight = false;
						this.movingLeft = true;
						this.moveThisDirectionTimer = Random.Range(0.2f, 3f);
					}
				}
			}
			if (this.moveThisDirectionTimer > 0f)
			{
				this.moveThisDirectionTimer -= Time.deltaTime;
			}
			if (Random.Range(0, 2) == 0 && this.possessCooldown <= 0f && !this.IsPossessed())
			{
				this.UpdateState(EnemySlowMouth.State.Attack);
				return;
			}
			this.IdlePukeExecute();
			if (Random.Range(0, 3) == 0)
			{
				this.idlePukePreviousState = EnemySlowMouth.State.Leave;
			}
		}
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x0002B614 File Offset: 0x00029814
	public bool IsPossessed()
	{
		if (!this.playerTarget)
		{
			return true;
		}
		bool flag = this.playerTarget.playerAvatarVisuals.GetComponentInChildren<EnemySlowMouthPlayerAvatarAttached>();
		bool flag2 = this.playerTarget.localCameraTransform.GetComponentInChildren<EnemySlowMouthCameraVisuals>();
		return flag || flag2;
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x0002B664 File Offset: 0x00029864
	private void PlayerEffects()
	{
		if (this.playerTarget)
		{
			if (this.playerTarget.voiceChat)
			{
				this.playerTarget.voiceChat.OverridePitch(0.75f, 1f, 2f, 0.1f, 0f, 0f);
			}
			this.playerTarget.OverridePupilSize(2f, 4, 1f, 1f, 5f, 0.5f, 0.1f);
		}
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x0002B6E8 File Offset: 0x000298E8
	private void StuckLogic()
	{
		float num = Vector3.Distance(base.transform.position, this.stuckPosition);
		float num2 = 0.25f;
		if (this.currentState == EnemySlowMouth.State.GoToPlayer || this.currentState == EnemySlowMouth.State.GoToPlayerOver || this.currentState == EnemySlowMouth.State.GoToPlayerUnder)
		{
			num2 = 1.5f;
		}
		if (num > num2)
		{
			this.stuckPosition = base.transform.position;
			this.stuckTimer = 0f;
			this.randomNudgeTimer = 0f;
			return;
		}
		this.stuckTimer += Time.deltaTime;
		this.randomNudgeTimer += Time.deltaTime;
		if (this.randomNudgeTimer > 2.5f)
		{
			float force = Random.Range(4f, 10f);
			this.RandomNudge(force);
			this.randomNudgeTimer = 0f;
			if (this.currentState == EnemySlowMouth.State.GoToPlayer)
			{
				if (Random.Range(0, 2) == 0)
				{
					this.UpdateState(EnemySlowMouth.State.GoToPlayerUnder);
				}
				else
				{
					this.UpdateState(EnemySlowMouth.State.GoToPlayerOver);
				}
			}
		}
		if (this.stuckTimer > 5f)
		{
			this.UpdateState(EnemySlowMouth.State.IdlePuke);
			this.stuckTimer = 0f;
		}
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x0002B7F8 File Offset: 0x000299F8
	private void IsPossessedBySeveral()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (SemiFunc.FPSImpulse5())
		{
			if (!this.playerTarget)
			{
				return;
			}
			if (this.playerTarget.isDisabled)
			{
				this.UpdateState(EnemySlowMouth.State.Leave);
				return;
			}
			if (this.playerTarget.isLocal)
			{
				if (this.playerTarget.localCameraTransform.GetComponentsInChildren<EnemySlowMouthCameraVisuals>().Length > 1)
				{
					this.UpdateState(EnemySlowMouth.State.Leave);
					return;
				}
			}
			else if (this.playerTarget.playerAvatarVisuals.GetComponentsInChildren<EnemySlowMouthPlayerAvatarAttached>().Length > 1)
			{
				this.UpdateState(EnemySlowMouth.State.Leave);
			}
		}
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x0002B880 File Offset: 0x00029A80
	private void MoveBackPosition()
	{
		if (this.moveBackTimer <= 0f)
		{
			this.moveBackTimer = 0.1f;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(base.transform.position, out navMeshHit, 0.5f, -1) && Physics.Raycast(base.transform.position, Vector3.down, 2f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.moveBackPosition = navMeshHit.position;
			}
		}
	}

	// Token: 0x040006DF RID: 1759
	public GameObject enemySlowMouthAttack;

	// Token: 0x040006E0 RID: 1760
	public GameObject localCameraMouthPrefab;

	// Token: 0x040006E1 RID: 1761
	public GameObject enemySlowMouthOnPlayerTop;

	// Token: 0x040006E2 RID: 1762
	public GameObject enemySlowMouthOnPlayerBot;

	// Token: 0x040006E3 RID: 1763
	public Transform particles;

	// Token: 0x040006E4 RID: 1764
	public Transform tentacles;

	// Token: 0x040006E5 RID: 1765
	private List<SpringTentacle> springTentacles = new List<SpringTentacle>();

	// Token: 0x040006E6 RID: 1766
	private List<ParticleSystem> spawnParticles = new List<ParticleSystem>();

	// Token: 0x040006E7 RID: 1767
	private EnemySlowMouthCameraVisuals cameraVisuals;

	// Token: 0x040006E8 RID: 1768
	private bool movingRight;

	// Token: 0x040006E9 RID: 1769
	private bool movingLeft = true;

	// Token: 0x040006EA RID: 1770
	private float moveThisDirectionTimer;

	// Token: 0x040006EB RID: 1771
	private float looseTargetTimer;

	// Token: 0x040006EC RID: 1772
	private float looseTargetTime;

	// Token: 0x040006ED RID: 1773
	private float randomNudgeTimer;

	// Token: 0x040006EE RID: 1774
	[FormerlySerializedAs("state")]
	public EnemySlowMouth.State currentState;

	// Token: 0x040006EF RID: 1775
	private Vector3 agentDestination;

	// Token: 0x040006F0 RID: 1776
	public SemiPuke semiPuke;

	// Token: 0x040006F1 RID: 1777
	private EnemyVision enemyVision;

	// Token: 0x040006F2 RID: 1778
	public Transform mouthTransform;

	// Token: 0x040006F3 RID: 1779
	public Collider enemyCollider;

	// Token: 0x040006F4 RID: 1780
	public Transform enemyVisuals;

	// Token: 0x040006F5 RID: 1781
	private bool stateImpulse;

	// Token: 0x040006F6 RID: 1782
	private bool stateStartFixed;

	// Token: 0x040006F7 RID: 1783
	private float stateTimer;

	// Token: 0x040006F8 RID: 1784
	private PhotonView photonView;

	// Token: 0x040006F9 RID: 1785
	private PlayerAvatar attachTarget;

	// Token: 0x040006FA RID: 1786
	private Enemy enemy;

	// Token: 0x040006FB RID: 1787
	public EnemyRigidbody enemyRigidbody;

	// Token: 0x040006FC RID: 1788
	public PhysGrabObject physGrabObject;

	// Token: 0x040006FD RID: 1789
	public Transform followTarget;

	// Token: 0x040006FE RID: 1790
	public Vector3 followTargetStartPosition;

	// Token: 0x040006FF RID: 1791
	public Transform centerTransform;

	// Token: 0x04000700 RID: 1792
	public AudioSource audioSourceVO;

	// Token: 0x04000701 RID: 1793
	private float idleBreakerVOCooldown = 20f;

	// Token: 0x04000702 RID: 1794
	private float idlePukeCooldown = 20f;

	// Token: 0x04000703 RID: 1795
	private EnemySlowMouth.State idlePukePreviousState;

	// Token: 0x04000704 RID: 1796
	public EnemySlowMouthAnim enemySlowMouthAnim;

	// Token: 0x04000705 RID: 1797
	private Vector3 followPointPositionPrev;

	// Token: 0x04000706 RID: 1798
	private Transform currentTarget;

	// Token: 0x04000707 RID: 1799
	private SpringFloat spawnDespawnScaleSpring;

	// Token: 0x04000708 RID: 1800
	private Vector3 targetDestination;

	// Token: 0x04000709 RID: 1801
	private bool waitForTargettingLoop;

	// Token: 0x0400070A RID: 1802
	private float visionTimer;

	// Token: 0x0400070B RID: 1803
	private bool visionPrevious;

	// Token: 0x0400070C RID: 1804
	private PlayerAvatar playerTarget;

	// Token: 0x0400070D RID: 1805
	private float enemyHiddenTimer;

	// Token: 0x0400070E RID: 1806
	private Vector3 moveBackPosition;

	// Token: 0x0400070F RID: 1807
	private float targetForwardOffset = 1.5f;

	// Token: 0x04000710 RID: 1808
	private Vector3 targetPosition;

	// Token: 0x04000711 RID: 1809
	private float targetedPlayerTime;

	// Token: 0x04000712 RID: 1810
	private float targetedPlayerTimeMax = 10f;

	// Token: 0x04000713 RID: 1811
	private float moveBackTimer;

	// Token: 0x04000714 RID: 1812
	private Vector3 enemyGroundPosition;

	// Token: 0x04000715 RID: 1813
	internal Vector3 detachPosition;

	// Token: 0x04000716 RID: 1814
	internal Quaternion detachRotation;

	// Token: 0x04000717 RID: 1815
	private float attachedTimer;

	// Token: 0x04000718 RID: 1816
	private float possessCooldown;

	// Token: 0x04000719 RID: 1817
	private float stuckTimer;

	// Token: 0x0400071A RID: 1818
	private Vector3 stuckPosition;

	// Token: 0x0400071B RID: 1819
	private float aggroTimer;

	// Token: 0x0400071C RID: 1820
	public Sound soundSpawnVO;

	// Token: 0x0400071D RID: 1821
	public Sound soundIdleBreakerVO;

	// Token: 0x0400071E RID: 1822
	public Sound soundHurtVO;

	// Token: 0x0400071F RID: 1823
	public Sound soundDieVO;

	// Token: 0x04000720 RID: 1824
	public Sound soundNoticeVO;

	// Token: 0x04000721 RID: 1825
	public Sound soundChaseLoopVO;

	// Token: 0x04000722 RID: 1826
	public Sound soundDetachVO;

	// Token: 0x04000723 RID: 1827
	public Sound soundDetach;

	// Token: 0x04000724 RID: 1828
	public Sound soundStunLoopVO;

	// Token: 0x0200031A RID: 794
	public enum State
	{
		// Token: 0x040028F6 RID: 10486
		Spawn,
		// Token: 0x040028F7 RID: 10487
		Idle,
		// Token: 0x040028F8 RID: 10488
		Despawn,
		// Token: 0x040028F9 RID: 10489
		Roam,
		// Token: 0x040028FA RID: 10490
		Investigate,
		// Token: 0x040028FB RID: 10491
		Stun,
		// Token: 0x040028FC RID: 10492
		Leave,
		// Token: 0x040028FD RID: 10493
		Notice,
		// Token: 0x040028FE RID: 10494
		Attack,
		// Token: 0x040028FF RID: 10495
		Attached,
		// Token: 0x04002900 RID: 10496
		Puke,
		// Token: 0x04002901 RID: 10497
		Detach,
		// Token: 0x04002902 RID: 10498
		IdlePuke,
		// Token: 0x04002903 RID: 10499
		GoToPlayerOver,
		// Token: 0x04002904 RID: 10500
		GoToPlayerUnder,
		// Token: 0x04002905 RID: 10501
		MoveBackToNavmesh,
		// Token: 0x04002906 RID: 10502
		GoToPlayer,
		// Token: 0x04002907 RID: 10503
		Dead
	}
}
