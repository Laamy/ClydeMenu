using System;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public class PlayerCollisionController : MonoBehaviour
{
	// Token: 0x06000F80 RID: 3968 RVA: 0x0008C240 File Offset: 0x0008A440
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.GroundedDisableTimer > 0f)
		{
			this.GroundedDisableTimer -= Time.deltaTime;
		}
		base.transform.position = this.FollowTarget.position + this.Offset;
		base.transform.rotation = this.FollowTarget.rotation;
		if (PlayerController.instance.playerAvatarScript.fallDamageResetState || SemiFunc.MenuLevel())
		{
			this.ResetFalling();
		}
		PlayerTumble tumble = PlayerController.instance.playerAvatarScript.tumble;
		if (tumble && !this.Grounded && (!tumble.isTumbling || (float)tumble.physGrabObject.playerGrabbing.Count <= 0f))
		{
			if (GameDirector.instance.currentState == GameDirector.gameState.Main && this.fallLastY - base.transform.position.y > 0f)
			{
				this.fallDistance += Mathf.Abs(base.transform.position.y - this.fallLastY);
			}
			this.fallLastY = base.transform.position.y;
			if (PlayerController.instance.featherTimer > 0f || PlayerController.instance.antiGravityTimer > 0f)
			{
				this.fallDistance = 0f;
			}
		}
		else
		{
			this.fallLastY = base.transform.position.y;
			this.fallDistance = 0f;
		}
		if (LevelGenerator.Instance.Generated)
		{
			PlayerController.instance.playerAvatarScript.isGrounded = this.Grounded;
		}
		float b = 0f;
		bool flag = false;
		if (tumble && tumble.isTumbling)
		{
			if (tumble.physGrabObject.rbVelocity.magnitude > 6f)
			{
				this.tumbleVelocityTime += Time.deltaTime;
				if (this.tumbleVelocityTime > 0.5f || tumble.physGrabObject.rbVelocity.magnitude > 8f)
				{
					if (tumble.physGrabObject.rbVelocity.magnitude > 15f)
					{
						this.fallLoopStopTimer = 0f;
					}
					flag = true;
				}
			}
			else
			{
				this.tumbleVelocityTime = 0f;
			}
			b = Mathf.Clamp(tumble.physGrabObject.rbVelocity.magnitude / 20f, 0.8f, 1.25f);
		}
		this.fallLoopPitch = Mathf.Lerp(this.fallLoopPitch, b, 10f * Time.deltaTime);
		if (this.fallLoopStopTimer > 0f)
		{
			this.volume = 0f;
			this.fallLoopStopTimer -= Time.deltaTime;
			this.soundFallLoop.PlayLoop(false, 2f, 20f, this.fallLoopPitch);
			return;
		}
		if (!flag)
		{
			this.volume = 0f;
		}
		else
		{
			this.volume = Mathf.Lerp(this.volume, 1f, 0.75f * Time.deltaTime);
		}
		this.soundFallLoop.PlayLoop(flag, 5f, 5f, this.fallLoopPitch);
		this.soundFallLoop.LoopVolume = this.volume;
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x0008C576 File Offset: 0x0008A776
	public void StopFallLoop()
	{
		this.fallLoopStopTimer = 1f;
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x0008C583 File Offset: 0x0008A783
	public void ResetFalling()
	{
		this.StopFallLoop();
		this.fallDistance = 0f;
	}

	// Token: 0x040019BF RID: 6591
	public Transform FollowTarget;

	// Token: 0x040019C0 RID: 6592
	public Vector3 Offset;

	// Token: 0x040019C1 RID: 6593
	public PlayerCollisionGrounded CollisionGrounded;

	// Token: 0x040019C2 RID: 6594
	[Space]
	public float GroundedDisableTimer;

	// Token: 0x040019C3 RID: 6595
	public bool Grounded;

	// Token: 0x040019C4 RID: 6596
	internal float fallDistance;

	// Token: 0x040019C5 RID: 6597
	private float fallLastY;

	// Token: 0x040019C6 RID: 6598
	private float tumbleVelocityTime;

	// Token: 0x040019C7 RID: 6599
	private float fallLoopPitch;

	// Token: 0x040019C8 RID: 6600
	private float fallLoopStopTimer;

	// Token: 0x040019C9 RID: 6601
	private float volume;

	// Token: 0x040019CA RID: 6602
	public Sound soundFallLoop;
}
