using System;
using UnityEngine;

// Token: 0x0200000B RID: 11
public class AudioListenerFollow : MonoBehaviour
{
	// Token: 0x06000026 RID: 38 RVA: 0x00002A30 File Offset: 0x00000C30
	private void Awake()
	{
		AudioListenerFollow.instance = this;
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002A38 File Offset: 0x00000C38
	private void Start()
	{
		this.TargetPositionTransform = Camera.main.transform;
		this.TargetRotationTransform = Camera.main.transform;
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00002A5C File Offset: 0x00000C5C
	private void Update()
	{
		if (!this.TargetPositionTransform)
		{
			return;
		}
		if (SpectateCamera.instance && SpectateCamera.instance.CheckState(SpectateCamera.State.Death))
		{
			base.transform.position = this.TargetPositionTransform.position;
		}
		else
		{
			base.transform.position = this.TargetPositionTransform.position + this.TargetPositionTransform.forward * AssetManager.instance.mainCamera.nearClipPlane;
		}
		if (!this.TargetRotationTransform)
		{
			return;
		}
		base.transform.rotation = this.TargetRotationTransform.rotation;
	}

	// Token: 0x04000043 RID: 67
	public static AudioListenerFollow instance;

	// Token: 0x04000044 RID: 68
	public Transform TargetPositionTransform;

	// Token: 0x04000045 RID: 69
	public Transform TargetRotationTransform;
}
