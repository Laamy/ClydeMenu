using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200000C RID: 12
public class AudioLoopDistance : MonoBehaviour
{
	// Token: 0x0600002A RID: 42 RVA: 0x00002B10 File Offset: 0x00000D10
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioLowPassLogic = base.GetComponent<AudioLowPassLogic>();
		this.audioLowPassLogic.Setup();
		this.volumeDefault = this.audioSource.volume;
		this.audioSource.volume = 0f;
		foreach (AudioLoopDistanceParticle audioLoopDistanceParticle in base.GetComponentsInChildren<AudioLoopDistanceParticle>())
		{
			bool flag = false;
			ParticleSystem[] array = this.particles;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j].transform == audioLoopDistanceParticle.transform)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Debug.LogError("Particle not hooked up to Audio: " + audioLoopDistanceParticle.name, audioLoopDistanceParticle.transform);
			}
		}
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002BDD File Offset: 0x00000DDD
	private void Start()
	{
		AudioManager.instance.audioLoopDistances.Add(this);
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00002BEF File Offset: 0x00000DEF
	private void OnDestroy()
	{
		AudioManager.instance.audioLoopDistances.Remove(this);
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00002C02 File Offset: 0x00000E02
	public void Restart()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002C17 File Offset: 0x00000E17
	private IEnumerator Logic()
	{
		yield return new WaitForSeconds(0.1f);
		for (;;)
		{
			float _distance = Vector3.Distance(AudioManager.instance.AudioListener.transform.position, base.transform.position);
			if (_distance < this.audioSource.maxDistance + 5f)
			{
				if (!this.audioSource.isPlaying)
				{
					this.audioSource.time = Random.Range(0f, this.audioSource.clip.length);
					this.audioSource.Play();
					this.audioLowPassLogic.Setup();
					ParticleSystem[] array = this.particles;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Play();
					}
				}
				while (this.audioLowPassLogic.Volume < this.volumeDefault)
				{
					this.audioLowPassLogic.Volume += Time.deltaTime;
					yield return null;
				}
			}
			else if (this.audioSource.isPlaying)
			{
				while (this.audioLowPassLogic.Volume > 0f)
				{
					this.audioLowPassLogic.Volume -= Time.deltaTime;
					yield return null;
				}
				this.audioSource.Stop();
				ParticleSystem[] array = this.particles;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Stop();
				}
			}
			if (Mathf.Abs(this.audioSource.maxDistance - _distance) > 20f)
			{
				yield return new WaitForSeconds(Random.Range(3f, 6f));
			}
			else
			{
				yield return new WaitForSeconds(Random.Range(0.5f, 2f));
			}
		}
		yield break;
	}

	// Token: 0x04000046 RID: 70
	private AudioSource audioSource;

	// Token: 0x04000047 RID: 71
	private AudioLowPassLogic audioLowPassLogic;

	// Token: 0x04000048 RID: 72
	private float volumeDefault;

	// Token: 0x04000049 RID: 73
	public ParticleSystem[] particles;
}
