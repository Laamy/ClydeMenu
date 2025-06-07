using System;
using UnityEngine;

// Token: 0x020001F4 RID: 500
public class MenuButtonEsc : MonoBehaviour
{
	// Token: 0x060010F5 RID: 4341 RVA: 0x0009C275 File Offset: 0x0009A475
	private void Start()
	{
		this.parentTransform = base.GetComponentInParent<MenuPage>().transform;
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x0009C288 File Offset: 0x0009A488
	private void Update()
	{
	}

	// Token: 0x04001CD4 RID: 7380
	private Transform parentTransform;
}
