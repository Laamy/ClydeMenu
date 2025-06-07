using System;
using UnityEngine;

// Token: 0x02000089 RID: 137
public class EnemyValuableThrowerAnim : MonoBehaviour
{
	// Token: 0x060005BB RID: 1467 RVA: 0x00038D60 File Offset: 0x00036F60
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x00038D7C File Offset: 0x00036F7C
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
		base.transform.position = this.followTarget.position;
		base.transform.rotation = this.followTarget.rotation;
		if (this.enemy.Rigidbody.velocity.magnitude > 0.5f)
		{
			this.animator.SetBool("Move", true);
			this.animator.SetBool("Move Slow", false);
		}
		else if (this.enemy.Rigidbody.velocity.magnitude > 0.2f)
		{
			this.animator.SetBool("Move", false);
			this.animator.SetBool("Move Slow", true);
		}
		else
		{
			this.animator.SetBool("Move", false);
			this.animator.SetBool("Move Slow", false);
		}
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			this.stun = false;
			this.animator.SetBool("Despawn", true);
		}
		else
		{
			this.animator.SetBool("Despawn", false);
		}
		if (this.controller.currentState == EnemyValuableThrower.State.PickUpTarget || this.controller.currentState == EnemyValuableThrower.State.TargetPlayer)
		{
			this.animator.SetBool("Pickup", true);
		}
		else
		{
			this.animator.SetBool("Pickup", false);
		}
		if (this.enemy.Jump.jumping)
		{
			this.animator.SetBool("Jumping", true);
		}
		else
		{
			this.animator.SetBool("Jumping", false);
		}
		if (this.enemy.IsStunned())
		{
			this.stun = true;
			this.animator.SetBool("Stun", true);
		}
		else
		{
			this.stun = false;
			this.animator.SetBool("Stun", false);
		}
		if (this.stun && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("Stun"))
		{
			this.animator.SetTrigger("Stun Impulse");
		}
		this.stunSound.PlayLoop(this.stun, 10f, 10f, 1f);
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x00038FCD File Offset: 0x000371CD
	public void OnSpawn()
	{
		this.stun = false;
		this.animator.SetBool("Stun", false);
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x00038FFD File Offset: 0x000371FD
	public void NoticeSet(int _playerID)
	{
		this.animator.SetTrigger("Notice");
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x0003900F File Offset: 0x0003720F
	public void ResetStateTimer()
	{
		this.controller.ResetStateTimer();
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x0003901C File Offset: 0x0003721C
	public void SpawnStart()
	{
		this.spawnSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x00039049 File Offset: 0x00037249
	public void DespawnStart()
	{
		this.despawnSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x00039076 File Offset: 0x00037276
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x00039088 File Offset: 0x00037288
	public void Throw()
	{
		this.pickupOutroThrowSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.controller.Throw();
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x000390C0 File Offset: 0x000372C0
	public void Footstep()
	{
		this.footstepSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x00039128 File Offset: 0x00037328
	public void FootstepSmall()
	{
		this.footstepSmallSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Light, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x0003918D File Offset: 0x0003738D
	public void MoveShort()
	{
		this.moveShortSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x000391BA File Offset: 0x000373BA
	public void MoveLong()
	{
		this.moveLongSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x000391E7 File Offset: 0x000373E7
	public void Jump()
	{
		this.jumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x00039214 File Offset: 0x00037414
	public void Land()
	{
		this.landSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00039241 File Offset: 0x00037441
	public void PickupIntro()
	{
		this.pickupIntroSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x0003926E File Offset: 0x0003746E
	public void PickupOutro()
	{
		this.pickupOutroTellSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x0003929B File Offset: 0x0003749B
	public void StunStop()
	{
		this.stunStopSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x000392C8 File Offset: 0x000374C8
	public void Notice()
	{
		this.noticeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x040008F5 RID: 2293
	public Transform followTarget;

	// Token: 0x040008F6 RID: 2294
	public EnemyValuableThrower controller;

	// Token: 0x040008F7 RID: 2295
	public Enemy enemy;

	// Token: 0x040008F8 RID: 2296
	public Materials.MaterialTrigger material;

	// Token: 0x040008F9 RID: 2297
	internal Animator animator;

	// Token: 0x040008FA RID: 2298
	[Space]
	public ParticleSystem particleBits;

	// Token: 0x040008FB RID: 2299
	public ParticleSystem particleImpact;

	// Token: 0x040008FC RID: 2300
	public ParticleSystem particleDirectionalBits;

	// Token: 0x040008FD RID: 2301
	private bool stun;

	// Token: 0x040008FE RID: 2302
	[Space]
	public Sound footstepSound;

	// Token: 0x040008FF RID: 2303
	public Sound footstepSmallSound;

	// Token: 0x04000900 RID: 2304
	[Space]
	public Sound moveShortSound;

	// Token: 0x04000901 RID: 2305
	public Sound moveLongSound;

	// Token: 0x04000902 RID: 2306
	[Space]
	public Sound spawnSound;

	// Token: 0x04000903 RID: 2307
	public Sound despawnSound;

	// Token: 0x04000904 RID: 2308
	[Space]
	public Sound jumpSound;

	// Token: 0x04000905 RID: 2309
	public Sound landSound;

	// Token: 0x04000906 RID: 2310
	public Sound noticeSound;

	// Token: 0x04000907 RID: 2311
	[Space]
	public Sound pickupIntroSound;

	// Token: 0x04000908 RID: 2312
	public Sound pickupOutroTellSound;

	// Token: 0x04000909 RID: 2313
	public Sound pickupOutroThrowSound;

	// Token: 0x0400090A RID: 2314
	[Space]
	public Sound stunSound;

	// Token: 0x0400090B RID: 2315
	public Sound stunStopSound;

	// Token: 0x0400090C RID: 2316
	[Space]
	public Sound hurtSound;

	// Token: 0x0400090D RID: 2317
	public Sound deathSound;
}
