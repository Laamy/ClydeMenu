using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200015C RID: 348
public class ItemEquippable : MonoBehaviourPunCallbacks
{
	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000BD0 RID: 3024 RVA: 0x00068B7A File Offset: 0x00066D7A
	private Rigidbody rb
	{
		get
		{
			return base.GetComponent<Rigidbody>();
		}
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x00068B82 File Offset: 0x00066D82
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x00068B90 File Offset: 0x00066D90
	public bool IsEquipped()
	{
		return this.currentState == ItemEquippable.ItemState.Equipped;
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x00068B9B File Offset: 0x00066D9B
	private bool CollisionCheck()
	{
		return false;
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x00068BA0 File Offset: 0x00066DA0
	public void RequestEquip(int spot, int requestingPlayerId = -1)
	{
		if (this.IsEquipped() || this.currentState == ItemEquippable.ItemState.Unequipping)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			base.photonView.RPC("RPC_RequestEquip", RpcTarget.MasterClient, new object[]
			{
				spot,
				requestingPlayerId
			});
			return;
		}
		this.RPC_RequestEquip(spot, -1);
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x00068BF8 File Offset: 0x00066DF8
	[PunRPC]
	private void RPC_RequestEquip(int spotIndex, int physGrabberPhotonViewID)
	{
		bool flag = SemiFunc.IsMultiplayer();
		if (this.currentState != ItemEquippable.ItemState.Idle)
		{
			return;
		}
		if (flag)
		{
			base.photonView.RPC("RPC_UpdateItemState", RpcTarget.All, new object[]
			{
				2,
				spotIndex,
				physGrabberPhotonViewID
			});
			return;
		}
		this.RPC_UpdateItemState(2, spotIndex, physGrabberPhotonViewID);
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x00068C54 File Offset: 0x00066E54
	[PunRPC]
	private void RPC_UpdateItemState(int state, int spotIndex, int ownerId)
	{
		bool flag = SemiFunc.IsMultiplayer();
		PlayerAvatar playerAvatar = PlayerAvatar.instance;
		if (SemiFunc.IsMultiplayer())
		{
			PhotonView photonView = PhotonView.Find(ownerId);
			playerAvatar = ((photonView != null) ? photonView.GetComponent<PlayerAvatar>() : null);
		}
		InventorySpot inventorySpot = null;
		if (flag)
		{
			if (PhysGrabber.instance.photonView.ViewID == ownerId)
			{
				if (spotIndex != -1)
				{
					inventorySpot = Inventory.instance.GetSpotByIndex(spotIndex);
				}
			}
			else
			{
				inventorySpot = null;
			}
		}
		else if (spotIndex != -1)
		{
			inventorySpot = Inventory.instance.GetSpotByIndex(spotIndex);
		}
		bool flag2 = false;
		if (inventorySpot == null)
		{
			flag2 = true;
		}
		if (inventorySpot != null && inventorySpot.IsOccupied())
		{
			flag2 = true;
		}
		if (Inventory.instance.IsItemEquipped(this))
		{
			flag2 = true;
		}
		if (state == 2)
		{
			string instanceName = base.GetComponent<ItemAttributes>().instanceName;
			StatsManager.instance.PlayerInventoryUpdate(playerAvatar.steamID, instanceName, spotIndex, true);
			this.currentState = ItemEquippable.ItemState.Equipped;
			if (!flag2)
			{
				this.equippedSpot = inventorySpot;
				InventorySpot inventorySpot2 = this.equippedSpot;
				if (inventorySpot2 != null)
				{
					inventorySpot2.EquipItem(this);
				}
			}
			else
			{
				this.equippedSpot = null;
			}
		}
		else
		{
			InventorySpot inventorySpot3 = this.equippedSpot;
			if (inventorySpot3 != null)
			{
				inventorySpot3.UnequipItem();
			}
			this.equippedSpot = null;
		}
		this.inventorySpotIndex = spotIndex;
		this.currentState = (ItemEquippable.ItemState)state;
		this.ownerPlayerId = ownerId;
		this.stateStart = true;
		this.UpdateVisuals();
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x00068D7C File Offset: 0x00066F7C
	private void IsEquippingAndUnequippingTimer()
	{
		if (this.isEquippingTimer > 0f)
		{
			if (this.isEquippingTimer <= 0f)
			{
				this.isEquipping = false;
			}
			this.isEquippingTimer -= Time.deltaTime;
		}
		if (this.isUnequippingTimer > 0f)
		{
			if (this.isUnequippingTimer <= 0f)
			{
				this.isUnequipping = false;
			}
			this.isUnequippingTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x00068DF0 File Offset: 0x00066FF0
	public void RequestUnequip()
	{
		if (!this.IsEquipped())
		{
			return;
		}
		this.currentState = ItemEquippable.ItemState.Unequipping;
		if (SemiFunc.IsMultiplayer())
		{
			base.photonView.RPC("RPC_StartUnequip", RpcTarget.All, new object[]
			{
				this.ownerPlayerId
			});
			return;
		}
		this.RPC_StartUnequip(this.ownerPlayerId);
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x00068E46 File Offset: 0x00067046
	[PunRPC]
	private void RPC_StartUnequip(int requestingPlayerId)
	{
		if (this.ownerPlayerId != requestingPlayerId)
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer() || PhysGrabber.instance.photonView.ViewID == this.ownerPlayerId)
		{
			this.PerformUnequip(requestingPlayerId);
		}
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x00068E78 File Offset: 0x00067078
	private void PerformUnequip(int requestingPlayerId)
	{
		this.unequipTimer = 0.4f;
		this.SetRotation();
		this.currentState = ItemEquippable.ItemState.Unequipping;
		this.physGrabObject.OverrideDeactivateReset();
		if (SemiFunc.IsMultiplayer())
		{
			this.RayHitTestNew(1f);
			base.photonView.RPC("RPC_CompleteUnequip", RpcTarget.MasterClient, new object[]
			{
				requestingPlayerId,
				this.teleportPosition
			});
			return;
		}
		this.RayHitTestNew(1f);
		this.RPC_CompleteUnequip(requestingPlayerId, this.teleportPosition);
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x00068F04 File Offset: 0x00067104
	private bool RayHitTestNew(float distance)
	{
		int layerMask = SemiFunc.LayerMaskGetVisionObstruct() & ~LayerMask.GetMask(new string[]
		{
			"Ignore Raycast",
			"CollisionCheck"
		});
		RaycastHit raycastHit;
		if (Camera.main && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, distance, layerMask))
		{
			this.teleportPosition = raycastHit.point;
		}
		else
		{
			this.teleportPosition = Camera.main.transform.position + Camera.main.transform.forward * distance;
		}
		return this.CollisionCheck();
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x00068FB4 File Offset: 0x000671B4
	private bool RayHitTest(float distance)
	{
		int layerMask = SemiFunc.LayerMaskGetVisionObstruct() & ~LayerMask.GetMask(new string[]
		{
			"Ignore Raycast",
			"CollisionCheck"
		});
		if (Camera.main)
		{
			RaycastHit raycastHit;
			Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, distance, layerMask);
		}
		return this.CollisionCheck();
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x00069023 File Offset: 0x00067223
	private Vector3 GetUnequipPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x00069030 File Offset: 0x00067230
	[PunRPC]
	private void RPC_CompleteUnequip(int physGrabberPhotonViewID, Vector3 teleportPos)
	{
		PhysGrabber physGrabber;
		if (SemiFunc.IsMultiplayer())
		{
			physGrabber = PhotonView.Find(physGrabberPhotonViewID).GetComponent<PhysGrabber>();
		}
		else
		{
			physGrabber = PhysGrabber.instance;
		}
		StatsManager.instance.PlayerInventoryUpdate(physGrabber.playerAvatar.steamID, "", this.inventorySpotIndex, true);
		Transform visionTransform = physGrabber.playerAvatar.PlayerVisionTarget.VisionTransform;
		this.physGrabObject.Teleport(teleportPos, Quaternion.LookRotation(visionTransform.transform.forward, Vector3.up));
		this.rb.isKinematic = false;
		int num = (this.equippedSpot != null) ? this.equippedSpot.inventorySpotIndex : -1;
		InventorySpot inventorySpot = this.equippedSpot;
		if (inventorySpot != null)
		{
			inventorySpot.UnequipItem();
		}
		this.equippedSpot = null;
		this.ownerPlayerId = -1;
		if (SemiFunc.IsMultiplayer())
		{
			base.photonView.RPC("RPC_UpdateItemState", RpcTarget.All, new object[]
			{
				3,
				num,
				physGrabberPhotonViewID
			});
			return;
		}
		this.RPC_UpdateItemState(3, num, -1);
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x00069136 File Offset: 0x00067336
	private void UpdateVisuals()
	{
		if (this.currentState == ItemEquippable.ItemState.Equipped)
		{
			this.SetItemActive(false);
			return;
		}
		if (this.currentState == ItemEquippable.ItemState.Idle)
		{
			this.SetItemActive(true);
			return;
		}
		if (this.currentState == ItemEquippable.ItemState.Unequipping)
		{
			base.StartCoroutine(this.AnimateUnequip());
		}
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x0006916F File Offset: 0x0006736F
	private void SetItemActive(bool isActive)
	{
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x00069171 File Offset: 0x00067371
	private IEnumerator AnimateUnequip()
	{
		float duration = 0.2f;
		float elapsed = 0f;
		Vector3 originalScale = base.transform.localScale;
		Vector3 targetScale = Vector3.one;
		List<Collider> colliders = new List<Collider>();
		colliders.AddRange(base.GetComponents<Collider>());
		colliders.AddRange(base.GetComponentsInChildren<Collider>());
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.physGrabObject.OverrideMass(0.1f, 1f);
			this.physGrabObject.impactDetector.PlayerHitDisableSet();
		}
		this.isUnequipping = true;
		this.isUnequippingTimer = 0.2f;
		Collider _unequipCollider = null;
		bool _hasUnequipCollider = false;
		foreach (Collider collider in colliders)
		{
			PhysGrabObjectBoxCollider component = collider.GetComponent<PhysGrabObjectBoxCollider>();
			if (component && component.unEquipCollider)
			{
				collider.enabled = true;
				_hasUnequipCollider = true;
				_unequipCollider = collider;
				colliders.Remove(collider);
				break;
			}
		}
		if (_hasUnequipCollider)
		{
			using (List<Collider>.Enumerator enumerator = colliders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Collider collider2 = enumerator.Current;
					collider2.enabled = false;
				}
				goto IL_20A;
			}
		}
		using (List<Collider>.Enumerator enumerator = colliders.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Collider collider3 = enumerator.Current;
				collider3.enabled = true;
			}
			goto IL_20A;
		}
		IL_1B4:
		float t = elapsed / duration;
		base.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
		elapsed += Time.deltaTime;
		yield return null;
		IL_20A:
		if (elapsed >= duration)
		{
			if (_hasUnequipCollider)
			{
				_unequipCollider.enabled = false;
				foreach (Collider collider4 in colliders)
				{
					collider4.enabled = true;
				}
			}
			this.isUnequipping = false;
			this.isUnequippingTimer = 0f;
			base.transform.localScale = targetScale;
			this.ForceGrab();
			this.forceGrabTimer = 0.2f;
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.physGrabObject.OverrideMass(0.25f, 0.1f);
				while (this.physGrabObject.rb.mass < this.physGrabObject.massOriginal)
				{
					this.physGrabObject.OverrideMass(this.physGrabObject.rb.mass + 5f * Time.deltaTime, 0.1f);
					yield return null;
				}
			}
			yield break;
		}
		goto IL_1B4;
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x00069180 File Offset: 0x00067380
	private void ForceGrab()
	{
		if (!SemiFunc.IsMultiplayer() || PhysGrabber.instance.photonView.ViewID == this.ownerPlayerId)
		{
			PhysGrabber.instance.OverrideGrab(this.physGrabObject);
		}
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x000691B0 File Offset: 0x000673B0
	private IEnumerator AnimateEquip()
	{
		float duration = 0.1f;
		float elapsed = 0f;
		Vector3 originalScale = base.transform.localScale;
		Vector3 targetScale = originalScale * 0.01f;
		List<Collider> list = new List<Collider>();
		list.AddRange(base.GetComponents<Collider>());
		list.AddRange(base.GetComponentsInChildren<Collider>());
		this.isEquipping = true;
		this.isEquippingTimer = 0.2f;
		using (List<Collider>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Collider collider = enumerator.Current;
				collider.enabled = false;
			}
			goto IL_10F;
		}
		IL_BB:
		float t = elapsed / duration;
		base.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
		elapsed += Time.deltaTime;
		yield return null;
		IL_10F:
		if (elapsed >= duration)
		{
			this.isEquipping = false;
			this.isEquippingTimer = 0f;
			base.transform.localScale = targetScale;
			yield break;
		}
		goto IL_BB;
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x000691C0 File Offset: 0x000673C0
	public void ForceUnequip(Vector3 dropPosition, int physGrabberPhotonViewID)
	{
		if (this.currentState == ItemEquippable.ItemState.Idle)
		{
			return;
		}
		dropPosition += Random.insideUnitSphere * 0.2f;
		if (SemiFunc.IsMultiplayer())
		{
			base.photonView.RPC("RPC_ForceUnequip", RpcTarget.All, new object[]
			{
				dropPosition,
				physGrabberPhotonViewID
			});
			return;
		}
		this.RPC_ForceUnequip(dropPosition, physGrabberPhotonViewID);
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x00069228 File Offset: 0x00067428
	[PunRPC]
	private void RPC_ForceUnequip(Vector3 dropPosition, int physGrabberPhotonViewID)
	{
		PlayerAvatar playerAvatar = PlayerAvatar.instance;
		if (SemiFunc.IsMultiplayer())
		{
			PhotonView photonView = PhotonView.Find(physGrabberPhotonViewID);
			playerAvatar = ((photonView != null) ? photonView.GetComponent<PlayerAvatar>() : null);
		}
		if (this.currentState == ItemEquippable.ItemState.Idle)
		{
			return;
		}
		this.ownerPlayerId = -1;
		this.currentState = ItemEquippable.ItemState.Unequipping;
		StatsManager.instance.PlayerInventoryUpdate(playerAvatar.steamID, "", this.inventorySpotIndex, true);
		if (this.equippedSpot)
		{
			this.equippedSpot.UnequipItem();
			this.equippedSpot = null;
		}
		this.UpdateVisuals();
		this.physGrabObject.OverrideDeactivateReset();
		this.physGrabObject.Teleport(dropPosition, Quaternion.identity);
		base.StartCoroutine(this.AnimateUnequip());
		this.SetItemActive(true);
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x000692E0 File Offset: 0x000674E0
	private void WasEquippedTimer()
	{
		if (this.isEquippedPrev != this.isEquipped)
		{
			this.wasEquippedTimer = 0.5f;
			this.isEquippedPrev = this.isEquipped;
		}
		if (this.wasEquippedTimer > 0f)
		{
			this.wasEquippedTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x00069334 File Offset: 0x00067534
	private void Update()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		this.WasEquippedTimer();
		this.IsEquippingAndUnequippingTimer();
		switch (this.currentState)
		{
		case ItemEquippable.ItemState.Idle:
			this.StateIdle();
			break;
		case ItemEquippable.ItemState.Equipping:
			this.StateEquipping();
			break;
		case ItemEquippable.ItemState.Equipped:
			this.StateEquipped();
			break;
		case ItemEquippable.ItemState.Unequipping:
			this.StateUnequipping();
			break;
		}
		if (this.unequipTimer > 0f)
		{
			this.unequipTimer -= Time.deltaTime;
		}
		if (this.equipTimer > 0f)
		{
			this.equipTimer -= Time.deltaTime;
		}
		if (this.itemEquipCubeShowTimer > 0f)
		{
			this.itemEquipCubeShowTimer -= Time.deltaTime;
			if (this.itemEquipCubeShowTimer <= 0f)
			{
				Vector3 localScale = base.transform.localScale;
				base.transform.localScale = Vector3.one;
				base.transform.localScale = localScale;
			}
		}
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x00069424 File Offset: 0x00067624
	private void StateIdleStart()
	{
		if (!this.stateStart)
		{
			return;
		}
		this.stateStart = false;
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x00069438 File Offset: 0x00067638
	private void StateIdle()
	{
		if (this.currentState != ItemEquippable.ItemState.Idle)
		{
			return;
		}
		this.StateIdleStart();
		this.isEquipped = false;
		if (this.forceGrabTimer > 0f)
		{
			this.forceGrabTimer -= Time.deltaTime;
			if (this.forceGrabTimer <= 0f)
			{
				this.ForceGrab();
			}
		}
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x0006948D File Offset: 0x0006768D
	private void StateEquippingStart()
	{
		if (!this.stateStart)
		{
			return;
		}
		this.stateStart = false;
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x0006949F File Offset: 0x0006769F
	private void StateEquipping()
	{
		if (this.currentState != ItemEquippable.ItemState.Equipping)
		{
			return;
		}
		this.StateEquippingStart();
		this.currentState = ItemEquippable.ItemState.Equipped;
		this.isEquipped = true;
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x000694C0 File Offset: 0x000676C0
	private void StateEquippedStart()
	{
		if (!this.stateStart)
		{
			return;
		}
		this.stateStart = false;
		AssetManager.instance.soundEquip.Play(this.physGrabObject.midPoint, 1f, 1f, 1f, 1f);
		base.StartCoroutine(this.AnimateEquip());
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x0006951C File Offset: 0x0006771C
	private void StateEquipped()
	{
		if (this.currentState != ItemEquippable.ItemState.Equipped)
		{
			return;
		}
		this.StateEquippedStart();
		foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing)
		{
			physGrabber.OverrideGrabRelease();
		}
		if (!this.isEquipped)
		{
			this.equipTimer = 0.5f;
		}
		this.isEquipped = true;
		if (this.physGrabObject.transform.localScale.magnitude < 0.1f)
		{
			this.physGrabObject.OverrideDeactivate(0.1f);
		}
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x000695CC File Offset: 0x000677CC
	private void StateUnequippingStart()
	{
		if (!this.stateStart)
		{
			return;
		}
		this.stateStart = false;
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x000695E0 File Offset: 0x000677E0
	private void StateUnequipping()
	{
		AssetManager.instance.soundUnequip.Play(this.physGrabObject.midPoint, 1f, 1f, 1f, 1f);
		if (this.currentState != ItemEquippable.ItemState.Unequipping)
		{
			return;
		}
		this.currentState = ItemEquippable.ItemState.Idle;
		this.isEquipped = false;
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x00069634 File Offset: 0x00067834
	private void SetRotation()
	{
		this.physGrabObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
		this.physGrabObject.rb.rotation = this.physGrabObject.transform.rotation;
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x0006968A File Offset: 0x0006788A
	private void OnDestroy()
	{
		if (this.equippedSpot)
		{
			this.equippedSpot.UnequipItem();
		}
	}

	// Token: 0x04001336 RID: 4918
	[FormerlySerializedAs("_currentState")]
	[SerializeField]
	private ItemEquippable.ItemState currentState;

	// Token: 0x04001337 RID: 4919
	public Sprite ItemIcon;

	// Token: 0x04001338 RID: 4920
	private InventorySpot equippedSpot;

	// Token: 0x04001339 RID: 4921
	private int ownerPlayerId = -1;

	// Token: 0x0400133A RID: 4922
	internal bool isEquipped;

	// Token: 0x0400133B RID: 4923
	internal bool isEquippedPrev;

	// Token: 0x0400133C RID: 4924
	internal float wasEquippedTimer;

	// Token: 0x0400133D RID: 4925
	internal bool isUnequipping;

	// Token: 0x0400133E RID: 4926
	internal bool isEquipping;

	// Token: 0x0400133F RID: 4927
	private float isUnequippingTimer;

	// Token: 0x04001340 RID: 4928
	private float isEquippingTimer;

	// Token: 0x04001341 RID: 4929
	public LayerMask ObstructionLayers;

	// Token: 0x04001342 RID: 4930
	public SemiFunc.emojiIcon itemEmojiIcon;

	// Token: 0x04001343 RID: 4931
	internal string itemEmoji;

	// Token: 0x04001344 RID: 4932
	internal int inventorySpotIndex;

	// Token: 0x04001345 RID: 4933
	internal float unequipTimer;

	// Token: 0x04001346 RID: 4934
	internal float equipTimer;

	// Token: 0x04001347 RID: 4935
	private const float animationDuration = 0.4f;

	// Token: 0x04001348 RID: 4936
	private PhysGrabObject physGrabObject;

	// Token: 0x04001349 RID: 4937
	private bool stateStart = true;

	// Token: 0x0400134A RID: 4938
	private float itemEquipCubeShowTimer;

	// Token: 0x0400134B RID: 4939
	private Vector3 teleportPosition;

	// Token: 0x0400134C RID: 4940
	private float forceGrabTimer;

	// Token: 0x0400134D RID: 4941
	internal PhysGrabber latestOwner;

	// Token: 0x02000383 RID: 899
	public enum ItemState
	{
		// Token: 0x04002B49 RID: 11081
		Idle,
		// Token: 0x04002B4A RID: 11082
		Equipping,
		// Token: 0x04002B4B RID: 11083
		Equipped,
		// Token: 0x04002B4C RID: 11084
		Unequipping
	}
}
