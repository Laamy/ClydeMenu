using System;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class PlayerCollision : MonoBehaviour
{
	// Token: 0x06000F7C RID: 3964 RVA: 0x0008C1B2 File Offset: 0x0008A3B2
	private void Awake()
	{
		PlayerCollision.instance = this;
	}

	// Token: 0x06000F7D RID: 3965 RVA: 0x0008C1BC File Offset: 0x0008A3BC
	private void Update()
	{
		if (this.Player.Crouching && CameraCrouchPosition.instance.Active && CameraCrouchPosition.instance.Lerp > 0.5f)
		{
			base.transform.localScale = this.CrouchCollision.localScale;
			return;
		}
		base.transform.localScale = this.StandCollision.localScale;
	}

	// Token: 0x06000F7E RID: 3966 RVA: 0x0008C220 File Offset: 0x0008A420
	public void SetCrouchCollision()
	{
		base.transform.localScale = this.CrouchCollision.localScale;
	}

	// Token: 0x040019BB RID: 6587
	public static PlayerCollision instance;

	// Token: 0x040019BC RID: 6588
	public PlayerController Player;

	// Token: 0x040019BD RID: 6589
	public Transform StandCollision;

	// Token: 0x040019BE RID: 6590
	public Transform CrouchCollision;
}
