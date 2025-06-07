using System;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class ConstantMusic : MonoBehaviour
{
	// Token: 0x06000046 RID: 70 RVA: 0x000035EF File Offset: 0x000017EF
	private void Awake()
	{
		ConstantMusic.instance = this;
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00003604 File Offset: 0x00001804
	private void Update()
	{
		if (!this.setup && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			this.audioSource.clip = this.clip;
			this.audioSource.volume = this.volume;
			this.audioSource.Play();
			this.setup = true;
		}
	}

	// Token: 0x06000048 RID: 72 RVA: 0x0000365C File Offset: 0x0000185C
	public void Setup()
	{
		if (!LevelGenerator.Instance.Level.ConstantMusicPreset)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.clip = LevelGenerator.Instance.Level.ConstantMusicPreset.clip;
		this.volume = LevelGenerator.Instance.Level.ConstantMusicPreset.volume;
	}

	// Token: 0x0400007A RID: 122
	public static ConstantMusic instance;

	// Token: 0x0400007B RID: 123
	private AudioSource audioSource;

	// Token: 0x0400007C RID: 124
	private bool setup;

	// Token: 0x0400007D RID: 125
	private AudioClip clip;

	// Token: 0x0400007E RID: 126
	private float volume;
}
