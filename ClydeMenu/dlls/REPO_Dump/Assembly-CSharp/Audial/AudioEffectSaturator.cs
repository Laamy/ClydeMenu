using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002E7 RID: 743
	public class AudioEffectSaturator : MonoBehaviour
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06001762 RID: 5986 RVA: 0x000C8DBC File Offset: 0x000C6FBC
		// (set) Token: 0x06001763 RID: 5987 RVA: 0x000C8DC4 File Offset: 0x000C6FC4
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

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06001764 RID: 5988 RVA: 0x000C8DDC File Offset: 0x000C6FDC
		// (set) Token: 0x06001765 RID: 5989 RVA: 0x000C8DE4 File Offset: 0x000C6FE4
		public float Threshold
		{
			get
			{
				return this._threshold;
			}
			set
			{
				this._threshold = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06001766 RID: 5990 RVA: 0x000C8DFC File Offset: 0x000C6FFC
		// (set) Token: 0x06001767 RID: 5991 RVA: 0x000C8E04 File Offset: 0x000C7004
		public float Amount
		{
			get
			{
				return this._amount;
			}
			set
			{
				this._amount = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x000C8E1C File Offset: 0x000C701C
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (this.Amount == 0f)
			{
				return;
			}
			for (int i = 0; i < channels; i++)
			{
				for (int j = 0; j < data.Length; j += channels)
				{
					this.input = data[j + i] * this.InputGain;
					this.sampleAbs = Mathf.Abs(this.input);
					this.sampleSign = Mathf.Sign(this.input);
					if (this.sampleAbs > 1f)
					{
						this.input = (this.Threshold + 1f) / 2f * this.sampleSign;
					}
					else if (this.sampleAbs > this.Threshold)
					{
						this.input = (this.Threshold + (this.sampleAbs - this.Threshold) / (1f + Mathf.Pow((this.sampleAbs - this.Threshold) / (1f - this.Amount), 2f))) * this.sampleSign;
					}
					data[j + i] = this.input;
				}
			}
		}

		// Token: 0x0400278F RID: 10127
		[SerializeField]
		[Range(0f, 3f)]
		private float _inputGain = 1f;

		// Token: 0x04002790 RID: 10128
		[SerializeField]
		[Range(0f, 1f)]
		private float _threshold = 0.247f;

		// Token: 0x04002791 RID: 10129
		[SerializeField]
		[Range(0f, 1f)]
		public float _amount = 0.5f;

		// Token: 0x04002792 RID: 10130
		private float input;

		// Token: 0x04002793 RID: 10131
		private float sampleAbs;

		// Token: 0x04002794 RID: 10132
		private float sampleSign;
	}
}
