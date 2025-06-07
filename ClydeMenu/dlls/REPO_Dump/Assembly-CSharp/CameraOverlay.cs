using System;
using UnityEngine;

// Token: 0x02000294 RID: 660
public class CameraOverlay : MonoBehaviour
{
	// Token: 0x060014BE RID: 5310 RVA: 0x000B71FE File Offset: 0x000B53FE
	private void Start()
	{
		this.overlayCamera = base.GetComponent<Camera>();
		CameraOverlay.instance = this;
	}

	// Token: 0x040023A9 RID: 9129
	internal Camera overlayCamera;

	// Token: 0x040023AA RID: 9130
	public static CameraOverlay instance;
}
