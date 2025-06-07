using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000227 RID: 551
public class MenuElementRegion : MonoBehaviour
{
	// Token: 0x06001245 RID: 4677 RVA: 0x000A5147 File Offset: 0x000A3347
	private void Start()
	{
		this.menuElementHover = base.GetComponent<MenuElementHover>();
		this.initialFadeAlpha = this.fadePanel.color.a;
		this.UpdateIntro();
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x000A5174 File Offset: 0x000A3374
	private void Update()
	{
		this.UpdateIntro();
		if (this.menuElementHover.isHovering)
		{
			Color color = this.fadePanel.color;
			color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * 10f);
			this.fadePanel.color = color;
			if (SemiFunc.InputDown(InputKey.Confirm) || Input.GetMouseButtonDown(0))
			{
				this.parentPage.PickRegion(this.regionCode);
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, -1f, -1f, false);
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

	// Token: 0x06001247 RID: 4679 RVA: 0x000A5244 File Offset: 0x000A3444
	private void UpdateIntro()
	{
		if (!this.animationSkip)
		{
			this.introLerp += Time.deltaTime * 5f;
			if (this.introLerp > 1f)
			{
				this.animationSkip = true;
			}
			this.animationTransform.anchoredPosition = new Vector3(-this.introCurve.Evaluate(this.introLerp) * 10f, 0f, 0f);
		}
	}

	// Token: 0x04001EBE RID: 7870
	public Image fadePanel;

	// Token: 0x04001EBF RID: 7871
	public MenuPageRegions parentPage;

	// Token: 0x04001EC0 RID: 7872
	private MenuElementHover menuElementHover;

	// Token: 0x04001EC1 RID: 7873
	private float initialFadeAlpha;

	// Token: 0x04001EC2 RID: 7874
	public TextMeshProUGUI textName;

	// Token: 0x04001EC3 RID: 7875
	public TextMeshProUGUI textPing;

	// Token: 0x04001EC4 RID: 7876
	[Space]
	public bool animationSkip;

	// Token: 0x04001EC5 RID: 7877
	public RectTransform animationTransform;

	// Token: 0x04001EC6 RID: 7878
	public AnimationCurve introCurve;

	// Token: 0x04001EC7 RID: 7879
	private float introLerp;

	// Token: 0x04001EC8 RID: 7880
	internal string regionCode = "";
}
