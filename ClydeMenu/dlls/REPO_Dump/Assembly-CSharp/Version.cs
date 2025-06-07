using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200010A RID: 266
[CreateAssetMenu(fileName = "Version - ", menuName = "Other/Version", order = 0)]
public class Version : ScriptableObject
{
	// Token: 0x0600092D RID: 2349 RVA: 0x00057AAC File Offset: 0x00055CAC
	private void Discord()
	{
		string text = this.discordRole;
		text = text + "\n# ''R.E.P.O. " + this.title + "'' is now live! :taxman_laugh:";
		if (this.newList.Count > 0)
		{
			text += "\n\n";
			text += "## NEW";
			foreach (string text2 in this.newList)
			{
				text = text + "\n> - " + text2;
			}
		}
		if (this.changesList.Count > 0)
		{
			text += "\n\n";
			text += "## CHANGES";
			foreach (string text3 in this.changesList)
			{
				text = text + "\n> - " + text3;
			}
		}
		if (this.balancingList.Count > 0)
		{
			text += "\n\n";
			text += "## BALANCING";
			foreach (string text4 in this.balancingList)
			{
				text = text + "\n> - " + text4;
			}
		}
		if (this.fixList.Count > 0)
		{
			text += "\n\n";
			text += "## FIXES";
			foreach (string text5 in this.fixList)
			{
				text = text + "\n> - " + text5;
			}
		}
		text += "\n\n";
		text += "# __Thanks for helping us test! :smile~1:__";
		GUIUtility.systemCopyBuffer = text;
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x00057CB8 File Offset: 0x00055EB8
	private void Steam()
	{
		string text = "[hr][/hr]";
		text += "[b][list]";
		if (this.newList.Count > 0)
		{
			text += "\n[*][url=#NEW]NEW[/url]";
		}
		if (this.changesList.Count > 0)
		{
			text += "\n[*][url=#CHANGES]CHANGES[/url]";
		}
		if (this.balancingList.Count > 0)
		{
			text += "\n[*][url=#BALANCING]BALANCING[/url]";
		}
		if (this.fixList.Count > 0)
		{
			text += "\n[*][url=#FIXES]FIXES[/url]";
		}
		text += "\n[/list][/b]";
		if (this.newList.Count > 0)
		{
			text += "\n[hr][/hr][h2=NEW]NEW[/h2][list]";
			foreach (string text2 in this.newList)
			{
				text = text + "\n[*]" + text2;
			}
			text += "[/list]";
		}
		if (this.changesList.Count > 0)
		{
			text += "\n[hr][/hr][h2=CHANGES]CHANGES[/h2][list]";
			foreach (string text3 in this.changesList)
			{
				text = text + "\n[*]" + text3;
			}
			text += "[/list]";
		}
		if (this.balancingList.Count > 0)
		{
			text += "\n[hr][/hr][h2=BALANCING]BALANCING[/h2][list]";
			foreach (string text4 in this.balancingList)
			{
				text = text + "\n[*]" + text4;
			}
			text += "[/list]";
		}
		if (this.fixList.Count > 0)
		{
			text += "\n[hr][/hr][h2=FIXES]FIXES[/h2][list]";
			foreach (string text5 in this.fixList)
			{
				text = text + "\n[*]" + text5;
			}
			text += "[/list]";
		}
		text += "\n[hr][/hr]";
		GUIUtility.systemCopyBuffer = text;
	}

	// Token: 0x040010B7 RID: 4279
	public string title = "v0.0.0";

	// Token: 0x040010B8 RID: 4280
	public string discordRole = "@";

	// Token: 0x040010B9 RID: 4281
	[TextArea(0, 10)]
	public List<string> newList = new List<string>();

	// Token: 0x040010BA RID: 4282
	[TextArea(0, 10)]
	public List<string> changesList = new List<string>();

	// Token: 0x040010BB RID: 4283
	[TextArea(0, 10)]
	public List<string> balancingList = new List<string>();

	// Token: 0x040010BC RID: 4284
	[TextArea(0, 10)]
	public List<string> fixList = new List<string>();
}
