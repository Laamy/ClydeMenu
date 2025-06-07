using System;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class GraphicsButtonGlitchLoop : MonoBehaviour
{
	// Token: 0x06000960 RID: 2400 RVA: 0x000588CC File Offset: 0x00056ACC
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateGlitchLoop();
	}
}
