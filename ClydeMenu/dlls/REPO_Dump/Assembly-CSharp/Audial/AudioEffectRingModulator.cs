using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002E6 RID: 742
	public class AudioEffectRingModulator : MonoBehaviour
	{
		// Token: 0x0600175A RID: 5978 RVA: 0x000C8CB2 File Offset: 0x000C6EB2
		private void OnEnable()
		{
			this.sampleRate = (Settings.SampleRate = (float)AudioSettings.outputSampleRate);
			this.ResetUtils();
		}

		// Token: 0x0600175B RID: 5979 RVA: 0x000C8CCC File Offset: 0x000C6ECC
		private void ResetUtils()
		{
			this.carrierLFO = new LFO(1f / this.CarrierFrequency);
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600175C RID: 5980 RVA: 0x000C8CE5 File Offset: 0x000C6EE5
		// (set) Token: 0x0600175D RID: 5981 RVA: 0x000C8CED File Offset: 0x000C6EED
		public float CarrierFrequency
		{
			get
			{
				return this._carrierFrequency;
			}
			set
			{
				this._carrierFrequency = Mathf.Clamp(value, 20f, 5000f);
				this.carrierLFO.SetRate(1f / this._carrierFrequency);
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600175E RID: 5982 RVA: 0x000C8D1C File Offset: 0x000C6F1C
		// (set) Token: 0x0600175F RID: 5983 RVA: 0x000C8D24 File Offset: 0x000C6F24
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

		// Token: 0x06001760 RID: 5984 RVA: 0x000C8D3C File Offset: 0x000C6F3C
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

		// Token: 0x0400278A RID: 10122
		private float output;

		// Token: 0x0400278B RID: 10123
		private float sampleRate;

		// Token: 0x0400278C RID: 10124
		public LFO carrierLFO;

		// Token: 0x0400278D RID: 10125
		[SerializeField]
		private float _carrierFrequency = 400f;

		// Token: 0x0400278E RID: 10126
		[SerializeField]
		private float _dryWet = 0.75f;
	}
}
