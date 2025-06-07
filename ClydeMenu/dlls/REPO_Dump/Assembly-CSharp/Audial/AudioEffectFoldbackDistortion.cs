using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002E1 RID: 737
	public class AudioEffectFoldbackDistortion : MonoBehaviour
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06001722 RID: 5922 RVA: 0x000C81D0 File Offset: 0x000C63D0
		// (set) Token: 0x06001723 RID: 5923 RVA: 0x000C81D8 File Offset: 0x000C63D8
		public float InputGain
		{
			get
			{
				return this._inputGain;
			}
			set
			{
				this._inputGain = Mathf.Clamp(value, 0f, 3f);
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06001724 RID: 5924 RVA: 0x000C81F0 File Offset: 0x000C63F0
		// (set) Token: 0x06001725 RID: 5925 RVA: 0x000C81F8 File Offset: 0x000C63F8
		public float SoftDistortAmount
		{
			get
			{
				return this._softDistortAmount;
			}
			set
			{
				this._softDistortAmount = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06001726 RID: 5926 RVA: 0x000C8210 File Offset: 0x000C6410
		// (set) Token: 0x06001727 RID: 5927 RVA: 0x000C8218 File Offset: 0x000C6418
		public float Threshold
		{
			get
			{
				return this._threshold;
			}
			set
			{
				this._threshold = Mathf.Clamp(value, 1E-06f, 1f);
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06001728 RID: 5928 RVA: 0x000C8230 File Offset: 0x000C6430
		// (set) Token: 0x06001729 RID: 5929 RVA: 0x000C8238 File Offset: 0x000C6438
		public float DistortAmount
		{
			get
			{
				return this._distortAmount;
			}
			set
			{
				this._distortAmount = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600172A RID: 5930 RVA: 0x000C8250 File Offset: 0x000C6450
		// (set) Token: 0x0600172B RID: 5931 RVA: 0x000C8258 File Offset: 0x000C6458
		public float OutputGain
		{
			get
			{
				return this._outputGain;
			}
			set
			{
				this._outputGain = Mathf.Clamp(value, 0f, 5f);
			}
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x000C8270 File Offset: 0x000C6470
		private float foldBack(float sample, float threshold)
		{
			if (Mathf.Abs(sample) > this.Threshold)
			{
				return Mathf.Abs(Mathf.Abs(sample - this.Threshold % (this.Threshold * 4f)) - this.Threshold * 2f) - this.Threshold + 0.3f * sample;
			}
			return sample;
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x000C82C8 File Offset: 0x000C64C8
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					data[i + j] *= this.InputGain;
					float num = this.foldBack(data[i + j], this.softThreshold);
					data[i + j] = (1f - this.SoftDistortAmount) * data[i + j] + this.SoftDistortAmount * num;
					data[i + j] *= this.OutputGain;
					float num2 = this.foldBack(data[i + j], this.Threshold);
					data[i + j] = (1f - this.DistortAmount) * data[i + j] + this.DistortAmount * num2;
					data[i + j] *= this.OutputGain;
				}
			}
		}

		// Token: 0x04002769 RID: 10089
		[SerializeField]
		[Range(0f, 3f)]
		private float _inputGain = 1.14f;

		// Token: 0x0400276A RID: 10090
		[SerializeField]
		[Range(0f, 1f)]
		private float _softDistortAmount = 0.177f;

		// Token: 0x0400276B RID: 10091
		private float softThreshold = 0.002f;

		// Token: 0x0400276C RID: 10092
		[SerializeField]
		[Range(1E-06f, 1f)]
		private float _threshold = 0.244f;

		// Token: 0x0400276D RID: 10093
		[SerializeField]
		[Range(0f, 1f)]
		private float _distortAmount = 0.904f;

		// Token: 0x0400276E RID: 10094
		[SerializeField]
		[Range(0f, 5f)]
		private float _outputGain = 1f;
	}
}
