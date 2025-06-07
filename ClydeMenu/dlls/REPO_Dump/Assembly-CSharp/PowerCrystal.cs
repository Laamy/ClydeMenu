using System;
using UnityEngine;

// Token: 0x020000EE RID: 238
public class PowerCrystal : MonoBehaviour
{
	// Token: 0x06000860 RID: 2144 RVA: 0x000514EA File Offset: 0x0004F6EA
	private void Start()
	{
		ItemManager.instance.powerCrystals.Add(base.GetComponent<PhysGrabObject>());
	}
}
