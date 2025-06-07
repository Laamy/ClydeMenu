using System;
using UnityEngine;

// Token: 0x020002BE RID: 702
public class UraniumHurtVignette : MonoBehaviour
{
	// Token: 0x06001616 RID: 5654 RVA: 0x000C1E42 File Offset: 0x000C0042
	private void Start()
	{
		this.hurtCollider = base.GetComponent<HurtCollider>();
	}

	// Token: 0x06001617 RID: 5655 RVA: 0x000C1E50 File Offset: 0x000C0050
	private void Update()
	{
	}

	// Token: 0x06001618 RID: 5656 RVA: 0x000C1E54 File Offset: 0x000C0054
	public void HurtVignette()
	{
		if (this.hurtCollider.onImpactPlayerAvatar.isLocal)
		{
			PostProcessing.Instance.VignetteOverride(new Color(0f, 0.6f, 0f), 0.5f, 0.5f, 5f, 2f, 0.33f, base.gameObject);
			CameraZoom.Instance.OverrideZoomSet(80f, 0.33f, 3f, 1f, base.gameObject, 50);
			CameraGlitch.Instance.PlayLong();
		}
	}

	// Token: 0x04002649 RID: 9801
	private HurtCollider hurtCollider;
}
