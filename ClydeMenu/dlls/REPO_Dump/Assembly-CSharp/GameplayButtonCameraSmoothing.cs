using System;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class GameplayButtonCameraSmoothing : MonoBehaviour
{
	// Token: 0x06000946 RID: 2374 RVA: 0x00058608 File Offset: 0x00056808
	public void ButtonPressed()
	{
		GameplayManager.instance.UpdateCameraSmoothing();
	}
}
