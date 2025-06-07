using System;
using TMPro;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class BuildName : MonoBehaviour
{
	// Token: 0x0600092B RID: 2347 RVA: 0x00057A86 File Offset: 0x00055C86
	private void Start()
	{
		base.GetComponent<TextMeshProUGUI>().text = BuildManager.instance.version.title;
	}
}
