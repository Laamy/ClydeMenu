using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000142 RID: 322
public class ItemDrone : MonoBehaviour
{
	// Token: 0x06000ADE RID: 2782 RVA: 0x00060064 File Offset: 0x0005E264
	private void Start()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.name == "Particles")
			{
				this.teleportParticles = transform.gameObject;
				break;
			}
		}
		this.customTargetingCondition = base.GetComponent<ITargetingCondition>();
		this.droneCollider = base.GetComponentInChildren<Collider>();
		this.cameraMain = Camera.main;
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemEquippable.itemEmoji = this.emojiIcon.ToString();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.rb = base.GetComponent<Rigidbody>();
		this.photonView = base.GetComponent<PhotonView>();
		this.lineBetweenTwoPoints = base.GetComponent<LineBetweenTwoPoints>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		if (!this.itemBattery)
		{
			this.hasBattery = false;
		}
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		this.emojiIcon = this.itemAttributes.emojiIcon;
		this.colorPreset = this.itemAttributes.colorPreset;
		this.droneColor = this.colorPreset.GetColorMain();
		this.batteryColor = this.colorPreset.GetColorLight();
		this.beamColor = this.colorPreset.GetColorDark();
		this.batteryDrainRate = this.batteryDrainPreset.GetBatteryDrainRate();
		this.itemBattery.batteryDrainRate = this.batteryDrainRate;
		this.itemBattery.batteryColor = this.batteryColor;
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		int num = 0;
		if (num < componentsInChildren.Length)
		{
			Collider collider = componentsInChildren[num];
			this.physicMaterialOriginal = collider.material;
		}
		Sound.CopySound(this.itemDroneSounds.DroneLoop, this.soundDroneLoop);
		Sound.CopySound(this.itemDroneSounds.DroneBeamLoop, this.soundDroneBeamLoop);
		ItemLight componentInChildren = base.GetComponentInChildren<ItemLight>();
		if (componentInChildren)
		{
			componentInChildren.itemLight.color = this.droneColor;
		}
		AudioSource component = base.GetComponent<AudioSource>();
		this.soundDroneLoop.Source = component;
		this.soundDroneBeamLoop.Source = component;
		foreach (object obj2 in base.transform)
		{
			Transform transform2 = (Transform)obj2;
			if (transform2.name == "Drone Icon")
			{
				this.onSwitchTransform = transform2;
				this.onSwitchTransform.GetComponent<Renderer>().material.SetTexture("_EmissionMap", this.droneIcon);
				this.onSwitchTransform.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.droneColor);
			}
			if (transform2.name == "Drone")
			{
				this.droneTransform = transform2;
				foreach (object obj3 in transform2)
				{
					Transform transform3 = (Transform)obj3;
					if (transform3.name.Contains("Drone Triangle"))
					{
						foreach (object obj4 in transform3)
						{
							Transform transform4 = (Transform)obj4;
							this.droneTriangleTransforms.Add(transform4);
						}
					}
					if (transform3.name.Contains("Drone Pyramid"))
					{
						foreach (object obj5 in transform3)
						{
							Transform transform5 = (Transform)obj5;
							this.dronePyramidTransforms.Add(transform5);
							transform5.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.droneColor);
						}
					}
				}
			}
		}
		this.droneTransform.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.droneColor);
		this.physGrabObject.clientNonKinematic = true;
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x00060508 File Offset: 0x0005E708
	private void AnimateDrone()
	{
		if (!this.itemActivated)
		{
			return;
		}
		this.lerpAnimationProgress += Time.deltaTime * 10f;
		if (this.lerpAnimationProgress > 1f)
		{
			this.lerpAnimationProgress = 1f;
			this.animationOpen = true;
		}
		float num = 15f;
		if (this.magnetActive)
		{
			num = 60f;
		}
		foreach (Transform transform in this.dronePyramidTransforms)
		{
			float num2 = -33f;
			if (this.lerpAnimationProgress != 1f)
			{
				transform.localRotation = Quaternion.Euler(0f, Mathf.Lerp(0f, num2, this.lerpAnimationProgress), 0f);
			}
			else
			{
				float num3 = Mathf.Sin(Time.time * num) * 5f;
				transform.localRotation = Quaternion.Euler(0f, num2 + num3, 0f);
			}
		}
		foreach (Transform transform2 in this.droneTriangleTransforms)
		{
			float num4 = 45f;
			if (this.lerpAnimationProgress != 1f)
			{
				transform2.localRotation = Quaternion.Euler(Mathf.Lerp(0f, num4, this.lerpAnimationProgress), 0f, 0f);
			}
			else
			{
				float num5 = Mathf.Sin(Time.time * num / 3f) * 10f;
				transform2.localRotation = Quaternion.Euler(num4 + num5, 0f, 0f);
			}
		}
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x000606C4 File Offset: 0x0005E8C4
	private bool TargetFindPlayer()
	{
		if (this.itemBattery.batteryLife <= 0f)
		{
			return false;
		}
		float num = 10000f;
		foreach (Collider collider in Physics.OverlapSphere(base.transform.position, 1f, LayerMask.GetMask(new string[]
		{
			"Player"
		})))
		{
			PlayerAvatar playerAvatar = collider.GetComponentInParent<PlayerAvatar>();
			if (!playerAvatar)
			{
				PlayerController componentInParent = collider.GetComponentInParent<PlayerController>();
				if (componentInParent)
				{
					playerAvatar = componentInParent.playerAvatarScript;
				}
			}
			if (playerAvatar && (this.customTargetingCondition == null || this.customTargetingCondition.CustomTargetingCondition(playerAvatar.gameObject)))
			{
				float num2 = Vector3.Distance(base.transform.position, playerAvatar.PlayerVisionTarget.VisionTransform.position);
				if (num2 < num)
				{
					num = num2;
					this.playerAvatarTarget = playerAvatar;
					this.targetIsPlayer = true;
					if (this.playerAvatarTarget.isLocal)
					{
						this.targetIsLocalPlayer = true;
					}
				}
			}
		}
		if (this.playerAvatarTarget)
		{
			Transform transform = this.playerAvatarTarget.PlayerVisionTarget.VisionTransform;
			Vector3 newAttachPoint = transform.position;
			if (this.playerAvatarTarget.isTumbling && this.playerAvatarTarget.transform)
			{
				this.playerTumbleTarget = this.playerAvatarTarget.tumble;
				transform = this.playerTumbleTarget.transform;
				newAttachPoint = this.playerTumbleTarget.physGrabObject.centerPoint;
			}
			this.targetIsLocalPlayer = this.playerAvatarTarget.isLocal;
			this.targetIsPlayer = true;
			this.NewRayHitPoint(newAttachPoint, this.playerAvatarTarget.GetComponent<PhotonView>().ViewID, -1, transform);
			this.attachPoint = this.rayHitPosition;
			this.ActivateMagnet();
			return true;
		}
		return false;
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x0006088C File Offset: 0x0005EA8C
	private void GetPlayerTumbleTarget()
	{
		if (!this.magnetTarget)
		{
			return;
		}
		if (this.playerTumbleTarget && !this.playerTumbleTarget.playerAvatar.isTumbling)
		{
			this.playerAvatarTarget = this.playerTumbleTarget.playerAvatar;
			this.targetIsLocalPlayer = this.playerAvatarTarget.isLocal;
			this.targetIsPlayer = true;
			this.ActivateMagnet();
			this.playerTumbleTarget = null;
			Transform visionTransform = this.playerAvatarTarget.PlayerVisionTarget.VisionTransform;
			Vector3 position = visionTransform.position;
			this.NewRayHitPoint(position, this.playerAvatarTarget.GetComponent<PhotonView>().ViewID, -1, visionTransform);
		}
		if (this.playerAvatarTarget && this.playerAvatarTarget.isTumbling)
		{
			this.playerTumbleTarget = this.playerAvatarTarget.tumble;
			this.targetIsLocalPlayer = false;
			this.targetIsPlayer = false;
			this.magnetTarget = this.playerTumbleTarget.transform;
			this.magnetTargetPhysGrabObject = this.playerTumbleTarget.physGrabObject;
			this.playerAvatarTarget = null;
			this.attachPoint = this.rayHitPosition;
			this.ActivateMagnet();
		}
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x000609A4 File Offset: 0x0005EBA4
	private void FullReset()
	{
		this.hadTarget = false;
		this.magnetTarget = null;
		this.magnetActivePrev = true;
		this.magnetActive = false;
		this.magnetTargetPhysGrabObject = null;
		this.magnetTargetRigidbody = null;
		this.DeactivateMagnet();
		this.playerTumbleTarget = null;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.rb.velocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
		}
		this.attachPoint = Vector3.zero;
		this.attachPointFound = false;
		this.rayHitPosition = Vector3.zero;
		this.animatedRayHitPosition = Vector3.zero;
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x00060A37 File Offset: 0x0005EC37
	private void ToggleOnFullInit()
	{
		if (!this.fullInit && this.itemActivated && !this.togglePrevious)
		{
			this.fullReset = false;
			this.fullInit = true;
			this.togglePrevious = true;
		}
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x00060A66 File Offset: 0x0005EC66
	private void ToggleOffFullReset()
	{
		if (!this.fullReset && !this.itemActivated && this.togglePrevious)
		{
			this.fullReset = true;
			this.fullInit = false;
			this.togglePrevious = false;
		}
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x00060A95 File Offset: 0x0005EC95
	private void ToggleOffIfLostTarget()
	{
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x00060A98 File Offset: 0x0005EC98
	private void ToggleOffIfEnemyTargetIsDead()
	{
		if (this.magnetTarget && this.targetIsEnemy)
		{
			if (this.enemyTarget)
			{
				if (!this.enemyTarget.Spawned && this.itemToggle.toggleState)
				{
					this.ForceTurnOff();
					this.enemyTarget = null;
					return;
				}
			}
			else if (this.itemToggle.toggleState)
			{
				this.ForceTurnOff();
				this.enemyTarget = null;
			}
		}
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x00060B0C File Offset: 0x0005ED0C
	private void ToggleOffIfPlayerTargetIsDead()
	{
		if (SemiFunc.FPSImpulse5() && this.targetIsPlayer)
		{
			if (this.playerAvatarTarget && this.playerAvatarTarget.isDisabled && this.itemToggle.toggleState)
			{
				this.ButtonToggleSet(false);
				this.playerAvatarTarget = null;
			}
			if (this.playerTumbleTarget && this.playerTumbleTarget.playerAvatar.isDisabled && this.itemToggle.toggleState)
			{
				this.ButtonToggleSet(false);
				this.playerTumbleTarget = null;
			}
		}
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x00060B97 File Offset: 0x0005ED97
	private void ForceTurnOff()
	{
		this.itemBattery.BatteryToggle(false);
		this.ButtonToggleSet(false);
		this.itemToggle.ToggleItem(false, -1);
		this.hadTarget = false;
		this.itemActivated = false;
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x00060BC8 File Offset: 0x0005EDC8
	private void Update()
	{
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		if (this.magnetActivePrev != this.magnetActive)
		{
			this.BatteryToggle(this.magnetActive);
			this.magnetActivePrev = this.magnetActive;
		}
		if (this.hadTarget && !this.magnetActive)
		{
			this.ForceTurnOff();
			return;
		}
		if (!SemiFunc.RunIsLevel() && !SemiFunc.RunIsLobby() && !SemiFunc.RunIsShop() && !SemiFunc.RunIsArena() && !SemiFunc.RunIsTutorial())
		{
			return;
		}
		this.soundDroneLoop.PlayLoop(this.itemActivated, 2f, 2f, 1f);
		this.AnimateDrone();
		if (!this.itemActivated)
		{
			this.onNoBatteryTimer = 0f;
		}
		if (this.itemActivated)
		{
			this.physGrabObject.impactDetector.canHurtLogic = false;
		}
		else
		{
			this.physGrabObject.impactDetector.canHurtLogic = true;
		}
		if (this.itemActivated && this.magnetActive && this.magnetTarget && !this.itemEquippable.isEquipped)
		{
			if (this.rayHitPosition != Vector3.zero && !this.targetIsPlayer)
			{
				bool flag = false;
				if (this.playerTumbleTarget && this.playerTumbleTarget.playerAvatar.isLocal)
				{
					flag = true;
				}
				if (!flag)
				{
					this.animatedRayHitPosition = Vector3.Lerp(this.animatedRayHitPosition, this.rayHitPosition, Time.deltaTime * 10f);
					this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, this.magnetTarget.TransformPoint(this.animatedRayHitPosition));
					this.connectionPoint = this.magnetTarget.TransformPoint(this.animatedRayHitPosition);
				}
				else
				{
					Vector3 b = new Vector3(0f, -0.5f, 0f);
					Vector3 point = this.cameraMain.transform.position + b;
					this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, point);
					this.connectionPoint = point;
				}
			}
			else
			{
				this.animatedRayHitPosition = Vector3.Lerp(this.animatedRayHitPosition, this.rayHitPosition, Time.deltaTime * 10f);
				if (!this.targetIsPlayer)
				{
					this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, this.magnetTargetPhysGrabObject.midPoint);
					this.connectionPoint = this.magnetTargetPhysGrabObject.midPoint;
				}
				if (this.targetIsPlayer)
				{
					Vector3 zero = new Vector3(0f, -0.5f, 0f);
					if (this.playerAvatarTarget.isTumbling)
					{
						zero = Vector3.zero;
					}
					if (!this.targetIsLocalPlayer)
					{
						this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, this.magnetTarget.position + zero);
						this.connectionPoint = this.magnetTarget.position + zero;
					}
					else
					{
						Vector3 point2 = this.cameraMain.transform.position + zero;
						this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, point2);
						this.connectionPoint = point2;
					}
				}
			}
		}
		if (!this.itemActivated && !this.magnetActive && this.animationOpen)
		{
			this.lerpAnimationProgress += Time.deltaTime * 10f;
			if (this.lerpAnimationProgress > 1f)
			{
				this.lerpAnimationProgress = 1f;
				this.animationOpen = false;
			}
			foreach (Transform transform in this.dronePyramidTransforms)
			{
				float b2 = 0f;
				transform.localRotation = Quaternion.Euler(0f, Mathf.Lerp(-33f, b2, this.lerpAnimationProgress), 0f);
			}
			foreach (Transform transform2 in this.droneTriangleTransforms)
			{
				float b3 = 0f;
				transform2.localRotation = Quaternion.Euler(Mathf.Lerp(45f, b3, this.lerpAnimationProgress), 0f, 0f);
			}
		}
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.GetPlayerTumbleTarget();
		if (this.itemToggle.toggleState != this.itemActivated)
		{
			this.ButtonToggle();
		}
		if (this.physGrabObject.playerGrabbing.Count == 1)
		{
			this.lastPlayerToTouch = this.physGrabObject.playerGrabbing[0].transform;
		}
		if (!this.itemActivated)
		{
			return;
		}
		this.springConstant = 40f;
		this.dampingCoefficient = 10f;
		if (!this.magnetActive)
		{
			this.checkTimer += Time.deltaTime;
			if (this.checkTimer > 0.5f)
			{
				bool flag2 = false;
				this.playerTumbleTarget = null;
				this.playerAvatarTarget = null;
				this.targetIsPlayer = false;
				this.targetIsEnemy = false;
				this.targetIsLocalPlayer = false;
				if (this.targetPlayers)
				{
					flag2 = this.TargetFindPlayer();
				}
				if (this.targetValuables || this.targetNonValuables || this.targetEnemies)
				{
					if (flag2)
					{
						this.SphereCheck(0.5f);
					}
					else
					{
						flag2 = this.SphereCheck(1f);
					}
				}
				if (flag2)
				{
					this.hadTarget = true;
					this.ActivateMagnet();
				}
				this.checkTimer = 0f;
			}
		}
		else if (!this.attachPointFound)
		{
			if (!this.targetIsPlayer)
			{
				if (this.rayTimer <= 0f)
				{
					this.FindBeamAttachPosition();
					this.rayTimer = 0.5f;
				}
				else
				{
					this.rayTimer -= Time.deltaTime;
				}
			}
		}
		else if (this.rb.velocity.magnitude > 0.2f)
		{
			this.newAttachPointTimer += Time.deltaTime;
			if (this.newAttachPointTimer > 0.5f)
			{
				this.attachPointFound = false;
				this.newAttachPointTimer = 0f;
				this.rayTimer = 0f;
			}
		}
		if (this.itemActivated && this.hasBattery && this.itemBattery.batteryLife <= 0f)
		{
			if (!this.itemBattery.batteryActive)
			{
				this.itemBattery.BatteryToggle(true);
			}
			this.onNoBatteryTimer += Time.deltaTime;
			if (this.onNoBatteryTimer >= 1.5f)
			{
				this.ForceTurnOff();
				this.onNoBatteryTimer = 0f;
			}
		}
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x0006125C File Offset: 0x0005F45C
	public void ButtonToggleSet(bool toggle)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.ButtonToggleRPC(toggle);
			return;
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("ButtonToggleRPC", RpcTarget.All, new object[]
			{
				toggle
			});
		}
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x00061294 File Offset: 0x0005F494
	private void FixedUpdate()
	{
		if (!this.itemActivated)
		{
			return;
		}
		if (this.magnetTarget)
		{
			ItemEquippable componentInParent = this.magnetTarget.GetComponentInParent<ItemEquippable>();
			if (componentInParent && componentInParent.isEquipped && this.magnetActive)
			{
				this.ForceTurnOff();
				this.DeactivateMagnet();
			}
		}
		if (this.itemEquippable.isEquipped)
		{
			if (this.magnetActive)
			{
				this.ForceTurnOff();
				this.DeactivateMagnet();
			}
			return;
		}
		if (!SemiFunc.RunIsLevel() && !SemiFunc.RunIsLobby() && !SemiFunc.RunIsShop() && !SemiFunc.RunIsArena() && !SemiFunc.RunIsTutorial())
		{
			return;
		}
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.itemActivated)
		{
			return;
		}
		if (this.magnetActive)
		{
			if (!this.magnetTarget)
			{
				this.DeactivateMagnet();
				return;
			}
			if (Vector3.Distance(base.transform.position, this.magnetTarget.position) > 4f)
			{
				this.FindTeleportSpot();
			}
			Collider collider = null;
			if (this.magnetTarget)
			{
				collider = this.magnetTarget.GetComponent<Collider>();
			}
			if (!this.playerTumbleTarget && (!this.magnetTarget || !this.magnetTarget.gameObject.activeSelf || !this.magnetTarget.gameObject.activeInHierarchy || (collider && !collider.enabled)))
			{
				this.DeactivateMagnet();
				return;
			}
			this.physGrabObject.OverrideMaterial(this.physicMaterialSlippery, 0.1f);
			if (this.randomNudgeTimer <= 0f)
			{
				if (Vector3.Distance(base.transform.position, this.magnetTarget.transform.position) > 1.5f)
				{
					Vector3 lhs = base.transform.position - this.magnetTarget.transform.position;
					Vector3[] array = new Vector3[]
					{
						Vector3.up,
						Vector3.down,
						Vector3.left,
						Vector3.right
					};
					Vector3 rhs = array[Random.Range(0, array.Length)];
					Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
					if (normalized != Vector3.zero)
					{
						this.rb.AddForce(normalized * 1f, ForceMode.Impulse);
						this.rb.AddTorque(normalized * 10f, ForceMode.Impulse);
					}
				}
				this.randomNudgeTimer = 0.5f;
			}
			else
			{
				this.randomNudgeTimer -= Time.fixedDeltaTime;
			}
			if (this.attachPointFound)
			{
				Vector3 a = this.magnetTarget.TransformPoint(this.attachPoint) - base.transform.position;
				Vector3 a2 = this.springConstant * a;
				Vector3 velocity = this.rb.velocity;
				Vector3 b = -this.dampingCoefficient * velocity;
				Vector3 vector = a2 + b;
				vector = Vector3.ClampMagnitude(vector, 20f);
				this.rb.AddForce(vector);
				if (!this.magnetTarget.gameObject.activeSelf)
				{
					this.DeactivateMagnet();
				}
				SemiFunc.PhysLookAtPositionWithForce(this.rb, base.transform, this.magnetTarget.TransformPoint(this.rayHitPosition), 1f);
				return;
			}
			Vector3 vector2 = this.magnetTarget.position - base.transform.position;
			if (vector2.magnitude > 0.8f)
			{
				this.rb.AddForce(vector2.normalized * 3f);
			}
			else
			{
				Vector3 force = -this.rb.velocity * 0.9f;
				this.rb.AddForce(force);
			}
			if (!this.magnetTarget.gameObject.activeSelf)
			{
				this.DeactivateMagnet();
			}
			SemiFunc.PhysLookAtPositionWithForce(this.rb, base.transform, this.magnetTarget.position, 1f);
		}
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x00061694 File Offset: 0x0005F894
	[PunRPC]
	public void TeleportEffectRPC(Vector3 startPosition, Vector3 endPosition)
	{
		this.itemDroneSounds.DroneRetract.Pitch = 3f;
		this.itemDroneSounds.DroneRetract.Play(startPosition, 1f, 1f, 1f, 1f);
		this.itemDroneSounds.DroneRetract.Pitch = 4f;
		this.itemDroneSounds.DroneRetract.Play(endPosition, 1f, 1f, 1f, 1f);
		Object.Instantiate<GameObject>(this.teleportParticles, startPosition, Quaternion.identity);
		Object.Instantiate<GameObject>(this.teleportParticles, endPosition, Quaternion.identity);
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0006173C File Offset: 0x0005F93C
	private void TeleportEffect(Vector3 startPosition, Vector3 endPosition)
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("TeleportEffectRPC", RpcTarget.All, new object[]
				{
					startPosition,
					endPosition
				});
				return;
			}
		}
		else
		{
			this.TeleportEffectRPC(startPosition, endPosition);
		}
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0006178C File Offset: 0x0005F98C
	private void FindTeleportSpot()
	{
		if (!this.magnetActive)
		{
			return;
		}
		if (!this.magnetTarget)
		{
			return;
		}
		if (this.teleportSpotTimer > 0f)
		{
			this.teleportSpotTimer -= Time.deltaTime;
			return;
		}
		ItemEquippable componentInParent = this.magnetTarget.GetComponentInParent<ItemEquippable>();
		if (componentInParent && componentInParent.isEquipped)
		{
			return;
		}
		Vector3 vector = this.magnetTarget.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
		int i = 0;
		while (i < 10)
		{
			vector = this.magnetTarget.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			float num = Vector3.Distance(vector, this.magnetTarget.position);
			float maxDistance = Mathf.Max(0f, num - 0.2f);
			RaycastHit[] array = Physics.RaycastAll(vector, this.magnetTarget.position - vector, maxDistance, SemiFunc.LayerMaskGetVisionObstruct());
			bool flag = false;
			foreach (RaycastHit raycastHit in array)
			{
				if (raycastHit.transform.GetComponentInParent<Rigidbody>() != this.magnetTarget.GetComponentInParent<Rigidbody>() && raycastHit.transform != base.transform)
				{
					flag = true;
					break;
				}
			}
			if (!flag && Physics.OverlapBox(vector, new Vector3(0.2f, 0.2f, 0.2f), base.transform.rotation, SemiFunc.LayerMaskGetVisionObstruct()).Length == 0)
			{
				this.TeleportEffect(base.transform.position, vector);
				if (SemiFunc.IsMultiplayer())
				{
					this.physGrabObject.photonTransformView.Teleport(vector, base.transform.rotation);
					break;
				}
				base.transform.position = vector;
				break;
			}
			else
			{
				i++;
			}
		}
		this.teleportSpotTimer = 0.2f;
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x000619A8 File Offset: 0x0005FBA8
	private void DeactivateMagnet()
	{
		if (!this.magnetActive)
		{
			return;
		}
		this.attachPointFound = false;
		this.playerAvatarTarget = null;
		this.targetIsPlayer = false;
		this.targetIsLocalPlayer = false;
		this.targetIsEnemy = false;
		this.playerTumbleTarget = null;
		this.magnetTargetPhysGrabObject = null;
		this.magnetTargetRigidbody = null;
		this.MagnetActiveToggle(false);
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x000619FD File Offset: 0x0005FBFD
	private void ActivateMagnet()
	{
		if (this.magnetActive)
		{
			return;
		}
		this.MagnetActiveToggle(true);
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x00061A0F File Offset: 0x0005FC0F
	private void BatteryToggle(bool activated)
	{
		if (this.hasBattery)
		{
			this.itemBattery.batteryActive = activated;
		}
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x00061A28 File Offset: 0x0005FC28
	private void ButtonToggleLogic(bool activated)
	{
		this.FullReset();
		this.MagnetActiveToggle(activated);
		this.droneOwner = SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID);
		this.lerpAnimationProgress = 0f;
		if (activated)
		{
			this.onSwitchTransform.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.droneColor);
			this.itemDroneSounds.DroneStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		else
		{
			if (this.magnetActive)
			{
				this.DeactivateMagnet();
			}
			this.itemDroneSounds.DroneEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		this.itemActivated = activated;
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x00061B00 File Offset: 0x0005FD00
	public void ButtonToggle()
	{
		this.itemActivated = !this.itemActivated;
		if (GameManager.instance.gameMode == 0)
		{
			this.ButtonToggleLogic(this.itemActivated);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("ButtonToggleRPC", RpcTarget.All, new object[]
			{
				this.itemActivated
			});
		}
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x00061B61 File Offset: 0x0005FD61
	[PunRPC]
	private void ButtonToggleRPC(bool activated)
	{
		this.ButtonToggleLogic(activated);
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x00061B6C File Offset: 0x0005FD6C
	private Transform GetHighestParentWithRigidbody(Transform child)
	{
		if (base.GetComponent<Rigidbody>() != null && child.GetComponent<PhotonView>() != null)
		{
			return child;
		}
		Transform transform = child;
		while (transform.parent != null)
		{
			if (transform.parent.GetComponent<Rigidbody>() != null && transform.parent.GetComponent<PhotonView>() != null)
			{
				return transform.parent;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x00061BE0 File Offset: 0x0005FDE0
	private void MagnetActiveToggleLogic(bool activated)
	{
		this.magnetActive = activated;
		this.lerpAnimationProgress = 0f;
		if (!activated)
		{
			this.itemDroneSounds.DroneRetract.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.rayHitPosition = Vector3.zero;
			return;
		}
		this.itemDroneSounds.DroneDeploy.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x00061C6E File Offset: 0x0005FE6E
	public void MagnetActiveToggle(bool toggleBool)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.MagnetActiveToggleLogic(toggleBool);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("MagnetActiveToggleRPC", RpcTarget.All, new object[]
			{
				toggleBool
			});
		}
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x00061CAB File Offset: 0x0005FEAB
	[PunRPC]
	private void MagnetActiveToggleRPC(bool activated)
	{
		this.MagnetActiveToggleLogic(activated);
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x00061CB4 File Offset: 0x0005FEB4
	private void NewRayHitPointLogic(Vector3 newRayHitPosition, int photonViewId, int colliderID, Transform newMagnetTarget)
	{
		if (newMagnetTarget)
		{
			this.magnetTargetPhysGrabObject = newMagnetTarget.GetComponent<PhysGrabObject>();
			if (colliderID != -1)
			{
				this.magnetTarget = newMagnetTarget.GetComponent<PhysGrabObject>().FindColliderFromID(colliderID);
				this.targetIsPlayer = false;
				this.targetIsLocalPlayer = false;
			}
			else
			{
				this.magnetTarget = newMagnetTarget;
			}
			this.animatedRayHitPosition = this.rayHitPosition;
			this.rayHitPosition = this.magnetTarget.InverseTransformPoint(newRayHitPosition);
			this.magnetTargetRigidbody = this.GetHighestParentWithRigidbody(this.magnetTarget).GetComponent<Rigidbody>();
			PlayerTumble component = this.magnetTargetRigidbody.GetComponent<PlayerTumble>();
			if (component)
			{
				if (component.isTumbling)
				{
					this.playerTumbleTarget = component;
					return;
				}
				this.DeactivateMagnet();
				return;
			}
		}
		else
		{
			this.magnetTargetPhysGrabObject = PhotonView.Find(photonViewId).gameObject.GetComponent<PhysGrabObject>();
			if (colliderID != -1)
			{
				this.magnetTarget = PhotonView.Find(photonViewId).gameObject.GetComponent<PhysGrabObject>().FindColliderFromID(colliderID);
				this.targetIsPlayer = false;
				this.targetIsLocalPlayer = false;
			}
			else
			{
				this.targetIsPlayer = true;
				this.playerAvatarTarget = PhotonView.Find(photonViewId).GetComponent<PlayerAvatar>();
				this.magnetTarget = this.playerAvatarTarget.PlayerVisionTarget.VisionTransform;
				this.targetIsLocalPlayer = this.playerAvatarTarget.isLocal;
				if (PhotonView.Find(photonViewId).GetComponent<PlayerAvatar>().isLocal)
				{
					this.targetIsLocalPlayer = true;
				}
			}
			this.animatedRayHitPosition = this.rayHitPosition;
			this.rayHitPosition = this.magnetTarget.InverseTransformPoint(newRayHitPosition);
			this.magnetTargetRigidbody = this.GetHighestParentWithRigidbody(this.magnetTarget).GetComponent<Rigidbody>();
		}
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x00061E40 File Offset: 0x00060040
	private void NewRayHitPoint(Vector3 newAttachPoint, int photonViewId, int colliderID, Transform newMagnetTarget)
	{
		if (!GameManager.Multiplayer())
		{
			this.NewRayHitPointLogic(newAttachPoint, photonViewId, colliderID, newMagnetTarget);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("NewRayHitPointRPC", RpcTarget.All, new object[]
			{
				newAttachPoint,
				photonViewId,
				colliderID
			});
		}
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x00061E99 File Offset: 0x00060099
	[PunRPC]
	private void NewRayHitPointRPC(Vector3 newAttachPoint, int photonViewId, int colliderID)
	{
		this.NewRayHitPointLogic(newAttachPoint, photonViewId, colliderID, null);
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x00061EA8 File Offset: 0x000600A8
	private bool SphereCheck(float _radius)
	{
		bool flag = false;
		if (this.itemBattery.batteryLife <= 0f)
		{
			return false;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, _radius);
		Transform transform = null;
		PhysGrabObject physGrabObject = null;
		Rigidbody rigidbody = null;
		Transform transform2 = null;
		int colliderID = -1;
		bool flag2 = false;
		EnemyParent enemyParent = null;
		PlayerTumble playerTumble = null;
		float num = 10000f;
		foreach (Collider collider in array)
		{
			Transform highestParentWithRigidbody = this.GetHighestParentWithRigidbody(collider.transform);
			PhysGrabObjectCollider component = collider.GetComponent<PhysGrabObjectCollider>();
			PhysGrabObject physGrabObject2 = null;
			bool flag3 = false;
			bool flag4 = false;
			EnemyParent enemyParent2 = null;
			PlayerTumble componentInParent = collider.GetComponentInParent<PlayerTumble>();
			if (highestParentWithRigidbody)
			{
				PhysGrabObjectImpactDetector component2 = highestParentWithRigidbody.GetComponent<PhysGrabObjectImpactDetector>();
				physGrabObject2 = highestParentWithRigidbody.GetComponent<PhysGrabObject>();
				if (component2)
				{
					if (component2.isValuable)
					{
						flag4 = true;
					}
					if (component2.isEnemy)
					{
						flag3 = true;
						enemyParent2 = component2.GetComponentInParent<EnemyParent>();
					}
				}
			}
			bool flag5 = true;
			if (this.customTargetingCondition != null && highestParentWithRigidbody)
			{
				flag5 = this.customTargetingCondition.CustomTargetingCondition(highestParentWithRigidbody.gameObject);
			}
			bool flag6 = this.targetValuables && flag4;
			if (!flag6)
			{
				flag6 = (this.targetEnemies && flag3);
			}
			if (!flag6)
			{
				flag6 = (this.targetNonValuables && !flag4);
			}
			if (component && highestParentWithRigidbody != base.transform && highestParentWithRigidbody && flag6 && flag5 && highestParentWithRigidbody.gameObject.activeSelf)
			{
				float num2 = Vector3.Distance(base.transform.position, physGrabObject2.centerPoint);
				if (num2 < num)
				{
					bool flag7 = false;
					RaycastHit raycastHit;
					if (Physics.Raycast(base.transform.position, physGrabObject2.centerPoint - base.transform.position, out raycastHit, (physGrabObject2.centerPoint - base.transform.position).magnitude, LayerMask.GetMask(new string[]
					{
						"Default"
					})) && raycastHit.collider.transform != collider.transform && raycastHit.collider.transform != base.transform)
					{
						flag7 = true;
					}
					if (!flag7)
					{
						num = num2;
						transform = collider.transform;
						physGrabObject = physGrabObject2;
						rigidbody = highestParentWithRigidbody.GetComponent<Rigidbody>();
						transform2 = highestParentWithRigidbody;
						colliderID = component.colliderID;
						flag2 = flag3;
						enemyParent = enemyParent2;
						playerTumble = componentInParent;
						flag = true;
					}
				}
			}
		}
		if (flag)
		{
			this.playerTumbleTarget = playerTumble;
			this.playerAvatarTarget = null;
			this.targetIsPlayer = false;
			this.targetIsEnemy = false;
			this.targetIsLocalPlayer = false;
			this.magnetTarget = transform;
			this.magnetTargetPhysGrabObject = physGrabObject;
			this.magnetTargetRigidbody = rigidbody;
			this.NewRayHitPoint(transform.position, transform2.GetComponent<PhotonView>().ViewID, colliderID, transform2);
			this.attachPoint = this.rayHitPosition;
			this.targetIsEnemy = flag2;
			this.enemyTarget = enemyParent;
		}
		return flag;
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0006218C File Offset: 0x0006038C
	private void FindBeamAttachPosition()
	{
		if (!this.magnetTarget)
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			float num = 0.5f;
			Vector3 b = new Vector3(Random.Range(-num, num), Random.Range(-num, num), Random.Range(-num, num));
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, this.magnetTarget.position - base.transform.position + b, out raycastHit, 1f, SemiFunc.LayerMaskGetPhysGrabObject()))
			{
				Transform highestParentWithRigidbody = this.GetHighestParentWithRigidbody(raycastHit.collider.transform);
				PhysGrabObjectCollider component = raycastHit.collider.transform.GetComponent<PhysGrabObjectCollider>();
				if (component && highestParentWithRigidbody == this.magnetTargetPhysGrabObject.transform)
				{
					Vector3 normalized = (base.transform.position - raycastHit.point).normalized;
					this.NewRayHitPoint(raycastHit.point, highestParentWithRigidbody.GetComponent<PhotonView>().ViewID, component.colliderID, highestParentWithRigidbody);
					Vector3 position = raycastHit.point + normalized * 0.5f;
					this.attachPoint = this.magnetTarget.InverseTransformPoint(position);
					this.attachPointFound = true;
				}
			}
		}
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x000622E4 File Offset: 0x000604E4
	public void SetTumbleTarget(PlayerTumble tumble)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.SetTumbleTargetRPC(tumble.photonView.ViewID);
			return;
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("SetTumbleTargetRPC", RpcTarget.All, new object[]
			{
				tumble.photonView.ViewID
			});
		}
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x0006233C File Offset: 0x0006053C
	[PunRPC]
	public void SetTumbleTargetRPC(int _photonViewID)
	{
		PhotonView photonView = PhotonView.Find(_photonViewID);
		if (photonView)
		{
			PlayerTumble component = photonView.GetComponent<PlayerTumble>();
			if (component)
			{
				this.playerTumbleTarget = component;
			}
		}
	}

	// Token: 0x04001183 RID: 4483
	public GameObject teleportParticles;

	// Token: 0x04001184 RID: 4484
	private ItemAttributes itemAttributes;

	// Token: 0x04001185 RID: 4485
	internal SemiFunc.emojiIcon emojiIcon;

	// Token: 0x04001186 RID: 4486
	public Texture droneIcon;

	// Token: 0x04001187 RID: 4487
	internal ColorPresets colorPreset;

	// Token: 0x04001188 RID: 4488
	public BatteryDrainPresets batteryDrainPreset;

	// Token: 0x04001189 RID: 4489
	internal float batteryDrainRate = 0.1f;

	// Token: 0x0400118A RID: 4490
	internal Color droneColor;

	// Token: 0x0400118B RID: 4491
	internal Color batteryColor;

	// Token: 0x0400118C RID: 4492
	internal Color beamColor;

	// Token: 0x0400118D RID: 4493
	private float checkTimer;

	// Token: 0x0400118E RID: 4494
	private Transform magnetTarget;

	// Token: 0x0400118F RID: 4495
	internal PhysGrabObject magnetTargetPhysGrabObject;

	// Token: 0x04001190 RID: 4496
	internal Rigidbody magnetTargetRigidbody;

	// Token: 0x04001191 RID: 4497
	internal bool magnetActive;

	// Token: 0x04001192 RID: 4498
	public PlayerTumble playerTumbleTarget;

	// Token: 0x04001193 RID: 4499
	private Rigidbody rb;

	// Token: 0x04001194 RID: 4500
	private bool attachPointFound;

	// Token: 0x04001195 RID: 4501
	private Vector3 attachPoint;

	// Token: 0x04001196 RID: 4502
	private float springConstant = 50f;

	// Token: 0x04001197 RID: 4503
	private float dampingCoefficient = 5f;

	// Token: 0x04001198 RID: 4504
	private float newAttachPointTimer;

	// Token: 0x04001199 RID: 4505
	internal bool itemActivated;

	// Token: 0x0400119A RID: 4506
	private PhotonView photonView;

	// Token: 0x0400119B RID: 4507
	private Vector3 rayHitPosition;

	// Token: 0x0400119C RID: 4508
	private Vector3 animatedRayHitPosition;

	// Token: 0x0400119D RID: 4509
	private LineBetweenTwoPoints lineBetweenTwoPoints;

	// Token: 0x0400119E RID: 4510
	public Transform lineStartPoint;

	// Token: 0x0400119F RID: 4511
	private float rayTimer;

	// Token: 0x040011A0 RID: 4512
	private Transform prevMagnetTarget;

	// Token: 0x040011A1 RID: 4513
	private Transform droneTransform;

	// Token: 0x040011A2 RID: 4514
	private List<Transform> dronePyramidTransforms = new List<Transform>();

	// Token: 0x040011A3 RID: 4515
	private List<Transform> droneTriangleTransforms = new List<Transform>();

	// Token: 0x040011A4 RID: 4516
	private float lerpAnimationProgress;

	// Token: 0x040011A5 RID: 4517
	private bool hasBattery = true;

	// Token: 0x040011A6 RID: 4518
	private ItemBattery itemBattery;

	// Token: 0x040011A7 RID: 4519
	private float onNoBatteryTimer;

	// Token: 0x040011A8 RID: 4520
	private bool animationOpen;

	// Token: 0x040011A9 RID: 4521
	private Transform onSwitchTransform;

	// Token: 0x040011AA RID: 4522
	public ItemDroneSounds itemDroneSounds;

	// Token: 0x040011AB RID: 4523
	internal Sound soundDroneLoop = new Sound();

	// Token: 0x040011AC RID: 4524
	internal Sound soundDroneBeamLoop = new Sound();

	// Token: 0x040011AD RID: 4525
	public PhysicMaterial physicMaterialSlippery;

	// Token: 0x040011AE RID: 4526
	public bool targetValuables;

	// Token: 0x040011AF RID: 4527
	public bool targetPlayers;

	// Token: 0x040011B0 RID: 4528
	public bool targetEnemies;

	// Token: 0x040011B1 RID: 4529
	public bool targetNonValuables;

	// Token: 0x040011B2 RID: 4530
	internal Vector3 connectionPoint;

	// Token: 0x040011B3 RID: 4531
	internal Transform lastPlayerToTouch;

	// Token: 0x040011B4 RID: 4532
	private PhysGrabObject physGrabObject;

	// Token: 0x040011B5 RID: 4533
	private float randomNudgeTimer;

	// Token: 0x040011B6 RID: 4534
	private Collider droneCollider;

	// Token: 0x040011B7 RID: 4535
	private PhysicMaterial physicMaterialOriginal;

	// Token: 0x040011B8 RID: 4536
	private ItemToggle itemToggle;

	// Token: 0x040011B9 RID: 4537
	internal PlayerAvatar playerAvatarTarget;

	// Token: 0x040011BA RID: 4538
	private bool targetIsPlayer;

	// Token: 0x040011BB RID: 4539
	internal bool targetIsLocalPlayer;

	// Token: 0x040011BC RID: 4540
	private ItemEquippable itemEquippable;

	// Token: 0x040011BD RID: 4541
	private Camera cameraMain;

	// Token: 0x040011BE RID: 4542
	internal PlayerAvatar droneOwner;

	// Token: 0x040011BF RID: 4543
	private float teleportSpotTimer;

	// Token: 0x040011C0 RID: 4544
	private bool hadTarget;

	// Token: 0x040011C1 RID: 4545
	private bool targetIsEnemy;

	// Token: 0x040011C2 RID: 4546
	private bool togglePrevious;

	// Token: 0x040011C3 RID: 4547
	private bool fullReset;

	// Token: 0x040011C4 RID: 4548
	private bool fullInit;

	// Token: 0x040011C5 RID: 4549
	private EnemyParent enemyTarget;

	// Token: 0x040011C6 RID: 4550
	private bool magnetActivePrev;

	// Token: 0x040011C7 RID: 4551
	private ITargetingCondition customTargetingCondition;
}
