using System;
using UnityEngine;

// Token: 0x0200011F RID: 287
public class GraphicsButtonShadowDistance : MonoBehaviour
{
	// Token: 0x0600096E RID: 2414 RVA: 0x00058958 File Offset: 0x00056B58
	public void ButtonPress()
	{
		GraphicsManager.instance.UpdateShadowDistance();
	}
}
