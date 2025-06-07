using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000044 RID: 68
public class EnemyBowtieAnim : MonoBehaviour
{
	// Token: 0x060001B0 RID: 432 RVA: 0x00011C39 File Offset: 0x0000FE39
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x00011C54 File Offset: 0x0000FE54
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
		if (this.enemy.Rigidbody.velocity.magnitude > 0.1f)
		{
			this.animator.SetBool("move", true);
		}
		else
		{
			this.animator.SetBool("move", false);
		}
		if (this.controller.currentState == EnemyBowtie.State.Leave)
		{
			this.animator.SetBool("leaving", true);
		}
		else
		{
			this.animator.SetBool("leaving", false);
		}
		if (this.controller.currentState == EnemyBowtie.State.Idle || this.controller.currentState == EnemyBowtie.State.Roam || this.controller.currentState == EnemyBowtie.State.Investigate)
		{
			if (this.soundGroanPauseTimer > 0f)
			{
				this.StopGroaning();
			}
			else
			{
				this.GroanLoopSound.PlayLoop(true, 5f, 5f, 1f);
			}
		}
		else
		{
			this.StopGroaning();
		}
		if (this.soundGroanPauseTimer > 0f)
		{
			this.soundGroanPauseTimer -= Time.deltaTime;
		}
		if (this.controller.currentState == EnemyBowtie.State.Yell)
		{
			this.animator.SetBool("yell", true);
			this.yell = true;
		}
		else
		{
			this.animator.SetBool("yell", false);
			this.yell = false;
			this.particleYell.Stop();
			this.particleYellSmall.Stop();
		}
		this.YellLoopSound.PlayLoop(this.yell, 5f, 5f, 1f);
		this.YellLoopSoundGlobal.PlayLoop(this.yell, 5f, 5f, 1f);
		if (this.controller.currentState == EnemyBowtie.State.Despawn)
		{
			this.animator.SetBool("despawn", true);
			this.particleYell.Stop();
			this.particleYellSmall.Stop();
		}
		else
		{
			this.animator.SetBool("despawn", false);
		}
		bool flag = false;
		if (this.controller.currentState == EnemyBowtie.State.Stun)
		{
			flag = true;
			this.stunSound.PlayLoop(true, 2f, 5f, 1f);
			this.landImpulse = false;
			if (this.stunImpulse)
			{
				this.StopGroaning();
				this.stunSound.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
				this.animator.SetTrigger("Stun Impulse");
				this.stunImpulse = false;
			}
			this.animator.SetBool("stunned", true);
			if (this.soundStunPauseTimer > 0f)
			{
				this.StopStunSound();
			}
			else
			{
				this.stunSound.PlayLoop(true, 2f, 5f, 1f);
			}
		}
		else
		{
			this.StopStunSound();
			this.animator.SetBool("stunned", false);
			this.stunImpulse = true;
		}
		if (this.soundStunPauseTimer > 0f)
		{
			this.soundStunPauseTimer -= Time.deltaTime;
		}
		if (!flag && this.enemy.Jump.jumping)
		{
			if (this.jumpImpulse)
			{
				this.animator.SetTrigger("Jump Impulse");
				this.animator.SetBool("falling", false);
				this.jumpImpulse = false;
				this.landImpulse = true;
				return;
			}
			if (this.controller.enemy.Rigidbody.physGrabObject.rbVelocity.y < 0f)
			{
				this.animator.SetBool("falling", true);
				return;
			}
		}
		else
		{
			if (this.landImpulse)
			{
				this.animator.SetTrigger("Land Impulse");
				this.landImpulse = false;
			}
			this.animator.SetBool("falling", false);
			this.jumpImpulse = true;
		}
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0001205E File Offset: 0x0001025E
	public void OnSpawn()
	{
		this.animator.SetBool("stunned", false);
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x00012087 File Offset: 0x00010287
	public void NoticeSet(int _playerID)
	{
		this.animator.SetTrigger("Notice");
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x00012099 File Offset: 0x00010299
	public void DespawnStart()
	{
		this.despawnSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x000120C8 File Offset: 0x000102C8
	public void Despawn()
	{
		this.particleDespawnSpark.Play();
		this.despawnSparkSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0001211C File Offset: 0x0001031C
	public void Footstep()
	{
		this.footstepSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Heavy, true, this.material, Materials.HostType.Enemy);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.3f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.1f);
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x000121E0 File Offset: 0x000103E0
	public void FootstepSmall()
	{
		this.footstepSmallSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Heavy, true, this.material, Materials.HostType.Enemy);
		GameDirector.instance.CameraShake.ShakeDistance(1.5f, 3f, 10f, base.transform.position, 0.3f);
		GameDirector.instance.CameraImpact.ShakeDistance(1.5f, 3f, 10f, base.transform.position, 0.1f);
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x000122A1 File Offset: 0x000104A1
	public void StompLeft()
	{
		this.particleStompL.Play();
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x000122AE File Offset: 0x000104AE
	public void StompRight()
	{
		this.particleStompR.Play();
	}

	// Token: 0x060001BA RID: 442 RVA: 0x000122BB File Offset: 0x000104BB
	public void MoveShort()
	{
		this.moveShortSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001BB RID: 443 RVA: 0x000122E8 File Offset: 0x000104E8
	public void MoveLong()
	{
		this.moveLongSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001BC RID: 444 RVA: 0x00012315 File Offset: 0x00010515
	public void Jump()
	{
		this.jumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001BD RID: 445 RVA: 0x00012342 File Offset: 0x00010542
	public void Land()
	{
		this.landSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001BE RID: 446 RVA: 0x00012370 File Offset: 0x00010570
	public void Notice()
	{
		this.noticeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060001BF RID: 447 RVA: 0x00012404 File Offset: 0x00010604
	public void YellStart()
	{
		this.yellStartSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.yellStartSoundGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.particleYell.Play();
		this.particleYellSmall.Play();
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x00012480 File Offset: 0x00010680
	public void yellShake()
	{
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 12f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 12f, base.transform.position, 0.1f);
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x000124E9 File Offset: 0x000106E9
	public void EnemyInvestigate()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			EnemyDirector.instance.SetInvestigate(base.transform.position, 15f, false);
		}
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x00012514 File Offset: 0x00010714
	public void YellStop()
	{
		this.yellEndSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.yellEndSoundGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x00012577 File Offset: 0x00010777
	public void StopStunSound()
	{
		this.stunSound.PlayLoop(false, 2f, 5f, 1f);
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x00012594 File Offset: 0x00010794
	public void StopGroaning()
	{
		this.GroanLoopSound.PlayLoop(false, 5f, 5f, 1f);
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x000125B1 File Offset: 0x000107B1
	public void StunPause()
	{
		this.soundStunPauseTimer = 0.5f;
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x000125BE File Offset: 0x000107BE
	public void GroanPause()
	{
		this.soundGroanPauseTimer = 1f;
	}

	// Token: 0x0400038D RID: 909
	public Transform followTarget;

	// Token: 0x0400038E RID: 910
	public EnemyBowtie controller;

	// Token: 0x0400038F RID: 911
	public Enemy enemy;

	// Token: 0x04000390 RID: 912
	public Materials.MaterialTrigger material;

	// Token: 0x04000391 RID: 913
	internal Animator animator;

	// Token: 0x04000392 RID: 914
	private bool attackImpulse;

	// Token: 0x04000393 RID: 915
	private bool stunImpulse;

	// Token: 0x04000394 RID: 916
	private bool jumpImpulse;

	// Token: 0x04000395 RID: 917
	private bool landImpulse;

	// Token: 0x04000396 RID: 918
	private bool noticeImpulse;

	// Token: 0x04000397 RID: 919
	[Space]
	public ParticleSystem particleBits;

	// Token: 0x04000398 RID: 920
	public ParticleSystem particleImpact;

	// Token: 0x04000399 RID: 921
	public ParticleSystem particleDirectionalBits;

	// Token: 0x0400039A RID: 922
	public ParticleSystem particleEyes;

	// Token: 0x0400039B RID: 923
	public ParticleSystem particleDespawnSpark;

	// Token: 0x0400039C RID: 924
	public ParticleSystem particleYell;

	// Token: 0x0400039D RID: 925
	public ParticleSystem particleYellSmall;

	// Token: 0x0400039E RID: 926
	public ParticleSystem particleStompL;

	// Token: 0x0400039F RID: 927
	public ParticleSystem particleStompR;

	// Token: 0x040003A0 RID: 928
	private float soundStunPauseTimer;

	// Token: 0x040003A1 RID: 929
	private float soundGroanPauseTimer;

	// Token: 0x040003A2 RID: 930
	[Space]
	public Sound footstepSound;

	// Token: 0x040003A3 RID: 931
	public Sound footstepSmallSound;

	// Token: 0x040003A4 RID: 932
	[Space]
	public Sound moveShortSound;

	// Token: 0x040003A5 RID: 933
	public Sound moveLongSound;

	// Token: 0x040003A6 RID: 934
	public Sound GroanLoopSound;

	// Token: 0x040003A7 RID: 935
	[Space]
	public Sound despawnSound;

	// Token: 0x040003A8 RID: 936
	public Sound despawnSparkSound;

	// Token: 0x040003A9 RID: 937
	[Space]
	public Sound jumpSound;

	// Token: 0x040003AA RID: 938
	public Sound landSound;

	// Token: 0x040003AB RID: 939
	public Sound noticeSound;

	// Token: 0x040003AC RID: 940
	[Space]
	public Sound yellStartSound;

	// Token: 0x040003AD RID: 941
	public Sound yellStartSoundGlobal;

	// Token: 0x040003AE RID: 942
	public Sound yellEndSound;

	// Token: 0x040003AF RID: 943
	public Sound yellEndSoundGlobal;

	// Token: 0x040003B0 RID: 944
	public Sound YellLoopSound;

	// Token: 0x040003B1 RID: 945
	public Sound YellLoopSoundGlobal;

	// Token: 0x040003B2 RID: 946
	private bool yell;

	// Token: 0x040003B3 RID: 947
	[Space]
	public Sound stunSound;

	// Token: 0x040003B4 RID: 948
	[Space]
	public Sound hurtSound;

	// Token: 0x040003B5 RID: 949
	public Sound deathSound;
}
