using System;
using UnityEngine;

// Token: 0x020001C5 RID: 453
public class PlayerCollisionStand : MonoBehaviour
{
	// Token: 0x06000F8D RID: 3981 RVA: 0x0008C621 File Offset: 0x0008A821
	private void Awake()
	{
		PlayerCollisionStand.instance = this;
		this.Collider = base.GetComponent<CapsuleCollider>();
	}

	// Token: 0x06000F8E RID: 3982 RVA: 0x0008C638 File Offset: 0x0008A838
	public bool CheckBlocked()
	{
		if (this.setBlockedTimer > 0f)
		{
			return true;
		}
		Vector3 point = base.transform.position + this.Offset + Vector3.up * this.Collider.radius;
		Vector3 point2 = base.transform.position + this.Offset + Vector3.up * this.Collider.height - Vector3.up * this.Collider.radius;
		return Physics.OverlapCapsule(point, point2, this.Collider.radius, this.LayerMask).Length != 0;
	}

	// Token: 0x06000F8F RID: 3983 RVA: 0x0008C6F1 File Offset: 0x0008A8F1
	private void Update()
	{
		if (this.setBlockedTimer > 0f)
		{
			this.setBlockedTimer -= Time.deltaTime;
		}
		base.transform.position = this.TargetTransform.position;
	}

	// Token: 0x06000F90 RID: 3984 RVA: 0x0008C728 File Offset: 0x0008A928
	public void SetBlocked()
	{
		this.setBlockedTimer = 0.25f;
		PlayerCollision.instance.SetCrouchCollision();
	}

	// Token: 0x040019D5 RID: 6613
	public static PlayerCollisionStand instance;

	// Token: 0x040019D6 RID: 6614
	public PlayerCollisionController CollisionController;

	// Token: 0x040019D7 RID: 6615
	private CapsuleCollider Collider;

	// Token: 0x040019D8 RID: 6616
	public LayerMask LayerMask;

	// Token: 0x040019D9 RID: 6617
	public Transform TargetTransform;

	// Token: 0x040019DA RID: 6618
	public Vector3 Offset;

	// Token: 0x040019DB RID: 6619
	private bool checkActive;

	// Token: 0x040019DC RID: 6620
	private float setBlockedTimer;
}
