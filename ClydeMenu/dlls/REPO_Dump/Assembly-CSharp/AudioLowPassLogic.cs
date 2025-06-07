using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200000E RID: 14
[RequireComponent(typeof(AudioLowPassFilter))]
public class AudioLowPassLogic : MonoBehaviour
{
	// Token: 0x06000031 RID: 49 RVA: 0x00002C36 File Offset: 0x00000E36
	private void Start()
	{
		if (this.ForceStart)
		{
			this.Setup();
		}
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00002C48 File Offset: 0x00000E48
	public void Setup()
	{
		if (this.Fetch)
		{
			this.audioListener = AudioListenerFollow.instance.transform;
			this.AudioLowpassFilter = base.GetComponent<AudioLowPassFilter>();
			this.AudioSource = base.GetComponent<AudioSource>();
			this.LowPassMin = AudioManager.instance.lowpassValueMin;
			this.LowPassMax = AudioManager.instance.lowpassValueMax;
			if (this.HasCustomFalloff)
			{
				this.FalloffMultiplier = this.CustomFalloff;
			}
			this.Falloff = this.AudioSource.maxDistance;
			this.LayerMask = LayerMask.GetMask(new string[]
			{
				"Default",
				"PhysGrabObject",
				"PhysGrabObjectHinge"
			});
			if (!this.volumeFetched)
			{
				if (this.HasCustomVolume)
				{
					this.VolumeMultiplier = this.CustomVolume;
				}
				this.volumeFetched = true;
				this.Volume = this.AudioSource.volume;
			}
			this.Fetch = false;
		}
		this.CheckStart();
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00002D3C File Offset: 0x00000F3C
	private void Update()
	{
		if (this.LogicActive)
		{
			if (this.LowPass)
			{
				if (this.AudioLowpassFilter.cutoffFrequency != this.LowPassMin || this.AudioSource.maxDistance != this.Falloff * this.FalloffMultiplier || Mathf.Abs(this.AudioSource.volume - this.Volume * this.VolumeMultiplier) > 0.001f)
				{
					this.AudioLowpassFilter.cutoffFrequency -= (this.LowPassMax - this.LowPassMin) * 10f * Time.deltaTime;
					this.AudioLowpassFilter.cutoffFrequency = Mathf.Clamp(this.AudioLowpassFilter.cutoffFrequency, this.LowPassMin, this.LowPassMax);
					float t = (this.AudioLowpassFilter.cutoffFrequency - this.LowPassMin) / (this.LowPassMax - this.LowPassMin);
					this.AudioSource.maxDistance = Mathf.Lerp(this.Falloff * this.FalloffMultiplier, this.Falloff, t);
					this.AudioSource.volume = Mathf.Lerp(this.Volume * this.VolumeMultiplier, this.Volume, t);
				}
			}
			else if (this.AudioLowpassFilter.cutoffFrequency != this.LowPassMax || this.AudioSource.maxDistance != this.Falloff || Mathf.Abs(this.AudioSource.volume - this.Volume) > 0.001f)
			{
				this.AudioLowpassFilter.cutoffFrequency += (this.LowPassMax - this.LowPassMin) * 1f * Time.deltaTime;
				this.AudioLowpassFilter.cutoffFrequency = Mathf.Clamp(this.AudioLowpassFilter.cutoffFrequency, this.LowPassMin, this.LowPassMax);
				float t2 = (this.AudioLowpassFilter.cutoffFrequency - this.LowPassMin) / (this.LowPassMax - this.LowPassMin);
				this.AudioSource.maxDistance = Mathf.Lerp(this.Falloff * this.FalloffMultiplier, this.Falloff, t2);
				this.AudioSource.volume = Mathf.Lerp(this.Volume * this.VolumeMultiplier, this.Volume, t2);
			}
			this.First = false;
		}
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00002F7D File Offset: 0x0000117D
	private void OnEnable()
	{
		if (!this.Fetch)
		{
			this.CheckStart();
		}
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00002F8D File Offset: 0x0000118D
	private void OnDisable()
	{
		this.LogicActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00002F9C File Offset: 0x0000119C
	private void CheckStart()
	{
		if (!this.LogicActive)
		{
			this.First = true;
			if (base.gameObject.activeSelf)
			{
				base.StartCoroutine(this.Check());
			}
		}
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00002FC7 File Offset: 0x000011C7
	private IEnumerator Check()
	{
		this.LogicActive = true;
		while (this.AudioSource && (this.AlwaysActive || !this.AudioSource.loop || this.AudioSource.isPlaying || this.First) && !this.Fetch)
		{
			if (!this.audioListener)
			{
				if (!AudioListenerFollow.instance)
				{
					yield return null;
					continue;
				}
				this.audioListener = AudioListenerFollow.instance.transform;
			}
			this.CheckLogic();
			yield return new WaitForSeconds(0.25f);
		}
		this.LogicActive = false;
		this.First = true;
		yield break;
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00002FD8 File Offset: 0x000011D8
	private void CheckLogic()
	{
		this.LowPass = true;
		bool flag = SpectateCamera.instance;
		if (!this.audioListener || !this.AudioSource || this.AudioSource.spatialBlend <= 0f || (flag && SpectateCamera.instance.CheckState(SpectateCamera.State.Death)))
		{
			this.LowPass = false;
		}
		else
		{
			Vector3 direction = this.audioListener.position - base.transform.position;
			if (direction.magnitude < 20f)
			{
				this.LowPass = false;
				Collider[] array = Physics.OverlapSphere(base.transform.position, 0.1f, this.LayerMask, QueryTriggerInteraction.Collide);
				List<Collider> list = new List<Collider>();
				foreach (Collider collider in array)
				{
					if (collider.transform.CompareTag("Wall"))
					{
						list.Add(collider);
					}
				}
				foreach (RaycastHit raycastHit in Physics.RaycastAll(base.transform.position, direction, direction.magnitude, this.LayerMask, QueryTriggerInteraction.Collide))
				{
					if (raycastHit.collider.transform.CompareTag("Wall"))
					{
						bool flag2 = true;
						using (List<Collider>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.transform == raycastHit.collider.transform)
								{
									flag2 = false;
									break;
								}
							}
						}
						if (flag2)
						{
							bool flag3 = false;
							if (this.LowPassIgnoreColliders.Count > 0)
							{
								foreach (Collider collider2 in this.LowPassIgnoreColliders)
								{
									if (collider2 && collider2.transform == raycastHit.collider.transform)
									{
										flag3 = true;
										break;
									}
								}
							}
							if (!flag3)
							{
								this.LowPass = true;
								break;
							}
						}
					}
				}
			}
		}
		if (this.First)
		{
			if (this.AudioSource)
			{
				if (this.LowPass)
				{
					this.AudioLowpassFilter.cutoffFrequency = this.LowPassMin;
					this.AudioSource.maxDistance = this.Falloff * this.FalloffMultiplier;
					this.AudioSource.volume = this.Volume * this.VolumeMultiplier;
				}
				else
				{
					this.AudioLowpassFilter.cutoffFrequency = this.LowPassMax;
					this.AudioSource.maxDistance = this.Falloff;
					this.AudioSource.volume = this.Volume;
				}
			}
			this.First = false;
		}
	}

	// Token: 0x0400004A RID: 74
	public bool LowPass;

	// Token: 0x0400004B RID: 75
	[Space]
	public bool ForceStart;

	// Token: 0x0400004C RID: 76
	public bool AlwaysActive;

	// Token: 0x0400004D RID: 77
	[Space]
	public bool HasCustomVolume;

	// Token: 0x0400004E RID: 78
	[Range(0f, 1f)]
	public float CustomVolume = 0.5f;

	// Token: 0x0400004F RID: 79
	private float VolumeMultiplier = 0.5f;

	// Token: 0x04000050 RID: 80
	[Space]
	public bool HasCustomFalloff;

	// Token: 0x04000051 RID: 81
	[Range(0f, 1f)]
	public float CustomFalloff = 0.8f;

	// Token: 0x04000052 RID: 82
	private float FalloffMultiplier = 0.8f;

	// Token: 0x04000053 RID: 83
	internal bool Fetch = true;

	// Token: 0x04000054 RID: 84
	private bool First = true;

	// Token: 0x04000055 RID: 85
	private bool LogicActive;

	// Token: 0x04000056 RID: 86
	internal float Falloff;

	// Token: 0x04000057 RID: 87
	private float LowPassMin;

	// Token: 0x04000058 RID: 88
	private float LowPassMax;

	// Token: 0x04000059 RID: 89
	private AudioLowPassFilter AudioLowpassFilter;

	// Token: 0x0400005A RID: 90
	private AudioSource AudioSource;

	// Token: 0x0400005B RID: 91
	private LayerMask LayerMask;

	// Token: 0x0400005C RID: 92
	internal bool volumeFetched;

	// Token: 0x0400005D RID: 93
	internal float Volume;

	// Token: 0x0400005E RID: 94
	internal List<Collider> LowPassIgnoreColliders = new List<Collider>();

	// Token: 0x0400005F RID: 95
	private Transform audioListener;
}
