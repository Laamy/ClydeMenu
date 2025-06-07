using System;
using UnityEngine;

// Token: 0x0200011B RID: 283
public class GraphicsButtonLightDistance : MonoBehaviour
{
	// Token: 0x06000966 RID: 2406 RVA: 0x00058908 File Offset: 0x00056B08
	public void ButtonPress()
	{
		GraphicsManager.instance.UpdateLightDistance();
	}
}
