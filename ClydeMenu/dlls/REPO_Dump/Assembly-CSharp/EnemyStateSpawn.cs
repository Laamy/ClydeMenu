using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200009A RID: 154
public class EnemyStateSpawn : MonoBehaviour
{
	// Token: 0x0600062A RID: 1578 RVA: 0x0003CBD8 File Offset: 0x0003ADD8
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x0003CBE8 File Offset: 0x0003ADE8
	private void Update()
	{
		if (this.Enemy.CurrentState != EnemyState.Spawn)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.WaitTimer = 2f;
			this.Active = true;
		}
		if (this.WaitTimer <= 0f)
		{
			this.Enemy.CurrentState = EnemyState.Roaming;
			this.WaitTimer = 0f;
			return;
		}
		this.WaitTimer -= Time.deltaTime;
	}

	// Token: 0x04000A35 RID: 2613
	private Enemy Enemy;

	// Token: 0x04000A36 RID: 2614
	private bool Active;

	// Token: 0x04000A37 RID: 2615
	private float WaitTimer;

	// Token: 0x04000A38 RID: 2616
	public UnityEvent OnSpawn;
}
