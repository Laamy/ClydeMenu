using System;
using UnityEngine;

// Token: 0x0200005E RID: 94
public class EnemyHeadLoop : MonoBehaviour
{
	// Token: 0x06000313 RID: 787 RVA: 0x0001E90C File Offset: 0x0001CB0C
	private void Update()
	{
		if (this.Enemy.PlayerRoom.SameLocal || this.Enemy.OnScreen.OnScreenLocal)
		{
			if (!this.Active)
			{
				this.AudioSource.Play();
				this.Active = true;
			}
			if (this.AudioSource.volume < this.VolumeMax)
			{
				this.AudioSource.volume += this.FadeInSpeed * Time.deltaTime;
				this.AudioSource.volume = Mathf.Min(this.AudioSource.volume, this.VolumeMax);
				return;
			}
		}
		else if (this.Active && this.AudioSource.volume > 0f)
		{
			this.AudioSource.volume -= this.FadeOutSpeed * Time.deltaTime;
			if (this.AudioSource.volume <= 0f)
			{
				this.AudioSource.Stop();
				this.Active = false;
			}
		}
	}

	// Token: 0x0400055F RID: 1375
	public Enemy Enemy;

	// Token: 0x04000560 RID: 1376
	public AudioSource AudioSource;

	// Token: 0x04000561 RID: 1377
	[Space]
	public float VolumeMax;

	// Token: 0x04000562 RID: 1378
	public float FadeInSpeed;

	// Token: 0x04000563 RID: 1379
	public float FadeOutSpeed;

	// Token: 0x04000564 RID: 1380
	private bool Active;
}
