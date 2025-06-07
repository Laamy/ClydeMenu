using System;
using UnityEngine;

// Token: 0x02000046 RID: 70
public class EnemyCeilingEyeAnim : MonoBehaviour
{
	// Token: 0x060001DA RID: 474 RVA: 0x000131FE File Offset: 0x000113FE
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x060001DB RID: 475 RVA: 0x00013218 File Offset: 0x00011418
	private void Update()
	{
		if (this.controller.currentState == EnemyCeilingEye.State.HasTarget || this.controller.currentState == EnemyCeilingEye.State.TargetLost)
		{
			this.animator.SetBool("hasTarget", true);
		}
		else
		{
			this.animator.SetBool("hasTarget", false);
		}
		if (this.controller.enemy.CurrentState == EnemyState.Despawn || this.controller.currentState == EnemyCeilingEye.State.Move)
		{
			this.animator.SetBool("despawn", true);
		}
		else
		{
			this.animator.SetBool("despawn", false);
		}
		this.SfxStaringLoop();
		if (this.controller.deathImpulse)
		{
			this.controller.deathImpulse = false;
			this.animator.SetTrigger("Death");
		}
	}

	// Token: 0x060001DC RID: 476 RVA: 0x000132DA File Offset: 0x000114DA
	public void SetSpawn()
	{
		this.animator.Play("Ceiling Eye Spawn", 0, 0f);
	}

	// Token: 0x060001DD RID: 477 RVA: 0x000132F2 File Offset: 0x000114F2
	public void SetAttack()
	{
		this.animator.Play("Ceiling Eye Attack", 0, 0f);
	}

	// Token: 0x060001DE RID: 478 RVA: 0x0001330A File Offset: 0x0001150A
	public void SetDespawn()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.controller.enemy.CurrentState == EnemyState.Despawn)
		{
			this.controller.enemy.EnemyParent.Despawn();
			return;
		}
		this.controller.OnSpawn();
	}

	// Token: 0x060001DF RID: 479 RVA: 0x00013349 File Offset: 0x00011549
	public void AttackFinished()
	{
		this.controller.enemy.EnemyParent.SpawnedTimerSet(0f);
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x00013368 File Offset: 0x00011568
	public void Explosion()
	{
		Vector3 b = new Vector3(0f, -0.5f, 0f);
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position + b, Vector3.down, out raycastHit, 30f, SemiFunc.LayerMaskGetVisionObstruct()))
		{
			this.particleScriptExplosion.Spawn(raycastHit.point, 2f, 50, 50, 1f, false, false, 1f);
		}
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x000133E4 File Offset: 0x000115E4
	public void DeathEffect()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.particleImpact.Play();
		this.particleBits.Play();
		this.sfxDeath.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0001348E File Offset: 0x0001168E
	public void TeleportParticlesStart()
	{
		this.TeleportParticles.Play();
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0001349B File Offset: 0x0001169B
	public void TeleportParticlesStop()
	{
		this.TeleportParticles.Stop();
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x000134A8 File Offset: 0x000116A8
	public void SfxBlink()
	{
		this.sfxBlink.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x000134D5 File Offset: 0x000116D5
	public void SfxDespawn()
	{
		this.sfxDespawn.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x00013502 File Offset: 0x00011702
	public void SfxSpawn()
	{
		this.sfxSpawn.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x0001352F File Offset: 0x0001172F
	public void SfxLaserBuildup()
	{
		this.sfxLaserBuildup.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0001355C File Offset: 0x0001175C
	public void SfxLaserBeam()
	{
		this.sfxLaserBeam.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x000135F0 File Offset: 0x000117F0
	public void SfxStaringStart()
	{
		if (this.controller.currentState == EnemyCeilingEye.State.HasTarget && this.controller.targetPlayer.isLocal)
		{
			AudioScare.instance.PlayCustom(this.sfxStaringStart, 0.3f, 20f);
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060001EA RID: 490 RVA: 0x00013694 File Offset: 0x00011894
	public void SfxStaringLoop()
	{
		this.sfxStareLoop.PlayLoop(this.isPlayingStaringLoop, 0.05f, 0.25f, 1f);
		this.sfxTwitchLoop.PlayLoop(this.isPlayingTwitchLoop, 0.1f, 0.25f, 1f);
		if (this.controller.currentState == EnemyCeilingEye.State.HasTarget)
		{
			this.isPlayingStaringLoop = true;
		}
		else
		{
			this.isPlayingStaringLoop = false;
		}
		if (this.controller.currentState == EnemyCeilingEye.State.HasTarget && this.controller.targetPlayer.isLocal)
		{
			this.isPlayingTwitchLoop = true;
			return;
		}
		this.isPlayingTwitchLoop = false;
	}

	// Token: 0x040003C6 RID: 966
	public EnemyCeilingEye controller;

	// Token: 0x040003C7 RID: 967
	private Animator animator;

	// Token: 0x040003C8 RID: 968
	public Enemy enemy;

	// Token: 0x040003C9 RID: 969
	public ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x040003CA RID: 970
	public ParticleSystem TeleportParticles;

	// Token: 0x040003CB RID: 971
	public ParticleSystem particleImpact;

	// Token: 0x040003CC RID: 972
	public ParticleSystem particleBits;

	// Token: 0x040003CD RID: 973
	[Header("Sounds")]
	public Sound sfxBlink;

	// Token: 0x040003CE RID: 974
	public Sound sfxDespawn;

	// Token: 0x040003CF RID: 975
	public Sound sfxSpawn;

	// Token: 0x040003D0 RID: 976
	public Sound sfxDeath;

	// Token: 0x040003D1 RID: 977
	public Sound sfxLaserBuildup;

	// Token: 0x040003D2 RID: 978
	public Sound sfxLaserBeam;

	// Token: 0x040003D3 RID: 979
	public AudioClip sfxStaringStart;

	// Token: 0x040003D4 RID: 980
	public Sound sfxStareLoop;

	// Token: 0x040003D5 RID: 981
	public Sound sfxTwitchLoop;

	// Token: 0x040003D6 RID: 982
	private bool isPlayingTwitchLoop = true;

	// Token: 0x040003D7 RID: 983
	private bool isPlayingStaringLoop = true;
}
