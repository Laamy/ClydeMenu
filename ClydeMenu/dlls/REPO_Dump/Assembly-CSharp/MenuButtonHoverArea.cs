using System;
using TMPro;
using UnityEngine;

// Token: 0x020001F5 RID: 501
public class MenuButtonHoverArea : MonoBehaviour
{
	// Token: 0x060010F8 RID: 4344 RVA: 0x0009C292 File Offset: 0x0009A492
	private void Start()
	{
		this.menuButton = base.GetComponentInParent<MenuButton>();
		this.rectTransform = base.GetComponent<RectTransform>();
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x0009C2AC File Offset: 0x0009A4AC
	private void Update()
	{
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x0009C2AE File Offset: 0x0009A4AE
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		this.rectTransform = base.GetComponent<RectTransform>();
	}

	// Token: 0x04001CD5 RID: 7381
	private MenuButton menuButton;

	// Token: 0x04001CD6 RID: 7382
	public TextMeshProUGUI text;

	// Token: 0x04001CD7 RID: 7383
	private RectTransform rectTransform;
}
