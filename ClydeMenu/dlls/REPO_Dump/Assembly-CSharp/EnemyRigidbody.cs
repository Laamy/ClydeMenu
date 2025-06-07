using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000A5 RID: 165
public class EnemyRigidbody : MonoBehaviour
{
	// Token: 0x0600068C RID: 1676 RVA: 0x0003EA44 File Offset: 0x0003CC44
	private void Awake()
	{
		this.enemyParent = base.GetComponentInParent<EnemyParent>();
		this.yOffset = base.transform.position.y - this.followTarget.position.y;
		this.enemy.Rigidbody = this;
		this.enemy.HasRigidbody = true;
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.impactDetector.impactFragilityMultiplier = this.impactFragility;
		if (this.playerCollision)
		{
			this.hasPlayerCollision = true;
			this.playerCollisionActive = true;
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.playerCollision.enabled = false;
			}
		}
		this.rb = base.GetComponent<Rigidbody>();
		this.photonView = base.GetComponent<PhotonView>();
		this.overchargeMultiplier = this.enemyParent.overchargeMultiplier;
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x0003EB1C File Offset: 0x0003CD1C
	public void IdleSet(float time)
	{
		this.idleTimer = time;
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x0003EB28 File Offset: 0x0003CD28
	private void Update()
	{
		if (this.physGrabObject.playerGrabbing.Count > 0)
		{
			this.PhysGrabOverCharge();
			if (this.physGrabObject.grabbedLocal)
			{
				ItemInfoUI.instance.ItemInfoText(null, this.enemyParent.enemyName, true);
			}
			this.onGrabbedPlayerAvatar = this.physGrabObject.playerGrabbing[0].playerAvatar;
			this.onGrabbedPosition = this.physGrabObject.playerGrabbing[0].physGrabPoint.position;
			this.onGrabbed.Invoke();
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.physGrabObject.enemyInteractTimer = 10f;
			if (this.touchingCartTimer > 0f)
			{
				this.touchingCartTimer -= Time.deltaTime;
			}
			if (this.stunMassOverride && this.enemy.IsStunned())
			{
				this.physGrabObject.OverrideMass(this.physGrabObject.massOriginal * this.stunMassOverrideMultiplier, 0.1f);
			}
			this.positionSpeed = this.positionSpeedChase;
			this.rotationSpeed = this.rotationSpeedChase;
			this.distanceWarp = this.distanceWarpChase;
			this.positionSpeedLerpCurrent = this.positionSpeedLerpChase;
			if (this.idleTimer > 0f)
			{
				this.positionSpeed = this.positionSpeedIdle;
				this.rotationSpeed = this.rotationSpeedIdle;
				this.distanceWarp = this.distanceWarpIdle;
				this.positionSpeedLerpCurrent = this.positionSpeedLerpIdle;
				this.idleTimer -= Time.deltaTime;
			}
			if (this.overrideFollowPositionTimer > 0f)
			{
				this.positionSpeed = this.overrideFollowPositionSpeed;
				if (this.overrideFollowPositionLerp != -1f)
				{
					this.positionSpeedLerpCurrent = this.overrideFollowPositionLerp;
				}
				this.overrideFollowPositionTimer -= Time.deltaTime;
			}
			if (this.overrideFollowRotationTimer > 0f)
			{
				this.rotationSpeed = this.overrideFollowRotationSpeed;
				this.overrideFollowRotationTimer -= Time.deltaTime;
			}
			if (this.disableNoGravityTimer > 0f)
			{
				this.disableNoGravityTimer -= Time.deltaTime;
			}
			else if (!this.gravity)
			{
				this.physGrabObject.OverrideZeroGravity(0.1f);
			}
		}
		if (!this.enemy.IsStunned())
		{
			this.impactDetector.ImpactDisable(0.25f);
		}
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x0003ED70 File Offset: 0x0003CF70
	private void FixedUpdate()
	{
		if (!this.frozen)
		{
			this.velocity = this.physGrabObject.rbVelocity;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.physGrabObject.spawned)
		{
			if (this.teleportedTimer > 0f)
			{
				this.rb.velocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
				this.teleportedTimer -= Time.fixedDeltaTime;
				return;
			}
			if (this.hasPlayerCollision)
			{
				if (this.enemy.IsStunned())
				{
					if (this.playerCollisionActive)
					{
						this.playerCollisionActive = false;
						this.playerCollision.enabled = false;
					}
				}
				else if (!this.playerCollisionActive)
				{
					this.playerCollisionActive = true;
					this.playerCollision.enabled = true;
				}
			}
			if (this.enemy.FreezeTimer > 0f)
			{
				this.rb.velocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
				return;
			}
			if (this.frozen)
			{
				this.rb.AddForce(this.freezeVelocity, ForceMode.VelocityChange);
				this.rb.AddTorque(this.freezeAngularVelocity, ForceMode.VelocityChange);
				this.rb.AddForce(this.freezeForce, ForceMode.Impulse);
				this.rb.AddTorque(this.freezeTorque, ForceMode.Impulse);
				this.freezeForce = Vector3.zero;
				this.freezeTorque = Vector3.zero;
				this.frozen = false;
				return;
			}
			bool flag = false;
			if (this.physGrabObject.playerGrabbing.Count > 0)
			{
				this.enemy.SetChaseTarget(this.physGrabObject.playerGrabbing[0].playerAvatar);
				if (this.physGrabObject.grabDisplacementCurrent.magnitude >= this.grabForceNeeded.amount || EnemyDirector.instance.debugEasyGrab)
				{
					this.grabShakeReleaseTimer = 0f;
					this.grabForceTimer += Time.fixedDeltaTime;
					if (this.grabForceTimer >= this.grabTimeNeeded)
					{
						flag = true;
						if (this.grabOverride)
						{
							this.grabStrengthTimer = this.grabStrengthTime;
						}
					}
				}
				else
				{
					this.grabShakeReleaseTimer += Time.fixedDeltaTime;
				}
				if (this.grabShakeReleaseTimer > 3f && this.enemy.StateStunned.stunTimer <= 0.25f && !this.grabbed)
				{
					this.GrabReleaseShake();
				}
				this.grabTimeCurrent += Time.fixedDeltaTime;
				if (!EnemyDirector.instance.debugNoGrabMaxTime && this.enemy.StateStunned.stunTimer <= 0.25f && this.grabTimeCurrent >= this.grabTimeMaxRandom * (float)this.physGrabObject.playerGrabbing.Count)
				{
					this.GrabReleaseShake();
				}
			}
			else
			{
				this.grabTimeCurrent = 0f;
				this.grabTimeMaxRandom = this.grabTimeMax * Random.Range(0.9f, 1.1f);
				this.grabForceTimer = 0f;
				this.grabShakeReleaseTimer = 0f;
			}
			if (this.grabStrengthTimer > 0f)
			{
				flag = true;
				if (this.grabStun && this.enemy.HasStateStunned)
				{
					this.enemy.StateStunned.Set(0.1f);
				}
				if (this.rb.velocity.magnitude < 2f)
				{
					this.grabStrengthTimer -= Time.fixedDeltaTime;
					if (this.grabStrengthTimer <= 0f)
					{
						this.GrabReleaseShake();
					}
				}
			}
			if (flag)
			{
				this.enemy.StuckCount = 0;
				if (this.enemy.HasJump)
				{
					this.enemy.Jump.jumpCooldown = 1f;
				}
			}
			if (this.grabbedPrevious != flag)
			{
				this.GrabbedSet(flag);
			}
			if (this.customGravity > 0f && this.gravity && this.disableNoGravityTimer <= 0f && this.rb.useGravity && this.physGrabObject.playerGrabbing.Count <= 0)
			{
				this.rb.AddForce(-Vector3.up * this.customGravity, ForceMode.Force);
			}
			if (this.grabbed)
			{
				if (this.materialState != 0)
				{
					Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].material = this.ColliderMaterialGrabbed;
					}
					this.materialState = 0;
				}
			}
			else if (this.enemy.IsStunned() || this.colliderMaterialStunnedOverrideTimer > 0f)
			{
				if (this.materialState != 1)
				{
					Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].material = this.ColliderMaterialStunned;
					}
					this.materialState = 1;
				}
			}
			else if (this.disableFollowPositionTimer > 0f || this.disableFollowRotationTimer > 0f)
			{
				if (this.materialState != 2)
				{
					Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].material = this.ColliderMaterialDisabled;
					}
					this.materialState = 2;
				}
			}
			else if (this.enemy.HasJump && this.enemy.Jump.jumping)
			{
				if (this.materialState != 3)
				{
					Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].material = this.ColliderMaterialJumping;
					}
					this.materialState = 3;
				}
			}
			else if (this.materialState != 4)
			{
				Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].material = this.ColliderMaterialDefault;
				}
				this.materialState = 4;
			}
			if (this.colliderMaterialStunnedOverrideTimer > 0f)
			{
				this.colliderMaterialStunnedOverrideTimer -= Time.fixedDeltaTime;
			}
			if (this.disableFollowRotationTimer <= 0f && !this.enemy.IsStunned())
			{
				this.rotationSpeedLerp += this.disableFollowRotationResetSpeed * Time.fixedDeltaTime;
				this.rotationSpeedLerp = Mathf.Clamp01(this.rotationSpeedLerp);
				this.rotationSpeedCurrent = Mathf.Lerp(0f, this.rotationSpeed, this.speedResetCurve.Evaluate(this.rotationSpeedLerp));
				Vector3 vector = SemiFunc.PhysFollowRotation(base.transform, this.followTarget.rotation, this.rb, this.rotationSpeedCurrent);
				if (this.grabStrengthTimer > 0f)
				{
					vector = Vector3.Lerp(Vector3.zero, vector, this.grabRotationStrength);
				}
				this.rb.AddTorque(vector, ForceMode.Impulse);
			}
			else
			{
				this.rotationSpeedLerp = 0f;
				this.disableFollowRotationTimer -= Time.fixedDeltaTime;
			}
			if (this.disableFollowPositionTimer <= 0f && !this.enemy.IsStunned())
			{
				this.timeSinceStun += Time.fixedDeltaTime;
				this.positionSpeedLerp += this.disableFollowPositionResetSpeed * Time.fixedDeltaTime;
				this.positionSpeedLerp = Mathf.Clamp01(this.positionSpeedLerp);
				this.positionSpeedCurrent = Mathf.Lerp(0f, this.positionSpeed, this.speedResetCurve.Evaluate(this.positionSpeedLerp));
				Vector3 vector2 = SemiFunc.PhysFollowPosition(this.rb.transform.position, this.followTarget.position, this.rb.velocity, this.positionSpeedCurrent);
				if (this.grabStrengthTimer > 0f)
				{
					vector2 = Vector3.Lerp(Vector3.zero, vector2, this.grabPositionStrength);
				}
				if ((this.gravity || this.disableNoGravityTimer > 0f) && this.physGrabObject.playerGrabbing.Count <= 0)
				{
					vector2.y = 0f;
				}
				vector2 = Vector3.Lerp(this.positionForce, vector2, this.positionSpeedLerpCurrent * Time.fixedDeltaTime);
				this.rb.AddForce(vector2, ForceMode.Impulse);
			}
			else
			{
				this.timeSinceStun = 0f;
				this.positionSpeedLerp = 0f;
				this.disableFollowPositionTimer -= Time.fixedDeltaTime;
			}
			if (!this.grabbed && Vector3.Distance(this.lastMovingPosition, base.transform.position) < this.notMovingDistance)
			{
				this.notMovingTimer += Time.fixedDeltaTime;
			}
			else
			{
				this.lastMovingPosition = base.transform.position;
				this.notMovingTimer = 0f;
			}
			if (this.enemy.HasNavMeshAgent && !this.grabbed)
			{
				float num = Vector3.Distance(new Vector3(this.followTarget.position.x, 0f, this.followTarget.position.z), new Vector3(this.rb.position.x, 0f, this.rb.position.z));
				bool flag2 = false;
				if (this.enemy.HasJump && this.enemy.Jump.jumping)
				{
					flag2 = true;
				}
				if (this.warpDisableTimer <= 0f && num >= this.distanceWarp && !flag2)
				{
					if (this.enemy.NavMeshAgent.IsDisabled() || this.enemy.NavMeshAgent.IsStopped())
					{
						this.enemy.transform.position = this.rb.position;
						this.timeSinceLastWarp = 0f;
						if (LevelGenerator.Instance.Generated && (!this.enemy.HasAttackPhysObject || !this.enemy.AttackStuckPhysObject.Active) && this.notMovingTimer >= 1f)
						{
							this.enemy.StuckCount++;
						}
					}
					else if (this.enemy.NavMeshAgent.Agent.velocity.magnitude > 0.1f || num >= this.distanceWarp * 2f)
					{
						RaycastHit raycastHit;
						if (Physics.Raycast(this.rb.position + Vector3.up * 0.1f, Vector3.down, out raycastHit, 10f, LayerMask.GetMask(new string[]
						{
							"Default",
							"NavmeshOnly",
							"PlayerOnlyCollision"
						})))
						{
							this.enemy.NavMeshAgent.AgentMove(raycastHit.point);
						}
						else
						{
							this.enemy.NavMeshAgent.AgentMove(this.rb.position);
						}
						this.timeSinceLastWarp = 0f;
						if (LevelGenerator.Instance.Generated && (!this.enemy.HasAttackPhysObject || !this.enemy.AttackStuckPhysObject.Active) && this.notMovingTimer >= 1f)
						{
							this.enemy.StuckCount++;
						}
					}
				}
				else if (!this.enemy.NavMeshAgent.IsDisabled() && !this.enemy.NavMeshAgent.IsStopped())
				{
					this.timeSinceLastWarp += Time.fixedDeltaTime;
					if (this.timeSinceLastWarp >= 3f)
					{
						this.enemy.StuckCount = 0;
					}
				}
			}
			if (this.warpDisableTimer > 0f)
			{
				this.warpDisableTimer -= Time.fixedDeltaTime;
			}
			if (this.stunFromFall && (!this.enemy.HasJump || !this.enemy.Jump.jumping) && !this.grabbed && this.gravity && this.disableNoGravityTimer <= 0f && this.rb.useGravity && (!this.enemy.HasGrounded || !this.enemy.Grounded.grounded))
			{
				if (this.rb.velocity.y < -2f)
				{
					if (this.stunFromFallTimer >= this.stunFromFallTime && this.enemy.HasStateStunned)
					{
						if (!this.enemy.IsStunned())
						{
							this.rb.AddTorque(-base.transform.right * (this.rb.mass * 0.5f), ForceMode.Impulse);
						}
						this.enemy.StateStunned.Set(3f);
					}
					this.stunFromFallTimer += Time.fixedDeltaTime;
					return;
				}
				this.stunFromFallTimer = 0f;
				return;
			}
			else
			{
				this.stunFromFallTimer = 0f;
			}
		}
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x0003F9CC File Offset: 0x0003DBCC
	private void PhysGrabOverCharge()
	{
		int count = this.physGrabObject.playerGrabbing.Count;
		int num = (int)(this.enemy.EnemyParent.difficulty + 1);
		float num2 = 0.08f * this.overchargeMultiplier;
		if (this.physGrabObject.grabbedLocal)
		{
			float amount = num2 * (float)num / (float)count;
			PhysGrabber.instance.PhysGrabOverCharge(amount, this.overchargeMultiplier);
		}
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x0003FA30 File Offset: 0x0003DC30
	private void OnCollisionStay(Collision other)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.enemy.CurrentState == EnemyState.Despawn)
			{
				return;
			}
			if (other.gameObject.CompareTag("Phys Grab Object"))
			{
				PhysGrabObject physGrabObject = other.gameObject.GetComponent<PhysGrabObject>();
				if (!physGrabObject)
				{
					physGrabObject = other.gameObject.GetComponentInParent<PhysGrabObject>();
				}
				if (physGrabObject)
				{
					this.onTouchPhysObjectPhysObject = physGrabObject;
					this.onTouchPhysObjectPosition = other.GetContact(0).point;
					this.onTouchPhysObject.Invoke();
					physGrabObject.EnemyInteractTimeSet();
					PhysGrabCart component = physGrabObject.GetComponent<PhysGrabCart>();
					if (component)
					{
						this.touchingCartTimer = 0.25f;
						foreach (PhysGrabInCart.CartObject cartObject in Enumerable.ToList<PhysGrabInCart.CartObject>(component.physGrabInCart.inCartObjects))
						{
							cartObject.physGrabObject.EnemyInteractTimeSet();
						}
					}
					if (this.enemy.CheckChase())
					{
						if (this.enemy.FreezeTimer <= 0f)
						{
							Vector3 normalized = (physGrabObject.centerPoint - this.physGrabObject.centerPoint).normalized;
							if (Vector3.Dot((this.followTarget.position - this.physGrabObject.centerPoint).normalized, normalized) > 0f)
							{
								Vector3 force = normalized * 10f;
								force.y = 5f;
								physGrabObject.rb.AddForce(force, ForceMode.Impulse);
								physGrabObject.rb.AddTorque(Random.insideUnitSphere * force.magnitude, ForceMode.Impulse);
								physGrabObject.lightBreakImpulse = true;
								PhysGrabHinge component2 = physGrabObject.GetComponent<PhysGrabHinge>();
								if (component2 && component2.brokenTimer >= 1.5f)
								{
									component2.DestroyHinge();
								}
								GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 15f, base.transform.position, 0.1f);
								GameDirector.instance.CameraShake.ShakeDistance(5f, 5f, 15f, base.transform.position, 0.1f);
								this.rb.AddForce(-normalized * 2f, ForceMode.Impulse);
								this.DisableFollowPosition(0.1f, 5f);
								return;
							}
						}
					}
					else
					{
						PlayerTumble component3 = physGrabObject.GetComponent<PlayerTumble>();
						if (component3)
						{
							this.onTouchPlayerAvatar = component3.playerAvatar;
							this.onTouchPlayer.Invoke();
							this.enemy.SetChaseTarget(component3.playerAvatar);
							return;
						}
						if (physGrabObject.playerGrabbing.Count > 0)
						{
							PlayerAvatar playerAvatar = physGrabObject.playerGrabbing[0].playerAvatar;
							this.onTouchPlayerGrabbedObjectAvatar = playerAvatar;
							this.onTouchPlayerGrabbedObjectPhysObject = physGrabObject;
							this.onTouchPlayerGrabbedObjectPosition = other.GetContact(0).point;
							this.onTouchPlayerGrabbedObject.Invoke();
							this.enemy.SetChaseTarget(playerAvatar);
							return;
						}
					}
				}
			}
			else if (other.gameObject.CompareTag("Player"))
			{
				PlayerController componentInParent = other.gameObject.GetComponentInParent<PlayerController>();
				if (componentInParent)
				{
					this.onTouchPlayerAvatar = componentInParent.playerAvatarScript;
					this.onTouchPlayer.Invoke();
					this.enemy.SetChaseTarget(componentInParent.playerAvatarScript);
					return;
				}
				PlayerAvatar componentInParent2 = other.gameObject.GetComponentInParent<PlayerAvatar>();
				if (componentInParent2)
				{
					this.onTouchPlayerAvatar = componentInParent2;
					this.onTouchPlayer.Invoke();
					this.enemy.SetChaseTarget(componentInParent2);
				}
			}
		}
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x0003FDD0 File Offset: 0x0003DFD0
	public void DisableFollowPosition(float time, float resetSpeed)
	{
		this.disableFollowPositionTimer = time;
		this.disableFollowPositionResetSpeed = resetSpeed;
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x0003FDE0 File Offset: 0x0003DFE0
	public void DisableFollowRotation(float time, float resetSpeed)
	{
		this.disableFollowRotationTimer = time;
		this.disableFollowRotationResetSpeed = resetSpeed;
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x0003FDF0 File Offset: 0x0003DFF0
	public void DisableNoGravity(float time)
	{
		this.disableNoGravityTimer = time;
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x0003FDF9 File Offset: 0x0003DFF9
	public void OverrideFollowPosition(float time, float speed, float lerp = -1f)
	{
		this.overrideFollowPositionTimer = time;
		this.overrideFollowPositionSpeed = speed;
		this.overrideFollowPositionLerp = lerp;
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x0003FE10 File Offset: 0x0003E010
	public void OverrideFollowRotation(float time, float speed)
	{
		this.overrideFollowRotationTimer = time;
		this.overrideFollowRotationSpeed = speed;
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x0003FE20 File Offset: 0x0003E020
	public void Teleport()
	{
		this.physGrabObject.Teleport(this.followTarget.position + new Vector3(0f, this.yOffset, 0f), this.followTarget.rotation);
		if (!this.rb.isKinematic)
		{
			this.rb.velocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
		}
		this.freezeForce = Vector3.zero;
		this.freezeTorque = Vector3.zero;
		this.frozen = false;
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x0003FEB4 File Offset: 0x0003E0B4
	public void FreezeForces(Vector3 force, Vector3 torque)
	{
		if (!this.frozen)
		{
			this.freezeVelocity = this.rb.velocity;
			this.freezeAngularVelocity = this.rb.angularVelocity;
			this.frozen = true;
		}
		this.freezeForce += force;
		this.freezeTorque += torque;
		this.rb.velocity = Vector3.zero;
		this.rb.angularVelocity = Vector3.zero;
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x0003FF38 File Offset: 0x0003E138
	public void JumpImpulse()
	{
		if (this.materialState != 3)
		{
			Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material = this.ColliderMaterialJumping;
			}
			this.materialState = 3;
		}
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x0003FF78 File Offset: 0x0003E178
	public void StuckReset()
	{
		this.notMovingTimer = 0f;
		this.enemy.StuckCount = 0;
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x0003FF91 File Offset: 0x0003E191
	public void WarpDisable(float time)
	{
		this.warpDisableTimer = time;
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x0003FF9A File Offset: 0x0003E19A
	public void OverrideColliderMaterialStunned(float _time)
	{
		this.colliderMaterialStunnedOverrideTimer = _time;
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x0003FFA4 File Offset: 0x0003E1A4
	public void LightImpact()
	{
		if (this.enemy.HasHealth)
		{
			this.enemy.Health.LightImpact();
		}
		GameDirector.instance.CameraShake.ShakeDistance(this.impactShakeLight, 5f, 15f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(this.impactShakeLight, 5f, 15f, base.transform.position, 0.1f);
		this.onImpactLight.Invoke();
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00040038 File Offset: 0x0003E238
	public void MediumImpact()
	{
		if (this.enemy.HasHealth)
		{
			this.enemy.Health.MediumImpact();
		}
		GameDirector.instance.CameraShake.ShakeDistance(this.impactShakeMedium, 5f, 15f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(this.impactShakeMedium, 5f, 15f, base.transform.position, 0.1f);
		this.onImpactMedium.Invoke();
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x000400CC File Offset: 0x0003E2CC
	public void HeavyImpact()
	{
		if (this.enemy.HasHealth)
		{
			this.enemy.Health.HeavyImpact();
		}
		GameDirector.instance.CameraShake.ShakeDistance(this.impactShakeHeavy, 5f, 15f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(this.impactShakeHeavy, 5f, 15f, base.transform.position, 0.1f);
		this.onImpactHeavy.Invoke();
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00040160 File Offset: 0x0003E360
	public void GrabRelease()
	{
		bool flag = false;
		foreach (PhysGrabber physGrabber in Enumerable.ToList<PhysGrabber>(this.physGrabObject.playerGrabbing))
		{
			if (!SemiFunc.IsMultiplayer())
			{
				physGrabber.ReleaseObject(0.1f);
			}
			else
			{
				physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
				{
					false,
					0.1f
				});
			}
			flag = true;
		}
		if (flag)
		{
			if (GameManager.instance.gameMode == 0)
			{
				this.GrabReleaseRPC(default(PhotonMessageInfo));
				return;
			}
			this.photonView.RPC("GrabReleaseRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00040230 File Offset: 0x0003E430
	[PunRPC]
	private void GrabReleaseRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.physGrabObject.grabDisableTimer = 1f;
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x000402B4 File Offset: 0x0003E4B4
	private void GrabReleaseShake()
	{
		this.grabStrengthTimer = 0f;
		this.GrabbedSet(false);
		float d = 1f * this.rb.mass;
		this.rb.AddRelativeTorque(Vector3.up * d, ForceMode.Impulse);
		this.GrabRelease();
		this.DisableFollowRotation(0.5f, 50f);
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00040314 File Offset: 0x0003E514
	private void GrabbedSet(bool _grabbed)
	{
		this.grabbed = _grabbed;
		this.grabbedPrevious = _grabbed;
		if (GameManager.Multiplayer() && PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("GrabbedSetRPC", RpcTarget.All, new object[]
			{
				this.grabbed
			});
		}
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00040362 File Offset: 0x0003E562
	[PunRPC]
	private void GrabbedSetRPC(bool _grabbed, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.grabbed = _grabbed;
	}

	// Token: 0x04000ABA RID: 2746
	public Enemy enemy;

	// Token: 0x04000ABB RID: 2747
	public Transform followTarget;

	// Token: 0x04000ABC RID: 2748
	internal PhysGrabObject physGrabObject;

	// Token: 0x04000ABD RID: 2749
	internal PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04000ABE RID: 2750
	internal PhotonView photonView;

	// Token: 0x04000ABF RID: 2751
	internal Rigidbody rb;

	// Token: 0x04000AC0 RID: 2752
	internal Vector3 velocity;

	// Token: 0x04000AC1 RID: 2753
	[Space]
	public bool gravity = true;

	// Token: 0x04000AC2 RID: 2754
	public float customGravity;

	// Token: 0x04000AC3 RID: 2755
	[Space]
	public GrabForce grabForceNeeded;

	// Token: 0x04000AC4 RID: 2756
	public float grabTimeNeeded = 0.5f;

	// Token: 0x04000AC5 RID: 2757
	private float grabForceTimer;

	// Token: 0x04000AC6 RID: 2758
	private float grabShakeReleaseTimer;

	// Token: 0x04000AC7 RID: 2759
	public bool grabStun;

	// Token: 0x04000AC8 RID: 2760
	public bool grabOverride;

	// Token: 0x04000AC9 RID: 2761
	public float grabPositionStrength;

	// Token: 0x04000ACA RID: 2762
	public float grabRotationStrength;

	// Token: 0x04000ACB RID: 2763
	public float grabStrengthTime = 1f;

	// Token: 0x04000ACC RID: 2764
	internal float grabStrengthTimer;

	// Token: 0x04000ACD RID: 2765
	public float grabTimeMax = 3f;

	// Token: 0x04000ACE RID: 2766
	private float grabTimeMaxRandom;

	// Token: 0x04000ACF RID: 2767
	private float grabTimeCurrent;

	// Token: 0x04000AD0 RID: 2768
	internal bool grabbed;

	// Token: 0x04000AD1 RID: 2769
	private bool grabbedPrevious;

	// Token: 0x04000AD2 RID: 2770
	[Space]
	public float positionSpeedIdle = 1f;

	// Token: 0x04000AD3 RID: 2771
	public float positionSpeedLerpIdle = 10f;

	// Token: 0x04000AD4 RID: 2772
	public float positionSpeedChase = 2f;

	// Token: 0x04000AD5 RID: 2773
	public float positionSpeedLerpChase = 50f;

	// Token: 0x04000AD6 RID: 2774
	private float positionSpeed;

	// Token: 0x04000AD7 RID: 2775
	private float positionSpeedCurrent;

	// Token: 0x04000AD8 RID: 2776
	internal float positionSpeedLerp = 1f;

	// Token: 0x04000AD9 RID: 2777
	private float positionSpeedLerpCurrent = 1f;

	// Token: 0x04000ADA RID: 2778
	private Vector3 positionForce;

	// Token: 0x04000ADB RID: 2779
	[Space]
	public float rotationSpeedIdle = 1f;

	// Token: 0x04000ADC RID: 2780
	public float rotationSpeedChase = 2f;

	// Token: 0x04000ADD RID: 2781
	private float rotationSpeed;

	// Token: 0x04000ADE RID: 2782
	private float rotationSpeedCurrent;

	// Token: 0x04000ADF RID: 2783
	private float rotationSpeedLerp = 1f;

	// Token: 0x04000AE0 RID: 2784
	[Space]
	public float distanceWarpIdle = 1f;

	// Token: 0x04000AE1 RID: 2785
	public float distanceWarpChase = 2f;

	// Token: 0x04000AE2 RID: 2786
	private float distanceWarp;

	// Token: 0x04000AE3 RID: 2787
	private float timeSinceLastWarp;

	// Token: 0x04000AE4 RID: 2788
	[Space]
	public float notMovingDistance = 1f;

	// Token: 0x04000AE5 RID: 2789
	internal float notMovingTimer;

	// Token: 0x04000AE6 RID: 2790
	private Vector3 lastMovingPosition;

	// Token: 0x04000AE7 RID: 2791
	[Space]
	public bool stunFromFall = true;

	// Token: 0x04000AE8 RID: 2792
	private float stunFromFallTime = 1f;

	// Token: 0x04000AE9 RID: 2793
	private float stunFromFallTimer;

	// Token: 0x04000AEA RID: 2794
	public bool stunMassOverride;

	// Token: 0x04000AEB RID: 2795
	public float stunMassOverrideMultiplier = 1f;

	// Token: 0x04000AEC RID: 2796
	[Space]
	public AnimationCurve speedResetCurve;

	// Token: 0x04000AED RID: 2797
	public float stunResetSpeed = 10f;

	// Token: 0x04000AEE RID: 2798
	internal float disableFollowPositionTimer;

	// Token: 0x04000AEF RID: 2799
	internal float disableFollowPositionResetSpeed;

	// Token: 0x04000AF0 RID: 2800
	internal float disableFollowRotationTimer;

	// Token: 0x04000AF1 RID: 2801
	internal float disableFollowRotationResetSpeed;

	// Token: 0x04000AF2 RID: 2802
	internal float disableNoGravityTimer;

	// Token: 0x04000AF3 RID: 2803
	internal float overrideFollowPositionTimer;

	// Token: 0x04000AF4 RID: 2804
	internal float overrideFollowPositionSpeed;

	// Token: 0x04000AF5 RID: 2805
	internal float overrideFollowPositionLerp;

	// Token: 0x04000AF6 RID: 2806
	internal float overrideFollowRotationTimer;

	// Token: 0x04000AF7 RID: 2807
	internal float overrideFollowRotationSpeed;

	// Token: 0x04000AF8 RID: 2808
	private float idleTimer;

	// Token: 0x04000AF9 RID: 2809
	internal float timeSinceStun;

	// Token: 0x04000AFA RID: 2810
	[Space]
	public PhysicMaterial ColliderMaterialDefault;

	// Token: 0x04000AFB RID: 2811
	public PhysicMaterial ColliderMaterialDisabled;

	// Token: 0x04000AFC RID: 2812
	public PhysicMaterial ColliderMaterialStunned;

	// Token: 0x04000AFD RID: 2813
	public PhysicMaterial ColliderMaterialGrabbed;

	// Token: 0x04000AFE RID: 2814
	public PhysicMaterial ColliderMaterialJumping;

	// Token: 0x04000AFF RID: 2815
	private float colliderMaterialStunnedOverrideTimer;

	// Token: 0x04000B00 RID: 2816
	[Space]
	public Collider playerCollision;

	// Token: 0x04000B01 RID: 2817
	private bool hasPlayerCollision;

	// Token: 0x04000B02 RID: 2818
	private bool playerCollisionActive;

	// Token: 0x04000B03 RID: 2819
	private int materialState = -1;

	// Token: 0x04000B04 RID: 2820
	internal float teleportedTimer;

	// Token: 0x04000B05 RID: 2821
	internal float touchingCartTimer;

	// Token: 0x04000B06 RID: 2822
	internal bool frozen;

	// Token: 0x04000B07 RID: 2823
	private Vector3 freezeVelocity;

	// Token: 0x04000B08 RID: 2824
	private Vector3 freezeAngularVelocity;

	// Token: 0x04000B09 RID: 2825
	private Vector3 freezeForce;

	// Token: 0x04000B0A RID: 2826
	private Vector3 freezeTorque;

	// Token: 0x04000B0B RID: 2827
	internal float yOffset;

	// Token: 0x04000B0C RID: 2828
	private float overchargeMultiplier;

	// Token: 0x04000B0D RID: 2829
	[Space]
	public float impactShakeLight = 1f;

	// Token: 0x04000B0E RID: 2830
	public float impactShakeMedium = 2f;

	// Token: 0x04000B0F RID: 2831
	public float impactShakeHeavy = 4f;

	// Token: 0x04000B10 RID: 2832
	public float impactFragility = 1f;

	// Token: 0x04000B11 RID: 2833
	[Space]
	public UnityEvent onImpactLight;

	// Token: 0x04000B12 RID: 2834
	public UnityEvent onImpactMedium;

	// Token: 0x04000B13 RID: 2835
	public UnityEvent onImpactHeavy;

	// Token: 0x04000B14 RID: 2836
	public UnityEvent onTouchPlayer;

	// Token: 0x04000B15 RID: 2837
	internal PlayerAvatar onTouchPlayerAvatar;

	// Token: 0x04000B16 RID: 2838
	public UnityEvent onTouchPlayerGrabbedObject;

	// Token: 0x04000B17 RID: 2839
	internal PlayerAvatar onTouchPlayerGrabbedObjectAvatar;

	// Token: 0x04000B18 RID: 2840
	internal PhysGrabObject onTouchPlayerGrabbedObjectPhysObject;

	// Token: 0x04000B19 RID: 2841
	internal Vector3 onTouchPlayerGrabbedObjectPosition;

	// Token: 0x04000B1A RID: 2842
	public UnityEvent onTouchPhysObject;

	// Token: 0x04000B1B RID: 2843
	internal PhysGrabObject onTouchPhysObjectPhysObject;

	// Token: 0x04000B1C RID: 2844
	internal Vector3 onTouchPhysObjectPosition;

	// Token: 0x04000B1D RID: 2845
	public UnityEvent onGrabbed;

	// Token: 0x04000B1E RID: 2846
	internal PlayerAvatar onGrabbedPlayerAvatar;

	// Token: 0x04000B1F RID: 2847
	internal Vector3 onGrabbedPosition;

	// Token: 0x04000B20 RID: 2848
	private float warpDisableTimer;

	// Token: 0x04000B21 RID: 2849
	private EnemyParent enemyParent;
}
