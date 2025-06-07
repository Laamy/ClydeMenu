using System;
using UnityEngine;

// Token: 0x0200005F RID: 95
public class EnemyHeadPupil : MonoBehaviour
{
	// Token: 0x06000315 RID: 789 RVA: 0x0001EA11 File Offset: 0x0001CC11
	private void Update()
	{
		if (this.Active)
		{
			base.transform.localScale = new Vector3(this.EyeTarget.PupilCurrentSize, base.transform.localScale.y, this.EyeTarget.PupilCurrentSize);
		}
	}

	// Token: 0x04000565 RID: 1381
	public EnemyHeadEyeTarget EyeTarget;

	// Token: 0x04000566 RID: 1382
	public bool Active = true;
}
