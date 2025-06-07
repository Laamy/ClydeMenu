using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200004A RID: 74
public class EnemyFloater : MonoBehaviour
{
	// Token: 0x0600022F RID: 559 RVA: 0x00016AC2 File Offset: 0x00014CC2
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000230 RID: 560 RVA: 0x00016AD0 File Offset: 0x00014CD0
	private void Update()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.FloatingAnimation();
		if (this.enemy.CurrentState == EnemyState.Despawn && !this.enemy.IsStunned() && this.currentState == EnemyFloater.State.Idle)
		{
			this.UpdateState(EnemyFloater.State.Despawn);
		}
		if (this.enemy.IsStunned())
		{
			this.UpdateState(EnemyFloater.State.Stun);
		}
		switch (this.currentState)
		{
		case EnemyFloater.State.Spawn:
			this.StateSpawn();
			break;
		case EnemyFloater.State.Idle:
			this.StateIdle();
			break;
		case EnemyFloater.State.Roam:
			this.StateRoam();
			break;
		case EnemyFloater.State.Investigate:
			this.StateInvestigate();
			break;
		case EnemyFloater.State.Notice:
			this.StateNotice();
			break;
		case EnemyFloater.State.GoToPlayer:
			this.StateGoToPlayer();
			break;
		case EnemyFloater.State.Sneak:
			this.StateSneak();
			break;
		case EnemyFloater.State.ChargeAttack:
			this.StateChargeAttack();
			break;
		case EnemyFloater.State.DelayAttack:
			this.StateDelayAttack();
			break;
		case EnemyFloater.State.Attack:
			this.StateAttack();
			break;
		case EnemyFloater.State.Stun:
			this.StateStun();
			break;
		case EnemyFloater.State.Leave:
			this.StateLeave();
			break;
		case EnemyFloater.State.Despawn:
			this.StateDespawn();
			break;
		}
		this.RotationLogic();
		this.TimerLogic();
	}

	// Token: 0x06000231 RID: 561 RVA: 0x00016BE4 File Offset: 0x00014DE4
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
			this.UpdateState(EnemyFloater.State.Idle);
		}
	}

	// Token: 0x06000232 RID: 562 RVA: 0x00016C34 File Offset: 0x00014E34
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
			this.UpdateState(EnemyFloater.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyFloater.State.Leave);
		}
	}

	// Token: 0x06000233 RID: 563 RVA: 0x00016CCC File Offset: 0x00014ECC
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
			if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
			{
				this.UpdateState(EnemyFloater.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyFloater.State.Leave);
		}
	}

	// Token: 0x06000234 RID: 564 RVA: 0x00016E48 File Offset: 0x00015048
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
				this.UpdateState(EnemyFloater.State.Idle);
				return;
			}
			if (Vector3.Distance(base.transform.position, this.agentDestination) < 2f)
			{
				this.UpdateState(EnemyFloater.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyFloater.State.Leave);
		}
	}

	// Token: 0x06000235 RID: 565 RVA: 0x00016F14 File Offset: 0x00015114
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
			if (Vector3.Distance(this.feetTransform.position, this.targetPlayer.transform.position) < 2.5f)
			{
				this.UpdateState(EnemyFloater.State.ChargeAttack);
				return;
			}
			this.UpdateState(EnemyFloater.State.GoToPlayer);
		}
	}

	// Token: 0x06000236 RID: 566 RVA: 0x00016FBC File Offset: 0x000151BC
	public void StateGoToPlayer()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemyFloater.State.Idle);
			return;
		}
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.targetPosition = this.targetPlayer.transform.position;
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.enemy.NavMeshAgent.OverrideAgent(2f, this.enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyFloater.State.Idle);
			return;
		}
		if (Vector3.Distance(this.feetTransform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f && this.stateTimer > 1.5f)
		{
			this.UpdateState(EnemyFloater.State.ChargeAttack);
		}
	}

	// Token: 0x06000237 RID: 567 RVA: 0x000170B0 File Offset: 0x000152B0
	public void StateSneak()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemyFloater.State.Idle);
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
		this.targetPosition = this.targetPlayer.transform.position;
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.enemy.NavMeshAgent.OverrideAgent(1.5f, this.enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyFloater.State.Idle);
			return;
		}
		if (Vector3.Distance(this.feetTransform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f || this.enemy.OnScreen.OnScreenAny)
		{
			this.UpdateState(EnemyFloater.State.Notice);
		}
	}

	// Token: 0x06000238 RID: 568 RVA: 0x000171EC File Offset: 0x000153EC
	public void StateChargeAttack()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 7f;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyFloater.State.DelayAttack);
		}
	}

	// Token: 0x06000239 RID: 569 RVA: 0x00017264 File Offset: 0x00015464
	public void StateDelayAttack()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 3f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyFloater.State.Attack);
		}
	}

	// Token: 0x0600023A RID: 570 RVA: 0x000172B4 File Offset: 0x000154B4
	public void StateAttack()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
			this.attackCount++;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.attackCount >= 3 || Random.Range(0f, 1f) <= 0.3f)
			{
				this.attackCount = 0;
				this.UpdateState(EnemyFloater.State.Leave);
				return;
			}
			this.UpdateState(EnemyFloater.State.Idle);
		}
	}

	// Token: 0x0600023B RID: 571 RVA: 0x00017340 File Offset: 0x00015540
	public void StateStun()
	{
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = this.enemy.Rigidbody.transform.position;
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyFloater.State.Idle);
		}
	}

	// Token: 0x0600023C RID: 572 RVA: 0x00017398 File Offset: 0x00015598
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
			this.stateImpulse = false;
			SemiFunc.EnemyLeaveStart(this.enemy);
		}
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		this.enemy.NavMeshAgent.OverrideAgent(1.5f, this.enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f || this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyFloater.State.Idle);
		}
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00017517 File Offset: 0x00015717
	public void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
	}

	// Token: 0x0600023E RID: 574 RVA: 0x00017553 File Offset: 0x00015753
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyFloater.State.Spawn);
		}
	}

	// Token: 0x0600023F RID: 575 RVA: 0x00017570 File Offset: 0x00015770
	public void OnHurt()
	{
		this.animator.sfxHurt.Play(this.animator.transform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemyFloater.State.Leave)
		{
			this.UpdateState(EnemyFloater.State.Idle);
		}
	}

	// Token: 0x06000240 RID: 576 RVA: 0x000175CC File Offset: 0x000157CC
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
		this.animator.SfxDeath();
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x06000241 RID: 577 RVA: 0x00017704 File Offset: 0x00015904
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && (this.currentState == EnemyFloater.State.Idle || this.currentState == EnemyFloater.State.Roam || this.currentState == EnemyFloater.State.Investigate))
		{
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			this.UpdateState(EnemyFloater.State.Investigate);
		}
	}

	// Token: 0x06000242 RID: 578 RVA: 0x00017750 File Offset: 0x00015950
	public void OnVision()
	{
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (this.currentState == EnemyFloater.State.Roam || this.currentState == EnemyFloater.State.Idle || this.currentState == EnemyFloater.State.Investigate || this.currentState == EnemyFloater.State.Leave)
		{
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			if (!this.enemy.OnScreen.OnScreenAny)
			{
				this.UpdateState(EnemyFloater.State.Sneak);
			}
			else
			{
				this.UpdateState(EnemyFloater.State.Notice);
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
		else if ((this.currentState == EnemyFloater.State.GoToPlayer || this.currentState == EnemyFloater.State.Sneak) && this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
		{
			this.stateTimer = 2f;
		}
	}

	// Token: 0x06000243 RID: 579 RVA: 0x0001783C File Offset: 0x00015A3C
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				return;
			}
			if (this.currentState == EnemyFloater.State.Leave)
			{
				this.grabAggroTimer = 60f;
				PlayerAvatar onGrabbedPlayerAvatar = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
				if (onGrabbedPlayerAvatar.transform.position.y - this.enemy.transform.position.y > 1.15f || onGrabbedPlayerAvatar.transform.position.y - this.enemy.transform.position.y < -1f)
				{
					return;
				}
				this.targetPlayer = onGrabbedPlayerAvatar;
				if (!this.enemy.IsStunned())
				{
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("NoticeRPC", RpcTarget.All, new object[]
						{
							this.targetPlayer.photonView.ViewID
						});
					}
					else
					{
						this.NoticeRPC(this.targetPlayer.photonView.ViewID, default(PhotonMessageInfo));
					}
				}
				this.UpdateState(EnemyFloater.State.Notice);
			}
		}
	}

	// Token: 0x06000244 RID: 580 RVA: 0x00017958 File Offset: 0x00015B58
	private void UpdateState(EnemyFloater.State _state)
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

	// Token: 0x06000245 RID: 581 RVA: 0x000179E0 File Offset: 0x00015BE0
	private void FloatingAnimation()
	{
		float num = 0.1f;
		float num2 = 0.4f;
		float t = this.followParentCurve.Evaluate(this.followParentLerp);
		float num3 = Mathf.Lerp(-num, num, t);
		float num4 = 0f;
		Vector3 localPosition = new Vector3(this.followParentTransform.localPosition.x, num3 + num4, this.followParentTransform.localPosition.z);
		this.followParentLerp += Time.deltaTime * num2;
		if (this.followParentLerp > 1f)
		{
			this.followParentLerp = 0f;
		}
		this.followParentTransform.localPosition = localPosition;
	}

	// Token: 0x06000246 RID: 582 RVA: 0x00017A80 File Offset: 0x00015C80
	private void RotationLogic()
	{
		if (this.currentState == EnemyFloater.State.Notice)
		{
			if (this.targetPlayer && Vector3.Distance(this.targetPlayer.transform.position, this.enemy.Rigidbody.transform.position) > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.targetPlayer.transform.position - this.enemy.Rigidbody.transform.position);
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

	// Token: 0x06000247 RID: 583 RVA: 0x00017BCD File Offset: 0x00015DCD
	private void TimerLogic()
	{
		this.visionTimer -= Time.deltaTime;
	}

	// Token: 0x06000248 RID: 584 RVA: 0x00017BE1 File Offset: 0x00015DE1
	[PunRPC]
	private void UpdateStateRPC(EnemyFloater.State _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
		if (this.currentState == EnemyFloater.State.Spawn)
		{
			this.animator.OnSpawn();
		}
	}

	// Token: 0x06000249 RID: 585 RVA: 0x00017C08 File Offset: 0x00015E08
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

	// Token: 0x0600024A RID: 586 RVA: 0x00017C78 File Offset: 0x00015E78
	[PunRPC]
	private void NoticeRPC(int _playerID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.animator.NoticeSet(_playerID);
	}

	// Token: 0x04000418 RID: 1048
	public EnemyFloater.State currentState;

	// Token: 0x04000419 RID: 1049
	public float stateTimer;

	// Token: 0x0400041A RID: 1050
	public EnemyFloaterAnim animator;

	// Token: 0x0400041B RID: 1051
	public ParticleSystem particleDeathImpact;

	// Token: 0x0400041C RID: 1052
	public ParticleSystem particleDeathBitsFar;

	// Token: 0x0400041D RID: 1053
	public ParticleSystem particleDeathBitsShort;

	// Token: 0x0400041E RID: 1054
	public ParticleSystem particleDeathSmoke;

	// Token: 0x0400041F RID: 1055
	public SpringQuaternion rotationSpring;

	// Token: 0x04000420 RID: 1056
	private Quaternion rotationTarget;

	// Token: 0x04000421 RID: 1057
	private bool stateImpulse = true;

	// Token: 0x04000422 RID: 1058
	internal PlayerAvatar targetPlayer;

	// Token: 0x04000423 RID: 1059
	public Enemy enemy;

	// Token: 0x04000424 RID: 1060
	private PhotonView photonView;

	// Token: 0x04000425 RID: 1061
	private Vector3 agentDestination;

	// Token: 0x04000426 RID: 1062
	private Vector3 backToNavMeshPosition;

	// Token: 0x04000427 RID: 1063
	private Vector3 stuckAttackTarget;

	// Token: 0x04000428 RID: 1064
	private Vector3 targetPosition;

	// Token: 0x04000429 RID: 1065
	private float visionTimer;

	// Token: 0x0400042A RID: 1066
	private bool visionPrevious;

	// Token: 0x0400042B RID: 1067
	public Transform feetTransform;

	// Token: 0x0400042C RID: 1068
	public Transform followParentTransform;

	// Token: 0x0400042D RID: 1069
	public AnimationCurve followParentCurve;

	// Token: 0x0400042E RID: 1070
	private float followParentLerp;

	// Token: 0x0400042F RID: 1071
	private float grabAggroTimer;

	// Token: 0x04000430 RID: 1072
	private int attackCount;

	// Token: 0x0200030D RID: 781
	public enum State
	{
		// Token: 0x0400286D RID: 10349
		Spawn,
		// Token: 0x0400286E RID: 10350
		Idle,
		// Token: 0x0400286F RID: 10351
		Roam,
		// Token: 0x04002870 RID: 10352
		Investigate,
		// Token: 0x04002871 RID: 10353
		Notice,
		// Token: 0x04002872 RID: 10354
		GoToPlayer,
		// Token: 0x04002873 RID: 10355
		Sneak,
		// Token: 0x04002874 RID: 10356
		ChargeAttack,
		// Token: 0x04002875 RID: 10357
		DelayAttack,
		// Token: 0x04002876 RID: 10358
		Attack,
		// Token: 0x04002877 RID: 10359
		Stun,
		// Token: 0x04002878 RID: 10360
		Leave,
		// Token: 0x04002879 RID: 10361
		Despawn
	}
}
