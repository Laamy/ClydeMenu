using System;
using UnityEngine;

// Token: 0x020000CC RID: 204
public class ToolFollow : MonoBehaviour
{
	// Token: 0x06000748 RID: 1864 RVA: 0x000458B5 File Offset: 0x00043AB5
	public void Activate()
	{
		this.Active = true;
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x000458BE File Offset: 0x00043ABE
	public void Deactivate()
	{
		this.Active = false;
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x000458C7 File Offset: 0x00043AC7
	private void Start()
	{
		this.StartPosition = base.transform.localPosition;
		this.StartRotation = base.transform.localEulerAngles;
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x000458EC File Offset: 0x00043AEC
	private void Update()
	{
		if (this.Active)
		{
			base.transform.localPosition = new Vector3(this.StartPosition.x, this.StartPosition.y + this.CameraBob.transform.localPosition.y * 0.1f, this.StartPosition.z);
			base.transform.localRotation = Quaternion.Euler(this.StartRotation.x + this.CameraBob.transform.localPosition.y * 25f, this.StartRotation.y + this.CameraBob.transform.localEulerAngles.y * 2f, this.StartRotation.z + this.CameraBob.transform.localEulerAngles.z * 15f);
			return;
		}
		base.transform.localPosition = this.StartPosition;
		base.transform.localRotation = Quaternion.Euler(this.StartRotation);
	}

	// Token: 0x04000CC7 RID: 3271
	public CameraBob CameraBob;

	// Token: 0x04000CC8 RID: 3272
	private Vector3 StartPosition;

	// Token: 0x04000CC9 RID: 3273
	private Vector3 StartRotation;

	// Token: 0x04000CCA RID: 3274
	private bool Active;
}
