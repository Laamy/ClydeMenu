using System;
using UnityEngine;

// Token: 0x0200021A RID: 538
public class MenuElementHover : MonoBehaviour
{
	// Token: 0x060011EF RID: 4591 RVA: 0x000A2D2C File Offset: 0x000A0F2C
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.menuSelectableElement = base.GetComponent<MenuSelectableElement>();
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.buttonPitch = SemiFunc.MenuGetPitchFromYPos(this.rectTransform);
		if (this.menuSelectableElement)
		{
			this.menuID = this.menuSelectableElement.menuID;
		}
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x000A2D8C File Offset: 0x000A0F8C
	private void Update()
	{
		if (this.disableTimer > 0f)
		{
			this.disableTimer -= Time.deltaTime;
			this.isHovering = false;
		}
		else if (SemiFunc.UIMouseHover(this.parentPage, this.rectTransform, this.menuID, 0f, 0f))
		{
			if (!this.isHovering && this.hasHoverEffect)
			{
				MenuManager.instance.MenuEffectHover(this.buttonPitch, -1f);
			}
			this.isHovering = true;
		}
		else if (this.isHovering)
		{
			this.isHovering = false;
		}
		if (this.hasHoverEffect && this.isHovering)
		{
			SemiFunc.MenuSelectionBoxTargetSet(this.parentPage, this.rectTransform, default(Vector2), default(Vector2));
		}
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x000A2E54 File Offset: 0x000A1054
	public void Disable()
	{
		this.disableTimer = 0.1f;
	}

	// Token: 0x04001E56 RID: 7766
	internal bool isHovering;

	// Token: 0x04001E57 RID: 7767
	private RectTransform rectTransform;

	// Token: 0x04001E58 RID: 7768
	private MenuSelectableElement menuSelectableElement;

	// Token: 0x04001E59 RID: 7769
	private MenuPage parentPage;

	// Token: 0x04001E5A RID: 7770
	private float buttonPitch;

	// Token: 0x04001E5B RID: 7771
	public bool hasHoverEffect = true;

	// Token: 0x04001E5C RID: 7772
	internal string menuID = "-1";

	// Token: 0x04001E5D RID: 7773
	private float disableTimer;
}
