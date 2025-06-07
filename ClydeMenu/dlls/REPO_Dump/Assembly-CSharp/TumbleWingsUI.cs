using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000186 RID: 390
public class TumbleWingsUI : SemiUI
{
	// Token: 0x06000D5C RID: 3420 RVA: 0x00074F71 File Offset: 0x00073171
	protected override void Start()
	{
		base.Start();
		TumbleWingsUI.instance = this;
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x00074F80 File Offset: 0x00073180
	protected override void Update()
	{
		base.Update();
		if (!PlayerAvatar.instance.isTumbling || PlayerAvatar.instance.isDisabled)
		{
			base.Hide();
		}
		if (!PlayerAvatar.instance.upgradeTumbleWingsVisualsActive || (this.itemUpgradePlayerTumbleWingsLogic && this.itemUpgradePlayerTumbleWingsLogic.tumbleWingTimer < 0f))
		{
			base.Hide();
		}
		else
		{
			this.imageBar.rectTransform.localScale = new Vector3(this.itemUpgradePlayerTumbleWingsLogic.tumbleWingTimer / 1f, 1f, 1f);
		}
		float num = -54f;
		float num2 = 50f;
		float num3 = Mathf.Sin(Time.time * num2) * num;
		this.rectTransformLeftWing.localRotation = Quaternion.Euler(0f, 0f, -24f - num3);
		this.rectTransformRightWing.localRotation = Quaternion.Euler(0f, 0f, 24f + num3);
	}

	// Token: 0x04001544 RID: 5444
	public RawImage imageBar;

	// Token: 0x04001545 RID: 5445
	public RectTransform rectTransformLeftWing;

	// Token: 0x04001546 RID: 5446
	public RectTransform rectTransformRightWing;

	// Token: 0x04001547 RID: 5447
	public static TumbleWingsUI instance;

	// Token: 0x04001548 RID: 5448
	internal ItemUpgradePlayerTumbleWingsLogic itemUpgradePlayerTumbleWingsLogic;
}
