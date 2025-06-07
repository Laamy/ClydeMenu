using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200006E RID: 110
public class EnemyRobe : MonoBehaviour
{
	// Token: 0x060003A8 RID: 936 RVA: 0x00024697 File Offset: 0x00022897
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
		this.idleBreakTimer = Random.Range(this.idleBreakTimeMin, this.idleBreakTimeMax);
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x000246C8 File Offset: 0x000228C8
	private void Update()
	{
		this.EndPieceLogic();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.idleBreakTimer >= 0f)
		{
			this.idleBreakTimer -= Time.deltaTime;
			if (this.idleBreakTimer <= 0f && this.CanIdleBreak())
			{
				this.IdleBreak();
				this.idleBreakTimer = Random.Range(this.idleBreakTimeMin, this.idleBreakTimeMax);
			}
		}
		this.RotationLogic();
		this.RigidbodyRotationSpeed();
		this.ChaseTimer();
		if (this.enemy.IsStunned())
		{
			this.UpdateState(EnemyRobe.State.Stun);
		}
		else if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			this.UpdateState(EnemyRobe.State.Despawn);
		}
		switch (this.currentState)
		{
		case EnemyRobe.State.Spawn:
			this.StateSpawn();
			break;
		case EnemyRobe.State.Idle:
			this.StateIdle();
			break;
		case EnemyRobe.State.Roam:
			this.StateRoam();
			break;
		case EnemyRobe.State.Investigate:
			this.StateInvestigate();
			break;
		case EnemyRobe.State.TargetPlayer:
			this.MoveTowardPlayer();
			this.StateTargetPlayer();
			break;
		case EnemyRobe.State.LookUnderStart:
			this.StateLookUnderStart();
			break;
		case EnemyRobe.State.LookUnder:
			this.StateLookUnder();
			break;
		case EnemyRobe.State.LookUnderAttack:
			this.StateLookUnderAttack();
			break;
		case EnemyRobe.State.LookUnderStop:
			this.StateLookUnderStop();
			break;
		case EnemyRobe.State.SeekPlayer:
			this.StateSeekPlayer();
			break;
		case EnemyRobe.State.Attack:
			this.StateAttack();
			break;
		case EnemyRobe.State.StuckAttack:
			this.StateStuckAttack();
			break;
		case EnemyRobe.State.Stun:
			this.StateStun();
			break;
		case EnemyRobe.State.Leave:
			this.StateLeave();
			break;
		case EnemyRobe.State.Despawn:
			this.StateDespawn();
			break;
		}
		if (this.currentState != EnemyRobe.State.TargetPlayer)
		{
			this.overrideAgentLerp = 0f;
		}
		if (this.currentState != EnemyRobe.State.TargetPlayer && this.isOnScreen)
		{
			this.isOnScreen = false;
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("UpdateOnScreenRPC", RpcTarget.Others, new object[]
				{
					this.isOnScreen
				});
			}
		}
		if (this.isOnScreen && this.targetPlayer && this.targetPlayer.isLocal)
		{
			SemiFunc.DoNotLookEffect(base.gameObject, true, true, true, true, true, true);
		}
	}

	// Token: 0x060003AA RID: 938 RVA: 0x000248C8 File Offset: 0x00022AC8
	private void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 3f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.Idle);
		}
	}

	// Token: 0x060003AB RID: 939 RVA: 0x00024918 File Offset: 0x00022B18
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = Random.Range(2f, 5f);
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyRobe.State.Leave);
		}
	}

	// Token: 0x060003AC RID: 940 RVA: 0x000249C4 File Offset: 0x00022BC4
	private void StateRoam()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
			this.stateTimer = 5f;
			LevelPoint levelPoint = SemiFunc.LevelPointGet(base.transform.position, 5f, 15f);
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
			}
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (this.enemy.Rigidbody.notMovingTimer > 1f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		if (this.stateTimer <= 0f)
		{
			this.AttackNearestPhysObjectOrGoToIdle();
			return;
		}
		if (Vector3.Distance(base.transform.position, this.agentDestination) < 2f)
		{
			this.UpdateState(EnemyRobe.State.Idle);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyRobe.State.Leave);
		}
	}

	// Token: 0x060003AD RID: 941 RVA: 0x00024B5C File Offset: 0x00022D5C
	private void StateInvestigate()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 5f;
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateImpulse = false;
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
			if (Vector3.Distance(base.transform.position, this.agentDestination) < 2f)
			{
				this.UpdateState(EnemyRobe.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyRobe.State.Leave);
		}
	}

	// Token: 0x060003AE RID: 942 RVA: 0x00024C28 File Offset: 0x00022E28
	private void StateTargetPlayer()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		this.enemy.Rigidbody.OverrideFollowPosition(0.2f, 5f, 30f);
		if (Vector3.Distance(this.enemy.CenterTransform.position, this.targetPlayer.transform.position) < 2f)
		{
			this.UpdateState(EnemyRobe.State.Attack);
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.SeekPlayer);
			return;
		}
		if (this.chaseTime >= 10f && this.stateTimer <= 1f)
		{
			this.UpdateState(EnemyRobe.State.Leave);
			return;
		}
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.enemy.Vision.DisableVision(2f);
			this.UpdateState(EnemyRobe.State.SeekPlayer);
		}
		if (SemiFunc.EnemyLookUnderCondition(this.enemy, this.stateTimer, 0.5f, this.targetPlayer))
		{
			this.UpdateState(EnemyRobe.State.LookUnderStart);
		}
	}

	// Token: 0x060003AF RID: 943 RVA: 0x00024D48 File Offset: 0x00022F48
	private void StateLookUnderStart()
	{
		if (this.stateImpulse)
		{
			this.lookUnderPosition = this.targetPlayer.transform.position;
			this.lookUnderPositionNavmesh = this.targetPlayer.LastNavmeshPosition;
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.OverrideAgent(3f, 10f, 0.2f);
		this.enemy.Rigidbody.OverrideFollowPosition(0.2f, 3f, -1f);
		this.enemy.NavMeshAgent.SetDestination(this.lookUnderPositionNavmesh);
		if (Vector3.Distance(base.transform.position, this.lookUnderPositionNavmesh) < 1f)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyRobe.State.LookUnder);
				return;
			}
		}
		else if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.UpdateState(EnemyRobe.State.SeekPlayer);
		}
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x00024E50 File Offset: 0x00023050
	private void StateLookUnder()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 5f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		this.enemy.Vision.StandOverride(0.25f);
		Vector3 b = new Vector3(this.enemy.Rigidbody.transform.position.x, 0f, this.enemy.Rigidbody.transform.position.z);
		if (Vector3.Dot((new Vector3(this.targetPlayer.transform.position.x, 0f, this.targetPlayer.transform.position.z) - b).normalized, this.enemy.Rigidbody.transform.forward) > 0.75f && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.targetPlayer.transform.position) < 2.5f)
		{
			this.UpdateState(EnemyRobe.State.LookUnderAttack);
			return;
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.LookUnderStop);
		}
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x00024F90 File Offset: 0x00023190
	private void StateLookUnderAttack()
	{
		if (this.stateImpulse)
		{
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("LookUnderAttackImpulseRPC", RpcTarget.All, Array.Empty<object>());
			}
			else
			{
				this.LookUnderAttackImpulseRPC(default(PhotonMessageInfo));
			}
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.targetPlayer.isDisabled)
			{
				this.UpdateState(EnemyRobe.State.LookUnderStop);
				return;
			}
			this.UpdateState(EnemyRobe.State.LookUnder);
		}
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x00025020 File Offset: 0x00023220
	private void StateLookUnderStop()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.SeekPlayer);
		}
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x00025070 File Offset: 0x00023270
	private void StateSeekPlayer()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 20f;
			this.stateImpulse = false;
			LevelPoint levelPointAhead = this.enemy.GetLevelPointAhead(this.targetPosition);
			if (levelPointAhead)
			{
				this.targetPosition = levelPointAhead.transform.position;
			}
			this.enemy.Rigidbody.notMovingTimer = 0f;
		}
		this.enemy.NavMeshAgent.OverrideAgent(3f, 3f, 0.2f);
		this.enemy.Rigidbody.OverrideFollowPosition(0.2f, 3f, -1f);
		if (Vector3.Distance(base.transform.position, this.targetPosition) < 2f)
		{
			LevelPoint levelPointAhead2 = this.enemy.GetLevelPointAhead(this.targetPosition);
			if (levelPointAhead2)
			{
				this.targetPosition = levelPointAhead2.transform.position;
			}
		}
		if (this.enemy.Rigidbody.notMovingTimer >= 3f)
		{
			this.AttackNearestPhysObjectOrGoToIdle();
			return;
		}
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f || this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.UpdateState(EnemyRobe.State.Roam);
		}
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x000251D0 File Offset: 0x000233D0
	private void StateAttack()
	{
		if (this.stateImpulse)
		{
			this.attackImpulse = true;
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("AttackImpulseRPC", RpcTarget.Others, Array.Empty<object>());
			}
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateTimer = 2f;
			this.stateImpulse = false;
			return;
		}
		this.enemy.NavMeshAgent.Stop(0.2f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.SeekPlayer);
		}
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x00025290 File Offset: 0x00023490
	private void StateStuckAttack()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateTimer = 1.5f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.Stop(0.2f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.Attack);
		}
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x00025328 File Offset: 0x00023528
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
			this.UpdateState(EnemyRobe.State.Idle);
		}
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x00025390 File Offset: 0x00023590
	private void StateLeave()
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
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f || this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.Idle);
		}
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x000254E8 File Offset: 0x000236E8
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
		}
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x0002553C File Offset: 0x0002373C
	private void IdleBreak()
	{
		if (!GameManager.Multiplayer())
		{
			this.IdleBreakRPC(default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("IdleBreakRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060003BA RID: 954 RVA: 0x00025578 File Offset: 0x00023778
	internal void UpdateState(EnemyRobe.State _state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
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

	// Token: 0x060003BB RID: 955 RVA: 0x000255F2 File Offset: 0x000237F2
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyRobe.State.Spawn);
		}
	}

	// Token: 0x060003BC RID: 956 RVA: 0x00025610 File Offset: 0x00023810
	public void OnHurt()
	{
		this.robeAnim.sfxHurt.Play(this.robeAnim.transform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemyRobe.State.Leave)
		{
			this.UpdateState(EnemyRobe.State.Idle);
		}
	}

	// Token: 0x060003BD RID: 957 RVA: 0x0002566C File Offset: 0x0002386C
	public void OnDeath()
	{
		this.robeAnim.DeathParticlesImpulse();
		this.robeAnim.SfxDeath();
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x060003BE RID: 958 RVA: 0x0002570C File Offset: 0x0002390C
	public void OnVision()
	{
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (this.currentState == EnemyRobe.State.Idle || this.currentState == EnemyRobe.State.Roam || this.currentState == EnemyRobe.State.Investigate || this.currentState == EnemyRobe.State.SeekPlayer || this.currentState == EnemyRobe.State.Leave)
		{
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			this.UpdateState(EnemyRobe.State.TargetPlayer);
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
				return;
			}
		}
		else if (this.currentState == EnemyRobe.State.TargetPlayer)
		{
			if (this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
			{
				this.stateTimer = Mathf.Max(this.stateTimer, 1f);
				return;
			}
		}
		else if (this.currentState == EnemyRobe.State.LookUnderStart)
		{
			if (this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer && !this.targetPlayer.isCrawling)
			{
				this.UpdateState(EnemyRobe.State.TargetPlayer);
				return;
			}
		}
		else if (this.currentState == EnemyRobe.State.LookUnder && this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
		{
			if (this.targetPlayer.isCrawling)
			{
				this.lookUnderPosition = this.targetPlayer.transform.position;
				return;
			}
			this.UpdateState(EnemyRobe.State.LookUnderStop);
		}
	}

	// Token: 0x060003BF RID: 959 RVA: 0x0002587C File Offset: 0x00023A7C
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.currentState == EnemyRobe.State.Idle || this.currentState == EnemyRobe.State.Roam || this.currentState == EnemyRobe.State.Investigate)
			{
				this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
				this.UpdateState(EnemyRobe.State.Investigate);
				return;
			}
			if (this.currentState == EnemyRobe.State.SeekPlayer)
			{
				this.targetPosition = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			}
		}
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x000258EC File Offset: 0x00023AEC
	public void OnGrabbed()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.grabAggroTimer > 0f)
		{
			return;
		}
		if (this.currentState == EnemyRobe.State.Leave)
		{
			this.grabAggroTimer = 60f;
			this.targetPlayer = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
			this.UpdateState(EnemyRobe.State.TargetPlayer);
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x00025977 File Offset: 0x00023B77
	private bool CanIdleBreak()
	{
		return this.currentState == EnemyRobe.State.Idle || this.currentState == EnemyRobe.State.Investigate || this.currentState == EnemyRobe.State.Roam;
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x00025998 File Offset: 0x00023B98
	private void MoveTowardPlayer()
	{
		bool flag = false;
		if (this.enemy.OnScreen.GetOnScreen(this.targetPlayer))
		{
			flag = true;
			this.overrideAgentLerp += Time.deltaTime / 4f;
		}
		else
		{
			this.overrideAgentLerp -= Time.deltaTime / 0.01f;
		}
		if (flag != this.isOnScreen)
		{
			this.isOnScreen = flag;
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("UpdateOnScreenRPC", RpcTarget.Others, new object[]
				{
					this.isOnScreen
				});
			}
		}
		this.overrideAgentLerp = Mathf.Clamp(this.overrideAgentLerp, 0f, 1f);
		float b = 25f;
		float b2 = 25f;
		float speed = Mathf.Lerp(this.enemy.NavMeshAgent.DefaultSpeed, b, this.overrideAgentLerp);
		float speed2 = Mathf.Lerp(this.enemy.Rigidbody.positionSpeedChase, b2, this.overrideAgentLerp);
		this.enemy.NavMeshAgent.OverrideAgent(speed, this.enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		this.enemy.Rigidbody.OverrideFollowPosition(1f, speed2, -1f);
		this.targetPosition = this.targetPlayer.transform.position;
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x00025B00 File Offset: 0x00023D00
	private void RotationLogic()
	{
		if (this.currentState == EnemyRobe.State.StuckAttack)
		{
			if (Vector3.Distance(this.stuckAttackTarget, this.enemy.Rigidbody.transform.position) > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.stuckAttackTarget - this.enemy.Rigidbody.transform.position);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
		}
		else if (this.currentState == EnemyRobe.State.LookUnderStart || this.currentState == EnemyRobe.State.LookUnder || this.currentState == EnemyRobe.State.LookUnderAttack)
		{
			if (Vector3.Distance(this.lookUnderPosition, base.transform.position) > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.lookUnderPosition - base.transform.position);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
		}
		else if (this.currentState == EnemyRobe.State.TargetPlayer || this.currentState == EnemyRobe.State.Attack)
		{
			if (this.targetPlayer && Vector3.Distance(this.targetPlayer.transform.position, base.transform.position) > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.targetPlayer.transform.position - base.transform.position);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
		}
		else if (this.enemy.NavMeshAgent.AgentVelocity.normalized.magnitude > 0.1f)
		{
			this.rotationTarget = Quaternion.LookRotation(this.enemy.NavMeshAgent.AgentVelocity.normalized);
			this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
		}
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.rotationSpring, this.rotationTarget, -1f);
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x00025D60 File Offset: 0x00023F60
	private void EndPieceLogic()
	{
		this.endPieceSource.rotation = SemiFunc.SpringQuaternionGet(this.endPieceSpring, this.endPieceTarget.rotation, -1f);
		this.endPieceTarget.localEulerAngles = new Vector3(-this.enemy.Rigidbody.physGrabObject.rbVelocity.y * 30f, 0f, 0f);
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x00025DD0 File Offset: 0x00023FD0
	private void AttackNearestPhysObjectOrGoToIdle()
	{
		this.stuckAttackTarget = Vector3.zero;
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.stuckAttackTarget = SemiFunc.EnemyGetNearestPhysObject(this.enemy);
		}
		if (this.stuckAttackTarget != Vector3.zero)
		{
			this.UpdateState(EnemyRobe.State.StuckAttack);
			return;
		}
		this.UpdateState(EnemyRobe.State.Idle);
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x00025E34 File Offset: 0x00024034
	private void RigidbodyRotationSpeed()
	{
		if (this.currentState == EnemyRobe.State.Roam)
		{
			this.enemy.Rigidbody.rotationSpeedIdle = 1f;
			this.enemy.Rigidbody.rotationSpeedChase = 1f;
			return;
		}
		this.enemy.Rigidbody.rotationSpeedIdle = 2f;
		this.enemy.Rigidbody.rotationSpeedChase = 2f;
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x00025EA0 File Offset: 0x000240A0
	private void ChaseTimer()
	{
		if (this.currentState == EnemyRobe.State.TargetPlayer)
		{
			this.chaseTimer = 3f;
		}
		if (this.chaseTimer > 0f)
		{
			if (this.previousTargetNavmeshPosition != this.targetPlayer.LastNavmeshPosition)
			{
				this.previousTargetNavmeshPosition = this.targetPlayer.LastNavmeshPosition;
				this.chaseTime = 0f;
			}
			this.chaseTime += Time.deltaTime;
			this.chaseTimer -= Time.deltaTime;
			return;
		}
		this.chaseTime = 0f;
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x00025F32 File Offset: 0x00024132
	[PunRPC]
	private void UpdateStateRPC(EnemyRobe.State _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
		this.stateImpulse = true;
		if (this.currentState == EnemyRobe.State.Spawn)
		{
			this.robeAnim.SetSpawn();
		}
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x00025F60 File Offset: 0x00024160
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

	// Token: 0x060003CA RID: 970 RVA: 0x00025FD0 File Offset: 0x000241D0
	[PunRPC]
	private void UpdateOnScreenRPC(bool _onScreen, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.isOnScreen = _onScreen;
	}

	// Token: 0x060003CB RID: 971 RVA: 0x00025FE2 File Offset: 0x000241E2
	[PunRPC]
	private void AttackImpulseRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.attackImpulse = true;
	}

	// Token: 0x060003CC RID: 972 RVA: 0x00025FF4 File Offset: 0x000241F4
	[PunRPC]
	private void LookUnderAttackImpulseRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.lookUnderAttackImpulse = true;
	}

	// Token: 0x060003CD RID: 973 RVA: 0x00026006 File Offset: 0x00024206
	[PunRPC]
	private void IdleBreakRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.idleBreakTrigger = true;
	}

	// Token: 0x04000659 RID: 1625
	[Header("References")]
	public EnemyRobeAnim robeAnim;

	// Token: 0x0400065A RID: 1626
	internal Enemy enemy;

	// Token: 0x0400065B RID: 1627
	public EnemyRobe.State currentState;

	// Token: 0x0400065C RID: 1628
	private bool stateImpulse;

	// Token: 0x0400065D RID: 1629
	private float stateTimer;

	// Token: 0x0400065E RID: 1630
	internal PlayerAvatar targetPlayer;

	// Token: 0x0400065F RID: 1631
	private PhotonView photonView;

	// Token: 0x04000660 RID: 1632
	private float roamWaitTimer;

	// Token: 0x04000661 RID: 1633
	private Vector3 agentDestination;

	// Token: 0x04000662 RID: 1634
	private float overrideAgentLerp;

	// Token: 0x04000663 RID: 1635
	private Vector3 targetPosition;

	// Token: 0x04000664 RID: 1636
	public Transform eyeLocation;

	// Token: 0x04000665 RID: 1637
	internal bool isOnScreen;

	// Token: 0x04000666 RID: 1638
	internal bool attackImpulse;

	// Token: 0x04000667 RID: 1639
	[Header("Idle Break")]
	public float idleBreakTimeMin = 45f;

	// Token: 0x04000668 RID: 1640
	public float idleBreakTimeMax = 90f;

	// Token: 0x04000669 RID: 1641
	private float idleBreakTimer;

	// Token: 0x0400066A RID: 1642
	internal bool idleBreakTrigger;

	// Token: 0x0400066B RID: 1643
	[Space]
	public SpringQuaternion rotationSpring;

	// Token: 0x0400066C RID: 1644
	private Quaternion rotationTarget;

	// Token: 0x0400066D RID: 1645
	[Space]
	public SpringQuaternion endPieceSpring;

	// Token: 0x0400066E RID: 1646
	public Transform endPieceSource;

	// Token: 0x0400066F RID: 1647
	public Transform endPieceTarget;

	// Token: 0x04000670 RID: 1648
	private float grabAggroTimer;

	// Token: 0x04000671 RID: 1649
	private Vector3 lookUnderPositionNavmesh;

	// Token: 0x04000672 RID: 1650
	private Vector3 lookUnderPosition;

	// Token: 0x04000673 RID: 1651
	internal bool lookUnderAttackImpulse;

	// Token: 0x04000674 RID: 1652
	private Vector3 stuckAttackTarget;

	// Token: 0x04000675 RID: 1653
	private float chaseTime;

	// Token: 0x04000676 RID: 1654
	private float chaseTimer;

	// Token: 0x04000677 RID: 1655
	private Vector3 previousTargetNavmeshPosition;

	// Token: 0x02000318 RID: 792
	public enum State
	{
		// Token: 0x040028D3 RID: 10451
		Spawn,
		// Token: 0x040028D4 RID: 10452
		Idle,
		// Token: 0x040028D5 RID: 10453
		Roam,
		// Token: 0x040028D6 RID: 10454
		Investigate,
		// Token: 0x040028D7 RID: 10455
		TargetPlayer,
		// Token: 0x040028D8 RID: 10456
		LookUnderStart,
		// Token: 0x040028D9 RID: 10457
		LookUnder,
		// Token: 0x040028DA RID: 10458
		LookUnderAttack,
		// Token: 0x040028DB RID: 10459
		LookUnderStop,
		// Token: 0x040028DC RID: 10460
		SeekPlayer,
		// Token: 0x040028DD RID: 10461
		Attack,
		// Token: 0x040028DE RID: 10462
		StuckAttack,
		// Token: 0x040028DF RID: 10463
		Stun,
		// Token: 0x040028E0 RID: 10464
		Leave,
		// Token: 0x040028E1 RID: 10465
		Despawn
	}
}
