using System;
using TMPro;
using UnityEngine;

// Token: 0x0200026E RID: 622
public class ArenaMessageUI : SemiUI
{
	// Token: 0x060013C1 RID: 5057 RVA: 0x000AF53A File Offset: 0x000AD73A
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		ArenaMessageUI.instance = this;
		this.Text.text = "";
		this.originalGradient = this.Text.colorGradient;
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x000AF578 File Offset: 0x000AD778
	public void ArenaText(string message)
	{
		if (message != this.Text.text)
		{
			this.messageTimer = 0f;
			base.SemiUIResetAllShakeEffects();
		}
		this.messageTimer = 0.1f;
		if (message != this.messagePrev)
		{
			this.Text.text = message;
			base.SemiUISpringShakeY(5f, 5f, 0.3f);
			base.SemiUISpringScale(0.1f, 2.5f, 0.2f);
			this.messagePrev = message;
		}
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x000AF5FF File Offset: 0x000AD7FF
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

	// Token: 0x040021E8 RID: 8680
	private TextMeshProUGUI Text;

	// Token: 0x040021E9 RID: 8681
	public static ArenaMessageUI instance;

	// Token: 0x040021EA RID: 8682
	private string messagePrev = "prev";

	// Token: 0x040021EB RID: 8683
	private float messageTimer;

	// Token: 0x040021EC RID: 8684
	private GameObject bigMessageEmojiObject;

	// Token: 0x040021ED RID: 8685
	private TextMeshProUGUI emojiText;

	// Token: 0x040021EE RID: 8686
	private VertexGradient originalGradient;
}
