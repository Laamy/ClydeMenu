using System;
using UnityEngine;

// Token: 0x020000BE RID: 190
public class RoachSmashCleanEffect : MonoBehaviour
{
	// Token: 0x06000708 RID: 1800 RVA: 0x00042BE5 File Offset: 0x00040DE5
	private void Start()
	{
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x00042BE7 File Offset: 0x00040DE7
	private void Update()
	{
		if (!this.CleanEffectDone)
		{
			if (this.CleanEffectTimer > this.CleanEffectDelay)
			{
				this.cleanEffect.Clean();
				this.CleanEffectDone = true;
				return;
			}
			this.CleanEffectTimer += Time.deltaTime;
		}
	}

	// Token: 0x04000BF3 RID: 3059
	public CleanEffect cleanEffect;

	// Token: 0x04000BF4 RID: 3060
	public float CleanEffectDelay;

	// Token: 0x04000BF5 RID: 3061
	private float CleanEffectTimer;

	// Token: 0x04000BF6 RID: 3062
	private bool CleanEffectDone;
}
