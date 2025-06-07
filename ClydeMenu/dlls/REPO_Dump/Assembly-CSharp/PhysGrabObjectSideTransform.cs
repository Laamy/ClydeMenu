using System;
using UnityEngine;

// Token: 0x020001A2 RID: 418
public class PhysGrabObjectSideTransform : MonoBehaviour
{
	// Token: 0x06000E0A RID: 3594 RVA: 0x0007CCC2 File Offset: 0x0007AEC2
	private void Start()
	{
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.prevPosition = base.transform.position;
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x0007CCE4 File Offset: 0x0007AEE4
	private void FixedUpdate()
	{
		float num = Vector3.Distance(this.prevPosition, base.transform.position) / Time.fixedDeltaTime * 5f;
		if (num > this.velocity)
		{
			this.velocity = num;
			this.velocityResetTimer = 0.1f;
		}
		if (this.velocityResetTimer > 0f)
		{
			this.velocityResetTimer -= Time.fixedDeltaTime;
		}
		else
		{
			this.velocity = 0f;
		}
		this.prevPosition = base.transform.position;
	}

	// Token: 0x040016F3 RID: 5875
	[HideInInspector]
	public Vector3 prevPosition;

	// Token: 0x040016F4 RID: 5876
	[HideInInspector]
	public float velocity;

	// Token: 0x040016F5 RID: 5877
	private float velocityResetTimer;

	// Token: 0x040016F6 RID: 5878
	private float impactTimer;

	// Token: 0x040016F7 RID: 5879
	private MeshRenderer meshRenderer;
}
