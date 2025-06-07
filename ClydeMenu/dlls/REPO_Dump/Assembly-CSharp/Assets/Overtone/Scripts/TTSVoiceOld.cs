using System;
using LeastSquares.Overtone;
using UnityEngine;

namespace Assets.Overtone.Scripts
{
	// Token: 0x020002D1 RID: 721
	public class TTSVoiceOld : MonoBehaviour
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06001689 RID: 5769 RVA: 0x000C68D2 File Offset: 0x000C4AD2
		// (set) Token: 0x0600168A RID: 5770 RVA: 0x000C68DA File Offset: 0x000C4ADA
		public TTSVoiceNative VoiceModel { get; private set; }

		// Token: 0x0600168B RID: 5771 RVA: 0x000C68E4 File Offset: 0x000C4AE4
		private void Update()
		{
			if (this.voiceName != this.oldVoiceName)
			{
				this.oldVoiceName = this.voiceName;
				this.VoiceModel = TTSVoiceNative.LoadVoiceFromResources(this.voiceName);
			}
			if (this.speakerId != this.oldSpeakerId)
			{
				this.oldSpeakerId = this.speakerId;
				this.VoiceModel.SetSpeakerId(this.speakerId);
			}
		}

		// Token: 0x0600168C RID: 5772 RVA: 0x000C694C File Offset: 0x000C4B4C
		private void OnDestroy()
		{
			TTSVoiceNative voiceModel = this.VoiceModel;
			if (voiceModel == null)
			{
				return;
			}
			voiceModel.Dispose();
		}

		// Token: 0x0400270F RID: 9999
		public string voiceName;

		// Token: 0x04002710 RID: 10000
		public int speakerId;

		// Token: 0x04002711 RID: 10001
		private string oldVoiceName;

		// Token: 0x04002712 RID: 10002
		private int oldSpeakerId;
	}
}
