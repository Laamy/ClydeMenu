using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000097 RID: 151
public class EnemyStateLookUnder : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x0600061B RID: 1563 RVA: 0x0003C18D File Offset: 0x0003A38D
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x0003C1A8 File Offset: 0x0003A3A8
	private void Update()
	{
		if (this.Enemy.CurrentState != EnemyState.LookUnder)
		{
			if (this.Active)
			{
				this.WaitDone = false;
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.WaitDone = false;
			this.WaitTimer = Random.Range(this.WaitTimerMin, this.WaitTimerMax);
			this.LookTimer = Random.Range(this.LookTimerMin, this.LookTimerMax);
			this.Active = true;
		}
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		this.Enemy.SetChaseTimer();
		this.Enemy.NavMeshAgent.SetDestination(this.Enemy.StateChase.SawPlayerNavmeshPosition);
		this.Enemy.NavMeshAgent.UpdateAgent(this.Speed, this.Acceleration);
		if (!this.WaitDone)
		{
			if (Vector3.Distance(base.transform.position, this.Enemy.StateChase.SawPlayerNavmeshPosition) < 0.1f)
			{
				this.WaitTimer -= Time.deltaTime;
				if (this.WaitTimer <= 0f)
				{
					this.WaitDone = true;
				}
			}
		}
		else
		{
			this.LookTimer -= Time.deltaTime;
			if (this.LookTimer <= 0f)
			{
				this.Enemy.CurrentState = EnemyState.ChaseSlow;
			}
		}
		if (this.Enemy.Vision.VisionTriggered[this.Enemy.TargetPlayerAvatar.photonView.ViewID] && this.Enemy.StateChase.SawPlayerNavmeshPosition != this.Enemy.TargetPlayerAvatar.LastNavmeshPosition)
		{
			this.Enemy.CurrentState = EnemyState.Chase;
		}
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x0003C355 File Offset: 0x0003A555
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.WaitDone);
			return;
		}
		this.WaitDone = (bool)stream.ReceiveNext();
	}

	// Token: 0x040009FB RID: 2555
	private Enemy Enemy;

	// Token: 0x040009FC RID: 2556
	private PlayerController Player;

	// Token: 0x040009FD RID: 2557
	private bool Active;

	// Token: 0x040009FE RID: 2558
	public float Speed;

	// Token: 0x040009FF RID: 2559
	public float Acceleration;

	// Token: 0x04000A00 RID: 2560
	[Space]
	public float WaitTimerMin;

	// Token: 0x04000A01 RID: 2561
	public float WaitTimerMax;

	// Token: 0x04000A02 RID: 2562
	internal float WaitTimer = 999f;

	// Token: 0x04000A03 RID: 2563
	internal bool WaitDone;

	// Token: 0x04000A04 RID: 2564
	[Space]
	public float LookTimerMin;

	// Token: 0x04000A05 RID: 2565
	public float LookTimerMax;

	// Token: 0x04000A06 RID: 2566
	private float LookTimer = 999f;
}
