using System;
using UnityEngine;

// Token: 0x02000112 RID: 274
public class GameplayButtonPlayerNames : MonoBehaviour
{
	// Token: 0x06000948 RID: 2376 RVA: 0x0005861C File Offset: 0x0005681C
	public void ButtonPressed()
	{
		GameplayManager.instance.UpdatePlayerNames();
	}
}
