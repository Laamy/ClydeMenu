using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Overtone.Scripts;
using UnityEngine;

namespace LeastSquares.Overtone
{
	// Token: 0x020002D6 RID: 726
	public class TTSEngine : MonoBehaviour
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600169A RID: 5786 RVA: 0x000C6B4F File Offset: 0x000C4D4F
		// (set) Token: 0x0600169B RID: 5787 RVA: 0x000C6B57 File Offset: 0x000C4D57
		public bool Loaded { get; private set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600169C RID: 5788 RVA: 0x000C6B60 File Offset: 0x000C4D60
		// (set) Token: 0x0600169D RID: 5789 RVA: 0x000C6B68 File Offset: 0x000C4D68
		public bool Disposed { get; private set; }

		// Token: 0x0600169E RID: 5790 RVA: 0x000C6B74 File Offset: 0x000C4D74
		private void Awake()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				this._context = TTSNative.OvertoneStart();
				this.Loaded = true;
			}
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x000C6BC0 File Offset: 0x000C4DC0
		public Task<AudioClip> Speak(string text, TTSVoiceNative voice)
		{
			TTSEngine.<Speak>d__11 <Speak>d__;
			<Speak>d__.<>t__builder = AsyncTaskMethodBuilder<AudioClip>.Create();
			<Speak>d__.<>4__this = this;
			<Speak>d__.text = text;
			<Speak>d__.voice = voice;
			<Speak>d__.<>1__state = -1;
			<Speak>d__.<>t__builder.Start<TTSEngine.<Speak>d__11>(ref <Speak>d__);
			return <Speak>d__.<>t__builder.Task;
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x000C6C13 File Offset: 0x000C4E13
		private AudioClip MakeClip(string name, TTSResult result)
		{
			AudioClip audioClip = AudioClip.Create(name ?? string.Empty, result.Samples.Length, (int)result.Channels, (int)result.SampleRate, false);
			audioClip.SetData(result.Samples, 0);
			return audioClip;
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x000C6C48 File Offset: 0x000C4E48
		public Task<TTSResult> SpeakSamples(SpeechUnit unit, TTSVoiceNative voice)
		{
			TTSEngine.<SpeakSamples>d__13 <SpeakSamples>d__;
			<SpeakSamples>d__.<>t__builder = AsyncTaskMethodBuilder<TTSResult>.Create();
			<SpeakSamples>d__.<>4__this = this;
			<SpeakSamples>d__.unit = unit;
			<SpeakSamples>d__.voice = voice;
			<SpeakSamples>d__.<>1__state = -1;
			<SpeakSamples>d__.<>t__builder.Start<TTSEngine.<SpeakSamples>d__13>(ref <SpeakSamples>d__);
			return <SpeakSamples>d__.<>t__builder.Task;
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x000C6C9C File Offset: 0x000C4E9C
		private float[] PtrToSamples(IntPtr int16Buffer, uint samplesLength)
		{
			float[] array = new float[samplesLength];
			short[] array2 = new short[samplesLength];
			Marshal.Copy(int16Buffer, array2, 0, (int)samplesLength);
			int num = 0;
			while ((long)num < (long)((ulong)samplesLength))
			{
				array[num] = (float)array2[num] / 32767f;
				num++;
			}
			return array;
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x000C6CDC File Offset: 0x000C4EDC
		private void OnDestroy()
		{
			this.Dispose();
		}

		// Token: 0x060016A4 RID: 5796 RVA: 0x000C6CE4 File Offset: 0x000C4EE4
		private void Dispose()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				this.Disposed = true;
				if (this._context != IntPtr.Zero)
				{
					TTSNative.OvertoneFree(this._context);
				}
				Debug.Log("Successfully cleaned up TTS Engine");
			}
		}

		// Token: 0x0400271A RID: 10010
		private IntPtr _context;

		// Token: 0x0400271B RID: 10011
		private readonly object _lock = new object();
	}
}
