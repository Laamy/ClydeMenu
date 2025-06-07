using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200012C RID: 300
public class RunManager : MonoBehaviour
{
	// Token: 0x06000997 RID: 2455 RVA: 0x00059219 File Offset: 0x00057419
	private void Awake()
	{
		if (!RunManager.instance)
		{
			RunManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		this.levelPrevious = this.levelCurrent;
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x00059254 File Offset: 0x00057454
	private void Update()
	{
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (LevelGenerator.Instance.Generated && !SteamClient.IsValid && !SteamManager.instance.enabled)
		{
			Debug.LogError("Steam not initialized. Quitting game.");
			Application.Quit();
		}
		if (SemiFunc.DebugDev())
		{
			if (Input.GetKeyDown(KeyCode.F3))
			{
				if (SemiFunc.RunIsArena())
				{
					this.ChangeLevel(true, true, RunManager.ChangeLevelType.Normal);
				}
				else
				{
					this.ChangeLevel(true, false, RunManager.ChangeLevelType.Normal);
				}
			}
			if (!this.restarting && ChatManager.instance && !ChatManager.instance.chatActive && !MenuManager.instance.textInputActive && Input.GetKeyDown(KeyCode.Backspace))
			{
				this.ResetProgress();
				this.RestartScene();
				if (this.levelCurrent != this.levelTutorial)
				{
					SemiFunc.OnSceneSwitch(this.gameOver, false);
				}
			}
		}
		if (this.restarting)
		{
			this.RestartScene();
		}
		if (!this.restarting && this.runStarted && GameDirector.instance.PlayerList.Count > 0 && !SemiFunc.RunIsArena() && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			bool flag = true;
			using (List<PlayerAvatar>.Enumerator enumerator = GameDirector.instance.PlayerList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.isDisabled)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.allPlayersDead = true;
				if (SpectateCamera.instance && SpectateCamera.instance.CheckState(SpectateCamera.State.Normal))
				{
					this.ChangeLevel(false, true, RunManager.ChangeLevelType.Normal);
				}
			}
		}
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x000593F4 File Offset: 0x000575F4
	private void OnApplicationQuit()
	{
		DataDirector.instance.SaveDeleteCheck(true);
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x00059404 File Offset: 0x00057604
	public void ChangeLevel(bool _completedLevel, bool _levelFailed, RunManager.ChangeLevelType _changeLevelType = RunManager.ChangeLevelType.Normal)
	{
		if ((!SemiFunc.MenuLevel() && !SemiFunc.IsMasterClientOrSingleplayer()) || this.restarting)
		{
			return;
		}
		this.gameOver = false;
		if (_levelFailed && this.levelCurrent != this.levelLobby && this.levelCurrent != this.levelShop)
		{
			if (this.levelCurrent == this.levelArena)
			{
				this.ResetProgress();
				if (SemiFunc.IsMultiplayer())
				{
					this.levelCurrent = this.levelLobbyMenu;
				}
				else
				{
					this.SetRunLevel();
				}
				this.gameOver = true;
			}
			else
			{
				this.levelCurrent = this.levelArena;
			}
		}
		if (!this.gameOver && this.levelCurrent != this.levelArena)
		{
			if (_changeLevelType == RunManager.ChangeLevelType.RunLevel)
			{
				this.SetRunLevel();
			}
			else if (_changeLevelType == RunManager.ChangeLevelType.LobbyMenu)
			{
				this.levelCurrent = this.levelLobbyMenu;
			}
			else if (_changeLevelType == RunManager.ChangeLevelType.MainMenu)
			{
				this.levelCurrent = this.levelMainMenu;
			}
			else if (_changeLevelType == RunManager.ChangeLevelType.Tutorial)
			{
				this.levelCurrent = this.levelTutorial;
			}
			else if (_changeLevelType == RunManager.ChangeLevelType.Recording)
			{
				this.levelCurrent = this.levelRecording;
			}
			else if (_changeLevelType == RunManager.ChangeLevelType.Shop)
			{
				this.levelCurrent = this.levelShop;
			}
			else if (this.levelCurrent == this.levelMainMenu || this.levelCurrent == this.levelLobbyMenu)
			{
				this.levelCurrent = this.levelLobby;
			}
			else if (_completedLevel && this.levelCurrent != this.levelLobby && this.levelCurrent != this.levelShop)
			{
				this.previousRunLevel = this.levelCurrent;
				this.levelsCompleted++;
				SemiFunc.StatSetRunLevel(this.levelsCompleted);
				SemiFunc.LevelSuccessful();
				this.levelCurrent = this.levelShop;
			}
			else if (this.levelCurrent == this.levelLobby)
			{
				this.SetRunLevel();
			}
			else if (this.levelCurrent == this.levelShop)
			{
				this.levelCurrent = this.levelLobby;
			}
		}
		if (this.debugLevel && this.levelCurrent != this.levelMainMenu && this.levelCurrent != this.levelLobbyMenu)
		{
			this.levelCurrent = this.debugLevel;
		}
		if (GameManager.Multiplayer())
		{
			this.runManagerPUN.photonView.RPC("UpdateLevelRPC", RpcTarget.OthersBuffered, new object[]
			{
				this.levelCurrent.name,
				this.levelsCompleted,
				this.gameOver
			});
		}
		Debug.Log("Changed level to: " + this.levelCurrent.name);
		if (this.levelCurrent == this.levelShop)
		{
			this.saveLevel = 1;
		}
		else
		{
			this.saveLevel = 0;
		}
		SemiFunc.StatSetSaveLevel(this.saveLevel);
		this.RestartScene();
		if (_changeLevelType != RunManager.ChangeLevelType.Tutorial)
		{
			SemiFunc.OnSceneSwitch(this.gameOver, false);
		}
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x000596F8 File Offset: 0x000578F8
	public void RestartScene()
	{
		if (!this.restarting)
		{
			this.restarting = true;
			if (!GameDirector.instance)
			{
				return;
			}
			using (List<PlayerAvatar>.Enumerator enumerator = GameDirector.instance.PlayerList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayerAvatar playerAvatar = enumerator.Current;
					playerAvatar.OutroStart();
				}
				return;
			}
		}
		if (!this.restartingDone)
		{
			bool flag = true;
			if (!GameDirector.instance)
			{
				flag = false;
			}
			else
			{
				using (List<PlayerAvatar>.Enumerator enumerator = GameDirector.instance.PlayerList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.outroDone)
						{
							flag = false;
							break;
						}
					}
				}
			}
			if (flag)
			{
				if (this.gameOver)
				{
					NetworkManager.instance.DestroyAll();
					this.gameOver = false;
				}
				if (this.lobbyJoin)
				{
					this.lobbyJoin = false;
					this.restartingDone = true;
					SceneManager.LoadSceneAsync("LobbyJoin");
					return;
				}
				if (!this.waitToChangeScene)
				{
					this.restartingDone = true;
					if (!GameManager.Multiplayer())
					{
						SceneManager.LoadSceneAsync("Main");
						return;
					}
					if (PhotonNetwork.IsMasterClient)
					{
						PhotonNetwork.LoadLevel("Reload");
					}
				}
			}
		}
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x00059844 File Offset: 0x00057A44
	public void UpdateLevel(string _levelName, int _levelsCompleted, bool _gameOver)
	{
		if (LobbyMenuOpen.instance)
		{
			DataDirector.instance.RunsPlayedAdd();
		}
		this.levelsCompleted = _levelsCompleted;
		SemiFunc.StatSetRunLevel(this.levelsCompleted);
		if (_levelName == this.levelLobbyMenu.name)
		{
			this.levelCurrent = this.levelLobbyMenu;
		}
		else if (_levelName == this.levelLobby.name)
		{
			this.levelCurrent = this.levelLobby;
		}
		else if (_levelName == this.levelShop.name)
		{
			this.levelCurrent = this.levelShop;
		}
		else if (_levelName == this.levelArena.name)
		{
			this.levelCurrent = this.levelArena;
		}
		else if (_levelName == this.levelRecording.name)
		{
			this.levelCurrent = this.levelRecording;
		}
		else
		{
			foreach (Level level in this.levels)
			{
				if (level.name == _levelName)
				{
					this.levelCurrent = level;
					break;
				}
			}
		}
		SemiFunc.OnSceneSwitch(_gameOver, false);
		Debug.Log("updated level to: " + this.levelCurrent.name);
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x000599A0 File Offset: 0x00057BA0
	public void ResetProgress()
	{
		if (StatsManager.instance)
		{
			StatsManager.instance.ResetAllStats();
		}
		this.levelsCompleted = 0;
		this.loadLevel = 0;
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x000599C8 File Offset: 0x00057BC8
	public void EnemiesSpawnedRemoveStart()
	{
		this.enemiesSpawnedToDelete.Clear();
		foreach (EnemySetup enemySetup in this.enemiesSpawned)
		{
			bool flag = false;
			foreach (EnemySetup y in this.enemiesSpawnedToDelete)
			{
				if (enemySetup == y)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.enemiesSpawnedToDelete.Add(enemySetup);
			}
		}
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x00059A7C File Offset: 0x00057C7C
	public void EnemiesSpawnedRemoveEnd()
	{
		foreach (EnemySetup enemySetup in this.enemiesSpawnedToDelete)
		{
			this.enemiesSpawned.Remove(enemySetup);
		}
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x00059AD8 File Offset: 0x00057CD8
	public void SetRunLevel()
	{
		if (this.testerMode)
		{
			this.levelCurrent = this.levels[this.levels.Count - 1];
			return;
		}
		this.levelCurrent = this.previousRunLevel;
		while (this.levelCurrent == this.previousRunLevel)
		{
			this.levelCurrent = this.levels[Random.Range(0, this.levels.Count)];
		}
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x00059B4F File Offset: 0x00057D4F
	public IEnumerator LeaveToMainMenu()
	{
		while (PhotonNetwork.NetworkingClient.State != ClientState.Disconnected && PhotonNetwork.NetworkingClient.State != ClientState.PeerCreated)
		{
			yield return null;
		}
		Debug.Log("Leave to Main Menu");
		SemiFunc.OnSceneSwitch(false, true);
		this.levelCurrent = this.levelMainMenu;
		SceneManager.LoadSceneAsync("Reload");
		yield return null;
		yield break;
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x00059B60 File Offset: 0x00057D60
	public void TesterToggle()
	{
		this.testerMode = !this.testerMode;
		SemiLogger.LogAxel("Tester Mode: " + this.testerMode.ToString(), null, default(Color?));
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x00059BA0 File Offset: 0x00057DA0
	public string MoonGetName(int _moonLevel)
	{
		if (this.moonLevels.Count > _moonLevel)
		{
			return this.moonLevels[_moonLevel].name;
		}
		return "";
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x00059BC7 File Offset: 0x00057DC7
	public List<string> MoonGetAttributes(int _moonLevel)
	{
		if (this.moonLevels.Count > _moonLevel)
		{
			return this.moonLevels[_moonLevel].moonAttributes;
		}
		return new List<string>();
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x00059BEE File Offset: 0x00057DEE
	public Sprite MoonGetIcon(int _moonLevel)
	{
		if (this.moonLevels.Count > _moonLevel)
		{
			return this.moonLevels[_moonLevel].moonIcon;
		}
		return null;
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x00059C14 File Offset: 0x00057E14
	public void UpdateMoonLevel()
	{
		this.moonLevelPrev = this.moonLevel;
		this.moonLevel = Mathf.Max(0, Mathf.CeilToInt((float)(this.levelsCompleted - 5) / 5f));
		if (this.moonLevel != this.moonLevelPrev && this.moonLevels.Count > this.moonLevel)
		{
			this.moonLevelChanged = true;
		}
	}

	// Token: 0x0400110D RID: 4365
	public static RunManager instance;

	// Token: 0x0400110E RID: 4366
	internal int saveLevel;

	// Token: 0x0400110F RID: 4367
	internal int loadLevel;

	// Token: 0x04001110 RID: 4368
	internal Level debugLevel;

	// Token: 0x04001111 RID: 4369
	internal bool skipMainMenu;

	// Token: 0x04001112 RID: 4370
	internal bool localMultiplayerTest;

	// Token: 0x04001113 RID: 4371
	internal bool runStarted;

	// Token: 0x04001114 RID: 4372
	internal RunManagerPUN runManagerPUN;

	// Token: 0x04001115 RID: 4373
	public int levelsCompleted;

	// Token: 0x04001116 RID: 4374
	public Level levelCurrent;

	// Token: 0x04001117 RID: 4375
	internal Level levelPrevious;

	// Token: 0x04001118 RID: 4376
	private Level previousRunLevel;

	// Token: 0x04001119 RID: 4377
	internal bool restarting;

	// Token: 0x0400111A RID: 4378
	internal bool restartingDone;

	// Token: 0x0400111B RID: 4379
	internal int levelsMax = 10;

	// Token: 0x0400111C RID: 4380
	[Space]
	public Level levelMainMenu;

	// Token: 0x0400111D RID: 4381
	public Level levelLobbyMenu;

	// Token: 0x0400111E RID: 4382
	public Level levelLobby;

	// Token: 0x0400111F RID: 4383
	public Level levelShop;

	// Token: 0x04001120 RID: 4384
	public Level levelTutorial;

	// Token: 0x04001121 RID: 4385
	public Level levelRecording;

	// Token: 0x04001122 RID: 4386
	public Level levelArena;

	// Token: 0x04001123 RID: 4387
	[Space]
	public List<Level> levels;

	// Token: 0x04001124 RID: 4388
	internal int runLives = 3;

	// Token: 0x04001125 RID: 4389
	internal bool levelFailed;

	// Token: 0x04001126 RID: 4390
	internal bool waitToChangeScene;

	// Token: 0x04001127 RID: 4391
	internal bool lobbyJoin;

	// Token: 0x04001128 RID: 4392
	internal bool masterSwitched;

	// Token: 0x04001129 RID: 4393
	internal bool gameOver;

	// Token: 0x0400112A RID: 4394
	internal bool allPlayersDead;

	// Token: 0x0400112B RID: 4395
	[Space]
	public List<EnemySetup> enemiesSpawned;

	// Token: 0x0400112C RID: 4396
	private List<EnemySetup> enemiesSpawnedToDelete = new List<EnemySetup>();

	// Token: 0x0400112D RID: 4397
	internal bool skipLoadingUI = true;

	// Token: 0x0400112E RID: 4398
	internal Color loadingFadeColor = Color.black;

	// Token: 0x0400112F RID: 4399
	internal float loadingAnimationTime;

	// Token: 0x04001130 RID: 4400
	internal List<PlayerVoiceChat> voiceChats = new List<PlayerVoiceChat>();

	// Token: 0x04001131 RID: 4401
	internal bool testerMode;

	// Token: 0x04001132 RID: 4402
	internal bool levelIsShop;

	// Token: 0x04001133 RID: 4403
	private int moonLevelPrev;

	// Token: 0x04001134 RID: 4404
	internal int moonLevel;

	// Token: 0x04001135 RID: 4405
	internal bool moonLevelChanged;

	// Token: 0x04001136 RID: 4406
	public List<Moon> moonLevels = new List<Moon>();

	// Token: 0x02000371 RID: 881
	public enum ChangeLevelType
	{
		// Token: 0x04002ACD RID: 10957
		Normal,
		// Token: 0x04002ACE RID: 10958
		RunLevel,
		// Token: 0x04002ACF RID: 10959
		Tutorial,
		// Token: 0x04002AD0 RID: 10960
		LobbyMenu,
		// Token: 0x04002AD1 RID: 10961
		MainMenu,
		// Token: 0x04002AD2 RID: 10962
		Shop,
		// Token: 0x04002AD3 RID: 10963
		Recording
	}

	// Token: 0x02000372 RID: 882
	public enum SaveLevel
	{
		// Token: 0x04002AD5 RID: 10965
		Lobby,
		// Token: 0x04002AD6 RID: 10966
		Shop
	}
}
