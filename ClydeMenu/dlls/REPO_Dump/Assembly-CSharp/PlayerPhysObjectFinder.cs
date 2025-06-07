using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class PlayerPhysObjectFinder : MonoBehaviour
{
	// Token: 0x06000FF9 RID: 4089 RVA: 0x000932FB File Offset: 0x000914FB
	private void Awake()
	{
		this.collider = base.GetComponent<CapsuleCollider>();
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x00093309 File Offset: 0x00091509
	private void OnEnable()
	{
		this.CoroutineActivate();
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x00093311 File Offset: 0x00091511
	private void OnDisable()
	{
		this.coroutineActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x00093320 File Offset: 0x00091520
	private void CoroutineActivate()
	{
		if (!this.coroutineActive)
		{
			this.coroutineActive = true;
			base.StartCoroutine(this.Logic());
		}
	}

	// Token: 0x06000FFD RID: 4093 RVA: 0x0009333E File Offset: 0x0009153E
	private IEnumerator Logic()
	{
		for (;;)
		{
			this.physGrabObjects.Clear();
			Vector3 point = base.transform.position + Vector3.up * this.collider.radius;
			Vector3 point2 = base.transform.position + Vector3.up * this.collider.height - Vector3.up * this.collider.radius;
			Collider[] array = Physics.OverlapCapsule(point, point2, this.collider.radius, SemiFunc.LayerMaskGetPhysGrabObject());
			for (int i = 0; i < array.Length; i++)
			{
				PhysGrabObject componentInParent = array[i].GetComponentInParent<PhysGrabObject>();
				if (componentInParent)
				{
					this.physGrabObjects.Add(componentInParent);
				}
			}
			yield return new WaitForSeconds(0.05f);
		}
		yield break;
	}

	// Token: 0x04001B30 RID: 6960
	private bool coroutineActive;

	// Token: 0x04001B31 RID: 6961
	private CapsuleCollider collider;

	// Token: 0x04001B32 RID: 6962
	internal List<PhysGrabObject> physGrabObjects = new List<PhysGrabObject>();
}
