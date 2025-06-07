using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001A3 RID: 419
public class PhysGrabObject : MonoBehaviour, IPunObservable
{
	// Token: 0x06000E0D RID: 3597 RVA: 0x0007CD74 File Offset: 0x0007AF74
	private void Awake()
	{
		this.photonTransformView = base.GetComponent<PhotonTransformView>();
		if (!this.photonTransformView)
		{
			Debug.LogError("No Photon Transform View found on " + base.gameObject.name);
		}
		this.physGrabCart = base.GetComponent<PhysGrabCart>();
		this.isCart = this.physGrabCart;
		this.forceGrabPoint = base.transform.Find("Force Grab Point");
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.isKinematic = true;
		Transform transform = base.transform.Find("Center of Mass");
		if (transform)
		{
			this.rb.centerOfMass = transform.localPosition;
		}
		this.rb.interpolation = RigidbodyInterpolation.Interpolate;
		this.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		this.angularDragOriginal = this.rb.angularDrag;
		this.dragOriginal = this.rb.drag;
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		if (this.impactDetector)
		{
			this.hasImpactDetector = true;
		}
		if (base.GetComponent<ValuableObject>())
		{
			this.isValuable = true;
		}
		this.mapCustom = base.GetComponent<MapCustom>();
		if (this.mapCustom)
		{
			this.hasMapCustom = true;
		}
		foreach (Transform transform2 in base.GetComponentsInParent<Transform>())
		{
			if (transform2.name.Contains("debug") || transform2.name.Contains("Debug"))
			{
				this.spawned = true;
			}
		}
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x0007CEFC File Offset: 0x0007B0FC
	public void TurnXYZ(Quaternion turnX, Quaternion turnY, Quaternion turnZ)
	{
		Vector3 point = turnY * Vector3.forward;
		Vector3 point2 = turnY * Vector3.up;
		point = turnZ * point;
		point2 = turnZ * point2;
		foreach (PhysGrabber physGrabber in this.playerGrabbing)
		{
			physGrabber.cameraRelativeGrabbedForward = turnX * point;
			physGrabber.cameraRelativeGrabbedUp = turnX * point2;
		}
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x0007CF88 File Offset: 0x0007B188
	public void TorqueToTarget(PhysGrabber player, Quaternion target, float strength, float dampen)
	{
		if (this.rb.isKinematic)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		Vector3 forward = base.transform.forward;
		Vector3 up = base.transform.up;
		Vector3 vector2 = target * Vector3.forward;
		Vector3 vector3 = target * Vector3.up;
		player.cameraRelativeGrabbedUp = vector3;
		player.cameraRelativeGrabbedForward = vector2;
		Vector3 vector4 = Vector3.Cross(forward, vector2);
		if (vector4.sqrMagnitude > 1E-08f)
		{
			float value = Vector3.Angle(forward, vector2);
			vector += vector4.normalized * Mathf.Clamp(value, 0f, 60f);
		}
		Vector3 vector5 = Vector3.Cross(up, vector3);
		if (vector5.sqrMagnitude > 1E-08f)
		{
			float value2 = Vector3.Angle(up, vector3);
			vector += vector5.normalized * Mathf.Clamp(value2, 0f, 60f);
		}
		vector *= this.rb.mass;
		vector = Vector3.ClampMagnitude(vector, 60f).normalized;
		if (this.rb.mass < 1f)
		{
			vector *= 0.75f;
		}
		if (this.rb.drag == 0f)
		{
			this.rb.drag = 0.05f;
		}
		if (this.rb.angularDrag == 0f)
		{
			this.rb.angularDrag = 0.05f;
		}
		float num = Vector3.Angle(base.transform.forward, vector2) / 180f;
		float num2 = Vector3.Angle(base.transform.up, vector3) / 180f;
		float num3 = Mathf.Clamp01(this.rb.mass);
		float d = (num + num2) * dampen * num3;
		vector *= strength;
		vector *= d;
		Vector3 torque = vector * num3;
		this.rb.AddTorque(torque, ForceMode.Impulse);
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x0007D174 File Offset: 0x0007B374
	private void Start()
	{
		if (!SemiFunc.IsMultiplayer() && this.photonTransformView)
		{
			this.photonTransformView.enabled = false;
			this.photonTransformView = null;
		}
		this.mainCamera = Camera.main;
		this.isEnemy = base.GetComponent<EnemyRigidbody>();
		this.isMelee = base.GetComponent<ItemMelee>();
		this.isGun = base.GetComponent<ItemGun>();
		if (!this.isEnemy)
		{
			this.isNonValuable = base.GetComponent<NotValuableObject>();
		}
		Quaternion rotation = base.transform.rotation;
		base.transform.rotation = Quaternion.identity;
		Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		bool flag = false;
		foreach (Collider collider in componentsInChildren)
		{
			if (!collider.isTrigger)
			{
				if (flag)
				{
					bounds.Encapsulate(collider.bounds);
				}
				else
				{
					bounds = collider.bounds;
					flag = true;
				}
			}
		}
		this.itemHeightY = bounds.size.y;
		this.itemWidthX = bounds.size.x;
		this.itemLengthZ = bounds.size.z;
		base.transform.rotation = rotation;
		if (flag)
		{
			this.boundingBox = bounds.size;
			this.midPointOffset = base.transform.InverseTransformPoint(bounds.center);
		}
		else
		{
			this.boundingBox = Vector3.one;
			Debug.LogWarning("No colliders found on the object or its children!");
		}
		int num = 0;
		foreach (PhysGrabObjectCollider physGrabObjectCollider in base.GetComponentsInChildren<PhysGrabObjectCollider>())
		{
			this.colliders.Add(physGrabObjectCollider.transform);
			physGrabObjectCollider.colliderID = num;
			num++;
		}
		this.roomVolumeCheck = base.GetComponent<RoomVolumeCheck>();
		this.photonView = base.GetComponent<PhotonView>();
		this.hinge = base.GetComponent<PhysGrabHinge>();
		if (this.hinge)
		{
			this.hasHinge = true;
		}
		if (GameManager.instance.gameMode == 1)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				this.prevTargetPos = base.transform.position;
				this.prevTargetRot = base.transform.rotation;
				this.targetPos = base.transform.position;
				this.targetRot = base.transform.rotation;
				this.isMaster = true;
			}
			if (PhotonNetwork.IsMasterClient && this.spawned)
			{
				this.photonView.TransferOwnership(PhotonNetwork.MasterClient);
			}
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (!base.GetComponent<EnemyRigidbody>())
			{
				base.StartCoroutine(this.EnableRigidbody());
			}
		}
		else
		{
			this.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
		}
		if (this.overrideTagsAndLayers)
		{
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (!transform.CompareTag("Cart") && !transform.CompareTag("Grab Area") && !transform.CompareTag("Wall"))
				{
					if (transform.gameObject.layer != LayerMask.NameToLayer("PlayerOnlyCollision") && transform.gameObject.layer != LayerMask.NameToLayer("Triggers"))
					{
						transform.gameObject.tag = "Phys Grab Object";
					}
					if (transform.gameObject.layer != LayerMask.NameToLayer("IgnorePhysGrab") && transform.gameObject.layer != LayerMask.NameToLayer("CollisionCheck") && transform.gameObject.layer != LayerMask.NameToLayer("CartWheels") && transform.gameObject.layer != LayerMask.NameToLayer("PhysGrabObjectHinge") && transform.gameObject.layer != LayerMask.NameToLayer("PhysGrabObjectCart") && transform.gameObject.layer != LayerMask.NameToLayer("Triggers") && transform.gameObject.layer != LayerMask.NameToLayer("PlayerOnlyCollision"))
					{
						transform.gameObject.layer = LayerMask.NameToLayer("PhysGrabObject");
					}
				}
			}
		}
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x0007D5C8 File Offset: 0x0007B7C8
	public void OverrideFragility(float multiplier)
	{
		if (!this.impactDetector)
		{
			return;
		}
		if (!this.isValuable)
		{
			return;
		}
		this.overrideFragilityTimer = 0.1f;
		this.impactDetector.fragilityMultiplier = multiplier;
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x0007D5F8 File Offset: 0x0007B7F8
	private void OverrideVariousTick()
	{
		if (this.overrideKnockOutOfGrabDisableTimer <= 0f)
		{
			this.overrideKnockOutOfGrabDisable = false;
		}
		if (this.overrideKnockOutOfGrabDisableTimer > 0f)
		{
			this.overrideKnockOutOfGrabDisableTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x0007D630 File Offset: 0x0007B830
	private void OverrideTimersTick()
	{
		if (this.timerAlterDeactivate > 0f)
		{
			if (this.isActive)
			{
				base.transform.position = new Vector3(0f, 3000f, 0f);
			}
			this.isActive = false;
			this.rb.detectCollisions = false;
			this.rb.isKinematic = true;
			if (SemiFunc.IsMultiplayer() && !SemiFunc.MenuLevel() && this.photonTransformView.enabled)
			{
				this.photonTransformView.enabled = false;
			}
			this.timerAlterDeactivate -= Time.fixedDeltaTime;
		}
		else if (this.timerAlterDeactivate != -123f)
		{
			this.OverrideDeactivateReset();
		}
		if (this.mapCustom && this.hasMapCustom && !this.isActive)
		{
			this.mapCustom.Hide();
		}
		this.OverrideGrabRelativePositionTick();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.OverrideVariousTick();
		this.OverrideStrengthTick();
		if (this.overrideMassGoDownTimer > 0f)
		{
			this.overrideMassGoDownTimer -= Time.deltaTime;
		}
		if (this.timerAlterMass <= 0f && this.timerAlterMass != -123f)
		{
			if (this.massOriginal == 0f)
			{
				this.massOriginal = this.rb.mass;
			}
			this.ResetMass();
		}
		if (this.timerAlterMass > 0f)
		{
			this.rb.mass = this.alterMassValue;
			this.timerAlterMass -= Time.deltaTime;
		}
		if (this.impactDetector)
		{
			if (this.overrideFragilityTimer > 0f)
			{
				this.overrideFragilityTimer -= Time.deltaTime;
			}
			else if (this.overrideFragilityTimer != -123f)
			{
				this.impactDetector.fragilityMultiplier = 1f;
				this.overrideFragilityTimer = -123f;
			}
		}
		if (this.overrideAngularDragGoDownTimer > 0f)
		{
			this.overrideAngularDragGoDownTimer -= Time.deltaTime;
		}
		if (this.timerAlterAngularDrag > 0f)
		{
			this.rb.angularDrag = this.alterAngularDragValue;
			this.timerAlterAngularDrag -= Time.deltaTime;
		}
		else if (this.timerAlterAngularDrag != -123f)
		{
			this.rb.angularDrag = this.angularDragOriginal;
			this.timerAlterAngularDrag = -123f;
			this.alterAngularDragValue = 0f;
		}
		if (this.overrideDragGoDownTimer > 0f)
		{
			this.overrideDragGoDownTimer -= Time.deltaTime;
		}
		if (this.timerAlterDrag > 0f)
		{
			this.rb.drag = this.alterDragValue;
			this.timerAlterDrag -= Time.deltaTime;
		}
		else if (this.timerAlterDrag != -123f)
		{
			this.rb.drag = this.dragOriginal;
			this.timerAlterDrag = -123f;
			this.alterDragValue = 0f;
		}
		if (this.timerAlterIndestructible > 0f)
		{
			if (this.impactDetector)
			{
				this.impactDetector.isIndestructible = true;
			}
			this.timerAlterIndestructible -= Time.deltaTime;
		}
		else if (this.timerAlterIndestructible != -123f)
		{
			this.ResetIndestructible();
		}
		if (this.timerAlterMaterial > 0f)
		{
			this.timerAlterMaterial -= Time.deltaTime;
		}
		else if (this.timerAlterMaterial != -123f)
		{
			foreach (Transform transform in this.colliders)
			{
				if (transform)
				{
					transform.GetComponent<Collider>().material = SemiFunc.PhysicMaterialPhysGrabObject();
				}
				else
				{
					this.colliders.Remove(transform);
				}
			}
			this.timerAlterMaterial = -123f;
			this.alterMaterialCurrent = null;
		}
		if (this.timerZeroGravity > 0f)
		{
			this.rb.useGravity = false;
			this.timerZeroGravity -= Time.deltaTime;
		}
		else if (this.timerZeroGravity != -123f)
		{
			this.rb.useGravity = true;
			this.timerZeroGravity = -123f;
		}
		if ((!this.hasHinge || this.hinge.dead || this.hinge.broken) && this.rb.useGravity)
		{
			if (this.grabbed)
			{
				if (this.timerAlterAngularDrag <= 0f)
				{
					this.rb.angularDrag = 0.5f;
				}
				if (this.timerAlterDrag <= 0f)
				{
					this.rb.drag = 0.5f;
					return;
				}
			}
			else
			{
				if (this.timerAlterAngularDrag <= 0f)
				{
					this.rb.angularDrag = this.angularDragOriginal;
				}
				if (this.timerAlterDrag <= 0f)
				{
					this.rb.drag = this.dragOriginal;
				}
			}
		}
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x0007DB08 File Offset: 0x0007BD08
	private IEnumerator EnableRigidbody()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.1f);
		this.spawned = true;
		this.rb.isKinematic = false;
		if (this.spawnTorque != Vector3.zero)
		{
			this.rb.AddTorque(this.spawnTorque, ForceMode.Impulse);
		}
		yield break;
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x0007DB18 File Offset: 0x0007BD18
	private void TickImpactTimers()
	{
		if (this.impactHappenedTimer > 0f)
		{
			this.impactHappenedTimer -= Time.fixedDeltaTime;
		}
		if (this.impactLightTimer > 0f)
		{
			this.impactLightTimer -= Time.fixedDeltaTime;
		}
		if (this.impactMediumTimer > 0f)
		{
			this.impactMediumTimer -= Time.fixedDeltaTime;
		}
		if (this.impactHeavyTimer > 0f)
		{
			this.impactHeavyTimer -= Time.fixedDeltaTime;
		}
		if (this.breakLightTimer > 0f)
		{
			this.breakLightTimer -= Time.fixedDeltaTime;
		}
		if (this.breakMediumTimer > 0f)
		{
			this.breakMediumTimer -= Time.fixedDeltaTime;
		}
		if (this.breakHeavyTimer > 0f)
		{
			this.breakHeavyTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x0007DBFE File Offset: 0x0007BDFE
	public void OverrideMinTorqueStrength(float value, float time = 0.1f)
	{
		this.overrideMinTorqueStrengthTimer = time;
		this.overrideMinTorqueStrength = value;
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x0007DC0E File Offset: 0x0007BE0E
	public void OverrideMinGrabStrength(float value, float time = 0.1f)
	{
		this.overrideMinGrabStrengthTimer = time;
		this.overrideMinGrabStrength = value;
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x0007DC1E File Offset: 0x0007BE1E
	public void OverrideGrabStrength(float value, float time = 0.1f)
	{
		this.overrideGrabStrengthTimer = time;
		this.overrideGrabStrength = value;
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x0007DC2E File Offset: 0x0007BE2E
	public void OverrideTorqueStrengthX(float value, float time = 0.1f)
	{
		this.overrideTorqueStrengthXTimer = time;
		this.overrideTorqueStrengthX = value;
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x0007DC3E File Offset: 0x0007BE3E
	public void OverrideTorqueStrengthY(float value, float time = 0.1f)
	{
		this.overrideTorqueStrengthYTimer = time;
		this.overrideTorqueStrengthY = value;
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x0007DC4E File Offset: 0x0007BE4E
	public void OverrideTorqueStrengthZ(float value, float time = 0.1f)
	{
		this.overrideTorqueStrengthZTimer = time;
		this.overrideTorqueStrengthZ = value;
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x0007DC5E File Offset: 0x0007BE5E
	public void OverrideTorqueStrength(float value, float time = 0.1f)
	{
		this.overrideTorqueStrengthTimer = time;
		this.overrideTorqueStrength = value;
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x0007DC6E File Offset: 0x0007BE6E
	public void OverrideExtraGrabStrengthDisable(float time = 0.1f)
	{
		this.overrideExtraGrabStrengthDisableTimer = time;
		this.overrideExtraGrabStrengthDisable = true;
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x0007DC7E File Offset: 0x0007BE7E
	public void OverrideExtraTorqueStrengthDisable(float time = 0.1f)
	{
		this.overrideExtraTorqueStrengthDisableTimer = time;
		this.overrideExtraTorqueStrengthDisable = true;
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x0007DC8E File Offset: 0x0007BE8E
	public void OverrideKnockOutOfGrabDisable(float time = 0.1f)
	{
		this.overrideKnockOutOfGrabDisableTimer = time;
		this.overrideKnockOutOfGrabDisable = true;
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x0007DCA0 File Offset: 0x0007BEA0
	public void OverrideStrengthTick()
	{
		if (this.overrideTorqueStrengthXTimer <= 0f)
		{
			this.overrideTorqueStrengthX = 1f;
		}
		if (this.overrideTorqueStrengthXTimer > 0f)
		{
			this.overrideTorqueStrengthXTimer -= Time.deltaTime;
		}
		if (this.overrideTorqueStrengthYTimer <= 0f)
		{
			this.overrideTorqueStrengthY = 1f;
		}
		if (this.overrideTorqueStrengthYTimer > 0f)
		{
			this.overrideTorqueStrengthYTimer -= Time.deltaTime;
		}
		if (this.overrideTorqueStrengthZTimer <= 0f)
		{
			this.overrideTorqueStrengthZ = 1f;
		}
		if (this.overrideTorqueStrengthZTimer > 0f)
		{
			this.overrideTorqueStrengthZTimer -= Time.deltaTime;
		}
		if (this.overrideTorqueStrengthTimer <= 0f)
		{
			this.overrideTorqueStrength = 1f;
		}
		if (this.overrideTorqueStrengthTimer > 0f)
		{
			this.overrideTorqueStrengthTimer -= Time.deltaTime;
		}
		if (this.overrideGrabStrengthTimer <= 0f)
		{
			this.overrideGrabStrength = 1f;
		}
		if (this.overrideGrabStrengthTimer > 0f)
		{
			this.overrideGrabStrengthTimer -= Time.deltaTime;
		}
		if (this.overrideMinTorqueStrengthTimer <= 0f)
		{
			this.overrideMinTorqueStrength = 0f;
		}
		if (this.overrideMinTorqueStrengthTimer > 0f)
		{
			this.overrideMinTorqueStrengthTimer -= Time.deltaTime;
		}
		if (this.overrideMinGrabStrengthTimer <= 0f)
		{
			this.overrideMinGrabStrength = 0f;
		}
		if (this.overrideMinGrabStrengthTimer > 0f)
		{
			this.overrideMinGrabStrengthTimer -= Time.deltaTime;
		}
		if (this.overrideExtraGrabStrengthDisableTimer <= 0f)
		{
			this.overrideExtraGrabStrengthDisable = false;
		}
		if (this.overrideExtraGrabStrengthDisableTimer > 0f)
		{
			this.overrideExtraGrabStrengthDisable = true;
			this.overrideExtraGrabStrengthDisableTimer -= Time.deltaTime;
		}
		if (this.overrideExtraTorqueStrengthDisableTimer <= 0f)
		{
			this.overrideExtraTorqueStrengthDisable = false;
		}
		if (this.overrideExtraTorqueStrengthDisableTimer > 0f)
		{
			this.overrideExtraTorqueStrengthDisable = true;
			this.overrideExtraTorqueStrengthDisableTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x0007DEA4 File Offset: 0x0007C0A4
	private void FixedUpdate()
	{
		this.TickImpactTimers();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!this.rb.IsSleeping())
			{
				Debug.DrawLine(this.midPoint, this.midPoint + Vector3.up * 5f, Color.red);
			}
			this.rbVelocity = this.rb.velocity;
			this.rbAngularVelocity = this.rb.angularVelocity;
			this.isKinematic = this.rb.isKinematic;
			if (!this.isKinematic)
			{
				float num = 40f;
				float num2 = 30f;
				Vector3 velocity = this.rb.velocity;
				if (velocity.sqrMagnitude > num * num)
				{
					this.rb.velocity = Vector3.ClampMagnitude(velocity, num);
				}
				Vector3 angularVelocity = this.rb.angularVelocity;
				if (angularVelocity.sqrMagnitude > num2 * num2)
				{
					this.rb.angularVelocity = Vector3.ClampMagnitude(angularVelocity, num2);
				}
			}
		}
		if (this.frozenTimer > 0f)
		{
			this.frozenTimer -= Time.fixedDeltaTime;
			this.rb.MovePosition(this.frozenPosition);
			this.rb.MoveRotation(this.frozenRotation);
			if (!this.rb.isKinematic)
			{
				this.rb.velocity = Vector3.zero;
				this.rbVelocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
				this.rbAngularVelocity = Vector3.zero;
			}
			return;
		}
		if (this.frozen)
		{
			this.rb.AddForce(this.frozenVelocity, ForceMode.VelocityChange);
			this.rb.AddTorque(this.frozenAngularVelocity, ForceMode.VelocityChange);
			this.rb.AddForce(this.frozenForce, ForceMode.Impulse);
			this.rb.AddTorque(this.frozenTorque, ForceMode.Impulse);
			this.frozenForce = Vector3.zero;
			this.frozenTorque = Vector3.zero;
			this.frozen = false;
			return;
		}
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			this.rbVelocity = this.rb.velocity;
			this.rbAngularVelocity = this.rb.angularVelocity;
		}
		if (this.playerGrabbing.Count > 0)
		{
			if (this.hasNeverBeenGrabbed)
			{
				this.OverrideIndestructible(0.5f);
				this.hasNeverBeenGrabbed = false;
			}
			this.grabbed = true;
			this.heldByLocalPlayer = false;
			if (GameManager.Multiplayer())
			{
				using (List<PhysGrabber>.Enumerator enumerator = this.playerGrabbing.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.photonView.IsMine)
						{
							this.heldByLocalPlayer = true;
						}
					}
					goto IL_2A0;
				}
			}
			this.heldByLocalPlayer = true;
		}
		else
		{
			this.heldByLocalPlayer = false;
			this.grabbed = false;
		}
		IL_2A0:
		this.PhysicsGrabbingManipulation();
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x0007E168 File Offset: 0x0007C368
	private void PhysicsGrabbingManipulation()
	{
		if ((!GameManager.Multiplayer() || this.isMaster) && !this.rb.isKinematic)
		{
			Vector3 vector = Vector3.zero;
			this.grabDisplacementCurrent = Vector3.zero;
			Vector3 a = Vector3.zero;
			int count = this.playerGrabbing.Count;
			float mass = this.rb.mass;
			foreach (PhysGrabber physGrabber in this.playerGrabbing)
			{
				float num = physGrabber.forceMax;
				if (this.isCart && this.physGrabCart.inCart.GetComponent<BoxCollider>().bounds.Contains(physGrabber.transform.position))
				{
					num *= 0.25f;
				}
				physGrabber.grabbedPhysGrabObject = this;
				if (!physGrabber.physGrabForcesDisabled)
				{
					Vector3 a2 = physGrabber.physGrabPointPullerPosition;
					if (this.overrideGrabRelativeVerticalPositionTimer != 0f)
					{
						Vector3 up = physGrabber.playerAvatar.localCameraTransform.up;
						a2 += up * this.overrideGrabRelativeVerticalPosition;
					}
					if (this.overrideGrabRelativeHorizontalPositionTimer != 0f)
					{
						Vector3 right = physGrabber.playerAvatar.localCameraTransform.right;
						a2 += right * this.overrideGrabRelativeHorizontalPosition;
					}
					Vector3 vector2 = Vector3.ClampMagnitude(a2 - physGrabber.physGrabPoint.position, num) * 10f;
					vector2 = Vector3.ClampMagnitude(vector2, num);
					Vector3 pointVelocity = this.rb.GetPointVelocity(physGrabber.physGrabPoint.position);
					Vector3 vector3 = vector2 * physGrabber.springConstant - pointVelocity * physGrabber.dampingConstant;
					Mathf.Max(mass / 3f, 2f);
					Vector3 a3 = Vector3.ClampMagnitude(vector3, num) * 2f / mass;
					float num2 = physGrabber.grabStrength;
					if (this.overrideExtraGrabStrengthDisable)
					{
						num2 = 1f;
					}
					if (this.overrideMinGrabStrengthTimer > 0f)
					{
						num2 = Mathf.Max(num2, this.overrideMinGrabStrength + num2 / 5f);
					}
					if (this.overrideGrabStrengthTimer > 0f)
					{
						num2 = this.overrideGrabStrength;
					}
					float num3 = 7f;
					float num4 = 20f;
					float num5 = 20f;
					float num6 = 20f;
					float num7 = 10f;
					float b = num2 / (1f + num2);
					if (mass < 2f)
					{
						num7 = num3;
					}
					if (mass >= 2f && mass < 4f)
					{
						num7 = num4;
					}
					if (mass >= 4f && mass < 8f)
					{
						num7 = num5;
					}
					if (mass >= 8f)
					{
						num7 = num6;
					}
					float t = Mathf.Min((num2 - 1f) / num7, 1f);
					num2 = Mathf.Lerp(num2, b, t);
					Vector3 vector4 = a3 * num2 * physGrabber.forceConstant;
					if (this.hasHinge && !this.hinge.dead && !this.hinge.broken)
					{
						vector4 *= 2f;
					}
					using (List<PhysGrabObject>.Enumerator enumerator2 = physGrabber.playerAvatar.physObjectStander.physGrabObjects.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current == this && vector4.y > 0f)
							{
								vector4.y = 0f;
							}
						}
					}
					if (this.isCart && this.physGrabCart.inCart.GetComponent<BoxCollider>().bounds.Contains(physGrabber.playerAvatar.transform.position) && vector4.y > 0f)
					{
						vector4.y = 0f;
					}
					float d = Mathf.Min(Vector3.Distance(a2, physGrabber.physGrabPoint.position) * 10f, 1f);
					vector4 *= d;
					Vector3 vector5 = Vector3.Lerp(physGrabber.currentGrabForce, vector4, 0.8f);
					physGrabber.currentGrabForce = vector4;
					if (this.isMelee || this.isGun)
					{
						vector5 /= (float)count;
					}
					this.rb.AddForceAtPosition(vector5, physGrabber.physGrabPoint.position, ForceMode.Acceleration);
					a += vector4;
					this.grabDisplacementCurrent += vector2 * physGrabber.grabStrength;
					if (!this.hasHinge || this.hinge.dead || this.hinge.broken)
					{
						Transform localCameraTransform = physGrabber.playerAvatar.localCameraTransform;
						Vector3 vector6 = localCameraTransform.TransformDirection(physGrabber.physRotation * physGrabber.cameraRelativeGrabbedForward);
						Vector3 vector7 = localCameraTransform.TransformDirection(physGrabber.physRotation * physGrabber.cameraRelativeGrabbedUp);
						Vector3 forward = base.transform.forward;
						Vector3 up2 = base.transform.up;
						Vector3 vector8 = Vector3.zero;
						num2 = physGrabber.grabStrength;
						Vector3 vector9 = Vector3.Cross(forward, vector6);
						if (vector9.sqrMagnitude > 1E-08f)
						{
							vector8 += vector9.normalized * Mathf.Clamp(Vector3.Angle(forward, vector6), 0f, 60f);
						}
						Vector3 vector10 = Vector3.Cross(up2, vector7);
						if (vector10.sqrMagnitude > 1E-08f)
						{
							vector8 += vector10.normalized * Mathf.Clamp(Vector3.Angle(up2, vector7), 0f, 60f);
						}
						vector8 = Vector3.ClampMagnitude(vector8, 60f).normalized;
						vector8 *= this.overrideTorqueStrength;
						if (physGrabber.mouseTurningVelocity.magnitude > 0.1f && this.massOriginal > 1f)
						{
							float num8 = Mathf.Max(mass, 0.1f);
							float num9 = 2f / num8;
							float num10 = 1f + this.boundingBox.magnitude;
							num9 += num10;
							if (num9 < 1f)
							{
								num9 = 1f;
							}
							if (num9 > 10f)
							{
								num9 = 10f;
							}
							vector8 *= num9;
						}
						float num11 = Mathf.Clamp01(mass);
						if (mass > 1f)
						{
							num11 *= 0.9f;
						}
						float num12 = Vector3.Angle(base.transform.forward, vector6) / 180f;
						float num13 = Vector3.Angle(base.transform.up, vector7) / 180f;
						float num14 = num12 + num13;
						vector8 *= num14 * 15f * num11 * Time.fixedDeltaTime;
						Quaternion b2 = Quaternion.LookRotation(vector6, vector7);
						Vector3 direction = base.transform.InverseTransformDirection(vector8);
						float num15 = this.overrideTorqueStrengthX;
						float num16 = this.overrideTorqueStrengthY;
						float num17 = this.overrideTorqueStrengthZ;
						if (num15 > 1f)
						{
							num15 *= num14;
						}
						if (num16 > 1f)
						{
							num16 *= num14;
						}
						if (num17 > 1f)
						{
							num17 *= num14;
						}
						direction.x *= num15;
						direction.y *= num16;
						direction.z *= num17;
						vector8 = base.transform.TransformDirection(direction);
						Vector3 a4 = vector8 * num11;
						if (this.overrideExtraTorqueStrengthDisable)
						{
							num2 = 1f;
						}
						if (this.overrideMinTorqueStrengthTimer > 0f)
						{
							num2 = Mathf.Max(num2, this.overrideMinTorqueStrength + num2 / 5f);
						}
						if (num2 < this.overrideTorqueStrength)
						{
							num2 = this.overrideTorqueStrength;
						}
						num7 = 10f;
						b = num2 / (1f + num2);
						if (mass < 2f)
						{
							num7 = num3;
						}
						if (mass >= 2f && mass < 4f)
						{
							num7 = num4;
						}
						if (mass >= 4f && mass < 8f)
						{
							num7 = num5;
						}
						if (mass >= 8f)
						{
							num7 = num6;
						}
						t = Mathf.Min((num2 - 1f) / num7, 1f);
						num2 = Mathf.Lerp(num2, b, t);
						if (this.isRotating)
						{
							num2 *= 5f;
						}
						float d2 = Mathf.Min(Quaternion.Angle(base.transform.rotation, b2) / 30f, 1f);
						float d3 = num2 + 10f;
						Vector3 vector11 = a4 * d3 * d2;
						float num18 = Mathf.Max(mass * 30f, 1f);
						num18 = num18 / 7f * mass * 6f;
						num18 /= 1f + num2;
						if (!this.isMelee)
						{
							float num19 = this.itemHeightY * this.itemHeightY;
							if (num19 > 10f)
							{
								num19 *= 2.5f;
							}
							float num20 = this.itemWidthX * this.itemWidthX;
							float num21 = this.itemLengthZ * this.itemLengthZ;
							float num22 = (num19 + num20 + num21) * 4f;
							float num23 = 1f + num22;
							num23 /= 1f + num2;
							vector11 /= num23;
						}
						vector11 *= 6500f / num18;
						if (this.isEnemy)
						{
							vector11 *= 0.5f;
						}
						Vector3 vector12 = Vector3.Lerp(physGrabber.currentTorqueForce, vector11, 0.9f);
						physGrabber.currentTorqueForce = vector11;
						if (this.isMelee || this.isGun)
						{
							vector12 /= (float)count;
						}
						if (this.isRotating)
						{
							vector += vector12 * (physGrabber.mouseTurningVelocity.magnitude / 100f);
						}
						else
						{
							vector += vector12;
						}
					}
				}
			}
			if (vector.magnitude > 0f)
			{
				if (this.isCart)
				{
					vector.z = 0f;
					vector.x = 0f;
				}
				this.rb.AddTorque(vector, ForceMode.Acceleration);
				this.rb.angularVelocity *= 0.8f;
			}
			if (a.magnitude > 0f)
			{
				this.rb.velocity *= 0.98f;
			}
		}
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x0007EBB8 File Offset: 0x0007CDB8
	public void OverrideGrabVerticalPosition(float pos)
	{
		this.overrideGrabRelativeVerticalPosition = pos;
		this.overrideGrabRelativeVerticalPositionTimer = 0.1f;
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x0007EBCC File Offset: 0x0007CDCC
	public void OverrideGrabHorizontalPosition(float pos)
	{
		this.overrideGrabRelativeHorizontalPosition = pos;
		this.overrideGrabRelativeHorizontalPositionTimer = 0.1f;
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x0007EBE0 File Offset: 0x0007CDE0
	private void OverrideGrabRelativePositionTick()
	{
		if (this.overrideGrabRelativeHorizontalPositionTimer <= 0f)
		{
			this.overrideGrabRelativeHorizontalPosition = 0f;
		}
		if (this.overrideGrabRelativeHorizontalPositionTimer > 0f)
		{
			this.overrideGrabRelativeHorizontalPositionTimer -= Time.deltaTime;
		}
		if (this.overrideGrabRelativeVerticalPositionTimer <= 0f)
		{
			this.overrideGrabRelativeVerticalPosition = 0f;
		}
		if (this.overrideGrabRelativeVerticalPositionTimer > 0f)
		{
			this.overrideGrabRelativeVerticalPositionTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x0007EC5B File Offset: 0x0007CE5B
	public void OverrideZeroGravity(float time = 0.1f)
	{
		this.timerZeroGravity = time;
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x0007EC64 File Offset: 0x0007CE64
	public void OverrideDrag(float value, float time = 0.1f)
	{
		this.timerAlterDrag = time;
		if (this.alterDragValue <= value)
		{
			this.alterDragValue = value;
			this.overrideDragGoDownTimer = 0.1f;
			return;
		}
		if (this.overrideDragGoDownTimer <= 0f)
		{
			this.alterDragValue = value;
		}
	}

	// Token: 0x06000E28 RID: 3624 RVA: 0x0007EC9D File Offset: 0x0007CE9D
	public void OverrideAngularDrag(float value, float time = 0.1f)
	{
		this.timerAlterAngularDrag = time;
		if (this.alterAngularDragValue <= value)
		{
			this.alterAngularDragValue = value;
			this.overrideAngularDragGoDownTimer = 0.1f;
			return;
		}
		if (this.overrideAngularDragGoDownTimer <= 0f)
		{
			this.timerAlterAngularDrag = value;
		}
	}

	// Token: 0x06000E29 RID: 3625 RVA: 0x0007ECD6 File Offset: 0x0007CED6
	public void OverrideIndestructible(float time = 0.1f)
	{
		this.timerAlterIndestructible = time;
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x0007ECDF File Offset: 0x0007CEDF
	public void OverrideDeactivate(float time = 0.1f)
	{
		this.timerAlterDeactivate = time;
		this.rb.isKinematic = true;
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x0007ECF4 File Offset: 0x0007CEF4
	public void OverrideDeactivateReset()
	{
		this.isActive = true;
		this.rb.detectCollisions = true;
		if (this.spawned)
		{
			this.rb.isKinematic = false;
		}
		if (SemiFunc.IsMultiplayer() && !SemiFunc.MenuLevel())
		{
			this.photonTransformView.enabled = true;
		}
		this.timerAlterDeactivate = -123f;
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x0007ED4D File Offset: 0x0007CF4D
	public void OverrideBreakEffects(float _time)
	{
		this.overrideDisableBreakEffectsTimer = _time;
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x0007ED58 File Offset: 0x0007CF58
	public void OverrideMaterial(PhysicMaterial material, float time = 0.1f)
	{
		if (this.alterMaterialCurrent != this.alterMaterialPrevious || this.alterMaterialCurrent == null)
		{
			this.alterMaterialPrevious = this.alterMaterialCurrent;
			foreach (Transform transform in this.colliders)
			{
				if (transform)
				{
					Collider component = transform.GetComponent<Collider>();
					if (component)
					{
						component.material = material;
					}
				}
				else
				{
					this.colliders.Remove(transform);
				}
			}
		}
		this.alterMaterialCurrent = material;
		this.timerAlterMaterial = time;
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x0007EE0C File Offset: 0x0007D00C
	public void ResetIndestructible()
	{
		if (this.impactDetector)
		{
			this.impactDetector.isIndestructible = false;
		}
		this.timerAlterIndestructible = -123f;
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x0007EE32 File Offset: 0x0007D032
	public void SetPositionLogic(Vector3 _position, Quaternion _rotation)
	{
		this.photonTransformView.Teleport(_position, _rotation);
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x0007EE44 File Offset: 0x0007D044
	[PunRPC]
	private void SetPositionRPC(Vector3 position, Quaternion rotation)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.SetPositionLogic(position, rotation);
			return;
		}
		base.transform.position = position;
		base.transform.rotation = rotation;
		this.rb.position = position;
		this.rb.rotation = rotation;
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x0007EE94 File Offset: 0x0007D094
	public void Teleport(Vector3 position, Quaternion rotation)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			base.transform.position = position;
			base.transform.rotation = rotation;
			this.rb.position = position;
			this.rb.rotation = rotation;
			return;
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonTransformView.Teleport(position, rotation);
			return;
		}
		this.photonView.RPC("SetPositionRPC", RpcTarget.MasterClient, new object[]
		{
			position,
			rotation
		});
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x0007EF17 File Offset: 0x0007D117
	public void OverrideMass(float value, float time = 0.1f)
	{
		this.timerAlterMass = time;
		if (this.alterMassValue <= value)
		{
			this.alterMassValue = value;
			this.overrideMassGoDownTimer = 0.1f;
			return;
		}
		if (this.overrideMassGoDownTimer <= 0f)
		{
			this.alterMassValue = value;
		}
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x0007EF50 File Offset: 0x0007D150
	public void ResetMass()
	{
		this.rb.mass = this.massOriginal;
		this.timerAlterMass = -123f;
		this.alterMassValue = 0f;
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x0007EF79 File Offset: 0x0007D179
	private void IsRotatingTimer()
	{
		if (this.isRotatingTimer <= 0f)
		{
			this.isRotating = false;
		}
		if (this.isRotatingTimer > 0f)
		{
			this.isRotatingTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x0007EFB0 File Offset: 0x0007D1B0
	private void Update()
	{
		if (this.grabbed)
		{
			for (int i = 0; i < this.playerGrabbing.Count; i++)
			{
				if (this.playerGrabbing[i].isRotating)
				{
					this.isRotating = true;
					this.isRotatingTimer = 0.1f;
				}
				if (!this.playerGrabbing[i] || !this.playerGrabbing[i].grabbed)
				{
					this.playerGrabbing.RemoveAt(i);
				}
			}
		}
		this.IsRotatingTimer();
		this.midPoint = base.transform.TransformPoint(this.midPointOffset);
		this.OverrideTimersTick();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.playerGrabbing.Count > 0)
			{
				this.lastPlayerGrabbing = this.playerGrabbing[this.playerGrabbing.Count - 1].playerAvatar;
				this.grabbedTimer = 0.5f;
			}
			else if (this.lastPlayerGrabbing)
			{
				this.grabbedTimer -= Time.deltaTime;
				if (this.grabbedTimer <= 0f)
				{
					this.lastPlayerGrabbing = null;
				}
			}
			if (this.enemyInteractTimer > 0f)
			{
				this.enemyInteractTimer -= Time.deltaTime;
				if (this.playerGrabbing.Count > 0)
				{
					this.enemyInteractTimer = 0f;
				}
			}
			if (this.hasImpactDetector && !this.impactDetector.isIndestructible)
			{
				if (this.heavyImpactImpulse)
				{
					this.impactDetector.ImpactHeavy(150f, this.centerPoint);
					this.heavyImpactImpulse = false;
				}
				if (this.mediumImpactImpulse)
				{
					this.impactDetector.ImpactMedium(80f, this.centerPoint);
					this.mediumImpactImpulse = false;
				}
				if (this.lightImpactImpulse)
				{
					this.impactDetector.ImpactLight(20f, this.centerPoint);
					this.lightImpactImpulse = false;
				}
				if (this.heavyBreakImpulse)
				{
					if (this.isValuable)
					{
						this.impactDetector.BreakHeavy(this.centerPoint, 0f);
					}
					else
					{
						this.impactDetector.Break(0f, this.centerPoint, this.impactDetector.breakLevelHeavy);
					}
					this.heavyBreakImpulse = false;
				}
				if (this.mediumBreakImpulse)
				{
					if (this.isValuable)
					{
						this.impactDetector.BreakMedium(this.centerPoint);
					}
					else
					{
						this.impactDetector.Break(0f, this.centerPoint, this.impactDetector.breakLevelMedium);
					}
					this.mediumBreakImpulse = false;
				}
				if (this.lightBreakImpulse)
				{
					if (this.isValuable)
					{
						this.impactDetector.BreakLight(this.centerPoint);
					}
					else
					{
						this.impactDetector.Break(0f, this.centerPoint, this.impactDetector.breakLevelLight);
					}
					this.lightBreakImpulse = false;
				}
			}
			if (this.overrideDisableBreakEffectsTimer > 0f)
			{
				this.overrideDisableBreakEffectsTimer -= Time.deltaTime;
			}
			if (this.dead && this.playerGrabbing.Count == 0)
			{
				this.DestroyPhysGrabObject();
			}
		}
		if (this.grabDisableTimer > 0f)
		{
			this.grabDisableTimer -= Time.deltaTime;
		}
		this.centerPoint = this.midPoint;
		if (SemiFunc.IsMasterClientOrSingleplayer() && base.transform.position.y < -50f)
		{
			if (this.impactDetector.destroyDisable)
			{
				if (this.impactDetector.destroyDisableTeleport)
				{
					this.Teleport(TruckSafetySpawnPoint.instance.transform.position, TruckSafetySpawnPoint.instance.transform.rotation);
					return;
				}
			}
			else
			{
				this.impactDetector.DestroyObject(true);
			}
		}
	}

	// Token: 0x06000E36 RID: 3638 RVA: 0x0007F346 File Offset: 0x0007D546
	public void EnemyInteractTimeSet()
	{
		this.enemyInteractTimer = 10f;
	}

	// Token: 0x06000E37 RID: 3639 RVA: 0x0007F354 File Offset: 0x0007D554
	public void FreezeForces(float _time, Vector3 _force, Vector3 _torque)
	{
		if (this.rb.isKinematic)
		{
			return;
		}
		this.frozenTimer = _time;
		if (!this.frozen)
		{
			this.frozenPosition = base.transform.position;
			this.frozenRotation = base.transform.rotation;
			this.frozenVelocity = this.rb.velocity;
			this.frozenAngularVelocity = this.rb.angularVelocity;
			this.frozenForce = Vector3.zero;
			this.frozenTorque = Vector3.zero;
			this.frozen = true;
		}
		this.frozenForce += _force;
		this.frozenTorque += _torque;
		this.rb.velocity = Vector3.zero;
		this.rb.angularVelocity = Vector3.zero;
	}

	// Token: 0x06000E38 RID: 3640 RVA: 0x0007F423 File Offset: 0x0007D623
	private void OnDestroy()
	{
		if (RoundDirector.instance.dollarHaulList.Contains(base.gameObject))
		{
			RoundDirector.instance.dollarHaulList.Remove(base.gameObject);
		}
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x0007F452 File Offset: 0x0007D652
	public void DestroyPhysGrabObject()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.DestroyPhysGrabObjectRPC();
			return;
		}
		this.photonView.RPC("DestroyPhysGrabObjectRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x0007F47D File Offset: 0x0007D67D
	[PunRPC]
	private void DestroyPhysGrabObjectRPC()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x0007F48A File Offset: 0x0007D68A
	private void OnDisable()
	{
		RoundDirector.instance.PhysGrabObjectRemove(this);
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x0007F497 File Offset: 0x0007D697
	private void OnEnable()
	{
		RoundDirector.instance.PhysGrabObjectAdd(this);
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x0007F4A4 File Offset: 0x0007D6A4
	public void GrabStarted(PhysGrabber player)
	{
		if (!this.grabbedLocal)
		{
			this.grabbedLocal = true;
			if (GameManager.instance.gameMode == 0)
			{
				if (!this.playerGrabbing.Contains(player))
				{
					this.playerGrabbing.Add(player);
					return;
				}
			}
			else
			{
				this.photonView.RPC("GrabStartedRPC", RpcTarget.MasterClient, new object[]
				{
					player.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x0007F514 File Offset: 0x0007D714
	public void GrabEnded(PhysGrabber player)
	{
		if (this.grabbedLocal)
		{
			this.grabbedLocal = false;
			if (GameManager.instance.gameMode == 0)
			{
				this.Throw(player);
				if (this.playerGrabbing.Contains(player))
				{
					this.playerGrabbing.Remove(player);
					return;
				}
			}
			else
			{
				this.photonView.RPC("GrabEndedRPC", RpcTarget.MasterClient, new object[]
				{
					player.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x0007F58C File Offset: 0x0007D78C
	public void GrabLink(int playerPhotonID, int colliderID, Vector3 point, Vector3 cameraRelativeGrabbedForward, Vector3 cameraRelativeGrabbedUp)
	{
		this.photonView.RPC("GrabLinkRPC", RpcTarget.All, new object[]
		{
			playerPhotonID,
			colliderID,
			point,
			cameraRelativeGrabbedForward,
			cameraRelativeGrabbedUp
		});
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x0007F5E0 File Offset: 0x0007D7E0
	[PunRPC]
	private void GrabLinkRPC(int playerPhotonID, int colliderID, Vector3 point, Vector3 cameraRelativeGrabbedForward, Vector3 cameraRelativeGrabbedUp)
	{
		PhysGrabber component = PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>();
		component.physGrabPoint.position = point;
		component.localGrabPosition = base.transform.InverseTransformPoint(point);
		component.grabbedObjectTransform = base.transform;
		component.grabbedPhysGrabObjectColliderID = colliderID;
		component.grabbedPhysGrabObjectCollider = this.FindColliderFromID(colliderID).GetComponent<Collider>();
		component.prevGrabbed = component.grabbed;
		component.grabbed = true;
		Transform localCameraTransform = component.playerAvatar.localCameraTransform;
		if (this.playerGrabbing.Count != 0)
		{
			component.cameraRelativeGrabbedForward = localCameraTransform.InverseTransformDirection(base.transform.forward);
			component.cameraRelativeGrabbedUp = localCameraTransform.InverseTransformDirection(base.transform.up);
		}
		else
		{
			component.cameraRelativeGrabbedForward = localCameraTransform.InverseTransformDirection(base.transform.forward);
			component.cameraRelativeGrabbedUp = localCameraTransform.InverseTransformDirection(base.transform.up);
			this.camRelForward = base.transform.InverseTransformDirection(base.transform.forward);
			this.camRelUp = base.transform.InverseTransformDirection(base.transform.up);
		}
		component.cameraRelativeGrabbedForward = component.cameraRelativeGrabbedForward.normalized;
		component.cameraRelativeGrabbedUp = component.cameraRelativeGrabbedUp.normalized;
		if (component.photonView.IsMine)
		{
			Vector3 localGrabPosition = component.localGrabPosition;
			this.photonView.RPC("GrabPointSyncRPC", RpcTarget.All, new object[]
			{
				playerPhotonID,
				localGrabPosition
			});
		}
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x0007F75D File Offset: 0x0007D95D
	[PunRPC]
	private void GrabPointSyncRPC(int playerPhotonID, Vector3 localPointInBox)
	{
		PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>().localGrabPosition = localPointInBox;
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x0007F770 File Offset: 0x0007D970
	[PunRPC]
	private void GrabStartedRPC(int playerPhotonID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		PhysGrabber component = PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>();
		if (component && SemiFunc.OwnerOnlyRPC(_info, component.photonView) && !this.playerGrabbing.Contains(component))
		{
			this.photonView.RPC("GrabPlayerAddRPC", RpcTarget.All, new object[]
			{
				playerPhotonID
			});
		}
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x0007F7D0 File Offset: 0x0007D9D0
	[PunRPC]
	private void GrabPlayerAddRPC(int photonViewID)
	{
		PhysGrabber component = PhotonView.Find(photonViewID).GetComponent<PhysGrabber>();
		if (component)
		{
			this.playerGrabbing.Add(component);
		}
	}

	// Token: 0x06000E44 RID: 3652 RVA: 0x0007F800 File Offset: 0x0007DA00
	[PunRPC]
	private void GrabPlayerRemoveRPC(int photonViewID)
	{
		PhysGrabber component = PhotonView.Find(photonViewID).GetComponent<PhysGrabber>();
		if (component)
		{
			this.playerGrabbing.Remove(component);
		}
	}

	// Token: 0x06000E45 RID: 3653 RVA: 0x0007F830 File Offset: 0x0007DA30
	private void Throw(PhysGrabber player)
	{
		float d = Mathf.Max(this.rb.mass * 1.5f, 1f);
		Vector3 vector = Vector3.ClampMagnitude(player.physGrabPointPullerPosition - player.physGrabPoint.position, player.forceMax) * d;
		vector *= 0.5f + player.throwStrength;
		this.rb.AddForce(vector, ForceMode.Impulse);
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x0007F8A4 File Offset: 0x0007DAA4
	[PunRPC]
	private void GrabEndedRPC(int playerPhotonID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		PhysGrabber component = PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>();
		if (component && SemiFunc.OwnerOnlyRPC(_info, component.photonView))
		{
			this.Throw(component);
			component.prevGrabbed = component.grabbed;
			component.grabbed = false;
			if (this.playerGrabbing.Contains(component))
			{
				this.photonView.RPC("GrabPlayerRemoveRPC", RpcTarget.All, new object[]
				{
					playerPhotonID
				});
			}
		}
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x0007F91B File Offset: 0x0007DB1B
	public void PhysRidingDisabledSet(bool _state)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.PhysRidingDisabledRPC(_state);
			return;
		}
		this.photonView.RPC("PhysRidingDisabledRPC", RpcTarget.All, new object[]
		{
			_state
		});
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x0007F94C File Offset: 0x0007DB4C
	[PunRPC]
	private void PhysRidingDisabledRPC(bool _state)
	{
		this.physRidingDisabled = _state;
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x0007F958 File Offset: 0x0007DB58
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			if (!this.impactDetector)
			{
				this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
			}
			if (!this.rb)
			{
				this.rb = base.GetComponent<Rigidbody>();
			}
			stream.SendNext(this.rbVelocity);
			stream.SendNext(this.rbAngularVelocity);
			stream.SendNext(this.impactDetector.isSliding);
			stream.SendNext(this.isKinematic);
			return;
		}
		if (!this.impactDetector)
		{
			this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		}
		this.rbVelocity = (Vector3)stream.ReceiveNext();
		this.rbAngularVelocity = (Vector3)stream.ReceiveNext();
		this.impactDetector.isSliding = (bool)stream.ReceiveNext();
		this.isKinematic = (bool)stream.ReceiveNext();
		this.lastUpdateTime = Time.time;
	}

	// Token: 0x06000E4A RID: 3658 RVA: 0x0007FA60 File Offset: 0x0007DC60
	public Transform FindColliderFromID(int colliderID)
	{
		foreach (Transform transform in this.colliders)
		{
			if (transform.GetComponent<PhysGrabObjectCollider>().colliderID == colliderID)
			{
				return transform;
			}
		}
		return null;
	}

	// Token: 0x040016F8 RID: 5880
	public bool clientNonKinematic;

	// Token: 0x040016F9 RID: 5881
	public bool overrideTagsAndLayers = true;

	// Token: 0x040016FA RID: 5882
	internal PhotonView photonView;

	// Token: 0x040016FB RID: 5883
	internal PhotonTransformView photonTransformView;

	// Token: 0x040016FC RID: 5884
	[HideInInspector]
	public Rigidbody rb;

	// Token: 0x040016FD RID: 5885
	private bool isMaster;

	// Token: 0x040016FE RID: 5886
	internal RoomVolumeCheck roomVolumeCheck;

	// Token: 0x040016FF RID: 5887
	internal PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04001700 RID: 5888
	private bool hasImpactDetector;

	// Token: 0x04001701 RID: 5889
	internal Vector3 targetPos;

	// Token: 0x04001702 RID: 5890
	private float distance;

	// Token: 0x04001703 RID: 5891
	internal Quaternion targetRot;

	// Token: 0x04001704 RID: 5892
	private float angle;

	// Token: 0x04001705 RID: 5893
	internal Vector3 grabDisplacementCurrent;

	// Token: 0x04001706 RID: 5894
	[HideInInspector]
	public bool dead;

	// Token: 0x04001707 RID: 5895
	[HideInInspector]
	public bool grabbed;

	// Token: 0x04001708 RID: 5896
	[HideInInspector]
	public bool grabbedLocal;

	// Token: 0x04001709 RID: 5897
	public List<PhysGrabber> playerGrabbing = new List<PhysGrabber>();

	// Token: 0x0400170A RID: 5898
	[HideInInspector]
	public bool spawned;

	// Token: 0x0400170B RID: 5899
	internal PlayerAvatar lastPlayerGrabbing;

	// Token: 0x0400170C RID: 5900
	internal float grabbedTimer;

	// Token: 0x0400170D RID: 5901
	[HideInInspector]
	public bool lightBreakImpulse;

	// Token: 0x0400170E RID: 5902
	[HideInInspector]
	public bool mediumBreakImpulse;

	// Token: 0x0400170F RID: 5903
	[HideInInspector]
	public bool heavyBreakImpulse;

	// Token: 0x04001710 RID: 5904
	[HideInInspector]
	public bool lightImpactImpulse;

	// Token: 0x04001711 RID: 5905
	[HideInInspector]
	public bool mediumImpactImpulse;

	// Token: 0x04001712 RID: 5906
	[HideInInspector]
	public bool heavyImpactImpulse;

	// Token: 0x04001713 RID: 5907
	[HideInInspector]
	public float enemyInteractTimer;

	// Token: 0x04001714 RID: 5908
	internal float angularDragOriginal;

	// Token: 0x04001715 RID: 5909
	internal float dragOriginal;

	// Token: 0x04001716 RID: 5910
	internal bool isValuable;

	// Token: 0x04001717 RID: 5911
	internal bool isEnemy;

	// Token: 0x04001718 RID: 5912
	internal bool isPlayer;

	// Token: 0x04001719 RID: 5913
	internal bool isMelee;

	// Token: 0x0400171A RID: 5914
	internal bool isNonValuable;

	// Token: 0x0400171B RID: 5915
	internal bool isKinematic;

	// Token: 0x0400171C RID: 5916
	[HideInInspector]
	public float massOriginal;

	// Token: 0x0400171D RID: 5917
	private float lastUpdateTime;

	// Token: 0x0400171E RID: 5918
	[TupleElementNames(new string[]
	{
		"position",
		"timestamp"
	})]
	private List<ValueTuple<Vector3, double>> positionBuffer = new List<ValueTuple<Vector3, double>>();

	// Token: 0x0400171F RID: 5919
	[TupleElementNames(new string[]
	{
		"rotation",
		"timestamp"
	})]
	private List<ValueTuple<Quaternion, double>> rotationBuffer = new List<ValueTuple<Quaternion, double>>();

	// Token: 0x04001720 RID: 5920
	private float gradualLerp;

	// Token: 0x04001721 RID: 5921
	private Vector3 prevTargetPos;

	// Token: 0x04001722 RID: 5922
	private Quaternion prevTargetRot;

	// Token: 0x04001723 RID: 5923
	internal Vector3 rbVelocity = Vector3.zero;

	// Token: 0x04001724 RID: 5924
	internal Vector3 rbAngularVelocity = Vector3.zero;

	// Token: 0x04001725 RID: 5925
	internal Vector3 currentPosition;

	// Token: 0x04001726 RID: 5926
	internal Quaternion currentRotation;

	// Token: 0x04001727 RID: 5927
	private bool hasHinge;

	// Token: 0x04001728 RID: 5928
	private PhysGrabHinge hinge;

	// Token: 0x04001729 RID: 5929
	private float timerZeroGravity;

	// Token: 0x0400172A RID: 5930
	private float timerAlterDrag;

	// Token: 0x0400172B RID: 5931
	private float alterDragValue;

	// Token: 0x0400172C RID: 5932
	private float timerAlterAngularDrag;

	// Token: 0x0400172D RID: 5933
	private float alterAngularDragValue;

	// Token: 0x0400172E RID: 5934
	private float timerAlterMass;

	// Token: 0x0400172F RID: 5935
	private float alterMassValue;

	// Token: 0x04001730 RID: 5936
	private float timerAlterMaterial;

	// Token: 0x04001731 RID: 5937
	private float timerAlterDeactivate = -123f;

	// Token: 0x04001732 RID: 5938
	private float overrideFragilityTimer;

	// Token: 0x04001733 RID: 5939
	internal float overrideDisableBreakEffectsTimer;

	// Token: 0x04001734 RID: 5940
	private bool isActive = true;

	// Token: 0x04001735 RID: 5941
	private PhysicMaterial alterMaterialPrevious;

	// Token: 0x04001736 RID: 5942
	private PhysicMaterial alterMaterialCurrent;

	// Token: 0x04001737 RID: 5943
	[HideInInspector]
	public Vector3 midPoint;

	// Token: 0x04001738 RID: 5944
	[HideInInspector]
	public Vector3 midPointOffset;

	// Token: 0x04001739 RID: 5945
	private Vector3 grabRotation;

	// Token: 0x0400173A RID: 5946
	private bool isHidden;

	// Token: 0x0400173B RID: 5947
	internal float grabDisableTimer;

	// Token: 0x0400173C RID: 5948
	internal bool heldByLocalPlayer;

	// Token: 0x0400173D RID: 5949
	private CollisionDetectionMode previousCollisionDetectionMode;

	// Token: 0x0400173E RID: 5950
	private Camera mainCamera;

	// Token: 0x0400173F RID: 5951
	private float timerAlterIndestructible;

	// Token: 0x04001740 RID: 5952
	internal Transform forceGrabPoint;

	// Token: 0x04001741 RID: 5953
	private MapCustom mapCustom;

	// Token: 0x04001742 RID: 5954
	private bool hasMapCustom;

	// Token: 0x04001743 RID: 5955
	private bool isCart;

	// Token: 0x04001744 RID: 5956
	private PhysGrabCart physGrabCart;

	// Token: 0x04001745 RID: 5957
	[HideInInspector]
	public List<Transform> colliders = new List<Transform>();

	// Token: 0x04001746 RID: 5958
	[HideInInspector]
	public Vector3 centerPoint;

	// Token: 0x04001747 RID: 5959
	public Vector3 camRelForward;

	// Token: 0x04001748 RID: 5960
	public Vector3 camRelUp;

	// Token: 0x04001749 RID: 5961
	internal bool frozen;

	// Token: 0x0400174A RID: 5962
	private float frozenTimer;

	// Token: 0x0400174B RID: 5963
	private Vector3 frozenPosition;

	// Token: 0x0400174C RID: 5964
	private Quaternion frozenRotation;

	// Token: 0x0400174D RID: 5965
	private Vector3 frozenVelocity;

	// Token: 0x0400174E RID: 5966
	private Vector3 frozenAngularVelocity;

	// Token: 0x0400174F RID: 5967
	private Vector3 frozenForce;

	// Token: 0x04001750 RID: 5968
	private Vector3 frozenTorque;

	// Token: 0x04001751 RID: 5969
	private float overrideDragGoDownTimer;

	// Token: 0x04001752 RID: 5970
	private float overrideAngularDragGoDownTimer;

	// Token: 0x04001753 RID: 5971
	private float overrideMassGoDownTimer;

	// Token: 0x04001754 RID: 5972
	internal float impactHappenedTimer;

	// Token: 0x04001755 RID: 5973
	internal float impactLightTimer;

	// Token: 0x04001756 RID: 5974
	internal float impactMediumTimer;

	// Token: 0x04001757 RID: 5975
	internal float impactHeavyTimer;

	// Token: 0x04001758 RID: 5976
	internal float breakLightTimer;

	// Token: 0x04001759 RID: 5977
	internal float breakMediumTimer;

	// Token: 0x0400175A RID: 5978
	internal float breakHeavyTimer;

	// Token: 0x0400175B RID: 5979
	internal bool hasNeverBeenGrabbed = true;

	// Token: 0x0400175C RID: 5980
	[HideInInspector]
	public Vector3 boundingBox;

	// Token: 0x0400175D RID: 5981
	internal Vector3 spawnTorque = Vector3.zero;

	// Token: 0x0400175E RID: 5982
	private float smoothRotationDelta;

	// Token: 0x0400175F RID: 5983
	private bool rbIsSleepingPrevious;

	// Token: 0x04001760 RID: 5984
	private float overrideTorqueStrengthX = 1f;

	// Token: 0x04001761 RID: 5985
	private float overrideTorqueStrengthXTimer;

	// Token: 0x04001762 RID: 5986
	private float overrideTorqueStrengthY = 1f;

	// Token: 0x04001763 RID: 5987
	private float overrideTorqueStrengthYTimer;

	// Token: 0x04001764 RID: 5988
	private float overrideTorqueStrengthZ = 1f;

	// Token: 0x04001765 RID: 5989
	private float overrideTorqueStrengthZTimer;

	// Token: 0x04001766 RID: 5990
	private float overrideTorqueStrength = 1f;

	// Token: 0x04001767 RID: 5991
	private float overrideTorqueStrengthTimer;

	// Token: 0x04001768 RID: 5992
	private float overrideGrabStrength = 1f;

	// Token: 0x04001769 RID: 5993
	private float overrideGrabStrengthTimer;

	// Token: 0x0400176A RID: 5994
	private float overrideGrabRelativeVerticalPosition;

	// Token: 0x0400176B RID: 5995
	private float overrideGrabRelativeVerticalPositionTimer;

	// Token: 0x0400176C RID: 5996
	private float overrideGrabRelativeHorizontalPosition;

	// Token: 0x0400176D RID: 5997
	private float overrideGrabRelativeHorizontalPositionTimer;

	// Token: 0x0400176E RID: 5998
	internal bool physRidingDisabled;

	// Token: 0x0400176F RID: 5999
	private float overrideMinTorqueStrength;

	// Token: 0x04001770 RID: 6000
	private float overrideMinTorqueStrengthTimer;

	// Token: 0x04001771 RID: 6001
	private float overrideMinGrabStrength;

	// Token: 0x04001772 RID: 6002
	private float overrideMinGrabStrengthTimer;

	// Token: 0x04001773 RID: 6003
	private bool overrideExtraGrabStrengthDisable;

	// Token: 0x04001774 RID: 6004
	private float overrideExtraGrabStrengthDisableTimer;

	// Token: 0x04001775 RID: 6005
	private bool overrideExtraTorqueStrengthDisable;

	// Token: 0x04001776 RID: 6006
	private float overrideExtraTorqueStrengthDisableTimer;

	// Token: 0x04001777 RID: 6007
	internal bool isRotating;

	// Token: 0x04001778 RID: 6008
	private float isRotatingTimer;

	// Token: 0x04001779 RID: 6009
	private float itemHeightY;

	// Token: 0x0400177A RID: 6010
	private float itemWidthX;

	// Token: 0x0400177B RID: 6011
	private float itemLengthZ;

	// Token: 0x0400177C RID: 6012
	private float overrideKnockOutOfGrabDisableTimer;

	// Token: 0x0400177D RID: 6013
	internal bool overrideKnockOutOfGrabDisable;

	// Token: 0x0400177E RID: 6014
	internal bool isGun;
}
