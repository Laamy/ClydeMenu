using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class DirtFinderMapScanlines : MonoBehaviour
{
	// Token: 0x06000D95 RID: 3477 RVA: 0x000768A1 File Offset: 0x00074AA1
	private void OnEnable()
	{
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x000768B0 File Offset: 0x00074AB0
	private IEnumerator Logic()
	{
		for (;;)
		{
			base.transform.localPosition += new Vector3(0f, 0f, this.Speed);
			if (base.transform.localPosition.z < this.MaxZ)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, 0f);
				base.transform.localPosition += new Vector3(0f, 0f, this.Speed);
			}
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x040015C4 RID: 5572
	public float Speed;

	// Token: 0x040015C5 RID: 5573
	public float MaxZ;
}
