using System;
using UnityEngine;

// Token: 0x020001F9 RID: 505
public class MenuHolder : MonoBehaviour
{
	// Token: 0x0600111A RID: 4378 RVA: 0x0009CDF8 File Offset: 0x0009AFF8
	private void Start()
	{
		MenuHolder.instance = this;
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x0009CE00 File Offset: 0x0009B000
	private void Update()
	{
	}

	// Token: 0x04001D01 RID: 7425
	public static MenuHolder instance;
}
