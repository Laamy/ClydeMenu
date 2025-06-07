using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class LevelMusic : MonoBehaviour
{
	// Token: 0x0600004B RID: 75 RVA: 0x000036DB File Offset: 0x000018DB
	private void Awake()
	{
		LevelMusic.instance = this;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600004C RID: 76 RVA: 0x000036EF File Offset: 0x000018EF
	private void Start()
	{
		this.cooldownTime = Random.Range(this.cooldownTimeMin, this.cooldownTimeMax);
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00003714 File Offset: 0x00001914
	public void Setup()
	{
		if (!LevelGenerator.Instance.Level.MusicPreset)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.levelMusicTracksRandom = new List<LevelMusic.LevelMusicTrack>(LevelGenerator.Instance.Level.MusicPreset.tracks);
		this.levelMusicTracksRandom.Shuffle<LevelMusic.LevelMusicTrack>();
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00003770 File Offset: 0x00001970
	private void Update()
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			this.Interrupt(10f);
		}
		if (this.interrupt)
		{
			this.interruptVolumeLerp += this.fadeInterrupt * Time.deltaTime;
			this.audioSource.volume = Mathf.Lerp(this.interruptVolume, 0f, this.fadeCurve.Evaluate(this.interruptVolumeLerp));
			if (this.audioSource.volume <= 0.01f)
			{
				this.audioSource.Stop();
				this.interrupt = false;
				return;
			}
		}
		else if (this.active)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				if (!this.activePlayed)
				{
					this.audioSource.clip = this.levelMusicTracksRandom[this.trackIndex].audioClip;
					this.audioSource.volume = this.levelMusicTracksRandom[this.trackIndex].volume;
					this.audioSource.PlayScheduled(AudioSettings.dspTime + 0.5);
					if (SemiFunc.IsMultiplayer())
					{
						this.photonView.RPC("PlayTrack", RpcTarget.Others, new object[]
						{
							this.audioSource.clip.name
						});
					}
					this.trackIndex++;
					if (this.trackIndex >= this.levelMusicTracksRandom.Count)
					{
						this.trackIndex = 0;
						this.levelMusicTracksRandom.Shuffle<LevelMusic.LevelMusicTrack>();
					}
					this.activePlayed = true;
					return;
				}
				if (!this.audioSource.isPlaying)
				{
					this.active = false;
					return;
				}
			}
		}
		else if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.cooldownTime -= Time.deltaTime;
			if (this.cooldownTime <= 0f)
			{
				this.cooldownTime = Random.Range(this.cooldownTimeMin, this.cooldownTimeMax);
				this.active = true;
				this.activePlayed = false;
			}
		}
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00003954 File Offset: 0x00001B54
	public void Interrupt(float interruptSpeed)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.cooldownTime < this.cooldownTimeMin)
		{
			this.cooldownTime = Random.Range(this.cooldownTimeMin, this.cooldownTimeMax);
		}
		if (!this.audioSource.isPlaying || (this.interrupt && interruptSpeed <= this.fadeInterrupt))
		{
			return;
		}
		this.interrupt = true;
		this.fadeInterrupt = interruptSpeed;
		this.interruptVolume = this.audioSource.volume;
		this.interruptVolumeLerp = 0f;
		this.active = false;
	}

	// Token: 0x06000050 RID: 80 RVA: 0x000039E0 File Offset: 0x00001BE0
	[PunRPC]
	public void PlayTrack(string _trackName, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (!this.audioSource.isPlaying)
		{
			foreach (LevelMusic.LevelMusicTrack levelMusicTrack in this.levelMusicTracksRandom)
			{
				if (levelMusicTrack.audioClip.name == _trackName)
				{
					this.audioSource.clip = levelMusicTrack.audioClip;
					this.audioSource.volume = levelMusicTrack.volume;
					this.audioSource.PlayScheduled(AudioSettings.dspTime + 0.5);
				}
			}
		}
	}

	// Token: 0x04000081 RID: 129
	public static LevelMusic instance;

	// Token: 0x04000082 RID: 130
	private PhotonView photonView;

	// Token: 0x04000083 RID: 131
	private AudioSource audioSource;

	// Token: 0x04000084 RID: 132
	private bool active;

	// Token: 0x04000085 RID: 133
	private bool activePlayed;

	// Token: 0x04000086 RID: 134
	public AnimationCurve fadeCurve;

	// Token: 0x04000087 RID: 135
	private bool interrupt;

	// Token: 0x04000088 RID: 136
	private float fadeInterrupt;

	// Token: 0x04000089 RID: 137
	private float interruptVolume;

	// Token: 0x0400008A RID: 138
	private float interruptVolumeLerp;

	// Token: 0x0400008B RID: 139
	[Space]
	public float cooldownTimeMin;

	// Token: 0x0400008C RID: 140
	public float cooldownTimeMax;

	// Token: 0x0400008D RID: 141
	private float cooldownTime;

	// Token: 0x0400008E RID: 142
	[Space]
	private List<LevelMusic.LevelMusicTrack> levelMusicTracksRandom;

	// Token: 0x0400008F RID: 143
	private int trackIndex;

	// Token: 0x020002FC RID: 764
	[Serializable]
	public class LevelMusicTrack
	{
		// Token: 0x040027FB RID: 10235
		public string name;

		// Token: 0x040027FC RID: 10236
		public AudioClip audioClip;

		// Token: 0x040027FD RID: 10237
		[Range(0f, 1f)]
		public float volume = 0.8f;
	}
}
