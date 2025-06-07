using System;
using UnityEngine;

// Token: 0x02000164 RID: 356
public class ItemMineTrigger : MonoBehaviour
{
	// Token: 0x06000C21 RID: 3105 RVA: 0x0006B084 File Offset: 0x00069284
	private void Start()
	{
		this.parentPhysGrabObject = base.GetComponentInParent<PhysGrabObject>();
		this.itemMine = base.GetComponentInParent<ItemMine>();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			Object.Destroy(this);
			return;
		}
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x0006B0AC File Offset: 0x000692AC
	private void OnTriggerEnter(Collider other)
	{
		if (this.targetAcquired || !this.itemMine || this.itemMine.state != ItemMine.States.Armed)
		{
			return;
		}
		if (!this.PassesTriggerChecks(other))
		{
			return;
		}
		this.TryAcquireTarget(other);
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x0006B0E4 File Offset: 0x000692E4
	private void OnTriggerStay(Collider other)
	{
		if (this.targetAcquired || !this.itemMine || this.itemMine.state != ItemMine.States.Armed)
		{
			return;
		}
		if (!this.PassesTriggerChecks(other))
		{
			return;
		}
		this.visionCheckTimer += Time.deltaTime;
		if (this.visionCheckTimer > 0.5f)
		{
			this.visionCheckTimer = 0f;
			this.TryAcquireTarget(other);
		}
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x0006B150 File Offset: 0x00069350
	private bool PassesTriggerChecks(Collider other)
	{
		PhysGrabObject componentInParent = other.GetComponentInParent<PhysGrabObject>();
		if (this.enemyTrigger)
		{
			if (!componentInParent || !componentInParent.isEnemy)
			{
				return false;
			}
		}
		else if (componentInParent && componentInParent.isEnemy && !this.itemMine.triggeredByEnemies)
		{
			return false;
		}
		if (componentInParent && !this.itemMine.triggeredByRigidBodies && !componentInParent.isEnemy && !componentInParent.isPlayer)
		{
			return false;
		}
		PlayerAvatar exists = other.GetComponentInParent<PlayerAvatar>();
		PlayerController componentInParent2 = other.GetComponentInParent<PlayerController>();
		if (componentInParent2)
		{
			exists = componentInParent2.playerAvatarScript;
		}
		return (!componentInParent || this.itemMine.triggeredByPlayers || (!componentInParent.isPlayer && !exists)) && (!componentInParent || componentInParent.isEnemy || componentInParent.grabbed || componentInParent.rb.velocity.magnitude >= 0.1f || componentInParent.rb.angularVelocity.magnitude >= 0.1f) && (!(componentInParent ? componentInParent.GetComponent<PlayerTumble>() : null) || this.itemMine.triggeredByPlayers);
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x0006B280 File Offset: 0x00069480
	private void TryAcquireTarget(Collider other)
	{
		if (this.targetAcquired)
		{
			return;
		}
		PhysGrabObject componentInParent = other.GetComponentInParent<PhysGrabObject>();
		PlayerAvatar playerAvatar = other.GetComponentInParent<PlayerAvatar>();
		PlayerAccess componentInParent2 = other.GetComponentInParent<PlayerAccess>();
		PlayerController playerController = componentInParent2 ? componentInParent2.GetComponentInChildren<PlayerController>() : null;
		Vector3 position = this.itemMine.transform.position;
		if (componentInParent)
		{
			Vector3 midPoint = componentInParent.midPoint;
			if (!this.VisionObstruct(position, midPoint, componentInParent))
			{
				if (componentInParent.isEnemy)
				{
					this.LockOnTarget(ItemMineTrigger.TargetType.Enemy, componentInParent, playerAvatar, playerController);
					return;
				}
				if (!componentInParent.isPlayer && componentInParent != this.parentPhysGrabObject)
				{
					this.LockOnTarget(ItemMineTrigger.TargetType.RigidBody, componentInParent, playerAvatar, playerController);
					return;
				}
			}
		}
		if (playerAvatar)
		{
			Vector3 position2 = playerAvatar.PlayerVisionTarget.VisionTransform.position;
			if (!this.VisionObstruct(position, position2, null))
			{
				this.LockOnTarget(ItemMineTrigger.TargetType.Player, componentInParent, playerAvatar, playerController);
				return;
			}
		}
		if (playerController)
		{
			playerAvatar = playerController.playerAvatarScript;
			if (playerAvatar)
			{
				Vector3 position3 = playerAvatar.PlayerVisionTarget.VisionTransform.position;
				if (!this.VisionObstruct(position, position3, null))
				{
					this.LockOnTarget(ItemMineTrigger.TargetType.Player, componentInParent, playerAvatar, playerController);
					return;
				}
			}
		}
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x0006B394 File Offset: 0x00069594
	private void LockOnTarget(ItemMineTrigger.TargetType type, PhysGrabObject physObj, PlayerAvatar playerAvatar, PlayerController playerController)
	{
		if (!this.itemMine)
		{
			return;
		}
		switch (type)
		{
		case ItemMineTrigger.TargetType.Enemy:
			this.itemMine.wasTriggeredByEnemy = true;
			this.itemMine.triggeredPhysGrabObject = physObj;
			this.itemMine.triggeredTransform = physObj.transform;
			this.itemMine.triggeredPosition = physObj.transform.position;
			break;
		case ItemMineTrigger.TargetType.RigidBody:
			this.itemMine.wasTriggeredByRigidBody = true;
			this.itemMine.triggeredPhysGrabObject = physObj;
			this.itemMine.triggeredTransform = physObj.transform;
			this.itemMine.triggeredPosition = physObj.transform.position;
			break;
		case ItemMineTrigger.TargetType.Player:
			this.itemMine.wasTriggeredByPlayer = true;
			if (playerAvatar)
			{
				this.itemMine.triggeredPlayerAvatar = playerAvatar;
				PlayerTumble tumble = playerAvatar.tumble;
				if (tumble)
				{
					this.itemMine.triggeredPlayerTumble = tumble;
					this.itemMine.triggeredPhysGrabObject = tumble.physGrabObject;
				}
				this.itemMine.triggeredTransform = playerAvatar.PlayerVisionTarget.VisionTransform;
				this.itemMine.triggeredPosition = playerAvatar.PlayerVisionTarget.VisionTransform.position;
			}
			else if (physObj)
			{
				PlayerTumble componentInParent = physObj.GetComponentInParent<PlayerTumble>();
				if (componentInParent)
				{
					this.itemMine.triggeredPlayerAvatar = componentInParent.playerAvatar;
					this.itemMine.triggeredPlayerTumble = componentInParent;
					this.itemMine.triggeredPhysGrabObject = componentInParent.physGrabObject;
					this.itemMine.triggeredTransform = componentInParent.playerAvatar.PlayerVisionTarget.VisionTransform;
					this.itemMine.triggeredPosition = componentInParent.playerAvatar.PlayerVisionTarget.VisionTransform.position;
				}
			}
			break;
		}
		this.targetAcquired = true;
		this.itemMine.SetTriggered();
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x0006B564 File Offset: 0x00069764
	private bool VisionObstruct(Vector3 start, Vector3 end, PhysGrabObject targetPhysObj)
	{
		int layerMask = SemiFunc.LayerMaskGetVisionObstruct();
		Vector3 normalized = (end - start).normalized;
		float maxDistance = Vector3.Distance(start, end);
		foreach (RaycastHit raycastHit in Physics.RaycastAll(start, normalized, maxDistance, layerMask))
		{
			if (raycastHit.collider.CompareTag("Wall") || raycastHit.collider.CompareTag("Ceiling"))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040013AA RID: 5034
	private PhysGrabObject parentPhysGrabObject;

	// Token: 0x040013AB RID: 5035
	private ItemMine itemMine;

	// Token: 0x040013AC RID: 5036
	public bool enemyTrigger;

	// Token: 0x040013AD RID: 5037
	private bool targetAcquired;

	// Token: 0x040013AE RID: 5038
	private float visionCheckTimer;

	// Token: 0x02000388 RID: 904
	private enum TargetType
	{
		// Token: 0x04002B64 RID: 11108
		None,
		// Token: 0x04002B65 RID: 11109
		Enemy,
		// Token: 0x04002B66 RID: 11110
		RigidBody,
		// Token: 0x04002B67 RID: 11111
		Player
	}
}
