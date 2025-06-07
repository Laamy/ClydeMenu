using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002DF RID: 735
	public class AudioEffectFader : MonoBehaviour
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06001712 RID: 5906 RVA: 0x000C7F70 File Offset: 0x000C6170
		// (set) Token: 0x06001713 RID: 5907 RVA: 0x000C7F78 File Offset: 0x000C6178
		public float Gain
		{
			get
			{
				return this._gain;
			}
			set
			{
				this._gain = Mathf.Clamp(value, 0f, 3f);
			}
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x000C7F90 File Offset: 0x000C6190
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (this.Mute)
			{
				for (int i = 0; i < data.Length; i++)
				{
					data[i] = 0f;
				}
				return;
			}
			for (int j = 0; j < data.Length; j++)
			{
				data[j] *= this.Gain;
			}
		}

		// Token: 0x0400275E RID: 10078
		[SerializeField]
		private float _gain = 1f;

		// Token: 0x0400275F RID: 10079
		public bool Mute;

		// Token: 0x04002760 RID: 10080
		private LFO lfo = new LFO();
	}
}
