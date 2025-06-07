using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200006B RID: 107
public class EnemyHunter : MonoBehaviour
{
	// Token: 0x0600036C RID: 876 RVA: 0x00021D65 File Offset: 0x0001FF65
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (!Application.isEditor || (SemiFunc.IsMultiplayer() && !GameManager.instance.localTest))
		{
			this.debugSpawn = false;
		}
	}

	// Token: 0x0600036D RID: 877 RVA: 0x00021D94 File Offset: 0x0001FF94
	private void Update()
	{
		this.VerticalRotationLogic();
		this.HurtColliderTimer();
		this.LineRendererLogic();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.enemy.Rigidbody.physGrabObject.rbVelocity.y < -0.5f && (this.enemy.Rigidbody.timeSinceStun == 0f || this.enemy.Rigidbody.timeSinceStun > 3f))
			{
				this.tripTimer += Time.deltaTime;
				if (this.enemy.Rigidbody.physGrabObject.rbVelocity.y <= -2f)
				{
					this.tripTimer = 999f;
				}
			}
			else
			{
				this.tripTimer = 0f;
			}
			if (this.tripTimer > 0.5f)
			{
				this.enemy.StateStunned.Set(2f);
				this.tripTimer = 0f;
			}
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyHunter.State.Stun);
			}
			if (this.enemy.CurrentState == EnemyState.Despawn && !this.enemy.IsStunned())
			{
				this.UpdateState(EnemyHunter.State.Despawn);
			}
			this.ShotsFiredLogic();
			this.HorizontalRotationLogic();
			this.LeaveInterruptLogic();
			switch (this.currentState)
			{
			case EnemyHunter.State.Spawn:
				this.StateSpawn();
				return;
			case EnemyHunter.State.Idle:
				this.StateIdle();
				return;
			case EnemyHunter.State.Roam:
				this.StateRoam();
				return;
			case EnemyHunter.State.Investigate:
				this.StateInvestigate();
				return;
			case EnemyHunter.State.InvestigateWalk:
				this.StateInvestigateWalk();
				return;
			case EnemyHunter.State.Aim:
				this.AimLogic();
				this.StateAim();
				return;
			case EnemyHunter.State.Shoot:
				this.StateShoot();
				return;
			case EnemyHunter.State.ShootEnd:
				this.StateShootEnd();
				return;
			case EnemyHunter.State.LeaveStart:
				this.StateLeaveStart();
				return;
			case EnemyHunter.State.Leave:
				this.StateLeave();
				return;
			case EnemyHunter.State.Despawn:
				this.StateDespawn();
				return;
			case EnemyHunter.State.Stun:
				this.StateStun();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x0600036E RID: 878 RVA: 0x00021F68 File Offset: 0x00020168
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
			this.UpdateState(EnemyHunter.State.Idle);
		}
	}

	// Token: 0x0600036F RID: 879 RVA: 0x00021FB8 File Offset: 0x000201B8
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = Random.Range(2f, 8f);
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
			this.UpdateState(EnemyHunter.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyHunter.State.LeaveStart);
		}
	}

	// Token: 0x06000370 RID: 880 RVA: 0x00022060 File Offset: 0x00020260
	private void StateRoam()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
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
				this.enemy.NavMeshAgent.SetDestination(navMeshHit.position);
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			this.pitCheckTimer = 0.1f;
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateImpulse = false;
		}
		else
		{
			if (this.enemy.Rigidbody.notMovingTimer > 1f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.PitCheckLogic())
			{
				this.enemy.NavMeshAgent.ResetPath();
			}
			if (this.stateTimer <= 0f || !this.enemy.NavMeshAgent.HasPath())
			{
				this.UpdateState(EnemyHunter.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyHunter.State.LeaveStart);
		}
	}

	// Token: 0x06000371 RID: 881 RVA: 0x000221EC File Offset: 0x000203EC
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
	}

	// Token: 0x06000372 RID: 882 RVA: 0x0002223D File Offset: 0x0002043D
	private void StateStun()
	{
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyHunter.State.Idle);
		}
	}

	// Token: 0x06000373 RID: 883 RVA: 0x00022254 File Offset: 0x00020454
	private void StateInvestigate()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			float num = 12f;
			Vector3 direction = this.investigatePoint - this.investigateRayTransform.position;
			bool flag = false;
			if (direction.magnitude < num && !this.investigatePathfindOnly)
			{
				flag = true;
				foreach (RaycastHit raycastHit in Physics.RaycastAll(this.investigateRayTransform.position, direction, direction.magnitude, SemiFunc.LayerMaskGetVisionObstruct()))
				{
					if (Vector3.Distance(this.investigatePoint, raycastHit.point) >= 1f && !raycastHit.transform.CompareTag("Player") && !raycastHit.transform.GetComponent<EnemyRigidbody>())
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
				this.enemy.NavMeshAgent.ResetPath();
				this.UpdateState(EnemyHunter.State.Aim);
				return;
			}
			this.UpdateState(EnemyHunter.State.InvestigateWalk);
		}
	}

	// Token: 0x06000374 RID: 884 RVA: 0x00022378 File Offset: 0x00020578
	private void StateInvestigateWalk()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.SetDestination(this.investigatePoint);
			this.pitCheckTimer = 0f;
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		else
		{
			if (this.enemy.Rigidbody.notMovingTimer > 1f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.PitCheckLogic())
			{
				this.enemy.NavMeshAgent.ResetPath();
			}
			if (this.stateTimer <= 0f || !this.enemy.NavMeshAgent.HasPath())
			{
				this.UpdateState(EnemyHunter.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyHunter.State.LeaveStart);
		}
	}

	// Token: 0x06000375 RID: 885 RVA: 0x00022454 File Offset: 0x00020654
	private void StateAim()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			this.stateImpulse = false;
			this.investigatePointSpread = Vector3.zero;
			this.investigatePointSpreadTarget = Vector3.zero;
			this.investigatePointSpreadTimer = 0f;
			if (this.shootFast)
			{
				this.stateTimer = 0.5f;
			}
			else
			{
				this.stateTimer = Random.Range(0.25f, 1f);
			}
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyHunter.State.Shoot);
		}
	}

	// Token: 0x06000376 RID: 886 RVA: 0x00022550 File Offset: 0x00020750
	private void StateShoot()
	{
		if (this.stateImpulse)
		{
			Vector3 vector = this.gunAimTransform.position + this.gunAimTransform.forward * 50f;
			float radius = 1f;
			if (this.shootFast)
			{
				radius = 1.5f;
			}
			if (Vector3.Distance(base.transform.position, this.investigatePoint) > 10f)
			{
				radius = 0.5f;
			}
			bool flag = false;
			foreach (RaycastHit raycastHit in Physics.SphereCastAll(this.gunAimTransform.position, radius, this.gunAimTransform.forward, 50f, LayerMask.GetMask(new string[]
			{
				"Player"
			}) + LayerMask.GetMask(new string[]
			{
				"PhysGrabObject"
			})))
			{
				PlayerAvatar playerAvatar = null;
				bool flag2 = false;
				if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
				{
					flag2 = true;
					PlayerController componentInParent = raycastHit.transform.GetComponentInParent<PlayerController>();
					if (componentInParent)
					{
						playerAvatar = componentInParent.playerAvatarScript;
					}
					else
					{
						playerAvatar = raycastHit.transform.GetComponentInParent<PlayerAvatar>();
					}
				}
				else
				{
					PlayerTumble componentInParent2 = raycastHit.transform.GetComponentInParent<PlayerTumble>();
					if (componentInParent2)
					{
						playerAvatar = componentInParent2.playerAvatar;
						flag2 = true;
					}
				}
				if (flag2)
				{
					bool flag3 = true;
					if (raycastHit.point != Vector3.zero)
					{
						Vector3 direction = raycastHit.point - this.gunAimTransform.position;
						foreach (RaycastHit raycastHit2 in Physics.RaycastAll(this.gunAimTransform.position, direction, direction.magnitude, SemiFunc.LayerMaskGetVisionObstruct()))
						{
							if (raycastHit2.transform.gameObject.layer != LayerMask.NameToLayer("Player") && (raycastHit2.transform.gameObject.layer != LayerMask.NameToLayer("PhysGrabObject") || !base.GetComponentInParent<PlayerTumble>()))
							{
								flag3 = false;
								break;
							}
						}
					}
					if (flag3)
					{
						if (raycastHit.point != Vector3.zero)
						{
							vector = raycastHit.point;
							flag = true;
							break;
						}
						if (playerAvatar)
						{
							vector = playerAvatar.PlayerVisionTarget.VisionTransform.position;
							flag = true;
							break;
						}
						break;
					}
				}
			}
			RaycastHit raycastHit3;
			if (!flag && Physics.Raycast(this.gunAimTransform.position, this.gunAimTransform.forward, out raycastHit3, 50f, SemiFunc.LayerMaskGetVisionObstruct() + LayerMask.GetMask(new string[]
			{
				"Enemy"
			})))
			{
				vector = raycastHit3.point;
			}
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("ShootRPC", RpcTarget.All, new object[]
				{
					vector
				});
			}
			else
			{
				this.ShootRPC(vector, default(PhotonMessageInfo));
			}
			this.stateImpulse = false;
			this.stateTimer = 2f;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyHunter.State.ShootEnd);
		}
	}

	// Token: 0x06000377 RID: 887 RVA: 0x000228AC File Offset: 0x00020AAC
	private void StateShootEnd()
	{
		if (this.stateImpulse)
		{
			this.shotsFired++;
			this.stateImpulse = false;
			this.stateTimer = 2f;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.shotsFired >= this.shotsFiredMax)
			{
				this.UpdateState(EnemyHunter.State.LeaveStart);
				return;
			}
			this.UpdateState(EnemyHunter.State.Idle);
		}
	}

	// Token: 0x06000378 RID: 888 RVA: 0x00022954 File Offset: 0x00020B54
	private void StateLeaveStart()
	{
		if (this.stateImpulse)
		{
			this.shotsFired++;
			this.stateImpulse = false;
			this.stateTimer = 3f;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyHunter.State.Leave);
		}
	}

	// Token: 0x06000379 RID: 889 RVA: 0x000229E8 File Offset: 0x00020BE8
	private void StateLeave()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			if (!this.enemy.EnemyParent.playerClose)
			{
				this.UpdateState(EnemyHunter.State.Idle);
				return;
			}
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, 25f, 50f, false);
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f);
			}
			NavMeshHit navMeshHit;
			if (levelPoint && NavMesh.SamplePosition(levelPoint.transform.position, out navMeshHit, 5f, -1) && Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.leavePosition = navMeshHit.position;
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			this.stateImpulse = false;
			this.stateTimer = 5f;
		}
		this.enemy.NavMeshAgent.SetDestination(this.leavePosition);
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		if (this.PitCheckLogic())
		{
			this.stateImpulse = true;
			return;
		}
		if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.leavePosition) < 1f)
		{
			this.UpdateState(EnemyHunter.State.Idle);
		}
	}

	// Token: 0x0600037A RID: 890 RVA: 0x00022B7D File Offset: 0x00020D7D
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyHunter.State.Spawn);
		}
	}

	// Token: 0x0600037B RID: 891 RVA: 0x00022B9C File Offset: 0x00020D9C
	public void OnHurt()
	{
		this.soundHurt.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		this.enemyHunterAnim.StopHumming(1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemyHunter.State.Leave)
		{
			this.UpdateState(EnemyHunter.State.Idle);
		}
	}

	// Token: 0x0600037C RID: 892 RVA: 0x00022C04 File Offset: 0x00020E04
	public void OnDeath()
	{
		this.soundDeath.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		foreach (ParticleSystem particleSystem in this.deathEffects)
		{
			particleSystem.Play();
		}
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x0600037D RID: 893 RVA: 0x00022CF8 File Offset: 0x00020EF8
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.enemy.Rigidbody.timeSinceStun > 1.5f)
		{
			if (this.currentState == EnemyHunter.State.Idle || this.currentState == EnemyHunter.State.Roam || this.currentState == EnemyHunter.State.InvestigateWalk || (this.currentState == EnemyHunter.State.ShootEnd && this.shotsFired < this.shotsFiredMax) || (this.currentState == EnemyHunter.State.Leave && this.leaveInterruptCounter >= 3))
			{
				this.investigatePathfindOnly = this.enemy.StateInvestigate.onInvestigateTriggeredPathfindOnly;
				this.investigatePoint = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
				if (Vector3.Distance(this.investigatePoint, base.transform.position) < 4f && !this.investigatePathfindOnly)
				{
					this.shootFast = true;
				}
				else
				{
					this.shootFast = false;
				}
				this.InvestigateTransformGet();
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("UpdateInvestigationPoint", RpcTarget.Others, new object[]
					{
						this.investigatePoint
					});
				}
				this.UpdateState(EnemyHunter.State.Investigate);
				return;
			}
			if (this.currentState == EnemyHunter.State.Leave && Vector3.Distance(base.transform.position, this.enemy.StateInvestigate.onInvestigateTriggeredPosition) < 5f)
			{
				this.leaveInterruptCounter++;
				this.leaveInterruptTimer = 3f;
			}
		}
	}

	// Token: 0x0600037E RID: 894 RVA: 0x00022E5C File Offset: 0x0002105C
	public void OnTouchPlayer()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.enemy.Rigidbody.timeSinceStun > 1.5f && (this.currentState == EnemyHunter.State.Idle || this.currentState == EnemyHunter.State.Roam || this.currentState == EnemyHunter.State.InvestigateWalk || this.currentState == EnemyHunter.State.ShootEnd || this.currentState == EnemyHunter.State.LeaveStart || this.currentState == EnemyHunter.State.Leave))
		{
			this.shootFast = true;
			this.investigatePoint = this.enemy.Rigidbody.onTouchPlayerAvatar.PlayerVisionTarget.VisionTransform.position;
			this.investigatePointHasTransform = true;
			this.investigatePointTransform = this.enemy.Rigidbody.onTouchPlayerAvatar.PlayerVisionTarget.VisionTransform;
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("UpdateInvestigationPoint", RpcTarget.Others, new object[]
				{
					this.investigatePoint
				});
			}
			this.UpdateState(EnemyHunter.State.Aim);
		}
	}

	// Token: 0x0600037F RID: 895 RVA: 0x00022F50 File Offset: 0x00021150
	public void OnTouchPlayerGrabbedObject()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.enemy.Rigidbody.timeSinceStun > 1.5f && (this.currentState == EnemyHunter.State.Idle || this.currentState == EnemyHunter.State.Roam || this.currentState == EnemyHunter.State.InvestigateWalk || this.currentState == EnemyHunter.State.ShootEnd || this.currentState == EnemyHunter.State.LeaveStart || this.currentState == EnemyHunter.State.Leave))
		{
			this.shootFast = true;
			this.investigatePoint = this.enemy.Rigidbody.onTouchPlayerGrabbedObjectPosition;
			this.investigatePointHasTransform = true;
			this.investigatePointTransform = this.enemy.Rigidbody.onTouchPlayerGrabbedObjectAvatar.PlayerVisionTarget.VisionTransform;
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("UpdateInvestigationPoint", RpcTarget.Others, new object[]
				{
					this.investigatePoint
				});
			}
			this.UpdateState(EnemyHunter.State.Aim);
		}
	}

	// Token: 0x06000380 RID: 896 RVA: 0x00023030 File Offset: 0x00021230
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.enemy.Rigidbody.timeSinceStun > 1.5f && (this.currentState == EnemyHunter.State.Idle || this.currentState == EnemyHunter.State.Roam || this.currentState == EnemyHunter.State.InvestigateWalk || this.currentState == EnemyHunter.State.ShootEnd || this.currentState == EnemyHunter.State.LeaveStart || this.currentState == EnemyHunter.State.Leave))
		{
			this.shootFast = true;
			this.investigatePoint = this.enemy.Rigidbody.onGrabbedPlayerAvatar.PlayerVisionTarget.VisionTransform.position;
			this.investigatePointHasTransform = true;
			this.investigatePointTransform = this.enemy.Rigidbody.onGrabbedPlayerAvatar.PlayerVisionTarget.VisionTransform;
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("UpdateInvestigationPoint", RpcTarget.Others, new object[]
				{
					this.investigatePoint
				});
			}
			this.UpdateState(EnemyHunter.State.Aim);
		}
	}

	// Token: 0x06000381 RID: 897 RVA: 0x00023124 File Offset: 0x00021324
	private void UpdateState(EnemyHunter.State _state)
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

	// Token: 0x06000382 RID: 898 RVA: 0x00023198 File Offset: 0x00021398
	private void AimLogic()
	{
		if (!this.investigatePointTransform)
		{
			this.investigatePointHasTransform = false;
		}
		Vector3 vector = this.investigatePoint;
		if (this.investigatePointHasTransform)
		{
			Vector3 a = this.investigatePointTransform.position - this.investigatePointTransformPrevious;
			a.y = 0f;
			vector = this.investigatePointTransform.position + a * 25f;
			this.investigatePointTransformPrevious = this.investigatePointTransform.position;
		}
		if (this.investigatePointSpreadTimer <= 0f)
		{
			Vector3 vector2 = Random.insideUnitSphere * Random.Range(0f, 0.5f);
			if (Vector3.Distance(base.transform.position, vector) > 10f)
			{
				vector2 = Random.insideUnitSphere * Random.Range(0.5f, 1f);
			}
			this.investigatePointSpreadTimer = Random.Range(0.1f, 0.5f);
			this.investigatePointSpreadTarget = vector2;
		}
		else
		{
			this.investigatePointSpreadTimer -= Time.deltaTime;
		}
		this.investigatePointSpread = Vector3.Lerp(this.investigatePointSpread, this.investigatePointSpreadTarget, Time.deltaTime * 20f);
		vector += this.investigatePointSpread;
		float num = 5f;
		if (this.shootFast)
		{
			num = 20f;
		}
		this.investigatePoint = Vector3.Lerp(this.investigatePoint, vector, num * Time.deltaTime);
		Vector3 position = base.transform.position;
		base.transform.position += this.gunAimTransform.position - this.verticalAimTransform.position;
		Quaternion rotation = base.transform.rotation;
		base.transform.LookAt(this.investigatePoint);
		base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y, 0f);
		Quaternion rotation2 = base.transform.rotation;
		base.transform.rotation = rotation;
		base.transform.position = position;
		this.investigateAimHorizontal = rotation2;
		Vector3 position2 = this.verticalAimTransform.position;
		this.verticalAimTransform.position += this.gunAimTransform.position - this.verticalAimTransform.position;
		this.verticalAimTransform.LookAt(this.investigatePoint);
		float num2 = 45f;
		float num3 = this.verticalAimTransform.localEulerAngles.x;
		if (num3 < 180f)
		{
			num3 = Mathf.Clamp(num3, 0f, num2);
		}
		else
		{
			num3 = Mathf.Clamp(num3, 360f - num2, 360f);
		}
		this.verticalAimTransform.localEulerAngles = new Vector3(num3, 0f, 0f);
		Quaternion localRotation = this.verticalAimTransform.localRotation;
		this.verticalAimTransform.position = position2;
		this.investigateAimVertical = localRotation;
		if (SemiFunc.IsMultiplayer())
		{
			if (this.investigateAimVerticalRPCTimer <= 0f)
			{
				if (this.investigateAimVerticalPrevious != this.investigateAimVertical)
				{
					this.investigateAimVerticalRPCTimer = 1f;
					this.photonView.RPC("UpdateVerticalAimRPC", RpcTarget.Others, new object[]
					{
						this.investigateAimVertical
					});
					this.investigateAimVerticalPrevious = this.investigateAimVertical;
					return;
				}
			}
			else
			{
				this.investigateAimVerticalRPCTimer -= Time.deltaTime;
			}
		}
	}

	// Token: 0x06000383 RID: 899 RVA: 0x00023500 File Offset: 0x00021700
	private void HorizontalRotationLogic()
	{
		if (this.currentState == EnemyHunter.State.Idle || this.currentState == EnemyHunter.State.Roam || this.currentState == EnemyHunter.State.InvestigateWalk || this.currentState == EnemyHunter.State.LeaveStart || this.currentState == EnemyHunter.State.Leave)
		{
			this.horizontalAimSpring.damping = 0.7f;
			this.horizontalAimSpring.speed = 3f;
			if (this.enemy.NavMeshAgent.AgentVelocity.magnitude > 0.01f)
			{
				Quaternion rotation = base.transform.rotation;
				base.transform.rotation = Quaternion.LookRotation(this.enemy.NavMeshAgent.AgentVelocity.normalized);
				base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y, 0f);
				Quaternion rotation2 = base.transform.rotation;
				base.transform.rotation = rotation;
				this.horizontalAimTarget = rotation2;
			}
		}
		else if (this.currentState == EnemyHunter.State.Aim)
		{
			if (this.shootFast)
			{
				this.horizontalAimSpring.damping = 0.9f;
				this.horizontalAimSpring.speed = 30f;
			}
			else
			{
				this.horizontalAimSpring.damping = 0.8f;
				this.horizontalAimSpring.speed = 20f;
			}
			this.horizontalAimTarget = this.investigateAimHorizontal;
		}
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.horizontalAimSpring, this.horizontalAimTarget, -1f);
	}

	// Token: 0x06000384 RID: 900 RVA: 0x0002367C File Offset: 0x0002187C
	private void VerticalRotationLogic()
	{
		if (this.currentState == EnemyHunter.State.Aim || this.currentState == EnemyHunter.State.Shoot)
		{
			this.verticalAimTransform.localRotation = SemiFunc.SpringQuaternionGet(this.verticalAimSpring, this.investigateAimVertical, -1f);
			return;
		}
		this.verticalAimTransform.localRotation = SemiFunc.SpringQuaternionGet(this.verticalAimSpring, Quaternion.identity, -1f);
	}

	// Token: 0x06000385 RID: 901 RVA: 0x000236E0 File Offset: 0x000218E0
	private bool PitCheckLogic()
	{
		if (this.pitCheckTimer <= 0f && this.enemy.NavMeshAgent.AgentVelocity.normalized.magnitude > 0.1f)
		{
			this.pitCheckTimer = 0.5f;
			Vector3 normalized = this.enemy.NavMeshAgent.AgentVelocity.normalized;
			normalized.y = 0f;
			bool flag = Physics.Raycast(base.transform.position + normalized + Vector3.up * 1f, Vector3.down, 5f, SemiFunc.LayerMaskGetVisionObstruct());
			if (!flag)
			{
				this.enemy.NavMeshAgent.Warp(base.transform.position - normalized * 0.5f);
				this.enemy.NavMeshAgent.ResetPath();
				this.enemy.NavMeshAgent.Agent.velocity = Vector3.zero;
			}
			return !flag;
		}
		this.pitCheckTimer -= Time.deltaTime;
		return false;
	}

	// Token: 0x06000386 RID: 902 RVA: 0x000237FE File Offset: 0x000219FE
	private void HurtColliderTimer()
	{
		if (this.hurtColliderTimer > 0f)
		{
			this.hurtColliderTimer -= Time.deltaTime;
			if (this.hurtColliderTimer <= 0f)
			{
				this.hurtCollider.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000387 RID: 903 RVA: 0x00023840 File Offset: 0x00021A40
	private void LineRendererLogic()
	{
		if (this.lineRendererActive)
		{
			this.lineRenderer.widthMultiplier = this.lineRendererWidthCurve.Evaluate(this.lineRendererLerp);
			this.lineRendererLerp += Time.deltaTime * 5f;
			if (this.lineRendererLerp >= 1f)
			{
				this.lineRenderer.gameObject.SetActive(false);
				this.lineRendererActive = false;
			}
		}
	}

	// Token: 0x06000388 RID: 904 RVA: 0x000238AE File Offset: 0x00021AAE
	private void ShotsFiredLogic()
	{
		if (this.currentState == EnemyHunter.State.Spawn || this.currentState == EnemyHunter.State.Leave)
		{
			this.shotsFired = 0;
		}
	}

	// Token: 0x06000389 RID: 905 RVA: 0x000238CC File Offset: 0x00021ACC
	private void InvestigateTransformGet()
	{
		this.investigatePointHasTransform = false;
		if (this.investigatePathfindOnly)
		{
			return;
		}
		foreach (Collider collider in Physics.OverlapSphere(this.investigatePoint, 1.5f, LayerMask.GetMask(new string[]
		{
			"Player"
		}) + LayerMask.GetMask(new string[]
		{
			"PhysGrabObject"
		})))
		{
			if (collider.CompareTag("Player"))
			{
				PlayerController componentInParent = collider.GetComponentInParent<PlayerController>();
				if (componentInParent)
				{
					this.investigatePointHasTransform = true;
					this.investigatePointTransform = componentInParent.playerAvatarScript.PlayerVisionTarget.VisionTransform;
				}
				else
				{
					PlayerAvatar componentInParent2 = collider.GetComponentInParent<PlayerAvatar>();
					if (componentInParent2)
					{
						this.investigatePointHasTransform = true;
						this.investigatePointTransform = componentInParent2.PlayerVisionTarget.VisionTransform;
					}
				}
			}
			else
			{
				PlayerTumble componentInParent3 = collider.GetComponentInParent<PlayerTumble>();
				if (componentInParent3)
				{
					this.investigatePointHasTransform = true;
					this.investigatePointTransform = componentInParent3.playerAvatar.PlayerVisionTarget.VisionTransform;
				}
			}
		}
	}

	// Token: 0x0600038A RID: 906 RVA: 0x000239CF File Offset: 0x00021BCF
	private void LeaveInterruptLogic()
	{
		if (this.currentState != EnemyHunter.State.Leave)
		{
			this.leaveInterruptTimer = 0f;
			return;
		}
		if (this.leaveInterruptTimer <= 0f)
		{
			this.leaveInterruptCounter = 0;
			return;
		}
		this.leaveInterruptTimer -= Time.deltaTime;
	}

	// Token: 0x0600038B RID: 907 RVA: 0x00023A0E File Offset: 0x00021C0E
	[PunRPC]
	private void UpdateStateRPC(EnemyHunter.State _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
		if (this.currentState == EnemyHunter.State.Spawn)
		{
			this.enemyHunterAnim.OnSpawn();
		}
		if (this.currentState == EnemyHunter.State.Stun)
		{
			this.enemyHunterAnim.StopHumming(1f);
		}
	}

	// Token: 0x0600038C RID: 908 RVA: 0x00023A4D File Offset: 0x00021C4D
	[PunRPC]
	private void UpdateVerticalAimRPC(Quaternion _rotation, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.investigateAimVertical = _rotation;
	}

	// Token: 0x0600038D RID: 909 RVA: 0x00023A60 File Offset: 0x00021C60
	[PunRPC]
	private void ShootRPC(Vector3 _hitPosition, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		Vector3 vector = _hitPosition - this.gunTipTransform.position;
		this.lineRenderer.gameObject.SetActive(true);
		this.lineRenderer.SetPosition(0, this.gunTipTransform.position);
		this.lineRenderer.SetPosition(1, this.gunTipTransform.position + vector.normalized * 0.5f);
		this.lineRenderer.SetPosition(2, _hitPosition - vector.normalized * 0.5f);
		this.lineRenderer.SetPosition(3, _hitPosition);
		this.lineRendererActive = true;
		this.lineRendererLerp = 0f;
		this.hurtCollider.transform.position = _hitPosition;
		this.hurtCollider.transform.rotation = Quaternion.LookRotation(this.gunTipTransform.forward);
		this.hurtCollider.gameObject.SetActive(true);
		this.hurtColliderTimer = 0.25f;
		this.shootEffectTransform.position = this.gunTipTransform.position;
		this.shootEffectTransform.rotation = this.gunTipTransform.rotation;
		foreach (ParticleSystem particleSystem in this.shootEffects)
		{
			particleSystem.Play();
		}
		this.hitEffectTransform.position = _hitPosition;
		this.hitEffectTransform.rotation = this.gunTipTransform.rotation;
		foreach (ParticleSystem particleSystem2 in this.hitEffects)
		{
			particleSystem2.Play();
		}
		this.enemyHunterAlwaysActive.Trigger();
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 15f, this.gunTipTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 15f, this.gunTipTransform.position, 0.05f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, _hitPosition, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, _hitPosition, 0.05f);
		this.soundShoot.Play(this.gunTipTransform.position, 1f, 1f, 1f, 1f);
		this.soundShootGlobal.Play(this.gunTipTransform.position, 1f, 1f, 1f, 1f);
		this.soundHit.Play(_hitPosition, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600038E RID: 910 RVA: 0x00023D64 File Offset: 0x00021F64
	[PunRPC]
	private void UpdateInvestigationPoint(Vector3 _point, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.investigatePoint = _point;
	}

	// Token: 0x04000606 RID: 1542
	public bool debugSpawn;

	// Token: 0x04000607 RID: 1543
	[Space]
	public EnemyHunter.State currentState;

	// Token: 0x04000608 RID: 1544
	private bool stateImpulse;

	// Token: 0x04000609 RID: 1545
	internal float stateTimer;

	// Token: 0x0400060A RID: 1546
	[Space]
	public Enemy enemy;

	// Token: 0x0400060B RID: 1547
	public EnemyHunterAnim enemyHunterAnim;

	// Token: 0x0400060C RID: 1548
	public EnemyHunterAlwaysActive enemyHunterAlwaysActive;

	// Token: 0x0400060D RID: 1549
	private PhotonView photonView;

	// Token: 0x0400060E RID: 1550
	public Transform investigateRayTransform;

	// Token: 0x0400060F RID: 1551
	public Transform verticalAimTransform;

	// Token: 0x04000610 RID: 1552
	public Transform gunAimTransform;

	// Token: 0x04000611 RID: 1553
	public Transform gunTipTransform;

	// Token: 0x04000612 RID: 1554
	public HurtCollider hurtCollider;

	// Token: 0x04000613 RID: 1555
	private float hurtColliderTimer;

	// Token: 0x04000614 RID: 1556
	private bool shootFast;

	// Token: 0x04000615 RID: 1557
	[Space]
	public LineRenderer lineRenderer;

	// Token: 0x04000616 RID: 1558
	public AnimationCurve lineRendererWidthCurve;

	// Token: 0x04000617 RID: 1559
	private float lineRendererLerp;

	// Token: 0x04000618 RID: 1560
	private bool lineRendererActive;

	// Token: 0x04000619 RID: 1561
	[Space]
	public SpringQuaternion horizontalAimSpring;

	// Token: 0x0400061A RID: 1562
	private Quaternion horizontalAimTarget = Quaternion.identity;

	// Token: 0x0400061B RID: 1563
	public SpringQuaternion verticalAimSpring;

	// Token: 0x0400061C RID: 1564
	private float pitCheckTimer;

	// Token: 0x0400061D RID: 1565
	private int shotsFired;

	// Token: 0x0400061E RID: 1566
	private int shotsFiredMax = 4;

	// Token: 0x0400061F RID: 1567
	private Vector3 leavePosition;

	// Token: 0x04000620 RID: 1568
	private Vector3 investigatePoint;

	// Token: 0x04000621 RID: 1569
	private bool investigatePathfindOnly;

	// Token: 0x04000622 RID: 1570
	private Quaternion investigateAimHorizontal = Quaternion.identity;

	// Token: 0x04000623 RID: 1571
	private Quaternion investigateAimVertical = Quaternion.identity;

	// Token: 0x04000624 RID: 1572
	private Quaternion investigateAimVerticalPrevious = Quaternion.identity;

	// Token: 0x04000625 RID: 1573
	private float investigateAimVerticalRPCTimer;

	// Token: 0x04000626 RID: 1574
	private bool investigatePointHasTransform;

	// Token: 0x04000627 RID: 1575
	private Transform investigatePointTransform;

	// Token: 0x04000628 RID: 1576
	private Vector3 investigatePointTransformPrevious;

	// Token: 0x04000629 RID: 1577
	private Vector3 investigatePointSpread;

	// Token: 0x0400062A RID: 1578
	private Vector3 investigatePointSpreadTarget;

	// Token: 0x0400062B RID: 1579
	private float investigatePointSpreadTimer;

	// Token: 0x0400062C RID: 1580
	private int leaveInterruptCounter;

	// Token: 0x0400062D RID: 1581
	private float leaveInterruptTimer;

	// Token: 0x0400062E RID: 1582
	private float tripTimer;

	// Token: 0x0400062F RID: 1583
	[Space]
	public Transform shootEffectTransform;

	// Token: 0x04000630 RID: 1584
	public List<ParticleSystem> shootEffects;

	// Token: 0x04000631 RID: 1585
	[Space]
	public Transform hitEffectTransform;

	// Token: 0x04000632 RID: 1586
	public List<ParticleSystem> hitEffects;

	// Token: 0x04000633 RID: 1587
	[Space]
	public List<ParticleSystem> deathEffects;

	// Token: 0x04000634 RID: 1588
	[Space]
	public Sound soundHurt;

	// Token: 0x04000635 RID: 1589
	public Sound soundDeath;

	// Token: 0x04000636 RID: 1590
	public Sound soundShoot;

	// Token: 0x04000637 RID: 1591
	public Sound soundShootGlobal;

	// Token: 0x04000638 RID: 1592
	public Sound soundHit;

	// Token: 0x02000317 RID: 791
	public enum State
	{
		// Token: 0x040028C6 RID: 10438
		Spawn,
		// Token: 0x040028C7 RID: 10439
		Idle,
		// Token: 0x040028C8 RID: 10440
		Roam,
		// Token: 0x040028C9 RID: 10441
		Investigate,
		// Token: 0x040028CA RID: 10442
		InvestigateWalk,
		// Token: 0x040028CB RID: 10443
		Aim,
		// Token: 0x040028CC RID: 10444
		Shoot,
		// Token: 0x040028CD RID: 10445
		ShootEnd,
		// Token: 0x040028CE RID: 10446
		LeaveStart,
		// Token: 0x040028CF RID: 10447
		Leave,
		// Token: 0x040028D0 RID: 10448
		Despawn,
		// Token: 0x040028D1 RID: 10449
		Stun
	}
}
