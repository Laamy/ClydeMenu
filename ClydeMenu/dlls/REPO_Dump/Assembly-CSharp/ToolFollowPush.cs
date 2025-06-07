using System;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class ToolFollowPush : MonoBehaviour
{
	// Token: 0x0600074D RID: 1869 RVA: 0x00045A06 File Offset: 0x00043C06
	public void Push(Vector3 position, Quaternion rotation, float amount)
	{
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, position, amount);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, rotation, amount);
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x00045A44 File Offset: 0x00043C44
	private void Update()
	{
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, Vector3.zero, this.SettleSpeed * Time.deltaTime);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, Quaternion.identity, this.SettleSpeed * Time.deltaTime);
	}

	// Token: 0x04000CCB RID: 3275
	private Vector3 PushPosition;

	// Token: 0x04000CCC RID: 3276
	private Quaternion PushRotation;

	// Token: 0x04000CCD RID: 3277
	public float SettleSpeed;
}
