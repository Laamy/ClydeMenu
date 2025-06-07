using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000289 RID: 649
public class StatsUI : SemiUI
{
	// Token: 0x0600144E RID: 5198 RVA: 0x000B35C4 File Offset: 0x000B17C4
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		StatsUI.instance = this;
		this.textNumbers = base.transform.Find("StatsNumbers").GetComponent<TextMeshProUGUI>();
		this.Text.text = "";
		this.upgradesHeader = base.transform.Find("Upgrades Header").GetComponent<TextMeshProUGUI>();
		this.textNumbers.text = "";
		this.upgradesHeader.enabled = false;
	}

	// Token: 0x0600144F RID: 5199 RVA: 0x000B364C File Offset: 0x000B184C
	public void Fetch()
	{
		this.playerUpgrades = StatsManager.instance.FetchPlayerUpgrades(PlayerController.instance.playerSteamID);
		this.Text.text = "";
		this.textNumbers.text = "";
		this.upgradesHeader.enabled = false;
		this.scanlineObject.SetActive(false);
		foreach (KeyValuePair<string, int> keyValuePair in this.playerUpgrades)
		{
			string text = keyValuePair.Key.ToUpper();
			if (keyValuePair.Value > 0)
			{
				TextMeshProUGUI text2 = this.Text;
				text2.text = text2.text + text + "\n";
				TextMeshProUGUI textMeshProUGUI = this.textNumbers;
				textMeshProUGUI.text = textMeshProUGUI.text + "<b>" + keyValuePair.Value.ToString() + "\n</b>";
			}
		}
		if (this.Text.text != "")
		{
			this.upgradesHeader.enabled = true;
			this.scanlineObject.SetActive(true);
		}
	}

	// Token: 0x06001450 RID: 5200 RVA: 0x000B377C File Offset: 0x000B197C
	public void ShowStats()
	{
		base.SemiUISpringShakeY(20f, 10f, 0.3f);
		base.SemiUISpringScale(0.4f, 5f, 0.2f);
		this.showStatsTimer = 5f;
	}

	// Token: 0x06001451 RID: 5201 RVA: 0x000B37B4 File Offset: 0x000B19B4
	protected override void Update()
	{
		base.Update();
		base.Hide();
		if (this.showStatsTimer > 0f)
		{
			this.showStatsTimer -= Time.deltaTime;
			base.Show();
		}
		if (this.showTimer > 0f)
		{
			if (!this.fetched)
			{
				this.Fetch();
				return;
			}
		}
		else
		{
			this.fetched = false;
		}
	}

	// Token: 0x040022D3 RID: 8915
	private TextMeshProUGUI Text;

	// Token: 0x040022D4 RID: 8916
	private TextMeshProUGUI textNumbers;

	// Token: 0x040022D5 RID: 8917
	private TextMeshProUGUI upgradesHeader;

	// Token: 0x040022D6 RID: 8918
	public GameObject scanlineObject;

	// Token: 0x040022D7 RID: 8919
	public static StatsUI instance;

	// Token: 0x040022D8 RID: 8920
	private Dictionary<string, int> playerUpgrades = new Dictionary<string, int>();

	// Token: 0x040022D9 RID: 8921
	private float showStatsTimer;

	// Token: 0x040022DA RID: 8922
	private bool fetched;
}
