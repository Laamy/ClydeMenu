using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200015A RID: 346
public class ItemAttributes : MonoBehaviour
{
	// Token: 0x06000BBE RID: 3006 RVA: 0x00068393 File Offset: 0x00066593
	private void OnValidate()
	{
		if (!SemiFunc.OnValidateCheck())
		{
			bool enabled = base.enabled;
		}
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x000683A4 File Offset: 0x000665A4
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (this.item)
		{
			this.colorPreset = this.item.colorPreset;
		}
		else
		{
			this.colorPreset = null;
		}
		if (this.item)
		{
			this.emojiIcon = this.item.emojiIcon;
			return;
		}
		this.emojiIcon = SemiFunc.emojiIcon.drone_heal;
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x0006840C File Offset: 0x0006660C
	private void Start()
	{
		this.itemName = this.item.itemName;
		this.instanceName = null;
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemType = this.item.itemType;
		this.itemAssetName = this.item.itemAssetName;
		this.itemValueMin = this.item.value.valueMin;
		this.itemValueMax = this.item.value.valueMax;
		if (SemiFunc.RunIsShop())
		{
			this.shopItem = true;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			ItemVolume componentInChildren = base.GetComponentInChildren<ItemVolume>();
			if (componentInChildren)
			{
				this.itemVolume = componentInChildren.transform;
			}
			if (this.itemVolume)
			{
				Vector3 b = this.itemVolume.position - base.transform.position;
				base.transform.position -= b;
				Rigidbody component = base.GetComponent<Rigidbody>();
				component.position -= b;
				if (SemiFunc.IsMultiplayer())
				{
					base.GetComponent<PhotonTransformView>().Teleport(component.position, base.transform.rotation);
				}
				Object.Destroy(this.itemVolume.gameObject);
			}
		}
		if (!this.shopItem)
		{
			ItemManager.instance.AddSpawnedItem(this);
		}
		base.transform.parent = LevelGenerator.Instance.ItemParent.transform;
		this.GetValue();
		this.roomVolumeCheck = base.GetComponent<RoomVolumeCheck>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		if (this.itemEquippable)
		{
			this.itemEquippable.itemEmojiIcon = this.emojiIcon;
			this.itemEquippable.itemEmoji = this.emojiIcon.ToString();
		}
		base.StartCoroutine(this.GenerateIcon());
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x000685F5 File Offset: 0x000667F5
	private IEnumerator GenerateIcon()
	{
		yield return null;
		if (this.itemEquippable && !this.icon)
		{
			SemiIconMaker componentInChildren = base.GetComponentInChildren<SemiIconMaker>();
			if (componentInChildren)
			{
				this.icon = componentInChildren.CreateIconFromRenderTexture();
			}
			else
			{
				Debug.LogWarning("No IconMaker found in " + base.gameObject.name + ", add SemiIconMaker prefab and align the camera to make a proper icon... or make a custom icon and assign it in the Item Attributes!");
			}
		}
		this.hasIcon = true;
		yield break;
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x00068604 File Offset: 0x00066804
	private IEnumerator LateStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.photonView = base.GetComponent<PhotonView>();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			StatsManager.instance.ItemFetchName(this.itemAssetName, this, this.photonView.ViewID);
		}
		yield break;
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x00068614 File Offset: 0x00066814
	public void GetValue()
	{
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		float num = Random.Range(this.itemValueMin, this.itemValueMax) * ShopManager.instance.itemValueMultiplier;
		if (num < 1000f)
		{
			num = 1000f;
		}
		num = Mathf.Ceil(num / 1000f);
		if (this.itemType == SemiFunc.itemType.item_upgrade)
		{
			num = ShopManager.instance.UpgradeValueGet(num, this.item);
		}
		else if (this.itemType == SemiFunc.itemType.healthPack)
		{
			num = ShopManager.instance.HealthPackValueGet(num);
		}
		else if (this.itemType == SemiFunc.itemType.power_crystal)
		{
			num = ShopManager.instance.CrystalValueGet(num);
		}
		this.value = (int)num;
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("GetValueRPC", RpcTarget.Others, new object[]
			{
				this.value
			});
		}
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x000686E5 File Offset: 0x000668E5
	[PunRPC]
	public void GetValueRPC(int _value)
	{
		this.value = _value;
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x000686EE File Offset: 0x000668EE
	public void DisableUI(bool _disable)
	{
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("DisableUIRPC", RpcTarget.All, new object[]
			{
				_disable
			});
			return;
		}
		this.DisableUIRPC(_disable);
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x0006871F File Offset: 0x0006691F
	[PunRPC]
	public void DisableUIRPC(bool _disable)
	{
		this.disableUI = _disable;
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x00068728 File Offset: 0x00066928
	private void ShopInTruckLogic()
	{
		if (!SemiFunc.RunIsShop())
		{
			return;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.inStartRoomCheckTimer > 0f)
			{
				this.inStartRoomCheckTimer -= Time.deltaTime;
				return;
			}
			bool flag = false;
			using (List<RoomVolume>.Enumerator enumerator = this.roomVolumeCheck.CurrentRooms.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Extraction)
					{
						flag = true;
					}
				}
			}
			if (!flag && this.inStartRoom)
			{
				ShopManager.instance.ShoppingListItemRemove(this);
				this.inStartRoom = false;
			}
			this.inStartRoomCheckTimer = 0.5f;
			if (flag && !this.inStartRoom)
			{
				ShopManager.instance.ShoppingListItemAdd(this);
				this.inStartRoom = true;
			}
		}
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x000687FC File Offset: 0x000669FC
	private void Update()
	{
		if (this.showInfoTimer > 0f && this.physGrabObject.grabbedLocal)
		{
			this.itemTag = "";
			this.promptName = "";
			this.showInfoTimer = 0f;
		}
		if (this.showInfoTimer > 0f)
		{
			if (!PhysGrabber.instance.grabbed)
			{
				this.ShowingInfo();
				this.showInfoTimer -= Time.fixedDeltaTime;
			}
			else
			{
				this.showInfoTimer = 0f;
			}
		}
		this.ShopInTruckLogic();
		if (this.physGrabObject.playerGrabbing.Count > 0 && !this.disableUI && PhysGrabber.instance.grabbedPhysGrabObject == this.physGrabObject && PhysGrabber.instance.grabbed)
		{
			this.ShowingInfo();
		}
		if (this.isHeldTimer > 0f)
		{
			this.isHeldTimer -= Time.deltaTime;
		}
		if (this.isHeldTimer <= 0f && this.physGrabObject.grabbedLocal)
		{
			this.isHeldTimer = 0.2f;
		}
		if (this.isHeldTimer > 0f)
		{
			if (SemiFunc.RunIsShop() && !PhysGrabber.instance.grabbed && PhysGrabber.instance.currentlyLookingAtItemAttributes == this)
			{
				WorldSpaceUIValue.instance.Show(this.physGrabObject, this.value, true, this.costOffset);
			}
			SemiFunc.UIItemInfoText(this, this.promptName);
			return;
		}
		if (this.itemTag != "")
		{
			this.itemTag = "";
			this.promptName = "";
		}
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x00068994 File Offset: 0x00066B94
	public void ShowingInfo()
	{
		if (this.isHeldTimer < 0f)
		{
			return;
		}
		bool grabbedLocal = this.physGrabObject.grabbedLocal;
		if (grabbedLocal || SemiFunc.RunIsShop())
		{
			this.isHeldTimer = 0.2f;
			bool flag = SemiFunc.RunIsShop() && (this.itemType == SemiFunc.itemType.item_upgrade || this.itemType == SemiFunc.itemType.healthPack);
			ItemToggle itemToggle = this.itemToggle;
			if (itemToggle && !itemToggle.disabled && !flag && this.itemTag == "")
			{
				this.itemTag = InputManager.instance.InputDisplayReplaceTags("[interact]");
				if (grabbedLocal)
				{
					this.promptName = this.itemName + " <color=#FFFFFF>[" + this.itemTag + "]</color>";
					return;
				}
				this.promptName = this.itemName;
				return;
			}
			else
			{
				if (!flag && this.showInfoTimer <= 0f && itemToggle && !itemToggle.disabled && this.itemTag != "")
				{
					this.promptName = this.itemName + " <color=#FFFFFF>[" + this.itemTag + "]</color>";
					return;
				}
				this.promptName = this.itemName;
			}
		}
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x00068AC6 File Offset: 0x00066CC6
	public void ShowInfo()
	{
		if (SemiFunc.RunIsShop() && !this.physGrabObject.grabbedLocal)
		{
			this.isHeldTimer = 0.1f;
			this.showInfoTimer = 0.1f;
		}
	}

	// Token: 0x06000BCB RID: 3019 RVA: 0x00068AF2 File Offset: 0x00066CF2
	private void OnDestroy()
	{
		if (this.icon)
		{
			Object.Destroy(this.icon);
		}
	}

	// Token: 0x04001319 RID: 4889
	private PhotonView photonView;

	// Token: 0x0400131A RID: 4890
	public Item item;

	// Token: 0x0400131B RID: 4891
	public Vector3 costOffset;

	// Token: 0x0400131C RID: 4892
	internal SemiFunc.emojiIcon emojiIcon;

	// Token: 0x0400131D RID: 4893
	internal ColorPresets colorPreset;

	// Token: 0x0400131E RID: 4894
	internal int value;

	// Token: 0x0400131F RID: 4895
	internal RoomVolumeCheck roomVolumeCheck;

	// Token: 0x04001320 RID: 4896
	private float inStartRoomCheckTimer;

	// Token: 0x04001321 RID: 4897
	private bool inStartRoom;

	// Token: 0x04001322 RID: 4898
	private ItemEquippable itemEquippable;

	// Token: 0x04001323 RID: 4899
	private Transform itemVolume;

	// Token: 0x04001324 RID: 4900
	internal bool shopItem;

	// Token: 0x04001325 RID: 4901
	private PhysGrabObject physGrabObject;

	// Token: 0x04001326 RID: 4902
	internal bool disableUI;

	// Token: 0x04001327 RID: 4903
	internal string itemName;

	// Token: 0x04001328 RID: 4904
	internal string instanceName;

	// Token: 0x04001329 RID: 4905
	internal float showInfoTimer;

	// Token: 0x0400132A RID: 4906
	internal bool hasIcon;

	// Token: 0x0400132B RID: 4907
	public Sprite icon;

	// Token: 0x0400132C RID: 4908
	private SemiFunc.itemType itemType;

	// Token: 0x0400132D RID: 4909
	private ItemToggle itemToggle;

	// Token: 0x0400132E RID: 4910
	private float isHeldTimer;

	// Token: 0x0400132F RID: 4911
	private string itemTag = "";

	// Token: 0x04001330 RID: 4912
	private string promptName = "";

	// Token: 0x04001331 RID: 4913
	private string itemAssetName = "";

	// Token: 0x04001332 RID: 4914
	private float itemValueMin;

	// Token: 0x04001333 RID: 4915
	private float itemValueMax;
}
