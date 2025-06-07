using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001B7 RID: 439
public class PlayerAvatar : MonoBehaviour, IPunObservable
{
	// Token: 0x06000EEE RID: 3822 RVA: 0x00086614 File Offset: 0x00084814
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.photonView = base.GetComponent<PhotonView>();
		this.collider = base.GetComponentInChildren<Collider>();
		this.isDisabled = false;
		base.transform.position = Vector3.zero + Vector3.forward * 2f;
		this.playerAvatarCollision = base.GetComponent<PlayerAvatarCollision>();
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			PhotonView component = playerAvatar.GetComponent<PhotonView>();
			if (this.photonView.Owner == component.Owner)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}
		GameDirector.instance.PlayerList.Add(this);
		if (!SemiFunc.IsMultiplayer() || this.photonView.IsMine)
		{
			this.isLocal = true;
		}
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x00086710 File Offset: 0x00084910
	private void OnDestroy()
	{
		GameDirector.instance.PlayerList.Remove(this);
		foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
		{
			enemyParent.Enemy.PlayerRemoved(this.photonView.ViewID);
		}
		Object.Destroy(base.transform.parent.gameObject);
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x0008679C File Offset: 0x0008499C
	private void Start()
	{
		this.overridePupilSizeSpring.speed = 15f;
		this.overridePupilSizeSpring.damping = 0.3f;
		this.localCamera = Camera.main;
		this.deadTimer = this.deadTime;
		if (!SemiFunc.IsMultiplayer() || this.photonView.IsMine)
		{
			base.StartCoroutine(this.WaitForSteamID());
			this.playerTransform = PlayerController.instance.transform;
			this.playerTransform.position = base.transform.position;
			PlayerController.instance.playerAvatar = base.gameObject;
			PlayerController.instance.playerAvatarScript = base.gameObject.GetComponent<PlayerAvatar>();
			if (PlayerAvatar.instance)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			PlayerAvatar.instance = this;
		}
		this.SoundSetup(this.jumpSound);
		this.SoundSetup(this.extraJumpSound);
		this.SoundSetup(this.landSound);
		this.SoundSetup(this.slideSound);
		this.SoundSetup(this.standToCrouchSound);
		this.SoundSetup(this.crouchToStandSound);
		this.SoundSetup(this.crouchToCrawlSound);
		this.SoundSetup(this.crawlToCrouchSound);
		this.SoundSetup(this.tumbleStartSound);
		this.SoundSetup(this.tumbleStopSound);
		this.SoundSetup(this.tumbleBreakFreeSound);
		this.AddToStatsManager();
		if (SemiFunc.IsMasterClient() && LevelGenerator.Instance.Generated)
		{
			LevelGenerator.Instance.PlayerSpawn();
		}
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x00086920 File Offset: 0x00084B20
	private IEnumerator WaitForSteamID()
	{
		while (this.steamID == null)
		{
			yield return null;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.PlayerAvatarSetColor(DataDirector.instance.ColorGetBody());
		}
		else if (!SemiFunc.IsMainMenu())
		{
			this.PlayerAvatarSetColor(DataDirector.instance.ColorGetBody());
		}
		yield break;
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x0008692F File Offset: 0x00084B2F
	private IEnumerator LateStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.2f);
		if (StatsManager.instance.playerUpgradeMapPlayerCount.ContainsKey(this.steamID))
		{
			this.upgradeMapPlayerCount = StatsManager.instance.playerUpgradeMapPlayerCount[this.steamID];
		}
		if (StatsManager.instance.playerUpgradeTumbleWings.ContainsKey(this.steamID))
		{
			this.upgradeTumbleWings = (float)StatsManager.instance.playerUpgradeTumbleWings[this.steamID];
		}
		if (StatsManager.instance.playerUpgradeCrouchRest.ContainsKey(this.steamID))
		{
			this.upgradeCrouchRest = (float)StatsManager.instance.playerUpgradeCrouchRest[this.steamID];
		}
		WorldSpaceUIParent.instance.PlayerName(this);
		yield break;
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x00086940 File Offset: 0x00084B40
	private void CrouchRestUpgrade()
	{
		if ((this.isCrouching || this.isCrawling) && !this.isSliding && !this.isMoving)
		{
			float num = this.upgradeCrouchRest;
			if (this.isLocal && !this.isTumbling)
			{
				float energyCurrent = PlayerController.instance.EnergyCurrent;
				float energyStart = PlayerController.instance.EnergyStart;
				if (SemiFunc.FPSImpulse5() && energyCurrent < energyStart)
				{
					EnergyUI.instance.SemiUITextFlashColor(Color.white, 0.1f);
					EnergyUI.instance.SemiUISpringScale(0.5f, 0.2f, 0.1f);
				}
				num += 1f;
				PlayerController.instance.EnergyCurrent = Mathf.Min(PlayerController.instance.EnergyCurrent + num * Time.deltaTime, energyStart);
			}
		}
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x00086A08 File Offset: 0x00084C08
	private void AddToStatsManager()
	{
		string text = SemiFunc.PlayerGetName(this);
		string text2 = SteamClient.SteamId.Value.ToString();
		if (GameManager.Multiplayer() && GameManager.instance.localTest)
		{
			int num = 0;
			Player[] playerList = PhotonNetwork.PlayerList;
			for (int i = 0; i < playerList.Length; i++)
			{
				if (playerList[i].IsLocal)
				{
					text = text + " " + num.ToString();
					text2 += num.ToString();
				}
				num++;
			}
		}
		if (GameManager.Multiplayer())
		{
			if (this.photonView.IsMine)
			{
				this.photonView.RPC("AddToStatsManagerRPC", RpcTarget.AllBuffered, new object[]
				{
					text,
					text2
				});
				return;
			}
		}
		else
		{
			this.AddToStatsManagerRPC(text, text2, default(PhotonMessageInfo));
		}
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x00086AD8 File Offset: 0x00084CD8
	private void FinalHealCheck()
	{
		if (!this.isLocal)
		{
			return;
		}
		if (!SemiFunc.RunIsLevel() && !SemiFunc.RunIsTutorial())
		{
			return;
		}
		if (SemiFunc.FPSImpulse5() && RoundDirector.instance.allExtractionPointsCompleted && this.RoomVolumeCheck.inTruck && !this.finalHeal)
		{
			this.FinalHeal();
		}
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x00086B2C File Offset: 0x00084D2C
	private void FinalHeal()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("FinalHealRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.FinalHealRPC(default(PhotonMessageInfo));
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x00086B68 File Offset: 0x00084D68
	[PunRPC]
	public void FinalHealRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		if (this.finalHeal)
		{
			return;
		}
		if (this.isLocal)
		{
			this.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Green, 2f, 1);
			this.playerHealth.Heal(25, true);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			TruckScreenText.instance.MessageSendCustom("", this.playerName + " {arrowright}{truck}{check}\n {point}{shades}{pointright}<b><color=#00FF00>+25</color></b>{heart}", 0);
		}
		TruckHealer.instance.Heal(this);
		this.truckReturn.Play(this.PlayerVisionTarget.VisionTransform.position, 1f, 1f, 1f, 1f);
		this.truckReturnGlobal.Play(this.PlayerVisionTarget.VisionTransform.position, 1f, 1f, 1f, 1f);
		this.playerAvatarVisuals.effectGetIntoTruck.gameObject.SetActive(true);
		this.finalHeal = true;
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x00086C64 File Offset: 0x00084E64
	[PunRPC]
	public void AddToStatsManagerRPC(string _playerName, string _steamID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			this.playerName = _playerName;
			this.steamID = _steamID;
			if (!SemiFunc.IsMultiplayer() || (SemiFunc.IsMultiplayer() && this.photonView.IsMine))
			{
				PlayerController.instance.PlayerSetName(this.playerName, this.steamID);
			}
			if (StatsManager.instance)
			{
				StatsManager.instance.PlayerAdd(_steamID, _playerName);
			}
		}
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x00086CD8 File Offset: 0x00084ED8
	[PunRPC]
	public void UpdateMyPlayerVoiceChat(int photonViewID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.voiceChat = PhotonView.Find(photonViewID).GetComponent<PlayerVoiceChat>();
		this.voiceChat.playerAvatar = this;
		if (this.voiceChat.TTSinstantiated)
		{
			this.voiceChat.ttsVoice.playerAvatar = this;
		}
		if (!SemiFunc.MenuLevel())
		{
			this.voiceChat.ToggleLobby(false);
		}
		this.voiceChatFetched = true;
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x00086D55 File Offset: 0x00084F55
	[PunRPC]
	public void ResetPhysPusher()
	{
		this.playerPhysPusher.Reset = true;
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x00086D64 File Offset: 0x00084F64
	public void SetDisabled()
	{
		if (GameManager.Multiplayer())
		{
			if (this.photonView.IsMine)
			{
				this.photonView.RPC("SetDisabledRPC", RpcTarget.All, Array.Empty<object>());
				PlayerVoiceChat.instance.OverridePitchCancel();
				return;
			}
		}
		else
		{
			this.SetDisabledRPC(default(PhotonMessageInfo));
		}
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x00086DB5 File Offset: 0x00084FB5
	[PunRPC]
	public void SetDisabledRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			this.isDisabled = true;
		}
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x00086DCC File Offset: 0x00084FCC
	public void UpdateState(bool isCrouching, bool isSprinting, bool isCrawling, bool isSliding, bool isMoving)
	{
		this.SetState(isCrouching, isSprinting, isCrawling, isSliding, isMoving);
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x00086DDC File Offset: 0x00084FDC
	private void FixedUpdate()
	{
		this.OverridePupilSizeTick();
		this.OverrideAnimationSpeedTick();
		if (SemiFunc.IsMultiplayer() && this.isLocal)
		{
			this.playerPingTimer -= Time.deltaTime;
			if (this.playerPingTimer <= 0f)
			{
				this.playerPing = PhotonNetwork.GetPing();
				this.playerPingTimer = 6f;
			}
		}
		if (!LevelGenerator.Instance.Generated)
		{
			if (this.spawned)
			{
				this.clientPosition = this.spawnPosition;
				this.clientPositionCurrent = this.spawnPosition;
				this.clientRotation = this.spawnRotation;
				this.clientRotationCurrent = this.spawnRotation;
				base.transform.position = this.spawnPosition;
				base.transform.rotation = this.spawnRotation;
				this.rb.MovePosition(base.transform.position);
				this.rb.MoveRotation(base.transform.rotation);
				if (PlayerController.instance.playerAvatarScript == this)
				{
					PlayerController.instance.transform.position = this.spawnPosition;
					PlayerController.instance.transform.rotation = this.spawnRotation;
				}
				if (this.spawnImpulse)
				{
					if (this.spawnFrames <= 0)
					{
						if (GameManager.Multiplayer())
						{
							LevelGenerator.Instance.PhotonView.RPC("PlayerSpawnedRPC", RpcTarget.All, Array.Empty<object>());
						}
						else
						{
							LevelGenerator.Instance.playerSpawned++;
						}
						this.spawnImpulse = false;
						return;
					}
					this.spawnFrames--;
				}
			}
			return;
		}
		if (this.spawnDoneImpulse)
		{
			if (PlayerController.instance.playerAvatarScript == this)
			{
				if (TruckScreenText.instance && !SemiFunc.MenuLevel())
				{
					Vector3 position = TruckScreenText.instance.transform.position;
					Vector3 eulerAngles = Quaternion.LookRotation(position - base.transform.position).eulerAngles;
					CameraAim.Instance.CameraAimSpawn(eulerAngles.y);
					CameraAim.Instance.AimTargetSet(position, 0.3f, 4f, base.gameObject, 0);
				}
				else
				{
					CameraAim.Instance.CameraAimSpawn(this.spawnRotation.eulerAngles.y);
				}
				if (SemiFunc.MenuLevel())
				{
					PlayerController.instance.rb.isKinematic = false;
				}
			}
			this.rb.isKinematic = false;
			this.spawnDoneImpulse = false;
		}
		if (this.photonView.IsMine || !SemiFunc.IsMultiplayer())
		{
			this.rbVelocityRaw = PlayerController.instance.rb.velocity;
			this.rb.MovePosition(base.transform.position);
			this.rb.MoveRotation(base.transform.rotation);
			return;
		}
		this.rb.MovePosition(this.clientPositionCurrent);
		this.rb.MoveRotation(this.clientRotationCurrent);
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x000870B8 File Offset: 0x000852B8
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		this.CrouchRestUpgrade();
		this.FinalHealCheck();
		this.OverrideAnimationSpeedLogic();
		this.OverridePupilSizeLogic();
		if (GameManager.Multiplayer() && GameDirector.instance.currentState >= GameDirector.gameState.Main)
		{
			if (this.voiceChatFetched)
			{
				if (!this.isDisabled)
				{
					this.voiceChat.transform.position = Vector3.Lerp(this.voiceChat.transform.position, this.PlayerVisionTarget.VisionTransform.transform.position, 30f * Time.deltaTime);
				}
			}
			else if (this.photonView.IsMine && PlayerVoiceChat.instance)
			{
				this.photonView.RPC("UpdateMyPlayerVoiceChat", RpcTarget.AllBuffered, new object[]
				{
					PlayerVoiceChat.instance.photonView.ViewID
				});
			}
		}
		if (this.photonView.IsMine || GameManager.instance.gameMode == 0)
		{
			if (this.playerTransform)
			{
				base.transform.position = this.playerTransform.position;
				base.transform.rotation = this.playerTransform.rotation;
			}
			this.localCameraRotation = PlayerController.instance.cameraGameObject.transform.rotation;
			this.localCameraPosition = PlayerController.instance.cameraGameObject.transform.position;
			this.localCameraTransform.position = PlayerController.instance.cameraGameObjectLocal.transform.position;
			this.localCameraTransform.rotation = PlayerController.instance.cameraGameObjectLocal.transform.rotation;
			this.InputDirection = PlayerController.instance.InputDirection;
		}
		else
		{
			this.clientPositionCurrent = this.clientPosition;
			this.clientRotationCurrent = this.clientRotation;
			this.localCameraTransform.position = Vector3.Lerp(this.localCameraTransform.position, this.localCameraPosition, 20f * Time.deltaTime);
			this.localCameraTransform.rotation = Quaternion.Lerp(this.localCameraTransform.rotation, this.localCameraRotation, 20f * Time.deltaTime);
		}
		if (this.deadSet)
		{
			if (this.isLocal && this.deadEnemyLookAtTransform)
			{
				CameraAim.Instance.AimTargetSet(this.deadEnemyLookAtTransform.position, 1f, 80f, this.deadEnemyLookAtTransform.gameObject, 0);
			}
			this.deadTimer -= Time.deltaTime;
			if (this.deadTimer <= 0f)
			{
				this.PlayerDeathDone();
			}
		}
		if (this.tumble)
		{
			this.isTumbling = this.tumble.isTumbling;
		}
		if (this.isTumbling)
		{
			this.collider.enabled = false;
		}
		else
		{
			this.collider.enabled = true;
		}
		this.LastNavMeshPositionTimer += Time.deltaTime;
		RaycastHit raycastHit;
		NavMeshHit navMeshHit;
		if (Physics.Raycast(base.transform.position + Vector3.up * 0.1f, Vector3.down, out raycastHit, 2f, LayerMask.GetMask(new string[]
		{
			"Default",
			"NavmeshOnly",
			"PlayerOnlyCollision"
		})) && NavMesh.SamplePosition(raycastHit.point, out navMeshHit, 0.5f, -1))
		{
			this.LastNavmeshPosition = navMeshHit.position;
			this.LastNavMeshPositionTimer = 0f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			if (base.transform.position.y < -100f)
			{
				this.tumble.TumbleSet(true, false);
				this.tumble.physGrabObject.Teleport(TruckSafetySpawnPoint.instance.transform.position, TruckSafetySpawnPoint.instance.transform.rotation);
				this.tumble.physGrabObject.rb.velocity = Vector3.zero;
				this.tumble.physGrabObject.rb.angularVelocity = Vector3.zero;
				this.FallDamageResetSet(2f);
			}
			this.FallDamageResetLogic();
		}
		if (this.enemyVisionFreezeTimer > 0f)
		{
			this.enemyVisionFreezeTimer -= Time.deltaTime;
		}
		if (this.isLocal && PlayerController.instance.CollisionController.fallDistance >= 8f)
		{
			float fallDistance = PlayerController.instance.CollisionController.fallDistance;
			float num = 5f;
			float num2 = 4f;
			if (fallDistance > num)
			{
				int damage = 5;
				float time = 0.5f;
				if (fallDistance > num + num2 * 4f)
				{
					damage = 100;
					time = 2f;
				}
				else if (fallDistance > num + num2 * 3f)
				{
					damage = 50;
					time = 2f;
				}
				else if (fallDistance > num + num2 * 2f)
				{
					damage = 25;
					time = 3f;
				}
				else if (fallDistance > num + num2)
				{
					damage = 15;
					time = 3f;
				}
				this.tumble.TumbleRequest(true, false);
				this.tumble.TumbleOverrideTime(time);
				if (SemiFunc.FPSImpulse15())
				{
					this.tumble.ImpactHurtSet(0.5f, damage);
				}
			}
		}
		if (!this.colorWasSet)
		{
			this.noColorFailsafeTimer -= Time.deltaTime;
			if (this.noColorFailsafeTimer <= 0f)
			{
				this.playerAvatarVisuals.SetColor(0, default(Color));
				this.colorWasSet = true;
			}
		}
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x00087629 File Offset: 0x00085829
	public void SetState(bool crouching, bool sprinting, bool crawling, bool sliding, bool moving)
	{
		this.isCrouching = crouching;
		this.isSprinting = sprinting;
		this.isCrawling = crawling;
		this.isSliding = sliding;
		this.isMoving = moving;
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x00087650 File Offset: 0x00085850
	private void OverrideAnimationSpeedActivate(bool active, float _speedMulti, float _in, float _out, float _time = 0.1f)
	{
		if (!this.isLocal)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("OverrideAnimationSpeedActivateRPC", RpcTarget.All, new object[]
			{
				active,
				_speedMulti,
				_in,
				_out,
				_time
			});
			return;
		}
		this.OverrideAnimationSpeedActivateRPC(active, _speedMulti, _in, _out, _time);
	}

	// Token: 0x06000F02 RID: 3842 RVA: 0x000876C1 File Offset: 0x000858C1
	[PunRPC]
	public void OverrideAnimationSpeedActivateRPC(bool active, float _speedMulti, float _in, float _out, float _time = 0.1f)
	{
		this.overrideAnimationSpeedActive = active;
		this.overrrideAnimationSpeedTimer = _time;
		this.overrrideAnimationSpeedTarget = _speedMulti;
		this.overrrideAnimationSpeedIn = _in;
		this.overrrideAnimationSpeedOut = _out;
		this.overrideAnimationSpeedTime = _time;
	}

	// Token: 0x06000F03 RID: 3843 RVA: 0x000876F0 File Offset: 0x000858F0
	public void OverrideAnimationSpeed(float _speedMulti, float _in, float _out, float _time = 0.1f)
	{
		float num = this.overrrideAnimationSpeedTarget;
		this.overrrideAnimationSpeedTimer = _time;
		this.overrrideAnimationSpeedTarget = _speedMulti;
		this.overrrideAnimationSpeedIn = _in;
		this.overrrideAnimationSpeedOut = _out;
		this.overrideAnimationSpeedTime = _time;
		if (SemiFunc.IsMultiplayer() && (!this.overrideAnimationSpeedActive || num != _speedMulti))
		{
			this.OverrideAnimationSpeedActivate(true, _speedMulti, _in, _out, _time);
		}
	}

	// Token: 0x06000F04 RID: 3844 RVA: 0x00087748 File Offset: 0x00085948
	private void OverrideAnimationSpeedTick()
	{
		if (this.overrrideAnimationSpeedTimer > 0f)
		{
			this.overrrideAnimationSpeedTimer -= Time.fixedDeltaTime;
			if (this.overrrideAnimationSpeedTimer <= 0f && SemiFunc.IsMultiplayer() && this.overrideAnimationSpeedActive)
			{
				this.OverrideAnimationSpeedActivate(false, this.overrrideAnimationSpeedTarget, this.overrrideAnimationSpeedIn, this.overrrideAnimationSpeedOut, this.overrideAnimationSpeedTime);
			}
		}
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x000877B0 File Offset: 0x000859B0
	private void OverrideAnimationSpeedLogic()
	{
		if (!this.playerAvatarVisuals)
		{
			return;
		}
		if (this.overrrideAnimationSpeedTimer <= 0f && this.playerAvatarVisuals.animationSpeedMultiplier == 1f)
		{
			return;
		}
		if (!this.isLocal && this.overrideAnimationSpeedActive)
		{
			this.OverrideAnimationSpeed(this.overrrideAnimationSpeedTarget, this.overrrideAnimationSpeedIn, this.overrrideAnimationSpeedOut, this.overrideAnimationSpeedTime);
		}
		if (this.overrrideAnimationSpeedTimer > 0f)
		{
			this.overrideAnimationSpeedLerp = Mathf.Lerp(this.overrideAnimationSpeedLerp, 1f, Time.deltaTime * this.overrrideAnimationSpeedIn);
		}
		else
		{
			this.overrideAnimationSpeedLerp = Mathf.Lerp(this.overrideAnimationSpeedLerp, 0f, Time.deltaTime * this.overrrideAnimationSpeedOut);
		}
		this.playerAvatarVisuals.animationSpeedMultiplier = Mathf.Lerp(1f, this.overrrideAnimationSpeedTarget, this.overrideAnimationSpeedLerp);
		if (this.playerAvatarVisuals.animationSpeedMultiplier > 0.98f)
		{
			this.playerAvatarVisuals.animationSpeedMultiplier = 1f;
		}
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x000878B0 File Offset: 0x00085AB0
	private void OverridePupilSizeActivate(bool active, float _multiplier, int _prio, float springSpeedIn, float dampIn, float springSpeedOut, float dampOut, float _time = 0.1f)
	{
		if (!this.isLocal)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("OverridePupilSizeActivateRPC", RpcTarget.All, new object[]
			{
				active,
				_multiplier,
				_prio,
				springSpeedIn,
				dampIn,
				springSpeedOut,
				dampOut,
				_time
			});
			return;
		}
		this.OverridePupilSizeActivateRPC(active, _multiplier, _prio, springSpeedIn, dampIn, springSpeedOut, dampOut, _time);
	}

	// Token: 0x06000F07 RID: 3847 RVA: 0x00087948 File Offset: 0x00085B48
	[PunRPC]
	public void OverridePupilSizeActivateRPC(bool active, float _multiplier, int _prio, float springSpeedIn, float dampIn, float springSpeedOut, float dampOut, float _time = 0.1f)
	{
		this.overridePupilSizeActive = active;
		this.overridePupilSizeMultiplier = _multiplier;
		this.overridePupilSizeMultiplierTarget = _multiplier;
		this.overridePupilSizePrio = _prio;
		this.overridePupilSpringSpeedIn = springSpeedIn;
		this.overridePupilSpringDampIn = dampIn;
		this.overridePupilSpringSpeedOut = springSpeedOut;
		this.overridePupilSpringDampOut = dampOut;
		this.overridePupilSizeTime = _time;
	}

	// Token: 0x06000F08 RID: 3848 RVA: 0x0008799C File Offset: 0x00085B9C
	public void OverridePupilSize(float _multiplier, int _prio, float springSpeedIn, float springDampIn, float springSpeedOut, float springDampOut, float _time = 0.1f)
	{
		if (this.overridePupilSizeTimer > 0f && _prio < this.overridePupilSizePrio)
		{
			return;
		}
		float num = this.overridePupilSizeMultiplierTarget;
		this.overridePupilSizeMultiplier = _multiplier;
		this.overridePupilSizeMultiplierTarget = _multiplier;
		this.overridePupilSizePrio = _prio;
		this.overridePupilSpringSpeedIn = springSpeedIn;
		this.overridePupilSpringDampIn = springDampIn;
		this.overridePupilSpringSpeedOut = springSpeedOut;
		this.overridePupilSpringDampOut = springDampOut;
		this.overridePupilSizeTime = _time;
		this.overridePupilSizeTimer = _time;
		if (SemiFunc.IsMultiplayer() && (!this.overridePupilSizeActive || num != _multiplier))
		{
			this.OverridePupilSizeActivate(true, _multiplier, _prio, springSpeedIn, springDampIn, springSpeedOut, springDampOut, _time);
		}
	}

	// Token: 0x06000F09 RID: 3849 RVA: 0x00087A30 File Offset: 0x00085C30
	private void OverridePupilSizeTick()
	{
		if (this.overridePupilSizeTimer > 0f)
		{
			this.overridePupilSizeTimer -= Time.fixedDeltaTime;
			if (this.overridePupilSizeTimer <= 0f && SemiFunc.IsMultiplayer() && this.overridePupilSizeActive)
			{
				this.OverridePupilSizeActivate(false, this.overridePupilSizeMultiplierTarget, this.overridePupilSizePrio, this.overridePupilSpringSpeedIn, this.overridePupilSpringDampIn, this.overridePupilSpringSpeedOut, this.overridePupilSpringDampOut, this.overridePupilSizeTime);
			}
		}
	}

	// Token: 0x06000F0A RID: 3850 RVA: 0x00087AAC File Offset: 0x00085CAC
	private void OverridePupilSizeLogic()
	{
		if (!this.playerAvatarVisuals)
		{
			return;
		}
		if (!this.isLocal && this.overridePupilSizeActive)
		{
			this.OverridePupilSize(this.overridePupilSizeMultiplierTarget, this.overridePupilSizePrio, this.overridePupilSpringSpeedIn, this.overridePupilSpringDampIn, this.overridePupilSpringSpeedOut, this.overridePupilSpringDampOut, this.overridePupilSizeTime);
		}
		if (this.overridePupilSizeTimer > 0f)
		{
			this.overridePupilSizeSpring.speed = this.overridePupilSpringSpeedIn;
			this.overridePupilSizeSpring.damping = this.overridePupilSpringDampIn;
			this.playerAvatarVisuals.playerEyes.pupilSizeMultiplier = SemiFunc.SpringFloatGet(this.overridePupilSizeSpring, this.overridePupilSizeMultiplierTarget, -1f);
			return;
		}
		this.overridePupilSizeSpring.speed = this.overridePupilSpringSpeedOut;
		this.overridePupilSizeSpring.damping = this.overridePupilSpringDampOut;
		this.playerAvatarVisuals.playerEyes.pupilSizeMultiplier = SemiFunc.SpringFloatGet(this.overridePupilSizeSpring, 1f, -1f);
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x00087BA4 File Offset: 0x00085DA4
	public void SetSpectate()
	{
		Object.Instantiate<GameObject>(this.spectateCamera).GetComponent<SpectateCamera>().SetDeath(this.spectatePoint);
		this.spectating = true;
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x00087BC8 File Offset: 0x00085DC8
	public void SoundSetup(Sound _sound)
	{
		if (this.photonView.IsMine)
		{
			_sound.SpatialBlend = 0f;
			return;
		}
		_sound.Volume *= 0.5f;
		_sound.VolumeRandom *= 0.5f;
		_sound.SpatialBlend = 1f;
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x00087C1D File Offset: 0x00085E1D
	public void EnemyVisionFreezeTimerSet(float _time)
	{
		this.enemyVisionFreezeTimer = _time;
	}

	// Token: 0x06000F0E RID: 3854 RVA: 0x00087C28 File Offset: 0x00085E28
	public void FlashlightFlicker(float _multiplier)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("FlashlightFlickerRPC", RpcTarget.All, new object[]
			{
				_multiplier
			});
			return;
		}
		this.FlashlightFlickerRPC(_multiplier, default(PhotonMessageInfo));
	}

	// Token: 0x06000F0F RID: 3855 RVA: 0x00087C6D File Offset: 0x00085E6D
	[PunRPC]
	public void FlashlightFlickerRPC(float _multiplier, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.flashlightController.FlickerSet(_multiplier);
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x00087C8C File Offset: 0x00085E8C
	public void Slide()
	{
		this.slideSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		if (!GameManager.Multiplayer())
		{
			Materials.Instance.Slide(base.transform.position, this.MaterialTrigger, 0f, true);
			return;
		}
		Materials.Instance.Slide(base.transform.position, this.MaterialTrigger, 0f, true);
		this.photonView.RPC("SlideRPC", RpcTarget.Others, Array.Empty<object>());
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x00087D24 File Offset: 0x00085F24
	[PunRPC]
	private void SlideRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.slideSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Slide(base.transform.position, this.MaterialTrigger, 1f, false);
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x00087D8C File Offset: 0x00085F8C
	public void Jump(bool _powerupEffect)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.JumpRPC(_powerupEffect, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("JumpRPC", RpcTarget.All, new object[]
		{
			_powerupEffect
		});
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x00087DD8 File Offset: 0x00085FD8
	[PunRPC]
	private void JumpRPC(bool _powerupEffect, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.playerAvatarVisuals.JumpImpulse();
		this.jumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.HostType hostType = Materials.HostType.LocalPlayer;
		if (!this.isLocal)
		{
			hostType = Materials.HostType.OtherPlayer;
		}
		Materials.Instance.Impulse(base.transform.position, Vector3.down, Materials.SoundType.Heavy, true, this.MaterialTrigger, hostType);
		if (_powerupEffect)
		{
			this.extraJumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.playerAvatarVisuals.PowerupJumpEffect();
		}
	}

	// Token: 0x06000F14 RID: 3860 RVA: 0x00087E94 File Offset: 0x00086094
	public void Land()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.LandRPC(default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("LandRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x00087ED4 File Offset: 0x000860D4
	[PunRPC]
	private void LandRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position + Vector3.up * 0.1f, Vector3.down, out raycastHit, 0.25f, SemiFunc.LayerMaskGetPhysGrabObject()))
		{
			PhysGrabObject component = raycastHit.transform.GetComponent<PhysGrabObject>();
			if (component)
			{
				component.mediumBreakImpulse = true;
				return;
			}
		}
		EnemyDirector.instance.SetInvestigate(base.transform.position + Vector3.up * 0.2f, 10f, false);
		Materials.HostType hostType = Materials.HostType.LocalPlayer;
		if (!this.isLocal)
		{
			hostType = Materials.HostType.OtherPlayer;
		}
		this.landSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(base.transform.position, Vector3.down, Materials.SoundType.Heavy, true, this.MaterialTrigger, hostType);
		Vector3 position = this.PlayerVisionTarget.VisionTransform.position;
		if (this.isLocal)
		{
			position = this.localCameraPosition;
		}
		SemiFunc.PlayerEyesOverrideSoft(position, 2f, base.gameObject, 5f);
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x00088008 File Offset: 0x00086208
	public void Footstep(Materials.SoundType soundType)
	{
		if (RecordingDirector.instance)
		{
			return;
		}
		Materials.HostType hostType = Materials.HostType.LocalPlayer;
		if (!this.isLocal)
		{
			hostType = Materials.HostType.OtherPlayer;
		}
		Materials.Instance.Impulse(base.transform.position, Vector3.down, soundType, true, this.MaterialTrigger, hostType);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (soundType == Materials.SoundType.Heavy)
			{
				EnemyDirector.instance.SetInvestigate(base.transform.position + Vector3.up * 0.2f, 5f, false);
				return;
			}
			if (soundType == Materials.SoundType.Medium)
			{
				EnemyDirector.instance.SetInvestigate(base.transform.position + Vector3.up * 0.2f, 1f, false);
			}
		}
	}

	// Token: 0x06000F17 RID: 3863 RVA: 0x000880C0 File Offset: 0x000862C0
	public void StandToCrouch()
	{
		if (this.isSprinting)
		{
			return;
		}
		this.standToCrouchSound.Play(base.transform.position, 1f, 1f, 1f, 1f).pitch *= this.playerAvatarVisuals.animationSpeedMultiplier;
	}

	// Token: 0x06000F18 RID: 3864 RVA: 0x00088117 File Offset: 0x00086317
	private float GetPitchMulti()
	{
		return Mathf.Clamp(this.playerAvatarVisuals.animationSpeedMultiplier, 0.5f, 1.5f);
	}

	// Token: 0x06000F19 RID: 3865 RVA: 0x00088134 File Offset: 0x00086334
	public void CrouchToStand()
	{
		AudioSource audioSource = this.crouchToStandSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		float pitchMulti = this.GetPitchMulti();
		audioSource.pitch *= pitchMulti;
	}

	// Token: 0x06000F1A RID: 3866 RVA: 0x00088180 File Offset: 0x00086380
	public void CrouchToCrawl()
	{
		if (this.isSliding || this.isSprinting)
		{
			return;
		}
		AudioSource audioSource = this.crouchToCrawlSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		float pitchMulti = this.GetPitchMulti();
		audioSource.pitch *= pitchMulti;
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x000881DC File Offset: 0x000863DC
	public void CrawlToCrouch()
	{
		if (this.isSliding || this.isSprinting)
		{
			return;
		}
		AudioSource audioSource = this.crawlToCrouchSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		float pitchMulti = this.GetPitchMulti();
		audioSource.pitch *= pitchMulti;
	}

	// Token: 0x06000F1C RID: 3868 RVA: 0x00088238 File Offset: 0x00086438
	public void TumbleStart()
	{
		AudioSource audioSource = this.tumbleStartSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		float pitchMulti = this.GetPitchMulti();
		audioSource.pitch *= pitchMulti;
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x00088284 File Offset: 0x00086484
	public void TumbleStop()
	{
		AudioSource audioSource = this.tumbleStopSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		float pitchMulti = this.GetPitchMulti();
		audioSource.pitch *= pitchMulti;
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x000882D0 File Offset: 0x000864D0
	public void TumbleBreakFree()
	{
		this.tumbleBreakFreeSound.Play(base.transform.position, 1f, 1f, 1f, 1f).pitch *= this.GetPitchMulti();
		this.playerAvatarVisuals.TumbleBreakFreeEffect();
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x00088324 File Offset: 0x00086524
	public void PlayerGlitchShort()
	{
		if (GameManager.instance.gameMode == 0)
		{
			CameraGlitch.Instance.PlayShort();
			return;
		}
		this.photonView.RPC("PlayerGlitchShortRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x00088353 File Offset: 0x00086553
	[PunRPC]
	private void PlayerGlitchShortRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		if (this.photonView.IsMine)
		{
			CameraGlitch.Instance.PlayShort();
		}
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x0008837C File Offset: 0x0008657C
	public void Spawn(Vector3 position, Quaternion rotation)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.SpawnRPC(position, rotation, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("SpawnRPC", RpcTarget.All, new object[]
		{
			position,
			rotation
		});
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x000883D0 File Offset: 0x000865D0
	[PunRPC]
	private void SpawnRPC(Vector3 position, Quaternion rotation, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (!this.photonView)
		{
			this.photonView = base.GetComponent<PhotonView>();
		}
		if (!this.rb)
		{
			this.rb = base.GetComponent<Rigidbody>();
		}
		if (!GameManager.Multiplayer() || this.photonView.IsMine)
		{
			PlayerController.instance.transform.position = position;
			PlayerController.instance.transform.rotation = rotation;
		}
		this.rb.position = position;
		this.rb.rotation = rotation;
		base.transform.position = position;
		base.transform.rotation = rotation;
		this.clientPosition = position;
		this.clientPositionCurrent = position;
		this.clientRotation = rotation;
		this.clientRotationCurrent = rotation;
		this.spawnPosition = position;
		this.spawnRotation = rotation;
		this.playerAvatarVisuals.visualPosition = position;
		this.spawned = true;
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x000884BC File Offset: 0x000866BC
	public void PlayerDeath(int enemyIndex)
	{
		if (this.deadSet)
		{
			return;
		}
		if (GameManager.instance.gameMode == 0)
		{
			this.PlayerDeathRPC(enemyIndex, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("PlayerDeathRPC", RpcTarget.All, new object[]
		{
			enemyIndex
		});
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x00088510 File Offset: 0x00086710
	[PunRPC]
	public void PlayerDeathRPC(int enemyIndex, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.playerHealth.Death();
		this.deadSet = true;
		if (!this.isLocal)
		{
			this.deathBuildupSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		if (this.isLocal)
		{
			this.deadEnemyLookAtTransform = null;
			Enemy enemy = SemiFunc.EnemyGetFromIndex(enemyIndex);
			if (enemy)
			{
				if (enemy.KillLookAtTransform)
				{
					this.deadEnemyLookAtTransform = enemy.KillLookAtTransform;
				}
				else
				{
					Debug.LogError("Enemy has no kill look at transform..." + enemy.name);
				}
			}
			this.physGrabber.ReleaseObject(0.1f);
			if (this.playerTransform)
			{
				this.playerTransform.parent.gameObject.SetActive(false);
			}
			CameraGlitch.Instance.PlayLongHurt();
			GameDirector.instance.DeathStart();
		}
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x0008860C File Offset: 0x0008680C
	private void PlayerDeathDone()
	{
		if (this.voiceChatFetched)
		{
			this.voiceChat.OverrideMute(1f);
			this.voiceChat.ToggleLobby(true);
		}
		if (SemiFunc.RunIsTutorial())
		{
			TutorialDirector.instance.deadPlayer = true;
		}
		if (this.isDisabled)
		{
			return;
		}
		this.isDisabled = true;
		Object.Instantiate<GameObject>(this.deathSpot, base.transform.position, Quaternion.identity);
		if (GameManager.Multiplayer())
		{
			if (!this.isLocal)
			{
				if (SpectateCamera.instance)
				{
					SpectateCamera.instance.UpdatePlayer(this);
				}
			}
			else
			{
				this.physGrabber.ReleaseObject(0.1f);
				if (SemiFunc.IsMultiplayer())
				{
					if (!SemiFunc.RunIsArena() && Inventory.instance.physGrabber.photonView.ViewID == this.physGrabber.photonView.ViewID)
					{
						Inventory.instance.ForceUnequip();
					}
				}
				else
				{
					Inventory.instance.ForceUnequip();
				}
			}
		}
		this.deathSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.playerDeathHead.Trigger();
		this.playerDeathEffects.Trigger();
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x0008874C File Offset: 0x0008694C
	public void OutroStart()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.OutroStartRPC(default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("OutroStartRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x0008878B File Offset: 0x0008698B
	[PunRPC]
	public void OutroStartRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (this.isLocal)
		{
			GameDirector.instance.OutroStart();
		}
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x000887A8 File Offset: 0x000869A8
	public void OutroDone()
	{
		if (this.quitApplication)
		{
			Application.Quit();
			return;
		}
		if (NetworkManager.instance.leavePhotonRoom)
		{
			NetworkManager.instance.LeavePhotonRoom();
			return;
		}
		if (GameManager.instance.gameMode == 0)
		{
			this.OutroDoneRPC(default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("OutroDoneRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000F29 RID: 3881 RVA: 0x0008880C File Offset: 0x00086A0C
	[PunRPC]
	public void OutroDoneRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.outroDone = true;
	}

	// Token: 0x06000F2A RID: 3882 RVA: 0x00088824 File Offset: 0x00086A24
	public void ForceImpulse(Vector3 _force)
	{
		if (!GameManager.Multiplayer())
		{
			this.ForceImpulseRPC(_force, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("ForceImpulseRPC", RpcTarget.All, new object[]
		{
			_force
		});
	}

	// Token: 0x06000F2B RID: 3883 RVA: 0x00088869 File Offset: 0x00086A69
	[PunRPC]
	private void ForceImpulseRPC(Vector3 _force, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		if (!GameManager.Multiplayer() || this.photonView.IsMine)
		{
			PlayerController.instance.ForceImpulse(_force);
		}
	}

	// Token: 0x06000F2C RID: 3884 RVA: 0x0008889C File Offset: 0x00086A9C
	public void PlayerAvatarSetColor(int colorIndex)
	{
		if (!GameManager.Multiplayer())
		{
			this.SetColorRPC(colorIndex, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("SetColorRPC", RpcTarget.AllBuffered, new object[]
		{
			colorIndex
		});
	}

	// Token: 0x06000F2D RID: 3885 RVA: 0x000888E4 File Offset: 0x00086AE4
	[PunRPC]
	private void SetColorRPC(int colorIndex, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		if (colorIndex < 0 || colorIndex >= AssetManager.instance.playerColors.Count)
		{
			return;
		}
		this.colorWasSet = true;
		if (this.isLocal)
		{
			DataDirector.instance.ColorSetBody(colorIndex);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			StatsManager.instance.SetPlayerColor(this.steamID, colorIndex);
		}
		this.playerAvatarVisuals.SetColor(colorIndex, default(Color));
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x00088960 File Offset: 0x00086B60
	public void Revive(bool _revivedByTruck = false)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ReviveRPC(_revivedByTruck, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("ReviveRPC", RpcTarget.All, new object[]
		{
			_revivedByTruck
		});
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x000889AC File Offset: 0x00086BAC
	[PunRPC]
	public void ReviveRPC(bool _revivedByTruck, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (!this.playerDeathHead)
		{
			Debug.LogError("Tried to revive without death head...");
			return;
		}
		TutorialDirector.instance.playerRevived = true;
		if (_revivedByTruck)
		{
			TruckHealer.instance.Heal(this);
		}
		Vector3 position = this.playerDeathHead.physGrabObject.centerPoint - Vector3.up * 0.25f;
		Vector3 eulerAngles = this.playerDeathHead.physGrabObject.transform.eulerAngles;
		if (SemiFunc.RunIsTutorial())
		{
			position = Vector3.zero + Vector3.up * 2f - Vector3.right * 5f;
			this.playerDeathHead.transform.position = position;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.tumble.physGrabObject.Teleport(position, base.transform.rotation);
		}
		base.transform.position = position;
		this.clientPositionCurrent = base.transform.position;
		this.clientPosition = base.transform.position;
		this.clientPhysRiding = false;
		base.gameObject.SetActive(true);
		this.playerAvatarVisuals.gameObject.SetActive(true);
		this.playerAvatarVisuals.transform.position = base.transform.position;
		this.playerAvatarVisuals.visualPosition = base.transform.position;
		this.playerAvatarVisuals.Revive();
		this.isDisabled = false;
		this.playerDeathHead.Reset();
		this.playerDeathEffects.Reset();
		this.playerReviveEffects.Trigger();
		this.deadSet = false;
		this.deadTimer = this.deadTime;
		if (this.voiceChat)
		{
			this.voiceChat.ToggleLobby(false);
		}
		this.playerAvatarCollision.SetCrouch();
		this.playerHealth.SetMaterialGreen();
		if (this.isLocal)
		{
			this.playerHealth.HealOther(1, true);
			this.playerTransform.position = base.transform.position;
			this.playerTransform.parent.gameObject.SetActive(true);
			CameraAim.Instance.CameraAimSpawn(eulerAngles.y);
			GameDirector.instance.Revive();
			SpectateCamera.instance.StopSpectate();
			PlayerController.instance.Revive(eulerAngles);
			CameraGlitch.Instance.PlayLongHeal();
		}
		else if (!_revivedByTruck && SemiFunc.RunIsLevel())
		{
			PlayerAvatar playerAvatarScript = PlayerController.instance.playerAvatarScript;
			if (!playerAvatarScript.isDisabled && Vector3.Distance(playerAvatarScript.transform.position, base.transform.position) < 10f && playerAvatarScript.playerHealth.health >= 50 && !TutorialDirector.instance.playerHealed && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialHealing, 1))
			{
				TutorialDirector.instance.ActivateTip("Healing", 0.5f, false);
			}
		}
		this.RoomVolumeCheck.CheckSet();
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x00088C94 File Offset: 0x00086E94
	private void FallDamageResetLogic()
	{
		if (this.fallDamageResetTimer > 0f)
		{
			this.fallDamageResetTimer -= Time.deltaTime;
			this.fallDamageResetState = true;
		}
		else
		{
			this.fallDamageResetState = false;
		}
		if (this.fallDamageResetState != this.fallDamageResetStatePrevious)
		{
			this.fallDamageResetStatePrevious = this.fallDamageResetState;
			if (!GameManager.Multiplayer())
			{
				this.FallDamageResetUpdateRPC(this.fallDamageResetState, default(PhotonMessageInfo));
				return;
			}
			this.photonView.RPC("FallDamageResetUpdateRPC", RpcTarget.All, new object[]
			{
				this.fallDamageResetState
			});
		}
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x00088D2C File Offset: 0x00086F2C
	public void FallDamageResetSet(float _time)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.fallDamageResetTimer = _time;
		}
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x00088D3C File Offset: 0x00086F3C
	[PunRPC]
	private void FallDamageResetUpdateRPC(bool _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.fallDamageResetState = _state;
		if (this.isLocal && this.fallDamageResetState && this.tumble)
		{
			this.tumble.impactHurtTimer = 0f;
			this.tumble.impactHurtDamage = 0;
		}
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x00088D92 File Offset: 0x00086F92
	private void ChatMessageSpeak(string _message, bool crouching)
	{
		if (!this.voiceChat)
		{
			return;
		}
		if (!this.voiceChat.ttsVoice)
		{
			return;
		}
		this.voiceChat.ttsVoice.TTSSpeakNow(_message, crouching);
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x00088DC8 File Offset: 0x00086FC8
	public void ChatMessageSend(string _message)
	{
		bool flag = this.isCrouching;
		SemiFunc.Command(_message);
		if (!SemiFunc.IsMultiplayer())
		{
			this.ChatMessageSpeak(_message, flag);
			return;
		}
		if (this.isDisabled)
		{
			flag = true;
		}
		this.photonView.RPC("ChatMessageSendRPC", RpcTarget.All, new object[]
		{
			_message,
			flag
		});
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x00088E20 File Offset: 0x00087020
	[PunRPC]
	public void ChatMessageSendRPC(string _message, bool crouching, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		this.ChatMessageSpeak(_message, crouching);
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x00088E48 File Offset: 0x00087048
	public void LoadingLevelAnimationCompleted()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("LoadingLevelAnimationCompletedRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.LoadingLevelAnimationCompletedRPC(default(PhotonMessageInfo));
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x00088E82 File Offset: 0x00087082
	[PunRPC]
	public void LoadingLevelAnimationCompletedRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.levelAnimationCompleted = true;
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x00088E9A File Offset: 0x0008709A
	public void HealedOther()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("HealedOtherRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x00088EB9 File Offset: 0x000870B9
	[PunRPC]
	public void HealedOtherRPC()
	{
		if (this.isLocal)
		{
			TutorialDirector.instance.playerHealed = true;
		}
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x00088ED0 File Offset: 0x000870D0
	public void PlayerExpressionSet(int _expressionIndex, float _percent)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.PlayerExpressionSetRPC(_expressionIndex, _percent, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("PlayerExpressionSetRPC", RpcTarget.All, new object[]
		{
			_expressionIndex,
			_percent
		});
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x00088F20 File Offset: 0x00087120
	public void PlayerExpressionReset()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.PlayerExpressionResetRPC(default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("PlayerExpressionResetRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x00088F5A File Offset: 0x0008715A
	[PunRPC]
	public void PlayerExpressionSetRPC(int _expressionIndex, float _percent, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.playerExpression.expressions[_expressionIndex].stopExpressing = false;
		this.playerExpressions[_expressionIndex] = _percent;
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x00088F8F File Offset: 0x0008718F
	[PunRPC]
	public void PlayerExpressionStopRPC(int _expressionIndex, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.playerExpressions[_expressionIndex] = 0f;
		this.playerExpression.expressions[_expressionIndex].stopExpressing = true;
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x00088FC8 File Offset: 0x000871C8
	public void PlayerExpressionStop(int _expressionIndex)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.PlayerExpressionStopRPC(_expressionIndex, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("PlayerExpressionStopRPC", RpcTarget.All, new object[]
		{
			_expressionIndex
		});
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x00089010 File Offset: 0x00087210
	public void UpgradeTumbleWingsVisualsActive(bool _visualsActive = true)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.UpgradeTumbleWingsVisualsActiveRPC(_visualsActive, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("UpgradeTumbleWingsVisualsActiveRPC", RpcTarget.All, new object[]
		{
			_visualsActive
		});
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x00089055 File Offset: 0x00087255
	[PunRPC]
	private void UpgradeTumbleWingsVisualsActiveRPC(bool _visualsActive = true, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.upgradeTumbleWingsVisualsActive = _visualsActive;
		this.upgradeTumbleWingsLogic.tumbleWingTimer = 1f;
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x0008907D File Offset: 0x0008727D
	[PunRPC]
	public void PlayerExpressionResetRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.playerExpressions.Clear();
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x0008909C File Offset: 0x0008729C
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(info, this.photonView))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.isCrouching);
			stream.SendNext(this.isSprinting);
			stream.SendNext(this.isCrawling);
			stream.SendNext(this.isSliding);
			stream.SendNext(this.isMoving);
			stream.SendNext(this.isGrounded);
			stream.SendNext(this.Interact);
			stream.SendNext(this.InputDirection);
			stream.SendNext(PlayerController.instance.VelocityRelative);
			stream.SendNext(this.rbVelocityRaw);
			stream.SendNext(PlayerController.instance.transform.position);
			stream.SendNext(PlayerController.instance.transform.rotation);
			stream.SendNext(this.localCameraPosition);
			stream.SendNext(this.localCameraRotation);
			stream.SendNext(PlayerController.instance.CollisionGrounded.physRiding);
			stream.SendNext(PlayerController.instance.CollisionGrounded.physRidingID);
			stream.SendNext(PlayerController.instance.CollisionGrounded.physRidingPosition);
			stream.SendNext(this.flashlightLightAim.clientAimPoint);
			stream.SendNext(this.playerPing);
			return;
		}
		this.isCrouching = (bool)stream.ReceiveNext();
		this.isSprinting = (bool)stream.ReceiveNext();
		this.isCrawling = (bool)stream.ReceiveNext();
		this.isSliding = (bool)stream.ReceiveNext();
		this.isMoving = (bool)stream.ReceiveNext();
		this.isGrounded = (bool)stream.ReceiveNext();
		this.Interact = (bool)stream.ReceiveNext();
		this.InputDirection = (Vector3)stream.ReceiveNext();
		this.rbVelocity = (Vector3)stream.ReceiveNext();
		this.rbVelocityRaw = (Vector3)stream.ReceiveNext();
		this.clientPosition = (Vector3)stream.ReceiveNext();
		this.clientRotation = (Quaternion)stream.ReceiveNext();
		this.clientPositionDelta = Vector3.Distance(this.clientPositionCurrent, this.clientPosition);
		this.localCameraPosition = (Vector3)stream.ReceiveNext();
		this.localCameraRotation = (Quaternion)stream.ReceiveNext();
		this.clientPhysRiding = (bool)stream.ReceiveNext();
		this.clientPhysRidingID = (int)stream.ReceiveNext();
		this.clientPhysRidingPosition = (Vector3)stream.ReceiveNext();
		if (this.clientPhysRiding)
		{
			PhotonView photonView = PhotonView.Find(this.clientPhysRidingID);
			if (photonView)
			{
				this.clientPhysRidingTransform = photonView.transform;
			}
			else
			{
				this.clientPhysRiding = false;
			}
		}
		this.playerAvatarVisuals.PhysRidingCheck();
		this.flashlightLightAim.clientAimPoint = (Vector3)stream.ReceiveNext();
		this.playerPing = (int)stream.ReceiveNext();
	}

	// Token: 0x0400188B RID: 6283
	public PhotonView photonView;

	// Token: 0x0400188C RID: 6284
	public Transform playerTransform;

	// Token: 0x0400188D RID: 6285
	public Transform lowPassRaycastPoint;

	// Token: 0x0400188E RID: 6286
	public GameObject spectateCamera;

	// Token: 0x0400188F RID: 6287
	public Transform spectatePoint;

	// Token: 0x04001890 RID: 6288
	public PhysGrabber physGrabber;

	// Token: 0x04001891 RID: 6289
	public PlayerPhysPusher playerPhysPusher;

	// Token: 0x04001892 RID: 6290
	public PlayerAvatarVisuals playerAvatarVisuals;

	// Token: 0x04001893 RID: 6291
	public PlayerExpression playerExpression;

	// Token: 0x04001894 RID: 6292
	public PlayerHealth playerHealth;

	// Token: 0x04001895 RID: 6293
	public FlashlightController flashlightController;

	// Token: 0x04001896 RID: 6294
	public FlashlightLightAim flashlightLightAim;

	// Token: 0x04001897 RID: 6295
	public MapToolController mapToolController;

	// Token: 0x04001898 RID: 6296
	public PlayerDeathEffects playerDeathEffects;

	// Token: 0x04001899 RID: 6297
	public PlayerReviveEffects playerReviveEffects;

	// Token: 0x0400189A RID: 6298
	public PlayerDeathHead playerDeathHead;

	// Token: 0x0400189B RID: 6299
	public PlayerHealthGrab healthGrab;

	// Token: 0x0400189C RID: 6300
	public PlayerTumble tumble;

	// Token: 0x0400189D RID: 6301
	public PlayerPhysObjectStander physObjectStander;

	// Token: 0x0400189E RID: 6302
	public PlayerPhysObjectFinder physObjectFinder;

	// Token: 0x0400189F RID: 6303
	public GameObject deathSpot;

	// Token: 0x040018A0 RID: 6304
	private Collider collider;

	// Token: 0x040018A1 RID: 6305
	internal string playerName;

	// Token: 0x040018A2 RID: 6306
	internal string steamID;

	// Token: 0x040018A3 RID: 6307
	[Space]
	public Transform localCameraTransform;

	// Token: 0x040018A4 RID: 6308
	private Camera localCamera;

	// Token: 0x040018A5 RID: 6309
	internal Vector3 localCameraPosition = Vector3.zero;

	// Token: 0x040018A6 RID: 6310
	internal Quaternion localCameraRotation = Quaternion.identity;

	// Token: 0x040018A7 RID: 6311
	[Space]
	public PlayerVisionTarget PlayerVisionTarget;

	// Token: 0x040018A8 RID: 6312
	public RoomVolumeCheck RoomVolumeCheck;

	// Token: 0x040018A9 RID: 6313
	public Materials.MaterialTrigger MaterialTrigger;

	// Token: 0x040018AA RID: 6314
	[Space]
	internal bool isLocal;

	// Token: 0x040018AB RID: 6315
	internal bool isDisabled;

	// Token: 0x040018AC RID: 6316
	internal bool outroDone;

	// Token: 0x040018AD RID: 6317
	internal bool spawned;

	// Token: 0x040018AE RID: 6318
	private bool spawnImpulse = true;

	// Token: 0x040018AF RID: 6319
	private int spawnFrames = 3;

	// Token: 0x040018B0 RID: 6320
	private bool spawnDoneImpulse = true;

	// Token: 0x040018B1 RID: 6321
	private Vector3 spawnPosition;

	// Token: 0x040018B2 RID: 6322
	internal Quaternion spawnRotation;

	// Token: 0x040018B3 RID: 6323
	internal bool finalHeal;

	// Token: 0x040018B4 RID: 6324
	internal bool isCrouching;

	// Token: 0x040018B5 RID: 6325
	internal bool isSprinting;

	// Token: 0x040018B6 RID: 6326
	internal bool isCrawling;

	// Token: 0x040018B7 RID: 6327
	internal bool isSliding;

	// Token: 0x040018B8 RID: 6328
	internal bool isMoving;

	// Token: 0x040018B9 RID: 6329
	internal bool isGrounded;

	// Token: 0x040018BA RID: 6330
	internal bool isTumbling;

	// Token: 0x040018BB RID: 6331
	private bool Interact;

	// Token: 0x040018BC RID: 6332
	internal Vector3 InputDirection;

	// Token: 0x040018BD RID: 6333
	internal Vector3 LastNavmeshPosition;

	// Token: 0x040018BE RID: 6334
	internal float LastNavMeshPositionTimer;

	// Token: 0x040018BF RID: 6335
	internal PlayerVoiceChat voiceChat;

	// Token: 0x040018C0 RID: 6336
	internal bool voiceChatFetched;

	// Token: 0x040018C1 RID: 6337
	private Rigidbody rb;

	// Token: 0x040018C2 RID: 6338
	internal Vector3 rbVelocity;

	// Token: 0x040018C3 RID: 6339
	internal Vector3 rbVelocityRaw;

	// Token: 0x040018C4 RID: 6340
	private float rbDiscreteTimer;

	// Token: 0x040018C5 RID: 6341
	internal Vector3 clientPosition = Vector3.zero;

	// Token: 0x040018C6 RID: 6342
	internal Vector3 clientPositionCurrent = Vector3.zero;

	// Token: 0x040018C7 RID: 6343
	internal float clientPositionDelta;

	// Token: 0x040018C8 RID: 6344
	internal Quaternion clientRotation = Quaternion.identity;

	// Token: 0x040018C9 RID: 6345
	internal Quaternion clientRotationCurrent = Quaternion.identity;

	// Token: 0x040018CA RID: 6346
	public Sound jumpSound;

	// Token: 0x040018CB RID: 6347
	public Sound extraJumpSound;

	// Token: 0x040018CC RID: 6348
	public Sound landSound;

	// Token: 0x040018CD RID: 6349
	public Sound slideSound;

	// Token: 0x040018CE RID: 6350
	[Space]
	public Sound standToCrouchSound;

	// Token: 0x040018CF RID: 6351
	public Sound crouchToStandSound;

	// Token: 0x040018D0 RID: 6352
	[Space]
	public Sound crouchToCrawlSound;

	// Token: 0x040018D1 RID: 6353
	public Sound crawlToCrouchSound;

	// Token: 0x040018D2 RID: 6354
	[Space]
	public Sound deathBuildupSound;

	// Token: 0x040018D3 RID: 6355
	public Sound deathSound;

	// Token: 0x040018D4 RID: 6356
	[Space]
	public Sound tumbleStartSound;

	// Token: 0x040018D5 RID: 6357
	public Sound tumbleStopSound;

	// Token: 0x040018D6 RID: 6358
	public Sound tumbleBreakFreeSound;

	// Token: 0x040018D7 RID: 6359
	[Space]
	public Sound truckReturn;

	// Token: 0x040018D8 RID: 6360
	public Sound truckReturnGlobal;

	// Token: 0x040018D9 RID: 6361
	internal bool clientPhysRiding;

	// Token: 0x040018DA RID: 6362
	internal int clientPhysRidingID;

	// Token: 0x040018DB RID: 6363
	internal Vector3 clientPhysRidingPosition;

	// Token: 0x040018DC RID: 6364
	internal Transform clientPhysRidingTransform;

	// Token: 0x040018DD RID: 6365
	public static PlayerAvatar instance;

	// Token: 0x040018DE RID: 6366
	internal bool spectating;

	// Token: 0x040018DF RID: 6367
	internal bool deadSet;

	// Token: 0x040018E0 RID: 6368
	private float deadTime = 0.5f;

	// Token: 0x040018E1 RID: 6369
	private float deadTimer;

	// Token: 0x040018E2 RID: 6370
	internal float enemyVisionFreezeTimer;

	// Token: 0x040018E3 RID: 6371
	private Transform deadEnemyLookAtTransform;

	// Token: 0x040018E4 RID: 6372
	internal int steamIDshort;

	// Token: 0x040018E5 RID: 6373
	internal PlayerAvatarCollision playerAvatarCollision;

	// Token: 0x040018E6 RID: 6374
	internal bool fallDamageResetState;

	// Token: 0x040018E7 RID: 6375
	private bool fallDamageResetStatePrevious;

	// Token: 0x040018E8 RID: 6376
	private float fallDamageResetTimer;

	// Token: 0x040018E9 RID: 6377
	internal int playerPing;

	// Token: 0x040018EA RID: 6378
	private float playerPingTimer;

	// Token: 0x040018EB RID: 6379
	internal bool quitApplication;

	// Token: 0x040018EC RID: 6380
	private float overrrideAnimationSpeedTimer;

	// Token: 0x040018ED RID: 6381
	private float overrrideAnimationSpeedTarget;

	// Token: 0x040018EE RID: 6382
	private float overrrideAnimationSpeedIn;

	// Token: 0x040018EF RID: 6383
	private float overrrideAnimationSpeedOut;

	// Token: 0x040018F0 RID: 6384
	private float overrideAnimationSpeedLerp;

	// Token: 0x040018F1 RID: 6385
	private bool overrideAnimationSpeedActive;

	// Token: 0x040018F2 RID: 6386
	private float overrideAnimationSpeedTime;

	// Token: 0x040018F3 RID: 6387
	private SpringFloat overridePupilSizeSpring = new SpringFloat();

	// Token: 0x040018F4 RID: 6388
	private bool overridePupilSizeActive;

	// Token: 0x040018F5 RID: 6389
	private float overridePupilSizeTimer;

	// Token: 0x040018F6 RID: 6390
	private float overridePupilSizeTime;

	// Token: 0x040018F7 RID: 6391
	private float overridePupilSizeMultiplier = 1f;

	// Token: 0x040018F8 RID: 6392
	private float overridePupilSizeMultiplierTarget = 1f;

	// Token: 0x040018F9 RID: 6393
	private float overridePupilSpringSpeedIn = 15f;

	// Token: 0x040018FA RID: 6394
	private float overridePupilSpringDampIn = 0.3f;

	// Token: 0x040018FB RID: 6395
	private float overridePupilSpringSpeedOut = 15f;

	// Token: 0x040018FC RID: 6396
	private float overridePupilSpringDampOut = 0.3f;

	// Token: 0x040018FD RID: 6397
	private int overridePupilSizePrio;

	// Token: 0x040018FE RID: 6398
	internal int upgradeMapPlayerCount;

	// Token: 0x040018FF RID: 6399
	internal bool levelAnimationCompleted;

	// Token: 0x04001900 RID: 6400
	internal float upgradeCrouchRest;

	// Token: 0x04001901 RID: 6401
	internal float upgradeTumbleWings;

	// Token: 0x04001902 RID: 6402
	internal bool upgradeTumbleWingsVisualsActive;

	// Token: 0x04001903 RID: 6403
	internal WorldSpaceUIPlayerName worldSpaceUIPlayerName;

	// Token: 0x04001904 RID: 6404
	internal Dictionary<int, float> playerExpressions = new Dictionary<int, float>();

	// Token: 0x04001905 RID: 6405
	public ItemUpgradePlayerTumbleWingsLogic upgradeTumbleWingsLogic;

	// Token: 0x04001906 RID: 6406
	private bool colorWasSet;

	// Token: 0x04001907 RID: 6407
	private float noColorFailsafeTimer = 10f;
}
