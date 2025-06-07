using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000216 RID: 534
public class MenuButtonColor : MonoBehaviour
{
	// Token: 0x060011DD RID: 4573 RVA: 0x000A280C File Offset: 0x000A0A0C
	private void Start()
	{
		this.parentPage = base.GetComponentInParent<MenuPage>();
		List<Color> playerColors = AssetManager.instance.playerColors;
		this.color = playerColors[this.colorID];
		this.menuButton = base.GetComponent<MenuButton>();
		this.menuPageColor = base.GetComponentInParent<MenuPageColor>();
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x060011DE RID: 4574 RVA: 0x000A2867 File Offset: 0x000A0A67
	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(0.1f);
		while (this.parentPage.currentPageState != MenuPage.PageState.Active)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (this.color == PlayerAvatar.instance.playerAvatarVisuals.color)
		{
			this.menuPageColor.SetColor(this.colorID, base.GetComponent<RectTransform>());
		}
		yield break;
	}

	// Token: 0x060011DF RID: 4575 RVA: 0x000A2878 File Offset: 0x000A0A78
	private void Update()
	{
		if (this.menuButton.clicked && !this.buttonClicked)
		{
			this.menuPageColor.SetColor(this.colorID, base.GetComponent<RectTransform>());
			PlayerAvatar.instance.PlayerAvatarSetColor(this.colorID);
			this.buttonClicked = true;
		}
		if (this.buttonClicked && !this.menuButton.clicked)
		{
			this.buttonClicked = false;
		}
	}

	// Token: 0x04001E3A RID: 7738
	internal int colorID;

	// Token: 0x04001E3B RID: 7739
	internal Color color = Color.white;

	// Token: 0x04001E3C RID: 7740
	private MenuButton menuButton;

	// Token: 0x04001E3D RID: 7741
	private MenuPageColor menuPageColor;

	// Token: 0x04001E3E RID: 7742
	private MenuPage parentPage;

	// Token: 0x04001E3F RID: 7743
	private bool buttonClicked;
}
