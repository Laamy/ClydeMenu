using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000086 RID: 134
public class EnemyUpscream : MonoBehaviour
{
	// Token: 0x06000570 RID: 1392 RVA: 0x00035ABF File Offset: 0x00033CBF
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x00035ADC File Offset: 0x00033CDC
	private void Update()
	{
		this.HeadLogic();
		this.EyeLogic();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				this.grabAggroTimer -= Time.deltaTime;
			}
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyUpscream.State.Stun);
			}
			switch (this.currentState)
			{
			case EnemyUpscream.State.Spawn:
				this.StateSpawn();
				break;
			case EnemyUpscream.State.Idle:
				this.StateIdle();
				this.IdleBreakLogic();
				break;
			case EnemyUpscream.State.Roam:
				this.StateRoam();
				this.AgentVelocityRotation();
				this.IdleBreakLogic();
				break;
			case EnemyUpscream.State.Investigate:
				this.StateInvestigate();
				this.AgentVelocityRotation();
				break;
			case EnemyUpscream.State.PlayerNotice:
				this.StatePlayerNotice();
				break;
			case EnemyUpscream.State.GoToPlayer:
				this.AgentVelocityRotation();
				this.StateGoToPlayer();
				break;
			case EnemyUpscream.State.Attack:
				this.StateAttack();
				break;
			case EnemyUpscream.State.Leave:
				this.StateLeave();
				break;
			case EnemyUpscream.State.IdleBreak:
				this.StateIdleBreak();
				break;
			case EnemyUpscream.State.Stun:
				this.StateStun();
				break;
			}
		}
		if (this.currentState == EnemyUpscream.State.Attack && this.targetPlayer)
		{
			if (this.targetPlayer.isLocal)
			{
				PlayerController.instance.InputDisable(0.1f);
				CameraAim.Instance.AimTargetSet(this.visionTransform.position, 0.1f, 5f, base.gameObject, 90);
				CameraZoom.Instance.OverrideZoomSet(50f, 0.1f, 5f, 5f, base.gameObject, 50);
				Color color = new Color(0.4f, 0f, 0f, 1f);
				PostProcessing.Instance.VignetteOverride(color, 0.75f, 1f, 3.5f, 2.5f, 0.5f, base.gameObject);
			}
			if (this.attackImpulse)
			{
				if (this.targetPlayer.isLocal)
				{
					this.targetPlayer.physGrabber.ReleaseObject(0.1f);
					CameraGlitch.Instance.PlayLong();
				}
				this.attackImpulse = false;
				this.upscreamAnim.animator.SetTrigger("Attack");
				return;
			}
		}
		else
		{
			this.attackImpulse = true;
		}
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x00035CFB File Offset: 0x00033EFB
	private void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyUpscream.State.Idle);
		}
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x00035D34 File Offset: 0x00033F34
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			if (this.previousState == EnemyUpscream.State.Spawn)
			{
				this.stateTimer = 0.5f;
			}
			else
			{
				this.stateTimer = Random.Range(3f, 8f);
			}
			this.stateImpulse = false;
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyUpscream.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyUpscream.State.Leave);
		}
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x00035DF4 File Offset: 0x00033FF4
	private void StateRoam()
	{
		float num = Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetDestination());
		if (this.stateImpulse || !this.enemy.NavMeshAgent.HasPath() || num < 1f)
		{
			if (this.stateImpulse)
			{
				this.roamWaitTimer = 0f;
				this.stateImpulse = false;
			}
			if (this.roamWaitTimer <= 0f)
			{
				this.stateTimer = 5f;
				this.roamWaitTimer = Random.Range(0f, 5f);
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
					this.agentPoint = navMeshHit.position;
					this.enemy.NavMeshAgent.SetDestination(this.agentPoint);
				}
			}
			else
			{
				this.roamWaitTimer -= Time.deltaTime;
			}
		}
		else
		{
			SemiFunc.EnemyCartJump(this.enemy);
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
				if (this.stateTimer <= 0f)
				{
					this.UpdateState(EnemyUpscream.State.Idle);
				}
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyUpscream.State.Leave);
		}
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x00035FD8 File Offset: 0x000341D8
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
			this.enemy.NavMeshAgent.SetDestination(this.agentPoint);
			SemiFunc.EnemyCartJump(this.enemy);
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentPoint) < 2f)
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyUpscream.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyUpscream.State.Leave);
		}
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x000360B4 File Offset: 0x000342B4
	private void StatePlayerNotice()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 0.5f;
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
		}
		this.enemy.NavMeshAgent.Stop(0.5f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.enemy.NavMeshAgent.Stop(0f);
			this.UpdateState(EnemyUpscream.State.GoToPlayer);
		}
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x00036160 File Offset: 0x00034360
	private void StateGoToPlayer()
	{
		if (!this.enemy.Jump.jumping)
		{
			this.enemy.NavMeshAgent.SetDestination(this.targetPlayer.transform.position);
		}
		else
		{
			this.enemy.NavMeshAgent.Disable(0.1f);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPlayer.transform.position, 5f * Time.deltaTime);
		}
		SemiFunc.EnemyCartJump(this.enemy);
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
			return;
		}
		this.enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.targetPlayer.transform.position) < 1.5f && !this.enemy.Jump.jumping && !this.enemy.IsStunned())
		{
			this.enemy.NavMeshAgent.ResetPath();
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyUpscream.State.Attack);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetDestination()) < 1f)
		{
			if (this.stateTimer <= 0f)
			{
				this.enemy.Jump.StuckReset();
				this.UpdateState(EnemyUpscream.State.Leave);
			}
			else if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.targetPlayer.transform.position) > 1.5f)
			{
				this.enemy.Jump.StuckTrigger(this.targetPlayer.transform.position - this.enemy.Rigidbody.transform.position);
			}
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyUpscream.State.Leave);
		}
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x00036390 File Offset: 0x00034590
	private void StateAttack()
	{
		if (this.stateImpulse)
		{
			this.attacks++;
			this.stateTimer = 1.5f;
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
		}
		Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.targetPlayer.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 50f * Time.deltaTime);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.attacks >= 3 || Random.Range(0f, 1f) <= 0.5f)
			{
				this.UpdateState(EnemyUpscream.State.Leave);
				return;
			}
			this.UpdateState(EnemyUpscream.State.Idle);
		}
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x000364C0 File Offset: 0x000346C0
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
				this.agentPoint = navMeshHit.position;
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
		SemiFunc.EnemyCartJump(this.enemy);
		this.enemy.NavMeshAgent.SetDestination(this.agentPoint);
		this.enemy.NavMeshAgent.OverrideAgent(this.enemy.NavMeshAgent.DefaultSpeed + 2.5f, this.enemy.NavMeshAgent.DefaultAcceleration + 2.5f, 0.2f);
		this.enemy.Rigidbody.OverrideFollowPosition(1f, 10f, -1f);
		if (Vector3.Distance(base.transform.position, this.agentPoint) < 1f || this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyUpscream.State.Idle);
		}
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x00036680 File Offset: 0x00034880
	private void StateIdleBreak()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyUpscream.State.Idle);
		}
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x00036702 File Offset: 0x00034902
	private void StateStun()
	{
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyUpscream.State.Idle);
		}
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x00036718 File Offset: 0x00034918
	internal void UpdateState(EnemyUpscream.State _state)
	{
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.currentState == _state)
		{
			return;
		}
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateStateRPC", RpcTarget.All, new object[]
			{
				_state
			});
			return;
		}
		this.UpdateStateRPC(_state, default(PhotonMessageInfo));
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x00036778 File Offset: 0x00034978
	private void IdleBreakLogic()
	{
		if (this.idleBreakTimer >= 0f)
		{
			this.idleBreakTimer -= Time.deltaTime;
			if (this.idleBreakTimer <= 0f)
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyUpscream.State.IdleBreak);
				this.idleBreakTimer = Random.Range(this.idleBreakTimeMin, this.idleBreakTimeMax);
			}
		}
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x000367DA File Offset: 0x000349DA
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyUpscream.State.Spawn);
		}
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x000367F7 File Offset: 0x000349F7
	public void OnHurt()
	{
		this.upscreamAnim.hurtSound.Play(this.upscreamAnim.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x00036830 File Offset: 0x00034A30
	public void OnDeath()
	{
		ParticleSystem[] array = this.deathEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x000368D0 File Offset: 0x00034AD0
	public void OnVision()
	{
		if (this.currentState == EnemyUpscream.State.Idle || this.currentState == EnemyUpscream.State.Roam || this.currentState == EnemyUpscream.State.IdleBreak || this.currentState == EnemyUpscream.State.Investigate || this.currentState == EnemyUpscream.State.Leave)
		{
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
			}
			if (!this.enemy.IsStunned())
			{
				if (GameManager.Multiplayer())
				{
					this.photonView.RPC("NoticeSetRPC", RpcTarget.All, new object[]
					{
						this.enemy.Vision.onVisionTriggeredID
					});
				}
				else
				{
					this.upscreamAnim.NoticeSet(this.enemy.Vision.onVisionTriggeredID);
				}
			}
			this.UpdateState(EnemyUpscream.State.PlayerNotice);
			return;
		}
		if (this.currentState == EnemyUpscream.State.GoToPlayer && this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
		{
			this.stateTimer = 2f;
		}
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x000369F4 File Offset: 0x00034BF4
	public void OnInvestigate()
	{
		if (this.currentState == EnemyUpscream.State.Roam || this.currentState == EnemyUpscream.State.Idle || this.currentState == EnemyUpscream.State.IdleBreak || this.currentState == EnemyUpscream.State.Investigate)
		{
			this.UpdateState(EnemyUpscream.State.Investigate);
			this.agentPoint = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
		}
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x00036A44 File Offset: 0x00034C44
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				return;
			}
			if (this.currentState == EnemyUpscream.State.Leave)
			{
				this.grabAggroTimer = 60f;
				if (this.targetPlayer != this.enemy.Rigidbody.onGrabbedPlayerAvatar)
				{
					this.targetPlayer = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
						{
							this.targetPlayer.photonView.ViewID
						});
					}
				}
				if (!this.enemy.IsStunned())
				{
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("NoticeSetRPC", RpcTarget.All, new object[]
						{
							this.targetPlayer.photonView.ViewID
						});
					}
					else
					{
						this.upscreamAnim.NoticeSet(this.targetPlayer.photonView.ViewID);
					}
				}
				this.UpdateState(EnemyUpscream.State.PlayerNotice);
			}
		}
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x00036B50 File Offset: 0x00034D50
	public void HeadLogic()
	{
		Quaternion targetRotation = this.headIdleTransform.rotation;
		if (this.targetPlayer && (this.currentState == EnemyUpscream.State.PlayerNotice || this.currentState == EnemyUpscream.State.GoToPlayer || this.currentState == EnemyUpscream.State.Attack) && !this.enemy.IsStunned())
		{
			Vector3 a = this.targetPlayer.PlayerVisionTarget.VisionTransform.position;
			if (this.targetPlayer.isLocal)
			{
				a = this.targetPlayer.localCameraPosition;
			}
			targetRotation = Quaternion.LookRotation(a - this.headTransform.position);
		}
		this.headTransform.rotation = SemiFunc.SpringQuaternionGet(this.headSpring, targetRotation, -1f);
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x00036C00 File Offset: 0x00034E00
	public void EyeLogic()
	{
		if (this.currentState == EnemyUpscream.State.PlayerNotice || this.currentState == EnemyUpscream.State.GoToPlayer || this.currentState == EnemyUpscream.State.Attack)
		{
			this.eyeLeftSpring.damping = 0.6f;
			this.eyeLeftSpring.speed = 15f;
			this.eyeRightSpring.damping = 0.6f;
			this.eyeRightSpring.speed = 15f;
			this.eyeLeftTransform.rotation = SemiFunc.SpringQuaternionGet(this.eyeLeftSpring, this.eyeLeftTarget.rotation, -1f);
			this.eyeRightTransform.rotation = SemiFunc.SpringQuaternionGet(this.eyeRightSpring, this.eyeRightTarget.rotation, -1f);
			return;
		}
		this.eyeLeftSpring.damping = 0.2f;
		this.eyeLeftSpring.speed = 15f;
		this.eyeRightSpring.damping = 0.2f;
		this.eyeRightSpring.speed = 15f;
		this.eyeLeftTransform.rotation = SemiFunc.SpringQuaternionGet(this.eyeLeftSpring, this.eyeLeftIdle.rotation, -1f);
		this.eyeRightTransform.rotation = SemiFunc.SpringQuaternionGet(this.eyeRightSpring, this.eyeRightIdle.rotation, -1f);
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x00036D44 File Offset: 0x00034F44
	private void AgentVelocityRotation()
	{
		if (this.enemy.NavMeshAgent.AgentVelocity.magnitude > 0.005f)
		{
			Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.enemy.NavMeshAgent.AgentVelocity.normalized).eulerAngles.y, 0f);
			float num = 2f;
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, num * Time.deltaTime);
		}
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x00036DD0 File Offset: 0x00034FD0
	[PunRPC]
	private void UpdateStateRPC(EnemyUpscream.State _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.previousState = this.currentState;
		this.currentState = _state;
		this.stateImpulse = true;
		this.stateTimer = 0f;
		if (this.currentState == EnemyUpscream.State.Spawn)
		{
			this.upscreamAnim.SetSpawn();
		}
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x00036E20 File Offset: 0x00035020
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

	// Token: 0x06000589 RID: 1417 RVA: 0x00036E90 File Offset: 0x00035090
	[PunRPC]
	private void NoticeSetRPC(int _playerID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.upscreamAnim.NoticeSet(_playerID);
	}

	// Token: 0x040008B6 RID: 2230
	[Header("References")]
	public EnemyUpscreamAnim upscreamAnim;

	// Token: 0x040008B7 RID: 2231
	internal Enemy enemy;

	// Token: 0x040008B8 RID: 2232
	public ParticleSystem[] deathEffects;

	// Token: 0x040008B9 RID: 2233
	public EnemyUpscream.State currentState;

	// Token: 0x040008BA RID: 2234
	public EnemyUpscream.State previousState;

	// Token: 0x040008BB RID: 2235
	private float stateTimer;

	// Token: 0x040008BC RID: 2236
	private bool attackImpulse;

	// Token: 0x040008BD RID: 2237
	private bool stateImpulse;

	// Token: 0x040008BE RID: 2238
	internal PlayerAvatar targetPlayer;

	// Token: 0x040008BF RID: 2239
	private Vector3 targetPosition;

	// Token: 0x040008C0 RID: 2240
	public Transform visionTransform;

	// Token: 0x040008C1 RID: 2241
	private float hasVisionTimer;

	// Token: 0x040008C2 RID: 2242
	private Vector3 agentPoint;

	// Token: 0x040008C3 RID: 2243
	private float roamWaitTimer;

	// Token: 0x040008C4 RID: 2244
	private PhotonView photonView;

	// Token: 0x040008C5 RID: 2245
	[Header("Head")]
	public SpringQuaternion headSpring;

	// Token: 0x040008C6 RID: 2246
	public Transform headTransform;

	// Token: 0x040008C7 RID: 2247
	public Transform headIdleTransform;

	// Token: 0x040008C8 RID: 2248
	[Header("Eyes")]
	public SpringQuaternion eyeLeftSpring;

	// Token: 0x040008C9 RID: 2249
	[Space(10f)]
	public Transform eyeLeftTransform;

	// Token: 0x040008CA RID: 2250
	public Transform eyeLeftIdle;

	// Token: 0x040008CB RID: 2251
	public Transform eyeLeftTarget;

	// Token: 0x040008CC RID: 2252
	[Space(10f)]
	public SpringQuaternion eyeRightSpring;

	// Token: 0x040008CD RID: 2253
	[Space(10f)]
	public Transform eyeRightTransform;

	// Token: 0x040008CE RID: 2254
	public Transform eyeRightIdle;

	// Token: 0x040008CF RID: 2255
	public Transform eyeRightTarget;

	// Token: 0x040008D0 RID: 2256
	[Header("Idle Break")]
	public float idleBreakTimeMin = 45f;

	// Token: 0x040008D1 RID: 2257
	public float idleBreakTimeMax = 90f;

	// Token: 0x040008D2 RID: 2258
	private float idleBreakTimer;

	// Token: 0x040008D3 RID: 2259
	private float grabAggroTimer;

	// Token: 0x040008D4 RID: 2260
	private int attacks;

	// Token: 0x02000323 RID: 803
	public enum State
	{
		// Token: 0x04002953 RID: 10579
		Spawn,
		// Token: 0x04002954 RID: 10580
		Idle,
		// Token: 0x04002955 RID: 10581
		Roam,
		// Token: 0x04002956 RID: 10582
		Investigate,
		// Token: 0x04002957 RID: 10583
		PlayerNotice,
		// Token: 0x04002958 RID: 10584
		GoToPlayer,
		// Token: 0x04002959 RID: 10585
		Attack,
		// Token: 0x0400295A RID: 10586
		Leave,
		// Token: 0x0400295B RID: 10587
		IdleBreak,
		// Token: 0x0400295C RID: 10588
		Stun
	}
}
