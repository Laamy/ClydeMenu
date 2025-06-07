using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

// Token: 0x020001DA RID: 474
[Serializable]
public class PlayerVoiceChat : MonoBehaviour
{
	// Token: 0x0600103A RID: 4154 RVA: 0x000952D8 File Offset: 0x000934D8
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		foreach (PlayerVoiceChat playerVoiceChat in RunManager.instance.voiceChats)
		{
			PhotonView component = playerVoiceChat.GetComponent<PhotonView>();
			if (this.photonView.Owner == component.Owner)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}
		RunManager.instance.voiceChats.Add(this);
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x0009536C File Offset: 0x0009356C
	private void Start()
	{
		this.clipSampleData = new float[this.sampleDataLength];
		this.audioSource = base.GetComponent<AudioSource>();
		this.recorder = base.GetComponent<Recorder>();
		this.speaker = base.GetComponent<Speaker>();
		if (this.photonView.IsMine)
		{
			if (PlayerVoiceChat.instance)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			PlayerVoiceChat.instance = this;
			this.audioSource.volume = 0f;
			this.voiceGain = 0f;
		}
		Object.DontDestroyOnLoad(base.gameObject);
		this.ToggleLobby(true);
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x00095406 File Offset: 0x00093606
	public void OverrideClipLoudnessAnimationValue(float _value)
	{
		this.overrideAddToClipLoudness = _value;
		this.overrideAddToClipLoudnessTimer = 0.1f;
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x0009541A File Offset: 0x0009361A
	private void OverrideClipLoudnessAnimationValueTick()
	{
		if (this.overrideAddToClipLoudnessTimer > 0f)
		{
			this.overrideAddToClipLoudnessTimer -= Time.deltaTime;
			return;
		}
		this.overrideAddToClipLoudness = 0f;
	}

	// Token: 0x0600103E RID: 4158 RVA: 0x00095447 File Offset: 0x00093647
	private void FixedUpdate()
	{
		this.OverridePitchTick();
		this.OverrideClipLoudnessAnimationValueTick();
	}

	// Token: 0x0600103F RID: 4159 RVA: 0x00095458 File Offset: 0x00093658
	private void Update()
	{
		this.OverridePitchLogic();
		if (this.photonView.IsMine)
		{
			this.microphoneVolumeSetting = DataDirector.instance.SettingValueFetch(DataDirector.Setting.MicVolume);
			if (this.microphoneVolumeSetting != this.microphoneVolumeSettingPrevious)
			{
				this.microphoneVolumeSettingPrevious = this.microphoneVolumeSetting;
				this.photonView.RPC("MicrophoneVolumeSettingRPC", RpcTarget.OthersBuffered, new object[]
				{
					this.microphoneVolumeSetting
				});
			}
		}
		this.microphoneVolumeMultiplier = (float)this.microphoneVolumeSetting * 0.01f;
		if (!this.TTSinstantiated && this.playerAvatar)
		{
			if (this.TTSinstantiatedTimer > 3f && (PunVoiceClient.Instance.Client.State == ClientState.Joined || PunVoiceClient.Instance.Client.State == ClientState.Disconnected))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.TTSprefab, base.transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				this.ttsVoice = gameObject.GetComponent<TTSVoice>();
				this.ttsAudioSource = this.ttsVoice.GetComponent<AudioSource>();
				this.lowPassLogicTTS = this.ttsAudioSource.GetComponent<AudioLowPassLogic>();
				this.lowPassLogicTTS.Fetch = true;
				this.ttsVoice.playerAvatar = this.playerAvatar;
				if (this.playerAvatar.isLocal)
				{
					this.recorder.RecordingEnabled = true;
					this.photonView.RPC("RecordingEnabledRPC", RpcTarget.AllBuffered, Array.Empty<object>());
				}
				this.TTSinstantiated = true;
			}
			else
			{
				this.TTSinstantiatedTimer += Time.deltaTime;
			}
		}
		if (this.TTSinstantiated && this.playerAvatar.isLocal)
		{
			this.microphoneEnabledPrevious = this.microphoneEnabled;
			this.microphoneEnabled = false;
			if (this.currentDeviceName != "NONE")
			{
				string[] devices = Microphone.devices;
				for (int i = 0; i < devices.Length; i++)
				{
					if (devices[i] == this.currentDeviceName)
					{
						this.microphoneEnabled = true;
						break;
					}
				}
			}
			if (this.currentDeviceName == "" || this.currentDeviceName != SessionManager.instance.micDeviceCurrent || this.microphoneEnabled != this.microphoneEnabledPrevious)
			{
				this.currentDeviceName = SessionManager.instance.micDeviceCurrent;
				if (!this.microphoneEnabled && this.currentDeviceName != "")
				{
					this.recorder.MicrophoneDevice = new DeviceInfo("", null);
				}
				else if (this.currentDeviceName != "NONE")
				{
					this.recorder.MicrophoneDevice = new DeviceInfo(this.currentDeviceName, null);
				}
			}
		}
		if (this.clipCheckTimer <= 0f)
		{
			this.clipCheckTimer = 0.001f;
			this.clipLoudness = 0f;
			this.clipLoudnessTTS = 0f;
			this.clipLoudnessNoTTS = 0f;
			if (this.audioSource && this.audioSource.clip && this.audioSource.isPlaying)
			{
				this.audioSource.clip.GetData(this.clipSampleData, this.audioSource.timeSamples);
				foreach (float f in this.clipSampleData)
				{
					this.clipLoudness += Mathf.Abs(f);
				}
				this.clipLoudness /= (float)this.sampleDataLength;
				this.clipLoudnessNoTTS = this.clipLoudness;
			}
			this.clipLoudness *= this.microphoneVolumeMultiplier;
			this.clipLoudnessNoTTS *= this.microphoneVolumeMultiplier;
			this.clipLoudness += this.overrideAddToClipLoudness;
			if (this.ttsVoice && this.ttsAudioSource.isPlaying)
			{
				this.ttsAudioSource.GetSpectrumData(this.ttsAudioSpectrum, 0, FFTWindow.BlackmanHarris);
				float num = Mathf.Max(this.ttsAudioSpectrum) * 2f;
				this.clipLoudnessTTS = num;
				if (num > this.clipLoudness)
				{
					this.clipLoudness = num;
				}
			}
			if (this.clipLoudness > 0.05f)
			{
				this.clipLoudnessCrawlingCounter++;
			}
			else
			{
				this.clipLoudnessCrawlingCounter = 0;
			}
		}
		else
		{
			this.clipCheckTimer -= Time.deltaTime;
		}
		if (this.photonView.IsMine)
		{
			if (!this.debug)
			{
				if (this.clipLoudness > 0.005f)
				{
					this.isTalking = true;
					this.isTalkingTimer = 0.5f;
				}
			}
			else if (this.debugTalkingTimer > 0f)
			{
				this.debugTalkingTimer -= Time.deltaTime;
				this.isTalkingTimer = 1f;
				this.isTalking = true;
				if (this.debugTalkingTimer <= 0f)
				{
					this.debugTalkingCooldown = Random.Range(3f, 10f);
				}
			}
			else
			{
				this.debugTalkingCooldown -= Time.deltaTime;
				if (this.debugTalkingCooldown <= 0f)
				{
					this.debugTalkingTimer = Random.Range(1f, 6f);
				}
			}
			if (this.isTalkingTimer > 0f)
			{
				this.isTalkingTimer -= Time.deltaTime;
				if (this.isTalkingTimer <= 0f)
				{
					this.isTalking = false;
				}
			}
			if (this.isTalking != this.isTalkingPrevious)
			{
				this.isTalkingPrevious = this.isTalking;
				if (this.isTalking)
				{
					this.isTalkingStartTime = Time.time;
				}
				this.photonView.RPC("IsTalkingRPC", RpcTarget.Others, new object[]
				{
					this.isTalking
				});
			}
		}
		if (this.debug)
		{
			if (this.isTalking)
			{
				this.lowPassLogic.Volume = Mathf.Lerp(this.lowPassLogic.Volume, 1f, Time.deltaTime * 20f);
				if (this.lowPassLogicTTS)
				{
					this.lowPassLogicTTS.Volume = this.lowPassLogic.Volume;
				}
			}
			else
			{
				this.lowPassLogic.Volume = Mathf.Lerp(this.lowPassLogic.Volume, 0f, Time.deltaTime * 20f);
				if (this.lowPassLogicTTS)
				{
					this.lowPassLogicTTS.Volume = this.lowPassLogic.Volume;
				}
			}
		}
		if (SemiFunc.IsMultiplayer() && SemiFunc.IsMasterClient())
		{
			if (this.playerAvatar && !this.playerAvatar.isDisabled)
			{
				bool flag = false;
				if (this.playerAvatar.isCrawling)
				{
					if (this.clipLoudnessCrawlingCounter > 10)
					{
						flag = true;
					}
				}
				else if (this.playerAvatar.isCrouching)
				{
					if (this.clipLoudness > 0.05f)
					{
						flag = true;
					}
				}
				else if (this.clipLoudness > 0.025f)
				{
					flag = true;
				}
				if (flag && this.investigateTimer <= 0f)
				{
					this.investigateTimer = 1f;
					EnemyDirector.instance.SetInvestigate(this.playerAvatar.PlayerVisionTarget.VisionTransform.transform.position, 5f, false);
				}
			}
			if (this.investigateTimer >= 0f)
			{
				this.investigateTimer -= Time.deltaTime;
			}
		}
		if (this.SpatialDisableTimer > 0f || this.inLobbyMixer || this.photonView.IsMine)
		{
			this.audioSource.spatialBlend = 0f;
			this.SpatialDisableTimer -= Time.deltaTime;
		}
		else
		{
			this.audioSource.spatialBlend = 1f;
		}
		float volume = 0f;
		if (!this.photonView.IsMine)
		{
			volume = this.voiceGain * this.microphoneVolumeMultiplier;
		}
		this.lowPassLogic.Volume = volume;
		if (this.lowPassLogicTTS && this.playerAvatar)
		{
			if (this.playerAvatar.isCrouching)
			{
				this.lowPassLogicTTS.Volume = 0.8f;
			}
			else
			{
				this.lowPassLogicTTS.Volume = 1f;
			}
		}
		if (this.TTSinstantiated && this.playerAvatar.isLocal)
		{
			bool transmitEnabled = true;
			if (!this.microphoneEnabled || DataDirector.instance.toggleMute || (AudioManager.instance.pushToTalk && !SemiFunc.InputHold(InputKey.PushToTalk)))
			{
				transmitEnabled = false;
			}
			if (this.toggleMute != DataDirector.instance.toggleMute)
			{
				this.toggleMute = DataDirector.instance.toggleMute;
				this.photonView.RPC("ToggleMuteRPC", RpcTarget.OthersBuffered, new object[]
				{
					this.toggleMute
				});
			}
			this.recorder.TransmitEnabled = transmitEnabled;
		}
		bool flag2 = false;
		if (this.overrideMuteTimer > 0f)
		{
			flag2 = true;
			this.overrideMuteTimer -= Time.deltaTime;
		}
		if (this.TTSinstantiated && (flag2 || (this.playerAvatar.isLocal && !this.recorder.TransmitEnabled)))
		{
			this.clipLoudnessNoTTS = 0f;
			if (!flag2)
			{
				this.clipLoudness = this.clipLoudnessTTS;
			}
			else
			{
				this.clipLoudness = 0f;
			}
			this.audioSource.volume = 0f;
			this.ttsAudioSource.volume = 0f;
			this.lowPassLogic.Volume = 0f;
			this.isTalking = false;
		}
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x00095DB0 File Offset: 0x00093FB0
	private void LateUpdate()
	{
		this.TtsFollowVoiceSettings();
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x00095DB8 File Offset: 0x00093FB8
	private void OnDestroy()
	{
		RunManager.instance.voiceChats.Remove(this);
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x00095DCC File Offset: 0x00093FCC
	private void TtsFollowVoiceSettings()
	{
		if (!this.ttsVoice)
		{
			return;
		}
		if (!this.playerAvatar)
		{
			return;
		}
		if (this.playerAvatar.isCrouching || this.playerAvatar.isCrawling)
		{
			this.ttsVoice.setVoice(1);
		}
		else
		{
			this.ttsVoice.setVoice(0);
		}
		if (SemiFunc.IsMultiplayer())
		{
			Vector3 forward = this.playerAvatar.PlayerVisionTarget.VisionTransform.transform.forward;
			float num = Mathf.Lerp(0.7f, 1.3f, (forward.y + 1f) / 1.5f) + this.TTSPitchChange;
			num *= this.pitchMultiplier;
			if (this.playerAvatar.isDisabled)
			{
				num = 1f;
			}
			this.ttsAudioSource.pitch = this.audioSource.pitch * num;
			this.ttsAudioSource.spatialBlend = this.audioSource.spatialBlend;
		}
		if (this.inLobbyMixer != this.inLobbyMixerTTS)
		{
			this.inLobbyMixerTTS = this.inLobbyMixer;
			if (this.inLobbyMixer)
			{
				this.ttsAudioSource.outputAudioMixerGroup = this.mixerTTSSpectate;
				return;
			}
			this.ttsAudioSource.outputAudioMixerGroup = this.mixerTTSSound;
		}
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x00095F08 File Offset: 0x00094108
	public void SetDebug()
	{
		this.debug = true;
		this.audioSource.Stop();
		this.audioSource.clip = this.debugClip;
		this.audioSource.time = Random.Range(0f, this.debugClip.length);
		this.audioSource.loop = true;
		if (this.photonView.IsMine)
		{
			this.audioSource.pitch = 0.8f * this.pitchMultiplier;
			this.audioSource.volume = 0.3f;
			this.lowPassLogic.Volume = 0.3f;
		}
		else
		{
			this.audioSource.pitch = 1.25f * this.pitchMultiplier;
		}
		this.audioSource.Play();
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x00095FCC File Offset: 0x000941CC
	public void ToggleLobby(bool _toggle)
	{
		if (_toggle)
		{
			this.inLobbyMixer = true;
			if (this.photonView.IsMine)
			{
				this.volumeOn.TransitionTo(0.1f);
			}
			base.transform.position = new Vector3(1000f, 1000f, 1000f);
			this.audioSource.outputAudioMixerGroup = this.mixerMicrophoneSpectate;
			return;
		}
		this.inLobbyMixer = false;
		if (this.photonView.IsMine)
		{
			this.volumeOff.TransitionTo(0.1f);
			if (AudioManager.instance)
			{
				AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.On, 0.5f);
			}
		}
		this.audioSource.outputAudioMixerGroup = this.mixerMicrophoneSound;
		if (this.ttsAudioSource)
		{
			this.ttsAudioSource.outputAudioMixerGroup = this.mixerTTSSound;
		}
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x000960A0 File Offset: 0x000942A0
	[PunRPC]
	public void IsTalkingRPC(bool _isTalking, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.isTalking = _isTalking;
		if (this.isTalking)
		{
			this.isTalkingStartTime = Time.time;
		}
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x000960CB File Offset: 0x000942CB
	[PunRPC]
	public void MicrophoneVolumeSettingRPC(int _volume, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		_volume = Mathf.Clamp(_volume, 0, 100);
		this.microphoneVolumeSetting = _volume;
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x000960FA File Offset: 0x000942FA
	[PunRPC]
	public void RecordingEnabledRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.recordingEnabled = true;
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x0009611E File Offset: 0x0009431E
	[PunRPC]
	public void ToggleMuteRPC(bool _toggleMute, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.toggleMute = _toggleMute;
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x00096142 File Offset: 0x00094342
	public void SpatialDisable(float _time)
	{
		if (this.photonView.IsMine)
		{
			return;
		}
		this.SpatialDisableTimer = _time;
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x0009615C File Offset: 0x0009435C
	public void OverridePitch(float _multiplier, float _timeIn, float _timeOut, float _overrideTimer = 0.1f, float _oscillation = 0f, float _oscillationSpeed = 0f)
	{
		float num = this.overridePitchMultiplierTarget;
		this.overridePitchMultiplierTarget = _multiplier;
		this.overridePitchSpeedIn = _timeIn;
		this.overridePitchSpeedOut = _timeOut;
		this.overridePitchTimer = _overrideTimer;
		this.overridePitchTime = _overrideTimer;
		this.overridePitchOscillation = _oscillation;
		this.overridePitchOscillationSpeed = _oscillationSpeed;
		this.overridePitchIsActive = true;
		if (this.overridePitchIsActive && num < 0f && this.overridePitchMultiplierTarget < num)
		{
			this.overridePitchIsActive = false;
		}
		if (this.overridePitchIsActive && num > 0f && this.overridePitchMultiplierTarget > num)
		{
			this.overridePitchIsActive = false;
		}
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x000961EC File Offset: 0x000943EC
	public void OverridePitchCancel()
	{
		this.overridePitchMultiplierTarget = 1f;
		this.overridePitchSpeedIn = 0.1f;
		this.overridePitchSpeedOut = 0.1f;
		this.overridePitchTimer = 0f;
		this.overridePitchTime = 0f;
		this.overridePitchIsActive = false;
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x0009622C File Offset: 0x0009442C
	private void OverridePitchTick()
	{
		if (this.overridePitchTimer <= 0f)
		{
			this.overridePitchIsActive = false;
		}
		if (this.overridePitchTimer > 0f)
		{
			this.overridePitchIsActive = true;
			this.overridePitchTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x0600104D RID: 4173 RVA: 0x00096268 File Offset: 0x00094468
	private void OverridePitchLogic()
	{
		this.audioSource.pitch = this.pitchMultiplier;
		if (!this.overridePitchIsActive && this.overridePitchLerp < 0.05f)
		{
			this.pitchMultiplier = 1f;
			return;
		}
		if (this.overridePitchTimer > 0f)
		{
			this.overridePitchLerp += Time.deltaTime / this.overridePitchSpeedIn;
			if (this.overridePitchLerp > 1f)
			{
				this.overridePitchLerp = 1f;
			}
		}
		else
		{
			this.overridePitchLerp -= Time.deltaTime / this.overridePitchSpeedOut;
			if (this.overridePitchLerp < 0f)
			{
				this.overridePitchLerp = 0f;
			}
		}
		float num = Mathf.Sin(Time.time * this.overridePitchOscillationSpeed) * (this.overridePitchOscillation * this.overridePitchLerp);
		if (this.overridePitchOscillationSpeed == 0f)
		{
			num = 0f;
		}
		this.pitchMultiplier = Mathf.Lerp(1f, this.overridePitchMultiplierTarget, this.overridePitchLerp) + num;
	}

	// Token: 0x0600104E RID: 4174 RVA: 0x00096369 File Offset: 0x00094569
	public void OverrideMute(float _time)
	{
		this.overrideMuteTimer = _time;
	}

	// Token: 0x04001B90 RID: 7056
	public GameObject TTSprefab;

	// Token: 0x04001B91 RID: 7057
	public static PlayerVoiceChat instance;

	// Token: 0x04001B92 RID: 7058
	[FormerlySerializedAs("textToSpeech")]
	public TTSVoice ttsVoice;

	// Token: 0x04001B93 RID: 7059
	public AudioClip debugClip;

	// Token: 0x04001B94 RID: 7060
	internal bool debug;

	// Token: 0x04001B95 RID: 7061
	private float debugTalkingTimer;

	// Token: 0x04001B96 RID: 7062
	private float debugTalkingCooldown;

	// Token: 0x04001B97 RID: 7063
	internal bool inLobbyMixer;

	// Token: 0x04001B98 RID: 7064
	internal bool inLobbyMixerTTS;

	// Token: 0x04001B99 RID: 7065
	internal bool isTalking;

	// Token: 0x04001B9A RID: 7066
	private bool isTalkingPrevious;

	// Token: 0x04001B9B RID: 7067
	private float isTalkingTimer;

	// Token: 0x04001B9C RID: 7068
	internal float isTalkingStartTime;

	// Token: 0x04001B9D RID: 7069
	internal float voiceGain = 0.5f;

	// Token: 0x04001B9E RID: 7070
	internal PhotonView photonView;

	// Token: 0x04001B9F RID: 7071
	internal PlayerAvatar playerAvatar;

	// Token: 0x04001BA0 RID: 7072
	internal AudioSource audioSource;

	// Token: 0x04001BA1 RID: 7073
	private Recorder recorder;

	// Token: 0x04001BA2 RID: 7074
	private Speaker speaker;

	// Token: 0x04001BA3 RID: 7075
	[Space]
	public AudioMixerGroup mixerMicrophoneSound;

	// Token: 0x04001BA4 RID: 7076
	public AudioMixerGroup mixerMicrophoneSpectate;

	// Token: 0x04001BA5 RID: 7077
	public AudioMixerGroup mixerTTSSound;

	// Token: 0x04001BA6 RID: 7078
	public AudioMixerGroup mixerTTSSpectate;

	// Token: 0x04001BA7 RID: 7079
	[Space]
	public AudioMixerSnapshot volumeOff;

	// Token: 0x04001BA8 RID: 7080
	public AudioMixerSnapshot volumeOn;

	// Token: 0x04001BA9 RID: 7081
	[Space]
	public AudioLowPassLogic lowPassLogic;

	// Token: 0x04001BAA RID: 7082
	public AudioLowPassLogic lowPassLogicTTS;

	// Token: 0x04001BAB RID: 7083
	private float SpatialDisableTimer;

	// Token: 0x04001BAC RID: 7084
	private int sampleDataLength = 1024;

	// Token: 0x04001BAD RID: 7085
	internal float clipLoudnessNoTTS;

	// Token: 0x04001BAE RID: 7086
	internal float clipLoudnessTTS;

	// Token: 0x04001BAF RID: 7087
	internal float clipLoudness;

	// Token: 0x04001BB0 RID: 7088
	internal int clipLoudnessCrawlingCounter;

	// Token: 0x04001BB1 RID: 7089
	private float[] clipSampleData;

	// Token: 0x04001BB2 RID: 7090
	private float clipCheckTimer;

	// Token: 0x04001BB3 RID: 7091
	private string currentDeviceName = "";

	// Token: 0x04001BB4 RID: 7092
	private float investigateTimer;

	// Token: 0x04001BB5 RID: 7093
	public AudioSource ttsAudioSource;

	// Token: 0x04001BB6 RID: 7094
	private float[] ttsAudioSpectrum = new float[1024];

	// Token: 0x04001BB7 RID: 7095
	internal bool TTSinstantiated;

	// Token: 0x04001BB8 RID: 7096
	private float TTSinstantiatedTimer;

	// Token: 0x04001BB9 RID: 7097
	private float TTSPitchChangeTimer;

	// Token: 0x04001BBA RID: 7098
	private float TTSPitchChangeTarget;

	// Token: 0x04001BBB RID: 7099
	private float TTSPitchChange;

	// Token: 0x04001BBC RID: 7100
	private float TTSPitchChangeSpeed;

	// Token: 0x04001BBD RID: 7101
	private float switchDeviceTimer;

	// Token: 0x04001BBE RID: 7102
	private int microphoneVolumeSetting = -1;

	// Token: 0x04001BBF RID: 7103
	private int microphoneVolumeSettingPrevious = -1;

	// Token: 0x04001BC0 RID: 7104
	internal float microphoneVolumeMultiplier = 1f;

	// Token: 0x04001BC1 RID: 7105
	private float pitchMultiplier = 1f;

	// Token: 0x04001BC2 RID: 7106
	private float overridePitchTimer;

	// Token: 0x04001BC3 RID: 7107
	private float overridePitchMultiplierTarget = 1f;

	// Token: 0x04001BC4 RID: 7108
	private float overridePitchOscillation;

	// Token: 0x04001BC5 RID: 7109
	private float overridePitchOscillationSpeed;

	// Token: 0x04001BC6 RID: 7110
	private float overridePitchSpeedIn;

	// Token: 0x04001BC7 RID: 7111
	private float overridePitchSpeedOut;

	// Token: 0x04001BC8 RID: 7112
	private float overridePitchLerp;

	// Token: 0x04001BC9 RID: 7113
	private float overridePitchTime;

	// Token: 0x04001BCA RID: 7114
	private bool overridePitchIsActive;

	// Token: 0x04001BCB RID: 7115
	private float currentBoost;

	// Token: 0x04001BCC RID: 7116
	private float overrideAddToClipLoudnessTimer;

	// Token: 0x04001BCD RID: 7117
	private float overrideAddToClipLoudness;

	// Token: 0x04001BCE RID: 7118
	internal bool recordingEnabled;

	// Token: 0x04001BCF RID: 7119
	internal bool microphoneEnabled;

	// Token: 0x04001BD0 RID: 7120
	private bool microphoneEnabledPrevious;

	// Token: 0x04001BD1 RID: 7121
	private float overrideMuteTimer;

	// Token: 0x04001BD2 RID: 7122
	internal bool toggleMute;
}
