using System;
using UnityEngine;

// Token: 0x02000092 RID: 146
public class EnemyStateChaseBegin : MonoBehaviour
{
	// Token: 0x06000605 RID: 1541 RVA: 0x0003B651 File Offset: 0x00039851
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x0003B66C File Offset: 0x0003986C
	private void Update()
	{
		if (this.Enemy.CurrentState != EnemyState.ChaseBegin)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			if (this.Enemy.MasterClient)
			{
				this.Enemy.StateChase.ChaseCanReach = true;
				this.Enemy.NavMeshAgent.ResetPath();
				this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
			}
			this.TargetPlayer = PlayerController.instance.playerAvatarScript;
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				if (!playerAvatar.isDisabled && playerAvatar.photonView.ViewID == this.Enemy.TargetPlayerViewID)
				{
					this.TargetPlayer = playerAvatar;
				}
			}
			foreach (PlayerAvatar playerAvatar2 in GameDirector.instance.PlayerList)
			{
				if (!playerAvatar2.isDisabled && playerAvatar2.isLocal)
				{
					if (GameManager.instance.gameMode == 0 || this.TargetPlayer == playerAvatar2 || this.Enemy.PlayerRoom.SameLocal || this.Enemy.OnScreen.OnScreenLocal)
					{
						this.LocalEffect = true;
						GameDirector.instance.CameraImpact.Shake(5f, 0.25f);
						GameDirector.instance.CameraShake.Shake(3f, 0.5f);
						if (this.Stinger)
						{
							CameraGlitch.Instance.PlayShort();
							AudioScare.instance.PlayImpact();
						}
					}
					else
					{
						this.LocalEffect = false;
						GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.25f);
						GameDirector.instance.CameraShake.ShakeDistance(3f, 5f, 10f, base.transform.position, 0.5f);
					}
				}
			}
			this.Active = true;
		}
		this.Enemy.SetChaseTimer();
		if (this.Enemy.MasterClient)
		{
			this.Enemy.NavMeshAgent.UpdateAgent(0f, 5f);
			this.Enemy.NavMeshAgent.Stop(0.1f);
			base.transform.LookAt(this.TargetPlayer.transform.position);
			base.transform.localEulerAngles = new Vector3(0f, base.transform.localEulerAngles.y, 0f);
			this.StateTimer -= Time.deltaTime;
			if (this.StateTimer <= 0f)
			{
				this.Enemy.CurrentState = EnemyState.Chase;
			}
		}
	}

	// Token: 0x040009CC RID: 2508
	private Enemy Enemy;

	// Token: 0x040009CD RID: 2509
	private PlayerController Player;

	// Token: 0x040009CE RID: 2510
	[HideInInspector]
	public bool Active;

	// Token: 0x040009CF RID: 2511
	[Space]
	public float StateTimeMin;

	// Token: 0x040009D0 RID: 2512
	public float StateTimeMax;

	// Token: 0x040009D1 RID: 2513
	private float StateTimer;

	// Token: 0x040009D2 RID: 2514
	[Space]
	internal PlayerAvatar TargetPlayer;

	// Token: 0x040009D3 RID: 2515
	[HideInInspector]
	public bool LocalEffect;

	// Token: 0x040009D4 RID: 2516
	[Space]
	public bool Stinger;
}
