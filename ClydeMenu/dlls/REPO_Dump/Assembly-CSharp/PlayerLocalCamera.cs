using System;
using UnityEngine;

// Token: 0x020001CF RID: 463
public class PlayerLocalCamera : MonoBehaviour
{
	// Token: 0x06000FF5 RID: 4085 RVA: 0x00093100 File Offset: 0x00091300
	private void OnDrawGizmos()
	{
		if (this.debug)
		{
			Gizmos.color = new Color(1f, 0f, 0.79f, 0.5f);
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawSphere(Vector3.zero, 0.1f);
			Gizmos.DrawCube(new Vector3(0f, 0f, 0.15f), new Vector3(0.1f, 0.1f, 0.3f));
		}
	}

	// Token: 0x04001B2E RID: 6958
	public bool debug;
}
