using System;
using UnityEngine;

// Token: 0x02000093 RID: 147
public class EnemyStateChaseEnd : MonoBehaviour
{
	// Token: 0x06000608 RID: 1544 RVA: 0x0003B998 File Offset: 0x00039B98
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x0003B9B4 File Offset: 0x00039BB4
	private void Update()
	{
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		if (this.Enemy.CurrentState != EnemyState.ChaseEnd)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.Enemy.NavMeshAgent.ResetPath();
			this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
			this.Active = true;
		}
		this.Enemy.NavMeshAgent.UpdateAgent(0f, 5f);
		this.StateTimer -= Time.deltaTime;
		if (this.StateTimer <= 0f)
		{
			this.Enemy.CurrentState = EnemyState.Roaming;
		}
	}

	// Token: 0x040009D5 RID: 2517
	private Enemy Enemy;

	// Token: 0x040009D6 RID: 2518
	private PlayerController Player;

	// Token: 0x040009D7 RID: 2519
	private bool Active;

	// Token: 0x040009D8 RID: 2520
	public float StateTimeMin;

	// Token: 0x040009D9 RID: 2521
	public float StateTimeMax;

	// Token: 0x040009DA RID: 2522
	private float StateTimer;
}
