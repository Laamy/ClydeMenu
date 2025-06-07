using System;
using UnityEngine;

// Token: 0x02000237 RID: 567
public class DisableInGame : MonoBehaviour
{
	// Token: 0x06001297 RID: 4759 RVA: 0x000A734D File Offset: 0x000A554D
	private void Start()
	{
		base.gameObject.SetActive(false);
	}
}
