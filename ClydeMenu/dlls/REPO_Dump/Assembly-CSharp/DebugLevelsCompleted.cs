using System;
using TMPro;
using UnityEngine;

// Token: 0x02000264 RID: 612
public class DebugLevelsCompleted : MonoBehaviour
{
	// Token: 0x0600139C RID: 5020 RVA: 0x000AEC2A File Offset: 0x000ACE2A
	private void Update()
	{
		this.Text.text = "Levels Completed: " + RunManager.instance.levelsCompleted.ToString();
	}

	// Token: 0x040021BE RID: 8638
	public TextMeshProUGUI Text;
}
