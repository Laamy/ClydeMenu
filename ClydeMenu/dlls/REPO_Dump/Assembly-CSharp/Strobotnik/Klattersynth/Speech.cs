using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Strobotnik.Klattersynth
{
	// Token: 0x020002DA RID: 730
	public class Speech : MonoBehaviour
	{
		// Token: 0x060016BC RID: 5820 RVA: 0x000C6F13 File Offset: 0x000C5113
		private bool errCheck(bool errorWhenTrue, string logErrorString)
		{
			if (errorWhenTrue)
			{
				if (logErrorString != null)
				{
					Debug.LogError(logErrorString, this);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060016BD RID: 5821 RVA: 0x000C6F28 File Offset: 0x000C5128
		private void cache(SpeechClip sc)
		{
			if (this.maxAutoCachedClips <= 0)
			{
				return;
			}
			if (this.cachedSpeechClips == null)
			{
				this.cachedSpeechClips = new List<SpeechClip>(this.maxAutoCachedClips);
			}
			else
			{
				int num = this.cachedSpeechClips.FindIndex((SpeechClip x) => x.hash == sc.hash);
				if (num >= 0)
				{
					this.cachedSpeechClips[num] = sc;
					return;
				}
				if (this.cachedSpeechClips.Count >= this.maxAutoCachedClips)
				{
					this.cachedSpeechClips.RemoveRange(0, this.cachedSpeechClips.Count - (this.maxAutoCachedClips - 1));
				}
			}
			this.cachedSpeechClips.Add(sc);
		}

		// Token: 0x060016BE RID: 5822 RVA: 0x000C6FDC File Offset: 0x000C51DC
		private SpeechClip findFromCache(StringBuilder text, int freq, SpeechSynth.VoicingSource voicingSrc, bool bracketsAsPhonemes)
		{
			if (this.cachedSpeechClips == null)
			{
				return null;
			}
			ulong hash = SpeechSynth.makeHashCode(text, freq, voicingSrc, bracketsAsPhonemes);
			int num = this.cachedSpeechClips.FindIndex((SpeechClip x) => x.hash == hash);
			if (num < 0)
			{
				return null;
			}
			SpeechClip speechClip = this.cachedSpeechClips[num];
			if (num < this.cachedSpeechClips.Count - 1)
			{
				this.cachedSpeechClips.RemoveAt(num);
				this.cachedSpeechClips.Add(speechClip);
			}
			return speechClip;
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x000C7060 File Offset: 0x000C5260
		public void speakNextScheduled()
		{
			if (this.scheduled == null || this.scheduled.Count == 0)
			{
				return;
			}
			Speech.ScheduledUnit scheduledUnit = this.scheduled[0];
			this.scheduled.RemoveAt(0);
			if (scheduledUnit.pregenClip != null)
			{
				this.speak(scheduledUnit.pregenClip);
				return;
			}
			this.speak(scheduledUnit.voiceBaseFrequency, scheduledUnit.voicingSource, scheduledUnit.text, scheduledUnit.bracketsAsPhonemes);
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x000C70CF File Offset: 0x000C52CF
		public bool isTalking()
		{
			return this.talking;
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x000C70D7 File Offset: 0x000C52D7
		public float getCurrentLoudness()
		{
			return this.speechSynth.getCurrentLoudness();
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x000C70E4 File Offset: 0x000C52E4
		public string getPhonemes()
		{
			return this.speechSynth.getPhonemes();
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x000C70F1 File Offset: 0x000C52F1
		public void speak(SpeechClip pregenSpeech)
		{
			this.speechSynth.speak(pregenSpeech);
			this.talking = true;
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x000C7106 File Offset: 0x000C5306
		public void speak(string text, bool bracketsAsPhonemes = false)
		{
			this.speak(this.voiceBaseFrequency, this.voicingSource, text, bracketsAsPhonemes);
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x000C711C File Offset: 0x000C531C
		public void speak(int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource, string text, bool bracketsAsPhonemes = false)
		{
			if (this.errCheck(text == null, "null text"))
			{
				return;
			}
			if (this.speakSB == null)
			{
				this.speakSB = new StringBuilder(text.Length * 3 / 2);
			}
			this.speakSB.Length = 0;
			this.speakSB.Append(text);
			this.speak(voiceBaseFrequency, voicingSource, this.speakSB, bracketsAsPhonemes);
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x000C7181 File Offset: 0x000C5381
		public void speak(StringBuilder text, bool bracketsAsPhonemes = false)
		{
			this.speak(this.voiceBaseFrequency, this.voicingSource, text, bracketsAsPhonemes);
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x000C7197 File Offset: 0x000C5397
		private void VoiceText(StringBuilder text, float wordTime)
		{
			if (this.ttsVoice)
			{
				this.ttsVoice.VoiceText(text.ToString(), wordTime);
			}
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x000C71B8 File Offset: 0x000C53B8
		public void speak(int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource, StringBuilder text, bool bracketsAsPhonemes = false)
		{
			this.VoiceText(text, Time.time);
			text = new StringBuilder(text.ToString().ToLower());
			if (this.errCheck(text == null, "null text (SB)"))
			{
				return;
			}
			if (!this.useStreamingMode)
			{
				SpeechClip speechClip = this.findFromCache(text, voiceBaseFrequency, voicingSource, bracketsAsPhonemes);
				if (speechClip == null)
				{
					this.pregenerate(out speechClip, voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes, true);
				}
				if (speechClip != null)
				{
					this.talking = true;
					this.speechSynth.speak(speechClip);
					return;
				}
			}
			else
			{
				this.talking = true;
				this.speechSynth.speak(text, voiceBaseFrequency, voicingSource, bracketsAsPhonemes);
			}
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x000C7248 File Offset: 0x000C5448
		public void pregenerate(string text, bool bracketsAsPhonemes = false)
		{
			SpeechClip speechClip;
			this.pregenerate(out speechClip, text, bracketsAsPhonemes, true);
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x000C7260 File Offset: 0x000C5460
		public void pregenerate(out SpeechClip speechClip, string text, bool bracketsAsPhonemes = false, bool addToCache = false)
		{
			speechClip = null;
			if (this.errCheck(text == null, "null text"))
			{
				return;
			}
			if (this.speakSB == null)
			{
				this.speakSB = new StringBuilder(text.Length * 3 / 2);
			}
			this.speakSB.Length = 0;
			this.speakSB.Append(text);
			this.pregenerate(out speechClip, this.speakSB, bracketsAsPhonemes, addToCache);
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x000C72C8 File Offset: 0x000C54C8
		public void pregenerate(out SpeechClip speechClip, StringBuilder text, bool bracketsAsPhonemes = false, bool addToCache = false)
		{
			this.pregenerate(out speechClip, this.voiceBaseFrequency, this.voicingSource, text, bracketsAsPhonemes, addToCache);
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x000C72E1 File Offset: 0x000C54E1
		public void pregenerate(out SpeechClip speechClip, int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource, StringBuilder text, bool bracketsAsPhonemes = false, bool addToCache = false)
		{
			speechClip = null;
			if (this.errCheck(text == null, "null text (SB)"))
			{
				return;
			}
			speechClip = this.speechSynth.pregenerate(text, voiceBaseFrequency, voicingSource, bracketsAsPhonemes, true);
			if (speechClip != null && addToCache)
			{
				this.cache(speechClip);
			}
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x000C7320 File Offset: 0x000C5520
		public void schedule(SpeechClip speechClip)
		{
			Speech.ScheduledUnit scheduledUnit = default(Speech.ScheduledUnit);
			scheduledUnit.pregenClip = speechClip;
			this.scheduled.Add(scheduledUnit);
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x000C7349 File Offset: 0x000C5549
		public void scheduleClear()
		{
			this.scheduled.Clear();
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x000C7356 File Offset: 0x000C5556
		public void schedule(string text, bool bracketsAsPhonemes = false)
		{
			this.schedule(this.voiceBaseFrequency, this.voicingSource, text, bracketsAsPhonemes);
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x000C736C File Offset: 0x000C556C
		public void schedule(StringBuilder text, bool bracketsAsPhonemes = false)
		{
			this.schedule(this.voiceBaseFrequency, this.voicingSource, text, bracketsAsPhonemes);
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x000C7384 File Offset: 0x000C5584
		public void schedule(int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource, string text, bool bracketsAsPhonemes = false)
		{
			if (!this.talking && this.scheduled.Count == 0)
			{
				this.speak(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
				return;
			}
			Speech.ScheduledUnit scheduledUnit = default(Speech.ScheduledUnit);
			scheduledUnit.voiceBaseFrequency = voiceBaseFrequency;
			scheduledUnit.voicingSource = voicingSource;
			scheduledUnit.text = text;
			scheduledUnit.bracketsAsPhonemes = bracketsAsPhonemes;
			scheduledUnit.pregenClip = null;
			this.scheduled.Add(scheduledUnit);
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x000C73EF File Offset: 0x000C55EF
		public void schedule(int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource, StringBuilder text, bool bracketsAsPhonemes = false)
		{
			if (!this.talking && this.scheduled.Count == 0)
			{
				this.speak(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
				return;
			}
			this.schedule(voiceBaseFrequency, voicingSource, text.ToString(), bracketsAsPhonemes);
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x000C7422 File Offset: 0x000C5622
		public void stop(bool allScheduled = false)
		{
			if (allScheduled)
			{
				this.scheduled.Clear();
			}
			this.speechSynth.stop();
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x000C743D File Offset: 0x000C563D
		public void cacheClear()
		{
			if (this.cachedSpeechClips != null)
			{
				this.cachedSpeechClips.Clear();
			}
		}

		// Token: 0x060016D5 RID: 5845 RVA: 0x000C7454 File Offset: 0x000C5654
		private void Awake()
		{
			this.audioSrc = base.GetComponentInParent<AudioSource>();
			this.speechSynth = new SpeechSynth();
			this.speechSynth.init(this.audioSrc, this.useStreamingMode, 11025, this.msPerSpeechFrame, this.flutter, this.flutterSpeed);
			this.ttsVoice = base.GetComponentInParent<TTSVoice>();
		}

		// Token: 0x060016D6 RID: 5846 RVA: 0x000C74B2 File Offset: 0x000C56B2
		private void Update()
		{
			this.talking = this.speechSynth.update();
			if (!this.talking)
			{
				this.speakNextScheduled();
			}
		}

		// Token: 0x060016D7 RID: 5847 RVA: 0x000C74D3 File Offset: 0x000C56D3
		private void OnDestroy()
		{
			this.ClearAllData();
		}

		// Token: 0x060016D8 RID: 5848 RVA: 0x000C74DC File Offset: 0x000C56DC
		private void ClearAllData()
		{
			this.stop(true);
			this.cacheClear();
			this.scheduleClear();
			if (this.speakSB != null)
			{
				this.speakSB.Clear();
			}
			if (this.audioSrc)
			{
				this.audioSrc.Stop();
			}
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x000C7528 File Offset: 0x000C5728
		private void OnDisable()
		{
			this.ClearAllData();
		}

		// Token: 0x04002727 RID: 10023
		[Tooltip("When true, speech is real-time generated (played using a single small looping audio clip).\n\nNOTE: Not supported with WebGL, will be auto-disabled in Start() when running in WebGL!")]
		public bool useStreamingMode = true;

		// Token: 0x04002728 RID: 10024
		[Tooltip("Maximum amount of speech clips to automatically cache in non-streaming mode.\n(Least recently used are discarded when going over this amount.)")]
		public int maxAutoCachedClips = 10;

		// Token: 0x04002729 RID: 10025
		[Tooltip("Base frequency for the synthesized voice.\nCan be runtime-adjusted.")]
		public int voiceBaseFrequency = 220;

		// Token: 0x0400272A RID: 10026
		[Tooltip("Type of \"voicing source\".\nCan be runtime-adjusted.")]
		public SpeechSynth.VoicingSource voicingSource;

		// Token: 0x0400272B RID: 10027
		[Tooltip("How many milliseconds to use per one \"speech frame\".")]
		[Range(1f, 100f)]
		public int msPerSpeechFrame = 10;

		// Token: 0x0400272C RID: 10028
		[Tooltip("Amount of flutter in voice.")]
		[Range(0f, 200f)]
		public int flutter = 10;

		// Token: 0x0400272D RID: 10029
		[Tooltip("Speed of the flutter.")]
		[Range(0.001f, 100f)]
		public float flutterSpeed = 1f;

		// Token: 0x0400272E RID: 10030
		private TTSVoice ttsVoice;

		// Token: 0x0400272F RID: 10031
		private const int sampleRate = 11025;

		// Token: 0x04002730 RID: 10032
		private bool talking;

		// Token: 0x04002731 RID: 10033
		private AudioSource audioSrc;

		// Token: 0x04002732 RID: 10034
		private SpeechSynth speechSynth;

		// Token: 0x04002733 RID: 10035
		private StringBuilder speakSB;

		// Token: 0x04002734 RID: 10036
		private List<SpeechClip> cachedSpeechClips;

		// Token: 0x04002735 RID: 10037
		private List<Speech.ScheduledUnit> scheduled = new List<Speech.ScheduledUnit>(5);

		// Token: 0x0200042A RID: 1066
		private struct ScheduledUnit
		{
			// Token: 0x04002E2F RID: 11823
			public string text;

			// Token: 0x04002E30 RID: 11824
			public int voiceBaseFrequency;

			// Token: 0x04002E31 RID: 11825
			public SpeechSynth.VoicingSource voicingSource;

			// Token: 0x04002E32 RID: 11826
			public bool bracketsAsPhonemes;

			// Token: 0x04002E33 RID: 11827
			public SpeechClip pregenClip;
		}
	}
}
