using System;
using UnityEngine;

// Token: 0x020001FB RID: 507
public class MenuKeybindToggle : MonoBehaviour
{
	// Token: 0x0600111F RID: 4383 RVA: 0x0009CE47 File Offset: 0x0009B047
	public void ToggleRebind1()
	{
		InputManager.instance.InputToggleRebind(this.inputKey, true);
	}

	// Token: 0x06001120 RID: 4384 RVA: 0x0009CE5A File Offset: 0x0009B05A
	public void ToggleRebind2()
	{
		InputManager.instance.InputToggleRebind(this.inputKey, false);
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x0009CE70 File Offset: 0x0009B070
	public void FetchSetting()
	{
		MenuTwoOptions component = base.GetComponent<MenuTwoOptions>();
		MenuKeybindToggle component2 = base.GetComponent<MenuKeybindToggle>();
		component.startSettingFetch = InputManager.instance.InputToggleGet(component2.inputKey);
	}

	// Token: 0x04001D04 RID: 7428
	public InputKey inputKey;
}
