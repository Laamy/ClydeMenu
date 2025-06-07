using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001C8 RID: 456
public class PlayerDeathHead : MonoBehaviour
{
	// Token: 0x06000FB7 RID: 4023 RVA: 0x0008E468 File Offset: 0x0008C668
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
		this.roomVolumeCheck = base.GetComponent<RoomVolumeCheck>();
		this.smokeParticleRateOverTimeDefault = this.smokeParticles.emission.rateOverTime.constant;
		this.smokeParticleRateOverDistanceDefault = this.smokeParticles.emission.rateOverDistance.constant;
		this.localSeenEffectTimer = this.localSeenEffectTime;
		foreach (MeshRenderer meshRenderer in this.eyeRenderers)
		{
			if (!this.eyeMaterial)
			{
				this.eyeMaterial = meshRenderer.material;
			}
			meshRenderer.material = this.eyeMaterial;
		}
		this.eyeMaterialAmount = Shader.PropertyToID("_ColorOverlayAmount");
		this.eyeMaterialColor = Shader.PropertyToID("_ColorOverlay");
		this.eyeFlashCurve = AssetManager.instance.animationCurveImpact;
		this.smokeParticleTimer = this.smokeParticleTime;
		this.physGrabObject.impactDetector.destroyDisableTeleport = false;
		this.colliders = base.GetComponentsInChildren<Collider>();
		this.SetColliders(false);
		base.StartCoroutine(this.Setup());
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x0008E595 File Offset: 0x0008C795
	private IEnumerator Setup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("SetupRPC", RpcTarget.OthersBuffered, new object[]
				{
					this.playerAvatar.steamID
				});
			}
			this.SetupDone();
			this.physGrabObject.Teleport(new Vector3(0f, 3000f, 0f), Quaternion.identity);
			if (SemiFunc.RunIsArena())
			{
				this.physGrabObject.impactDetector.destroyDisable = false;
			}
			this.setup = true;
		}
		yield break;
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x0008E5A4 File Offset: 0x0008C7A4
	private IEnumerator SetupClient()
	{
		while (!this.physGrabObject)
		{
			yield return new WaitForSeconds(0.1f);
		}
		while (!this.physGrabObject.impactDetector)
		{
			yield return new WaitForSeconds(0.1f);
		}
		while (!this.physGrabObject.impactDetector.particles)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.SetupDone();
		yield break;
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x0008E5B4 File Offset: 0x0008C7B4
	private void SetupDone()
	{
		if (!this.playerAvatar)
		{
			Debug.LogError("PlayerDeathHead: PlayerAvatar not found", base.gameObject);
			return;
		}
		if (SemiFunc.RunIsLevel() && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialReviving, 1) && !this.playerAvatar.isLocal)
		{
			this.tutorialPossible = true;
		}
		base.transform.parent = this.playerAvatar.transform.parent;
		if (SemiFunc.IsMultiplayer() && this.playerAvatar == SessionManager.instance.CrownedPlayerGet())
		{
			this.arenaCrown.SetActive(true);
		}
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x0008E650 File Offset: 0x0008C850
	private void Update()
	{
		if (!this.serverSeen)
		{
			this.mapCustom.Hide();
		}
		if ((!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient) && this.setup)
		{
			if (!this.triggered)
			{
				this.physGrabObject.OverrideDeactivate(0.1f);
			}
			else if (this.triggeredTimer > 0f)
			{
				this.physGrabObject.OverrideDeactivate(0.1f);
				this.triggeredTimer -= Time.deltaTime;
				if (this.triggeredTimer <= 0f)
				{
					this.physGrabObject.OverrideDeactivateReset();
					this.physGrabObject.rb.AddForce(this.playerAvatar.localCameraTransform.up * 2f, ForceMode.Impulse);
					this.physGrabObject.rb.AddForce(this.physGrabObject.transform.forward * 0.5f, ForceMode.Impulse);
					this.physGrabObject.rb.AddTorque(this.physGrabObject.transform.right * 0.2f, ForceMode.Impulse);
				}
			}
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (this.triggered)
			{
				this.inExtractionPoint = this.roomVolumeCheck.inExtractionPoint;
				if (this.inExtractionPoint != this.inExtractionPointPrevious)
				{
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("FlashEyeRPC", RpcTarget.All, new object[]
						{
							this.inExtractionPoint
						});
					}
					else
					{
						this.FlashEyeRPC(this.inExtractionPoint, default(PhotonMessageInfo));
					}
					this.inExtractionPointPrevious = this.inExtractionPoint;
				}
			}
			else
			{
				this.inExtractionPoint = false;
				this.inExtractionPointPrevious = false;
			}
		}
		if (this.smokeParticles.isPlaying)
		{
			this.smokeParticleTimer -= Time.deltaTime;
			if (this.smokeParticleTimer <= 0f)
			{
				this.smokeParticleRateOverTimeCurrent -= 1f * Time.deltaTime;
				this.smokeParticleRateOverTimeCurrent = Mathf.Max(this.smokeParticleRateOverTimeCurrent, 0f);
				this.smokeParticleRateOverDistanceCurrent -= 10f * Time.deltaTime;
				this.smokeParticleRateOverDistanceCurrent = Mathf.Max(this.smokeParticleRateOverDistanceCurrent, 0f);
				ParticleSystem.EmissionModule emission = this.smokeParticles.emission;
				emission.rateOverTime = new ParticleSystem.MinMaxCurve(this.smokeParticleRateOverTimeCurrent);
				emission.rateOverDistance = new ParticleSystem.MinMaxCurve(this.smokeParticleRateOverDistanceCurrent);
				if (this.smokeParticleRateOverTimeCurrent <= 0f && this.smokeParticleRateOverDistanceCurrent <= 0f)
				{
					this.smokeParticles.Stop();
				}
			}
		}
		if (this.eyeFlash)
		{
			this.eyeFlashLerp += 2f * Time.deltaTime;
			this.eyeFlashLerp = Mathf.Clamp01(this.eyeFlashLerp);
			this.eyeMaterial.SetFloat(this.eyeMaterialAmount, this.eyeFlashCurve.Evaluate(this.eyeFlashLerp));
			this.eyeFlashLight.intensity = this.eyeFlashCurve.Evaluate(this.eyeFlashLerp) * this.eyeFlashLightIntensity;
			if (this.eyeFlashLerp > 1f)
			{
				this.eyeFlash = false;
				this.eyeMaterial.SetFloat(this.eyeMaterialAmount, 0f);
				this.eyeFlashLight.gameObject.SetActive(false);
			}
		}
		if (this.triggered && !this.localSeen && !PlayerController.instance.playerAvatarScript.isDisabled)
		{
			if (this.seenCooldownTimer > 0f)
			{
				this.seenCooldownTimer -= Time.deltaTime;
			}
			else
			{
				Vector3 localCameraPosition = PlayerController.instance.playerAvatarScript.localCameraPosition;
				float num = Vector3.Distance(base.transform.position, localCameraPosition);
				if (num <= 10f && SemiFunc.OnScreen(base.transform.position, -0.15f, -0.15f))
				{
					Vector3 normalized = (localCameraPosition - base.transform.position).normalized;
					RaycastHit raycastHit;
					if (!Physics.Raycast(this.physGrabObject.centerPoint, normalized, out raycastHit, num, LayerMask.GetMask(new string[]
					{
						"Default"
					})))
					{
						this.localSeen = true;
						TutorialDirector.instance.playerSawHead = true;
						if (!this.serverSeen && SemiFunc.RunIsLevel())
						{
							if (SemiFunc.IsMultiplayer())
							{
								this.photonView.RPC("SeenSetRPC", RpcTarget.All, new object[]
								{
									true
								});
							}
							else
							{
								this.SeenSetRPC(true, default(PhotonMessageInfo));
							}
							if (PlayerController.instance.deathSeenTimer <= 0f)
							{
								this.localSeenEffect = true;
								PlayerController.instance.deathSeenTimer = 30f;
								GameDirector.instance.CameraImpact.Shake(2f, 0.5f);
								GameDirector.instance.CameraShake.Shake(2f, 1f);
								AudioScare.instance.PlayCustom(this.seenSound, 0.3f, 60f);
								ValuableDiscover.instance.New(this.physGrabObject, ValuableDiscoverGraphic.State.Bad);
							}
						}
					}
				}
			}
		}
		if (this.localSeenEffect)
		{
			this.localSeenEffectTimer -= Time.deltaTime;
			CameraZoom.Instance.OverrideZoomSet(75f, 0.1f, 0.25f, 0.25f, base.gameObject, 150);
			PostProcessing.Instance.VignetteOverride(Color.black, 0.4f, 1f, 1f, 0.5f, 0.1f, base.gameObject);
			PostProcessing.Instance.SaturationOverride(-50f, 1f, 0.5f, 0.1f, base.gameObject);
			PostProcessing.Instance.ContrastOverride(5f, 1f, 0.5f, 0.1f, base.gameObject);
			GameDirector.instance.CameraImpact.Shake(10f * Time.deltaTime, 0.1f);
			GameDirector.instance.CameraShake.Shake(10f * Time.deltaTime, 1f);
			if (this.localSeenEffectTimer <= 0f)
			{
				this.localSeenEffect = false;
			}
		}
		if (this.triggered && SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.roomVolumeCheck.CurrentRooms.Count <= 0)
			{
				this.outsideLevelTimer += Time.deltaTime;
				if (this.outsideLevelTimer >= 5f)
				{
					if (RoundDirector.instance.extractionPointActive)
					{
						this.physGrabObject.Teleport(RoundDirector.instance.extractionPointCurrent.safetySpawn.position, RoundDirector.instance.extractionPointCurrent.safetySpawn.rotation);
					}
					else
					{
						this.physGrabObject.Teleport(TruckSafetySpawnPoint.instance.transform.position, TruckSafetySpawnPoint.instance.transform.rotation);
					}
				}
			}
			else
			{
				this.outsideLevelTimer = 0f;
			}
		}
		if (this.tutorialPossible)
		{
			if (this.triggered && this.localSeen)
			{
				this.tutorialTimer -= Time.deltaTime;
				if (this.tutorialTimer <= 0f)
				{
					if (!RoundDirector.instance.allExtractionPointsCompleted && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialReviving, 1))
					{
						TutorialDirector.instance.ActivateTip("Reviving", 0.5f, false);
					}
					this.tutorialPossible = false;
				}
			}
			else
			{
				this.tutorialTimer = 5f;
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (RoundDirector.instance.allExtractionPointsCompleted && this.triggered && !this.playerAvatar.finalHeal)
			{
				this.inTruck = this.roomVolumeCheck.inTruck;
				if (this.inTruck != this.inTruckPrevious)
				{
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("FlashEyeRPC", RpcTarget.All, new object[]
						{
							this.inTruck
						});
					}
					else
					{
						this.FlashEyeRPC(this.inTruck, default(PhotonMessageInfo));
					}
					this.inTruckPrevious = this.inTruck;
				}
			}
			else
			{
				this.inTruck = false;
				this.inTruckPrevious = false;
			}
			if (this.inTruck)
			{
				this.inTruckReviveTimer -= Time.deltaTime;
				if (this.inTruckReviveTimer <= 0f)
				{
					this.playerAvatar.Revive(true);
					return;
				}
			}
			else
			{
				this.inTruckReviveTimer = 2f;
			}
		}
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x0008EEA4 File Offset: 0x0008D0A4
	private void UpdateColor()
	{
		if (!this.headRenderer)
		{
			return;
		}
		this.headRenderer.material = this.playerAvatar.playerHealth.bodyMaterial;
		this.headRenderer.material.SetFloat(Shader.PropertyToID("_ColorOverlayAmount"), 0f);
		Color color = this.playerAvatar.playerAvatarVisuals.color;
		this.physGrabObject.impactDetector.particles.gradient = new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(color, 0f),
				new GradientColorKey(color, 1f)
			}
		};
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x0008EF56 File Offset: 0x0008D156
	public void Revive()
	{
		if (this.triggered && this.inExtractionPoint)
		{
			this.playerAvatar.Revive(false);
		}
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x0008EF74 File Offset: 0x0008D174
	public void Trigger()
	{
		this.seenCooldownTimer = this.seenCooldownTime;
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (this.playerAvatar.isLocal)
			{
				PlayerController.instance.col.enabled = false;
			}
			else
			{
				this.playerAvatar.playerAvatarCollision.Collider.enabled = false;
			}
			Collider[] array = this.playerAvatar.tumble.colliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			this.physGrabObject.Teleport(this.playerAvatar.playerAvatarCollision.deathHeadPosition, this.playerAvatar.localCameraTransform.rotation);
			this.triggeredTimer = 0.1f;
		}
		this.UpdateColor();
		this.triggered = true;
		this.SetColliders(true);
		if (this.smokeParticles)
		{
			this.smokeParticles.Play();
		}
		this.smokeParticleRateOverTimeCurrent = this.smokeParticleRateOverTimeDefault;
		this.smokeParticleRateOverDistanceCurrent = this.smokeParticleRateOverDistanceDefault;
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x0008F078 File Offset: 0x0008D278
	public void Reset()
	{
		this.triggered = false;
		this.smokeParticleTimer = this.smokeParticleTime;
		this.localSeenEffectTimer = this.localSeenEffectTime;
		this.localSeen = false;
		this.localSeenEffect = false;
		this.SetColliders(false);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.physGrabObject.Teleport(new Vector3(0f, 3000f, 0f), Quaternion.identity);
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("SeenSetRPC", RpcTarget.All, new object[]
				{
					false
				});
				return;
			}
			this.SeenSetRPC(false, default(PhotonMessageInfo));
		}
	}

	// Token: 0x06000FC0 RID: 4032 RVA: 0x0008F11C File Offset: 0x0008D31C
	private void SetColliders(bool _enabled)
	{
		foreach (Collider collider in this.colliders)
		{
			if (collider)
			{
				collider.enabled = _enabled;
			}
		}
	}

	// Token: 0x06000FC1 RID: 4033 RVA: 0x0008F154 File Offset: 0x0008D354
	[PunRPC]
	public void SetupRPC(string _steamID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar.steamID == _steamID)
			{
				this.playerAvatar = playerAvatar;
				this.playerAvatar.playerDeathHead = this;
				break;
			}
		}
		base.StartCoroutine(this.SetupClient());
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x0008F1E0 File Offset: 0x0008D3E0
	[PunRPC]
	public void FlashEyeRPC(bool _positive, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.inExtractionPoint = _positive;
		if (_positive)
		{
			this.eyeMaterial.SetColor(this.eyeMaterialColor, this.eyeFlashPositiveColor);
			this.eyeFlashPositiveSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.eyeFlashLight.color = this.eyeFlashPositiveColor;
		}
		else
		{
			this.eyeMaterial.SetColor(this.eyeMaterialColor, this.eyeFlashNegativeColor);
			this.eyeFlashNegativeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.eyeFlashLight.color = this.eyeFlashNegativeColor;
		}
		this.eyeFlash = true;
		this.eyeFlashLerp = 0f;
		this.eyeFlashLight.gameObject.SetActive(true);
		GameDirector.instance.CameraImpact.ShakeDistance(1f, 2f, 8f, base.transform.position, 0.25f);
		GameDirector.instance.CameraShake.ShakeDistance(1f, 2f, 8f, base.transform.position, 0.5f);
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x0008F327 File Offset: 0x0008D527
	[PunRPC]
	public void SeenSetRPC(bool _toggle, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (SemiFunc.MasterOnlyRPC(_info) || (this.triggered && _toggle))
		{
			this.serverSeen = _toggle;
		}
	}

	// Token: 0x04001A63 RID: 6755
	public PlayerAvatar playerAvatar;

	// Token: 0x04001A64 RID: 6756
	public MeshRenderer headRenderer;

	// Token: 0x04001A65 RID: 6757
	public ParticleSystem smokeParticles;

	// Token: 0x04001A66 RID: 6758
	public MapCustom mapCustom;

	// Token: 0x04001A67 RID: 6759
	public GameObject arenaCrown;

	// Token: 0x04001A68 RID: 6760
	private float smokeParticleTime = 3f;

	// Token: 0x04001A69 RID: 6761
	private float smokeParticleTimer;

	// Token: 0x04001A6A RID: 6762
	private float smokeParticleRateOverTimeDefault;

	// Token: 0x04001A6B RID: 6763
	private float smokeParticleRateOverTimeCurrent;

	// Token: 0x04001A6C RID: 6764
	private float smokeParticleRateOverDistanceDefault;

	// Token: 0x04001A6D RID: 6765
	private float smokeParticleRateOverDistanceCurrent;

	// Token: 0x04001A6E RID: 6766
	internal PhysGrabObject physGrabObject;

	// Token: 0x04001A6F RID: 6767
	private PhotonView photonView;

	// Token: 0x04001A70 RID: 6768
	private RoomVolumeCheck roomVolumeCheck;

	// Token: 0x04001A71 RID: 6769
	private bool setup;

	// Token: 0x04001A72 RID: 6770
	private bool triggered;

	// Token: 0x04001A73 RID: 6771
	private float triggeredTimer;

	// Token: 0x04001A74 RID: 6772
	internal bool inExtractionPoint;

	// Token: 0x04001A75 RID: 6773
	private bool inExtractionPointPrevious;

	// Token: 0x04001A76 RID: 6774
	internal bool inTruck;

	// Token: 0x04001A77 RID: 6775
	private bool inTruckPrevious;

	// Token: 0x04001A78 RID: 6776
	[Space]
	public MeshRenderer[] eyeRenderers;

	// Token: 0x04001A79 RID: 6777
	public Light eyeFlashLight;

	// Token: 0x04001A7A RID: 6778
	public Color eyeFlashPositiveColor;

	// Token: 0x04001A7B RID: 6779
	public Color eyeFlashNegativeColor;

	// Token: 0x04001A7C RID: 6780
	public float eyeFlashStrength;

	// Token: 0x04001A7D RID: 6781
	public float eyeFlashLightIntensity;

	// Token: 0x04001A7E RID: 6782
	public Sound eyeFlashPositiveSound;

	// Token: 0x04001A7F RID: 6783
	public Sound eyeFlashNegativeSound;

	// Token: 0x04001A80 RID: 6784
	private Material eyeMaterial;

	// Token: 0x04001A81 RID: 6785
	private int eyeMaterialAmount;

	// Token: 0x04001A82 RID: 6786
	private int eyeMaterialColor;

	// Token: 0x04001A83 RID: 6787
	private AnimationCurve eyeFlashCurve;

	// Token: 0x04001A84 RID: 6788
	private float eyeFlashLerp;

	// Token: 0x04001A85 RID: 6789
	private bool eyeFlash;

	// Token: 0x04001A86 RID: 6790
	public AudioClip seenSound;

	// Token: 0x04001A87 RID: 6791
	private bool serverSeen;

	// Token: 0x04001A88 RID: 6792
	private float seenCooldownTime = 2f;

	// Token: 0x04001A89 RID: 6793
	private float seenCooldownTimer;

	// Token: 0x04001A8A RID: 6794
	private bool localSeen;

	// Token: 0x04001A8B RID: 6795
	private bool localSeenEffect;

	// Token: 0x04001A8C RID: 6796
	private float localSeenEffectTime = 2f;

	// Token: 0x04001A8D RID: 6797
	private float localSeenEffectTimer;

	// Token: 0x04001A8E RID: 6798
	private float outsideLevelTimer;

	// Token: 0x04001A8F RID: 6799
	private bool tutorialPossible;

	// Token: 0x04001A90 RID: 6800
	private float tutorialTimer;

	// Token: 0x04001A91 RID: 6801
	private float inTruckReviveTimer;

	// Token: 0x04001A92 RID: 6802
	private Collider[] colliders;
}
