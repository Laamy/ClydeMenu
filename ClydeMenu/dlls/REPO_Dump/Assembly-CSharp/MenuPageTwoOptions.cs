using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000220 RID: 544
public class MenuPageTwoOptions : MonoBehaviour
{
	// Token: 0x0600121D RID: 4637 RVA: 0x000A406B File Offset: 0x000A226B
	private void Start()
	{
		MenuPageTwoOptions.instance = this;
		this.menuPage = base.GetComponent<MenuPage>();
		this.bodyTextMesh.richText = this.richText;
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x000A4090 File Offset: 0x000A2290
	private void Update()
	{
		if (SemiFunc.InputDown(InputKey.Back) && MenuManager.instance.currentMenuPageIndex == MenuPageIndex.PopUpTwoOptions)
		{
			this.ButtonEventOption2();
		}
		if (this.option1Button.buttonText.text != this.option1Button.buttonTextString)
		{
			this.option1Button.buttonText.text = this.option1Button.buttonTextString;
		}
		if (this.option2Button.buttonText.text != this.option2Button.buttonTextString)
		{
			this.option2Button.buttonText.text = this.option2Button.buttonTextString;
		}
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x000A4133 File Offset: 0x000A2333
	public void ButtonEventOption1()
	{
		if (this.option1Event != null)
		{
			this.option1Event.Invoke();
		}
		MenuManager.instance.PageReactivatePageUnderThisPage(this.menuPage);
		this.menuPage.PageStateSet(MenuPage.PageState.Closing);
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x000A4164 File Offset: 0x000A2364
	public void ButtonEventOption2()
	{
		if (this.option2Event != null)
		{
			this.option2Event.Invoke();
		}
		MenuManager.instance.PageReactivatePageUnderThisPage(this.menuPage);
		this.menuPage.PageStateSet(MenuPage.PageState.Closing);
	}

	// Token: 0x04001E8E RID: 7822
	public static MenuPageTwoOptions instance;

	// Token: 0x04001E8F RID: 7823
	internal MenuPage menuPage;

	// Token: 0x04001E90 RID: 7824
	internal UnityEvent option1Event;

	// Token: 0x04001E91 RID: 7825
	internal UnityEvent option2Event;

	// Token: 0x04001E92 RID: 7826
	public TextMeshProUGUI bodyTextMesh;

	// Token: 0x04001E93 RID: 7827
	public MenuButton option1Button;

	// Token: 0x04001E94 RID: 7828
	public MenuButton option2Button;

	// Token: 0x04001E95 RID: 7829
	internal bool richText = true;
}
