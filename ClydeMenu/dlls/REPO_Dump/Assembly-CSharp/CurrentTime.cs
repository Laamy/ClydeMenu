using System;
using TMPro;
using UnityEngine;

// Token: 0x02000274 RID: 628
public class CurrentTime : MonoBehaviour
{
	// Token: 0x060013D9 RID: 5081 RVA: 0x000AFE9B File Offset: 0x000AE09B
	private void Start()
	{
		this.hour = Random.Range(0, 23);
		this.minute = Random.Range(0, 59);
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x000AFEBC File Offset: 0x000AE0BC
	private void Update()
	{
		string text = this.hour.ToString();
		if ((float)this.hour < 10f)
		{
			text = "0" + text;
		}
		string text2 = this.minute.ToString();
		if ((float)this.minute < 10f)
		{
			text2 = "0" + text2;
		}
		this.textMesh.text = text + ":" + text2;
	}

	// Token: 0x04002216 RID: 8726
	public TextMeshProUGUI textMesh;

	// Token: 0x04002217 RID: 8727
	[HideInInspector]
	public int hour;

	// Token: 0x04002218 RID: 8728
	[HideInInspector]
	public int minute;
}
