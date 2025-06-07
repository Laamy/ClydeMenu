using System;
using TMPro;
using UnityEngine;

// Token: 0x02000281 RID: 641
public class MissionUI : SemiUI
{
	// Token: 0x0600141E RID: 5150 RVA: 0x000B17C7 File Offset: 0x000AF9C7
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		MissionUI.instance = this;
		this.Text.text = "";
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x000B17F4 File Offset: 0x000AF9F4
	public void MissionText(string message, Color colorMain, Color colorFlash, float time = 3f)
	{
		if (this.messageTimer > 0f)
		{
			return;
		}
		this.bigMessageColor = colorMain;
		this.bigMessageFlashColor = colorFlash;
		this.messageTimer = time;
		message = "<b>FOCUS > </b>" + message;
		if (message != this.messagePrev)
		{
			this.Text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, this.bigMessageColor);
			this.Text.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, this.bigMessageColor);
			this.Text.color = this.bigMessageColor;
			this.Text.text = message;
			base.SemiUISpringShakeY(20f, 10f, 0.3f);
			base.SemiUITextFlashColor(this.bigMessageFlashColor, 0.2f);
			base.SemiUISpringScale(0.4f, 5f, 0.2f);
			this.messagePrev = message;
		}
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x000B18D8 File Offset: 0x000AFAD8
	protected override void Update()
	{
		base.Update();
		if (this.messageTimer > 0f)
		{
			this.messageTimer -= Time.deltaTime;
			return;
		}
		this.messagePrev = "prev";
		base.Hide();
	}

	// Token: 0x04002261 RID: 8801
	internal TextMeshProUGUI Text;

	// Token: 0x04002262 RID: 8802
	public static MissionUI instance;

	// Token: 0x04002263 RID: 8803
	private string messagePrev = "prev";

	// Token: 0x04002264 RID: 8804
	private Color bigMessageColor = Color.white;

	// Token: 0x04002265 RID: 8805
	private Color bigMessageFlashColor = Color.white;

	// Token: 0x04002266 RID: 8806
	private float messageTimer;

	// Token: 0x04002267 RID: 8807
	private GameObject bigMessageEmojiObject;

	// Token: 0x04002268 RID: 8808
	private TextMeshProUGUI emojiText;
}
