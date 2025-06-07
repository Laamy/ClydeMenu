using System;
using UnityEngine;

// Token: 0x02000011 RID: 17
[CreateAssetMenu(fileName = "Rarity - _____", menuName = "Other/Rarity Preset", order = 0)]
public class RarityPreset : ScriptableObject
{
	// Token: 0x0400006B RID: 107
	[Range(0f, 100f)]
	public float chance;
}
