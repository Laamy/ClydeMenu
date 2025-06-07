using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002DE RID: 734
	public class AudioEffectDistortion : MonoBehaviour
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06001708 RID: 5896 RVA: 0x000C7E34 File Offset: 0x000C6034
		// (set) Token: 0x06001709 RID: 5897 RVA: 0x000C7E3C File Offset: 0x000C603C
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

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600170A RID: 5898 RVA: 0x000C7E54 File Offset: 0x000C6054
		// (set) Token: 0x0600170B RID: 5899 RVA: 0x000C7E5C File Offset: 0x000C605C
		public float Threshold
		{
			get
			{
				return this._threshold;
			}
			set
			{
				this._threshold = Mathf.Clamp(value, 1E-05f, 1f);
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600170C RID: 5900 RVA: 0x000C7E74 File Offset: 0x000C6074
		// (set) Token: 0x0600170D RID: 5901 RVA: 0x000C7E7C File Offset: 0x000C607C
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

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600170E RID: 5902 RVA: 0x000C7E94 File Offset: 0x000C6094
		// (set) Token: 0x0600170F RID: 5903 RVA: 0x000C7E9C File Offset: 0x000C609C
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

		// Token: 0x06001710 RID: 5904 RVA: 0x000C7EB4 File Offset: 0x000C60B4
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					data[i + j] *= this.InputGain;
					float num = data[i + j];
					if (Mathf.Abs(num) > this.Threshold)
					{
						num = Mathf.Sign(num);
					}
					data[i + j] = (1f - this.DryWet) * data[i + j] + this.DryWet * num;
					data[i + j] *= this.OutputGain;
				}
			}
		}

		// Token: 0x0400275A RID: 10074
		[SerializeField]
		[Range(0f, 3f)]
		private float _inputGain = 1f;

		// Token: 0x0400275B RID: 10075
		[SerializeField]
		[Range(1E-05f, 1f)]
		private float _threshold = 0.036f;

		// Token: 0x0400275C RID: 10076
		[SerializeField]
		[Range(0f, 1f)]
		private float _dryWet = 0.258f;

		// Token: 0x0400275D RID: 10077
		[SerializeField]
		[Range(0f, 5f)]
		private float _outputGain = 1f;
	}
}
