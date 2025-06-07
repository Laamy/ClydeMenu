using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000268 RID: 616
public class FadeOverlay : MonoBehaviour
{
	// Token: 0x060013A6 RID: 5030 RVA: 0x000AED08 File Offset: 0x000ACF08
	private void Awake()
	{
		FadeOverlay.Instance = this;
	}

	// Token: 0x060013A7 RID: 5031 RVA: 0x000AED10 File Offset: 0x000ACF10
	private void Update()
	{
		if (GameDirector.instance.currentState == GameDirector.gameState.Load || GameDirector.instance.currentState == GameDirector.gameState.Start || GameDirector.instance.currentState == GameDirector.gameState.Outro || GameDirector.instance.currentState == GameDirector.gameState.End || GameDirector.instance.currentState == GameDirector.gameState.EndWait)
		{
			this.Image.color = new Color32(0, 0, 0, byte.MaxValue);
			return;
		}
		this.IntroLerp += Time.deltaTime * this.IntroSpeed;
		float num = this.IntroCurve.Evaluate(this.IntroLerp);
		this.Image.color = new Color32(0, 0, 0, (byte)(255f * num));
	}

	// Token: 0x040021C1 RID: 8641
	public static FadeOverlay Instance;

	// Token: 0x040021C2 RID: 8642
	public Image Image;

	// Token: 0x040021C3 RID: 8643
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x040021C4 RID: 8644
	public float IntroSpeed;

	// Token: 0x040021C5 RID: 8645
	private float IntroLerp;
}
