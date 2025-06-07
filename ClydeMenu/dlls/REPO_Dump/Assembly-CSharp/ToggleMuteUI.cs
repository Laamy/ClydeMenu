using System;

// Token: 0x0200028B RID: 651
public class ToggleMuteUI : SemiUI
{
	// Token: 0x06001458 RID: 5208 RVA: 0x000B3AC4 File Offset: 0x000B1CC4
	protected override void Update()
	{
		base.Update();
		if (!DataDirector.instance.toggleMute)
		{
			base.Hide();
			return;
		}
		base.Show();
	}
}
