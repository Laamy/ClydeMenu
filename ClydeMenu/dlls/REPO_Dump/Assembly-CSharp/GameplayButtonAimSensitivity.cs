using System;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class GameplayButtonAimSensitivity : MonoBehaviour
{
	// Token: 0x06000942 RID: 2370 RVA: 0x000585E0 File Offset: 0x000567E0
	public void ButtonPressed()
	{
		GameplayManager.instance.UpdateAimSensitivity();
	}
}
