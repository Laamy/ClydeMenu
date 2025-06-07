using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000079 RID: 121
public class EnemySlowMouthPlayerAvatarAttached : MonoBehaviour
{
	// Token: 0x0600048D RID: 1165 RVA: 0x0002DBCC File Offset: 0x0002BDCC
	private void Start()
	{
		this.loudnessSpring = new SpringFloat();
		this.loudnessSpring.damping = 0.5f;
		this.loudnessSpring.speed = 20f;
		this.springFloatScale = new SpringFloat();
		this.springFloatScale.damping = 0.35f;
		this.springFloatScale.speed = 10f;
		base.transform.localScale = Vector3.one * 2f;
		this.springFloatScale.lastPosition = 2f;
		this.playerVoiceChat = this.playerTarget.voiceChat;
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x0002DC6A File Offset: 0x0002BE6A
	private void StateIntro()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.loudnessTarget = 0f;
		this.scaleTarget = 1f;
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x0002DC91 File Offset: 0x0002BE91
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.loudnessTarget = 0f;
		this.scaleTarget = 1f;
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x0002DCB8 File Offset: 0x0002BEB8
	private void StatePuke()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		float num = Mathf.Sin(Time.time * 40f) * 0.05f;
		this.loudnessTarget = 0.2f + num;
		this.scaleTarget = 1f;
		this.semiPuke.PukeActive(this.semiPuke.transform.position, this.playerTarget.localCameraTransform.rotation);
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x0002DD30 File Offset: 0x0002BF30
	private void StateOutro()
	{
		if (this.stateStart)
		{
			this.particles.gameObject.SetActive(true);
			this.stateStart = false;
		}
		this.loudnessTarget = 0f;
		this.scaleTarget = 0f;
		if (base.transform.localScale.x < 0.05f)
		{
			this.enemySlowMouth.UpdateState(EnemySlowMouth.State.Detach);
			Object.Destroy(this.jawBot.gameObject);
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0002DDB4 File Offset: 0x0002BFB4
	private void StateMachine()
	{
		switch (this.state)
		{
		case EnemySlowMouthPlayerAvatarAttached.State.Intro:
			this.StateIntro();
			return;
		case EnemySlowMouthPlayerAvatarAttached.State.Idle:
			this.StateIdle();
			return;
		case EnemySlowMouthPlayerAvatarAttached.State.Puke:
			this.StatePuke();
			return;
		case EnemySlowMouthPlayerAvatarAttached.State.Outro:
			this.StateOutro();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x0002DDFC File Offset: 0x0002BFFC
	private void Update()
	{
		if (this.playerVoiceChat)
		{
			this.playerVoiceChat.OverrideClipLoudnessAnimationValue(this.loudnessAdd);
		}
		else if (this.playerTarget)
		{
			this.playerVoiceChat = this.playerTarget.voiceChat;
		}
		this.loudnessAdd = SemiFunc.SpringFloatGet(this.loudnessSpring, this.loudnessTarget, -1f);
		Quaternion rotation = this.playerTarget.playerAvatarVisuals.playerEyes.eyeLeft.rotation;
		Quaternion rotation2 = this.playerTarget.playerAvatarVisuals.playerEyes.eyeRight.rotation;
		this.eyeTransforms[0].rotation = rotation;
		this.eyeTransforms[1].rotation = rotation2;
		this.StateSynchingWithParentEnemy();
		this.StateMachine();
		base.transform.localScale = Vector3.one * SemiFunc.SpringFloatGet(this.springFloatScale, this.scaleTarget, -1f);
		this.jawBot.localScale = Vector3.one * SemiFunc.SpringFloatGet(this.springFloatScale, this.scaleTarget, -1f);
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x0002DF20 File Offset: 0x0002C120
	private void StateSynchingWithParentEnemy()
	{
		if (!this.enemySlowMouth)
		{
			this.StateSet(EnemySlowMouthPlayerAvatarAttached.State.Outro);
			return;
		}
		bool flag = this.enemySlowMouth.currentState == EnemySlowMouth.State.Puke;
		if (this.enemySlowMouth.currentState == EnemySlowMouth.State.Attached || this.enemySlowMouth.currentState == EnemySlowMouth.State.Puke || this.enemySlowMouth.currentState == EnemySlowMouth.State.Detach)
		{
			if (flag)
			{
				this.StateSet(EnemySlowMouthPlayerAvatarAttached.State.Puke);
				return;
			}
			if (this.state != EnemySlowMouthPlayerAvatarAttached.State.Intro)
			{
				this.StateSet(EnemySlowMouthPlayerAvatarAttached.State.Idle);
				return;
			}
		}
		else
		{
			this.StateSet(EnemySlowMouthPlayerAvatarAttached.State.Outro);
		}
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x0002DFA6 File Offset: 0x0002C1A6
	private void StateSet(EnemySlowMouthPlayerAvatarAttached.State _state)
	{
		if (this.state != _state)
		{
			this.state = _state;
			this.stateStart = true;
		}
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x0002DFC0 File Offset: 0x0002C1C0
	private void OnDisable()
	{
		if (this.playerTarget.isDisabled)
		{
			this.enemySlowMouth.UpdateState(EnemySlowMouth.State.Detach);
			this.enemySlowMouth.detachPosition = this.playerTarget.localCameraPosition;
			this.enemySlowMouth.detachRotation = this.playerTarget.localCameraRotation;
			Object.Destroy(this.jawBot.gameObject);
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0400077A RID: 1914
	public Transform jawBot;

	// Token: 0x0400077B RID: 1915
	public Transform particles;

	// Token: 0x0400077C RID: 1916
	private SpringFloat springFloatScale;

	// Token: 0x0400077D RID: 1917
	internal EnemySlowMouth enemySlowMouth;

	// Token: 0x0400077E RID: 1918
	internal SemiPuke semiPuke;

	// Token: 0x0400077F RID: 1919
	private float scaleTarget = 1f;

	// Token: 0x04000780 RID: 1920
	private bool stateStart;

	// Token: 0x04000781 RID: 1921
	internal PlayerAvatar playerTarget;

	// Token: 0x04000782 RID: 1922
	public List<Transform> eyeTransforms;

	// Token: 0x04000783 RID: 1923
	private PlayerVoiceChat playerVoiceChat;

	// Token: 0x04000784 RID: 1924
	private float loudnessAdd;

	// Token: 0x04000785 RID: 1925
	private SpringFloat loudnessSpring;

	// Token: 0x04000786 RID: 1926
	private float loudnessTarget;

	// Token: 0x04000787 RID: 1927
	public EnemySlowMouthPlayerAvatarAttached.State state;

	// Token: 0x0200031D RID: 797
	public enum State
	{
		// Token: 0x04002918 RID: 10520
		Intro,
		// Token: 0x04002919 RID: 10521
		Idle,
		// Token: 0x0400291A RID: 10522
		Puke,
		// Token: 0x0400291B RID: 10523
		Outro
	}
}
