using System;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class PropLight : MonoBehaviour
{
	// Token: 0x060008DC RID: 2268 RVA: 0x0005581C File Offset: 0x00053A1C
	private void Awake()
	{
		this.lightComponent = base.GetComponent<Light>();
		this.originalIntensity = this.lightComponent.intensity;
		this.halo = (base.GetComponent("Halo") as Behaviour);
		if (this.halo)
		{
			this.hasHalo = true;
		}
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x00055870 File Offset: 0x00053A70
	private void Start()
	{
		if (LevelGenerator.Instance.Generated)
		{
			SemiFunc.LightAdd(this);
		}
	}

	// Token: 0x04001024 RID: 4132
	public bool levelLight = true;

	// Token: 0x04001025 RID: 4133
	internal bool turnedOff;

	// Token: 0x04001026 RID: 4134
	[Range(0f, 2f)]
	public float lightRangeMultiplier = 1f;

	// Token: 0x04001027 RID: 4135
	internal Light lightComponent;

	// Token: 0x04001028 RID: 4136
	internal float originalIntensity;

	// Token: 0x04001029 RID: 4137
	internal Behaviour halo;

	// Token: 0x0400102A RID: 4138
	internal bool hasHalo;
}
