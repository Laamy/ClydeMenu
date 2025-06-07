using System;
using UnityEngine;

// Token: 0x02000069 RID: 105
public class EnemyHiddenAnim : MonoBehaviour
{
	// Token: 0x0600034A RID: 842 RVA: 0x0002087C File Offset: 0x0001EA7C
	private void Update()
	{
		this.BreathingLogic();
		this.FootstepLogic();
		if (this.enemyHidden.currentState == EnemyHidden.State.Stun)
		{
			if (this.soundStunStartImpulse)
			{
				this.StopBreathing();
				this.soundStunStart.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
				this.soundStunStartImpulse = false;
			}
			if (this.soundStunPauseTimer > 0f)
			{
				if (this.soundStunStopImpulse)
				{
					this.soundStunStop.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
					this.soundStunStopImpulse = false;
				}
				this.soundStunLoop.PlayLoop(false, 2f, 5f, 1f);
				this.particleBreathConstant.Stop();
			}
			else
			{
				this.soundStunLoop.PlayLoop(true, 2f, 10f, 1f);
				this.particleBreathConstant.Play();
				this.soundStunStopImpulse = true;
			}
		}
		else
		{
			if (this.soundStunStopImpulse)
			{
				this.soundStunStop.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
				this.soundStunStopImpulse = false;
			}
			this.soundStunLoop.PlayLoop(false, 2f, 5f, 1f);
			this.particleBreathConstant.Stop();
			this.soundStunStartImpulse = true;
		}
		if (this.soundStunPauseTimer > 0f)
		{
			this.soundStunPauseTimer -= Time.deltaTime;
		}
		if (this.enemy.Jump.jumping)
		{
			if (this.soundJumpImpulse)
			{
				this.particleBreath.Play();
				this.soundJump.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
				this.StopBreathing();
				this.soundJumpImpulse = false;
			}
			this.soundLandImpulse = true;
		}
		else
		{
			if (this.soundLandImpulse)
			{
				this.particleBreathFast.Play();
				this.soundLand.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
				this.StopBreathing();
				this.soundLandImpulse = false;
			}
			this.soundJumpImpulse = true;
		}
		if (this.enemyHidden.currentState == EnemyHidden.State.PlayerPickup)
		{
			if (this.soundPlayerPickupImpulse)
			{
				this.StopBreathing();
				this.particleBreath.Play();
				this.soundPlayerPickup.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
				this.soundPlayerPickupImpulse = false;
			}
		}
		else
		{
			this.soundPlayerPickupImpulse = true;
		}
		if (this.enemyHidden.currentState == EnemyHidden.State.PlayerReleaseWait)
		{
			if (this.soundPlayerReleaseImpulse)
			{
				this.StopBreathing();
				this.particleBreath.Play();
				this.soundPlayerRelease.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
				this.soundPlayerReleaseImpulse = false;
			}
		}
		else
		{
			this.soundPlayerReleaseImpulse = true;
		}
		if (this.enemyHidden.currentState == EnemyHidden.State.PlayerMove && !this.enemy.Jump.jumping)
		{
			this.soundPlayerMove.PlayLoop(true, 2f, 10f, 1f);
			this.soundPlayerMoveImpulse = true;
			return;
		}
		if (this.soundPlayerMoveImpulse)
		{
			this.soundPlayerMoveStop.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
			this.soundPlayerMoveImpulse = false;
		}
		this.soundPlayerMove.PlayLoop(false, 2f, 10f, 1f);
	}

