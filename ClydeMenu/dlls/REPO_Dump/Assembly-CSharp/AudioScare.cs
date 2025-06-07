using System;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class AudioScare : MonoBehaviour
{
	// Token: 0x0600003D RID: 61 RVA: 0x000033E9 File Offset: 0x000015E9
	private void Awake()
	{
		AudioScare.instance = this;
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00003400 File Offset: 0x00001600
	public void PlayImpact()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		LevelMusic.instance.Interrupt(20f);
		this.source.volume = 1f;
		this.source.clip = this.impactSounds[Random.Range(0, this.impactSounds.Length)];
		this.source.Play();
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00003470 File Offset: 0x00001670
	public void PlaySoft()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		LevelMusic.instance.Interrupt(20f);
		this.source.volume = 1f;
		this.source.clip = this.softSounds[Random.Range(0, this.softSounds.Length)];
		this.source.Play();
	}

	// Token: 0x06000040 RID: 64 RVA: 0x000034E0 File Offset: 0x000016E0
	public void PlayCustom(AudioClip _sound, float _volume = 0.3f, float _interrupt = 20f)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		LevelMusic.instance.Interrupt(_interrupt);
		this.source.Stop();
		this.source.volume = _volume;
		this.source.clip = _sound;
		this.source.Play();
	}

	// Token: 0x04000067 RID: 103
	public static AudioScare instance;

	// Token: 0x04000068 RID: 104
	private AudioSource source;

	// Token: 0x04000069 RID: 105
	public AudioClip[] impactSounds;

	// Token: 0x0400006A RID: 106
	public AudioClip[] softSounds;
}
