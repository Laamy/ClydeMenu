using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000AE RID: 174
public class UIScanlines : MonoBehaviour
{
	// Token: 0x060006CD RID: 1741 RVA: 0x00041970 File Offset: 0x0003FB70
	private void Start()
	{
		this.image = base.GetComponent<Image>();
		this.originalAlpha = this.image.color.a;
		this.parentText = base.GetComponentInParent<TextMeshProUGUI>();
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x000419A0 File Offset: 0x0003FBA0
	private void Update()
	{
		if (!this.parentText)
		{
			return;
		}
		if (this.changeColorTimer <= 0f)
		{
			Color color = this.parentText.color;
			this.image.color = new Color(color.r, color.g, color.b, this.originalAlpha);
			this.changeColorTimer = 0.03f;
			return;
		}
		this.changeColorTimer -= Time.deltaTime;
	}

	// Token: 0x04000B90 RID: 2960
	private TextMeshProUGUI parentText;

	// Token: 0x04000B91 RID: 2961
	private float originalAlpha;

	// Token: 0x04000B92 RID: 2962
	private Image image;

	// Token: 0x04000B93 RID: 2963
	private float changeColorTimer;
}
