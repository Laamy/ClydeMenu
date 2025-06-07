using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E6 RID: 230
[CreateAssetMenu(fileName = "NewMoon", menuName = "Other/Moon")]
public class Moon : ScriptableObject
{
	// Token: 0x04000F08 RID: 3848
	[Space]
	public string moonName = "N/A";

	// Token: 0x04000F09 RID: 3849
	public Sprite moonIcon;

	// Token: 0x04000F0A RID: 3850
	[Space]
	public List<string> moonAttributes;
}
