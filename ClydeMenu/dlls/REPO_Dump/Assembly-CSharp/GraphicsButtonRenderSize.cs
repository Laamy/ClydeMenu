using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class GraphicsButtonRenderSize : MonoBehaviour
{
	// Token: 0x0600096C RID: 2412 RVA: 0x00058944 File Offset: 0x00056B44
	public void ButtonPress()
	{
		GraphicsManager.instance.UpdateRenderSize();
	}
}
