using System;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class GraphicsButtonShadowQuality : MonoBehaviour
{
	// Token: 0x06000970 RID: 2416 RVA: 0x0005896C File Offset: 0x00056B6C
	public void ButtonPress()
	{
		GraphicsManager.instance.UpdateShadowQuality();
	}
}
