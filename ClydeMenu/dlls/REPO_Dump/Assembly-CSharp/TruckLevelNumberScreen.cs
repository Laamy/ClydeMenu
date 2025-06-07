using System;
using UnityEngine;

// Token: 0x020000F1 RID: 241
public class TruckLevelNumberScreen : MonoBehaviour
{
	// Token: 0x06000866 RID: 2150 RVA: 0x0005153F File Offset: 0x0004F73F
	private void Start()
	{
		this.arenaPedistalScreen = base.GetComponent<ArenaPedistalScreen>();
		this.arenaPedistalScreen.SwitchNumber(RunManager.instance.levelsCompleted + 1, false);
	}

	// Token: 0x04000F80 RID: 3968
	private ArenaPedistalScreen arenaPedistalScreen;
}
