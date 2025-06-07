using System;
using UnityEngine;

// Token: 0x020001E8 RID: 488
[CreateAssetMenu(fileName = "Effect Presets", menuName = "Effect Presets/Explosion Preset", order = 1)]
public class ExplosionPreset : ScriptableObject
{
	// Token: 0x04001C5A RID: 7258
	[Header("Settings")]
	[Space(5f)]
	public float explosionForceMultiplier = 1f;

	// Token: 0x04001C5B RID: 7259
	[Space(20f)]
	[Header("Colors")]
	[Space(5f)]
	public Gradient explosionColors;

	// Token: 0x04001C5C RID: 7260
	public Gradient smokeColors;

	// Token: 0x04001C5D RID: 7261
	public Gradient lightColor;

	// Token: 0x04001C5E RID: 7262
	[Space(20f)]
	[Header("Sounds")]
	[Space(5f)]
	public Sound explosionSoundSmall;

	// Token: 0x04001C5F RID: 7263
	public Sound explosionSoundSmallGlobal;

	// Token: 0x04001C60 RID: 7264
	public Sound explosionSoundMedium;

	// Token: 0x04001C61 RID: 7265
	public Sound explosionSoundMediumGlobal;

	// Token: 0x04001C62 RID: 7266
	public Sound explosionSoundBig;

	// Token: 0x04001C63 RID: 7267
	public Sound explosionSoundBigGlobal;
}
