using System;
using UnityEngine;

// Token: 0x02000005 RID: 5
[Serializable]
public class LevelAmbienceBreaker
{
	// Token: 0x06000010 RID: 16 RVA: 0x0000241B File Offset: 0x0000061B
	public void Test()
	{
		if (Application.isPlaying)
		{
			AmbienceBreakers.instance.LiveTest(this.ambience, this);
		}
	}

	// Token: 0x04000012 RID: 18
	[HideInInspector]
	public string name;

	// Token: 0x04000013 RID: 19
	public AudioClip sound;

	// Token: 0x04000014 RID: 20
	[Range(0f, 1f)]
	public float volume = 0.5f;

	// Token: 0x04000015 RID: 21
	internal LevelAmbience ambience;
}
