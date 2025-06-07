using System;
using UnityEngine;

// Token: 0x0200001B RID: 27
[CreateAssetMenu(fileName = "HingeAudio - _____", menuName = "Audio/Hinge Audio Preset", order = 1)]
public class HingeAudio : ScriptableObject
{
	// Token: 0x06000061 RID: 97 RVA: 0x00003C37 File Offset: 0x00001E37
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		this.moveLoop.Type = AudioManager.AudioType.MaterialImpact;
		this.moveLoopEnd.Type = AudioManager.AudioType.MaterialImpact;
	}

	// Token: 0x040000B5 RID: 181
	[Header("Move Loop")]
	public bool moveLoopEnabled = true;

	// Token: 0x040000B6 RID: 182
	public float moveLoopVelocityMult = 1f;

	// Token: 0x040000B7 RID: 183
	public float moveLoopThreshold = 1f;

	// Token: 0x040000B8 RID: 184
	public float moveLoopFadeInSpeed = 5f;

	// Token: 0x040000B9 RID: 185
	public float moveLoopFadeOutSpeed = 5f;

	// Token: 0x040000BA RID: 186
	public Sound moveLoop;

	// Token: 0x040000BB RID: 187
	public Sound moveLoopEnd;

	// Token: 0x040000BC RID: 188
	[Header("One Shots")]
	public Sound Close;

	// Token: 0x040000BD RID: 189
	public Sound CloseHeavy;

	// Token: 0x040000BE RID: 190
	public Sound Open;

	// Token: 0x040000BF RID: 191
	public Sound OpenHeavy;

	// Token: 0x040000C0 RID: 192
	public Sound HingeBreak;
}
