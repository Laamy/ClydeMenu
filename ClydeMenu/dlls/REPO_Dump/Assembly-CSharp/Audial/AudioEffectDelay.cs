using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002DD RID: 733
	public class AudioEffectDelay : MonoBehaviour
	{
		// Token: 0x060016F8 RID: 5880 RVA: 0x000C7A80 File Offset: 0x000C5C80
		private void Awake()
		{
			this.sampleRate = (float)AudioSettings.outputSampleRate;
			this.ChangeDelay();
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060016F9 RID: 5881 RVA: 0x000C7A94 File Offset: 0x000C5C94
		// (set) Token: 0x060016FA RID: 5882 RVA: 0x000C7A9C File Offset: 0x000C5C9C
		public float BPM
		{
			get
			{
				return this._BPM;
			}
			set
			{
				this._BPM = Mathf.Clamp(value, 40f, 300f);
				this.ChangeDelay();
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060016FB RID: 5883 RVA: 0x000C7ABA File Offset: 0x000C5CBA
		// (set) Token: 0x060016FC RID: 5884 RVA: 0x000C7AC2 File Offset: 0x000C5CC2
		public int DelayCount
		{
			get
			{
				return this._delayCount;
			}
			set
			{
				this._delayCount = Mathf.Clamp(value, 1, 8);
				this.ChangeDelay();
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060016FD RID: 5885 RVA: 0x000C7AD8 File Offset: 0x000C5CD8
		// (set) Token: 0x060016FE RID: 5886 RVA: 0x000C7AE0 File Offset: 0x000C5CE0
		public int DelayUnit
		{
			get
			{
				return this._delayUnit;
			}
			set
			{
				this._delayUnit = Mathf.Clamp(value, 1, 32);
				this.ChangeDelay();
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060016FF RID: 5887 RVA: 0x000C7AF7 File Offset: 0x000C5CF7
		// (set) Token: 0x06001700 RID: 5888 RVA: 0x000C7AFF File Offset: 0x000C5CFF
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

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06001701 RID: 5889 RVA: 0x000C7B17 File Offset: 0x000C5D17
		// (set) Token: 0x06001702 RID: 5890 RVA: 0x000C7B1F File Offset: 0x000C5D1F
		public float DecayLength
		{
			get
			{
				return this._decayLength;
			}
			set
			{
				this._decayLength = Mathf.Clamp(value, 0.1f, 1f);
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06001703 RID: 5891 RVA: 0x000C7B37 File Offset: 0x000C5D37
		// (set) Token: 0x06001704 RID: 5892 RVA: 0x000C7B3F File Offset: 0x000C5D3F
		public float Pan
		{
			get
			{
				return this._pan;
			}
			set
			{
				this._pan = Mathf.Clamp(value, -1f, 1f);
			}
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x000C7B58 File Offset: 0x000C5D58
		private void ChangeDelay()
		{
			this.delayLength = (float)this.DelayCount * (240f / this.BPM) / (float)this.DelayUnit;
			this.delaySamples = (int)(this.delayLength * this.sampleRate);
			this.delayBuffer = new float[2, this.delaySamples];
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x000C7BB0 File Offset: 0x000C5DB0
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (this.delayBuffer == null)
			{
				this.ChangeDelay();
			}
			float[] array = new float[channels];
			float[] array2 = new float[]
			{
				1f,
				1f
			};
			if (this.Pan > 0f)
			{
				array2[0] = 1f - Mathf.Abs(this.Pan);
			}
			else if (this.Pan < 0f)
			{
				array2[1] = 1f - Mathf.Abs(this.Pan);
			}
			for (int i = 0; i < data.Length; i += channels)
			{
				this.index %= this.delaySamples;
				if (this.PingPong)
				{
					for (int j = 0; j < channels; j++)
					{
						array[j] = this.delayBuffer[j, this.index];
						this.delayBuffer[j, this.index] = 0f;
					}
					for (int k = 0; k < channels; k++)
					{
						float num = data[i + k];
						float num2 = array[(k + 1) % channels];
						this.output = num * (1f - this.DryWet) + num2 * this.DryWet;
						data[i + k] = this.output;
						this.delayBuffer[k, this.index] += num2 * this.DecayLength;
						this.delayBuffer[(k + 1) % channels, this.index] += num * array2[k];
					}
				}
				else
				{
					for (int l = 0; l < channels; l++)
					{
						array[l] = this.delayBuffer[l, this.index];
						this.delayBuffer[l, this.index] = 0f;
						float num3 = data[i + l];
						float num4 = array[l];
						this.output = num3 * (1f - this.DryWet) + num4 * this.DryWet;
						data[i + l] = this.output;
						this.delayBuffer[l, this.index] += num4 * this.DecayLength;
						this.delayBuffer[l, this.index] += num3 * array2[l];
					}
				}
				this.index++;
			}
		}

		// Token: 0x0400274D RID: 10061
		private float sampleRate;

		// Token: 0x0400274E RID: 10062
		private float[,] delayBuffer;

		// Token: 0x0400274F RID: 10063
		private int index;

		// Token: 0x04002750 RID: 10064
		[SerializeField]
		private float _BPM = 120f;

		// Token: 0x04002751 RID: 10065
		[SerializeField]
		private int _delayCount = 3;

		// Token: 0x04002752 RID: 10066
		[SerializeField]
		private int _delayUnit = 8;

		// Token: 0x04002753 RID: 10067
		[SerializeField]
		private float _dryWet = 0.5f;

		// Token: 0x04002754 RID: 10068
		[SerializeField]
		private float _decayLength = 0.25f;

		// Token: 0x04002755 RID: 10069
		[SerializeField]
		private float _pan;

		// Token: 0x04002756 RID: 10070
		public bool PingPong;

		// Token: 0x04002757 RID: 10071
		private float delayLength;

		// Token: 0x04002758 RID: 10072
		private int delaySamples;

		// Token: 0x04002759 RID: 10073
		private float output;
	}
}
