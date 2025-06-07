using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000030 RID: 48
public class CameraUpdate : MonoBehaviour
{
	// Token: 0x060000B7 RID: 183 RVA: 0x000072A0 File Offset: 0x000054A0
	private void Update()
	{
		if (this.updateTimer <= -this.updateRate)
		{
			foreach (Camera camera in this.cams)
			{
				camera.enabled = true;
			}
			this.updateTimer += this.updateRate;
			return;
		}
		foreach (Camera camera2 in this.cams)
		{
			camera2.enabled = false;
		}
		this.updateTimer -= Time.deltaTime;
	}

	// Token: 0x040001DE RID: 478
	public float updateRate = 0.5f;

	// Token: 0x040001DF RID: 479
	private float updateTimer;

	// Token: 0x040001E0 RID: 480
	public List<Camera> cams;
}
