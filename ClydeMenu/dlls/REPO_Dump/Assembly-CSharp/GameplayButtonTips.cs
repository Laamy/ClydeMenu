using System;
using UnityEngine;

// Token: 0x02000113 RID: 275
public class GameplayButtonTips : MonoBehaviour
{
	// Token: 0x0600094A RID: 2378 RVA: 0x00058630 File Offset: 0x00056830
	public void ButtonPressed()
	{
		GameplayManager.instance.UpdateTips();
	}
}
