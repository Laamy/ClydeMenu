using System;
using UnityEngine;

// Token: 0x020001BF RID: 447
public class PlayerAvatarVisuals : MonoBehaviour
{
	// Token: 0x06000F61 RID: 3937 RVA: 0x0008A578 File Offset: 0x00088778
	private void Start()
	{
		this.playerAvatarRightArm = base.GetComponentInChildren<PlayerAvatarRightArm>();
		this.playerAvatarTalkAnimation = base.GetComponentInChildren<PlayerAvatarTalkAnimation>();
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
		if (!this.isMenuAvatar && (!GameManager.Multiplayer() || (this.playerAvatar && this.playerAvatar.photonView.IsMine)))
		{
			this.animator.enabled = false;
			this.meshParent.SetActive(false);
		}
		if (SemiFunc.IsMultiplayer() && !SemiFunc.RunIsArena())
		{
			PlayerAvatar x = SessionManager.instance.CrownedPlayerGet();
			if (!this.isMenuAvatar)
			{
				if (x == this.playerAvatar)
				{
					this.arenaCrown.SetActive(true);
					return;
				}
			}
			else if (x == PlayerAvatar.instance)
			{
				this.arenaCrown.SetActive(true);
			}
		}
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x0008A650 File Offset: 0x00088850
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (SemiFunc.FPSImpulse5() && !this.crownSetterWasHere && PlayerCrownSet.instance && PlayerCrownSet.instance.crownOwnerFetched)
		{
			if (this.playerAvatar && PlayerCrownSet.instance.crownOwnerSteamID == this.playerAvatar.steamID)
			{
				this.arenaCrown.SetActive(true);
			}
			this.crownSetterWasHere = true;
		}
		this.deltaTime = Time.deltaTime * this.animationSpeedMultiplier;
		this.deltaTime = Mathf.Max(this.deltaTime, 0f);
		if (this.isMenuAvatar)
		{
			this.MenuAvatarGetColorsFromRealAvatar();
		}
		if (!this.isMenuAvatar && this.playerAvatar.isDisabled)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (!this.isMenuAvatar)
		{
			if (!GameManager.Multiplayer() || this.playerAvatar.photonView.IsMine)
			{
				if (this.playerAvatar)
				{
					base.transform.position = this.playerAvatar.transform.position;
					base.transform.rotation = this.playerAvatar.transform.rotation;
				}
			}
			else
			{
				if (this.playerAvatar.isTumbling && this.playerAvatar.tumble)
				{
					this.visualFollowLerp = 0f;
					this.visualPosition = this.playerAvatar.tumble.followPosition.position;
					this.bodySpringTarget = this.playerAvatar.tumble.followPosition.rotation;
					this.playerAvatar.clientPosition = this.visualPosition;
					this.playerAvatar.clientPositionCurrent = this.visualPosition;
				}
				else if (!this.playerAvatar.clientPhysRiding || !this.PhysRiderPointInstance)
				{
					float num = Mathf.Lerp(0f, 25f, this.visualFollowLerp);
					this.visualFollowLerp = Mathf.Clamp01(this.visualFollowLerp + 2f * this.deltaTime);
					this.visualPosition = Vector3.Lerp(this.visualPosition, this.playerAvatar.clientPositionCurrent, num * this.deltaTime);
				}
				else if (this.PhysRiderPointInstance)
				{
					float num2 = Mathf.Lerp(0f, 25f, this.visualFollowLerp);
					this.visualFollowLerp = Mathf.Clamp01(this.visualFollowLerp + 2f * this.deltaTime);
					this.visualPosition = Vector3.Lerp(this.visualPosition, this.PhysRiderPointInstance.transform.position, num2 * this.deltaTime);
					this.playerAvatar.clientPosition = this.visualPosition;
					this.playerAvatar.clientPositionCurrent = this.visualPosition;
				}
				if (!this.playerAvatar.isTumbling)
				{
					if (this.animSliding)
					{
						if (this.animSlidingImpulse && this.playerAvatar.rbVelocity.magnitude > 0.1f)
						{
							this.bodySpringTarget = Quaternion.LookRotation(base.transform.TransformDirection(this.playerAvatar.rbVelocity).normalized, Vector3.up);
						}
					}
					else
					{
						this.bodySpringTarget = this.playerAvatar.clientRotationCurrent;
					}
					base.transform.rotation = SemiFunc.SpringQuaternionGet(this.bodySpring, this.bodySpringTarget, this.deltaTime);
				}
				else if (this.playerAvatar.tumble.tumbleSetTimer <= 0f)
				{
					this.bodySpring.lastRotation = this.bodySpringTarget;
					base.transform.rotation = this.bodySpringTarget;
				}
				base.transform.position = this.visualPosition;
				if (this.playerAvatar.playerHealth.hurtFreeze)
				{
					this.animator.speed = 0f;
					return;
				}
				if (this.turnDifferenceResetTimer > 0f)
				{
					this.turnDifferenceResetTimer -= Time.deltaTime;
					if (this.turnDifferenceResetTimer <= 0f)
					{
						this.turnDifference = 0f;
					}
				}
				float num3 = Quaternion.Angle(Quaternion.Euler(0f, this.turnPrevious, 0f), Quaternion.Euler(0f, this.bodySpringTarget.eulerAngles.y, 0f));
				if (num3 != 0f)
				{
					this.turnDifferenceResetTimer = 0.2f;
					this.turnDifference = num3;
				}
				float num4 = this.turnPrevious - this.bodySpringTarget.eulerAngles.y;
				if (Mathf.Abs(num4) < 180f && num4 != 0f)
				{
					this.turnDirection = Mathf.Sign(num4);
				}
				if (this.playerAvatar.isTumbling)
				{
					this.turnDifference = 0f;
				}
				this.turnPrevious = this.bodySpringTarget.eulerAngles.y;
			}
		}
		if (this.isMenuAvatar || (GameManager.Multiplayer() && !this.playerAvatar.photonView.IsMine))
		{
			if (this.playerEyes && this.playerEyes.lookAtActive && GameDirector.instance.currentState == GameDirector.gameState.Main && this.playerAvatar && this.playerAvatar.PlayerVisionTarget && this.playerAvatar.PlayerVisionTarget.VisionTransform)
			{
				Vector3 b = this.playerAvatar.PlayerVisionTarget.VisionTransform.position;
				Vector3 forward = this.playerAvatar.localCameraTransform.forward;
				if (this.playerAvatar.tumble && this.playerAvatar.tumble.isTumbling)
				{
					forward = this.playerAvatar.tumble.transform.forward;
				}
				if (this.isMenuAvatar)
				{
					b = base.transform.position + Vector3.up * 1.5f;
					forward = base.transform.forward;
				}
				Vector3 vector = this.playerEyes.lookAt.position - b;
				vector = SemiFunc.ClampDirection(vector, forward, 40f);
				this.headLookAtTransform.rotation = Quaternion.Slerp(this.headLookAtTransform.rotation, Quaternion.LookRotation(vector), this.deltaTime * 15f);
			}
			else
			{
				this.headLookAtTransform.localRotation = Quaternion.Slerp(this.headLookAtTransform.localRotation, Quaternion.identity, this.deltaTime * 15f);
			}
			float num5 = 0f;
			if (!this.playerAvatar.isTumbling && !this.isMenuAvatar)
			{
				num5 = this.playerAvatar.localCameraRotation.eulerAngles.x;
			}
			if (this.headTiltOverrideTimer > 0f)
			{
				num5 += this.headTiltOverrideAmount;
				this.headTiltOverrideAmount = 0f;
			}
			if (num5 > 90f)
			{
				num5 -= 360f;
			}
			if (this.playerAvatar.isCrawling)
			{
				num5 *= 0.4f;
			}
			else if (this.playerAvatar.isCrouching)
			{
				num5 *= 0.75f;
			}
			float num6 = this.headLookAtTransform.localEulerAngles.x;
			if (num6 > 90f)
			{
				num6 -= 360f;
			}
			if (this.isMenuAvatar)
			{
				num6 *= 1.25f;
			}
			num5 += num6;
			if (this.playerAvatar.isCrouching)
			{
				num5 = Mathf.Clamp(num5, -40f, 40f);
			}
			else if (this.playerAvatar.isCrouching)
			{
				num5 = Mathf.Clamp(num5, -60f, 65f);
			}
			else
			{
				num5 = Mathf.Clamp(num5, -75f, 85f);
			}
			float num7 = SemiFunc.SpringFloatGet(this.lookUpSpring, num5, this.deltaTime);
			this.headUpTransform.localRotation = Quaternion.Euler(num7 * 0.5f, 0f, 0f);
			this.bodyTopUpTransform.localRotation = Quaternion.Euler(num7 * 0.25f, 0f, 0f);
			this.upDifference = Quaternion.Angle(Quaternion.Euler(this.upPrevious, 0f, 0f), Quaternion.Euler(this.headUpTransform.eulerAngles.x, 0f, 0f));
			float f = this.upPrevious - this.headUpTransform.eulerAngles.x;
			if (Mathf.Abs(f) < 180f)
			{
				this.upDirection = Mathf.Sign(f);
			}
			this.upPrevious = this.headUpTransform.eulerAngles.x;
			this.headTiltOverrideTimer -= this.deltaTime;
			float num8 = 0f;
			if (this.turnDifference > 1f)
			{
				num8 = this.turnDifference * 5f * -this.turnDirection;
			}
			num8 = Mathf.Clamp(num8, -100f, 100f);
			this.headSideSteer = Mathf.Lerp(this.headSideSteer, num8, 20f * this.deltaTime);
			Quaternion quaternion = Quaternion.Euler(0f, this.headLookAtTransform.localRotation.eulerAngles.y + this.headSideSteer, 0f);
			quaternion = Quaternion.Slerp(Quaternion.identity, quaternion, 0.5f);
			Quaternion quaternion2 = SemiFunc.SpringQuaternionGet(this.lookSideSpring, quaternion, this.deltaTime);
			this.headSideTransform.localRotation = quaternion2;
			this.bodyTopSideTransform.localRotation = Quaternion.Slerp(Quaternion.identity, quaternion2, 0.5f);
			Vector3 zero = Vector3.zero;
			if (this.isMenuAvatar && PlayerAvatarMenu.instance && PlayerAvatarMenu.instance.rb && Mathf.Abs(PlayerAvatarMenu.instance.rb.angularVelocity.magnitude) > 1f)
			{
				zero.z = PlayerAvatarMenu.instance.rb.angularVelocity.y * 0.01f;
			}
			else if (this.playerAvatar.rbVelocity.magnitude > 0.1f)
			{
				Vector3 vector2 = base.transform.TransformDirection(this.playerAvatar.rbVelocity);
				if (Vector3.Dot(vector2.normalized, base.transform.forward) < -0.5f)
				{
					zero.x = -3f;
				}
				if (Vector3.Dot(vector2.normalized, base.transform.forward) > 0.5f)
				{
					zero.x = 3f;
				}
				if (Vector3.Dot(vector2.normalized, base.transform.right) > 0.5f)
				{
					zero.z = -3f;
				}
				if (Vector3.Dot(vector2.normalized, base.transform.right) < -0.5f)
				{
					zero.z = 3f;
				}
			}
			if (this.tiltSprinting != this.animSprinting)
			{
				if (this.tiltSprinting)
				{
					this.tiltTimer = 0.25f;
					this.tiltTarget = this.leanSpringTargetPrevious * 2f;
				}
				else
				{
					this.tiltTimer = 0.25f;
					this.tiltTarget = zero * 3f;
				}
				this.tiltSprinting = this.animSprinting;
			}
			this.leanTransform.localRotation = SemiFunc.SpringQuaternionGet(this.leanSpring, Quaternion.Euler(zero), this.deltaTime);
			this.tiltTransform.localRotation = SemiFunc.SpringQuaternionGet(this.tiltSpring, Quaternion.Euler(this.tiltTarget), this.deltaTime);
			if (this.tiltTimer > 0f)
			{
				this.tiltTimer -= this.deltaTime;
				if (this.tiltTimer <= 0f)
				{
					this.tiltTarget = Vector3.zero;
				}
			}
			this.leanSpringTargetPrevious = zero;
			bool flag = false;
			float num9 = 15f;
			float num10 = 0.5f;
			Vector3 vector3 = Vector3.zero;
			if (this.isMenuAvatar && PlayerAvatarMenu.instance && PlayerAvatarMenu.instance.rb && Mathf.Abs(PlayerAvatarMenu.instance.rb.angularVelocity.magnitude) > 1f)
			{
				flag = true;
				num9 = 10f;
				num10 = 0.7f;
				Vector3 vector4 = Quaternion.Euler(0f, -PlayerAvatarMenu.instance.rb.angularVelocity.y * 0.1f, 0f) * Vector3.forward;
				vector4.y = 0f;
				vector3 = vector4;
			}
			else if (this.playerAvatar.isMoving && !this.animJumping && this.playerAvatar.rbVelocity.magnitude > 0.1f)
			{
				flag = true;
				num9 = 10f;
				num10 = 0.7f;
				Vector3 normalized = this.playerAvatar.rbVelocity.normalized;
				normalized.y = 0f;
				vector3 = normalized;
			}
			if (this.legTwistActive != flag)
			{
				this.legTwistActive = flag;
				this.legTwistSpring.speed = num9;
				this.legTwistSpring.damping = num10;
			}
			else
			{
				this.legTwistSpring.speed = Mathf.Lerp(this.legTwistSpring.speed, num9, this.deltaTime * 5f);
				this.legTwistSpring.damping = Mathf.Lerp(this.legTwistSpring.damping, num10, this.deltaTime * 5f);
			}
			Quaternion targetRotation = Quaternion.identity;
			if (vector3 != Vector3.zero)
			{
				targetRotation = Quaternion.LookRotation(vector3, Vector3.up);
			}
			this.legTwistTransform.localRotation = SemiFunc.SpringQuaternionGet(this.legTwistSpring, targetRotation, this.deltaTime);
			this.AnimationLogic();
		}
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x0008B3F2 File Offset: 0x000895F2
	public void HeadTiltOverride(float _amount)
	{
		this.headTiltOverrideAmount += _amount;
		this.headTiltOverrideTimer = 0.1f;
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x0008B40D File Offset: 0x0008960D
	public void HeadTiltImpulse(float _amount)
	{
		this.lookUpSpring.springVelocity += _amount;
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x0008B424 File Offset: 0x00089624
	private void MenuAvatarGetColorsFromRealAvatar()
	{
		if (this.isMenuAvatar && !this.playerAvatar)
		{
			this.playerAvatar = PlayerAvatar.instance;
		}
		if (this.playerAvatar && this.playerAvatar.playerAvatarVisuals.color != this.color)
		{
			this.SetColor(-1, this.playerAvatar.playerAvatarVisuals.color);
		}
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x0008B492 File Offset: 0x00089692
	private void OnDestroy()
	{
		Object.Destroy(this.PhysRiderPointInstance);
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x0008B4A0 File Offset: 0x000896A0
	private void AnimationLogic()
	{
		if (this.isMenuAvatar && !this.expressionAvatar && PlayerAvatarMenu.instance && PlayerAvatarMenu.instance.rb)
		{
			if (Mathf.Abs(PlayerAvatarMenu.instance.rb.angularVelocity.magnitude) > 1f)
			{
				this.animator.SetBool("Turning", true);
				return;
			}
			this.animator.SetBool("Turning", false);
			return;
		}
		else
		{
			if (this.isMenuAvatar)
			{
				return;
			}
			bool flag = false;
			if (this.playerAvatar.isTumbling)
			{
				if (!this.animSprinting && !this.animTumbling)
				{
					this.animator.SetTrigger("TumblingImpulse");
					this.animTumbling = true;
				}
				if ((this.playerAvatar.tumble.physGrabObject.rbVelocity.magnitude > 1f && !this.playerAvatar.tumble.physGrabObject.impactDetector.inCart) || this.playerAvatar.tumble.physGrabObject.rbAngularVelocity.magnitude > 1f)
				{
					this.animator.SetBool("TumblingMove", true);
					flag = true;
				}
				else
				{
					this.animator.SetBool("TumblingMove", false);
				}
				this.animator.SetBool("Tumbling", true);
			}
			else
			{
				this.animator.SetBool("Tumbling", false);
				this.animator.SetBool("TumblingMove", false);
				this.animTumbling = false;
			}
			if (this.playerAvatar.isCrouching || this.playerAvatar.isTumbling)
			{
				this.animator.SetBool("Crouching", true);
			}
			else
			{
				this.animator.SetBool("Crouching", false);
			}
			if (this.playerAvatar.isCrawling || this.playerAvatar.isTumbling)
			{
				this.animator.SetBool("Crawling", true);
			}
			else
			{
				this.animator.SetBool("Crawling", false);
			}
			if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch to Crawl") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Crawl") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Crawl Move") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
			{
				this.animInCrawl = true;
			}
			else
			{
				this.animInCrawl = false;
			}
			if (this.playerAvatar.isMoving && !this.animJumping)
			{
				this.animator.SetBool("Moving", true);
			}
			else
			{
				this.animator.SetBool("Moving", false);
			}
			if (!this.playerAvatar.isMoving && !this.animJumping && Mathf.Abs(this.turnDifference) > 0.5f)
			{
				this.animator.SetBool("Turning", true);
			}
			else
			{
				this.animator.SetBool("Turning", false);
			}
			if (this.playerAvatar.isSprinting && !this.animJumping && !this.animTumbling)
			{
				if (!this.animSprinting && !this.animSliding)
				{
					this.animator.SetTrigger("SprintingImpulse");
					this.animSprinting = true;
				}
				this.animator.SetBool("Sprinting", true);
			}
			else
			{
				this.animator.SetBool("Sprinting", false);
				this.animSprinting = false;
			}
			this.animSlidingImpulse = false;
			if (this.playerAvatar.isSliding && !this.animJumping && !this.animTumbling)
			{
				if (!this.animSliding)
				{
					this.animSlidingImpulse = true;
					this.animator.SetTrigger("SlidingImpulse");
				}
				this.animator.SetBool("Sliding", true);
				this.animSliding = true;
			}
			else
			{
				this.animator.SetBool("Sliding", false);
				this.animSliding = false;
			}
			if (this.animJumping)
			{
				if (this.animJumpingImpulse)
				{
					this.animJumpTimer = 0.2f;
					this.animJumpingImpulse = false;
					this.animator.SetTrigger("JumpingImpulse");
					this.animator.SetBool("Jumping", true);
					this.animator.SetBool("Falling", false);
				}
				else if (this.playerAvatar.rbVelocityRaw.y < -0.5f && this.animJumpTimer <= 0f)
				{
					this.animator.SetBool("Falling", true);
				}
				if (this.playerAvatar.isGrounded && this.animJumpTimer <= 0f)
				{
					this.animJumpedTimer = 0.5f;
					this.animJumping = false;
				}
				this.animJumpTimer -= this.deltaTime;
			}
			else
			{
				this.animator.SetBool("Jumping", false);
				this.animator.SetBool("Falling", false);
			}
			if (this.animJumpedTimer > 0f)
			{
				this.animJumpedTimer -= this.deltaTime;
			}
			if (!this.playerAvatar.isGrounded)
			{
				this.animFallingTimer += this.deltaTime;
			}
			else
			{
				this.animFallingTimer = 0f;
			}
			if (!this.playerAvatar.isCrawling && !this.animJumping && !this.animSliding && !this.animTumbling && this.animFallingTimer > 0.25f && this.animJumpedTimer <= 0f)
			{
				this.animJumpTimer = 0.2f;
				this.animJumping = true;
				this.animJumpingImpulse = false;
				this.animator.SetTrigger("FallingImpulse");
				this.animator.SetBool("Jumping", true);
				this.animator.SetBool("Falling", true);
			}
			if (flag)
			{
				float num = Mathf.Max(this.playerAvatar.tumble.physGrabObject.rbVelocity.magnitude, this.playerAvatar.tumble.physGrabObject.rbAngularVelocity.magnitude) * 0.5f;
				num = Mathf.Clamp(num, 0.5f, 1.25f);
				this.animator.speed = num * this.animationSpeedMultiplier;
				this.playerAvatar.tumble.TumbleMoveSoundSet(flag, num);
				return;
			}
			if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Sprint"))
			{
				float num2 = 1f + (float)StatsManager.instance.playerUpgradeSpeed[this.playerAvatar.steamID] * 0.1f;
				this.animator.speed = num2 * this.animationSpeedMultiplier;
				return;
			}
			if (this.playerAvatar.isMoving && this.playerAvatar.mapToolController.Active)
			{
				this.animator.speed = 0.5f * this.animationSpeedMultiplier;
				return;
			}
			this.animator.speed = 1f * this.animationSpeedMultiplier;
			return;
		}
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x0008BB6C File Offset: 0x00089D6C
	public void JumpImpulse()
	{
		if (this.playerAvatar.isCrawling || this.animTumbling)
		{
			return;
		}
		this.animJumpingImpulse = true;
		this.animJumping = true;
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x0008BB94 File Offset: 0x00089D94
	public void PhysRidingCheck()
	{
		bool flag = this.PhysRiderPointInstance != null;
		if (flag && this.PhysRiderPointInstance.transform.parent != this.playerAvatar.clientPhysRidingTransform)
		{
			Object.Destroy(this.PhysRiderPointInstance);
			flag = false;
		}
		if (!flag)
		{
			this.PhysRiderPointInstance = Object.Instantiate<GameObject>(this.PhysRiderPoint, Vector3.zero, Quaternion.identity, this.playerAvatar.clientPhysRidingTransform);
		}
		this.PhysRiderPointInstance.transform.localPosition = this.playerAvatar.clientPhysRidingPosition;
	}

	// Token: 0x06000F6A RID: 3946 RVA: 0x0008BC24 File Offset: 0x00089E24
	public void SetColor(int _colorIndex, Color _setColor = default(Color))
	{
		bool flag = false;
		Color value;
		if (_colorIndex != -1)
		{
			value = AssetManager.instance.playerColors[_colorIndex];
		}
		else
		{
			value = _setColor;
			flag = true;
		}
		int nameID = Shader.PropertyToID("_AlbedoColor");
		this.color = value;
		if (!flag)
		{
			this.playerAvatar.playerHealth.bodyMaterial.SetColor(nameID, value);
		}
		else
		{
			PlayerHealth componentInParent = base.GetComponentInParent<PlayerHealth>();
			if (componentInParent)
			{
				componentInParent.bodyMaterial.SetColor(nameID, value);
			}
		}
		if (SemiFunc.RunIsLobbyMenu() && MenuPageLobby.instance)
		{
			foreach (MenuPlayerListed menuPlayerListed in MenuPageLobby.instance.menuPlayerListedList)
			{
				if (menuPlayerListed.playerAvatar == this.playerAvatar)
				{
					menuPlayerListed.playerHead.SetColor(value);
					break;
				}
			}
		}
		this.colorSet = true;
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x0008BD1C File Offset: 0x00089F1C
	public void Revive()
	{
		this.bodySpringTarget = this.playerAvatar.clientRotationCurrent;
		this.bodySpring.lastRotation = this.bodySpringTarget;
		this.turnPrevious = this.bodySpringTarget.eulerAngles.y;
		this.playerAvatar.isCrawling = true;
		this.playerAvatar.isCrouching = true;
		this.playerAvatar.isTumbling = false;
		this.playerAvatar.isMoving = false;
		this.playerAvatar.isSprinting = false;
		this.visualFollowLerp = 1f;
		this.animator.Play("Crawl");
		this.animInCrawl = true;
		this.animator.SetBool("Crouching", true);
		this.animator.SetBool("Crawling", true);
		this.animator.SetBool("Moving", false);
		this.animator.SetBool("Sprinting", false);
		this.animator.SetBool("Sliding", false);
		this.animator.SetBool("Jumping", false);
		this.animator.SetBool("Falling", false);
		this.animator.SetBool("Turning", false);
		this.animator.SetBool("Tumbling", false);
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x0008BE58 File Offset: 0x0008A058
	public void PowerupJumpEffect()
	{
		ParticleSystem[] array = this.powerupJumpEffect;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x0008BE84 File Offset: 0x0008A084
	public void TumbleBreakFreeEffect()
	{
		ParticleSystem[] array = this.tumbleBreakFreeEffect;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x0008BEAE File Offset: 0x0008A0AE
	public void FootstepLight()
	{
		this.playerAvatar.Footstep(Materials.SoundType.Light);
	}

	// Token: 0x06000F6F RID: 3951 RVA: 0x0008BEBC File Offset: 0x0008A0BC
	public void FootstepMedium()
	{
		if (this.isMenuAvatar)
		{
			return;
		}
		this.playerAvatar.Footstep(Materials.SoundType.Medium);
	}

	// Token: 0x06000F70 RID: 3952 RVA: 0x0008BED3 File Offset: 0x0008A0D3
	public void FootstepHeavy()
	{
		this.playerAvatar.Footstep(Materials.SoundType.Heavy);
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x0008BEE1 File Offset: 0x0008A0E1
	public void StandToCrouch()
	{
		if (this.playerAvatar)
		{
			this.playerAvatar.StandToCrouch();
		}
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x0008BEFB File Offset: 0x0008A0FB
	public void CrouchToStand()
	{
		if (this.playerAvatar)
		{
			this.playerAvatar.CrouchToStand();
		}
	}

	// Token: 0x06000F73 RID: 3955 RVA: 0x0008BF15 File Offset: 0x0008A115
	public void CrouchToCrawl()
	{
		this.playerAvatar.CrouchToCrawl();
	}

	// Token: 0x06000F74 RID: 3956 RVA: 0x0008BF22 File Offset: 0x0008A122
	public void CrawlToCrouch()
	{
		this.playerAvatar.CrawlToCrouch();
	}

	// Token: 0x0400196B RID: 6507
	public bool isMenuAvatar;

	// Token: 0x0400196C RID: 6508
	[Space]
	public PlayerAvatar playerAvatar;

	// Token: 0x0400196D RID: 6509
	public GameObject meshParent;

	// Token: 0x0400196E RID: 6510
	internal Animator animator;

	// Token: 0x0400196F RID: 6511
	private bool animSprinting;

	// Token: 0x04001970 RID: 6512
	private bool animSliding;

	// Token: 0x04001971 RID: 6513
	private bool animSlidingImpulse;

	// Token: 0x04001972 RID: 6514
	private bool animJumping;

	// Token: 0x04001973 RID: 6515
	private bool animJumpingImpulse;

	// Token: 0x04001974 RID: 6516
	private float animJumpTimer;

	// Token: 0x04001975 RID: 6517
	private float animJumpedTimer;

	// Token: 0x04001976 RID: 6518
	private float animFallingTimer;

	// Token: 0x04001977 RID: 6519
	internal bool animInCrawl;

	// Token: 0x04001978 RID: 6520
	internal bool animTumbling;

	// Token: 0x04001979 RID: 6521
	internal PlayerAvatarTalkAnimation playerAvatarTalkAnimation;

	// Token: 0x0400197A RID: 6522
	internal PlayerAvatarRightArm playerAvatarRightArm;

	// Token: 0x0400197B RID: 6523
	[Space]
	public Transform headUpTransform;

	// Token: 0x0400197C RID: 6524
	public Transform headSideTransform;

	// Token: 0x0400197D RID: 6525
	public Transform TTSTransform;

	// Token: 0x0400197E RID: 6526
	[Space]
	public Transform bodyTopUpTransform;

	// Token: 0x0400197F RID: 6527
	public Transform bodyTopSideTransform;

	// Token: 0x04001980 RID: 6528
	[Space]
	public GameObject PhysRiderPoint;

	// Token: 0x04001981 RID: 6529
	public PlayerEyes playerEyes;

	// Token: 0x04001982 RID: 6530
	private GameObject PhysRiderPointInstance;

	// Token: 0x04001983 RID: 6531
	[Space]
	public ParticleSystem[] powerupJumpEffect;

	// Token: 0x04001984 RID: 6532
	public ParticleSystem[] tumbleBreakFreeEffect;

	// Token: 0x04001985 RID: 6533
	[Space]
	public Transform effectGetIntoTruck;

	// Token: 0x04001986 RID: 6534
	private float effectGetIntoTruckTimer;

	// Token: 0x04001987 RID: 6535
	[Space]
	public GameObject arenaCrown;

	// Token: 0x04001988 RID: 6536
	public Transform leanTransform;

	// Token: 0x04001989 RID: 6537
	public SpringQuaternion leanSpring;

	// Token: 0x0400198A RID: 6538
	private Vector3 leanSpringTargetPrevious;

	// Token: 0x0400198B RID: 6539
	[Space]
	public Transform tiltTransform;

	// Token: 0x0400198C RID: 6540
	public SpringQuaternion tiltSpring;

	// Token: 0x0400198D RID: 6541
	private bool tiltSprinting;

	// Token: 0x0400198E RID: 6542
	private float tiltTimer;

	// Token: 0x0400198F RID: 6543
	private Vector3 tiltTarget;

	// Token: 0x04001990 RID: 6544
	[Space]
	public SpringQuaternion bodySpring;

	// Token: 0x04001991 RID: 6545
	[HideInInspector]
	public Quaternion bodySpringTarget;

	// Token: 0x04001992 RID: 6546
	public Transform legTwistTransform;

	// Token: 0x04001993 RID: 6547
	public SpringQuaternion legTwistSpring;

	// Token: 0x04001994 RID: 6548
	private bool legTwistActive;

	// Token: 0x04001995 RID: 6549
	public Transform headLookAtTransform;

	// Token: 0x04001996 RID: 6550
	public SpringFloat lookUpSpring;

	// Token: 0x04001997 RID: 6551
	public SpringQuaternion lookSideSpring;

	// Token: 0x04001998 RID: 6552
	public Transform attachPointJawTop;

	// Token: 0x04001999 RID: 6553
	public Transform attachPointJawBottom;

	// Token: 0x0400199A RID: 6554
	public Transform attachPointTopHeadMiddle;

	// Token: 0x0400199B RID: 6555
	public Transform attachNeck;

	// Token: 0x0400199C RID: 6556
	private Vector3 positionLast;

	// Token: 0x0400199D RID: 6557
	internal Vector3 visualPosition = Vector3.zero;

	// Token: 0x0400199E RID: 6558
	private float visualFollowLerp;

	// Token: 0x0400199F RID: 6559
	internal float turnDifference;

	// Token: 0x040019A0 RID: 6560
	private float turnDifferenceResetTimer;

	// Token: 0x040019A1 RID: 6561
	internal float turnDirection;

	// Token: 0x040019A2 RID: 6562
	private float turnPrevious;

	// Token: 0x040019A3 RID: 6563
	private float headSideSteer;

	// Token: 0x040019A4 RID: 6564
	internal float upDifference;

	// Token: 0x040019A5 RID: 6565
	internal float upDirection;

	// Token: 0x040019A6 RID: 6566
	private float upPrevious;

	// Token: 0x040019A7 RID: 6567
	private float headTiltOverrideAmount;

	// Token: 0x040019A8 RID: 6568
	private float headTiltOverrideTimer;

	// Token: 0x040019A9 RID: 6569
	internal float animationSpeedMultiplier = 1f;

	// Token: 0x040019AA RID: 6570
	internal float deltaTime;

	// Token: 0x040019AB RID: 6571
	internal Color color;

	// Token: 0x040019AC RID: 6572
	internal bool colorSet;

	// Token: 0x040019AD RID: 6573
	private bool crownSetterWasHere;

	// Token: 0x040019AE RID: 6574
	internal bool expressionAvatar;
}
