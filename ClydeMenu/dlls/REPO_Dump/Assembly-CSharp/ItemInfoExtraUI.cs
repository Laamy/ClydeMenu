using System;
using TMPro;
using UnityEngine;

// Token: 0x0200027E RID: 638
public class ItemInfoExtraUI : SemiUI
{
	// Token: 0x06001413 RID: 5139 RVA: 0x000B1070 File Offset: 0x000AF270
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		ItemInfoExtraUI.instance = this;
		this.Text.text = "";
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x000B109C File Offset: 0x000AF29C
	public void ItemInfoText(string message, Color color)
	{
		if (this.messageTimer > 0f)
		{
			return;
		}
		this.messageTimer = 0.2f;
		if (message != this.messagePrev)
		{
			this.Text.text = message;
			base.SemiUISpringShakeY(20f, 10f, 0.3f);
			base.SemiUISpringScale(0.4f, 5f, 0.2f);
			this.textColor = color;
			this.Text.color = this.textColor;
			this.messagePrev = message;
		}
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x000B1128 File Offset: 0x000AF328
	protected override void Update()
	{
		if (Inventory.instance.InventorySpotsOccupied() > 0)
		{
			base.SemiUIScoot(new Vector2(0f, 5f));
		}
		else
		{
			base.SemiUIScoot(new Vector2(0f, -10f));
		}
		base.Update();
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		if (!SemiFunc.RunIsShop())
		{
			this.Text.fontSize = 12f;
		}
		if (this.messageTimer > 0f)
		{
			this.messageTimer -= Time.deltaTime;
			return;
		}
		this.Text.color = Color.white;
		this.messagePrev = "prev";
		base.Hide();
	}

	// Token: 0x04002249 RID: 8777
	private TextMeshProUGUI Text;

	// Token: 0x0400224A RID: 8778
	public static ItemInfoExtraUI instance;

	// Token: 0x0400224B RID: 8779
	private string messagePrev = "prev";

	// Token: 0x0400224C RID: 8780
	private float messageTimer;

	// Token: 0x0400224D RID: 8781
	private GameObject bigMessageEmojiObject;

	// Token: 0x0400224E RID: 8782
	private TextMeshProUGUI emojiText;

	// Token: 0x0400224F RID: 8783
	private Color textColor;
}
