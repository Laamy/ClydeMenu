using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000088 RID: 136
public class EnemyValuableThrower : MonoBehaviour
{
	// Token: 0x0600059B RID: 1435 RVA: 0x000374B1 File Offset: 0x000356B1
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x000374CC File Offset: 0x000356CC
	private void Update()
	{
		if (this.currentState == EnemyValuableThrower.State.GetValuable || this.currentState == EnemyValuableThrower.State.GoToTarget || this.currentState == EnemyValuableThrower.State.PickUpTarget || this.currentState == EnemyValuableThrower.State.TargetPlayer || this.currentState == EnemyValuableThrower.State.Throw)
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
			if (this.enemy.IsStunned() || !LevelGenerator.Instance.Generated)
			{
				return;
			}
			switch (this.currentState)
			{
			case EnemyValuableThrower.State.Spawn:
				this.StateSpawn();
				break;
			case EnemyValuableThrower.State.Idle:
				this.StateIdle();
				this.AgentVelocityRotation();
				break;
			case EnemyValuableThrower.State.Roam:
				this.StateRoam();
				this.AgentVelocityRotation();
				break;
			case EnemyValuableThrower.State.Investigate:
				this.StateInvestigate();
				this.AgentVelocityRotation();
				break;
			case EnemyValuableThrower.State.PlayerNotice:
				this.StatePlayerNotice();
				this.PlayerLookAt();
				break;
			case EnemyValuableThrower.State.GetValuable:
				this.StateGetValuable();
				break;
			case EnemyValuableThrower.State.GoToTarget:
				this.ValuableFailsafe();
				this.TargetFailsafe();
				this.AgentVelocityRotation();
				this.StateGoToTarget();
				break;
			case EnemyValuableThrower.State.PickUpTarget:
				this.DropOnStun();
				this.TargetFailsafe();
				this.ValuableTargetFollow();
				this.StatePickUpTarget();
				break;
			case EnemyValuableThrower.State.TargetPlayer:
				this.DropOnStun();
				this.TargetFailsafe();
				this.PlayerLookAt();
				this.ValuableTargetFollow();
				this.StateTargetPlayer();
				break;
			case EnemyValuableThrower.State.Throw:
				this.DropOnStun();
				this.TargetFailsafe();
				this.PlayerLookAt();
				this.ValuableTargetFollow();
				this.StateThrow();
				break;
			case EnemyValuableThrower.State.Leave:
				this.AgentVelocityRotation();
				this.StateLeave();
				break;
			}
			this.pickupTargetParent.position = this.enemy.Rigidbody.transform.position;
			Quaternion b = Quaternion.Euler(0f, this.enemy.Rigidbody.transform.rotation.eulerAngles.y, 0f);
			this.pickupTargetParent.rotation = Quaternion.Slerp(this.pickupTargetParent.rotation, b, 5f * Time.deltaTime);
		}
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x00037770 File Offset: 0x00035970
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
			this.UpdateState(EnemyValuableThrower.State.Idle);
		}
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x000377C0 File Offset: 0x000359C0
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
			this.UpdateState(EnemyValuableThrower.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyValuableThrower.State.Leave);
		}
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x0003786C File Offset: 0x00035A6C
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
			this.UpdateState(EnemyValuableThrower.State.Idle);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyValuableThrower.State.Leave);
		}
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x000379FC File Offset: 0x00035BFC
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
			if ((this.stateTimer <= 0f || Vector3.Distance(this.enemy.Rigidbody.transform.position, this.agentDestination) < 2f) && !this.enemy.Jump.jumping)
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyValuableThrower.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyValuableThrower.State.Leave);
		}
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x00037AF4 File Offset: 0x00035CF4
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
			this.UpdateState(EnemyValuableThrower.State.GetValuable);
		}
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x00037B7C File Offset: 0x00035D7C
	private void StateGetValuable()
	{
		if (this.currentState != EnemyValuableThrower.State.GetValuable)
		{
			return;
		}
		this.valuableTarget = null;
		PhysGrabObject exists = null;
		PhysGrabObject exists2 = null;
		float num = 999f;
		float num2 = 999f;
		Collider[] array = Physics.OverlapSphere(this.playerTarget.transform.position, 10f, LayerMask.GetMask(new string[]
		{
			"PhysGrabObject"
		}));
		for (int i = 0; i < array.Length; i++)
		{
			ValuableObject componentInParent = array[i].GetComponentInParent<ValuableObject>();
			if (componentInParent && componentInParent.volumeType <= ValuableVolume.Type.Big)
			{
				float num3 = Vector3.Distance(this.playerTarget.transform.position, componentInParent.transform.position);
				NavMeshHit navMeshHit;
				if (NavMesh.SamplePosition(componentInParent.transform.position, out navMeshHit, 1f, -1))
				{
					if (num3 < num2)
					{
						num2 = num3;
						exists2 = componentInParent.physGrabObject;
					}
				}
				else if (num3 < num)
				{
					num = num3;
					exists = componentInParent.physGrabObject;
				}
			}
		}
		if (exists2)
		{
			this.valuableTarget = exists2;
		}
		else if (exists)
		{
			this.valuableTarget = exists;
		}
		if (!this.valuableTarget)
		{
			this.UpdateState(EnemyValuableThrower.State.Leave);
			return;
		}
		this.UpdateState(EnemyValuableThrower.State.GoToTarget);
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x00037CB0 File Offset: 0x00035EB0
	private void StateGoToTarget()
	{
		if (this.enemy.IsStunned() || !this.valuableTarget)
		{
			return;
		}
		this.enemy.NavMeshAgent.SetDestination(this.valuableTarget.transform.position);
		if (this.stateImpulse)
		{
			this.stateTimer = 5f;
			this.stateImpulse = false;
			return;
		}
		SemiFunc.EnemyCartJump(this.enemy);
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.valuableTarget.transform.position) < 1.25f)
		{
			this.enemy.NavMeshAgent.ResetPath();
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyValuableThrower.State.PickUpTarget);
		}
		else if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetDestination()) < 1f)
		{
			if (this.stateTimer <= 0f)
			{
				this.enemy.Jump.StuckReset();
				this.UpdateState(EnemyValuableThrower.State.Leave);
			}
			else if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.valuableTarget.centerPoint) > 1.5f)
			{
				this.enemy.Jump.StuckTrigger(this.valuableTarget.transform.position - this.enemy.Rigidbody.transform.position);
				this.enemy.Rigidbody.DisableFollowPosition(1f, 10f);
			}
		}
		if (this.enemy.Rigidbody.notMovingTimer > 2f || Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyValuableThrower.State.Leave);
			}
		}
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x00037EC4 File Offset: 0x000360C4
	private void StatePickUpTarget()
	{
		if (this.currentState != EnemyValuableThrower.State.PickUpTarget)
		{
			return;
		}
		if (this.stateImpulse)
		{
			foreach (PhysGrabber physGrabber in Enumerable.ToList<PhysGrabber>(this.valuableTarget.playerGrabbing))
			{
				if (!SemiFunc.IsMultiplayer())
				{
					physGrabber.ReleaseObject(0.1f);
				}
				else
				{
					physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
					{
						false,
						0.1f
					});
				}
			}
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			Quaternion to = Quaternion.Euler(0f, Quaternion.LookRotation(this.pickUpPosition - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 180f * Time.deltaTime);
			this.pickUpPosition = this.valuableTarget.midPoint;
			this.stateTimer = 999f;
			this.stateImpulse = false;
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyValuableThrower.State.TargetPlayer);
		}
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x00038048 File Offset: 0x00036248
	private void StateTargetPlayer()
	{
		if (this.currentState != EnemyValuableThrower.State.TargetPlayer)
		{
			return;
		}
		if (this.stateImpulse)
		{
			this.stateTimer = 10f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		Vector3 direction = this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position;
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(this.enemy.Rigidbody.transform.position, direction, out raycastHit, direction.magnitude, SemiFunc.LayerMaskGetVisionObstruct());
		if (flag && (raycastHit.transform.CompareTag("Player") || raycastHit.transform.GetComponent<PlayerTumble>()))
		{
			flag = false;
		}
		if (!flag && Vector3.Distance(base.transform.position, this.playerTarget.transform.position) < 3f)
		{
			this.enemy.NavMeshAgent.SetDestination(base.transform.position - base.transform.forward * 3f);
		}
		else if (flag || Vector3.Distance(base.transform.position, this.playerTarget.transform.position) > 5f)
		{
			this.enemy.NavMeshAgent.SetDestination(this.playerTarget.transform.position);
		}
		else
		{
			this.enemy.NavMeshAgent.ResetPath();
			if (this.stateTimer <= 8f)
			{
				this.UpdateState(EnemyValuableThrower.State.Throw);
			}
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyValuableThrower.State.Throw);
		}
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x000381FC File Offset: 0x000363FC
	private void StateThrow()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.attacks++;
			this.stateTimer = 3f;
			this.stateImpulse = false;
		}
		if (!this.valuableTarget)
		{
			this.stateTimer = Mathf.Clamp(this.stateTimer, this.stateTimer, 1f);
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.attacks >= 3 || Random.Range(0f, 1f) <= 0.3f)
			{
				this.attacks = 0;
				this.UpdateState(EnemyValuableThrower.State.Leave);
				return;
			}
			this.UpdateState(EnemyValuableThrower.State.GetValuable);
		}
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x000382C0 File Offset: 0x000364C0
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
			this.stateTimer = 10f;
			this.stateImpulse = false;
			SemiFunc.EnemyLeaveStart(this.enemy);
			return;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		SemiFunc.EnemyCartJump(this.enemy);
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
		{
			this.UpdateState(EnemyValuableThrower.State.Idle);
		}
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x000383DB File Offset: 0x000365DB
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyValuableThrower.State.Spawn);
		}
		if (this.anim.isActiveAndEnabled)
		{
			this.anim.OnSpawn();
		}
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x00038410 File Offset: 0x00036610
	public void OnHurt()
	{
		this.anim.hurtSound.Play(this.anim.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x00038448 File Offset: 0x00036648
	public void OnDeath()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.anim.particleImpact.Play();
		this.anim.particleBits.Play();
		this.anim.particleDirectionalBits.transform.rotation = Quaternion.LookRotation(-this.enemy.Health.hurtDirection.normalized);
		this.anim.particleDirectionalBits.Play();
		this.anim.deathSound.Play(this.anim.transform.position, 1f, 1f, 1f, 1f);
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x0003855C File Offset: 0x0003675C
	public void OnVisionTriggered()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.currentState != EnemyValuableThrower.State.Idle && this.currentState != EnemyValuableThrower.State.Roam && this.currentState != EnemyValuableThrower.State.Investigate && this.currentState != EnemyValuableThrower.State.Leave)
			{
				return;
			}
			if (this.playerTarget != this.enemy.Vision.onVisionTriggeredPlayer)
			{
				this.playerTarget = this.enemy.Vision.onVisionTriggeredPlayer;
				if (GameManager.Multiplayer())
				{
					this.photonView.RPC("UpdatePlayerTargetRPC", RpcTarget.Others, new object[]
					{
						this.playerTarget.photonView.ViewID
					});
				}
			}
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
					this.anim.NoticeSet(this.enemy.Vision.onVisionTriggeredID);
				}
			}
			this.UpdateState(EnemyValuableThrower.State.PlayerNotice);
		}
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x00038669 File Offset: 0x00036869
	public void OnInvestigate()
	{
		if (this.currentState == EnemyValuableThrower.State.Roam || this.currentState == EnemyValuableThrower.State.Idle || this.currentState == EnemyValuableThrower.State.Investigate)
		{
			this.UpdateState(EnemyValuableThrower.State.Investigate);
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
		}
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x000386A4 File Offset: 0x000368A4
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				return;
			}
			if (this.currentState == EnemyValuableThrower.State.Leave)
			{
				this.grabAggroTimer = 60f;
				if (this.playerTarget != this.enemy.Rigidbody.onGrabbedPlayerAvatar)
				{
					this.playerTarget = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("UpdatePlayerTargetRPC", RpcTarget.Others, new object[]
						{
							this.playerTarget.photonView.ViewID
						});
					}
				}
				this.UpdateState(EnemyValuableThrower.State.PlayerNotice);
				if (!this.enemy.IsStunned())
				{
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("NoticeRPC", RpcTarget.All, new object[]
						{
							this.playerTarget.photonView.ViewID
						});
						return;
					}
					this.anim.NoticeSet(this.playerTarget.photonView.ViewID);
				}
			}
		}
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x000387B0 File Offset: 0x000369B0
	private void UpdateState(EnemyValuableThrower.State _state)
	{
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

	// Token: 0x060005AF RID: 1455 RVA: 0x00038804 File Offset: 0x00036A04
	private void ValuableTargetFollow()
	{
		if (!this.valuableTarget)
		{
			return;
		}
		if (Vector3.Distance(this.valuableTarget.transform.position, this.pickupTarget.position) > 2f)
		{
			this.valuableTarget = null;
			this.UpdateState(EnemyValuableThrower.State.Leave);
			return;
		}
		Vector3 midPoint = this.valuableTarget.midPoint;
		midPoint.y = this.valuableTarget.transform.position.y;
		Vector3 position = this.pickupTarget.position;
		this.valuableTarget.OverrideZeroGravity(0.1f);
		this.valuableTarget.OverrideMass(0.5f, 0.1f);
		this.valuableTarget.OverrideIndestructible(0.1f);
		this.valuableTarget.OverrideBreakEffects(0.1f);
		if (Mathf.Abs(midPoint.y - position.y) > 0.25f)
		{
			Vector3 vector = this.enemy.Rigidbody.transform.position + this.enemy.Rigidbody.transform.forward;
			position = new Vector3(vector.x, position.y, vector.z);
		}
		Vector3 a = SemiFunc.PhysFollowPosition(midPoint, position, this.valuableTarget.rb.velocity, 5f);
		this.valuableTarget.rb.AddForce(a * (5f * Time.fixedDeltaTime), ForceMode.Impulse);
		Vector3 a2 = SemiFunc.PhysFollowRotation(this.valuableTarget.transform, this.pickupTarget.rotation, this.valuableTarget.rb, 0.5f);
		this.valuableTarget.rb.AddTorque(a2 * (5f * Time.fixedDeltaTime), ForceMode.Impulse);
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x000389C4 File Offset: 0x00036BC4
	private void AgentVelocityRotation()
	{
		if (this.enemy.NavMeshAgent.AgentVelocity.magnitude > 0.05f)
		{
			Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.enemy.NavMeshAgent.AgentVelocity.normalized).eulerAngles.y, 0f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 5f * Time.deltaTime);
		}
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x00038A4C File Offset: 0x00036C4C
	private void PlayerLookAt()
	{
		Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 50f * Time.deltaTime);
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x00038AD1 File Offset: 0x00036CD1
	private void ValuableFailsafe()
	{
		if (!this.valuableTarget)
		{
			this.UpdateState(EnemyValuableThrower.State.GetValuable);
		}
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x00038AE7 File Offset: 0x00036CE7
	private void TargetFailsafe()
	{
		if (!this.playerTarget || this.playerTarget.isDisabled)
		{
			this.UpdateState(EnemyValuableThrower.State.Leave);
		}
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x00038B0B File Offset: 0x00036D0B
	private void DropOnStun()
	{
		if (this.enemy.IsStunned())
		{
			this.UpdateState(EnemyValuableThrower.State.GoToTarget);
		}
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x00038B21 File Offset: 0x00036D21
	public void ResetStateTimer()
	{
		this.stateTimer = 0f;
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x00038B30 File Offset: 0x00036D30
	public void Throw()
	{
		if (!this.valuableTarget)
		{
			return;
		}
		if (!this.playerTarget)
		{
			return;
		}
		foreach (PhysGrabber physGrabber in Enumerable.ToList<PhysGrabber>(this.valuableTarget.playerGrabbing))
		{
			if (!SemiFunc.IsMultiplayer())
			{
				physGrabber.ReleaseObject(0.1f);
			}
			else
			{
				physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
				{
					false,
					0.1f
				});
			}
		}
		Vector3 vector = this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.valuableTarget.centerPoint;
		vector = Vector3.Lerp(base.transform.forward, vector, 0.5f);
		this.valuableTarget.ResetMass();
		float num = 20f * this.valuableTarget.rb.mass;
		num = Mathf.Min(num, 100f);
		this.valuableTarget.ResetIndestructible();
		this.valuableTarget.rb.AddForce(vector * num, ForceMode.Impulse);
		this.valuableTarget.rb.AddTorque(this.valuableTarget.transform.right * 0.5f, ForceMode.Impulse);
		this.valuableTarget.impactDetector.PlayerHurtMultiplier(5f, 2f);
		this.valuableTarget = null;
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x00038CC0 File Offset: 0x00036EC0
	[PunRPC]
	private void UpdateStateRPC(EnemyValuableThrower.State _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x00038CD2 File Offset: 0x00036ED2
	[PunRPC]
	private void NoticeRPC(int _playerID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.anim.NoticeSet(_playerID);
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x00038CEC File Offset: 0x00036EEC
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
				break;
			}
		}
	}

	// Token: 0x040008E7 RID: 2279
	private PhotonView photonView;

	// Token: 0x040008E8 RID: 2280
	public EnemyValuableThrower.State currentState;

	// Token: 0x040008E9 RID: 2281
	private bool stateImpulse;

	// Token: 0x040008EA RID: 2282
	public float stateTimer;

	// Token: 0x040008EB RID: 2283
	private Vector3 agentDestination;

	// Token: 0x040008EC RID: 2284
	private int attacks;

	// Token: 0x040008ED RID: 2285
	[Space]
	public EnemyValuableThrowerAnim anim;

	// Token: 0x040008EE RID: 2286
	public Transform pickupTargetParent;

	// Token: 0x040008EF RID: 2287
	public Transform pickupTarget;

	// Token: 0x040008F0 RID: 2288
	private Enemy enemy;

	// Token: 0x040008F1 RID: 2289
	private PlayerAvatar playerTarget;

	// Token: 0x040008F2 RID: 2290
	private PhysGrabObject valuableTarget;

	// Token: 0x040008F3 RID: 2291
	private Vector3 pickUpPosition;

	// Token: 0x040008F4 RID: 2292
	private float grabAggroTimer;

	// Token: 0x02000324 RID: 804
	public enum State
	{
		// Token: 0x0400295E RID: 10590
		Spawn,
		// Token: 0x0400295F RID: 10591
		Idle,
		// Token: 0x04002960 RID: 10592
		Roam,
		// Token: 0x04002961 RID: 10593
		Investigate,
		// Token: 0x04002962 RID: 10594
		PlayerNotice,
		// Token: 0x04002963 RID: 10595
		GetValuable,
		// Token: 0x04002964 RID: 10596
		GoToTarget,
		// Token: 0x04002965 RID: 10597
		PickUpTarget,
		// Token: 0x04002966 RID: 10598
		TargetPlayer,
		// Token: 0x04002967 RID: 10599
		Throw,
		// Token: 0x04002968 RID: 10600
		Leave
	}
}
