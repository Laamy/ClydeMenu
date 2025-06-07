using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200014A RID: 330
public class ItemToggle : MonoBehaviour
{
	// Token: 0x06000B44 RID: 2884 RVA: 0x00064580 File Offset: 0x00062780
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x000645A8 File Offset: 0x000627A8
	private void Update()
	{
		if (this.autoTurnOffWhenEquipped && this.itemEquippable && this.itemEquippable.isEquipped && this.toggleState)
		{
			this.ToggleItem(false, -1);
		}
		if (this.playSound && !this.fetchSound)
		{
			this.soundOn = AssetManager.instance.soundDeviceTurnOn;
			this.soundOff = AssetManager.instance.soundDeviceTurnOff;
			this.fetchSound = true;
		}
		if (this.physGrabObject.heldByLocalPlayer && !this.disabled && SemiFunc.InputDown(InputKey.Interact))
		{
			TutorialDirector.instance.playerUsedToggle = true;
			bool toggle = !this.toggleState;
			int player = SemiFunc.PhotonViewIDPlayerAvatarLocal();
			this.ToggleItem(toggle, player);
		}
		if (this.toggleImpulseTimer > 0f)
		{
			this.toggleImpulse = true;
			this.toggleImpulseTimer -= Time.deltaTime;
			return;
		}
		this.toggleImpulse = false;
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x0006468C File Offset: 0x0006288C
	private void ToggleItemLogic(bool toggle, int player = -1)
	{
		this.toggleStatePrevious = this.toggleState;
		this.toggleState = toggle;
		this.playerTogglePhotonID = player;
		if (this.playSound)
		{
			if (this.toggleState)
			{
				this.soundOn.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			else
			{
				this.soundOff.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		this.toggleImpulseTimer = 0.2f;
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x00064726 File Offset: 0x00062926
	public void ToggleItem(bool toggle, int player = -1)
	{
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("ToggleItemRPC", RpcTarget.All, new object[]
			{
				toggle,
				player
			});
			return;
		}
		this.ToggleItemLogic(toggle, player);
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x00064761 File Offset: 0x00062961
	[PunRPC]
	private void ToggleItemRPC(bool toggle, int player = -1)
	{
		this.ToggleItemLogic(toggle, player);
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x0006476B File Offset: 0x0006296B
	public void ToggleDisable(bool _disable)
	{
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("ToggleDisableRPC", RpcTarget.All, new object[]
			{
				_disable
			});
			return;
		}
		this.ToggleDisableRPC(_disable);
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x0006479C File Offset: 0x0006299C
	[PunRPC]
	private void ToggleDisableRPC(bool _disable)
	{
		this.disabled = _disable;
	}

	// Token: 0x0400122E RID: 4654
	[HideInInspector]
	public bool toggleState;

	// Token: 0x0400122F RID: 4655
	public bool playSound;

	// Token: 0x04001230 RID: 4656
	private bool fetchSound;

	// Token: 0x04001231 RID: 4657
	internal bool toggleStatePrevious;

	// Token: 0x04001232 RID: 4658
	private PhotonView photonView;

	// Token: 0x04001233 RID: 4659
	private PhysGrabObject physGrabObject;

	// Token: 0x04001234 RID: 4660
	private ItemEquippable itemEquippable;

	// Token: 0x04001235 RID: 4661
	private Sound soundOn;

	// Token: 0x04001236 RID: 4662
	private Sound soundOff;

	// Token: 0x04001237 RID: 4663
	internal int playerTogglePhotonID;

	// Token: 0x04001238 RID: 4664
	internal bool toggleImpulse;

	// Token: 0x04001239 RID: 4665
	private float toggleImpulseTimer;

	// Token: 0x0400123A RID: 4666
	internal bool disabled;

	// Token: 0x0400123B RID: 4667
	public bool autoTurnOffWhenEquipped = true;
}
