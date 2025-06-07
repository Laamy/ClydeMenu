using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000004 RID: 4
[CreateAssetMenu(fileName = "Level Ambience - _____", menuName = "Level/Level Ambience", order = 2)]
public class LevelAmbience : ScriptableObject
{
	// Token: 0x0600000E RID: 14 RVA: 0x00002360 File Offset: 0x00000560
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		foreach (LevelAmbienceBreaker levelAmbienceBreaker in this.breakers)
		{
			if (levelAmbienceBreaker.sound)
			{
				levelAmbienceBreaker.name = levelAmbienceBreaker.sound.name.ToUpper();
			}
			levelAmbienceBreaker.ambience = this;
		}
		if (Application.isPlaying && LevelGenerator.Instance && LevelGenerator.Instance.Generated)
		{
			AmbienceLoop.instance.LiveUpdate();
		}
	}

	// Token: 0x0400000F RID: 15
	public AudioClip loopClip;

	// Token: 0x04000010 RID: 16
	[Range(0f, 1f)]
	public float loopVolume = 0.5f;

	// Token: 0x04000011 RID: 17
	[Space(20f)]
	public List<LevelAmbienceBreaker> breakers;
}
