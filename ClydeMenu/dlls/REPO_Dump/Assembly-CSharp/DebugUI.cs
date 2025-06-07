using System;
using UnityEngine;

// Token: 0x02000266 RID: 614
public class DebugUI : MonoBehaviour
{
	// Token: 0x060013A0 RID: 5024 RVA: 0x000AECB2 File Offset: 0x000ACEB2
	private void Start()
	{
		if (!Application.isEditor)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060013A1 RID: 5025 RVA: 0x000AECC6 File Offset: 0x000ACEC6
	private void Update()
	{
		if (SemiFunc.DebugDev() && Input.GetKeyDown(KeyCode.F1))
		{
			this.enableParent.SetActive(!this.enableParent.activeSelf);
		}
	}

	// Token: 0x040021C0 RID: 8640
	public GameObject enableParent;
}
