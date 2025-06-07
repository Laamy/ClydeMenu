using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000096 RID: 150
public class EnemyStateInvestigate : MonoBehaviour
{
	// Token: 0x06000615 RID: 1557 RVA: 0x0003BDB2 File Offset: 0x00039FB2
	private void Awake()
	{
		this.PhotonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x0003BDC0 File Offset: 0x00039FC0
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
		this.Roaming = base.GetComponent<EnemyStateRoaming>();
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x0003BDE8 File Offset: 0x00039FE8
	private void Update()
	{
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		if (this.Enemy.CurrentState != EnemyState.Investigate)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.PhysObjectHitImpulse = true;
			this.PhysObjectHitCount = 0;
			this.Active = true;
		}
		this.Enemy.NavMeshAgent.UpdateAgent(this.Speed, this.Acceleration);
		if (this.Enemy.HasRigidbody)
		{
			this.Enemy.Rigidbody.IdleSet(0.1f);
		}
		bool flag = this.Enemy.AttackStuckPhysObject.Check();
		if (this.Enemy.AttackStuckPhysObject.Active)
		{
			if (this.PhysObjectHitImpulse)
			{
				this.PhysObjectHitCount++;
				this.PhysObjectHitImpulse = false;
			}
		}
		else
		{
			this.PhysObjectHitImpulse = true;
		}
		if (!this.Enemy.NavMeshAgent.Agent.hasPath || (flag && !this.Enemy.AttackStuckPhysObject.Active) || this.PhysObjectHitCount >= this.PhysObjectHitMax)
		{
			this.Enemy.CurrentState = EnemyState.Roaming;
			this.Roaming.RoamingLevelPoint = this.InvestigateLevelPoint;
			if (this.PhysObjectHitCount >= this.PhysObjectHitMax)
			{
				this.Roaming.RoamingChangeCurrent = 0;
				return;
			}
			this.Roaming.RoamingChangeCurrent = Random.Range(this.Roaming.RoamingChangeMin, this.Roaming.RoamingChangeMax + 1);
		}
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x0003BF60 File Offset: 0x0003A160
	public void Set(Vector3 position, bool _pathFindOnly)
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.PhotonView.RPC("SetRPC", RpcTarget.All, new object[]
				{
					position,
					_pathFindOnly
				});
			}
		}
		else
		{
			this.SetRPC(position, _pathFindOnly, default(PhotonMessageInfo));
		}
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
		{
			if (Vector3.Distance(playerAvatar.transform.position, position) < 1f && Vector3.Distance(playerAvatar.transform.position, this.Enemy.transform.position) < 2f)
			{
				this.Enemy.SetChaseTarget(playerAvatar);
				break;
			}
		}
		if (this.OnlyEvent)
		{
			return;
		}
		if (this.Enemy.CurrentState == EnemyState.Roaming || this.Enemy.CurrentState == EnemyState.Investigate || this.Enemy.CurrentState == EnemyState.ChaseEnd)
		{
			this.Enemy.CurrentState = EnemyState.Investigate;
			float num = float.MaxValue;
			LevelPoint investigateLevelPoint = null;
			foreach (LevelPoint levelPoint in LevelGenerator.Instance.LevelPathPoints)
			{
				float num2 = Vector3.Distance(position, levelPoint.transform.position);
				if (num2 < num)
				{
					num = num2;
					investigateLevelPoint = levelPoint;
				}
			}
			this.InvestigateLevelPoint = investigateLevelPoint;
			this.Enemy.NavMeshAgent.SetDestination(position);
			return;
		}
		if (this.Enemy.CurrentState == EnemyState.ChaseSlow)
		{
			this.Enemy.StateChase.ChasePosition = position;
			this.Enemy.NavMeshAgent.SetDestination(this.Enemy.StateChase.ChasePosition);
		}
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x0003C148 File Offset: 0x0003A348
	[PunRPC]
	public void SetRPC(Vector3 position, bool _pathfindOnly, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.onInvestigateTriggeredPosition = position;
		this.onInvestigateTriggeredPathfindOnly = _pathfindOnly;
		this.onInvestigateTriggered.Invoke();
	}

	// Token: 0x040009EB RID: 2539
	private Enemy Enemy;

	// Token: 0x040009EC RID: 2540
	private PlayerController Player;

	// Token: 0x040009ED RID: 2541
	private bool Active;

	// Token: 0x040009EE RID: 2542
	private EnemyStateRoaming Roaming;

	// Token: 0x040009EF RID: 2543
	private PhotonView PhotonView;

	// Token: 0x040009F0 RID: 2544
	public float rangeMultiplier = 1f;

	// Token: 0x040009F1 RID: 2545
	[Space]
	public bool OnlyEvent = true;

	// Token: 0x040009F2 RID: 2546
	private bool PhysObjectHitImpulse;

	// Token: 0x040009F3 RID: 2547
	private int PhysObjectHitCount;

	// Token: 0x040009F4 RID: 2548
	public int PhysObjectHitMax = 3;

	// Token: 0x040009F5 RID: 2549
	[Header("Movement")]
	public float Speed;

	// Token: 0x040009F6 RID: 2550
	public float Acceleration;

	// Token: 0x040009F7 RID: 2551
	[Header("Event")]
	public UnityEvent onInvestigateTriggered;

	// Token: 0x040009F8 RID: 2552
	internal Vector3 onInvestigateTriggeredPosition;

	// Token: 0x040009F9 RID: 2553
	internal bool onInvestigateTriggeredPathfindOnly;

	// Token: 0x040009FA RID: 2554
	private LevelPoint InvestigateLevelPoint;
}
