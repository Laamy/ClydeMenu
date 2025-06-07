using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002E3 RID: 739
	public class AudioEffectPanControl : MonoBehaviour
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600173C RID: 5948 RVA: 0x000C85F0 File Offset: 0x000C67F0
		// (set) Token: 0x0600173D RID: 5949 RVA: 0x000C85F8 File Offset: 0x000C67F8
		public float PanAmount
		{
			get
			{
				return this._panAmount;
			}
			set
			{
				this._panAmount = Mathf.Clamp(value, -1f, 1f);
			}
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x000C8610 File Offset: 0x000C6810
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (channels != 2)
			{
				return;
			}
			for (int i = 0; i < data.Length; i += channels)
			{
				if (Mathf.Sign(this.PanAmount) > 0f)
				{
					data[i] = (1f - Mathf.Abs(this.PanAmount)) * data[i];
				}
				else
				{
					data[i + 1] = (1f - Mathf.Abs(this.PanAmount)) * data[i + 1];
				}
			}
		}

		// Token: 0x04002777 RID: 10103
		[SerializeField]
		[Range(-1f, 1f)]
		private float _panAmount;
	}
}
