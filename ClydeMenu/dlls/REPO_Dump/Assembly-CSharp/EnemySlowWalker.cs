using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200007F RID: 127
public class EnemySlowWalker : MonoBehaviour, IPunObservable
{
	// Token: 0x060004C9 RID: 1225 RVA: 0x0002F91A File Offset: 0x0002DB1A
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x0002F928 File Offset: 0x0002DB28
	private void Start()
	{
		this.visionDotStandingDefault = this.enemy.Vision.VisionDotStanding;
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x0002F940 File Offset: 0x0002DB40
	private void Update()
	{
		this.HeadLookAt();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.enemy.CurrentState == EnemyState.Despawn && !this.enemy.IsStunned())
		{
			this.UpdateState(EnemySlowWalker.State.Despawn);
		}
		if (this.enemy.IsStunned())
		{
			this.UpdateState(EnemySlowWalker.State.Stun);
		}
		switch (this.currentState)
		{
		case EnemySlowWalker.State.Spawn:
			this.StateSpawn();
			break;
		case EnemySlowWalker.State.Idle:
			this.StateIdle();
			break;
		case EnemySlowWalker.State.Roam:
			this.StateRoam();
			break;
		case EnemySlowWalker.State.Investigate:
			this.StateInvestigate();
			break;
		case EnemySlowWalker.State.Notice:
			this.StateNotice();
			break;
		case EnemySlowWalker.State.GoToPlayer:
			this.StateGoToPlayer();
			break;
		case EnemySlowWalker.State.Sneak:
			this.StateSneak();
			break;
		case EnemySlowWalker.State.Attack:
			this.StateAttack();
			break;
		case EnemySlowWalker.State.StuckAttack:
			this.StateStuckAttack();
			break;
		case EnemySlowWalker.State.LookUnderStart:
			this.StateLookUnderStart();
			break;
		case EnemySlowWalker.State.LookUnderIntro:
			this.StateLookUnderIntro();
			break;
		case EnemySlowWalker.State.LookUnder:
			this.StateLookUnder();
			break;
		case EnemySlowWalker.State.LookUnderAttack:
			this.StateLookUnderAttack();
			break;
		case EnemySlowWalker.State.LookUnderStop:
			this.StateLookUnderStop();
			break;
		case EnemySlowWalker.State.Stun:
			this.StateStun();
			break;
		case EnemySlowWalker.State.Leave:
			this.StateLeave();
			break;
		case EnemySlowWalker.State.Despawn:
			this.StateDespawn();
			break;
		}
		this.RotationLogic();
		this.TimerLogic();
		this.VisionDotLogic();
		this.AttackOffsetLogic();
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x0002FA88 File Offset: 0x0002DC88
	public void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0002FAD8 File Offset: 0x0002DCD8
	public void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemySlowWalker.State.Leave);
		}
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x0002FB70 File Offset: 0x0002DD70
	public void StateRoam()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 5f;
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGet(base.transform.position, 10f, 25f);
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointGet(base.transform.position, 0f, 999f);
			}
			NavMeshHit navMeshHit;
			if (levelPoint && NavMesh.SamplePosition(levelPoint.transform.position + Random.insideUnitSphere * 3f, out navMeshHit, 5f, -1) && Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.agentDestination = navMeshHit.position;
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateImpulse = false;
		}
		else
		{
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			if (this.enemy.Rigidbody.notMovingTimer > 3f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.stateTimer <= 0f)
			{
				this.AttackNearestPhysObjectOrGoToIdle();
				return;
			}
			if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
			{
				this.UpdateState(EnemySlowWalker.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemySlowWalker.State.Leave);
		}
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x0002FCF4 File Offset: 0x0002DEF4
	public void StateInvestigate()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 5f;
			this.enemy.Rigidbody.notMovingTimer = 0f;
		}
		else
		{
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.stateTimer <= 0f)
			{
				this.AttackNearestPhysObjectOrGoToIdle();
				return;
			}
			if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
			{
				this.UpdateState(EnemySlowWalker.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemySlowWalker.State.Leave);
		}
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x0002FDC0 File Offset: 0x0002DFC0
	public void StateNotice()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.GoToPlayer);
		}
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0002FE38 File Offset: 0x0002E038
	public void StateGoToPlayer()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
			return;
		}
		if (this.stateImpulse)
		{
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateImpulse = false;
			this.stateTimer = 10f;
		}
		this.enemy.NavMeshAgent.OverrideAgent(0.8f, 30f, 0.2f);
		this.targetPosition = this.targetPlayer.transform.position;
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
			return;
		}
		if (Vector3.Distance(this.feetTransform.position, this.enemy.NavMeshAgent.GetPoint()) < 8f && this.stateTimer > 1.5f && this.enemy.Jump.timeSinceJumped > 1f)
		{
			this.UpdateState(EnemySlowWalker.State.Attack);
			return;
		}
		if (SemiFunc.EnemyLookUnderCondition(this.enemy, this.stateTimer, 9f, this.targetPlayer))
		{
			this.UpdateState(EnemySlowWalker.State.LookUnderStart);
			return;
		}
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.AttackNearestPhysObjectOrGoToIdle();
		}
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x0002FF94 File Offset: 0x0002E194
	public void StateSneak()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
			return;
		}
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.enemy.NavMeshAgent.OverrideAgent(0.8f, 30f, 0.2f);
		this.targetPosition = this.targetPlayer.transform.position;
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
			return;
		}
		if (Vector3.Distance(this.feetTransform.position, this.enemy.NavMeshAgent.GetPoint()) < 5f)
		{
			this.UpdateState(EnemySlowWalker.State.GoToPlayer);
			return;
		}
		if (this.enemy.OnScreen.OnScreenAny)
		{
			this.UpdateState(EnemySlowWalker.State.Notice);
		}
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x000300CC File Offset: 0x0002E2CC
	public void StateAttack()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 5f;
			this.attackCount++;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x00030154 File Offset: 0x0002E354
	public void StateStuckAttack()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.stateTimer = 3f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.Stop(0.2f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x000301E4 File Offset: 0x0002E3E4
	public void StateLookUnderStart()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
			return;
		}
		if (this.stateImpulse)
		{
			this.lookUnderPosition = this.targetPlayer.transform.position;
			this.lookUnderLookAtPosition = this.lookUnderPosition;
			this.lookUnderPositionNavmesh = this.targetPlayer.LastNavmeshPosition;
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.SetDestination(this.lookUnderPositionNavmesh);
		if (Vector3.Distance(base.transform.position, this.lookUnderPositionNavmesh) < 0.5f)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemySlowWalker.State.LookUnderIntro);
				return;
			}
		}
		else if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x000302E4 File Offset: 0x0002E4E4
	public void StateLookUnderIntro()
	{
		if (this.stateImpulse)
		{
			this.lookUnderAttackCount = 0;
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.LookUnder);
		}
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0003033C File Offset: 0x0002E53C
	public void StateLookUnder()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 3f;
			return;
		}
		this.stateTimer -= Time.deltaTime;
		this.enemy.Vision.StandOverride(0.25f);
		int num = 10;
		if (this.stateTimer < 2.75f && this.targetPlayer.isCrawling && this.lookUnderAttackCount < num && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.targetPlayer.transform.position) < 3f && Vector3.Dot(this.lookAtTransform.forward, this.targetPlayer.transform.position - this.lookAtTransform.position) > 0.5f)
		{
			this.UpdateState(EnemySlowWalker.State.LookUnderAttack);
			return;
		}
		if (this.stateTimer <= 0f || this.lookUnderAttackCount >= num)
		{
			this.UpdateState(EnemySlowWalker.State.LookUnderStop);
		}
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x00030444 File Offset: 0x0002E644
	public void StateLookUnderAttack()
	{
		if (this.stateImpulse)
		{
			this.lookUnderAttackCount++;
			this.stateTimer = 0.6f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.targetPlayer.isDisabled)
			{
				this.UpdateState(EnemySlowWalker.State.LookUnderStop);
				return;
			}
			this.UpdateState(EnemySlowWalker.State.LookUnder);
		}
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x000304B8 File Offset: 0x0002E6B8
	public void StateLookUnderStop()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x00030508 File Offset: 0x0002E708
	public void StateStun()
	{
		if (this.stateImpulse)
		{
			if (!this.enemy.Rigidbody.grabbed)
			{
				this.enemy.Rigidbody.rb.AddTorque(-base.transform.right * 15f, ForceMode.Impulse);
			}
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = this.enemy.Rigidbody.transform.position;
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x000305B0 File Offset: 0x0002E7B0
	public void StateLeave()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 5f;
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, 30f, 50f, false);
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f);
			}
			NavMeshHit navMeshHit;
			if (levelPoint && NavMesh.SamplePosition(levelPoint.transform.position + Random.insideUnitSphere * 3f, out navMeshHit, 5f, -1) && Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.agentDestination = navMeshHit.position;
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			SemiFunc.EnemyLeaveStart(this.enemy);
			this.stateImpulse = false;
		}
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f || this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x00030705 File Offset: 0x0002E905
	public void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x00030741 File Offset: 0x0002E941
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemySlowWalker.State.Spawn);
		}
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x00030760 File Offset: 0x0002E960
	public void OnHurt()
	{
		this.animator.sfxHurt.Play(this.animator.transform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemySlowWalker.State.Leave)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x000307BC File Offset: 0x0002E9BC
	public void OnDeath()
	{
		this.particleDeathImpact.transform.position = this.enemy.CenterTransform.position;
		this.particleDeathImpact.Play();
		this.particleDeathBitsFar.transform.position = this.enemy.CenterTransform.position;
		this.particleDeathBitsFar.Play();
		this.particleDeathBitsShort.transform.position = this.enemy.CenterTransform.position;
		this.particleDeathBitsShort.Play();
		this.particleDeathSmoke.transform.position = this.enemy.CenterTransform.position;
		this.particleDeathSmoke.Play();
		this.animator.sfxDeath.Play(this.animator.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x00030920 File Offset: 0x0002EB20
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && (this.currentState == EnemySlowWalker.State.Idle || this.currentState == EnemySlowWalker.State.Roam || this.currentState == EnemySlowWalker.State.Investigate))
		{
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			this.UpdateState(EnemySlowWalker.State.Investigate);
		}
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x0003096C File Offset: 0x0002EB6C
	public void OnVision()
	{
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (this.currentState == EnemySlowWalker.State.Roam || this.currentState == EnemySlowWalker.State.Idle || this.currentState == EnemySlowWalker.State.Investigate || this.currentState == EnemySlowWalker.State.Leave)
		{
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			if (!this.enemy.OnScreen.OnScreenAny)
			{
				this.UpdateState(EnemySlowWalker.State.Sneak);
			}
			else
			{
				this.UpdateState(EnemySlowWalker.State.Notice);
			}
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
				return;
			}
		}
		else if (this.currentState == EnemySlowWalker.State.GoToPlayer || this.currentState == EnemySlowWalker.State.Sneak)
		{
			if (this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
			{
				this.stateTimer = 10f;
				return;
			}
		}
		else if (this.currentState == EnemySlowWalker.State.LookUnderStart)
		{
			if (this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer && !this.targetPlayer.isCrawling)
			{
				this.UpdateState(EnemySlowWalker.State.GoToPlayer);
				return;
			}
		}
		else if ((this.currentState == EnemySlowWalker.State.LookUnder || this.currentState == EnemySlowWalker.State.LookUnderIntro || this.currentState == EnemySlowWalker.State.LookUnderAttack) && this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
		{
			if (this.targetPlayer.isCrawling)
			{
				this.lookUnderLookAtPosition = this.targetPlayer.transform.position;
				return;
			}
			this.UpdateState(EnemySlowWalker.State.LookUnderStop);
		}
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x00030B04 File Offset: 0x0002ED04
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				return;
			}
			if (this.currentState == EnemySlowWalker.State.Leave)
			{
				this.grabAggroTimer = 60f;
				PlayerAvatar onGrabbedPlayerAvatar = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
				if (onGrabbedPlayerAvatar.transform.position.y - this.enemy.transform.position.y > 1.15f || onGrabbedPlayerAvatar.transform.position.y - this.enemy.transform.position.y < -1f)
				{
					return;
				}
				this.targetPlayer = onGrabbedPlayerAvatar;
				this.UpdateState(EnemySlowWalker.State.Notice);
			}
		}
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x00030BBB File Offset: 0x0002EDBB
	public void OnLookUnderAttackHurtPlayer()
	{
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("LookUnderAttackCountResetRPC", RpcTarget.MasterClient, Array.Empty<object>());
			return;
		}
		this.LookUnderAttackCountResetRPC();
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x00030BE4 File Offset: 0x0002EDE4
	private void UpdateState(EnemySlowWalker.State _state)
	{
		if (this.currentState == _state)
		{
			return;
		}
		this.enemy.Rigidbody.notMovingTimer = 0f;
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

	// Token: 0x060004E5 RID: 1253 RVA: 0x00030C6C File Offset: 0x0002EE6C
	private void RotationLogic()
	{
		if (this.currentState == EnemySlowWalker.State.Notice)
		{
			if (this.targetPlayer && Vector3.Distance(this.targetPlayer.transform.position, this.enemy.Rigidbody.transform.position) > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.targetPlayer.transform.position - this.enemy.Rigidbody.transform.position);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
		}
		else if (this.currentState == EnemySlowWalker.State.LookUnderStart || this.currentState == EnemySlowWalker.State.LookUnderIntro || this.currentState == EnemySlowWalker.State.LookUnder || this.currentState == EnemySlowWalker.State.LookUnderAttack)
		{
			if (Vector3.Distance(this.lookUnderPosition, base.transform.position) > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.lookUnderPosition - base.transform.position);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
		}
		else if (this.enemy.NavMeshAgent.AgentVelocity.normalized.magnitude > 0.1f)
		{
			this.rotationTarget = Quaternion.LookRotation(this.enemy.NavMeshAgent.AgentVelocity.normalized);
			this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
		}
		if (this.currentState == EnemySlowWalker.State.Attack)
		{
			if (this.targetPlayer && Vector3.Distance(this.targetPlayer.transform.position, this.enemy.Rigidbody.transform.position) > 0.1f && this.stateTimer > 2.5f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.targetPlayer.transform.position - this.enemy.Rigidbody.transform.position);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
			this.horizontalRotationSpring.speed = 15f;
			this.horizontalRotationSpring.damping = 0.8f;
		}
		else
		{
			this.horizontalRotationSpring.speed = 5f;
			this.horizontalRotationSpring.damping = 0.7f;
		}
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.horizontalRotationSpring, this.rotationTarget, -1f);
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x00030F4C File Offset: 0x0002F14C
	private void HeadLookAt()
	{
		if (this.currentState == EnemySlowWalker.State.LookUnder || this.currentState == EnemySlowWalker.State.LookUnderAttack)
		{
			Vector3 vector = this.lookUnderLookAtPosition - this.lookAtTransform.position;
			vector = SemiFunc.ClampDirection(vector, this.animator.transform.forward, 60f);
			Quaternion localRotation = this.lookAtTransform.localRotation;
			this.lookAtTransform.rotation = Quaternion.LookRotation(vector);
			Quaternion localRotation2 = this.lookAtTransform.localRotation;
			localRotation2.eulerAngles = new Vector3(0f, localRotation2.eulerAngles.y, 0f);
			this.lookAtTransform.localRotation = Quaternion.Lerp(localRotation, localRotation2, Time.deltaTime * 10f);
			this.enemy.Vision.VisionTransform.rotation = this.lookAtTransform.rotation;
		}
		else
		{
			this.lookAtTransform.localRotation = Quaternion.Lerp(this.lookAtTransform.localRotation, Quaternion.identity, Time.deltaTime * 10f);
			this.enemy.Vision.VisionTransform.localRotation = Quaternion.identity;
		}
		this.animator.SpringLogic();
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0003107D File Offset: 0x0002F27D
	private void VisionDotLogic()
	{
		if (this.currentState == EnemySlowWalker.State.LookUnder)
		{
			this.enemy.Vision.VisionDotStanding = 0f;
			return;
		}
		this.enemy.Vision.VisionDotStanding = this.visionDotStandingDefault;
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x000310B5 File Offset: 0x0002F2B5
	private void TimerLogic()
	{
		this.visionTimer -= Time.deltaTime;
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x000310CC File Offset: 0x0002F2CC
	private void AttackOffsetLogic()
	{
		if (this.currentState != EnemySlowWalker.State.Attack)
		{
			this.attackOffsetActive = false;
		}
		if (this.attackOffsetActive)
		{
			this.attackOffsetTransform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this.attackOffsetTransform.localPosition.z, 1.5f, Time.deltaTime * 4f));
			this.enemy.Rigidbody.OverrideFollowPosition(0.2f, 5f, 40f);
			return;
		}
		this.attackOffsetTransform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this.attackOffsetTransform.localPosition.z, 0f, Time.deltaTime * 1f));
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x00031190 File Offset: 0x0002F390
	private void AttackNearestPhysObjectOrGoToIdle()
	{
		this.stuckAttackTarget = Vector3.zero;
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.stuckAttackTarget = SemiFunc.EnemyGetNearestPhysObject(this.enemy);
		}
		if (this.stuckAttackTarget != Vector3.zero)
		{
			this.UpdateState(EnemySlowWalker.State.StuckAttack);
			return;
		}
		this.UpdateState(EnemySlowWalker.State.Idle);
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x000311F1 File Offset: 0x0002F3F1
	[PunRPC]
	private void UpdateStateRPC(EnemySlowWalker.State _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
		if (this.currentState == EnemySlowWalker.State.Spawn)
		{
			this.animator.OnSpawn();
		}
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x00031218 File Offset: 0x0002F418
	[PunRPC]
	private void TargetPlayerRPC(int _playerID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar.photonView.ViewID == _playerID)
			{
				this.targetPlayer = playerAvatar;
			}
		}
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x00031288 File Offset: 0x0002F488
	[PunRPC]
	private void LookUnderAttackCountResetRPC()
	{
		this.lookUnderAttackCount = 0;
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x00031291 File Offset: 0x0002F491
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.lookUnderLookAtPosition);
			return;
		}
		this.lookUnderLookAtPosition = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x040007BF RID: 1983
	public EnemySlowWalker.State currentState;

	// Token: 0x040007C0 RID: 1984
	public float stateTimer;

	// Token: 0x040007C1 RID: 1985
	public EnemySlowWalkerAnim animator;

	// Token: 0x040007C2 RID: 1986
	public ParticleSystem particleDeathImpact;

	// Token: 0x040007C3 RID: 1987
	public ParticleSystem particleDeathBitsFar;

	// Token: 0x040007C4 RID: 1988
	public ParticleSystem particleDeathBitsShort;

	// Token: 0x040007C5 RID: 1989
	public ParticleSystem particleDeathSmoke;

	// Token: 0x040007C6 RID: 1990
	public SpringQuaternion horizontalRotationSpring;

	// Token: 0x040007C7 RID: 1991
	private Quaternion rotationTarget;

	// Token: 0x040007C8 RID: 1992
	private bool stateImpulse = true;

	// Token: 0x040007C9 RID: 1993
	internal PlayerAvatar targetPlayer;

	// Token: 0x040007CA RID: 1994
	public Enemy enemy;

	// Token: 0x040007CB RID: 1995
	private PhotonView photonView;

	// Token: 0x040007CC RID: 1996
	private Vector3 agentDestination;

	// Token: 0x040007CD RID: 1997
	private Vector3 backToNavMeshPosition;

	// Token: 0x040007CE RID: 1998
	private Vector3 stuckAttackTarget;

	// Token: 0x040007CF RID: 1999
	private Vector3 targetPosition;

	// Token: 0x040007D0 RID: 2000
	private float visionTimer;

	// Token: 0x040007D1 RID: 2001
	private bool visionPrevious;

	// Token: 0x040007D2 RID: 2002
	public Transform feetTransform;

	// Token: 0x040007D3 RID: 2003
	private float grabAggroTimer;

	// Token: 0x040007D4 RID: 2004
	private int attackCount;

	// Token: 0x040007D5 RID: 2005
	private Vector3 lookUnderPosition;

	// Token: 0x040007D6 RID: 2006
	private Vector3 lookUnderLookAtPosition;

	// Token: 0x040007D7 RID: 2007
	private Vector3 lookUnderPositionNavmesh;

	// Token: 0x040007D8 RID: 2008
	internal int lookUnderAttackCount;

	// Token: 0x040007D9 RID: 2009
	public Transform lookAtTransform;

	// Token: 0x040007DA RID: 2010
	private float visionDotStandingDefault;

	// Token: 0x040007DB RID: 2011
	internal bool attackOffsetActive;

	// Token: 0x040007DC RID: 2012
	public Transform attackOffsetTransform;

	// Token: 0x02000320 RID: 800
	public enum State
	{
		// Token: 0x04002929 RID: 10537
		Spawn,
		// Token: 0x0400292A RID: 10538
		Idle,
		// Token: 0x0400292B RID: 10539
		Roam,
		// Token: 0x0400292C RID: 10540
		Investigate,
		// Token: 0x0400292D RID: 10541
		Notice,
		// Token: 0x0400292E RID: 10542
		GoToPlayer,
		// Token: 0x0400292F RID: 10543
		Sneak,
		// Token: 0x04002930 RID: 10544
		Attack,
		// Token: 0x04002931 RID: 10545
		StuckAttack,
		// Token: 0x04002932 RID: 10546
		LookUnderStart,
		// Token: 0x04002933 RID: 10547
		LookUnderIntro,
		// Token: 0x04002934 RID: 10548
		LookUnder,
		// Token: 0x04002935 RID: 10549
		LookUnderAttack,
		// Token: 0x04002936 RID: 10550
		LookUnderStop,
		// Token: 0x04002937 RID: 10551
		Stun,
		// Token: 0x04002938 RID: 10552
		Leave,
		// Token: 0x04002939 RID: 10553
		Despawn
	}
}
