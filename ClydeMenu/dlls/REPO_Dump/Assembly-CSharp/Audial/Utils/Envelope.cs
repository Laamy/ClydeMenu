using System;
using UnityEngine;

namespace Audial.Utils
{
	// Token: 0x020002F2 RID: 754
	[Serializable]
	public class Envelope
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060017A0 RID: 6048 RVA: 0x000C98D9 File Offset: 0x000C7AD9
		// (set) Token: 0x060017A1 RID: 6049 RVA: 0x000C98E1 File Offset: 0x000C7AE1
		public float Attack
		{
			get
			{
				return this._attack;
			}
			set
			{
				this._attack = value;
				this.attackCoeff = Mathf.Exp(-1f / (Settings.SampleRate * this._attack));
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060017A2 RID: 6050 RVA: 0x000C9907 File Offset: 0x000C7B07
		// (set) Token: 0x060017A3 RID: 6051 RVA: 0x000C990F File Offset: 0x000C7B0F
		public float Release
		{
			get
			{
				return this._release;
			}
			set
			{
				this._release = value;
				this.releaseCoeff = Mathf.Exp(-1f / (Settings.SampleRate * this._release));
			}
		}

		// Token: 0x060017A4 RID: 6052 RVA: 0x000C9935 File Offset: 0x000C7B35
		public Envelope(float attack, float release)
		{
			this.Attack = attack;
			this.Release = release;
		}

		// Token: 0x060017A5 RID: 6053 RVA: 0x000C9958 File Offset: 0x000C7B58
		public float ProcessSample(float sample)
		{
			float num = (sample > this.env) ? this.attackCoeff : this.releaseCoeff;
			this.env = (1f - num) * sample + num * this.env;
			return this.env;
		}

		// Token: 0x040027CC RID: 10188
		private EnvelopeState envelopeState;

		// Token: 0x040027CD RID: 10189
		private float env;

		// Token: 0x040027CE RID: 10190
		private float _attack;

		// Token: 0x040027CF RID: 10191
		public float attackCoeff;

		// Token: 0x040027D0 RID: 10192
		private float _release;

		// Token: 0x040027D1 RID: 10193
		private float releaseCoeff;

		// Token: 0x040027D2 RID: 10194
		private float sustain = 1f;

		// Token: 0x040027D3 RID: 10195
		private float sampleRate;
	}
}
