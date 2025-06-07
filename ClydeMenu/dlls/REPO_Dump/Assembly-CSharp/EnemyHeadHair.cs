using System;
using UnityEngine;

// Token: 0x02000063 RID: 99
public class EnemyHeadHair : MonoBehaviour
{
	// Token: 0x06000320 RID: 800 RVA: 0x0001EE82 File Offset: 0x0001D082
	private void Start()
	{
		this.Scale = base.transform.localScale;
	}

	// Token: 0x06000321 RID: 801 RVA: 0x0001EE98 File Offset: 0x0001D098
	private void Update()
	{
		if (this.PositionSpeed == 0f)
		{
			base.transform.position = this.Target.position;
		}
		else
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.Target.position, this.PositionSpeed * Time.deltaTime);
		}
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.Target.rotation, this.RotationSpeed * Time.deltaTime);
		base.transform.localScale = new Vector3(this.Scale.x * this.Target.lossyScale.x, this.Scale.y * this.Target.lossyScale.y, this.Scale.z * this.Target.lossyScale.z);
	}

	// Token: 0x06000322 RID: 802 RVA: 0x0001EF94 File Offset: 0x0001D194
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		MeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = this.DebugShow;
		}
	}

	// Token: 0x04000579 RID: 1401
	public Transform Target;

	// Token: 0x0400057A RID: 1402
	public bool DebugShow;

	// Token: 0x0400057B RID: 1403
	[Space]
	public float PositionSpeed;

	// Token: 0x0400057C RID: 1404
	public float RotationSpeed;

	// Token: 0x0400057D RID: 1405
	private Vector3 Scale;
}
