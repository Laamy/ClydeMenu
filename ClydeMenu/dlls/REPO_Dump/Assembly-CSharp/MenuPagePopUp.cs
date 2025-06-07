using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000221 RID: 545
public class MenuPagePopUp : MonoBehaviour
{
	// Token: 0x06001222 RID: 4642 RVA: 0x000A41A4 File Offset: 0x000A23A4
	private void Start()
	{
		MenuPagePopUp.instance = this;
		this.menuPage = base.GetComponent<MenuPage>();
		this.bodyTextMesh.richText = this.richText;
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x000A41C9 File Offset: 0x000A23C9
	private void Update()
	{
		if (this.okButton.buttonText.text != this.okButton.buttonTextString)
		{
			this.okButton.buttonText.text = this.okButton.buttonTextString;
		}
	}

	// Token: 0x06001224 RID: 4644 RVA: 0x000A4208 File Offset: 0x000A2408
	public void ButtonEvent()
	{
		MenuManager.instance.PageReactivatePageUnderThisPage(this.menuPage);
		MenuManager.instance.MenuEffectPopUpClose();
		this.menuPage.PageStateSet(MenuPage.PageState.Closing);
	}

	// Token: 0x04001E96 RID: 7830
	public static MenuPagePopUp instance;

	// Token: 0x04001E97 RID: 7831
	internal MenuPage menuPage;

	// Token: 0x04001E98 RID: 7832
	internal UnityEvent option1Event;

	// Token: 0x04001E99 RID: 7833
	internal UnityEvent option2Event;

	// Token: 0x04001E9A RID: 7834
	public TextMeshProUGUI bodyTextMesh;

	// Token: 0x04001E9B RID: 7835
	public MenuButton okButton;

	// Token: 0x04001E9C RID: 7836
	internal bool richText = true;
}
