using System;
using UnityEngine;

// Token: 0x020000AD RID: 173
public class FlashlightTilt : MonoBehaviour
{
	// Token: 0x060006CB RID: 1739 RVA: 0x00041924 File Offset: 0x0003FB24
	private void Update()
	{
		Quaternion targetRotation = Quaternion.LookRotation(base.transform.parent.forward, Vector3.up);
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.spring, targetRotation, -1f);
	}

	// Token: 0x04000B8F RID: 2959
	public SpringQuaternion spring;
}
