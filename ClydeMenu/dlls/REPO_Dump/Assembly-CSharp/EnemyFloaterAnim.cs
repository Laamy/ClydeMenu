using System;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class EnemyFloaterAnim : MonoBehaviour
{
	// Token: 0x0600024C RID: 588 RVA: 0x00017CA0 File Offset: 0x00015EA0
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
		this.springHeadSpeed = this.springHead.speed;
		this.springHeadDamping = this.springHead.damping;
		this.springLegLSpeed = this.springLegL.speed;
		this.springLegLDamping = this.springLegL.damping;
		this.springLegRSpeed = this.springLegR.speed;
		this.springLegRDamping = this.springLegR.damping;
		this.springArmLSpeed = this.springArmL.speed;
		this.springArmLDamping = this.springArmL.damping;
		this.springArmRSpeed = this.springArmR.speed;
		this.springArmRDamping = this.springArmR.damping;
	}

	// Token: 0x0600024D RID: 589 RVA: 0x00017D70 File Offset: 0x00015F70
	private void Update()
	{
		this.SpringLogic();
		if (this.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
		else
		{
			this.animator.speed = 1f;
		}
		if (this.controller.currentState == EnemyFloater.State.Stun)
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
		if (this.controller.currentState == EnemyFloater.State.Notice)
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
		if (this.controller.currentState == EnemyFloater.State.ChargeAttack)
		{
			if (this.chargeAttackImpulse)
			{
				this.animator.SetTrigger(this.animChargeAttack);
				this.chargeAttackImpulse = false;
				this.sfxChargeAttackStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		else
		{
			this.chargeAttackImpulse = true;
		}
		this.SfxChargeAttackLoop();
		if (this.controller.currentState == EnemyFloater.State.DelayAttack)
		{
			if (this.delayAttackImpulse)
			{
				this.animator.SetTrigger(this.animDelayAttack);
				this.delayAttackImpulse = false;
			}
		}
		else
		{
			this.delayAttackImpulse = true;
		}
		this.SfxDelayAttackLoop();
		if (this.controller.currentState == EnemyFloater.State.Attack)
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
		if (this.controller.currentState == EnemyFloater.State.Despawn)
		{
			this.animator.SetBool(this.animDespawning, true);
			return;
		}
		this.animator.SetBool(this.animDespawning, false);
	}

	// Token: 0x0600024E RID: 590 RVA: 0x00017F6F File Offset: 0x0001616F
	public void OnSpawn()
	{
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x0600024F RID: 591 RVA: 0x00017F87 File Offset: 0x00016187
	public void NoticeSet(int _playerID)
	{
		this.animator.SetTrigger(this.animNotice);
	}

	// Token: 0x06000250 RID: 592 RVA: 0x00017F9C File Offset: 0x0001619C
	private void SpringLogic()
	{
		this.springHead.speed = this.springHeadSpeed * this.springSpeedMultiplier;
		this.springHead.damping = this.springHeadDamping * this.springDampingMultiplier;
		this.springHeadSource.rotation = SemiFunc.SpringQuaternionGet(this.springHead, this.springHeadTarget.transform.rotation, -1f);
		this.springLegL.speed = this.springLegLSpeed * this.springSpeedMultiplier;
		this.springLegL.damping = this.springLegLDamping * this.springDampingMultiplier;
		this.springLegLSource.rotation = SemiFunc.SpringQuaternionGet(this.springLegL, this.springLegLTarget.transform.rotation, -1f);
		this.springLegR.speed = this.springLegRSpeed * this.springSpeedMultiplier;
		this.springLegR.damping = this.springLegRDamping * this.springDampingMultiplier;
		this.springLegRSource.rotation = SemiFunc.SpringQuaternionGet(this.springLegR, this.springLegRTarget.transform.rotation, -1f);
		this.springArmL.speed = this.springArmLSpeed * this.springSpeedMultiplier;
		this.springArmL.damping = this.springArmLDamping * this.springDampingMultiplier;
		this.springArmLSource.rotation = SemiFunc.SpringQuaternionGet(this.springArmL, this.springArmLTarget.transform.rotation, -1f);
		this.springArmR.speed = this.springArmRSpeed * this.springSpeedMultiplier;
		this.springArmR.damping = this.springArmRDamping * this.springDampingMultiplier;
		this.springArmRSource.rotation = SemiFunc.SpringQuaternionGet(this.springArmR, this.springArmRTarget.transform.rotation, -1f);
	}

	// Token: 0x06000251 RID: 593 RVA: 0x00018170 File Offset: 0x00016370
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x06000252 RID: 594 RVA: 0x00018182 File Offset: 0x00016382
	public void DelayAttack()
	{
		this.attackLogic.StateSet(FloaterAttackLogic.FloaterAttackState.stop);
		this.SfxDelayAttack();
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00018196 File Offset: 0x00016396
	public void Attack()
	{
		this.attackLogic.StateSet(FloaterAttackLogic.FloaterAttackState.smash);
	}

	// Token: 0x06000254 RID: 596 RVA: 0x000181A4 File Offset: 0x000163A4
	public void SfxDelayAttack()
	{
		this.sfxDelayAttackLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.sfxDelayAttackGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00018208 File Offset: 0x00016408
	public void SfxAttackUp()
	{
		this.sfxAttackUpLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.sfxAttackUpGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000256 RID: 598 RVA: 0x0001826C File Offset: 0x0001646C
	public void SfxAttackDown()
	{
		this.sfxAttackDownLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.sfxAttackDownGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000257 RID: 599 RVA: 0x000182CF File Offset: 0x000164CF
	public void SfxMoveShort()
	{
		this.sfxMoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000258 RID: 600 RVA: 0x000182FC File Offset: 0x000164FC
	public void SfxMoveLong()
	{
		this.sfxMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000259 RID: 601 RVA: 0x00018329 File Offset: 0x00016529
	public void SfxHurt()
	{
		this.sfxHurt.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600025A RID: 602 RVA: 0x00018356 File Offset: 0x00016556
	public void SfxDeath()
	{
		this.sfxDeath.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600025B RID: 603 RVA: 0x00018383 File Offset: 0x00016583
	public void SfxChargeAttackLoop()
	{
		this.sfxChargeAttackLoop.PlayLoop(this.sfxChargeAttackLoopPlaying, 5f, 5f, 1f);
	}

	// Token: 0x0600025C RID: 604 RVA: 0x000183A5 File Offset: 0x000165A5
	public void SfxDelayAttackLoop()
	{
		this.sfxDelayAttackLoop.PlayLoop(this.sfxDelayAttackLoopPlaying, 5f, 5f, 1f);
	}

	// Token: 0x0600025D RID: 605 RVA: 0x000183C7 File Offset: 0x000165C7
	public void SfxStunnedLoop()
	{
		this.sfxStunnedLoop.PlayLoop(this.stunned, 5f, 5f, 1f);
	}

	// Token: 0x04000431 RID: 1073
	private int animStunned = Animator.StringToHash("stunned");

	// Token: 0x04000432 RID: 1074
	private int animDespawning = Animator.StringToHash("despawning");

	// Token: 0x04000433 RID: 1075
	private int animAttacking = Animator.StringToHash("attacking");

	// Token: 0x04000434 RID: 1076
	private int animStun = Animator.StringToHash("Stun");

	// Token: 0x04000435 RID: 1077
	private int animNotice = Animator.StringToHash("Notice");

	// Token: 0x04000436 RID: 1078
	private int animChargeAttack = Animator.StringToHash("ChargeAttack");

	// Token: 0x04000437 RID: 1079
	private int animDelayAttack = Animator.StringToHash("DelayAttack");

	// Token: 0x04000438 RID: 1080
	private int animAttack = Animator.StringToHash("Attack");

	// Token: 0x04000439 RID: 1081
	public Enemy enemy;

	// Token: 0x0400043A RID: 1082
	public EnemyFloater controller;

	// Token: 0x0400043B RID: 1083
	public FloaterAttackLogic attackLogic;

	// Token: 0x0400043C RID: 1084
	public float springSpeedMultiplier = 1f;

	// Token: 0x0400043D RID: 1085
	public float springDampingMultiplier = 1f;

	// Token: 0x0400043E RID: 1086
	public SpringQuaternion springHead;

	// Token: 0x0400043F RID: 1087
	private float springHeadSpeed;

	// Token: 0x04000440 RID: 1088
	private float springHeadDamping;

	// Token: 0x04000441 RID: 1089
	public Transform springHeadTarget;

	// Token: 0x04000442 RID: 1090
	public Transform springHeadSource;

	// Token: 0x04000443 RID: 1091
	public SpringQuaternion springLegL;

	// Token: 0x04000444 RID: 1092
	private float springLegLSpeed;

	// Token: 0x04000445 RID: 1093
	private float springLegLDamping;

	// Token: 0x04000446 RID: 1094
	public Transform springLegLTarget;

	// Token: 0x04000447 RID: 1095
	public Transform springLegLSource;

	// Token: 0x04000448 RID: 1096
	public SpringQuaternion springLegR;

	// Token: 0x04000449 RID: 1097
	private float springLegRSpeed;

	// Token: 0x0400044A RID: 1098
	private float springLegRDamping;

	// Token: 0x0400044B RID: 1099
	public Transform springLegRTarget;

	// Token: 0x0400044C RID: 1100
	public Transform springLegRSource;

	// Token: 0x0400044D RID: 1101
	public SpringQuaternion springArmL;

	// Token: 0x0400044E RID: 1102
	private float springArmLSpeed;

	// Token: 0x0400044F RID: 1103
	private float springArmLDamping;

	// Token: 0x04000450 RID: 1104
	public Transform springArmLTarget;

	// Token: 0x04000451 RID: 1105
	public Transform springArmLSource;

	// Token: 0x04000452 RID: 1106
	public SpringQuaternion springArmR;

	// Token: 0x04000453 RID: 1107
	private float springArmRSpeed;

	// Token: 0x04000454 RID: 1108
	private float springArmRDamping;

	// Token: 0x04000455 RID: 1109
	public Transform springArmRTarget;

	// Token: 0x04000456 RID: 1110
	public Transform springArmRSource;

	// Token: 0x04000457 RID: 1111
	[Header("One Shots")]
	public Sound sfxChargeAttackStart;

	// Token: 0x04000458 RID: 1112
	public Sound sfxDelayAttackLocal;

	// Token: 0x04000459 RID: 1113
	public Sound sfxDelayAttackGlobal;

	// Token: 0x0400045A RID: 1114
	public Sound sfxAttackUpLocal;

	// Token: 0x0400045B RID: 1115
	public Sound sfxAttackUpGlobal;

	// Token: 0x0400045C RID: 1116
	public Sound sfxAttackDownLocal;

	// Token: 0x0400045D RID: 1117
	public Sound sfxAttackDownGlobal;

	// Token: 0x0400045E RID: 1118
	public Sound sfxMoveShort;

	// Token: 0x0400045F RID: 1119
	public Sound sfxMoveLong;

	// Token: 0x04000460 RID: 1120
	public Sound sfxHurt;

	// Token: 0x04000461 RID: 1121
	public Sound sfxDeath;

	// Token: 0x04000462 RID: 1122
	[Header("Loops")]
	public Sound sfxChargeAttackLoop;

	// Token: 0x04000463 RID: 1123
	public Sound sfxDelayAttackLoop;

	// Token: 0x04000464 RID: 1124
	public Sound sfxStunnedLoop;

	// Token: 0x04000465 RID: 1125
	[Header("Animation Booleans")]
	public bool sfxChargeAttackLoopPlaying;

	// Token: 0x04000466 RID: 1126
	public bool sfxDelayAttackLoopPlaying;

	// Token: 0x04000467 RID: 1127
	internal Animator animator;

	// Token: 0x04000468 RID: 1128
	private bool idling;

	// Token: 0x04000469 RID: 1129
	private bool stunned;

	// Token: 0x0400046A RID: 1130
	private bool stunImpulse;

	// Token: 0x0400046B RID: 1131
	private bool noticeImpulse;

	// Token: 0x0400046C RID: 1132
	private bool delayAttackImpulse;

	// Token: 0x0400046D RID: 1133
	private bool attackImpulse;

	// Token: 0x0400046E RID: 1134
	private bool chargeAttackImpulse;
}
