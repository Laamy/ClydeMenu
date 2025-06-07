using System;
using UnityEngine;

// Token: 0x02000091 RID: 145
public class EnemyStateChase : MonoBehaviour
{
	// Token: 0x06000602 RID: 1538 RVA: 0x0003B17E File Offset: 0x0003937E
	private void Awake()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x0003B198 File Offset: 0x00039398
	private void Update()
	{
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		if (this.Enemy.CurrentState != EnemyState.Chase)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.Enemy.TargetPlayerAvatar.LastNavMeshPositionTimer = 0f;
			this.ChasePosition = this.Enemy.TargetPlayerAvatar.transform.position;
			this.VisionTimer = this.VisionTime;
			this.ChaseCanReachSet = false;
			this.SawPlayerHide = false;
			this.CantReachTime = 0f;
			this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
			this.Active = true;
		}
		this.Enemy.SetChaseTimer();
		this.Enemy.NavMeshAgent.UpdateAgent(this.Speed, this.Acceleration);
		if (this.Enemy.Vision.VisionTriggered[this.Enemy.TargetPlayerAvatar.photonView.ViewID])
		{
			this.VisionTimer = this.VisionTime;
		}
		else if (this.VisionTimer > 0f)
		{
			this.VisionTimer -= Time.deltaTime;
		}
		if (this.VisionTimer > 0f)
		{
			if (this.ChaseOnlyOnNavmesh || this.Enemy.TargetPlayerAvatar.LastNavMeshPositionTimer <= 0.25f)
			{
				this.Enemy.NavMeshAgent.Enable();
				this.Enemy.NavMeshAgent.SetDestination(this.Enemy.TargetPlayerAvatar.LastNavmeshPosition);
				if (this.ChaseCanReachSet)
				{
					Vector3 point = this.Enemy.NavMeshAgent.GetPoint();
					if (Vector3.Distance(point, this.Enemy.TargetPlayerAvatar.transform.position) > 0.5f)
					{
						this.ChaseCanReach = false;
					}
					else
					{
						this.ChaseCanReach = true;
					}
					if (this.Enemy.TargetPlayerAvatar.isCrawling && !this.ChaseCanReach && SemiFunc.EnemyLookUnderCondition(this.Enemy, this.StateTimer, 5f, this.Enemy.TargetPlayerAvatar))
					{
						this.SawPlayerHidePosition = this.Enemy.TargetPlayerAvatar.transform.position;
						this.SawPlayerNavmeshPosition = this.Enemy.TargetPlayerAvatar.LastNavmeshPosition;
						this.SawPlayerHide = true;
					}
					this.ChasePosition = point;
				}
				this.ChaseCanReachSet = true;
			}
			else
			{
				this.Enemy.NavMeshAgent.Disable(0.1f);
				base.transform.position = Vector3.MoveTowards(base.transform.position, this.Enemy.TargetPlayerAvatar.transform.position, this.Speed * Time.deltaTime);
			}
		}
		else
		{
			if (this.SawPlayerHide)
			{
				this.Enemy.CurrentState = EnemyState.LookUnder;
				return;
			}
			this.Enemy.NavMeshAgent.SetDestination(this.ChasePosition);
			if (Vector3.Distance(base.transform.position, this.ChasePosition) < 1f)
			{
				LevelPoint levelPointAhead = this.Enemy.GetLevelPointAhead(this.ChasePosition);
				if (levelPointAhead)
				{
					this.Enemy.NavMeshAgent.SetDestination(levelPointAhead.transform.position);
				}
				this.ChasePosition = this.Enemy.NavMeshAgent.GetDestination();
			}
			this.ChaseCanReach = true;
			this.ChaseCanReachSet = false;
		}
		if (this.ChaseCanReach && this.Enemy.Vision.VisionsTriggered[this.Enemy.TargetPlayerAvatar.photonView.ViewID] >= this.VisionsToReset)
		{
			this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
		}
		if (!this.ChaseCanReach)
		{
			this.CantReachTime += Time.deltaTime;
			if (this.CantReachTime > 2f)
			{
				this.Enemy.Vision.VisionsTriggered[this.Enemy.TargetPlayerAvatar.photonView.ViewID] = 0;
				this.Enemy.CurrentState = EnemyState.ChaseSlow;
				return;
			}
		}
		else
		{
			this.CantReachTime = 0f;
		}
		this.StateTimer -= Time.deltaTime;
		if (this.StateTimer <= 0f)
		{
			this.Enemy.CurrentState = EnemyState.ChaseSlow;
		}
		if (this.Enemy.TargetPlayerAvatar.isDisabled)
		{
			this.Enemy.Vision.VisionsTriggered[this.Enemy.TargetPlayerAvatar.photonView.ViewID] = 0;
			this.Enemy.CurrentState = EnemyState.Roaming;
		}
	}

	// Token: 0x040009B9 RID: 2489
	private Enemy Enemy;

	// Token: 0x040009BA RID: 2490
	private PlayerController Player;

	// Token: 0x040009BB RID: 2491
	private bool Active;

	// Token: 0x040009BC RID: 2492
	public float Speed;

	// Token: 0x040009BD RID: 2493
	public float Acceleration;

	// Token: 0x040009BE RID: 2494
	[Space]
	public float StateTimeMin;

	// Token: 0x040009BF RID: 2495
	public float StateTimeMax;

	// Token: 0x040009C0 RID: 2496
	private float StateTimer;

	// Token: 0x040009C1 RID: 2497
	[Space]
	public float VisionTime;

	// Token: 0x040009C2 RID: 2498
	[HideInInspector]
	public float VisionTimer;

	// Token: 0x040009C3 RID: 2499
	public int VisionsToReset;

	// Token: 0x040009C4 RID: 2500
	[HideInInspector]
	public Vector3 ChasePosition = Vector3.zero;

	// Token: 0x040009C5 RID: 2501
	[HideInInspector]
	public bool ChaseCanReach = true;

	// Token: 0x040009C6 RID: 2502
	private bool ChaseCanReachSet;

	// Token: 0x040009C7 RID: 2503
	private bool SawPlayerHide;

	// Token: 0x040009C8 RID: 2504
	internal Vector3 SawPlayerNavmeshPosition;

	// Token: 0x040009C9 RID: 2505
	internal Vector3 SawPlayerHidePosition;

	// Token: 0x040009CA RID: 2506
	private float CantReachTime;

	// Token: 0x040009CB RID: 2507
	[Space]
	public bool ChaseOnlyOnNavmesh = true;
}
