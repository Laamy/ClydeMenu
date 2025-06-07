using System;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class AmbienceLoop : MonoBehaviour
{
	// Token: 0x06000012 RID: 18 RVA: 0x00002448 File Offset: 0x00000648
	private void Awake()
	{
		AmbienceLoop.instance = this;
	}

	// Token: 0x06000013 RID: 19 RVA: 0x00002450 File Offset: 0x00000650
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (PlayerController.instance.playerAvatarScript.RoomVolumeCheck.CurrentRooms.Count > 0)
		{
			RoomAmbience roomAmbience = PlayerController.instance.playerAvatarScript.RoomVolumeCheck.CurrentRooms[0].RoomAmbience;
			if (roomAmbience && roomAmbience != this.roomAmbience)
			{
				this.roomAmbience = roomAmbience;
				this.roomLerpAmount = 0f;
				this.roomVolumePrevious = this.roomVolumeCurrent;
			}
		}
		if (!this.roomAmbience)
		{
			return;
		}
		if (this.roomLerpAmount < 1f)
		{
			this.roomLerpAmount += this.roomLerpSpeed * Time.deltaTime;
			float t = this.roomCurve.Evaluate(this.roomLerpAmount);
			this.roomVolumeCurrent = Mathf.Lerp(this.roomVolumePrevious, this.roomAmbience.volume, t);
		}
		this.source.volume = this.volume * this.roomVolumeCurrent;
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00002558 File Offset: 0x00000758
	public void Setup()
	{
		foreach (LevelAmbience levelAmbience in LevelGenerator.Instance.Level.AmbiencePresets)
		{
			if (levelAmbience.loopClip)
			{
				this.clip = levelAmbience.loopClip;
				this.volume = levelAmbience.loopVolume;
			}
		}
		this.source.clip = this.clip;
		this.source.volume = 0f;
		this.source.loop = true;
		this.source.Play();
	}

	// Token: 0x06000015 RID: 21 RVA: 0x0000260C File Offset: 0x0000080C
	public void LiveUpdate()
	{
		foreach (LevelAmbience levelAmbience in LevelGenerator.Instance.Level.AmbiencePresets)
		{
			if (levelAmbience.loopClip)
			{
				this.clip = levelAmbience.loopClip;
				this.volume = levelAmbience.loopVolume;
			}
		}
		this.source.volume = this.volume;
	}

	// Token: 0x04000016 RID: 22
	public static AmbienceLoop instance;

	// Token: 0x04000017 RID: 23
	public AudioSource source;

	// Token: 0x04000018 RID: 24
	private AudioClip clip;

	// Token: 0x04000019 RID: 25
	private float volume;

	// Token: 0x0400001A RID: 26
	[Space]
	public AnimationCurve roomCurve;

	// Token: 0x0400001B RID: 27
	public float roomLerpSpeed = 1f;

	// Token: 0x0400001C RID: 28
	private float roomLerpAmount;

	// Token: 0x0400001D RID: 29
	private float roomVolumePrevious;

	// Token: 0x0400001E RID: 30
	private float roomVolumeCurrent;

	// Token: 0x0400001F RID: 31
	private RoomAmbience roomAmbience;
}
