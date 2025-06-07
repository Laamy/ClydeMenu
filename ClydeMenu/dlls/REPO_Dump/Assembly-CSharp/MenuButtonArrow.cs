using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200020F RID: 527
public class MenuButtonArrow : MonoBehaviour
{
	// Token: 0x060011B6 RID: 4534 RVA: 0x000A197C File Offset: 0x0009FB7C
	private void Awake()
	{
		this.menuElementHover = base.GetComponentInChildren<MenuElementHover>();
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x060011B7 RID: 4535 RVA: 0x000A1998 File Offset: 0x0009FB98
	private void Update()
	{
		float num;
		if (this.menuElementHover.isHovering)
		{
			num = SemiFunc.SpringFloatGet(this.hoverSpring, 1f, -1f);
		}
		else
		{
			num = SemiFunc.SpringFloatGet(this.hoverSpring, 0f, -1f);
		}
		this.backgroundRect.localScale = new Vector3(1f + num * 0.25f, 1f + num * 0.25f, 1f);
		if (this.menuElementHover.isHovering && (SemiFunc.InputDown(InputKey.Confirm) || Input.GetMouseButtonDown(0)))
		{
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, -1f, -1f, false);
			this.hoverSpring.springVelocity += 50f;
			this.onClick.Invoke();
		}
		if (this.hideTimer > 0f)
		{
			this.hideTimer -= Time.deltaTime;
			this.menuElementHover.Disable();
			this.canvasGroup.alpha = Mathf.Lerp(this.canvasGroup.alpha, 0f, Time.deltaTime * 15f);
			return;
		}
		this.canvasGroup.alpha = Mathf.Lerp(this.canvasGroup.alpha, 1f, Time.deltaTime * 15f);
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x000A1AE7 File Offset: 0x0009FCE7
	public void Hide()
	{
		this.hideTimer = 0.1f;
	}

	// Token: 0x060011B9 RID: 4537 RVA: 0x000A1AF4 File Offset: 0x0009FCF4
	public void HideSetInstant()
	{
		this.Hide();
		this.canvasGroup.alpha = 0f;
	}

	// Token: 0x04001E13 RID: 7699
	private MenuElementHover menuElementHover;

	// Token: 0x04001E14 RID: 7700
	private CanvasGroup canvasGroup;

	// Token: 0x04001E15 RID: 7701
	public RectTransform backgroundRect;

	// Token: 0x04001E16 RID: 7702
	public SpringFloat hoverSpring;

	// Token: 0x04001E17 RID: 7703
	private float hideTimer;

	// Token: 0x04001E18 RID: 7704
	[Space]
	public UnityEvent onClick;
}
