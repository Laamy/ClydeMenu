using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021C RID: 540
public class MenuPageColor : MonoBehaviour
{
	// Token: 0x060011F6 RID: 4598 RVA: 0x000A2F80 File Offset: 0x000A1180
	private void Start()
	{
		this.menuPage = base.GetComponent<MenuPage>();
		List<Color> playerColors = AssetManager.instance.playerColors;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < playerColors.Count; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.colorButtonPrefab, this.colorButtonHolder);
			MenuButtonColor component = gameObject.GetComponent<MenuButtonColor>();
			MenuButton component2 = gameObject.GetComponent<MenuButton>();
			component.colorID = i;
			component.color = playerColors[i];
			component2.colorNormal = playerColors[i] + Color.black * 0.5f;
			component2.colorHover = playerColors[i];
			component2.colorClick = playerColors[i] + Color.white * 0.95f;
			RectTransform component3 = gameObject.GetComponent<RectTransform>();
			component3.SetSiblingIndex(0);
			component3.anchoredPosition = new Vector2((float)num, (float)(224 + num2));
			num += 38;
			if ((float)num > this.colorButtonHolder.rect.width)
			{
				num = 0;
				num2 -= 30;
			}
		}
		Object.Destroy(this.colorButtonPrefab);
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x000A3091 File Offset: 0x000A1291
	public void SetColor(int colorID, RectTransform buttonTransform)
	{
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, -1f, -1f, false);
		this.menuColorSelected.SetColor(AssetManager.instance.playerColors[colorID], buttonTransform.position);
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x000A30CB File Offset: 0x000A12CB
	public void ConfirmButton()
	{
		MenuManager.instance.PageReactivatePageUnderThisPage(this.menuPage);
		MenuManager.instance.MenuEffectPopUpClose();
		this.menuPage.PageStateSet(MenuPage.PageState.Closing);
	}

	// Token: 0x04001E67 RID: 7783
	public GameObject colorButtonPrefab;

	// Token: 0x04001E68 RID: 7784
	public RectTransform colorButtonHolder;

	// Token: 0x04001E69 RID: 7785
	public MenuColorSelected menuColorSelected;

	// Token: 0x04001E6A RID: 7786
	private MenuPage menuPage;
}
