using System;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class GameplayButtonAimInvertVertical : MonoBehaviour
{
	// Token: 0x06000940 RID: 2368 RVA: 0x000585CC File Offset: 0x000567CC
	public void ButtonPressed()
	{
		GameplayManager.instance.UpdateAimInvertVertical();
	}
}
