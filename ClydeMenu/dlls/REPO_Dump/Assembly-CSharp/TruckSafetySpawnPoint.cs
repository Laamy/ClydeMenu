using System;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class TruckSafetySpawnPoint : MonoBehaviour
{
	// Token: 0x060008C7 RID: 2247 RVA: 0x00054ED7 File Offset: 0x000530D7
	private void Awake()
	{
		TruckSafetySpawnPoint.instance = this;
	}

	// Token: 0x04001009 RID: 4105
	public static TruckSafetySpawnPoint instance;
}
