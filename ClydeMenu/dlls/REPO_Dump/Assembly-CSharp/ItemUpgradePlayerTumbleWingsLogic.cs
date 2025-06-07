using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000183 RID: 387
public class ItemUpgradePlayerTumbleWingsLogic : MonoBehaviour
{
	// Token: 0x06000D35 RID: 3381 RVA: 0x00073807 File Offset: 0x00071A07
	private void Start()
	{
		this.tumbleWingTimer = 1f;
		this.currentState = ItemUpgradePlayerTumbleWingsLogic.State.Inactive;
		this.stateStart = true;
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x0007382F File Offset: 0x00071A2F
	private IEnumerator LateStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (!SemiFunc.RunIsLobbyMenu())
		{
			this.lateStartDone = true;
		}
		yield break;
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x00073840 File Offset: 0x00071A40
	private void StateMachine()
	{
		if (!this.fetchComplete)
		{
			return;
		}
		switch (this.currentState)
		{
		case ItemUpgradePlayerTumbleWingsLogic.State.Intro:
			this.StateIntro();
			return;
		case ItemUpgradePlayerTumbleWingsLogic.State.Outro:
			this.StateOutro();
			return;
		case ItemUpgradePlayerTumbleWingsLogic.State.Active:
			this.StateActive();
			return;
		case ItemUpgradePlayerTumbleWingsLogic.State.Inactive:
			this.StateInactive();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x00073890 File Offset: 0x00071A90
	private void LoopSound()
	{
		if (this.playerAvatar.upgradeTumbleWings <= 0f)
		{
			return;
		}
		if (!this.playerAvatar || !this.playerTumble)
		{
			return;
		}
		if (!this.playerAvatar.playerAvatarVisuals.isActiveAndEnabled)
		{
			return;
		}
		if (SemiFunc.FPSImpulse30())
		{
			this.posPrev = this.posCurrent;
			this.posCurrent = base.transform.position;
			this.targetSpeed = Vector3.Distance(this.posPrev, this.posCurrent) * 5f;
		}
		this.currentSpeed = Mathf.Lerp(this.currentSpeed, this.targetSpeed, Time.deltaTime * 2f);
		this.pitchSpeed = Mathf.Clamp(this.currentSpeed * 2f, 1f, 4f);
		this.soundWingsLoop.PlayLoop(this.playerAvatar.upgradeTumbleWingsVisualsActive, 2f, 2f, this.pitchSpeed);
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x00073988 File Offset: 0x00071B88
	private void Update()
	{
		if (!this.lateStartDone)
		{
			return;
		}
		if (this.fetchComplete && this.playerAvatar && this.playerAvatar.upgradeTumbleWingsVisualsActive && this.currentState == ItemUpgradePlayerTumbleWingsLogic.State.Inactive)
		{
			this.StateSet(ItemUpgradePlayerTumbleWingsLogic.State.Intro);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.wingsSwitchCooldown > 0f)
		{
			this.wingsSwitchCooldown -= Time.deltaTime;
		}
		if (this.fetchComplete && (SemiFunc.IsMasterClientOrSingleplayer() || this.isLocal) && this.playerAvatar.upgradeTumbleWingsVisualsActive && this.tumbleWingTimer > 0f)
		{
			float num = 1f + this.playerAvatar.upgradeTumbleWings / 4f;
			this.tumbleWingTimer -= Time.deltaTime / num;
		}
		this.StateMachine();
		this.LoopSound();
		if (!this.fetchComplete && SemiFunc.FPSImpulse1() && !this.playerTumble && this.playerAvatar)
		{
			this.playerTumble = this.playerAvatar.tumble;
			if (this.playerTumble)
			{
				this.steamID = this.playerAvatar.steamID;
				this.physGrabObject = this.playerTumble.physGrabObject;
				if (this.playerAvatar.isLocal)
				{
					this.isLocal = true;
				}
				this.playerAvatar.upgradeTumbleWingsLogic = this;
				this.fetchComplete = true;
				this.StateSet(ItemUpgradePlayerTumbleWingsLogic.State.Inactive);
				if (this.isLocal)
				{
					this.localAudioSource.enabled = true;
					TumbleWingsUI.instance.itemUpgradePlayerTumbleWingsLogic = this;
					this.soundWingsLoop.Source = this.localAudioSource;
				}
			}
		}
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x00073B38 File Offset: 0x00071D38
	private void FixedUpdate()
	{
		if (!this.fetchComplete)
		{
			return;
		}
		if (!this.lateStartDone)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!this.playerAvatar)
		{
			return;
		}
		float num = this.playerAvatar.upgradeTumbleWings;
		if (num > 0f)
		{
			num += 6f;
			if (this.playerTumble.isTumbling && (this.playerTumble.isPlayerInputTriggered || this.playerTumble.tumbleOverride) && this.physGrabObject.playerGrabbing.Count == 0 && !SemiFunc.OnGroundCheck(this.playerTumble.transform.position, 1f, this.physGrabObject))
			{
				if (this.playerAvatar.upgradeTumbleWingsVisualsActive)
				{
					if (num <= 15f)
					{
						this.physGrabObject.rb.AddForceAtPosition(Vector3.up * (1.9f * num), this.transformWings.position, ForceMode.Force);
					}
					else
					{
						this.physGrabObject.OverrideZeroGravity(0.1f);
					}
					this.physGrabObject.rb.AddForceAtPosition(this.playerAvatar.localCameraTransform.forward * (0.01f * num), this.transformWings.position, ForceMode.Impulse);
				}
				Quaternion targetRotation = Quaternion.LookRotation(this.playerAvatar.localCameraTransform.forward, Vector3.up);
				Vector3 torque = SemiFunc.PhysFollowRotation(this.physGrabObject.transform, targetRotation, this.physGrabObject.rb, 20f);
				this.physGrabObject.rb.AddTorque(torque, ForceMode.Impulse);
				if (!this.playerAvatar.upgradeTumbleWingsVisualsActive && this.wingsSwitchCooldown <= 0f && this.hasBeenGrounded)
				{
					this.playerAvatar.UpgradeTumbleWingsVisualsActive(true);
					this.playerAvatar.upgradeTumbleWingsVisualsActive = true;
					this.hasBeenGrounded = false;
					this.tumbleWingTimer = 1f;
					this.wingsSwitchCooldown = 2f;
				}
			}
			else
			{
				this.hasBeenGrounded = true;
				if (this.wingsSwitchCooldown <= 0f)
				{
					this.TurnOffWings();
				}
			}
			if (this.tumbleWingTimer <= 0f)
			{
				this.TurnOffWings();
			}
		}
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x00073D54 File Offset: 0x00071F54
	private void TurnOffWings()
	{
		if (this.playerAvatar.upgradeTumbleWingsVisualsActive)
		{
			this.playerAvatar.UpgradeTumbleWingsVisualsActive(false);
			this.playerAvatar.upgradeTumbleWingsVisualsActive = false;
			this.wingsSwitchCooldown = 0.5f;
		}
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x00073D88 File Offset: 0x00071F88
	private void StateIntro()
	{
		if (this.stateStart)
		{
			this.transformWings.gameObject.SetActive(true);
			this.transformWings.localScale = Vector3.zero;
			this.lightWings.intensity = 0f;
			this.transformWingLeft.localRotation = Quaternion.Euler(0f, 0f, 0f);
			this.transformWingRight.localRotation = Quaternion.Euler(0f, 0f, 0f);
			this.stateStart = false;
		}
		this.transformWings.localScale = Vector3.Lerp(this.transformWings.localScale, Vector3.one, Time.deltaTime * 10f);
		if (this.transformWings.localScale.x > 0.98f)
		{
			this.transformWings.localScale = Vector3.one;
			this.StateSet(ItemUpgradePlayerTumbleWingsLogic.State.Active);
		}
		this.lightWings.intensity = this.transformWings.localScale.x * 3f;
		this.FlapWings();
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x00073E94 File Offset: 0x00072094
	private void StateOutro()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.transformWings.localScale = Vector3.Lerp(this.transformWings.localScale, Vector3.zero, Time.deltaTime * 10f);
		if (this.transformWings.localScale.x < 0.02f)
		{
			this.transformWings.localScale = Vector3.zero;
			this.StateSet(ItemUpgradePlayerTumbleWingsLogic.State.Inactive);
		}
		this.lightWings.intensity = this.transformWings.localScale.x * 3f;
		this.FlapWings();
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x00073F30 File Offset: 0x00072130
	private void StateActive()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.lightWings.intensity = 2f;
		}
		if (!this.playerAvatar.upgradeTumbleWingsVisualsActive)
		{
			this.StateSet(ItemUpgradePlayerTumbleWingsLogic.State.Outro);
			return;
		}
		this.FlapWings();
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x00073F6C File Offset: 0x0007216C
	private void StateInactive()
	{
		if (this.stateStart)
		{
			this.transformWings.gameObject.SetActive(false);
			this.stateStart = false;
		}
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x00073F90 File Offset: 0x00072190
	private void FlapWings()
	{
		float num = -54f;
		float num2 = 40f;
		float num3 = Mathf.Sin(Time.time * num2) * num;
		this.transformWingLeft.localRotation = Quaternion.Euler(0f, -34f - num3, 0f);
		this.transformWingRight.localRotation = Quaternion.Euler(0f, 34f + num3, 0f);
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x00073FFA File Offset: 0x000721FA
	private void StateSet(ItemUpgradePlayerTumbleWingsLogic.State newState)
	{
		if (this.currentState == newState)
		{
			return;
		}
		this.currentState = newState;
		this.stateStart = true;
	}

	// Token: 0x04001504 RID: 5380
	private PhysGrabObject physGrabObject;

	// Token: 0x04001505 RID: 5381
	private PlayerTumble playerTumble;

	// Token: 0x04001506 RID: 5382
	public PlayerAvatar playerAvatar;

	// Token: 0x04001507 RID: 5383
	private string steamID = "";

	// Token: 0x04001508 RID: 5384
	private bool stateStart;

	// Token: 0x04001509 RID: 5385
	public Transform transformWings;

	// Token: 0x0400150A RID: 5386
	public Transform transformWingLeft;

	// Token: 0x0400150B RID: 5387
	public Transform transformWingRight;

	// Token: 0x0400150C RID: 5388
	private bool isLocal;

	// Token: 0x0400150D RID: 5389
	public Sound soundWingsLoop;

	// Token: 0x0400150E RID: 5390
	private Vector3 posPrev;

	// Token: 0x0400150F RID: 5391
	private Vector3 posCurrent;

	// Token: 0x04001510 RID: 5392
	private float targetSpeed;

	// Token: 0x04001511 RID: 5393
	private float currentSpeed;

	// Token: 0x04001512 RID: 5394
	public AudioSource localAudioSource;

	// Token: 0x04001513 RID: 5395
	private bool lateStartDone;

	// Token: 0x04001514 RID: 5396
	private float pitchSpeed = 1f;

	// Token: 0x04001515 RID: 5397
	private bool fetchComplete;

	// Token: 0x04001516 RID: 5398
	private float wingsSwitchCooldown;

	// Token: 0x04001517 RID: 5399
	public Light lightWings;

	// Token: 0x04001518 RID: 5400
	internal float tumbleWingTimer;

	// Token: 0x04001519 RID: 5401
	private bool hasBeenGrounded = true;

	// Token: 0x0400151A RID: 5402
	private ItemUpgradePlayerTumbleWingsLogic.State currentState;

	// Token: 0x02000393 RID: 915
	private enum State
	{
		// Token: 0x04002B8C RID: 11148
		Intro,
		// Token: 0x04002B8D RID: 11149
		Outro,
		// Token: 0x04002B8E RID: 11150
		Active,
		// Token: 0x04002B8F RID: 11151
		Inactive
	}
}
