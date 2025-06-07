using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200027B RID: 635
public class Inventory : MonoBehaviour
{
	// Token: 0x060013FC RID: 5116 RVA: 0x000B0897 File Offset: 0x000AEA97
	private void Awake()
	{
		if (Inventory.instance != null && Inventory.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Inventory.instance = this;
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x000B08C8 File Offset: 0x000AEAC8
	public void InventorySpotAddAtIndex(InventorySpot spot, int index)
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		this.inventorySpots[index] = spot;
		using (List<InventorySpot>.Enumerator enumerator = this.inventorySpots.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == null)
				{
					return;
				}
			}
		}
		this.spotsFeched = true;
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x000B093C File Offset: 0x000AEB3C
	private void Start()
	{
		if (SemiFunc.RunIsArena())
		{
			base.enabled = false;
		}
		this.playerController = base.GetComponent<PlayerController>();
		for (int i = 0; i < 3; i++)
		{
			this.inventorySpots.Add(null);
		}
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x060013FF RID: 5119 RVA: 0x000B0988 File Offset: 0x000AEB88
	private IEnumerator LateStart()
	{
		yield return null;
		this.physGrabber = this.playerController.playerAvatarScript.physGrabber;
		this.playerAvatar = this.playerController.playerAvatarScript;
		yield break;
	}

	// Token: 0x06001400 RID: 5120 RVA: 0x000B0997 File Offset: 0x000AEB97
	public InventorySpot GetSpotByIndex(int index)
	{
		return this.inventorySpots[index];
	}

	// Token: 0x06001401 RID: 5121 RVA: 0x000B09A8 File Offset: 0x000AEBA8
	public bool IsItemEquipped(ItemEquippable item)
	{
		foreach (InventorySpot inventorySpot in this.inventorySpots)
		{
			if (inventorySpot && inventorySpot.CurrentItem == item)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001402 RID: 5122 RVA: 0x000B0A14 File Offset: 0x000AEC14
	public bool IsSpotOccupied(int index)
	{
		InventorySpot spotByIndex = this.GetSpotByIndex(index);
		return spotByIndex != null && spotByIndex.IsOccupied();
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x000B0A3A File Offset: 0x000AEC3A
	public List<InventorySpot> GetAllSpots()
	{
		return this.inventorySpots;
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x000B0A44 File Offset: 0x000AEC44
	public int GetFirstFreeInventorySpotIndex()
	{
		List<InventorySpot> allSpots = Inventory.instance.GetAllSpots();
		for (int i = 0; i < allSpots.Count; i++)
		{
			if (!allSpots[i].IsOccupied())
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x000B0A80 File Offset: 0x000AEC80
	public int InventorySpotsOccupied()
	{
		int num = 0;
		foreach (InventorySpot inventorySpot in this.inventorySpots)
		{
			if (inventorySpot && inventorySpot.IsOccupied())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x000B0AE4 File Offset: 0x000AECE4
	public void InventoryDropAll(Vector3 dropPosition, int playerViewID)
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		foreach (InventorySpot inventorySpot in this.inventorySpots)
		{
			if (inventorySpot.IsOccupied())
			{
				ItemEquippable currentItem = inventorySpot.CurrentItem;
				if (currentItem != null)
				{
					currentItem.ForceUnequip(dropPosition, playerViewID);
				}
			}
		}
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x000B0B58 File Offset: 0x000AED58
	public int GetBatteryStateFromInventorySpot(int index)
	{
		InventorySpot spotByIndex = this.GetSpotByIndex(index);
		if (spotByIndex != null && spotByIndex.IsOccupied())
		{
			ItemEquippable currentItem = spotByIndex.CurrentItem;
			if (currentItem != null)
			{
				ItemBattery component = currentItem.GetComponent<ItemBattery>();
				if (component != null)
				{
					return component.batteryLifeInt;
				}
			}
		}
		return -1;
	}

	// Token: 0x06001408 RID: 5128 RVA: 0x000B0BA8 File Offset: 0x000AEDA8
	public void ForceUnequip()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		foreach (InventorySpot inventorySpot in this.inventorySpots)
		{
			if (inventorySpot.IsOccupied())
			{
				ItemEquippable currentItem = inventorySpot.CurrentItem;
				if (currentItem)
				{
					if (SemiFunc.IsMultiplayer())
					{
						currentItem.GetComponent<ItemEquippable>().ForceUnequip(this.playerAvatar.PlayerVisionTarget.VisionTransform.position, this.physGrabber.photonView.ViewID);
					}
					else
					{
						currentItem.GetComponent<ItemEquippable>().ForceUnequip(this.playerAvatar.PlayerVisionTarget.VisionTransform.position, -1);
					}
				}
			}
		}
	}

	// Token: 0x0400223C RID: 8764
	public static Inventory instance;

	// Token: 0x0400223D RID: 8765
	internal readonly List<InventorySpot> inventorySpots = new List<InventorySpot>();

	// Token: 0x0400223E RID: 8766
	internal PhysGrabber physGrabber;

	// Token: 0x0400223F RID: 8767
	private PlayerController playerController;

	// Token: 0x04002240 RID: 8768
	private PlayerAvatar playerAvatar;

	// Token: 0x04002241 RID: 8769
	internal bool spotsFeched;
}
