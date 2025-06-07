using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000205 RID: 517
public class MenuSelectionBoxTop : MonoBehaviour
{
	// Token: 0x0600117C RID: 4476 RVA: 0x0009F7D0 File Offset: 0x0009D9D0
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.rawImage = base.GetComponentInChildren<RawImage>();
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x0009F7EC File Offset: 0x0009D9EC
	private void Update()
	{
		MenuSelectionBox activeSelectionBox = MenuManager.instance.activeSelectionBox;
		if (activeSelectionBox)
		{
			this.rectTransform.localPosition = activeSelectionBox.rectTransform.position - base.transform.parent.position;
			base.transform.localScale = activeSelectionBox.rectTransform.localScale;
			this.rawImage.color = activeSelectionBox.rawImage.color * 1.5f;
			this.fadeDone = false;
			return;
		}
		if (!this.fadeDone)
		{
			this.rawImage.color = new Color(1f, 1f, 1f, this.rawImage.color.a - Time.deltaTime);
			if (this.rawImage.color.a <= 0f)
			{
				this.fadeDone = true;
			}
		}
	}

	// Token: 0x04001DA9 RID: 7593
	private RectTransform rectTransform;

	// Token: 0x04001DAA RID: 7594
	private RawImage rawImage;

	// Token: 0x04001DAB RID: 7595
	private bool fadeDone;
}
