using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002EA RID: 746
	public class AudioEffectStateVariableFilter : MonoBehaviour
	{
		// Token: 0x06001774 RID: 6004 RVA: 0x000C90B1 File Offset: 0x000C72B1
		private void Awake()
		{
			this.sampleFrequency = (float)AudioSettings.outputSampleRate;
			this.UpdateFrequency();
			this.UpdateDamp();
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06001775 RID: 6005 RVA: 0x000C90CB File Offset: 0x000C72CB
		// (set) Token: 0x06001776 RID: 6006 RVA: 0x000C90D3 File Offset: 0x000C72D3
		public float Frequency
		{
			get
			{
				return this._frequency;
			}
			set
			{
				this._frequency = Mathf.Clamp(value, 50f, 12000f);
				this.UpdateFrequency();
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06001777 RID: 6007 RVA: 0x000C90F1 File Offset: 0x000C72F1
		// (set) Token: 0x06001778 RID: 6008 RVA: 0x000C90F9 File Offset: 0x000C72F9
		public float Resonance
		{
			get
			{
				return this._resonance;
			}
			set
			{
				this._resonance = Mathf.Clamp(value, 0f, 1f);
				this.UpdateDamp();
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06001779 RID: 6009 RVA: 0x000C9117 File Offset: 0x000C7317
		// (set) Token: 0x0600177A RID: 6010 RVA: 0x000C911F File Offset: 0x000C731F
		public float Drive
		{
			get
			{
				return this._drive;
			}
			set
			{
				this._drive = Mathf.Clamp(value, 0f, 0.1f);
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600177B RID: 6011 RVA: 0x000C9137 File Offset: 0x000C7337
		// (set) Token: 0x0600177C RID: 6012 RVA: 0x000C913F File Offset: 0x000C733F
		public float AdditiveGain
		{
			get
			{
				return this._additiveGain;
			}
			set
			{
				this._additiveGain = Mathf.Clamp(value, -1f, 1f);
			}
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x000C9157 File Offset: 0x000C7357
		private void UpdateFrequency()
		{
			this.freq = 2.0 * Math.Sin(3.141592653589793 * (double)this.Frequency / (double)(this.sampleFrequency * (float)this.passes));
			this.UpdateDamp();
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x000C9198 File Offset: 0x000C7398
		private void UpdateDamp()
		{
			this.damp = Math.Min(2.0 * (1.0 - Math.Pow((double)this.Resonance, 0.25)), Math.Min(2.0 - this.freq, 2.0 / this.freq - this.freq * 0.5));
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x000C9210 File Offset: 0x000C7410
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (this.Filter == FilterState.Bypass)
			{
				return;
			}
			double[] array = new double[channels];
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					array[j] = (double)(((double)Math.Abs(data[i + j]) > 1E-07) ? data[i + j] : 0f);
					this.output[j] = 0.0;
					for (int k = 0; k < this.passes; k++)
					{
						this.high[j] = array[j] - this.low[j] - this.damp * this.band[j];
						this.band[j] = this.freq * this.high[j] + this.band[j] - (double)(0.1f - this.Drive + 0.001f) * Math.Pow(this.band[j], 3.0);
						this.low[j] = this.freq * this.band[j] + this.low[j];
					}
					switch (this.Filter)
					{
					case FilterState.LowPass:
					case FilterState.LowShelf:
						this.output[j] = this.low[j];
						break;
					case FilterState.HighPass:
					case FilterState.HighShelf:
						this.output[j] = this.high[j];
						break;
					case FilterState.BandPass:
					case FilterState.BandAdd:
						this.output[j] = this.band[j];
						break;
					}
					if (this.Filter == FilterState.HighShelf || this.Filter == FilterState.LowShelf || this.Filter == FilterState.BandAdd)
					{
						data[i + j] += (float)this.output[j] * this.AdditiveGain;
					}
					else
					{
						data[i + j] = (float)this.output[j];
					}
				}
			}
		}

		// Token: 0x040027A6 RID: 10150
		private float sampleFrequency;

		// Token: 0x040027A7 RID: 10151
		private int passes = 2;

		// Token: 0x040027A8 RID: 10152
		[SerializeField]
		[Range(50f, 12000f)]
		private float _frequency = 440f;

		// Token: 0x040027A9 RID: 10153
		private double freq;

		// Token: 0x040027AA RID: 10154
		[SerializeField]
		[Range(0f, 1f)]
		private float _resonance = 0.5f;

		// Token: 0x040027AB RID: 10155
		[SerializeField]
		[Range(0f, 0.1f)]
		private float _drive = 0.1f;

		// Token: 0x040027AC RID: 10156
		public FilterState Filter = FilterState.BandPass;

		// Token: 0x040027AD RID: 10157
		[SerializeField]
		[Range(-1f, 1f)]
		private float _additiveGain = 0.25f;

		// Token: 0x040027AE RID: 10158
		private double[] notch = new double[2];

		// Token: 0x040027AF RID: 10159
		private double[] low = new double[2];

		// Token: 0x040027B0 RID: 10160
		private double[] high = new double[2];

		// Token: 0x040027B1 RID: 10161
		private double[] band = new double[2];

		// Token: 0x040027B2 RID: 10162
		private double[] output = new double[2];

		// Token: 0x040027B3 RID: 10163
		private double damp;
	}
}
