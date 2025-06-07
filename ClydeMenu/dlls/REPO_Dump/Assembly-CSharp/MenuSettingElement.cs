using System;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class MenuSettingElement : MonoBehaviour
{
	// Token: 0x06001188 RID: 4488 RVA: 0x0009FFA1 File Offset: 0x0009E1A1
	private void Start()
	{
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.parentPage.settingElements.Add(this);
		this.settingElementID = this.parentPage.settingElements.Count;
	}

	// Token: 0x06001189 RID: 4489 RVA: 0x0009FFD6 File Offset: 0x0009E1D6
	private void OnDestroy()
	{
		if (this.parentPage)
		{
			this.parentPage.settingElements.Remove(this);
		}
	}

	// Token: 0x04001DC2 RID: 7618
	private MenuPage parentPage;

	// Token: 0x04001DC3 RID: 7619
	internal int settingElementID;
}
