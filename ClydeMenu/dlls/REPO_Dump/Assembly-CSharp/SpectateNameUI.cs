using System;
using TMPro;

// Token: 0x02000288 RID: 648
public class SpectateNameUI : SemiUI
{
	// Token: 0x0600144A RID: 5194 RVA: 0x000B3579 File Offset: 0x000B1779
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		SpectateNameUI.instance = this;
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x000B3593 File Offset: 0x000B1793
	protected override void Update()
	{
		base.Update();
		base.Hide();
	}

	// Token: 0x0600144C RID: 5196 RVA: 0x000B35A1 File Offset: 0x000B17A1
	public void SetName(string name)
	{
		this.spectateName = name;
		this.Text.text = this.spectateName;
	}

	// Token: 0x040022D0 RID: 8912
	internal TextMeshProUGUI Text;

	// Token: 0x040022D1 RID: 8913
	public static SpectateNameUI instance;

	// Token: 0x040022D2 RID: 8914
	private string spectateName;
}
