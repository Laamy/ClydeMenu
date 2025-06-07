using System;
using TMPro;

// Token: 0x02000272 RID: 626
public class ChatUI : SemiUI
{
	// Token: 0x060013D2 RID: 5074 RVA: 0x000AFD50 File Offset: 0x000ADF50
	protected override void Start()
	{
		base.Start();
		ChatUI.instance = this;
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x000AFD5E File Offset: 0x000ADF5E
	protected override void Update()
	{
		base.Update();
	}

	// Token: 0x04002210 RID: 8720
	public static ChatUI instance;

	// Token: 0x04002211 RID: 8721
	public TextMeshProUGUI chatText;
}
