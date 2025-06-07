using System;
using UnityEngine;

// Token: 0x02000080 RID: 128
public class EnemySlowWalkerAnim : MonoBehaviour
{
	// Token: 0x060004F0 RID: 1264 RVA: 0x000312D8 File Offset: 0x0002F4D8
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
		this.springNeck01Speed = this.springNeck01.speed;
		this.springNeck01Damping = this.springNeck01.damping;
		this.springNeck02Speed = this.springNeck02.speed;
		this.springNeck02Damping = this.springNeck02.damping;
		this.springNeck03Speed = this.springNeck03.speed;
		this.springNeck03Damping = this.springNeck03.damping;
		this.springEyeFleshSpeed = this.springEyeFlesh.speed;
		this.springEyeFleshDamping = this.springEyeFlesh.damping;
		this.springEyeBallSpeed = this.springEyeBall.speed;
		this.springEyeBallDamping = this.springEyeBall.damping;
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x000313A8 File Offset: 0x0002F5A8
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
		if (!this.stunned && (this.enemy.Jump.jumping || this.enemy.Jump.jumpingDelay))
		{
			if (this.jumpImpulse)
			{
				this.animator.SetTrigger(this.animJump);
				this.animator.SetBool(this.animFalling, false);
				this.jumpImpulse = false;
				this.landImpulse = true;
			}
			else if (this.controller.enemy.Rigidbody.physGrabObject.rbVelocity.y < -1f)
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
		if (this.controller.currentState == EnemySlowWalker.State.LookUnder || this.controller.currentState == EnemySlowWalker.State.LookUnderIntro || this.controller.currentState == EnemySlowWalker.State.LookUnderAttack)
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
		if (this.controller.currentState == EnemySlowWalker.State.LookUnderAttack)
		{
			if (this.lookUnderAttackImpulse)
			{
				this.animator.SetTrigger(this.animLookUnderAttack);
				this.lookUnderAttackImpulse = false;
			}
		}
		else
		{
			this.lookUnderAttackImpulse = true;
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
		if (this.controller.currentState == EnemySlowWalker.State.Stun)
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
		this.SfxStunnedLoop();
		if (this.controller.currentState == EnemySlowWalker.State.Notice)
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
		if (this.controller.currentState == EnemySlowWalker.State.Attack)
		{
			if (this.attackImpulse)
			{
				this.animator.SetTrigger(this.animAttack);
				this.attackImpulse = false;
			}
		}
		else
		{
			this.attackImpulse = true;
		}
		if (this.controller.currentState == EnemySlowWalker.State.StuckAttack)
		{
			if (this.stuckAttackImpulse)
			{
				this.animator.SetTrigger(this.animStuckAttack);
				this.stuckAttackImpulse = false;
			}
		}
		else
		{
			this.stuckAttackImpulse = true;
		}
		if (this.controller.currentState == EnemySlowWalker.State.Despawn)
		{
			this.animator.SetBool(this.animDespawning, true);
			return;
		}
		this.animator.SetBool(this.animDespawning, false);
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x00031754 File Offset: 0x0002F954
	public void AttackOffsetStart()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.controller.attackOffsetActive = true;
		}
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x00031769 File Offset: 0x0002F969
	public void AttackOffsetStop()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.controller.attackOffsetActive = false;
		}
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0003177E File Offset: 0x0002F97E
	public void AttackStart()
	{
		this.slowWalkerAttack.SlowWalkerAttackStart();
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0003178B File Offset: 0x0002F98B
	private void VfxJump()
	{
		this.slowWalkerJumpEffect.JumpEffect();
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00031798 File Offset: 0x0002F998
	private void VfxLand()
	{
		this.slowWalkerJumpEffect.LandEffect();
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x000317A5 File Offset: 0x0002F9A5
	public void VfxSparkStart()
	{
		this.slowWalkerSparkEffect.PlaySparkEffect();
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x000317B2 File Offset: 0x0002F9B2
	public void VfxSparkStop()
	{
		this.slowWalkerSparkEffect.StopSparkEffect();
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x000317C0 File Offset: 0x0002F9C0
	public void SfxFootstepSmall()
	{
		this.sfxFootstepSmall.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(1f, 5f, 10f, base.transform.position, 0.25f);
		GameDirector.instance.CameraImpact.ShakeDistance(1f, 5f, 10f, base.transform.position, 0.1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Light, false, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x00031898 File Offset: 0x0002FA98
	public void SfxFootstepBig()
	{
		this.sfxFootstepBig.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(2f, 5f, 10f, base.transform.position, 0.25f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 5f, 10f, base.transform.position, 0.1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Heavy, false, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x00031970 File Offset: 0x0002FB70
	public void SfxJump()
	{
		this.sfxJump.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Heavy, false, this.material, Materials.HostType.Enemy);
		this.VfxJump();
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x00031A4C File Offset: 0x0002FC4C
	public void SfxLand()
	{
		this.sfxLand.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Heavy, false, this.material, Materials.HostType.Enemy);
		this.VfxLand();
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x00031B27 File Offset: 0x0002FD27
	public void SfxMoveShort()
	{
		this.sfxMoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.SfxNoiseShort();
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x00031B5A File Offset: 0x0002FD5A
	public void SfxMoveLong()
	{
		this.sfxMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.SfxNoiseLong();
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x00031B8D File Offset: 0x0002FD8D
	public void SfxAttackBuildupVoice()
	{
		this.sfxAttackBuildupVoice.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x00031BBA File Offset: 0x0002FDBA
	public void SfxAttackImpact()
	{
		this.sfxAttackImpact.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x00031BE7 File Offset: 0x0002FDE7
	public void SfxAttackImplosionBuildup()
	{
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x00031BE9 File Offset: 0x0002FDE9
	public void SfxAttackImplosionHit()
	{
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x00031BEB File Offset: 0x0002FDEB
	public void SfxAttackImplosionImpact()
	{
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x00031BED File Offset: 0x0002FDED
	public void SfxDeath()
	{
		this.sfxDeath.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x00031C1A File Offset: 0x0002FE1A
	public void SfxHurt()
	{
		this.sfxHurt.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x00031C47 File Offset: 0x0002FE47
	public void SfxNoiseShort()
	{
		if (Random.value <= 0.6f)
		{
			this.sfxNoiseShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x00031C80 File Offset: 0x0002FE80
	public void SfxNoiseLong()
	{
		if (Random.value <= 0.6f)
		{
			this.sfxNoiseLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x00031CB9 File Offset: 0x0002FEB9
	public void SfxNoticeVoice()
	{
		this.sfxNoticeVoice.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x00031CE6 File Offset: 0x0002FEE6
	public void SfxSwingShort()
	{
		this.sfxSwingShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x00031D13 File Offset: 0x0002FF13
	public void SfxSwingLong()
	{
		this.sfxSwingLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x00031D40 File Offset: 0x0002FF40
	public void SfxMaceTrailing()
	{
		this.sfxMaceTrailing.Play(this.maceTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x00031D6D File Offset: 0x0002FF6D
	public void SfxLookUnderIntro()
	{
		this.sfxLookUnderIntro.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00031D9A File Offset: 0x0002FF9A
	public void SfxLookUnderAttack()
	{
		this.sfxLookUnderAttack.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x00031DC7 File Offset: 0x0002FFC7
	public void SfxLookUnderOutro()
	{
		this.sfxLookUnderOutro.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x00031DF4 File Offset: 0x0002FFF4
	public void SfxStunnedLoop()
	{
		this.sfxStunnedLoop.PlayLoop(this.stunned, 5f, 5f, 1f);
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x00031E16 File Offset: 0x00030016
	public void OnSpawn()
	{
		this.animator.SetBool(this.animStunned, false);
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x00031E40 File Offset: 0x00030040
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x00031E52 File Offset: 0x00030052
	public void NoticeSet(int _playerID)
	{
		this.animator.SetTrigger(this.animNotice);
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x00031E68 File Offset: 0x00030068
	private bool IsMoving()
	{
		return this.controller.currentState == EnemySlowWalker.State.Roam || this.controller.currentState == EnemySlowWalker.State.Investigate || this.controller.currentState == EnemySlowWalker.State.GoToPlayer || this.controller.currentState == EnemySlowWalker.State.LookUnderStart || this.controller.currentState == EnemySlowWalker.State.Sneak || this.controller.currentState == EnemySlowWalker.State.Leave;
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x00031ED0 File Offset: 0x000300D0
	public void SpringLogic()
	{
		this.springNeck01.speed = this.springNeck01Speed * this.springSpeedMultiplier;
		this.springNeck01.damping = this.springNeck01Damping * this.springDampingMultiplier;
		this.springNeck01Source.rotation = SemiFunc.SpringQuaternionGet(this.springNeck01, this.springNeck01Target.transform.rotation, -1f);
		this.springNeck02.speed = this.springNeck02Speed * this.springSpeedMultiplier;
		this.springNeck02.damping = this.springNeck02Damping * this.springDampingMultiplier;
		this.springNeck02Source.rotation = SemiFunc.SpringQuaternionGet(this.springNeck02, this.springNeck02Target.transform.rotation, -1f);
		this.springNeck03.speed = this.springNeck03Speed * this.springSpeedMultiplier;
		this.springNeck03.damping = this.springNeck03Damping * this.springDampingMultiplier;
		this.springNeck03Source.rotation = SemiFunc.SpringQuaternionGet(this.springNeck03, this.springNeck03Target.transform.rotation, -1f);
		this.springEyeFlesh.speed = this.springEyeFleshSpeed * this.springSpeedMultiplier;
		this.springEyeFlesh.damping = this.springEyeFleshDamping * this.springDampingMultiplier;
		this.springEyeFleshSource.rotation = SemiFunc.SpringQuaternionGet(this.springEyeFlesh, this.springEyeFleshTarget.transform.rotation, -1f);
		this.springEyeBall.speed = this.springEyeBallSpeed * this.springSpeedMultiplier;
		this.springEyeBall.damping = this.springEyeBallDamping * this.springDampingMultiplier;
		this.springEyeBallSource.rotation = SemiFunc.SpringQuaternionGet(this.springEyeBall, this.springEyeBallTarget.transform.rotation, -1f);
	}

	// Token: 0x040007DD RID: 2013
	public Enemy enemy;

	// Token: 0x040007DE RID: 2014
	public EnemySlowWalker controller;

	// Token: 0x040007DF RID: 2015
	public Transform maceTransform;

	// Token: 0x040007E0 RID: 2016
	internal Animator animator;

	// Token: 0x040007E1 RID: 2017
	internal Materials.MaterialTrigger material = new Materials.MaterialTrigger();

	// Token: 0x040007E2 RID: 2018
	public SlowWalkerSparkEffect slowWalkerSparkEffect;

	// Token: 0x040007E3 RID: 2019
	private int animMoving = Animator.StringToHash("moving");

	// Token: 0x040007E4 RID: 2020
	private int animStunned = Animator.StringToHash("stunned");

	// Token: 0x040007E5 RID: 2021
	private int animDespawning = Animator.StringToHash("despawning");

	// Token: 0x040007E6 RID: 2022
	private int animFalling = Animator.StringToHash("falling");

	// Token: 0x040007E7 RID: 2023
	private int animLookingUnder = Animator.StringToHash("lookingUnder");

	// Token: 0x040007E8 RID: 2024
	private int animStun = Animator.StringToHash("Stun");

	// Token: 0x040007E9 RID: 2025
	private int animNotice = Animator.StringToHash("Notice");

	// Token: 0x040007EA RID: 2026
	private int animAttack = Animator.StringToHash("Attack");

	// Token: 0x040007EB RID: 2027
	private int animJump = Animator.StringToHash("Jump");

	// Token: 0x040007EC RID: 2028
	private int animLand = Animator.StringToHash("Land");

	// Token: 0x040007ED RID: 2029
	private int animLookUnder = Animator.StringToHash("LookUnder");

	// Token: 0x040007EE RID: 2030
	private int animLookUnderAttack = Animator.StringToHash("LookUnderAttack");

	// Token: 0x040007EF RID: 2031
	private int animStuckAttack = Animator.StringToHash("StuckAttack");

	// Token: 0x040007F0 RID: 2032
	public float springSpeedMultiplier = 1f;

	// Token: 0x040007F1 RID: 2033
	public float springDampingMultiplier = 1f;

	// Token: 0x040007F2 RID: 2034
	public SpringQuaternion springNeck01;

	// Token: 0x040007F3 RID: 2035
	private float springNeck01Speed;

	// Token: 0x040007F4 RID: 2036
	private float springNeck01Damping;

	// Token: 0x040007F5 RID: 2037
	public Transform springNeck01Target;

	// Token: 0x040007F6 RID: 2038
	public Transform springNeck01Source;

	// Token: 0x040007F7 RID: 2039
	public SpringQuaternion springNeck02;

	// Token: 0x040007F8 RID: 2040
	private float springNeck02Speed;

	// Token: 0x040007F9 RID: 2041
	private float springNeck02Damping;

	// Token: 0x040007FA RID: 2042
	public Transform springNeck02Target;

	// Token: 0x040007FB RID: 2043
	public Transform springNeck02Source;

	// Token: 0x040007FC RID: 2044
	public SpringQuaternion springNeck03;

	// Token: 0x040007FD RID: 2045
	private float springNeck03Speed;

	// Token: 0x040007FE RID: 2046
	private float springNeck03Damping;

	// Token: 0x040007FF RID: 2047
	public Transform springNeck03Target;

	// Token: 0x04000800 RID: 2048
	public Transform springNeck03Source;

	// Token: 0x04000801 RID: 2049
	public SpringQuaternion springEyeFlesh;

	// Token: 0x04000802 RID: 2050
	private float springEyeFleshSpeed;

	// Token: 0x04000803 RID: 2051
	private float springEyeFleshDamping;

	// Token: 0x04000804 RID: 2052
	public Transform springEyeFleshTarget;

	// Token: 0x04000805 RID: 2053
	public Transform springEyeFleshSource;

	// Token: 0x04000806 RID: 2054
	public SpringQuaternion springEyeBall;

	// Token: 0x04000807 RID: 2055
	private float springEyeBallSpeed;

	// Token: 0x04000808 RID: 2056
	private float springEyeBallDamping;

	// Token: 0x04000809 RID: 2057
	public Transform springEyeBallTarget;

	// Token: 0x0400080A RID: 2058
	public Transform springEyeBallSource;

	// Token: 0x0400080B RID: 2059
	private bool stunned;

	// Token: 0x0400080C RID: 2060
	private bool stunImpulse;

	// Token: 0x0400080D RID: 2061
	private bool noticeImpulse;

	// Token: 0x0400080E RID: 2062
	private bool delayAttackImpulse;

	// Token: 0x0400080F RID: 2063
	private bool attackImpulse;

	// Token: 0x04000810 RID: 2064
	private bool chargeAttackImpulse;

	// Token: 0x04000811 RID: 2065
	private bool jumpImpulse;

	// Token: 0x04000812 RID: 2066
	private bool landImpulse;

	// Token: 0x04000813 RID: 2067
	private bool lookUnderImpulse;

	// Token: 0x04000814 RID: 2068
	private bool lookUnderAttackImpulse;

	// Token: 0x04000815 RID: 2069
	private bool stuckAttackImpulse;

	// Token: 0x04000816 RID: 2070
	private float moveTimer;

	// Token: 0x04000817 RID: 2071
	private float jumpedTimer;

	// Token: 0x04000818 RID: 2072
	public Sound sfxFootstepSmall;

	// Token: 0x04000819 RID: 2073
	public Sound sfxFootstepBig;

	// Token: 0x0400081A RID: 2074
	public Sound sfxJump;

	// Token: 0x0400081B RID: 2075
	public Sound sfxLand;

	// Token: 0x0400081C RID: 2076
	public Sound sfxMoveShort;

	// Token: 0x0400081D RID: 2077
	public Sound sfxMoveLong;

	// Token: 0x0400081E RID: 2078
	public Sound sfxAttackBuildupVoice;

	// Token: 0x0400081F RID: 2079
	public Sound sfxAttackImpact;

	// Token: 0x04000820 RID: 2080
	public Sound sfxAttackImplosionBuildup;

	// Token: 0x04000821 RID: 2081
	public Sound sfxAttackImplosionHitLocal;

	// Token: 0x04000822 RID: 2082
	public Sound sfxAttackImplosionHitGlobal;

	// Token: 0x04000823 RID: 2083
	public Sound sfxAttackImplosionImpactLocal;

	// Token: 0x04000824 RID: 2084
	public Sound sfxAttackImplosionImpactGlobal;

	// Token: 0x04000825 RID: 2085
	public Sound sfxDeath;

	// Token: 0x04000826 RID: 2086
	public Sound sfxHurt;

	// Token: 0x04000827 RID: 2087
	public Sound sfxNoiseShort;

	// Token: 0x04000828 RID: 2088
	public Sound sfxNoiseLong;

	// Token: 0x04000829 RID: 2089
	public Sound sfxNoticeVoice;

	// Token: 0x0400082A RID: 2090
	public Sound sfxSwingShort;

	// Token: 0x0400082B RID: 2091
	public Sound sfxSwingLong;

	// Token: 0x0400082C RID: 2092
	public Sound sfxMaceTrailing;

	// Token: 0x0400082D RID: 2093
	public Sound sfxLookUnderIntro;

	// Token: 0x0400082E RID: 2094
	public Sound sfxLookUnderAttack;

	// Token: 0x0400082F RID: 2095
	public Sound sfxLookUnderOutro;

	// Token: 0x04000830 RID: 2096
	public Sound sfxStunnedLoop;

	// Token: 0x04000831 RID: 2097
	public SlowWalkerAttack slowWalkerAttack;

	// Token: 0x04000832 RID: 2098
	public SlowWalkerJumpEffect slowWalkerJumpEffect;
}
