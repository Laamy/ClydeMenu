using System;
using UnityEngine;

// Token: 0x02000149 RID: 329
public class ItemDeactivatedUntilLevel : MonoBehaviour
{
	// Token: 0x06000B42 RID: 2882 RVA: 0x0006455D File Offset: 0x0006275D
	private void Start()
	{
		if (SemiFunc.RunGetLevelsCompleted() < this.levelToActivate)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0400122D RID: 4653
	public int levelToActivate;
}
