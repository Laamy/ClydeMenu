using System;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class CameraUtils : MonoBehaviour
{
	// Token: 0x060000B9 RID: 185 RVA: 0x0000737B File Offset: 0x0000557B
	private void Awake()
	{
		CameraUtils.Instance = this;
	}

	// Token: 0x060000BA RID: 186 RVA: 0x00007383 File Offset: 0x00005583
	private void Start()
	{
		this.MainCamera = Camera.main;
	}

	// Token: 0x040001E1 RID: 481
	public static CameraUtils Instance;

	// Token: 0x040001E2 RID: 482
	public Camera MainCamera;
}
