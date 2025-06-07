using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000049 RID: 73
public class EnemyDuckAnim : MonoBehaviour
{
	// Token: 0x06000220 RID: 544 RVA: 0x00016184 File Offset: 0x00014384
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x06000221 RID: 545 RVA: 0x000161A0 File Offset: 0x000143A0
	private void Update()
	{
		if (this.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
		else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") && !this.animator.IsInTransition(0))
		{
			this.animator.speed = Mathf.Clamp(this.enemy.Rigidbody.velocity.magnitude + 0.2f, 0.8f, 1.2f);
		}
		else
		{
			this.animator.speed = 1f;
		}
		if (this.controller.currentState != EnemyDuck.State.AttackStart && this.controller.currentState != EnemyDuck.State.Transform && this.controller.currentState != EnemyDuck.State.ChaseNavmesh && this.controller.currentState != EnemyDuck.State.ChaseTowards && this.controller.currentState != EnemyDuck.State.ChaseMoveBack && this.controller.currentState != EnemyDuck.State.DeTransform)
		{
			if (this.enemy.Rigidbody.velocity.magnitude > 0.1f)
			{
				this.animator.SetBool("move", true);
			}
			else
			{
				this.animator.SetBool("move", false);
			}
			if (!this.enemy.IsStunned())
			{
				if (!this.enemy.Grounded.grounded && (this.controller.currentState == EnemyDuck.State.FlyBackToNavmesh || this.controller.currentState == EnemyDuck.State.FlyBackToNavmeshStop))
				{
					if (this.flyImpulse)
					{
						this.animator.SetTrigger("fly");
						this.animator.SetBool("falling", false);
						this.flyImpulse = false;
						this.landImpulse = true;
					}
					else if (this.controller.currentState == EnemyDuck.State.FlyBackToNavmeshStop)
					{
						this.animator.SetBool("falling", true);
					}
				}
				else if (this.enemy.Jump.jumping)
				{
					if (this.jumpImpulse)
					{
						this.animator.SetTrigger("jump");
						this.animator.SetBool("falling", false);
						this.jumpImpulse = false;
						this.landImpulse = true;
					}
					else if (this.controller.enemy.Rigidbody.physGrabObject.rbVelocity.y < 0f)
					{
						this.animator.SetBool("falling", true);
					}
				}
				else
				{
					if (this.landImpulse)
					{
						this.animator.SetTrigger("land");
						this.landImpulse = false;
					}
					this.animator.SetBool("falling", false);
					this.jumpImpulse = true;
					this.flyImpulse = true;
				}
			}
		}
		if (this.controller.currentState == EnemyDuck.State.AttackStart)
		{
			if (this.transformImpulse)
			{
				this.animator.SetTrigger("transform");
				this.transformImpulse = false;
			}
		}
		else
		{
			this.transformImpulse = true;
		}
		if (this.controller.currentState == EnemyDuck.State.AttackStart || this.controller.currentState == EnemyDuck.State.Transform || this.controller.currentState == EnemyDuck.State.ChaseNavmesh || this.controller.currentState == EnemyDuck.State.ChaseTowards || this.controller.currentState == EnemyDuck.State.ChaseMoveBack)
		{
			this.animator.SetBool("move", false);
			this.animator.SetBool("chase", true);
			if (this.soundHurtPauseTimer > 0f)
			{
				this.StopAttackSound();
			}
			else
			{
				this.attackLoopSound.PlayLoop(true, 5f, 5f, 1f);
			}
		}
		else
		{
			this.StopAttackSound();
			this.animator.SetBool("chase", false);
		}
		if (this.controller.currentState == EnemyDuck.State.Notice)
		{
			if (this.noticeImpulse)
			{
				this.animator.SetTrigger("notice");
				this.noticeImpulse = false;
			}
		}
		else
		{
			this.noticeImpulse = true;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !this.animator.IsInTransition(0))
		{
			this.idleBreakerTimer += Time.deltaTime;
			if (this.idleBreakerTimer > 5f)
			{
				this.idleBreakerTimer = 0f;
				if (Random.Range(0, 100) < 35)
				{
					this.controller.IdleBreakerSet();
				}
			}
		}
		if (this.controller.idleBreakerTrigger)
		{
			this.animator.SetTrigger("idlebreak");
			this.controller.idleBreakerTrigger = false;
		}
		if (this.controller.currentState == EnemyDuck.State.Stun)
		{
			this.landImpulse = false;
			if (this.stunImpulse)
			{
				this.animator.SetTrigger("stun");
				this.stunImpulse = false;
			}
			this.animator.SetBool("stunned", true);
			if (this.soundHurtPauseTimer > 0f)
			{
				this.stunSound.PlayLoop(false, 5f, 2f, 1f);
			}
			else
			{
				this.stunSound.PlayLoop(true, 5f, 5f, 1f);
			}
		}
		else
		{
			this.animator.SetBool("stunned", false);
			this.stunSound.PlayLoop(false, 5f, 1f, 1f);
			if (!this.stunImpulse)
			{
				this.stunStopSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.stunImpulse = true;
			}
		}
		if (this.controller.currentState == EnemyDuck.State.Despawn)
		{
			this.animator.SetBool("despawning", true);
		}
		else
		{
			this.animator.SetBool("despawning", false);
		}
		if (this.controller.currentState == EnemyDuck.State.FlyBackToNavmesh)
		{
			this.flyLoopSound.PlayLoop(true, 5f, 2f, 1f);
		}
		else
		{
			this.flyLoopSound.PlayLoop(false, 5f, 2f, 1f);
		}
		if (this.soundHurtPauseTimer > 0f)
		{
			this.soundHurtPauseTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000222 RID: 546 RVA: 0x0001679C File Offset: 0x0001499C
	public void OnSpawn()
	{
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x06000223 RID: 547 RVA: 0x000167B4 File Offset: 0x000149B4
	private void Quack()
	{
		this.quackSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (this.controller.currentState == EnemyDuck.State.Idle || this.controller.currentState == EnemyDuck.State.Roam || this.controller.currentState == EnemyDuck.State.Investigate || this.controller.currentState == EnemyDuck.State.Leave || this.controller.currentState == EnemyDuck.State.MoveBackToNavmesh)
			{
				return;
			}
			EnemyDirector.instance.SetInvestigate(base.transform.position, 10f, false);
		}
	}

	// Token: 0x06000224 RID: 548 RVA: 0x00016860 File Offset: 0x00014A60
	private void BiteSound()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.Rigidbody.GrabRelease();
		}
		GameDirector.instance.CameraShake.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.1f);
		this.biteSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0001690C File Offset: 0x00014B0C
	private void TransformSound()
	{
		if (!base.enabled)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, Camera.main.transform.position) < 10f)
		{
			AudioScare.instance.PlayImpact();
		}
		this.transformSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0001697D File Offset: 0x00014B7D
	private void JumpSound()
	{
		this.jumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000227 RID: 551 RVA: 0x000169AA File Offset: 0x00014BAA
	private void FootstepSound()
	{
		this.footstepSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000228 RID: 552 RVA: 0x000169D7 File Offset: 0x00014BD7
	private void MouthExtendSound()
	{
		this.mouthExtendSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000229 RID: 553 RVA: 0x00016A04 File Offset: 0x00014C04
	private void MouthRetractSound()
	{
		this.mouthRetractSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600022A RID: 554 RVA: 0x00016A31 File Offset: 0x00014C31
	private void StopAttackSound()
	{
		this.attackLoopSound.PlayLoop(false, 5f, 5f, 1f);
	}

	// Token: 0x0600022B RID: 555 RVA: 0x00016A4E File Offset: 0x00014C4E
	private void NoticeSound()
	{
		this.noticeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600022C RID: 556 RVA: 0x00016A7B File Offset: 0x00014C7B
	private void FlyFlapSound()
	{
		this.flyFlapSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600022D RID: 557 RVA: 0x00016AA8 File Offset: 0x00014CA8
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x040003FE RID: 1022
	public Enemy enemy;

	// Token: 0x040003FF RID: 1023
	internal Animator animator;

	// Token: 0x04000400 RID: 1024
	public EnemyDuck controller;

	// Token: 0x04000401 RID: 1025
	public Sound quackSound;

	// Token: 0x04000402 RID: 1026
	public Sound stunSound;

	// Token: 0x04000403 RID: 1027
	public Sound stunStopSound;

	// Token: 0x04000404 RID: 1028
	public Sound biteSound;

	// Token: 0x04000405 RID: 1029
	public Sound transformSound;

	// Token: 0x04000406 RID: 1030
	public Sound jumpSound;

	// Token: 0x04000407 RID: 1031
	public Sound footstepSound;

	// Token: 0x04000408 RID: 1032
	public Sound mouthExtendSound;

	// Token: 0x04000409 RID: 1033
	public Sound mouthRetractSound;

	// Token: 0x0400040A RID: 1034
	public Sound attackLoopSound;

	// Token: 0x0400040B RID: 1035
	public Sound hurtSound;

	// Token: 0x0400040C RID: 1036
	public Sound deathSound;

	// Token: 0x0400040D RID: 1037
	public Sound noticeSound;

	// Token: 0x0400040E RID: 1038
	public Sound flyFlapSound;

	// Token: 0x0400040F RID: 1039
	public Sound flyLoopSound;

	// Token: 0x04000410 RID: 1040
	public float soundHurtPauseTimer;

	// Token: 0x04000411 RID: 1041
	private bool jumpImpulse;

	// Token: 0x04000412 RID: 1042
	private bool flyImpulse;

	// Token: 0x04000413 RID: 1043
	private bool landImpulse;

	// Token: 0x04000414 RID: 1044
	private bool stunImpulse;

	// Token: 0x04000415 RID: 1045
	private bool noticeImpulse;

	// Token: 0x04000416 RID: 1046
	private bool transformImpulse;

	// Token: 0x04000417 RID: 1047
	private float idleBreakerTimer;
}
