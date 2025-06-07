using System;
using UnityEngine;

// Token: 0x020000A9 RID: 169
public class FlashlightBob : MonoBehaviour
{
	// Token: 0x060006B8 RID: 1720 RVA: 0x00040C98 File Offset: 0x0003EE98
	private void Update()
	{
		if (!this.PlayerAvatar.isLocal)
		{
			return;
		}
		Vector3 positionResult = CameraBob.Instance.positionResult;
		base.transform.localPosition = new Vector3(-positionResult.y * 0.2f, 0f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, positionResult.y * 30f, 0f);
	}

	// Token: 0x04000B55 RID: 2901
	public PlayerAvatar PlayerAvatar;
}
