using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200023B RID: 571
public class DataDirector : MonoBehaviour
{
	// Token: 0x060012A3 RID: 4771 RVA: 0x000A761F File Offset: 0x000A581F
	private void Awake()
	{
		if (DataDirector.instance == null)
		{
			DataDirector.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			this.InitializeSettings();
			this.LoadSettings();
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x000A7658 File Offset: 0x000A5858
	private void Update()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (this.toggleMuteCounterTimer > 0f)
			{
				this.toggleMuteCounterTimer -= Time.deltaTime;
				if (this.toggleMuteCounterTimer <= 0f)
				{
					this.toggleMuteCounter = 0;
				}
			}
			if (this.toggleMuteCooldown > 0f)
			{
				this.toggleMuteCooldown -= Time.deltaTime;
			}
			if (SemiFunc.InputDown(InputKey.ToggleMute))
			{
				this.toggleMute = !this.toggleMute;
				if (this.toggleMuteCooldown <= 0f && !SemiFunc.RunIsLobbyMenu() && PlayerController.instance && PlayerController.instance.playerAvatarScript && !PlayerController.instance.playerAvatarScript.isDisabled)
				{
					this.toggleMuteCounter++;
					this.toggleMuteCounterTimer = 1f;
					if (this.toggleMuteCounter > 5)
					{
						PlayerController.instance.playerAvatarScript.ChatMessageSend("SPAMMER DETECTED! :(");
						this.toggleMuteCooldown = 30f;
						return;
					}
					if (this.toggleMute)
					{
						PlayerController.instance.playerAvatarScript.ChatMessageSend("Muted!");
						return;
					}
					PlayerController.instance.playerAvatarScript.ChatMessageSend("Unmuted!");
					return;
				}
			}
		}
		else
		{
			this.toggleMute = false;
		}
	}

