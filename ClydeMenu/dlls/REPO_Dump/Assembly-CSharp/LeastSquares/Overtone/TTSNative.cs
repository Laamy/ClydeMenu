using System;
using System.Runtime.InteropServices;

namespace LeastSquares.Overtone
{
	// Token: 0x020002D8 RID: 728
	public static class TTSNative
	{
		// Token: 0x060016A7 RID: 5799
		[DllImport("overtone", CallingConvention = 2, EntryPoint = "overtone_start")]
		public static extern IntPtr OvertoneStart();

		// Token: 0x060016A8 RID: 5800
		[DllImport("overtone", CallingConvention = 2, EntryPoint = "overtone_text_2_audio")]
		public static extern TTSNative.OvertoneResult OvertoneText2Audio(IntPtr ctx, IntPtr text, IntPtr voice);

		// Token: 0x060016A9 RID: 5801
		[DllImport("overtone", CallingConvention = 2, EntryPoint = "overtone_load_voice")]
		public static extern IntPtr OvertoneLoadVoice(IntPtr configBuffer, uint configBufferSize, IntPtr modelBuffer, uint modelBufferSize);

		// Token: 0x060016AA RID: 5802
		[DllImport("overtone", CallingConvention = 2, EntryPoint = "overtone_set_speaker_id")]
		public static extern void OvertoneSetSpeakerId(IntPtr voice, long speakerId);

		// Token: 0x060016AB RID: 5803
		[DllImport("overtone", CallingConvention = 2, EntryPoint = "overtone_free_voice")]
		public static extern void OvertoneFreeVoice(IntPtr voice);

		// Token: 0x060016AC RID: 5804
		[DllImport("overtone", CallingConvention = 2, EntryPoint = "overtone_free_result")]
		public static extern void OvertoneFreeResult(TTSNative.OvertoneResult result);

		// Token: 0x060016AD RID: 5805
		[DllImport("overtone", CallingConvention = 2, EntryPoint = "overtone_free")]
		public static extern void OvertoneFree(IntPtr ctx);

		// Token: 0x04002720 RID: 10016
		private const string OvertoneLibrary = "overtone";

		// Token: 0x02000429 RID: 1065
		public struct OvertoneResult
		{
			// Token: 0x04002E2B RID: 11819
			public uint Channels;

			// Token: 0x04002E2C RID: 11820
			public uint SampleRate;

			// Token: 0x04002E2D RID: 11821
			public uint LengthSamples;

			// Token: 0x04002E2E RID: 11822
			public IntPtr Samples;
		}
	}
}
