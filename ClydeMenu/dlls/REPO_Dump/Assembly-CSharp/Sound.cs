using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000020 RID: 32
[Serializable]
public class Sound
{
	// Token: 0x0600006E RID: 110 RVA: 0x0000466C File Offset: 0x0000286C
	public AudioSource Play(Vector3 position, float volumeMultiplier = 1f, float falloffMultiplier = 1f, float offscreenVolumeMultiplier = 1f, float offscreenFalloffMultiplier = 1f)
	{
		if (this.Sounds.Length == 0)
		{
			return null;
		}
		AudioClip audioClip = this.Sounds[Random.Range(0, this.Sounds.Length)];
		float num = this.Pitch + Random.Range(-this.PitchRandom, this.PitchRandom);
		AudioSource audioSource = this.Source;
		if (!audioSource)
		{
			GameObject gameObject = AudioManager.instance.AudioDefault;
			switch (this.Type)
			{
			case AudioManager.AudioType.HighFalloff:
				gameObject = AudioManager.instance.AudioHighFalloff;
				break;
			case AudioManager.AudioType.Footstep:
				gameObject = AudioManager.instance.AudioFootstep;
				break;
			case AudioManager.AudioType.MaterialImpact:
				gameObject = AudioManager.instance.AudioMaterialImpact;
				break;
			case AudioManager.AudioType.Cutscene:
				gameObject = AudioManager.instance.AudioCutscene;
				break;
			case AudioManager.AudioType.AmbienceBreaker:
				gameObject = AudioManager.instance.AudioAmbienceBreaker;
				break;
			case AudioManager.AudioType.LowFalloff:
				gameObject = AudioManager.instance.AudioLowFalloff;
				break;
			case AudioManager.AudioType.Global:
				gameObject = AudioManager.instance.AudioGlobal;
				break;
			case AudioManager.AudioType.HigherFalloff:
				gameObject = AudioManager.instance.AudioHigherFalloff;
				break;
			case AudioManager.AudioType.Attack:
				gameObject = AudioManager.instance.AudioAttack;
				break;
			case AudioManager.AudioType.Persistent:
				gameObject = AudioManager.instance.AudioPersistent;
				break;
			}
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, position, Quaternion.identity, AudioManager.instance.SoundsParent);
			gameObject2.gameObject.name = audioClip.name;
			audioSource = gameObject2.GetComponent<AudioSource>();
			if (gameObject != AudioManager.instance.AudioPersistent)
			{
				Object.Destroy(gameObject2, audioClip.length / num);
			}
		}
		else if (!audioSource.enabled)
		{
			return null;
		}
		audioSource.minDistance *= this.FalloffMultiplier;
		audioSource.minDistance *= falloffMultiplier;
		audioSource.maxDistance *= this.FalloffMultiplier;
		audioSource.maxDistance *= falloffMultiplier;
		audioSource.clip = this.Sounds[Random.Range(0, this.Sounds.Length)];
		audioSource.volume = (this.Volume + Random.Range(-this.VolumeRandom, this.VolumeRandom)) * volumeMultiplier;
		if (this.SpatialBlend > 0f && (this.OffscreenVolume * offscreenVolumeMultiplier < 1f || this.OffscreenFalloff * offscreenFalloffMultiplier < 1f) && !SemiFunc.OnScreen(audioSource.transform.position, 0.1f, 0.1f))
		{
			audioSource.volume *= this.OffscreenVolume * offscreenVolumeMultiplier;
			audioSource.minDistance *= this.OffscreenFalloff * offscreenFalloffMultiplier;
			audioSource.maxDistance *= this.OffscreenFalloff * offscreenFalloffMultiplier;
		}
		audioSource.spatialBlend = this.SpatialBlend;
		audioSource.reverbZoneMix = this.ReverbMix;
		audioSource.dopplerLevel = this.Doppler;
		audioSource.pitch = num;
		audioSource.loop = false;
		if (this.SpatialBlend > 0f)
		{
			this.StartLowPass(audioSource);
		}
		audioSource.Play();
		return audioSource;
	}

	// Token: 0x0600006F RID: 111 RVA: 0x00004944 File Offset: 0x00002B44
	public void Stop()
	{
		this.Source.Stop();
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00004954 File Offset: 0x00002B54
	private void StartLowPass(AudioSource source)
	{
		this.LowPassLogic = source.GetComponent<AudioLowPassLogic>();
		if (this.LowPassLogic)
		{
			if (this.LowPassIgnoreColliders.Count > 0)
			{
				this.LowPassLogic.LowPassIgnoreColliders.AddRange(this.LowPassIgnoreColliders);
			}
			this.LowPassLogic.Setup();
			this.HasLowPassLogic = true;
		}
	}

	// Token: 0x06000071 RID: 113 RVA: 0x000049B0 File Offset: 0x00002BB0
	public void PlayLoop(bool playing, float fadeInSpeed, float fadeOutSpeed, float pitchMultiplier = 1f)
	{
		if (!this.AudioInfoFetched)
		{
			this.LoopClip = this.Sounds[Random.Range(0, this.Sounds.Length)];
			this.Source.clip = this.LoopClip;
		}
		if (!playing)
		{
			if (this.Source.isPlaying)
			{
				this.LoopVolumeCurrent -= fadeOutSpeed * Time.deltaTime;
				this.LoopVolumeCurrent = Mathf.Clamp(this.LoopVolumeCurrent, 0f, this.LoopVolume);
				this.LoopOffScreenLogic();
				this.Source.pitch = this.LoopPitch * pitchMultiplier;
				if (this.HasLowPassLogic)
				{
					this.LowPassLogic.Volume = this.LoopVolumeFinal;
				}
				else
				{
					this.Source.volume = this.LoopVolumeFinal;
				}
				if (this.LoopVolumeFinal <= 0f)
				{
					this.Source.Stop();
				}
			}
			return;
		}
		if (!this.Source.isPlaying)
		{
			this.LoopVolume = this.Volume + Random.Range(-this.VolumeRandom, this.VolumeRandom);
			this.LoopPitch = this.Pitch + Random.Range(-this.PitchRandom, this.PitchRandom);
			this.LoopVolumeCurrent = 0f;
			this.LoopVolumeFinal = this.LoopVolumeCurrent;
			this.Source.volume = this.LoopVolumeCurrent;
			this.Source.pitch = this.LoopPitch * pitchMultiplier;
			this.Source.spatialBlend = this.SpatialBlend;
			this.Source.reverbZoneMix = this.ReverbMix;
			this.Source.dopplerLevel = this.Doppler;
			this.Source.minDistance *= this.FalloffMultiplier;
			this.Source.maxDistance *= this.FalloffMultiplier;
			this.LoopFalloff = this.Source.maxDistance;
			this.Source.time = Random.Range(0f, this.Source.clip.length);
			if (this.StartTimeOverride != 999999f)
			{
				this.Source.time = this.StartTimeOverride;
			}
			this.Source.loop = true;
			this.StartLowPass(this.Source);
			this.Source.Play();
			return;
		}
		this.LoopVolumeCurrent += fadeInSpeed * Time.deltaTime;
		this.LoopVolumeCurrent = Mathf.Clamp(this.LoopVolumeCurrent, 0f, this.LoopVolume);
		this.LoopOffScreenLogic();
		this.Source.pitch = this.LoopPitch * pitchMultiplier;
		if (this.HasLowPassLogic)
		{
			this.LowPassLogic.Volume = this.LoopVolumeFinal;
			return;
		}
		this.Source.volume = this.LoopVolumeFinal;
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00004C70 File Offset: 0x00002E70
	private void LoopOffScreenLogic()
	{
		this.LoopVolumeFinal = this.LoopVolumeCurrent;
		if (this.SpatialBlend > 0f && (this.OffscreenVolume < 1f || this.OffscreenFalloff < 1f))
		{
			if (this.LoopOffScreenTimer <= 0f)
			{
				this.LoopOffScreenTimer = this.LoopOffScreenTime;
				this.LoopOffScreen = !SemiFunc.OnScreen(this.Source.transform.position, 0.1f, 0.1f);
			}
			else
			{
				this.LoopOffScreenTimer -= Time.deltaTime;
			}
			if (this.OffscreenVolume < 1f)
			{
				if (this.LoopOffScreen)
				{
					this.LoopOffScreenVolume = Mathf.Lerp(this.LoopOffScreenVolume, this.OffscreenVolume, 15f * Time.deltaTime);
				}
				else
				{
					this.LoopOffScreenVolume = Mathf.Lerp(this.LoopOffScreenVolume, 1f, 15f * Time.deltaTime);
				}
				this.LoopVolumeFinal *= this.LoopOffScreenVolume;
			}
			if (this.OffscreenFalloff < 1f)
			{
				if (this.LoopOffScreen)
				{
					this.LoopFalloffFinal = Mathf.Lerp(this.LoopFalloffFinal, this.LoopFalloff * this.OffscreenFalloff, 15f * Time.deltaTime);
				}
				else
				{
					this.LoopFalloffFinal = Mathf.Lerp(this.LoopFalloffFinal, this.LoopFalloff, 15f * Time.deltaTime);
				}
				if (this.HasLowPassLogic)
				{
					this.LowPassLogic.Falloff = this.LoopFalloffFinal;
					return;
				}
				this.Source.maxDistance = this.LoopFalloffFinal;
			}
		}
	}

	// Token: 0x06000073 RID: 115 RVA: 0x00004E08 File Offset: 0x00003008
	public static void CopySound(Sound from, Sound to)
	{
		to.Source = from.Source;
		to.Sounds = from.Sounds;
		to.Type = from.Type;
		to.Volume = from.Volume;
		to.VolumeRandom = from.VolumeRandom;
		to.Pitch = from.Pitch;
		to.PitchRandom = from.PitchRandom;
		to.SpatialBlend = from.SpatialBlend;
		to.ReverbMix = from.ReverbMix;
		to.Doppler = from.Doppler;
	}

	// Token: 0x04000108 RID: 264
	public AudioSource Source;

	// Token: 0x04000109 RID: 265
	public AudioClip[] Sounds;

	// Token: 0x0400010A RID: 266
	private AudioLowPassLogic LowPassLogic;

	// Token: 0x0400010B RID: 267
	private bool HasLowPassLogic;

	// Token: 0x0400010C RID: 268
	public AudioManager.AudioType Type;

	// Token: 0x0400010D RID: 269
	[Range(0f, 1f)]
	public float Volume = 0.5f;

	// Token: 0x0400010E RID: 270
	[Range(0f, 1f)]
	public float VolumeRandom = 0.1f;

	// Token: 0x0400010F RID: 271
	[Range(0f, 5f)]
	public float Pitch = 1f;

	// Token: 0x04000110 RID: 272
	[Range(0f, 2f)]
	public float PitchRandom = 0.1f;

	// Token: 0x04000111 RID: 273
	[Range(0f, 1f)]
	public float SpatialBlend = 1f;

	// Token: 0x04000112 RID: 274
	[Range(0f, 5f)]
	public float Doppler = 1f;

	// Token: 0x04000113 RID: 275
	[Range(0f, 1f)]
	public float ReverbMix = 1f;

	// Token: 0x04000114 RID: 276
	[Range(0f, 5f)]
	public float FalloffMultiplier = 1f;

	// Token: 0x04000115 RID: 277
	[Space]
	[Range(0f, 1f)]
	public float OffscreenVolume = 1f;

	// Token: 0x04000116 RID: 278
	[Range(0f, 1f)]
	public float OffscreenFalloff = 1f;

	// Token: 0x04000117 RID: 279
	[Space]
	public List<Collider> LowPassIgnoreColliders = new List<Collider>();

	// Token: 0x04000118 RID: 280
	private AudioClip LoopClip;

	// Token: 0x04000119 RID: 281
	internal float LoopVolume;

	// Token: 0x0400011A RID: 282
	internal float LoopVolumeCurrent;

	// Token: 0x0400011B RID: 283
	internal float LoopVolumeFinal;

	// Token: 0x0400011C RID: 284
	internal float LoopPitch;

	// Token: 0x0400011D RID: 285
	internal float LoopFalloff;

	// Token: 0x0400011E RID: 286
	internal float LoopFalloffFinal;

	// Token: 0x0400011F RID: 287
	private float LoopOffScreenTime = 0.25f;

	// Token: 0x04000120 RID: 288
	private float LoopOffScreenTimer;

	// Token: 0x04000121 RID: 289
	private bool LoopOffScreen;

	// Token: 0x04000122 RID: 290
	private float LoopOffScreenVolume;

	// Token: 0x04000123 RID: 291
	private float LoopOffScreenFalloff;

	// Token: 0x04000124 RID: 292
	internal float StartTimeOverride = 999999f;

	// Token: 0x04000125 RID: 293
	private bool AudioInfoFetched;
}
