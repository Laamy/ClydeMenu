using System;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class EnemyBangAnim : MonoBehaviour
{
	// Token: 0x0600012F RID: 303 RVA: 0x0000B970 File Offset: 0x00009B70
	private void Awake()
	{
		this.soundImpactLight.Volume *= this.volumeMultiplier;
		this.soundImpactMedium.Volume *= this.volumeMultiplier;
		this.soundImpactHeavy.Volume *= this.volumeMultiplier;
		this.soundMoveShort.Volume *= this.volumeMultiplier;
		this.soundMoveLong.Volume *= this.volumeMultiplier;
		this.soundFootstep.Volume *= this.volumeMultiplier;
		this.soundHurt.Volume *= this.volumeMultiplier;
		this.soundDeathSFX.Volume *= this.volumeMultiplier;
		this.soundDeathVO.Volume *= this.volumeMultiplier;
		this.soundJumpSFX.Volume *= this.volumeMultiplier;
		this.soundJumpVO.Volume *= this.volumeMultiplier;
		this.soundLandSFX.Volume *= this.volumeMultiplier;
		this.soundLandVO.Volume *= this.volumeMultiplier;
		this.soundStunIntro.Volume *= this.volumeMultiplier;
		this.soundStunLoop.Volume *= this.volumeMultiplier;
		this.soundStunOutro.Volume *= this.volumeMultiplier;
		this.soundIdleBreaker.Volume *= this.volumeMultiplier;
		this.soundAttackBreaker.Volume *= this.volumeMultiplier;
		this.soundFuseTell.Volume *= this.volumeMultiplier;
		this.soundFuseIgnite.Volume *= this.volumeMultiplier;
		this.soundFuseLoop.Volume *= this.volumeMultiplier;
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x06000130 RID: 304 RVA: 0x0000BB90 File Offset: 0x00009D90
	private void Update()
	{
		if (this.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
		else
		{
			this.animator.speed = 1f;
		}
		if (this.controller.currentState == EnemyBang.State.Spawn)
		{
			if (this.spawnImpulse)
			{
				this.spawnImpulse = false;
				this.animator.SetTrigger(this.animSpawn);
			}
		}
		else
		{
			this.spawnImpulse = true;
		}
		if ((this.controller.currentState == EnemyBang.State.Roam || this.controller.currentState == EnemyBang.State.Move || this.controller.currentState == EnemyBang.State.MoveUnder || this.controller.currentState == EnemyBang.State.MoveOver || this.controller.currentState == EnemyBang.State.MoveBack) && (this.enemy.Rigidbody.velocity.magnitude > 1f || this.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude > 1f))
		{
			this.moveTimer = 0.1f;
		}
		if (this.moveTimer > 0f)
		{
			this.moveTimer -= Time.deltaTime;
			this.animator.SetBool(this.animMoving, true);
		}
		else
		{
			this.animator.SetBool(this.animMoving, false);
		}
		if (this.enemy.Jump.jumping)
		{
			if (this.jumpImpulse)
			{
				if (!this.enemy.IsStunned())
				{
					this.animator.SetTrigger(this.animJump);
				}
				this.animator.SetBool(this.animFalling, false);
				this.jumpImpulse = false;
				this.landImpulse = true;
			}
			else if (this.controller.enemy.Rigidbody.physGrabObject.rbVelocity.y < 0f)
			{
				this.animator.SetBool(this.animFalling, true);
			}
		}
		else
		{
			if (this.landImpulse)
			{
				if (!this.enemy.IsStunned())
				{
					this.animator.SetTrigger(this.animLand);
				}
				this.moveTimer = 0f;
				this.landImpulse = false;
			}
			this.animator.SetBool(this.animFalling, false);
			this.jumpImpulse = true;
		}
		if (this.controller.currentState == EnemyBang.State.Fuse)
		{
			if (this.fuseImpulse)
			{
				this.animator.SetTrigger(this.animFuse);
				this.fuseImpulse = false;
			}
		}
		else
		{
			this.fuseImpulse = true;
		}
		if (this.fuseLoopTimer > 0f)
		{
			this.fuseLoopTimer -= Time.deltaTime;
			this.soundFuseLoop.PlayLoop(true, 2f, 2f, 1f + this.soundFuseLoopCurve.Evaluate(this.controller.fuseLerp) * 4f);
		}
		else
		{
			this.soundFuseLoop.PlayLoop(false, 2f, 2f, 1f + this.soundFuseLoopCurve.Evaluate(this.controller.fuseLerp) * 4f);
		}
		if (this.controller.currentState == EnemyBang.State.Stun)
		{
			if (this.stunImpulse)
			{
				this.soundStunIntro.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
				this.animator.SetTrigger(this.animStun);
				this.stunImpulse = false;
			}
			if (this.stunLoopPauseTimer <= 0f)
			{
				this.soundStunLoop.PlayLoop(true, 5f, 5f, 1f);
			}
			else
			{
				this.soundStunLoop.PlayLoop(false, 5f, 5f, 1f);
			}
			this.animator.SetBool(this.animStunned, true);
		}
		else
		{
			this.soundStunLoop.PlayLoop(false, 5f, 5f, 1f);
			this.animator.SetBool(this.animStunned, false);
			if (!this.stunImpulse)
			{
				this.soundStunOutro.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
				this.stunImpulse = true;
			}
		}
		if (this.stunLoopPauseTimer > 0f)
		{
			this.stunLoopPauseTimer -= Time.deltaTime;
		}
		if (this.controller.currentState == EnemyBang.State.Despawn)
		{
			this.animator.SetBool(this.animDespawning, true);
			return;
		}
		this.animator.SetBool(this.animDespawning, false);
	}

	// Token: 0x06000131 RID: 305 RVA: 0x0000C011 File Offset: 0x0000A211
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x06000132 RID: 306 RVA: 0x0000C023 File Offset: 0x0000A223
	public void FuseTellPlay()
	{
		this.soundFuseTell.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000133 RID: 307 RVA: 0x0000C05C File Offset: 0x0000A25C
	public void FootstepPlay()
	{
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.25f, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
		this.soundFootstep.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000134 RID: 308 RVA: 0x0000C0E0 File Offset: 0x0000A2E0
	public void JumpPlay()
	{
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.25f, Vector3.down, Materials.SoundType.Medium, false, this.material, Materials.HostType.Enemy);
		this.soundJumpSFX.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		this.soundJumpVO.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000135 RID: 309 RVA: 0x0000C198 File Offset: 0x0000A398
	public void LandPlay()
	{
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.25f, Vector3.down, Materials.SoundType.Heavy, true, this.material, Materials.HostType.Enemy);
		this.soundLandSFX.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		this.soundLandVO.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000136 RID: 310 RVA: 0x0000C250 File Offset: 0x0000A450
	public void MoveShortPlay()
	{
		this.soundMoveShort.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000137 RID: 311 RVA: 0x0000C287 File Offset: 0x0000A487
	public void MoveLongPlay()
	{
		this.soundMoveLong.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000138 RID: 312 RVA: 0x0000C2BE File Offset: 0x0000A4BE
	public void FuseLoop()
	{
		this.fuseLoopTimer = 0.1f;
	}

	// Token: 0x06000139 RID: 313 RVA: 0x0000C2CB File Offset: 0x0000A4CB
	public void StunLoopPause(float _time)
	{
		this.stunLoopPauseTimer = _time;
	}

	// Token: 0x04000283 RID: 643
	public Enemy enemy;

	// Token: 0x04000284 RID: 644
	public EnemyBang controller;

	// Token: 0x04000285 RID: 645
	internal Animator animator;

	// Token: 0x04000286 RID: 646
	internal Materials.MaterialTrigger material = new Materials.MaterialTrigger();

	// Token: 0x04000287 RID: 647
	private int animSpawn = Animator.StringToHash("spawn");

	// Token: 0x04000288 RID: 648
	private int animDespawning = Animator.StringToHash("despawning");

	// Token: 0x04000289 RID: 649
	private int animStun = Animator.StringToHash("stun");

	// Token: 0x0400028A RID: 650
	private int animStunned = Animator.StringToHash("stunned");

	// Token: 0x0400028B RID: 651
	private int animMoving = Animator.StringToHash("moving");

	// Token: 0x0400028C RID: 652
	private int animJump = Animator.StringToHash("jump");

	// Token: 0x0400028D RID: 653
	private int animFalling = Animator.StringToHash("falling");

	// Token: 0x0400028E RID: 654
	private int animLand = Animator.StringToHash("land");

	// Token: 0x0400028F RID: 655
	private int animFuse = Animator.StringToHash("fuse");

	// Token: 0x04000290 RID: 656
	private float moveTimer;

	// Token: 0x04000291 RID: 657
	private bool spawnImpulse = true;

	// Token: 0x04000292 RID: 658
	private bool stunImpulse = true;

	// Token: 0x04000293 RID: 659
	private bool jumpImpulse = true;

	// Token: 0x04000294 RID: 660
	private bool landImpulse = true;

	// Token: 0x04000295 RID: 661
	private bool fuseImpulse = true;

	// Token: 0x04000296 RID: 662
	[Range(0f, 1f)]
	public float volumeMultiplier;

	// Token: 0x04000297 RID: 663
	[Space]
	public Sound soundImpactLight;

	// Token: 0x04000298 RID: 664
	public Sound soundImpactMedium;

	// Token: 0x04000299 RID: 665
	public Sound soundImpactHeavy;

	// Token: 0x0400029A RID: 666
	[Space]
	public Sound soundMoveShort;

	// Token: 0x0400029B RID: 667
	public Sound soundMoveLong;

	// Token: 0x0400029C RID: 668
	[Space]
	public Sound soundFootstep;

	// Token: 0x0400029D RID: 669
	[Space]
	public Sound soundHurt;

	// Token: 0x0400029E RID: 670
	public Sound soundDeathSFX;

	// Token: 0x0400029F RID: 671
	public Sound soundDeathVO;

	// Token: 0x040002A0 RID: 672
	[Space]
	public Sound soundJumpSFX;

	// Token: 0x040002A1 RID: 673
	public Sound soundJumpVO;

	// Token: 0x040002A2 RID: 674
	[Space]
	public Sound soundLandSFX;

	// Token: 0x040002A3 RID: 675
	public Sound soundLandVO;

	// Token: 0x040002A4 RID: 676
	[Space]
	public Sound soundStunIntro;

	// Token: 0x040002A5 RID: 677
	public Sound soundStunLoop;

	// Token: 0x040002A6 RID: 678
	public Sound soundStunOutro;

	// Token: 0x040002A7 RID: 679
	private float stunLoopPauseTimer;

	// Token: 0x040002A8 RID: 680
	[Space]
	public Sound soundIdleBreaker;

	// Token: 0x040002A9 RID: 681
	public Sound soundAttackBreaker;

	// Token: 0x040002AA RID: 682
	[Space]
	public Sound soundFuseTell;

	// Token: 0x040002AB RID: 683
	public Sound soundExplosionTell;

	// Token: 0x040002AC RID: 684
	[Space]
	public Sound soundFuseIgnite;

	// Token: 0x040002AD RID: 685
	public Sound soundFuseLoop;

	// Token: 0x040002AE RID: 686
	public AnimationCurve soundFuseLoopCurve;

	// Token: 0x040002AF RID: 687
	private float fuseLoopTimer;
}
