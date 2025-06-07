using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002DC RID: 732
	public class AudioEffectCrusher : MonoBehaviour
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060016EE RID: 5870 RVA: 0x000C78E0 File Offset: 0x000C5AE0
		// (set) Token: 0x060016EF RID: 5871 RVA: 0x000C78E8 File Offset: 0x000C5AE8
		public int BitDepth
		{
			get
			{
				return this._bitDepth;
			}
			set
			{
				if (value == this._bitDepth)
				{
					return;
				}
				this._bitDepth = Mathf.Clamp(value, 1, 32);
				this.Callibrate();
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060016F0 RID: 5872 RVA: 0x000C7909 File Offset: 0x000C5B09
		// (set) Token: 0x060016F1 RID: 5873 RVA: 0x000C7911 File Offset: 0x000C5B11
		public float SampleRate
		{
			get
			{
				return this._sampleRate;
			}
			set
			{
				if (value == this._sampleRate)
				{
					return;
				}
				this._sampleRate = Mathf.Clamp(value, 0.001f, 1f);
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060016F2 RID: 5874 RVA: 0x000C7933 File Offset: 0x000C5B33
		// (set) Token: 0x060016F3 RID: 5875 RVA: 0x000C793B File Offset: 0x000C5B3B
		public float DryWet
		{
			get
			{
				return this._dryWet;
			}
			set
			{
				if (value == this._dryWet)
				{
					return;
				}
				this._dryWet = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x060016F4 RID: 5876 RVA: 0x000C795D File Offset: 0x000C5B5D
		private void Awake()
		{
			this.y = new float[2];
			this.Callibrate();
		}

		// Token: 0x060016F5 RID: 5877 RVA: 0x000C7971 File Offset: 0x000C5B71
		private void Callibrate()
		{
			this.m = 1L << (this._bitDepth - 1 & 31);
			this.m = ((this.m < 0L) ? 2147483647L : this.m);
		}

		// Token: 0x060016F6 RID: 5878 RVA: 0x000C79A8 File Offset: 0x000C5BA8
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				this.cnt += this.SampleRate;
				if (this.cnt >= 1f)
				{
					this.cnt -= 1f;
					for (int j = 0; j < channels; j++)
					{
						this.y[j] = (float)((long)(data[i + j] * (float)this.m)) / (float)this.m;
					}
				}
				for (int k = 0; k < channels; k++)
				{
					float num = this.y[k];
					data[i + k] = data[i + k] * (1f - this.DryWet) + num * this.DryWet;
				}
			}
		}

		// Token: 0x04002746 RID: 10054
		[SerializeField]
		[HideInInspector]
		private int _bitDepth = 8;

		// Token: 0x04002747 RID: 10055
		private long m;

		// Token: 0x04002748 RID: 10056
		[SerializeField]
		[Range(0.001f, 1f)]
		private float _sampleRate = 0.1f;

		// Token: 0x04002749 RID: 10057
		[SerializeField]
		[Range(0f, 1f)]
		private float _dryWet = 1f;

		// Token: 0x0400274A RID: 10058
		private float[] y;

		// Token: 0x0400274B RID: 10059
		private float cnt;

		// Token: 0x0400274C RID: 10060
		private LFO lfo;
	}
}
