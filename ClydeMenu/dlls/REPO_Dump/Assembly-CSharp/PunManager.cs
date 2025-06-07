using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

// Token: 0x02000171 RID: 369
public class PunManager : MonoBehaviour
{
	// Token: 0x06000C6A RID: 3178 RVA: 0x0006E198 File Offset: 0x0006C398
	private void Awake()
	{
		PunManager.instance = this;
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x0006E1A0 File Offset: 0x0006C3A0
	private void Start()
	{
		this.statsManager = StatsManager.instance;
		this.shopManager = ShopManager.instance;
		this.itemManager = ItemManager.instance;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x0006E1D0 File Offset: 0x0006C3D0
	public void SetItemName(string name, ItemAttributes itemAttributes, int photonViewID)
	{
		if (photonViewID == -1)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("SetItemNameRPC", RpcTarget.All, new object[]
			{
				name,
				photonViewID
			});
			return;
		}
		this.SetItemNameLOGIC(name, photonViewID, itemAttributes);
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x0006E220 File Offset: 0x0006C420
	private void SetItemNameLOGIC(string name, int photonViewID, ItemAttributes _itemAttributes = null)
	{
		if (photonViewID == -1 && SemiFunc.IsMultiplayer())
		{
			return;
		}
		ItemAttributes itemAttributes = _itemAttributes;
		if (SemiFunc.IsMultiplayer())
		{
			itemAttributes = PhotonView.Find(photonViewID).GetComponent<ItemAttributes>();
		}
		if (_itemAttributes == null && !SemiFunc.IsMultiplayer())
		{
			return;
		}
		itemAttributes.instanceName = name;
		ItemBattery component = itemAttributes.GetComponent<ItemBattery>();
		if (component)
		{
			component.SetBatteryLife(this.statsManager.itemStatBattery[name]);
		}
		ItemEquippable component2 = itemAttributes.GetComponent<ItemEquippable>();
		if (component2)
		{
			int spot = 0;
			List<PlayerAvatar> list = SemiFunc.PlayerGetList();
			int hashCode = name.GetHashCode();
			bool flag = false;
			PlayerAvatar playerAvatar = null;
			foreach (PlayerAvatar playerAvatar2 in list)
			{
				string steamID = playerAvatar2.steamID;
				if (StatsManager.instance.playerInventorySpot1[steamID] == hashCode && StatsManager.instance.playerInventorySpot1Taken[steamID] == 0)
				{
					spot = 0;
					flag = true;
					playerAvatar = playerAvatar2;
					StatsManager.instance.playerInventorySpot1Taken[steamID] = 1;
					break;
				}
				if (StatsManager.instance.playerInventorySpot2[steamID] == hashCode && StatsManager.instance.playerInventorySpot2Taken[steamID] == 0)
				{
					spot = 1;
					flag = true;
					playerAvatar = playerAvatar2;
					StatsManager.instance.playerInventorySpot2Taken[steamID] = 1;
					break;
				}
				if (StatsManager.instance.playerInventorySpot3[steamID] == hashCode && StatsManager.instance.playerInventorySpot3Taken[steamID] == 0)
				{
					spot = 2;
					flag = true;
					playerAvatar = playerAvatar2;
					StatsManager.instance.playerInventorySpot3Taken[steamID] = 1;
					break;
				}
			}
			if (flag)
			{
				int requestingPlayerId = -1;
				if (SemiFunc.IsMultiplayer())
				{
					requestingPlayerId = playerAvatar.photonView.ViewID;
				}
				component2.RequestEquip(spot, requestingPlayerId);
			}
		}
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x0006E3F4 File Offset: 0x0006C5F4
	public void CrownPlayerSync(string _steamID)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("CrownPlayerRPC", RpcTarget.AllBuffered, new object[]
			{
				_steamID
			});
		}
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x0006E420 File Offset: 0x0006C620
	[PunRPC]
	public void CrownPlayerRPC(string _steamID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		SessionManager.instance.crownedPlayerSteamID = _steamID;
		PlayerCrownSet component = Object.Instantiate<GameObject>(SessionManager.instance.crownPrefab).GetComponent<PlayerCrownSet>();
		component.crownOwnerFetched = true;
		component.crownOwnerSteamID = _steamID;
		StatsManager.instance.UpdateCrown(_steamID);
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x0006E46D File Offset: 0x0006C66D
	[PunRPC]
	public void SetItemNameRPC(string name, int photonViewID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.SetItemNameLOGIC(name, photonViewID, null);
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x0006E484 File Offset: 0x0006C684
	public void ShopUpdateCost()
	{
		int num = 0;
		List<ItemAttributes> list = new List<ItemAttributes>();
		foreach (ItemAttributes itemAttributes in ShopManager.instance.shoppingList)
		{
			if (itemAttributes)
			{
				itemAttributes.roomVolumeCheck.CheckSet();
				if (!itemAttributes.roomVolumeCheck.inExtractionPoint)
				{
					list.Add(itemAttributes);
				}
				else
				{
					num += itemAttributes.value;
				}
			}
			else
			{
				list.Add(itemAttributes);
			}
		}
		foreach (ItemAttributes itemAttributes2 in list)
		{
			ShopManager.instance.shoppingList.Remove(itemAttributes2);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("UpdateShoppingCostRPC", RpcTarget.All, new object[]
				{
					num
				});
				return;
			}
			this.UpdateShoppingCostRPC(num, default(PhotonMessageInfo));
		}
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x0006E5A0 File Offset: 0x0006C7A0
	private void Update()
	{
		if (SemiFunc.FPSImpulse5() && SemiFunc.IsMultiplayer() && SemiFunc.IsMasterClient() && this.totalHaul != RoundDirector.instance.totalHaul)
		{
			this.totalHaul = RoundDirector.instance.totalHaul;
			this.photonView.RPC("SyncHaul", RpcTarget.Others, new object[]
			{
				this.totalHaul
			});
		}
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x0006E609 File Offset: 0x0006C809
	[PunRPC]
	public void SyncHaul(int value, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		RoundDirector.instance.totalHaul = value;
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x0006E61F File Offset: 0x0006C81F
	[PunRPC]
	public void UpdateShoppingCostRPC(int value, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		ShopManager.instance.totalCost = value;
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x0006E638 File Offset: 0x0006C838
	public void ShopPopulateItemVolumes()
	{
		if (SemiFunc.IsNotMasterClient())
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (KeyValuePair<SemiFunc.itemSecretShopType, List<ItemVolume>> keyValuePair in ShopManager.instance.secretItemVolumes)
		{
			List<ItemVolume> value = keyValuePair.Value;
			foreach (ItemVolume itemVolume in value)
			{
				if (ShopManager.instance.potentialSecretItems.ContainsKey(keyValuePair.Key))
				{
					List<Item> list = ShopManager.instance.potentialSecretItems[keyValuePair.Key];
					if (Random.Range(0, 3) == 0 && itemVolume)
					{
						this.SpawnShopItem(itemVolume, ShopManager.instance.potentialSecretItems[keyValuePair.Key], ref num, true);
					}
				}
			}
			foreach (ItemVolume itemVolume2 in value)
			{
				if (itemVolume2)
				{
					Object.Destroy(itemVolume2.gameObject);
				}
			}
		}
		foreach (ItemVolume itemVolume3 in this.shopManager.itemVolumes)
		{
			if (this.shopManager.potentialItems.Count == 0 && this.shopManager.potentialItemConsumables.Count == 0)
			{
				break;
			}
			if ((num >= this.shopManager.itemSpawnTargetAmount || !this.SpawnShopItem(itemVolume3, this.shopManager.potentialItems, ref num, false)) && (num2 >= this.shopManager.itemConsumablesAmount || !this.SpawnShopItem(itemVolume3, this.shopManager.potentialItemConsumables, ref num2, false)))
			{
				if (num3 < this.shopManager.itemUpgradesAmount)
				{
					this.SpawnShopItem(itemVolume3, this.shopManager.potentialItemUpgrades, ref num3, false);
				}
				if (num4 < this.shopManager.itemHealthPacksAmount)
				{
					this.SpawnShopItem(itemVolume3, this.shopManager.potentialItemHealthPacks, ref num4, false);
				}
			}
		}
		foreach (ItemVolume itemVolume4 in this.shopManager.itemVolumes)
		{
			Object.Destroy(itemVolume4.gameObject);
		}
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x0006E8E8 File Offset: 0x0006CAE8
	private bool SpawnShopItem(ItemVolume itemVolume, List<Item> itemList, ref int spawnCount, bool isSecret = false)
	{
		for (int i = itemList.Count - 1; i >= 0; i--)
		{
			Item item = itemList[i];
			if (item.itemVolume == itemVolume.itemVolume)
			{
				ShopManager.instance.itemRotateHelper.transform.parent = itemVolume.transform;
				ShopManager.instance.itemRotateHelper.transform.localRotation = item.spawnRotationOffset;
				Quaternion rotation = ShopManager.instance.itemRotateHelper.transform.rotation;
				ShopManager.instance.itemRotateHelper.transform.parent = ShopManager.instance.transform;
				string prefabName = "Items/" + item.prefab.name;
				if (SemiFunc.IsMultiplayer())
				{
					PhotonNetwork.InstantiateRoomObject(prefabName, itemVolume.transform.position, rotation, 0, null);
				}
				else
				{
					Object.Instantiate<GameObject>(item.prefab, itemVolume.transform.position, rotation);
				}
				itemList.RemoveAt(i);
				if (!isSecret)
				{
					spawnCount++;
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x0006E9F0 File Offset: 0x0006CBF0
	public void TruckPopulateItemVolumes()
	{
		ItemManager.instance.spawnedItems.Clear();
		if (SemiFunc.IsNotMasterClient())
		{
			return;
		}
		List<ItemVolume> list = new List<ItemVolume>(this.itemManager.itemVolumes);
		List<Item> list2 = new List<Item>(this.itemManager.purchasedItems);
		while (list.Count > 0 && list2.Count > 0)
		{
			bool flag = false;
			for (int i = 0; i < list2.Count; i++)
			{
				Item item = list2[i];
				ItemVolume itemVolume = list.Find((ItemVolume v) => v.itemVolume == item.itemVolume);
				if (itemVolume)
				{
					this.SpawnItem(item, itemVolume);
					list.Remove(itemVolume);
					list2.RemoveAt(i);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
		}
		foreach (ItemVolume itemVolume2 in this.itemManager.itemVolumes)
		{
			Object.Destroy(itemVolume2.gameObject);
		}
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x0006EB04 File Offset: 0x0006CD04
	private void SpawnItem(Item item, ItemVolume volume)
	{
		ShopManager.instance.itemRotateHelper.transform.parent = volume.transform;
		ShopManager.instance.itemRotateHelper.transform.localRotation = item.spawnRotationOffset;
		Quaternion rotation = ShopManager.instance.itemRotateHelper.transform.rotation;
		ShopManager.instance.itemRotateHelper.transform.parent = ShopManager.instance.transform;
		if (SemiFunc.IsMasterClient())
		{
			PhotonNetwork.InstantiateRoomObject("Items/" + item.prefab.name, volume.transform.position, rotation, 0, null);
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			Object.Instantiate<GameObject>(item.prefab, volume.transform.position, rotation);
		}
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x0006EBC8 File Offset: 0x0006CDC8
	public void AddingItem(string itemName, int index, int photonViewID, ItemAttributes itemAttributes)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("AddingItemRPC", RpcTarget.All, new object[]
			{
				itemName,
				index,
				photonViewID
			});
			return;
		}
		this.AddingItemLOGIC(itemName, index, photonViewID, itemAttributes);
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x0006EC18 File Offset: 0x0006CE18
	private void AddingItemLOGIC(string itemName, int index, int photonViewID, ItemAttributes itemAttributes = null)
	{
		if (!StatsManager.instance.item.ContainsKey(itemName))
		{
			StatsManager.instance.item.Add(itemName, index);
			StatsManager.instance.itemStatBattery.Add(itemName, 100);
			StatsManager.instance.takenItemNames.Add(itemName);
		}
		else
		{
			Debug.LogWarning("Item " + itemName + " already exists in the dictionary");
		}
		this.SetItemNameLOGIC(itemName, photonViewID, itemAttributes);
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x0006EC8B File Offset: 0x0006CE8B
	[PunRPC]
	public void AddingItemRPC(string itemName, int index, int photonViewID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.AddingItemLOGIC(itemName, index, photonViewID, null);
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x0006ECA4 File Offset: 0x0006CEA4
	public void UpdateStat(string dictionaryName, string key, int value)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("UpdateStatRPC", RpcTarget.All, new object[]
			{
				dictionaryName,
				key,
				value
			});
			return;
		}
		this.UpdateStatRPC(dictionaryName, key, value, default(PhotonMessageInfo));
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x0006ECF3 File Offset: 0x0006CEF3
	[PunRPC]
	public void UpdateStatRPC(string dictionaryName, string key, int value, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		StatsManager.instance.DictionaryUpdateValue(dictionaryName, key, value);
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x0006ED0C File Offset: 0x0006CF0C
	public int SetRunStatSet(string statName, int value)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.statsManager.runStats[statName] = value;
				this.photonView.RPC("SetRunStatRPC", RpcTarget.Others, new object[]
				{
					statName,
					value
				});
			}
			else
			{
				this.statsManager.runStats[statName] = value;
			}
		}
		return this.statsManager.runStats[statName];
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x0006ED82 File Offset: 0x0006CF82
	[PunRPC]
	public void SetRunStatRPC(string statName, int value, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.statsManager.runStats[statName] = value;
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x0006EDA0 File Offset: 0x0006CFA0
	public int UpgradeItemBattery(string itemName)
	{
		Dictionary<string, int> itemBatteryUpgrades = this.statsManager.itemBatteryUpgrades;
		itemBatteryUpgrades[itemName]++;
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpgradeItemBatteryRPC", RpcTarget.Others, new object[]
			{
				itemName,
				this.statsManager.itemBatteryUpgrades[itemName]
			});
		}
		return this.statsManager.itemBatteryUpgrades[itemName];
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x0006EE17 File Offset: 0x0006D017
	[PunRPC]
	public void UpgradeItemBatteryRPC(string itemName, int value, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.statsManager.itemBatteryUpgrades[itemName] = value;
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x0006EE34 File Offset: 0x0006D034
	public int UpgradePlayerHealth(string _steamID)
	{
		Dictionary<string, int> playerUpgradeHealth = this.statsManager.playerUpgradeHealth;
		playerUpgradeHealth[_steamID]++;
		this.UpdateHealthRightAway(_steamID);
		return this.statsManager.playerUpgradeHealth[_steamID];
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x0006EE78 File Offset: 0x0006D078
	private void UpdateHealthRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (playerAvatar == SemiFunc.PlayerAvatarLocal())
		{
			playerAvatar.playerHealth.maxHealth += 20;
			playerAvatar.playerHealth.Heal(20, false);
		}
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x0006EEBC File Offset: 0x0006D0BC
	public int UpgradePlayerEnergy(string _steamID)
	{
		Dictionary<string, int> playerUpgradeStamina = this.statsManager.playerUpgradeStamina;
		playerUpgradeStamina[_steamID]++;
		this.UpdateEnergyRightAway(_steamID);
		return this.statsManager.playerUpgradeStamina[_steamID];
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x0006EEFF File Offset: 0x0006D0FF
	private void UpdateEnergyRightAway(string _steamID)
	{
		if (SemiFunc.PlayerAvatarGetFromSteamID(_steamID) == SemiFunc.PlayerAvatarLocal())
		{
			PlayerController.instance.EnergyStart += 10f;
			PlayerController.instance.EnergyCurrent = PlayerController.instance.EnergyStart;
		}
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x0006EF40 File Offset: 0x0006D140
	public int UpgradePlayerExtraJump(string _steamID)
	{
		Dictionary<string, int> playerUpgradeExtraJump = this.statsManager.playerUpgradeExtraJump;
		playerUpgradeExtraJump[_steamID]++;
		this.UpdateExtraJumpRightAway(_steamID);
		return this.statsManager.playerUpgradeExtraJump[_steamID];
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x0006EF83 File Offset: 0x0006D183
	private void UpdateExtraJumpRightAway(string _steamID)
	{
		if (SemiFunc.PlayerAvatarGetFromSteamID(_steamID) == SemiFunc.PlayerAvatarLocal())
		{
			PlayerController.instance.JumpExtra++;
		}
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x0006EFAC File Offset: 0x0006D1AC
	public int UpgradeMapPlayerCount(string _steamID)
	{
		Dictionary<string, int> playerUpgradeMapPlayerCount = this.statsManager.playerUpgradeMapPlayerCount;
		playerUpgradeMapPlayerCount[_steamID]++;
		this.UpdateMapPlayerCountRightAway(_steamID);
		return this.statsManager.playerUpgradeMapPlayerCount[_steamID];
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x0006EFF0 File Offset: 0x0006D1F0
	private void UpdateMapPlayerCountRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (playerAvatar == SemiFunc.PlayerAvatarLocal())
		{
			playerAvatar.upgradeMapPlayerCount++;
		}
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x0006F020 File Offset: 0x0006D220
	public int UpgradePlayerTumbleLaunch(string _steamID)
	{
		Dictionary<string, int> playerUpgradeLaunch = this.statsManager.playerUpgradeLaunch;
		playerUpgradeLaunch[_steamID]++;
		this.UpdateTumbleLaunchRightAway(_steamID);
		return this.statsManager.playerUpgradeLaunch[_steamID];
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x0006F063 File Offset: 0x0006D263
	private void UpdateTumbleLaunchRightAway(string _steamID)
	{
		SemiFunc.PlayerAvatarGetFromSteamID(_steamID).tumble.tumbleLaunch++;
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x0006F080 File Offset: 0x0006D280
	public int UpgradePlayerTumbleWings(string _steamID)
	{
		Dictionary<string, int> playerUpgradeTumbleWings = this.statsManager.playerUpgradeTumbleWings;
		playerUpgradeTumbleWings[_steamID]++;
		this.UpdateTumbleWingsRightAway(_steamID);
		return this.statsManager.playerUpgradeTumbleWings[_steamID];
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x0006F0C4 File Offset: 0x0006D2C4
	private void UpdateTumbleWingsRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (playerAvatar)
		{
			playerAvatar.upgradeTumbleWings += 1f;
		}
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x0006F0F4 File Offset: 0x0006D2F4
	public int UpgradePlayerSprintSpeed(string _steamID)
	{
		Dictionary<string, int> playerUpgradeSpeed = this.statsManager.playerUpgradeSpeed;
		playerUpgradeSpeed[_steamID]++;
		this.UpdateSprintSpeedRightAway(_steamID);
		return this.statsManager.playerUpgradeSpeed[_steamID];
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x0006F137 File Offset: 0x0006D337
	private void UpdateSprintSpeedRightAway(string _steamID)
	{
		if (SemiFunc.PlayerAvatarGetFromSteamID(_steamID) == SemiFunc.PlayerAvatarLocal())
		{
			PlayerController.instance.SprintSpeed += 1f;
			PlayerController.instance.SprintSpeedUpgrades += 1f;
		}
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x0006F178 File Offset: 0x0006D378
	public int UpgradePlayerCrouchRest(string _steamID)
	{
		Dictionary<string, int> playerUpgradeCrouchRest = this.statsManager.playerUpgradeCrouchRest;
		playerUpgradeCrouchRest[_steamID]++;
		this.UpdateCrouchRestRightAway(_steamID);
		return this.statsManager.playerUpgradeCrouchRest[_steamID];
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x0006F1BB File Offset: 0x0006D3BB
	private void UpdateCrouchRestRightAway(string _steamID)
	{
		if (SemiFunc.PlayerAvatarGetFromSteamID(_steamID) == SemiFunc.PlayerAvatarLocal())
		{
			PlayerAvatar.instance.upgradeCrouchRest += 1f;
		}
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x0006F1E8 File Offset: 0x0006D3E8
	public int UpgradePlayerGrabStrength(string _steamID)
	{
		Dictionary<string, int> playerUpgradeStrength = this.statsManager.playerUpgradeStrength;
		playerUpgradeStrength[_steamID]++;
		this.UpdateGrabStrengthRightAway(_steamID);
		return this.statsManager.playerUpgradeStrength[_steamID];
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x0006F22C File Offset: 0x0006D42C
	private void UpdateGrabStrengthRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (playerAvatar)
		{
			playerAvatar.physGrabber.grabStrength += 0.2f;
		}
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x0006F260 File Offset: 0x0006D460
	public int UpgradePlayerThrowStrength(string _steamID)
	{
		Dictionary<string, int> playerUpgradeThrow = this.statsManager.playerUpgradeThrow;
		playerUpgradeThrow[_steamID]++;
		this.UpdateThrowStrengthRightAway(_steamID);
		return this.statsManager.playerUpgradeThrow[_steamID];
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x0006F2A4 File Offset: 0x0006D4A4
	private void UpdateThrowStrengthRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (playerAvatar)
		{
			playerAvatar.physGrabber.throwStrength += 0.3f;
		}
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x0006F2D8 File Offset: 0x0006D4D8
	public int UpgradePlayerGrabRange(string _steamID)
	{
		Dictionary<string, int> playerUpgradeRange = this.statsManager.playerUpgradeRange;
		playerUpgradeRange[_steamID]++;
		this.UpdateGrabRangeRightAway(_steamID);
		return this.statsManager.playerUpgradeRange[_steamID];
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x0006F31C File Offset: 0x0006D51C
	private void UpdateGrabRangeRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (playerAvatar)
		{
			playerAvatar.physGrabber.grabRange += 1f;
		}
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x0006F350 File Offset: 0x0006D550
	public void SyncAllDictionaries()
	{
		StatsManager.instance.statsSynced = true;
		if (!SemiFunc.IsMultiplayer())
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.syncData.Clear();
			Hashtable hashtable = new Hashtable();
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.statsManager.dictionaryOfDictionaries)
			{
				string key = keyValuePair.Key;
				hashtable.Add(key, this.ConvertToHashtable(keyValuePair.Value));
				num++;
				num2++;
				if (num > 3 || num2 == this.statsManager.dictionaryOfDictionaries.Count)
				{
					this.syncData.Add(hashtable);
					num = 0;
				}
			}
			for (int i = 0; i < this.syncData.Count; i++)
			{
				bool flag = i == this.syncData.Count - 1;
				Hashtable hashtable2 = this.syncData[i];
				this.photonView.RPC("ReceiveSyncData", RpcTarget.Others, new object[]
				{
					hashtable2,
					flag
				});
			}
			this.syncData.Clear();
		}
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x0006F48C File Offset: 0x0006D68C
	private Hashtable ConvertToHashtable(Dictionary<string, int> dictionary)
	{
		Hashtable hashtable = new Hashtable();
		foreach (KeyValuePair<string, int> keyValuePair in dictionary)
		{
			hashtable.Add(keyValuePair.Key, keyValuePair.Value);
		}
		return hashtable;
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x0006F4F4 File Offset: 0x0006D6F4
	private Dictionary<K, V> ConvertToDictionary<K, V>(Hashtable hashtable)
	{
		Dictionary<K, V> dictionary = new Dictionary<K, V>();
		foreach (DictionaryEntry dictionaryEntry in hashtable)
		{
			dictionary.Add((K)((object)dictionaryEntry.Key), (V)((object)dictionaryEntry.Value));
		}
		return dictionary;
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x0006F560 File Offset: 0x0006D760
	[PunRPC]
	public void ReceiveSyncData(Hashtable data, bool finalChunk, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.statsManager.dictionaryOfDictionaries)
		{
			string key = keyValuePair.Key;
			if (data.ContainsKey(key))
			{
				list.Add(key);
			}
		}
		foreach (string text in list)
		{
			Dictionary<string, int> dictionary = this.statsManager.dictionaryOfDictionaries[text];
			foreach (DictionaryEntry dictionaryEntry in ((Hashtable)data[text]))
			{
				string text2 = (string)dictionaryEntry.Key;
				int num = (int)dictionaryEntry.Value;
				dictionary[text2] = num;
			}
		}
		if (finalChunk)
		{
			StatsManager.instance.statsSynced = true;
		}
	}

	// Token: 0x0400143C RID: 5180
	internal PhotonView photonView;

	// Token: 0x0400143D RID: 5181
	internal StatsManager statsManager;

	// Token: 0x0400143E RID: 5182
	private ShopManager shopManager;

	// Token: 0x0400143F RID: 5183
	private ItemManager itemManager;

	// Token: 0x04001440 RID: 5184
	public static PunManager instance;

	// Token: 0x04001441 RID: 5185
	private List<Hashtable> syncData = new List<Hashtable>();

	// Token: 0x04001442 RID: 5186
	public PhotonLagSimulationGui lagSimulationGui;

	// Token: 0x04001443 RID: 5187
	private int totalHaul;
}
