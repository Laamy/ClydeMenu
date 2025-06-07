using System;
using TMPro;
using UnityEngine;

// Token: 0x0200027F RID: 639
public class ItemInfoUI : SemiUI
{
	// Token: 0x06001417 RID: 5143 RVA: 0x000B11E7 File Offset: 0x000AF3E7
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		ItemInfoUI.instance = this;
		this.Text.text = "";
		this.originalGradient = this.Text.colorGradient;
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x000B1224 File Offset: 0x000AF424
	public void ItemInfoText(ItemAttributes _itemAttributes, string message, bool enemy = false)
	{
		ItemAttributes currentlyLookingAtItemAttributes = PhysGrabber.instance.currentlyLookingAtItemAttributes;
		if (!PhysGrabber.instance.grabbed && _itemAttributes && currentlyLookingAtItemAttributes && currentlyLookingAtItemAttributes != _itemAttributes)
		{
			return;
		}
		if (message != this.Text.text)
		{
			this.messageTimer = 0f;
			base.SemiUIResetAllShakeEffects();
		}
		if (enemy)
		{
			VertexGradient colorGradient = new VertexGradient(new Color(1f, 0f, 0f), new Color(1f, 0f, 0f), new Color(1f, 0.1f, 0f), new Color(1f, 0.1f, 0f));
			this.Text.fontSize = 30f;
			this.Text.colorGradient = colorGradient;
		}
		else
		{
			this.Text.colorGradient = this.originalGradient;
			if (!SemiFunc.RunIsShop())
			{
				this.Text.fontSize = 15f;
			}
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

	// Token: 0x06001419 RID: 5145 RVA: 0x000B137C File Offset: 0x000AF57C
	protected override void Update()
	{
		if (SemiFunc.RunIsShop())
		{
			base.SemiUIScoot(new Vector2(0f, -20f));
		}
		else if (ItemInfoExtraUI.instance.hideTimer <= 0f)
		{
			if (BatteryUI.instance.showTimer > 0f)
			{
				if (Inventory.instance.InventorySpotsOccupied() > 0)
				{
					base.SemiUIScoot(new Vector2(0f, 27f));
				}
				else
				{
					base.SemiUIScoot(new Vector2(0f, 13f));
				}
			}
			else if (Inventory.instance.InventorySpotsOccupied() > 0)
			{
				base.SemiUIScoot(new Vector2(0f, 13f));
			}
			else
			{
				base.SemiUIScoot(new Vector2(0f, -3f));
			}
		}
		else if (BatteryUI.instance.showTimer > 0f)
		{
			if (Inventory.instance.InventorySpotsOccupied() > 0)
			{
				base.SemiUIScoot(new Vector2(0f, 27f));
			}
			else
			{
				base.SemiUIScoot(new Vector2(0f, 13f));
			}
		}
		else if (Inventory.instance.InventorySpotsOccupied() > 0)
		{
			base.SemiUIScoot(new Vector2(0f, 5f));
		}
		else
		{
			base.SemiUIScoot(new Vector2(0f, -15f));
		}
		base.Update();
		if (this.messageTimer > 0f)
		{
			this.messageTimer -= Time.deltaTime;
			return;
		}
		this.messagePrev = "prev";
		base.Hide();
	}

	// Token: 0x04002250 RID: 8784
	private TextMeshProUGUI Text;

	// Token: 0x04002251 RID: 8785
	public static ItemInfoUI instance;

	// Token: 0x04002252 RID: 8786
	private string messagePrev = "prev";

	// Token: 0x04002253 RID: 8787
	private float messageTimer;

	// Token: 0x04002254 RID: 8788
	private GameObject bigMessageEmojiObject;

	// Token: 0x04002255 RID: 8789
	private TextMeshProUGUI emojiText;

	// Token: 0x04002256 RID: 8790
	private VertexGradient originalGradient;
}
