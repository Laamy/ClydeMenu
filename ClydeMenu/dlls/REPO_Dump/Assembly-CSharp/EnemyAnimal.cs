using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000039 RID: 57
public class EnemyAnimal : MonoBehaviour
{
	// Token: 0x060000DD RID: 221 RVA: 0x000083C9 File Offset: 0x000065C9
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060000DE RID: 222 RVA: 0x000083E4 File Offset: 0x000065E4
	private void Update()
	{
		if (this.currentState == EnemyAnimal.State.PlayerNotice || this.currentState == EnemyAnimal.State.GoToPlayer || this.currentState == EnemyAnimal.State.WreakHavoc)
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				if (Vector3.Distance(base.transform.position, playerAvatar.transform.position) < 8f)
				{
					SemiFunc.PlayerEyesOverride(playerAvatar, this.enemy.Vision.VisionTransform.position, 0.1f, base.gameObject);
				}
			}
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (this.grabAggroTimer > 0f)
			{
				this.grabAggroTimer -= Time.deltaTime;
			}
			switch (this.currentState)
			{
			case EnemyAnimal.State.Spawn:
				this.StateSpawn();
				return;
			case EnemyAnimal.State.Idle:
				this.StateIdle();
				return;
			case EnemyAnimal.State.Roam:
				this.StateRoam();
				return;
			case EnemyAnimal.State.Investigate:
				this.StateInvestigate();
				return;
			case EnemyAnimal.State.PlayerNotice:
				this.StatePlayerNotice();
				this.PlayerLookAt();
				return;
			case EnemyAnimal.State.GoToPlayer:
				this.StateGoToPlayer();
				return;
			case EnemyAnimal.State.WreakHavoc:
				this.StateWreakHavoc();
				return;
			case EnemyAnimal.State.Leave:
				this.StateLeave();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00008534 File Offset: 0x00006734
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
			this.UpdateState(EnemyAnimal.State.Idle);
		}
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00008584 File Offset: 0x00006784
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateTimer = Random.Range(2f, 6f);
			this.stateImpulse = false;
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyAnimal.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyAnimal.State.Leave);
		}
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x0000862C File Offset: 0x0000682C
	private void StateRoam()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
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
			this.stateTimer = 5f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
		{
			this.UpdateState(EnemyAnimal.State.Idle);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyAnimal.State.Leave);
		}
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x000087BC File Offset: 0x000069BC
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
			SemiFunc.EnemyCartJump(this.enemy);
			this.enemy.NavMeshAgent.OverrideAgent(4f, 12f, 0.25f);
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 2f)
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyAnimal.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyAnimal.State.Leave);
		}
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x000088B8 File Offset: 0x00006AB8
	private void StatePlayerNotice()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 0.5f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.ResetPath();
		this.enemy.NavMeshAgent.Stop(0.1f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.enemy.NavMeshAgent.Stop(0f);
			this.UpdateState(EnemyAnimal.State.GoToPlayer);
		}
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00008940 File Offset: 0x00006B40
	private void StateGoToPlayer()
	{
		this.enemy.NavMeshAgent.SetDestination(this.playerTarget.transform.position);
		if (this.stateImpulse)
		{
			this.stateTimer = 10f;
			this.stateImpulse = false;
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.playerTarget.transform.position) < 3f)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.UpdateState(EnemyAnimal.State.WreakHavoc);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetDestination()) < 1f && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.playerTarget.transform.position) > 1.5f)
		{
			this.enemy.Jump.StuckTrigger(this.playerTarget.transform.position - this.enemy.Rigidbody.transform.position);
			this.enemy.Rigidbody.DisableFollowPosition(1f, 10f);
		}
		this.enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyAnimal.State.Leave);
		}
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00008AD0 File Offset: 0x00006CD0
	private void StateWreakHavoc()
	{
		if (this.stateImpulse)
		{
			this.havocTimer = 0f;
			this.stateTimer = 20f;
			this.stateImpulse = false;
		}
		if (this.havocTimer <= 0f || Vector3.Distance(base.transform.position, this.enemy.NavMeshAgent.GetDestination()) < 0.25f)
		{
			LevelPoint levelPoint = SemiFunc.LevelPointInTargetRoomGet(this.playerTarget.RoomVolumeCheck, 1f, 10f, this.ignorePoint);
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointInTargetRoomGet(this.playerTarget.RoomVolumeCheck, 0f, 999f, this.ignorePoint);
			}
			NavMeshHit navMeshHit;
			if (!levelPoint || !NavMesh.SamplePosition(levelPoint.transform.position + Random.insideUnitSphere * 3f, out navMeshHit, 5f, -1))
			{
				this.UpdateState(EnemyAnimal.State.Leave);
				return;
			}
			if (Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.ignorePoint = levelPoint;
				this.agentDestination = navMeshHit.position;
				this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			}
			this.havocTimer = 2f;
		}
		this.enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		this.havocTimer -= Time.deltaTime;
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyAnimal.State.Leave);
		}
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00008C80 File Offset: 0x00006E80
	private void StateLeave()
	{
		if (this.stateImpulse)
		{
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, 25f, 50f, false);
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f);
			}
			if (levelPoint)
			{
				this.agentDestination = levelPoint.transform.position;
			}
			else
			{
				this.enemy.EnemyParent.SpawnedTimerSet(0f);
			}
			this.stateTimer = 10f;
			this.stateImpulse = false;
			SemiFunc.EnemyLeaveStart(this.enemy);
			return;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		this.enemy.NavMeshAgent.OverrideAgent(6f, 12f, 0.25f);
		if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
		{
			this.UpdateState(EnemyAnimal.State.Idle);
		}
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00008DAF File Offset: 0x00006FAF
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyAnimal.State.Spawn);
		}
		if (this.enemyAnimalAnim.isActiveAndEnabled)
		{
			this.enemyAnimalAnim.SetSpawn();
		}
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00008DE4 File Offset: 0x00006FE4
	public void OnHurt()
	{
		this.enemyAnimalAnim.hurtSound.Play(this.enemyAnimalAnim.transform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemyAnimal.State.Leave)
		{
			this.UpdateState(EnemyAnimal.State.Idle);
		}
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00008E40 File Offset: 0x00007040
	public void OnDeath()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.enemyAnimalAnim.particleImpact.Play();
		this.enemyAnimalAnim.particleBits.Play();
		Quaternion rotation = Quaternion.LookRotation(-this.enemy.Health.hurtDirection.normalized);
		this.enemyAnimalAnim.particleDirectionalBits.transform.rotation = rotation;
		this.enemyAnimalAnim.particleDirectionalBits.Play();
		this.enemyAnimalAnim.particleLegBits.transform.rotation = rotation;
		this.enemyAnimalAnim.particleLegBits.Play();
		this.enemyAnimalAnim.deathSound.Play(this.enemyAnimalAnim.transform.position, 1f, 1f, 1f, 1f);
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00008F7C File Offset: 0x0000717C
	public void OnVision()
	{
		if (this.currentState != EnemyAnimal.State.Idle && this.currentState != EnemyAnimal.State.Roam && this.currentState != EnemyAnimal.State.Investigate && this.currentState != EnemyAnimal.State.Leave)
		{
			return;
		}
		this.playerTarget = this.enemy.Vision.onVisionTriggeredPlayer;
		if (!this.enemy.IsStunned())
		{
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("NoticeRPC", RpcTarget.All, new object[]
				{
					this.enemy.Vision.onVisionTriggeredID
				});
			}
			else
			{
				this.enemyAnimalAnim.NoticeSet(this.enemy.Vision.onVisionTriggeredID);
			}
		}
		this.UpdateState(EnemyAnimal.State.PlayerNotice);
	}

	// Token: 0x060000EB RID: 235 RVA: 0x0000902B File Offset: 0x0000722B
	public void OnInvestigate()
	{
		if (this.currentState == EnemyAnimal.State.Roam || this.currentState == EnemyAnimal.State.Idle || this.currentState == EnemyAnimal.State.Investigate)
		{
			this.UpdateState(EnemyAnimal.State.Investigate);
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
		}
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00009068 File Offset: 0x00007268
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				return;
			}
			if (this.currentState == EnemyAnimal.State.Leave)
			{
				this.grabAggroTimer = 60f;
				this.playerTarget = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
				if (!this.enemy.IsStunned())
				{
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("NoticeRPC", RpcTarget.All, new object[]
						{
							this.playerTarget.photonView.ViewID
						});
					}
					else
					{
						this.enemyAnimalAnim.NoticeSet(this.playerTarget.photonView.ViewID);
					}
				}
				this.UpdateState(EnemyAnimal.State.PlayerNotice);
			}
		}
	}

	// Token: 0x060000ED RID: 237 RVA: 0x00009124 File Offset: 0x00007324
	private void UpdateState(EnemyAnimal.State _nextState)
	{
		this.stateTimer = 0f;
		this.stateImpulse = true;
		this.currentState = _nextState;
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateStateRPC", RpcTarget.Others, new object[]
			{
				_nextState
			});
		}
	}

	// Token: 0x060000EE RID: 238 RVA: 0x00009174 File Offset: 0x00007374
	private void PlayerLookAt()
	{
		Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 50f * Time.deltaTime);
	}

	// Token: 0x060000EF RID: 239 RVA: 0x000091F9 File Offset: 0x000073F9
	[PunRPC]
	private void UpdateStateRPC(EnemyAnimal.State _nextState, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _nextState;
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x0000920B File Offset: 0x0000740B
	[PunRPC]
	private void NoticeRPC(int _playerID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.enemyAnimalAnim.NoticeSet(_playerID);
	}

	// Token: 0x0400022C RID: 556
	private Enemy enemy;

	// Token: 0x0400022D RID: 557
	private PhotonView photonView;

	// Token: 0x0400022E RID: 558
	public EnemyAnimalAnim enemyAnimalAnim;

	// Token: 0x0400022F RID: 559
	public GameObject welts;

	// Token: 0x04000230 RID: 560
	public EnemyAnimal.State currentState;

	// Token: 0x04000231 RID: 561
	private float havocTimer;

	// Token: 0x04000232 RID: 562
	private LevelPoint ignorePoint;

	// Token: 0x04000233 RID: 563
	private float stateTimer;

	// Token: 0x04000234 RID: 564
	private bool stateImpulse;

	// Token: 0x04000235 RID: 565
	private Vector3 agentDestination;

	// Token: 0x04000236 RID: 566
	private PlayerAvatar playerTarget;

	// Token: 0x04000237 RID: 567
	private float grabAggroTimer;

	// Token: 0x02000303 RID: 771
	public enum State
	{
		// Token: 0x04002810 RID: 10256
		Spawn,
		// Token: 0x04002811 RID: 10257
		Idle,
		// Token: 0x04002812 RID: 10258
		Roam,
		// Token: 0x04002813 RID: 10259
		Investigate,
		// Token: 0x04002814 RID: 10260
		PlayerNotice,
		// Token: 0x04002815 RID: 10261
		GoToPlayer,
		// Token: 0x04002816 RID: 10262
		WreakHavoc,
		// Token: 0x04002817 RID: 10263
		Leave
	}
}
