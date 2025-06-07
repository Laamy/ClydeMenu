using System;
using TMPro;
using UnityEngine;

// Token: 0x02000271 RID: 625
public class BigMessageUI : SemiUI
{
	// Token: 0x060013CE RID: 5070 RVA: 0x000AFB48 File Offset: 0x000ADD48
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		BigMessageUI.instance = this;
		this.bigMessageEmojiObject = base.transform.Find("Big Message Emoji").gameObject;
		this.emojiText = this.bigMessageEmojiObject.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x060013CF RID: 5071 RVA: 0x000AFB9C File Offset: 0x000ADD9C
	public void BigMessage(string message, string emoji, float size, Color colorMain, Color colorFlash)
	{
		this.bigMessageColor = colorMain;
		this.bigMessageFlashColor = colorFlash;
		this.bigMessageTimer = 0.2f;
		this.bigMessage = message;
		if (this.bigMessage != this.bigMessagePrev)
		{
			this.Text.fontSize = size;
			this.Text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, this.bigMessageColor);
			this.Text.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, this.bigMessageColor);
			this.Text.color = this.bigMessageColor;
			this.Text.text = this.bigMessage;
			this.bigMessageEmoji = SemiFunc.EmojiText(emoji);
			this.emojiText.text = this.bigMessageEmoji;
			base.SemiUISpringShakeY(20f, 10f, 0.3f);
			base.SemiUITextFlashColor(this.bigMessageFlashColor, 0.2f);
			base.SemiUISpringScale(0.4f, 5f, 0.2f);
			this.bigMessagePrev = this.bigMessage;
		}
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x000AFCAC File Offset: 0x000ADEAC
	protected override void Update()
	{
		base.Update();
		this.bigMessageEmojiObject.SetActive(this.Text.enabled);
		if (this.bigMessageTimer > 0f)
		{
			this.bigMessageTimer -= Time.deltaTime;
			return;
		}
		this.bigMessage = "big";
		this.bigMessagePrev = "prev";
		base.Hide();
	}

	// Token: 0x04002206 RID: 8710
	private TextMeshProUGUI Text;

	// Token: 0x04002207 RID: 8711
	public static BigMessageUI instance;

	// Token: 0x04002208 RID: 8712
	private string bigMessagePrev = "prev";

	// Token: 0x04002209 RID: 8713
	private string bigMessage = "big";

	// Token: 0x0400220A RID: 8714
	private Color bigMessageColor = Color.white;

	// Token: 0x0400220B RID: 8715
	private Color bigMessageFlashColor = Color.white;

	// Token: 0x0400220C RID: 8716
	private float bigMessageTimer;

	// Token: 0x0400220D RID: 8717
	private string bigMessageEmoji = "";

	// Token: 0x0400220E RID: 8718
	private GameObject bigMessageEmojiObject;

	// Token: 0x0400220F RID: 8719
	private TextMeshProUGUI emojiText;
}
