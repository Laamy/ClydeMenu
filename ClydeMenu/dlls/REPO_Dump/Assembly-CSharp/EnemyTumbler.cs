using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000084 RID: 132
public class EnemyTumbler : MonoBehaviour
{
	// Token: 0x0600053F RID: 1343 RVA: 0x00033B0E File Offset: 0x00031D0E
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x00033B1C File Offset: 0x00031D1C
	private void Update()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.HopLogic();
			this.RotationLogic();
			if (this.visionTimer > 0f)
			{
				this.visionTimer -= Time.deltaTime;
			}
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyTumbler.State.Stunned);
			}
			else if (this.enemy.CurrentState == EnemyState.Despawn)
			{
				this.UpdateState(EnemyTumbler.State.Despawn);
			}
			switch (this.currentState)
			{
			case EnemyTumbler.State.Spawn:
				this.StateSpawn();
				return;
			case EnemyTumbler.State.Idle:
				this.StateIdle();
				return;
			case EnemyTumbler.State.Roam:
				this.StateRoam();
				return;
			case EnemyTumbler.State.Notice:
				this.StateNotice();
				return;
			case EnemyTumbler.State.Investigate:
				this.StateInvestigate();
				return;
			case EnemyTumbler.State.MoveToPlayer:
				this.StateMoveToPlayer();
				return;
			case EnemyTumbler.State.Tell:
				this.StateTell();
				return;
			case EnemyTumbler.State.Tumble:
				this.StateTumble();
				return;
			case EnemyTumbler.State.TumbleEnd:
				this.StateTumbleEnd();
				return;
			case EnemyTumbler.State.BackToNavmesh:
				this.StateBackToNavmesh();
				return;
			case EnemyTumbler.State.Leave:
				this.StateLeave();
				return;
			case EnemyTumbler.State.Stunned:
				this.StateStunned();
				return;
			case EnemyTumbler.State.Dead:
				this.StateDead();
				return;
			case EnemyTumbler.State.Despawn:
				this.StateDespawn();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x00033C34 File Offset: 0x00031E34
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
			this.UpdateState(EnemyTumbler.State.Idle);
		}
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x00033C84 File Offset: 0x00031E84
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
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
			this.UpdateState(EnemyTumbler.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyTumbler.State.Leave);
		}
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x00033D24 File Offset: 0x00031F24
	private void StateRoam()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = Random.Range(3f, 8f);
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
			if (!this.enemy.Jump.jumping && (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f))
			{
				this.UpdateState(EnemyTumbler.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyTumbler.State.Leave);
		}
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x00033EC0 File Offset: 0x000320C0
	private void StateNotice()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 1f;
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyTumbler.State.MoveToPlayer);
		}
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x00033F44 File Offset: 0x00032144
	private void StateInvestigate()
	{
		if (this.stateImpulse)
		{
			if (!this.enemy.Jump.jumping)
			{
				this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
				this.enemy.NavMeshAgent.ResetPath();
			}
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			this.stateTimer = 5f;
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateImpulse = false;
		}
		else
		{
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			SemiFunc.EnemyCartJump(this.enemy);
			if (this.enemy.Rigidbody.notMovingTimer > 3f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (!this.enemy.Jump.jumping && (this.stateTimer <= 0f || Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetDestination()) < 1f))
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyTumbler.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyTumbler.State.Leave);
		}
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x000340A8 File Offset: 0x000322A8
	private void StateMoveToPlayer()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.agentDestination = this.targetPlayer.transform.position;
		if (this.enemy.Grounded.grounded)
		{
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		}
		this.stateTimer -= Time.deltaTime;
		Vector3 position = this.targetPlayer.transform.position;
		position.y = this.enemy.Rigidbody.transform.position.y;
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, position) < 7f && !this.enemy.Jump.jumping && !this.VisionBlocked())
		{
			this.UpdateState(EnemyTumbler.State.Tell);
			return;
		}
		if (this.stateTimer <= 0f || this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.UpdateState(EnemyTumbler.State.Idle);
		}
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x000341C0 File Offset: 0x000323C0
	private void StateTell()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyTumbler.State.Tumble);
		}
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x00034244 File Offset: 0x00032444
	private void StateTumble()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			Vector3 normalized = Vector3.Lerp(this.targetPlayer.transform.position - this.enemy.Rigidbody.transform.position, Vector3.up, 0.6f).normalized;
			this.enemy.Rigidbody.rb.AddForce(normalized * 40f, ForceMode.Impulse);
			this.enemy.Rigidbody.rb.AddTorque(this.enemy.Rigidbody.transform.right * 8f, ForceMode.Impulse);
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		this.enemy.Rigidbody.DisableFollowPosition(0.2f, 10f);
		this.enemy.Rigidbody.DisableFollowRotation(0.2f, 10f);
		if (this.enemy.Rigidbody.rb.velocity.magnitude < 1f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		base.transform.position = this.enemy.Rigidbody.transform.position;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit, 1f, -1))
		{
			this.backToNavmeshPosition = navMeshHit.position;
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyTumbler.State.TumbleEnd);
		}
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x00034420 File Offset: 0x00032620
	private void StateTumbleEnd()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
			base.transform.position = this.enemy.Rigidbody.transform.position;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyTumbler.State.BackToNavmesh);
		}
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x000344A4 File Offset: 0x000326A4
	private void StateBackToNavmesh()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			base.transform.position = this.enemy.Rigidbody.transform.position;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit, 1f, -1))
		{
			this.enemy.NavMeshAgent.Warp(navMeshHit.position);
			this.UpdateState(EnemyTumbler.State.Idle);
		}
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x00034538 File Offset: 0x00032738
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
			this.stateImpulse = false;
			SemiFunc.EnemyLeaveStart(this.enemy);
		}
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f || this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyTumbler.State.Idle);
		}
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x00034690 File Offset: 0x00032890
	private void StateStunned()
	{
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit, 1f, -1))
		{
			this.backToNavmeshPosition = navMeshHit.position;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = this.enemy.Rigidbody.transform.position;
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyTumbler.State.BackToNavmesh);
		}
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x00034718 File Offset: 0x00032918
	private void StateDead()
	{
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x0003471C File Offset: 0x0003291C
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0003476D File Offset: 0x0003296D
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyTumbler.State.Spawn);
		}
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0003478C File Offset: 0x0003298C
	public void OnHurt()
	{
		this.enemyTumblerAnim.sfxHurt.Play(base.transform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemyTumbler.State.Leave)
		{
			this.UpdateState(EnemyTumbler.State.Idle);
		}
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x000347E4 File Offset: 0x000329E4
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
		this.particleDeathHat.transform.position = this.enemy.CenterTransform.position;
		this.particleDeathHat.Play();
		this.enemyTumblerAnim.sfxDeath.Play(this.enemyTumblerAnim.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x00034970 File Offset: 0x00032B70
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && (this.currentState == EnemyTumbler.State.Idle || this.currentState == EnemyTumbler.State.Roam || this.currentState == EnemyTumbler.State.Investigate))
		{
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			this.UpdateState(EnemyTumbler.State.Investigate);
		}
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x000349BC File Offset: 0x00032BBC
	public void OnVision()
	{
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (this.currentState == EnemyTumbler.State.Roam || this.currentState == EnemyTumbler.State.Idle || this.currentState == EnemyTumbler.State.Investigate || this.currentState == EnemyTumbler.State.Leave)
		{
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			this.UpdateState(EnemyTumbler.State.Notice);
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
				return;
			}
		}
		else if (this.currentState == EnemyTumbler.State.MoveToPlayer && this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
		{
			this.stateTimer = 2f;
		}
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x00034A84 File Offset: 0x00032C84
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
		if (this.currentState == EnemyTumbler.State.Leave)
		{
			this.grabAggroTimer = 60f;
			this.targetPlayer = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
			this.UpdateState(EnemyTumbler.State.Notice);
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x00034B10 File Offset: 0x00032D10
	public void OnHurtColliderImpactAny()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("OnHurtColliderImpactAnyRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
			this.OnHurtColliderImpactAnyRPC(default(PhotonMessageInfo));
		}
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x00034B54 File Offset: 0x00032D54
	public void OnHurtColliderImpactPlayer()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.OnHurtColliderImpactPlayerRPC(this.hurtCollider.onImpactPlayerAvatar.photonView.ViewID, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("OnHurtColliderImpactPlayerRPC", RpcTarget.All, new object[]
		{
			this.hurtCollider.onImpactPlayerAvatar.photonView.ViewID
		});
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x00034BC4 File Offset: 0x00032DC4
	private void UpdateState(EnemyTumbler.State _state)
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

	// Token: 0x06000558 RID: 1368 RVA: 0x00034C4C File Offset: 0x00032E4C
	private void RotationLogic()
	{
		if ((this.currentState == EnemyTumbler.State.Notice || this.currentState == EnemyTumbler.State.MoveToPlayer || this.currentState == EnemyTumbler.State.Tell) && !this.VisionBlocked())
		{
			Quaternion quaternion = Quaternion.Euler(0f, Quaternion.LookRotation(this.targetPlayer.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
			this.mainMeshTargetRotation = quaternion;
		}
		else
		{
			Vector3 agentVelocity = this.enemy.NavMeshAgent.AgentVelocity;
			agentVelocity.y = 0f;
			if (agentVelocity.magnitude > 1f)
			{
				this.mainMeshTargetRotation = Quaternion.Euler(0f, Quaternion.LookRotation(this.enemy.Rigidbody.rb.velocity.normalized).eulerAngles.y, 0f);
			}
		}
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.mainMeshSpring, this.mainMeshTargetRotation, -1f);
		this.headTargetCodeTransform.localEulerAngles = new Vector3(this.enemy.Rigidbody.rb.velocity.y * 5f, 0f, 0f);
		this.headTransform.rotation = SemiFunc.SpringQuaternionGet(this.headSpring, this.headTargetTransform.rotation, -1f);
		this.hatTransform.rotation = SemiFunc.SpringQuaternionGet(this.hatSpring, this.hatTargetTransform.rotation, -1f);
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x00034DEC File Offset: 0x00032FEC
	private bool VisionBlocked()
	{
		if (this.visionTimer <= 0f)
		{
			this.visionTimer = 0.1f;
			Vector3 direction = this.targetPlayer.PlayerVisionTarget.VisionTransform.position - this.enemy.Vision.VisionTransform.position;
			this.visionPrevious = Physics.Raycast(this.enemy.Vision.VisionTransform.position, direction, direction.magnitude, LayerMask.GetMask(new string[]
			{
				"Default"
			}));
		}
		return this.visionPrevious;
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x00034E84 File Offset: 0x00033084
	private void HopLogic()
	{
		bool flag = this.currentState == EnemyTumbler.State.BackToNavmesh;
		if (this.currentState == EnemyTumbler.State.Roam || this.currentState == EnemyTumbler.State.Investigate || this.currentState == EnemyTumbler.State.MoveToPlayer || this.currentState == EnemyTumbler.State.Leave || flag)
		{
			float d = 1f;
			if (this.currentState == EnemyTumbler.State.MoveToPlayer)
			{
				d = 2f;
			}
			if (this.enemy.Grounded.grounded && !this.enemy.Jump.jumping)
			{
				this.enemy.NavMeshAgent.Stop(0.1f);
				if (this.groundedPrevious != this.enemy.Grounded.grounded)
				{
					if (flag)
					{
						base.transform.position = this.enemy.Rigidbody.transform.position;
					}
					else
					{
						this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
					}
				}
			}
			if (this.hopMoveTimer <= 0f)
			{
				Vector3 steeringTarget = this.enemy.NavMeshAgent.Agent.steeringTarget;
				Vector3 normalized = (this.enemy.NavMeshAgent.Agent.steeringTarget - this.enemy.Rigidbody.physGrabObject.centerPoint).normalized;
				steeringTarget.y = this.enemy.Rigidbody.physGrabObject.centerPoint.y;
				Vector3 normalized2 = (steeringTarget - this.enemy.Rigidbody.physGrabObject.centerPoint).normalized;
				bool flag2 = false;
				bool flag3 = false;
				int num = 10;
				float d2 = 0.5f;
				float maxDistance = 2f;
				if (!flag)
				{
					Vector3 vector = this.enemy.Rigidbody.physGrabObject.centerPoint + normalized2 * d2;
					bool flag4 = false;
					for (int i = 0; i < num; i++)
					{
						if (Physics.Raycast(vector, Vector3.down, maxDistance, SemiFunc.LayerMaskGetVisionObstruct()))
						{
							if (flag4)
							{
								flag2 = true;
							}
						}
						else
						{
							if (i < 3)
							{
								flag4 = true;
							}
							flag3 = true;
						}
						vector += normalized2 * d2;
					}
					this.enemy.NavMeshAgent.Stop(0f);
				}
				if (flag2)
				{
					this.enemy.Rigidbody.rb.AddForce(Vector3.up * 30f + normalized * 20f, ForceMode.Impulse);
					this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.physGrabObject.centerPoint + normalized * 5f);
					this.hopMoveTimer = 2.25f;
				}
				else if (!flag && Vector3.Distance(base.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 1f)
				{
					this.enemy.Rigidbody.rb.AddForce(Vector3.up * 20f, ForceMode.Impulse);
					this.enemy.NavMeshAgent.Warp(this.enemy.NavMeshAgent.GetPoint());
					this.hopMoveTimer = 0.75f;
				}
				else if (flag3)
				{
					this.enemy.Rigidbody.rb.AddForce(Vector3.up * 20f, ForceMode.Impulse);
					this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.physGrabObject.centerPoint + normalized * 0.5f);
					this.hopMoveTimer = 0.75f;
				}
				else
				{
					this.enemy.Rigidbody.rb.AddForce(Vector3.up * 25f + normalized2 * 10f, ForceMode.Impulse);
					if (flag)
					{
						base.transform.position = Vector3.MoveTowards(base.transform.position, this.backToNavmeshPosition, 2f);
					}
					else
					{
						this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.physGrabObject.centerPoint + normalized * d);
					}
					this.hopMoveTimer = 1.25f;
				}
				this.enemy.Jump.JumpingSet(true);
				this.enemy.Rigidbody.WarpDisable(2f);
				this.enemy.Grounded.GroundedDisable(0.25f);
			}
			else
			{
				this.hopMoveTimer -= Time.deltaTime;
			}
		}
		this.groundedPrevious = this.enemy.Grounded.grounded;
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x0003533D File Offset: 0x0003353D
	[PunRPC]
	private void UpdateStateRPC(EnemyTumbler.State _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
		if (this.currentState == EnemyTumbler.State.Spawn)
		{
			this.enemyTumblerAnim.OnSpawn();
		}
		if (this.currentState == EnemyTumbler.State.Tumble)
		{
			this.enemyTumblerAnim.OnTumble();
		}
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x00035378 File Offset: 0x00033578
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

	// Token: 0x0600055D RID: 1373 RVA: 0x000353E8 File Offset: 0x000335E8
	[PunRPC]
	private void OnHurtColliderImpactAnyRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.enemyTumblerAnim.SfxOnHurtColliderImpactAny();
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x00035400 File Offset: 0x00033600
	[PunRPC]
	private void OnHurtColliderImpactPlayerRPC(int _playerID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
		{
			if (playerAvatar.photonView.ViewID == _playerID)
			{
				playerAvatar.tumble.OverrideEnemyHurt(3f);
				break;
			}
		}
	}

	// Token: 0x0400087A RID: 2170
	public bool debugSpawn;

	// Token: 0x0400087B RID: 2171
	public EnemyTumbler.State currentState;

	// Token: 0x0400087C RID: 2172
	private bool stateImpulse;

	// Token: 0x0400087D RID: 2173
	private float stateTimer;

	// Token: 0x0400087E RID: 2174
	internal PlayerAvatar targetPlayer;

	// Token: 0x0400087F RID: 2175
	public Enemy enemy;

	// Token: 0x04000880 RID: 2176
	public EnemyTumblerAnim enemyTumblerAnim;

	// Token: 0x04000881 RID: 2177
	private PhotonView photonView;

	// Token: 0x04000882 RID: 2178
	public HurtCollider hurtCollider;

	// Token: 0x04000883 RID: 2179
	private float hurtColliderTimer;

	// Token: 0x04000884 RID: 2180
	private float roamWaitTimer;

	// Token: 0x04000885 RID: 2181
	private Vector3 roamPoint;

	// Token: 0x04000886 RID: 2182
	private Vector3 backToNavmeshPosition;

	// Token: 0x04000887 RID: 2183
	private Vector3 agentDestination;

	// Token: 0x04000888 RID: 2184
	private Quaternion lookDirection;

	// Token: 0x04000889 RID: 2185
	private float visionTimer;

	// Token: 0x0400088A RID: 2186
	private bool visionPrevious;

	// Token: 0x0400088B RID: 2187
	private bool groundedPrevious;

	// Token: 0x0400088C RID: 2188
	private float hopMoveTimer;

	// Token: 0x0400088D RID: 2189
	public ParticleSystem particleDeathImpact;

	// Token: 0x0400088E RID: 2190
	public ParticleSystem particleDeathBitsFar;

	// Token: 0x0400088F RID: 2191
	public ParticleSystem particleDeathBitsShort;

	// Token: 0x04000890 RID: 2192
	public ParticleSystem particleDeathSmoke;

	// Token: 0x04000891 RID: 2193
	public ParticleSystem particleDeathHat;

	// Token: 0x04000892 RID: 2194
	[Space]
	public SpringQuaternion headSpring;

	// Token: 0x04000893 RID: 2195
	public Transform headTransform;

	// Token: 0x04000894 RID: 2196
	public Transform headTargetTransform;

	// Token: 0x04000895 RID: 2197
	public Transform headTargetCodeTransform;

	// Token: 0x04000896 RID: 2198
	[Space]
	public SpringQuaternion hatSpring;

	// Token: 0x04000897 RID: 2199
	public Transform hatTransform;

	// Token: 0x04000898 RID: 2200
	public Transform hatTargetTransform;

	// Token: 0x04000899 RID: 2201
	[Space]
	public SpringQuaternion mainMeshSpring;

	// Token: 0x0400089A RID: 2202
	private Quaternion mainMeshTargetRotation;

	// Token: 0x0400089B RID: 2203
	private float grabAggroTimer;

	// Token: 0x02000322 RID: 802
	public enum State
	{
		// Token: 0x04002944 RID: 10564
		Spawn,
		// Token: 0x04002945 RID: 10565
		Idle,
		// Token: 0x04002946 RID: 10566
		Roam,
		// Token: 0x04002947 RID: 10567
		Notice,
		// Token: 0x04002948 RID: 10568
		Investigate,
		// Token: 0x04002949 RID: 10569
		MoveToPlayer,
		// Token: 0x0400294A RID: 10570
		Tell,
		// Token: 0x0400294B RID: 10571
		Tumble,
		// Token: 0x0400294C RID: 10572
		TumbleEnd,
		// Token: 0x0400294D RID: 10573
		BackToNavmesh,
		// Token: 0x0400294E RID: 10574
		Leave,
		// Token: 0x0400294F RID: 10575
		Stunned,
		// Token: 0x04002950 RID: 10576
		Dead,
		// Token: 0x04002951 RID: 10577
		Despawn
	}
}
