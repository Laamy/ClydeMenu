using System;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class GraphicsButtonVsync : MonoBehaviour
{
	// Token: 0x06000972 RID: 2418 RVA: 0x00058980 File Offset: 0x00056B80
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateVsync();
	}
}
