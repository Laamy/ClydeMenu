using System;
using UnityEngine;

// Token: 0x02000202 RID: 514
public class MenuScrollBox : MonoBehaviour
{
	// Token: 0x06001176 RID: 4470 RVA: 0x0009F29C File Offset: 0x0009D49C
	private void Start()
	{
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.menuSelectableElement = this.scrollBarBackground.GetComponent<MenuSelectableElement>();
		this.scrollHandleTargetPosition = this.scrollHandle.localPosition.y;
		float num = 0f;
		foreach (object obj in this.scroller)
		{
			RectTransform rectTransform = (RectTransform)obj;
			float num2 = rectTransform.rect.height * rectTransform.pivot.y;
			if (rectTransform.localPosition.y - num2 < num)
			{
				num = rectTransform.localPosition.y - num2;
			}
		}
		this.scrollHeight = Mathf.Abs(num) + this.heightPadding;
		this.scrollerStartPosition = this.scrollHeight + 42f;
		this.scrollerEndPosition = this.scroller.localPosition.y;
		bool flag = true;
		if (this.scrollHeight < this.scrollBarBackground.rect.height)
		{
			this.scrollBar.SetActive(false);
			flag = false;
		}
		if (flag)
		{
			this.parentPage.scrollBoxes++;
		}
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x0009F3E4 File Offset: 0x0009D5E4
	private void Update()
	{
		if (this.parentPage.scrollBoxes > 1)
		{
			if (this.menuElementHover.isHovering)
			{
				this.scrollBoxActive = true;
			}
			else
			{
				this.scrollBoxActive = false;
			}
		}
		if (!this.scrollBar.activeSelf)
		{
			return;
		}
		if (!this.scrollBoxActive)
		{
			return;
		}
		if (Input.GetMouseButton(0) && SemiFunc.UIMouseHover(this.parentPage, this.scrollBarBackground, this.menuSelectableElement.menuID, 0f, 0f))
		{
			float num = SemiFunc.UIMouseGetLocalPositionWithinRectTransform(this.scrollBarBackground).y;
			if (num < this.scrollHandle.sizeDelta.y / 2f)
			{
				num = this.scrollHandle.sizeDelta.y / 2f;
			}
			if (num > this.scrollBarBackground.rect.height - this.scrollHandle.sizeDelta.y / 2f)
			{
				num = this.scrollBarBackground.rect.height - this.scrollHandle.sizeDelta.y / 2f;
			}
			this.scrollHandleTargetPosition = num;
		}
		if (SemiFunc.InputMovementY() != 0f || SemiFunc.InputScrollY() != 0f)
		{
			this.scrollHandleTargetPosition += SemiFunc.InputMovementY() * 20f / (this.scrollHeight * 0.01f);
			this.scrollHandleTargetPosition += SemiFunc.InputScrollY() / (this.scrollHeight * 0.01f);
			if (this.scrollHandleTargetPosition < this.scrollHandle.sizeDelta.y / 2f)
			{
				this.scrollHandleTargetPosition = this.scrollHandle.sizeDelta.y / 2f;
			}
			if (this.scrollHandleTargetPosition > this.scrollBarBackground.rect.height - this.scrollHandle.sizeDelta.y / 2f)
			{
				this.scrollHandleTargetPosition = this.scrollBarBackground.rect.height - this.scrollHandle.sizeDelta.y / 2f;
			}
		}
		this.scrollHandle.localPosition = new Vector3(this.scrollHandle.localPosition.x, Mathf.Lerp(this.scrollHandle.localPosition.y, this.scrollHandleTargetPosition, Time.deltaTime * 20f), this.scrollHandle.localPosition.z);
		this.scrollAmount = this.scrollHandle.localPosition.y / this.scrollBarBackground.rect.height * 1.1f;
		if (this.scrollAmount < 0f)
		{
			this.scrollAmount = 0f;
		}
		if (this.scrollAmount > 1f)
		{
			this.scrollAmount = 1f;
		}
		this.scroller.localPosition = new Vector3(this.scroller.localPosition.x, Mathf.Lerp(this.scrollerStartPosition, this.scrollerEndPosition, this.scrollAmount), this.scroller.localPosition.z);
	}

	// Token: 0x04001D93 RID: 7571
	public RectTransform scrollSize;

	// Token: 0x04001D94 RID: 7572
	public RectTransform scroller;

	// Token: 0x04001D95 RID: 7573
	public RectTransform scrollHandle;

	// Token: 0x04001D96 RID: 7574
	public RectTransform scrollBarBackground;

	// Token: 0x04001D97 RID: 7575
	public GameObject scrollBar;

	// Token: 0x04001D98 RID: 7576
	internal float scrollAmount;

	// Token: 0x04001D99 RID: 7577
	private float scrollAmountTarget;

	// Token: 0x04001D9A RID: 7578
	private float scrollHeight;

	// Token: 0x04001D9B RID: 7579
	internal MenuPage parentPage;

	// Token: 0x04001D9C RID: 7580
	private MenuSelectableElement menuSelectableElement;

	// Token: 0x04001D9D RID: 7581
	public MenuSelectionBox menuSelectionBox;

	// Token: 0x04001D9E RID: 7582
	internal float scrollerStartPosition;

	// Token: 0x04001D9F RID: 7583
	internal float scrollerEndPosition;

	// Token: 0x04001DA0 RID: 7584
	private float scrollHandleTargetPosition;

	// Token: 0x04001DA1 RID: 7585
	public MenuElementHover menuElementHover;

	// Token: 0x04001DA2 RID: 7586
	internal bool scrollBoxActive = true;

	// Token: 0x04001DA3 RID: 7587
	public float heightPadding;
}
