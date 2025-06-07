using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002E4 RID: 740
	public class AudioEffectPhaser : MonoBehaviour
	{
		// Token: 0x06001740 RID: 5952 RVA: 0x000C8680 File Offset: 0x000C6880
		private void Awake()
		{
			this.sampleRate = (Settings.SampleRate = (float)AudioSettings.outputSampleRate);
			this.ResetUtils();
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x000C869C File Offset: 0x000C689C
		private void ResetUtils()
		{
			this.allPassFilters[0] = new AllPassFilter(this.Rate, this.Intensity);
			this.allPassFilters[1] = new AllPassFilter(this.Rate, this.Intensity);
			this.allPassFilters[2] = new AllPassFilter(this.Rate, this.Intensity);
			this.allPassFilters[3] = new AllPassFilter(this.Rate, this.Intensity);
			this.SetIntensity(this.Intensity);
			this.lfo = new LFO(this.Rate);
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x000C872C File Offset: 0x000C692C
		private void SetIntensity(float i)
		{
			for (int j = 0; j < this.allPassFilters.Length; j++)
			{
				this.allPassFilters[j].gain = i * 0.6f;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06001743 RID: 5955 RVA: 0x000C8760 File Offset: 0x000C6960
		// (set) Token: 0x06001744 RID: 5956 RVA: 0x000C8768 File Offset: 0x000C6968
		public float Rate
		{
			get
			{
				return this._rate;
			}
			set
			{
				if (this._rate == value)
				{
					return;
				}
				this._rate = Mathf.Clamp(value, 0.1f, 8f);
				this.lfo.SetRate(this._rate);
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06001745 RID: 5957 RVA: 0x000C879B File Offset: 0x000C699B
		// (set) Token: 0x06001746 RID: 5958 RVA: 0x000C87A3 File Offset: 0x000C69A3
		public float Width
		{
			get
			{
				return this._width;
			}
			set
			{
				if (this._width == value)
				{
					return;
				}
				this._width = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06001747 RID: 5959 RVA: 0x000C87C5 File Offset: 0x000C69C5
		// (set) Token: 0x06001748 RID: 5960 RVA: 0x000C87CD File Offset: 0x000C69CD
		public float Intensity
		{
			get
			{
				return this._intensity;
			}
			set
			{
				this._intensity = Mathf.Clamp(value, 0f, 1f);
				this.SetIntensity(this._intensity);
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06001749 RID: 5961 RVA: 0x000C87F1 File Offset: 0x000C69F1
		// (set) Token: 0x0600174A RID: 5962 RVA: 0x000C87F9 File Offset: 0x000C69F9
		public float DryWet
		{
			get
			{
				return this._dryWet;
			}
			set
			{
				this._dryWet = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600174B RID: 5963 RVA: 0x000C8811 File Offset: 0x000C6A11
		// (set) Token: 0x0600174C RID: 5964 RVA: 0x000C881A File Offset: 0x000C6A1A
		private int Offset
		{
			get
			{
				return (int)this._offset;
			}
			set
			{
				this._offset = (float)value;
			}
		}

		// Token: 0x0600174D RID: 5965 RVA: 0x000C8824 File Offset: 0x000C6A24
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				float num = Mathf.Lerp(Mathf.Lerp(this.fromMin, this.fromMax, this.Width), Mathf.Lerp(this.toMin, this.toMax, this.Width), this.lfo.GetValue()) * this.sampleRate / 1000f;
				for (int j = 0; j < this.allPassFilters.Length; j++)
				{
					this.allPassFilters[j].Offset = (int)num;
					for (int k = 0; k < channels; k++)
					{
						float num2 = data[i + k];
						float num3 = this.allPassFilters[j].ProcessSample(k, num2);
						this.output = num2 * (1f - this.DryWet / 2f) + num3 * this.DryWet / 2f;
						data[i + k] = this.output;
					}
					this.allPassFilters[j].MoveIndex();
				}
				this.lfo.MoveIndex();
			}
		}

		// Token: 0x04002778 RID: 10104
		private float output;

		// Token: 0x04002779 RID: 10105
		private float sampleRate;

		// Token: 0x0400277A RID: 10106
		public LFO lfo;

		// Token: 0x0400277B RID: 10107
		public AllPassFilter[] allPassFilters = new AllPassFilter[4];

		// Token: 0x0400277C RID: 10108
		[SerializeField]
		private float _rate = 0.3f;

		// Token: 0x0400277D RID: 10109
		[SerializeField]
		private float _width = 0.5f;

		// Token: 0x0400277E RID: 10110
		[SerializeField]
		private float _intensity = 0.25f;

		// Token: 0x0400277F RID: 10111
		[SerializeField]
		private float _dryWet = 0.75f;

		// Token: 0x04002780 RID: 10112
		private float _offset;

		// Token: 0x04002781 RID: 10113
		private float fromMin = 0.43f;

		// Token: 0x04002782 RID: 10114
		private float fromMax = 0.193f;

		// Token: 0x04002783 RID: 10115
		private float toMin = 0.772f;

		// Token: 0x04002784 RID: 10116
		private float toMax = 0.962f;
	}
}
