using System;
using UnityEngine;

// Token: 0x020002BD RID: 701
public class UraniumCloudGeigerLoop : MonoBehaviour
{
	// Token: 0x06001614 RID: 5652 RVA: 0x000C1DD4 File Offset: 0x000BFFD4
	private void Update()
	{
		this.geigerSoundLoop.PlayLoop(this.isPlaying, 5f, 0.5f, 1f);
		if (this.timer > 0f)
		{
			this.isPlaying = true;
			this.timer -= Time.deltaTime;
			return;
		}
		this.isPlaying = false;
	}

	// Token: 0x04002646 RID: 9798
	public Sound geigerSoundLoop;

	// Token: 0x04002647 RID: 9799
	private float timer = 6f;

	// Token: 0x04002648 RID: 9800
	private bool isPlaying;
}
