using System;
using UnityEngine;

// Token: 0x0200026B RID: 619
public class HUDCanvas : MonoBehaviour
{
	// Token: 0x060013B0 RID: 5040 RVA: 0x000AEE84 File Offset: 0x000AD084
	private void Awake()
	{
		HUDCanvas.instance = this;
		this.rect = base.GetComponent<RectTransform>();
	}

	// Token: 0x040021CB RID: 8651
	public static HUDCanvas instance;

	// Token: 0x040021CC RID: 8652
	internal RectTransform rect;
}
