using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x0200027A RID: 634
public class InventorySpot : SemiUI
{
	// Token: 0x17000005 RID: 5
	// (get) Token: 0x060013EC RID: 5100 RVA: 0x000B045E File Offset: 0x000AE65E
	// (set) Token: 0x060013ED RID: 5101 RVA: 0x000B0466 File Offset: 0x000AE666
	public ItemEquippable CurrentItem { get; private set; }

	// Token: 0x060013EE RID: 5102 RVA: 0x000B0470 File Offset: 0x000AE670
	protected override void Start()
	{
		this.inventoryIcon = base.GetComponentInChildren<Image>();
		this.photonView = base.GetComponent<PhotonView>();
		this.UpdateUI();
		this.currentState = InventorySpot.SpotState.Empty;
		base.Start();
		this.uiText = null;
		this.SetEmoji(null);
		this.batteryVisualLogic = base.GetComponentInChildren<BatteryVisualLogic>();
		this.batteryVisualLogic.gameObject.SetActive(false);
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x000B04E0 File Offset: 0x000AE6E0
	private IEnumerator LateStart()
	{
		yield return null;
		if (!SemiFunc.MenuLevel() && !SemiFunc.RunIsLobbyMenu() && !SemiFunc.RunIsArena())
		{
			Inventory.instance.InventorySpotAddAtIndex(this, this.inventorySpotIndex);
		}
		yield break;
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x000B04F0 File Offset: 0x000AE6F0
	public void SetEmoji(Sprite emoji)
	{
		if (!emoji)
		{
			this.inventoryIcon.enabled = false;
			this.noItem.enabled = true;
			return;
		}
		this.noItem.enabled = false;
		this.inventoryIcon.enabled = true;
		this.inventoryIcon.sprite = emoji;
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x000B0542 File Offset: 0x000AE742
	public bool IsOccupied()
	{
		return this.currentState == InventorySpot.SpotState.Occupied;
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x000B054D File Offset: 0x000AE74D
	public void EquipItem(ItemEquippable item)
	{
		if (this.currentState != InventorySpot.SpotState.Empty)
		{
			return;
		}
		this.CurrentItem = item;
		this.currentState = InventorySpot.SpotState.Occupied;
		this.stateStart = true;
		this.UpdateUI();
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x000B0573 File Offset: 0x000AE773
	public void UnequipItem()
	{
		if (this.currentState != InventorySpot.SpotState.Occupied)
		{
			return;
		}
		this.CurrentItem = null;
		this.currentState = InventorySpot.SpotState.Empty;
		this.stateStart = true;
		this.UpdateUI();
	}

	// Token: 0x060013F4 RID: 5108 RVA: 0x000B059C File Offset: 0x000AE79C
	public void UpdateUI()
	{
		if (this.currentState == InventorySpot.SpotState.Occupied && this.CurrentItem != null)
		{
			base.SemiUISpringScale(0.5f, 2f, 0.2f);
			this.SetEmoji(this.CurrentItem.GetComponent<ItemAttributes>().icon);
			return;
		}
		this.SetEmoji(null);
		base.SemiUISpringScale(0.5f, 2f, 0.2f);
	}

	// Token: 0x060013F5 RID: 5109 RVA: 0x000B0608 File Offset: 0x000AE808
	protected override void Update()
	{
		if (SemiFunc.InputDown(InputKey.Inventory1) && this.inventorySpotIndex == 0)
		{
			this.HandleInput();
		}
		else if (SemiFunc.InputDown(InputKey.Inventory2) && this.inventorySpotIndex == 1)
		{
			this.HandleInput();
		}
		else if (SemiFunc.InputDown(InputKey.Inventory3) && this.inventorySpotIndex == 2)
		{
			this.HandleInput();
		}
		InventorySpot.SpotState spotState = this.currentState;
		if (spotState != InventorySpot.SpotState.Empty)
		{
			if (spotState == InventorySpot.SpotState.Occupied)
			{
				this.StateOccupied();
			}
		}
		else
		{
			this.StateEmpty();
		}
		base.Update();
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x000B0684 File Offset: 0x000AE884
	private void HandleInput()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		if (PlayerController.instance.InputDisableTimer > 0f)
		{
			return;
		}
		if (!this.handleInput && Time.time - this.lastEquipTime < this.equipCooldown)
		{
			return;
		}
		PhysGrabber.instance.OverrideGrabRelease();
		this.lastEquipTime = Time.time;
		this.handleInput = false;
		if (this.IsOccupied())
		{
			this.CurrentItem.RequestUnequip();
			return;
		}
		this.AttemptEquipItem();
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x000B0700 File Offset: 0x000AE900
	private void AttemptEquipItem()
	{
		ItemEquippable itemPlayerIsHolding = this.GetItemPlayerIsHolding();
		if (itemPlayerIsHolding != null)
		{
			itemPlayerIsHolding.RequestEquip(this.inventorySpotIndex, PhysGrabber.instance.photonView.ViewID);
		}
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x000B0738 File Offset: 0x000AE938
	private ItemEquippable GetItemPlayerIsHolding()
	{
		if (!PhysGrabber.instance.grabbed)
		{
			return null;
		}
		PhysGrabObject grabbedPhysGrabObject = PhysGrabber.instance.grabbedPhysGrabObject;
		if (grabbedPhysGrabObject == null)
		{
			return null;
		}
		return grabbedPhysGrabObject.GetComponent<ItemEquippable>();
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x000B0760 File Offset: 0x000AE960
	private void StateOccupied()
	{
		if (this.currentState != InventorySpot.SpotState.Occupied || !this.CurrentItem)
		{
			return;
		}
		if (this.stateStart)
		{
			this.batteryVisualLogic.BatteryBarsSet();
		}
		ItemBattery component = this.CurrentItem.GetComponent<ItemBattery>();
		if (component)
		{
			this.batteryVisualLogic.gameObject.SetActive(true);
			if (this.batteryVisualLogic.itemBattery != component)
			{
				this.batteryVisualLogic.itemBattery = component;
				this.batteryVisualLogic.BatteryBarsSet();
			}
			if (SemiFunc.RunIsLobby() && this.batteryVisualLogic.currentBars < this.batteryVisualLogic.batteryBars / 2)
			{
				this.batteryVisualLogic.OverrideChargeNeeded(0.2f);
			}
		}
		else if (this.stateStart)
		{
			this.batteryVisualLogic.BatteryOutro();
		}
		this.stateStart = false;
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x000B0834 File Offset: 0x000AEA34
	private void StateEmpty()
	{
		if (this.currentState != InventorySpot.SpotState.Empty)
		{
			return;
		}
		if (this.stateStart)
		{
			this.batteryVisualLogic.BatteryBarsSet();
			this.batteryVisualLogic.BatteryOutro();
			this.stateStart = false;
		}
		base.SemiUIScoot(new Vector2(0f, -20f));
	}

	// Token: 0x04002231 RID: 8753
	[FormerlySerializedAs("SpotIndex")]
	public int inventorySpotIndex;

	// Token: 0x04002233 RID: 8755
	private PhotonView photonView;

	// Token: 0x04002234 RID: 8756
	private float equipCooldown = 0.2f;

	// Token: 0x04002235 RID: 8757
	private float lastEquipTime;

	// Token: 0x04002236 RID: 8758
	[FormerlySerializedAs("_currentState")]
	[SerializeField]
	private InventorySpot.SpotState currentState;

	// Token: 0x04002237 RID: 8759
	internal Image inventoryIcon;

	// Token: 0x04002238 RID: 8760
	private bool handleInput;

	// Token: 0x04002239 RID: 8761
	public TextMeshProUGUI noItem;

	// Token: 0x0400223A RID: 8762
	private BatteryVisualLogic batteryVisualLogic;

	// Token: 0x0400223B RID: 8763
	private bool stateStart;

	// Token: 0x02000406 RID: 1030
	public enum SpotState
	{
		// Token: 0x04002D7C RID: 11644
		Empty,
		// Token: 0x04002D7D RID: 11645
		Occupied
	}
}
