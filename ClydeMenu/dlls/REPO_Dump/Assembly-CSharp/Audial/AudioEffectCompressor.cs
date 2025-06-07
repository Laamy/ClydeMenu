using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002DB RID: 731
	public class AudioEffectCompressor : MonoBehaviour
	{
		// Token: 0x060016DB RID: 5851 RVA: 0x000C7584 File Offset: 0x000C5784
		private void Awake()
		{
			Settings.SampleRate = (float)AudioSettings.outputSampleRate;
			this.envelope = new Envelope(this.Attack, this.Release);
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060016DC RID: 5852 RVA: 0x000C75A8 File Offset: 0x000C57A8
		// (set) Token: 0x060016DD RID: 5853 RVA: 0x000C75B0 File Offset: 0x000C57B0
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

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060016DE RID: 5854 RVA: 0x000C75C8 File Offset: 0x000C57C8
		// (set) Token: 0x060016DF RID: 5855 RVA: 0x000C75D0 File Offset: 0x000C57D0
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

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060016E0 RID: 5856 RVA: 0x000C75E8 File Offset: 0x000C57E8
		// (set) Token: 0x060016E1 RID: 5857 RVA: 0x000C75F0 File Offset: 0x000C57F0
		public float Slope
		{
			get
			{
				return this._slope;
			}
			set
			{
				this._slope = Mathf.Clamp(value, 0f, 2f);
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060016E2 RID: 5858 RVA: 0x000C7608 File Offset: 0x000C5808
		// (set) Token: 0x060016E3 RID: 5859 RVA: 0x000C7610 File Offset: 0x000C5810
		public float Attack
		{
			get
			{
				return this._attack;
			}
			set
			{
				this._attack = Mathf.Clamp(value, 0f, 1f);
				this.envelope.Attack = this._attack;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060016E4 RID: 5860 RVA: 0x000C7639 File Offset: 0x000C5839
		// (set) Token: 0x060016E5 RID: 5861 RVA: 0x000C7641 File Offset: 0x000C5841
		public float Release
		{
			get
			{
				return this._release;
			}
			set
			{
				this._release = Mathf.Clamp(value, 0f, 1f);
				this.envelope.Release = this._release;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060016E6 RID: 5862 RVA: 0x000C766A File Offset: 0x000C586A
		// (set) Token: 0x060016E7 RID: 5863 RVA: 0x000C7672 File Offset: 0x000C5872
		public float DryGain
		{
			get
			{
				return this._dryGain;
			}
			set
			{
				this._dryGain = Mathf.Clamp(value, 0f, 5f);
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060016E8 RID: 5864 RVA: 0x000C768A File Offset: 0x000C588A
		// (set) Token: 0x060016E9 RID: 5865 RVA: 0x000C7692 File Offset: 0x000C5892
		public float CompressedGain
		{
			get
			{
				return this._compressedGain;
			}
			set
			{
				this._compressedGain = Mathf.Clamp(value, 0f, 5f);
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060016EA RID: 5866 RVA: 0x000C76AA File Offset: 0x000C58AA
		// (set) Token: 0x060016EB RID: 5867 RVA: 0x000C76B2 File Offset: 0x000C58B2
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

		// Token: 0x060016EC RID: 5868 RVA: 0x000C76CC File Offset: 0x000C58CC
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (this.envelope == null)
			{
				return;
			}
			if (this.input == null || this.input.Length != channels)
			{
				this.input = new float[channels];
				this.compressed = new float[channels];
				this.dry = new float[channels];
			}
			for (int i = 0; i < data.Length; i += channels)
			{
				this.rms = 0f;
				for (int j = 0; j < channels; j++)
				{
					this.input[j] = data[i + j] * this.InputGain;
					this.rms += this.input[j] * this.input[j];
				}
				this.rms = Mathf.Pow(this.rms, 1f / (float)channels);
				this.env = this.envelope.ProcessSample(this.rms);
				this.compressMod = 1f;
				if (this.env > this.Threshold)
				{
					this.compressMod = Mathf.Clamp(this.compressMod - (this.env - this.Threshold) * this.Slope, 0f, 1f);
				}
				this.mergedData = 0f;
				for (int k = 0; k < channels; k++)
				{
					this.compressed[k] = this.input[k] * this.compressMod;
					this.mergedData += this.compressed[k] * this.compressed[k];
					data[i + k] = (this.compressed[k] * this.CompressedGain + this.input[k] * this.DryGain) * this.OutputGain;
				}
				this.mergedData = Mathf.Pow(this.mergedData, 1f / (float)channels);
			}
		}

		// Token: 0x04002736 RID: 10038
		[SerializeField]
		public Envelope envelope;

		// Token: 0x04002737 RID: 10039
		[SerializeField]
		private float _inputGain = 1f;

		// Token: 0x04002738 RID: 10040
		[SerializeField]
		private float _threshold = 0.247f;

		// Token: 0x04002739 RID: 10041
		[SerializeField]
		public float _slope = 1.727f;

		// Token: 0x0400273A RID: 10042
		[SerializeField]
		private float _attack = 0.0001f;

		// Token: 0x0400273B RID: 10043
		[SerializeField]
		public float _release = 0.68f;

		// Token: 0x0400273C RID: 10044
		[SerializeField]
		private float _dryGain;

		// Token: 0x0400273D RID: 10045
		[SerializeField]
		private float _compressedGain = 1f;

		// Token: 0x0400273E RID: 10046
		[SerializeField]
		private float _outputGain = 1f;

		// Token: 0x0400273F RID: 10047
		private float env;

		// Token: 0x04002740 RID: 10048
		private float mergedData;

		// Token: 0x04002741 RID: 10049
		private float[] input;

		// Token: 0x04002742 RID: 10050
		private float rms;

		// Token: 0x04002743 RID: 10051
		private float[] compressed;

		// Token: 0x04002744 RID: 10052
		private float[] dry;

		// Token: 0x04002745 RID: 10053
		private float compressMod;
	}
}
