using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002E5 RID: 741
	public class AudioEffectReverb : MonoBehaviour
	{
		// Token: 0x0600174F RID: 5967 RVA: 0x000C89AF File Offset: 0x000C6BAF
		private void Awake()
		{
			Settings.SampleRate = (float)AudioSettings.outputSampleRate;
			this.Initialize();
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06001750 RID: 5968 RVA: 0x000C89C2 File Offset: 0x000C6BC2
		// (set) Token: 0x06001751 RID: 5969 RVA: 0x000C89CA File Offset: 0x000C6BCA
		public float ReverbTime
		{
			get
			{
				return this._reverbTime;
			}
			set
			{
				this._reverbTime = Mathf.Clamp(value, 0.5f, 10f);
				this.Callibrate();
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06001752 RID: 5970 RVA: 0x000C89E8 File Offset: 0x000C6BE8
		// (set) Token: 0x06001753 RID: 5971 RVA: 0x000C89F0 File Offset: 0x000C6BF0
		public float ReverbGain
		{
			get
			{
				return this._reverbGain;
			}
			set
			{
				this._reverbGain = Mathf.Clamp(value, 0.5f, 5f);
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06001754 RID: 5972 RVA: 0x000C8A08 File Offset: 0x000C6C08
		// (set) Token: 0x06001755 RID: 5973 RVA: 0x000C8A10 File Offset: 0x000C6C10
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

		// Token: 0x06001756 RID: 5974 RVA: 0x000C8A28 File Offset: 0x000C6C28
		private void Initialize()
		{
			this.combFilters = new CombFilter[4];
			this.combFilters[0] = new CombFilter(29.7f, 1f);
			this.combFilters[1] = new CombFilter(37.1f, 1f);
			this.combFilters[2] = new CombFilter(41.1f, 1f);
			this.combFilters[3] = new CombFilter(43.7f, 1f);
			this.Callibrate();
			this.allPassFilters = new AllPassFilter[2];
			this.allPassFilters[0] = new AllPassFilter(5f, 1f);
			this.allPassFilters[0].SetGainByDecayTime(1.683f);
			this.allPassFilters[1] = new AllPassFilter(1.7f, 1f);
			this.allPassFilters[1].SetGainByDecayTime(2.232f);
		}

		// Token: 0x06001757 RID: 5975 RVA: 0x000C8B04 File Offset: 0x000C6D04
		private void Callibrate()
		{
			if (this.combFilters == null)
			{
				return;
			}
			for (int i = 0; i < this.combFilters.Length; i++)
			{
				this.combFilters[i].SetGainByDecayTime(this.ReverbTime * 1000f);
			}
		}

		// Token: 0x06001758 RID: 5976 RVA: 0x000C8B48 File Offset: 0x000C6D48
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (this.combFilters == null || this.allPassFilters == null)
			{
				this.Initialize();
			}
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					float num = data[i + j] * this.ReverbGain;
					for (int k = 0; k < this.combFilters.Length; k++)
					{
						num += this.combFilters[k].ProcessSample(j, data[i + j]);
					}
					num /= (float)this.combFilters.Length;
					float num2 = num / (float)this.combFilters.Length;
					for (int l = 0; l < this.allPassFilters.Length; l++)
					{
						num2 += this.allPassFilters[l].ProcessSample(j, num);
					}
					data[i + j] = data[i + j] * (1f - this.DryWet) + num2 * this.ReverbGain / (float)this.allPassFilters.Length * this.DryWet;
				}
				for (int m = 0; m < this.combFilters.Length; m++)
				{
					this.combFilters[m].MoveIndex();
				}
				for (int n = 0; n < this.allPassFilters.Length; n++)
				{
					this.allPassFilters[n].MoveIndex();
				}
			}
		}

		// Token: 0x04002785 RID: 10117
		[SerializeField]
		private float _reverbTime = 1.55f;

		// Token: 0x04002786 RID: 10118
		[SerializeField]
		private float _reverbGain = 1f;

		// Token: 0x04002787 RID: 10119
		[SerializeField]
		private float _dryWet = 0.16f;

		// Token: 0x04002788 RID: 10120
		private CombFilter[] combFilters;

		// Token: 0x04002789 RID: 10121
		private AllPassFilter[] allPassFilters;
	}
}
