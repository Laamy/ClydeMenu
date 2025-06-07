using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200008E RID: 142
public class EnemyDirector : MonoBehaviour
{
	// Token: 0x060005E5 RID: 1509 RVA: 0x00039E7A File Offset: 0x0003807A
	private void Awake()
	{
		EnemyDirector.instance = this;
		this.despawnedDecreaseTimer = 60f * this.despawnedDecreaseMinutes;
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00039E94 File Offset: 0x00038094
	private void Start()
	{
		this.spawnIdlePauseTimer = 60f * Random.Range(2f, 3f) * this.spawnIdlePauseCurve.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		if (Random.Range(0, 100) < 20)
		{
			this.spawnIdlePauseTimer *= Random.Range(0.1f, 0.25f);
		}
		this.spawnIdlePauseTimer = Mathf.Max(this.spawnIdlePauseTimer, 1f);
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00039F0C File Offset: 0x0003810C
	private void Update()
	{
		if (LevelGenerator.Instance.Generated && this.spawnIdlePauseTimer > 0f)
		{
			this.spawnIdlePauseTimer -= Time.deltaTime;
			if (this.spawnIdlePauseTimer <= 0f)
			{
				using (List<EnemyParent>.Enumerator enumerator = this.enemiesSpawned.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.firstSpawnPointUsed)
						{
							this.spawnIdlePauseTimer = 2f;
						}
					}
				}
			}
			if (this.debugNoSpawnIdlePause)
			{
				this.spawnIdlePauseTimer = 0f;
			}
		}
		this.despawnedDecreaseTimer -= Time.deltaTime;
		if (this.despawnedDecreaseTimer <= 0f)
		{
			this.despawnedTimeMultiplier -= this.despawnedDecreasePercent;
			if (this.despawnedTimeMultiplier < 0f)
			{
				this.despawnedTimeMultiplier = 0f;
			}
			this.despawnedDecreaseTimer = 60f * this.despawnedDecreaseMinutes;
		}
		if (RoundDirector.instance.allExtractionPointsCompleted)
		{
			foreach (EnemyParent enemyParent in this.enemiesSpawned)
			{
				if (enemyParent.DespawnedTimer > 30f)
				{
					enemyParent.DespawnedTimerSet(0f, false);
				}
			}
			if (this.investigatePointTimer <= 0f)
			{
				if (this.extractionsDoneState == EnemyDirector.ExtractionsDoneState.StartRoom)
				{
					this.enemyActionAmount = 0f;
					this.despawnedTimeMultiplier = 0f;
					if (this.extractionDoneStateImpulse)
					{
						this.extractionDoneStateTimer = 10f;
						this.extractionDoneStateImpulse = false;
						foreach (EnemyParent enemyParent2 in this.enemiesSpawned)
						{
							if (enemyParent2.Spawned)
							{
								bool flag = false;
								foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
								{
									if (!playerAvatar.isDisabled && Vector3.Distance(enemyParent2.Enemy.transform.position, playerAvatar.transform.position) < 25f)
									{
										flag = true;
										break;
									}
								}
								if (!flag)
								{
									enemyParent2.SpawnedTimerPause(0f);
									enemyParent2.SpawnedTimerSet(0f);
								}
							}
						}
					}
					this.investigatePointTimer = this.investigatePointTime;
					List<LevelPoint> list = SemiFunc.LevelPointsGetInStartRoom();
					if (list.Count > 0)
					{
						SemiFunc.EnemyInvestigate(list[Random.Range(0, list.Count)].transform.position, 100f, true);
					}
					this.extractionDoneStateTimer -= this.investigatePointTime;
					if (this.extractionDoneStateTimer <= 0f)
					{
						this.extractionsDoneState = EnemyDirector.ExtractionsDoneState.PlayerRoom;
					}
				}
				else
				{
					List<LevelPoint> list2 = SemiFunc.LevelPointsGetInPlayerRooms();
					if (list2.Count > 0)
					{
						SemiFunc.EnemyInvestigate(list2[Random.Range(0, list2.Count)].transform.position, 100f, true);
					}
					this.investigatePointTimer = this.investigatePointTime;
					this.investigatePointTime = Mathf.Min(this.investigatePointTime + 2f, 30f);
				}
			}
			else
			{
				this.investigatePointTimer -= Time.deltaTime;
			}
		}
		float num = 0f;
		foreach (EnemyParent enemyParent3 in this.enemiesSpawned)
		{
			if (enemyParent3.Spawned && enemyParent3.playerClose && !enemyParent3.forceLeave)
			{
				bool flag2 = false;
				foreach (PlayerAvatar playerAvatar2 in SemiFunc.PlayerGetList())
				{
					foreach (RoomVolume x in playerAvatar2.RoomVolumeCheck.CurrentRooms)
					{
						foreach (RoomVolume y in enemyParent3.currentRooms)
						{
							if (x == y)
							{
								flag2 = true;
								break;
							}
						}
					}
				}
				if (flag2)
				{
					float num2 = 0f;
					if (enemyParent3.difficulty == EnemyParent.Difficulty.Difficulty3)
					{
						num2 += 2f;
					}
					else if (enemyParent3.difficulty == EnemyParent.Difficulty.Difficulty2)
					{
						num2 += 1f;
					}
					else
					{
						num2 += 0.5f;
					}
					num += num2 * enemyParent3.actionMultiplier;
				}
			}
		}
		if (num > 0f)
		{
			this.enemyActionAmount += num * Time.deltaTime;
		}
		else
		{
			this.enemyActionAmount -= 0.1f * Time.deltaTime;
			this.enemyActionAmount = Mathf.Max(0f, this.enemyActionAmount);
		}
		float num3 = 120f;
		if (this.debugShortActionTimer)
		{
			num3 = 5f;
		}
		if (this.enemyActionAmount > num3)
		{
			this.enemyActionAmount = 0f;
			LevelPoint levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f);
			if (levelPoint)
			{
				this.SetInvestigate(levelPoint.transform.position, float.MaxValue, true);
			}
			if (RoundDirector.instance.allExtractionPointsCompleted && this.extractionsDoneState == EnemyDirector.ExtractionsDoneState.PlayerRoom)
			{
				this.investigatePointTimer = 60f;
			}
			foreach (EnemyParent enemyParent4 in this.enemiesSpawned)
			{
				if (enemyParent4.Spawned)
				{
					enemyParent4.forceLeave = true;
				}
			}
		}
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x0003A598 File Offset: 0x00038798
	public void AmountSetup()
	{
		this.amountCurve3Value = (int)this.amountCurve3.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		this.amountCurve2Value = (int)this.amountCurve2.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		this.amountCurve1Value = (int)this.amountCurve1.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		for (int i = 0; i < this.amountCurve3Value; i++)
		{
			this.PickEnemies(this.enemiesDifficulty3);
		}
		for (int j = 0; j < this.amountCurve2Value; j++)
		{
			this.PickEnemies(this.enemiesDifficulty2);
		}
		for (int k = 0; k < this.amountCurve1Value; k++)
		{
			this.PickEnemies(this.enemiesDifficulty1);
		}
		this.totalAmount = this.amountCurve1Value + this.amountCurve2Value + this.amountCurve3Value;
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x0003A65C File Offset: 0x0003885C
	private void PickEnemies(List<EnemySetup> _enemiesList)
	{
		int num = DataDirector.instance.SettingValueFetch(DataDirector.Setting.RunsPlayed);
		_enemiesList.Shuffle<EnemySetup>();
		EnemySetup enemySetup = null;
		float num2 = -1f;
		foreach (EnemySetup enemySetup2 in _enemiesList)
		{
			if ((!enemySetup2.levelsCompletedCondition || (RunManager.instance.levelsCompleted >= enemySetup2.levelsCompletedMin && RunManager.instance.levelsCompleted <= enemySetup2.levelsCompletedMax)) && num >= enemySetup2.runsPlayed)
			{
				int num3 = 0;
				using (List<EnemySetup>.Enumerator enumerator2 = RunManager.instance.enemiesSpawned.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current == enemySetup2)
						{
							num3++;
						}
					}
				}
				float num4 = 100f;
				if (enemySetup2.rarityPreset)
				{
					num4 = enemySetup2.rarityPreset.chance;
				}
				float maxInclusive = Mathf.Max(0f, num4 - 30f * (float)num3);
				float num5 = Random.Range(0f, maxInclusive);
				if (num5 > num2)
				{
					enemySetup = enemySetup2;
					num2 = num5;
				}
			}
		}
		this.enemyList.Add(enemySetup);
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x0003A7B8 File Offset: 0x000389B8
	public EnemySetup GetEnemy()
	{
		EnemySetup enemySetup = this.enemyList[this.enemyListIndex];
		this.enemyListIndex++;
		int num = 0;
		using (List<EnemySetup>.Enumerator enumerator = RunManager.instance.enemiesSpawned.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == enemySetup)
				{
					num++;
				}
			}
		}
		int num2 = 4;
		while (num < 8 && num2 > 0)
		{
			RunManager.instance.enemiesSpawned.Add(enemySetup);
			num++;
			num2--;
		}
		return enemySetup;
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x0003A85C File Offset: 0x00038A5C
	public void FirstSpawnPointAdd(EnemyParent _enemyParent)
	{
		List<LevelPoint> list = SemiFunc.LevelPointsGetAll();
		float num = 0f;
		LevelPoint levelPoint = null;
		foreach (LevelPoint levelPoint2 in list)
		{
			float num2 = Vector3.Distance(levelPoint2.transform.position, LevelGenerator.Instance.LevelPathTruck.transform.position);
			using (List<LevelPoint>.Enumerator enumerator2 = this.enemyFirstSpawnPoints.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current == levelPoint2)
					{
						num2 = 0f;
						break;
					}
				}
			}
			if (num2 > num)
			{
				num = num2;
				levelPoint = levelPoint2;
			}
		}
		if (levelPoint)
		{
			_enemyParent.firstSpawnPoint = levelPoint;
			this.enemyFirstSpawnPoints.Add(levelPoint);
		}
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x0003A94C File Offset: 0x00038B4C
	public void DebugResult()
	{
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x0003A950 File Offset: 0x00038B50
	public void SetInvestigate(Vector3 position, float radius, bool pathfindOnly = false)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.debugInvestigate)
			{
				Object.Instantiate<GameObject>(AssetManager.instance.debugEnemyInvestigate, position, Quaternion.identity).GetComponent<DebugEnemyInvestigate>().radius = radius;
			}
			foreach (EnemyParent enemyParent in this.enemiesSpawned)
			{
				if (!enemyParent.Spawned)
				{
					if (radius >= 15f)
					{
						enemyParent.DisableDecrease(5f);
					}
				}
				else if (enemyParent.Enemy.HasStateInvestigate && Vector3.Distance(position, enemyParent.Enemy.transform.position) / enemyParent.Enemy.StateInvestigate.rangeMultiplier < radius)
				{
					enemyParent.Enemy.StateInvestigate.Set(position, pathfindOnly);
				}
			}
		}
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x0003AA34 File Offset: 0x00038C34
	public void AddEnemyValuable(EnemyValuable _newValuable)
	{
		List<EnemyValuable> list = new List<EnemyValuable>();
		foreach (EnemyValuable enemyValuable in this.enemyValuables)
		{
			if (!enemyValuable)
			{
				list.Add(enemyValuable);
			}
		}
		foreach (EnemyValuable enemyValuable2 in list)
		{
			this.enemyValuables.Remove(enemyValuable2);
		}
		this.enemyValuables.Add(_newValuable);
		if (this.enemyValuables.Count > 10)
		{
			EnemyValuable enemyValuable3 = this.enemyValuables[0];
			this.enemyValuables.RemoveAt(0);
			enemyValuable3.Destroy();
		}
	}

	// Token: 0x04000975 RID: 2421
	private EnemyDirector.ExtractionsDoneState extractionsDoneState;

	// Token: 0x04000976 RID: 2422
	private float extractionDoneStateTimer;

	// Token: 0x04000977 RID: 2423
	private bool extractionDoneStateImpulse = true;

	// Token: 0x04000978 RID: 2424
	public static EnemyDirector instance;

	// Token: 0x04000979 RID: 2425
	internal bool debugNoVision;

	// Token: 0x0400097A RID: 2426
	internal EnemySetup[] debugEnemy;

	// Token: 0x0400097B RID: 2427
	internal float debugEnemyEnableTime;

	// Token: 0x0400097C RID: 2428
	internal float debugEnemyDisableTime;

	// Token: 0x0400097D RID: 2429
	internal bool debugEasyGrab;

	// Token: 0x0400097E RID: 2430
	internal bool debugSpawnClose;

	// Token: 0x0400097F RID: 2431
	internal bool debugDespawnClose;

	// Token: 0x04000980 RID: 2432
	internal bool debugInvestigate;

	// Token: 0x04000981 RID: 2433
	internal bool debugShortActionTimer;

	// Token: 0x04000982 RID: 2434
	internal bool debugNoSpawnedPause;

	// Token: 0x04000983 RID: 2435
	internal bool debugNoSpawnIdlePause;

	// Token: 0x04000984 RID: 2436
	internal bool debugNoGrabMaxTime;

	// Token: 0x04000985 RID: 2437
	public List<EnemySetup> enemiesDifficulty1;

	// Token: 0x04000986 RID: 2438
	public List<EnemySetup> enemiesDifficulty2;

	// Token: 0x04000987 RID: 2439
	public List<EnemySetup> enemiesDifficulty3;

	// Token: 0x04000988 RID: 2440
	[Space]
	public AnimationCurve spawnIdlePauseCurve;

	// Token: 0x04000989 RID: 2441
	[Space]
	public AnimationCurve amountCurve1;

	// Token: 0x0400098A RID: 2442
	private int amountCurve1Value;

	// Token: 0x0400098B RID: 2443
	public AnimationCurve amountCurve2;

	// Token: 0x0400098C RID: 2444
	private int amountCurve2Value;

	// Token: 0x0400098D RID: 2445
	public AnimationCurve amountCurve3;

	// Token: 0x0400098E RID: 2446
	private int amountCurve3Value;

	// Token: 0x0400098F RID: 2447
	internal int totalAmount;

	// Token: 0x04000990 RID: 2448
	private List<EnemySetup> enemyList = new List<EnemySetup>();

	// Token: 0x04000991 RID: 2449
	private int enemyListIndex;

	// Token: 0x04000992 RID: 2450
	[Space]
	public float despawnedDecreaseMinutes;

	// Token: 0x04000993 RID: 2451
	public float despawnedDecreasePercent;

	// Token: 0x04000994 RID: 2452
	internal float despawnedTimeMultiplier = 1f;

	// Token: 0x04000995 RID: 2453
	private float despawnedDecreaseTimer;

	// Token: 0x04000996 RID: 2454
	private float investigatePointTimer;

	// Token: 0x04000997 RID: 2455
	private float investigatePointTime = 3f;

	// Token: 0x04000998 RID: 2456
	private float enemyActionAmount;

	// Token: 0x04000999 RID: 2457
	internal float spawnIdlePauseTimer;

	// Token: 0x0400099A RID: 2458
	[Space]
	public List<EnemyParent> enemiesSpawned;

	// Token: 0x0400099B RID: 2459
	internal List<EnemyValuable> enemyValuables = new List<EnemyValuable>();

	// Token: 0x0400099C RID: 2460
	internal List<LevelPoint> enemyFirstSpawnPoints = new List<LevelPoint>();

	// Token: 0x02000325 RID: 805
	public enum ExtractionsDoneState
	{
		// Token: 0x0400296A RID: 10602
		StartRoom,
		// Token: 0x0400296B RID: 10603
		PlayerRoom
	}
}
