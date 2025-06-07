using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200008F RID: 143
[RequireComponent(typeof(PhotonView))]
public class EnemyParent : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x060005F0 RID: 1520 RVA: 0x0003AB64 File Offset: 0x00038D64
	private void Awake()
	{
		base.transform.parent = LevelGenerator.Instance.EnemyParent.transform;
		this.Enemy = base.GetComponentInChildren<Enemy>();
		if (EnemyDirector.instance.debugEnemy != null)
		{
			if (EnemyDirector.instance.debugEnemyEnableTime > 0f)
			{
				this.SpawnedTimeMax = EnemyDirector.instance.debugEnemyEnableTime;
				this.SpawnedTimeMin = this.SpawnedTimeMax;
			}
			if (EnemyDirector.instance.debugEnemyDisableTime > 0f)
			{
				this.DespawnedTimeMax = EnemyDirector.instance.debugEnemyDisableTime;
				this.DespawnedTimeMin = this.DespawnedTimeMax;
			}
		}
		base.StartCoroutine(this.Setup());
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x0003AC0A File Offset: 0x00038E0A
	private void Update()
	{
		if (SemiFunc.FPSImpulse1())
		{
			this.GetRoomVolume();
		}
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x0003AC19 File Offset: 0x00038E19
	private IEnumerator Setup()
	{
		while (!this.SetupDone)
		{
			yield return new WaitForSeconds(0.1f);
		}
		LevelGenerator.Instance.EnemiesSpawned++;
		EnemyDirector.instance.enemiesSpawned.Add(this);
		if (LevelGenerator.Instance.EnemiesSpawned >= LevelGenerator.Instance.EnemiesSpawnTarget)
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
				{
					enemyParent.Enemy.PlayerAdded(playerAvatar.photonView.ViewID);
				}
			}
			if (GameManager.Multiplayer())
			{
				LevelGenerator.Instance.PhotonView.RPC("EnemyReadyRPC", RpcTarget.All, Array.Empty<object>());
			}
		}
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			yield break;
		}
		if (this.Enemy.HasRigidbody)
		{
			float y = this.Enemy.Rigidbody.transform.localPosition.y - this.Enemy.transform.localPosition.y;
			Vector3 b = new Vector3(0f, y, 0f);
			Vector3 position = this.Enemy.transform.position + b;
			this.Enemy.Rigidbody.rb.position = position;
			this.Enemy.Rigidbody.rb.rotation = this.Enemy.Rigidbody.followTarget.rotation;
			this.Enemy.Rigidbody.physGrabObject.Teleport(position, this.Enemy.Rigidbody.followTarget.rotation);
			this.Enemy.Rigidbody.physGrabObject.spawned = true;
			this.Enemy.Rigidbody.rb.isKinematic = false;
		}
		base.StartCoroutine(this.Logic());
		base.StartCoroutine(this.PlayerCloseLogic());
		yield break;
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x0003AC28 File Offset: 0x00038E28
	private IEnumerator Logic()
	{
		this.Despawn();
		this.DespawnedTimer = Random.Range(2f, 5f);
		for (;;)
		{
			if (this.Spawned)
			{
				if (this.SpawnedTimer <= 0f)
				{
					if (!this.playerClose || EnemyDirector.instance.debugDespawnClose)
					{
						this.Enemy.CurrentState = EnemyState.Despawn;
					}
				}
				else if (this.spawnedTimerPauseTimer > 0f)
				{
					this.spawnedTimerPauseTimer -= Time.deltaTime;
				}
				else if (!this.playerClose || EnemyDirector.instance.debugDespawnClose)
				{
					this.SpawnedTimer -= Time.deltaTime;
				}
			}
			else if (this.DespawnedTimer <= 0f)
			{
				this.Spawn();
			}
			else
			{
				this.DespawnedTimer -= Time.deltaTime;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x0003AC37 File Offset: 0x00038E37
	private IEnumerator PlayerCloseLogic()
	{
		for (;;)
		{
			bool flag = false;
			float num = 20f;
			bool flag2 = false;
			float num2 = 6f;
			foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
			{
				if (!playerAvatar.isDisabled)
				{
					Vector3 a = new Vector3(playerAvatar.transform.position.x, 0f, playerAvatar.transform.position.z);
					Vector3 b = new Vector3(this.Enemy.transform.position.x, 0f, this.Enemy.transform.position.z);
					float num3 = Vector3.Distance(a, b);
					if (num3 <= num2)
					{
						flag2 = true;
						flag = true;
						break;
					}
					if (num3 <= num)
					{
						EnemyDirector.instance.spawnIdlePauseTimer = 0f;
						flag = true;
					}
				}
			}
			foreach (PlayerDeathSpot playerDeathSpot in GameDirector.instance.PlayerDeathSpots)
			{
				Vector3 a2 = new Vector3(playerDeathSpot.transform.position.x, 0f, playerDeathSpot.transform.position.z);
				Vector3 b2 = new Vector3(this.Enemy.transform.position.x, 0f, this.Enemy.transform.position.z);
				float num4 = Vector3.Distance(a2, b2);
				if (num4 <= num2)
				{
					flag2 = true;
					flag = true;
					break;
				}
				if (num4 <= num)
				{
					flag = true;
				}
			}
			this.playerClose = flag;
			this.playerVeryClose = flag2;
			if (flag)
			{
				this.valuableSpawnTimer = 10f;
			}
			else if (this.valuableSpawnTimer > 0f)
			{
				this.valuableSpawnTimer -= 1f;
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x0003AC46 File Offset: 0x00038E46
	public void DisableDecrease(float _time)
	{
		this.DespawnedTimer -= _time;
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x0003AC56 File Offset: 0x00038E56
	public void SpawnedTimerSet(float _time)
	{
		if (this.Spawned)
		{
			this.SpawnedTimer = _time;
			if (_time == 0f && SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.Enemy.CurrentState = EnemyState.Despawn;
			}
		}
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x0003AC83 File Offset: 0x00038E83
	public void DespawnedTimerSet(float _time, bool _min = false)
	{
		if (!this.Spawned)
		{
			if (!_min)
			{
				this.DespawnedTimer = _time;
				return;
			}
			this.DespawnedTimer = Mathf.Min(this.DespawnedTimer, _time);
		}
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x0003ACAA File Offset: 0x00038EAA
	public void SpawnedTimerReset()
	{
		if (this.Spawned)
		{
			this.SpawnedTimer = Random.Range(this.SpawnedTimeMin, this.SpawnedTimeMax);
			if (this.Enemy.CurrentState == EnemyState.Despawn)
			{
				this.Enemy.CurrentState = EnemyState.Roaming;
			}
		}
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x0003ACE6 File Offset: 0x00038EE6
	public void SpawnedTimerPause(float _time)
	{
		this.spawnedTimerPauseTimer = Mathf.Max(this.spawnedTimerPauseTimer, _time);
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x0003ACFC File Offset: 0x00038EFC
	public void GetRoomVolume()
	{
		this.currentRooms.Clear();
		foreach (Collider collider in Physics.OverlapBox(this.Enemy.CenterTransform.position, Vector3.one / 2f, base.transform.rotation, LayerMask.GetMask(new string[]
		{
			"RoomVolume"
		})))
		{
			RoomVolume roomVolume = collider.transform.GetComponent<RoomVolume>();
			if (!roomVolume)
			{
				roomVolume = collider.transform.GetComponentInParent<RoomVolume>();
			}
			if (!this.currentRooms.Contains(roomVolume))
			{
				this.currentRooms.Add(roomVolume);
			}
		}
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x0003ADA4 File Offset: 0x00038FA4
	private void Spawn()
	{
		this.SpawnedTimer = Random.Range(this.SpawnedTimeMin, this.SpawnedTimeMax);
		this.Enemy.CurrentState = EnemyState.Spawn;
		if (GameManager.Multiplayer())
		{
			base.photonView.RPC("SpawnRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.SpawnRPC(default(PhotonMessageInfo));
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x0003AE04 File Offset: 0x00039004
	[PunRPC]
	private void SpawnRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (this.Enemy.HasHealth)
		{
			this.Enemy.Health.OnSpawn();
		}
		if (this.Enemy.HasStateStunned)
		{
			this.Enemy.StateStunned.Spawn();
		}
		if (this.Enemy.HasJump)
		{
			this.Enemy.Jump.StuckReset();
		}
		this.Enemy.StuckCount = 0;
		this.Spawned = true;
		this.EnableObject.SetActive(true);
		this.Enemy.StateSpawn.OnSpawn.Invoke();
		this.Enemy.Spawn();
		if (!EnemyDirector.instance.debugNoSpawnedPause)
		{
			this.SpawnedTimerPause(Random.Range(3f, 4f) * 60f);
		}
		this.forceLeave = false;
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x0003AEE0 File Offset: 0x000390E0
	public void Despawn()
	{
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.Enemy.CurrentState = EnemyState.Despawn;
		this.DespawnedTimer = Random.Range(this.DespawnedTimeMin, this.DespawnedTimeMax) * EnemyDirector.instance.despawnedTimeMultiplier;
		this.DespawnedTimer = Mathf.Max(this.DespawnedTimer, 1f);
		if (this.Enemy.HasRigidbody)
		{
			this.Enemy.Rigidbody.grabbed = false;
			this.Enemy.Rigidbody.grabStrengthTimer = 0f;
			this.Enemy.Rigidbody.GrabRelease();
		}
		if (GameManager.Multiplayer())
		{
			base.photonView.RPC("DespawnRPC", RpcTarget.All, Array.Empty<object>());
		}
		else
		{
			this.DespawnRPC(default(PhotonMessageInfo));
		}
		if (this.Enemy.HasHealth && this.Enemy.Health.spawnValuable && this.Enemy.Health.healthCurrent <= 0)
		{
			if (this.valuableSpawnTimer > 0f && this.Enemy.Health.spawnValuableCurrent < this.Enemy.Health.spawnValuableMax)
			{
				GameObject gameObject = AssetManager.instance.enemyValuableSmall;
				if (this.difficulty == EnemyParent.Difficulty.Difficulty2)
				{
					gameObject = AssetManager.instance.enemyValuableMedium;
				}
				else if (this.difficulty == EnemyParent.Difficulty.Difficulty3)
				{
					gameObject = AssetManager.instance.enemyValuableBig;
				}
				Transform transform = this.Enemy.CustomValuableSpawnTransform;
				if (!transform)
				{
					transform = this.Enemy.CenterTransform;
				}
				if (!SemiFunc.IsMultiplayer())
				{
					Object.Instantiate<GameObject>(gameObject, transform.position, Quaternion.identity);
				}
				else
				{
					PhotonNetwork.InstantiateRoomObject("Valuables/" + gameObject.name, transform.position, Quaternion.identity, 0, null);
				}
				this.Enemy.Health.spawnValuableCurrent++;
			}
			this.DespawnedTimer *= 3f;
		}
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x0003B0DF File Offset: 0x000392DF
	[PunRPC]
	private void DespawnRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.Spawned = false;
		this.EnableObject.SetActive(false);
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x0003B0FD File Offset: 0x000392FD
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.SetupDone);
			return;
		}
		this.SetupDone = (bool)stream.ReceiveNext();
	}

	// Token: 0x0400099D RID: 2461
	public string enemyName = "Dinosaur";

	// Token: 0x0400099E RID: 2462
	internal bool SetupDone;

	// Token: 0x0400099F RID: 2463
	internal bool Spawned = true;

	// Token: 0x040009A0 RID: 2464
	internal Enemy Enemy;

	// Token: 0x040009A1 RID: 2465
	[Space]
	public EnemyParent.Difficulty difficulty;

	// Token: 0x040009A2 RID: 2466
	[Space]
	public float actionMultiplier = 1f;

	// Token: 0x040009A3 RID: 2467
	public float overchargeMultiplier = 1f;

	// Token: 0x040009A4 RID: 2468
	[Space]
	public GameObject EnableObject;

	// Token: 0x040009A5 RID: 2469
	[Space]
	public float SpawnedTimeMin;

	// Token: 0x040009A6 RID: 2470
	public float SpawnedTimeMax;

	// Token: 0x040009A7 RID: 2471
	[Space]
	public float DespawnedTimeMin;

	// Token: 0x040009A8 RID: 2472
	public float DespawnedTimeMax;

	// Token: 0x040009A9 RID: 2473
	[Space]
	public float SpawnedTimer;

	// Token: 0x040009AA RID: 2474
	public float DespawnedTimer;

	// Token: 0x040009AB RID: 2475
	private float spawnedTimerPauseTimer;

	// Token: 0x040009AC RID: 2476
	private float valuableSpawnTimer;

	// Token: 0x040009AD RID: 2477
	internal bool playerClose;

	// Token: 0x040009AE RID: 2478
	internal bool playerVeryClose;

	// Token: 0x040009AF RID: 2479
	internal bool forceLeave;

	// Token: 0x040009B0 RID: 2480
	internal List<RoomVolume> currentRooms = new List<RoomVolume>();

	// Token: 0x040009B1 RID: 2481
	internal LevelPoint firstSpawnPoint;

	// Token: 0x040009B2 RID: 2482
	internal bool firstSpawnPointUsed;

	// Token: 0x02000326 RID: 806
	public enum Difficulty
	{
		// Token: 0x0400296D RID: 10605
		Difficulty1,
		// Token: 0x0400296E RID: 10606
		Difficulty2,
		// Token: 0x0400296F RID: 10607
		Difficulty3
	}
}
