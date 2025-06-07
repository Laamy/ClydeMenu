using System;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class FlashlightSprint : MonoBehaviour
{
	// Token: 0x060006C9 RID: 1737 RVA: 0x00041870 File Offset: 0x0003FA70
	private void Update()
	{
		if (!this.PlayerAvatar.isLocal)
		{
			return;
		}
		if (PlayerController.instance.CanSlide)
		{
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(0f, 0f, this.Offset * GameplayManager.instance.cameraAnimation), this.Speed * Time.deltaTime);
			return;
		}
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(0f, 0f, 0f), this.Speed * Time.deltaTime);
	}

	// Token: 0x04000B8C RID: 2956
	public float Offset;

	// Token: 0x04000B8D RID: 2957
	public float Speed;

	// Token: 0x04000B8E RID: 2958
	public PlayerAvatar PlayerAvatar;
}
