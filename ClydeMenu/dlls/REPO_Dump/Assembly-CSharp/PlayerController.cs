using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001C6 RID: 454
public class PlayerController : MonoBehaviour
{
	// Token: 0x06000F92 RID: 3986 RVA: 0x0008C747 File Offset: 0x0008A947
	private void Awake()
	{
		PlayerController.instance = this;
	}

	// Token: 0x06000F93 RID: 3987 RVA: 0x0008C74F File Offset: 0x0008A94F
	public void PlayerSetName(string _playerName, string _steamID)
	{
		this.playerName = _playerName;
		this.playerSteamID = _steamID;
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x0008C75F File Offset: 0x0008A95F
	public void MoveForce(Vector3 direction, float amount, float time)
	{
		this.MoveForceDirection = direction.normalized;
		this.MoveForceAmount = amount;
		this.MoveForceTimer = time;
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x0008C77C File Offset: 0x0008A97C
	public void InputDisable(float time)
	{
		this.InputDisableTimer = time;
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x0008C785 File Offset: 0x0008A985
	public void MoveMult(float multiplier, float time)
	{
		this.MoveMultiplier = multiplier;
		this.MoveMultiplierTimer = time;
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x0008C795 File Offset: 0x0008A995
	public void CrouchDisable(float time)
	{
		this.CrouchInactiveTimer = Mathf.Max(time, this.CrouchInactiveTimer);
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x0008C7AC File Offset: 0x0008A9AC
	private void OnCollisionEnter(Collision other)
	{
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (other.gameObject.CompareTag("Phys Grab Object"))
		{
			this.playerAvatarScript.photonView.RPC("ResetPhysPusher", RpcTarget.MasterClient, Array.Empty<object>());
		}
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x0008C7FC File Offset: 0x0008A9FC
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.rbOriginalMass = this.rb.mass;
		this.rbOriginalDrag = this.rb.drag;
		this.AudioSource = base.GetComponent<AudioSource>();
		this.positionPrevious = base.transform.position;
		Inventory component = base.GetComponent<Inventory>();
		if (SemiFunc.RunIsArena())
		{
			component.enabled = false;
		}
		if (GameManager.instance.gameMode == 0)
		{
			Object.Instantiate<GameObject>(this.playerAvatarPrefab, base.transform.position, Quaternion.identity);
		}
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06000F9A RID: 3994 RVA: 0x0008C89E File Offset: 0x0008AA9E
	private IEnumerator LateStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.2f);
		string text = SemiFunc.PlayerGetSteamID(this.playerAvatarScript);
		if (StatsManager.instance.playerUpgradeStamina.ContainsKey(text))
		{
			this.EnergyStart += (float)StatsManager.instance.playerUpgradeStamina[text] * 10f;
			this.SprintSpeed += (float)StatsManager.instance.playerUpgradeSpeed[text];
			this.SprintSpeedUpgrades += (float)StatsManager.instance.playerUpgradeSpeed[text];
			this.JumpExtra = StatsManager.instance.playerUpgradeExtraJump[text];
		}
		this.EnergyCurrent = this.EnergyStart;
		this.playerOriginalMoveSpeed = this.MoveSpeed;
		this.playerOriginalSprintSpeed = this.SprintSpeed;
		this.playerOriginalCrouchSpeed = this.CrouchSpeed;
		this.playerOriginalCustomGravity = this.CustomGravity;
		if (SemiFunc.MenuLevel())
		{
			this.rb.isKinematic = true;
			base.gameObject.SetActive(false);
		}
		yield break;
	}

	// Token: 0x06000F9B RID: 3995 RVA: 0x0008C8AD File Offset: 0x0008AAAD
	public void ChangeState()
	{
		this.playerAvatarScript.UpdateState(this.Crouching, this.sprinting, this.Crawling, this.Sliding, this.moving);
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x0008C8D8 File Offset: 0x0008AAD8
	public void ForceImpulse(Vector3 force)
	{
		this.VelocityImpulse += base.transform.InverseTransformDirection(force);
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x0008C8F7 File Offset: 0x0008AAF7
	public void AntiGravity(float _timer)
	{
		this.antiGravityTimer = _timer;
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x0008C900 File Offset: 0x0008AB00
	public void Feather(float _timer)
	{
		this.featherTimer = _timer;
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x0008C909 File Offset: 0x0008AB09
	public void Kinematic(float _timer)
	{
		this.kinematicTimer = _timer;
		this.rb.isKinematic = true;
		this.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x0008C92C File Offset: 0x0008AB2C
	public void SetCrawl()
	{
		this.Crouching = true;
		this.Crawling = true;
		this.CrouchActiveTimer = this.CrouchTimeMin;
		this.sprinting = false;
		this.Sliding = false;
		this.moving = false;
		PlayerCollisionStand.instance.SetBlocked();
		CameraCrouchPosition.instance.Lerp = 1f;
		CameraCrouchPosition.instance.Active = true;
		CameraCrouchPosition.instance.ActivePrev = true;
		this.ChangeState();
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x0008C99D File Offset: 0x0008AB9D
	public void OverrideSpeed(float _speedMulti, float _time = 0.1f)
	{
		this.overrideSpeedTimer = _time;
		this.overrideSpeedMultiplier = _speedMulti;
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x0008C9B0 File Offset: 0x0008ABB0
	private void OverrideSpeedTick()
	{
		if (this.overrideSpeedTimer > 0f)
		{
			this.overrideSpeedTimer -= Time.fixedDeltaTime;
			if (this.overrideSpeedTimer <= 0f)
			{
				this.overrideSpeedMultiplier = 1f;
				this.MoveSpeed = this.playerOriginalMoveSpeed;
				this.SprintSpeed = this.playerOriginalSprintSpeed;
				this.CrouchSpeed = this.playerOriginalCrouchSpeed;
			}
		}
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x0008CA18 File Offset: 0x0008AC18
	private void OverrideSpeedLogic()
	{
		if (this.overrideSpeedTimer <= 0f)
		{
			return;
		}
		this.MoveSpeed = this.playerOriginalMoveSpeed * this.overrideSpeedMultiplier;
		this.SprintSpeed = this.playerOriginalSprintSpeed * this.overrideSpeedMultiplier;
		this.CrouchSpeed = this.playerOriginalCrouchSpeed * this.overrideSpeedMultiplier;
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x0008CA6C File Offset: 0x0008AC6C
	public void OverrideAnimationSpeed(float _animSpeedMulti, float _timeIn, float _timeOut, float _time = 0.1f)
	{
		this.playerAvatarScript.OverrideAnimationSpeed(_animSpeedMulti, _timeIn, _timeOut, _time);
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x0008CA7E File Offset: 0x0008AC7E
	public void OverrideTimeScale(float _timeScaleMulti, float _time = 0.1f)
	{
		this.overrideTimeScaleTimer = _time;
		this.overrideTimeScaleMultiplier = _timeScaleMulti;
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x0008CA90 File Offset: 0x0008AC90
	private void OverrideTimeScaleTick()
	{
		if (this.overrideTimeScaleTimer > 0f)
		{
			this.overrideTimeScaleTimer -= Time.fixedDeltaTime;
			if (this.overrideTimeScaleTimer <= 0f)
			{
				this.overrideTimeScaleMultiplier = 1f;
				this.rb.mass = this.rbOriginalMass;
				this.rb.drag = this.rbOriginalDrag;
				this.CustomGravity = this.playerOriginalCustomGravity;
				this.MoveSpeed = this.playerOriginalMoveSpeed;
				this.SprintSpeed = this.playerOriginalSprintSpeed;
				this.CrouchSpeed = this.playerOriginalCrouchSpeed;
				this.rb.useGravity = true;
			}
		}
	}

	// Token: 0x06000FA7 RID: 4007 RVA: 0x0008CB38 File Offset: 0x0008AD38
	private void OverrideTimeScaleLogic()
	{
		if (this.overrideTimeScaleTimer <= 0f)
		{
			return;
		}
		float t = this.overrideSpeedMultiplier;
		float y = this.rb.velocity.y;
		this.rb.velocity = Vector3.Lerp(Vector3.zero, this.rb.velocity, t);
		this.rb.velocity = new Vector3(this.rb.velocity.x, y, this.rb.velocity.z);
		this.rb.angularVelocity = Vector3.Lerp(Vector3.zero, this.rb.angularVelocity, t);
		this.rb.mass = Mathf.Lerp(0.01f, this.rbOriginalMass, t);
		this.rb.drag = Mathf.Lerp((1f + this.overrideSpeedMultiplier) * 10f, this.rbOriginalDrag, t);
		this.CustomGravity = Mathf.Lerp(0.1f, this.playerOriginalCustomGravity, t);
		this.MoveSpeed = Mathf.Lerp(0.1f, this.playerOriginalMoveSpeed, t);
		this.SprintSpeed = Mathf.Lerp(0.1f, this.playerOriginalSprintSpeed, t);
		this.CrouchSpeed = Mathf.Lerp(0.1f, this.playerOriginalCrouchSpeed, t);
		this.rb.useGravity = false;
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x0008CC8B File Offset: 0x0008AE8B
	public void OverrideLookSpeed(float _lookSpeedTarget, float timeIn, float timeOut, float _time = 0.1f)
	{
		this.overrideLookSpeedTimer = _time;
		this.overrideLookSpeedTarget = _lookSpeedTarget;
		this.overrideLookSpeedTimeIn = timeIn;
		this.overrideLookSpeedTimeOut = timeOut;
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x0008CCAA File Offset: 0x0008AEAA
	private void OverrideLookSpeedTick()
	{
		if (this.overrideLookSpeedTimer > 0f)
		{
			this.overrideLookSpeedTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x0008CCCC File Offset: 0x0008AECC
	private void OverrideLookSpeedLogic()
	{
		if (this.overrideLookSpeedTimer <= 0f && this.overrideLookSpeedProgress <= 0f)
		{
			return;
		}
		float smooth;
		if (this.overrideLookSpeedTimer > 0f)
		{
			this.overrideLookSpeedProgress += Time.fixedDeltaTime / this.overrideLookSpeedTimeIn;
			this.overrideLookSpeedProgress = Mathf.Clamp01(this.overrideLookSpeedProgress);
			this.overrideLookSpeedLerp = Mathf.SmoothStep(0f, 1f, this.overrideLookSpeedProgress);
			smooth = Mathf.Lerp(this.cameraAim.aimSmoothOriginal, this.overrideLookSpeedTarget, this.overrideLookSpeedLerp);
		}
		else
		{
			this.overrideLookSpeedProgress -= Time.fixedDeltaTime / this.overrideLookSpeedTimeOut;
			this.overrideLookSpeedProgress = Mathf.Clamp01(this.overrideLookSpeedProgress);
			this.overrideLookSpeedLerp = Mathf.SmoothStep(0f, 1f, this.overrideLookSpeedProgress);
			smooth = Mathf.Lerp(this.cameraAim.aimSmoothOriginal, this.overrideLookSpeedTarget, this.overrideLookSpeedLerp);
			if (this.overrideLookSpeedProgress <= 0f)
			{
				smooth = this.cameraAim.aimSmoothOriginal;
			}
		}
		this.cameraAim.OverrideAimSmooth(smooth, 0.1f);
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x0008CDF1 File Offset: 0x0008AFF1
	public void OverrideVoicePitch(float _voicePitchMulti, float _timeIn, float _timeOut, float _time = 0.1f)
	{
		if (this.playerAvatarScript.voiceChat)
		{
			this.playerAvatarScript.voiceChat.OverridePitch(_voicePitchMulti, _timeIn, _timeOut, _time, 0f, 0f);
		}
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x0008CE24 File Offset: 0x0008B024
	private void OverrideVoicePitchTick()
	{
		if (this.overrideVoicePitchTimer > 0f)
		{
			this.overrideVoicePitchTimer -= Time.fixedDeltaTime;
			if (this.overrideVoicePitchTimer <= 0f)
			{
				this.overrideVoicePitchMultiplier = 1f;
			}
		}
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x0008CE5D File Offset: 0x0008B05D
	public void OverrideJumpCooldown(float _cooldown)
	{
		this.OverrideJumpCooldownAmount = _cooldown;
		this.OverrideJumpCooldownTimer = 0.1f;
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x0008CE74 File Offset: 0x0008B074
	private void FixedUpdate()
	{
		if (GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			this.OverrideSpeedTick();
			this.OverrideTimeScaleTick();
			this.OverrideLookSpeedTick();
			this.OverrideVoicePitchTick();
			if (this.kinematicTimer > 0f)
			{
				this.VelocityImpulse = Vector3.zero;
				this.rb.isKinematic = true;
				this.kinematicTimer -= Time.fixedDeltaTime;
				if (this.kinematicTimer <= 0f)
				{
					this.rb.isKinematic = false;
				}
				return;
			}
			if (this.playerAvatarScript.isTumbling)
			{
				base.transform.position = this.playerAvatarScript.tumble.transform.position + Vector3.down * 0.3f;
			}
			if (this.Crawling != this.previousCrawlingState)
			{
				this.ChangeState();
				this.previousCrawlingState = this.Crawling;
			}
			if (this.Crouching != this.previousCrouchingState)
			{
				this.ChangeState();
				this.previousCrouchingState = this.Crouching;
			}
			if (this.sprinting != this.previousSprintingState)
			{
				this.ChangeState();
				this.previousSprintingState = this.sprinting;
			}
			if (this.Sliding != this.previousSlidingState)
			{
				this.ChangeState();
				this.previousSlidingState = this.Sliding;
			}
			if (this.moving != this.previousMovingState)
			{
				this.ChangeState();
				this.previousMovingState = this.moving;
			}
			base.transform.rotation = Quaternion.Euler(0f, this.cameraGameObject.transform.localRotation.eulerAngles.y, 0f);
			if ((SemiFunc.InputHold(InputKey.Sprint) || this.toggleSprint) && !this.playerAvatarScript.isTumbling && !this.Crouching && this.EnergyCurrent >= 1f)
			{
				if (this.rb.velocity.magnitude > 0.01f)
				{
					this.CanSlide = true;
					TutorialDirector.instance.playerSprinted = true;
					this.sprinting = true;
					this.SprintedTimer = 0.5f;
					this.SprintDrainTimer = 0.2f;
				}
			}
			else
			{
				if (this.SprintedTimer > 0f)
				{
					this.SprintedTimer -= Time.fixedDeltaTime;
					if (this.SprintedTimer <= 0f)
					{
						this.CanSlide = false;
						this.SprintedTimer = 0f;
					}
				}
				this.SprintSpeedLerp = 0f;
				this.sprinting = false;
			}
			if (this.SprintDrainTimer > 0f && !this.DebugEnergy)
			{
				float num = this.EnergySprintDrain;
				num += this.SprintSpeedUpgrades;
				this.EnergyCurrent -= num * Time.fixedDeltaTime;
				this.EnergyCurrent = Mathf.Max(0f, this.EnergyCurrent);
				if (this.EnergyCurrent <= 0f)
				{
					this.toggleSprint = false;
				}
				this.SprintDrainTimer -= Time.fixedDeltaTime;
			}
			if ((this.Crouching && PlayerCollisionStand.instance.CheckBlocked()) || this.playerAvatarScript.isTumbling)
			{
				TutorialDirector.instance.playerCrawled = true;
				this.Crawling = true;
			}
			else
			{
				this.Crawling = false;
			}
			if (this.playerAvatarScript.isTumbling || (this.CollisionController.Grounded && (SemiFunc.InputHold(InputKey.Crouch) || this.toggleCrouch)))
			{
				if (this.CrouchInactiveTimer <= 0f)
				{
					if (!this.Crouching)
					{
						this.CrouchActiveTimer = this.CrouchTimeMin;
					}
					TutorialDirector.instance.playerCrouched = true;
					this.Crouching = true;
					this.sprinting = false;
				}
			}
			else if (this.Crouching && this.CrouchActiveTimer <= 0f && !this.Crawling)
			{
				this.Crawling = false;
				this.Crouching = false;
				this.CrouchInactiveTimer = this.CrouchTimeMin;
			}
			if (this.CrouchActiveTimer > 0f)
			{
				this.CrouchActiveTimer -= Time.fixedDeltaTime;
			}
			if (this.CrouchInactiveTimer > 0f)
			{
				this.CrouchInactiveTimer -= Time.fixedDeltaTime;
			}
			if (this.sprinting || this.Crouching)
			{
				this.CanInteract = false;
			}
			else
			{
				this.CanInteract = true;
			}
			Vector3 vector = Vector3.zero;
			if (this.MoveForceTimer > 0f)
			{
				this.InputDirection = this.MoveForceDirection;
				this.MoveForceTimer -= Time.fixedDeltaTime;
				this.rb.velocity = this.MoveForceDirection * this.MoveForceAmount;
			}
			else if (this.InputDisableTimer <= 0f)
			{
				this.InputDirection = new Vector3(SemiFunc.InputMovementX(), 0f, SemiFunc.InputMovementY()).normalized;
				if (GameDirector.instance.DisableInput || this.playerAvatarScript.isTumbling)
				{
					this.InputDirection = Vector3.zero;
				}
				if (this.InputDirection.magnitude <= 0.1f)
				{
					this.SprintSpeedLerp = 0f;
				}
				if (this.MoveMultiplierTimer > 0f)
				{
					this.InputDirection *= this.MoveMultiplier;
				}
				if (this.sprinting)
				{
					this.SprintSpeedCurrent = Mathf.Lerp(this.MoveSpeed, this.SprintSpeed, this.SprintSpeedLerp);
					this.SprintSpeedLerp += this.SprintAcceleration * Time.fixedDeltaTime;
					this.SprintSpeedLerp = Mathf.Clamp01(this.SprintSpeedLerp);
					vector += this.InputDirection * this.SprintSpeedCurrent;
					this.SlideDirection = this.InputDirection * this.SprintSpeedCurrent;
					this.SlideDirectionCurrent = this.SlideDirection;
					this.Sliding = false;
				}
				else if (this.Crouching)
				{
					if (this.CanSlide)
					{
						this.playerAvatarScript.Slide();
						if (!this.DebugEnergy)
						{
							this.EnergyCurrent -= 5f;
						}
						this.EnergyCurrent = Mathf.Max(0f, this.EnergyCurrent);
						this.CanSlide = false;
						this.Sliding = true;
						this.SlideTimer = this.SlideTime;
					}
					if (this.SlideTimer > 0f)
					{
						vector += this.SlideDirectionCurrent;
						this.SlideDirectionCurrent -= this.SlideDirection * this.SlideDecay * Time.fixedDeltaTime;
						this.SlideTimer -= Time.fixedDeltaTime;
						if (this.SlideTimer <= 0f)
						{
							this.Sliding = false;
						}
					}
					if (this.debugSlow)
					{
						this.InputDirection *= 0.2f;
					}
					vector += this.InputDirection * this.CrouchSpeed;
				}
				else
				{
					if (this.debugSlow)
					{
						this.InputDirection *= 0.1f;
					}
					vector += this.InputDirection * this.MoveSpeed;
					this.Sliding = false;
				}
			}
			else
			{
				this.InputDirection = Vector3.zero;
			}
			if (this.InputDisableTimer > 0f)
			{
				this.InputDisableTimer -= Time.fixedDeltaTime;
			}
			if (this.MoveMultiplierTimer > 0f)
			{
				this.MoveMultiplierTimer -= Time.fixedDeltaTime;
			}
			if (this.antiGravityTimer > 0f)
			{
				if (this.rb.useGravity)
				{
					this.rb.drag = 2f;
					this.rb.useGravity = false;
				}
				this.antiGravityTimer -= Time.fixedDeltaTime;
			}
			else if (!this.rb.useGravity)
			{
				this.rb.drag = 0f;
				this.rb.useGravity = true;
			}
			vector += this.VelocityImpulse;
			this.VelocityRelativeNew += this.VelocityImpulse;
			this.VelocityImpulse = Vector3.zero;
			if (this.VelocityIdle)
			{
				this.VelocityRelativeNew = vector;
			}
			else
			{
				this.VelocityRelativeNew = Vector3.Lerp(this.VelocityRelativeNew, vector, this.MoveFriction * Time.fixedDeltaTime);
			}
			Vector3 vector2 = base.transform.InverseTransformDirection(this.rb.velocity);
			if (this.VelocityRelativeNew.magnitude > 0.1f)
			{
				this.VelocityIdle = false;
				this.col.material = this.PhysicMaterialMove;
				this.VelocityRelative = Vector3.Lerp(this.VelocityRelative, this.VelocityRelativeNew, this.MoveFriction * Time.fixedDeltaTime);
				this.VelocityRelative.y = vector2.y;
				this.rb.AddRelativeForce(this.VelocityRelative - vector2, ForceMode.Impulse);
				this.Velocity = base.transform.InverseTransformDirection(this.VelocityRelative - vector2);
			}
			else
			{
				this.VelocityIdle = true;
				this.col.material = this.PhysicMaterialIdle;
				this.VelocityRelative = Vector3.zero;
				this.Velocity = this.rb.velocity;
			}
			if (!this.CollisionController.Grounded && !this.JumpImpulse && this.featherTimer <= 0f)
			{
				if (this.rb.useGravity)
				{
					this.rb.AddForce(new Vector3(0f, -this.CustomGravity * Time.fixedDeltaTime, 0f), ForceMode.Impulse);
				}
				else
				{
					this.rb.AddForce(new Vector3(0f, -(this.CustomGravity * 0.1f) * Time.fixedDeltaTime, 0f), ForceMode.Impulse);
				}
			}
			if (this.JumpImpulse)
			{
				bool flag = false;
				foreach (PhysGrabObject physGrabObject in this.JumpGroundedObjects)
				{
					foreach (PhysGrabber physGrabber in physGrabObject.playerGrabbing)
					{
						if (physGrabber.playerAvatar == this.playerAvatarScript)
						{
							flag = true;
							physGrabber.ReleaseObject(0.1f);
							physGrabber.grabDisableTimer = 1f;
							break;
						}
					}
				}
				Vector3 b = new Vector3(0f, this.rb.velocity.y, 0f);
				float d = this.JumpForce;
				if (flag)
				{
					d = this.JumpForce * 0.5f;
				}
				this.rb.AddForce(Vector3.up * d - b, ForceMode.Impulse);
				this.JumpCooldown = 0.1f;
				this.JumpImpulse = false;
				this.CollisionGrounded.Grounded = false;
				this.JumpGroundedBuffer = 0f;
				this.CollisionController.GroundedDisableTimer = 0.1f;
				this.CollisionController.fallDistance = 0f;
			}
			if (this.VelocityRelativeNew.magnitude > 0.1f)
			{
				this.movingResetTimer = 0.1f;
				this.moving = true;
			}
			else if (this.movingResetTimer > 0f)
			{
				this.movingResetTimer -= Time.fixedDeltaTime;
				if (this.movingResetTimer <= 0f)
				{
					this.sprinting = false;
					this.moving = false;
				}
			}
			if (this.featherTimer > 0f)
			{
				if (this.rb.useGravity)
				{
					this.rb.useGravity = false;
				}
				if (this.antiGravityTimer <= 0f)
				{
					this.rb.AddForce(new Vector3(0f, -15f, 0f), ForceMode.Force);
				}
				this.featherTimer -= Time.fixedDeltaTime;
				if (this.featherTimer <= 0f)
				{
					this.rb.useGravity = true;
				}
			}
			this.OverrideTimeScaleLogic();
			this.OverrideSpeedLogic();
			this.OverrideLookSpeedLogic();
			this.positionPrevious = base.transform.position;
		}
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x0008DA44 File Offset: 0x0008BC44
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated || SemiFunc.MenuLevel())
		{
			return;
		}
		if (this.deathSeenTimer > 0f)
		{
			this.deathSeenTimer -= Time.deltaTime;
		}
		if (this.CollisionController.Grounded)
		{
			if (InputManager.instance.InputToggleGet(InputKey.Crouch))
			{
				if (SemiFunc.InputDown(InputKey.Crouch))
				{
					this.toggleCrouch = !this.toggleCrouch;
					if (this.toggleCrouch)
					{
						this.toggleSprint = false;
					}
				}
			}
			else
			{
				this.toggleCrouch = false;
			}
		}
		if (!this.playerAvatarScript.isTumbling)
		{
			if (InputManager.instance.InputToggleGet(InputKey.Sprint))
			{
				if (SemiFunc.InputDown(InputKey.Sprint))
				{
					this.toggleSprint = !this.toggleSprint;
					if (this.toggleSprint)
					{
						this.toggleCrouch = false;
					}
				}
			}
			else
			{
				this.toggleSprint = false;
			}
		}
		if (this.sprinting)
		{
			this.sprintRechargeTimer = this.sprintRechargeTime;
			if (SemiFunc.RunIsArena())
			{
				this.sprintRechargeTimer *= 0.5f;
			}
		}
		else if (this.sprintRechargeTimer > 0f)
		{
			this.sprintRechargeTimer -= Time.deltaTime;
		}
		else if (this.EnergyCurrent < this.EnergyStart)
		{
			float num = this.sprintRechargeAmount;
			if (SemiFunc.RunIsArena())
			{
				num *= 5f;
			}
			this.EnergyCurrent += num * Time.deltaTime;
			if (this.EnergyCurrent > this.EnergyStart)
			{
				this.EnergyCurrent = this.EnergyStart;
			}
		}
		if (!this.JumpImpulse)
		{
			if (SemiFunc.InputDown(InputKey.Jump) && !this.playerAvatarScript.isTumbling && this.InputDisableTimer <= 0f && this.OverrideJumpCooldownCurrent <= 0f)
			{
				this.JumpInputBuffer = 0.25f;
				if (this.OverrideJumpCooldownTimer > 0f)
				{
					this.OverrideJumpCooldownCurrent = this.OverrideJumpCooldownAmount;
				}
			}
			if (this.OverrideJumpCooldownTimer > 0f)
			{
				this.OverrideJumpCooldownTimer -= Time.deltaTime;
			}
			if (this.OverrideJumpCooldownCurrent > 0f)
			{
				this.OverrideJumpCooldownCurrent -= Time.deltaTime;
			}
			if (this.CollisionGrounded.Grounded)
			{
				this.JumpFirst = true;
				this.JumpExtraCurrent = this.JumpExtra;
				this.JumpGroundedBuffer = 0.25f;
			}
			else if (this.JumpGroundedBuffer > 0f)
			{
				this.JumpGroundedBuffer -= Time.deltaTime;
				if (this.JumpGroundedBuffer <= 0f)
				{
					this.JumpFirst = false;
				}
			}
			if (this.JumpInputBuffer > 0f)
			{
				this.JumpInputBuffer -= Time.deltaTime;
			}
			if (this.JumpCooldown > 0f)
			{
				this.JumpCooldown -= Time.deltaTime;
			}
			if (this.JumpInputBuffer > 0f && (this.JumpGroundedBuffer > 0f || (!this.JumpFirst && this.JumpExtraCurrent > 0)) && this.JumpCooldown <= 0f)
			{
				if (this.JumpFirst)
				{
					this.JumpFirst = false;
					this.playerAvatarScript.Jump(false);
				}
				else
				{
					this.JumpExtraCurrent--;
					this.playerAvatarScript.Jump(true);
				}
				CameraJump.instance.Jump();
				TutorialDirector.instance.playerJumped = true;
				this.JumpImpulse = true;
				this.JumpInputBuffer = 0f;
			}
			if (this.JumpGroundedBuffer <= 0f && this.JumpGroundedObjects.Count > 0)
			{
				this.JumpGroundedObjects.Clear();
			}
		}
		if (this.landCooldown > 0f)
		{
			this.landCooldown -= Time.deltaTime;
		}
		if (this.rb.velocity.y < -4f || (this.playerAvatarScript.tumble && this.playerAvatarScript.tumble.physGrabObject.rbVelocity.y < -4f))
		{
			this.CanLand = true;
		}
		if (this.GroundedPrevious != this.CollisionController.Grounded)
		{
			if (this.CollisionController.Grounded && this.CanLand)
			{
				if (!SemiFunc.MenuLevel() && this.landCooldown <= 0f)
				{
					this.landCooldown = 1f;
					CameraJump.instance.Land();
					this.playerAvatarScript.Land();
				}
				this.CanLand = false;
			}
			this.GroundedPrevious = this.CollisionController.Grounded;
		}
		if (this.tumbleInputDisableTimer > 0f)
		{
			this.tumbleInputDisableTimer -= Time.deltaTime;
		}
		if (this.playerAvatarScript.isTumbling)
		{
			this.col.enabled = false;
			this.rb.isKinematic = true;
			bool flag = false;
			if (this.playerAvatarScript.tumble.notMovingTimer > 0.5f && (Mathf.Abs(SemiFunc.InputMovementX()) > 0f || Mathf.Abs(SemiFunc.InputMovementY()) > 0f))
			{
				flag = true;
			}
			if ((SemiFunc.InputDown(InputKey.Jump) || SemiFunc.InputDown(InputKey.Tumble) || flag) && this.tumbleInputDisableTimer <= 0f && !this.playerAvatarScript.tumble.tumbleOverride && this.InputDisableTimer <= 0f)
			{
				this.playerAvatarScript.tumble.TumbleRequest(false, true);
				return;
			}
		}
		else
		{
			this.col.enabled = true;
			this.rb.isKinematic = false;
			if (SemiFunc.InputDown(InputKey.Tumble) && this.tumbleInputDisableTimer <= 0f && this.InputDisableTimer <= 0f)
			{
				TutorialDirector.instance.playerTumbled = true;
				this.playerAvatarScript.tumble.TumbleRequest(true, true);
			}
		}
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x0008DFD4 File Offset: 0x0008C1D4
	public void Revive(Vector3 _rotation)
	{
		base.transform.rotation = Quaternion.Euler(0f, _rotation.y, 0f);
		this.InputDisable(0.5f);
		this.Kinematic(0.2f);
		this.SetCrawl();
		this.CollisionController.ResetFalling();
		this.VelocityIdle = true;
		this.col.material = this.PhysicMaterialIdle;
		this.VelocityRelative = Vector3.zero;
		this.Velocity = Vector3.zero;
		this.EnergyCurrent = this.EnergyStart;
	}

	// Token: 0x040019DD RID: 6621
	public static PlayerController instance;

	// Token: 0x040019DE RID: 6622
	private bool previousCrouchingState;

	// Token: 0x040019DF RID: 6623
	private bool previousCrawlingState;

	// Token: 0x040019E0 RID: 6624
	private bool previousSprintingState;

	// Token: 0x040019E1 RID: 6625
	private bool previousSlidingState;

	// Token: 0x040019E2 RID: 6626
	private bool previousMovingState;

	// Token: 0x040019E3 RID: 6627
	public GameObject playerAvatar;

	// Token: 0x040019E4 RID: 6628
	public GameObject playerAvatarPrefab;

	// Token: 0x040019E5 RID: 6629
	public PlayerCollision PlayerCollision;

	// Token: 0x040019E6 RID: 6630
	[HideInInspector]
	public GameObject physGrabObject;

	// Token: 0x040019E7 RID: 6631
	[HideInInspector]
	public bool physGrabActive;

	// Token: 0x040019E8 RID: 6632
	[HideInInspector]
	public Transform physGrabPoint;

	// Token: 0x040019E9 RID: 6633
	[Space]
	public PlayerCollisionController CollisionController;

	// Token: 0x040019EA RID: 6634
	public PlayerCollisionGrounded CollisionGrounded;

	// Token: 0x040019EB RID: 6635
	private bool GroundedPrevious;

	// Token: 0x040019EC RID: 6636
	public Materials.MaterialTrigger MaterialTrigger;

	// Token: 0x040019ED RID: 6637
	[HideInInspector]
	public Rigidbody rb;

	// Token: 0x040019EE RID: 6638
	private bool CanLand;

	// Token: 0x040019EF RID: 6639
	private float landCooldown;

	// Token: 0x040019F0 RID: 6640
	[Space]
	public float MoveSpeed = 0.5f;

	// Token: 0x040019F1 RID: 6641
	public float MoveFriction = 5f;

	// Token: 0x040019F2 RID: 6642
	[HideInInspector]
	public Vector3 Velocity;

	// Token: 0x040019F3 RID: 6643
	[HideInInspector]
	public Vector3 VelocityRelative;

	// Token: 0x040019F4 RID: 6644
	private Vector3 VelocityRelativeNew;

	// Token: 0x040019F5 RID: 6645
	private bool VelocityIdle;

	// Token: 0x040019F6 RID: 6646
	private Vector3 VelocityImpulse = Vector3.zero;

	// Token: 0x040019F7 RID: 6647
	[Space]
	public float SprintSpeed = 1f;

	// Token: 0x040019F8 RID: 6648
	public float SprintSpeedUpgrades;

	// Token: 0x040019F9 RID: 6649
	private float SprintSpeedCurrent = 1f;

	// Token: 0x040019FA RID: 6650
	[HideInInspector]
	public float SprintSpeedLerp;

	// Token: 0x040019FB RID: 6651
	public float SprintAcceleration = 1f;

	// Token: 0x040019FC RID: 6652
	private float SprintedTimer;

	// Token: 0x040019FD RID: 6653
	private float SprintDrainTimer;

	// Token: 0x040019FE RID: 6654
	[Space]
	public float CrouchSpeed = 1f;

	// Token: 0x040019FF RID: 6655
	public float CrouchTimeMin = 0.2f;

	// Token: 0x04001A00 RID: 6656
	private float CrouchActiveTimer;

	// Token: 0x04001A01 RID: 6657
	private float CrouchInactiveTimer;

	// Token: 0x04001A02 RID: 6658
	[Space]
	public float SlideTime = 1f;

	// Token: 0x04001A03 RID: 6659
	public float SlideDecay = 0.1f;

	// Token: 0x04001A04 RID: 6660
	private float SlideTimer;

	// Token: 0x04001A05 RID: 6661
	private Vector3 SlideDirection;

	// Token: 0x04001A06 RID: 6662
	private Vector3 SlideDirectionCurrent;

	// Token: 0x04001A07 RID: 6663
	internal bool CanSlide;

	// Token: 0x04001A08 RID: 6664
	[HideInInspector]
	public bool Sliding;

	// Token: 0x04001A09 RID: 6665
	[Space]
	public float JumpForce = 20f;

	// Token: 0x04001A0A RID: 6666
	internal int JumpExtra;

	// Token: 0x04001A0B RID: 6667
	private int JumpExtraCurrent;

	// Token: 0x04001A0C RID: 6668
	private bool JumpFirst;

	// Token: 0x04001A0D RID: 6669
	public float CustomGravity = 20f;

	// Token: 0x04001A0E RID: 6670
	internal bool JumpImpulse;

	// Token: 0x04001A0F RID: 6671
	private float JumpCooldown;

	// Token: 0x04001A10 RID: 6672
	private float JumpGroundedBuffer;

	// Token: 0x04001A11 RID: 6673
	internal List<PhysGrabObject> JumpGroundedObjects = new List<PhysGrabObject>();

	// Token: 0x04001A12 RID: 6674
	private float JumpInputBuffer;

	// Token: 0x04001A13 RID: 6675
	private float OverrideJumpCooldownAmount;

	// Token: 0x04001A14 RID: 6676
	private float OverrideJumpCooldownCurrent;

	// Token: 0x04001A15 RID: 6677
	private float OverrideJumpCooldownTimer;

	// Token: 0x04001A16 RID: 6678
	public float StepUpForce = 2f;

	// Token: 0x04001A17 RID: 6679
	public bool DebugNoTumble;

	// Token: 0x04001A18 RID: 6680
	public bool DebugDisableOvercharge;

	// Token: 0x04001A19 RID: 6681
	[Space]
	public bool DebugEnergy;

	// Token: 0x04001A1A RID: 6682
	public float EnergyStart = 100f;

	// Token: 0x04001A1B RID: 6683
	[HideInInspector]
	public float EnergyCurrent;

	// Token: 0x04001A1C RID: 6684
	public float EnergySprintDrain = 1f;

	// Token: 0x04001A1D RID: 6685
	private float sprintRechargeTimer;

	// Token: 0x04001A1E RID: 6686
	private float sprintRechargeTime = 1f;

	// Token: 0x04001A1F RID: 6687
	private float sprintRechargeAmount = 2f;

	// Token: 0x04001A20 RID: 6688
	[Space(15f)]
	public CameraAim cameraAim;

	// Token: 0x04001A21 RID: 6689
	public GameObject cameraGameObject;

	// Token: 0x04001A22 RID: 6690
	public GameObject cameraGameObjectLocal;

	// Token: 0x04001A23 RID: 6691
	public Transform VisionTarget;

	// Token: 0x04001A24 RID: 6692
	[HideInInspector]
	public bool CanInteract;

	// Token: 0x04001A25 RID: 6693
	[HideInInspector]
	public bool moving;

	// Token: 0x04001A26 RID: 6694
	private float movingResetTimer;

	// Token: 0x04001A27 RID: 6695
	[HideInInspector]
	public bool sprinting;

	// Token: 0x04001A28 RID: 6696
	[HideInInspector]
	public bool Crouching;

	// Token: 0x04001A29 RID: 6697
	[HideInInspector]
	public bool Crawling;

	// Token: 0x04001A2A RID: 6698
	[HideInInspector]
	public Vector3 InputDirection;

	// Token: 0x04001A2B RID: 6699
	[HideInInspector]
	public AudioSource AudioSource;

	// Token: 0x04001A2C RID: 6700
	private Vector3 positionPrevious;

	// Token: 0x04001A2D RID: 6701
	private Vector3 MoveForceDirection = Vector3.zero;

	// Token: 0x04001A2E RID: 6702
	private float MoveForceAmount;

	// Token: 0x04001A2F RID: 6703
	private float MoveForceTimer;

	// Token: 0x04001A30 RID: 6704
	internal float InputDisableTimer;

	// Token: 0x04001A31 RID: 6705
	private float MoveMultiplier = 1f;

	// Token: 0x04001A32 RID: 6706
	private float MoveMultiplierTimer;

	// Token: 0x04001A33 RID: 6707
	internal string playerName;

	// Token: 0x04001A34 RID: 6708
	internal string playerSteamID;

	// Token: 0x04001A35 RID: 6709
	private float overrideSpeedTimer;

	// Token: 0x04001A36 RID: 6710
	internal float overrideSpeedMultiplier = 1f;

	// Token: 0x04001A37 RID: 6711
	private float overrideLookSpeedTimer;

	// Token: 0x04001A38 RID: 6712
	internal float overrideLookSpeedTarget = 1f;

	// Token: 0x04001A39 RID: 6713
	private float overrideLookSpeedTimeIn = 15f;

	// Token: 0x04001A3A RID: 6714
	private float overrideLookSpeedTimeOut = 0.3f;

	// Token: 0x04001A3B RID: 6715
	private float overrideLookSpeedLerp;

	// Token: 0x04001A3C RID: 6716
	private float overrideLookSpeedProgress;

	// Token: 0x04001A3D RID: 6717
	private float overrideVoicePitchTimer;

	// Token: 0x04001A3E RID: 6718
	internal float overrideVoicePitchMultiplier = 1f;

	// Token: 0x04001A3F RID: 6719
	private float overrideTimeScaleTimer;

	// Token: 0x04001A40 RID: 6720
	internal float overrideTimeScaleMultiplier = 1f;

	// Token: 0x04001A41 RID: 6721
	private Vector3 originalVelocity;

	// Token: 0x04001A42 RID: 6722
	private Vector3 originalAngularVelocity;

	// Token: 0x04001A43 RID: 6723
	public PlayerAvatar playerAvatarScript;

	// Token: 0x04001A44 RID: 6724
	[Space]
	public Collider col;

	// Token: 0x04001A45 RID: 6725
	public PhysicMaterial PhysicMaterialMove;

	// Token: 0x04001A46 RID: 6726
	public PhysicMaterial PhysicMaterialIdle;

	// Token: 0x04001A47 RID: 6727
	internal float antiGravityTimer;

	// Token: 0x04001A48 RID: 6728
	internal float featherTimer;

	// Token: 0x04001A49 RID: 6729
	internal float deathSeenTimer;

	// Token: 0x04001A4A RID: 6730
	internal float tumbleInputDisableTimer;

	// Token: 0x04001A4B RID: 6731
	private float kinematicTimer;

	// Token: 0x04001A4C RID: 6732
	private bool toggleSprint;

	// Token: 0x04001A4D RID: 6733
	private bool toggleCrouch;

	// Token: 0x04001A4E RID: 6734
	private float rbOriginalMass;

	// Token: 0x04001A4F RID: 6735
	private float rbOriginalDrag;

	// Token: 0x04001A50 RID: 6736
	private float playerOriginalMoveSpeed;

	// Token: 0x04001A51 RID: 6737
	private float playerOriginalCustomGravity;

	// Token: 0x04001A52 RID: 6738
	private float playerOriginalSprintSpeed;

	// Token: 0x04001A53 RID: 6739
	private float playerOriginalCrouchSpeed;

	// Token: 0x04001A54 RID: 6740
	internal bool debugSlow;
}
