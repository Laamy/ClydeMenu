using System;
using UnityEngine;

// Token: 0x02000208 RID: 520
public class MenuSetting : MonoBehaviour
{
	// Token: 0x0600118B RID: 4491 RVA: 0x0009FFFF File Offset: 0x0009E1FF
	private void Start()
	{
		this.FetchValues();
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x000A0007 File Offset: 0x0009E207
	public void FetchValues()
	{
		this.settingValue = DataDirector.instance.SettingValueFetch(this.setting);
		this.settingName = DataDirector.instance.SettingNameGet(this.setting);
	}

	// Token: 0x04001DC4 RID: 7620
	public DataDirector.Setting setting;

	// Token: 0x04001DC5 RID: 7621
	internal string settingName;

	// Token: 0x04001DC6 RID: 7622
	internal int settingValue;
}
