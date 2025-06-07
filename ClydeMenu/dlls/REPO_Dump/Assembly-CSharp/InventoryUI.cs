using System;

// Token: 0x0200027D RID: 637
public class InventoryUI : SemiUI
{
	// Token: 0x0600140F RID: 5135 RVA: 0x000B102F File Offset: 0x000AF22F
	private void Awake()
	{
		InventoryUI.instance = this;
	}

	// Token: 0x06001410 RID: 5136 RVA: 0x000B1037 File Offset: 0x000AF237
	protected override void Start()
	{
		base.Start();
		this.uiText = null;
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x000B1046 File Offset: 0x000AF246
	protected override void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		base.Update();
		if (SemiFunc.RunIsShop())
		{
			base.Hide();
		}
	}

	// Token: 0x04002248 RID: 8776
	public static InventoryUI instance;
}
