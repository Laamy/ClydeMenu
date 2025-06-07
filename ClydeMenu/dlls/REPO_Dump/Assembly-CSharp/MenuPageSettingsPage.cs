using System;
using UnityEngine;

// Token: 0x02000224 RID: 548
public class MenuPageSettingsPage : MonoBehaviour
{
	// Token: 0x06001234 RID: 4660 RVA: 0x000A4BBE File Offset: 0x000A2DBE
	private void Start()
	{
		this.menuPage = base.GetComponent<MenuPage>();
	}

	// Token: 0x06001235 RID: 4661 RVA: 0x000A4BCC File Offset: 0x000A2DCC
	private void Update()
	{
		if (this.menuPage.currentPageState == MenuPage.PageState.Closing && !this.saveSettings)
		{
			this.SaveSettings();
			this.saveSettings = true;
		}
	}

	// Token: 0x06001236 RID: 4662 RVA: 0x000A4BF4 File Offset: 0x000A2DF4
	public void ResetSettings()
	{
		DataDirector.instance.ResetSettingTypeToDefault(this.settingType);
		MenuManager.instance.PageCloseAllAddedOnTop();
		MenuManager.instance.PageAddOnTop(this.menuPage.menuPageIndex);
		if (this.settingType == DataDirector.SettingType.Graphics)
		{
			GraphicsManager.instance.UpdateAll();
			return;
		}
		if (this.settingType == DataDirector.SettingType.Gameplay)
		{
			GameplayManager.instance.UpdateAll();
			return;
		}
		if (this.settingType == DataDirector.SettingType.Audio)
		{
			AudioManager.instance.UpdateAll();
		}
	}

	// Token: 0x06001237 RID: 4663 RVA: 0x000A4C6B File Offset: 0x000A2E6B
	public void SaveSettings()
	{
		DataDirector.instance.SaveSettings();
	}

	// Token: 0x04001EAE RID: 7854
	private MenuPage menuPage;

	// Token: 0x04001EAF RID: 7855
	public DataDirector.SettingType settingType;

	// Token: 0x04001EB0 RID: 7856
	private bool saveSettings;
}
