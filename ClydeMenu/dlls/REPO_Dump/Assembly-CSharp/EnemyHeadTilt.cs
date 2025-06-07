using System;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class EnemyHeadTilt : MonoBehaviour
{
	// Token: 0x06000317 RID: 791 RVA: 0x0001EA60 File Offset: 0x0001CC60
	private void Update()
	{
		float z = Mathf.Clamp(Vector3.Cross(this.ForwardPrev, base.transform.forward).y * this.Amount, -this.MaxAmount, this.MaxAmount);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, Quaternion.Euler(0f, 0f, z), this.Speed * Time.deltaTime);
		this.ForwardPrev = base.transform.forward;
	}

	// Token: 0x04000567 RID: 1383
	public float Amount = -500f;

	// Token: 0x04000568 RID: 1384
	public float MaxAmount = 20f;

	// Token: 0x04000569 RID: 1385
	public float Speed = 10f;

	// Token: 0x0400056A RID: 1386
	private Vector3 ForwardPrev;
}
