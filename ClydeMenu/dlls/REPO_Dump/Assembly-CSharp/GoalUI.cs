using System;
using TMPro;
using UnityEngine;

// Token: 0x02000277 RID: 631
public class GoalUI : SemiUI
{
	// Token: 0x060013E3 RID: 5091 RVA: 0x000B00DD File Offset: 0x000AE2DD
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		GoalUI.instance = this;
	}

	// Token: 0x060013E4 RID: 5092 RVA: 0x000B00F8 File Offset: 0x000AE2F8
	protected override void Update()
	{
		base.Update();
		if (SemiFunc.RunIsLevel() || SemiFunc.RunIsTutorial())
		{
			int extractionPoints = RoundDirector.instance.extractionPoints;
			int extractionPointsCompleted = RoundDirector.instance.extractionPointsCompleted;
			this.Text.text = extractionPointsCompleted.ToString() + "<color=#7D250B> <size=45>/</size> </color><b>" + extractionPoints.ToString();
		}
		else
		{
			base.Hide();
		}
		if (HaulUI.instance.hideTimer > 0f)
		{
			base.SemiUIScoot(new Vector2(0f, 45f));
		}
	}

	// Token: 0x04002224 RID: 8740
	private TextMeshProUGUI Text;

	// Token: 0x04002225 RID: 8741
	public static GoalUI instance;
}
