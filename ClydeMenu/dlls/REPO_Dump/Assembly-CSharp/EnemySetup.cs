using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000090 RID: 144
[CreateAssetMenu(fileName = "Enemy - _____", menuName = "Other/Enemy Setup", order = 0)]
public class EnemySetup : ScriptableObject
{
	// Token: 0x040009B3 RID: 2483
	public List<GameObject> spawnObjects;

	// Token: 0x040009B4 RID: 2484
	public bool levelsCompletedCondition;

	// Token: 0x040009B5 RID: 2485
	[Range(0f, 10f)]
	public int levelsCompletedMin;

	// Token: 0x040009B6 RID: 2486
	[Range(0f, 10f)]
	public int levelsCompletedMax = 10;

	// Token: 0x040009B7 RID: 2487
	[Space]
	public RarityPreset rarityPreset;

	// Token: 0x040009B8 RID: 2488
	[Space]
	public int runsPlayed;
}
