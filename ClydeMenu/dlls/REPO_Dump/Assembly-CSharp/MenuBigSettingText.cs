using System;
using TMPro;
using UnityEngine;

// Token: 0x020001F3 RID: 499
public class MenuBigSettingText : MonoBehaviour
{
	// Token: 0x060010F3 RID: 4339 RVA: 0x0009C25F File Offset: 0x0009A45F
	private void Start()
	{
		this.textMeshPro = base.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x04001CD3 RID: 7379
	internal TextMeshProUGUI textMeshPro;
}
