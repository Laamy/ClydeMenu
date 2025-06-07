using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000075 RID: 117
public class EnemySlowMouthAnim : MonoBehaviour
{
	// Token: 0x0600045F RID: 1119 RVA: 0x0002B958 File Offset: 0x00029B58
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.jawOpen = 0f;
			this.talkVolume = 0f;
			this.stateStart = false;
		}
		this.CodeAnimatedTalk(1f);
		this.AnimateEyes(5f);
		this.Paddle(5f, 20f);
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x0002B9B0 File Offset: 0x00029BB0
	private void StatePuke()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		float targetFloat = 30f + Mathf.Sin(Time.time * 40f) * 10f;
		this.upperJaw.localRotation = Quaternion.Euler(this.upperJawStartRot - this.jawOpen, 0f, 0f);
		this.lowerJaw.localRotation = Quaternion.Euler(this.lowerJawStartRot + this.jawOpen, 0f, 0f);
		this.jawOpen = SemiFunc.SpringFloatGet(this.jawSpring, targetFloat, -1f);
		this.AnimateEyes(30f);
		this.EyeScaleSitter(6f);
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x0002BA64 File Offset: 0x00029C64
	private void StateStunned()
	{
		if (this.stateStart)
		{
			this.jawOpen = 20f;
			this.stateStart = false;
		}
		this.AnimateEyes(20f);
		this.EyeScaleSitter(4f);
		float targetFloat = 30f + Mathf.Sin(Time.time * 40f) * 10f;
		this.upperJaw.localRotation = Quaternion.Euler(this.upperJawStartRot - this.jawOpen, 0f, 0f);
		this.lowerJaw.localRotation = Quaternion.Euler(this.lowerJawStartRot + this.jawOpen, 0f, 0f);
		this.jawOpen = SemiFunc.SpringFloatGet(this.jawSpring, targetFloat, -1f);
		this.Paddle(30f, 10f);
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x0002BB34 File Offset: 0x00029D34
	private void StateTargetting()
	{
		if (this.stateStart)
		{
			this.jawOpen = 0f;
			this.stateStart = false;
		}
		this.AnimateEyes(20f);
		this.EyeScaleSitter(4f);
		float targetFloat = 30f + Mathf.Sin(Time.time * 40f) * 10f;
		this.upperJaw.localRotation = Quaternion.Euler(this.upperJawStartRot - this.jawOpen, 0f, 0f);
		this.lowerJaw.localRotation = Quaternion.Euler(this.lowerJawStartRot + this.jawOpen, 0f, 0f);
		this.jawOpen = SemiFunc.SpringFloatGet(this.jawSpring, targetFloat, -1f);
		this.Paddle(20f, 12f);
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x0002BC03 File Offset: 0x00029E03
	private void StateAttached()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x0002BC14 File Offset: 0x00029E14
	private void StateAggro()
	{
		if (this.stateStart)
		{
			this.jawOpen = 0f;
			this.stateStart = false;
		}
		this.AnimateEyes(20f);
		this.EyeScaleSitter(4f);
		this.CodeAnimatedTalk(1f);
		this.Paddle(10f, 20f);
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x0002BC6C File Offset: 0x00029E6C
	private void StateSpawnDespawn()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.upperJaw.localRotation = Quaternion.Slerp(this.upperJaw.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 5f);
		this.lowerJaw.localRotation = Quaternion.Slerp(this.lowerJaw.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 5f);
		this.Paddle(5f, 20f);
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x0002BD0C File Offset: 0x00029F0C
	private void StateDeath()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x0002BD20 File Offset: 0x00029F20
	private void StateLeave()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.jawOpen = 0f;
			this.talkVolume = 0f;
		}
		float targetFloat = 5f + Mathf.Sin(Time.time * 40f) * 5f;
		this.upperJaw.localRotation = Quaternion.Euler(this.upperJawStartRot - this.jawOpen, 0f, 0f);
		this.lowerJaw.localRotation = Quaternion.Euler(this.lowerJawStartRot + this.jawOpen, 0f, 0f);
		this.jawOpen = SemiFunc.SpringFloatGet(this.jawSpring, targetFloat, -1f);
		this.Paddle(10f, 20f);
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x0002BDE4 File Offset: 0x00029FE4
	private void EyesLookAtTarget()
	{
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x0002BDE8 File Offset: 0x00029FE8
	private void CodeAnimatedTalk(float _multiplier = 1f)
	{
		if (SemiFunc.FPSImpulse15())
		{
			float[] array = new float[1024];
			this.audioSource.GetSpectrumData(array, 0, FFTWindow.Hamming);
			float num = array[0] * 50000f * _multiplier;
			if (num > 20f)
			{
				num = 20f;
			}
			this.talkVolume = num;
			this.jawSpring.springVelocity += Random.Range(-25f, 0f);
		}
		this.upperJaw.localRotation = Quaternion.Euler(this.upperJawStartRot - this.jawOpen, 0f, 0f);
		this.lowerJaw.localRotation = Quaternion.Euler(this.lowerJawStartRot + this.jawOpen, 0f, 0f);
		this.jawSpring.damping = 0.2f;
		this.jawSpring.speed = 25f;
		this.jawOpen = SemiFunc.SpringFloatGet(this.jawSpring, this.talkVolume, -1f);
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0002BEE4 File Offset: 0x0002A0E4
	private void Start()
	{
		this.eyeLeftSpringScale = new SpringFloat();
		this.eyeLeftSpringScale.damping = 0.01f;
		this.eyeLeftSpringScale.speed = 40f;
		this.eyeRightSpringScale = new SpringFloat();
		this.eyeRightSpringScale.damping = 0.01f;
		this.eyeRightSpringScale.speed = 40f;
		this.eyeRotationSpring = new SpringQuaternion();
		this.eyeRotationSpring.damping = 0.01f;
		this.eyeRotationSpring.speed = 40f;
		this.startPos = base.transform.position;
		this.directionRotation = new SpringQuaternion();
		this.directionRotation.damping = 0.5f;
		this.directionRotation.speed = 10f;
		this.upperJawStartRot = this.upperJaw.localEulerAngles.x;
		this.upperJawStartRot = this.lowerJaw.localEulerAngles.x;
		this.audioSource = base.GetComponent<AudioSource>();
		this.jawSpring = new SpringFloat();
		this.jawSpring.damping = 0.12f;
		this.jawSpring.speed = 12f;
		this.eyeTarget = PlayerAvatar.instance.PlayerVisionTarget.VisionTransform;
		this.particleSystems.AddRange(this.particleTransforn.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x0002C03C File Offset: 0x0002A23C
	private void StateMachine()
	{
		foreach (Transform transform in this.eyes)
		{
			if (transform == this.eyes[0])
			{
				transform.localScale = Vector3.one * SemiFunc.SpringFloatGet(this.eyeLeftSpringScale, 1f, -1f);
			}
			else
			{
				transform.localScale = Vector3.one * SemiFunc.SpringFloatGet(this.eyeRightSpringScale, 1f, -1f);
			}
		}
		switch (this.state)
		{
		case EnemySlowMouthAnim.State.Idle:
			this.StateIdle();
			break;
		case EnemySlowMouthAnim.State.Puke:
			this.StatePuke();
			break;
		case EnemySlowMouthAnim.State.Stunned:
			this.StateStunned();
			break;
		case EnemySlowMouthAnim.State.Targetting:
			this.StateTargetting();
			break;
		case EnemySlowMouthAnim.State.Attached:
			this.StateAttached();
			break;
		case EnemySlowMouthAnim.State.Aggro:
			this.StateAggro();
			break;
		case EnemySlowMouthAnim.State.SpawnDespawn:
			this.StateSpawnDespawn();
			break;
		case EnemySlowMouthAnim.State.Death:
			this.StateDeath();
			break;
		case EnemySlowMouthAnim.State.Leave:
			this.StateLeave();
			break;
		}
		if (this.state == EnemySlowMouthAnim.State.SpawnDespawn || this.state == EnemySlowMouthAnim.State.Death)
		{
			this.PlayParticles(false);
			return;
		}
		if (this.jawOpen > 15f)
		{
			this.PlayParticles(true);
			return;
		}
		this.PlayParticles(false);
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x0002C1A0 File Offset: 0x0002A3A0
	public void UpdateState(EnemySlowMouthAnim.State _state)
	{
		if (this.state == _state)
		{
			return;
		}
		this.state = _state;
		this.stateStart = true;
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x0002C1BC File Offset: 0x0002A3BC
	private void Update()
	{
		this.StateMachine();
		if (this.paddleTimer > 0f)
		{
			this.paddleTimer -= Time.deltaTime;
		}
		if (this.paddleTimer <= 0f)
		{
			this.visualsTransform.localRotation = Quaternion.Slerp(this.visualsTransform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
		}
		if (this.talkVolume > 0f)
		{
			this.talkVolume = Mathf.Lerp(this.talkVolume, 0f, Time.deltaTime * 5f);
		}
		if (this.jawOpen > 0f)
		{
			this.jawOpen = Mathf.Lerp(this.jawOpen, 0f, Time.deltaTime * 5f);
		}
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x0002C284 File Offset: 0x0002A484
	private void AnimateEyes(float _eyeJitterAmount)
	{
		if (SemiFunc.FPSImpulse15())
		{
			this.eyeRotationSpring.springVelocity += Random.insideUnitSphere * _eyeJitterAmount;
		}
		foreach (Transform transform in this.eyes)
		{
			transform.localRotation = SemiFunc.SpringQuaternionGet(this.eyeRotationSpring, Quaternion.identity, -1f);
		}
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x0002C314 File Offset: 0x0002A514
	private void EyeScaleSitter(float _amount)
	{
		this.eyeLeftSpringScale.springVelocity += Random.Range(0f, _amount);
		this.eyeRightSpringScale.springVelocity += Random.Range(0f, _amount);
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x0002C350 File Offset: 0x0002A550
	public void PlayParticles(bool _play)
	{
		if (this.particlesPlaying == _play)
		{
			return;
		}
		this.particlesPlaying = _play;
		foreach (ParticleSystem particleSystem in this.particleSystems)
		{
			if (_play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x0002C3C0 File Offset: 0x0002A5C0
	private void Paddle(float _speed, float _amount)
	{
		float num = Mathf.Sin(Time.time * _speed) * _amount;
		this.visualsTransform.localRotation = Quaternion.Euler(0f, num * 2f, 0f);
		this.paddleTimer = 0.1f;
	}

	// Token: 0x04000725 RID: 1829
	public Transform visualsTransform;

	// Token: 0x04000726 RID: 1830
	private Vector3 startPos;

	// Token: 0x04000727 RID: 1831
	private Vector3 prevPos;

	// Token: 0x04000728 RID: 1832
	private SpringQuaternion directionRotation;

	// Token: 0x04000729 RID: 1833
	public Transform upperJaw;

	// Token: 0x0400072A RID: 1834
	public Transform lowerJaw;

	// Token: 0x0400072B RID: 1835
	public Transform headTransform;

	// Token: 0x0400072C RID: 1836
	private float upperJawStartRot;

	// Token: 0x0400072D RID: 1837
	private float lowerJawStartRot;

	// Token: 0x0400072E RID: 1838
	public Sound talkLoop;

	// Token: 0x0400072F RID: 1839
	private AudioSource audioSource;

	// Token: 0x04000730 RID: 1840
	private SpringFloat jawSpring;

	// Token: 0x04000731 RID: 1841
	private float jawOpen;

	// Token: 0x04000732 RID: 1842
	public List<Transform> eyes = new List<Transform>();

	// Token: 0x04000733 RID: 1843
	private Transform eyeTarget;

	// Token: 0x04000734 RID: 1844
	public Transform eyesMiddle;

	// Token: 0x04000735 RID: 1845
	private Quaternion eyeRotation;

	// Token: 0x04000736 RID: 1846
	private SpringQuaternion eyeRotationSpring;

	// Token: 0x04000737 RID: 1847
	public Transform particleTransforn;

	// Token: 0x04000738 RID: 1848
	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	// Token: 0x04000739 RID: 1849
	private bool particlesPlaying;

	// Token: 0x0400073A RID: 1850
	public EnemySlowMouth enemySlowMouth;

	// Token: 0x0400073B RID: 1851
	private bool stateStart;

	// Token: 0x0400073C RID: 1852
	private SpringFloat eyeLeftSpringScale;

	// Token: 0x0400073D RID: 1853
	private SpringFloat eyeRightSpringScale;

	// Token: 0x0400073E RID: 1854
	private float paddleTimer;

	// Token: 0x0400073F RID: 1855
	private float talkVolume;

	// Token: 0x04000740 RID: 1856
	public EnemySlowMouthAnim.State state;

	// Token: 0x0200031B RID: 795
	public enum State
	{
		// Token: 0x04002909 RID: 10505
		Idle,
		// Token: 0x0400290A RID: 10506
		Puke,
		// Token: 0x0400290B RID: 10507
		Stunned,
		// Token: 0x0400290C RID: 10508
		Targetting,
		// Token: 0x0400290D RID: 10509
		Attached,
		// Token: 0x0400290E RID: 10510
		Aggro,
		// Token: 0x0400290F RID: 10511
		SpawnDespawn,
		// Token: 0x04002910 RID: 10512
		Death,
		// Token: 0x04002911 RID: 10513
		Leave
	}
}
