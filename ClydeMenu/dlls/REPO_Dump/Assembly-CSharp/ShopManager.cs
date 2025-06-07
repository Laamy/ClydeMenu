using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class ShopManager : MonoBehaviour
{
	// Token: 0x06000CC7 RID: 3271 RVA: 0x00071118 File Offset: 0x0006F318
	private void Awake()
	{
		if (!ShopManager.instance)
		{
			ShopManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x00071144 File Offset: 0x0006F344
	private void Update()
	{
		if (SemiFunc.RunIsShop() && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			if (this.shopTutorial)
			{
				if (TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialShop, 1))
				{
					TutorialDirector.instance.ActivateTip("Shop", 2f, false);
				}
				this.shopTutorial = false;
				return;
			}
		}
		else
		{
			this.shopTutorial = true;
		}
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x000711A0 File Offset: 0x0006F3A0
	public void ShopCheck()
	{
		this.totalCost = 0;
		List<ItemAttributes> list = new List<ItemAttributes>();
		foreach (ItemAttributes itemAttributes in this.shoppingList)
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
					this.totalCost += itemAttributes.value;
				}
			}
			else
			{
				list.Add(itemAttributes);
			}
		}
		foreach (ItemAttributes itemAttributes2 in list)
		{
			this.shoppingList.Remove(itemAttributes2);
		}
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x00071284 File Offset: 0x0006F484
	public void ShoppingListItemAdd(ItemAttributes item)
	{
		this.shoppingList.Add(item);
		SemiFunc.ShopUpdateCost();
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x00071297 File Offset: 0x0006F497
	public void ShoppingListItemRemove(ItemAttributes item)
	{
		this.shoppingList.Remove(item);
		SemiFunc.ShopUpdateCost();
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x000712AB File Offset: 0x0006F4AB
	public void ShopInitialize()
	{
		if (SemiFunc.RunIsShop())
		{
			this.totalCurrency = SemiFunc.StatGetRunCurrency();
			this.totalCost = 0;
			this.shoppingList.Clear();
			this.GetAllItemsFromStatsManager();
			this.GetAllItemVolumesInScene();
			SemiFunc.ShopPopulateItemVolumes();
		}
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x000712E4 File Offset: 0x0006F4E4
	public float UpgradeValueGet(float _value, Item item)
	{
		_value -= _value * 0.1f * (float)(GameDirector.instance.PlayerList.Count - 1);
		_value += _value * this.upgradeValueIncrease * (float)StatsManager.instance.GetItemsUpgradesPurchased(item.itemAssetName);
		_value = Mathf.Ceil(_value);
		return _value;
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x00071338 File Offset: 0x0006F538
	public float HealthPackValueGet(float _value)
	{
		int num = Mathf.Min(RunManager.instance.levelsCompleted, 15);
		_value -= _value * 0.1f * (float)(GameDirector.instance.PlayerList.Count - 1);
		_value += _value * this.healthPackValueIncrease * (float)num;
		_value = Mathf.Ceil(_value);
		return _value;
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x00071390 File Offset: 0x0006F590
	public float CrystalValueGet(float _value)
	{
		int num = Mathf.Min(RunManager.instance.levelsCompleted, 15);
		_value += _value * this.crystalValueIncrease * (float)num;
		_value = Mathf.Ceil(_value);
		return _value;
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x000713C8 File Offset: 0x0006F5C8
	private void GetAllItemVolumesInScene()
	{
		if (SemiFunc.IsNotMasterClient())
		{
			return;
		}
		this.itemVolumes.Clear();
		foreach (ItemVolume itemVolume in Object.FindObjectsOfType<ItemVolume>())
		{
			if (itemVolume.itemSecretShopType == SemiFunc.itemSecretShopType.none)
			{
				this.itemVolumes.Add(itemVolume);
			}
			else
			{
				if (!this.secretItemVolumes.ContainsKey(itemVolume.itemSecretShopType))
				{
					this.secretItemVolumes.Add(itemVolume.itemSecretShopType, new List<ItemVolume>());
				}
				this.secretItemVolumes[itemVolume.itemSecretShopType].Add(itemVolume);
			}
		}
		foreach (List<ItemVolume> list in this.secretItemVolumes.Values)
		{
			list.Shuffle<ItemVolume>();
		}
		this.itemVolumes.Shuffle<ItemVolume>();
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x000714AC File Offset: 0x0006F6AC
	private void GetAllItemsFromStatsManager()
	{
		if (SemiFunc.IsNotMasterClient())
		{
			return;
		}
		this.potentialItems.Clear();
		this.potentialItemConsumables.Clear();
		this.potentialItemUpgrades.Clear();
		this.potentialItemHealthPacks.Clear();
		this.potentialSecretItems.Clear();
		this.itemConsumablesAmount = Random.Range(1, 6);
		foreach (Item item in StatsManager.instance.itemDictionary.Values)
		{
			int num = SemiFunc.StatGetItemsPurchased(item.itemAssetName);
			float num2 = item.value.valueMax / 1000f * this.itemValueMultiplier;
			if (item.itemType == SemiFunc.itemType.item_upgrade)
			{
				num2 = this.UpgradeValueGet(num2, item);
			}
			else if (item.itemType == SemiFunc.itemType.healthPack)
			{
				num2 = this.HealthPackValueGet(num2);
			}
			else if (item.itemType == SemiFunc.itemType.power_crystal)
			{
				num2 = this.CrystalValueGet(num2);
			}
			float num3 = Mathf.Clamp(num2, 1f, num2);
			bool flag = item.itemType == SemiFunc.itemType.power_crystal;
			bool flag2 = item.itemType == SemiFunc.itemType.item_upgrade;
			bool flag3 = item.itemType == SemiFunc.itemType.healthPack;
			int maxAmountInShop = item.maxAmountInShop;
			if (num < maxAmountInShop && (!item.maxPurchase || StatsManager.instance.GetItemsUpgradesPurchasedTotal(item.itemAssetName) < item.maxPurchaseAmount) && (num3 <= (float)this.totalCurrency || Random.Range(0, 100) < 25))
			{
				for (int i = 0; i < maxAmountInShop - num; i++)
				{
					if (flag2)
					{
						this.potentialItemUpgrades.Add(item);
					}
					else if (flag3)
					{
						this.potentialItemHealthPacks.Add(item);
					}
					else if (flag)
					{
						this.potentialItemConsumables.Add(item);
					}
					else if (item.itemSecretShopType == SemiFunc.itemSecretShopType.none)
					{
						this.potentialItems.Add(item);
					}
					else
					{
						if (!this.potentialSecretItems.ContainsKey(item.itemSecretShopType))
						{
							this.potentialSecretItems.Add(item.itemSecretShopType, new List<Item>());
						}
						this.potentialSecretItems[item.itemSecretShopType].Add(item);
					}
				}
			}
		}
		this.potentialItems.Shuffle<Item>();
		this.potentialItemConsumables.Shuffle<Item>();
		this.potentialItemUpgrades.Shuffle<Item>();
		this.potentialItemHealthPacks.Shuffle<Item>();
		foreach (List<Item> list in this.potentialSecretItems.Values)
		{
			list.Shuffle<Item>();
		}
	}

	// Token: 0x040014B7 RID: 5303
	public static ShopManager instance;

	// Token: 0x040014B8 RID: 5304
	public Transform itemRotateHelper;

	// Token: 0x040014B9 RID: 5305
	public List<ItemVolume> itemVolumes;

	// Token: 0x040014BA RID: 5306
	public List<Item> potentialItems = new List<Item>();

	// Token: 0x040014BB RID: 5307
	public List<Item> potentialItemConsumables = new List<Item>();

	// Token: 0x040014BC RID: 5308
	public List<Item> potentialItemUpgrades = new List<Item>();

	// Token: 0x040014BD RID: 5309
	public List<Item> potentialItemHealthPacks = new List<Item>();

	// Token: 0x040014BE RID: 5310
	public Dictionary<SemiFunc.itemSecretShopType, List<ItemVolume>> secretItemVolumes = new Dictionary<SemiFunc.itemSecretShopType, List<ItemVolume>>();

	// Token: 0x040014BF RID: 5311
	public Dictionary<SemiFunc.itemSecretShopType, List<Item>> potentialSecretItems = new Dictionary<SemiFunc.itemSecretShopType, List<Item>>();

	// Token: 0x040014C0 RID: 5312
	public int itemSpawnTargetAmount = 8;

	// Token: 0x040014C1 RID: 5313
	public int itemConsumablesAmount = 6;

	// Token: 0x040014C2 RID: 5314
	public int itemUpgradesAmount = 3;

	// Token: 0x040014C3 RID: 5315
	public int itemHealthPacksAmount = 3;

	// Token: 0x040014C4 RID: 5316
	internal List<ItemAttributes> shoppingList = new List<ItemAttributes>();

	// Token: 0x040014C5 RID: 5317
	[HideInInspector]
	public int totalCost;

	// Token: 0x040014C6 RID: 5318
	[HideInInspector]
	public int totalCurrency;

	// Token: 0x040014C7 RID: 5319
	[HideInInspector]
	public bool isThief;

	// Token: 0x040014C8 RID: 5320
	[HideInInspector]
	public Transform extractionPoint;

	// Token: 0x040014C9 RID: 5321
	internal float itemValueMultiplier = 4f;

	// Token: 0x040014CA RID: 5322
	internal float upgradeValueIncrease = 0.5f;

	// Token: 0x040014CB RID: 5323
	internal float healthPackValueIncrease = 0.05f;

	// Token: 0x040014CC RID: 5324
	internal float crystalValueIncrease = 0.2f;

	// Token: 0x040014CD RID: 5325
	private bool shopTutorial;
}
