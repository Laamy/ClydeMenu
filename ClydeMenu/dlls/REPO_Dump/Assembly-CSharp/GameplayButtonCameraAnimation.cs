using System;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class GameplayButtonCameraAnimation : MonoBehaviour
{
	// Token: 0x06000944 RID: 2372 RVA: 0x000585F4 File Offset: 0x000567F4
	public void ButtonPressed()
	{
		GameplayManager.instance.UpdateCameraAnimation();
	}
}
