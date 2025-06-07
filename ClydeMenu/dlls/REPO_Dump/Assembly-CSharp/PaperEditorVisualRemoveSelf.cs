using System;
using UnityEngine;

// Token: 0x020000B8 RID: 184
public class PaperEditorVisualRemoveSelf : MonoBehaviour
{
	// Token: 0x060006F0 RID: 1776 RVA: 0x00042179 File Offset: 0x00040379
	private void Start()
	{
		Object.Destroy(base.gameObject);
	}
}
