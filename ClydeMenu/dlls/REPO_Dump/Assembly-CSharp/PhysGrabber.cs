using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001A6 RID: 422
public class PhysGrabber : MonoBehaviour, IPunObservable
{
	// Token: 0x06000E54 RID: 3668 RVA: 0x00080210 File Offset: 0x0007E410
	private void Start()
	{
		this.minDistanceFromPlayerOriginal = this.minDistanceFromPlayer;
		base.StartCoroutine(this.LateStart());
		this.physGrabBeamScript = this.physGrabBeam.GetComponent<PhysGrabBeam>();
		this.physRotation = Quaternion.identity;
		this.physRotationBase = Quaternion.identity;
		this.mask = SemiFunc.LayerMaskGetVisionObstruct() - LayerMask.GetMask(new string[]
		{
			"Player"
		});
		this.playerAvatar = base.GetComponent<PlayerAvatar>();
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0 || this.photonView.IsMine)
		{
			this.isLocal = true;
			PhysGrabber.instance = this;
		}
		foreach (object obj in this.physGrabPoint)
		{
			Transform transform = (Transform)obj;
			if (transform.name == "Visual1")
			{
				this.physGrabPointVisual1 = transform.gameObject;
				foreach (object obj2 in transform)
				{
					Transform transform2 = (Transform)obj2;
					if (transform2.name == "Visual2")
					{
						this.physGrabPointVisual2 = transform2.gameObject;
					}
				}
			}
			if (transform.name == "Rotate")
			{
				this.physGrabPointVisualRotate = transform;
				transform.GetComponent<PhysGrabPointRotate>().physGrabber = this;
			}
			if (transform.name == "Grid")
			{
				this.physGrabPointVisualGrid = transform;
				foreach (object obj3 in transform)
				{
					Transform transform3 = (Transform)obj3;
					this.physGrabPointVisualGridObject = transform3.gameObject;
					this.physGrabPointVisualGridObject.SetActive(false);
				}
			}
		}
		this.physGrabPoint.SetParent(base.transform.parent, true);
		this.PhysGrabPointDeactivate();
		this.physGrabPointPuller.gameObject.SetActive(false);
		this.physGrabBeam.transform.SetParent(base.transform.parent, false);
		this.physGrabBeam.transform.position = Vector3.zero;
		this.physGrabBeam.transform.rotation = Quaternion.identity;
		this.prevGrabbed = this.grabbed;
		this.grabbed = false;
		this.physGrabBeamAlphaOriginal = this.physGrabBeam.GetComponent<LineRenderer>().material.color.a;
		this.SoundSetup(this.startSound);
		this.SoundSetup(this.loopSound);
		this.SoundSetup(this.stopSound);
		if (!this.isLocal)
		{
			return;
		}
		this.playerCamera = Camera.main;
		PlayerController.instance.physGrabPoint = this.physGrabPoint;
		this.physGrabPointPlane.SetParent(base.transform.parent, false);
		this.physGrabPointPlane.position = Vector3.zero;
		this.physGrabPointPlane.rotation = Quaternion.identity;
		this.physGrabPointPlane.SetParent(CameraAim.Instance.transform, false);
		this.physGrabPointPlane.localPosition = Vector3.zero;
		this.physGrabPointPlane.localRotation = Quaternion.identity;
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x000805A4 File Offset: 0x0007E7A4
	private void OnDestroy()
	{
		Object.Destroy(this.physGrabBeam);
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x000805B4 File Offset: 0x0007E7B4
	public void PhysGrabOverCharge(float _amount, float _multiplier = 1f)
	{
		if (PlayerController.instance.DebugDisableOvercharge)
		{
			return;
		}
		if (this.physGrabBeamOverChargeAmount == 0f && this.physGrabBeamOverchargeInitialBoostCooldown <= 0f)
		{
			this.physGrabBeamOverchargeInitialBoostCooldown = 1f;
			this.physGrabBeamOverChargeFloat += 0.1f * _multiplier;
		}
		this.physGrabBeamOverChargeAmount = _amount;
		this.physGrabBeamOverChargeTimer = 0.2f;
		this.physGrabBeamOverchargeDecreaseCooldown = 1.5f;
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x00080624 File Offset: 0x0007E824
	private void PhysGrabOverChargeImpact()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("PhysGrabOverChargeImpactRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.PhysGrabOverChargeImpactRPC(default(PhotonMessageInfo));
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x0008065E File Offset: 0x0007E85E
	[PunRPC]
	public void PhysGrabOverChargeImpactRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.OwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.physGrabBeam.GetComponent<PhysGrabBeam>().OverChargeLaunchPlayer();
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x00080680 File Offset: 0x0007E880
	private void PhysGrabOverChargeLogic()
	{
		if (this.physGrabBeamOverchargeInitialBoostCooldown > 0f)
		{
			this.physGrabBeamOverchargeInitialBoostCooldown -= Time.deltaTime;
		}
		if (this.physGrabBeamOverChargeFloat > 1f)
		{
			this.physGrabBeamOverChargeFloat = 1f;
		}
		if (!this.isLocal)
		{
			return;
		}
		if (!this.grabbed)
		{
			this.currentGrabForce = Vector3.zero;
			this.currentTorqueForce = Vector3.zero;
			this.physGrabBeamOverChargeAmount = 0f;
			this.physGrabBeamOverChargeTimer = 0f;
		}
		if (this.physGrabBeamOverChargeTimer <= 0f)
		{
			this.physGrabBeamOverChargeAmount = 0f;
		}
		if (this.physGrabBeamOverChargeTimer > 0f)
		{
			this.physGrabBeamOverChargeFloat += this.physGrabBeamOverChargeAmount * Time.deltaTime;
			if (this.physGrabBeamOverChargeFloat >= 1f)
			{
				this.physGrabBeamOverChargeFloat = 1f;
				this.OverrideGrabRelease();
				this.PhysGrabOverChargeImpact();
				this.physGrabBeamOverChargeTimer = 0f;
			}
			this.physGrabBeamOverCharge = (byte)(this.physGrabBeamOverChargeFloat * 200f);
			this.physGrabBeamOverChargeTimer -= Time.deltaTime;
			return;
		}
		if (this.physGrabBeamOverChargeFloat <= 0f)
		{
			if (this.physGrabBeamOverCharge > 0)
			{
				this.physGrabBeamOverCharge = 0;
			}
			return;
		}
		if (this.physGrabBeamOverchargeDecreaseCooldown > 0f)
		{
			this.physGrabBeamOverchargeDecreaseCooldown -= Time.deltaTime;
			return;
		}
		this.physGrabBeamOverChargeFloat -= 0.1f * Time.deltaTime;
		this.physGrabBeamOverCharge = (byte)(this.physGrabBeamOverChargeFloat * 200f);
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x000807FD File Offset: 0x0007E9FD
	public void OverrideGrabDistance(float dist)
	{
		this.prevPullerDistance = this.pullerDistance;
		this.pullerDistance = dist;
		this.overrideGrabDistance = dist;
		this.overrideGrabDistanceTimer = 0.1f;
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x00080824 File Offset: 0x0007EA24
	public void OverrideMinimumGrabDistance(float dist)
	{
		this.overrideMinimumGrabDistance = dist;
		this.overrideMinimumGrabDistanceTimer = 0.1f;
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x00080838 File Offset: 0x0007EA38
	private void OverrideGrabDistanceTick()
	{
		if (this.overrideGrabDistanceTimer > 0f)
		{
			this.overrideGrabDistanceTimer -= Time.deltaTime;
		}
		else if (this.overrideGrabDistanceTimer != -123f)
		{
			this.overrideGrabDistance = 0f;
			this.overrideGrabDistanceTimer = -123f;
		}
		if (this.overrideMinimumGrabDistanceTimer > 0f)
		{
			this.overrideMinimumGrabDistanceTimer -= Time.deltaTime;
			return;
		}
		if (this.overrideMinimumGrabDistanceTimer != -123f)
		{
			this.overrideMinimumGrabDistance = 0f;
			this.minDistanceFromPlayer = this.minDistanceFromPlayerOriginal;
			this.overrideMinimumGrabDistanceTimer = -123f;
		}
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x000808D8 File Offset: 0x0007EAD8
	private IEnumerator LateStart()
	{
		while (!this.playerAvatar)
		{
			yield return new WaitForSeconds(0.2f);
		}
		string _steamID = SemiFunc.PlayerGetSteamID(this.playerAvatar);
		yield return new WaitForSeconds(0.2f);
		while (!StatsManager.instance.playerUpgradeStrength.ContainsKey(_steamID))
		{
			yield return new WaitForSeconds(0.2f);
		}
		if (!SemiFunc.MenuLevel())
		{
			this.grabStrength += (float)StatsManager.instance.playerUpgradeStrength[_steamID] * 0.2f;
			this.throwStrength += (float)StatsManager.instance.playerUpgradeThrow[_steamID] * 0.3f;
			this.grabRange += (float)StatsManager.instance.playerUpgradeRange[_steamID] * 1f;
		}
		yield break;
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x000808E8 File Offset: 0x0007EAE8
	public void SoundSetup(Sound _sound)
	{
		if (!SemiFunc.IsMultiplayer() || this.photonView.IsMine)
		{
			_sound.SpatialBlend = 0f;
			return;
		}
		_sound.Volume *= 0.5f;
		_sound.VolumeRandom *= 0.5f;
		_sound.SpatialBlend = 1f;
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x00080944 File Offset: 0x0007EB44
	public void OverrideDisableRotationControls()
	{
		this.overrideDisableRotationControls = true;
		this.overrideDisableRotationControlsTimer = 0.1f;
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x00080958 File Offset: 0x0007EB58
	private void OverrideDisableRotationControlsTick()
	{
		if (this.overrideDisableRotationControlsTimer > 0f)
		{
			this.overrideDisableRotationControlsTimer -= Time.fixedDeltaTime;
			if (this.overrideDisableRotationControlsTimer <= 0f)
			{
				this.overrideDisableRotationControls = false;
			}
		}
	}

	// Token: 0x06000E61 RID: 3681 RVA: 0x0008098D File Offset: 0x0007EB8D
	public void OverrideGrab(PhysGrabObject target)
	{
		this.overrideGrab = true;
		this.overrideGrabTarget = target;
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x0008099D File Offset: 0x0007EB9D
	public void OverrideGrabPoint(Transform grabPoint)
	{
		this.overrideGrabPointTransform = grabPoint;
		this.overrideGrabPointTimer = 0.1f;
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x000809B1 File Offset: 0x0007EBB1
	public void OverrideGrabRelease()
	{
		this.overrideGrabRelease = true;
		this.overrideGrab = false;
		this.overrideGrabTarget = null;
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x000809C8 File Offset: 0x0007EBC8
	public void GrabberHeal()
	{
		if (!this.healing)
		{
			this.photonView.RPC("HealStart", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x000809E8 File Offset: 0x0007EBE8
	private void ColorStateSetColor(Color mainColor, Color emissionColor)
	{
		Material material = this.physGrabBeam.GetComponent<LineRenderer>().material;
		Material material2 = this.physGrabPointVisual1.GetComponent<MeshRenderer>().material;
		Material material3 = this.physGrabPointVisual2.GetComponent<MeshRenderer>().material;
		Material material4 = this.physGrabPointVisualRotate.GetComponent<MeshRenderer>().material;
		Light grabberLight = this.playerAvatar.playerAvatarVisuals.playerAvatarRightArm.grabberLight;
		Material material5 = this.playerAvatar.playerAvatarVisuals.playerAvatarRightArm.grabberOrbSpheres[0].GetComponent<MeshRenderer>().material;
		Material material6 = this.playerAvatar.playerAvatarVisuals.playerAvatarRightArm.grabberOrbSpheres[1].GetComponent<MeshRenderer>().material;
		if (material)
		{
			material.color = mainColor;
		}
		if (material)
		{
			material.SetColor("_EmissionColor", emissionColor);
		}
		if (material2)
		{
			material2.color = mainColor;
		}
		if (material2)
		{
			material2.SetColor("_EmissionColor", emissionColor);
		}
		if (material3)
		{
			material3.color = mainColor;
		}
		if (material3)
		{
			material3.SetColor("_EmissionColor", emissionColor);
		}
		if (material4)
		{
			material4.color = mainColor;
		}
		if (material4)
		{
			material4.SetColor("_EmissionColor", emissionColor);
		}
		if (grabberLight)
		{
			grabberLight.color = mainColor;
		}
		if (material5)
		{
			material5.color = mainColor;
		}
		if (material5)
		{
			material5.SetColor("_EmissionColor", emissionColor);
		}
		if (material6)
		{
			material6.color = mainColor;
		}
		if (material6)
		{
			material6.SetColor("_EmissionColor", emissionColor);
		}
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x00080B81 File Offset: 0x0007ED81
	public void OverrideColorToGreen(float time = 0.1f)
	{
		this.colorState = 1;
		this.colorStateOverrideTimer = time;
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x00080B91 File Offset: 0x0007ED91
	public void OverrideColorToPurple(float time = 0.1f)
	{
		this.colorState = 2;
		this.colorStateOverrideTimer = time;
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x00080BA4 File Offset: 0x0007EDA4
	private void ColorStates()
	{
		if (this.prevColorState == this.colorState)
		{
			return;
		}
		this.prevColorState = this.colorState;
		Color mainColor = new Color(1f, 0.1856f, 0f, 0.15f);
		Color emissionColor = new Color(1f, 0.1856f, 0f, 1f);
		if (this.colorState == 0)
		{
			if (!VideoGreenScreen.instance)
			{
				mainColor = new Color(1f, 0.1856f, 0f, 0.15f);
			}
			else
			{
				mainColor = new Color(1f, 0.1856f, 0f, 1f);
			}
			emissionColor = new Color(1f, 0.1856f, 0f, 1f);
			this.ColorStateSetColor(mainColor, emissionColor);
			return;
		}
		if (this.colorState == 1)
		{
			if (!VideoGreenScreen.instance)
			{
				mainColor = new Color(0f, 1f, 0f, 0.15f);
			}
			else
			{
				mainColor = new Color(0f, 1f, 0f, 1f);
			}
			emissionColor = new Color(0f, 1f, 0f, 1f);
			this.ColorStateSetColor(mainColor, emissionColor);
			return;
		}
		if (this.colorState == 2)
		{
			if (!VideoGreenScreen.instance)
			{
				mainColor = new Color(1f, 0f, 1f, 0.15f);
			}
			else
			{
				mainColor = new Color(1f, 0f, 1f, 1f);
			}
			emissionColor = new Color(1f, 0f, 1f, 1f);
			this.ColorStateSetColor(mainColor, emissionColor);
		}
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x00080D53 File Offset: 0x0007EF53
	private void ColorStateTick()
	{
		if (this.colorStateOverrideTimer > 0f)
		{
			this.colorStateOverrideTimer -= Time.fixedDeltaTime;
			return;
		}
		this.colorState = 0;
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x00080D7C File Offset: 0x0007EF7C
	[PunRPC]
	private void HealStart()
	{
		this.physGrabBeam.GetComponent<LineRenderer>().material = this.physGrabBeamMaterialBatteryCharge;
		this.physGrabPointVisual1.GetComponent<MeshRenderer>().material = this.physGrabBeamMaterialBatteryCharge;
		this.physGrabPointVisual2.GetComponent<MeshRenderer>().material = this.physGrabBeamMaterialBatteryCharge;
		this.physGrabBeam.GetComponent<PhysGrabBeam>().scrollSpeed = new Vector2(-5f, 0f);
		this.physGrabBeam.GetComponent<PhysGrabBeam>().lineMaterial = this.physGrabBeam.GetComponent<LineRenderer>().material;
		this.healing = true;
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x00080E14 File Offset: 0x0007F014
	private void ResetBeam()
	{
		if (this.healing)
		{
			this.physGrabBeam.GetComponent<LineRenderer>().material = this.physGrabBeamMaterial;
			this.physGrabPointVisual1.GetComponent<MeshRenderer>().material = this.physGrabBeamMaterial;
			this.physGrabPointVisual2.GetComponent<MeshRenderer>().material = this.physGrabBeamMaterial;
			this.physGrabBeam.GetComponent<PhysGrabBeam>().scrollSpeed = this.physGrabBeam.GetComponent<PhysGrabBeam>().originalScrollSpeed;
			this.physGrabBeam.GetComponent<PhysGrabBeam>().lineMaterial = this.physGrabBeam.GetComponent<LineRenderer>().material;
			this.healing = false;
		}
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x00080EB5 File Offset: 0x0007F0B5
	public void ChangeBeamAlpha(float alpha)
	{
		if (this.physGramBeamAlphaTimer == -123f)
		{
			this.physGrabBeamAlpha = this.physGrabBeamAlphaOriginal;
		}
		this.physGrabBeamAlphaChangeTo = alpha;
		this.physGramBeamAlphaTimer = 0.1f;
		this.physGrabBeamAlphaChangeProgress = 0f;
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x00080EF0 File Offset: 0x0007F0F0
	private void TickerBeamAlphaChange()
	{
		if (this.physGramBeamAlphaTimer > 0f)
		{
			this.physGrabBeamAlpha = Mathf.Lerp(this.physGrabBeamAlpha, this.physGrabBeamAlphaChangeTo, this.physGrabBeamAlphaChangeProgress);
			if (this.physGrabBeamAlphaChangeProgress < 1f)
			{
				this.physGrabBeamAlphaChangeProgress += 4f * Time.deltaTime;
				Material material = this.physGrabBeam.GetComponent<LineRenderer>().material;
				material.SetColor("_Color", new Color(material.color.r, material.color.g, material.color.b, this.physGrabBeamAlpha));
				Material material2 = this.physGrabPointVisual1.GetComponent<MeshRenderer>().material;
				Material material3 = this.physGrabPointVisual2.GetComponent<MeshRenderer>().material;
				material2.SetColor("_Color", new Color(material2.color.r, material2.color.g, material2.color.b, this.physGrabBeamAlpha));
				material3.SetColor("_Color", new Color(material3.color.r, material3.color.g, material3.color.b, this.physGrabBeamAlpha));
			}
		}
		else if (this.physGramBeamAlphaTimer != -123f)
		{
			this.physGrabBeamAlphaChangeProgress = 0f;
			Material material4 = this.physGrabBeam.GetComponent<LineRenderer>().material;
			material4.SetColor("_Color", new Color(material4.color.r, material4.color.g, material4.color.b, this.physGrabBeamAlphaOriginal));
			Material material5 = this.physGrabPointVisual1.GetComponent<MeshRenderer>().material;
			Material material6 = this.physGrabPointVisual2.GetComponent<MeshRenderer>().material;
			material5.SetColor("_Color", new Color(material5.color.r, material5.color.g, material5.color.b, this.physGrabBeamAlphaOriginal));
			material6.SetColor("_Color", new Color(material6.color.r, material6.color.g, material6.color.b, this.physGrabBeamAlphaOriginal));
			this.physGramBeamAlphaTimer = -123f;
		}
		if (this.physGramBeamAlphaTimer > 0f)
		{
			this.physGramBeamAlphaTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x00081158 File Offset: 0x0007F358
	public Quaternion GetRotationInput()
	{
		Quaternion rhs = Quaternion.AngleAxis(this.mouseTurningVelocity.y, Vector3.right);
		Quaternion lhs = Quaternion.AngleAxis(-this.mouseTurningVelocity.x, Vector3.up);
		Quaternion rhs2 = Quaternion.AngleAxis(this.mouseTurningVelocity.z, Vector3.forward);
		return lhs * rhs * rhs2;
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x000811B4 File Offset: 0x0007F3B4
	private void ObjectTurning()
	{
		if (!this.grabbedPhysGrabObject)
		{
			return;
		}
		if (!this.grabbed)
		{
			this.mouseTurningVelocity = Vector3.zero;
			this.physGrabPointVisualGrid.gameObject.SetActive(false);
			this.isRotating = false;
			return;
		}
		float num = Mathf.Max(this.grabbedPhysGrabObject.rb.mass, 1f);
		if (this.physGrabPointVisualGrid && this.grabbedPhysGrabObject)
		{
			this.physGrabPointVisualGrid.position = this.grabbedPhysGrabObject.midPoint;
		}
		if (this.mouseTurningVelocity.magnitude > 0.01f)
		{
			if (this.isRotating)
			{
				float t = 1f * Time.deltaTime;
				this.mouseTurningVelocity = Vector3.Lerp(this.mouseTurningVelocity, Vector3.zero, t);
			}
			else
			{
				float t2 = 10f * Time.deltaTime;
				this.mouseTurningVelocity = Vector3.Lerp(this.mouseTurningVelocity, Vector3.zero, t2);
			}
		}
		else
		{
			this.mouseTurningVelocity = Vector3.zero;
		}
		this.cameraRelativeGrabbedForward = this.cameraRelativeGrabbedForward.normalized;
		this.cameraRelativeGrabbedUp = this.cameraRelativeGrabbedUp.normalized;
		bool flag = false;
		if (this.isLocal && SemiFunc.InputHold(InputKey.Rotate))
		{
			flag = true;
		}
		if (flag)
		{
			float axis = Input.GetAxis("Mouse X");
			float num2 = Input.GetAxis("Mouse Y");
			float num3 = Mathf.Lerp(0.1f, 1f, num * 0.05f);
			float x = axis * num3;
			num2 *= num3;
			Vector3 a = new Vector3(x, num2, 0f) * 15f * this.grabStrength;
			if (this.grabbedPhysGrabObject.impactDetector.isCart)
			{
				a *= 0.25f;
			}
			if (a.magnitude != 0f)
			{
				this.mouseTurningVelocity += a * 15f * Time.deltaTime;
			}
			this.mouseTurningVelocity = Vector3.ClampMagnitude(this.mouseTurningVelocity, 70f);
			if (this.isLocal)
			{
				this.isRotatingTimer = 0.1f;
			}
		}
		if (this.isRotating)
		{
			this.physGrabPointVisualGrid.gameObject.SetActive(true);
			Transform localCameraTransform = this.playerAvatar.localCameraTransform;
			if (this.physRotatingTimer <= 0f)
			{
				this.physRotatingTimer = 0.25f;
				this.cameraRelativeGrabbedForward = localCameraTransform.InverseTransformDirection(this.grabbedObjectTransform.forward);
				this.cameraRelativeGrabbedUp = localCameraTransform.InverseTransformDirection(this.grabbedObjectTransform.up);
				this.physGrabPointVisualGrid.rotation = this.grabbedObjectTransform.rotation;
			}
			this.physRotatingTimer = 0.25f;
			float num4 = Mathf.Clamp(1f / num, 0f, 0.5f);
			if (num4 != 0f)
			{
				this.grabbedPhysGrabObject.OverrideAngularDrag(40f * num4, 0.1f);
			}
			Quaternion rhs = Quaternion.AngleAxis(this.mouseTurningVelocity.y, localCameraTransform.right);
			Quaternion lhs = Quaternion.AngleAxis(-this.mouseTurningVelocity.x, localCameraTransform.up);
			Quaternion rhs2 = Quaternion.AngleAxis(this.mouseTurningVelocity.z, localCameraTransform.forward);
			Quaternion quaternion = lhs * rhs * rhs2;
			quaternion = Quaternion.Slerp(Quaternion.identity, quaternion, Time.deltaTime * 20f);
			float num5 = 1f / num;
			num5 = Mathf.Clamp(num5, 0f, 1f);
			quaternion = Quaternion.Slerp(Quaternion.identity, quaternion, num5);
			this.physGrabPointVisualGrid.rotation = quaternion * this.physGrabPointVisualGrid.rotation;
			Quaternion rotation = this.grabbedObjectTransform.rotation;
			Quaternion quaternion2 = this.physGrabPointVisualGrid.rotation;
			float num6 = Quaternion.Angle(rotation, quaternion2);
			float num7 = 45f;
			if (num6 > num7)
			{
				float t3 = num7 / num6;
				quaternion2 = Quaternion.Slerp(rotation, quaternion2, t3);
			}
			this.physGrabPointVisualGrid.rotation = quaternion2;
			this.cameraRelativeGrabbedForward = localCameraTransform.InverseTransformDirection(this.grabbedObjectTransform.forward);
			this.cameraRelativeGrabbedUp = localCameraTransform.InverseTransformDirection(this.grabbedObjectTransform.up);
			foreach (PhysGrabber physGrabber in this.grabbedPhysGrabObject.playerGrabbing)
			{
				Transform localCameraTransform2 = physGrabber.playerAvatar.localCameraTransform;
				physGrabber.cameraRelativeGrabbedForward = localCameraTransform2.InverseTransformDirection(this.physGrabPointVisualGrid.forward);
				physGrabber.cameraRelativeGrabbedUp = localCameraTransform2.InverseTransformDirection(this.physGrabPointVisualGrid.up);
			}
			float t4 = 2f * Time.deltaTime;
			this.physGrabPointVisualGrid.transform.rotation = Quaternion.Slerp(this.physGrabPointVisualGrid.transform.rotation, this.grabbedObjectTransform.rotation, t4);
			return;
		}
		this.physGrabPointVisualGrid.gameObject.SetActive(false);
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x000816A8 File Offset: 0x0007F8A8
	private void OverrideGrabPointTimer()
	{
		if (this.overrideGrabPointTimer > 0f)
		{
			this.overrideGrabPointTimer -= Time.fixedDeltaTime;
			return;
		}
		this.overrideGrabPointTransform = null;
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x000816D4 File Offset: 0x0007F8D4
	private void FixedUpdate()
	{
		this.OverrideGrabPointTimer();
		this.OverrideDisableRotationControlsTick();
		if (this.isLocal)
		{
			if (this.grabbedPhysGrabObject)
			{
				bool isMelee = this.grabbedPhysGrabObject.isMelee;
			}
			if (!this.overrideDisableRotationControls)
			{
				if (this.isRotatingTimer > 0f)
				{
					SemiFunc.CameraOverrideStopAim();
					if (!this.isRotating && this.grabbedObjectTransform)
					{
						Transform localCameraTransform = this.playerAvatar.localCameraTransform;
						this.mouseTurningVelocity = Vector3.zero;
					}
					this.isRotating = true;
				}
				else
				{
					this.isRotating = false;
				}
			}
		}
		if (this.stopRotationTimer > 0f)
		{
			this.stopRotationTimer -= Time.fixedDeltaTime;
		}
		this.ColorStateTick();
	}

	// Token: 0x06000E72 RID: 3698 RVA: 0x0008178C File Offset: 0x0007F98C
	private void PushingPullingChecker()
	{
		if (this.overrideGrabDistanceTimer > 0f)
		{
			this.pullerDistance = this.overrideGrabDistance;
			this.prevPullerDistance = this.pullerDistance;
		}
		if (this.overrideMinimumGrabDistanceTimer > 0f && this.minDistanceFromPlayer < this.overrideMinimumGrabDistance)
		{
			this.minDistanceFromPlayer = this.overrideMinimumGrabDistance;
		}
		if (!this.grabbed)
		{
			this.isPushing = false;
			this.isPulling = false;
			this.isPushingTimer = 0f;
			this.isPullingTimer = 0f;
			this.prevPullerDistance = this.pullerDistance;
			return;
		}
		if (this.initialPressTimer > 0f)
		{
			this.prevPullerDistance = this.pullerDistance;
			this.isPushingTimer = 0f;
		}
		if (InputManager.instance.KeyPullAndPush() > 0f)
		{
			this.isPushingTimer = 0.1f;
		}
		if (InputManager.instance.KeyPullAndPush() < 0f)
		{
			this.isPullingTimer = 0.1f;
		}
		if (this.isPushingTimer > 0f)
		{
			TutorialDirector.instance.playerPushedAndPulled = true;
			this.isPushing = true;
			this.isPushingTimer -= Time.deltaTime;
		}
		else
		{
			this.isPushing = false;
		}
		if (this.isPullingTimer > 0f)
		{
			TutorialDirector.instance.playerPushedAndPulled = true;
			this.isPulling = true;
			this.isPullingTimer -= Time.deltaTime;
		}
		else
		{
			this.isPulling = false;
		}
		this.prevPullerDistance = this.pullerDistance;
		if (this.overrideGrabDistanceTimer > 0f)
		{
			this.pullerDistance = this.overrideGrabDistance;
			this.prevPullerDistance = this.pullerDistance;
		}
	}

	// Token: 0x06000E73 RID: 3699 RVA: 0x0008191F File Offset: 0x0007FB1F
	public void OverridePullDistanceIncrement(float distSpeed)
	{
		this.physGrabPointPlane.position += this.playerCamera.transform.forward * distSpeed;
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x0008194D File Offset: 0x0007FB4D
	private void OverchargeResetValues()
	{
		this.physGrabBeamOverCharge = 0;
		this.physGrabBeamOverChargeFloat = 0f;
		this.physGrabBeamOverChargeAmount = 0f;
		this.physGrabBeamOverChargeTimer = 0f;
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x00081977 File Offset: 0x0007FB77
	private void OnDisable()
	{
		this.OverchargeResetValues();
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x0008197F File Offset: 0x0007FB7F
	private void OnEnable()
	{
		this.OverchargeResetValues();
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x00081988 File Offset: 0x0007FB88
	private void Update()
	{
		if (this.isRotatingTimer > 0f)
		{
			this.isRotatingTimer -= Time.deltaTime;
		}
		this.PushingPullingChecker();
		this.ColorStates();
		this.ObjectTurning();
		this.PhysGrabOverChargeLogic();
		if (this.grabbedObjectTransform && this.grabbedObjectTransform.name == this.playerAvatar.healthGrab.name)
		{
			this.OverrideColorToGreen(0.1f);
		}
		this.OverrideGrabDistanceTick();
		this.TickerBeamAlphaChange();
		if (this.initialPressTimer > 0f)
		{
			this.initialPressTimer -= Time.deltaTime;
		}
		if (this.physRotatingTimer > 0f)
		{
			this.physRotatingTimer -= Time.deltaTime;
		}
		if (this.grabbed && this.grabbedObjectTransform)
		{
			if (!this.overrideGrabPointTransform)
			{
				this.physGrabPoint.position = this.grabbedObjectTransform.TransformPoint(this.localGrabPosition);
			}
			else
			{
				this.physGrabPoint.position = this.overrideGrabPointTransform.position;
			}
		}
		if (this.isLocal)
		{
			if (this.grabbedPhysGrabObject)
			{
				bool isMelee = this.grabbedPhysGrabObject.isMelee;
			}
			if (!SemiFunc.InputHold(InputKey.Rotate))
			{
				if (InputManager.instance.KeyPullAndPush() > 0f && Vector3.Distance(this.physGrabPointPuller.position, this.playerCamera.transform.position) < this.grabRange)
				{
					this.physGrabPointPlane.position += this.playerCamera.transform.forward * 0.2f;
				}
				if (InputManager.instance.KeyPullAndPush() < 0f && Vector3.Distance(this.physGrabPointPuller.position, this.playerCamera.transform.position) > this.minDistanceFromPlayer)
				{
					this.physGrabPointPlane.position -= this.playerCamera.transform.forward * 0.2f;
				}
			}
			if (this.overrideGrabDistanceTimer < 0f)
			{
				this.pullerDistance = Vector3.Distance(this.physGrabPointPuller.position, this.playerCamera.transform.position);
			}
			if (this.overrideGrabDistance > 0f)
			{
				Transform visionTransform = this.playerAvatar.PlayerVisionTarget.VisionTransform;
				this.physGrabPointPlane.position = visionTransform.position + visionTransform.forward * this.overrideGrabDistance;
			}
			else
			{
				if (this.pullerDistance < this.minDistanceFromPlayer)
				{
					this.physGrabPointPuller.position = this.playerCamera.transform.position + this.playerCamera.transform.forward * this.minDistanceFromPlayer;
				}
				if (this.pullerDistance > this.maxDistanceFromPlayer)
				{
					this.physGrabPointPuller.position = this.playerCamera.transform.position + this.playerCamera.transform.forward * this.maxDistanceFromPlayer;
				}
			}
		}
		else if (this.overrideGrabDistanceTimer <= 0f)
		{
			this.pullerDistance = Vector3.Distance(this.physGrabPointPuller.position, this.playerAvatar.localCameraPosition);
		}
		this.grabberAudioTransform.position = this.physGrabBeamComponent.PhysGrabPointOrigin.position;
		this.loopSound.PlayLoop(this.physGrabBeamScript.lineRenderer.enabled, 10f, 10f, 1f);
		if (!this.isLocal)
		{
			return;
		}
		this.ShowValue();
		bool flag = SemiFunc.InputHold(InputKey.Grab) || this.toggleGrab;
		if (this.debugStickyGrabber && !SemiFunc.InputHold(InputKey.Rotate))
		{
			flag = true;
		}
		if (InputManager.instance.InputToggleGet(InputKey.Grab))
		{
			if (SemiFunc.InputDown(InputKey.Grab))
			{
				this.toggleGrab = !this.toggleGrab;
				if (this.toggleGrab)
				{
					this.toggleGrabTimer = 0.1f;
				}
			}
		}
		else
		{
			this.toggleGrab = false;
		}
		if (this.toggleGrabTimer > 0f)
		{
			this.toggleGrabTimer -= Time.deltaTime;
		}
		else if (!this.grabbed && this.toggleGrab)
		{
			this.toggleGrab = false;
		}
		if (this.overrideGrab && (SemiFunc.InputHold(InputKey.Grab) || this.toggleGrab))
		{
			this.overrideGrab = false;
			this.overrideGrabTarget = null;
		}
		if (this.overrideGrab)
		{
			flag = true;
		}
		if (this.overrideGrabRelease)
		{
			flag = false;
			this.overrideGrabRelease = false;
		}
		if (PlayerController.instance.InputDisableTimer > 0f)
		{
			flag = false;
		}
		bool flag2 = false;
		if (flag && !this.grabbed)
		{
			if (this.grabDisableTimer <= 0f)
			{
				flag2 = true;
			}
		}
		else if (!flag && this.grabbed)
		{
			this.ReleaseObject(0.1f);
		}
		if (LevelGenerator.Instance.Generated && PlayerController.instance.InputDisableTimer <= 0f)
		{
			if (this.grabCheckTimer <= 0f || flag2)
			{
				this.grabCheckTimer = 0.02f;
				this.RayCheck(flag2);
			}
			else
			{
				this.grabCheckTimer -= Time.deltaTime;
			}
		}
		this.PhysGrabLogic();
		if (this.grabDisableTimer > 0f)
		{
			this.grabDisableTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x00081EE4 File Offset: 0x000800E4
	private void PhysGrabLogic()
	{
		this.grabReleaseDistance = Mathf.Max(this.grabRange * 2f, 10f);
		if (this.grabbed)
		{
			if (this.physRotatingTimer > 0f)
			{
				Aim.instance.SetState(Aim.State.Rotate);
			}
			else
			{
				Aim.instance.SetState(Aim.State.Grab);
			}
			if (Vector3.Distance(this.physGrabPoint.position, this.playerCamera.transform.position) > this.grabReleaseDistance)
			{
				this.ReleaseObject(0.1f);
				return;
			}
			if (this.grabbedPhysGrabObject)
			{
				if (!this.grabbedPhysGrabObject.enabled || this.grabbedPhysGrabObject.dead || !this.grabbedPhysGrabObjectCollider || !this.grabbedPhysGrabObjectCollider.enabled)
				{
					this.ReleaseObject(0.1f);
					return;
				}
			}
			else
			{
				if (!this.grabbedStaticGrabObject)
				{
					this.ReleaseObject(0.1f);
					return;
				}
				if (!this.grabbedStaticGrabObject.isActiveAndEnabled || this.grabbedStaticGrabObject.dead)
				{
					this.ReleaseObject(0.1f);
					return;
				}
			}
			this.physGrabPointPullerPosition = this.physGrabPointPuller.position;
			this.PhysGrabStarted();
			this.PhysGrabBeamActivate();
		}
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x0008201C File Offset: 0x0008021C
	private void PhysGrabBeamActivate()
	{
		if (GameManager.instance.gameMode == 0)
		{
			if (!this.physGrabBeamActive)
			{
				this.physGrabBeamScript.lineRenderer.enabled = true;
				this.physGrabForcesDisabled = false;
				this.physGrabBeamComponent.physGrabPointPullerSmoothPosition = this.physGrabPoint.position;
				this.physGrabBeamActive = true;
				this.PhysGrabStartEffects();
				return;
			}
		}
		else if (!this.physGrabBeamActive)
		{
			this.photonView.RPC("PhysGrabBeamActivateRPC", RpcTarget.All, Array.Empty<object>());
			this.physGrabBeamActive = true;
		}
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x000820A0 File Offset: 0x000802A0
	public void ShowValue()
	{
		if (this.grabbed && this.grabbedPhysGrabObject)
		{
			ValuableObject component = this.grabbedPhysGrabObject.GetComponent<ValuableObject>();
			if (component)
			{
				WorldSpaceUIValue.instance.Show(this.grabbedPhysGrabObject, (int)component.dollarValueCurrent, false, Vector3.zero);
				return;
			}
			if (SemiFunc.RunIsShop())
			{
				ItemAttributes component2 = this.grabbedPhysGrabObject.GetComponent<ItemAttributes>();
				if (component2)
				{
					WorldSpaceUIValue.instance.Show(this.grabbedPhysGrabObject, component2.value, true, component2.costOffset);
				}
			}
		}
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x0008212C File Offset: 0x0008032C
	private void PhysGrabStartEffects()
	{
		this.startSound.Play(this.loopSound.Source.transform.position, 1f, 1f, 1f, 1f);
		if (!GameManager.Multiplayer() || this.photonView.IsMine)
		{
			GameDirector.instance.CameraImpact.Shake(0.5f, 0.1f);
		}
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x0008219C File Offset: 0x0008039C
	private void PhysGrabEndEffects()
	{
		this.stopSound.Play(this.loopSound.Source.transform.position, 1f, 1f, 1f, 1f);
		if (!GameManager.Multiplayer() || this.photonView.IsMine)
		{
			GameDirector.instance.CameraImpact.Shake(0.5f, 0.1f);
		}
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0008220C File Offset: 0x0008040C
	[PunRPC]
	private void PhysGrabBeamActivateRPC()
	{
		this.PhysGrabStartEffects();
		this.initialPressTimer = 0.1f;
		this.physGrabForcesDisabled = false;
		this.physGrabBeamScript.lineRenderer.enabled = true;
		this.physGrabBeamComponent.physGrabPointPullerSmoothPosition = this.physGrabPoint.position;
		this.physGrabBeamActive = true;
		this.physGrabPointVisualRotate.GetComponent<PhysGrabPointRotate>().animationEval = 0f;
		this.PhysGrabPointActivate();
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x0008227C File Offset: 0x0008047C
	private void PhysGrabPointDeactivate()
	{
		this.physGrabPointVisualGrid.parent = this.physGrabPoint;
		this.physGrabPointVisualRotate.localScale = Vector3.zero;
		this.physGrabPointVisualRotate.GetComponent<PhysGrabPointRotate>().animationEval = 0f;
		this.GridObjectsRemove();
		this.physGrabPoint.gameObject.SetActive(false);
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x000822D8 File Offset: 0x000804D8
	private void PhysGrabPointActivate()
	{
		if (!this.grabbedObjectTransform)
		{
			return;
		}
		this.physGrabPointVisualRotate.localScale = Vector3.zero;
		PhysGrabPointRotate component = this.physGrabPointVisualRotate.GetComponent<PhysGrabPointRotate>();
		if (component)
		{
			component.animationEval = 0f;
			component.rotationActiveTimer = 0f;
		}
		this.physGrabPointVisualGrid.localPosition = Vector3.zero;
		this.physGrabPointVisualGrid.parent = base.transform.parent;
		this.physGrabPointVisualGrid.localScale = Vector3.one;
		this.grabbedPhysGrabObject = this.grabbedObjectTransform.GetComponent<PhysGrabObject>();
		if (this.grabbedPhysGrabObject)
		{
			this.physGrabPointVisualGrid.localRotation = this.grabbedPhysGrabObject.rb.rotation;
		}
		if (this.grabbedPhysGrabObject)
		{
			this.GridObjectsInstantiate();
		}
		this.physGrabPointVisualGrid.gameObject.SetActive(false);
		this.physGrabPoint.gameObject.SetActive(true);
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x000823D1 File Offset: 0x000805D1
	[PunRPC]
	private void PhysGrabBeamDeactivateRPC()
	{
		this.physGrabForcesDisabled = false;
		this.ResetBeam();
		this.physGrabBeamScript.lineRenderer.enabled = false;
		this.PhysGrabPointDeactivate();
		this.physGrabBeamActive = false;
		this.PhysGrabEndEffects();
		this.physRotation = Quaternion.identity;
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x0008240F File Offset: 0x0008060F
	private void PhysGrabBeamDeactivate()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.PhysGrabBeamDeactivateRPC();
			return;
		}
		this.photonView.RPC("PhysGrabBeamDeactivateRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x00082438 File Offset: 0x00080638
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(info, info.photonView))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.physGrabPointPullerPosition);
			stream.SendNext(this.physGrabPointPlane.position);
			stream.SendNext(this.mouseTurningVelocity);
			stream.SendNext(this.isRotating);
			stream.SendNext(this.colorState);
			stream.SendNext(this.physGrabBeamOverCharge);
			return;
		}
		this.physGrabPointPullerPosition = (Vector3)stream.ReceiveNext();
		this.physGrabPointPuller.position = this.physGrabPointPullerPosition;
		this.physGrabPointPlane.position = (Vector3)stream.ReceiveNext();
		this.mouseTurningVelocity = (Vector3)stream.ReceiveNext();
		this.isRotating = (bool)stream.ReceiveNext();
		this.colorState = (int)stream.ReceiveNext();
		this.physGrabBeamOverCharge = (byte)stream.ReceiveNext();
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x00082544 File Offset: 0x00080744
	private void PhysGrabStarted()
	{
		if (this.grabbedPhysGrabObject)
		{
			this.grabbedPhysGrabObject.GrabStarted(this);
			return;
		}
		if (this.grabbedStaticGrabObject)
		{
			this.grabbedStaticGrabObject.GrabStarted(this);
			return;
		}
		this.ReleaseObject(0.1f);
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x00082590 File Offset: 0x00080790
	private void PhysGrabEnded()
	{
		if (this.grabbedPhysGrabObject)
		{
			this.grabbedPhysGrabObject.GrabEnded(this);
			return;
		}
		if (this.grabbedStaticGrabObject)
		{
			this.grabbedStaticGrabObject.GrabEnded(this);
		}
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x000825C8 File Offset: 0x000807C8
	private void RayCheck(bool _grab)
	{
		if (this.playerAvatar.isDisabled || this.playerAvatar.isTumbling || this.playerAvatar.deadSet)
		{
			return;
		}
		float maxDistance = 10f;
		if (_grab)
		{
			this.grabDisableTimer = 0.1f;
		}
		Vector3 direction = this.playerCamera.transform.forward;
		if (this.overrideGrab && this.overrideGrabTarget)
		{
			direction = (this.overrideGrabTarget.transform.position - this.playerCamera.transform.position).normalized;
		}
		if (!_grab)
		{
			foreach (RaycastHit raycastHit in Physics.SphereCastAll(this.playerCamera.transform.position, 1f, direction, maxDistance, this.mask, QueryTriggerInteraction.Collide))
			{
				ValuableObject component = raycastHit.transform.GetComponent<ValuableObject>();
				if (component)
				{
					if (!component.discovered)
					{
						Vector3 direction2 = this.playerCamera.transform.position - raycastHit.point;
						RaycastHit[] array2 = Physics.SphereCastAll(raycastHit.point, 0.01f, direction2, direction2.magnitude, this.mask, QueryTriggerInteraction.Collide);
						bool flag = true;
						foreach (RaycastHit raycastHit2 in array2)
						{
							if (!raycastHit2.transform.CompareTag("Player") && raycastHit2.transform != raycastHit.transform)
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							component.Discover(ValuableDiscoverGraphic.State.Discover);
						}
					}
					else if (component.discoveredReminder)
					{
						Vector3 direction3 = this.playerCamera.transform.position - raycastHit.point;
						RaycastHit[] array4 = Physics.RaycastAll(raycastHit.point, direction3, direction3.magnitude, this.mask, QueryTriggerInteraction.Collide);
						bool flag2 = true;
						foreach (RaycastHit raycastHit3 in array4)
						{
							if (raycastHit3.collider.transform.CompareTag("Wall"))
							{
								flag2 = false;
								break;
							}
						}
						if (flag2)
						{
							component.discoveredReminder = false;
							component.Discover(ValuableDiscoverGraphic.State.Reminder);
						}
					}
				}
			}
		}
		RaycastHit raycastHit4;
		if (Physics.Raycast(this.playerCamera.transform.position, direction, out raycastHit4, maxDistance, this.mask, QueryTriggerInteraction.Ignore))
		{
			bool flag3 = this.overrideGrab && !this.overrideGrabTarget;
			bool flag4 = this.overrideGrab && this.overrideGrabTarget && raycastHit4.transform.GetComponentInParent<PhysGrabObject>() == this.overrideGrabTarget;
			if (!this.overrideGrab)
			{
				flag4 = true;
			}
			if (raycastHit4.collider.CompareTag("Phys Grab Object") && flag4)
			{
				if (raycastHit4.distance > this.grabRange)
				{
					return;
				}
				if (_grab)
				{
					this.grabbedPhysGrabObject = raycastHit4.transform.GetComponent<PhysGrabObject>();
					if (this.grabbedPhysGrabObject && this.grabbedPhysGrabObject.grabDisableTimer > 0f)
					{
						return;
					}
					if (this.grabbedPhysGrabObject && this.grabbedPhysGrabObject.rb.IsSleeping())
					{
						this.grabbedPhysGrabObject.OverrideIndestructible(0.5f);
						this.grabbedPhysGrabObject.OverrideBreakEffects(0.5f);
					}
					this.grabbedObjectTransform = raycastHit4.transform;
					if (this.grabbedPhysGrabObject)
					{
						PhysGrabObjectCollider component2 = raycastHit4.collider.GetComponent<PhysGrabObjectCollider>();
						this.grabbedPhysGrabObjectCollider = raycastHit4.collider;
						this.grabbedPhysGrabObjectColliderID = component2.colliderID;
						this.grabbedStaticGrabObject = null;
					}
					else
					{
						this.grabbedPhysGrabObject = null;
						this.grabbedPhysGrabObjectCollider = null;
						this.grabbedPhysGrabObjectColliderID = 0;
						this.grabbedStaticGrabObject = this.grabbedObjectTransform.GetComponent<StaticGrabObject>();
						if (!this.grabbedStaticGrabObject)
						{
							foreach (StaticGrabObject staticGrabObject in this.grabbedObjectTransform.GetComponentsInParent<StaticGrabObject>())
							{
								if (staticGrabObject.colliderTransform == raycastHit4.collider.transform)
								{
									this.grabbedStaticGrabObject = staticGrabObject;
								}
							}
						}
						if (!this.grabbedStaticGrabObject || !this.grabbedStaticGrabObject.enabled)
						{
							return;
						}
					}
					this.PhysGrabPointActivate();
					this.physGrabPointPuller.gameObject.SetActive(true);
					this.grabbedObject = raycastHit4.rigidbody;
					Vector3 vector = raycastHit4.point;
					if (this.grabbedPhysGrabObject && this.grabbedPhysGrabObject.roomVolumeCheck && this.grabbedPhysGrabObject.roomVolumeCheck.currentSize.magnitude < 0.5f)
					{
						vector = raycastHit4.collider.bounds.center;
					}
					float d = Vector3.Distance(this.playerCamera.transform.position, vector);
					Vector3 position = this.playerCamera.transform.position + this.playerCamera.transform.forward * d;
					this.physGrabPointPlane.position = position;
					this.physGrabPointPuller.position = position;
					if (this.physRotatingTimer <= 0f)
					{
						this.cameraRelativeGrabbedForward = Camera.main.transform.InverseTransformDirection(this.grabbedObjectTransform.forward);
						this.cameraRelativeGrabbedUp = Camera.main.transform.InverseTransformDirection(this.grabbedObjectTransform.up);
						this.cameraRelativeGrabbedRight = Camera.main.transform.InverseTransformDirection(this.grabbedObjectTransform.right);
					}
					if (GameManager.instance.gameMode == 0)
					{
						this.physGrabPoint.position = vector;
						if (!this.grabbedPhysGrabObject || !this.grabbedPhysGrabObject.forceGrabPoint)
						{
							this.localGrabPosition = this.grabbedObjectTransform.InverseTransformPoint(vector);
						}
						else
						{
							vector = this.grabbedPhysGrabObject.forceGrabPoint.position;
							d = 1f;
							position = this.playerCamera.transform.position + this.playerCamera.transform.forward * d - this.playerCamera.transform.up * 0.3f;
							this.physGrabPoint.position = vector;
							this.physGrabPointPlane.position = position;
							this.physGrabPointPuller.position = position;
							this.localGrabPosition = this.grabbedObjectTransform.InverseTransformPoint(vector);
						}
					}
					else if (this.grabbedPhysGrabObject)
					{
						if (this.grabbedPhysGrabObject.forceGrabPoint)
						{
							vector = this.grabbedPhysGrabObject.forceGrabPoint.position;
							Quaternion rotation = Quaternion.Euler(45f, 0f, 0f);
							this.cameraRelativeGrabbedForward = rotation * Vector3.forward;
							this.cameraRelativeGrabbedUp = rotation * Vector3.up;
							this.cameraRelativeGrabbedRight = rotation * Vector3.right;
							d = 1f;
							position = this.playerCamera.transform.position + this.playerCamera.transform.forward * d - this.playerCamera.transform.up * 0.3f;
							if (!this.overrideGrabPointTransform)
							{
								this.physGrabPoint.position = vector;
							}
							else
							{
								this.physGrabPoint.position = this.overrideGrabPointTransform.position;
							}
							this.physGrabPointPlane.position = position;
							this.physGrabPointPuller.position = position;
						}
						this.grabbedPhysGrabObject.GrabLink(this.photonView.ViewID, this.grabbedPhysGrabObjectColliderID, vector, this.cameraRelativeGrabbedForward, this.cameraRelativeGrabbedUp);
					}
					else if (this.grabbedStaticGrabObject)
					{
						this.grabbedStaticGrabObject.GrabLink(this.photonView.ViewID, vector);
					}
					if (this.isLocal)
					{
						PlayerController.instance.physGrabObject = this.grabbedObjectTransform.gameObject;
						PlayerController.instance.physGrabActive = true;
					}
					this.initialPressTimer = 0.1f;
					this.prevGrabbed = this.grabbed;
					this.grabbed = true;
				}
				if (!this.grabbed)
				{
					bool flag5 = false;
					PhysGrabObject exists = raycastHit4.transform.GetComponent<PhysGrabObject>();
					if (!exists)
					{
						exists = raycastHit4.transform.GetComponentInParent<PhysGrabObject>();
					}
					if (exists)
					{
						this.currentlyLookingAtPhysGrabObject = exists;
						flag5 = true;
					}
					StaticGrabObject staticGrabObject2 = raycastHit4.transform.GetComponent<StaticGrabObject>();
					if (!staticGrabObject2)
					{
						staticGrabObject2 = raycastHit4.transform.GetComponentInParent<StaticGrabObject>();
					}
					if (staticGrabObject2 && staticGrabObject2.enabled)
					{
						this.currentlyLookingAtStaticGrabObject = staticGrabObject2;
						flag5 = true;
					}
					ItemAttributes component3 = raycastHit4.transform.GetComponent<ItemAttributes>();
					if (component3)
					{
						this.currentlyLookingAtItemAttributes = component3;
						component3.ShowInfo();
					}
					if (flag5)
					{
						Aim.instance.SetState(Aim.State.Grabbable);
					}
				}
			}
		}
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x00082EEC File Offset: 0x000810EC
	public void ReleaseObject(float _disableTimer = 0.1f)
	{
		if (!this.grabbed)
		{
			return;
		}
		this.overrideGrab = false;
		this.overrideGrabTarget = null;
		if (!this.physGrabPoint)
		{
			return;
		}
		this.PhysGrabEnded();
		this.physGrabPoint.SetParent(base.transform.parent, true);
		this.grabbedObject = null;
		this.grabbedObjectTransform = null;
		this.prevGrabbed = this.grabbed;
		this.grabbed = false;
		this.physGrabBeamScript.lineRenderer.enabled = false;
		if (this.isLocal)
		{
			PlayerController.instance.physGrabObject = null;
			PlayerController.instance.physGrabActive = false;
		}
		if (this.physGrabPoint)
		{
			this.PhysGrabPointDeactivate();
		}
		if (this.physGrabPointPuller)
		{
			this.physGrabPointPuller.gameObject.SetActive(false);
		}
		this.PhysGrabBeamDeactivate();
		this.grabDisableTimer = 0.1f;
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x00082FCD File Offset: 0x000811CD
	[PunRPC]
	public void ReleaseObjectRPC(bool physGrabEnded, float _disableTimer = 0.1f)
	{
		if (this.isLocal)
		{
			if (!physGrabEnded)
			{
				this.grabbedStaticGrabObject = null;
			}
			this.ReleaseObject(0.1f);
			this.grabDisableTimer = _disableTimer;
		}
	}

	// Token: 0x06000E88 RID: 3720 RVA: 0x00082FF4 File Offset: 0x000811F4
	private void GridObjectsInstantiate()
	{
		PhysGrabObject physGrabObject = this.grabbedPhysGrabObject;
		if (physGrabObject.GetComponent<PhysGrabObjectImpactDetector>().isCart)
		{
			return;
		}
		Quaternion rotation = this.grabbedPhysGrabObject.rb.rotation;
		this.grabbedPhysGrabObject.rb.rotation = Quaternion.identity;
		foreach (Collider collider in physGrabObject.GetComponentsInChildren<Collider>())
		{
			if (!collider.isTrigger && collider.gameObject.activeSelf && !(collider is MeshCollider))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.physGrabPointVisualGridObject);
				gameObject.SetActive(true);
				this.SetGridObjectScale(gameObject.transform, collider);
				Quaternion rotation2 = this.grabbedObjectTransform.rotation;
				this.physGrabPointVisualGrid.rotation = Quaternion.identity;
				this.grabbedObjectTransform.rotation = Quaternion.identity;
				this.physGrabPointVisualGrid.localRotation = Quaternion.identity;
				Vector3 position = this.grabbedPhysGrabObject.transform.position;
				this.physGrabPointVisualGrid.position = this.grabbedPhysGrabObject.transform.TransformPoint(this.grabbedPhysGrabObject.midPointOffset);
				this.grabbedPhysGrabObject.transform.position = Vector3.zero;
				gameObject.transform.position = collider.bounds.center;
				gameObject.transform.rotation = collider.transform.rotation;
				gameObject.transform.SetParent(this.physGrabPointVisualGrid);
				this.physGrabPointVisualGridObjects.Add(gameObject);
				this.grabbedObjectTransform.rotation = rotation2;
				this.grabbedPhysGrabObject.transform.position = position;
			}
		}
		this.grabbedPhysGrabObject.rb.rotation = rotation;
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x000831B8 File Offset: 0x000813B8
	private void SetGridObjectScale(Transform _itemEquipCubeTransform, Collider _collider)
	{
		Quaternion rotation = _collider.transform.rotation;
		_collider.transform.rotation = Quaternion.identity;
		BoxCollider boxCollider = _collider as BoxCollider;
		if (boxCollider != null)
		{
			_itemEquipCubeTransform.localScale = Vector3.Scale(boxCollider.size, _collider.transform.lossyScale);
		}
		else
		{
			SphereCollider sphereCollider = _collider as SphereCollider;
			if (sphereCollider != null)
			{
				float num = sphereCollider.radius * Mathf.Max(new float[]
				{
					_collider.transform.lossyScale.x,
					_collider.transform.lossyScale.y,
					_collider.transform.lossyScale.z
				}) * 2f;
				_itemEquipCubeTransform.localScale = new Vector3(num, num, num);
			}
			else
			{
				CapsuleCollider capsuleCollider = _collider as CapsuleCollider;
				if (capsuleCollider != null)
				{
					float num2 = capsuleCollider.radius * Mathf.Max(_collider.transform.lossyScale.x, _collider.transform.lossyScale.z) * 2f;
					float y = capsuleCollider.height * _collider.transform.lossyScale.y;
					_itemEquipCubeTransform.localScale = new Vector3(num2, y, num2);
				}
				else
				{
					_itemEquipCubeTransform.localScale = _collider.bounds.size;
				}
			}
		}
		_collider.transform.rotation = rotation;
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x0008330C File Offset: 0x0008150C
	private void GridObjectsRemove()
	{
		foreach (GameObject obj in this.physGrabPointVisualGridObjects)
		{
			Object.Destroy(obj);
		}
		this.physGrabPointVisualGridObjects.Clear();
	}

	// Token: 0x04001794 RID: 6036
	private Camera playerCamera;

	// Token: 0x04001795 RID: 6037
	[HideInInspector]
	public float grabRange = 4f;

	// Token: 0x04001796 RID: 6038
	[HideInInspector]
	public float grabReleaseDistance = 8f;

	// Token: 0x04001797 RID: 6039
	public static PhysGrabber instance;

	// Token: 0x04001798 RID: 6040
	[Space]
	[HideInInspector]
	public float minDistanceFromPlayer = 1f;

	// Token: 0x04001799 RID: 6041
	[HideInInspector]
	public float maxDistanceFromPlayer = 2.5f;

	// Token: 0x0400179A RID: 6042
	private float minDistanceFromPlayerOriginal = 1f;

	// Token: 0x0400179B RID: 6043
	[Space]
	public PhysGrabBeam physGrabBeamComponent;

	// Token: 0x0400179C RID: 6044
	public GameObject physGrabBeam;

	// Token: 0x0400179D RID: 6045
	private PhysGrabBeam physGrabBeamScript;

	// Token: 0x0400179E RID: 6046
	public Transform physGrabPoint;

	// Token: 0x0400179F RID: 6047
	public Transform physGrabPointPuller;

	// Token: 0x040017A0 RID: 6048
	public Transform physGrabPointPlane;

	// Token: 0x040017A1 RID: 6049
	private GameObject physGrabPointVisual1;

	// Token: 0x040017A2 RID: 6050
	private GameObject physGrabPointVisual2;

	// Token: 0x040017A3 RID: 6051
	internal Vector3 grabbedcObjectPrevCamRelForward;

	// Token: 0x040017A4 RID: 6052
	internal Vector3 grabbedObjectPrevCamRelUp;

	// Token: 0x040017A5 RID: 6053
	internal PhysGrabObject grabbedPhysGrabObject;

	// Token: 0x040017A6 RID: 6054
	internal int grabbedPhysGrabObjectColliderID;

	// Token: 0x040017A7 RID: 6055
	internal Collider grabbedPhysGrabObjectCollider;

	// Token: 0x040017A8 RID: 6056
	internal StaticGrabObject grabbedStaticGrabObject;

	// Token: 0x040017A9 RID: 6057
	internal Rigidbody grabbedObject;

	// Token: 0x040017AA RID: 6058
	[HideInInspector]
	public Transform grabbedObjectTransform;

	// Token: 0x040017AB RID: 6059
	[HideInInspector]
	public float physGrabPointPullerDampen = 80f;

	// Token: 0x040017AC RID: 6060
	[HideInInspector]
	public float springConstant = 0.9f;

	// Token: 0x040017AD RID: 6061
	[HideInInspector]
	public float dampingConstant = 0.5f;

	// Token: 0x040017AE RID: 6062
	[HideInInspector]
	public float forceConstant = 4f;

	// Token: 0x040017AF RID: 6063
	[HideInInspector]
	public float forceMax = 4f;

	// Token: 0x040017B0 RID: 6064
	private bool physGrabBeamActive;

	// Token: 0x040017B1 RID: 6065
	[HideInInspector]
	public PhotonView photonView;

	// Token: 0x040017B2 RID: 6066
	[HideInInspector]
	public bool isLocal;

	// Token: 0x040017B3 RID: 6067
	[HideInInspector]
	public bool grabbed;

	// Token: 0x040017B4 RID: 6068
	internal float grabDisableTimer;

	// Token: 0x040017B5 RID: 6069
	[HideInInspector]
	public Vector3 physGrabPointPosition;

	// Token: 0x040017B6 RID: 6070
	[HideInInspector]
	public Vector3 physGrabPointPullerPosition;

	// Token: 0x040017B7 RID: 6071
	[HideInInspector]
	public PlayerAvatar playerAvatar;

	// Token: 0x040017B8 RID: 6072
	[HideInInspector]
	public Vector3 localGrabPosition;

	// Token: 0x040017B9 RID: 6073
	[HideInInspector]
	public Vector3 cameraRelativeGrabbedForward;

	// Token: 0x040017BA RID: 6074
	[HideInInspector]
	public Vector3 cameraRelativeGrabbedUp;

	// Token: 0x040017BB RID: 6075
	[HideInInspector]
	public Vector3 cameraRelativeGrabbedRight;

	// Token: 0x040017BC RID: 6076
	private Transform physGrabPointVisualRotate;

	// Token: 0x040017BD RID: 6077
	[HideInInspector]
	public Transform physGrabPointVisualGrid;

	// Token: 0x040017BE RID: 6078
	[HideInInspector]
	public GameObject physGrabPointVisualGridObject;

	// Token: 0x040017BF RID: 6079
	private List<GameObject> physGrabPointVisualGridObjects = new List<GameObject>();

	// Token: 0x040017C0 RID: 6080
	private float overrideMinimumGrabDistance;

	// Token: 0x040017C1 RID: 6081
	private float overrideMinimumGrabDistanceTimer;

	// Token: 0x040017C2 RID: 6082
	internal Vector3 currentGrabForce = Vector3.zero;

	// Token: 0x040017C3 RID: 6083
	internal Vector3 currentTorqueForce = Vector3.zero;

	// Token: 0x040017C4 RID: 6084
	private int prevColorState = -1;

	// Token: 0x040017C5 RID: 6085
	[HideInInspector]
	public int colorState;

	// Token: 0x040017C6 RID: 6086
	private float colorStateOverrideTimer;

	// Token: 0x040017C7 RID: 6087
	[Space]
	public LayerMask maskLayers;

	// Token: 0x040017C8 RID: 6088
	internal bool healing;

	// Token: 0x040017C9 RID: 6089
	internal ItemAttributes currentlyLookingAtItemAttributes;

	// Token: 0x040017CA RID: 6090
	internal PhysGrabObject currentlyLookingAtPhysGrabObject;

	// Token: 0x040017CB RID: 6091
	internal StaticGrabObject currentlyLookingAtStaticGrabObject;

	// Token: 0x040017CC RID: 6092
	[Space]
	public Material physGrabBeamMaterial;

	// Token: 0x040017CD RID: 6093
	public Material physGrabBeamMaterialBatteryCharge;

	// Token: 0x040017CE RID: 6094
	[HideInInspector]
	public bool physGrabForcesDisabled;

	// Token: 0x040017CF RID: 6095
	[HideInInspector]
	public float initialPressTimer;

	// Token: 0x040017D0 RID: 6096
	private bool overrideGrab;

	// Token: 0x040017D1 RID: 6097
	private bool overrideGrabRelease;

	// Token: 0x040017D2 RID: 6098
	private PhysGrabObject overrideGrabTarget;

	// Token: 0x040017D3 RID: 6099
	private float physGrabBeamAlpha = 1f;

	// Token: 0x040017D4 RID: 6100
	private float physGrabBeamAlphaChangeTo = 1f;

	// Token: 0x040017D5 RID: 6101
	private float physGramBeamAlphaTimer;

	// Token: 0x040017D6 RID: 6102
	private float physGrabBeamAlphaChangeProgress;

	// Token: 0x040017D7 RID: 6103
	private float physGrabBeamAlphaOriginal;

	// Token: 0x040017D8 RID: 6104
	private float overrideGrabDistance;

	// Token: 0x040017D9 RID: 6105
	private float overrideGrabDistanceTimer;

	// Token: 0x040017DA RID: 6106
	private float overrideDisableRotationControlsTimer;

	// Token: 0x040017DB RID: 6107
	private bool overrideDisableRotationControls;

	// Token: 0x040017DC RID: 6108
	private LayerMask mask;

	// Token: 0x040017DD RID: 6109
	private float grabCheckTimer;

	// Token: 0x040017DE RID: 6110
	internal float pullerDistance;

	// Token: 0x040017DF RID: 6111
	[Space]
	public Transform grabberAudioTransform;

	// Token: 0x040017E0 RID: 6112
	public Sound startSound;

	// Token: 0x040017E1 RID: 6113
	public Sound loopSound;

	// Token: 0x040017E2 RID: 6114
	public Sound stopSound;

	// Token: 0x040017E3 RID: 6115
	private float physRotatingTimer;

	// Token: 0x040017E4 RID: 6116
	internal Quaternion physRotation;

	// Token: 0x040017E5 RID: 6117
	private Quaternion physRotationBase;

	// Token: 0x040017E6 RID: 6118
	[HideInInspector]
	public Vector3 mouseTurningVelocity;

	// Token: 0x040017E7 RID: 6119
	[HideInInspector]
	public float grabStrength = 1f;

	// Token: 0x040017E8 RID: 6120
	[HideInInspector]
	public float throwStrength;

	// Token: 0x040017E9 RID: 6121
	internal bool debugStickyGrabber;

	// Token: 0x040017EA RID: 6122
	[HideInInspector]
	public float stopRotationTimer;

	// Token: 0x040017EB RID: 6123
	[HideInInspector]
	public Quaternion nextPhysRotation;

	// Token: 0x040017EC RID: 6124
	[HideInInspector]
	public bool isRotating;

	// Token: 0x040017ED RID: 6125
	private float isRotatingTimer;

	// Token: 0x040017EE RID: 6126
	internal bool isPushing;

	// Token: 0x040017EF RID: 6127
	internal bool isPulling;

	// Token: 0x040017F0 RID: 6128
	private float isPushingTimer;

	// Token: 0x040017F1 RID: 6129
	private float isPullingTimer;

	// Token: 0x040017F2 RID: 6130
	private float prevPullerDistance;

	// Token: 0x040017F3 RID: 6131
	internal bool prevGrabbed;

	// Token: 0x040017F4 RID: 6132
	private bool toggleGrab;

	// Token: 0x040017F5 RID: 6133
	private float toggleGrabTimer;

	// Token: 0x040017F6 RID: 6134
	private float overrideGrabPointTimer;

	// Token: 0x040017F7 RID: 6135
	private Transform overrideGrabPointTransform;

	// Token: 0x040017F8 RID: 6136
	internal byte physGrabBeamOverCharge;

	// Token: 0x040017F9 RID: 6137
	internal float physGrabBeamOverChargeFloat;

	// Token: 0x040017FA RID: 6138
	private float physGrabBeamOverChargeAmount;

	// Token: 0x040017FB RID: 6139
	private float physGrabBeamOverChargeTimer;

	// Token: 0x040017FC RID: 6140
	private float physGrabBeamOverchargeDecreaseCooldown;

	// Token: 0x040017FD RID: 6141
	private float physGrabBeamOverchargeInitialBoostCooldown;

	// Token: 0x020003AD RID: 941
	private enum ColorState
	{
		// Token: 0x04002BFE RID: 11262
		Orange,
		// Token: 0x04002BFF RID: 11263
		Green,
		// Token: 0x04002C00 RID: 11264
		Purple
	}
}
