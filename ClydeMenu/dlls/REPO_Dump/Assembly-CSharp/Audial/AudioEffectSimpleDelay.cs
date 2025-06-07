using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002E8 RID: 744
	public class AudioEffectSimpleDelay : MonoBehaviour
	{
		// Token: 0x0600176A RID: 5994 RVA: 0x000C8F4D File Offset: 0x000C714D
		private void Awake()
		{
			this.sampleRate = (Settings.SampleRate = (float)AudioSettings.outputSampleRate);
			this.combFilter = new CombFilter((float)this.DelayLengthMS, 0.5f);
			this.ChangeDelay();
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600176B RID: 5995 RVA: 0x000C8F7E File Offset: 0x000C717E
		// (set) Token: 0x0600176C RID: 5996 RVA: 0x000C8F86 File Offset: 0x000C7186
		public int DelayLengthMS
		{
			get
			{
				return this._delayLengthMS;
			}
			set
			{
				this._delayLengthMS = Mathf.Clamp(value, 10, 3000);
				this.ChangeDelay();
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600176D RID: 5997 RVA: 0x000C8FA1 File Offset: 0x000C71A1
		// (set) Token: 0x0600176E RID: 5998 RVA: 0x000C8FA9 File Offset: 0x000C71A9
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

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600176F RID: 5999 RVA: 0x000C8FC1 File Offset: 0x000C71C1
		// (set) Token: 0x06001770 RID: 6000 RVA: 0x000C8FC9 File Offset: 0x000C71C9
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

		// Token: 0x06001771 RID: 6001 RVA: 0x000C8FE1 File Offset: 0x000C71E1
		private void ChangeDelay()
		{
			this.combFilter.DelayLength = (float)this.DelayLengthMS;
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x000C8FF8 File Offset: 0x000C71F8
		private void OnAudioFilterRead(float[] data, int channels)
		{
			this.combFilter.gain = this.DecayLength;
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					float num = data[i + j];
					float num2 = this.combFilter.ProcessSample(j, num);
					this.output = num * (1f - this.DryWet / 2f) + num2 * this.DryWet / 2f;
					data[i + j] = this.output;
				}
				this.combFilter.MoveIndex();
			}
		}

		// Token: 0x04002795 RID: 10133
		private float sampleRate;

		// Token: 0x04002796 RID: 10134
		private CombFilter combFilter;

		// Token: 0x04002797 RID: 10135
		[SerializeField]
		private int _delayLengthMS = 120;

		// Token: 0x04002798 RID: 10136
		private int DelayLengthMSPrev = 10;

		// Token: 0x04002799 RID: 10137
		[SerializeField]
		private float _dryWet = 0.5f;

		// Token: 0x0400279A RID: 10138
		[SerializeField]
		private float _decayLength = 0.25f;

		// Token: 0x0400279B RID: 10139
		private float delayLength;

		// Token: 0x0400279C RID: 10140
		private int delaySamples;

		// Token: 0x0400279D RID: 10141
		private float output;
	}
}
