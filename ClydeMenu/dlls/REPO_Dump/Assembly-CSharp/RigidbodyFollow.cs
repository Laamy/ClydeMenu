using System;
using UnityEngine;

// Token: 0x02000137 RID: 311
public class RigidbodyFollow : MonoBehaviour
{
	// Token: 0x06000AB8 RID: 2744 RVA: 0x0005EBA9 File Offset: 0x0005CDA9
	private void Start()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x0005EBB8 File Offset: 0x0005CDB8
	private void FixedUpdate()
	{
		this.Rigidbody.position = this.Target.position;
		this.Rigidbody.rotation = this.Target.rotation;
		if (this.Scale)
		{
			base.transform.localScale = this.Target.localScale;
		}
	}

	// Token: 0x04001151 RID: 4433
	public Transform Target;

	// Token: 0x04001152 RID: 4434
	public bool Scale;

	// Token: 0x04001153 RID: 4435
	private Rigidbody Rigidbody;
}
