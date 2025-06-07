using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200021B RID: 539
public class MenuElementSaveFile : MonoBehaviour
{
	// Token: 0x060011F3 RID: 4595 RVA: 0x000A2E7B File Offset: 0x000A107B
	private void Start()
	{
		this.menuElementHover = base.GetComponent<MenuElementHover>();
		this.initialFadeAlpha = this.fadePanel.color.a;
		this.parentPageSaves = base.GetComponentInParent<MenuPageSaves>();
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x000A2EAC File Offset: 0x000A10AC
	private void Update()
	{
		if (this.menuElementHover.isHovering)
		{
			Color color = this.fadePanel.color;
			color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * 10f);
			this.fadePanel.color = color;
			if (SemiFunc.InputDown(InputKey.Confirm) || Input.GetMouseButtonDown(0))
			{
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, -1f, -1f, false);
				this.parentPageSaves.SaveFileSelected(this.saveFileName);
				return;
			}
		}
		else
		{
			Color color2 = this.fadePanel.color;
			color2.a = Mathf.Lerp(color2.a, this.initialFadeAlpha, Time.deltaTime * 10f);
			this.fadePanel.color = color2;
		}
	}

	// Token: 0x04001E5E RID: 7774
	public Image fadePanel;

	// Token: 0x04001E5F RID: 7775
	private MenuElementHover menuElementHover;

	// Token: 0x04001E60 RID: 7776
	private float initialFadeAlpha;

	// Token: 0x04001E61 RID: 7777
	private MenuPageSaves parentPageSaves;

	// Token: 0x04001E62 RID: 7778
	internal string saveFileName;

	// Token: 0x04001E63 RID: 7779
	public TextMeshProUGUI saveFileHeader;

	// Token: 0x04001E64 RID: 7780
	public TextMeshProUGUI saveFileHeaderLevel;

	// Token: 0x04001E65 RID: 7781
	public TextMeshProUGUI saveFileHeaderDate;

	// Token: 0x04001E66 RID: 7782
	public TextMeshProUGUI saveFileInfoRow1;
}
