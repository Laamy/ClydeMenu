using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200023E RID: 574
public class GameDirector : MonoBehaviour
{
	// Token: 0x060012BB RID: 4795 RVA: 0x000A82D6 File Offset: 0x000A64D6
	private void Awake()
	{
		this.MainCamera = Camera.main;
		this.MainCameraParent = this.MainCamera.transform.parent;
		GameDirector.instance = this;
		this.currentState = GameDirector.gameState.Load;
	}

	// Token: 0x060012BC RID: 4796 RVA: 0x000A8306 File Offset: 0x000A6506
	private void Start()
	{
		RunManager.instance.runStarted = true;
		RunManager.instance.allPlayersDead = false;
	}

	// Token: 0x060012BD RID: 4797 RVA: 0x000A8320 File Offset: 0x000A6520
	private void gameStateLoad()
	{
		if (this.gameStateStartImpulse)
		{
			this.cameraPosition.transform.localRotation = Quaternion.Euler(60f, 0f, 0f);
			AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.Off, 0f);
			this.gameStateStartImpulse = false;
		}
	}

	// Token: 0x060012BE RID: 4798 RVA: 0x000A8370 File Offset: 0x000A6570
	private void gameStateStart()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.gameStateStartImpulse)
		{
			AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.CutsceneOnly, 0.1f);
			this.gameStateTimer = 0.5f;
			LoadingUI.instance.LevelAnimationStart();
			this.gameStateStartImpulse = false;
			return;
		}
		if (SemiFunc.RunIsLevel() || SemiFunc.RunIsTutorial() || SemiFunc.RunIsShop() || SemiFunc.RunIsArena())
		{
			if (LoadingUI.instance.levelAnimationCompleted)
			{
				this.gameStateTimer -= Time.deltaTime;
			}
		}
		else
		{
			this.gameStateTimer -= Time.deltaTime;
		}
		if (this.gameStateTimer <= 0f)
		{
			LoadingUI.instance.StopLoading();
			if (RunManager.instance.levelCurrent == RunManager.instance.levelLobbyMenu)
			{
				AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.Spectate, 0.1f);
			}
			else
			{
				AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.On, 0.1f);
			}
			MusicManager.Instance.MusicMixerOff.TransitionTo(0f);
			MusicManager.Instance.MusicMixerOn.TransitionTo(0.1f);
			this.SoundIntro.Play(base.transform.position, 1f, 1f, 1f, 1f);
			if (!SemiFunc.MenuLevel())
			{
				this.SoundIntroRun.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.currentState = GameDirector.gameState.Main;
			this.gameStateStartImpulse = true;
			this.gameStateTimer = 0f;
		}
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x000A8502 File Offset: 0x000A6702
	private void gameStateMain()
	{
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x000A8504 File Offset: 0x000A6704
	private void gameStateOutro()
	{
		if (this.gameStateStartImpulse)
		{
			AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.CutsceneOnly, 0.25f);
			MusicManager.Instance.MusicMixerScareOnly.TransitionTo(0.25f);
			this.SoundOutro.Play(base.transform.position, 1f, 1f, 1f, 1f);
			if (!SemiFunc.MenuLevel())
			{
				this.SoundOutroRun.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.gameStateTimer = 1f;
			this.gameStateStartImpulse = false;
			HUD.instance.Hide();
			using (List<PlayerAvatar>.Enumerator enumerator = this.PlayerList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayerAvatar playerAvatar = enumerator.Current;
					if (playerAvatar.voiceChat)
					{
						playerAvatar.voiceChat.ToggleLobby(true);
					}
				}
				return;
			}
		}
		this.gameStateTimer -= Time.deltaTime;
		if (this.gameStateTimer <= 0f)
		{
			this.currentState = GameDirector.gameState.End;
			this.gameStateStartImpulse = true;
			this.gameStateTimer = 0f;
		}
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x000A864C File Offset: 0x000A684C
	private void gameStateEnd()
	{
		if (this.gameStateStartImpulse)
		{
			AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.Off, 0.5f);
			PlayerController.instance.playerAvatarScript.SetDisabled();
			LoadingUI.instance.StartLoading();
			this.gameStateTimer = 0.5f;
			this.gameStateStartImpulse = false;
			return;
		}
		this.gameStateTimer -= Time.deltaTime;
		if (this.gameStateTimer <= 0f)
		{
			PlayerController.instance.playerAvatarScript.OutroDone();
			this.currentState = GameDirector.gameState.EndWait;
		}
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x000A86D4 File Offset: 0x000A68D4
	private void gameStateDeath()
	{
		SemiFunc.UIShowSpectate();
		SemiFunc.UIHideHealth();
		SemiFunc.UIHideOvercharge();
		SemiFunc.UIHideEnergy();
		SemiFunc.UIHideInventory();
		SemiFunc.UIHideAim();
		if (this.gameStateStartImpulse)
		{
			this.gameStateTimer = 0.5f;
			this.deathFreezeTimer = this.deathFreezeTime;
			this.SoundDeath.Play(base.transform.position, 1f, 1f, 1f, 1f);
			HUD.instance.Hide();
			RenderTextureMain.instance.ChangeResolution(RenderTextureMain.instance.textureWidthOriginal * 0.2f, RenderTextureMain.instance.textureHeightOriginal * 0.2f, this.gameStateTimer);
			this.gameStateStartImpulse = false;
			return;
		}
		if (this.deathFreezeTimer > 0f)
		{
			this.deathFreezeTimer -= Time.deltaTime;
			if (this.deathFreezeTimer <= 0f)
			{
				AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.CutsceneOnly, 0.1f);
				RenderTextureMain.instance.Shake(this.gameStateTimer);
				RenderTextureMain.instance.ChangeSize(1.25f, 1.25f, this.gameStateTimer);
				CameraFreeze.Freeze(this.gameStateTimer + 0.1f);
			}
		}
		this.gameStateTimer -= Time.deltaTime;
		if (this.gameStateTimer <= 0f)
		{
			AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.Spectate, 0.1f);
			HUD.instance.Show();
			RenderTextureMain.instance.Shake(0f);
			RenderTextureMain.instance.sizeResetTimer = 0f;
			RenderTextureMain.instance.textureResetTimer = 0f;
			CameraFreeze.Freeze(0f);
			PlayerController.instance.playerAvatarScript.SetSpectate();
			this.currentState = GameDirector.gameState.Main;
		}
	}

	// Token: 0x060012C3 RID: 4803 RVA: 0x000A888C File Offset: 0x000A6A8C
	private void Update()
	{
		if (SemiFunc.InputDown(InputKey.Menu) && !SemiFunc.MenuLevel() && !ChatManager.instance.chatActive)
		{
			if (SemiFunc.InputDown(InputKey.Back) && ChatManager.instance.StateIsActive())
			{
				return;
			}
			if (!MenuManager.instance.currentMenuPage)
			{
				MenuManager.instance.PageOpen(MenuPageIndex.Escape, false);
			}
			else if (MenuManager.instance.currentMenuPage.menuPageIndex == MenuPageIndex.Escape)
			{
				MenuManager.instance.PageCloseAll();
			}
		}
		if (this.outroStart)
		{
			this.OutroStart();
		}
		if (this.currentState == GameDirector.gameState.Load)
		{
			this.gameStateLoad();
		}
		else if (this.currentState == GameDirector.gameState.Start)
		{
			this.gameStateStart();
		}
		else if (this.currentState == GameDirector.gameState.Main)
		{
			this.gameStateMain();
		}
		else if (this.currentState == GameDirector.gameState.Outro)
		{
			this.gameStateOutro();
		}
		else if (this.currentState == GameDirector.gameState.End)
		{
			this.gameStateEnd();
		}
		else if (this.currentState == GameDirector.gameState.Death)
		{
			this.gameStateDeath();
		}
		if (this.DisableInput)
		{
			this.DisableInputTimer -= Time.deltaTime;
			if (this.DisableInputTimer <= 0f)
			{
				this.DisableInput = false;
			}
		}
	}

	// Token: 0x060012C4 RID: 4804 RVA: 0x000A89A8 File Offset: 0x000A6BA8
	public void OutroStart()
	{
		this.outroStart = true;
		if (this.currentState == GameDirector.gameState.Main)
		{
			this.currentState = GameDirector.gameState.Outro;
			if (FadeOverlay.Instance)
			{
				FadeOverlay.Instance.Image.color = Color.black;
			}
			this.gameStateStartImpulse = true;
			this.gameStateTimer = 0f;
		}
	}

	// Token: 0x060012C5 RID: 4805 RVA: 0x000A89FE File Offset: 0x000A6BFE
	public void DeathStart()
	{
		this.currentState = GameDirector.gameState.Death;
		this.gameStateStartImpulse = true;
		this.gameStateTimer = 0f;
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x000A8A19 File Offset: 0x000A6C19
	public void Revive()
	{
		this.currentState = GameDirector.gameState.Main;
		this.gameStateStartImpulse = true;
		this.gameStateTimer = 0f;
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x000A8A34 File Offset: 0x000A6C34
	public void CommandSetFPS(int _fps)
	{
		Application.targetFrameRate = _fps;
	}

	// Token: 0x060012C8 RID: 4808 RVA: 0x000A8A3C File Offset: 0x000A6C3C
	public void CommandRecordingDirectorToggle()
	{
		if (RecordingDirector.instance != null)
		{
			Object.Destroy(RecordingDirector.instance.gameObject);
			FlashlightController.Instance.hideFlashlight = false;
			return;
		}
		Object.Instantiate(Resources.Load("Recording Director"));
	}

	// Token: 0x060012C9 RID: 4809 RVA: 0x000A8A78 File Offset: 0x000A6C78
	public void CommandGreenScreenToggle()
	{
		if (this.greenScreenActive)
		{
			Object.Destroy(VideoGreenScreen.instance.gameObject);
			HurtVignette.instance.gameObject.SetActive(true);
			this.greenScreenActive = false;
			return;
		}
		Object.Instantiate<GameObject>(this.greenScreenPrefab);
		HurtVignette.instance.gameObject.SetActive(false);
		this.greenScreenActive = true;
	}

	// Token: 0x060012CA RID: 4810 RVA: 0x000A8AD7 File Offset: 0x000A6CD7
	public void SetDisableInput(float time)
	{
		this.DisableInput = true;
		this.DisableInputTimer = time;
	}

	// Token: 0x060012CB RID: 4811 RVA: 0x000A8AE7 File Offset: 0x000A6CE7
	public void SetStart()
	{
		if (this.currentState >= GameDirector.gameState.Outro)
		{
			return;
		}
		this.gameStateStartImpulse = true;
		this.currentState = GameDirector.gameState.Start;
	}

	// Token: 0x060012CC RID: 4812 RVA: 0x000A8B01 File Offset: 0x000A6D01
	private void LateUpdate()
	{
		this.FPSImpulses();
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x000A8B0C File Offset: 0x000A6D0C
	private void FPSImpulses()
	{
		float deltaTime = Time.deltaTime;
		this.fpsImpulse1 = false;
		this.fpsImpulse5 = false;
		this.fpsImpulse15 = false;
		this.fpsImpulse30 = false;
		this.fpsImpulse60 = false;
		this.timer1FPS += deltaTime;
		this.timer5FPS += deltaTime;
		this.timer15FPS += deltaTime;
		this.timer30FPS += deltaTime;
		this.timer60FPS += deltaTime;
		while (this.timer1FPS >= 1f)
		{
			this.fpsImpulse1 = true;
			this.timer1FPS -= 1f;
		}
		while (this.timer5FPS >= 0.2f)
		{
			this.fpsImpulse5 = true;
			this.timer5FPS -= 0.2f;
		}
		while (this.timer15FPS >= 0.06666667f)
		{
			this.fpsImpulse15 = true;
			this.timer15FPS -= 0.06666667f;
		}
		while (this.timer30FPS >= 0.033333335f)
		{
			this.fpsImpulse30 = true;
			this.timer30FPS -= 0.033333335f;
		}
		while (this.timer60FPS >= 0.016666668f)
		{
			this.fpsImpulse60 = true;
			this.timer60FPS -= 0.016666668f;
		}
	}

	// Token: 0x04001FD8 RID: 8152
	public static GameDirector instance;

	// Token: 0x04001FD9 RID: 8153
	public GameDirector.gameState currentState = GameDirector.gameState.Start;

	// Token: 0x04001FDA RID: 8154
	public bool LevelCompleted;

	// Token: 0x04001FDB RID: 8155
	public bool LevelCompletedDone;

	// Token: 0x04001FDC RID: 8156
	[Header("Debug")]
	public float TimeScale = 1f;

	// Token: 0x04001FDD RID: 8157
	[Space(15f)]
	public bool RandomSeed;

	// Token: 0x04001FDE RID: 8158
	public int Seed;

	// Token: 0x04001FDF RID: 8159
	[Space(15f)]
	[Header("Audio")]
	public AudioMixerSnapshot volumeOff;

	// Token: 0x04001FE0 RID: 8160
	public AudioMixerSnapshot volumeOn;

	// Token: 0x04001FE1 RID: 8161
	public AudioMixerSnapshot volumeCutsceneOnly;

	// Token: 0x04001FE2 RID: 8162
	public Sound SoundIntro;

	// Token: 0x04001FE3 RID: 8163
	public Sound SoundIntroRun;

	// Token: 0x04001FE4 RID: 8164
	public Sound SoundOutro;

	// Token: 0x04001FE5 RID: 8165
	public Sound SoundOutroRun;

	// Token: 0x04001FE6 RID: 8166
	public Sound SoundDeath;

	// Token: 0x04001FE7 RID: 8167
	[Space(15f)]
	[Header("Enemy")]
	public bool LevelEnemyChasing;

	// Token: 0x04001FE8 RID: 8168
	[Space(15f)]
	[Header("Other")]
	public Camera MainCamera;

	// Token: 0x04001FE9 RID: 8169
	[HideInInspector]
	public Transform MainCameraParent;

	// Token: 0x04001FEA RID: 8170
	public RenderTexture MainRenderTexture;

	// Token: 0x04001FEB RID: 8171
	[Space(15f)]
	public CameraTarget camTarget;

	// Token: 0x04001FEC RID: 8172
	public AnimNoise camNoise;

	// Token: 0x04001FED RID: 8173
	private float gameStateTimer;

	// Token: 0x04001FEE RID: 8174
	private bool gameStateStartImpulse = true;

	// Token: 0x04001FEF RID: 8175
	[Space(15f)]
	public GameObject cameraPosition;

	// Token: 0x04001FF0 RID: 8176
	public Animator cameraTargetAnimator;

	// Token: 0x04001FF1 RID: 8177
	public CameraShake CameraShake;

	// Token: 0x04001FF2 RID: 8178
	public CameraShake CameraImpact;

	// Token: 0x04001FF3 RID: 8179
	public CameraBob CameraBob;

	// Token: 0x04001FF4 RID: 8180
	[Space(15f)]
	public int InitialCleaningSpots;

	// Token: 0x04001FF5 RID: 8181
	public List<PlayerAvatar> PlayerList = new List<PlayerAvatar>();

	// Token: 0x04001FF6 RID: 8182
	internal List<PlayerDeathSpot> PlayerDeathSpots = new List<PlayerDeathSpot>();

	// Token: 0x04001FF7 RID: 8183
	public bool DisableInput;

	// Token: 0x04001FF8 RID: 8184
	private float DisableInputTimer;

	// Token: 0x04001FF9 RID: 8185
	internal bool outroStart;

	// Token: 0x04001FFA RID: 8186
	private float deathFreezeTime = 0.2f;

	// Token: 0x04001FFB RID: 8187
	private float deathFreezeTimer;

	// Token: 0x04001FFC RID: 8188
	private float timer1FPS;

	// Token: 0x04001FFD RID: 8189
	private float timer5FPS;

	// Token: 0x04001FFE RID: 8190
	private float timer15FPS;

	// Token: 0x04001FFF RID: 8191
	private float timer30FPS;

	// Token: 0x04002000 RID: 8192
	private float timer60FPS;

	// Token: 0x04002001 RID: 8193
	private const float INTERVAL_1FPS = 1f;

	// Token: 0x04002002 RID: 8194
	private const float INTERVAL_5FPS = 0.2f;

	// Token: 0x04002003 RID: 8195
	private const float INTERVAL_15FPS = 0.06666667f;

	// Token: 0x04002004 RID: 8196
	private const float INTERVAL_30FPS = 0.033333335f;

	// Token: 0x04002005 RID: 8197
	private const float INTERVAL_60FPS = 0.016666668f;

	// Token: 0x04002006 RID: 8198
	internal bool fpsImpulse1;

	// Token: 0x04002007 RID: 8199
	internal bool fpsImpulse5;

	// Token: 0x04002008 RID: 8200
	internal bool fpsImpulse15;

	// Token: 0x04002009 RID: 8201
	internal bool fpsImpulse30;

	// Token: 0x0400200A RID: 8202
	internal bool fpsImpulse60;

	// Token: 0x0400200B RID: 8203
	internal bool greenScreenActive;

	// Token: 0x0400200C RID: 8204
	public GameObject greenScreenPrefab;

	// Token: 0x020003F7 RID: 1015
	public enum gameState
	{
		// Token: 0x04002D42 RID: 11586
		Load,
		// Token: 0x04002D43 RID: 11587
		Start,
		// Token: 0x04002D44 RID: 11588
		Main,
		// Token: 0x04002D45 RID: 11589
		Outro,
		// Token: 0x04002D46 RID: 11590
		End,
		// Token: 0x04002D47 RID: 11591
		EndWait,
		// Token: 0x04002D48 RID: 11592
		Death
	}
}
