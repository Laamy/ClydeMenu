using System;
using UnityEngine;

// Token: 0x02000284 RID: 644
public class OverlayState : MonoBehaviour
{
	// Token: 0x0600142B RID: 5163 RVA: 0x000B20A0 File Offset: 0x000B02A0
	private void Update()
	{
		if (this.RewindEffect.PlayRewind)
		{
			this.Play.SetActive(false);
			this.Stop.SetActive(false);
			this.Rewind.SetActive(true);
			return;
		}
		if (GameDirector.instance.currentState < GameDirector.gameState.Outro)
		{
			this.Play.SetActive(true);
			this.Stop.SetActive(false);
			this.Rewind.SetActive(false);
			return;
		}
		this.Play.SetActive(false);
		this.Stop.SetActive(true);
		this.Rewind.SetActive(false);
	}

	// Token: 0x04002280 RID: 8832
	public GameObject Play;

	// Token: 0x04002281 RID: 8833
	public GameObject Stop;

	// Token: 0x04002282 RID: 8834
	public GameObject Rewind;

	// Token: 0x04002283 RID: 8835
	[Space]
	public RewindEffect RewindEffect;
}
