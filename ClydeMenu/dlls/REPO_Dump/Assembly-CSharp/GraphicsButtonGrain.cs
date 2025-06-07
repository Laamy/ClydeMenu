using System;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class GraphicsButtonGrain : MonoBehaviour
{
	// Token: 0x06000962 RID: 2402 RVA: 0x000588E0 File Offset: 0x00056AE0
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateGrain();
	}
}
