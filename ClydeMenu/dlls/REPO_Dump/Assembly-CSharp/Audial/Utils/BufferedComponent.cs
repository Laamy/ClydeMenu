using System;
using UnityEngine;

namespace Audial.Utils
{
	// Token: 0x020002EF RID: 751
	public class BufferedComponent
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06001794 RID: 6036 RVA: 0x000C96A1 File Offset: 0x000C78A1
		// (set) Token: 0x06001795 RID: 6037 RVA: 0x000C96A9 File Offset: 0x000C78A9
		public float DelayLength
		{
			get
			{
				return this.delayLength;
			}
			set
			{
				this.delayLength = value;
				this.Offset = (int)(this.delayLength * (Settings.SampleRate / 1000f));
			}
		}

		// Token: 0x06001796 RID: 6038 RVA: 0x000C96CC File Offset: 0x000C78CC
		public BufferedComponent(float delayLength, float gain)
		{
			this.DelayLength = delayLength;
			this.gain = gain;
			this.bufferLength = (int)Settings.SampleRate * 10;
			this.buffer = new float[this.channelCount, this.bufferLength];
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x000C971A File Offset: 0x000C791A
		public void SetGainByDecayTime(float decayLength)
		{
			this.gain = Mathf.Pow(0.001f, this.delayLength / decayLength);
		}

		// Token: 0x06001798 RID: 6040 RVA: 0x000C9734 File Offset: 0x000C7934
		public float ProcessSample(int channel, float sample)
		{
			if (channel >= this.channelCount)
			{
				this.channelCount = channel + 1;
				this.buffer = new float[this.channelCount, this.bufferLength];
			}
			this.readIndex = ((this.Offset > this.writeIndex) ? (this.bufferLength + this.writeIndex - this.Offset) : (this.writeIndex - this.Offset));
			float num = this.buffer[channel, this.readIndex];
			this.buffer[channel, this.writeIndex] = sample + num * this.gain;
			return num;
		}

		// Token: 0x06001799 RID: 6041 RVA: 0x000C97D4 File Offset: 0x000C79D4
		public float ProcessSample(float sample)
		{
			this.readIndex = ((this.Offset > this.writeIndex) ? (this.bufferLength + this.writeIndex - this.Offset) : (this.writeIndex - this.Offset));
			float num = this.buffer[0, this.readIndex];
			this.buffer[0, this.writeIndex] = sample + num * this.gain;
			return num;
		}

		// Token: 0x0600179A RID: 6042 RVA: 0x000C9848 File Offset: 0x000C7A48
		public void MoveIndex()
		{
			this.writeIndex = (this.writeIndex + 1) % this.bufferLength;
		}

		// Token: 0x0600179B RID: 6043 RVA: 0x000C9860 File Offset: 0x000C7A60
		public void Reset()
		{
			for (int i = 0; i < this.buffer.Length; i++)
			{
				Array.Clear(this.buffer, 0, this.buffer.Length);
			}
			this.readIndex = 0;
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600179C RID: 6044 RVA: 0x000C98A1 File Offset: 0x000C7AA1
		// (set) Token: 0x0600179D RID: 6045 RVA: 0x000C98A9 File Offset: 0x000C7AA9
		public int Offset
		{
			get
			{
				return this._offset;
			}
			set
			{
				this._offset = (int)Mathf.Lerp((float)this._offset, (float)value, 0.8f);
			}
		}

		// Token: 0x040027BC RID: 10172
		public float[,] buffer;

		// Token: 0x040027BD RID: 10173
		private float loopTime;

		// Token: 0x040027BE RID: 10174
		public float delayLength;

		// Token: 0x040027BF RID: 10175
		public float decayLength;

		// Token: 0x040027C0 RID: 10176
		public int bufferLength;

		// Token: 0x040027C1 RID: 10177
		public float gain;

		// Token: 0x040027C2 RID: 10178
		public int readIndex;

		// Token: 0x040027C3 RID: 10179
		public int writeIndex;

		// Token: 0x040027C4 RID: 10180
		public int channelCount = 1;

		// Token: 0x040027C5 RID: 10181
		private float sampleRate;

		// Token: 0x040027C6 RID: 10182
		[SerializeField]
		private int _offset;
	}
}
