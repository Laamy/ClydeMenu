using System;
using UnityEngine;

// Token: 0x02000251 RID: 593
public class CollisionFree : MonoBehaviour
{
	// Token: 0x0600132E RID: 4910 RVA: 0x000AB494 File Offset: 0x000A9694
	private void OnTriggerExit(Collider other)
	{
		this.colliding = false;
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x000AB49D File Offset: 0x000A969D
	private void OnTriggerStay(Collider other)
	{
		this.colliding = true;
	}

	// Token: 0x040020AC RID: 8364
	[HideInInspector]
	public bool colliding;
}
