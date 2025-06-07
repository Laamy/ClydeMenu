using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200001A RID: 26
public class MusicManager : MonoBehaviour
{
	// Token: 0x0600005E RID: 94 RVA: 0x00003C03 File Offset: 0x00001E03
	private void Awake()
	{
		MusicManager.Instance = this;
	}

	// Token: 0x0600005F RID: 95 RVA: 0x00003C0B File Offset: 0x00001E0B
	private void Update()
	{
		if (this.MusicEnemySighting.Active)
		{
			this.MusicEnemyNear.LowerVolume(0.1f, 0.25f);
		}
	}

	// Token: 0x040000AE RID: 174
	public static MusicManager Instance;

	// Token: 0x040000AF RID: 175
	public AudioMixerSnapshot MusicMixerOn;

	// Token: 0x040000B0 RID: 176
	public AudioMixerSnapshot MusicMixerOff;

	// Token: 0x040000B1 RID: 177
	public AudioMixerSnapshot MusicMixerScareOnly;

	// Token: 0x040000B2 RID: 178
	[Space(15f)]
	public MusicEnemyNear MusicEnemyNear;

	// Token: 0x040000B3 RID: 179
	public MusicEnemySighting MusicEnemySighting;

	// Token: 0x040000B4 RID: 180
	public MusicEnemyCatch MusicEnemyCatch;
}
