using System;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class AudioPlay : MonoBehaviour
{
	// Token: 0x0600003A RID: 58 RVA: 0x0000330C File Offset: 0x0000150C
	public void Play(float volumeMult)
	{
		this.source.clip = this.sounds[Random.Range(0, this.sounds.Length)];
		this.source.volume = (this.volume + Random.Range(-this.volumeRandom, this.volumeRandom)) * volumeMult;
		this.source.pitch = this.pitch + Random.Range(-this.pitchRandom, this.pitchRandom);
		this.source.PlayOneShot(this.source.clip);
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00003399 File Offset: 0x00001599
	public void Update()
	{
		if (this.playImpulse)
		{
			this.Play(1f);
			this.playImpulse = false;
		}
	}

	// Token: 0x04000060 RID: 96
	public AudioSource source;

	// Token: 0x04000061 RID: 97
	public AudioClip[] sounds;

	// Token: 0x04000062 RID: 98
	[Range(0f, 1f)]
	public float volume = 0.5f;

	// Token: 0x04000063 RID: 99
	[Range(0f, 1f)]
	public float volumeRandom = 0.1f;

	// Token: 0x04000064 RID: 100
	[Range(0f, 5f)]
	public float pitch = 1f;

	// Token: 0x04000065 RID: 101
	[Range(0f, 2f)]
	public float pitchRandom = 0.1f;

	// Token: 0x04000066 RID: 102
	public bool playImpulse;
}
