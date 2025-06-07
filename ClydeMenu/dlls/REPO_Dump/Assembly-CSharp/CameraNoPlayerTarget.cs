using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class CameraNoPlayerTarget : MonoBehaviour
{
	// Token: 0x060000A1 RID: 161 RVA: 0x0000652D File Offset: 0x0000472D
	protected virtual void Awake()
	{
		CameraNoPlayerTarget.instance = this;
		this.cam = base.GetComponent<Camera>();
		this.cam.enabled = false;
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x0000654D File Offset: 0x0000474D
	protected virtual void Update()
	{
		SemiFunc.UIHideAim();
		SemiFunc.UIHideCurrency();
		SemiFunc.UIHideEnergy();
		SemiFunc.UIHideGoal();
		SemiFunc.UIHideHaul();
		SemiFunc.UIHideHealth();
		SemiFunc.UIHideOvercharge();
		SemiFunc.UIHideInventory();
		SemiFunc.UIHideShopCost();
	}

	// Token: 0x04000193 RID: 403
	public static CameraNoPlayerTarget instance;

	// Token: 0x04000194 RID: 404
	internal Camera cam;
}
