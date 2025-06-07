using System;
using UnityEngine;

// Token: 0x0200016F RID: 367
[CreateAssetMenu(fileName = "Battery Drain Preset", menuName = "Semi Presets/Battery Drain Preset")]
public class BatteryDrainPresets : ScriptableObject
{
	// Token: 0x06000C64 RID: 3172 RVA: 0x0006E15D File Offset: 0x0006C35D
	public float GetBatteryDrainRate()
	{
		return this.batteryDrainRate;
	}

	// Token: 0x04001438 RID: 5176
	public float batteryDrainRate = 0.1f;
}
