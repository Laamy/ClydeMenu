using System;
using UnityEngine;

// Token: 0x0200011C RID: 284
public class GraphicsButtonMaxFPS : MonoBehaviour
{
	// Token: 0x06000968 RID: 2408 RVA: 0x0005891C File Offset: 0x00056B1C
	public void ButtonPress()
	{
		GraphicsManager.instance.UpdateMaxFPS();
	}
}
