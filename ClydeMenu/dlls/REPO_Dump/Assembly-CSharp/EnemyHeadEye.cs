using System;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class EnemyHeadEye : MonoBehaviour
{
	// Token: 0x06000302 RID: 770 RVA: 0x0001E29C File Offset: 0x0001C49C
	private void Update()
	{
		Quaternion b = Quaternion.LookRotation(this.Target.position - base.transform.position);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, this.EyeTarget.Speed * Time.deltaTime);
		base.transform.localRotation = SemiFunc.ClampRotation(base.transform.localRotation, this.EyeTarget.Limit);
	}

	// Token: 0x04000529 RID: 1321
	public Transform Target;

	// Token: 0x0400052A RID: 1322
	public EnemyHeadEyeTarget EyeTarget;

	// Token: 0x0400052B RID: 1323
	private float CurrentX;

	// Token: 0x0400052C RID: 1324
	private float CurrentY;
}
