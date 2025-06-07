using System;
using UnityEngine;

// Token: 0x0200001C RID: 28
[CreateAssetMenu(fileName = "PhysAudio - _____", menuName = "Audio/Physics Audio Preset", order = 1)]
public class PhysAudio : ScriptableObject
{
	// Token: 0x06000063 RID: 99 RVA: 0x00003C94 File Offset: 0x00001E94
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		this.impactLight.Type = AudioManager.AudioType.MaterialImpact;
		this.impactMedium.Type = AudioManager.AudioType.MaterialImpact;
		this.impactHeavy.Type = AudioManager.AudioType.MaterialImpact;
		this.breakLight.Type = AudioManager.AudioType.MaterialImpact;
		this.breakMedium.Type = AudioManager.AudioType.MaterialImpact;
		this.breakHeavy.Type = AudioManager.AudioType.MaterialImpact;
		this.destroy.Type = AudioManager.AudioType.MaterialImpact;
	}

	// Token: 0x040000C1 RID: 193
	public Sound impactLight;

	// Token: 0x040000C2 RID: 194
	public Sound impactMedium;

	// Token: 0x040000C3 RID: 195
	public Sound impactHeavy;

	// Token: 0x040000C4 RID: 196
	[Space(20f)]
	public Sound breakLight;

	// Token: 0x040000C5 RID: 197
	public Sound breakMedium;

	// Token: 0x040000C6 RID: 198
	public Sound breakHeavy;

	// Token: 0x040000C7 RID: 199
	[Space(20f)]
	public Sound destroy;
}
