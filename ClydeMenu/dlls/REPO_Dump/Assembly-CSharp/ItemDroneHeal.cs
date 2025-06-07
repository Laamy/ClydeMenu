using System;
using UnityEngine;

// Token: 0x0200013E RID: 318
public class ItemDroneHeal : MonoBehaviour, ITargetingCondition
{
	// Token: 0x06000ACE RID: 2766 RVA: 0x0005F4D6 File Offset: 0x0005D6D6
	private void Start()
	{
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemDrone = base.GetComponent<ItemDrone>();
		this.myPhysGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x0005F4FC File Offset: 0x0005D6FC
	public bool CustomTargetingCondition(GameObject target)
	{
		PlayerAvatar component = target.GetComponent<PlayerAvatar>();
		return component.playerHealth.health < component.playerHealth.maxHealth;
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x0005F528 File Offset: 0x0005D728
	private void Update()
	{
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		if (this.itemDrone.itemActivated)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.myPhysGrabObject.OverrideZeroGravity(0.1f);
				this.myPhysGrabObject.OverrideDrag(1f, 0.1f);
				this.myPhysGrabObject.OverrideAngularDrag(10f, 0.1f);
			}
			if (this.itemDrone.magnetActive && this.itemDrone.playerAvatarTarget)
			{
				this.healTimer += Time.deltaTime;
				if (this.healTimer > this.healRate)
				{
					this.itemDrone.playerAvatarTarget.playerHealth.Heal(this.healAmount, true);
					if (this.itemDrone.playerAvatarTarget.playerHealth.health >= this.itemDrone.playerAvatarTarget.playerHealth.maxHealth)
					{
						this.itemDrone.MagnetActiveToggle(false);
					}
					this.healTimer = 0f;
				}
			}
		}
	}

	// Token: 0x0400116D RID: 4461
	private ItemDrone itemDrone;

	// Token: 0x0400116E RID: 4462
	private PhysGrabObject myPhysGrabObject;

	// Token: 0x0400116F RID: 4463
	private float healRate = 2f;

	// Token: 0x04001170 RID: 4464
	private float healTimer;

	// Token: 0x04001171 RID: 4465
	private int healAmount = 10;

	// Token: 0x04001172 RID: 4466
	private ItemEquippable itemEquippable;
}
