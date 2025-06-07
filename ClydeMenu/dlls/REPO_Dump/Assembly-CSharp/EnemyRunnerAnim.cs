using System;
using UnityEngine;

// Token: 0x02000072 RID: 114
public class EnemyRunnerAnim : MonoBehaviour
{
	// Token: 0x06000404 RID: 1028 RVA: 0x00028411 File Offset: 0x00026611
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x0002842C File Offset: 0x0002662C
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
		if (!this.stunned && this.enemy.Jump.jumping)
		{
			if (this.jumpImpulse)
			{
				this.animator.SetTrigger(this.animJump);
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
				this.animator.SetTrigger(this.animLand);
				this.landImpulse = false;
			}
			this.animator.SetBool(this.animFalling, false);
			this.jumpImpulse = true;
		}
		if (this.controller.currentState == EnemyRunner.State.LookUnder)
		{
			if (this.lookUnderImpulse)
			{
				this.animator.SetTrigger(this.animLookUnder);
				this.lookUnderImpulse = false;
			}
			this.animator.SetBool(this.animLookingUnder, true);
		}
		else
		{
			this.animator.SetBool(this.animLookingUnder, false);
			this.lookUnderImpulse = true;
		}
		float num = 0.05f;
		if (this.IsMoving() && (this.enemy.Rigidbody.velocity.magnitude > num || this.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude > num))
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
		if (this.controller.currentState == EnemyRunner.State.SeekPlayer || this.controller.currentState == EnemyRunner.State.Sneak)
		{
			this.animator.SetBool(this.animSeeking, true);
		}
		else
		{
			this.animator.SetBool(this.animSeeking, false);
		}
		if (this.controller.currentState == EnemyRunner.State.AttackPlayer || this.controller.currentState == EnemyRunner.State.AttackPlayerOver || this.controller.currentState == EnemyRunner.State.AttackPlayerBackToNavMesh || this.controller.currentState == EnemyRunner.State.StuckAttack || this.controller.currentState == EnemyRunner.State.LookUnderStart)
		{
			this.animator.SetBool(this.animAttacking, true);
		}
		else
		{
			this.animator.SetBool(this.animAttacking, false);
		}
		if (this.controller.currentState == EnemyRunner.State.Notice || this.controller.currentState == EnemyRunner.State.StuckAttackNotice)
		{
			if (this.noticeImpulse)
			{
				this.animator.SetTrigger(this.animNotice);
				this.noticeImpulse = false;
			}
		}
		else
		{
			this.noticeImpulse = true;
		}
		if (this.controller.currentState == EnemyRunner.State.Stun)
		{
			if (this.stunImpulse)
			{
				this.animator.SetTrigger(this.animStun);
				this.stunImpulse = false;
			}
			this.animator.SetBool(this.animStunned, true);
			this.stunned = true;
		}
		else
		{
			this.animator.SetBool(this.animStunned, false);
			this.stunImpulse = true;
			this.stunned = false;
		}
		this.sfxStunnedLoop.PlayLoop(this.stunned, 5f, 5f, 1f);
		if (this.controller.currentState == EnemyRunner.State.Despawn)
		{
			if (this.despawnImpulse)
			{
				this.animator.SetTrigger(this.animDespawn);
				this.despawnImpulse = false;
			}
		}
		else
		{
			this.despawnImpulse = true;
		}
		if (this.controller.currentState == EnemyRunner.State.Leave)
		{
			this.animator.SetBool(this.animLeaving, true);
			return;
		}
		this.animator.SetBool(this.animLeaving, false);
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00028814 File Offset: 0x00026A14
	public void OnSpawn()
	{
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x0002882C File Offset: 0x00026A2C
	private bool IsMoving()
	{
		return this.controller.currentState == EnemyRunner.State.Roam || this.controller.currentState == EnemyRunner.State.Investigate || this.controller.currentState == EnemyRunner.State.SeekPlayer || this.controller.currentState == EnemyRunner.State.Sneak || this.controller.currentState == EnemyRunner.State.Leave;
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00028882 File Offset: 0x00026A82
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00028894 File Offset: 0x00026A94
	public void LookUnderIntro()
	{
		if (!Camera.main)
		{
			return;
		}
		if (!AudioScare.instance)
		{
			return;
		}
		if (!GameDirector.instance)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, Camera.main.transform.position) < 10f)
		{
			AudioScare.instance.PlaySoft();
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00028954 File Offset: 0x00026B54
	public void SfxJump()
	{
		this.sfxJump.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.ShakeDistance(1f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(1f, 3f, 8f, base.transform.position, 0.5f);
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x000289E8 File Offset: 0x00026BE8
	public void SfxHurt()
	{
		this.sfxHurt.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x00028A15 File Offset: 0x00026C15
	public void SfxDeath()
	{
		this.sfxDeath.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x00028A42 File Offset: 0x00026C42
	public void SfxMoveShort()
	{
		this.sfxMoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x00028A6F File Offset: 0x00026C6F
	public void SfxMoveLong()
	{
		this.sfxMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00028A9C File Offset: 0x00026C9C
	public void SfxFootstepSlow()
	{
		this.sfxFootstepSlow.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 1f, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x00028B18 File Offset: 0x00026D18
	public void SfxFootstepFast()
	{
		this.sfxFootstepFast.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 1f, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00028B91 File Offset: 0x00026D91
	public void SfxAttackSlash()
	{
		this.sfxAttackSlash.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00028BC0 File Offset: 0x00026DC0
	public void SfxAttackGrunt()
	{
		if (this.attackGruntImpulse)
		{
			this.sfxAttackGrunt.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.attackGruntImpulse = false;
			return;
		}
		this.attackGruntImpulse = true;
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00028C10 File Offset: 0x00026E10
	public void SfxAttackUnderGrunt()
	{
		this.attackGruntCounter++;
		if (this.attackGruntCounter >= 3)
		{
			this.sfxAttackGrunt.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.attackGruntCounter = 0;
		}
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x00028C66 File Offset: 0x00026E66
	public void ResetAttackGruntCounter()
	{
		this.attackGruntCounter = 3;
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x00028C6F File Offset: 0x00026E6F
	public void SfxStunnedLoop()
	{
		this.sfxStunnedLoop.PlayLoop(this.stunned, 5f, 5f, 1f);
	}

	// Token: 0x040006B1 RID: 1713
	private int animMoving = Animator.StringToHash("moving");

	// Token: 0x040006B2 RID: 1714
	private int animSeeking = Animator.StringToHash("seeking");

	// Token: 0x040006B3 RID: 1715
	private int animAttacking = Animator.StringToHash("attacking");

	// Token: 0x040006B4 RID: 1716
	private int animStunned = Animator.StringToHash("stunned");

	// Token: 0x040006B5 RID: 1717
	private int animFalling = Animator.StringToHash("falling");

	// Token: 0x040006B6 RID: 1718
	private int animLookingUnder = Animator.StringToHash("lookingUnder");

	// Token: 0x040006B7 RID: 1719
	private int animLeaving = Animator.StringToHash("leaving");

	// Token: 0x040006B8 RID: 1720
	private int animLand = Animator.StringToHash("Land");

	// Token: 0x040006B9 RID: 1721
	private int animLookUnder = Animator.StringToHash("LookUnder");

	// Token: 0x040006BA RID: 1722
	private int animJump = Animator.StringToHash("Jump");

	// Token: 0x040006BB RID: 1723
	private int animNotice = Animator.StringToHash("Notice");

	// Token: 0x040006BC RID: 1724
	private int animStun = Animator.StringToHash("Stun");

	// Token: 0x040006BD RID: 1725
	private int animDespawn = Animator.StringToHash("Despawn");

	// Token: 0x040006BE RID: 1726
	public Enemy enemy;

	// Token: 0x040006BF RID: 1727
	public EnemyRunner controller;

	// Token: 0x040006C0 RID: 1728
	internal Animator animator;

	// Token: 0x040006C1 RID: 1729
	internal Materials.MaterialTrigger material = new Materials.MaterialTrigger();

	// Token: 0x040006C2 RID: 1730
	private bool stunned;

	// Token: 0x040006C3 RID: 1731
	private float moveTimer;

	// Token: 0x040006C4 RID: 1732
	private bool stunImpulse;

	// Token: 0x040006C5 RID: 1733
	private bool despawnImpulse;

	// Token: 0x040006C6 RID: 1734
	internal bool spawnImpulse;

	// Token: 0x040006C7 RID: 1735
	private bool landImpulse;

	// Token: 0x040006C8 RID: 1736
	private bool lookUnderImpulse;

	// Token: 0x040006C9 RID: 1737
	private bool noticeImpulse;

	// Token: 0x040006CA RID: 1738
	private bool jumpImpulse;

	// Token: 0x040006CB RID: 1739
	private float jumpedTimer;

	// Token: 0x040006CC RID: 1740
	[Header("One Shots")]
	public Sound sfxJump;

	// Token: 0x040006CD RID: 1741
	public Sound sfxHurt;

	// Token: 0x040006CE RID: 1742
	public Sound sfxDeath;

	// Token: 0x040006CF RID: 1743
	public Sound sfxMoveShort;

	// Token: 0x040006D0 RID: 1744
	public Sound sfxMoveLong;

	// Token: 0x040006D1 RID: 1745
	public Sound sfxFootstepSlow;

	// Token: 0x040006D2 RID: 1746
	public Sound sfxFootstepFast;

	// Token: 0x040006D3 RID: 1747
	public Sound sfxAttackSlash;

	// Token: 0x040006D4 RID: 1748
	public Sound sfxAttackGrunt;

	// Token: 0x040006D5 RID: 1749
	private bool attackGruntImpulse = true;

	// Token: 0x040006D6 RID: 1750
	private int attackGruntCounter;

	// Token: 0x040006D7 RID: 1751
	[Header("Loops")]
	public Sound sfxStunnedLoop;
}
