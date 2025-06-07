using System;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class EnemyRobeAnim : MonoBehaviour
{
	// Token: 0x060003CF RID: 975 RVA: 0x00026036 File Offset: 0x00024236
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x00026050 File Offset: 0x00024250
	private void Update()
	{
		if (this.controller.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
		else
		{
			this.animator.speed = 1f;
		}
		if (this.controller.isOnScreen)
		{
			this.animator.SetBool("isOnScreen", true);
		}
		else
		{
			this.animator.SetBool("isOnScreen", false);
		}
		if (this.controller.currentState == EnemyRobe.State.Despawn)
		{
			if (this.despawnImpulse)
			{
				this.animator.SetTrigger("despawn");
				this.despawnImpulse = false;
			}
		}
		else
		{
			this.despawnImpulse = true;
		}
		if (this.controller.attackImpulse)
		{
			this.controller.attackImpulse = false;
			if (!this.controller.enemy.IsStunned())
			{
				this.animator.SetTrigger("attack");
			}
		}
		if (this.controller.idleBreakTrigger)
		{
			this.controller.idleBreakTrigger = false;
			if (!this.controller.enemy.IsStunned())
			{
				this.animator.SetTrigger("idleBreak");
			}
		}
		if (this.controller.currentState == EnemyRobe.State.LookUnder || this.controller.currentState == EnemyRobe.State.LookUnderAttack)
		{
			if (this.lookUnderImpulse)
			{
				this.animator.SetTrigger("LookUnder");
				this.lookUnderImpulse = false;
			}
			this.animator.SetBool("LookingUnder", true);
		}
		else
		{
			this.animator.SetBool("LookingUnder", false);
			this.lookUnderImpulse = true;
		}
		if (this.controller.lookUnderAttackImpulse)
		{
			this.animator.SetTrigger("LookUnderAttack");
			this.controller.lookUnderAttackImpulse = false;
		}
		if (this.controller.currentState == EnemyRobe.State.Stun)
		{
			if (this.stunImpulse)
			{
				this.sfxStunStart.Play(this.controller.transform.position, 1f, 1f, 1f, 1f);
				this.animator.SetTrigger("Stun");
				this.stunImpulse = false;
			}
			this.animator.SetBool("Stunned", true);
			this.sfxStunLoop.PlayLoop(true, 2f, 2f, 1f);
		}
		else
		{
			this.sfxStunLoop.PlayLoop(false, 2f, 2f, 1f);
			this.animator.SetBool("Stunned", false);
			this.stunImpulse = true;
		}
		this.sfxTargetPlayerLoop.PlayLoop(this.isPlayingTargetPlayerLoop, 2f, 2f, 1f);
		this.sfxHandIdle.PlayLoop(this.isPlayingHandIdle, 2f, 2f, 1f);
		this.sfxHandAggressive.PlayLoop(this.isPlayingHandAggressive, 2f, 2f, 1f);
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x00026322 File Offset: 0x00024522
	public void SetSpawn()
	{
		this.animator.Play("Robe Spawn", 0, 0f);
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x0002633A File Offset: 0x0002453A
	public void SetDespawn()
	{
		this.controller.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x00026351 File Offset: 0x00024551
	public void TeleportParticlesStart()
	{
		this.teleportParticles.Play();
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x0002635E File Offset: 0x0002455E
	public void TeleportParticlesStop()
	{
		this.teleportParticles.Stop();
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x0002636B File Offset: 0x0002456B
	public void SpawnParticlesImpulse()
	{
		this.spawnParticles.Play();
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x00026378 File Offset: 0x00024578
	public void DeathParticlesImpulse()
	{
		ParticleSystem[] array = this.deathParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x000263A4 File Offset: 0x000245A4
	public void LookUnderIntro()
	{
		if (this.controller.targetPlayer.isLocal)
		{
			AudioScare.instance.PlaySoft();
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x00026429 File Offset: 0x00024629
	public void SfxTargetPlayerLoop()
	{
		if (this.controller.isOnScreen)
		{
			this.isPlayingTargetPlayerLoop = true;
			return;
		}
		this.isPlayingTargetPlayerLoop = false;
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x00026447 File Offset: 0x00024647
	public void SfxIdleBreak()
	{
		this.sfxIdleBreak.Play(this.controller.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060003DA RID: 986 RVA: 0x0002647C File Offset: 0x0002467C
	public void SfxAttack()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.sfxAttack.Play(this.visionTransform.transform.position, 1f, 1f, 1f, 1f);
		this.sfxAttackGlobal.Play(this.visionTransform.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060003DB RID: 987 RVA: 0x00026545 File Offset: 0x00024745
	public void SfxDeath()
	{
		this.sfxDeath.Play(this.visionTransform.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060003DC RID: 988 RVA: 0x00026578 File Offset: 0x00024778
	public void LookUnderAttack()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.sfxAttackUnder.Play(this.controller.transform.position, 1f, 1f, 1f, 1f);
		this.sfxAttackUnderGlobal.Play(this.controller.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x04000678 RID: 1656
	public EnemyRobe controller;

	// Token: 0x04000679 RID: 1657
	internal Animator animator;

	// Token: 0x0400067A RID: 1658
	public Transform visionTransform;

	// Token: 0x0400067B RID: 1659
	public ParticleSystem teleportParticles;

	// Token: 0x0400067C RID: 1660
	public ParticleSystem[] deathParticles;

	// Token: 0x0400067D RID: 1661
	public ParticleSystem spawnParticles;

	// Token: 0x0400067E RID: 1662
	[Header("Sounds")]
	public Sound sfxTargetPlayerLoop;

	// Token: 0x0400067F RID: 1663
	public Sound sfxIdleBreak;

	// Token: 0x04000680 RID: 1664
	public Sound sfxAttack;

	// Token: 0x04000681 RID: 1665
	public Sound sfxAttackGlobal;

	// Token: 0x04000682 RID: 1666
	public Sound sfxHurt;

	// Token: 0x04000683 RID: 1667
	public Sound sfxHandIdle;

	// Token: 0x04000684 RID: 1668
	public Sound sfxHandAggressive;

	// Token: 0x04000685 RID: 1669
	public Sound sfxStunStart;

	// Token: 0x04000686 RID: 1670
	public Sound sfxStunLoop;

	// Token: 0x04000687 RID: 1671
	public Sound sfxAttackUnder;

	// Token: 0x04000688 RID: 1672
	public Sound sfxAttackUnderGlobal;

	// Token: 0x04000689 RID: 1673
	public Sound sfxDeath;

	// Token: 0x0400068A RID: 1674
	public bool isPlayingTargetPlayerLoop;

	// Token: 0x0400068B RID: 1675
	public bool isPlayingHandIdle;

	// Token: 0x0400068C RID: 1676
	public bool isPlayingHandAggressive;

	// Token: 0x0400068D RID: 1677
	private bool stunImpulse;

	// Token: 0x0400068E RID: 1678
	private bool despawnImpulse;

	// Token: 0x0400068F RID: 1679
	private bool lookUnderImpulse;
}
