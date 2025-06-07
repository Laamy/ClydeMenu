using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001EC RID: 492
public class GameManager : MonoBehaviour
{
	// Token: 0x17000004 RID: 4
	// (get) Token: 0x060010BE RID: 4286 RVA: 0x0009A7EE File Offset: 0x000989EE
	// (set) Token: 0x060010BF RID: 4287 RVA: 0x0009A7F6 File Offset: 0x000989F6
	public int gameMode { get; private set; }

	// Token: 0x060010C0 RID: 4288 RVA: 0x0009A7FF File Offset: 0x000989FF
	private void Awake()
	{
		if (!GameManager.instance)
		{
			GameManager.instance = this;
			this.gameMode = 0;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x0009A831 File Offset: 0x00098A31
	public void SetGameMode(int mode)
	{
		this.gameMode = mode;
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x0009A83A File Offset: 0x00098A3A
	public void SetConnectRandom(bool _connectRandom)
	{
		this.connectRandom = _connectRandom;
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x0009A843 File Offset: 0x00098A43
	public static bool Multiplayer()
	{
		return GameManager.instance.gameMode == 1;
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x0009A852 File Offset: 0x00098A52
	public void PlayerMicrophoneSettingSet(string _name, float _value)
	{
		this.playerMicrophoneSettings[_name] = _value;
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x0009A861 File Offset: 0x00098A61
	public float PlayerMicrophoneSettingGet(string _name)
	{
		if (this.playerMicrophoneSettings.ContainsKey(_name))
		{
			return this.playerMicrophoneSettings[_name];
		}
		return 0.5f;
	}

	// Token: 0x04001C8C RID: 7308
	public static GameManager instance;

	// Token: 0x04001C8E RID: 7310
	public bool localTest;

	// Token: 0x04001C8F RID: 7311
	internal bool connectRandom;

	// Token: 0x04001C90 RID: 7312
	internal DebugComputerCheck.ServerMode serverMode;

	// Token: 0x04001C91 RID: 7313
	internal Dictionary<string, float> playerMicrophoneSettings = new Dictionary<string, float>();
}
