using System;
using UnityEngine;

// Token: 0x0200002B RID: 43
public class CameraPosition : MonoBehaviour
{
	// Token: 0x060000A4 RID: 164 RVA: 0x00006584 File Offset: 0x00004784
	private void Awake()
	{
		CameraPosition.instance = this;
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x0000658C File Offset: 0x0000478C
	private void Update()
	{
		float num = this.positionSmooth;
		if (this.tumbleSetTimer > 0f)
		{
			num *= 0.5f;
			this.tumbleSetTimer -= Time.deltaTime;
		}
		Vector3 vector = this.playerTransform.localPosition + this.playerOffset;
		if (SemiFunc.MenuLevel() && CameraNoPlayerTarget.instance)
		{
			vector = CameraNoPlayerTarget.instance.transform.position;
		}
		base.transform.localPosition = Vector3.Slerp(base.transform.localPosition, vector, num * Time.deltaTime);
		base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, Quaternion.identity, num * Time.deltaTime);
		if (SemiFunc.MenuLevel())
		{
			base.transform.localPosition = vector;
		}
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x0000665E File Offset: 0x0000485E
	public void TumbleSet()
	{
		this.tumbleSetTimer = 0.5f;
	}

	// Token: 0x04000195 RID: 405
	public static CameraPosition instance;

	// Token: 0x04000196 RID: 406
	public Transform playerTransform;

	// Token: 0x04000197 RID: 407
	public Vector3 playerOffset;

	// Token: 0x04000198 RID: 408
	public CameraTarget camController;

	// Token: 0x04000199 RID: 409
	public float positionSmooth = 2f;

	// Token: 0x0400019A RID: 410
	private float tumbleSetTimer;
}
