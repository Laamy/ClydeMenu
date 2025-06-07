using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000140 RID: 320
public class ItemDroneTorque : MonoBehaviour
{
	// Token: 0x06000AD5 RID: 2773 RVA: 0x0005F72C File Offset: 0x0005D92C
	private void Start()
	{
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemDrone = base.GetComponent<ItemDrone>();
		this.myPhysGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.itemAttributes = base.GetComponent<ItemAttributes>();
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x0005F784 File Offset: 0x0005D984
	private void RollTowards(Vector3 direction, Rigidbody targetRb)
	{
		Vector3 a = Vector3.Cross(Vector3.up, direction).normalized * 6f;
		float d = Mathf.Clamp(3f / targetRb.mass, 1f, 10f);
		a *= d;
		targetRb.angularVelocity = a / targetRb.mass;
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x0005F7E5 File Offset: 0x0005D9E5
	private void BatteryDrain(float amount)
	{
		this.itemBattery.batteryLife -= amount * Time.fixedDeltaTime;
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x0005F800 File Offset: 0x0005DA00
	private void FixedUpdate()
	{
		if (!this.itemDrone.itemActivated)
		{
			this.tumbledPlayer = false;
			this.tumbleEnemyTimer = 0f;
		}
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.itemDrone.itemActivated)
		{
			if (!this.itemDrone.droneOwner || (this.itemDrone.droneOwner && this.itemDrone.droneOwner.isDisabled))
			{
				this.itemToggle.ToggleItem(false, -1);
				return;
			}
			this.myPhysGrabObject.OverrideZeroGravity(0.1f);
			this.myPhysGrabObject.OverrideDrag(1f, 0.1f);
			this.myPhysGrabObject.OverrideAngularDrag(10f, 0.1f);
			if (this.itemDrone.magnetActive)
			{
				if (this.itemDrone.playerAvatarTarget && !this.tumbledPlayer)
				{
					if (!this.itemDrone.playerAvatarTarget.tumble.isTumbling)
					{
						this.itemDrone.playerAvatarTarget.tumble.TumbleRequest(true, false);
					}
					this.tumbledPlayer = true;
				}
				if (this.itemDrone.playerTumbleTarget)
				{
					Vector3 forward = this.itemDrone.playerTumbleTarget.playerAvatar.localCameraTransform.forward;
					if (SemiFunc.OnGroundCheck(this.itemDrone.magnetTargetRigidbody.position, 1.5f, this.itemDrone.magnetTargetPhysGrabObject))
					{
						Vector3 a = SemiFunc.PhysFollowDirection(this.itemDrone.magnetTargetRigidbody.transform, forward, this.itemDrone.magnetTargetRigidbody, 20f);
						this.itemDrone.magnetTargetRigidbody.AddTorque(a * 2.5f / this.itemDrone.magnetTargetRigidbody.mass, ForceMode.Force);
						Vector3 a2 = SemiFunc.PhysFollowPosition(this.itemDrone.magnetTargetRigidbody.position, this.itemDrone.magnetTargetRigidbody.position + forward * 2.5f, this.itemDrone.magnetTargetRigidbody.velocity, 25f);
						this.itemDrone.magnetTargetRigidbody.AddForce(a2 * 2f / this.itemDrone.magnetTargetRigidbody.mass, ForceMode.Force);
						this.BatteryDrain(2f);
						if (this.itemDrone.magnetTargetPhysGrabObject)
						{
							this.itemDrone.magnetTargetPhysGrabObject.OverrideMaterial(SemiFunc.PhysicMaterialSticky(), 0.1f);
						}
					}
				}
				if (this.itemDrone.magnetTargetPhysGrabObject && !this.itemDrone.playerAvatarTarget && !this.itemDrone.playerTumbleTarget)
				{
					Rigidbody magnetTargetRigidbody = this.itemDrone.magnetTargetRigidbody;
					Transform transform = this.itemDrone.droneOwner.transform;
					if (transform)
					{
						float num = Vector3.Distance(new Vector3(magnetTargetRigidbody.position.x, 0f, magnetTargetRigidbody.position.z), new Vector3(transform.position.x, 0f, transform.position.z));
						Vector3 vector = (transform.position - magnetTargetRigidbody.position).normalized;
						if (this.itemDrone.magnetTargetPhysGrabObject.isEnemy)
						{
							EnemyParent componentInParent = this.itemDrone.magnetTargetPhysGrabObject.GetComponentInParent<EnemyParent>();
							if (componentInParent)
							{
								SemiFunc.ItemAffectEnemyBatteryDrain(componentInParent, this.itemBattery, this.tumbleEnemyTimer, Time.fixedDeltaTime, 1f);
							}
							this.tumbleEnemyTimer += Time.fixedDeltaTime;
							vector = -vector;
						}
						float d = 2f;
						float num2 = Mathf.Clamp(magnetTargetRigidbody.mass / 1f, 0.2f, 1f);
						float num3 = num2 * 2f;
						if (num < num3)
						{
							d = Mathf.Clamp(num - num2, 0f, num3) / num3;
						}
						Vector3 a3 = SemiFunc.PhysFollowDirection(this.itemDrone.magnetTargetRigidbody.transform, vector, this.itemDrone.magnetTargetRigidbody, 10f) * d;
						this.itemDrone.magnetTargetRigidbody.AddTorque(a3 * 5f / this.itemDrone.magnetTargetRigidbody.mass, ForceMode.Force);
						Vector3 a4 = SemiFunc.PhysFollowPosition(this.itemDrone.magnetTargetRigidbody.position, this.itemDrone.magnetTargetRigidbody.position + vector, this.itemDrone.magnetTargetRigidbody.velocity, 10f) * d;
						this.itemDrone.magnetTargetRigidbody.AddForce(a4 * 2f / this.itemDrone.magnetTargetRigidbody.mass, ForceMode.Force);
						this.itemDrone.magnetTargetPhysGrabObject.OverrideFragility(0.65f);
						return;
					}
					Vector3 vector2 = -base.transform.forward.normalized;
					Vector3 a5 = SemiFunc.PhysFollowDirection(this.itemDrone.magnetTargetRigidbody.transform, vector2, this.itemDrone.magnetTargetRigidbody, 10f);
					this.itemDrone.magnetTargetRigidbody.AddTorque(a5 * 2f / this.itemDrone.magnetTargetRigidbody.mass, ForceMode.Force);
					Vector3 a6 = SemiFunc.PhysFollowPosition(this.itemDrone.magnetTargetRigidbody.position, this.itemDrone.magnetTargetRigidbody.position + vector2, this.itemDrone.magnetTargetRigidbody.velocity, 10f);
					this.itemDrone.magnetTargetRigidbody.AddForce(a6 * 1f / this.itemDrone.magnetTargetRigidbody.mass, ForceMode.Force);
				}
			}
		}
	}

	// Token: 0x04001176 RID: 4470
	private ItemDrone itemDrone;

	// Token: 0x04001177 RID: 4471
	private PhysGrabObject myPhysGrabObject;

	// Token: 0x04001178 RID: 4472
	private ItemEquippable itemEquippable;

	// Token: 0x04001179 RID: 4473
	private ItemToggle itemToggle;

	// Token: 0x0400117A RID: 4474
	private ItemBattery itemBattery;

	// Token: 0x0400117B RID: 4475
	private ItemAttributes itemAttributes;

	// Token: 0x0400117C RID: 4476
	private float tumbleEnemyTimer;

	// Token: 0x0400117D RID: 4477
	private bool tumbledPlayer;
}
