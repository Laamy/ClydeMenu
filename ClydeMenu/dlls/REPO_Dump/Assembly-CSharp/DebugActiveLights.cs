using System;
using TMPro;
using UnityEngine;

// Token: 0x02000262 RID: 610
public class DebugActiveLights : MonoBehaviour
{
	// Token: 0x06001396 RID: 5014 RVA: 0x000AEBAB File Offset: 0x000ACDAB
	private void Awake()
	{
		this.text = base.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x06001397 RID: 5015 RVA: 0x000AEBB9 File Offset: 0x000ACDB9
	private void Update()
	{
		this.text.text = "Active Lights: " + LightManager.instance.activeLightsAmount.ToString();
	}

	// Token: 0x040021BC RID: 8636
	private TextMeshProUGUI text;
}
