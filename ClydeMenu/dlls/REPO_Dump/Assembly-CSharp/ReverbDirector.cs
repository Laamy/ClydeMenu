using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200001D RID: 29
public class ReverbDirector : MonoBehaviour
{
	// Token: 0x06000065 RID: 101 RVA: 0x00003D05 File Offset: 0x00001F05
	private void Awake()
	{
		ReverbDirector.instance = this;
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00003D0D File Offset: 0x00001F0D
	private void Start()
	{
		this.Set();
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00003D18 File Offset: 0x00001F18
	public void Set()
	{
		this.dryLevel = this.currentPreset.dryLevel;
		this.dryLevelNew = this.dryLevel;
		this.dryLevelOld = this.dryLevel;
		this.mixer.SetFloat("ReverbDryLevel", this.dryLevel);
		this.room = this.currentPreset.room;
		this.roomNew = this.room;
		this.roomOld = this.room;
		this.mixer.SetFloat("ReverbRoom", this.room);
		this.roomHF = this.currentPreset.roomHF;
		this.roomHFNew = this.roomHF;
		this.roomHFOld = this.roomHF;
		this.mixer.SetFloat("ReverbRoomHF", this.roomHF);
		this.decayTime = this.currentPreset.decayTime;
		this.decayTimeNew = this.decayTime;
		this.decayTimeOld = this.decayTime;
		this.mixer.SetFloat("ReverbDecayTime", this.decayTime);
		this.decayHFRatio = this.currentPreset.decayHFRatio;
		this.decayHFRatioNew = this.decayHFRatio;
		this.decayHFRatioOld = this.decayHFRatio;
		this.mixer.SetFloat("ReverbDecayHFRatio", this.decayHFRatio);
		this.reflections = this.currentPreset.reflections;
		this.reflectionsNew = this.reflections;
		this.reflectionsOld = this.reflections;
		this.mixer.SetFloat("ReverbReflections", this.reflections);
		this.reflectDelay = this.currentPreset.reflectDelay;
		this.reflectDelayNew = this.reflectDelay;
		this.reflectDelayOld = this.reflectDelay;
		this.mixer.SetFloat("ReverbReflectDelay", this.reflectDelay);
		this.reverb = this.currentPreset.reverb;
		this.reverbNew = this.reverb;
		this.reverbOld = this.reverb;
		this.mixer.SetFloat("ReverbReverb", this.reverb);
		this.reverbDelay = this.currentPreset.reverbDelay;
		this.reverbDelayNew = this.reverbDelay;
		this.reverbDelayOld = this.reverbDelay;
		this.mixer.SetFloat("ReverbReverbDelay", this.reverbDelay);
		this.diffusion = this.currentPreset.diffusion;
		this.diffusionNew = this.diffusion;
		this.diffusionOld = this.diffusion;
		this.mixer.SetFloat("ReverbDiffusion", this.diffusion);
		this.density = this.currentPreset.density;
		this.densityNew = this.density;
		this.densityOld = this.density;
		this.mixer.SetFloat("ReverbDensity", this.density);
		this.hfReference = this.currentPreset.hfReference;
		this.hfReferenceNew = this.hfReference;
		this.hfReferenceOld = this.hfReference;
		this.mixer.SetFloat("ReverbHFReference", this.hfReference);
		this.roomLF = this.currentPreset.roomLF;
		this.roomLFNew = this.roomLF;
		this.roomLFOld = this.roomLF;
		this.mixer.SetFloat("ReverbRoomLF", this.roomLF);
		this.lfReference = this.currentPreset.lfReference;
		this.lfReferenceNew = this.lfReference;
		this.lfReferenceOld = this.lfReference;
		this.mixer.SetFloat("ReverbLFReference", this.lfReference);
	}

	// Token: 0x06000068 RID: 104 RVA: 0x000040A8 File Offset: 0x000022A8
	private void NewPreset()
	{
		this.dryLevelOld = this.dryLevel;
		this.dryLevelNew = this.currentPreset.dryLevel;
		this.roomOld = this.room;
		this.roomNew = this.currentPreset.room;
		this.roomHFOld = this.roomHF;
		this.roomHFNew = this.currentPreset.roomHF;
		this.decayTimeOld = this.decayTime;
		this.decayTimeNew = this.currentPreset.decayTime;
		this.decayHFRatioOld = this.decayHFRatio;
		this.decayHFRatioNew = this.currentPreset.decayHFRatio;
		this.reflectionsOld = this.reflections;
		this.reflectionsNew = this.currentPreset.reflections;
		this.reflectDelayOld = this.reflectDelay;
		this.reflectDelayNew = this.currentPreset.reflectDelay;
		this.reverbOld = this.reverb;
		this.reverbNew = this.currentPreset.reverb;
		this.reverbDelayOld = this.reverbDelay;
		this.reverbDelayNew = this.currentPreset.reverbDelay;
		this.diffusionOld = this.diffusion;
		this.diffusionNew = this.currentPreset.diffusion;
		this.densityOld = this.density;
		this.densityNew = this.currentPreset.density;
		this.hfReferenceOld = this.hfReference;
		this.hfReferenceNew = this.currentPreset.hfReference;
		this.roomLFOld = this.roomLF;
		this.roomLFNew = this.currentPreset.roomLF;
		this.lfReferenceOld = this.lfReference;
		this.lfReferenceNew = this.currentPreset.lfReference;
		this.lerpAmount = 0f;
	}

	// Token: 0x06000069 RID: 105 RVA: 0x00004258 File Offset: 0x00002458
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (PlayerController.instance.playerAvatarScript.RoomVolumeCheck.CurrentRooms.Count > 0)
		{
			ReverbPreset reverbPreset = PlayerController.instance.playerAvatarScript.RoomVolumeCheck.CurrentRooms[0].ReverbPreset;
			if (reverbPreset && reverbPreset != this.currentPreset)
			{
				this.currentPreset = reverbPreset;
				this.NewPreset();
			}
		}
		if (this.lerpAmount < 1f)
		{
			this.lerpAmount += this.lerpSpeed * Time.deltaTime;
			float t = this.reverbCurve.Evaluate(this.lerpAmount);
			this.dryLevel = Mathf.Lerp(this.dryLevelOld, this.dryLevelNew, t);
			this.mixer.SetFloat("ReverbDryLevel", this.dryLevel);
			this.room = Mathf.Lerp(this.roomOld, this.roomNew, t);
			this.mixer.SetFloat("ReverbRoom", this.room);
			this.roomHF = Mathf.Lerp(this.roomHFOld, this.roomHFNew, t);
			this.mixer.SetFloat("ReverbRoomHF", this.roomHF);
			this.decayTime = Mathf.Lerp(this.decayTimeOld, this.decayTimeNew, t);
			this.mixer.SetFloat("ReverbDecayTime", this.decayTime);
			this.decayHFRatio = Mathf.Lerp(this.decayHFRatioOld, this.decayHFRatioNew, t);
			this.mixer.SetFloat("ReverbDecayHFRatio", this.decayHFRatio);
			this.reflections = Mathf.Lerp(this.reflectionsOld, this.reflectionsNew, t);
			this.mixer.SetFloat("ReverbReflections", this.reflections);
			this.reflectDelay = Mathf.Lerp(this.reflectDelayOld, this.reflectDelayNew, t);
			this.mixer.SetFloat("ReverbReflectDelay", this.reflectDelay);
			this.reverb = Mathf.Lerp(this.reverbOld, this.reverbNew, t);
			this.mixer.SetFloat("ReverbReverb", this.reverb);
			this.reverbDelay = Mathf.Lerp(this.reverbDelayOld, this.reverbDelayNew, t);
			this.mixer.SetFloat("ReverbReverbDelay", this.reverbDelay);
			this.diffusion = Mathf.Lerp(this.diffusionOld, this.diffusionNew, t);
			this.mixer.SetFloat("ReverbDiffusion", this.diffusion);
			this.density = Mathf.Lerp(this.densityOld, this.densityNew, t);
			this.mixer.SetFloat("ReverbDensity", this.density);
			this.hfReference = Mathf.Lerp(this.hfReferenceOld, this.hfReferenceNew, t);
			this.mixer.SetFloat("ReverbHFReference", this.hfReference);
			this.roomLF = Mathf.Lerp(this.roomLFOld, this.roomLFNew, t);
			this.mixer.SetFloat("ReverbRoomLF", this.roomLF);
			this.lfReference = Mathf.Lerp(this.lfReferenceOld, this.lfReferenceNew, t);
			this.mixer.SetFloat("ReverbLFReference", this.lfReference);
		}
	}

	// Token: 0x040000C8 RID: 200
	public static ReverbDirector instance;

	// Token: 0x040000C9 RID: 201
	public AudioMixer mixer;

	// Token: 0x040000CA RID: 202
	[Space]
	public ReverbPreset currentPreset;

	// Token: 0x040000CB RID: 203
	[Space]
	public AnimationCurve reverbCurve;

	// Token: 0x040000CC RID: 204
	public float lerpSpeed = 1f;

	// Token: 0x040000CD RID: 205
	private float lerpAmount;

	// Token: 0x040000CE RID: 206
	private float dryLevel;

	// Token: 0x040000CF RID: 207
	private float dryLevelOld;

	// Token: 0x040000D0 RID: 208
	private float dryLevelNew;

	// Token: 0x040000D1 RID: 209
	private float room;

	// Token: 0x040000D2 RID: 210
	private float roomOld;

	// Token: 0x040000D3 RID: 211
	private float roomNew;

	// Token: 0x040000D4 RID: 212
	private float roomHF;

	// Token: 0x040000D5 RID: 213
	private float roomHFOld;

	// Token: 0x040000D6 RID: 214
	private float roomHFNew;

	// Token: 0x040000D7 RID: 215
	private float decayTime;

	// Token: 0x040000D8 RID: 216
	private float decayTimeOld;

	// Token: 0x040000D9 RID: 217
	private float decayTimeNew;

	// Token: 0x040000DA RID: 218
	private float decayHFRatio;

	// Token: 0x040000DB RID: 219
	private float decayHFRatioOld;

	// Token: 0x040000DC RID: 220
	private float decayHFRatioNew;

	// Token: 0x040000DD RID: 221
	private float reflections;

	// Token: 0x040000DE RID: 222
	private float reflectionsOld;

	// Token: 0x040000DF RID: 223
	private float reflectionsNew;

	// Token: 0x040000E0 RID: 224
	private float reflectDelay;

	// Token: 0x040000E1 RID: 225
	private float reflectDelayOld;

	// Token: 0x040000E2 RID: 226
	private float reflectDelayNew;

	// Token: 0x040000E3 RID: 227
	private float reverb;

	// Token: 0x040000E4 RID: 228
	private float reverbOld;

	// Token: 0x040000E5 RID: 229
	private float reverbNew;

	// Token: 0x040000E6 RID: 230
	private float reverbDelay;

	// Token: 0x040000E7 RID: 231
	private float reverbDelayOld;

	// Token: 0x040000E8 RID: 232
	private float reverbDelayNew;

	// Token: 0x040000E9 RID: 233
	private float diffusion;

	// Token: 0x040000EA RID: 234
	private float diffusionOld;

	// Token: 0x040000EB RID: 235
	private float diffusionNew;

	// Token: 0x040000EC RID: 236
	private float density;

	// Token: 0x040000ED RID: 237
	private float densityOld;

	// Token: 0x040000EE RID: 238
	private float densityNew;

	// Token: 0x040000EF RID: 239
	private float hfReference;

	// Token: 0x040000F0 RID: 240
	private float hfReferenceOld;

	// Token: 0x040000F1 RID: 241
	private float hfReferenceNew;

	// Token: 0x040000F2 RID: 242
	private float roomLF;

	// Token: 0x040000F3 RID: 243
	private float roomLFOld;

	// Token: 0x040000F4 RID: 244
	private float roomLFNew;

	// Token: 0x040000F5 RID: 245
	private float lfReference;

	// Token: 0x040000F6 RID: 246
	private float lfReferenceOld;

	// Token: 0x040000F7 RID: 247
	private float lfReferenceNew;

	// Token: 0x040000F8 RID: 248
	private float spawnTimer;
}
