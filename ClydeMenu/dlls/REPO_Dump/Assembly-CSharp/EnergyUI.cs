using System;
using TMPro;
using UnityEngine;

// Token: 0x02000276 RID: 630
public class EnergyUI : SemiUI
{
	// Token: 0x060013E0 RID: 5088 RVA: 0x000B003E File Offset: 0x000AE23E
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		EnergyUI.instance = this;
		this.textEnergyMax = base.transform.Find("EnergyMax").GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x060013E1 RID: 5089 RVA: 0x000B0074 File Offset: 0x000AE274
	protected override void Update()
	{
		base.Update();
		this.Text.text = Mathf.Ceil(PlayerController.instance.EnergyCurrent).ToString();
		this.textEnergyMax.text = "<b><color=orange>/</color></b>" + Mathf.Ceil(PlayerController.instance.EnergyStart).ToString();
	}

	// Token: 0x04002221 RID: 8737
	private TextMeshProUGUI Text;

	// Token: 0x04002222 RID: 8738
	public static EnergyUI instance;

	// Token: 0x04002223 RID: 8739
	private TextMeshProUGUI textEnergyMax;
}
