using System;
using UnityEngine;

// Token: 0x02000085 RID: 133
public class EnemyTumblerAnim : MonoBehaviour
{
	// Token: 0x06000560 RID: 1376 RVA: 0x0003547C File Offset: 0x0003367C
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x00035498 File Offset: 0x00033698
	private void Update()
	{
		if (this.enemy.Jump.jumping)
		{
			this.animator.SetBool("jumping", true);
			if (this.jumpImpulse)
			{
				this.jumpedTimer = 0f;
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
		this.jumpedTimer += Time.deltaTime;
		if (this.jumpedTimer > 0.5f)
		{
			if (this.enemy.Rigidbody.physGrabObject.rbVelocity.y < -0.1f)
			{
				this.animator.SetBool("falling", true);
			}
			else
			{
				this.animator.SetBool("falling", false);
			}
		}
		if (this.enemyTumbler.currentState == EnemyTumbler.State.Tell)
		{
			this.animator.SetBool("tell", true);
		}
		else
		{
			this.animator.SetBool("tell", false);
		}
		if (this.enemyTumbler.currentState == EnemyTumbler.State.Tumble)
		{
			this.animator.SetBool("tumble", true);
			this.tumble = true;
		}
		else
		{
			this.animator.SetBool("tumble", false);
			this.tumble = false;
		}
		this.sfxTumbleLoopLocal.PlayLoop(this.tumble, 5f, 5f, 1f);
		this.sfxTumbleLoopGlobal.PlayLoop(this.tumble, 5f, 5f, 1f);
		if (this.enemyTumbler.currentState == EnemyTumbler.State.Stunned)
		{
			if (this.stunImpulse)
			{
				this.animator.SetTrigger("Stun");
				this.stunImpulse = false;
			}
			this.animator.SetBool("stunned", true);
			this.stunned = true;
		}
		else
		{
			this.animator.SetBool("stunned", false);
			this.stunImpulse = true;
			this.stunned = false;
		}
		this.sfxStunnedLoop.PlayLoop(this.stunned, 5f, 5f, 1f);
		if (this.enemyTumbler.currentState == EnemyTumbler.State.Despawn)
		{
			this.animator.SetBool("despawning", true);
			return;
		}
		this.animator.SetBool("despawning", false);
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x000356EC File Offset: 0x000338EC
	public void OnSpawn()
	{
		this.animator.SetBool("stunned", false);
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x00035718 File Offset: 0x00033918
	private void OnDrawGizmos()
	{
		if (this.showGizmos)
		{
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, Quaternion.identity, new Vector3(1f, 0f, 1f));
			Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
			Gizmos.DrawWireSphere(Vector3.zero, this.gizmoMinDistance);
			Gizmos.color = new Color(0.9f, 0f, 0.1f, 0.5f);
			Gizmos.DrawWireSphere(Vector3.zero, this.gizmoMaxDistance);
		}
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x000357BA File Offset: 0x000339BA
	public void SfxOnHurtColliderImpactAny()
	{
		this.sfxHurtColliderImpactAny.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x000357E8 File Offset: 0x000339E8
	public void OnTumble()
	{
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x00035851 File Offset: 0x00033A51
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x00035864 File Offset: 0x00033A64
	public void SfxJump()
	{
		this.sfxJump.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.ShakeDistance(1f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(1f, 3f, 8f, base.transform.position, 0.5f);
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x000358F8 File Offset: 0x00033AF8
	public void SfxLand()
	{
		this.sfxLand.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.5f);
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x0003598C File Offset: 0x00033B8C
	public void SfxNotice()
	{
		this.sfxNotice.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x000359B9 File Offset: 0x00033BB9
	public void SfxCleaverSwing()
	{
		this.sfxCleaverSwing.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x000359E6 File Offset: 0x00033BE6
	public void SfxCharge()
	{
		this.sfxCharge.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x00035A13 File Offset: 0x00033C13
	public void SfxHurt()
	{
		this.sfxHurt.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x00035A40 File Offset: 0x00033C40
	public void SfxMoveShort()
	{
		this.sfxMoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x00035A6D File Offset: 0x00033C6D
	public void SfxMoveLong()
	{
		this.sfxMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0400089C RID: 2204
	public Enemy enemy;

	// Token: 0x0400089D RID: 2205
	public EnemyTumbler enemyTumbler;

	// Token: 0x0400089E RID: 2206
	internal Animator animator;

	// Token: 0x0400089F RID: 2207
	public Materials.MaterialTrigger material;

	// Token: 0x040008A0 RID: 2208
	private bool tumble;

	// Token: 0x040008A1 RID: 2209
	private bool stunned;

	// Token: 0x040008A2 RID: 2210
	private bool stunImpulse;

	// Token: 0x040008A3 RID: 2211
	internal bool spawnImpulse;

	// Token: 0x040008A4 RID: 2212
	private bool jumpImpulse;

	// Token: 0x040008A5 RID: 2213
	private float jumpedTimer;

	// Token: 0x040008A6 RID: 2214
	[Header("One Shots")]
	public Sound sfxJump;

	// Token: 0x040008A7 RID: 2215
	public Sound sfxLand;

	// Token: 0x040008A8 RID: 2216
	public Sound sfxNotice;

	// Token: 0x040008A9 RID: 2217
	public Sound sfxCleaverSwing;

	// Token: 0x040008AA RID: 2218
	public Sound sfxCharge;

	// Token: 0x040008AB RID: 2219
	public Sound sfxHurt;

	// Token: 0x040008AC RID: 2220
	public Sound sfxMoveShort;

	// Token: 0x040008AD RID: 2221
	public Sound sfxMoveLong;

	// Token: 0x040008AE RID: 2222
	public Sound sfxDeath;

	// Token: 0x040008AF RID: 2223
	public Sound sfxHurtColliderImpactAny;

	// Token: 0x040008B0 RID: 2224
	[Header("Loops")]
	public Sound sfxStunnedLoop;

	// Token: 0x040008B1 RID: 2225
	public Sound sfxTumbleLoopLocal;

	// Token: 0x040008B2 RID: 2226
	public Sound sfxTumbleLoopGlobal;

	// Token: 0x040008B3 RID: 2227
	public bool showGizmos = true;

	// Token: 0x040008B4 RID: 2228
	public float gizmoMinDistance = 3f;

	// Token: 0x040008B5 RID: 2229
	public float gizmoMaxDistance = 8f;
}
