using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000098 RID: 152
public class EnemyStateRoaming : MonoBehaviour
{
	// Token: 0x0600061F RID: 1567 RVA: 0x0003C3A9 File Offset: 0x0003A5A9
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x0003C3C4 File Offset: 0x0003A5C4
	private void Update()
	{
		if (GameDirector.instance.currentState < GameDirector.gameState.Main)
		{
			return;
		}
		if (this.Enemy.CurrentState != EnemyState.Roaming)
		{
			if (this.Active)
			{
				this.RoamingLevelPoint = null;
				this.RoamingCooldown = 0f;
				this.RoamingChangeCurrent = 0;
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.PhysObjectHitImpulse = true;
			this.PhysObjectHitCount = 0;
			this.Active = true;
		}
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		if (this.Enemy.HasRigidbody)
		{
			this.Enemy.Rigidbody.IdleSet(0.1f);
		}
		this.Enemy.NavMeshAgent.UpdateAgent(this.Speed, this.Acceleration);
		this.PlayerNear();
		this.PlayerFar();
		this.PlayerTurn();
		this.PickPath();
		this.Stuck();
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x0003C4A0 File Offset: 0x0003A6A0
	private void PlayerNear()
	{
		if (SemiFunc.EnemyForceLeave(this.Enemy))
		{
			this.PlayerFarTimer = 0f;
			this.PlayerFarTime = Random.Range(this.PlayerFarTimeMin, this.PlayerFarTimeMax);
			this.PlayerFarMove = true;
			this.RoamingChangeCurrent = 0;
			return;
		}
		if (this.Enemy.PlayerDistance.PlayerDistanceClosest <= this.PlayerNearDistance)
		{
			this.PlayerNearTimer += Time.deltaTime;
		}
		else
		{
			this.PlayerNearTimer -= this.PlayerNearDecrease * Time.deltaTime;
			this.PlayerNearTimer = Mathf.Max(this.PlayerNearTimer, 0f);
		}
		if (this.PlayerNearTimer >= this.PlayerNearTimeMax)
		{
			this.PlayerFarTimer = 0f;
			this.PlayerFarTime = Random.Range(this.PlayerFarTimeMin, this.PlayerFarTimeMax);
			this.PlayerFarMove = true;
			this.RoamingChangeCurrent = 0;
		}
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x0003C584 File Offset: 0x0003A784
	private void PlayerFar()
	{
		if (this.PlayerFarMove)
		{
			this.PlayerFarTimer += Time.deltaTime;
			if (this.PlayerFarTimer >= this.PlayerFarTime)
			{
				this.PlayerFarMove = false;
				this.PlayerFarTimer = 0f;
			}
		}
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x0003C5C0 File Offset: 0x0003A7C0
	private void PlayerTurn()
	{
		if (this.RoamingOnScreenCooldownTimer > 0f)
		{
			this.RoamingOnScreenCooldownTimer -= Time.deltaTime;
			return;
		}
		if (this.RoamingTurnWaitTimer > 0f)
		{
			this.RoamingCooldown = 1f;
			this.RoamingTurnWaitTimer -= Time.deltaTime;
			if (this.RoamingTurnWaitTimer <= 0f)
			{
				this.RoamingChangeCurrent = Random.Range(this.RoamingChangeMin, this.RoamingChangeMax + 1);
				this.RoamingLevelPoint = this.Enemy.GetLevelPointAhead(this.RoamingTurnPlayer.transform.position);
				this.RoamingCooldown = 0f;
				this.RoamingOnScreenCooldownTimer = this.RoamingOnScreenCooldown;
				return;
			}
		}
		else if (this.Enemy.OnScreen.OnScreenAny)
		{
			this.RoamingOnScreenTimer += Time.deltaTime;
			if (this.RoamingOnScreenTimer >= this.RoamingOnScreenTime)
			{
				if (GameManager.instance.gameMode == 1)
				{
					List<PlayerAvatar> list = new List<PlayerAvatar>();
					foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
					{
						if (!playerAvatar.isDisabled && this.Enemy.OnScreen.OnScreenPlayer[playerAvatar.photonView.ViewID])
						{
							list.Add(playerAvatar);
						}
					}
					if (list.Count <= 0)
					{
						this.RoamingOnScreenTimer = 0f;
						return;
					}
					this.RoamingTurnPlayer = list[Random.Range(0, list.Count)];
				}
				else
				{
					this.RoamingTurnPlayer = PlayerController.instance.playerAvatarScript;
				}
				this.RoamingOnScreenTimer = 0f;
				this.RoamingTurnWaitTimer = this.RoamingTurnWaitTime;
				this.Enemy.NavMeshAgent.ResetPath();
				this.RoamingCooldown = 1f;
				return;
			}
		}
		else
		{
			this.RoamingOnScreenTimer -= Time.deltaTime;
			this.RoamingOnScreenTimer = Mathf.Clamp01(this.RoamingOnScreenTimer);
		}
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x0003C7D0 File Offset: 0x0003A9D0
	private void PickPath()
	{
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		if (!this.Enemy.NavMeshAgent.HasPath())
		{
			if (this.RoamingCooldown <= 0f || !this.RoamingLevelPoint)
			{
				LevelPoint levelPoint = this.RoamingLevelPoint;
				if (this.RoamingChangeCurrent <= 0 || !this.RoamingLevelPoint)
				{
					if (this.PlayerFarMove)
					{
						levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f);
					}
					else
					{
						levelPoint = LevelGenerator.Instance.LevelPathPoints[Random.Range(0, LevelGenerator.Instance.LevelPathPoints.Count)];
					}
					this.RoamingChangeCurrent = Random.Range(this.RoamingChangeMin, this.RoamingChangeMax + 1);
				}
				else
				{
					this.RoamingChangeCurrent--;
				}
				if (levelPoint)
				{
					Vector3 vector = levelPoint.transform.position;
					vector += Random.insideUnitSphere * Random.Range(this.RoamingPathRadiusMin, this.RoamingPathRadiusMax);
					if (this.Enemy.NavMeshAgent.CalculatePath(vector).status == NavMeshPathStatus.PathComplete)
					{
						this.RoamingCooldown = Random.Range(this.RoamingCooldownMin, this.RoamingCooldownMax);
						this.RoamingLevelPoint = levelPoint;
						this.RoamingTargetPosition = vector;
						this.Enemy.NavMeshAgent.SetDestination(this.RoamingTargetPosition);
						return;
					}
				}
			}
			else
			{
				this.RoamingCooldown -= Time.deltaTime;
			}
		}
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x0003C944 File Offset: 0x0003AB44
	private void Stuck()
	{
		if (this.Enemy.NavMeshAgent.HasPath())
		{
			this.Enemy.AttackStuckPhysObject.Check();
			if (this.Enemy.AttackStuckPhysObject.Active)
			{
				if (this.PhysObjectHitImpulse)
				{
					this.PhysObjectHitCount++;
					this.PhysObjectHitImpulse = false;
				}
			}
			else
			{
				this.PhysObjectHitImpulse = true;
			}
			if (this.PhysObjectHitCount >= this.PhysObjectHitMax)
			{
				this.PhysObjectHitImpulse = true;
				this.PhysObjectHitCount = 0;
				this.Enemy.NavMeshAgent.ResetPath();
				this.RoamingChangeCurrent = 1;
			}
		}
	}

	// Token: 0x04000A07 RID: 2567
	private Enemy Enemy;

	// Token: 0x04000A08 RID: 2568
	private PlayerController Player;

	// Token: 0x04000A09 RID: 2569
	private bool Active;

	// Token: 0x04000A0A RID: 2570
	[Header("Movement")]
	public float Speed;

	// Token: 0x04000A0B RID: 2571
	public float Acceleration;

	// Token: 0x04000A0C RID: 2572
	[Header("Roaming")]
	public float RoamingCooldownMin;

	// Token: 0x04000A0D RID: 2573
	public float RoamingCooldownMax;

	// Token: 0x04000A0E RID: 2574
	internal LevelPoint RoamingLevelPoint;

	// Token: 0x04000A0F RID: 2575
	private Vector3 RoamingTargetPosition;

	// Token: 0x04000A10 RID: 2576
	internal float RoamingCooldown;

	// Token: 0x04000A11 RID: 2577
	[Space]
	public float RoamingPathRadiusMin;

	// Token: 0x04000A12 RID: 2578
	public float RoamingPathRadiusMax;

	// Token: 0x04000A13 RID: 2579
	[Space]
	public int RoamingChangeMin;

	// Token: 0x04000A14 RID: 2580
	public int RoamingChangeMax;

	// Token: 0x04000A15 RID: 2581
	internal int RoamingChangeCurrent;

	// Token: 0x04000A16 RID: 2582
	private Vector3 RoamingStuckPosition;

	// Token: 0x04000A17 RID: 2583
	[Space]
	public float RoamingTeleportChance;

	// Token: 0x04000A18 RID: 2584
	[Space]
	public float RoamingOnScreenTime;

	// Token: 0x04000A19 RID: 2585
	private float RoamingOnScreenTimer;

	// Token: 0x04000A1A RID: 2586
	private float RoamingOnScreenCooldownTimer;

	// Token: 0x04000A1B RID: 2587
	public float RoamingOnScreenCooldown;

	// Token: 0x04000A1C RID: 2588
	private float RoamingTurnWaitTimer;

	// Token: 0x04000A1D RID: 2589
	public float RoamingTurnWaitTime;

	// Token: 0x04000A1E RID: 2590
	private PlayerAvatar RoamingTurnPlayer;

	// Token: 0x04000A1F RID: 2591
	[Header("Player Near")]
	public float PlayerNearTimeMax;

	// Token: 0x04000A20 RID: 2592
	public float PlayerNearDistance;

	// Token: 0x04000A21 RID: 2593
	public float PlayerNearDecrease;

	// Token: 0x04000A22 RID: 2594
	private float PlayerNearTimer;

	// Token: 0x04000A23 RID: 2595
	[Header("Player Far")]
	public float PlayerFarTimeMin;

	// Token: 0x04000A24 RID: 2596
	public float PlayerFarTimeMax;

	// Token: 0x04000A25 RID: 2597
	private float PlayerFarTime;

	// Token: 0x04000A26 RID: 2598
	public float PlayerFarDistance;

	// Token: 0x04000A27 RID: 2599
	private float PlayerFarTimer;

	// Token: 0x04000A28 RID: 2600
	private bool PlayerFarMove;

	// Token: 0x04000A29 RID: 2601
	[Header("Phys Object")]
	public int PhysObjectHitMax = 3;

	// Token: 0x04000A2A RID: 2602
	private bool PhysObjectHitImpulse;

	// Token: 0x04000A2B RID: 2603
	private int PhysObjectHitCount;
}
