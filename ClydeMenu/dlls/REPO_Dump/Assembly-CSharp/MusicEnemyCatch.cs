using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class MusicEnemyCatch : MonoBehaviour
{
	// Token: 0x06000053 RID: 83 RVA: 0x00003AAC File Offset: 0x00001CAC
	public void Play()
	{
		this.Source.clip = this.Sounds[Random.Range(0, this.Sounds.Length)];
		this.Source.Play();
		this.LevelMusic.Interrupt(10f);
	}

	// Token: 0x04000091 RID: 145
	public LevelMusic LevelMusic;

	// Token: 0x04000092 RID: 146
	public AudioSource Source;

	// Token: 0x04000093 RID: 147
	public AudioClip[] Sounds;
}
