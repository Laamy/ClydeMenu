using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000062 RID: 98
public class EnemyHeadVisual : MonoBehaviour, IPunObservable
{
	// Token: 0x0600031C RID: 796 RVA: 0x0001ECA4 File Offset: 0x0001CEA4
	private void Update()
	{
		if (this.enemy.FreezeTimer > 0f)
		{
			return;
		}
		if (this.enemy.MasterClient)
		{
			if (this.enemy.CheckChase())
			{
				this.PositionFollowCurrent = this.PositionFollowChasing;
				this.RotationFollowCurrent = this.RotationFollowChasing;
			}
			else
			{
				this.PositionFollowCurrent = this.PositionFollowIdle;
				this.RotationFollowCurrent = this.RotationFollowIdle;
			}
		}
		if (this.spawnTimer > 0f || this.enemy.TeleportedTimer > 0f)
		{
			base.transform.position = this.FollowPosition.position;
			this.TargetRotation.rotation = this.FollowRotation.rotation;
			if (LevelGenerator.Instance.Generated)
			{
				this.spawnTimer -= Time.deltaTime;
				return;
			}
		}
		else
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.FollowPosition.position, this.PositionFollowCurrent * Time.deltaTime);
			this.TargetRotation.rotation = Quaternion.Lerp(this.TargetRotation.rotation, this.FollowRotation.rotation, this.RotationFollowCurrent * Time.deltaTime);
		}
	}

	// Token: 0x0600031D RID: 797 RVA: 0x0001EDDE File Offset: 0x0001CFDE
	public void Spawn()
	{
		base.transform.position = this.FollowPosition.position;
		this.TargetRotation.rotation = this.FollowRotation.rotation;
	}

	// Token: 0x0600031E RID: 798 RVA: 0x0001EE0C File Offset: 0x0001D00C
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.PositionFollowCurrent);
			stream.SendNext(this.RotationFollowCurrent);
			return;
		}
		this.PositionFollowCurrent = (float)stream.ReceiveNext();
		this.RotationFollowCurrent = (float)stream.ReceiveNext();
	}

	// Token: 0x0400056D RID: 1389
	public EnemyHeadController Controller;

	// Token: 0x0400056E RID: 1390
	public Enemy enemy;

	// Token: 0x0400056F RID: 1391
	private float spawnTimer = 1f;

	// Token: 0x04000570 RID: 1392
	[Space]
	public Transform FollowPosition;

	// Token: 0x04000571 RID: 1393
	public Transform FollowRotation;

	// Token: 0x04000572 RID: 1394
	public Transform TargetRotation;

	// Token: 0x04000573 RID: 1395
	private float PositionFollowCurrent;

	// Token: 0x04000574 RID: 1396
	private float RotationFollowCurrent;

	// Token: 0x04000575 RID: 1397
	[Space]
	[Header("Idle")]
	public float PositionFollowIdle;

	// Token: 0x04000576 RID: 1398
	public float RotationFollowIdle;

	// Token: 0x04000577 RID: 1399
	[Space]
	[Header("Chasing")]
	public float PositionFollowChasing;

	// Token: 0x04000578 RID: 1400
	public float RotationFollowChasing;
}
