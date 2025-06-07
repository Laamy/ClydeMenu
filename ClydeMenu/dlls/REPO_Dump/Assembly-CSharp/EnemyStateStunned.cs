using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200009B RID: 155
public class EnemyStateStunned : MonoBehaviour
{
	// Token: 0x0600062D RID: 1581 RVA: 0x0003CC6C File Offset: 0x0003AE6C
	private void Start()
	{
		this.enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x0003CC7C File Offset: 0x0003AE7C
	private void Update()
	{
		if (this.stunTimer > 0f)
		{
			this.enemy.CurrentState = EnemyState.Stunned;
			this.stunTimer -= Time.deltaTime;
			if (this.stunTimer <= 0f)
			{
				this.enemy.CurrentState = EnemyState.Roaming;
			}
		}
		if (this.enemy.CurrentState != EnemyState.Stunned)
		{
			if (this.active)
			{
				if (this.enemy.HasRigidbody && this.enemy.HasNavMeshAgent)
				{
					this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
				}
				this.onStunnedEnd.Invoke();
				this.active = false;
			}
			return;
		}
		if (!this.active)
		{
			this.onStunnedStart.Invoke();
			this.active = true;
		}
		this.enemy.DisableChase(0.25f);
		if (this.enemy.HasRigidbody)
		{
			this.enemy.Rigidbody.DisableFollowPosition(0.1f, this.enemy.Rigidbody.stunResetSpeed);
			this.enemy.Rigidbody.DisableFollowRotation(0.1f, this.enemy.Rigidbody.stunResetSpeed);
			this.enemy.Rigidbody.DisableNoGravity(0.1f);
			this.enemy.Rigidbody.physGrabObject.OverrideDrag(0.05f, 0.1f);
			this.enemy.Rigidbody.physGrabObject.OverrideAngularDrag(0.05f, 0.1f);
		}
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x0003CE0E File Offset: 0x0003B00E
	public void Spawn()
	{
		this.stunTimer = 0f;
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x0003CE1B File Offset: 0x0003B01B
	public void Set(float time)
	{
		if (time > this.stunTimer && this.enemy.TeleportedTimer <= 0f)
		{
			this.stunTimer = time;
		}
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x0003CE3F File Offset: 0x0003B03F
	public void Reset()
	{
		this.stunTimer = 0.1f;
	}

	// Token: 0x04000A39 RID: 2617
	private Enemy enemy;

	// Token: 0x04000A3A RID: 2618
	private bool active;

	// Token: 0x04000A3B RID: 2619
	[HideInInspector]
	public float stunTimer;

	// Token: 0x04000A3C RID: 2620
	[Space]
	public UnityEvent onStunnedStart;

	// Token: 0x04000A3D RID: 2621
	public UnityEvent onStunnedEnd;
}
