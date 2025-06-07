using System;
using TMPro;
using UnityEngine;

// Token: 0x02000287 RID: 647
public class ShopCostUI : SemiUI
{
	// Token: 0x06001447 RID: 5191 RVA: 0x000B3414 File Offset: 0x000B1614
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		ShopCostUI.instance = this;
		this.originalColor = this.Text.color;
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x000B3440 File Offset: 0x000B1640
	protected override void Update()
	{
		base.Update();
		if (SemiFunc.RunIsShop())
		{
			int num = SemiFunc.ShopGetTotalCost();
			string text = SemiFunc.DollarGetString(num);
			if (num > 0)
			{
				this.Text.text = "-$" + text + "K";
				this.Text.color = this.originalColor;
			}
			else
			{
				base.Hide();
			}
			this.currentValue = num;
			if (this.currentValue != this.prevValue)
			{
				Color color = Color.white;
				if (this.currentValue > this.prevValue)
				{
					color = Color.red;
				}
				base.SemiUISpringShakeY(20f, 10f, 0.3f);
				base.SemiUITextFlashColor(color, 0.2f);
				base.SemiUISpringScale(0.4f, 5f, 0.2f);
				this.prevValue = this.currentValue;
			}
		}
		if (!SemiFunc.RunIsShop())
		{
			base.Hide();
		}
		if (this.showTimer > 0f && SemiFunc.RunIsLevel())
		{
			this.Text.text = "+$" + this.animatedValue.ToString() + "K";
			this.Text.color = Color.green;
		}
	}

	// Token: 0x040022CA RID: 8906
	private TextMeshProUGUI Text;

	// Token: 0x040022CB RID: 8907
	public static ShopCostUI instance;

	// Token: 0x040022CC RID: 8908
	public int animatedValue;

	// Token: 0x040022CD RID: 8909
	private Color originalColor;

	// Token: 0x040022CE RID: 8910
	private int currentValue;

	// Token: 0x040022CF RID: 8911
	private int prevValue;
}
