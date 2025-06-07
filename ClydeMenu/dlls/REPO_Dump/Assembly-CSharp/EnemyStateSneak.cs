using System;
using UnityEngine;

// Token: 0x02000099 RID: 153
public class EnemyStateSneak : MonoBehaviour
{
	// Token: 0x06000627 RID: 1575 RVA: 0x0003C9EE File Offset: 0x0003ABEE
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x0003CA08 File Offset: 0x0003AC08
	private void Update()
	{
		if (this.Enemy.CurrentState != EnemyState.Sneak)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.TargetPlayer = PlayerController.instance.playerAvatarScript;
			if (GameManager.instance.gameMode == 1)
			{
				foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
				{
					if (!playerAvatar.isDisabled && playerAvatar.photonView.ViewID == this.Enemy.TargetPlayerViewID)
					{
						this.TargetPlayer = playerAvatar;
					}
				}
			}
			this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
			this.Active = true;
		}
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		this.Enemy.NavMeshAgent.UpdateAgent(this.Speed, this.Acceleration);
		this.Enemy.NavMeshAgent.SetDestination(this.TargetPlayer.transform.position);
		if (this.Enemy.HasRigidbody)
		{
			this.Enemy.Rigidbody.IdleSet(0.1f);
		}
		if (this.Enemy.Vision.VisionsTriggered[this.Enemy.TargetPlayerAvatar.photonView.ViewID] >= this.Enemy.Vision.VisionsToTrigger)
		{
			this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
		}
		this.StateTimer -= Time.deltaTime;
		if (this.StateTimer <= 0f)
		{
			this.Enemy.CurrentState = EnemyState.Roaming;
		}
	}

	// Token: 0x04000A2C RID: 2604
	private Enemy Enemy;

	// Token: 0x04000A2D RID: 2605
	private PlayerController Player;

	// Token: 0x04000A2E RID: 2606
	private bool Active;

	// Token: 0x04000A2F RID: 2607
	public float Speed;

	// Token: 0x04000A30 RID: 2608
	public float Acceleration;

	// Token: 0x04000A31 RID: 2609
	[Space]
	public float StateTimeMin;

	// Token: 0x04000A32 RID: 2610
	public float StateTimeMax;

	// Token: 0x04000A33 RID: 2611
	private float StateTimer;

	// Token: 0x04000A34 RID: 2612
	private PlayerAvatar TargetPlayer;
}