	// Token: 0x060012A5 RID: 4773 RVA: 0x000A77A8 File Offset: 0x000A59A8
	private void InitializeSettings()
	{
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.MasterVolume, "Master Volume", 75);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.MusicVolume, "Music Volume", 75);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.SfxVolume, "Sfx Volume", 75);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.ProximityVoice, "Proximity Voice Volume", 75);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.TextToSpeechVolume, "Text to Speech Volume", 75);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.MicDevice, "Microphone", 1);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.MicVolume, "Microphone Volume", 100);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.PushToTalk, "Push to Talk", 0);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.Resolution, "Resolution", 0);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.Fullscreen, "Fullscreen", 0);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.LightDistance, "Light Distance", 3);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.Vsync, "Vsync", 0);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.Bloom, "Bloom", 1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.ChromaticAberration, "Chromatic Aberration", 1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.Grain, "Grain", 1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.MotionBlur, "Motion Blur", 1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.LensEffect, "Lens Effect", 1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.GlitchLoop, "Glitch Loop", 1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.MaxFPS, "Max FPS", -1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.ShadowQuality, "Shadow Quality", 2);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.ShadowDistance, "Shadow Distance", 3);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.WindowMode, "Window Mode", 0);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.RenderSize, "Pixelation", 2);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.Gamma, "Gamma", 40);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.Tips, "Tips", 1);
		this.aimSensitivityDefault = 35;
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.AimSensitivity, "Aim Sensitivity", this.aimSensitivityDefault);
		this.cameraSmoothingDefault = 80;
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.CameraSmoothing, "Camera Smoothing", this.cameraSmoothingDefault);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.CameraShake, "Camera Shake", 100);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.CameraNoise, "Camera Noise", 100);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.CameraAnimation, "Camera Animation", 4);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.PlayerNames, "Player Names", 1);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.AimInvertVertical, "Aim Invert Vertical", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.RunsPlayed, "Runs Played", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialPlayed, "Tutorial Played", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialJumping, "Tutorial Jumping", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialSprinting, "Tutorial Sprinting", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialSneaking, "Tutorial Sneaking", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialHiding, "Tutorial Hiding", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialTumbling, "Tutorial Tumbling", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialPushingAndPulling, "Tutorial Pushing and Pulling", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialRotating, "Tutorial Rotating", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialReviving, "Tutorial Reviving", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialHealing, "Tutorial Healing", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialCartHandling, "Tutorial Cart Handling", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialItemToggling, "Tutorial Item Toggling", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialInventoryFill, "Tutorial Inventory Fill", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialMap, "Tutorial Map", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialChargingStation, "Tutorial Charging Station", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialOnlyOneExtraction, "Tutorial Only One Extraction", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialChat, "Tutorial Chat", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialFinalExtraction, "Tutorial Final Extraction", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialMultipleExtractions, "Tutorial Multiple Extractions", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialShop, "Tutorial Shop", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialExpressions, "Tutorial Expressions", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialOvercharge1, "Tutorial Overcharge 1", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialOvercharge2, "Tutorial Overcharge 2", 0);
	}

	// Token: 0x060012A6 RID: 4774 RVA: 0x000A7B18 File Offset: 0x000A5D18
	private void SettingAdd(DataDirector.SettingType settingType, DataDirector.Setting setting, string _name, int value)
	{
		if (this.settings.ContainsKey(settingType))
		{
			this.settings[settingType].Add(setting);
		}
		else
		{
			Dictionary<DataDirector.SettingType, List<DataDirector.Setting>> dictionary = this.settings;
			List<DataDirector.Setting> list = new List<DataDirector.Setting>();
			list.Add(setting);
			dictionary[settingType] = list;
		}
		if (this.settingsName.ContainsKey(setting))
		{
			Debug.LogError("Setting already exists: " + setting.ToString() + " " + _name);
			return;
		}
		this.settingsName[setting] = _name;
		this.settingsValue[setting] = value;
		this.defaultSettingsValue[setting] = value;
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x000A7BB9 File Offset: 0x000A5DB9
	public int SettingValueFetch(DataDirector.Setting setting)
	{
		if (!this.settingsValue.ContainsKey(setting))
		{
			return 0;
		}
		return this.settingsValue[setting];
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x000A7BD7 File Offset: 0x000A5DD7
	public float SettingValueFetchFloat(DataDirector.Setting setting)
	{
		return (float)this.settingsValue[setting] / 100f;
	}

	// Token: 0x060012A9 RID: 4777 RVA: 0x000A7BEC File Offset: 0x000A5DEC
	public void SettingValueSet(DataDirector.Setting setting, int value)
	{
		if (this.settingsValue.ContainsKey(setting))
		{
			this.settingsValue[setting] = value;
			return;
		}
		Debug.LogWarning("Setting not found: " + setting.ToString());
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x000A7C26 File Offset: 0x000A5E26
	public string SettingNameGet(DataDirector.Setting setting)
	{
		if (!this.settingsName.ContainsKey(setting))
		{
			return null;
		}
		return this.settingsName[setting];
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x000A7C44 File Offset: 0x000A5E44
	public void SaveSettings()
	{
		SettingsSaveData settingsSaveData = new SettingsSaveData();
		settingsSaveData.settingsValue = new Dictionary<string, int>();
		foreach (KeyValuePair<DataDirector.Setting, int> keyValuePair in this.settingsValue)
		{
			settingsSaveData.settingsValue[keyValuePair.Key.ToString()] = keyValuePair.Value;
		}
		ES3Settings es3Settings = new ES3Settings(new Enum[]
		{
			ES3.Location.Cache
		});
		es3Settings.path = "SettingsData.es3";
		ES3.Save<SettingsSaveData>("Settings", settingsSaveData, es3Settings);
		ES3.Save<string>("PlayerBodyColor", this.playerBodyColor, es3Settings);
		ES3.Save<string>("micDevice", this.micDevice, es3Settings);
		ES3.StoreCachedFile(es3Settings);
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x000A7D20 File Offset: 0x000A5F20
	public void ColorSetBody(int colorID)
	{
		string text = colorID.ToString();
		this.playerBodyColor = text;
		this.SaveSettings();
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x000A7D44 File Offset: 0x000A5F44
	public int ColorGetBody()
	{
		ES3Settings es3Settings = new ES3Settings("SettingsData.es3", new Enum[]
		{
			ES3.Location.File
		});
		if (ES3.KeyExists("PlayerBodyColor", es3Settings))
		{
			this.playerBodyColor = ES3.Load<string>("PlayerBodyColor", es3Settings);
		}
		return int.Parse(this.playerBodyColor);
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x000A7D94 File Offset: 0x000A5F94
	public void LoadSettings()
	{
		try
		{
			ES3Settings es3Settings = new ES3Settings("SettingsData.es3", new Enum[]
			{
				ES3.Location.File
			});
			if (ES3.FileExists(es3Settings))
			{
				if (ES3.KeyExists("Settings", es3Settings))
				{
					using (Dictionary<string, int>.Enumerator enumerator = ES3.Load<SettingsSaveData>("Settings", es3Settings).settingsValue.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, int> keyValuePair = enumerator.Current;
							DataDirector.Setting setting;
							if (Enum.TryParse<DataDirector.Setting>(keyValuePair.Key, ref setting) && this.settingsValue.ContainsKey(setting))
							{
								this.settingsValue[setting] = keyValuePair.Value;
							}
						}
						goto IL_B1;
					}
				}
				Debug.LogWarning("Key 'Settings' not found in file: " + es3Settings.FullPath);
				IL_B1:
				if (ES3.KeyExists("PlayerBodyColor", es3Settings))
				{
					this.playerBodyColor = ES3.Load<string>("PlayerBodyColor", es3Settings);
				}
				if (ES3.KeyExists("micDevice", es3Settings))
				{
					this.micDevice = ES3.Load<string>("micDevice", es3Settings);
				}
			}
			else
			{
				this.SaveSettings();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to load settings: " + ex.Message);
			ES3.DeleteFile("SettingsData.es3");
			this.SaveSettings();
		}
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x000A7EE0 File Offset: 0x000A60E0
	public void ResetSettingToDefault(DataDirector.Setting setting)
	{
		if (this.defaultSettingsValue.ContainsKey(setting))
		{
			this.settingsValue[setting] = this.defaultSettingsValue[setting];
			return;
		}
		Debug.LogWarning("Default value not found for setting: " + setting.ToString());
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x000A7F30 File Offset: 0x000A6130
	public void ResetSettingTypeToDefault(DataDirector.SettingType settingType)
	{
		if (this.settings.ContainsKey(settingType))
		{
			using (List<DataDirector.Setting>.Enumerator enumerator = this.settings[settingType].GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DataDirector.Setting setting = enumerator.Current;
					if (this.defaultSettingsValue.ContainsKey(setting))
					{
						this.settingsValue[setting] = this.defaultSettingsValue[setting];
					}
				}
				return;
			}
		}
		Debug.LogWarning("SettingType not found: " + settingType.ToString());
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x000A7FD4 File Offset: 0x000A61D4
	public void RunsPlayedAdd()
	{
		int value = this.SettingValueFetch(DataDirector.Setting.RunsPlayed) + 1;
		this.SettingValueSet(DataDirector.Setting.RunsPlayed, value);
		this.SaveSettings();
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x000A7FFB File Offset: 0x000A61FB
	public void TutorialPlayed()
	{
		this.SettingValueSet(DataDirector.Setting.TutorialPlayed, 1);
		this.SaveSettings();
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x000A800C File Offset: 0x000A620C
	public void SaveDeleteCheck(bool _leaveGame)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer() || RunManager.instance.levelPrevious == RunManager.instance.levelTutorial || RunManager.instance.levelPrevious == RunManager.instance.levelMainMenu || RunManager.instance.levelPrevious == RunManager.instance.levelRecording)
		{
			return;
		}
		bool flag = false;
		if (SemiFunc.RunIsArena() || GameManager.instance.connectRandom)
		{
			flag = true;
		}
		else if (RunManager.instance.allPlayersDead && RunManager.instance.levelPrevious != RunManager.instance.levelMainMenu && RunManager.instance.levelPrevious != RunManager.instance.levelLobbyMenu && RunManager.instance.levelPrevious != RunManager.instance.levelTutorial && RunManager.instance.levelPrevious != RunManager.instance.levelLobby && RunManager.instance.levelPrevious != RunManager.instance.levelShop && RunManager.instance.levelPrevious != RunManager.instance.levelRecording)
		{
			flag = true;
		}
		else if (_leaveGame && RunManager.instance.levelsCompleted == 0)
		{
			flag = true;
		}
		if (flag)
		{
			SemiFunc.SaveFileDelete(StatsManager.instance.saveFileCurrent);
		}
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x000A8169 File Offset: 0x000A6369
	public void PhotonSetRegion()
	{
		Debug.Log("Photon - Set Region");
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = DataDirector.instance.networkRegion;
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x000A818E File Offset: 0x000A638E
	public void PhotonSetVersion()
	{
		Debug.Log("Photon - Set Version");
		PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = BuildManager.instance.version.title;
	}

	// Token: 0x060012B6 RID: 4790 RVA: 0x000A81B8 File Offset: 0x000A63B8
	public void PhotonSetAppId()
	{
		Debug.Log("Photon - Set AppId");
		PhotonNetwork.ServerPortOverrides = PhotonPortDefinition.AlternativeUdpPorts;
		PhotonNetwork.ObjectsInOneUpdate = 10;
	}

	// Token: 0x04001FC1 RID: 8129
	public static DataDirector instance;

	// Token: 0x04001FC2 RID: 8130
	private string playerBodyColor = "0";

	// Token: 0x04001FC3 RID: 8131
	internal string micDevice = "";

	// Token: 0x04001FC4 RID: 8132
	private Dictionary<DataDirector.Setting, string> settingsName = new Dictionary<DataDirector.Setting, string>();

	// Token: 0x04001FC5 RID: 8133
	private Dictionary<DataDirector.Setting, int> settingsValue = new Dictionary<DataDirector.Setting, int>();

	// Token: 0x04001FC6 RID: 8134
	private Dictionary<DataDirector.Setting, int> defaultSettingsValue = new Dictionary<DataDirector.Setting, int>();

	// Token: 0x04001FC7 RID: 8135
	private Dictionary<DataDirector.SettingType, List<DataDirector.Setting>> settings = new Dictionary<DataDirector.SettingType, List<DataDirector.Setting>>();

	// Token: 0x04001FC8 RID: 8136
	internal string networkPassword;

	// Token: 0x04001FC9 RID: 8137
	internal string networkRegion;

	// Token: 0x04001FCA RID: 8138
	internal string networkServerName;

	// Token: 0x04001FCB RID: 8139
	internal string networkJoinServerName;

	// Token: 0x04001FCC RID: 8140
	public const string SERVER_NAME_PROP_KEY = "server_name";

	// Token: 0x04001FCD RID: 8141
	public TypedLobby customLobby = new TypedLobby("custom", LobbyType.Default);

	// Token: 0x04001FCE RID: 8142
	public TypedLobby privateLobby = new TypedLobby("private", LobbyType.Default);

	// Token: 0x04001FCF RID: 8143
	internal int aimSensitivityDefault;

	// Token: 0x04001FD0 RID: 8144
	internal int cameraSmoothingDefault;

	// Token: 0x04001FD1 RID: 8145
	internal bool toggleMute;

	// Token: 0x04001FD2 RID: 8146
	private float toggleMuteCooldown;

	// Token: 0x04001FD3 RID: 8147
	private int toggleMuteCounter;

	// Token: 0x04001FD4 RID: 8148
	private float toggleMuteCounterTimer;

	// Token: 0x020003F5 RID: 1013
	public enum Setting
	{
		// Token: 0x04002D03 RID: 11523
		MusicVolume,
		// Token: 0x04002D04 RID: 11524
		SfxVolume,
		// Token: 0x04002D05 RID: 11525
		AmbienceVolume,
		// Token: 0x04002D06 RID: 11526
		MicDevice,
		// Token: 0x04002D07 RID: 11527
		ProximityVoice,
		// Token: 0x04002D08 RID: 11528
		Resolution,
		// Token: 0x04002D09 RID: 11529
		Fullscreen,
		// Token: 0x04002D0A RID: 11530
		MicVolume,
		// Token: 0x04002D0B RID: 11531
		TextToSpeechVolume,
		// Token: 0x04002D0C RID: 11532
		CameraShake,
		// Token: 0x04002D0D RID: 11533
		CameraAnimation,
		// Token: 0x04002D0E RID: 11534
		Tips,
		// Token: 0x04002D0F RID: 11535
		Vsync,
		// Token: 0x04002D10 RID: 11536
		MasterVolume,
		// Token: 0x04002D11 RID: 11537
		CameraSmoothing,
		// Token: 0x04002D12 RID: 11538
		LightDistance,
		// Token: 0x04002D13 RID: 11539
		Bloom,
		// Token: 0x04002D14 RID: 11540
		LensEffect,
		// Token: 0x04002D15 RID: 11541
		MotionBlur,
		// Token: 0x04002D16 RID: 11542
		MaxFPS,
		// Token: 0x04002D17 RID: 11543
		ShadowQuality,
		// Token: 0x04002D18 RID: 11544
		ShadowDistance,
		// Token: 0x04002D19 RID: 11545
		ChromaticAberration,
		// Token: 0x04002D1A RID: 11546
		Grain,
		// Token: 0x04002D1B RID: 11547
		WindowMode,
		// Token: 0x04002D1C RID: 11548
		RenderSize,
		// Token: 0x04002D1D RID: 11549
		GlitchLoop,
		// Token: 0x04002D1E RID: 11550
		AimSensitivity,
		// Token: 0x04002D1F RID: 11551
		CameraNoise,
		// Token: 0x04002D20 RID: 11552
		Gamma,
		// Token: 0x04002D21 RID: 11553
		PlayerNames,
		// Token: 0x04002D22 RID: 11554
		RunsPlayed,
		// Token: 0x04002D23 RID: 11555
		PushToTalk,
		// Token: 0x04002D24 RID: 11556
		TutorialPlayed,
		// Token: 0x04002D25 RID: 11557
		TutorialJumping,
		// Token: 0x04002D26 RID: 11558
		TutorialSprinting,
		// Token: 0x04002D27 RID: 11559
		TutorialSneaking,
		// Token: 0x04002D28 RID: 11560
		TutorialHiding,
		// Token: 0x04002D29 RID: 11561
		TutorialTumbling,
		// Token: 0x04002D2A RID: 11562
		TutorialPushingAndPulling,
		// Token: 0x04002D2B RID: 11563
		TutorialRotating,
		// Token: 0x04002D2C RID: 11564
		TutorialReviving,
		// Token: 0x04002D2D RID: 11565
		TutorialHealing,
		// Token: 0x04002D2E RID: 11566
		TutorialCartHandling,
		// Token: 0x04002D2F RID: 11567
		TutorialItemToggling,
		// Token: 0x04002D30 RID: 11568
		TutorialInventoryFill,
		// Token: 0x04002D31 RID: 11569
		TutorialMap,
		// Token: 0x04002D32 RID: 11570
		TutorialChargingStation,
		// Token: 0x04002D33 RID: 11571
		TutorialOnlyOneExtraction,
		// Token: 0x04002D34 RID: 11572
		TutorialChat,
		// Token: 0x04002D35 RID: 11573
		TutorialFinalExtraction,
		// Token: 0x04002D36 RID: 11574
		TutorialMultipleExtractions,
		// Token: 0x04002D37 RID: 11575
		TutorialShop,
		// Token: 0x04002D38 RID: 11576
		TutorialExpressions,
		// Token: 0x04002D39 RID: 11577
		TutorialOvercharge1,
		// Token: 0x04002D3A RID: 11578
		TutorialOvercharge2,
		// Token: 0x04002D3B RID: 11579
		AimInvertVertical
	}

	// Token: 0x020003F6 RID: 1014
	public enum SettingType
	{
		// Token: 0x04002D3D RID: 11581
		Audio,
		// Token: 0x04002D3E RID: 11582
		Gameplay,
		// Token: 0x04002D3F RID: 11583
		Graphics,
		// Token: 0x04002D40 RID: 11584
		None
	}
}
