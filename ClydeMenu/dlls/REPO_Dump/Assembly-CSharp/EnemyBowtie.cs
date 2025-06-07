using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000043 RID: 67
public class EnemyBowtie : MonoBehaviour
{
	// Token: 0x06000194 RID: 404 RVA: 0x00010BF5 File Offset: 0x0000EDF5
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x06000195 RID: 405 RVA: 0x00010C10 File Offset: 0x0000EE10
	private void Update()
	{
		this.HurtColliderLeaveLogic();
		this.SpringLogic();
		this.PlayerEyesLogic();
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (this.grabAggroTimer > 0f)
			{
				this.grabAggroTimer -= Time.deltaTime;
			}
			if (!LevelGenerator.Instance.Generated)
			{
				return;
			}
			this.HorizontalRotationLogic();
			this.VerticalRotationLogic();
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyBowtie.State.Stun);
			}
			else if (this.enemy.CurrentState == EnemyState.Despawn)
			{
				this.UpdateState(EnemyBowtie.State.Despawn);
			}
			switch (this.currentState)
			{
			case EnemyBowtie.State.Spawn:
				this.StateSpawn();
				return;
			case EnemyBowtie.State.Idle:
				this.StateIdle();
				return;
			case EnemyBowtie.State.Roam:
				this.StateRoam();
				return;
			case EnemyBowtie.State.Investigate:
				this.StateInvestigate();
				return;
			case EnemyBowtie.State.PlayerNotice:
				this.StatePlayerNotice();
				return;
			case EnemyBowtie.State.Yell:
				this.StateYell();
				return;
			case EnemyBowtie.State.YellEnd:
				this.StateYellEnd();
				return;
			case EnemyBowtie.State.Leave:
				this.StateLeave();
				return;
			case EnemyBowtie.State.Stun:
				this.StateStun();
				return;
			case EnemyBowtie.State.Despawn:
				this.StateDespawn();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06000196 RID: 406 RVA: 0x00010D20 File Offset: 0x0000EF20
	private void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBowtie.State.Idle);
		}
	}

	// Token: 0x06000197 RID: 407 RVA: 0x00010D58 File Offset: 0x0000EF58
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
			this.UpdateState(EnemyBowtie.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyBowtie.State.Leave);
		}
	}

	// Token: 0x06000198 RID: 408 RVA: 0x00010E00 File Offset: 0x0000F000
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
			this.UpdateState(EnemyBowtie.State.Idle);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyBowtie.State.Leave);
		}
	}

	// Token: 0x06000199 RID: 409 RVA: 0x00010F90 File Offset: 0x0000F190
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
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 2f)
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyBowtie.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyBowtie.State.Leave);
		}
	}

	// Token: 0x0600019A RID: 410 RVA: 0x0001106C File Offset: 0x0000F26C
	private void StatePlayerNotice()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		this.enemy.Jump.SurfaceJumpDisable(0.5f);
		this.enemy.NavMeshAgent.ResetPath();
		this.enemy.NavMeshAgent.Stop(0.1f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.enemy.NavMeshAgent.Stop(0f);
			this.UpdateState(EnemyBowtie.State.Yell);
		}
	}

	// Token: 0x0600019B RID: 411 RVA: 0x00011108 File Offset: 0x0000F308
	private void StateYell()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.stateTimer = 5f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		this.enemy.Jump.SurfaceJumpDisable(0.5f);
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBowtie.State.YellEnd);
		}
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0001117C File Offset: 0x0000F37C
	private void StateYellEnd()
	{
		if (this.stateImpulse)
		{
			this.attacks++;
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		this.enemy.Jump.SurfaceJumpDisable(0.5f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.attacks >= 3 || Random.Range(0f, 1f) <= 0.3f)
			{
				this.attacks = 0;
				this.UpdateState(EnemyBowtie.State.Leave);
				return;
			}
			this.UpdateState(EnemyBowtie.State.Idle);
		}
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0001121C File Offset: 0x0000F41C
	private void StateLeave()
	{
		if (this.stateImpulse)
		{
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, 30f, 60f, false);
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
			this.stateTimer = 5f;
			this.stateImpulse = false;
			SemiFunc.EnemyLeaveStart(this.enemy);
			return;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		this.enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
		{
			this.UpdateState(EnemyBowtie.State.Idle);
		}
	}

	// Token: 0x0600019E RID: 414 RVA: 0x0001134B File Offset: 0x0000F54B
	private void StateStun()
	{
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyBowtie.State.Idle);
		}
	}

	// Token: 0x0600019F RID: 415 RVA: 0x00011361 File Offset: 0x0000F561
	private void StateDespawn()
	{
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x00011363 File Offset: 0x0000F563
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyBowtie.State.Idle);
		}
		if (this.anim.isActiveAndEnabled)
		{
			this.anim.OnSpawn();
		}
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x00011398 File Offset: 0x0000F598
	public void OnHurt()
	{
		this.anim.GroanPause();
		this.anim.StunPause();
		this.anim.hurtSound.Play(this.anim.transform.position, 1f, 1f, 1f, 1f);
		if (this.currentState == EnemyBowtie.State.Yell)
		{
			this.UpdateState(EnemyBowtie.State.YellEnd);
		}
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x00011400 File Offset: 0x0000F600
	public void OnDeath()
	{
		this.anim.GroanPause();
		this.anim.StunPause();
		this.anim.deathSound.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.anim.particleImpact.Play();
		this.anim.particleBits.Play();
		this.anim.particleEyes.Play();
		this.anim.particleDirectionalBits.transform.rotation = Quaternion.LookRotation(-this.enemy.Health.hurtDirection.normalized);
		this.anim.particleDirectionalBits.Play();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x00011540 File Offset: 0x0000F740
	public void OnVisionTriggered()
	{
		if (this.currentState != EnemyBowtie.State.Idle && this.currentState != EnemyBowtie.State.Roam && this.currentState != EnemyBowtie.State.Investigate && this.currentState != EnemyBowtie.State.Leave)
		{
			return;
		}
		if (this.enemy.Jump.jumping)
		{
			return;
		}
		if (this.enemy.IsStunned())
		{
			return;
		}
		PlayerAvatar onVisionTriggeredPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
		if (Mathf.Abs(onVisionTriggeredPlayer.transform.position.y - this.enemy.transform.position.y) > 4f)
		{
			return;
		}
		this.playerTarget = onVisionTriggeredPlayer;
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("NoticeRPC", RpcTarget.All, new object[]
			{
				this.enemy.Vision.onVisionTriggeredID
			});
		}
		else
		{
			this.anim.NoticeSet(this.enemy.Vision.onVisionTriggeredID);
		}
		this.UpdateState(EnemyBowtie.State.PlayerNotice);
		this.VerticalAimSet(100f);
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x00011643 File Offset: 0x0000F843
	public void OnInvestigate()
	{
		if (this.currentState == EnemyBowtie.State.Roam || this.currentState == EnemyBowtie.State.Idle || this.currentState == EnemyBowtie.State.Investigate)
		{
			this.UpdateState(EnemyBowtie.State.Investigate);
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
		}
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x00011680 File Offset: 0x0000F880
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				return;
			}
			if (this.currentState == EnemyBowtie.State.Leave)
			{
				this.grabAggroTimer = 60f;
				PlayerAvatar onGrabbedPlayerAvatar = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
				if (onGrabbedPlayerAvatar.transform.position.y - this.enemy.transform.position.y > 1.15f || onGrabbedPlayerAvatar.transform.position.y - this.enemy.transform.position.y < -1f)
				{
					return;
				}
				this.playerTarget = onGrabbedPlayerAvatar;
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
						this.anim.NoticeSet(this.playerTarget.photonView.ViewID);
					}
				}
				this.UpdateState(EnemyBowtie.State.PlayerNotice);
			}
		}
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x00011798 File Offset: 0x0000F998
	private void UpdateState(EnemyBowtie.State _state)
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
		}
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x000117F4 File Offset: 0x0000F9F4
	private void HorizontalRotationLogic()
	{
		if (this.currentState == EnemyBowtie.State.Roam || this.currentState == EnemyBowtie.State.Investigate || this.currentState == EnemyBowtie.State.Leave)
		{
			if (this.enemy.NavMeshAgent.AgentVelocity.magnitude > 0.05f)
			{
				Quaternion quaternion = Quaternion.Euler(0f, Quaternion.LookRotation(this.enemy.NavMeshAgent.AgentVelocity.normalized).eulerAngles.y, 0f);
				this.horizontalRotationTarget = quaternion;
			}
		}
		else if (this.currentState == EnemyBowtie.State.PlayerNotice)
		{
			Quaternion quaternion2 = Quaternion.Euler(0f, Quaternion.LookRotation(this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
			this.horizontalRotationTarget = quaternion2;
		}
		else if (this.currentState == EnemyBowtie.State.Yell)
		{
			Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
			this.horizontalRotationTarget = Quaternion.Slerp(this.horizontalRotationTarget, b, 1f * Time.deltaTime);
		}
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.horizontalRotationSpring, this.horizontalRotationTarget, -1f);
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0001197C File Offset: 0x0000FB7C
	private void VerticalRotationLogic()
	{
		if (this.currentState == EnemyBowtie.State.Yell)
		{
			this.VerticalAimSet(1f);
			this.verticalRotationTransform.localRotation = SemiFunc.SpringQuaternionGet(this.verticalRotationSpring, this.verticalRotationTarget, -1f);
			return;
		}
		this.verticalRotationTransform.localRotation = SemiFunc.SpringQuaternionGet(this.verticalRotationSpring, Quaternion.identity, -1f);
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x000119E0 File Offset: 0x0000FBE0
	private void VerticalAimSet(float _lerp)
	{
		this.verticalRotationTransform.LookAt(this.playerTarget.transform);
		float num = 45f;
		float num2 = this.verticalRotationTransform.localEulerAngles.x;
		if (num2 < 180f)
		{
			num2 = Mathf.Clamp(num2, 0f, num);
		}
		else
		{
			num2 = Mathf.Clamp(num2, 360f - num, 360f);
		}
		this.verticalRotationTransform.localRotation = Quaternion.Euler(num2, 0f, 0f);
		Quaternion localRotation = this.verticalRotationTransform.localRotation;
		this.verticalRotationTransform.localRotation = Quaternion.identity;
		this.verticalRotationTarget = Quaternion.Lerp(this.verticalRotationTarget, localRotation, _lerp * Time.deltaTime);
	}

	// Token: 0x060001AA RID: 426 RVA: 0x00011A94 File Offset: 0x0000FC94
	private void HurtColliderLeaveLogic()
	{
		if (!this.enemy.Jump.jumping && this.currentState == EnemyBowtie.State.Leave)
		{
			this.hurtColliderLeave.gameObject.SetActive(true);
			return;
		}
		this.hurtColliderLeave.gameObject.SetActive(false);
	}

	// Token: 0x060001AB RID: 427 RVA: 0x00011AD4 File Offset: 0x0000FCD4
	private void SpringLogic()
	{
		this.headTransform.rotation = SemiFunc.SpringQuaternionGet(this.headSpring, this.HeadTargetTransform.rotation, -1f);
		this.eyeRightTransform.rotation = SemiFunc.SpringQuaternionGet(this.eyeRightSpring, this.eyeRightTargetTransform.rotation, -1f);
		this.eyeLeftTransform.rotation = SemiFunc.SpringQuaternionGet(this.eyeLeftSpring, this.eyeLeftTargetTransform.rotation, -1f);
	}

	// Token: 0x060001AC RID: 428 RVA: 0x00011B54 File Offset: 0x0000FD54
	private void PlayerEyesLogic()
	{
		if (this.currentState == EnemyBowtie.State.PlayerNotice || this.currentState == EnemyBowtie.State.Yell || this.currentState == EnemyBowtie.State.YellEnd)
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				if (Vector3.Distance(base.transform.position, playerAvatar.transform.position) < 8f)
				{
					SemiFunc.PlayerEyesOverride(playerAvatar, this.enemy.Vision.VisionTransform.position, 0.1f, base.gameObject);
				}
			}
		}
	}

	// Token: 0x060001AD RID: 429 RVA: 0x00011C08 File Offset: 0x0000FE08
	[PunRPC]
	private void UpdateStateRPC(EnemyBowtie.State _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
	}

	// Token: 0x060001AE RID: 430 RVA: 0x00011C1A File Offset: 0x0000FE1A
	[PunRPC]
	private void NoticeRPC(int _playerID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.anim.NoticeSet(_playerID);
	}

	// Token: 0x04000373 RID: 883
	private PhotonView photonView;

	// Token: 0x04000374 RID: 884
	public EnemyBowtie.State currentState;

	// Token: 0x04000375 RID: 885
	public float stateTimer;

	// Token: 0x04000376 RID: 886
	private bool stateImpulse;

	// Token: 0x04000377 RID: 887
	[Space]
	public EnemyBowtieAnim anim;

	// Token: 0x04000378 RID: 888
	public Transform hurtColliderLeave;

	// Token: 0x04000379 RID: 889
	[Space]
	public SpringQuaternion headSpring;

	// Token: 0x0400037A RID: 890
	public Transform headTransform;

	// Token: 0x0400037B RID: 891
	public Transform HeadTargetTransform;

	// Token: 0x0400037C RID: 892
	[Space]
	public SpringQuaternion eyeRightSpring;

	// Token: 0x0400037D RID: 893
	public Transform eyeRightTransform;

	// Token: 0x0400037E RID: 894
	public Transform eyeRightTargetTransform;

	// Token: 0x0400037F RID: 895
	[Space]
	public SpringQuaternion eyeLeftSpring;

	// Token: 0x04000380 RID: 896
	public Transform eyeLeftTransform;

	// Token: 0x04000381 RID: 897
	public Transform eyeLeftTargetTransform;

	// Token: 0x04000382 RID: 898
	[Space]
	public SpringQuaternion horizontalRotationSpring;

	// Token: 0x04000383 RID: 899
	private Quaternion horizontalRotationTarget;

	// Token: 0x04000384 RID: 900
	[Space]
	public Transform verticalRotationTransform;

	// Token: 0x04000385 RID: 901
	public SpringQuaternion verticalRotationSpring;

	// Token: 0x04000386 RID: 902
	private Quaternion verticalRotationTarget;

	// Token: 0x04000387 RID: 903
	private float roamWaitTimer;

	// Token: 0x04000388 RID: 904
	private Vector3 agentDestination;

	// Token: 0x04000389 RID: 905
	internal Enemy enemy;

	// Token: 0x0400038A RID: 906
	private PlayerAvatar playerTarget;

	// Token: 0x0400038B RID: 907
	private float grabAggroTimer;

	// Token: 0x0400038C RID: 908
	private int attacks;

	// Token: 0x0200030A RID: 778
	public enum State
	{
		// Token: 0x04002846 RID: 10310
		Spawn,
		// Token: 0x04002847 RID: 10311
		Idle,
		// Token: 0x04002848 RID: 10312
		Roam,
		// Token: 0x04002849 RID: 10313
		Investigate,
		// Token: 0x0400284A RID: 10314
		PlayerNotice,
		// Token: 0x0400284B RID: 10315
		Yell,
		// Token: 0x0400284C RID: 10316
		YellEnd,
		// Token: 0x0400284D RID: 10317
		Leave,
		// Token: 0x0400284E RID: 10318
		Stun,
		// Token: 0x0400284F RID: 10319
		Despawn
	}
}
