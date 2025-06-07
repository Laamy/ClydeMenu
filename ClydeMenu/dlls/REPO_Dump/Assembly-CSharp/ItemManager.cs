using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class ItemManager : MonoBehaviour
{
	// Token: 0x06000BF3 RID: 3059 RVA: 0x000696BA File Offset: 0x000678BA
	private void Awake()
	{
		if (!ItemManager.instance)
		{
			ItemManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x000696E5 File Offset: 0x000678E5
	private void Start()
	{
		base.StartCoroutine(this.TurnOffIconLights());
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x000696F4 File Offset: 0x000678F4
	public void TurnOffIconLightsAgain()
	{
		base.StartCoroutine(this.TurnOffIconLights());
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x00069703 File Offset: 0x00067903
	private IEnumerator TurnOffIconLights()
	{
		if (SemiFunc.RunIsShop() || SemiFunc.MenuLevel())
		{
			this.itemIconLights.SetActive(false);
			yield break;
		}
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.2f);
		}
		if (SemiFunc.RunIsArena())
		{
			this.itemIconLights.SetActive(false);
			yield break;
		}
		for (;;)
		{
			if (!this.spawnedItems.Exists((ItemAttributes x) => !x.hasIcon))
			{
				break;
			}
			yield return new WaitForSeconds(0.2f);
		}
		this.itemIconLights.SetActive(false);
		yield break;
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x00069712 File Offset: 0x00067912
	public void ResetAllItems()
	{
		this.purchasedItems.Clear();
		this.powerCrystals.Clear();
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x0006972A File Offset: 0x0006792A
	public void ItemsInitialize()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		if (SemiFunc.RunIsLevel() || SemiFunc.RunIsLobby() || SemiFunc.RunIsTutorial())
		{
			this.GetAllItemVolumesInScene();
			this.GetPurchasedItems();
			SemiFunc.TruckPopulateItemVolumes();
		}
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x0006975C File Offset: 0x0006795C
	public int IsInLocalPlayersInventory(string itemName)
	{
		for (int i = 0; i < this.localPlayerInventory.Count; i++)
		{
			if (this.localPlayerInventory[i] == itemName)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x00069798 File Offset: 0x00067998
	public void FetchLocalPlayersInventory()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		this.localPlayerInventory.Clear();
		Inventory inventory = Inventory.instance;
		if (inventory != null)
		{
			foreach (InventorySpot inventorySpot in inventory.GetAllSpots())
			{
				ItemEquippable itemEquippable = (inventorySpot != null) ? inventorySpot.CurrentItem : null;
				if (itemEquippable != null)
				{
					ItemAttributes component = itemEquippable.GetComponent<ItemAttributes>();
					if (component != null)
					{
						this.localPlayerInventory.Add(component.item.itemName);
					}
				}
			}
		}
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x00069844 File Offset: 0x00067A44
	public void GetAllItemVolumesInScene()
	{
		if (SemiFunc.IsNotMasterClient())
		{
			return;
		}
		this.itemVolumes.Clear();
		foreach (ItemVolume itemVolume in Object.FindObjectsOfType<ItemVolume>())
		{
			this.itemVolumes.Add(itemVolume);
		}
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x00069888 File Offset: 0x00067A88
	public void AddSpawnedItem(ItemAttributes item)
	{
		this.spawnedItems.Add(item);
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x00069898 File Offset: 0x00067A98
	private void GetPurchasedItems()
	{
		this.purchasedItems.Clear();
		foreach (KeyValuePair<string, int> keyValuePair in StatsManager.instance.itemsPurchased)
		{
			string key = keyValuePair.Key;
			int value = keyValuePair.Value;
			if (StatsManager.instance.itemDictionary.ContainsKey(key))
			{
				Item item = StatsManager.instance.itemDictionary[key];
				bool flag = item.itemType == SemiFunc.itemType.power_crystal && !SemiFunc.RunIsLobby();
				bool flag2 = item.itemType == SemiFunc.itemType.cart && SemiFunc.RunIsLobby();
				if (!flag && !flag2 && !item.disabled)
				{
					int num = Mathf.Clamp(value, 0, StatsManager.instance.itemDictionary[key].maxAmount);
					for (int i = 0; i < num; i++)
					{
						this.purchasedItems.Add(item);
					}
				}
			}
			else
			{
				Debug.LogWarning("Item '" + key + "' not found in the itemDictionary.");
			}
		}
	}

	// Token: 0x0400134E RID: 4942
	public static ItemManager instance;

	// Token: 0x0400134F RID: 4943
	public List<ItemVolume> itemVolumes;

	// Token: 0x04001350 RID: 4944
	public List<Item> purchasedItems = new List<Item>();

	// Token: 0x04001351 RID: 4945
	public List<PhysGrabObject> powerCrystals = new List<PhysGrabObject>();

	// Token: 0x04001352 RID: 4946
	public List<ItemAttributes> spawnedItems = new List<ItemAttributes>();

	// Token: 0x04001353 RID: 4947
	public List<string> localPlayerInventory = new List<string>();

	// Token: 0x04001354 RID: 4948
	internal bool firstIcon = true;

	// Token: 0x04001355 RID: 4949
	public GameObject itemIconLights;
}
