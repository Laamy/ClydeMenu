using System;
using UnityEngine;

// Token: 0x020001B3 RID: 435
public class PlayerArmCollision : MonoBehaviour
{
	// Token: 0x06000EE1 RID: 3809 RVA: 0x00086241 File Offset: 0x00084441
	private void OnCollisionStay(Collision other)
	{
		this.Blocked = true;
		this.BlockedTimer = 0.25f;
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x00086255 File Offset: 0x00084455
	private void Update()
	{
		if (this.BlockedTimer <= 0f)
		{
			this.Blocked = false;
			return;
		}
		this.BlockedTimer -= Time.deltaTime;
	}

	// Token: 0x04001879 RID: 6265
	public bool Blocked;

	// Token: 0x0400187A RID: 6266
	public float BlockDistance;

	// Token: 0x0400187B RID: 6267
	private float BlockedTimer;
}
