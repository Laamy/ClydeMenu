using System;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class MenuPageSettingsControls : MonoBehaviour
{
	// Token: 0x06001230 RID: 4656 RVA: 0x000A4B75 File Offset: 0x000A2D75
	private void Start()
	{
		this.menuPage = base.GetComponent<MenuPage>();
	}

	// Token: 0x06001231 RID: 4657 RVA: 0x000A4B83 File Offset: 0x000A2D83
	public void ResetControls()
	{
		InputManager.instance.LoadKeyBindings("DefaultKeyBindings.es3");
		MenuManager.instance.PageCloseAllAddedOnTop();
		MenuManager.instance.PageAddOnTop(MenuPageIndex.SettingsControls);
	}

	// Token: 0x06001232 RID: 4658 RVA: 0x000A4BAA File Offset: 0x000A2DAA
	public void SaveControls()
	{
		InputManager.instance.SaveCurrentKeyBindings();
	}

	// Token: 0x04001EAD RID: 7853
	private MenuPage menuPage;
}
