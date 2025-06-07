using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002A1 RID: 673
public class SemiZuperFlames : MonoBehaviour
{
	// Token: 0x06001505 RID: 5381 RVA: 0x000B9CFC File Offset: 0x000B7EFC
	private void StateNone()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.PlayBaseParticles(false);
		if (this.flamesActive)
		{
			this.StateSet(SemiZuperFlames.State.FlamesStart);
		}
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x000B9D24 File Offset: 0x000B7F24
	private void StateFlamesStart()
	{
		if (this.stateStart)
		{
			this.soundPukeStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.flameLight.enabled = true;
			this.flameLight.intensity = 0f;
			this.PlayAllParticles(true);
			this.stateStart = false;
		}
		this.PlayBaseParticles(true);
		this.StateSet(SemiZuperFlames.State.Flames);
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x000B9D9C File Offset: 0x000B7F9C
	private void StateFlames()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.PlayBaseParticles(true);
		this.flameLight.intensity = Mathf.Lerp(this.flameLight.intensity, 1f, Time.deltaTime * 20f);
		this.flameLight.intensity += Mathf.Sin(Time.time * 20f) * 0.05f;
		if (!this.flamesActive)
		{
			this.StateSet(SemiZuperFlames.State.FlamesEnd);
		}
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x000B9E24 File Offset: 0x000B8024
	private void StateFlamesEnd()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.soundPukeEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.flameLight.enabled = false;
			this.PlayAllParticles(false);
			this.pukeEnd.Play();
		}
		this.PlayBaseParticles(false);
		this.flameLight.intensity = Mathf.Lerp(this.flameLight.intensity, 0f, Time.deltaTime * 40f);
		if (this.flameLight.intensity < 0.01f)
		{
			this.flameLight.intensity = 0f;
			this.StateSet(SemiZuperFlames.State.None);
		}
	}

	// Token: 0x06001509 RID: 5385 RVA: 0x000B9EE4 File Offset: 0x000B80E4
	private void PlayAllParticles(bool _play)
	{
		foreach (ParticleSystem particleSystem in this.pukeParticles)
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

	// Token: 0x0600150A RID: 5386 RVA: 0x000B9F44 File Offset: 0x000B8144
	private void StateMachine()
	{
		bool playing = this.flamesActive;
		this.soundPukeLoop.PlayLoop(playing, 2f, 2f, 1f);
		switch (this.state)
		{
		case SemiZuperFlames.State.None:
			this.StateNone();
			return;
		case SemiZuperFlames.State.FlamesStart:
			this.StateFlamesStart();
			return;
		case SemiZuperFlames.State.Flames:
			this.StateFlames();
			return;
		case SemiZuperFlames.State.FlamesEnd:
			this.StateFlamesEnd();
			return;
		default:
			return;
		}
	}

	// Token: 0x0600150B RID: 5387 RVA: 0x000B9FAC File Offset: 0x000B81AC
	private void Start()
	{
		this.baseParticles = new List<ParticleSystem>(this.BaseParticlesTransform.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x0600150C RID: 5388 RVA: 0x000B9FC4 File Offset: 0x000B81C4
	private void Update()
	{
		this.StateMachine();
	}

	// Token: 0x0600150D RID: 5389 RVA: 0x000B9FCC File Offset: 0x000B81CC
	private void PlayBaseParticles(bool _play)
	{
		if (this.baseParticlesPlaying == _play)
		{
			return;
		}
		foreach (ParticleSystem particleSystem in this.baseParticles)
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
		this.baseParticlesPlaying = _play;
	}

	// Token: 0x0600150E RID: 5390 RVA: 0x000BA03C File Offset: 0x000B823C
	private void StateSet(SemiZuperFlames.State _state)
	{
		if (this.state == _state)
		{
			return;
		}
		this.state = _state;
		this.stateStart = true;
	}

	// Token: 0x0600150F RID: 5391 RVA: 0x000BA058 File Offset: 0x000B8258
	public void FlamesActive(Vector3 _position, Quaternion _direction)
	{
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.position = _position;
		base.transform.rotation = _direction;
		this.flamesActive = true;
	}

	// Token: 0x06001510 RID: 5392 RVA: 0x000BA0A4 File Offset: 0x000B82A4
	public void FlamesInactive()
	{
		this.flamesActive = false;
	}

	// Token: 0x04002456 RID: 9302
	public SemiZuperFlames.State state;

	// Token: 0x04002457 RID: 9303
	public Transform BaseParticlesTransform;

	// Token: 0x04002458 RID: 9304
	private bool flamesActive;

	// Token: 0x04002459 RID: 9305
	private bool stateStart;

	// Token: 0x0400245A RID: 9306
	private bool baseParticlesPlaying;

	// Token: 0x0400245B RID: 9307
	public Light flameLight;

	// Token: 0x0400245C RID: 9308
	public List<ParticleSystem> pukeParticles = new List<ParticleSystem>();

	// Token: 0x0400245D RID: 9309
	public ParticleSystem pukeEnd = new ParticleSystem();

	// Token: 0x0400245E RID: 9310
	private List<ParticleSystem> baseParticles = new List<ParticleSystem>();

	// Token: 0x0400245F RID: 9311
	public Sound soundPukeStart;

	// Token: 0x04002460 RID: 9312
	public Sound soundPukeEnd;

	// Token: 0x04002461 RID: 9313
	public Sound soundPukeLoop;

	// Token: 0x02000411 RID: 1041
	public enum State
	{
		// Token: 0x04002DAA RID: 11690
		None,
		// Token: 0x04002DAB RID: 11691
		FlamesStart,
		// Token: 0x04002DAC RID: 11692
		Flames,
		// Token: 0x04002DAD RID: 11693
		FlamesEnd
	}
}
