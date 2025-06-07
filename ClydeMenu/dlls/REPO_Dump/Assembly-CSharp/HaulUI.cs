using System;
using TMPro;
using UnityEngine;

// Token: 0x02000278 RID: 632
public class HaulUI : SemiUI
{
	// Token: 0x060013E6 RID: 5094 RVA: 0x000B0188 File Offset: 0x000AE388
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		HaulUI.instance = this;
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x000B01A4 File Offset: 0x000AE3A4
	protected override void Update()
	{
		base.Update();
		string text2;
		if (SemiFunc.RunIsLevel())
		{
			if (!RoundDirector.instance.extractionPointActive)
			{
				base.Hide();
			}
			int num = RoundDirector.instance.currentHaul;
			int extractionHaulGoal = RoundDirector.instance.extractionHaulGoal;
			num = Mathf.Max(0, num);
			this.currentHaulValue = num;
			string text = "<color=#558B2F>$</color>";
			text2 = string.Concat(new string[]
			{
				"<size=30>",
				text,
				SemiFunc.DollarGetString(num),
				"<color=#616161> <size=45>/</size> </color>",
				text,
				"<u>",
				SemiFunc.DollarGetString(extractionHaulGoal)
			});
			if (this.currentHaulValue > this.prevHaulValue)
			{
				base.SemiUISpringShakeY(10f, 10f, 0.3f);
				base.SemiUISpringScale(0.05f, 5f, 0.2f);
				base.SemiUITextFlashColor(Color.green, 0.2f);
				this.prevHaulValue = this.currentHaulValue;
			}
			if (this.currentHaulValue < this.prevHaulValue)
			{
				base.SemiUISpringShakeY(10f, 10f, 0.3f);
				base.SemiUISpringScale(0.05f, 5f, 0.2f);
				base.SemiUITextFlashColor(Color.red, 0.2f);
				this.prevHaulValue = this.currentHaulValue;
			}
		}
		else
		{
			text2 = SemiFunc.DollarGetString(SemiFunc.StatGetRunCurrency());
			base.Hide();
		}
		this.Text.text = text2;
	}

	// Token: 0x04002226 RID: 8742
	private TextMeshProUGUI Text;

	// Token: 0x04002227 RID: 8743
	public static HaulUI instance;

	// Token: 0x04002228 RID: 8744
	private int prevHaulValue;

	// Token: 0x04002229 RID: 8745
	private int currentHaulValue;
}
