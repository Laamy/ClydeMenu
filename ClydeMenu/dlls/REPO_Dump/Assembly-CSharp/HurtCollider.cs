using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020000DC RID: 220
public class HurtCollider : MonoBehaviour
{
	// Token: 0x060007DE RID: 2014 RVA: 0x0004D07C File Offset: 0x0004B27C
	private void Awake()
	{
		this.BoxCollider = base.GetComponent<BoxCollider>();
		if (!this.BoxCollider)
		{
			this.SphereCollider = base.GetComponent<SphereCollider>();
			this.Collider = this.SphereCollider;
			this.ColliderIsBox = false;
		}
		else
		{
			this.Collider = this.BoxCollider;
		}
		this.Collider.isTrigger = true;
		this.timerOriginal = this.timer;
		this.LayerMask = SemiFunc.LayerMaskGetPhysGrabObject() + LayerMask.GetMask(new string[]
		{
			"Player"
		}) + LayerMask.GetMask(new string[]
		{
			"Default"
		}) + LayerMask.GetMask(new string[]
		{
			"Enemy"
		});
		this.RayMask = LayerMask.GetMask(new string[]
		{
			"Default",
			"PhysGrabObjectHinge"
		});
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0004D15E File Offset: 0x0004B35E
	private void OnEnable()
	{
		if (!this.colliderCheckRunning)
		{
			this.colliderCheckRunning = true;
			base.StartCoroutine(this.ColliderCheck());
		}
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0004D17C File Offset: 0x0004B37C
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.colliderCheckRunning = false;
		this.cooldownLogicRunning = false;
		this.hits.Clear();
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0004D1A0 File Offset: 0x0004B3A0
	private void HasTimerLogic()
	{
		if (this.hasTimer)
		{
			if (this.timer <= 0f)
			{
				if (this.destroyOnTimerEnd)
				{
					Object.Destroy(base.gameObject);
				}
				else
				{
					base.gameObject.SetActive(false);
					this.timer = this.timerOriginal;
				}
			}
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
			}
		}
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0004D20E File Offset: 0x0004B40E
	private IEnumerator CooldownLogic()
	{
		while (this.hits.Count > 0)
		{
			for (int i = 0; i < this.hits.Count; i++)
			{
				HurtCollider.Hit hit = this.hits[i];
				hit.cooldown -= Time.deltaTime;
				if (hit.cooldown <= 0f)
				{
					this.hits.RemoveAt(i);
					i--;
				}
			}
			yield return null;
		}
		this.cooldownLogicRunning = false;
		yield break;
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x0004D220 File Offset: 0x0004B420
	private bool CanHit(GameObject hitObject, float cooldown, bool raycast, Vector3 hitPosition, HurtCollider.HitType hitType)
	{
		using (List<HurtCollider.Hit>.Enumerator enumerator = this.hits.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.hitObject == hitObject)
				{
					return false;
				}
			}
		}
		HurtCollider.Hit hit = new HurtCollider.Hit();
		hit.hitObject = hitObject;
		hit.cooldown = cooldown;
		hit.hitType = hitType;
		this.hits.Add(hit);
		if (!this.cooldownLogicRunning)
		{
			base.StartCoroutine(this.CooldownLogic());
			this.cooldownLogicRunning = true;
		}
		if (raycast)
		{
			Vector3 normalized = (hitPosition - this.Collider.bounds.center).normalized;
			float maxDistance = Vector3.Distance(hitPosition, this.Collider.bounds.center);
			foreach (RaycastHit raycastHit in Physics.RaycastAll(this.Collider.bounds.center, normalized, maxDistance, this.RayMask, QueryTriggerInteraction.Collide))
			{
				if (raycastHit.collider.gameObject.CompareTag("Wall"))
				{
					PhysGrabObject componentInParent = hitObject.GetComponentInParent<PhysGrabObject>();
					PhysGrabObject componentInParent2 = raycastHit.collider.gameObject.GetComponentInParent<PhysGrabObject>();
					if (!componentInParent || componentInParent != componentInParent2)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0004D3A0 File Offset: 0x0004B5A0
	private IEnumerator ColliderCheck()
	{
		yield return null;
		while (!LevelGenerator.Instance || !LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		for (;;)
		{
			Collider[] array;
			if (this.ColliderIsBox)
			{
				Vector3 center = base.transform.TransformPoint(this.BoxCollider.center);
				Vector3 a = new Vector3(base.transform.lossyScale.x * this.BoxCollider.size.x, base.transform.lossyScale.y * this.BoxCollider.size.y, base.transform.lossyScale.z * this.BoxCollider.size.z);
				array = Physics.OverlapBox(center, a * 0.5f, base.transform.rotation, this.LayerMask, QueryTriggerInteraction.Collide);
			}
			else
			{
				Vector3 center2 = this.Collider.bounds.center;
				float radius = base.transform.lossyScale.x * this.SphereCollider.radius;
				array = Physics.OverlapSphere(center2, radius, this.LayerMask, QueryTriggerInteraction.Collide);
			}
			if (array.Length != 0)
			{
				foreach (Collider collider in array)
				{
					if (this.playerLogic && this.playerDamageCooldown > 0f && collider.gameObject.CompareTag("Player"))
					{
						PlayerAvatar playerAvatar = collider.gameObject.GetComponentInParent<PlayerAvatar>();
						if (!playerAvatar)
						{
							PlayerController componentInParent = collider.gameObject.GetComponentInParent<PlayerController>();
							if (componentInParent)
							{
								playerAvatar = componentInParent.playerAvatarScript;
							}
						}
						if (playerAvatar)
						{
							this.PlayerHurt(playerAvatar);
						}
					}
					if (this.enemyDamageCooldown > 0f || this.physDamageCooldown > 0f || this.playerDamageCooldown > 0f)
					{
						if (collider.gameObject.CompareTag("Phys Grab Object"))
						{
							PhysGrabObject componentInParent2 = collider.gameObject.GetComponentInParent<PhysGrabObject>();
							if (!this.ignoreObjects.Contains(componentInParent2) && componentInParent2)
							{
								bool flag = false;
								PlayerTumble componentInParent3 = collider.gameObject.GetComponentInParent<PlayerTumble>();
								if (componentInParent3)
								{
									flag = true;
								}
								if (this.playerLogic && this.playerDamageCooldown > 0f && flag)
								{
									this.PlayerHurt(componentInParent3.playerAvatar);
								}
								if (SemiFunc.IsMasterClientOrSingleplayer())
								{
									EnemyRigidbody enemyRigidbody = null;
									if (this.enemyLogic && !flag)
									{
										enemyRigidbody = collider.gameObject.GetComponentInParent<EnemyRigidbody>();
										this.EnemyHurtRigidbody(enemyRigidbody, componentInParent2);
									}
									if (this.physLogic && !enemyRigidbody && !flag && this.physDamageCooldown > 0f && this.CanHit(componentInParent2.gameObject, this.physDamageCooldown, this.physRayCast, componentInParent2.centerPoint, HurtCollider.HitType.PhysObject))
									{
										bool flag2 = false;
										PhysGrabObjectImpactDetector componentInParent4 = collider.gameObject.GetComponentInParent<PhysGrabObjectImpactDetector>();
										if (componentInParent4)
										{
											if (this.physHingeDestroy)
											{
												PhysGrabHinge component = componentInParent2.GetComponent<PhysGrabHinge>();
												if (component)
												{
													component.DestroyHinge();
													flag2 = true;
												}
											}
											else if (this.physHingeBreak)
											{
												PhysGrabHinge component2 = componentInParent2.GetComponent<PhysGrabHinge>();
												if (component2 && component2.joint)
												{
													component2.joint.breakForce = 0f;
													component2.joint.breakTorque = 0f;
													flag2 = true;
												}
											}
											if (!flag2)
											{
												if (this.physDestroy)
												{
													if (!componentInParent4.destroyDisable)
													{
														PhysGrabHinge component3 = componentInParent2.GetComponent<PhysGrabHinge>();
														if (component3)
														{
															component3.DestroyHinge();
														}
														else
														{
															componentInParent4.DestroyObject(true);
														}
													}
													else
													{
														this.PhysObjectHurt(componentInParent2, HurtCollider.BreakImpact.Heavy, 50f, 30f, true, true);
													}
													flag2 = true;
												}
												else if (componentInParent2 && this.PhysObjectHurt(componentInParent2, this.physImpact, this.physHitForce, this.physHitTorque, true, false))
												{
													flag2 = true;
												}
											}
										}
										if (flag2)
										{
											this.onImpactAny.Invoke();
											this.onImpactPhysObject.Invoke();
										}
									}
								}
							}
						}
						else if (SemiFunc.IsMasterClientOrSingleplayer() && this.enemyLogic)
						{
							Enemy componentInParent5 = collider.gameObject.GetComponentInParent<Enemy>();
							if (componentInParent5 && !componentInParent5.HasRigidbody && this.CanHit(componentInParent5.gameObject, this.enemyDamageCooldown, this.enemyRayCast, componentInParent5.transform.position, HurtCollider.HitType.Enemy) && this.EnemyHurt(componentInParent5))
							{
								this.onImpactAny.Invoke();
								this.onImpactEnemyEnemy = componentInParent5;
								this.onImpactEnemy.Invoke();
							}
							if (this.enemyHitTriggers)
							{
								EnemyParent componentInParent6 = collider.gameObject.GetComponentInParent<EnemyParent>();
								if (componentInParent6)
								{
									EnemyRigidbody componentInChildren = componentInParent6.GetComponentInChildren<EnemyRigidbody>();
									if (componentInChildren)
									{
										this.EnemyHurtRigidbody(componentInChildren, componentInChildren.physGrabObject);
									}
								}
							}
						}
					}
				}
			}
			yield return new WaitForSeconds(0.05f);
		}
		yield break;
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0004D3B0 File Offset: 0x0004B5B0
	private void EnemyHurtRigidbody(EnemyRigidbody _enemyRigidbody, PhysGrabObject _physGrabObject)
	{
		if (this.enemyDamageCooldown > 0f && _enemyRigidbody && this.CanHit(_physGrabObject.gameObject, this.enemyDamageCooldown, this.enemyRayCast, _physGrabObject.centerPoint, HurtCollider.HitType.Enemy) && this.EnemyHurt(_enemyRigidbody.enemy))
		{
			this.onImpactAny.Invoke();
			this.onImpactEnemyEnemy = _enemyRigidbody.enemy;
			this.onImpactEnemy.Invoke();
		}
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0004D424 File Offset: 0x0004B624
	private bool EnemyHurt(Enemy _enemy)
	{
		if (_enemy == this.enemyHost)
		{
			return false;
		}
		if (!this.enemyLogic)
		{
			return false;
		}
		bool flag = false;
		if (this.enemyKill)
		{
			if (_enemy.HasHealth)
			{
				_enemy.Health.Hurt(_enemy.Health.healthCurrent, base.transform.forward);
			}
			else if (_enemy.HasStateDespawn)
			{
				_enemy.EnemyParent.SpawnedTimerSet(0f);
				_enemy.CurrentState = EnemyState.Despawn;
				flag = true;
			}
		}
		if (!flag)
		{
			if (this.enemyStun && _enemy.HasStateStunned && _enemy.Type <= this.enemyStunType)
			{
				_enemy.StateStunned.Set(this.enemyStunTime);
			}
			if (this.enemyFreezeTime > 0f)
			{
				_enemy.Freeze(this.enemyFreezeTime);
			}
			if (_enemy.HasRigidbody)
			{
				this.PhysObjectHurt(_enemy.Rigidbody.physGrabObject, this.enemyImpact, this.enemyHitForce, this.enemyHitTorque, true, false);
				if (this.enemyFreezeTime > 0f)
				{
					_enemy.Rigidbody.FreezeForces(this.applyForce, this.applyTorque);
				}
			}
			if (this.enemyDamage > 0 && _enemy.HasHealth)
			{
				_enemy.Health.Hurt(this.enemyDamage, this.applyForce.normalized);
			}
		}
		return true;
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0004D570 File Offset: 0x0004B770
	private void Update()
	{
		this.HasTimerLogic();
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0004D578 File Offset: 0x0004B778
	private void PlayerHurt(PlayerAvatar _player)
	{
		if (GameManager.Multiplayer() && !_player.photonView.IsMine)
		{
			return;
		}
		int enemyIndex = SemiFunc.EnemyGetIndex(this.enemyHost);
		if (this.playerKill)
		{
			this.onImpactAny.Invoke();
			this.onImpactPlayer.Invoke();
			_player.playerHealth.Hurt(_player.playerHealth.health, true, enemyIndex);
			return;
		}
		if (this.CanHit(_player.gameObject, this.playerDamageCooldown, this.playerRayCast, _player.PlayerVisionTarget.VisionTransform.position, HurtCollider.HitType.Player))
		{
			_player.playerHealth.Hurt(this.playerDamage, true, enemyIndex);
			bool flag = false;
			Vector3 center = this.Collider.bounds.center;
			Vector3 vector = (_player.PlayerVisionTarget.VisionTransform.position - center).normalized;
			vector = SemiFunc.ClampDirection(vector, base.transform.forward, this.hitSpread);
			bool flag2 = _player.tumble.isTumbling;
			if (this.playerTumbleTime > 0f && _player.playerHealth.health > 0)
			{
				_player.tumble.TumbleRequest(true, false);
				_player.tumble.TumbleOverrideTime(this.playerTumbleTime);
				if (this.playerTumbleImpactHurtTime > 0f)
				{
					_player.tumble.ImpactHurtSet(this.playerTumbleImpactHurtTime, this.playerTumbleImpactHurtDamage);
				}
				flag2 = true;
				flag = true;
			}
			if (flag2 && (this.playerTumbleForce > 0f || this.playerTumbleTorque > 0f))
			{
				flag = true;
				if (this.playerTumbleForce > 0f)
				{
					_player.tumble.TumbleForce(vector * this.playerTumbleForce);
				}
				if (this.playerTumbleTorque > 0f)
				{
					Vector3 rhs = Vector3.zero;
					if (this.playerTumbleTorqueAxis == HurtCollider.TorqueAxis.up)
					{
						rhs = _player.transform.up;
					}
					if (this.playerTumbleTorqueAxis == HurtCollider.TorqueAxis.down)
					{
						rhs = -_player.transform.up;
					}
					if (this.playerTumbleTorqueAxis == HurtCollider.TorqueAxis.right)
					{
						rhs = _player.transform.right;
					}
					if (this.playerTumbleTorqueAxis == HurtCollider.TorqueAxis.left)
					{
						rhs = -_player.transform.right;
					}
					if (this.playerTumbleTorqueAxis == HurtCollider.TorqueAxis.forward)
					{
						rhs = _player.transform.forward;
					}
					if (this.playerTumbleTorqueAxis == HurtCollider.TorqueAxis.back)
					{
						rhs = -_player.transform.forward;
					}
					Vector3 torque = Vector3.Cross((_player.localCameraPosition - center).normalized, rhs) * this.playerTumbleTorque;
					_player.tumble.TumbleTorque(torque);
				}
			}
			if (!flag2 && this.playerHitForce > 0f)
			{
				PlayerController.instance.ForceImpulse(vector * this.playerHitForce);
			}
			if (this.playerHitForce > 0f || this.playerDamage > 0 || flag)
			{
				this.onImpactPlayerAvatar = _player;
				this.onImpactAny.Invoke();
				this.onImpactPlayer.Invoke();
			}
		}
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0004D868 File Offset: 0x0004BA68
	private bool PhysObjectHurt(PhysGrabObject physGrabObject, HurtCollider.BreakImpact impact, float hitForce, float hitTorque, bool apply, bool destroyLaunch)
	{
		bool result = false;
		if (impact == HurtCollider.BreakImpact.Light)
		{
			physGrabObject.lightBreakImpulse = true;
			result = true;
		}
		else if (impact == HurtCollider.BreakImpact.Medium)
		{
			physGrabObject.mediumBreakImpulse = true;
			result = true;
		}
		else if (impact == HurtCollider.BreakImpact.Heavy)
		{
			physGrabObject.heavyBreakImpulse = true;
			result = true;
		}
		if (this.enemyHost && impact != HurtCollider.BreakImpact.None && physGrabObject.playerGrabbing.Count <= 0 && !physGrabObject.impactDetector.isEnemy)
		{
			physGrabObject.impactDetector.enemyInteractionTimer = 2f;
		}
		if (hitForce > 0f)
		{
			if (hitForce >= 5f && physGrabObject.playerGrabbing.Count > 0 && !physGrabObject.overrideKnockOutOfGrabDisable)
			{
				foreach (PhysGrabber physGrabber in Enumerable.ToList<PhysGrabber>(physGrabObject.playerGrabbing))
				{
					if (!SemiFunc.IsMultiplayer())
					{
						physGrabber.ReleaseObjectRPC(true, 2f);
					}
					else
					{
						physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
						{
							false,
							1f
						});
					}
				}
			}
			Vector3 center = this.Collider.bounds.center;
			Vector3 vector = (physGrabObject.centerPoint - center).normalized;
			vector = SemiFunc.ClampDirection(vector, base.transform.forward, this.hitSpread);
			this.applyForce = vector * hitForce;
			Vector3 normalized = (physGrabObject.centerPoint - center).normalized;
			Vector3 rhs = -physGrabObject.transform.up;
			this.applyTorque = Vector3.Cross(normalized, rhs) * hitTorque;
			if (apply)
			{
				if (destroyLaunch && !physGrabObject.rb.isKinematic)
				{
					physGrabObject.rb.velocity = Vector3.zero;
					physGrabObject.rb.angularVelocity = Vector3.zero;
					physGrabObject.impactDetector.destroyDisableLaunches++;
					physGrabObject.impactDetector.destroyDisableLaunchesTimer = 10f;
					Vector3 vector2 = Random.insideUnitSphere.normalized * 4f;
					if (physGrabObject.impactDetector.destroyDisableLaunches >= 3)
					{
						vector2 *= 20f;
						physGrabObject.impactDetector.destroyDisableLaunches = 0;
					}
					vector2.y = 0f;
					this.applyForce = (Vector3.up * 20f + vector2) * physGrabObject.rb.mass;
					this.applyTorque = Random.insideUnitSphere.normalized * 0.25f * physGrabObject.rb.mass;
				}
				physGrabObject.rb.AddForce(this.applyForce, ForceMode.Impulse);
				physGrabObject.rb.AddTorque(this.applyTorque, ForceMode.Impulse);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0004DB54 File Offset: 0x0004BD54
	private void OnDrawGizmos()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		SphereCollider component2 = base.GetComponent<SphereCollider>();
		if (component2 && (base.transform.localScale.z != base.transform.localScale.x || base.transform.localScale.z != base.transform.localScale.y))
		{
			Debug.LogError("Sphere Collider must be uniform scale");
		}
		Gizmos.color = new Color(1f, 0f, 0.39f, 6f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		if (component)
		{
			Gizmos.DrawWireCube(component.center, component.size);
		}
		if (component2)
		{
			Gizmos.DrawWireSphere(component2.center, component2.radius);
		}
		Gizmos.color = new Color(1f, 0f, 0.39f, 0.2f);
		if (component)
		{
			Gizmos.DrawCube(component.center, component.size);
		}
		if (component2)
		{
			Gizmos.DrawSphere(component2.center, component2.radius);
		}
		Gizmos.color = Color.white;
		Gizmos.matrix = Matrix4x4.identity;
		Vector3 vector = Vector3.zero;
		if (component)
		{
			vector = component.bounds.center;
		}
		if (component2)
		{
			vector = component2.bounds.center;
		}
		Vector3 vector2 = vector + base.transform.forward * 0.5f;
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector2 + Vector3.LerpUnclamped(-base.transform.forward, -base.transform.right, 0.5f) * 0.25f);
		Gizmos.DrawLine(vector2, vector2 + Vector3.LerpUnclamped(-base.transform.forward, base.transform.right, 0.5f) * 0.25f);
		if (this.hitSpread < 180f)
		{
			Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
			Vector3 vector3 = (Quaternion.AngleAxis(this.hitSpread, base.transform.right) * base.transform.forward).normalized * 1.5f;
			Vector3 vector4 = (Quaternion.AngleAxis(-this.hitSpread, base.transform.right) * base.transform.forward).normalized * 1.5f;
			Vector3 vector5 = (Quaternion.AngleAxis(this.hitSpread, base.transform.up) * base.transform.forward).normalized * 1.5f;
			Vector3 vector6 = (Quaternion.AngleAxis(-this.hitSpread, base.transform.up) * base.transform.forward).normalized * 1.5f;
			Gizmos.DrawRay(vector, vector3);
			Gizmos.DrawRay(vector, vector4);
			Gizmos.DrawRay(vector, vector5);
			Gizmos.DrawRay(vector, vector6);
			Gizmos.DrawLineStrip(new Vector3[]
			{
				vector + vector3,
				vector + vector5,
				vector + vector4,
				vector + vector6
			}, true);
			return;
		}
		if (this.hitSpread > 180f)
		{
			Debug.LogError("Hit Spread cannot be greater than 180 degrees");
		}
	}

	// Token: 0x04000E21 RID: 3617
	public bool playerLogic = true;

	// Token: 0x04000E22 RID: 3618
	[Space]
	public bool playerKill = true;

	// Token: 0x04000E23 RID: 3619
	public int playerDamage = 10;

	// Token: 0x04000E24 RID: 3620
	public float playerDamageCooldown = 0.25f;

	// Token: 0x04000E25 RID: 3621
	public float playerHitForce;

	// Token: 0x04000E26 RID: 3622
	public bool playerRayCast;

	// Token: 0x04000E27 RID: 3623
	public float playerTumbleForce;

	// Token: 0x04000E28 RID: 3624
	public float playerTumbleTorque;

	// Token: 0x04000E29 RID: 3625
	public HurtCollider.TorqueAxis playerTumbleTorqueAxis = HurtCollider.TorqueAxis.down;

	// Token: 0x04000E2A RID: 3626
	public float playerTumbleTime;

	// Token: 0x04000E2B RID: 3627
	public float playerTumbleImpactHurtTime;

	// Token: 0x04000E2C RID: 3628
	public int playerTumbleImpactHurtDamage;

	// Token: 0x04000E2D RID: 3629
	public bool physLogic = true;

	// Token: 0x04000E2E RID: 3630
	[Space]
	public bool physDestroy = true;

	// Token: 0x04000E2F RID: 3631
	public bool physHingeDestroy = true;

	// Token: 0x04000E30 RID: 3632
	public bool physHingeBreak;

	// Token: 0x04000E31 RID: 3633
	public HurtCollider.BreakImpact physImpact = HurtCollider.BreakImpact.Medium;

	// Token: 0x04000E32 RID: 3634
	public float physDamageCooldown = 0.25f;

	// Token: 0x04000E33 RID: 3635
	public float physHitForce;

	// Token: 0x04000E34 RID: 3636
	public float physHitTorque;

	// Token: 0x04000E35 RID: 3637
	public bool physRayCast;

	// Token: 0x04000E36 RID: 3638
	public bool enemyLogic = true;

	// Token: 0x04000E37 RID: 3639
	public Enemy enemyHost;

	// Token: 0x04000E38 RID: 3640
	[Space]
	[FormerlySerializedAs("enemyDespawn")]
	public bool enemyKill = true;

	// Token: 0x04000E39 RID: 3641
	public bool enemyStun = true;

	// Token: 0x04000E3A RID: 3642
	public float enemyStunTime = 2f;

	// Token: 0x04000E3B RID: 3643
	public EnemyType enemyStunType = EnemyType.Medium;

	// Token: 0x04000E3C RID: 3644
	public float enemyFreezeTime = 0.1f;

	// Token: 0x04000E3D RID: 3645
	[Space]
	public HurtCollider.BreakImpact enemyImpact = HurtCollider.BreakImpact.Medium;

	// Token: 0x04000E3E RID: 3646
	public int enemyDamage;

	// Token: 0x04000E3F RID: 3647
	public float enemyDamageCooldown = 0.25f;

	// Token: 0x04000E40 RID: 3648
	public float enemyHitForce;

	// Token: 0x04000E41 RID: 3649
	public float enemyHitTorque;

	// Token: 0x04000E42 RID: 3650
	public bool enemyRayCast;

	// Token: 0x04000E43 RID: 3651
	public bool enemyHitTriggers = true;

	// Token: 0x04000E44 RID: 3652
	[Range(0f, 180f)]
	public float hitSpread = 180f;

	// Token: 0x04000E45 RID: 3653
	public List<PhysGrabObject> ignoreObjects = new List<PhysGrabObject>();

	// Token: 0x04000E46 RID: 3654
	public bool hasTimer;

	// Token: 0x04000E47 RID: 3655
	public float timer = 0.2f;

	// Token: 0x04000E48 RID: 3656
	public bool destroyOnTimerEnd;

	// Token: 0x04000E49 RID: 3657
	public UnityEvent onImpactAny;

	// Token: 0x04000E4A RID: 3658
	public UnityEvent onImpactPlayer;

	// Token: 0x04000E4B RID: 3659
	internal PlayerAvatar onImpactPlayerAvatar;

	// Token: 0x04000E4C RID: 3660
	public UnityEvent onImpactPhysObject;

	// Token: 0x04000E4D RID: 3661
	public UnityEvent onImpactEnemy;

	// Token: 0x04000E4E RID: 3662
	internal Enemy onImpactEnemyEnemy;

	// Token: 0x04000E4F RID: 3663
	private float timerOriginal;

	// Token: 0x04000E50 RID: 3664
	private Collider Collider;

	// Token: 0x04000E51 RID: 3665
	private BoxCollider BoxCollider;

	// Token: 0x04000E52 RID: 3666
	private SphereCollider SphereCollider;

	// Token: 0x04000E53 RID: 3667
	private bool ColliderIsBox = true;

	// Token: 0x04000E54 RID: 3668
	private LayerMask LayerMask;

	// Token: 0x04000E55 RID: 3669
	private LayerMask RayMask;

	// Token: 0x04000E56 RID: 3670
	internal List<HurtCollider.Hit> hits = new List<HurtCollider.Hit>();

	// Token: 0x04000E57 RID: 3671
	private bool colliderCheckRunning;

	// Token: 0x04000E58 RID: 3672
	private bool cooldownLogicRunning;

	// Token: 0x04000E59 RID: 3673
	private Vector3 applyForce;

	// Token: 0x04000E5A RID: 3674
	private Vector3 applyTorque;

	// Token: 0x0200033F RID: 831
	public enum BreakImpact
	{
		// Token: 0x040029E1 RID: 10721
		None,
		// Token: 0x040029E2 RID: 10722
		Light,
		// Token: 0x040029E3 RID: 10723
		Medium,
		// Token: 0x040029E4 RID: 10724
		Heavy
	}

	// Token: 0x02000340 RID: 832
	public enum TorqueAxis
	{
		// Token: 0x040029E6 RID: 10726
		up,
		// Token: 0x040029E7 RID: 10727
		down,
		// Token: 0x040029E8 RID: 10728
		left,
		// Token: 0x040029E9 RID: 10729
		right,
		// Token: 0x040029EA RID: 10730
		forward,
		// Token: 0x040029EB RID: 10731
		back
	}

	// Token: 0x02000341 RID: 833
	public enum HitType
	{
		// Token: 0x040029ED RID: 10733
		Player,
		// Token: 0x040029EE RID: 10734
		PhysObject,
		// Token: 0x040029EF RID: 10735
		Enemy
	}

	// Token: 0x02000342 RID: 834
	public class Hit
	{
		// Token: 0x040029F0 RID: 10736
		public HurtCollider.HitType hitType;

		// Token: 0x040029F1 RID: 10737
		public GameObject hitObject;

		// Token: 0x040029F2 RID: 10738
		public float cooldown;
	}
}
