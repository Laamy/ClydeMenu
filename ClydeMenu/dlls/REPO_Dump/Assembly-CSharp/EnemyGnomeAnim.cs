using System;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class EnemyGnomeAnim : MonoBehaviour
{
	// Token: 0x060002A8 RID: 680 RVA: 0x0001B672 File Offset: 0x00019872
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0001B68C File Offset: 0x0001988C
	private void Update()
	{
		if (this.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
		else
		{
			this.animator.speed = 1f;
			if (this.enemyGnome.currentState == EnemyGnome.State.Stun)
			{
				this.animator.speed = Mathf.Clamp(this.enemyGnome.enemy.Rigidbody.physGrabObject.rbVelocity.magnitude, 1f, 3f);
			}
		}
		if (this.enemyGnome.currentState == EnemyGnome.State.Attack)
		{
			if (this.attackImpulse)
			{
				this.animator.SetTrigger("Attack");
				this.attackImpulse = false;
			}
		}
		else
		{
			this.attackImpulse = true;
		}
		bool flag = false;
		if (this.enemyGnome.currentState == EnemyGnome.State.Stun)
		{
			flag = true;
			this.landImpulse = false;
			if (this.stunImpulse)
			{
				this.soundStun.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
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
		if (!flag && this.enemy.Jump.jumping)
		{
			if (this.jumpImpulse)
			{
				this.animator.SetTrigger("Jump");
				this.animator.SetBool("Falling", false);
				this.jumpImpulse = false;
				this.landImpulse = true;
			}
			else if (this.enemyGnome.enemy.Rigidbody.physGrabObject.rbVelocity.y < 0f)
			{
				this.animator.SetBool("Falling", true);
			}
		}
		else
		{
			if (this.landImpulse)
			{
				this.animator.SetTrigger("Land");
				this.landImpulse = false;
			}
			this.animator.SetBool("Falling", false);
			this.jumpImpulse = true;
		}
		if (this.idleBreakerImpulse)
		{
			this.animator.SetTrigger("IdleBreaker");
			this.idleBreakerImpulse = false;
		}
		if (this.enemyGnome.currentState == EnemyGnome.State.Notice)
		{
			if (this.noticeImpulse)
			{
				this.animator.SetTrigger("Notice");
				this.noticeImpulse = false;
			}
		}
		else
		{
			this.noticeImpulse = true;
		}
		if (this.enemyGnome.enemy.Rigidbody.physGrabObject.rbVelocity.magnitude > 0.2f || this.enemyGnome.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude > 0.5f)
		{
			this.animator.SetBool("Moving", true);
		}
		else
		{
			this.animator.SetBool("Moving", false);
		}
		if (this.enemyGnome.currentState == EnemyGnome.State.Despawn)
		{
			this.animator.SetBool("Despawning", true);
			return;
		}
		this.animator.SetBool("Despawning", false);
	}

	// Token: 0x060002AA RID: 682 RVA: 0x0001B998 File Offset: 0x00019B98
	public void OnSpawn()
	{
		this.animator.SetBool("Despawning", false);
		this.animator.SetBool("Stunned", false);
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x060002AB RID: 683 RVA: 0x0001B9D2 File Offset: 0x00019BD2
	public void AttackDone()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemyGnome.UpdateState(EnemyGnome.State.AttackDone);
		}
	}

	// Token: 0x060002AC RID: 684 RVA: 0x0001B9E8 File Offset: 0x00019BE8
	public void Footstep()
	{
		this.soundFootstep.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Light, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0001BA52 File Offset: 0x00019C52
	public void DespawnSet()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0001BA64 File Offset: 0x00019C64
	public void MoveShort()
	{
		this.soundMoveShort.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002AF RID: 687 RVA: 0x0001BA96 File Offset: 0x00019C96
	public void MoveLong()
	{
		this.soundMoveLong.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x0001BAC8 File Offset: 0x00019CC8
	public void PickaxeTell()
	{
		this.soundPickaxeTell.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0001BAFC File Offset: 0x00019CFC
	public void PickaxeHit()
	{
		GameDirector.instance.CameraShake.ShakeDistance(2f, 2f, 5f, this.enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 2f, 5f, this.enemy.CenterTransform.position, 0.05f);
		this.soundPickaxeHit.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x0001BB9F File Offset: 0x00019D9F
	public void IdleBreaker()
	{
		this.soundIdleBreaker.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x0001BBD1 File Offset: 0x00019DD1
	public void Notice()
	{
		this.soundNotice.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x0001BC03 File Offset: 0x00019E03
	public void Jump()
	{
		this.soundJump.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x0001BC35 File Offset: 0x00019E35
	public void Land()
	{
		this.soundLand.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x0001BC67 File Offset: 0x00019E67
	public void Spawn()
	{
		this.soundSpawn.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x0001BC99 File Offset: 0x00019E99
	public void Despawn()
	{
		this.soundDespawn.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0001BCCB File Offset: 0x00019ECB
	public void StunOutro()
	{
		this.soundStunOutro.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040004B7 RID: 1207
	public Enemy enemy;

	// Token: 0x040004B8 RID: 1208
	public EnemyGnome enemyGnome;

	// Token: 0x040004B9 RID: 1209
	internal Animator animator;

	// Token: 0x040004BA RID: 1210
	internal Materials.MaterialTrigger material = new Materials.MaterialTrigger();

	// Token: 0x040004BB RID: 1211
	private bool attackImpulse;

	// Token: 0x040004BC RID: 1212
	private bool stunImpulse;

	// Token: 0x040004BD RID: 1213
	private bool jumpImpulse;

	// Token: 0x040004BE RID: 1214
	private bool landImpulse;

	// Token: 0x040004BF RID: 1215
	internal bool idleBreakerImpulse;

	// Token: 0x040004C0 RID: 1216
	private bool noticeImpulse;

	// Token: 0x040004C1 RID: 1217
	[Space]
	public Sound soundFootstep;

	// Token: 0x040004C2 RID: 1218
	[Space]
	public Sound soundMoveShort;

	// Token: 0x040004C3 RID: 1219
	public Sound soundMoveLong;

	// Token: 0x040004C4 RID: 1220
	[Space]
	public Sound soundPickaxeTell;

	// Token: 0x040004C5 RID: 1221
	public Sound soundPickaxeHit;

	// Token: 0x040004C6 RID: 1222
	[Space]
	public Sound soundIdleBreaker;

	// Token: 0x040004C7 RID: 1223
	public Sound soundNotice;

	// Token: 0x040004C8 RID: 1224
	[Space]
	public Sound soundSpawn;

	// Token: 0x040004C9 RID: 1225
	public Sound soundDespawn;

	// Token: 0x040004CA RID: 1226
	[Space]
	public Sound soundJump;

	// Token: 0x040004CB RID: 1227
	public Sound soundLand;

	// Token: 0x040004CC RID: 1228
	[Space]
	public Sound soundStun;

	// Token: 0x040004CD RID: 1229
	public Sound soundStunOutro;
}
