using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000095 RID: 149
public class EnemyStateDespawn : MonoBehaviour
{
	// Token: 0x0600060F RID: 1551 RVA: 0x0003BBE0 File Offset: 0x00039DE0
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x0003BBF0 File Offset: 0x00039DF0
	private void Update()
	{
		if (this.Enemy.MasterClient)
		{
			if (this.ChaseDespawn)
			{
				this.ChaseDespawnLogic();
			}
			if (this.StuckDespawn)
			{
				this.StuckDespawnLogic();
			}
		}
		if (this.Enemy.CurrentState != EnemyState.Despawn)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.Active = true;
		}
		bool masterClient = this.Enemy.MasterClient;
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x0003BC60 File Offset: 0x00039E60
	public void Despawn()
	{
		this.OnDespawn.Invoke();
		this.Enemy.EnemyParent.Despawn();
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x0003BC80 File Offset: 0x00039E80
	private void ChaseDespawnLogic()
	{
		if (this.DespawnAfterChaseTime == 0f)
		{
			return;
		}
		if (this.Enemy.CurrentState == EnemyState.Chase || this.Enemy.CurrentState == EnemyState.ChaseSlow || this.Enemy.CurrentState == EnemyState.LookUnder)
		{
			this.ChaseTimer += Time.deltaTime;
			this.ChaseResetTimer = 10f;
			if (this.ChaseTimer >= this.DespawnAfterChaseTime)
			{
				this.Enemy.CurrentState = EnemyState.Despawn;
				this.ChaseTimer = 0f;
				return;
			}
		}
		else
		{
			if (this.ChaseResetTimer <= 0f)
			{
				this.ChaseTimer = 0f;
				return;
			}
			this.ChaseResetTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x0003BD38 File Offset: 0x00039F38
	private void StuckDespawnLogic()
	{
		if (this.Enemy.StuckCount >= this.StuckDespawnCount && this.Enemy.CurrentState != EnemyState.Despawn)
		{
			this.Enemy.Vision.DisableTimer = 0.25f;
			this.Enemy.CurrentState = EnemyState.Despawn;
		}
	}

	// Token: 0x040009E2 RID: 2530
	private Enemy Enemy;

	// Token: 0x040009E3 RID: 2531
	private bool Active;

	// Token: 0x040009E4 RID: 2532
	public bool StuckDespawn = true;

	// Token: 0x040009E5 RID: 2533
	public int StuckDespawnCount = 10;

	// Token: 0x040009E6 RID: 2534
	[Space]
	public bool ChaseDespawn = true;

	// Token: 0x040009E7 RID: 2535
	public float DespawnAfterChaseTime = 10f;

	// Token: 0x040009E8 RID: 2536
	internal float ChaseTimer;

	// Token: 0x040009E9 RID: 2537
	internal float ChaseResetTimer;

	// Token: 0x040009EA RID: 2538
	[Space]
	public UnityEvent OnDespawn;
}