	// Token: 0x0600034B RID: 843 RVA: 0x00020C4C File Offset: 0x0001EE4C
	private void BreathingLogic()
	{
		if (this.enemy.Jump.jumping || this.enemyHidden.currentState == EnemyHidden.State.Stun || this.enemyHidden.currentState == EnemyHidden.State.PlayerRelease || this.enemyHidden.currentState == EnemyHidden.State.PlayerReleaseWait || this.enemyHidden.currentState == EnemyHidden.State.PlayerPickup)
		{
			this.breathingState = EnemyHiddenAnim.BreathingState.None;
		}
		else if (this.enemyHidden.currentState == EnemyHidden.State.PlayerMove)
		{
			this.breathingState = EnemyHiddenAnim.BreathingState.FastNoSound;
		}
		else if (this.enemyHidden.currentState == EnemyHidden.State.PlayerGoTo || this.enemyHidden.currentState == EnemyHidden.State.Leave)
		{
			this.breathingState = EnemyHiddenAnim.BreathingState.Fast;
		}
		else if (this.enemyHidden.currentState == EnemyHidden.State.Roam || this.enemyHidden.currentState == EnemyHidden.State.Investigate)
		{
			this.breathingState = EnemyHiddenAnim.BreathingState.Medium;
		}
		else
		{
			this.breathingState = EnemyHiddenAnim.BreathingState.Slow;
		}
		if (this.breathingState == EnemyHiddenAnim.BreathingState.None)
		{
			this.soundBreatheIn.Stop();
			this.soundBreatheOut.Stop();
		}
		if (this.breathingTimer <= 0f)
		{
			if (this.breathingCurrent)
			{
				this.breathingCurrent = false;
				if (this.breathingState != EnemyHiddenAnim.BreathingState.FastNoSound)
				{
					if (this.breathingState == EnemyHiddenAnim.BreathingState.Fast)
					{
						this.soundBreatheInFast.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
					}
					else
					{
						this.soundBreatheIn.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
					}
				}
				else
				{
					this.particleBreathFast.Play();
				}
				this.breathingTimer = 3f;
			}
			else
			{
				this.breathingCurrent = true;
				if (this.breathingState != EnemyHiddenAnim.BreathingState.FastNoSound)
				{
					if (this.breathingState == EnemyHiddenAnim.BreathingState.Fast)
					{
						this.soundBreatheOutFast.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
					}
					else
					{
						this.soundBreatheOut.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
					}
					this.particleBreath.Play();
				}
				else
				{
					this.particleBreathFast.Play();
				}
				this.breathingTimer = 4.5f;
			}
		}
		if (this.breathingState == EnemyHiddenAnim.BreathingState.Slow)
		{
			this.breathingTimer -= 1f * Time.deltaTime;
			return;
		}
		if (this.breathingState == EnemyHiddenAnim.BreathingState.Medium)
		{
			this.breathingTimer -= 2f * Time.deltaTime;
			return;
		}
		this.breathingTimer -= 5f * Time.deltaTime;
	}

	// Token: 0x0600034C RID: 844 RVA: 0x00020EE4 File Offset: 0x0001F0E4
	private void FootstepLogic()
	{
		if (this.movingTimer > 0f)
		{
			this.movingTimer -= Time.deltaTime;
		}
		if ((this.enemyHidden.currentState == EnemyHidden.State.Roam || this.enemyHidden.currentState == EnemyHidden.State.Investigate || this.enemyHidden.currentState == EnemyHidden.State.PlayerGoTo || this.enemyHidden.currentState == EnemyHidden.State.PlayerMove || this.enemyHidden.currentState == EnemyHidden.State.Leave) && this.enemy.Rigidbody.velocity.magnitude > 0.5f)
		{
			this.movingTimer = 0.25f;
		}
		if (this.enemyHidden.currentState == EnemyHidden.State.Stun || this.enemy.Jump.jumping)
		{
			this.footstepState = EnemyHiddenAnim.FootstepState.None;
		}
		else if (this.enemyHidden.currentState == EnemyHidden.State.StunEnd || this.enemyHidden.currentState == EnemyHidden.State.PlayerNotice)
		{
			this.footstepState = EnemyHiddenAnim.FootstepState.TimedSteps;
		}
		else if (this.movingTimer > 0f)
		{
			if (this.enemyHidden.currentState == EnemyHidden.State.PlayerGoTo || this.enemyHidden.currentState == EnemyHidden.State.PlayerMove || this.enemyHidden.currentState == EnemyHidden.State.Leave)
			{
				this.footstepState = EnemyHiddenAnim.FootstepState.Sprinting;
			}
			else
			{
				this.footstepState = EnemyHiddenAnim.FootstepState.Moving;
			}
		}
		else if (this.footstepState == EnemyHiddenAnim.FootstepState.Moving)
		{
			this.footstepState = EnemyHiddenAnim.FootstepState.TwoStep;
		}
		else if (this.footstepState != EnemyHiddenAnim.FootstepState.TwoStep)
		{
			this.footstepState = EnemyHiddenAnim.FootstepState.Standing;
		}
		if (this.enemy.Jump.jumping)
		{
			if (this.jumpStartImpulse)
			{
				this.jumpStopImpulse = true;
				this.jumpStartImpulse = false;
				this.FootstepSet();
				this.FootstepSet();
			}
		}
		else if (this.jumpStopImpulse)
		{
			this.jumpStopImpulse = false;
			this.jumpStartImpulse = true;
			this.FootstepSet();
			this.FootstepSet();
		}
		if ((this.footstepState == EnemyHiddenAnim.FootstepState.Moving || this.footstepState == EnemyHiddenAnim.FootstepState.Sprinting) && Vector3.Distance(this.transformFoot.position, this.footstepPositionPrevious) > 1f)
		{
			this.FootstepSet();
		}
		if (this.footstepState == EnemyHiddenAnim.FootstepState.TimedSteps)
		{
			if (this.timedStepsTimer <= 0f)
			{
				this.timedStepsTimer = 0.25f;
				this.FootstepSet();
			}
			else
			{
				this.timedStepsTimer -= Time.deltaTime;
			}
		}
		else
		{
			this.timedStepsTimer = 0f;
		}
		if (this.footstepState == EnemyHiddenAnim.FootstepState.TwoStep)
		{
			if (this.stopStepTimer == -1f)
			{
				this.FootstepSet();
				this.stopStepTimer = 0.25f;
				return;
			}
			this.stopStepTimer -= Time.deltaTime;
			if (this.stopStepTimer <= 0f)
			{
				this.footstepState = EnemyHiddenAnim.FootstepState.Standing;
				this.FootstepSet();
				this.stopStepTimer = -1f;
				return;
			}
		}
		else
		{
			this.stopStepTimer = -1f;
		}
	}

