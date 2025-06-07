using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000235 RID: 565
public class Blink : MonoBehaviour
{
	// Token: 0x06001293 RID: 4755 RVA: 0x000A70C4 File Offset: 0x000A52C4
	private void Update()
	{
		if (this.blinkTimer <= 0f)
		{
			if (this.targetImage.enabled)
			{
				this.targetImage.enabled = false;
			}
			else
			{
				this.targetImage.enabled = true;
			}
			this.blinkTimer = this.blinkTime;
			return;
		}
		this.blinkTimer -= Time.deltaTime;
	}

	// Token: 0x04001F8C RID: 8076
	public Image targetImage;

	// Token: 0x04001F8D RID: 8077
	public float blinkTime;

	// Token: 0x04001F8E RID: 8078
	private float blinkTimer;
}
