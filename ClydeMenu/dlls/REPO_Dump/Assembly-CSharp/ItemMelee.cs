using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000161 RID: 353
public class ItemMelee : MonoBehaviour, IPunObservable
{
	// Token: 0x06000C09 RID: 3081 RVA: 0x00069D24 File Offset: 0x00067F24
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>().transform;
		this.hurtColliderRotation = base.transform.Find("Hurt Collider Rotation");
		this.physGrabObjectImpactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.hurtCollider.gameObject.SetActive(false);
		this.trailRenderer = base.GetComponentInChildren<TrailRenderer>();
		this.swingPoint = base.transform.Find("Swing Point");
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.particleSystem = base.transform.Find("Particles").GetComponent<ParticleSystem>();
		this.particleSystemGroundHit = base.transform.Find("Particles Ground Hit").GetComponent<ParticleSystem>();
		this.photonView = base.GetComponent<PhotonView>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.forceGrabPoint = base.transform.Find("Force Grab Point");
		this.meshHealthy = base.transform.Find("Mesh Healthy");
		this.meshBroken = base.transform.Find("Mesh Broken");
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		if (SemiFunc.RunIsArena())
		{
			HurtCollider component = this.hurtCollider.GetComponent<HurtCollider>();
			component.playerDamage = component.enemyDamage;
		}
		this.rb.mass = 0.1f;
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x00069E78 File Offset: 0x00068078
	private void DisableHurtBoxWhenEquipping()
	{
		if (this.itemEquippable.equipTimer > 0f || this.itemEquippable.unequipTimer > 0f)
		{
			this.hurtCollider.gameObject.SetActive(false);
			this.swingTimer = 0f;
			this.trailRenderer.emitting = false;
		}
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x00069ED4 File Offset: 0x000680D4
	private void FixedUpdate()
	{
		this.DisableHurtBoxWhenEquipping();
		bool flag = false;
		foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing)
		{
			if (physGrabber.isRotating)
			{
				flag = true;
			}
			if (physGrabber.isLocal)
			{
				physGrabber.OverrideMinimumGrabDistance(1.2f);
			}
		}
		float pos = -0.2f;
		this.physGrabObject.OverrideGrabVerticalPosition(pos);
		if (!flag)
		{
			Quaternion turnY = this.currentYRotation;
			Quaternion turnX = Quaternion.Euler(45f, 0f, 0f);
			this.physGrabObject.TurnXYZ(turnX, turnY, Quaternion.identity);
		}
		if (this.itemEquippable.equipTimer > 0f || this.itemEquippable.unequipTimer > 0f)
		{
			return;
		}
		if (this.isBroken)
		{
			this.physGrabObject.OverrideExtraGrabStrengthDisable(0.1f);
			this.physGrabObject.OverrideExtraTorqueStrengthDisable(0.1f);
			this.physGrabObject.OverrideTorqueStrength(0.001f, 0.1f);
			return;
		}
		if (this.prevPosUpdateTimer > 0.1f)
		{
			this.prevPosition = this.swingPoint.position;
			this.prevPosUpdateTimer = 0f;
		}
		else
		{
			this.prevPosUpdateTimer += Time.fixedDeltaTime;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!flag && this.physGrabObject.playerGrabbing.Count > 0)
		{
			this.physGrabObject.OverrideKnockOutOfGrabDisable(0.1f);
			this.physGrabObject.OverrideMinGrabStrength(5f, 0.1f);
			this.physGrabObject.OverrideExtraTorqueStrengthDisable(0.1f);
			if (!this.customTorqueStrength)
			{
				this.physGrabObject.OverrideTorqueStrength(0.4f, 0.1f);
			}
			else
			{
				this.physGrabObject.OverrideTorqueStrength(this.torqueStrength, 0.1f);
			}
			this.physGrabObject.OverrideMaterial(SemiFunc.PhysicMaterialSlippery(), 0.1f);
		}
		if (flag)
		{
			this.physGrabObject.OverrideMinTorqueStrength(4f, 0.1f);
		}
		if (this.distanceCheckTimer > 0.1f)
		{
			this.prevPosDistance = Vector3.Distance(this.prevPosition, this.swingPoint.position) * 10f * this.rb.mass;
			this.distanceCheckTimer = 0f;
		}
		this.distanceCheckTimer += Time.fixedDeltaTime;
		this.TurnWeapon();
		Vector3 vector = this.prevPosition - this.swingPoint.position;
		float num = 0.8f;
		if (!this.physGrabObject.grabbed)
		{
			num = 0.4f;
		}
		if (vector.magnitude > num * this.swingDetectSpeedMultiplier && this.swingPoint.position - this.prevPosition != Vector3.zero)
		{
			this.swingTimer = 0.4f;
			if (!this.isSwinging)
			{
				this.newSwing = true;
			}
			this.swingDirection = Quaternion.LookRotation(this.swingPoint.position - this.prevPosition);
		}
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x0006A1EC File Offset: 0x000683EC
	private void TurnWeapon()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.customRotation != Quaternion.identity && !this.turnWeapon)
		{
			Quaternion turnX = Quaternion.Euler(45f, 0f, 0f);
			this.physGrabObject.TurnXYZ(turnX, this.customRotation, Quaternion.identity);
		}
		float num = this.turnWeaponStrength;
		if (!this.turnWeapon)
		{
			return;
		}
		if (this.physGrabObject.grabbed && !this.playerAvatar)
		{
			this.playerAvatar = this.physGrabObject.playerGrabbing[0].GetComponent<PlayerAvatar>();
		}
		if (!this.physGrabObject.grabbed && this.playerAvatar)
		{
			this.playerAvatar = null;
		}
		if (!this.physGrabObject.grabbed)
		{
			return;
		}
		Vector3 forward = Vector3.forward;
		Vector3 up = Vector3.up;
		Transform transform = this.playerAvatar.transform;
		Vector3 direction = this.rb.velocity / Time.fixedDeltaTime;
		if (direction.magnitude > 200f)
		{
			Vector3 vector = this.playerAvatar.transform.InverseTransformDirection(direction);
			Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
			Quaternion quaternion = Quaternion.identity;
			if (vector2 != Vector3.zero)
			{
				quaternion = Quaternion.LookRotation(vector2);
			}
			Quaternion quaternion2 = Quaternion.Euler(0f, Mathf.Round(Quaternion.Euler(0f, quaternion.eulerAngles.y + 90f, 0f).eulerAngles.y / 90f) * 90f, 0f);
			if (quaternion2.eulerAngles.y == 270f)
			{
				quaternion2 = Quaternion.Euler(0f, 90f, 0f);
			}
			if (quaternion2.eulerAngles.y == 180f)
			{
				quaternion2 = Quaternion.Euler(0f, 0f, 0f);
			}
			this.targetYRotation = quaternion2;
		}
		this.currentYRotation = Quaternion.Slerp(this.currentYRotation, this.targetYRotation, Time.deltaTime * 5f);
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x0006A414 File Offset: 0x00068614
	private void Update()
	{
		if (this.grabbedTimer > 0f)
		{
			this.grabbedTimer -= Time.deltaTime;
		}
		if (this.physGrabObject.grabbed)
		{
			this.grabbedTimer = 1f;
		}
		if (this.hitFreezeDelay > 0f)
		{
			this.hitFreezeDelay -= Time.deltaTime;
			if (this.hitFreezeDelay <= 0f)
			{
				this.physGrabObject.FreezeForces(this.hitFreeze, Vector3.zero, Vector3.zero);
			}
		}
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.spawnTimer > 0f)
		{
			this.prevPosition = this.swingPoint.position;
			this.swingTimer = 0f;
			this.spawnTimer -= Time.deltaTime;
			return;
		}
		if (this.hitCooldown > 0f)
		{
			this.hitCooldown -= Time.deltaTime;
		}
		if (this.enemyOrPVPDurabilityLossCooldown > 0f)
		{
			this.enemyOrPVPDurabilityLossCooldown -= Time.deltaTime;
		}
		if (this.groundHitCooldown > 0f)
		{
			this.groundHitCooldown -= Time.deltaTime;
		}
		if (this.groundHitSoundTimer > 0f)
		{
			this.groundHitSoundTimer -= Time.deltaTime;
		}
		this.DisableHurtBoxWhenEquipping();
		if (this.itemEquippable.equipTimer > 0f || this.itemEquippable.unequipTimer > 0f)
		{
			return;
		}
		this.soundSwingLoop.PlayLoop(this.hurtCollider.gameObject.activeSelf, 10f, 10f, 3f);
		if (SemiFunc.IsMultiplayer() && !SemiFunc.IsMasterClient() && this.isSwinging)
		{
			this.swingTimer = 1f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.itemBattery.batteryLife <= 0f)
			{
				this.MeleeBreak();
			}
			else
			{
				this.MeleeFix();
			}
			if (this.durabilityLossCooldown > 0f)
			{
				this.durabilityLossCooldown -= Time.deltaTime;
			}
			if (!this.isBroken)
			{
				if (this.physGrabObject.grabbedLocal)
				{
					if (!this.itemBattery.batteryActive)
					{
					}
				}
				else
				{
					bool batteryActive = this.itemBattery.batteryActive;
				}
			}
		}
		if (this.isBroken)
		{
			return;
		}
		if (this.hitSoundDelayTimer > 0f)
		{
			this.hitSoundDelayTimer -= Time.deltaTime;
		}
		if (this.swingPitch != this.swingPitchTarget && this.swingPitchTargetProgress >= 1f)
		{
			this.swingPitch = this.swingPitchTarget;
		}
		Vector3 b = this.prevPosition - this.swingPoint.position;
		if (b.magnitude > 0.1f)
		{
			this.hurtColliderRotation.LookAt(this.hurtColliderRotation.position - b, Vector3.up);
			this.hurtColliderRotation.localEulerAngles = new Vector3(0f, this.hurtColliderRotation.localEulerAngles.y, 0f);
			this.hurtColliderRotation.localEulerAngles = new Vector3(0f, Mathf.Round(this.hurtColliderRotation.localEulerAngles.y / 90f) * 90f, 0f);
		}
		Vector3 vector = this.prevPosition - this.swingPoint.position;
		Vector3 normalized = this.swingStartDirection.normalized;
		Vector3 normalized2 = vector.normalized;
		double num = (double)Vector3.Dot(normalized, normalized2);
		double num2 = 0.85;
		if (!this.physGrabObject.grabbed)
		{
			num2 = 0.1;
		}
		if (num > num2)
		{
			this.swingTimer = 0f;
		}
		if (this.isSwinging)
		{
			this.ActivateHitbox();
		}
		if (this.hitTimer > 0f)
		{
			this.hitTimer -= Time.deltaTime;
		}
		if (this.swingTimer <= 0f)
		{
			if (this.hitBoxTimer <= 0f)
			{
				this.hurtCollider.gameObject.SetActive(false);
			}
			else
			{
				this.hitBoxTimer -= Time.deltaTime;
			}
			this.trailRenderer.emitting = false;
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.isSwinging = false;
				return;
			}
		}
		else
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.isSwinging = true;
			}
			if (this.hitTimer <= 0f)
			{
				this.hitBoxTimer = 0.4f;
			}
			this.swingTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x0006A86C File Offset: 0x00068A6C
	public void SwingHit()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.SwingHitRPC(true);
			return;
		}
		this.photonView.RPC("SwingHitRPC", RpcTarget.All, new object[]
		{
			true
		});
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x0006A89D File Offset: 0x00068A9D
	public void EnemySwingHit()
	{
		if (this.enemyOrPVPDurabilityLossCooldown <= 0f)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				this.EnemyOrPVPSwingHitRPC(false);
				return;
			}
			this.photonView.RPC("EnemyOrPVPSwingHitRPC", RpcTarget.All, new object[]
			{
				false
			});
		}
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x0006A8DB File Offset: 0x00068ADB
	public void PlayerSwingHit()
	{
		if (this.enemyOrPVPDurabilityLossCooldown <= 0f)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				this.EnemyOrPVPSwingHitRPC(true);
				return;
			}
			this.photonView.RPC("EnemyOrPVPSwingHitRPC", RpcTarget.All, new object[]
			{
				true
			});
		}
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x0006A91C File Offset: 0x00068B1C
	[PunRPC]
	public void EnemyOrPVPSwingHitRPC(bool _playerHit)
	{
		if (_playerHit)
		{
			if (SemiFunc.RunIsArena())
			{
				this.itemBattery.batteryLife -= this.durabilityDrainOnEnemiesAndPVP;
			}
		}
		else
		{
			this.itemBattery.batteryLife -= this.durabilityDrainOnEnemiesAndPVP;
		}
		this.enemyOrPVPDurabilityLossCooldown = 0.1f;
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x0006A970 File Offset: 0x00068B70
	[PunRPC]
	public void SwingHitRPC(bool durabilityLoss)
	{
		bool flag = false;
		if (durabilityLoss)
		{
			if (this.hitCooldown > 0f)
			{
				return;
			}
			if (this.hitSoundDelayTimer > 0f)
			{
				return;
			}
			this.hitSoundDelayTimer = 0.1f;
			this.hitCooldown = 0.3f;
		}
		else
		{
			if (this.groundHitCooldown > 0f)
			{
				return;
			}
			if (this.groundHitSoundTimer > 0f)
			{
				return;
			}
			this.groundHitCooldown = 0.3f;
			this.groundHitSoundTimer = 0.1f;
			flag = true;
		}
		if (!flag)
		{
			this.soundHit.Pitch = 1f;
			this.soundHit.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.particleSystem.Play();
		}
		else
		{
			this.soundHit.Pitch = 2f;
			this.soundHit.Play(base.transform.position, 0.5f, 1f, 1f, 1f);
			this.particleSystemGroundHit.Play();
		}
		if (this.physGrabObject.grabbed && !this.rb.isKinematic)
		{
			this.rb.velocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
		}
		if (this.hitBoxTimer > 0.05f)
		{
			this.hitBoxTimer = 0.05f;
		}
		this.hitTimer = 0.5f;
		if (SemiFunc.IsMasterClientOrSingleplayer() && durabilityLoss && this.durabilityLossCooldown <= 0f && SemiFunc.RunIsLevel())
		{
			this.itemBattery.batteryLife -= this.durabilityDrain;
			this.durabilityLossCooldown = 0.1f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.hitFreeze > 0f && !flag)
		{
			this.hitFreezeDelay = 0.06f;
		}
		if (this.onHit != null)
		{
			this.onHit.Invoke();
		}
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 10f, base.transform.position, 0.1f);
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x0006AB7C File Offset: 0x00068D7C
	public void GroundHit()
	{
		if (this.hitTimer > 0f)
		{
			return;
		}
		if (this.hitBoxTimer <= 0f)
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.SwingHitRPC(false);
			return;
		}
		this.photonView.RPC("SwingHitRPC", RpcTarget.All, new object[]
		{
			false
		});
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x0006ABD4 File Offset: 0x00068DD4
	private void MeleeBreak()
	{
		if (this.isBroken)
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.MeleeBreakRPC();
			return;
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("MeleeBreakRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x0006AC0C File Offset: 0x00068E0C
	[PunRPC]
	public void MeleeBreakRPC()
	{
		if (this.physGrabObject.isMelee)
		{
			this.particleSystem.Play();
			this.physGrabObject.isMelee = false;
			this.physGrabObject.forceGrabPoint = null;
			this.itemBattery.BatteryToggle(false);
			this.isBroken = true;
			this.hurtCollider.gameObject.SetActive(false);
			this.trailRenderer.emitting = false;
			this.meshHealthy.gameObject.SetActive(false);
			this.meshBroken.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x0006AC9B File Offset: 0x00068E9B
	private void MeleeFix()
	{
		if (!this.isBroken)
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.MeleeFixRPC();
			return;
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("MeleeFixRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x0006ACD4 File Offset: 0x00068ED4
	[PunRPC]
	public void MeleeFixRPC()
	{
		if (!this.physGrabObject.isMelee)
		{
			this.particleSystem.Play();
			this.physGrabObject.isMelee = true;
			this.physGrabObject.forceGrabPoint = this.forceGrabPoint;
			this.itemBattery.BatteryToggle(true);
			this.isBroken = false;
			this.meshHealthy.gameObject.SetActive(true);
			this.meshBroken.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x0006AD4C File Offset: 0x00068F4C
	public void ActivateHitbox()
	{
		if (this.hitTimer > 0f)
		{
			return;
		}
		if (this.newSwing)
		{
			this.soundSwing.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.swingPitchTarget = this.prevPosDistance;
			this.swingPitchTargetProgress = 0f;
			if (this.swingPoint)
			{
				this.swingStartDirection = this.swingPoint.position - this.prevPosition;
			}
			this.swingTimer = 0.8f;
			this.hitBoxTimer = 0.8f;
			if (this.grabbedTimer > 0f)
			{
				float num = 150f;
				if (!this.physGrabObject.grabbed)
				{
					num *= 0.5f;
				}
			}
			this.newSwing = false;
		}
		if (this.hurtCollider)
		{
			this.hurtCollider.gameObject.SetActive(true);
		}
		if (this.trailRenderer)
		{
			this.trailRenderer.emitting = true;
		}
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x0006AE5C File Offset: 0x0006905C
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			stream.SendNext(this.isSwinging);
			return;
		}
		bool flag = this.isSwinging;
		this.isSwinging = (bool)stream.ReceiveNext();
		if (!flag && this.isSwinging)
		{
			this.newSwing = true;
			this.ActivateHitbox();
		}
	}

	// Token: 0x0400136E RID: 4974
	private float durabilityDrain = 2.5f;

	// Token: 0x0400136F RID: 4975
	public float durabilityDrainOnEnemiesAndPVP = 5f;

	// Token: 0x04001370 RID: 4976
	public float hitFreeze = 0.2f;

	// Token: 0x04001371 RID: 4977
	public float hitFreezeDelay;

	// Token: 0x04001372 RID: 4978
	public float swingDetectSpeedMultiplier = 1f;

	// Token: 0x04001373 RID: 4979
	public bool turnWeapon = true;

	// Token: 0x04001374 RID: 4980
	public bool customTorqueStrength;

	// Token: 0x04001375 RID: 4981
	public float torqueStrength = 0.4f;

	// Token: 0x04001376 RID: 4982
	public float turnWeaponStrength = 40f;

	// Token: 0x04001377 RID: 4983
	public Quaternion customRotation = Quaternion.identity;

	// Token: 0x04001378 RID: 4984
	public UnityEvent onHit;

	// Token: 0x04001379 RID: 4985
	private Transform hurtCollider;

	// Token: 0x0400137A RID: 4986
	private Transform hurtColliderRotation;

	// Token: 0x0400137B RID: 4987
	private PhysGrabObjectImpactDetector physGrabObjectImpactDetector;

	// Token: 0x0400137C RID: 4988
	private PhysGrabObject physGrabObject;

	// Token: 0x0400137D RID: 4989
	private Rigidbody rb;

	// Token: 0x0400137E RID: 4990
	private float swingTimer = 0.1f;

	// Token: 0x0400137F RID: 4991
	private float hitBoxTimer = 0.1f;

	// Token: 0x04001380 RID: 4992
	private TrailRenderer trailRenderer;

	// Token: 0x04001381 RID: 4993
	public Sound soundSwingLoop;

	// Token: 0x04001382 RID: 4994
	public Sound soundSwing;

	// Token: 0x04001383 RID: 4995
	public Sound soundHit;

	// Token: 0x04001384 RID: 4996
	private Vector3 prevPosition;

	// Token: 0x04001385 RID: 4997
	private float prevPosDistance;

	// Token: 0x04001386 RID: 4998
	private float prevPosUpdateTimer;

	// Token: 0x04001387 RID: 4999
	private Transform swingPoint;

	// Token: 0x04001388 RID: 5000
	private Quaternion swingDirection;

	// Token: 0x04001389 RID: 5001
	private PlayerAvatar playerAvatar;

	// Token: 0x0400138A RID: 5002
	private float hitSoundDelayTimer;

	// Token: 0x0400138B RID: 5003
	private ParticleSystem particleSystem;

	// Token: 0x0400138C RID: 5004
	private ParticleSystem particleSystemGroundHit;

	// Token: 0x0400138D RID: 5005
	private PhotonView photonView;

	// Token: 0x0400138E RID: 5006
	private float swingPitch = 1f;

	// Token: 0x0400138F RID: 5007
	private float swingPitchTarget;

	// Token: 0x04001390 RID: 5008
	private float swingPitchTargetProgress;

	// Token: 0x04001391 RID: 5009
	private float distanceCheckTimer;

	// Token: 0x04001392 RID: 5010
	private ItemBattery itemBattery;

	// Token: 0x04001393 RID: 5011
	private Vector3 swingStartDirection = Vector3.zero;

	// Token: 0x04001394 RID: 5012
	private Transform forceGrabPoint;

	// Token: 0x04001395 RID: 5013
	private bool isBroken;

	// Token: 0x04001396 RID: 5014
	private Quaternion targetYRotation;

	// Token: 0x04001397 RID: 5015
	private Quaternion currentYRotation;

	// Token: 0x04001398 RID: 5016
	private float durabilityLossCooldown;

	// Token: 0x04001399 RID: 5017
	private Transform meshHealthy;

	// Token: 0x0400139A RID: 5018
	private Transform meshBroken;

	// Token: 0x0400139B RID: 5019
	private bool isSwinging;

	// Token: 0x0400139C RID: 5020
	private bool newSwing;

	// Token: 0x0400139D RID: 5021
	private float hitTimer;

	// Token: 0x0400139E RID: 5022
	private ItemEquippable itemEquippable;

	// Token: 0x0400139F RID: 5023
	private float hitCooldown;

	// Token: 0x040013A0 RID: 5024
	private float groundHitCooldown;

	// Token: 0x040013A1 RID: 5025
	private float groundHitSoundTimer;

	// Token: 0x040013A2 RID: 5026
	private float spawnTimer = 3f;

	// Token: 0x040013A3 RID: 5027
	private float grabbedTimer;

	// Token: 0x040013A4 RID: 5028
	private float enemyOrPVPDurabilityLossCooldown;
}
