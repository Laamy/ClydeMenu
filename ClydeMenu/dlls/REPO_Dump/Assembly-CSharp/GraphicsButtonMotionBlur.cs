using System;
using UnityEngine;

// Token: 0x0200011D RID: 285
public class GraphicsButtonMotionBlur : MonoBehaviour
{
	// Token: 0x0600096A RID: 2410 RVA: 0x00058930 File Offset: 0x00056B30
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateMotionBlur();
	}
}
