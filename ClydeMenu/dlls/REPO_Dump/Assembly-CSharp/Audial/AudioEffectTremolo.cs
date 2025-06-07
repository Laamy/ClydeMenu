using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002EC RID: 748
	public class AudioEffectTremolo : MonoBehaviour
	{
		// Token: 0x06001785 RID: 6021 RVA: 0x000C94EE File Offset: 0x000C76EE
		private void Awake()
		{
			this.sampleRate = (Settings.SampleRate = (float)AudioSettings.outputSampleRate);
			this.ResetUtils();
		}

		// Token: 0x06001786 RID: 6022 RVA: 0x000C9508 File Offset: 0x000C7708
		private void ResetUtils()
		{
			this.carrierLFO = new LFO(1f / this.CarrierFrequency);
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06001787 RID: 6023 RVA: 0x000C9521 File Offset: 0x000C7721
		// (set) Token: 0x06001788 RID: 6024 RVA: 0x000C9529 File Offset: 0x000C7729
		public float CarrierFrequency
		{
			get
			{
				return this._carrierFrequency;
			}
			set
			{
				this._carrierFrequency = Mathf.Clamp(value, 2f, 20f);
				this.carrierLFO.SetRate(1f / this._carrierFrequency);
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06001789 RID: 6025 RVA: 0x000C9558 File Offset: 0x000C7758
		// (set) Token: 0x0600178A RID: 6026 RVA: 0x000C9560 File Offset: 0x000C7760
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

		// Token: 0x0600178B RID: 6027 RVA: 0x000C9578 File Offset: 0x000C7778
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					float num = data[i + j];
					float num2 = num * this.carrierLFO.GetValue();
					data[i + j] = num * (1f - this.DryWet) + num2 * this.DryWet;
				}
				this.carrierLFO.MoveIndex();
			}
		}

		// Token: 0x040027B5 RID: 10165
		private float output;

		// Token: 0x040027B6 RID: 10166
		private float sampleRate;

		// Token: 0x040027B7 RID: 10167
		public LFO carrierLFO;

		// Token: 0x040027B8 RID: 10168
		[SerializeField]
		private float _carrierFrequency = 10f;

		// Token: 0x040027B9 RID: 10169
		[SerializeField]
		private float _dryWet = 0.75f;
	}
}
