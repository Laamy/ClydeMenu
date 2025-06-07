using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public class PlayerTumble : MonoBehaviour
{
	// Token: 0x0600100D RID: 4109 RVA: 0x000937A5 File Offset: 0x000919A5
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x000937CB File Offset: 0x000919CB
	private void Start()
	{
		if (SemiFunc.RunIsLobbyMenu())
		{
			return;
		}
		base.StartCoroutine(this.Setup());
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x000937E2 File Offset: 0x000919E2
	private IEnumerator Setup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("SetupRPC", RpcTarget.OthersBuffered, new object[]
				{
					this.playerAvatar.steamID
				});
			}
			this.SetupDone();
		}
		yield break;
	}

	// Token: 0x06001010 RID: 4112 RVA: 0x000937F4 File Offset: 0x000919F4
	private void SetupDone()
	{
		Collider[] array = this.colliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		this.playerAvatar.SoundSetup(this.tumbleLaunchSound);
		base.transform.parent = this.playerAvatar.transform.parent;
		this.setup = true;
		string text = SemiFunc.PlayerGetSteamID(this.playerAvatar);
		if (StatsManager.instance.playerUpgradeLaunch.ContainsKey(text))
		{
			this.tumbleLaunch = StatsManager.instance.playerUpgradeLaunch[SemiFunc.PlayerGetSteamID(this.playerAvatar)];
		}
		this.physGrabObject.impactDetector.destroyDisableTeleport = false;
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x000938A4 File Offset: 0x00091AA4
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
				this.playerAvatar.tumble = this;
				break;
			}
		}
		this.SetupDone();
	}

	// Token: 0x06001012 RID: 4114 RVA: 0x00093928 File Offset: 0x00091B28
	private void Update()
	{
		if (SemiFunc.RunIsLobbyMenu() || !this.physGrabObject.spawned)
		{
			return;
		}
		if (this.isTumbling)
		{
			this.rb.isKinematic = false;
		}
		else
		{
			this.rb.isKinematic = true;
		}
		if (!this.isTumbling && this.playerAvatar)
		{
			Vector3 position = this.playerAvatar.transform.position + Vector3.up * 0.3f;
			Quaternion rotation = this.playerAvatar.transform.rotation;
			this.rb.MovePosition(position);
			this.rb.MoveRotation(rotation);
		}
		if (this.tumbleSetTimer > 0f)
		{
			this.tumbleSetTimer -= Time.deltaTime;
		}
		if (this.tumbleMoveSoundTimer > 0f)
		{
			this.tumbleMoveSoundTimer -= Time.deltaTime;
			this.tumbleMoveSound.PlayLoop(true, 1f, 1f, this.tumbleMoveSoundSpeed);
		}
		else
		{
			this.tumbleMoveSound.PlayLoop(false, 1f, 1f, this.tumbleMoveSoundSpeed);
		}
		if (this.isTumbling && this.playerAvatar.isLocal)
		{
			CameraZoom.Instance.OverrideZoomSet(55f, 0.1f, 1f, 1f, base.gameObject, 150);
			PostProcessing.Instance.VignetteOverride(Color.black, 0.6f, 0.2f, 2f, 2f, 0.1f, base.gameObject);
		}
		bool flag = false;
		if (this.isTumbling)
		{
			Vector3 rbVelocity = this.physGrabObject.rbVelocity;
			if (rbVelocity.magnitude > 4f && !this.physGrabObject.impactDetector.inCart)
			{
				flag = true;
				this.hurtCollider.transform.LookAt(this.hurtCollider.transform.position + rbVelocity);
				if (this.physGrabObject.playerGrabbing.Count == 0 && this.overrideEnemyHurtTimer <= 0f)
				{
					this.hurtCollider.enemyLogic = true;
				}
				else
				{
					this.hurtCollider.enemyLogic = false;
				}
				if (this.playerAvatar.isLocal)
				{
					this.hurtCollider.playerLogic = false;
				}
			}
		}
		if (this.hurtColliderPauseTimer > 0f)
		{
			flag = false;
			this.hurtColliderPauseTimer -= Time.deltaTime;
		}
		if (flag)
		{
			if (!this.hurtCollider.gameObject.activeSelf)
			{
				this.hurtCollider.gameObject.SetActive(true);
			}
		}
		else if (this.hurtCollider.gameObject.activeSelf)
		{
			this.hurtCollider.gameObject.SetActive(false);
		}
		if (this.overrideEnemyHurtTimer > 0f)
		{
			this.overrideEnemyHurtTimer -= Time.deltaTime;
		}
		if (this.isTumbling)
		{
			if ((Vector3.Distance(this.notMovingPositionLast, base.transform.position) <= 0.5f || this.physGrabObject.impactDetector.inCart) && this.physGrabObject.playerGrabbing.Count <= 0)
			{
				this.notMovingTimer += Time.deltaTime;
			}
			else
			{
				this.notMovingTimer = 0f;
				this.notMovingPositionLast = base.transform.position;
			}
		}
		else
		{
			this.notMovingTimer = 0f;
			this.notMovingPositionLast = base.transform.position;
		}
		if (this.breakFreeCooldown <= 0f)
		{
			if (this.physGrabObject.playerGrabbing.Count > 0 && this.playerAvatar.isLocal && SemiFunc.InputDown(InputKey.Jump))
			{
				this.breakFreeCooldown = 0.5f;
				this.TumbleForce(this.playerAvatar.localCameraTransform.forward * 15f);
				this.TumbleTorque(base.transform.right * 10f);
				this.BreakFree(this.playerAvatar.localCameraTransform.forward);
			}
		}
		else
		{
			this.breakFreeCooldown -= Time.deltaTime;
		}
		if (this.impactHurtTimer > 0f)
		{
			this.impactHurtTimer -= Time.deltaTime;
		}
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.physGrabObject.playerGrabbing.Count > 0)
		{
			this.TumbleOverrideTime(1f);
		}
		if (this.tumbleOverrideTimer > 0f)
		{
			this.tumbleOverrideTimer -= Time.deltaTime;
			this.tumbleOverride = true;
		}
		else
		{
			this.tumbleOverride = false;
		}
		if (this.tumbleOverride != this.tumbleOverridePrevious)
		{
			if (this.tumbleOverride)
			{
				this.TumbleOverride(true);
			}
			else
			{
				this.TumbleOverride(false);
			}
			this.tumbleOverridePrevious = this.tumbleOverride;
		}
		if (this.isTumbling && this.playerAvatar.isDisabled)
		{
			this.TumbleRequest(false, false);
		}
		if (this.isTumbling != this.isTumblingPrevious)
		{
			if (this.isTumbling)
			{
				this.SetPosition();
				Vector3 rbVelocityRaw = this.playerAvatar.rbVelocityRaw;
				this.rb.AddForce(rbVelocityRaw, ForceMode.VelocityChange);
				Vector3 a = Vector3.Cross(Vector3.up, rbVelocityRaw);
				if (a.magnitude <= 0f)
				{
					a = Random.insideUnitSphere.normalized * 1f;
				}
				this.rb.AddTorque(a * 2f, ForceMode.VelocityChange);
			}
			this.isTumblingPrevious = this.isTumbling;
		}
	}

	// Token: 0x06001013 RID: 4115 RVA: 0x00093E98 File Offset: 0x00092098
	private void FixedUpdate()
	{
		if (!this.isTumbling || (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient))
		{
			return;
		}
		if (this.isTumbling && this.playerAvatar.playerHealth.hurtFreeze && this.playerAvatar.deadSet)
		{
			this.physGrabObject.FreezeForces(0.1f, Vector3.zero, Vector3.zero);
			return;
		}
		if (this.customGravityOverrideTimer > 0f)
		{
			this.customGravityOverrideTimer -= Time.fixedDeltaTime;
		}
		if (this.rb.useGravity && this.physGrabObject.playerGrabbing.Count <= 0 && this.customGravityOverrideTimer <= 0f)
		{
			this.rb.AddForce(-Vector3.up * this.customGravity, ForceMode.Force);
		}
		if (this.tumbleForceTimer > 0f)
		{
			this.tumbleForceTimer -= Time.fixedDeltaTime;
		}
		if (this.tumbleForceTimer <= 0f && !this.playerAvatar.playerHealth.hurtFreeze)
		{
			if (this.tumbleForce.magnitude > 0f)
			{
				this.rb.AddForce(this.tumbleForce, ForceMode.Impulse);
				this.tumbleForce = Vector3.zero;
			}
			if (this.tumbleTorque.magnitude > 0f)
			{
				this.rb.AddTorque(this.tumbleTorque, ForceMode.Impulse);
				this.tumbleTorque = Vector3.zero;
			}
		}
		if (this.notMovingTimer > 2f)
		{
			this.lookAtLerp += 0.5f * Time.fixedDeltaTime;
			this.lookAtLerp = Mathf.Clamp01(this.lookAtLerp);
			Vector3 vector = SemiFunc.PhysFollowRotation(base.transform, this.playerAvatar.localCameraRotation, this.rb, 5f);
			vector = Vector3.Lerp(Vector3.zero, vector, 3f * Time.fixedDeltaTime);
			vector = Vector3.Lerp(Vector3.zero, vector, this.lookAtLerp);
			this.rb.AddTorque(vector, ForceMode.Impulse);
			return;
		}
		this.lookAtLerp = 0f;
	}

	// Token: 0x06001014 RID: 4116 RVA: 0x000940A4 File Offset: 0x000922A4
	public void DisableCustomGravity(float _time)
	{
		this.customGravityOverrideTimer = _time;
	}

	// Token: 0x06001015 RID: 4117 RVA: 0x000940AD File Offset: 0x000922AD
	private void SetPosition()
	{
		this.rb.isKinematic = false;
		this.tumbleForceTimer = 0.1f;
		this.rb.velocity = Vector3.zero;
		this.rb.angularVelocity = Vector3.zero;
	}

	// Token: 0x06001016 RID: 4118 RVA: 0x000940E6 File Offset: 0x000922E6
	public void OverrideEnemyHurt(float _time)
	{
		this.overrideEnemyHurtTimer = _time;
	}

	// Token: 0x06001017 RID: 4119 RVA: 0x000940F0 File Offset: 0x000922F0
	public void HitEnemy()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.playerAvatar.isLocal)
			{
				this.playerAvatar.playerHealth.Hurt(5, true, -1);
				return;
			}
			this.playerAvatar.playerHealth.HurtOther(5, base.transform.position, true, -1);
		}
	}

	// Token: 0x06001018 RID: 4120 RVA: 0x00094144 File Offset: 0x00092344
	public void TumbleImpact()
	{
		if (this.playerAvatar.isLocal)
		{
			PlayerController.instance.CollisionController.StopFallLoop();
		}
		if (this.hurtColliderPauseTimer > 0f)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer() && !this.hurtCollider.onImpactPlayerAvatar.photonView.IsMine)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("TumbleImpactRPC", RpcTarget.All, new object[]
			{
				this.hurtCollider.onImpactPlayerAvatar.photonView.ViewID
			});
			return;
		}
		this.TumbleImpactRPC(this.hurtCollider.onImpactPlayerAvatar.photonView.ViewID);
	}

	// Token: 0x06001019 RID: 4121 RVA: 0x000941F4 File Offset: 0x000923F4
	[PunRPC]
	public void TumbleImpactRPC(int _playerID)
	{
		float time = 0.15f;
		this.hurtColliderPauseTimer = 0.5f;
		Vector3 vector = Vector3.zero;
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
		{
			if (playerAvatar.photonView.ViewID == _playerID)
			{
				playerAvatar.playerHealth.HurtFreezeOverride(time);
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					vector = (playerAvatar.transform.position - base.transform.position).normalized;
					playerAvatar.tumble.physGrabObject.FreezeForces(time, vector * 5f, Vector3.zero);
					break;
				}
				break;
			}
		}
		this.impactParticle.gameObject.SetActive(true);
		this.impactParticle.transform.position = Vector3.Lerp(base.transform.position, base.transform.position + vector, 0.5f);
		this.impactSound.Play(this.impactParticle.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 15f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 5f, 15f, base.transform.position, 0.5f);
		this.playerAvatar.playerHealth.HurtFreezeOverride(time);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.physGrabObject.FreezeForces(time, vector * -5f, Vector3.zero);
		}
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x000943C8 File Offset: 0x000925C8
	public void TumbleOverride(bool _active)
	{
		if (!GameManager.Multiplayer())
		{
			this.TumbleOverrideRPC(_active, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("TumbleOverrideRPC", RpcTarget.All, new object[]
		{
			_active
		});
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x0009440D File Offset: 0x0009260D
	[PunRPC]
	public void TumbleOverrideRPC(bool _active, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.playerAvatar.photonView))
		{
			return;
		}
		this.tumbleOverride = _active;
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x0009442A File Offset: 0x0009262A
	public void TumbleOverrideTime(float _time)
	{
		if (!GameManager.Multiplayer())
		{
			this.TumbleOverrideTimeRPC(_time);
			return;
		}
		this.photonView.RPC("TumbleOverrideTimeRPC", RpcTarget.MasterClient, new object[]
		{
			_time
		});
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x0009445B File Offset: 0x0009265B
	[PunRPC]
	public void TumbleOverrideTimeRPC(float _time)
	{
		this.tumbleOverrideTimer = _time;
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x00094464 File Offset: 0x00092664
	public void TumbleForce(Vector3 _force)
	{
		if (!GameManager.Multiplayer())
		{
			this.TumbleForceRPC(_force, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("TumbleForceRPC", RpcTarget.MasterClient, new object[]
		{
			_force
		});
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x000944A9 File Offset: 0x000926A9
	[PunRPC]
	public void TumbleForceRPC(Vector3 _force, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.playerAvatar.photonView))
		{
			return;
		}
		this.tumbleForce += _force;
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x000944D1 File Offset: 0x000926D1
	public void TumbleTorque(Vector3 _torque)
	{
		if (!GameManager.Multiplayer())
		{
			this.TumbleTorqueRPC(_torque);
			return;
		}
		this.photonView.RPC("TumbleTorqueRPC", RpcTarget.MasterClient, new object[]
		{
			_torque
		});
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x00094502 File Offset: 0x00092702
	[PunRPC]
	public void TumbleTorqueRPC(Vector3 _torque)
	{
		this.tumbleTorque += _torque;
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x00094518 File Offset: 0x00092718
	public void TumbleRequest(bool _isTumbling, bool _playerInput)
	{
		if (PlayerController.instance.DebugNoTumble && !_playerInput)
		{
			return;
		}
		if (SemiFunc.MenuLevel())
		{
			return;
		}
		if (this.isTumbling == _isTumbling)
		{
			return;
		}
		if (!GameManager.Multiplayer())
		{
			this.TumbleRequestRPC(_isTumbling, _playerInput);
			return;
		}
		this.photonView.RPC("TumbleRequestRPC", RpcTarget.MasterClient, new object[]
		{
			_isTumbling,
			_playerInput
		});
	}

	// Token: 0x06001023 RID: 4131 RVA: 0x00094580 File Offset: 0x00092780
	[PunRPC]
	public void TumbleRequestRPC(bool _isTumbling, bool _playerInput)
	{
		if (SemiFunc.MenuLevel())
		{
			return;
		}
		if (this.isTumbling == _isTumbling)
		{
			return;
		}
		this.TumbleSet(_isTumbling, _playerInput);
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x0009459C File Offset: 0x0009279C
	public void TumbleSet(bool _isTumbling, bool _playerInput)
	{
		this.isTumbling = _isTumbling;
		this.SetPosition();
		if (_playerInput)
		{
			this.isPlayerInputTriggered = true;
		}
		else
		{
			this.isPlayerInputTriggered = false;
		}
		if (this.isTumbling)
		{
			this.rb.isKinematic = false;
			if (this.tumbleLaunch > 0 && _playerInput)
			{
				Vector3 b = this.playerAvatar.localCameraTransform.forward * (3f * (float)this.tumbleLaunch);
				this.tumbleForce += b;
			}
		}
		else
		{
			this.rb.isKinematic = true;
			this.tumbleForce = Vector3.zero;
		}
		if (!GameManager.Multiplayer())
		{
			this.TumbleSetRPC(this.isTumbling, _playerInput, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("TumbleSetRPC", RpcTarget.All, new object[]
		{
			this.isTumbling,
			_playerInput
		});
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x00094684 File Offset: 0x00092884
	[PunRPC]
	public void TumbleSetRPC(bool _isTumbling, bool _playerInput, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.playerAvatar.photonView))
		{
			return;
		}
		if (this.playerAvatar.isLocal && _isTumbling && !_playerInput)
		{
			ChatManager.instance.TumbleInterruption();
		}
		this.isTumbling = _isTumbling;
		this.playerAvatar.isTumbling = this.isTumbling;
		this.playerAvatar.EnemyVisionFreezeTimerSet(0.5f);
		Vector3 position = this.playerAvatar.transform.position + Vector3.up * 0.3f;
		Quaternion rotation = this.playerAvatar.transform.rotation;
		if (!this.rb.isKinematic)
		{
			this.rb.velocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.physGrabObject.photonTransformView.Teleport(position, rotation);
		}
		else
		{
			this.physGrabObject.rb.position = position;
			this.physGrabObject.rb.rotation = rotation;
		}
		if (this.playerAvatar.isLocal)
		{
			this.playerAvatar.physGrabber.ReleaseObject(0.1f);
			PlayerController.instance.tumbleInputDisableTimer = 1f;
			GameDirector.instance.CameraImpact.Shake(1f, 0.1f);
			GameDirector.instance.CameraShake.Shake(2f, 0.5f);
			CameraPosition.instance.TumbleSet();
		}
		Collider[] array;
		if (this.isTumbling)
		{
			if (this.tumbleLaunch > 0 && _playerInput)
			{
				this.tumbleLaunchSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.playerAvatar.playerAvatarVisuals.PowerupJumpEffect();
			}
			this.playerAvatar.TumbleStart();
			this.tumbleSetTimer = 0.1f;
			if (this.playerAvatar.isLocal)
			{
				PlayerController.instance.col.enabled = false;
			}
			else
			{
				this.playerAvatar.playerAvatarCollision.Collider.enabled = false;
			}
			array = this.colliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
			return;
		}
		this.playerAvatar.TumbleStop();
		if (this.playerAvatar.isLocal)
		{
			PlayerController.instance.col.enabled = true;
		}
		else
		{
			this.playerAvatar.playerAvatarCollision.Collider.enabled = true;
		}
		array = this.colliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x00094914 File Offset: 0x00092B14
	public void BreakImpact()
	{
		if ((!SemiFunc.IsMultiplayer() || (this.playerAvatar && this.playerAvatar.isLocal)) && this.impactHurtTimer > 0f)
		{
			PlayerController.instance.CollisionController.ResetFalling();
			this.playerAvatar.playerHealth.Hurt(this.impactHurtDamage, true, -1);
			this.impactHurtTimer = 0f;
		}
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x00094984 File Offset: 0x00092B84
	public void ImpactHurtSet(float _time, int _damage)
	{
		if (!GameManager.Multiplayer())
		{
			this.ImpactHurtSetRPC(_time, _damage, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("ImpactHurtSetRPC", RpcTarget.All, new object[]
		{
			_time,
			_damage
		});
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x000949D4 File Offset: 0x00092BD4
	[PunRPC]
	public void ImpactHurtSetRPC(float _time, int _damage, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.playerAvatar.photonView))
		{
			return;
		}
		if (this.impactHurtTimer <= 0f || (this.impactHurtTimer <= _time && _damage == this.impactHurtDamage) || _damage > this.impactHurtDamage)
		{
			this.impactHurtTimer = _time;
			this.impactHurtDamage = _damage;
		}
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x00094A2B File Offset: 0x00092C2B
	private void BreakFree(Vector3 _direction)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("BreakFreeRPC", RpcTarget.All, new object[]
			{
				_direction
			});
		}
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x00094A54 File Offset: 0x00092C54
	[PunRPC]
	private void BreakFreeRPC(Vector3 _direction, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.playerAvatar.photonView))
		{
			return;
		}
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 2f, 5f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(2f, 2f, 5f, base.transform.position, 0.25f);
		this.playerAvatar.TumbleBreakFree();
		foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing)
		{
			if (physGrabber.playerAvatar.isLocal && Vector3.Dot((physGrabber.playerAvatar.PlayerVisionTarget.VisionTransform.position - base.transform.position).normalized, _direction) > 0.5f)
			{
				physGrabber.OverridePullDistanceIncrement(-1f);
			}
		}
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x00094B74 File Offset: 0x00092D74
	public void TumbleMoveSoundSet(bool _active, float _speed)
	{
		_speed = 1f - _speed;
		_speed = 1f + _speed * 0.25f;
		this.tumbleMoveSoundSpeed = _speed;
		this.tumbleMoveSoundTimer = 0.1f;
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x00094BA0 File Offset: 0x00092DA0
	private void OnDrawGizmos()
	{
		if (!this.isTumbling)
		{
			return;
		}
		float d = 0.1f;
		Gizmos.color = new Color(1f, 0.93f, 0.99f, 0.8f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one * d);
		Gizmos.color = new Color(0.28f, 1f, 0f, 0.5f);
		Gizmos.DrawCube(Vector3.zero, Vector3.one * d);
	}

	// Token: 0x04001B47 RID: 6983
	internal bool setup;

	// Token: 0x04001B48 RID: 6984
	public PlayerAvatar playerAvatar;

	// Token: 0x04001B49 RID: 6985
	public Transform followPosition;

	// Token: 0x04001B4A RID: 6986
	public ParticleSystem impactParticle;

	// Token: 0x04001B4B RID: 6987
	public Sound impactSound;

	// Token: 0x04001B4C RID: 6988
	[Space]
	public Collider[] colliders;

	// Token: 0x04001B4D RID: 6989
	public HurtCollider hurtCollider;

	// Token: 0x04001B4E RID: 6990
	[Space]
	public float customGravity = 10f;

	// Token: 0x04001B4F RID: 6991
	private float customGravityOverrideTimer;

	// Token: 0x04001B50 RID: 6992
	internal Rigidbody rb;

	// Token: 0x04001B51 RID: 6993
	internal PhysGrabObject physGrabObject;

	// Token: 0x04001B52 RID: 6994
	internal PhotonView photonView;

	// Token: 0x04001B53 RID: 6995
	internal bool isTumbling;

	// Token: 0x04001B54 RID: 6996
	private bool isTumblingPrevious = true;

	// Token: 0x04001B55 RID: 6997
	internal float tumbleSetTimer;

	// Token: 0x04001B56 RID: 6998
	internal float notMovingTimer;

	// Token: 0x04001B57 RID: 6999
	private Vector3 notMovingPositionLast;

	// Token: 0x04001B58 RID: 7000
	private Vector3 tumbleForce;

	// Token: 0x04001B59 RID: 7001
	private Vector3 tumbleTorque;

	// Token: 0x04001B5A RID: 7002
	private float tumbleForceTimer;

	// Token: 0x04001B5B RID: 7003
	private float tumbleOverrideTimer;

	// Token: 0x04001B5C RID: 7004
	internal bool tumbleOverride;

	// Token: 0x04001B5D RID: 7005
	private bool tumbleOverridePrevious;

	// Token: 0x04001B5E RID: 7006
	private float lookAtLerp;

	// Token: 0x04001B5F RID: 7007
	[Space]
	public Sound tumbleMoveSound;

	// Token: 0x04001B60 RID: 7008
	public Sound tumbleLaunchSound;

	// Token: 0x04001B61 RID: 7009
	private float tumbleMoveSoundTimer;

	// Token: 0x04001B62 RID: 7010
	private float tumbleMoveSoundSpeed;

	// Token: 0x04001B63 RID: 7011
	internal int tumbleLaunch;

	// Token: 0x04001B64 RID: 7012
	private float overrideEnemyHurtTimer;

	// Token: 0x04001B65 RID: 7013
	internal float impactHurtTimer;

	// Token: 0x04001B66 RID: 7014
	internal int impactHurtDamage;

	// Token: 0x04001B67 RID: 7015
	private float hurtColliderPauseTimer;

	// Token: 0x04001B68 RID: 7016
	private float breakFreeCooldown;

	// Token: 0x04001B69 RID: 7017
	internal bool isPlayerInputTriggered;
}
