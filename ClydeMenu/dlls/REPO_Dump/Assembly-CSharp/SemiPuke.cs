using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200007A RID: 122
public class SemiPuke : MonoBehaviour
{
	// Token: 0x06000498 RID: 1176 RVA: 0x0002E041 File Offset: 0x0002C241
	private void StateNone()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.PlayBaseParticles(false);
		if (this.pukeActiveTimer > 0f)
		{
			this.StateSet(SemiPuke.State.PukeStart);
		}
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x0002E070 File Offset: 0x0002C270
	private void StatePukeStart()
	{
		if (this.stateStart)
		{
			this.soundPukeStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.pukeLight.enabled = true;
			this.pukeLight.intensity = 0f;
			this.PlayAllParticles(true);
			this.stateStart = false;
		}
		this.PlayBaseParticles(true);
		this.StateSet(SemiPuke.State.Puke);
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x0002E0E8 File Offset: 0x0002C2E8
	private void StatePuke()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.PlayBaseParticles(true);
		this.pukeLight.intensity = Mathf.Lerp(this.pukeLight.intensity, 1f, Time.deltaTime * 20f);
		this.pukeLight.intensity += Mathf.Sin(Time.time * 20f) * 0.05f;
		if (this.pukeActiveTimer <= 0f)
		{
			this.StateSet(SemiPuke.State.PukeEnd);
		}
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x0002E174 File Offset: 0x0002C374
	private void StatePukeEnd()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.soundPukeEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.pukeLight.enabled = false;
			this.PlayAllParticles(false);
			this.pukeEnd.Play();
		}
		this.PlayBaseParticles(false);
		this.pukeLight.intensity = Mathf.Lerp(this.pukeLight.intensity, 0f, Time.deltaTime * 40f);
		if (this.pukeLight.intensity < 0.01f)
		{
			this.pukeLight.intensity = 0f;
			this.StateSet(SemiPuke.State.None);
		}
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0002E234 File Offset: 0x0002C434
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

	// Token: 0x0600049D RID: 1181 RVA: 0x0002E294 File Offset: 0x0002C494
	private void StateMachine()
	{
		bool playing = this.pukeActiveTimer > 0f;
		this.soundPukeLoop.PlayLoop(playing, 2f, 2f, 1f);
		if (this.pukeActiveTimer > 0f)
		{
			this.pukeActiveTimer -= Time.deltaTime;
		}
		switch (this.state)
		{
		case SemiPuke.State.None:
			this.StateNone();
			return;
		case SemiPuke.State.PukeStart:
			this.StatePukeStart();
			return;
		case SemiPuke.State.Puke:
			this.StatePuke();
			return;
		case SemiPuke.State.PukeEnd:
			this.StatePukeEnd();
			return;
		default:
			return;
		}
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x0002E322 File Offset: 0x0002C522
	private void Start()
	{
		this.baseParticles = new List<ParticleSystem>(this.BaseParticlesTransform.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x0002E33A File Offset: 0x0002C53A
	private void Update()
	{
		this.StateMachine();
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x0002E344 File Offset: 0x0002C544
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

	// Token: 0x060004A1 RID: 1185 RVA: 0x0002E3B4 File Offset: 0x0002C5B4
	private void StateSet(SemiPuke.State _state)
	{
		if (this.state == _state)
		{
			return;
		}
		this.state = _state;
		this.stateStart = true;
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x0002E3D0 File Offset: 0x0002C5D0
	public void PukeActive(Vector3 _position, Quaternion _direction)
	{
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.position = _position;
		base.transform.rotation = _direction;
		this.pukeActiveTimer = 0.2f;
	}

	// Token: 0x04000788 RID: 1928
	public SemiPuke.State state;

	// Token: 0x04000789 RID: 1929
	public Transform BaseParticlesTransform;

	// Token: 0x0400078A RID: 1930
	private float pukeActiveTimer;

	// Token: 0x0400078B RID: 1931
	private bool stateStart;

	// Token: 0x0400078C RID: 1932
	private bool baseParticlesPlaying;

	// Token: 0x0400078D RID: 1933
	public Light pukeLight;

	// Token: 0x0400078E RID: 1934
	public List<ParticleSystem> pukeParticles = new List<ParticleSystem>();

	// Token: 0x0400078F RID: 1935
	public ParticleSystem pukeEnd = new ParticleSystem();

	// Token: 0x04000790 RID: 1936
	private List<ParticleSystem> baseParticles = new List<ParticleSystem>();

	// Token: 0x04000791 RID: 1937
	public Sound soundPukeStart;

	// Token: 0x04000792 RID: 1938
	public Sound soundPukeEnd;

	// Token: 0x04000793 RID: 1939
	public Sound soundPukeLoop;

	// Token: 0x0200031E RID: 798
	public enum State
	{
		// Token: 0x0400291D RID: 10525
		None,
		// Token: 0x0400291E RID: 10526
		PukeStart,
		// Token: 0x0400291F RID: 10527
		Puke,
		// Token: 0x04002920 RID: 10528
		PukeEnd
	}
}
