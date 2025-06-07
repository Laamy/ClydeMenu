using System;
using UnityEngine;

// Token: 0x0200020A RID: 522
public class MenuSliderPlayerMicGain : MonoBehaviour
{
	// Token: 0x06001191 RID: 4497 RVA: 0x000A0049 File Offset: 0x0009E249
	private void Start()
	{
		this.menuSlider = base.GetComponent<MenuSlider>();
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x000A0058 File Offset: 0x0009E258
	private void Update()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			return;
		}
		if (!this.playerAvatar.voiceChatFetched)
		{
			return;
		}
		if (this.currentValue != (float)this.menuSlider.currentValue && this.playerAvatar.voiceChatFetched)
		{
			if (!this.fetched)
			{
				this.playerAvatar.voiceChat.voiceGain = GameManager.instance.PlayerMicrophoneSettingGet(this.playerAvatar.steamID);
				MenuButtonKick componentInChildren = base.GetComponentInChildren<MenuButtonKick>();
				if (componentInChildren)
				{
					componentInChildren.Setup(this.playerAvatar);
				}
				this.menuSlider.settingsValue = this.playerAvatar.voiceChat.voiceGain;
				this.menuSlider.currentValue = (int)(this.playerAvatar.voiceChat.voiceGain * 200f);
				this.menuSlider.SetBar(this.menuSlider.settingsValue);
				this.menuSlider.SetBarScaleInstant();
				this.fetched = true;
			}
			this.currentValue = (float)this.menuSlider.currentValue;
			this.playerAvatar.voiceChat.voiceGain = this.currentValue / 200f;
			GameManager.instance.PlayerMicrophoneSettingSet(this.playerAvatar.steamID, this.playerAvatar.voiceChat.voiceGain);
		}
		this.menuSlider.ExtraBarSet(this.playerAvatar.voiceChat.clipLoudnessNoTTS * 5f);
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x000A01C6 File Offset: 0x0009E3C6
	public void SliderNameSet(string name)
	{
		this.menuSlider = base.GetComponent<MenuSlider>();
		this.menuSlider.elementName = name;
		this.menuSlider.elementNameText.text = name;
	}

	// Token: 0x04001DC7 RID: 7623
	internal MenuSlider menuSlider;

	// Token: 0x04001DC8 RID: 7624
	internal PlayerAvatar playerAvatar;

	// Token: 0x04001DC9 RID: 7625
	private float currentValue;

	// Token: 0x04001DCA RID: 7626
	private bool fetched;
}
