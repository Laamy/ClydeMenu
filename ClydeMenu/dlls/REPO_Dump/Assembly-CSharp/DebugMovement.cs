using System;
using UnityEngine;

// Token: 0x020001E0 RID: 480
public class DebugMovement : MonoBehaviour
{
	// Token: 0x0600106D RID: 4205 RVA: 0x00097E9A File Offset: 0x0009609A
	private void Start()
	{
		this.startPos = base.transform.position;
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x00097EB0 File Offset: 0x000960B0
	private void Update()
	{
		base.transform.position = this.startPos + new Vector3(Mathf.Sin(Time.time * this.speed), Mathf.Cos(Time.time * 0.5f * this.speed), Mathf.Cos(Time.time * 0.25f * this.speed));
	}

	// Token: 0x04001C34 RID: 7220
	public float speed = 1f;

	// Token: 0x04001C35 RID: 7221
	public float leftRight = 1f;

	// Token: 0x04001C36 RID: 7222
	public float upDown = 1f;

	// Token: 0x04001C37 RID: 7223
	private Vector3 startPos;
}
