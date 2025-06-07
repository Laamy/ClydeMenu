using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A8 RID: 424
public class PhysGrabInCart : MonoBehaviour
{
	// Token: 0x06000E9C RID: 3740 RVA: 0x000847B0 File Offset: 0x000829B0
	private void Update()
	{
		for (int i = 0; i < this.inCartObjects.Count; i++)
		{
			PhysGrabInCart.CartObject cartObject = this.inCartObjects[i];
			cartObject.timer -= Time.deltaTime;
			if (cartObject.timer <= 0f || !cartObject.physGrabObject)
			{
				this.inCartObjects.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x06000E9D RID: 3741 RVA: 0x0008481C File Offset: 0x00082A1C
	public void Add(PhysGrabObject _physGrabObject)
	{
		foreach (PhysGrabInCart.CartObject cartObject in this.inCartObjects)
		{
			if (cartObject.physGrabObject == _physGrabObject)
			{
				cartObject.timer = 1f;
				return;
			}
		}
		PhysGrabInCart.CartObject cartObject2 = new PhysGrabInCart.CartObject();
		cartObject2.physGrabObject = _physGrabObject;
		cartObject2.timer = 1f;
		this.inCartObjects.Add(cartObject2);
	}

	// Token: 0x04001835 RID: 6197
	public PhysGrabCart cart;

	// Token: 0x04001836 RID: 6198
	internal List<PhysGrabInCart.CartObject> inCartObjects = new List<PhysGrabInCart.CartObject>();

	// Token: 0x020003B0 RID: 944
	public class CartObject
	{
		// Token: 0x04002C09 RID: 11273
		public PhysGrabObject physGrabObject;

		// Token: 0x04002C0A RID: 11274
		public float timer;
	}
}