	// Token: 0x0600034D RID: 845 RVA: 0x00021180 File Offset: 0x0001F380
	private void FootstepSet()
	{
		Vector3 b = this.transformFoot.right * (-0.3f * (float)this.footstepCurrent);
		Vector3 b2 = Random.insideUnitSphere * 0.15f;
		b2.y = 0f;
		RaycastHit raycastHit;
		if (Physics.Raycast(this.transformFoot.position + b + b2, Vector3.down * 2f, out raycastHit, 3f, LayerMask.GetMask(new string[]
		{
			"Default"
		})))
		{
			ParticleSystem particleSystem = this.particleFootstepShapeRight;
			Vector3 a = this.footstepPositionPreviousRight;
			if (this.footstepCurrent == 1)
			{
				particleSystem = this.particleFootstepShapeLeft;
				a = this.footstepPositionPreviousLeft;
			}
			if (Vector3.Distance(a, raycastHit.point) > 0.2f)
			{
				particleSystem.transform.position = raycastHit.point + Vector3.up * 0.02f;
				particleSystem.transform.eulerAngles = new Vector3(0f, this.transformFoot.eulerAngles.y, 0f);
				particleSystem.Play();
				this.particleFootstepSmoke.transform.position = particleSystem.transform.position;
				this.particleFootstepSmoke.transform.rotation = particleSystem.transform.rotation;
				this.particleFootstepSmoke.Play();
				Materials.Instance.Impulse(particleSystem.transform.position + Vector3.up * 0.5f, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
				if (this.footstepState == EnemyHiddenAnim.FootstepState.Sprinting)
				{
					this.soundFootstepSprint.Play(particleSystem.transform.position, 1f, 1f, 1f, 1f);
				}
				else
				{
					this.soundFootstep.Play(particleSystem.transform.position, 1f, 1f, 1f, 1f);
				}
				if (this.footstepCurrent == 1)
				{
					this.footstepPositionPreviousLeft = raycastHit.point;
				}
				else
				{
					this.footstepPositionPreviousRight = raycastHit.point;
				}
				this.footstepCurrent *= -1;
			}
		}
		this.footstepPositionPrevious = this.transformFoot.position;
	}

	// Token: 0x0600034E RID: 846 RVA: 0x000213C3 File Offset: 0x0001F5C3
	public void StopBreathing()
	{
		this.soundBreatheIn.Stop();
		this.soundBreatheInFast.Stop();
		this.soundBreatheOut.Stop();
		this.soundBreatheOutFast.Stop();
	}

	// Token: 0x0600034F RID: 847 RVA: 0x000213F1 File Offset: 0x0001F5F1
	public void StunPause()
	{
		this.soundStunPauseTimer = 1f;
	}

	// Token: 0x06000350 RID: 848 RVA: 0x000213FE File Offset: 0x0001F5FE
	public void Hurt()
	{
		this.StopBreathing();
		this.StunPause();
		this.soundHurt.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000351 RID: 849 RVA: 0x0002143C File Offset: 0x0001F63C
	public void Death()
	{
		this.particleBreathConstant.Stop();
		this.StopBreathing();
		this.StunPause();
		this.soundDeath.Play(this.particleBreath.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040005BA RID: 1466
	private EnemyHiddenAnim.BreathingState breathingState;

	// Token: 0x040005BB RID: 1467
	private EnemyHiddenAnim.FootstepState footstepState;

	// Token: 0x040005BC RID: 1468
	[Space]
	public Enemy enemy;

	// Token: 0x040005BD RID: 1469
	public EnemyHidden enemyHidden;

	// Token: 0x040005BE RID: 1470
	internal Materials.MaterialTrigger material = new Materials.MaterialTrigger();

	// Token: 0x040005BF RID: 1471
	[Space]
	public ParticleSystem particleBreath;

	// Token: 0x040005C0 RID: 1472
	public ParticleSystem particleBreathFast;

	// Token: 0x040005C1 RID: 1473
	public ParticleSystem particleBreathConstant;

	// Token: 0x040005C2 RID: 1474
	private bool breathingCurrent;

	// Token: 0x040005C3 RID: 1475
	private float breathingTimer;

	// Token: 0x040005C4 RID: 1476
	[Space]
	public Transform transformFoot;

	// Token: 0x040005C5 RID: 1477
	public ParticleSystem particleFootstepShapeRight;

	// Token: 0x040005C6 RID: 1478
	public ParticleSystem particleFootstepShapeLeft;

	// Token: 0x040005C7 RID: 1479
	public ParticleSystem particleFootstepSmoke;

	// Token: 0x040005C8 RID: 1480
	private Vector3 footstepPositionPrevious;

	// Token: 0x040005C9 RID: 1481
	private Vector3 footstepPositionPreviousRight;

	// Token: 0x040005CA RID: 1482
	private Vector3 footstepPositionPreviousLeft;

	// Token: 0x040005CB RID: 1483
	private int footstepCurrent = 1;

	// Token: 0x040005CC RID: 1484
	private float movingTimer;

	// Token: 0x040005CD RID: 1485
	private float stopStepTimer;

	// Token: 0x040005CE RID: 1486
	private float timedStepsTimer;

	// Token: 0x040005CF RID: 1487
	private bool jumpStartImpulse = true;

	// Token: 0x040005D0 RID: 1488
	private bool jumpStopImpulse;

	// Token: 0x040005D1 RID: 1489
	[Space]
	public Sound soundBreatheIn;

	// Token: 0x040005D2 RID: 1490
	public Sound soundBreatheOut;

	// Token: 0x040005D3 RID: 1491
	[Space]
	public Sound soundBreatheInFast;

	// Token: 0x040005D4 RID: 1492
	public Sound soundBreatheOutFast;

	// Token: 0x040005D5 RID: 1493
	[Space]
	public Sound soundFootstep;

	// Token: 0x040005D6 RID: 1494
	public Sound soundFootstepSprint;

	// Token: 0x040005D7 RID: 1495
	[Space]
	public Sound soundStunStart;

	// Token: 0x040005D8 RID: 1496
	private bool soundStunStartImpulse;

	// Token: 0x040005D9 RID: 1497
	public Sound soundStunLoop;

	// Token: 0x040005DA RID: 1498
	public Sound soundStunStop;

	// Token: 0x040005DB RID: 1499
	private bool soundStunStopImpulse;

	// Token: 0x040005DC RID: 1500
	private float soundStunPauseTimer;

	// Token: 0x040005DD RID: 1501
	[Space]
	public Sound soundJump;

	// Token: 0x040005DE RID: 1502
	private bool soundJumpImpulse;

	// Token: 0x040005DF RID: 1503
	public Sound soundLand;

	// Token: 0x040005E0 RID: 1504
	private bool soundLandImpulse;

	// Token: 0x040005E1 RID: 1505
	[Space]
	public Sound soundPlayerPickup;

	// Token: 0x040005E2 RID: 1506
	private bool soundPlayerPickupImpulse;

	// Token: 0x040005E3 RID: 1507
	public Sound soundPlayerRelease;

	// Token: 0x040005E4 RID: 1508
	private bool soundPlayerReleaseImpulse;

	// Token: 0x040005E5 RID: 1509
	public Sound soundPlayerMove;

	// Token: 0x040005E6 RID: 1510
	public Sound soundPlayerMoveStop;

	// Token: 0x040005E7 RID: 1511
	private bool soundPlayerMoveImpulse;

	// Token: 0x040005E8 RID: 1512
	[Space]
	public Sound soundHurt;

	// Token: 0x040005E9 RID: 1513
	public Sound soundDeath;

	// Token: 0x02000314 RID: 788
	public enum BreathingState
	{
		// Token: 0x040028AE RID: 10414
		None,
		// Token: 0x040028AF RID: 10415
		Slow,
		// Token: 0x040028B0 RID: 10416
		Medium,
		// Token: 0x040028B1 RID: 10417
		Fast,
		// Token: 0x040028B2 RID: 10418
		FastNoSound
	}

	// Token: 0x02000315 RID: 789
	public enum FootstepState
	{
		// Token: 0x040028B4 RID: 10420
		None,
		// Token: 0x040028B5 RID: 10421
		Standing,
		// Token: 0x040028B6 RID: 10422
		TwoStep,
		// Token: 0x040028B7 RID: 10423
		Moving,
		// Token: 0x040028B8 RID: 10424
		Sprinting,
		// Token: 0x040028B9 RID: 10425
		TimedSteps
	}
}
