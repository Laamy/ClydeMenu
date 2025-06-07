using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002EB RID: 747
	public class AudioEffectStereoWidener : MonoBehaviour
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06001781 RID: 6017 RVA: 0x000C9461 File Offset: 0x000C7661
		// (set) Token: 0x06001782 RID: 6018 RVA: 0x000C9469 File Offset: 0x000C7669
		public float Width
		{
			get
			{
				return this._width;
			}
			set
			{
				this._width = Mathf.Clamp(value, 0f, 2f);
			}
		}

		// Token: 0x06001783 RID: 6019 RVA: 0x000C9484 File Offset: 0x000C7684
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (channels < 2)
			{
				return;
			}
			float num = this.Width * 0.5f;
			for (int i = 0; i < data.Length; i += channels)
			{
				float num2 = (data[i] + data[i + 1]) * 0.5f;
				float num3 = (data[i] - data[i + 1]) * num;
				data[i] = num2 + num3;
				data[i + 1] = num2 - num3;
			}
		}

		// Token: 0x040027B4 RID: 10164
		[SerializeField]
		[Range(0f, 2f)]
		private float _width = 1.3f;
	}
}
