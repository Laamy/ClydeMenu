using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001DF RID: 479
public class DebugComputerCheck : MonoBehaviour
{
	// Token: 0x06001066 RID: 4198 RVA: 0x00097964 File Offset: 0x00095B64
	private void Start()
	{
		if (!Application.isEditor)
		{
			this.Active = false;
			base.gameObject.SetActive(false);
			return;
		}
		bool flag = false;
		foreach (string text in this.computerNames)
		{
			if (SystemInfo.deviceName == text)
			{
				DebugComputerCheck.instance = this;
				flag = true;
			}
		}
		if (!flag)
		{
			this.Active = false;
			base.gameObject.SetActive(false);
			return;
		}
		GameManager.instance.serverMode = this.serverMode;
		if (this.DebugDisable)
		{
			this.Active = false;
			return;
		}
		if (this.Mode == DebugComputerCheck.StartMode.SinglePlayer)
		{
			RunManager.instance.skipMainMenu = true;
			if (RunManager.instance.levelCurrent == RunManager.instance.levelMainMenu)
			{
				RunManager.instance.SetRunLevel();
			}
		}
		if (this.Mode == DebugComputerCheck.StartMode.Multiplayer)
		{
			RunManager.instance.localMultiplayerTest = true;
		}
		if (this.PlayerDebug && this.InfiniteEnergy)
		{
			PlayerController.instance.DebugEnergy = true;
		}
		if (this.PlayerDebug && this.NoTumbleMode)
		{
			PlayerController.instance.DebugNoTumble = true;
		}
		if (this.PlayerDebug && this.DisableOvercharge)
		{
			PlayerController.instance.DebugDisableOvercharge = true;
		}
		if (this.LevelDebug && RunManager.instance.levelCurrent != RunManager.instance.levelMainMenu && RunManager.instance.levelCurrent != RunManager.instance.levelLobbyMenu)
		{
			if (this.LevelOverride)
			{
				RunManager.instance.debugLevel = this.LevelOverride;
				RunManager.instance.levelCurrent = this.LevelOverride;
			}
			if (this.LevelsCompleted > 0)
			{
				RunManager.instance.levelsCompleted = this.LevelsCompleted;
			}
			if (this.StartRoomOverride)
			{
				LevelGenerator.Instance.DebugStartRoom = this.StartRoomOverride;
			}
			LevelGenerator.Instance.DebugLevelSize = this.LevelSizeMultiplier;
			if (!this.ModuleOverrideActive)
			{
				this.ModuleOverride = null;
			}
			if (this.ModuleOverride)
			{
				LevelGenerator.Instance.DebugModule = this.ModuleOverride;
				if (this.ModuleType == Module.Type.Normal)
				{
					LevelGenerator.Instance.DebugNormal = true;
				}
				else if (this.ModuleType == Module.Type.Passage)
				{
					LevelGenerator.Instance.DebugPassage = true;
					LevelGenerator.Instance.DebugAmount = 6;
				}
				else if (this.ModuleType == Module.Type.DeadEnd || this.ModuleType == Module.Type.Extraction)
				{
					LevelGenerator.Instance.DebugDeadEnd = true;
					LevelGenerator.Instance.DebugAmount = 1;
				}
			}
			if (this.OnlyOneModule)
			{
				LevelGenerator.Instance.DebugAmount = 1;
			}
		}
		if (this.OtherDebug)
		{
			ValuableDirector.instance.valuableDebug = this.valuableDebug;
		}
		if (this.EnemyDebug)
		{
			if (this.EnemyDisable)
			{
				LevelGenerator.Instance.DebugNoEnemy = true;
			}
			if (this.EnemyNoVision)
			{
				EnemyDirector.instance.debugNoVision = true;
			}
			if (this.EnemyEasyGrab)
			{
				EnemyDirector.instance.debugEasyGrab = true;
			}
			if (this.EnemySpawnClose)
			{
				EnemyDirector.instance.debugSpawnClose = true;
			}
			if (this.EnemyDespawnClose)
			{
				EnemyDirector.instance.debugDespawnClose = true;
			}
			if (this.EnemyInvestigates)
			{
				EnemyDirector.instance.debugInvestigate = true;
			}
			if (this.EnemyShortActionTimer)
			{
				EnemyDirector.instance.debugShortActionTimer = true;
			}
			if (this.EnemyNoSpawnedPause)
			{
				EnemyDirector.instance.debugNoSpawnedPause = true;
			}
			if (this.EnemyNoSpawnIdlePause)
			{
				EnemyDirector.instance.debugNoSpawnIdlePause = true;
			}
			if (this.EnemyNoGrabMaxTime)
			{
				EnemyDirector.instance.debugNoGrabMaxTime = true;
			}
			if (this.EnemyOverride.Length != 0)
			{
				EnemyDirector.instance.debugEnemy = this.EnemyOverride;
				EnemyDirector.instance.debugEnemyEnableTime = this.EnemyEnableTimeOverride;
				EnemyDirector.instance.debugEnemyDisableTime = this.EnemyDisableTimeOverride;
			}
		}
		if (this.OtherDebug && this.LowHaul)
		{
			RoundDirector.instance.debugLowHaul = true;
		}
		if (this.OtherDebug && this.InfiniteBattery)
		{
			RoundDirector.instance.debugInfiniteBattery = true;
		}
		if (this.OtherDebug && this.DisableMusic)
		{
			LevelMusic.instance.gameObject.SetActive(false);
			ConstantMusic.instance.gameObject.SetActive(false);
			MusicEnemyNear.instance.gameObject.SetActive(false);
		}
		if (this.OtherDebug && this.DisableLoadingLevelAnimation)
		{
			LoadingUI.instance.debugDisableLevelAnimation = true;
		}
		base.StartCoroutine(this.StatsUpdate());
		if (this.PlayerDebug && this.DebugVoice)
		{
			base.StartCoroutine(this.VoiceUpdate());
		}
		if (this.OtherDebug && this.SimulateLag)
		{
			base.StartCoroutine(this.SimulateLagUpdate());
		}
		base.StartCoroutine(this.AfterLevelGen());
		if (this.PlayerDebug && this.DebugMapActive)
		{
			Map.Instance.debugActive = true;
		}
		if (this.PlayerDebug && (this.StickyGrabber || this.PowerGrabber))
		{
			base.StartCoroutine(this.PhysGrabber());
		}
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x00097E27 File Offset: 0x00096027
	private IEnumerator SimulateLagUpdate()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		PunManager.instance.lagSimulationGui.enabled = true;
		yield break;
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x00097E2F File Offset: 0x0009602F
	private IEnumerator AfterLevelGen()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (this.EmptyBatteries)
		{
			StatsManager.instance.EmptyAllBatteries();
		}
		yield break;
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x00097E3E File Offset: 0x0009603E
	private IEnumerator StatsUpdate()
	{
		if (!this.StatsDebug)
		{
			yield break;
		}
		while (!StatsManager.instance || !StatsManager.instance.statsSynced || !PunManager.instance.statsManager)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (this.Currency > 0)
		{
			SemiFunc.StatSetRunCurrency(this.Currency);
		}
		if (this.BuyAllItems)
		{
			StatsManager.instance.BuyAllItems();
		}
		if (this.StartCrystals != 1)
		{
			StatsManager.instance.itemsPurchased["Item Power Crystal"] = this.StartCrystals;
		}
		yield break;
	}

	// Token: 0x0600106A RID: 4202 RVA: 0x00097E4D File Offset: 0x0009604D
	private IEnumerator VoiceUpdate()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		foreach (PlayerAvatar _player in GameDirector.instance.PlayerList)
		{
			while (!_player.voiceChat)
			{
				yield return new WaitForSeconds(0.1f);
			}
			_player.voiceChat.SetDebug();
			_player = null;
		}
		List<PlayerAvatar>.Enumerator enumerator = default(List<PlayerAvatar>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x00097E55 File Offset: 0x00096055
	private IEnumerator PhysGrabber()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (this.StickyGrabber)
		{
			PlayerController.instance.playerAvatarScript.physGrabber.debugStickyGrabber = true;
		}
		if (this.PowerGrabber)
		{
			PlayerController.instance.playerAvatarScript.physGrabber.grabStrength = 5f;
		}
		yield break;
	}

	// Token: 0x04001C01 RID: 7169
	public static DebugComputerCheck instance;

	// Token: 0x04001C02 RID: 7170
	internal bool Active = true;

	// Token: 0x04001C03 RID: 7171
	public bool DebugDisable;

	// Token: 0x04001C04 RID: 7172
	public SemiFunc.User DebugUser;

	// Token: 0x04001C05 RID: 7173
	public string[] computerNames;

	// Token: 0x04001C06 RID: 7174
	public DebugComputerCheck.StartMode Mode;

	// Token: 0x04001C07 RID: 7175
	public DebugComputerCheck.ServerMode serverMode;

	// Token: 0x04001C08 RID: 7176
	public bool LevelDebug = true;

	// Token: 0x04001C09 RID: 7177
	public Level LevelOverride;

	// Token: 0x04001C0A RID: 7178
	public GameObject StartRoomOverride;

	// Token: 0x04001C0B RID: 7179
	public bool ModuleOverrideActive = true;

	// Token: 0x04001C0C RID: 7180
	public GameObject ModuleOverride;

	// Token: 0x04001C0D RID: 7181
	public Module.Type ModuleType;

	// Token: 0x04001C0E RID: 7182
	public bool OnlyOneModule;

	// Token: 0x04001C0F RID: 7183
	public float LevelSizeMultiplier = 1f;

	// Token: 0x04001C10 RID: 7184
	public int LevelsCompleted;

	// Token: 0x04001C11 RID: 7185
	public bool EnemyDebug = true;

	// Token: 0x04001C12 RID: 7186
	public EnemySetup[] EnemyOverride;

	// Token: 0x04001C13 RID: 7187
	public float EnemyEnableTimeOverride;

	// Token: 0x04001C14 RID: 7188
	public float EnemyDisableTimeOverride;

	// Token: 0x04001C15 RID: 7189
	public bool EnemyDisable;

	// Token: 0x04001C16 RID: 7190
	public bool EnemyNoVision;

	// Token: 0x04001C17 RID: 7191
	public bool EnemySpawnClose;

	// Token: 0x04001C18 RID: 7192
	public bool EnemyDespawnClose;

	// Token: 0x04001C19 RID: 7193
	public bool EnemyInvestigates;

	// Token: 0x04001C1A RID: 7194
	public bool EnemyShortActionTimer;

	// Token: 0x04001C1B RID: 7195
	public bool EnemyNoSpawnedPause;

	// Token: 0x04001C1C RID: 7196
	public bool EnemyNoSpawnIdlePause;

	// Token: 0x04001C1D RID: 7197
	public bool EnemyNoGrabMaxTime;

	// Token: 0x04001C1E RID: 7198
	public bool EnemyEasyGrab;

	// Token: 0x04001C1F RID: 7199
	public bool PlayerDebug;

	// Token: 0x04001C20 RID: 7200
	public bool InfiniteEnergy;

	// Token: 0x04001C21 RID: 7201
	public bool GodMode;

	// Token: 0x04001C22 RID: 7202
	public bool NoTumbleMode;

	// Token: 0x04001C23 RID: 7203
	public bool DisableOvercharge;

	// Token: 0x04001C24 RID: 7204
	public bool DebugVoice;

	// Token: 0x04001C25 RID: 7205
	public bool DebugMapActive;

	// Token: 0x04001C26 RID: 7206
	public bool PowerGrabber;

	// Token: 0x04001C27 RID: 7207
	public bool StickyGrabber;

	// Token: 0x04001C28 RID: 7208
	public bool OtherDebug;

	// Token: 0x04001C29 RID: 7209
	public ValuableDirector.ValuableDebug valuableDebug;

	// Token: 0x04001C2A RID: 7210
	public bool DisableMusic;

	// Token: 0x04001C2B RID: 7211
	public bool DisableLoadingLevelAnimation;

	// Token: 0x04001C2C RID: 7212
	public bool LowHaul;

	// Token: 0x04001C2D RID: 7213
	public bool InfiniteBattery;

	// Token: 0x04001C2E RID: 7214
	public bool SimulateLag;

	// Token: 0x04001C2F RID: 7215
	public bool StatsDebug;

	// Token: 0x04001C30 RID: 7216
	public int Currency;

	// Token: 0x04001C31 RID: 7217
	public bool EmptyBatteries;

	// Token: 0x04001C32 RID: 7218
	public int StartCrystals = 1;

	// Token: 0x04001C33 RID: 7219
	public bool BuyAllItems;

	// Token: 0x020003C6 RID: 966
	public enum StartMode
	{
		// Token: 0x04002C51 RID: 11345
		Normal,
		// Token: 0x04002C52 RID: 11346
		SinglePlayer,
		// Token: 0x04002C53 RID: 11347
		Multiplayer
	}

	// Token: 0x020003C7 RID: 967
	public enum ServerMode
	{
		// Token: 0x04002C55 RID: 11349
		Normal,
		// Token: 0x04002C56 RID: 11350
		Local,
		// Token: 0x04002C57 RID: 11351
		Dev
	}
}
