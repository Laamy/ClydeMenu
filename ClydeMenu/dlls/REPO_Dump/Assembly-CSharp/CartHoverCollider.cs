using System;
using UnityEngine;

// Token: 0x0200019D RID: 413
public class CartHoverCollider : MonoBehaviour
{
	// Token: 0x06000DC4 RID: 3524 RVA: 0x0007841D File Offset: 0x0007661D
	private void OnTriggerStay(Collider other)
	{
		this.cartHover = true;
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x00078426 File Offset: 0x00076626
	private void OnTriggerExit(Collider other)
	{
		this.cartHover = false;
	}

	// Token: 0x04001638 RID: 5688
	[HideInInspector]
	public bool cartHover;
}
