using System;
using UnityEngine;

// Token: 0x020001FA RID: 506
public class MenuInputPercentSetting : MonoBehaviour
{
	// Token: 0x0600111D RID: 4381 RVA: 0x0009CE0A File Offset: 0x0009B00A
	private void Start()
	{
		this.menuSlider = base.GetComponent<MenuSlider>();
		this.menuSlider.SetBar((float)InputManager.instance.inputPercentSettings[this.setting] / 100f);
	}

	// Token: 0x04001D02 RID: 7426
	public InputPercentSetting setting;

	// Token: 0x04001D03 RID: 7427
	private MenuSlider menuSlider;
}
