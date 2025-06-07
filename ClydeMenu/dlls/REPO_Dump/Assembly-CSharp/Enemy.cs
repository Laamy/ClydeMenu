using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200008C RID: 140
[RequireComponent(typeof(PhotonView))]
public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x060005CF RID: 1487 RVA: 0x00039364 File Offset: 0x00037564
	private void Awake()
	{
		this.photonTransformView = base.transform.parent.GetComponentInChildren<PhotonTransformView>();
		this.EnemyParent = base.GetComponentInParent<EnemyParent>();
		this.PhotonView = base.GetComponent<PhotonView>();
		this.Vision = base.GetComponent<EnemyVision>();
		if (this.Vision)
		{
			this.HasVision = true;
		}
		this.VisionMask = SemiFunc.LayerMaskGetVisionObstruct() + LayerMask.GetMask(new string[]
		{
			"HideTriggers"
		});
		this.PlayerDistance = base.GetComponent<EnemyPlayerDistance>();
		if (this.PlayerDistance)
		{
			this.HasPlayerDistance = true;
		}
		this.OnScreen = base.GetComponent<EnemyOnScreen>();
		if (this.OnScreen)
		{
			this.HasOnScreen = true;
		}
		this.PlayerRoom = base.GetComponent<EnemyPlayerRoom>();
		if (this.PlayerRoom)
		{
			this.HasPlayerRoom = true;
		}
		this.NavMeshAgent = base.GetComponent<EnemyNavMeshAgent>();
		if (this.NavMeshAgent)
		{
			this.HasNavMeshAgent = true;
		}
		this.AttackStuckPhysObject = base.GetComponent<EnemyAttackStuckPhysObject>();
		if (this.AttackStuckPhysObject)
		{
			this.HasAttackPhysObject = true;
		}
		this.StateInvestigate = base.GetComponent<EnemyStateInvestigate>();
		if (this.StateInvestigate)
		{
			this.HasStateInvestigate = true;
		}
		this.StateChaseBegin = base.GetComponent<EnemyStateChaseBegin>();
		if (this.StateChaseBegin)
		{
			this.HasStateChaseBegin = true;
		}
		this.StateChase = base.GetComponent<EnemyStateChase>();
		if (this.StateChase)
		{
			this.HasStateChase = true;
		}
		this.StateLookUnder = base.GetComponent<EnemyStateLookUnder>();
		if (this.StateLookUnder)
		{
			this.HasStateLookUnder = true;
		}
		this.StateDespawn = base.GetComponent<EnemyStateDespawn>();
		if (this.StateDespawn)
		{
			this.HasStateDespawn = true;
		}
		this.StateSpawn = base.GetComponent<EnemyStateSpawn>();
		if (this.StateSpawn)
		{
			this.HasStateSpawn = true;
		}
		this.StateStunned = base.GetComponent<EnemyStateStunned>();
		if (this.StateStunned)
		{
			this.HasStateStunned = true;
		}
		this.Health = base.GetComponent<EnemyHealth>();
		if (this.Health)
		{
			this.HasHealth = true;
		}
		if (!this.CenterTransform)
		{
			Debug.LogError("Center Transform not set in " + base.gameObject.name, base.gameObject);
		}
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x000395B5 File Offset: 0x000377B5
	private void Start()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			this.MasterClient = true;
			return;
		}
		this.MasterClient = false;
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x000395D4 File Offset: 0x000377D4
	private void Update()
	{
		if (SemiFunc.IsMultiplayer() && !this.MasterClient)
		{
			float num = 1f / (float)PhotonNetwork.SerializationRate;
			float num2 = this.PositionDistance / num;
			this.moveDirection = (this.PositionTarget - base.transform.position).normalized;
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.PositionTarget, num2 * Time.deltaTime);
		}
		if (this.MasterClient)
		{
			this.Stunned = false;
			if (this.HasStateStunned && this.StateStunned.stunTimer > 0f)
			{
				this.Stunned = true;
			}
		}
		if (this.FreezeTimer > 0f)
		{
			this.FreezeTimer -= Time.deltaTime;
		}
		if (this.TeleportedTimer > 0f)
		{
			this.StuckCount = 0;
			this.TeleportedTimer -= Time.deltaTime;
		}
		if (this.ChaseTimer > 0f)
		{
			this.ChaseTimer -= Time.deltaTime;
		}
		if (this.DisableChaseTimer > 0f)
		{
			this.DisableChaseTimer -= Time.deltaTime;
		}
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x00039705 File Offset: 0x00037905
	public void Spawn()
	{
		this.Stunned = false;
		this.FreezeTimer = 0f;
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x00039719 File Offset: 0x00037919
	public bool IsStunned()
	{
		return this.Stunned;
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x00039721 File Offset: 0x00037921
	public void DisableChase(float time)
	{
		this.DisableChaseTimer = time;
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x0003972A File Offset: 0x0003792A
	public void SetChaseTimer()
	{
		this.ChaseTimer = 0.1f;
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x00039737 File Offset: 0x00037937
	public bool CheckChase()
	{
		return this.ChaseTimer > 0f;
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x00039748 File Offset: 0x00037948
	public void SetChaseTarget(PlayerAvatar playerAvatar)
	{
		if (EnemyDirector.instance.debugNoVision)
		{
			return;
		}
		if (this.DisableChaseTimer > 0f)
		{
			return;
		}
		if (!this.HasVision)
		{
			return;
		}
		if (!playerAvatar.isDisabled)
		{
			if (this.Vision.DisableTimer > 0f)
			{
				return;
			}
			this.Vision.VisionTrigger(playerAvatar.photonView.ViewID, playerAvatar, false, false);
			if (!this.HasStateChase)
			{
				return;
			}
			if (!this.CheckChase() || this.CurrentState == EnemyState.ChaseSlow)
			{
				this.CurrentState = EnemyState.ChaseBegin;
				this.TargetPlayerViewID = playerAvatar.photonView.ViewID;
				this.TargetPlayerAvatar = playerAvatar;
			}
		}
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x000397E8 File Offset: 0x000379E8
	public LevelPoint TeleportToPoint(float minDistance, float maxDistance)
	{
		LevelPoint levelPoint = null;
		if (SemiFunc.EnemySpawnIdlePause())
		{
			levelPoint = this.EnemyParent.firstSpawnPoint;
		}
		else
		{
			if (RoundDirector.instance.allExtractionPointsCompleted)
			{
				levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, minDistance, maxDistance, true);
			}
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, minDistance, maxDistance, false);
			}
		}
		if (levelPoint)
		{
			this.TeleportPosition = new Vector3(levelPoint.transform.position.x, levelPoint.transform.position.y, levelPoint.transform.position.z);
			this.EnemyTeleported(this.TeleportPosition);
		}
		return levelPoint;
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x00039898 File Offset: 0x00037A98
	public LevelPoint GetLevelPointAhead(Vector3 currentTargetPosition)
	{
		LevelPoint result = null;
		Vector3 normalized = (currentTargetPosition - base.transform.position).normalized;
		LevelPoint levelPoint = null;
		float num = 1000f;
		foreach (LevelPoint levelPoint2 in LevelGenerator.Instance.LevelPathPoints)
		{
			if (levelPoint2)
			{
				float num2 = Vector3.Distance(levelPoint2.transform.position, currentTargetPosition);
				if (num2 < num)
				{
					num = num2;
					levelPoint = levelPoint2;
				}
			}
		}
		if (!levelPoint)
		{
			return null;
		}
		float num3 = -1f;
		foreach (LevelPoint levelPoint3 in levelPoint.ConnectedPoints)
		{
			if (levelPoint3)
			{
				Vector3 normalized2 = (levelPoint3.transform.position - levelPoint.transform.position).normalized;
				float num4 = Vector3.Dot(normalized, normalized2);
				if (num4 > num3)
				{
					num3 = num4;
					result = levelPoint3;
				}
			}
		}
		return result;
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x000399D0 File Offset: 0x00037BD0
	public void Freeze(float time)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.FreezeRPC(time, default(PhotonMessageInfo));
			return;
		}
		base.photonView.RPC("FreezeRPC", RpcTarget.All, new object[]
		{
			time
		});
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x00039A1A File Offset: 0x00037C1A
	[PunRPC]
	public void FreezeRPC(float time, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.FreezeTimer = time;
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00039A2C File Offset: 0x00037C2C
	public void PlayerAdded(int photonID)
	{
		if (this.HasVision)
		{
			this.Vision.PlayerAdded(photonID);
		}
		if (this.HasOnScreen)
		{
			this.OnScreen.PlayerAdded(photonID);
		}
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00039A58 File Offset: 0x00037C58
	public void PlayerRemoved(int photonID)
	{
		if (this.StateChaseBegin != null && this.StateChaseBegin.TargetPlayer != null && this.StateChaseBegin.TargetPlayer.photonView.ViewID == photonID)
		{
			this.StateChaseBegin.TargetPlayer = null;
			this.CurrentState = EnemyState.Roaming;
		}
		if (this.TargetPlayerAvatar != null && this.TargetPlayerAvatar.photonView.ViewID == photonID)
		{
			this.TargetPlayerAvatar = PlayerController.instance.playerAvatarScript;
			this.TargetPlayerViewID = this.TargetPlayerAvatar.photonView.ViewID;
		}
		if (this.HasVision)
		{
			this.Vision.PlayerRemoved(photonID);
		}
		if (this.HasOnScreen)
		{
			this.OnScreen.PlayerRemoved(photonID);
		}
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00039B20 File Offset: 0x00037D20
	public void EnemyTeleported(Vector3 teleportPosition)
	{
		base.transform.position = teleportPosition;
		if (this.HasNavMeshAgent)
		{
			this.NavMeshAgent.Warp(teleportPosition);
		}
		if (this.HasRigidbody)
		{
			this.Rigidbody.Teleport();
		}
		if (GameManager.instance.gameMode == 0)
		{
			this.EnemyTeleportedRPC(teleportPosition, default(PhotonMessageInfo));
			return;
		}
		base.photonView.RPC("EnemyTeleportedRPC", RpcTarget.All, new object[]
		{
			teleportPosition
		});
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00039B9D File Offset: 0x00037D9D
	[PunRPC]
	private void EnemyTeleportedRPC(Vector3 teleportPosition, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.PositionDistance = 0f;
		this.PositionTarget = teleportPosition;
		this.TeleportPosition = teleportPosition;
		base.transform.position = teleportPosition;
		this.TeleportedTimer = 1f;
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00039BD8 File Offset: 0x00037DD8
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(this.CurrentState);
			stream.SendNext(this.TargetPlayerViewID);
			stream.SendNext(this.Stunned);
			return;
		}
		this.PositionTarget = (Vector3)stream.ReceiveNext();
		this.PositionDistance = Vector3.Distance(base.transform.position, this.PositionTarget);
		this.CurrentState = (EnemyState)stream.ReceiveNext();
		this.TargetPlayerViewID = (int)stream.ReceiveNext();
		this.Stunned = (bool)stream.ReceiveNext();
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (!playerAvatar.isDisabled && playerAvatar.photonView.ViewID == this.TargetPlayerViewID)
			{
				this.TargetPlayerAvatar = playerAvatar;
				break;
			}
		}
	}

	// Token: 0x04000921 RID: 2337
	internal PhotonView PhotonView;

	// Token: 0x04000922 RID: 2338
	internal EnemyParent EnemyParent;

	// Token: 0x04000923 RID: 2339
	internal bool MasterClient;

	// Token: 0x04000924 RID: 2340
	public EnemyType Type = EnemyType.Medium;

	// Token: 0x04000925 RID: 2341
	[Space]
	public EnemyState CurrentState;

	// Token: 0x04000926 RID: 2342
	private EnemyState PreviousState;

	// Token: 0x04000927 RID: 2343
	private int CurrentStateIndex;

	// Token: 0x04000928 RID: 2344
	[Space]
	public Transform CenterTransform;

	// Token: 0x04000929 RID: 2345
	public Transform KillLookAtTransform;

	// Token: 0x0400092A RID: 2346
	public Transform CustomValuableSpawnTransform;

	// Token: 0x0400092B RID: 2347
	internal LayerMask VisionMask;

	// Token: 0x0400092C RID: 2348
	private Vector3 PositionTarget;

	// Token: 0x0400092D RID: 2349
	private float PositionDistance;

	// Token: 0x0400092E RID: 2350
	internal int StuckCount;

	// Token: 0x0400092F RID: 2351
	internal EnemyVision Vision;

	// Token: 0x04000930 RID: 2352
	internal bool HasVision;

	// Token: 0x04000931 RID: 2353
	internal EnemyPlayerDistance PlayerDistance;

	// Token: 0x04000932 RID: 2354
	internal bool HasPlayerDistance;

	// Token: 0x04000933 RID: 2355
	internal EnemyOnScreen OnScreen;

	// Token: 0x04000934 RID: 2356
	internal bool HasOnScreen;

	// Token: 0x04000935 RID: 2357
	internal EnemyPlayerRoom PlayerRoom;

	// Token: 0x04000936 RID: 2358
	internal bool HasPlayerRoom;

	// Token: 0x04000937 RID: 2359
	internal EnemyRigidbody Rigidbody;

	// Token: 0x04000938 RID: 2360
	internal bool HasRigidbody;

	// Token: 0x04000939 RID: 2361
	internal EnemyNavMeshAgent NavMeshAgent;

	// Token: 0x0400093A RID: 2362
	internal bool HasNavMeshAgent;

	// Token: 0x0400093B RID: 2363
	internal EnemyAttackStuckPhysObject AttackStuckPhysObject;

	// Token: 0x0400093C RID: 2364
	internal bool HasAttackPhysObject;

	// Token: 0x0400093D RID: 2365
	internal EnemyStateInvestigate StateInvestigate;

	// Token: 0x0400093E RID: 2366
	internal bool HasStateInvestigate;

	// Token: 0x0400093F RID: 2367
	internal EnemyStateChaseBegin StateChaseBegin;

	// Token: 0x04000940 RID: 2368
	internal bool HasStateChaseBegin;

	// Token: 0x04000941 RID: 2369
	internal EnemyStateChase StateChase;

	// Token: 0x04000942 RID: 2370
	internal bool HasStateChase;

	// Token: 0x04000943 RID: 2371
	internal EnemyStateLookUnder StateLookUnder;

	// Token: 0x04000944 RID: 2372
	internal bool HasStateLookUnder;

	// Token: 0x04000945 RID: 2373
	internal EnemyStateDespawn StateDespawn;

	// Token: 0x04000946 RID: 2374
	internal bool HasStateDespawn;

	// Token: 0x04000947 RID: 2375
	internal EnemyStateSpawn StateSpawn;

	// Token: 0x04000948 RID: 2376
	internal bool HasStateSpawn;

	// Token: 0x04000949 RID: 2377
	private bool Stunned;

	// Token: 0x0400094A RID: 2378
	internal EnemyStateStunned StateStunned;

	// Token: 0x0400094B RID: 2379
	internal bool HasStateStunned;

	// Token: 0x0400094C RID: 2380
	internal EnemyGrounded Grounded;

	// Token: 0x0400094D RID: 2381
	internal bool HasGrounded;

	// Token: 0x0400094E RID: 2382
	internal EnemyJump Jump;

	// Token: 0x0400094F RID: 2383
	internal bool HasJump;

	// Token: 0x04000950 RID: 2384
	internal EnemyHealth Health;

	// Token: 0x04000951 RID: 2385
	internal bool HasHealth;

	// Token: 0x04000952 RID: 2386
	internal PlayerAvatar TargetPlayerAvatar;

	// Token: 0x04000953 RID: 2387
	internal int TargetPlayerViewID;

	// Token: 0x04000954 RID: 2388
	protected internal float TeleportedTimer;

	// Token: 0x04000955 RID: 2389
	protected internal Vector3 TeleportPosition;

	// Token: 0x04000956 RID: 2390
	[HideInInspector]
	public float FreezeTimer;

	// Token: 0x04000957 RID: 2391
	private float ChaseTimer;

	// Token: 0x04000958 RID: 2392
	internal float DisableChaseTimer;

	// Token: 0x04000959 RID: 2393
	private PhotonTransformView photonTransformView;

	// Token: 0x0400095A RID: 2394
	[Space]
	public bool SightingStinger;

	// Token: 0x0400095B RID: 2395
	public bool EnemyNearMusic;

	// Token: 0x0400095C RID: 2396
	internal Vector3 moveDirection;
}
