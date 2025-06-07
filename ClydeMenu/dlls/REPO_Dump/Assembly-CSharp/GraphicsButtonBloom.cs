using System;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class GraphicsButtonBloom : MonoBehaviour
{
	// Token: 0x0600095A RID: 2394 RVA: 0x00058890 File Offset: 0x00056A90
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateBloom();
	}
}
