using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200027C RID: 636
public class InventoryBattery : MonoBehaviour
{
	// Token: 0x0600140A RID: 5130 RVA: 0x000B0C84 File Offset: 0x000AEE84
	private void Start()
	{
		this.batteryState = 6;
		this.batteryImage = base.GetComponent<RawImage>();
		this.batteryImage.enabled = false;
		this.originalLocalScale = base.transform.localScale;
		base.transform.localScale = Vector3.zero;
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x000B0CD4 File Offset: 0x000AEED4
	public void BatteryFetch()
	{
		if (!Inventory.instance)
		{
			return;
		}
		if (!this.batteryImage)
		{
			return;
		}
		int batteryStateFromInventorySpot = Inventory.instance.GetBatteryStateFromInventorySpot(this.inventorySpot);
		if (batteryStateFromInventorySpot != -1 && this.redBlinkTimer == 0f)
		{
			this.batteryState = batteryStateFromInventorySpot;
			this.batteryImage.color = new Color(1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x0600140C RID: 5132 RVA: 0x000B0D49 File Offset: 0x000AEF49
	public void BatteryShow()
	{
		this.batteryShowTimer = 0.2f;
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x000B0D58 File Offset: 0x000AEF58
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.batteryShowTimer > 0f)
		{
			this.batteryShowTimer -= Time.deltaTime;
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, this.originalLocalScale, Time.deltaTime * 30f);
			this.batteryImage.enabled = true;
		}
		else if (this.batteryImage.enabled)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.zero, Time.deltaTime * 30f);
			if (base.transform.localScale.x < 0.01f)
			{
				base.transform.localScale = Vector3.zero;
				this.batteryImage.enabled = false;
			}
		}
		if (this.batteryState == 0)
		{
			this.batteryImage.uvRect = new Rect(-0.006f, -0.921f, 0.4f, 0.2f);
		}
		if (this.batteryState == 1)
		{
			this.batteryImage.uvRect = new Rect(0.369f, -0.687f, 0.4f, 0.2f);
		}
		if (this.batteryState == 2)
		{
			this.batteryImage.uvRect = new Rect(-0.006f, -0.687f, 0.4f, 0.2f);
		}
		if (this.batteryState == 3)
		{
			this.batteryImage.uvRect = new Rect(0.369f, -0.4523f, 0.4f, 0.2f);
		}
		if (this.batteryState == 4)
		{
			this.batteryImage.uvRect = new Rect(-0.006f, -0.4523f, 0.4f, 0.2f);
		}
		if (this.batteryState == 5)
		{
			this.batteryImage.uvRect = new Rect(0.369f, -0.218f, 0.4f, 0.2f);
		}
		if (this.batteryState == 6)
		{
			this.batteryImage.uvRect = new Rect(-0.006f, -0.218f, 0.4f, 0.2f);
		}
		if (this.batteryState > 6)
		{
			this.batteryState = 6;
		}
		if (this.batteryState < 0)
		{
			this.batteryState = 0;
		}
		if (this.batteryState <= 3 && SemiFunc.RunIsLobby())
		{
			this.redBlinkTimer += Time.deltaTime;
			if (this.redBlinkTimer > 0.5f)
			{
				this.batteryImage.color = new Color(1f, 0f, 0f, 1f);
			}
			else
			{
				this.batteryImage.color = new Color(1f, 1f, 1f, 1f);
			}
			if (this.redBlinkTimer > 1f)
			{
				this.redBlinkTimer = 0f;
			}
		}
	}

	// Token: 0x04002242 RID: 8770
	public int inventorySpot;

	// Token: 0x04002243 RID: 8771
	private int batteryState;

	// Token: 0x04002244 RID: 8772
	internal RawImage batteryImage;

	// Token: 0x04002245 RID: 8773
	private float redBlinkTimer;

	// Token: 0x04002246 RID: 8774
	private float batteryShowTimer;

	// Token: 0x04002247 RID: 8775
	private Vector3 originalLocalScale;
}
