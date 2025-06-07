using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001A1 RID: 417
public class PhysGrabObjectImpactDetector : MonoBehaviour, IPunObservable
{
	// Token: 0x06000DE6 RID: 3558 RVA: 0x00079D6C File Offset: 0x00077F6C
	private void Start()
	{
		this.inCartVolumeMultiplier = 0.6f;
		if (base.GetComponent<PhysGrabHinge>())
		{
			this.isHinge = true;
		}
		this.cart = base.GetComponent<PhysGrabCart>();
		if (this.cart)
		{
			this.isCart = true;
		}
		this.enemyRigidbody = base.GetComponent<EnemyRigidbody>();
		if (this.enemyRigidbody)
		{
			this.isEnemy = true;
		}
		this.previousSlidingPosition = base.transform.position;
		this.valuableObject = base.GetComponent<ValuableObject>();
		Transform transform = base.transform.Find("ForceCenterPoint");
		if (transform)
		{
			this.centerPoint = transform.position;
		}
		if (this.valuableObject)
		{
			this.isValuable = true;
			this.breakLogic = true;
			this.fragility = this.valuableObject.durabilityPreset.fragility;
			this.durability = this.valuableObject.durabilityPreset.durability;
			this.impactAudio = this.valuableObject.audioPreset;
			this.impactAudioPitch = this.valuableObject.audioPresetPitch;
		}
		else
		{
			this.notValuableObject = base.GetComponent<NotValuableObject>();
			this.isNotValuable = true;
			if (this.notValuableObject)
			{
				if (this.notValuableObject.durabilityPreset)
				{
					this.breakLogic = true;
					this.fragility = this.notValuableObject.durabilityPreset.fragility;
					this.durability = this.notValuableObject.durabilityPreset.durability;
				}
				this.impactAudio = this.notValuableObject.audioPreset;
				this.impactAudioPitch = this.notValuableObject.audioPresetPitch;
			}
		}
		if (this.impactAudio)
		{
			this.audioActive = true;
		}
		else
		{
			this.audioActive = false;
		}
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.rb = base.GetComponent<Rigidbody>();
		this.mainCamera = Camera.main;
		this.ColliderGet(base.transform);
		this.colliderVolume /= 200000f;
		GameObject gameObject = Object.Instantiate<GameObject>(Resources.Load<GameObject>("Phys Object Particles"), new Vector3(0f, 0f, 0f), Quaternion.identity);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.particles = gameObject.GetComponent<PhysObjectParticles>();
		this.particles.multiplier = this.particleMultiplier;
		if (this.isValuable)
		{
			this.particles.gradient = this.valuableObject.particleColors;
		}
		if (this.notValuableObject)
		{
			this.particles.gradient = this.notValuableObject.particleColors;
		}
		this.particles.colliderTransforms = this.colliderTransforms;
		this.originalPosition = this.rb.position;
		this.originalRotation = this.rb.rotation;
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x0007A064 File Offset: 0x00078264
	private void ColliderGet(Transform transform)
	{
		if (transform.CompareTag("Phys Grab Object") && transform.GetComponent<Collider>())
		{
			this.colliderTransforms.Add(transform);
			Bounds bounds = transform.transform.GetComponent<Collider>().bounds;
			float num = bounds.size.x * 100f * (bounds.size.y * 100f) * (bounds.size.z * 100f);
			if (transform.GetComponent<SphereCollider>())
			{
				num *= 0.55f;
			}
			this.colliderVolume += num;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			this.ColliderGet(transform2);
		}
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x0007A154 File Offset: 0x00078354
	[PunRPC]
	private void InCartRPC(bool inCartState, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.inCart = inCartState;
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x0007A166 File Offset: 0x00078366
	private void IndestructibleSpawnTimer()
	{
		if (this.indestructibleSpawnTimer > 0f)
		{
			this.physGrabObject.OverrideIndestructible(0.1f);
			this.indestructibleSpawnTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x0007A198 File Offset: 0x00078398
	private void Update()
	{
		this.IndestructibleSpawnTimer();
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.inCartPrevious != this.inCart)
		{
			this.inCartPrevious = this.inCart;
			if (GameManager.instance.gameMode == 1)
			{
				this.photonView.RPC("InCartRPC", RpcTarget.Others, new object[]
				{
					this.inCart
				});
			}
		}
		if (this.timerInCart > 0f)
		{
			this.inCart = true;
			this.timerInCart -= Time.deltaTime;
		}
		else if (this.inCart)
		{
			this.inCart = false;
			this.currentCartPrev = this.currentCart;
			this.currentCart = null;
		}
		if (this.isValuable && !this.valuableObject.dollarValueSet)
		{
			return;
		}
		if (this.isCollidingTimer > 0f)
		{
			this.isColliding = true;
			this.isCollidingTimer -= Time.deltaTime;
		}
		else
		{
			this.isColliding = false;
		}
		if (this.isSliding)
		{
			Vector3 b = this.previousSlidingPosition;
			b.y = 0f;
			Vector3 position = base.transform.position;
			position.y = 0f;
			float num = (position - b).magnitude / Time.deltaTime;
			if (num >= this.slidingSpeedThreshold)
			{
				this.slidingAudioSpeed = Mathf.Lerp(this.slidingAudioSpeed, 1f + num * 0.01f, 10f * Time.deltaTime);
				Materials.Instance.SlideLoop(this.rb.worldCenterOfMass, this.materialTrigger, 1f, 1f + this.slidingAudioSpeed);
			}
			if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
			{
				this.slidingTimer -= Time.deltaTime;
				if (this.slidingTimer < 0f)
				{
					this.isSliding = false;
				}
			}
		}
		this.previousSlidingPosition = base.transform.position;
		if (this.playerHurtMultiplierTimer > 0f)
		{
			this.playerHurtMultiplierTimer -= Time.deltaTime;
			if (this.playerHurtMultiplierTimer <= 0f)
			{
				this.playerHurtMultiplier = 1f;
			}
		}
		if (this.physGrabObject.grabbed)
		{
			this.collisionsActiveTimer = 0.5f;
		}
		if (this.rb.velocity.magnitude > 0.01f || this.rb.angularVelocity.magnitude > 0.1f)
		{
			this.collisionsActiveTimer = 0.5f;
		}
		if (this.collisionsActiveTimer > 0f)
		{
			if (!this.collisionsActive)
			{
				this.collisionActivatedBuffer = 0.1f;
			}
			this.collisionsActive = true;
			this.collisionsActiveTimer -= Time.deltaTime;
		}
		else
		{
			this.collisionsActive = false;
		}
		if (this.collisionActivatedBuffer > 0f)
		{
			this.collisionActivatedBuffer -= Time.deltaTime;
		}
		if (this.playerHitDisableTimer > 0f)
		{
			this.playerHitDisableTimer -= Time.deltaTime;
		}
		else if (this.playerHitDisable)
		{
			this.playerHitDisable = false;
			this.physGrabObject.PhysRidingDisabledSet(false);
		}
		if (this.breakLevel1Cooldown > 0f)
		{
			this.breakLevel1Cooldown -= Time.deltaTime;
		}
		if (this.breakLevel2Cooldown > 0f)
		{
			this.breakLevel2Cooldown -= Time.deltaTime;
		}
		if (this.breakLevel3Cooldown > 0f)
		{
			this.breakLevel3Cooldown -= Time.deltaTime;
		}
		if (this.impactLightCooldown > 0f)
		{
			this.impactLightCooldown -= Time.deltaTime;
		}
		if (this.impactMediumCooldown > 0f)
		{
			this.impactMediumCooldown -= Time.deltaTime;
		}
		if (this.impactHeavyCooldown > 0f)
		{
			this.impactHeavyCooldown -= Time.deltaTime;
		}
		if (this.impactCooldown > 0f)
		{
			this.impactCooldown -= Time.deltaTime;
		}
		if (this.impulseTimerDeactivateImpacts > 0f)
		{
			this.impulseTimerDeactivateImpacts -= Time.deltaTime;
		}
		if (this.resetPrevPositionTimer > 0f)
		{
			this.resetPrevPositionTimer -= Time.deltaTime;
			this.previousPosition = Vector3.zero;
		}
		if (this.enemyInteractionTimer > 0f)
		{
			this.enemyInteractionTimer -= Time.deltaTime;
		}
		if (this.destroyDisableLaunchesTimer > 0f)
		{
			this.destroyDisableLaunchesTimer -= Time.deltaTime;
			if (this.destroyDisableLaunchesTimer <= 0f)
			{
				this.destroyDisableLaunches = 0;
			}
		}
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x0007A630 File Offset: 0x00078830
	private void FixedUpdate()
	{
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.inCart && !this.isEnemy && this.physGrabObject.playerGrabbing.Count == 0 && this.currentCart && !this.rb.isKinematic && !base.GetComponent<PlayerTumble>())
		{
			PhysGrabCart component = this.currentCart.GetComponent<PhysGrabCart>();
			if (component.actualVelocity.magnitude > 1f)
			{
				Vector3 velocity = this.rb.velocity;
				this.rb.velocity = Vector3.Lerp(this.rb.velocity, component.actualVelocity, 30f * Time.fixedDeltaTime);
				if (this.rb.velocity.y > velocity.y)
				{
					this.rb.velocity = new Vector3(this.rb.velocity.x, velocity.y, this.rb.velocity.z);
				}
			}
		}
		this.impactHappened = false;
		this.breakForce = 0f;
		this.impactForce = 0f;
		if (this.impactDisabledTimer <= 0f)
		{
			Vector3 vector = this.rb.velocity / Time.fixedDeltaTime;
			Vector3 vector2 = this.rb.angularVelocity / Time.fixedDeltaTime;
			float magnitude = this.previousVelocity.magnitude;
			float num = Mathf.Abs(magnitude - vector.magnitude);
			float magnitude2 = this.previousAngularVelocity.magnitude;
			float num2 = Mathf.Abs(magnitude2 - vector2.magnitude);
			Vector3 normalized = vector.normalized;
			Vector3 normalized2 = this.previousVelocity.normalized;
			float num3 = Vector3.Angle(normalized, normalized2);
			Vector3 normalized3 = vector2.normalized;
			Vector3 normalized4 = this.previousAngularVelocity.normalized;
			float num4 = Vector3.Angle(normalized3, normalized4);
			num *= 1f;
			num2 *= 0.4f * this.rb.mass;
			num3 *= 0.2f;
			num4 *= 0.02f * this.rb.mass;
			if ((num > 1f && magnitude > 1f) || (num2 > 1f && magnitude2 > 1f) || (num3 > 1f && magnitude > 1f) || (num4 > 1f && magnitude2 > 1f))
			{
				this.impactHappened = true;
				float num5 = num * 2f;
				float num6 = Mathf.Max(this.rb.mass, 1f);
				this.breakForce += num5 * num6;
			}
			this.breakForce *= 8f;
			this.impactForce = this.breakForce / 8f * this.impactFragilityMultiplier;
			this.breakForce = this.breakForce * (this.fragility / 100f) * this.fragilityMultiplier;
			if (this.impactHappened)
			{
				if (this.inCart)
				{
					this.breakForce = 0f;
				}
				if (this.inCart || this.isCart)
				{
					this.impactForce *= 0.3f;
				}
			}
		}
		else
		{
			this.impactDisabledTimer -= Time.fixedDeltaTime;
		}
		this.previousPreviousVelocityRaw = this.previousVelocityRaw;
		this.previousVelocityRaw = this.rb.velocity;
		this.previousVelocity = this.rb.velocity / Time.fixedDeltaTime;
		this.previousAngularVelocity = this.rb.angularVelocity / Time.fixedDeltaTime;
		if (Vector3.Distance(this.prevPos, base.transform.position) > 0.01f || Quaternion.Angle(this.prevRot, base.transform.rotation) > 0.1f)
		{
			this.isMoving = true;
		}
		this.prevPos = base.transform.position;
		this.prevRot = base.transform.rotation;
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x0007AA37 File Offset: 0x00078C37
	public void ImpactDisable(float time)
	{
		this.impactDisabledTimer = time;
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x0007AA40 File Offset: 0x00078C40
	private void EnemyInvestigate(float radius)
	{
		if (this.physGrabObject.enemyInteractTimer > 0f || this.inCart || this.isCart)
		{
			return;
		}
		EnemyDirector.instance.SetInvestigate(base.transform.position, radius, false);
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x0007AA7C File Offset: 0x00078C7C
	public void DestroyObject(bool effects = true)
	{
		if (this.destroyDisable)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		EnemyRigidbody component = base.GetComponent<EnemyRigidbody>();
		if (component)
		{
			component.enemy.EnemyParent.Despawn();
			return;
		}
		if (!this.physGrabObject.dead)
		{
			this.physGrabObject.dead = true;
			this.EnemyInvestigate(15f);
			if (!SemiFunc.IsMultiplayer())
			{
				this.DestroyObjectRPC(effects, default(PhotonMessageInfo));
				return;
			}
			this.photonView.RPC("DestroyObjectRPC", RpcTarget.All, new object[]
			{
				effects
			});
		}
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x0007AB18 File Offset: 0x00078D18
	[PunRPC]
	public void DestroyObjectRPC(bool effects, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.physGrabObject.dead = true;
		if (effects)
		{
			GameDirector.instance.CameraImpact.ShakeDistance(10f, 1f, 6f, base.transform.position, 0.1f);
		}
		if (this.particles)
		{
			this.particles.transform.parent = null;
			this.particles.DestroyParticles();
		}
		if (this.audioActive && effects)
		{
			AudioSource audioSource = this.impactAudio.destroy.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			if (audioSource)
			{
				audioSource.pitch *= this.impactAudioPitch;
			}
		}
		this.onDestroy.Invoke();
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x0007ABF4 File Offset: 0x00078DF4
	public void BreakHeavy(Vector3 contactPoint, float minimumValue = 0f)
	{
		float num = 0.1f * (1f + 9f * (100f - this.durability) / 100f);
		bool flag = false;
		if (this.isValuable || this.breakLogic)
		{
			float num2 = 0f;
			if (this.isValuable)
			{
				num2 = Mathf.Round(this.valuableObject.dollarValueOriginal * num);
				num2 += Mathf.Round(Random.Range(-num2 * 0.1f, num2 * 0.1f));
				if (minimumValue != 0f)
				{
					num2 = Mathf.Max(num2, minimumValue);
				}
				num2 = Mathf.Clamp(num2, 1f, this.valuableObject.dollarValueCurrent);
			}
			this.Break(num2, contactPoint, this.breakLevelHeavy);
			flag = true;
		}
		if (this.isNotValuable && this.notValuableObject.hasHealth)
		{
			this.notValuableObject.Impact(PhysGrabObjectImpactDetector.ImpactState.Heavy);
			flag = true;
		}
		if (flag)
		{
			this.EnemyInvestigate(10f);
		}
		this.breakLevel3Cooldown = 0.6f;
		this.breakLevel2Cooldown = 0.4f;
		this.breakLevel1Cooldown = 0.3f;
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x0007AD00 File Offset: 0x00078F00
	public void BreakMedium(Vector3 contactPoint)
	{
		float num = 0.05f * (1f + 9f * (100f - this.durability) / 100f);
		bool flag = false;
		if (this.isValuable || this.breakLogic)
		{
			float num2 = 0f;
			if (this.isValuable)
			{
				num2 = Mathf.Round(this.valuableObject.dollarValueOriginal * num);
				num2 += Mathf.Round(Random.Range(-num2 * 0.1f, num2 * 0.1f));
				num2 = Mathf.Clamp(num2, 0f, this.valuableObject.dollarValueCurrent);
			}
			this.Break(num2, contactPoint, this.breakLevelMedium);
			flag = true;
		}
		if (this.isNotValuable && this.notValuableObject.hasHealth)
		{
			this.notValuableObject.Impact(PhysGrabObjectImpactDetector.ImpactState.Medium);
			flag = true;
		}
		if (flag)
		{
			this.EnemyInvestigate(5f);
		}
		this.breakLevel2Cooldown = 0.4f;
		this.breakLevel1Cooldown = 0.3f;
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x0007ADF0 File Offset: 0x00078FF0
	public void BreakLight(Vector3 contactPoint)
	{
		float num = 0.01f * (1f + 9f * (100f - this.durability) / 100f);
		bool flag = false;
		if (this.isValuable || this.breakLogic)
		{
			float num2 = 0f;
			if (this.isValuable)
			{
				num2 = Mathf.Round(this.valuableObject.dollarValueOriginal * num);
				num2 += Mathf.Round(Random.Range(-num2 * 0.1f, num2 * 0.1f));
				num2 = Mathf.Clamp(num2, 0f, this.valuableObject.dollarValueCurrent);
			}
			this.Break(num2, contactPoint, this.breakLevelLight);
			flag = true;
		}
		if (this.isNotValuable && this.notValuableObject.hasHealth)
		{
			this.notValuableObject.Impact(PhysGrabObjectImpactDetector.ImpactState.Light);
			flag = true;
		}
		if (flag)
		{
			this.EnemyInvestigate(3f);
		}
		this.breakLevel1Cooldown = 0.3f;
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x0007AED4 File Offset: 0x000790D4
	internal void Break(float valueLost, Vector3 _contactPoint, int breakLevel)
	{
		bool flag = false;
		if (this.isValuable && !this.isIndestructible && !this.destroyDisable)
		{
			flag = true;
		}
		if (GameManager.instance.gameMode == 0)
		{
			this.BreakRPC(valueLost, _contactPoint, breakLevel, flag, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("BreakRPC", RpcTarget.All, new object[]
		{
			valueLost,
			_contactPoint,
			breakLevel,
			flag
		});
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x0007AF58 File Offset: 0x00079158
	private void HealLogic(float healAmount, Vector3 healingPoint)
	{
		this.valuableObject.dollarValueCurrent += Mathf.Floor(healAmount);
		this.valuableObject.dollarValueCurrent = Mathf.Clamp(this.valuableObject.dollarValueCurrent, 0f, this.valuableObject.dollarValueOriginal);
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x0007AFA8 File Offset: 0x000791A8
	public float Heal(float healPercent, Vector3 healingPoint)
	{
		float result = 0f;
		if (this.isValuable)
		{
			if (GameManager.Multiplayer())
			{
				if (PhotonNetwork.IsMasterClient)
				{
					float num = this.valuableObject.dollarValueOriginal * healPercent;
					num = Mathf.Clamp(num, 0f, this.valuableObject.dollarValueOriginal - this.valuableObject.dollarValueCurrent);
					if (num > 0f)
					{
						this.photonView.RPC("HealRPC", RpcTarget.All, new object[]
						{
							this.valuableObject.dollarValueOriginal * healPercent
						});
					}
					result = num;
				}
			}
			else
			{
				float num2 = this.valuableObject.dollarValueOriginal * healPercent;
				num2 = Mathf.Clamp(num2, 0f, this.valuableObject.dollarValueOriginal - this.valuableObject.dollarValueCurrent);
				if (num2 > 0f)
				{
					this.HealLogic(num2, healingPoint);
				}
				result = num2;
			}
		}
		return result;
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x0007B084 File Offset: 0x00079284
	public void PlayerHitDisableSet()
	{
		if (!this.playerHitDisable)
		{
			this.physGrabObject.PhysRidingDisabledSet(true);
			this.playerHitDisable = true;
		}
		this.playerHitDisableTimer = 0.5f;
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x0007B0AC File Offset: 0x000792AC
	[PunRPC]
	private void HealRPC(float healAmount, Vector3 healingPoint)
	{
		this.HealLogic(healAmount, healingPoint);
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x0007B0B8 File Offset: 0x000792B8
	private void ResetObject()
	{
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
		}
		this.rb.velocity = Vector3.zero;
		this.rb.angularVelocity = Vector3.zero;
		this.valuableObject.dollarValueCurrent = this.valuableObject.dollarValueOriginal;
		this.rb.position = this.originalPosition;
		this.rb.rotation = this.originalRotation;
		base.transform.position = this.originalPosition;
		AssetManager.instance.soundUnequip.Play(this.originalPosition, 1f, 1f, 1f, 1f);
		this.BreakEffect(this.breakLevelLight, this.originalPosition);
		Vector3 position = this.physGrabObject.transform.TransformPoint(this.physGrabObject.midPointOffset);
		Object.Instantiate<GameObject>(AssetManager.instance.prefabTeleportEffect, position, Quaternion.identity).transform.localScale = Vector3.one * 2f;
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x0007B240 File Offset: 0x00079440
	[PunRPC]
	private void BreakRPC(float valueLost, Vector3 _contactPoint, int breakLevel, bool _loseValue, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (_loseValue)
		{
			if (this.valuableObject)
			{
				float dollarValueCurrent = this.valuableObject.dollarValueCurrent;
				this.valuableObject.dollarValueCurrent -= valueLost;
				bool flag = false;
				if (this.valuableObject.dollarValueCurrent < this.valuableObject.dollarValueOriginal * 0.15f)
				{
					if (!SemiFunc.RunIsTutorial())
					{
						this.DestroyObject(true);
					}
					else
					{
						if (this.particles)
						{
							this.particles.DestroyParticles();
						}
						this.ResetObject();
						this.ImpactHeavy(1000f, _contactPoint);
					}
					flag = true;
				}
				if (flag)
				{
					valueLost = dollarValueCurrent;
				}
			}
			WorldSpaceUIParent.instance.ValueLostCreate(_contactPoint, (int)valueLost);
		}
		if (_loseValue || !this.valuableObject)
		{
			this.onAllBreaks.Invoke();
		}
		if (breakLevel == this.breakLevelHeavy)
		{
			if (_loseValue || !this.valuableObject)
			{
				this.onBreakHeavy.Invoke();
			}
			if (this.physGrabObject)
			{
				this.physGrabObject.heavyBreakImpulse = false;
			}
		}
		if (breakLevel == this.breakLevelMedium)
		{
			if (_loseValue || !this.valuableObject)
			{
				this.onBreakMedium.Invoke();
			}
			if (this.physGrabObject)
			{
				this.physGrabObject.mediumBreakImpulse = false;
			}
		}
		if (breakLevel == this.breakLevelLight)
		{
			if (_loseValue || !this.valuableObject)
			{
				this.onBreakLight.Invoke();
			}
			if (this.physGrabObject)
			{
				this.physGrabObject.lightBreakImpulse = false;
			}
		}
		this.BreakEffect(breakLevel, _contactPoint);
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x0007B3DC File Offset: 0x000795DC
	public void BreakEffect(int breakLevel, Vector3 contactPoint)
	{
		if (!this.particleDisable && this.particles)
		{
			this.particles.ImpactSmoke(5, contactPoint, this.colliderVolume);
		}
		if (breakLevel == this.breakLevelHeavy)
		{
			if (this.audioActive && this.impactAudio)
			{
				this.impactAudio.breakHeavy.Play(contactPoint, 1f, 1f, 1f, 1f);
			}
			if (this.physGrabObject)
			{
				SemiFunc.PlayerEyesOverrideSoft(this.physGrabObject.centerPoint, 1f, base.gameObject, 10f);
			}
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 1f, 6f, contactPoint, 0.1f);
		}
		if (breakLevel == this.breakLevelMedium)
		{
			if (this.audioActive && this.impactAudio)
			{
				AudioSource audioSource = this.impactAudio.breakMedium.Play(contactPoint, 1f, 1f, 1f, 1f);
				if (audioSource)
				{
					audioSource.pitch *= this.impactAudioPitch;
				}
			}
			if (this.physGrabObject)
			{
				SemiFunc.PlayerEyesOverrideSoft(this.physGrabObject.centerPoint, 1f, base.gameObject, 5f);
			}
			GameDirector.instance.CameraImpact.ShakeDistance(3f, 1f, 6f, contactPoint, 0.1f);
		}
		if (breakLevel == this.breakLevelLight)
		{
			if (this.audioActive && this.impactAudio)
			{
				AudioSource audioSource2 = this.impactAudio.breakLight.Play(contactPoint, 1f, 1f, 1f, 1f);
				if (audioSource2)
				{
					audioSource2.pitch *= this.impactAudioPitch;
				}
			}
			if (this.physGrabObject)
			{
				SemiFunc.PlayerEyesOverrideSoft(this.physGrabObject.centerPoint, 1f, base.gameObject, 3f);
			}
			GameDirector.instance.CameraImpact.ShakeDistance(1f, 1f, 6f, contactPoint, 0.1f);
		}
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x0007B610 File Offset: 0x00079810
	public void ImpactHeavy(float force, Vector3 contactPoint)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ImpactHeavyRPC(force, contactPoint, default(PhotonMessageInfo));
		}
		else
		{
			this.photonView.RPC("ImpactHeavyRPC", RpcTarget.All, new object[]
			{
				force,
				contactPoint
			});
		}
		this.physGrabObject.impactHappenedTimer = 0.1f;
		this.physGrabObject.impactHeavyTimer = 0.1f;
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x0007B688 File Offset: 0x00079888
	[PunRPC]
	private void ImpactHeavyRPC(float force, Vector3 contactPoint, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (!this.physGrabObject)
		{
			return;
		}
		SemiFunc.PlayerEyesOverrideSoft(this.physGrabObject.centerPoint, 1f, base.gameObject, 8f);
		if (this.audioActive && !this.isHinge && this.impactAudio)
		{
			float volumeMultiplier = this.ImpactSoundGetVolume(force, this.impactAudio.impactHeavy.Volume);
			AudioSource audioSource = this.impactAudio.impactHeavy.Play(contactPoint, volumeMultiplier, 1f, 1f, 1f);
			if (audioSource)
			{
				audioSource.pitch *= this.impactAudioPitch;
			}
		}
		if (!this.particleDisable && !this.inCart && this.particles)
		{
			this.particles.ImpactSmoke(5, contactPoint, this.colliderVolume);
		}
		this.onAllImpacts.Invoke();
		this.onImpactHeavy.Invoke();
		this.EnemyInvestigate(1f);
		this.physGrabObject.impactHappenedTimer = 0.1f;
		this.physGrabObject.impactHeavyTimer = 0.1f;
	}

	// Token: 0x06000DFD RID: 3581 RVA: 0x0007B7B0 File Offset: 0x000799B0
	public void ImpactMedium(float force, Vector3 contactPoint)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ImpactMediumRPC(force, contactPoint, default(PhotonMessageInfo));
		}
		else
		{
			this.photonView.RPC("ImpactMediumRPC", RpcTarget.All, new object[]
			{
				force,
				contactPoint
			});
		}
		this.physGrabObject.impactHappenedTimer = 0.1f;
		this.physGrabObject.impactMediumTimer = 0.1f;
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x0007B828 File Offset: 0x00079A28
	[PunRPC]
	private void ImpactMediumRPC(float force, Vector3 contactPoint, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (!this.physGrabObject)
		{
			return;
		}
		SemiFunc.PlayerEyesOverrideSoft(this.physGrabObject.centerPoint, 1f, base.gameObject, 5f);
		if (this.audioActive && !this.isHinge && this.impactAudio)
		{
			float volumeMultiplier = this.ImpactSoundGetVolume(force, this.impactAudio.impactMedium.Volume);
			AudioSource audioSource = this.impactAudio.impactMedium.Play(contactPoint, volumeMultiplier, 1f, 1f, 1f);
			if (audioSource)
			{
				audioSource.pitch *= this.impactAudioPitch;
			}
		}
		this.onImpactMedium.Invoke();
		this.onAllImpacts.Invoke();
		if (!this.rb.isKinematic)
		{
			this.rb.angularVelocity *= 0.55f;
		}
		this.EnemyInvestigate(0.5f);
		this.physGrabObject.impactHappenedTimer = 0.1f;
		this.physGrabObject.impactMediumTimer = 0.1f;
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x0007B948 File Offset: 0x00079B48
	private float ImpactSoundGetVolume(float force, float volume)
	{
		float num = Mathf.Clamp01(force * 0.01f);
		if (this.inCart)
		{
			num *= this.inCartVolumeMultiplier;
		}
		return Mathf.Clamp(num, 0.1f, 1f);
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x0007B984 File Offset: 0x00079B84
	public void ImpactLight(float force, Vector3 contactPoint)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ImpactLightRPC(force, contactPoint, default(PhotonMessageInfo));
		}
		else
		{
			this.photonView.RPC("ImpactLightRPC", RpcTarget.All, new object[]
			{
				force,
				contactPoint
			});
		}
		this.EnemyInvestigate(0.2f);
		this.physGrabObject.impactHappenedTimer = 0.1f;
		this.physGrabObject.impactLightTimer = 0.1f;
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x0007BA04 File Offset: 0x00079C04
	public void changeInCart()
	{
		this.timerInCart = 0.1f;
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x0007BA14 File Offset: 0x00079C14
	[PunRPC]
	private void ImpactLightRPC(float force, Vector3 contactPoint, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (!this.physGrabObject)
		{
			return;
		}
		SemiFunc.PlayerEyesOverrideSoft(this.physGrabObject.centerPoint, 1f, base.gameObject, 3f);
		if (this.audioActive && !this.isHinge && this.impactAudio)
		{
			float num = this.ImpactSoundGetVolume(force, this.impactAudio.impactLight.Volume);
			if (this.inCart)
			{
				num *= this.inCartVolumeMultiplier;
			}
			AudioSource audioSource = this.impactAudio.impactLight.Play(contactPoint, num, 1f, 1f, 1f);
			if (audioSource)
			{
				audioSource.pitch *= this.impactAudioPitch;
			}
		}
		if (!this.rb.isKinematic)
		{
			this.rb.angularVelocity *= 0.6f;
		}
		this.onAllImpacts.Invoke();
		this.onImpactLight.Invoke();
		this.physGrabObject.impactHappenedTimer = 0.1f;
		this.physGrabObject.impactLightTimer = 0.1f;
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x0007BB3C File Offset: 0x00079D3C
	private void OnTriggerStay(Collider other)
	{
		if ((GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient) && other.CompareTag("Cart"))
		{
			bool flag = true;
			if (this.centerPointNeedsToBeInsideCart)
			{
				Vector3 point = base.transform.TransformPoint(this.centerPoint);
				Collider component = other.GetComponent<Collider>();
				if (component && !component.bounds.Contains(point))
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.currentCartPrev = this.currentCart;
				this.currentCart = other.GetComponentInParent<PhysGrabCart>();
				if (this.currentCart)
				{
					this.currentCart.physGrabInCart.Add(this.physGrabObject);
				}
				this.changeInCart();
			}
		}
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x0007BBF4 File Offset: 0x00079DF4
	private void OnCollisionStay(Collision collision)
	{
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.collisionsActive)
		{
			return;
		}
		if (!this.isMoving)
		{
			return;
		}
		this.isCollidingTimer = 0.1f;
		if (!this.isHinge && !this.slidingDisable && !this.isCart && !this.inCart && !this.isCart && this.isValuable && this.valuableObject.volumeType >= ValuableVolume.Type.Medium && collision.gameObject.GetComponent<MaterialSurface>())
		{
			if (this.rb.velocity.magnitude > this.slidingSpeedThreshold && (Mathf.Abs(this.rb.velocity.x) > Mathf.Abs(this.rb.velocity.y) || Mathf.Abs(this.rb.velocity.z) > Mathf.Abs(this.rb.velocity.y)))
			{
				this.isSliding = true;
				this.slidingTimer = 0.1f;
			}
			PhysGrabObject component = collision.gameObject.GetComponent<PhysGrabObject>();
			if (component && component.rb.velocity.magnitude > this.rb.velocity.magnitude * 0.8f)
			{
				this.isSliding = false;
			}
		}
		Vector3 a = Vector3.zero;
		foreach (ContactPoint contactPoint in collision.contacts)
		{
			a += contactPoint.point;
		}
		if (collision.contacts.Length != 0)
		{
			this.contactPoint = a / (float)collision.contacts.Length;
		}
		else
		{
			this.contactPoint = Vector3.zero;
		}
		PhysGrabObjectImpactDetector component2 = collision.gameObject.GetComponent<PhysGrabObjectImpactDetector>();
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		bool flag3 = this.isCart && this.contactPoint.y < base.transform.position.y;
		if (this.impactHappened && (!this.isCart || !component2 || !component2.inCart) && !flag3)
		{
			if (this.impulseTimerDeactivateImpacts <= 0f)
			{
				if (this.impactForce > 150f && this.impactHeavyCooldown <= 0f)
				{
					flag2 = true;
					this.ImpactHeavy(this.impactForce, this.contactPoint);
					this.impactHeavyCooldown = 0.5f;
					this.impactMediumCooldown = 0.5f;
					this.impactLightCooldown = 0.5f;
				}
				if (this.impactForce > 80f && this.impactMediumCooldown <= 0f)
				{
					flag2 = true;
					this.ImpactMedium(this.impactForce, this.contactPoint);
					this.impactMediumCooldown = 0.5f;
					this.impactLightCooldown = 0.5f;
				}
				if (this.impactForce > 20f && this.impactLightCooldown <= 0f)
				{
					flag2 = true;
					this.ImpactLight(this.impactForce, this.contactPoint);
					this.impactLightCooldown = 0.5f;
				}
			}
			if (this.indestructibleSpawnTimer <= 0f)
			{
				float num2 = Mathf.Max(this.rb.mass, 1f);
				if (this.breakForce > this.impactLevel3 * num2 && this.breakLevel3Cooldown <= 0f && !this.inCart)
				{
					flag = true;
					num = 3;
				}
				if (this.breakForce > this.impactLevel2 * num2 && this.breakLevel2Cooldown <= 0f && !flag && !this.inCart)
				{
					flag = true;
					num = 2;
				}
				if (this.breakForce > this.impactLevel1 * num2 && this.breakLevel1Cooldown <= 0f && !flag && !this.inCart)
				{
					flag = true;
					num = 1;
				}
			}
		}
		bool flag4 = false;
		bool flag5 = false;
		if (flag && (!this.isEnemy || this.enemyRigidbody.enemy.IsStunned()))
		{
			flag4 = true;
		}
		if (flag2 && (!this.isEnemy || this.enemyRigidbody.enemy.IsStunned()))
		{
			flag4 = true;
		}
		if (flag && this.isBrokenHinge)
		{
			flag5 = true;
		}
		if (!this.canHurtLogic)
		{
			flag4 = false;
		}
		bool flag6 = false;
		if (this.playerHitDisable && (collision.transform.CompareTag("Player") || collision.transform.GetComponent<PlayerTumble>()))
		{
			this.playerHitDisableTimer = 0.5f;
		}
		if (!this.playerHitDisable && flag4 && (flag || (flag2 && this.isCart)))
		{
			bool flag7 = false;
			PlayerTumble playerTumble = null;
			if (collision.transform.CompareTag("Player"))
			{
				flag7 = true;
			}
			else
			{
				playerTumble = collision.transform.GetComponent<PlayerTumble>();
				if (playerTumble)
				{
					flag7 = true;
				}
			}
			if (flag7 && this.isCart)
			{
				if (this.physGrabObject.playerGrabbing.Count <= 0)
				{
					flag7 = false;
				}
				else if (this.cart.inCart.GetComponent<BoxCollider>().bounds.Contains(collision.transform.position))
				{
					flag7 = false;
				}
			}
			if (flag7)
			{
				PlayerController componentInParent = collision.transform.GetComponentInParent<PlayerController>();
				PlayerAvatar playerAvatar;
				if (playerTumble)
				{
					playerAvatar = playerTumble.playerAvatar;
				}
				else if (componentInParent)
				{
					playerAvatar = componentInParent.playerAvatarScript;
				}
				else
				{
					playerAvatar = collision.transform.GetComponentInParent<PlayerAvatar>();
					if (!playerAvatar)
					{
						playerAvatar = collision.transform.GetComponent<PlayerAvatar>();
					}
					if (!playerAvatar)
					{
						playerAvatar = collision.transform.GetComponentInChildren<PlayerAvatar>();
					}
					if (!playerAvatar)
					{
						PlayerPhysPusher component3 = collision.transform.GetComponent<PlayerPhysPusher>();
						if (component3)
						{
							playerAvatar = component3.Player;
						}
					}
				}
				bool flag8 = false;
				using (List<PhysGrabber>.Enumerator enumerator = this.physGrabObject.playerGrabbing.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.playerAvatar == playerAvatar)
						{
							flag8 = true;
							break;
						}
					}
				}
				if (playerAvatar && !flag8)
				{
					Vector3 vector = playerAvatar.PlayerVisionTarget.VisionTransform.transform.position - this.contactPoint;
					float magnitude = this.previousPreviousVelocityRaw.magnitude;
					Vector3 direction = Vector3.Lerp(this.previousPreviousVelocityRaw.normalized, vector.normalized, 0f);
					if (magnitude >= 3f)
					{
						PlayerAvatar playerAvatar2 = null;
						foreach (RaycastHit raycastHit in this.rb.SweepTestAll(direction, 1f, QueryTriggerInteraction.Collide))
						{
							playerAvatar2 = this.ImpactGetPlayer(raycastHit.collider, componentInParent, playerTumble);
							if (playerAvatar2 == playerAvatar)
							{
								break;
							}
						}
						if (!playerAvatar2)
						{
							foreach (Collider hit in Physics.OverlapSphere(this.contactPoint, 0.2f, LayerMask.GetMask(new string[]
							{
								"Player"
							})))
							{
								playerAvatar2 = this.ImpactGetPlayer(hit, componentInParent, playerTumble);
								if (playerAvatar2 == playerAvatar)
								{
									break;
								}
							}
						}
						if (playerAvatar2 == playerAvatar)
						{
							bool flag9 = false;
							if (!playerAvatar.isTumbling)
							{
								using (List<PhysGrabObject>.Enumerator enumerator2 = playerAvatar.physObjectFinder.physGrabObjects.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										if (enumerator2.Current == this.physGrabObject)
										{
											flag9 = true;
											break;
										}
									}
								}
							}
							if (!flag9)
							{
								bool flag10 = false;
								float time = 0.1f;
								if (!this.playerHurtDisable && !this.isIndestructible && !this.destroyDisable && this.isValuable)
								{
									time = 0.15f;
									int damage = Mathf.RoundToInt((float)(5 * num) * (this.rb.mass * 0.5f) * this.playerHurtMultiplier);
									playerAvatar.playerHealth.HurtOther(damage, this.contactPoint, true, -1);
									flag10 = true;
								}
								bool flag11 = false;
								if (this.isHinge)
								{
									if (magnitude >= 3f)
									{
										flag11 = true;
									}
								}
								else if (this.isCart)
								{
									if (magnitude >= 3f)
									{
										flag11 = true;
									}
								}
								else if (magnitude >= 6f)
								{
									flag11 = true;
								}
								if (flag10 || flag11)
								{
									if (!playerTumble)
									{
										playerTumble = playerAvatar.tumble;
									}
									playerTumble.TumbleRequest(true, false);
									playerTumble.TumbleOverrideTime(2f);
									Vector3 force = vector.normalized * 4f * (float)num;
									Vector3 torque = Vector3.Cross((playerAvatar.localCameraPosition - this.contactPoint).normalized, playerAvatar.transform.forward) * 5f * force.magnitude;
									playerTumble.physGrabObject.FreezeForces(time, force, torque);
									playerAvatar.playerHealth.HurtFreezeOverride(time);
									this.physGrabObject.FreezeForces(time, Vector3.zero, Vector3.zero);
									flag6 = true;
								}
							}
						}
					}
				}
			}
		}
		if ((flag4 || flag5) && !this.playerHurtDisable && this.enemyInteractionTimer <= 0f && !this.isIndestructible && !this.destroyDisable && (this.isValuable || this.isEnemy || flag5) && collision.transform.CompareTag("Enemy"))
		{
			EnemyRigidbody component4 = collision.transform.GetComponent<EnemyRigidbody>();
			if (component4 && component4.enemy.HasHealth && component4.enemy.Health.objectHurt && component4.enemy.Health.objectHurtDisableTimer <= 0f)
			{
				Vector3 vector2 = component4.physGrabObject.centerPoint - this.contactPoint;
				float magnitude2 = this.previousPreviousVelocityRaw.magnitude;
				Vector3 direction2 = Vector3.Lerp(this.previousPreviousVelocityRaw.normalized, vector2.normalized, 0.5f);
				if (magnitude2 > 2f)
				{
					EnemyRigidbody enemyRigidbody = null;
					foreach (RaycastHit raycastHit2 in this.rb.SweepTestAll(direction2, 1f, QueryTriggerInteraction.Collide))
					{
						enemyRigidbody = raycastHit2.transform.GetComponent<EnemyRigidbody>();
					}
					if (!enemyRigidbody)
					{
						Collider[] array2 = Physics.OverlapSphere(this.contactPoint, 0.2f, SemiFunc.LayerMaskGetPhysGrabObject());
						for (int i = 0; i < array2.Length; i++)
						{
							enemyRigidbody = array2[i].GetComponentInParent<EnemyRigidbody>();
						}
					}
					if (enemyRigidbody == component4)
					{
						flag6 = true;
						int num3 = Mathf.RoundToInt((float)(10 * num) * (this.rb.mass * 0.5f));
						num3 = Mathf.RoundToInt((float)num3 * component4.enemy.Health.objectHurtMultiplier);
						component4.enemy.Health.Hurt(num3, -vector2.normalized);
						if (this.isValuable)
						{
							float num4 = 0f;
							switch (this.valuableObject.volumeType)
							{
							case ValuableVolume.Type.Tiny:
								num4 = 0.2f;
								break;
							case ValuableVolume.Type.Small:
								num4 = 0.2f;
								break;
							case ValuableVolume.Type.Medium:
								num4 = 0.14285715f;
								break;
							case ValuableVolume.Type.Big:
								num4 = 0.1f;
								break;
							case ValuableVolume.Type.Wide:
								num4 = 0.1f;
								break;
							case ValuableVolume.Type.Tall:
								num4 = 0.1f;
								break;
							case ValuableVolume.Type.VeryTall:
								num4 = 0.1f;
								break;
							}
							num4 += Random.Range(-0.05f, 0.05f);
							this.BreakHeavy(this.contactPoint, (float)Mathf.CeilToInt(this.valuableObject.dollarValueOriginal * num4));
						}
						flag2 = false;
						flag = false;
						if (component4.enemy.Health.onObjectHurt != null)
						{
							if (this.physGrabObject.grabbedTimer > 0f)
							{
								component4.enemy.Health.onObjectHurtPlayer = this.physGrabObject.lastPlayerGrabbing;
							}
							else
							{
								component4.enemy.Health.onObjectHurtPlayer = null;
							}
							component4.enemy.Health.onObjectHurt.Invoke();
						}
						Vector3 force2 = vector2.normalized * (2f * (float)num);
						component4.rb.AddForce(force2, ForceMode.Impulse);
						Vector3 normalized = vector2.normalized;
						Vector3 rhs = -component4.rb.transform.up;
						Vector3 torque2 = Vector3.Cross(normalized, rhs) * (2f * (float)num);
						component4.rb.AddTorque(torque2, ForceMode.Impulse);
						EnemyType type = component4.enemy.Type;
						if (this.isValuable)
						{
							if (component4.enemy.HasStateStunned && component4.enemy.Health.objectHurtStun)
							{
								float mass = this.valuableObject.physAttributePreset.mass;
								bool flag12 = false;
								switch (type)
								{
								case EnemyType.VeryLight:
									if (mass >= 0.5f)
									{
										flag12 = true;
									}
									break;
								case EnemyType.Light:
									if (mass >= 1f)
									{
										flag12 = true;
									}
									break;
								case EnemyType.Medium:
									if (mass >= 2f)
									{
										flag12 = true;
									}
									break;
								case EnemyType.Heavy:
									if (mass >= 3.5f)
									{
										flag12 = true;
									}
									break;
								case EnemyType.VeryHeavy:
									if (mass >= 5f)
									{
										flag12 = true;
									}
									break;
								}
								if (flag12)
								{
									component4.enemy.StateStunned.Set(2f);
								}
							}
						}
						else if (this.isBrokenHinge)
						{
							if (type <= EnemyType.Medium && component4.enemy.HasStateStunned && component4.enemy.Health.objectHurtStun)
							{
								component4.enemy.StateStunned.Set(2f);
							}
							this.DestroyObject(true);
						}
					}
				}
			}
		}
		if (flag6)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				this.ImpactEffectRPC(this.contactPoint, default(PhotonMessageInfo));
			}
			else
			{
				this.photonView.RPC("ImpactEffectRPC", RpcTarget.All, new object[]
				{
					this.contactPoint
				});
			}
		}
		if (flag && this.physGrabObject.overrideDisableBreakEffectsTimer <= 0f)
		{
			if ((this.destroyDisable || this.isIndestructible || !this.isValuable) && !this.indestructibleBreakEffects)
			{
				if (!flag2)
				{
					if (num == 1)
					{
						this.ImpactLight(this.impactForce, this.contactPoint);
					}
					if (num == 2)
					{
						this.ImpactMedium(this.impactForce, this.contactPoint);
					}
					if (num == 3)
					{
						this.ImpactHeavy(this.impactForce, this.contactPoint);
						return;
					}
				}
			}
			else
			{
				if (num == 1)
				{
					this.BreakLight(this.contactPoint);
				}
				if (num == 2)
				{
					this.BreakMedium(this.contactPoint);
				}
				if (num == 3)
				{
					this.BreakHeavy(this.contactPoint, 0f);
				}
			}
		}
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x0007CAC8 File Offset: 0x0007ACC8
	[PunRPC]
	private void ImpactEffectRPC(Vector3 _position, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		AssetManager.instance.PhysImpactEffect(_position);
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x0007CAE0 File Offset: 0x0007ACE0
	private PlayerAvatar ImpactGetPlayer(Collider _hit, PlayerController _playerController, PlayerTumble _playerTumble)
	{
		if (_playerTumble)
		{
			PlayerTumble playerTumble = _hit.transform.GetComponent<PlayerTumble>();
			if (!playerTumble)
			{
				playerTumble = _hit.transform.GetComponentInParent<PlayerTumble>();
			}
			if (playerTumble)
			{
				return playerTumble.playerAvatar;
			}
		}
		PlayerAvatar playerAvatar = _hit.transform.GetComponentInParent<PlayerAvatar>();
		if (!playerAvatar)
		{
			playerAvatar = _hit.transform.GetComponent<PlayerAvatar>();
		}
		if (!playerAvatar)
		{
			playerAvatar = _hit.transform.GetComponentInChildren<PlayerAvatar>();
		}
		if (!playerAvatar)
		{
			PlayerPhysPusher component = _hit.transform.GetComponent<PlayerPhysPusher>();
			if (component)
			{
				playerAvatar = component.Player;
			}
		}
		if (!playerAvatar && _playerController)
		{
			PlayerController playerController = _hit.transform.GetComponentInParent<PlayerController>();
			if (!playerController && _hit.transform.GetComponentInParent<PlayerCollisionController>())
			{
				playerController = PlayerController.instance;
			}
			if (playerController)
			{
				playerAvatar = playerController.playerAvatarScript;
			}
		}
		return playerAvatar;
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x0007CBC9 File Offset: 0x0007ADC9
	public void PlayerHurtMultiplier(float _multiplier, float _time)
	{
		this.playerHurtMultiplier = _multiplier;
		this.playerHurtMultiplierTimer = _time;
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x0007CBD9 File Offset: 0x0007ADD9
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0400168B RID: 5771
	public bool particleDisable;

	// Token: 0x0400168C RID: 5772
	[Range(0f, 4f)]
	public float particleMultiplier = 1f;

	// Token: 0x0400168D RID: 5773
	[Space]
	public bool playerHurtDisable;

	// Token: 0x0400168E RID: 5774
	public bool slidingDisable;

	// Token: 0x0400168F RID: 5775
	public bool destroyDisable;

	// Token: 0x04001690 RID: 5776
	internal int destroyDisableLaunches;

	// Token: 0x04001691 RID: 5777
	internal float destroyDisableLaunchesTimer;

	// Token: 0x04001692 RID: 5778
	internal bool destroyDisableTeleport = true;

	// Token: 0x04001693 RID: 5779
	public bool indestructibleBreakEffects = true;

	// Token: 0x04001694 RID: 5780
	public bool canHurtLogic = true;

	// Token: 0x04001695 RID: 5781
	[HideInInspector]
	public PhysObjectParticles particles;

	// Token: 0x04001696 RID: 5782
	private List<Transform> colliderTransforms = new List<Transform>();

	// Token: 0x04001697 RID: 5783
	private EnemyRigidbody enemyRigidbody;

	// Token: 0x04001698 RID: 5784
	[HideInInspector]
	public bool isEnemy;

	// Token: 0x04001699 RID: 5785
	internal float enemyInteractionTimer;

	// Token: 0x0400169A RID: 5786
	private Rigidbody rb;

	// Token: 0x0400169B RID: 5787
	private Materials.MaterialTrigger materialTrigger = new Materials.MaterialTrigger();

	// Token: 0x0400169C RID: 5788
	[HideInInspector]
	public float fragility = 50f;

	// Token: 0x0400169D RID: 5789
	[HideInInspector]
	public float durability = 100f;

	// Token: 0x0400169E RID: 5790
	private float impactLevel1 = 300f;

	// Token: 0x0400169F RID: 5791
	private float impactLevel2 = 400f;

	// Token: 0x040016A0 RID: 5792
	private float impactLevel3 = 500f;

	// Token: 0x040016A1 RID: 5793
	private float breakLevel1Cooldown;

	// Token: 0x040016A2 RID: 5794
	private float breakLevel2Cooldown;

	// Token: 0x040016A3 RID: 5795
	private float breakLevel3Cooldown;

	// Token: 0x040016A4 RID: 5796
	private float impactLightCooldown;

	// Token: 0x040016A5 RID: 5797
	private float impactMediumCooldown;

	// Token: 0x040016A6 RID: 5798
	private float impactHeavyCooldown;

	// Token: 0x040016A7 RID: 5799
	private Vector3 previousPosition;

	// Token: 0x040016A8 RID: 5800
	private Vector3 previousRotation;

	// Token: 0x040016A9 RID: 5801
	private Camera mainCamera;

	// Token: 0x040016AA RID: 5802
	private float impactCooldown;

	// Token: 0x040016AB RID: 5803
	internal bool isIndestructible;

	// Token: 0x040016AC RID: 5804
	internal float impulseTimerDeactivateImpacts = 5f;

	// Token: 0x040016AD RID: 5805
	internal float highestVelocity;

	// Token: 0x040016AE RID: 5806
	internal float impactForce;

	// Token: 0x040016AF RID: 5807
	internal float resetPrevPositionTimer;

	// Token: 0x040016B0 RID: 5808
	private PhysGrabObject physGrabObject;

	// Token: 0x040016B1 RID: 5809
	private PhotonView photonView;

	// Token: 0x040016B2 RID: 5810
	internal bool isHinge;

	// Token: 0x040016B3 RID: 5811
	internal bool isBrokenHinge;

	// Token: 0x040016B4 RID: 5812
	private ValuableObject valuableObject;

	// Token: 0x040016B5 RID: 5813
	private NotValuableObject notValuableObject;

	// Token: 0x040016B6 RID: 5814
	private bool isNotValuable;

	// Token: 0x040016B7 RID: 5815
	private bool breakLogic;

	// Token: 0x040016B8 RID: 5816
	[HideInInspector]
	public bool isValuable;

	// Token: 0x040016B9 RID: 5817
	private bool collisionsActive;

	// Token: 0x040016BA RID: 5818
	private float collisionsActiveTimer;

	// Token: 0x040016BB RID: 5819
	private float collisionActivatedBuffer;

	// Token: 0x040016BC RID: 5820
	[HideInInspector]
	public bool isSliding;

	// Token: 0x040016BD RID: 5821
	private float slidingTimer;

	// Token: 0x040016BE RID: 5822
	private float slidingGain;

	// Token: 0x040016BF RID: 5823
	private float slidingSpeedThreshold = 0.1f;

	// Token: 0x040016C0 RID: 5824
	private float slidingAudioSpeed;

	// Token: 0x040016C1 RID: 5825
	private Vector3 previousSlidingPosition;

	// Token: 0x040016C2 RID: 5826
	internal Vector3 previousVelocity;

	// Token: 0x040016C3 RID: 5827
	internal Vector3 previousAngularVelocity;

	// Token: 0x040016C4 RID: 5828
	internal Vector3 previousVelocityRaw;

	// Token: 0x040016C5 RID: 5829
	internal Vector3 previousPreviousVelocityRaw;

	// Token: 0x040016C6 RID: 5830
	private bool impactHappened;

	// Token: 0x040016C7 RID: 5831
	internal float impactDisabledTimer;

	// Token: 0x040016C8 RID: 5832
	private Vector3 contactPoint;

	// Token: 0x040016C9 RID: 5833
	private PhysAudio impactAudio;

	// Token: 0x040016CA RID: 5834
	private float impactAudioPitch = 1f;

	// Token: 0x040016CB RID: 5835
	private bool audioActive;

	// Token: 0x040016CC RID: 5836
	private float colliderVolume;

	// Token: 0x040016CD RID: 5837
	private float timerInCart;

	// Token: 0x040016CE RID: 5838
	internal int breakLevelHeavy;

	// Token: 0x040016CF RID: 5839
	internal int breakLevelMedium = 1;

	// Token: 0x040016D0 RID: 5840
	internal int breakLevelLight = 2;

	// Token: 0x040016D1 RID: 5841
	private Vector3 prevPos;

	// Token: 0x040016D2 RID: 5842
	private Quaternion prevRot;

	// Token: 0x040016D3 RID: 5843
	private bool isMoving;

	// Token: 0x040016D4 RID: 5844
	private float breakForce;

	// Token: 0x040016D5 RID: 5845
	private Vector3 originalPosition;

	// Token: 0x040016D6 RID: 5846
	private Quaternion originalRotation;

	// Token: 0x040016D7 RID: 5847
	public UnityEvent onAllImpacts;

	// Token: 0x040016D8 RID: 5848
	public UnityEvent onImpactLight;

	// Token: 0x040016D9 RID: 5849
	public UnityEvent onImpactMedium;

	// Token: 0x040016DA RID: 5850
	public UnityEvent onImpactHeavy;

	// Token: 0x040016DB RID: 5851
	[Space(15f)]
	public UnityEvent onAllBreaks;

	// Token: 0x040016DC RID: 5852
	public UnityEvent onBreakLight;

	// Token: 0x040016DD RID: 5853
	public UnityEvent onBreakMedium;

	// Token: 0x040016DE RID: 5854
	public UnityEvent onBreakHeavy;

	// Token: 0x040016DF RID: 5855
	[Space(15f)]
	public UnityEvent onDestroy;

	// Token: 0x040016E0 RID: 5856
	[HideInInspector]
	public bool inCart;

	// Token: 0x040016E1 RID: 5857
	private bool inCartPrevious;

	// Token: 0x040016E2 RID: 5858
	[HideInInspector]
	public bool isCart;

	// Token: 0x040016E3 RID: 5859
	private PhysGrabCart cart;

	// Token: 0x040016E4 RID: 5860
	private float inCartVolumeMultiplier;

	// Token: 0x040016E5 RID: 5861
	private float impactCheckTimer;

	// Token: 0x040016E6 RID: 5862
	internal PhysGrabCart currentCart;

	// Token: 0x040016E7 RID: 5863
	internal PhysGrabCart currentCartPrev;

	// Token: 0x040016E8 RID: 5864
	internal float indestructibleSpawnTimer = 5f;

	// Token: 0x040016E9 RID: 5865
	internal bool isColliding;

	// Token: 0x040016EA RID: 5866
	private float isCollidingTimer;

	// Token: 0x040016EB RID: 5867
	[HideInInspector]
	public float fragilityMultiplier = 1f;

	// Token: 0x040016EC RID: 5868
	[HideInInspector]
	public float impactFragilityMultiplier = 1f;

	// Token: 0x040016ED RID: 5869
	private float playerHurtMultiplier = 1f;

	// Token: 0x040016EE RID: 5870
	private float playerHurtMultiplierTimer;

	// Token: 0x040016EF RID: 5871
	[Space(15f)]
	public bool centerPointNeedsToBeInsideCart;

	// Token: 0x040016F0 RID: 5872
	internal Vector3 centerPoint = Vector3.zero;

	// Token: 0x040016F1 RID: 5873
	private bool playerHitDisable;

	// Token: 0x040016F2 RID: 5874
	private float playerHitDisableTimer;

	// Token: 0x020003AB RID: 939
	public enum ImpactState
	{
		// Token: 0x04002BF6 RID: 11254
		None,
		// Token: 0x04002BF7 RID: 11255
		Light,
		// Token: 0x04002BF8 RID: 11256
		Medium,
		// Token: 0x04002BF9 RID: 11257
		Heavy
	}
}
