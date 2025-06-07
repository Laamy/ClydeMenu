using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020001F8 RID: 504
public class MenuController : MonoBehaviour
{
	// Token: 0x06001113 RID: 4371 RVA: 0x0009CC79 File Offset: 0x0009AE79
	private void Awake()
	{
		MenuController.instance = this;
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x0009CC84 File Offset: 0x0009AE84
	private void Update()
	{
		SemiFunc.CursorUnlock(0.1f);
		if (this.picked)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Joystick1Button0))
		{
			this.OnSinglePlayerPicked();
			return;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Joystick1Button1))
		{
			this.OnMultiplayerPicked();
			return;
		}
		if (Input.GetKeyDown(KeyCode.Alpha3) || SteamManager.instance.joinLobby)
		{
			this.OnMultiplayerOnlinePicked();
			return;
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			this.OnTutorialPicked();
		}
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x0009CD07 File Offset: 0x0009AF07
	public void OnSinglePlayerPicked()
	{
		this.picked = true;
		RunManager.instance.levelCurrent = RunManager.instance.levelLobby;
		RunManager.instance.ResetProgress();
		GameManager.instance.SetGameMode(0);
		SceneManager.LoadScene("Main");
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x0009CD44 File Offset: 0x0009AF44
	public void OnMultiplayerPicked()
	{
		this.picked = true;
		RunManager.instance.levelCurrent = RunManager.instance.levelLobby;
		RunManager.instance.ResetProgress();
		GameManager.instance.SetGameMode(1);
		GameManager.instance.localTest = true;
		Object.Instantiate<GameObject>(this.networkConnectPrefab);
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x0009CD98 File Offset: 0x0009AF98
	public void OnMultiplayerOnlinePicked()
	{
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x0009CD9C File Offset: 0x0009AF9C
	public void OnTutorialPicked()
	{
		this.picked = true;
		RunManager.instance.levelCurrent = RunManager.instance.levelLobby;
		RunManager.instance.ResetProgress();
		GameManager.instance.SetGameMode(0);
		SceneManager.LoadScene("Main");
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Tutorial);
	}

	// Token: 0x04001CFE RID: 7422
	public static MenuController instance;

	// Token: 0x04001CFF RID: 7423
	public GameObject networkConnectPrefab;

	// Token: 0x04001D00 RID: 7424
	private bool picked;
}
