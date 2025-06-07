using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class CameraFreeze : MonoBehaviour
{
	// Token: 0x0600008E RID: 142 RVA: 0x00005ED0 File Offset: 0x000040D0
	private void Awake()
	{
		CameraFreeze.instance = this;
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00005ED8 File Offset: 0x000040D8
	private void Update()
	{
		if (this.timer > 0f)
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				foreach (Camera camera in this.cameras)
				{
					camera.enabled = true;
				}
			}
		}
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00005F58 File Offset: 0x00004158
	public static void Freeze(float _time)
	{
		if (_time <= 0f)
		{
			foreach (Camera camera in CameraFreeze.instance.cameras)
			{
				camera.enabled = true;
			}
			CameraFreeze.instance.timer = _time;
			return;
		}
		if (CameraFreeze.instance.timer <= 0f)
		{
			foreach (Camera camera2 in CameraFreeze.instance.cameras)
			{
				camera2.enabled = false;
			}
		}
		CameraFreeze.instance.timer = _time;
	}

	// Token: 0x06000091 RID: 145 RVA: 0x00006024 File Offset: 0x00004224
	public static bool IsFrozen()
	{
		return CameraFreeze.instance.timer > 0f;
	}

	// Token: 0x0400017B RID: 379
	public static CameraFreeze instance;

	// Token: 0x0400017C RID: 380
	public List<Camera> cameras = new List<Camera>();

	// Token: 0x0400017D RID: 381
	private float timer;
}
