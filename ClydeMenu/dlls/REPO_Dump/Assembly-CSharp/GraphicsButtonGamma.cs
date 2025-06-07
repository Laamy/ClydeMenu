using System;
using UnityEngine;

// Token: 0x02000117 RID: 279
public class GraphicsButtonGamma : MonoBehaviour
{
	// Token: 0x0600095E RID: 2398 RVA: 0x000588B8 File Offset: 0x00056AB8
	public void ButtonPress()
	{
		GraphicsManager.instance.UpdateGamma();
	}
}
