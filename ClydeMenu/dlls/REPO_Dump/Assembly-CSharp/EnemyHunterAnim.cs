using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200006D RID: 109
public class EnemyHunterAnim : MonoBehaviour
{
	// Token: 0x06000394 RID: 916 RVA: 0x00023F57 File Offset: 0x00022157
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x06000395 RID: 917 RVA: 0x00023F74 File Offset: 0x00022174
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
		if ((this.enemyHunter.currentState == EnemyHunter.State.Roam || this.enemyHunter.currentState == EnemyHunter.State.InvestigateWalk || this.enemyHunter.currentState == EnemyHunter.State.Leave) && (this.enemy.Rigidbody.velocity.magnitude > 0.2f || this.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude > 0.25f))
		{
			this.moveTimer = 0.1f;
		}
		if (this.moveTimer > 0f)
		{
			this.moveTimer -= Time.deltaTime;
			this.animator.SetBool("Moving", true);
		}
		else
		{
			this.animator.SetBool("Moving", false);
		}
		if (this.hummingStopTimer > 0f)
		{
			this.hummingStopTimer -= Time.deltaTime;
			this.soundHumming.PlayLoop(false, 2f, 20f, 1f);
		}
		else
		{
			this.soundHumming.PlayLoop(true, 2f, 2f, 1f);
		}
		if (this.enemyHunter.currentState == EnemyHunter.State.LeaveStart)
		{
			this.animator.SetBool("Leaving", true);
		}
		else
		{
			this.animator.SetBool("Leaving", false);
		}
		if (this.enemyHunter.currentState == EnemyHunter.State.Stun)
		{
			if (this.stunImpulse)
			{
				this.animator.SetTrigger("Stun");
				this.stunImpulse = false;
			}
			this.animator.SetBool("Stunned", true);
		}
		else
		{
			this.animator.SetBool("Stunned", false);
			this.stunImpulse = true;
		}
		if (this.enemyHunter.currentState == EnemyHunter.State.Aim)
		{
			this.animator.SetBool("Aiming", true);
		}
		else
		{
			this.animator.SetBool("Aiming", false);
		}
		if (this.enemyHunter.currentState == EnemyHunter.State.Shoot || this.enemyHunter.currentState == EnemyHunter.State.ShootEnd)
		{
			this.animator.SetBool("Shooting", true);
		}
		else
		{
			this.animator.SetBool("Shooting", false);
		}
		if (this.enemyHunter.currentState == EnemyHunter.State.Despawn)
		{
			this.animator.SetBool("Despawning", true);
			return;
		}
		this.animator.SetBool("Despawning", false);
	}

	// Token: 0x06000396 RID: 918 RVA: 0x000241F1 File Offset: 0x000223F1
	public void OnSpawn()
	{
		this.animator.SetBool("Stunned", false);
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x06000397 RID: 919 RVA: 0x0002421A File Offset: 0x0002241A
	public void StopHumming(float _multiplier)
	{
		this.hummingStopTimer = 30f * _multiplier;
	}

	// Token: 0x06000398 RID: 920 RVA: 0x0002422C File Offset: 0x0002242C
	public void TeleportEffect()
	{
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.05f);
		foreach (ParticleSystem particleSystem in this.teleportEffects)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x06000399 RID: 921 RVA: 0x000242D8 File Offset: 0x000224D8
	public void FootstepShort()
	{
		this.soundFootstepShort.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 1f, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00024358 File Offset: 0x00022558
	public void FootstepLong()
	{
		this.soundFootstepLong.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 1f, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x0600039B RID: 923 RVA: 0x000243D8 File Offset: 0x000225D8
	public void AimStart()
	{
		int num = Random.Range(0, this.aimStartClips.Length);
		this.soundAimStart.Sounds[0] = this.aimStartClips[num];
		this.soundAimStartGlobal.Sounds[0] = this.aimStartGlobalClips[num];
		this.soundAimStart.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		this.soundAimStartGlobal.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		this.StopHumming(1f);
	}

	// Token: 0x0600039C RID: 924 RVA: 0x00024489 File Offset: 0x00022689
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x0600039D RID: 925 RVA: 0x0002449B File Offset: 0x0002269B
	public void Reload01()
	{
		this.soundReload01.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600039E RID: 926 RVA: 0x000244CD File Offset: 0x000226CD
	public void Reload02()
	{
		this.soundReload02.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600039F RID: 927 RVA: 0x000244FF File Offset: 0x000226FF
	public void MoveShort()
	{
		this.soundMoveShort.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x00024531 File Offset: 0x00022731
	public void MoveLong()
	{
		this.soundMoveLong.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x00024563 File Offset: 0x00022763
	public void GunLong()
	{
		this.soundGunLong.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00024595 File Offset: 0x00022795
	public void GunShort()
	{
		this.soundGunShort.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x000245C7 File Offset: 0x000227C7
	public void Spawn()
	{
		this.soundSpawn.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x000245F9 File Offset: 0x000227F9
	public void DespawnSound()
	{
		this.soundDespawn.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x0002462B File Offset: 0x0002282B
	public void LeaveStartSound()
	{
		this.soundLeaveStart.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		this.StopHumming(0.25f);
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x00024668 File Offset: 0x00022868
	public void LeaveStartDone()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.enemyHunter.currentState == EnemyHunter.State.LeaveStart)
		{
			this.enemyHunter.stateTimer = 0f;
		}
	}

	// Token: 0x0400063F RID: 1599
	public Enemy enemy;

	// Token: 0x04000640 RID: 1600
	public EnemyHunter enemyHunter;

	// Token: 0x04000641 RID: 1601
	internal Animator animator;

	// Token: 0x04000642 RID: 1602
	public Materials.MaterialTrigger material;

	// Token: 0x04000643 RID: 1603
	private float moveTimer;

	// Token: 0x04000644 RID: 1604
	private bool stunImpulse;

	// Token: 0x04000645 RID: 1605
	internal bool spawnImpulse;

	// Token: 0x04000646 RID: 1606
	private float hummingStopTimer;

	// Token: 0x04000647 RID: 1607
	private bool humming;

	// Token: 0x04000648 RID: 1608
	[Space]
	public List<ParticleSystem> teleportEffects;

	// Token: 0x04000649 RID: 1609
	[Space]
	public Sound soundFootstepShort;

	// Token: 0x0400064A RID: 1610
	public Sound soundFootstepLong;

	// Token: 0x0400064B RID: 1611
	public Sound soundReload01;

	// Token: 0x0400064C RID: 1612
	public Sound soundAimStart;

	// Token: 0x0400064D RID: 1613
	public Sound soundAimStartGlobal;

	// Token: 0x0400064E RID: 1614
	public Sound soundReload02;

	// Token: 0x0400064F RID: 1615
	public Sound soundMoveShort;

	// Token: 0x04000650 RID: 1616
	public Sound soundMoveLong;

	// Token: 0x04000651 RID: 1617
	public Sound soundGunLong;

	// Token: 0x04000652 RID: 1618
	public Sound soundGunShort;

	// Token: 0x04000653 RID: 1619
	public Sound soundSpawn;

	// Token: 0x04000654 RID: 1620
	public Sound soundDespawn;

	// Token: 0x04000655 RID: 1621
	public Sound soundLeaveStart;

	// Token: 0x04000656 RID: 1622
	public Sound soundHumming;

	// Token: 0x04000657 RID: 1623
	[Space]
	public AudioClip[] aimStartClips;

	// Token: 0x04000658 RID: 1624
	public AudioClip[] aimStartGlobalClips;
}
