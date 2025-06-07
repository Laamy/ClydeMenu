using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D9 RID: 473
public class PlayerVoice : MonoBehaviour
{
	// Token: 0x06001034 RID: 4148 RVA: 0x00094E54 File Offset: 0x00093054
	private void Awake()
	{
		PlayerVoice.Instance = this;
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x00094E5C File Offset: 0x0009305C
	private void Start()
	{
		base.StartCoroutine(this.EnemySetup());
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x00094E6B File Offset: 0x0009306B
	private IEnumerator EnemySetup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x00094E74 File Offset: 0x00093074
	public void PlayCrouchHush()
	{
		if (this.CurrentVoiceSource != null && this.CurrentVoiceSource.isPlaying)
		{
			this.VoicesToFade.Add(this.CurrentVoiceSource);
			this.CurrentVoiceSource = null;
		}
		this.SprintingTimer = 0f;
		this.SprintLoop.PlayLoop(false, 1f, 50f, 1f);
		this.CurrentVoiceSource = this.CrouchHush.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.VoicePauseTimer = this.CurrentVoiceSource.clip.length * 1.2f;
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x00094F28 File Offset: 0x00093128
	private void Update()
	{
		if (this.Player.Crouching && this.VoicePauseTimer <= 0f)
		{
			float num = Mathf.Clamp(this.LevelEnemy.PlayerDistance.PlayerDistanceLocal, this.CrouchLoopDistanceMin, this.CrouchLoopDistanceMax);
			float b = Mathf.Lerp(this.CrouchLoopVolumeMin, this.CrouchLoopVolumeMax, 1f - (num - this.CrouchLoopDistanceMin) / (this.CrouchLoopDistanceMax - this.CrouchLoopDistanceMin));
			this.CrouchLoopVolume = Mathf.Lerp(this.CrouchLoopVolume, b, Time.deltaTime * 5f);
			this.CrouchLoop.LoopVolume = this.CrouchLoopVolume;
			this.CrouchLoop.PlayLoop(true, 1f, 1f, 1f);
		}
		else
		{
			this.CrouchLoop.PlayLoop(false, 1f, 1f, 1f);
		}
		if (this.Player.SprintSpeedLerp >= 1f)
		{
			this.SprintingTimer = 0.5f;
		}
		else if (this.SprintingTimer > 0f)
		{
			this.SprintingTimer -= Time.deltaTime;
		}
		if (this.SprintingTimer > 0f)
		{
			this.SprintLoop.PlayLoop(true, 1f, 5f, 1f);
			if (!this.SprintLoopPlaying)
			{
				if (this.CurrentVoiceSource != null && this.CurrentVoiceSource.isPlaying)
				{
					this.VoicesToFade.Add(this.CurrentVoiceSource);
					this.CurrentVoiceSource = null;
				}
				this.SprintLoop.LoopVolume = 0f;
				this.SprintLoopPlaying = true;
			}
			this.SprintLoopLerp += Time.deltaTime * this.SprintVolumeSpeed;
			this.SprintLoopLerp = Mathf.Clamp01(this.SprintLoopLerp);
			this.SprintLoop.LoopVolume = Mathf.Lerp(0f, this.SprintVolume, this.SprintLoopLerp);
		}
		else
		{
			this.SprintLoop.PlayLoop(false, 1f, 5f, 1f);
			if (this.SprintLoopPlaying)
			{
				if (!this.Player.Crouching)
				{
					if (this.CurrentVoiceSource != null && this.CurrentVoiceSource.isPlaying)
					{
						this.VoicesToFade.Add(this.CurrentVoiceSource);
						this.CurrentVoiceSource = null;
					}
					this.CurrentVoiceSource = this.SprintStop.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.VoicePauseTimer = this.CurrentVoiceSource.clip.length * 1.2f;
				}
				this.SprintLoopLerp = 0f;
				this.SprintLoopPlaying = false;
			}
		}
		if (this.VoicePauseTimer > 0f)
		{
			this.VoicePauseTimer -= Time.deltaTime;
		}
		foreach (AudioSource audioSource in this.VoicesToFade)
		{
			if (audioSource == null)
			{
				this.VoicesToFade.Remove(audioSource);
				break;
			}
			if (audioSource.volume <= 0.01f)
			{
				audioSource.Stop();
				this.VoicesToFade.Remove(audioSource);
				Object.Destroy(audioSource.gameObject);
				break;
			}
			audioSource.volume -= 2f * Time.deltaTime;
		}
	}

	// Token: 0x04001B7C RID: 7036
	public static PlayerVoice Instance;

	// Token: 0x04001B7D RID: 7037
	public PlayerController Player;

	// Token: 0x04001B7E RID: 7038
	internal AudioSource CurrentVoiceSource;

	// Token: 0x04001B7F RID: 7039
	private List<AudioSource> VoicesToFade = new List<AudioSource>();

	// Token: 0x04001B80 RID: 7040
	[Space]
	public Sound CrouchLoop;

	// Token: 0x04001B81 RID: 7041
	public float CrouchLoopDistanceMax = 10f;

	// Token: 0x04001B82 RID: 7042
	public float CrouchLoopDistanceMin = 1f;

	// Token: 0x04001B83 RID: 7043
	public float CrouchLoopVolumeMax = 1f;

	// Token: 0x04001B84 RID: 7044
	public float CrouchLoopVolumeMin = 1f;

	// Token: 0x04001B85 RID: 7045
	private float CrouchLoopVolume;

	// Token: 0x04001B86 RID: 7046
	[Space]
	public Sound CrouchHush;

	// Token: 0x04001B87 RID: 7047
	[Space]
	private float VoicePauseTimer;

	// Token: 0x04001B88 RID: 7048
	private Enemy LevelEnemy;

	// Token: 0x04001B89 RID: 7049
	[Space]
	public Sound SprintStop;

	// Token: 0x04001B8A RID: 7050
	public Sound SprintLoop;

	// Token: 0x04001B8B RID: 7051
	public float SprintVolume;

	// Token: 0x04001B8C RID: 7052
	public float SprintVolumeSpeed;

	// Token: 0x04001B8D RID: 7053
	private bool SprintLoopPlaying;

	// Token: 0x04001B8E RID: 7054
	private float SprintingTimer;

	// Token: 0x04001B8F RID: 7055
	private float SprintLoopLerp;
}
