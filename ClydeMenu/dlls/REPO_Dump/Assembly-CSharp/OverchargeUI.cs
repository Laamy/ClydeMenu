using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000283 RID: 643
public class OverchargeUI : SemiUI
{
	// Token: 0x06001428 RID: 5160 RVA: 0x000B1E2D File Offset: 0x000B002D
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		OverchargeUI.instance = this;
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x000B1E48 File Offset: 0x000B0048
	protected override void Update()
	{
		base.Update();
		if (!PlayerAvatar.instance.isDisabled)
		{
			if (!PhysGrabber.instance || (PhysGrabber.instance && PhysGrabber.instance.physGrabBeamOverChargeFloat <= 0f))
			{
				base.Hide();
			}
		}
		else
		{
			base.Hide();
		}
		if (!this.isHidden)
		{
			float num = Mathf.Ceil(PhysGrabber.instance.physGrabBeamOverChargeFloat * 100f);
			if (!this.roundBar.enabled)
			{
				this.roundBar.enabled = true;
			}
			this.roundBar.fillAmount = num / 100f;
			this.Text.text = num.ToString();
			this.textOverchargeMax.text = "<b><color=red>/</color></b>" + Mathf.Ceil(100f).ToString();
			if (PhysGrabber.instance.grabbed && PhysGrabber.instance.grabbedPhysGrabObject && PhysGrabber.instance.grabbedPhysGrabObject.isEnemy)
			{
				if (num > 70f)
				{
					this.warningTimer += Time.deltaTime * 2f;
				}
				if (num > 80f)
				{
					this.warningTimer += Time.deltaTime * 2f;
				}
				if (num > 90f)
				{
					this.warningTimer += Time.deltaTime * 2f;
				}
				if (num > 95f)
				{
					this.warningTimer += Time.deltaTime * 2f;
				}
			}
			else if (num > 90f)
			{
				this.warningTimer += Time.deltaTime * 2f;
			}
			if (this.warningTimer > 1f)
			{
				Color color = new Color(1f, 0.6f, 0f);
				this.soundOverchargeWarning.Play(PlayerAvatar.instance.transform.position, 1f, 1f, 1f, 1f);
				base.SemiUITextFlashColor(color, 0.05f);
				base.SemiUISpringScale(1.1f, 0.25f, 0.05f);
				this.warningTimer = 0f;
				return;
			}
		}
		else if (this.roundBar.enabled)
		{
			this.roundBar.enabled = false;
		}
	}

	// Token: 0x04002279 RID: 8825
	private TextMeshProUGUI Text;

	// Token: 0x0400227A RID: 8826
	public static OverchargeUI instance;

	// Token: 0x0400227B RID: 8827
	public TextMeshProUGUI textOverchargeMax;

	// Token: 0x0400227C RID: 8828
	private float warningTimer;

	// Token: 0x0400227D RID: 8829
	public Sound soundOverchargeWarning;

	// Token: 0x0400227E RID: 8830
	public Image roundBar;

	// Token: 0x0400227F RID: 8831
	public OverchargeRoundBarUI overchargeRoundBarUI;
}
