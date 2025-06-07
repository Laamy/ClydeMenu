using System;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class GraphicsButtonChromaticAberration : MonoBehaviour
{
	// Token: 0x0600095C RID: 2396 RVA: 0x000588A4 File Offset: 0x00056AA4
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateChromaticAberration();
	}
}
