using System;
using UnityEngine;

// Token: 0x02000261 RID: 609
public class BatteryUI : SemiUI
{
	// Token: 0x06001391 RID: 5009 RVA: 0x000AE8F3 File Offset: 0x000ACAF3
	protected override void Start()
	{
		base.Start();
		BatteryUI.instance = this;
		this.batteryCurrentBars = 6;
		this.originalLocalScale = base.transform.localScale;
		base.transform.localScale = Vector3.zero;
	}

	// Token: 0x06001392 RID: 5010 RVA: 0x000AE92C File Offset: 0x000ACB2C
	private void BatteryLogic()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (!SemiFunc.FPSImpulse15())
		{
			return;
		}
		if (!this.batteryVisualLogic)
		{
			return;
		}
		if (this.batteryCurrentBars > this.batteryCurrenyBarsMax)
		{
			this.batteryCurrentBars = this.batteryCurrenyBarsMax;
		}
		if (this.batteryCurrentBars < 0)
		{
			this.batteryCurrentBars = 0;
		}
		if (this.batteryCurrentBarsPrev != this.batteryCurrentBars)
		{
			this.batteryVisualLogic.BatteryBarsUpdate(this.batteryCurrentBars, false);
			this.batteryCurrentBarsPrev = this.batteryCurrentBars;
		}
		if (this.batteryCurrentBars <= this.batteryCurrenyBarsMax / 2 && SemiFunc.RunIsLobby())
		{
			float num = 1.8f;
			this.redBlinkTimer += Time.deltaTime * num;
			if (this.redBlinkTimer > 0.5f)
			{
				this.batteryVisualLogic.HideCurrentBar(true, Color.red);
			}
			else
			{
				this.batteryVisualLogic.BatteryColorMainReset();
			}
			if (this.redBlinkTimer > 1f)
			{
				this.redBlinkTimer = 0f;
				this.batteryVisualLogic.HideCurrentBar(false, Color.red);
			}
		}
	}

	// Token: 0x06001393 RID: 5011 RVA: 0x000AEA38 File Offset: 0x000ACC38
	protected override void Update()
	{
		if (this.batteryVisualLogic && SemiFunc.RunIsLobby() && this.batteryVisualLogic.currentBars <= this.batteryVisualLogic.batteryBars / 2)
		{
			this.batteryVisualLogic.OverrideChargeNeeded(0.2f);
		}
		if (Inventory.instance.InventorySpotsOccupied() > 0)
		{
			base.SemiUIScoot(new Vector2(0f, 5f));
		}
		else
		{
			base.SemiUIScoot(new Vector2(0f, -10f));
		}
		base.Update();
		if (SemiFunc.RunIsShop())
		{
			base.Hide();
			return;
		}
		if (this.batteryShowTimer > 0f)
		{
			if (!PhysGrabber.instance.grabbed)
			{
				this.batteryShowTimer = 0f;
			}
			this.batteryShowTimer -= Time.deltaTime;
			return;
		}
		base.Hide();
	}

	// Token: 0x06001394 RID: 5012 RVA: 0x000AEB10 File Offset: 0x000ACD10
	public void BatteryFetch()
	{
		if (!this.batteryVisualLogic)
		{
			return;
		}
		if (!PhysGrabber.instance.grabbed)
		{
			return;
		}
		if (!SemiFunc.FPSImpulse5())
		{
			return;
		}
		PhysGrabObject grabbedPhysGrabObject = PhysGrabber.instance.grabbedPhysGrabObject;
		if (!grabbedPhysGrabObject)
		{
			return;
		}
		ItemBattery component = grabbedPhysGrabObject.GetComponent<ItemBattery>();
		if (!component)
		{
			return;
		}
		if (component.onlyShowWhenItemToggleIsOn && !grabbedPhysGrabObject.GetComponent<ItemToggle>().toggleState)
		{
			return;
		}
		int batteryLifeInt = component.batteryLifeInt;
		if (batteryLifeInt != -1)
		{
			this.batteryCurrentBars = batteryLifeInt;
			this.batteryCurrenyBarsMax = component.batteryBars;
		}
		this.batteryShowTimer = 1f;
	}

	// Token: 0x040021B4 RID: 8628
	public static BatteryUI instance;

	// Token: 0x040021B5 RID: 8629
	private int batteryCurrentBars;

	// Token: 0x040021B6 RID: 8630
	private int batteryCurrenyBarsMax;

	// Token: 0x040021B7 RID: 8631
	private int batteryCurrentBarsPrev;

	// Token: 0x040021B8 RID: 8632
	private float redBlinkTimer;

	// Token: 0x040021B9 RID: 8633
	private float batteryShowTimer;

	// Token: 0x040021BA RID: 8634
	private Vector3 originalLocalScale;

	// Token: 0x040021BB RID: 8635
	public BatteryVisualLogic batteryVisualLogic;
}
