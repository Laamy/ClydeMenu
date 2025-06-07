using System;
using UnityEngine;

// Token: 0x020002A6 RID: 678
public class SquirtCode : MonoBehaviour
{
	// Token: 0x06001539 RID: 5433 RVA: 0x000BB3A2 File Offset: 0x000B95A2
	private void Start()
	{
		this.SquirtPlane1OriginalScale = this.SquirtPlane1.localScale;
		this.SquirtPlane2OriginalScale = this.SquirtPlane2.localScale;
		this.mainOriginalScale = base.transform.localScale;
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x000BB3D8 File Offset: 0x000B95D8
	private void Update()
	{
		base.transform.Rotate(Vector3.right * Time.deltaTime * 800f);
		this.SquirtPlane1.localScale = new Vector3(this.SquirtPlane1OriginalScale.x, this.SquirtPlane1OriginalScale.y, this.SquirtPlane1OriginalScale.z + Mathf.Sin(Time.time * 50f) * 0.15f);
		this.SquirtPlane2.localScale = new Vector3(this.SquirtPlane1OriginalScale.x, this.SquirtPlane1OriginalScale.y, this.SquirtPlane1OriginalScale.z + Mathf.Sin(Time.time * 50f + 50f) * 0.15f);
		base.transform.localScale = new Vector3(this.mainOriginalScale.x + Mathf.Sin(Time.time * 50f) * 0.15f, this.mainOriginalScale.y, this.mainOriginalScale.z);
	}

	// Token: 0x04002498 RID: 9368
	public Transform SquirtPlane1;

	// Token: 0x04002499 RID: 9369
	public Transform SquirtPlane2;

	// Token: 0x0400249A RID: 9370
	private Vector3 SquirtPlane1OriginalScale;

	// Token: 0x0400249B RID: 9371
	private Vector3 SquirtPlane2OriginalScale;

	// Token: 0x0400249C RID: 9372
	private Vector3 mainOriginalScale;
}
