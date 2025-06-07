using System;
using System.Collections.Generic;
using Strobotnik.Klattersynth;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000D3 RID: 211
public class TTSVoice : MonoBehaviour
{
	// Token: 0x0600075F RID: 1887 RVA: 0x00046707 File Offset: 0x00044907
	private void Init()
	{
		if (this.voices.Length != 0)
		{
			this.setVoice(0);
			return;
		}
		Debug.LogError("Empty voices array!", this);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x00046731 File Offset: 0x00044931
	public void TTSSpeakNow(string text, bool crouch)
	{
		this.StartSpeakingWithHighlight(text, crouch);
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x0004673C File Offset: 0x0004493C
	public void StopAndClearVoice()
	{
		this.voices[0].stop(true);
		this.voices[0].cacheClear();
		this.voices[0].scheduleClear();
		this.voices[1].stop(true);
		this.voices[1].cacheClear();
		this.voices[1].scheduleClear();
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x0004679C File Offset: 0x0004499C
	public void StartSpeakingWithHighlight(string text, bool crouch)
	{
		this.StopAndClearVoice();
		if (crouch)
		{
			this.voicingSource = SpeechSynth.VoicingSource.whisper;
			this.setVoice(1);
		}
		else
		{
			this.voicingSource = SpeechSynth.VoicingSource.natural;
			this.setVoice(0);
		}
		if (!this.activeVoice)
		{
			Debug.LogError("Active voice is not set.");
			return;
		}
		text = this.TranslateSpecialLetters(text);
		this.words = new List<string>(text.Split(' ', 0));
		foreach (string text2 in this.words)
		{
			this.activeVoice.schedule(text2, false);
		}
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x00046854 File Offset: 0x00044A54
	public void setVoice(int num)
	{
		if (num >= 0 && num < this.voices.Length)
		{
			this.activeVoice = this.voices[num];
			this.activeVoiceNum = num;
			return;
		}
		Debug.LogWarning("Invalid voice: " + num.ToString(), this);
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x00046892 File Offset: 0x00044A92
	public void VoiceText(string text, float wordTime)
	{
		if (this.playerAvatar && WorldSpaceUIParent.instance)
		{
			WorldSpaceUIParent.instance.TTS(this.playerAvatar, text, wordTime);
		}
		this.voiceText = text;
		this.currentWordTime = wordTime;
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x000468CD File Offset: 0x00044ACD
	private void Start()
	{
		this.Init();
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x000468E4 File Offset: 0x00044AE4
	private string TranslateSpecialLetters(string _text)
	{
		if (_text.Contains("ö") || _text.Contains("Ö"))
		{
			_text.Replace("ö", "oe");
			_text.Replace("Ö", "OE");
		}
		if (_text.Contains("ä") || _text.Contains("Ä"))
		{
			_text.Replace("ä", "ae");
			_text.Replace("Ä", "AE");
		}
		if (_text.Contains("å") || _text.Contains("Å"))
		{
			_text.Replace("å", "oa");
			_text.Replace("Å", "OA");
		}
		if (_text.Contains("ü") || _text.Contains("Ü"))
		{
			_text.Replace("ü", "ue");
			_text.Replace("Ü", "UE");
		}
		if (_text.Contains("ß"))
		{
			_text.Replace("ß", "ss");
		}
		if (_text.Contains("æ") || _text.Contains("Æ"))
		{
			_text.Replace("æ", "ae");
			_text.Replace("Æ", "AE");
		}
		if (_text.Contains("ø") || _text.Contains("Ø"))
		{
			_text.Replace("ø", "oe");
			_text.Replace("Ø", "OE");
		}
		return _text;
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00046A78 File Offset: 0x00044C78
	private void Update()
	{
		if (this.playerAvatar && this.isPlayerAvatarDisabledPrev != this.playerAvatar.isDisabled)
		{
			this.StopAndClearVoice();
			this.isPlayerAvatarDisabledPrev = this.playerAvatar.isDisabled;
		}
		this.isSpeaking = this.audioSource.isPlaying;
		if (this.isSpeaking && this.playerAvatar.voiceChat.clipLoudnessTTS <= 0.01f)
		{
			if (this.noClipLoudnessTimer > 2f)
			{
				this.StopAndClearVoice();
			}
			this.noClipLoudnessTimer += Time.deltaTime;
			return;
		}
		this.noClipLoudnessTimer = 0f;
	}

	// Token: 0x04000D0A RID: 3338
	internal PlayerAvatar playerAvatar;

	// Token: 0x04000D0B RID: 3339
	internal bool isPlayerAvatarDisabledPrev;

	// Token: 0x04000D0C RID: 3340
	public Text baseFreqHzLabel;

	// Token: 0x04000D0D RID: 3341
	public Speech[] voices;

	// Token: 0x04000D0E RID: 3342
	internal string voiceText;

	// Token: 0x04000D0F RID: 3343
	internal int voiceTextWordIndex;

	// Token: 0x04000D10 RID: 3344
	internal bool isSpeaking;

	// Token: 0x04000D11 RID: 3345
	internal AudioSource audioSource;

	// Token: 0x04000D12 RID: 3346
	private Speech activeVoice;

	// Token: 0x04000D13 RID: 3347
	private int activeVoiceNum;

	// Token: 0x04000D14 RID: 3348
	private int voiceBaseFrequency;

	// Token: 0x04000D15 RID: 3349
	private SpeechSynth.VoicingSource voicingSource;

	// Token: 0x04000D16 RID: 3350
	private List<string> words;

	// Token: 0x04000D17 RID: 3351
	internal float currentWordTime;

	// Token: 0x04000D18 RID: 3352
	private float tumbleCooldown;

	// Token: 0x04000D19 RID: 3353
	private float noClipLoudnessTimer;
}
