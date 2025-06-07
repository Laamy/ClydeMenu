using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000129 RID: 297
[CreateAssetMenu(fileName = "Valuables - _____", menuName = "Level/Level Valuables Preset", order = 2)]
public class LevelValuables : ScriptableObject
{
	// Token: 0x04001103 RID: 4355
	public List<GameObject> tiny;

	// Token: 0x04001104 RID: 4356
	public List<GameObject> small;

	// Token: 0x04001105 RID: 4357
	public List<GameObject> medium;

	// Token: 0x04001106 RID: 4358
	public List<GameObject> big;

	// Token: 0x04001107 RID: 4359
	public List<GameObject> wide;

	// Token: 0x04001108 RID: 4360
	public List<GameObject> tall;

	// Token: 0x04001109 RID: 4361
	public List<GameObject> veryTall;
}
