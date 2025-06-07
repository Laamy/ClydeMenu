using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000175 RID: 373
public class ItemGun : MonoBehaviour
{
	// Token: 0x06000CB2 RID: 3250 RVA: 0x00070260 File Offset: 0x0006E460
	private void Start()
	{
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.photonView = base.GetComponent<PhotonView>();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.triggerAnimationCurve = AssetManager.instance.animationCurveClickInOut;
		if (this.onStateIdleUpdate == null)
		{
			this.hasIdleUpdate = false;
		}
		if (this.onStateIdleFixedUpdate == null)
		{
			this.hasIdleFixedUpdate = false;
		}
		if (this.onStateOutOfAmmoUpdate == null)
		{
			this.hasOutOfAmmoUpdate = false;
		}
		if (this.onStateOutOfAmmoFixedUpdate == null)
		{
			this.hasOutOfAmmoFixedUpdate = false;
		}
		if (this.onStateBuildupUpdate == null)
		{
			this.hasBuildupUpdate = false;
		}
		if (this.onStateBuildupFixedUpdate == null)
		{
			this.hasBuildupFixedUpdate = false;
		}
		if (this.onStateShootingUpdate == null)
		{
			this.hasShootingUpdate = false;
		}
		if (this.onStateShootingFixedUpdate == null)
		{
			this.hasShootingFixedUpdate = false;
		}
		if (this.onStateReloadingUpdate == null)
		{
			this.hasReloadingUpdate = false;
		}
		if (this.onStateReloadingFixedUpdate == null)
		{
			this.hasReloadingFixedUpdate = false;
		}
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0007035B File Offset: 0x0006E55B
	private void FixedUpdate()
	{
		this.StateMachine(true);
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x00070364 File Offset: 0x0006E564
	private void Update()
	{
		this.StateMachine(false);
		if (this.physGrabObject.grabbed && this.physGrabObject.grabbedLocal)
		{
			PhysGrabber.instance.OverrideGrabDistance(this.distanceKeep);
		}
		if (this.triggerAnimationActive)
		{
			float num = 45f;
			this.triggerAnimationEval += Time.deltaTime * 4f;
			this.gunTrigger.localRotation = Quaternion.Euler(num * this.triggerAnimationCurve.Evaluate(this.triggerAnimationEval), 0f, 0f);
			if (this.triggerAnimationEval >= 1f)
			{
				this.gunTrigger.localRotation = Quaternion.Euler(0f, 0f, 0f);
				this.triggerAnimationActive = false;
				this.triggerAnimationEval = 1f;
			}
		}
		this.UpdateMaster();
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x0007043C File Offset: 0x0006E63C
	private void UpdateMaster()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.physGrabObject.playerGrabbing.Count > 0)
		{
			Quaternion turnX = Quaternion.Euler(this.aimVerticalOffset, 0f, 0f);
			Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
			Quaternion identity = Quaternion.identity;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;
			foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing)
			{
				if (flag4)
				{
					if (physGrabber.playerAvatar.isCrouching)
					{
						flag2 = true;
					}
					if (physGrabber.playerAvatar.isCrawling)
					{
						flag3 = true;
					}
					flag4 = false;
				}
				if (physGrabber.isRotating)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.physGrabObject.TurnXYZ(turnX, turnY, identity);
			}
			float num = this.grabVerticalOffset;
			if (flag2)
			{
				num += 0.5f;
			}
			if (flag3)
			{
				num -= 0.5f;
			}
			this.physGrabObject.OverrideGrabVerticalPosition(num);
			if (!flag)
			{
				if (this.stateCurrent == ItemGun.State.OutOfAmmo)
				{
					this.physGrabObject.OverrideTorqueStrength(0.01f, 0.1f);
					this.physGrabObject.OverrideExtraTorqueStrengthDisable(0.1f);
					this.physGrabObject.OverrideExtraGrabStrengthDisable(0.1f);
				}
				else if (this.physGrabObject.grabbed)
				{
					this.physGrabObject.OverrideTorqueStrength(12f, 0.1f);
					this.physGrabObject.OverrideAngularDrag(20f, 0.1f);
				}
			}
			if (flag)
			{
				this.physGrabObject.OverrideTorqueStrength(2f, 0.1f);
				this.physGrabObject.OverrideAngularDrag(20f, 0.1f);
			}
		}
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x00070608 File Offset: 0x0006E808
	public void Misfire()
	{
		if (this.physGrabObject.grabbed)
		{
			return;
		}
		if (this.physGrabObject.hasNeverBeenGrabbed)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if ((float)Random.Range(0, 100) < this.misfirePercentageChange)
		{
			this.Shoot();
		}
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x00070648 File Offset: 0x0006E848
	public void Shoot()
	{
		bool flag = false;
		if (this.itemBattery.batteryLifeInt <= 0)
		{
			flag = true;
		}
		if (Random.Range(0, 10000) == 0)
		{
			flag = false;
		}
		if (flag)
		{
			return;
		}
		if (this.hasOneShot)
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("ShootRPC", RpcTarget.All, Array.Empty<object>());
			}
			else
			{
				this.ShootRPC(default(PhotonMessageInfo));
			}
			this.StateSet(ItemGun.State.Reloading);
			return;
		}
		if (this.hasBuildUp)
		{
			this.StateSet(ItemGun.State.Buildup);
			return;
		}
		this.StateSet(ItemGun.State.Shooting);
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x000706CF File Offset: 0x0006E8CF
	private void MuzzleFlash()
	{
		Object.Instantiate<GameObject>(this.muzzleFlashPrefab, this.gunMuzzle.position, this.gunMuzzle.rotation, this.gunMuzzle).GetComponent<ItemGunMuzzleFlash>().ActivateAllEffects();
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x00070702 File Offset: 0x0006E902
	private void StartTriggerAnimation()
	{
		this.triggerAnimationActive = true;
		this.triggerAnimationEval = 0f;
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x00070718 File Offset: 0x0006E918
	[PunRPC]
	public void ShootRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		float distanceMin = 3f * this.cameraShakeMultiplier;
		float distanceMax = 16f * this.cameraShakeMultiplier;
		SemiFunc.CameraShakeImpactDistance(this.gunMuzzle.position, 5f * this.cameraShakeMultiplier, 0.1f, distanceMin, distanceMax);
		SemiFunc.CameraShakeDistance(this.gunMuzzle.position, 0.1f * this.cameraShakeMultiplier, 0.1f * this.cameraShakeMultiplier, distanceMin, distanceMax);
		this.soundShoot.Play(this.gunMuzzle.position, 1f, 1f, 1f, 1f);
		this.soundShootGlobal.Play(this.gunMuzzle.position, 1f, 1f, 1f, 1f);
		this.MuzzleFlash();
		this.StartTriggerAnimation();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.investigateRadius > 0f)
			{
				EnemyDirector.instance.SetInvestigate(base.transform.position, this.investigateRadius, false);
			}
			this.physGrabObject.rb.AddForceAtPosition(-this.gunMuzzle.forward * this.gunRecoilForce, this.gunMuzzle.position, ForceMode.Impulse);
			if (!this.batteryDrainFullBar)
			{
				this.itemBattery.batteryLife -= this.batteryDrain;
			}
			else
			{
				this.itemBattery.RemoveFullBar(this.batteryDrainFullBars);
			}
			for (int i = 0; i < this.numberOfBullets; i++)
			{
				Vector3 endPosition = this.gunMuzzle.position;
				bool hit = false;
				bool flag = false;
				Vector3 vector = this.gunMuzzle.forward;
				if (this.gunRandomSpread > 0f)
				{
					float angle = Random.Range(0f, this.gunRandomSpread / 2f);
					float angle2 = Random.Range(0f, 360f);
					Vector3 normalized = Vector3.Cross(vector, Random.onUnitSphere).normalized;
					Quaternion rhs = Quaternion.AngleAxis(angle, normalized);
					vector = (Quaternion.AngleAxis(angle2, vector) * rhs * vector).normalized;
				}
				RaycastHit raycastHit;
				if (Physics.Raycast(this.gunMuzzle.position, vector, out raycastHit, this.gunRange, SemiFunc.LayerMaskGetVisionObstruct() + LayerMask.GetMask(new string[]
				{
					"Enemy"
				})))
				{
					endPosition = raycastHit.point;
					hit = true;
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					endPosition = this.gunMuzzle.position + this.gunMuzzle.forward * this.gunRange;
					hit = true;
				}
				this.ShootBullet(endPosition, hit);
			}
		}
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x000709C4 File Offset: 0x0006EBC4
	private void ShootBullet(Vector3 _endPosition, bool _hit)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("ShootBulletRPC", RpcTarget.All, new object[]
			{
				_endPosition,
				_hit
			});
			return;
		}
		this.ShootBulletRPC(_endPosition, _hit, default(PhotonMessageInfo));
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x00070A1C File Offset: 0x0006EC1C
	[PunRPC]
	public void ShootBulletRPC(Vector3 _endPosition, bool _hit, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (this.physGrabObject.playerGrabbing.Count > 1)
		{
			foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing)
			{
				physGrabber.OverrideGrabRelease();
			}
		}
		ItemGunBullet component = Object.Instantiate<GameObject>(this.bulletPrefab, this.gunMuzzle.position, this.gunMuzzle.rotation).GetComponent<ItemGunBullet>();
		component.hitPosition = _endPosition;
		component.bulletHit = _hit;
		this.hurtCollider = component.GetComponentInChildren<HurtCollider>();
		this.soundHit.Play(_endPosition, 1f, 1f, 1f, 1f);
		component.shootLineWidthCurve = this.shootLineWidthCurve;
		component.ActivateAll();
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x00070B04 File Offset: 0x0006ED04
	private void StateSet(ItemGun.State _state)
	{
		if (_state == this.stateCurrent)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("StateSetRPC", RpcTarget.All, new object[]
				{
					(int)_state
				});
				return;
			}
		}
		else
		{
			this.StateSetRPC((int)_state, default(PhotonMessageInfo));
		}
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x00070B5C File Offset: 0x0006ED5C
	private void ShootLogic()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemToggle.toggleState != this.prevToggleState)
		{
			if (this.itemBattery.batteryLifeInt <= 0)
			{
				this.soundNoAmmoClick.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.StartTriggerAnimation();
				SemiFunc.CameraShakeImpact(1f, 0.1f);
				this.physGrabObject.rb.AddForceAtPosition(-this.gunMuzzle.forward * 1f, this.gunMuzzle.position, ForceMode.Impulse);
			}
			else
			{
				this.Shoot();
			}
			this.prevToggleState = this.itemToggle.toggleState;
		}
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x00070C24 File Offset: 0x0006EE24
	[PunRPC]
	private void StateSetRPC(int state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.stateStart = true;
		this.statePrev = this.stateCurrent;
		this.stateCurrent = (ItemGun.State)state;
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x00070C4C File Offset: 0x0006EE4C
	private void StateMachine(bool _fixedUpdate)
	{
		switch (this.stateCurrent)
		{
		case ItemGun.State.Idle:
			this.StateIdle(_fixedUpdate);
			return;
		case ItemGun.State.OutOfAmmo:
			this.StateOutOfAmmo(_fixedUpdate);
			return;
		case ItemGun.State.Buildup:
			this.StateBuildup(_fixedUpdate);
			return;
		case ItemGun.State.Shooting:
			this.StateShooting(_fixedUpdate);
			return;
		case ItemGun.State.Reloading:
			this.StateReloading(_fixedUpdate);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x00070CA4 File Offset: 0x0006EEA4
	private void StateIdle(bool _fixedUpdate)
	{
		if (this.stateStart && !_fixedUpdate)
		{
			if (this.onStateIdleStart != null)
			{
				this.onStateIdleStart.Invoke();
			}
			this.stateStart = false;
			this.prevToggleState = this.itemToggle.toggleState;
		}
		if (!_fixedUpdate)
		{
			this.ShootLogic();
			if (this.hasIdleUpdate)
			{
				this.onStateIdleUpdate.Invoke();
			}
		}
		if (_fixedUpdate && this.hasIdleFixedUpdate)
		{
			this.onStateIdleFixedUpdate.Invoke();
		}
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x00070D1C File Offset: 0x0006EF1C
	private void StateOutOfAmmo(bool _fixedUpdate)
	{
		if (this.stateStart && !_fixedUpdate)
		{
			if (this.onStateOutOfAmmoStart != null)
			{
				this.onStateOutOfAmmoStart.Invoke();
			}
			this.stateStart = false;
			this.prevToggleState = this.itemToggle.toggleState;
		}
		if (!_fixedUpdate)
		{
			if (this.itemBattery.batteryLifeInt > 0)
			{
				this.StateSet(ItemGun.State.Idle);
				return;
			}
			this.ShootLogic();
			if (this.hasOutOfAmmoUpdate)
			{
				this.onStateOutOfAmmoUpdate.Invoke();
			}
		}
		if (_fixedUpdate && this.hasOutOfAmmoFixedUpdate)
		{
			this.onStateOutOfAmmoFixedUpdate.Invoke();
		}
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x00070DA8 File Offset: 0x0006EFA8
	private void StateBuildup(bool _fixedUpdate)
	{
		if (this.stateStart && !_fixedUpdate)
		{
			if (this.onStateBuildupStart != null)
			{
				this.onStateBuildupStart.Invoke();
			}
			this.stateTimer = 0f;
			this.stateTimeMax = this.buildUpTime;
			this.stateStart = false;
		}
		if (!_fixedUpdate)
		{
			if (this.hasBuildupUpdate)
			{
				this.onStateBuildupUpdate.Invoke();
			}
			this.stateTimer += Time.deltaTime;
			if (this.itemEquippable && this.itemEquippable.isEquipped)
			{
				this.StateSet(ItemGun.State.Idle);
			}
			if (this.stateTimer >= this.stateTimeMax && this.itemBattery.batteryLifeInt > 0)
			{
				this.StateSet(ItemGun.State.Shooting);
			}
		}
		if (_fixedUpdate && this.hasBuildupFixedUpdate)
		{
			this.onStateBuildupFixedUpdate.Invoke();
		}
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x00070E74 File Offset: 0x0006F074
	private void StateShooting(bool _fixedUpdate)
	{
		if (this.stateStart && !_fixedUpdate)
		{
			this.stateStart = false;
			if (this.onStateShootingStart != null)
			{
				this.onStateShootingStart.Invoke();
			}
			if (!this.hasOneShot)
			{
				this.stateTimeMax = this.shootTime;
				this.stateTimer = 0f;
			}
			else
			{
				this.stateTimer = 0.001f;
			}
			if (this.itemBattery.batteryLifeInt > 0)
			{
				this.itemBattery.RemoveFullBar(1);
			}
		}
		if (!_fixedUpdate)
		{
			this.stateTimer += Time.deltaTime;
			if (this.stateTimer >= this.stateTimeMax || (this.itemEquippable && this.itemEquippable.isEquipped))
			{
				this.StateSet(ItemGun.State.Reloading);
			}
			if (this.hasShootingUpdate)
			{
				this.onStateShootingUpdate.Invoke();
			}
		}
		if (_fixedUpdate && this.hasShootingFixedUpdate)
		{
			this.onStateShootingFixedUpdate.Invoke();
		}
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x00070F5C File Offset: 0x0006F15C
	private void StateReloading(bool _fixedUpdate)
	{
		if (this.stateStart && !_fixedUpdate)
		{
			this.stateStart = false;
			if (this.onStateReloadingStart != null)
			{
				this.onStateReloadingStart.Invoke();
			}
			this.stateTimeMax = this.shootCooldown;
			this.stateTimer = 0f;
		}
		if (!_fixedUpdate)
		{
			this.stateTimer += Time.deltaTime;
			if (this.stateTimer >= this.stateTimeMax)
			{
				if (this.itemBattery.batteryLifeInt > 0)
				{
					this.StateSet(ItemGun.State.Idle);
				}
				else
				{
					this.StateSet(ItemGun.State.OutOfAmmo);
				}
			}
			if (this.hasReloadingUpdate)
			{
				this.onStateReloadingUpdate.Invoke();
			}
		}
		if (_fixedUpdate && this.hasReloadingFixedUpdate)
		{
			this.onStateReloadingFixedUpdate.Invoke();
		}
	}

	// Token: 0x04001471 RID: 5233
	private PhysGrabObject physGrabObject;

	// Token: 0x04001472 RID: 5234
	private ItemToggle itemToggle;

	// Token: 0x04001473 RID: 5235
	public bool hasOneShot = true;

	// Token: 0x04001474 RID: 5236
	public float shootTime = 1f;

	// Token: 0x04001475 RID: 5237
	public bool hasBuildUp;

	// Token: 0x04001476 RID: 5238
	public float buildUpTime = 1f;

	// Token: 0x04001477 RID: 5239
	public int numberOfBullets = 1;

	// Token: 0x04001478 RID: 5240
	[Range(0f, 65f)]
	public float gunRandomSpread;

	// Token: 0x04001479 RID: 5241
	public float gunRange = 50f;

	// Token: 0x0400147A RID: 5242
	public float distanceKeep = 0.8f;

	// Token: 0x0400147B RID: 5243
	public float gunRecoilForce = 1f;

	// Token: 0x0400147C RID: 5244
	public float cameraShakeMultiplier = 1f;

	// Token: 0x0400147D RID: 5245
	public float torqueMultiplier = 1f;

	// Token: 0x0400147E RID: 5246
	public float grabStrengthMultiplier = 1f;

	// Token: 0x0400147F RID: 5247
	public float shootCooldown = 1f;

	// Token: 0x04001480 RID: 5248
	public float batteryDrain = 0.1f;

	// Token: 0x04001481 RID: 5249
	public bool batteryDrainFullBar;

	// Token: 0x04001482 RID: 5250
	public int batteryDrainFullBars = 1;

	// Token: 0x04001483 RID: 5251
	[Range(0f, 100f)]
	public float misfirePercentageChange = 50f;

	// Token: 0x04001484 RID: 5252
	public AnimationCurve shootLineWidthCurve;

	// Token: 0x04001485 RID: 5253
	public float grabVerticalOffset = -0.2f;

	// Token: 0x04001486 RID: 5254
	public float aimVerticalOffset = -5f;

	// Token: 0x04001487 RID: 5255
	public float investigateRadius = 20f;

	// Token: 0x04001488 RID: 5256
	public Transform gunMuzzle;

	// Token: 0x04001489 RID: 5257
	public GameObject bulletPrefab;

	// Token: 0x0400148A RID: 5258
	public GameObject muzzleFlashPrefab;

	// Token: 0x0400148B RID: 5259
	public Transform gunTrigger;

	// Token: 0x0400148C RID: 5260
	internal HurtCollider hurtCollider;

	// Token: 0x0400148D RID: 5261
	public Sound soundShoot;

	// Token: 0x0400148E RID: 5262
	public Sound soundShootGlobal;

	// Token: 0x0400148F RID: 5263
	public Sound soundNoAmmoClick;

	// Token: 0x04001490 RID: 5264
	public Sound soundHit;

	// Token: 0x04001491 RID: 5265
	private ItemBattery itemBattery;

	// Token: 0x04001492 RID: 5266
	private PhotonView photonView;

	// Token: 0x04001493 RID: 5267
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04001494 RID: 5268
	private bool prevToggleState;

	// Token: 0x04001495 RID: 5269
	private AnimationCurve triggerAnimationCurve;

	// Token: 0x04001496 RID: 5270
	private float triggerAnimationEval;

	// Token: 0x04001497 RID: 5271
	private bool triggerAnimationActive;

	// Token: 0x04001498 RID: 5272
	public UnityEvent onStateIdleStart;

	// Token: 0x04001499 RID: 5273
	public UnityEvent onStateIdleUpdate;

	// Token: 0x0400149A RID: 5274
	public UnityEvent onStateIdleFixedUpdate;

	// Token: 0x0400149B RID: 5275
	[Space(20f)]
	public UnityEvent onStateOutOfAmmoStart;

	// Token: 0x0400149C RID: 5276
	public UnityEvent onStateOutOfAmmoUpdate;

	// Token: 0x0400149D RID: 5277
	public UnityEvent onStateOutOfAmmoFixedUpdate;

	// Token: 0x0400149E RID: 5278
	[Space(20f)]
	public UnityEvent onStateBuildupStart;

	// Token: 0x0400149F RID: 5279
	public UnityEvent onStateBuildupUpdate;

	// Token: 0x040014A0 RID: 5280
	public UnityEvent onStateBuildupFixedUpdate;

	// Token: 0x040014A1 RID: 5281
	[Space(20f)]
	public UnityEvent onStateShootingStart;

	// Token: 0x040014A2 RID: 5282
	public UnityEvent onStateShootingUpdate;

	// Token: 0x040014A3 RID: 5283
	public UnityEvent onStateShootingFixedUpdate;

	// Token: 0x040014A4 RID: 5284
	[Space(20f)]
	public UnityEvent onStateReloadingStart;

	// Token: 0x040014A5 RID: 5285
	public UnityEvent onStateReloadingUpdate;

	// Token: 0x040014A6 RID: 5286
	public UnityEvent onStateReloadingFixedUpdate;

	// Token: 0x040014A7 RID: 5287
	private bool hasIdleUpdate = true;

	// Token: 0x040014A8 RID: 5288
	private bool hasIdleFixedUpdate = true;

	// Token: 0x040014A9 RID: 5289
	private bool hasOutOfAmmoUpdate = true;

	// Token: 0x040014AA RID: 5290
	private bool hasOutOfAmmoFixedUpdate = true;

	// Token: 0x040014AB RID: 5291
	private bool hasBuildupUpdate = true;

	// Token: 0x040014AC RID: 5292
	private bool hasBuildupFixedUpdate = true;

	// Token: 0x040014AD RID: 5293
	private bool hasShootingUpdate = true;

	// Token: 0x040014AE RID: 5294
	private bool hasShootingFixedUpdate = true;

	// Token: 0x040014AF RID: 5295
	private bool hasReloadingUpdate = true;

	// Token: 0x040014B0 RID: 5296
	private bool hasReloadingFixedUpdate = true;

	// Token: 0x040014B1 RID: 5297
	internal float stateTimer;

	// Token: 0x040014B2 RID: 5298
	internal float stateTimeMax;

	// Token: 0x040014B3 RID: 5299
	internal ItemGun.State stateCurrent;

	// Token: 0x040014B4 RID: 5300
	private ItemGun.State statePrev;

	// Token: 0x040014B5 RID: 5301
	private bool stateStart;

	// Token: 0x040014B6 RID: 5302
	private ItemEquippable itemEquippable;

	// Token: 0x0200038F RID: 911
	public enum State
	{
		// Token: 0x04002B7F RID: 11135
		Idle,
		// Token: 0x04002B80 RID: 11136
		OutOfAmmo,
		// Token: 0x04002B81 RID: 11137
		Buildup,
		// Token: 0x04002B82 RID: 11138
		Shooting,
		// Token: 0x04002B83 RID: 11139
		Reloading
	}
}
