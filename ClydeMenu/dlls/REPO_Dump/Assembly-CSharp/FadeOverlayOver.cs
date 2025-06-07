using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000269 RID: 617
public class FadeOverlayOver : MonoBehaviour
{
	// Token: 0x060013A9 RID: 5033 RVA: 0x000AEDCE File Offset: 0x000ACFCE
	private void Awake()
	{
		FadeOverlayOver.Instance = this;
	}

	// Token: 0x060013AA RID: 5034 RVA: 0x000AEDD8 File Offset: 0x000ACFD8
	private void Update()
	{
		if (GameDirector.instance.currentState == GameDirector.gameState.Load || GameDirector.instance.currentState == GameDirector.gameState.End || GameDirector.instance.currentState == GameDirector.gameState.EndWait)
		{
			this.Image.color = new Color32(0, 0, 0, byte.MaxValue);
			return;
		}
		this.Image.color = new Color32(0, 0, 0, 0);
	}

	// Token: 0x040021C6 RID: 8646
	public static FadeOverlayOver Instance;

	// Token: 0x040021C7 RID: 8647
	public Image Image;
}
