using System;
using UnityEngine;

// Token: 0x02000087 RID: 135
public class EnemyUpscreamAnim : MonoBehaviour
{
	// Token: 0x0600058B RID: 1419 RVA: 0x00036EC5 File Offset: 0x000350C5
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x00036EE0 File Offset: 0x000350E0
	private void Update()
	{
		this.SetAnimationSpeed();
		if (this.controller.enemy.CurrentState == EnemyState.Despawn)
		{
			this.animator.SetBool("despawn", true);
		}
		else
		{
			this.animator.SetBool("despawn", false);
		}
		if (this.controller.currentState == EnemyUpscream.State.IdleBreak)
		{
			if (this.idleBreakImpulse)
			{
				this.animator.SetTrigger("IdleBreak");
				this.idleBreakImpulse = false;
			}
		}
		else
		{
			this.idleBreakImpulse = true;
		}
		if (this.enemy.Jump.jumping)
		{
			this.animator.SetBool("jumping", true);
			if (this.jumpImpulse)
			{
				this.animator.SetTrigger("Jump");
				this.animator.SetBool("falling", false);
				this.jumpImpulse = false;
			}
		}
		else
		{
			this.animator.SetBool("jumping", false);
			this.jumpImpulse = true;
		}
		if (this.enemy.Rigidbody.physGrabObject.rbVelocity.y < -0.1f)
		{
			this.animator.SetBool("falling", true);
		}
		else
		{
			this.animator.SetBool("falling", false);
		}
		if (this.enemy.Rigidbody.physGrabObject.rbVelocity.magnitude > 0.1f || this.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude > 0.5f)
		{
			this.moveTimer = 0.2f;
		}
		if (this.moveTimer > 0f)
		{
			this.moveTimer -= Time.deltaTime;
			this.animator.SetBool("move", true);
		}
		else
		{
			this.animator.SetBool("move", false);
		}
		if (this.controller.currentState == EnemyUpscream.State.Stun)
		{
			if (this.stunImpulse)
			{
				this.animator.SetTrigger("Stun");
				this.stunImpulse = false;
			}
			this.animator.SetBool("stunned", true);
			return;
		}
		this.animator.SetBool("stunned", false);
		this.stunImpulse = true;
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x000370FB File Offset: 0x000352FB
	public void SetSpawn()
	{
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x00037113 File Offset: 0x00035313
	public void SetDespawn()
	{
		this.controller.UpdateState(EnemyUpscream.State.Spawn);
		this.controller.enemy.EnemyParent.Despawn();
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x00037136 File Offset: 0x00035336
	public void NoticeSet(int _playerID)
	{
		this.animator.SetTrigger("Notice");
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x00037148 File Offset: 0x00035348
	public void TeleportParticlesStart()
	{
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x0003714A File Offset: 0x0003534A
	public void TeleportParticlesStop()
	{
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x0003714C File Offset: 0x0003534C
	public void SfxImpactFootstep()
	{
		if (this.enemy.Grounded.grounded)
		{
			this.stepSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
		}
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x000371C3 File Offset: 0x000353C3
	public void SfxIdleBreak()
	{
		this.sfxIdleBreak.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x000371F0 File Offset: 0x000353F0
	public void SfxAttack()
	{
		this.sfxAttackLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.sfxAttackGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x00037253 File Offset: 0x00035453
	public void Jump()
	{
		this.jumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x00037280 File Offset: 0x00035480
	public void Land()
	{
		this.landSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x000372AD File Offset: 0x000354AD
	public void DespawnSound()
	{
		this.despawnSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x000372DC File Offset: 0x000354DC
	public void AttackImpulse()
	{
		if ((!SemiFunc.IsMultiplayer() || SemiFunc.IsMasterClient()) && this.controller.targetPlayer)
		{
			Vector3 a = (this.controller.targetPlayer.transform.position - base.transform.position).normalized;
			a = Vector3.Lerp(a, Vector3.up, 0.6f);
			this.controller.targetPlayer.tumble.TumbleRequest(true, false);
			this.controller.targetPlayer.tumble.TumbleForce(a * 45f);
			this.controller.targetPlayer.tumble.TumbleTorque(-this.controller.targetPlayer.transform.right * 45f);
			this.controller.targetPlayer.tumble.TumbleOverrideTime(2f);
			this.controller.targetPlayer.tumble.ImpactHurtSet(3f, 10);
		}
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x000373F4 File Offset: 0x000355F4
	private void SetAnimationSpeed()
	{
		if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
		{
			float num = this.enemy.Rigidbody.physGrabObject.rbVelocity.magnitude + this.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude;
			num = Mathf.Clamp(num, 0.5f, 4f);
			this.animator.speed = num * 0.6f;
		}
		else
		{
			this.animator.speed = 1f;
		}
		if (this.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
	}

	// Token: 0x040008D5 RID: 2261
	public Enemy enemy;

	// Token: 0x040008D6 RID: 2262
	public EnemyUpscream controller;

	// Token: 0x040008D7 RID: 2263
	internal Animator animator;

	// Token: 0x040008D8 RID: 2264
	public Materials.MaterialTrigger material;

	// Token: 0x040008D9 RID: 2265
	private bool idleBreakImpulse;

	// Token: 0x040008DA RID: 2266
	private bool stunImpulse;

	// Token: 0x040008DB RID: 2267
	private bool jumpImpulse;

	// Token: 0x040008DC RID: 2268
	public Sound sfxAttackLocal;

	// Token: 0x040008DD RID: 2269
	public Sound sfxAttackGlobal;

	// Token: 0x040008DE RID: 2270
	public Sound hurtSound;

	// Token: 0x040008DF RID: 2271
	public Sound jumpSound;

	// Token: 0x040008E0 RID: 2272
	public Sound landSound;

	// Token: 0x040008E1 RID: 2273
	public Sound stepSound;

	// Token: 0x040008E2 RID: 2274
	public Sound sfxIdleBreak;

	// Token: 0x040008E3 RID: 2275
	public Sound despawnSound;

	// Token: 0x040008E4 RID: 2276
	public bool isPlayingTargetPlayerLoop;

	// Token: 0x040008E5 RID: 2277
	private float currentSpeed;

	// Token: 0x040008E6 RID: 2278
	private float moveTimer;
}
