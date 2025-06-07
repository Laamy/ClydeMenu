using System;

namespace Audial.Utils
{
	// Token: 0x020002EE RID: 750
	public class AllPassFilter : BufferedComponent
	{
		// Token: 0x06001792 RID: 6034 RVA: 0x000C9684 File Offset: 0x000C7884
		public AllPassFilter(float delayLength, float gain) : base(delayLength, gain)
		{
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x000C968E File Offset: 0x000C788E
		public new float ProcessSample(int channel, float sample)
		{
			return base.ProcessSample(channel, sample) - this.gain * sample;
		}
	}
}
