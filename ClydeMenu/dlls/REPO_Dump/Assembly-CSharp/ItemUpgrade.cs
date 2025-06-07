using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000185 RID: 389
public class ItemUpgrade : MonoBehaviour
{
	// Token: 0x06000D46 RID: 3398 RVA: 0x00074074 File Offset: 0x00072274
	private void Start()
	{
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.customTargetingCondition = base.GetComponent<ITargetingCondition>();
		this.particleEffects = base.transform.Find("Particle Effects");
		this.cameraMain = Camera.main;
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.rb = base.GetComponent<Rigidbody>();
		this.photonView = base.GetComponent<PhotonView>();
		this.lineBetweenTwoPoints = base.GetComponent<LineBetweenTwoPoints>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.beamColor = this.colorPreset.GetColorDark();
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		int num = 0;
		if (num < componentsInChildren.Length)
		{
			Collider collider = componentsInChildren[num];
			this.physicMaterialOriginal = collider.material;
		}
		if (SemiFunc.RunIsShop())
		{
			this.itemToggle.enabled = false;
		}
		this.physGrabObject.clientNonKinematic = true;
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x00074158 File Offset: 0x00072358
	private void Update()
	{
		if (this.physGrabObject.playerGrabbing.Count > 0)
		{
			bool flag = false;
			using (List<PhysGrabber>.Enumerator enumerator = this.physGrabObject.playerGrabbing.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.isRotating)
					{
						flag = true;
					}
				}
			}
			float dist = 0.5f;
			if (this.physGrabObject.grabbed)
			{
				if (this.physGrabObject.grabbedLocal && !this.pushedOrPulled)
				{
					PhysGrabber.instance.OverrideGrabDistance(dist);
				}
				if (PhysGrabber.instance.isPulling || PhysGrabber.instance.isPushing)
				{
					this.pushedOrPulled = true;
				}
			}
			else
			{
				this.pushedOrPulled = false;
			}
			if (!flag && !this.pushedOrPulled)
			{
				Quaternion turnX = Quaternion.Euler(45f, 0f, 0f);
				Quaternion turnY = Quaternion.Euler(45f, 180f, 0f);
				Quaternion identity = Quaternion.identity;
				this.physGrabObject.TurnXYZ(turnX, turnY, identity);
			}
		}
		else
		{
			this.pushedOrPulled = false;
		}
		this.TargetingLogic();
		this.PlayerUpgradeLogic();
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x00074288 File Offset: 0x00072488
	private void PlayerUpgrade()
	{
		if (this.upgradeDone)
		{
			return;
		}
		Transform transform = base.transform.Find("Mesh");
		if (transform)
		{
			transform.GetComponent<MeshRenderer>().enabled = false;
		}
		this.upgradeEvent.Invoke();
		this.particleEffects.parent = null;
		this.particleEffects.gameObject.SetActive(true);
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID);
		if (playerAvatar.isLocal)
		{
			StatsUI.instance.Fetch();
			StatsUI.instance.ShowStats();
			CameraGlitch.Instance.PlayUpgrade();
		}
		else
		{
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 1f, 6f, base.transform.position, 0.2f);
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			playerAvatar.playerHealth.MaterialEffectOverride(PlayerHealth.Effect.Upgrade);
		}
		StatsManager.instance.itemsPurchased[this.itemAttributes.item.itemAssetName] = Mathf.Max(StatsManager.instance.itemsPurchased[this.itemAttributes.item.itemAssetName] - 1, 0);
		this.impactDetector.DestroyObject(false);
		this.upgradeDone = true;
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x000743C5 File Offset: 0x000725C5
	private void PlayerUpgradeLogic()
	{
		if (!this.isPlayerUpgrade)
		{
			return;
		}
		if (this.itemToggle.toggleState)
		{
			this.PlayerUpgrade();
		}
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x000743E4 File Offset: 0x000725E4
	private void TargetingLogic()
	{
		if (this.isPlayerUpgrade)
		{
			return;
		}
		if (this.magnetActive && !this.physGrabObject.grabbed)
		{
			this.DeactivateMagnet();
		}
		if (this.itemActivated && this.magnetActive)
		{
			if (this.magnetTarget && !this.magnetTarget.gameObject.activeSelf)
			{
				this.magnetActive = false;
				this.magnetTarget = null;
			}
			if (this.magnetTarget)
			{
				if (this.rayHitPosition != Vector3.zero && !this.targetIsPlayer)
				{
					this.animatedRayHitPosition = Vector3.Lerp(this.animatedRayHitPosition, this.rayHitPosition, Time.deltaTime * 10f);
					this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, this.magnetTarget.TransformPoint(this.animatedRayHitPosition));
					this.connectionPoint = this.magnetTarget.TransformPoint(this.animatedRayHitPosition);
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
						Vector3 b = new Vector3(0f, -0.5f, 0f);
						if (!this.targetIsLocalPlayer)
						{
							this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, this.magnetTarget.position + b);
							this.connectionPoint = this.magnetTarget.position + b;
						}
						else
						{
							Vector3 point = this.cameraMain.transform.position + b;
							this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, point);
							this.connectionPoint = point;
						}
					}
				}
			}
		}
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
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
		if (this.physGrabObject.grabbed)
		{
			this.checkTimer += Time.deltaTime;
			if (this.checkTimer > 0.5f)
			{
				if (this.SphereCheck())
				{
					this.ActivateMagnet();
				}
				this.checkTimer = 0f;
				return;
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
					return;
				}
				this.rayTimer -= Time.deltaTime;
				return;
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
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x00074744 File Offset: 0x00072944
	private void MagnetLogic()
	{
		if (this.isPlayerUpgrade)
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
			if (this.attachPointFound)
			{
				Vector3 a = this.magnetTarget.TransformPoint(this.attachPoint) - base.transform.position;
				Vector3 a2 = this.springConstant * a;
				Vector3 velocity = this.rb.velocity;
				Vector3 b = -this.dampingCoefficient * velocity;
				Vector3.ClampMagnitude(a2 + b, 20f);
			}
			else
			{
				float magnitude = (this.magnetTarget.position - base.transform.position).magnitude;
			}
			if (this.targetIsPlayer)
			{
				if (Vector3.Distance(base.transform.position, this.magnetTarget.position) > 1.8f)
				{
					this.DeactivateMagnet();
				}
			}
			else if (Vector3.Distance(base.transform.position, this.magnetTarget.position) > 1f)
			{
				this.DeactivateMagnet();
			}
			if (this.magnetTargetPhysGrabObject)
			{
				this.magnetTargetPhysGrabObject.OverrideZeroGravity(0.1f);
				this.magnetTargetPhysGrabObject.OverrideMass(0.1f, 0.1f);
				this.magnetTargetPhysGrabObject.OverrideMaterial(SemiFunc.PhysicMaterialSticky(), 0.1f);
				this.magnetTargetRigidbody.AddForce((base.transform.position - this.magnetTarget.position).normalized * 1f, ForceMode.Force);
			}
		}
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x000748F9 File Offset: 0x00072AF9
	private void FixedUpdate()
	{
		this.MagnetLogic();
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x00074901 File Offset: 0x00072B01
	private void DeactivateMagnet()
	{
		this.attachPointFound = false;
		this.MagnetActiveToggle(false);
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x00074911 File Offset: 0x00072B11
	private void ActivateMagnet()
	{
		this.MagnetActiveToggle(true);
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x0007491A File Offset: 0x00072B1A
	private void ButtonToggleLogic(bool activated)
	{
		if (!activated && this.magnetActive)
		{
			this.DeactivateMagnet();
		}
		this.itemActivated = activated;
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x00074934 File Offset: 0x00072B34
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

	// Token: 0x06000D51 RID: 3409 RVA: 0x00074995 File Offset: 0x00072B95
	[PunRPC]
	private void ButtonToggleRPC(bool activated)
	{
		this.ButtonToggleLogic(activated);
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x000749A0 File Offset: 0x00072BA0
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

	// Token: 0x06000D53 RID: 3411 RVA: 0x00074A12 File Offset: 0x00072C12
	private void MagnetActiveToggleLogic(bool activated)
	{
		this.magnetActive = activated;
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x00074A1B File Offset: 0x00072C1B
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

	// Token: 0x06000D55 RID: 3413 RVA: 0x00074A58 File Offset: 0x00072C58
	[PunRPC]
	private void MagnetActiveToggleRPC(bool activated)
	{
		this.MagnetActiveToggleLogic(activated);
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x00074A64 File Offset: 0x00072C64
	private void NewRayHitPointLogic(Vector3 newRayHitPosition, int photonViewId, int colliderID, Transform newMagnetTarget)
	{
		this.targetIsPlayer = false;
		this.targetIsLocalPlayer = false;
		if (newMagnetTarget)
		{
			this.magnetTargetPhysGrabObject = newMagnetTarget.GetComponent<PhysGrabObject>();
			if (colliderID != -1)
			{
				this.magnetTarget = newMagnetTarget.GetComponent<PhysGrabObject>().FindColliderFromID(colliderID);
			}
			else
			{
				this.magnetTarget = newMagnetTarget;
			}
			this.animatedRayHitPosition = this.rayHitPosition;
			this.rayHitPosition = this.magnetTarget.InverseTransformPoint(newRayHitPosition);
			this.magnetTargetRigidbody = this.GetHighestParentWithRigidbody(this.magnetTarget).GetComponent<Rigidbody>();
			return;
		}
		this.magnetTargetPhysGrabObject = PhotonView.Find(photonViewId).gameObject.GetComponent<PhysGrabObject>();
		if (colliderID != -1)
		{
			this.magnetTarget = PhotonView.Find(photonViewId).gameObject.GetComponent<PhysGrabObject>().FindColliderFromID(colliderID);
		}
		else
		{
			this.targetIsPlayer = true;
			this.magnetTarget = PhotonView.Find(photonViewId).GetComponent<PlayerAvatar>().PlayerVisionTarget.VisionTransform;
			if (PhotonView.Find(photonViewId).GetComponent<PlayerAvatar>().isLocal)
			{
				this.targetIsLocalPlayer = true;
			}
			this.playerAvatarTarget = PhotonView.Find(photonViewId).GetComponent<PlayerAvatar>();
		}
		this.animatedRayHitPosition = this.rayHitPosition;
		this.rayHitPosition = this.magnetTarget.InverseTransformPoint(newRayHitPosition);
		this.magnetTargetRigidbody = this.GetHighestParentWithRigidbody(this.magnetTarget).GetComponent<Rigidbody>();
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x00074BA8 File Offset: 0x00072DA8
	private void NewRayHitPoint(Vector3 newAttachPoint, int photonViewId, int colliderID, Transform newMagnetTarget)
	{
		if (GameManager.instance.gameMode == 0)
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

	// Token: 0x06000D58 RID: 3416 RVA: 0x00074C06 File Offset: 0x00072E06
	[PunRPC]
	private void NewRayHitPointRPC(Vector3 newAttachPoint, int photonViewId, int colliderID)
	{
		this.NewRayHitPointLogic(newAttachPoint, photonViewId, colliderID, null);
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x00074C14 File Offset: 0x00072E14
	private bool SphereCheck()
	{
		bool result = false;
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.75f);
		float num = 10000f;
		foreach (Collider collider in array)
		{
			Transform highestParentWithRigidbody = this.GetHighestParentWithRigidbody(collider.transform);
			PhysGrabObjectCollider component = collider.GetComponent<PhysGrabObjectCollider>();
			bool flag = false;
			if (highestParentWithRigidbody != null)
			{
				PhysGrabObjectImpactDetector component2 = highestParentWithRigidbody.GetComponent<PhysGrabObjectImpactDetector>();
				if (component2 != null && component2.isValuable)
				{
					flag = true;
				}
			}
			bool flag2 = true;
			if (this.customTargetingCondition != null && highestParentWithRigidbody != null)
			{
				flag2 = this.customTargetingCondition.CustomTargetingCondition(highestParentWithRigidbody.gameObject);
			}
			bool flag3 = false;
			if (!flag3)
			{
				flag3 = !flag;
			}
			if (component != null && highestParentWithRigidbody != base.transform && highestParentWithRigidbody != null && flag3 && flag2)
			{
				float num2 = Vector3.Distance(base.transform.position, collider.transform.position);
				if (num2 < num)
				{
					bool flag4 = false;
					RaycastHit raycastHit;
					if (Physics.Raycast(base.transform.position, collider.transform.position - base.transform.position, out raycastHit, 1f, SemiFunc.LayerMaskGetVisionObstruct()) && raycastHit.collider.transform != collider.transform && raycastHit.collider.transform != base.transform)
					{
						flag4 = true;
					}
					if (!flag4)
					{
						num = num2;
						this.magnetTarget = collider.transform;
						this.magnetTargetPhysGrabObject = highestParentWithRigidbody.GetComponent<PhysGrabObject>();
						this.magnetTargetRigidbody = highestParentWithRigidbody.GetComponent<Rigidbody>();
						Vector3 position = collider.transform.position;
						this.NewRayHitPoint(position, highestParentWithRigidbody.GetComponent<PhotonView>().ViewID, component.colliderID, highestParentWithRigidbody);
						this.attachPoint = this.rayHitPosition;
						result = true;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x00074E08 File Offset: 0x00073008
	private void FindBeamAttachPosition()
	{
		for (int i = 0; i < 6; i++)
		{
			float num = 0.5f;
			Vector3 b = new Vector3(Random.Range(-num, num), Random.Range(-num, num), Random.Range(-num, num));
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, this.magnetTarget.position - base.transform.position + b, out raycastHit, 1f, SemiFunc.LayerMaskGetPhysGrabObject()))
			{
				Transform highestParentWithRigidbody = this.GetHighestParentWithRigidbody(raycastHit.collider.transform);
				PhysGrabObjectCollider component = raycastHit.collider.transform.GetComponent<PhysGrabObjectCollider>();
				if (component != null && highestParentWithRigidbody == this.magnetTargetPhysGrabObject.transform)
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

	// Token: 0x0400151C RID: 5404
	public UnityEvent upgradeEvent;

	// Token: 0x0400151D RID: 5405
	public bool isPlayerUpgrade;

	// Token: 0x0400151E RID: 5406
	public ColorPresets colorPreset;

	// Token: 0x0400151F RID: 5407
	internal Color beamColor;

	// Token: 0x04001520 RID: 5408
	private float checkTimer;

	// Token: 0x04001521 RID: 5409
	private Transform magnetTarget;

	// Token: 0x04001522 RID: 5410
	internal PhysGrabObject magnetTargetPhysGrabObject;

	// Token: 0x04001523 RID: 5411
	internal Rigidbody magnetTargetRigidbody;

	// Token: 0x04001524 RID: 5412
	internal bool magnetActive;

	// Token: 0x04001525 RID: 5413
	private Rigidbody rb;

	// Token: 0x04001526 RID: 5414
	private bool attachPointFound;

	// Token: 0x04001527 RID: 5415
	private Vector3 attachPoint;

	// Token: 0x04001528 RID: 5416
	private float springConstant = 50f;

	// Token: 0x04001529 RID: 5417
	private float dampingCoefficient = 5f;

	// Token: 0x0400152A RID: 5418
	private float newAttachPointTimer;

	// Token: 0x0400152B RID: 5419
	internal bool itemActivated;

	// Token: 0x0400152C RID: 5420
	private PhotonView photonView;

	// Token: 0x0400152D RID: 5421
	private Vector3 rayHitPosition;

	// Token: 0x0400152E RID: 5422
	private Vector3 animatedRayHitPosition;

	// Token: 0x0400152F RID: 5423
	private LineBetweenTwoPoints lineBetweenTwoPoints;

	// Token: 0x04001530 RID: 5424
	public Transform lineStartPoint;

	// Token: 0x04001531 RID: 5425
	private float rayTimer;

	// Token: 0x04001532 RID: 5426
	private Transform prevMagnetTarget;

	// Token: 0x04001533 RID: 5427
	private Transform droneTransform;

	// Token: 0x04001534 RID: 5428
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04001535 RID: 5429
	private ItemAttributes itemAttributes;

	// Token: 0x04001536 RID: 5430
	private Transform particleEffects;

	// Token: 0x04001537 RID: 5431
	private Transform onSwitchTransform;

	// Token: 0x04001538 RID: 5432
	internal Vector3 connectionPoint;

	// Token: 0x04001539 RID: 5433
	internal Transform lastPlayerToTouch;

	// Token: 0x0400153A RID: 5434
	private PhysGrabObject physGrabObject;

	// Token: 0x0400153B RID: 5435
	private PhysicMaterial physicMaterialOriginal;

	// Token: 0x0400153C RID: 5436
	private ItemToggle itemToggle;

	// Token: 0x0400153D RID: 5437
	internal PlayerAvatar playerAvatarTarget;

	// Token: 0x0400153E RID: 5438
	private bool targetIsPlayer;

	// Token: 0x0400153F RID: 5439
	internal bool targetIsLocalPlayer;

	// Token: 0x04001540 RID: 5440
	private Camera cameraMain;

	// Token: 0x04001541 RID: 5441
	private bool upgradeDone;

	// Token: 0x04001542 RID: 5442
	private bool pushedOrPulled;

	// Token: 0x04001543 RID: 5443
	private ITargetingCondition customTargetingCondition;
}
