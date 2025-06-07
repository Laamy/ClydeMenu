using System;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class GraphicsButtonLensDistortion : MonoBehaviour
{
	// Token: 0x06000964 RID: 2404 RVA: 0x000588F4 File Offset: 0x00056AF4
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateLensDistortion();
	}
}
