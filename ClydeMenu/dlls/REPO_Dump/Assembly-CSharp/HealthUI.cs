using System;
using TMPro;
using UnityEngine;

// Token: 0x02000279 RID: 633
public class HealthUI : SemiUI
{
	// Token: 0x060013E9 RID: 5097 RVA: 0x000B0311 File Offset: 0x000AE511
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		HealthUI.instance = this;
		this.textMaxHealth = base.transform.Find("HealthMax").GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x060013EA RID: 5098 RVA: 0x000B0348 File Offset: 0x000AE548
	protected override void Update()
	{
		base.Update();
		if (this.playerHealth)
		{
			this.playerHealthValue = this.playerHealth.health;
			if (this.playerHealthValue != this.playerHealthPrevious)
			{
				base.SemiUISpringShakeY(20f, 10f, 0.3f);
				Color color = Color.white;
				if (this.playerHealthValue < this.playerHealthPrevious)
				{
					color = Color.red;
				}
				base.SemiUITextFlashColor(color, 0.2f);
				base.SemiUISpringScale(0.3f, 5f, 0.2f);
				this.playerHealthPrevious = this.playerHealthValue;
			}
		}
		if (this.setup)
		{
			if (LevelGenerator.Instance.Generated)
			{
				this.playerHealth = PlayerController.instance.playerAvatarScript.playerHealth;
				this.setup = false;
				return;
			}
		}
		else
		{
			this.Text.text = this.playerHealthValue.ToString();
			this.textMaxHealth.text = "<b><color=#008b20>/</color></b>" + this.playerHealth.maxHealth.ToString();
		}
	}

	// Token: 0x0400222A RID: 8746
	private TextMeshProUGUI Text;

	// Token: 0x0400222B RID: 8747
	private PlayerHealth playerHealth;

	// Token: 0x0400222C RID: 8748
	private bool setup = true;

	// Token: 0x0400222D RID: 8749
	public static HealthUI instance;

	// Token: 0x0400222E RID: 8750
	private int playerHealthValue;

	// Token: 0x0400222F RID: 8751
	private int playerHealthPrevious;

	// Token: 0x04002230 RID: 8752
	private TextMeshProUGUI textMaxHealth;
}
