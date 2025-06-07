using System;
using TMPro;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class CurrencyUI : SemiUI
{
	// Token: 0x060013D5 RID: 5077 RVA: 0x000AFD6E File Offset: 0x000ADF6E
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		CurrencyUI.instance = this;
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x000AFD88 File Offset: 0x000ADF88
	protected override void Update()
	{
		base.Update();
		if (SemiFunc.RunIsLevel() || SemiFunc.RunIsTutorial())
		{
			base.Hide();
		}
		if (this.showTimer > 0f)
		{
			int value = SemiFunc.StatGetRunCurrency();
			this.currentHaulValue = value;
			if (this.currentHaulValue != this.prevHaulValue)
			{
				Color color = Color.green;
				if (this.currentHaulValue < this.prevHaulValue)
				{
					color = Color.red;
				}
				base.SemiUISpringShakeY(20f, 10f, 0.3f);
				base.SemiUITextFlashColor(color, 0.2f);
				base.SemiUISpringScale(0.4f, 5f, 0.2f);
				this.prevHaulValue = this.currentHaulValue;
			}
			string text = SemiFunc.DollarGetString(value);
			this.Text.text = "$" + text + "K";
		}
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x000AFE60 File Offset: 0x000AE060
	public void FetchCurrency()
	{
		string text = SemiFunc.DollarGetString(SemiFunc.StatGetRunCurrency());
		this.Text.text = "$" + text + "K";
	}

	// Token: 0x04002212 RID: 8722
	private TextMeshProUGUI Text;

	// Token: 0x04002213 RID: 8723
	public static CurrencyUI instance;

	// Token: 0x04002214 RID: 8724
	private int prevHaulValue;

	// Token: 0x04002215 RID: 8725
	private int currentHaulValue;
}
