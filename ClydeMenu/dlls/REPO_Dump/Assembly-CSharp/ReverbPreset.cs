using System;
using UnityEngine;

// Token: 0x0200001E RID: 30
[CreateAssetMenu(fileName = "Reverb - _____", menuName = "Audio/Reverb Preset", order = 0)]
public class ReverbPreset : ScriptableObject
{
	// Token: 0x0600006B RID: 107 RVA: 0x000045B1 File Offset: 0x000027B1
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		if (Application.isPlaying)
		{
			ReverbDirector.instance.Set();
		}
	}

	// Token: 0x040000F9 RID: 249
	[Range(-10000f, 0f)]
	public float dryLevel;

	// Token: 0x040000FA RID: 250
	[Range(-10000f, 0f)]
	public float room = -686f;

	// Token: 0x040000FB RID: 251
	[Range(-10000f, 0f)]
	public float roomHF = -454f;

	// Token: 0x040000FC RID: 252
	[Range(0.1f, 20f)]
	public float decayTime = 1f;

	// Token: 0x040000FD RID: 253
	[Range(0.1f, 2f)]
	public float decayHFRatio = 0.83f;

	// Token: 0x040000FE RID: 254
	[Range(-10000f, 1000f)]
	public float reflections = -1646f;

	// Token: 0x040000FF RID: 255
	[Range(0f, 0.3f)]
	public float reflectDelay;

	// Token: 0x04000100 RID: 256
	[Range(-10000f, 2000f)]
	public float reverb = 53f;

	// Token: 0x04000101 RID: 257
	[Range(0f, 0.1f)]
	public float reverbDelay;

	// Token: 0x04000102 RID: 258
	[Range(0f, 100f)]
	public float diffusion = 100f;

	// Token: 0x04000103 RID: 259
	[Range(0f, 100f)]
	public float density = 100f;

	// Token: 0x04000104 RID: 260
	[Range(20f, 20000f)]
	public float hfReference = 5000f;

	// Token: 0x04000105 RID: 261
	[Range(-10000f, 0f)]
	public float roomLF = -4659f;

	// Token: 0x04000106 RID: 262
	[Range(20f, 1000f)]
	public float lfReference = 250f;
}
