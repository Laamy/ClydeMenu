using System;
using TMPro;
using UnityEngine;

// Token: 0x02000280 RID: 640
public class LobbyChatUI : SemiUI
{
	// Token: 0x0600141B RID: 5147 RVA: 0x000B1520 File Offset: 0x000AF720
	protected override void Start()
	{
		base.Start();
		this.rectTransform = base.GetComponent<RectTransform>();
		this.menuPlayerListed = base.GetComponentInParent<MenuPlayerListed>();
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x000B1540 File Offset: 0x000AF740
	protected override void Update()
	{
		base.Update();
		if (this.isGameplay && (SemiFunc.RunIsLobbyMenu() || (PlayerAvatar.instance && PlayerAvatar.instance.isDisabled)))
		{
			this.uiText.text = "";
			return;
		}
		if (this.spectateName && this.prevPlayerName != this.spectateName.text)
		{
			this.offsetFetched = false;
		}
		if (this.isSpectate)
		{
			base.SemiUIScoot(new Vector2(-200f + this.chatOffsetXPos, 0f));
		}
		if (this.isSpectate && this.spectateName && !this.offsetFetched)
		{
			float num = this.spectateName.preferredWidth;
			if (num > 155f)
			{
				num = 155f;
			}
			this.rectTransform.localPosition = this.spectateName.rectTransform.localPosition + new Vector3(num, 25f, 0f);
			this.chatOffsetXPos = this.rectTransform.localPosition.x;
			this.offsetFetched = true;
			this.prevPlayerName = this.spectateName.text;
		}
		if (!this.ttsVoice)
		{
			if (!this.isGameplay)
			{
				if (this.menuPlayerListed.playerAvatar.voiceChat && this.menuPlayerListed.playerAvatar.voiceChat.TTSinstantiated)
				{
					this.ttsVoice = this.menuPlayerListed.playerAvatar.voiceChat.ttsVoice;
					return;
				}
			}
			else if (PlayerAvatar.instance && PlayerAvatar.instance.voiceChat && PlayerAvatar.instance.voiceChat.ttsVoice)
			{
				this.ttsVoice = PlayerAvatar.instance.voiceChat.ttsVoice;
			}
			return;
		}
		if (this.prevWordTime != this.ttsVoice.currentWordTime)
		{
			base.SemiUITextFlashColor(Color.yellow, 0.2f);
			base.SemiUISpringShakeY(4f, 5f, 0.2f);
			this.prevWordTime = this.ttsVoice.currentWordTime;
			this.uiText.text = this.ttsVoice.voiceText;
		}
		if (this.ttsVoice.isSpeaking)
		{
			this.uiText.text = this.ttsVoice.voiceText;
			return;
		}
		this.uiText.text = "";
	}

	// Token: 0x04002257 RID: 8791
	private TTSVoice ttsVoice;

	// Token: 0x04002258 RID: 8792
	private MenuPlayerListed menuPlayerListed;

	// Token: 0x04002259 RID: 8793
	private float prevWordTime;

	// Token: 0x0400225A RID: 8794
	public bool isSpectate;

	// Token: 0x0400225B RID: 8795
	public bool isGameplay;

	// Token: 0x0400225C RID: 8796
	public TextMeshProUGUI spectateName;

	// Token: 0x0400225D RID: 8797
	private RectTransform rectTransform;

	// Token: 0x0400225E RID: 8798
	private float chatOffsetXPos;

	// Token: 0x0400225F RID: 8799
	private bool offsetFetched;

	// Token: 0x04002260 RID: 8800
	private string prevPlayerName = "";
}
