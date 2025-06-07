using System;
using UnityEngine;

// Token: 0x020000EF RID: 239
[CreateAssetMenu(fileName = "Screen Next Message Delay Preset", menuName = "Semi Presets/Screen Next Message Delay Preset")]
public class ScreenNextMessageDelaySettings : ScriptableObject
{
	// Token: 0x06000862 RID: 2146 RVA: 0x00051509 File Offset: 0x0004F709
	public float GetDelay()
	{
		return this.delay;
	}

	// Token: 0x04000F7E RID: 3966
	public float delay = 1f;
}
