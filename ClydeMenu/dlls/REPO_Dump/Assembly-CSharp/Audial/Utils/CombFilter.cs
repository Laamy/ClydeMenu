using System;

namespace Audial.Utils
{
	// Token: 0x020002F0 RID: 752
	public class CombFilter : BufferedComponent
	{
		// Token: 0x0600179E RID: 6046 RVA: 0x000C98C5 File Offset: 0x000C7AC5
		public CombFilter(float delayLength, float gain) : base(delayLength, gain)
		{
		}

		// Token: 0x0600179F RID: 6047 RVA: 0x000C98CF File Offset: 0x000C7ACF
		public new float ProcessSample(int channel, float sample)
		{
			return base.ProcessSample(channel, sample);
		}
	}
}
