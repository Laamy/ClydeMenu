using System;
using UnityEngine;

// Token: 0x02000136 RID: 310
public class PunVoiceClientLogic : MonoBehaviour
{
	// Token: 0x06000AB6 RID: 2742 RVA: 0x0005EB54 File Offset: 0x0005CD54
	private void Awake()
	{
		Debug.Log("PunVoiceClientLogic Awake");
		if (!PunVoiceClientLogic.instance)
		{
			PunVoiceClientLogic.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		if (PunVoiceClientLogic.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04001150 RID: 4432
	public static PunVoiceClientLogic instance;
}
