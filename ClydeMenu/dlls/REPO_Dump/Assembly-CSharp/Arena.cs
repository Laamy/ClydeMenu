using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000D4 RID: 212
public class Arena : MonoBehaviour
{
	// Token: 0x06000769 RID: 1897 RVA: 0x00046B25 File Offset: 0x00044D25
	private void Awake()
	{
		Arena.instance = this;
		this.ArenaInit();
		SessionManager.instance.ResetCrown();
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x00046B3D File Offset: 0x00044D3D
	private void ArenaInit()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.ArenaInitMultiplayer();
		}
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x00046B54 File Offset: 0x00044D54
	private void ArenaInitMultiplayer()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.itemVolumes.childCount; i++)
		{
			list.Add(i);
		}
		for (int j = 0; j < this.itemVolumes.childCount; j++)
		{
			int num = Random.Range(0, list.Count);
			this.itemVolumes.GetChild(j).SetSiblingIndex(list[num]);
			list.RemoveAt(num);
		}
		this.itemsMelee.Shuffle<Item>();
		this.itemsGuns.Shuffle<Item>();
		this.itemsCarts.Shuffle<Item>();
		this.itemsDronesAndOrbs.Shuffle<Item>();
		this.itemsHealth.Shuffle<Item>();
		this.itemsUsables.Shuffle<Item>();
		this.itemsMid = new List<Item>();
		this.itemsMid.AddRange(this.itemsMelee);
		this.itemsMid.AddRange(this.itemsGuns);
		ItemManager.instance.ResetAllItems();
		for (int k = 0; k < 1; k++)
		{
			ItemManager.instance.purchasedItems.Add(this.itemsUsables[k]);
		}
		for (int l = 0; l < 5; l++)
		{
			ItemManager.instance.purchasedItems.Add(this.itemsMelee[l]);
		}
		for (int m = 0; m < 3; m++)
		{
			if (Random.Range(0, 100) < 30)
			{
				ItemManager.instance.purchasedItems.Add(this.itemsGuns[m]);
			}
		}
		for (int n = 0; n < 1; n++)
		{
			if (Random.Range(0, 100) < 30)
			{
				ItemManager.instance.purchasedItems.Add(this.itemsCarts[n]);
			}
		}
		for (int num2 = 0; num2 < 3; num2++)
		{
			if (Random.Range(0, 100) < 30)
			{
				ItemManager.instance.purchasedItems.Add(this.itemsDronesAndOrbs[num2]);
			}
		}
		for (int num3 = 0; num3 < 3; num3++)
		{
			if (Random.Range(0, 100) < 30)
			{
				ItemManager.instance.purchasedItems.Add(this.itemsHealth[num3]);
			}
		}
		ItemManager.instance.GetAllItemVolumesInScene();
		PunManager.instance.TruckPopulateItemVolumes();
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x00046D8C File Offset: 0x00044F8C
	private void Start()
	{
		if (this.crownTransform)
		{
			this.crownTransformPosition = this.crownTransform.position;
		}
		else
		{
			this.crownTransformPosition = Vector3.zero;
		}
		this.numberOfPlayers = SemiFunc.PlayerGetAll().Count;
		this.photonView = base.GetComponent<PhotonView>();
		this.platforms = new List<ArenaPlatform>();
		this.platforms.AddRange(base.GetComponentsInChildren<ArenaPlatform>());
		this.floorDoorStartPos = this.floorDoorTransform.localPosition;
		this.floorDoorEndPos = new Vector3(this.floorDoorStartPos.x, this.floorDoorStartPos.y, 8.25f);
		this.startPosCrownMechanicLineTransform = this.crownMechanicLineTransform.localPosition.y;
		this.playersAlive = SemiFunc.PlayerGetAll().Count;
		this.playersAlivePrev = this.playersAlive;
		this.pedistalScreens = new List<ArenaPedistalScreen>();
		this.pedistalScreens.AddRange(base.GetComponentsInChildren<ArenaPedistalScreen>());
		foreach (ArenaPedistalScreen arenaPedistalScreen in this.pedistalScreens)
		{
			arenaPedistalScreen.SwitchNumber(this.playersAlive, false);
		}
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x00046ECC File Offset: 0x000450CC
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x00046EE0 File Offset: 0x000450E0
	private void StateFalling()
	{
		if (this.stateStart)
		{
			this.pipeLights.SetActive(true);
			this.stateStart = false;
			this.hurtCollider.SetActive(true);
			this.stateTimer = 5f;
			this.soundArenaHatchOpen.Play(this.floorDoorTransform.position, 1f, 1f, 1f, 1f);
			CameraGlitch.Instance.PlayLong();
			GameDirector.instance.CameraShake.Shake(4f, 0.25f);
			GameDirector.instance.CameraImpact.Shake(4f, 0.1f);
			this.musicToggle = true;
			this.musicSource.time = 0f;
			if (this.numberOfPlayers > 1 && SemiFunc.IsMultiplayer())
			{
				foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetAll())
				{
					playerAvatar.playerHealth.InvincibleSet(5f);
				}
			}
			if (this.numberOfPlayers < 2 || !SemiFunc.IsMultiplayer())
			{
				foreach (ArenaPlatform arenaPlatform in this.platforms)
				{
					arenaPlatform.StateSet(ArenaPlatform.States.GoDown);
				}
			}
		}
		if (this.numberOfPlayers > 1 && SemiFunc.IsMultiplayer())
		{
			ArenaMessageUI.instance.ArenaText("LAST LOSER STANDING");
		}
		else
		{
			ArenaMessageUI.instance.ArenaText("GAME OVER");
		}
		if (SemiFunc.FPSImpulse5())
		{
			foreach (PlayerAvatar playerAvatar2 in SemiFunc.PlayerGetAll())
			{
				playerAvatar2.FallDamageResetSet(1f);
			}
		}
		this.floorDoorAnimationProgress += Time.deltaTime * 2f;
		if (this.floorDoorAnimationProgress >= 1f)
		{
			this.floorDoorAnimationProgress = 1f;
		}
		this.floorDoorTransform.localPosition = Vector3.Lerp(this.floorDoorStartPos, this.floorDoorEndPos, this.floorDoorCurve.Evaluate(this.floorDoorAnimationProgress));
		if (this.stateTimer <= 0f)
		{
			if (this.numberOfPlayers > 1 && SemiFunc.IsMultiplayer())
			{
				this.StateSet(Arena.States.Starting);
				return;
			}
			this.StateSet(Arena.States.GameOver);
		}
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x00047150 File Offset: 0x00045350
	private void StateStarting()
	{
		if (this.stateStart)
		{
			this.pipeLights.SetActive(false);
			this.stateStart = false;
			this.hurtCollider.SetActive(false);
			this.safeBars.SetActive(false);
			this.stateTimer = 2f;
		}
		if (this.stateTimer <= 0f)
		{
			this.StateSet(Arena.States.Level1);
		}
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x000471B0 File Offset: 0x000453B0
	private void StatePlatformWarning()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			return;
		}
		if (this.stateStart)
		{
			this.level++;
			this.nextLevel = this.level + Arena.States.Level1;
			this.stateStart = false;
			int num = this.level - 1;
			this.platforms[num].StateSet(ArenaPlatform.States.Warning);
			this.platforms[num].PulsateLights();
			this.soundArenaWarning.Play(base.transform.position, 1f, 1f, 1f, 1f);
			GameDirector.instance.CameraShake.Shake(4f, 0.25f);
			GameDirector.instance.CameraImpact.Shake(4f, 0.1f);
			this.stateTimer = 3f;
		}
		if (this.stateTimer % 1f < 0.1f)
		{
			if (!this.warningSound)
			{
				this.warningSound = true;
				if (this.stateTimer > 1f)
				{
					this.soundArenaWarning.Play(base.transform.position, 1f, 1f, 1f, 1f);
					GameDirector.instance.CameraShake.Shake(2f, 0.25f);
					GameDirector.instance.CameraImpact.Shake(2f, 0.1f);
				}
				int num2 = this.level - 1;
				this.platforms[num2].PulsateLights();
			}
		}
		else
		{
			this.warningSound = false;
		}
		if (this.stateTimer <= 0f)
		{
			this.StateSet(Arena.States.PlatformRemove);
		}
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x00047350 File Offset: 0x00045550
	private void StatePlatformRemove()
	{
		if (this.stateStart)
		{
			int num = this.level - 1;
			this.platforms[num].StateSet(ArenaPlatform.States.GoDown);
			this.stateStart = false;
			this.soundArenaRemove.Play(base.transform.position, 1f, 1f, 1f, 1f);
			GameDirector.instance.CameraShake.Shake(8f, 0.5f);
			GameDirector.instance.CameraImpact.Shake(8f, 0.1f);
			this.stateTimer = 3f;
		}
		if (this.stateTimer <= 0f && !this.finalPlatform)
		{
			this.StateSet(this.nextLevel);
		}
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x00047413 File Offset: 0x00045613
	private void StateLevel1()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.stateTimer = 30f;
		}
		if (this.stateTimer <= 0f)
		{
			this.NextLevel();
		}
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x00047442 File Offset: 0x00045642
	private void StateLevel2()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.stateTimer = 30f;
		}
		if (this.stateTimer <= 0f)
		{
			this.NextLevel();
		}
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x00047471 File Offset: 0x00045671
	private void StateLevel3()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.stateTimer = 30f;
		}
		if (this.stateTimer <= 0f)
		{
			this.finalPlatform = true;
			this.NextLevel();
		}
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x000474A8 File Offset: 0x000456A8
	private void StateGameOver()
	{
		if (this.stateStart)
		{
			this.musicToggle = false;
			this.stateStart = false;
			if (this.numberOfPlayers > 1 && SemiFunc.IsMultiplayer())
			{
				this.stateTimer = 10f;
			}
			else
			{
				this.stateTimer = 3f;
			}
			if (!this.winnerPlayer)
			{
				this.soundArenaMusicLoseJingle.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		if (this.winnerPlayer)
		{
			ArenaMessageWinUI.instance.ArenaText("KING OF THE LOSERS!", true);
			if (this.crownMesh)
			{
				this.crownMesh.enabled = false;
			}
		}
		else if (this.numberOfPlayers > 1 && SemiFunc.IsMultiplayer())
		{
			ArenaMessageWinUI.instance.ArenaText("EVERYONE'S A LOSER!", false);
		}
		if (this.stateTimer <= 0f && SemiFunc.IsMasterClientOrSingleplayer())
		{
			RunManager.instance.ChangeLevel(false, true, RunManager.ChangeLevelType.Normal);
		}
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x000475A4 File Offset: 0x000457A4
	private void GameOver()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x000475B5 File Offset: 0x000457B5
	private void NextLevel()
	{
		this.StateSet(Arena.States.PlatformWarning);
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x000475C0 File Offset: 0x000457C0
	private void StateMachine()
	{
		switch (this.currentState)
		{
		case Arena.States.Idle:
			this.StateIdle();
			return;
		case Arena.States.Level1:
			this.StateLevel1();
			return;
		case Arena.States.Level2:
			this.StateLevel2();
			return;
		case Arena.States.Level3:
			this.StateLevel3();
			return;
		case Arena.States.Falling:
			this.StateFalling();
			return;
		case Arena.States.Starting:
			this.StateStarting();
			return;
		case Arena.States.PlatformWarning:
			this.StatePlatformWarning();
			return;
		case Arena.States.PlatformRemove:
			this.StatePlatformRemove();
			return;
		case Arena.States.GameOver:
			this.StateGameOver();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00047640 File Offset: 0x00045840
	private void Update()
	{
		this.StateMachine();
		if (this.stateTimer > 0f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		SemiFunc.UIHideCurrency();
		SemiFunc.UIHideInventory();
		SemiFunc.UIHideHaul();
		SemiFunc.UIHideGoal();
		this.MusicLogic();
		if (this.numberOfPlayers > 1 && SemiFunc.IsMultiplayer())
		{
			this.CrownVisuals();
			this.CrownLogic();
			this.MainLightAnimation();
			this.SpawnMidWeapons();
		}
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x000476B4 File Offset: 0x000458B4
	private void MusicLogic()
	{
		this.soundArenaMusic.PlayLoop(this.musicToggle, 20f, 2f, 1f);
		if (!this.musicTogglePrev && this.musicToggle)
		{
			this.soundArenaMusic.Source.time = 0f;
			this.musicTogglePrev = this.musicToggle;
		}
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x00047714 File Offset: 0x00045914
	private void SpawnMidWeapons()
	{
		if (SemiFunc.IsMultiplayer() && SemiFunc.IsMasterClient() && this.playersAlive > 1 && this.level >= 2 && this.currentState != Arena.States.GameOver)
		{
			if (this.midSpawnerTimer > 5f)
			{
				Item item = this.itemsMid[Random.Range(0, this.itemsMid.Count)];
				PhotonNetwork.Instantiate("Items/" + item.itemAssetName, this.itemsMidSpawner.position, this.itemsMidSpawner.rotation, 0, null);
				this.midSpawnerTimer = 0f;
				return;
			}
			this.midSpawnerTimer += Time.deltaTime;
		}
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x000477CC File Offset: 0x000459CC
	private void MainLightAnimation()
	{
		if (!this.arenaMainLight.enabled)
		{
			return;
		}
		if (this.arenaMainLight.intensity > 0.05f)
		{
			this.arenaMainLight.intensity = Mathf.Lerp(this.arenaMainLight.intensity, 0f, Time.deltaTime * 2f);
			return;
		}
		this.arenaMainLight.enabled = false;
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x00047834 File Offset: 0x00045A34
	private void CrownLogic()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.currentState == Arena.States.GameOver)
		{
			return;
		}
		List<PlayerAvatar> list = SemiFunc.PlayerGetAll();
		int count = list.Count;
		this.playersAlive = count;
		using (List<PlayerAvatar>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isDisabled)
				{
					this.playersAlive--;
				}
			}
		}
		if (this.playersAlivePrev != this.playersAlive)
		{
			if ((this.playersAlive > 1 || this.playersAlive == 0) && SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("PlayerKilledRPC", RpcTarget.All, new object[]
				{
					this.playersAlive
				});
			}
			this.playersAlivePrev = this.playersAlive;
		}
		if (this.playersAlive <= 0)
		{
			this.StateSet(Arena.States.GameOver);
			return;
		}
		if (SemiFunc.FPSImpulse15() && !this.crownCageDestroyed && this.playersAlive < 2)
		{
			this.DestroyCrownCage();
			this.crownCageDestroyed = true;
		}
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00047940 File Offset: 0x00045B40
	private void AllPlayersKilled()
	{
		this.soundAllPlayersDead.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.arenaMainLight.color = new Color(0f, 1f, 0f);
		this.arenaMainLight.enabled = true;
		this.arenaMainLight.intensity = 10f;
		foreach (ArenaPedistalScreen arenaPedistalScreen in this.pedistalScreens)
		{
			arenaPedistalScreen.SwitchNumber(1, true);
		}
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x000479F8 File Offset: 0x00045BF8
	[PunRPC]
	private void PlayerKilledRPC(int _playersAlive, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.soundArenaPlayerEliminated.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.arenaMainLight.color = new Color(0.8f, 0.3f, 0f);
		this.arenaMainLight.enabled = true;
		this.arenaMainLight.intensity = 8f;
		this.playersAlive = _playersAlive;
		foreach (ArenaPedistalScreen arenaPedistalScreen in this.pedistalScreens)
		{
			arenaPedistalScreen.SwitchNumber(this.playersAlive, false);
		}
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x00047AC8 File Offset: 0x00045CC8
	private void CrownVisuals()
	{
		if (this.currentState == Arena.States.GameOver)
		{
			return;
		}
		if (!this.crownTransform)
		{
			return;
		}
		this.crownTransform.Rotate(Vector3.up, Time.deltaTime * 50f);
		this.crownTransform.localRotation = Quaternion.Euler(this.crownTransform.localRotation.x, this.crownTransform.localRotation.y + Time.time * 50f, 20f * Mathf.Sin(Time.time * 2f));
		if (this.crownCageDestroyed)
		{
			return;
		}
		this.crownSphere.material.mainTextureOffset = new Vector2(0f, Time.time * 0.1f);
		this.crownMechanic.Rotate(Vector3.up, Time.deltaTime * 500f);
		float num = 0.05f;
		float num2 = 2f;
		this.crownMechanicLineTransform.localPosition = new Vector3(this.crownMechanicLineTransform.localPosition.x, this.startPosCrownMechanicLineTransform - num / 2f + num * Mathf.Sin(Time.time * num2), this.crownMechanicLineTransform.localPosition.z);
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x00047BFC File Offset: 0x00045DFC
	public void StateSet(Arena.States state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.StateSetRPC(state, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("StateSetRPC", RpcTarget.All, new object[]
		{
			state
		});
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x00047C49 File Offset: 0x00045E49
	[PunRPC]
	public void StateSetRPC(Arena.States state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x00047C64 File Offset: 0x00045E64
	private void DestroyCrownCage()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("DestroyCrownCageRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.DestroyCrownCageRPC(default(PhotonMessageInfo));
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x00047CA0 File Offset: 0x00045EA0
	public void CrownGrab()
	{
		if (SemiFunc.IsMasterClient())
		{
			int viewID = this.crownGrab.playerGrabbing[0].photonView.ViewID;
			this.photonView.RPC("CrownGrabRPC", RpcTarget.All, new object[]
			{
				viewID
			});
		}
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x00047CF0 File Offset: 0x00045EF0
	[PunRPC]
	public void CrownGrabRPC(int photonViewID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (this.winnerPlayer)
		{
			return;
		}
		PhysGrabber component = PhotonView.Find(photonViewID).GetComponent<PhysGrabber>();
		this.winnerPlayer = component.playerAvatar;
		SessionManager.instance.crownedPlayerSteamID = this.winnerPlayer.steamID;
		ArenaMessageWinUI.instance.kingObject.GetComponent<MenuPlayerListed>().ForcePlayer(this.winnerPlayer);
		this.crownExplosion.SetActive(true);
		this.soundArenaMusicWinJingle.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x00047D94 File Offset: 0x00045F94
	[PunRPC]
	public void DestroyCrownCageRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (this.crownCageDestroyed)
		{
			return;
		}
		this.musicToggle = false;
		this.AllPlayersKilled();
		this.soundCrownCageDestroy.Play(this.crownTransformPosition, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(10f, 0.5f);
		GameDirector.instance.CameraImpact.Shake(10f, 0.1f);
		if (this.crownCageDestroyParticles)
		{
			this.crownCageDestroyParticles.SetActive(true);
		}
		if (this.crownSphere)
		{
			Object.Destroy(this.crownSphere.gameObject);
		}
		if (this.crownMechanic)
		{
			Object.Destroy(this.crownMechanic.gameObject);
		}
		this.crownCageDestroyed = true;
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x00047E72 File Offset: 0x00046072
	public void OpenHatch()
	{
		this.StateSet(Arena.States.Falling);
	}

	// Token: 0x04000D1A RID: 3354
	private PhotonView photonView;

	// Token: 0x04000D1B RID: 3355
	private List<ArenaPlatform> platforms;

	// Token: 0x04000D1C RID: 3356
	public static Arena instance;

	// Token: 0x04000D1D RID: 3357
	[Space(10f)]
	[Header("Items")]
	public List<Item> itemsUsables;

	// Token: 0x04000D1E RID: 3358
	public List<Item> itemsMelee;

	// Token: 0x04000D1F RID: 3359
	public List<Item> itemsGuns;

	// Token: 0x04000D20 RID: 3360
	public List<Item> itemsCarts;

	// Token: 0x04000D21 RID: 3361
	public List<Item> itemsDronesAndOrbs;

	// Token: 0x04000D22 RID: 3362
	public List<Item> itemsHealth;

	// Token: 0x04000D23 RID: 3363
	private List<Item> itemsMid;

	// Token: 0x04000D24 RID: 3364
	[Space(10f)]
	public Transform floorDoorTransform;

	// Token: 0x04000D25 RID: 3365
	public GameObject hurtCollider;

	// Token: 0x04000D26 RID: 3366
	public GameObject startStuff;

	// Token: 0x04000D27 RID: 3367
	public Transform pipeTransform;

	// Token: 0x04000D28 RID: 3368
	public GameObject safeBars;

	// Token: 0x04000D29 RID: 3369
	public AnimationCurve floorDoorCurve;

	// Token: 0x04000D2A RID: 3370
	public Transform itemVolumes;

	// Token: 0x04000D2B RID: 3371
	public MeshRenderer crownSphere;

	// Token: 0x04000D2C RID: 3372
	public MeshRenderer crownMesh;

	// Token: 0x04000D2D RID: 3373
	public Transform crownTransform;

	// Token: 0x04000D2E RID: 3374
	public GameObject crownPlatform;

	// Token: 0x04000D2F RID: 3375
	public Transform crownMechanic;

	// Token: 0x04000D30 RID: 3376
	public Transform crownMechanicLineTransform;

	// Token: 0x04000D31 RID: 3377
	public GameObject crownCageDestroyParticles;

	// Token: 0x04000D32 RID: 3378
	public StaticGrabObject crownGrab;

	// Token: 0x04000D33 RID: 3379
	public GameObject crownExplosion;

	// Token: 0x04000D34 RID: 3380
	public Transform itemsMidSpawner;

	// Token: 0x04000D35 RID: 3381
	private Vector3 crownTransformPosition;

	// Token: 0x04000D36 RID: 3382
	private bool stateStart;

	// Token: 0x04000D37 RID: 3383
	private float stateTimer;

	// Token: 0x04000D38 RID: 3384
	private Vector3 floorDoorStartPos;

	// Token: 0x04000D39 RID: 3385
	private Vector3 floorDoorEndPos;

	// Token: 0x04000D3A RID: 3386
	private float floorDoorAnimationProgress;

	// Token: 0x04000D3B RID: 3387
	private float startPosCrownMechanicLineTransform;

	// Token: 0x04000D3C RID: 3388
	private float midSpawnerTimer;

	// Token: 0x04000D3D RID: 3389
	private List<ArenaPedistalScreen> pedistalScreens;

	// Token: 0x04000D3E RID: 3390
	internal PlayerAvatar winnerPlayer;

	// Token: 0x04000D3F RID: 3391
	internal Arena.States currentState;

	// Token: 0x04000D40 RID: 3392
	internal Arena.States nextLevel = Arena.States.Level1;

	// Token: 0x04000D41 RID: 3393
	private int level;

	// Token: 0x04000D42 RID: 3394
	private bool warningSound;

	// Token: 0x04000D43 RID: 3395
	private AudioClip soundWarning;

	// Token: 0x04000D44 RID: 3396
	private bool finalPlatform;

	// Token: 0x04000D45 RID: 3397
	private bool crownCageDestroyed;

	// Token: 0x04000D46 RID: 3398
	private int numberOfPlayers;

	// Token: 0x04000D47 RID: 3399
	private bool musicToggle;

	// Token: 0x04000D48 RID: 3400
	private bool musicTogglePrev;

	// Token: 0x04000D49 RID: 3401
	public AudioSource musicSource;

	// Token: 0x04000D4A RID: 3402
	private int playersAlive = 6;

	// Token: 0x04000D4B RID: 3403
	private int playersAlivePrev = 6;

	// Token: 0x04000D4C RID: 3404
	public GameObject pipeLights;

	// Token: 0x04000D4D RID: 3405
	public Light arenaMainLight;

	// Token: 0x04000D4E RID: 3406
	public Sound soundArenaStart;

	// Token: 0x04000D4F RID: 3407
	public Sound soundArenaWarning;

	// Token: 0x04000D50 RID: 3408
	public Sound soundArenaRemove;

	// Token: 0x04000D51 RID: 3409
	public Sound soundArenaPlayerEliminated;

	// Token: 0x04000D52 RID: 3410
	public Sound soundArenaHatchOpen;

	// Token: 0x04000D53 RID: 3411
	public Sound soundArenaMusic;

	// Token: 0x04000D54 RID: 3412
	public Sound soundArenaMusicWinJingle;

	// Token: 0x04000D55 RID: 3413
	public Sound soundArenaMusicLoseJingle;

	// Token: 0x04000D56 RID: 3414
	public Sound soundCrownCageDestroy;

	// Token: 0x04000D57 RID: 3415
	public Sound soundAllPlayersDead;

	// Token: 0x0200033A RID: 826
	public enum States
	{
		// Token: 0x040029C1 RID: 10689
		Idle,
		// Token: 0x040029C2 RID: 10690
		Level1,
		// Token: 0x040029C3 RID: 10691
		Level2,
		// Token: 0x040029C4 RID: 10692
		Level3,
		// Token: 0x040029C5 RID: 10693
		Falling,
		// Token: 0x040029C6 RID: 10694
		Starting,
		// Token: 0x040029C7 RID: 10695
		PlatformWarning,
		// Token: 0x040029C8 RID: 10696
		PlatformRemove,
		// Token: 0x040029C9 RID: 10697
		GameOver
	}
}
