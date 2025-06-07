using System;
using UnityEngine;

// Token: 0x02000094 RID: 148
public class EnemyStateChaseSlow : MonoBehaviour
{
	// Token: 0x0600060B RID: 1547 RVA: 0x0003BA70 File Offset: 0x00039C70
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x0003BA80 File Offset: 0x00039C80
	private void Update()
	{
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		if (this.Enemy.CurrentState != EnemyState.ChaseSlow)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.ChaseAhead();
			this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
			this.Active = true;
		}
		this.Enemy.SetChaseTimer();
		this.Enemy.NavMeshAgent.UpdateAgent(this.Speed, this.Acceleration);
		if (Vector3.Distance(base.transform.position, this.Enemy.NavMeshAgent.Agent.destination) < 1f)
		{
			this.ChaseAhead();
		}
		this.StateTimer -= Time.deltaTime;
		if (this.StateTimer <= 0f)
		{
			this.Enemy.CurrentState = EnemyState.ChaseEnd;
		}
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x0003BB6C File Offset: 0x00039D6C
	private void ChaseAhead()
	{
		LevelPoint levelPointAhead = this.Enemy.GetLevelPointAhead(this.Enemy.StateChase.ChasePosition);
		if (levelPointAhead)
		{
			this.Enemy.StateChase.ChasePosition = levelPointAhead.transform.position;
		}
		this.Enemy.NavMeshAgent.SetDestination(this.Enemy.StateChase.ChasePosition);
	}

	// Token: 0x040009DB RID: 2523
	private Enemy Enemy;

	// Token: 0x040009DC RID: 2524
	private bool Active;

	// Token: 0x040009DD RID: 2525
	public float Speed;

	// Token: 0x040009DE RID: 2526
	public float Acceleration;

	// Token: 0x040009DF RID: 2527
	[Space]
	public float StateTimeMin;

	// Token: 0x040009E0 RID: 2528
	public float StateTimeMax;

	// Token: 0x040009E1 RID: 2529
	private float StateTimer;
}
