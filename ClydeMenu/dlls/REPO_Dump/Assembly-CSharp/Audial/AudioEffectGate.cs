using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002E2 RID: 738
	public class AudioEffectGate : MonoBehaviour
	{
		// Token: 0x0600172F RID: 5935 RVA: 0x000C83ED File Offset: 0x000C65ED
		private void OnEnable()
		{
			this.sampleRate = (float)AudioSettings.outputSampleRate;
			this.envelope = new Envelope(this.Attack, this.Release);
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06001730 RID: 5936 RVA: 0x000C8412 File Offset: 0x000C6612
		// (set) Token: 0x06001731 RID: 5937 RVA: 0x000C841A File Offset: 0x000C661A
		public float InputGain
		{
			get
			{
				return this._inputGain;
			}
			set
			{
				if (value == this._inputGain)
				{
					return;
				}
				this._inputGain = Mathf.Clamp(value, 0f, 3f);
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06001732 RID: 5938 RVA: 0x000C843C File Offset: 0x000C663C
		// (set) Token: 0x06001733 RID: 5939 RVA: 0x000C8444 File Offset: 0x000C6644
		public float Threshold
		{
			get
			{
				return this._threshold;
			}
			set
			{
				if (value == this._threshold)
				{
					return;
				}
				this._threshold = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06001734 RID: 5940 RVA: 0x000C8466 File Offset: 0x000C6666
		// (set) Token: 0x06001735 RID: 5941 RVA: 0x000C846E File Offset: 0x000C666E
		public float Attack
		{
			get
			{
				return this._attack;
			}
			set
			{
				if (value == this._attack)
				{
					return;
				}
				this._attack = Mathf.Clamp(value, 0f, 1f);
				this.envelope.Attack = this._attack;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06001736 RID: 5942 RVA: 0x000C84A1 File Offset: 0x000C66A1
		// (set) Token: 0x06001737 RID: 5943 RVA: 0x000C84A9 File Offset: 0x000C66A9
		public float Release
		{
			get
			{
				return this._release;
			}
			set
			{
				if (value == this._release)
				{
					return;
				}
				this._release = Mathf.Clamp(value, 0f, 1f);
				this.envelope.Release = this._release;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06001738 RID: 5944 RVA: 0x000C84DC File Offset: 0x000C66DC
		// (set) Token: 0x06001739 RID: 5945 RVA: 0x000C84E4 File Offset: 0x000C66E4
		public float OutputGain
		{
			get
			{
				return this._outputGain;
			}
			set
			{
				if (value == this._outputGain)
				{
					return;
				}
				this._outputGain = Mathf.Clamp(value, 0f, 5f);
			}
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x000C8508 File Offset: 0x000C6708
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				data[i] *= this.InputGain;
				data[i + 1] *= this.InputGain;
				float sample = Mathf.Sqrt(data[i] * data[i] + data[i + 1] * data[i + 1]);
				float num = this.envelope.ProcessSample(sample);
				float num2 = 1f;
				if (num < this.Threshold)
				{
					num2 = Mathf.Pow(num / 4f, 4f);
				}
				data[i] *= num2 * this.OutputGain;
				data[i + 1] *= num2 * this.OutputGain;
			}
		}

		// Token: 0x0400276F RID: 10095
		[SerializeField]
		private Envelope envelope;

		// Token: 0x04002770 RID: 10096
		private float sampleRate;

		// Token: 0x04002771 RID: 10097
		[SerializeField]
		private float _inputGain = 1f;

		// Token: 0x04002772 RID: 10098
		[SerializeField]
		private float _threshold = 0.247f;

		// Token: 0x04002773 RID: 10099
		[SerializeField]
		private float _attack;

		// Token: 0x04002774 RID: 10100
		[SerializeField]
		public float _release = 0.75f;

		// Token: 0x04002775 RID: 10101
		[SerializeField]
		private float _outputGain = 1f;

		// Token: 0x04002776 RID: 10102
		private float env;
	}
}
