using System;
using UnityEngine;

// Token: 0x02000008 RID: 8
public class AudioButtonPushToTalk : MonoBehaviour
{
	// Token: 0x06000019 RID: 25 RVA: 0x0000270D File Offset: 0x0000090D
	public void ButtonPressed()
	{
		AudioManager.instance.UpdatePushToTalk();
	}
}
