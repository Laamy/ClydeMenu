using System;
using UnityEngine;

// Token: 0x020001DE RID: 478
public class DebugAxelTemp : MonoBehaviour
{
	// Token: 0x06001063 RID: 4195 RVA: 0x000978A8 File Offset: 0x00095AA8
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			this.clipData = new float[this.loopClipLength];
			for (int i = 0; i < this.clipData.Length; i++)
			{
				this.clipData[i] = Random.Range(-1f, 1f);
			}
			AudioClip.Create("Speech Loop", this.loopClipLength, 1, this.sampleRate, true, new AudioClip.PCMReaderCallback(this.callback_audioRead));
		}
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x00097920 File Offset: 0x00095B20
	private void callback_audioRead(float[] output)
	{
		for (int i = 0; i < output.Length; i++)
		{
			output[i] = this.clipData[i];
		}
	}

	// Token: 0x04001BFE RID: 7166
	private int loopClipLength = 4096;

	// Token: 0x04001BFF RID: 7167
	private int sampleRate = 11025;

	// Token: 0x04001C00 RID: 7168
	private float[] clipData;
}
