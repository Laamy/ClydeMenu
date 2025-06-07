using System;
using TMPro;
using UnityEngine;

// Token: 0x0200026F RID: 623
public class ArenaMessageWinUI : SemiUI
{
	// Token: 0x060013C5 RID: 5061 RVA: 0x000AF64B File Offset: 0x000AD84B
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		ArenaMessageWinUI.instance = this;
		this.Text.text = "";
		this.originalGradient = this.Text.colorGradient;
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x000AF688 File Offset: 0x000AD888
	public void ArenaText(string message, bool _kingCrowned = false)
	{
		if (message != this.Text.text)
		{
			this.messageTimer = 0f;
			base.SemiUIResetAllShakeEffects();
		}
		this.messageTimer = 0.1f;
		if (_kingCrowned)
		{
			if (!this.kingObject.activeSelf)
			{
				this.kingObject.SetActive(true);
				this.loserObject.transform.localPosition = new Vector3(0f, 3000f, 0f);
				this.loserObject.SetActive(false);
				this.backgroundObject.SetActive(true);
			}
		}
		else if (!this.loserObject.activeSelf)
		{
			this.loserObject.SetActive(true);
			this.kingObject.transform.localPosition = new Vector3(0f, 3000f, 0f);
			this.kingObject.SetActive(false);
			this.backgroundObject.SetActive(true);
		}
		if (message != this.messagePrev)
		{
			this.Text.text = message;
			base.SemiUISpringShakeY(5f, 5f, 0.3f);
			base.SemiUISpringScale(0.1f, 2.5f, 0.2f);
			this.messagePrev = message;
		}
	}

	// Token: 0x060013C7 RID: 5063 RVA: 0x000AF7C1 File Offset: 0x000AD9C1
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

	// Token: 0x040021EF RID: 8687
	private TextMeshProUGUI Text;

	// Token: 0x040021F0 RID: 8688
	public static ArenaMessageWinUI instance;

	// Token: 0x040021F1 RID: 8689
	private string messagePrev = "prev";

	// Token: 0x040021F2 RID: 8690
	private float messageTimer;

	// Token: 0x040021F3 RID: 8691
	private GameObject bigMessageEmojiObject;

	// Token: 0x040021F4 RID: 8692
	private TextMeshProUGUI emojiText;

	// Token: 0x040021F5 RID: 8693
	private VertexGradient originalGradient;

	// Token: 0x040021F6 RID: 8694
	public GameObject kingObject;

	// Token: 0x040021F7 RID: 8695
	public GameObject loserObject;

	// Token: 0x040021F8 RID: 8696
	public GameObject backgroundObject;
}
