using System;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class Fireplace : MonoBehaviour
{
	// Token: 0x060008F7 RID: 2295 RVA: 0x0005648C File Offset: 0x0005468C
	private void Awake()
	{
		if (this.isLit)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.fire, base.transform.position, base.transform.rotation);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localRotation = Quaternion.identity;
			if (this.isCornerFireplace)
			{
				this.fireOffset = new Vector3(0.824f, 0.028f, 0.824f);
				gameObject.transform.localRotation *= Quaternion.Euler(0f, 45f, 0f);
			}
			gameObject.transform.localPosition = this.fireOffset;
		}
	}

	// Token: 0x04001069 RID: 4201
	public bool isLit;

	// Token: 0x0400106A RID: 4202
	public bool isCornerFireplace;

	// Token: 0x0400106B RID: 4203
	public GameObject fire;

	// Token: 0x0400106C RID: 4204
	private Vector3 fireOffset = new Vector3(0f, 0.028f, 0.249f);
}
