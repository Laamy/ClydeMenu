using System;
using UnityEngine;

// Token: 0x020000F0 RID: 240
[CreateAssetMenu(fileName = "Screen Text Reveal Delay Preset", menuName = "Semi Presets/Screen Text Reveal Delay Preset")]
public class ScreenTextRevealDelaySettings : ScriptableObject
{
	// Token: 0x06000864 RID: 2148 RVA: 0x00051524 File Offset: 0x0004F724
	public float GetDelay()
	{
		return this.delay;
	}

	// Token: 0x04000F7F RID: 3967
	public float delay = 0.1f;
}
