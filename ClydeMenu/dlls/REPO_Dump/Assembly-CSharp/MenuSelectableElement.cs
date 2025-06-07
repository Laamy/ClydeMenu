using System;
using UnityEngine;

// Token: 0x02000204 RID: 516
public class MenuSelectableElement : MonoBehaviour
{
	// Token: 0x0600117A RID: 4474 RVA: 0x0009F718 File Offset: 0x0009D918
	private void Start()
	{
		this.menuID = SemiFunc.MenuGetSelectableID(base.gameObject);
		this.rectTransform = base.GetComponent<RectTransform>();
		this.parentPage = base.GetComponentInParent<MenuPage>();
		if (this.parentPage)
		{
			this.parentPage.selectableElements.Add(this);
			if (this.rectTransform.localPosition.y < this.parentPage.bottomElementYPos)
			{
				this.parentPage.bottomElementYPos = this.rectTransform.localPosition.y;
			}
		}
		this.isInScrollBox = false;
		MenuScrollBox componentInParent = base.GetComponentInParent<MenuScrollBox>();
		if (componentInParent)
		{
			this.isInScrollBox = true;
			this.menuScrollBox = componentInParent;
		}
	}

	// Token: 0x04001DA4 RID: 7588
	internal string menuID;

	// Token: 0x04001DA5 RID: 7589
	internal RectTransform rectTransform;

	// Token: 0x04001DA6 RID: 7590
	internal MenuPage parentPage;

	// Token: 0x04001DA7 RID: 7591
	internal bool isInScrollBox;

	// Token: 0x04001DA8 RID: 7592
	internal MenuScrollBox menuScrollBox;
}
