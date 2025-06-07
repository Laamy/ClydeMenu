using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002E0 RID: 736
	[Serializable]
	public class AudioEffectFlanger : MonoBehaviour
	{
		// Token: 0x06001716 RID: 5910 RVA: 0x000C7FF9 File Offset: 0x000C61F9
		private void Awake()
		{
			this.sampleRate = (Settings.SampleRate = (float)AudioSettings.outputSampleRate);
			this.ResetUtils();
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x000C8013 File Offset: 0x000C6213
		private void ResetUtils()
		{
			this.combFilter = new CombFilter(this.Intensity, 0.5f);
			this.lfo = new LFO(this.Rate);
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06001718 RID: 5912 RVA: 0x000C803C File Offset: 0x000C623C
		// (set) Token: 0x06001719 RID: 5913 RVA: 0x000C8044 File Offset: 0x000C6244
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600171A RID: 5914 RVA: 0x000C8077 File Offset: 0x000C6277
		// (set) Token: 0x0600171B RID: 5915 RVA: 0x000C807F File Offset: 0x000C627F
		public float Intensity
		{
			get
			{
				return this._intensity;
			}
			set
			{
				this._intensity = Mathf.Clamp(value, 0.1f, 0.9f);
				this.combFilter.gain = this._intensity;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600171C RID: 5916 RVA: 0x000C80A8 File Offset: 0x000C62A8
		// (set) Token: 0x0600171D RID: 5917 RVA: 0x000C80B0 File Offset: 0x000C62B0
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

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600171E RID: 5918 RVA: 0x000C80C8 File Offset: 0x000C62C8
		// (set) Token: 0x0600171F RID: 5919 RVA: 0x000C80D1 File Offset: 0x000C62D1
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

		// Token: 0x06001720 RID: 5920 RVA: 0x000C80DC File Offset: 0x000C62DC
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				this.combFilter.Offset = (int)Mathf.Lerp(1f * this.sampleRate / 1000f, 5f * this.sampleRate / 1000f, this.lfo.GetValue());
				for (int j = 0; j < channels; j++)
				{
					float num = data[i + j];
					float num2 = this.combFilter.ProcessSample(j, num);
					this.output = num * (1f - this.DryWet / 2f) + num2 * this.DryWet / 2f;
					data[i + j] = this.output;
				}
				this.combFilter.MoveIndex();
				this.lfo.MoveIndex();
			}
		}

		// Token: 0x04002761 RID: 10081
		private float sampleRate;

		// Token: 0x04002762 RID: 10082
		private float output;

		// Token: 0x04002763 RID: 10083
		private LFO lfo;

		// Token: 0x04002764 RID: 10084
		[SerializeField]
		private CombFilter combFilter;

		// Token: 0x04002765 RID: 10085
		[SerializeField]
		private float _rate = 0.3f;

		// Token: 0x04002766 RID: 10086
		[SerializeField]
		private float _intensity = 0.25f;

		// Token: 0x04002767 RID: 10087
		[SerializeField]
		[Range(0f, 1f)]
		private float _dryWet = 0.75f;

		// Token: 0x04002768 RID: 10088
		private float _offset;
	}
}
