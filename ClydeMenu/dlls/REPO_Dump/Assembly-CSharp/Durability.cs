using System;
using UnityEngine;

// Token: 0x02000127 RID: 295
[CreateAssetMenu(fileName = "Durability ", menuName = "Phys Object/Durability Preset", order = 1)]
public class Durability : ScriptableObject
{
	// Token: 0x04001100 RID: 4352
	[Range(0f, 100f)]
	public float fragility = 100f;

	// Token: 0x04001101 RID: 4353
	[Range(0f, 100f)]
	public float durability = 100f;
}
